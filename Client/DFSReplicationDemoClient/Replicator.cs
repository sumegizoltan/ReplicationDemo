using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace DFSReplicationDemoClient
{
    /// <summary>
    /// Delegate for replication progresses
    /// </summary>
    public delegate void ReplicatorEventHandler(object sender, EventArgs e);

    /// <summary>
    /// The 'Singleton' class for the DFS Replication
    /// (Distributed File System Replication)
    /// </summary>
    public class Replicator
    {
        #region declaration
        protected List<Folder> folders = null;
        private static Replicator instance = null;
        protected int interval = 0;
        protected bool isIntervalEnabled = true;
        protected bool isStarted = false;

        private static object syncLock = new object();
        #endregion

        #region events
        public event ReplicatorEventHandler InstanceStarted = null;
        public event ReplicatorEventHandler InstanceStopped = null;
        public event ReplicatorEventHandler Replicated = null;
        #endregion

        #region ctor
        public Replicator() 
            :this(null)
        {
        }

        public Replicator(Folder[] folders)
        {
            if (folders != null)
            {
                this.folders = folders.ToList();
            }
            else
            {
                this.folders = new List<Folder>();
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Add new collection to the directories
        /// </summary>
        /// <param name="folders">new directories for the replication</param>
        protected void AddFolders(Folder[] folders)
        {
            if (folders != null)
            {
#if TEST
#else
                Monitor.Enter(syncLock);
#endif
                
                foreach (Folder f in folders)
                {
                    if (!this.folders.Exists(li => (li.Destination == f.Destination) &
                                                    (li.Source == f.Source)))
                    { 
                        if (!String.IsNullOrEmpty(f.Destination + f.Source))
                            this.folders.Add(f); 
                    }
                }
#if TEST
#else
                Monitor.Exit(syncLock);
#endif
            }
        }

        /// <summary>
        /// Get the Replicator instance
        /// 
        /// Support multithreaded applications through 'Double checked locking' 
        /// pattern which (once the instance exists) avoids locking each
        /// time the method is invoked
        /// </summary>
        /// <param name="folders">directories for the replication</param>
        /// <return>Singleton instance</return>
        public static Replicator GetReplicator(IOptions options = null)
        {
            if (instance == null)
            {
#if TEST
#else
                Monitor.Enter(syncLock);
#endif
                
                if (instance == null)
                {
                    if (options != null)
                    {
                        instance = new Replicator(options.Folders);
                    }
                    else
                    {
                        instance = new Replicator();
                    }
                }

#if TEST
#else
                Monitor.Exit(syncLock);
#endif
            }
            else if (options != null)
            {
                if (options.Folders != null)
                {
                    instance.AddFolders(options.Folders);
                }
            }

            if (options != null)
            {
                instance.interval = options.TimerInterval;
                instance.isIntervalEnabled = options.IsIntervalEnabled;
            }

            return instance;
        }
        
        /// <summary> 
        /// Replication
        /// </summary>
        /// <param name="sender">sender - Replicator instance</param>
        /// <param name="e">Event arguments</param>
        public static void OnReplicated(object sender, EventArgs e)
        {
            Replicator dfs = (Replicator)sender;
            string[] directories;
            string directory;
            string file;
            string[] sourceFiles;

            foreach (Folder f in dfs.Folders)
            {
                try
                {
                    if (!Directory.Exists(f.Source))
                        Directory.CreateDirectory(f.Source);

                    if (!Directory.Exists(f.Destination))
                        Directory.CreateDirectory(f.Destination);

                    sourceFiles = Directory.GetFiles(f.Source, f.FileMask, SearchOption.AllDirectories);

                    foreach (string item in sourceFiles)
                    {
                        if (!item.EndsWith("Thumbs.db"))
                        {
                            file = item.Replace(f.Source, f.Destination);
                            directories = file.Split(@"\".ToCharArray());
                            directory = String.Join(@"\", directories, 0, directories.Length - 1);

                            try
                            {
                                if (!Directory.Exists(directory))
                                    Directory.CreateDirectory(directory);

                                // TODO - copy only when fileinfo not equal
                                File.Copy(item, file, true);

                                Common.ShowInfo(item);
                            }
                            catch (Exception ex2)
                            {
                                Common.RaiseError(dfs, ex2.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.RaiseError(dfs, ex.Message);
                }
            }
        }

        /// <summary>
        /// Remove collection from the directories
        /// </summary>
        /// <param name="folders">removable directories from the replication</param>
        public static void RemoveFolders(Folder[] folders)
        {
            if (folders != null)
            {
                Replicator replicator = Replicator.GetReplicator();

#if TEST
#else
                Monitor.Enter(syncLock);
#endif
                
                foreach (Folder f in folders)
                {
                    replicator.folders.RemoveAll(li => (li.Destination == f.Destination) &
                                                        (li.Source == f.Source));
                }

#if TEST
#else
                Monitor.Exit(syncLock);
#endif
            }
        }

        /// <summary>
        /// Start replication
        /// </summary>
        public void StartInstance()
        {
#if TEST
            this.isStarted = true;
#else
            Monitor.Enter(syncLock);
            this.isStarted = true;
            Monitor.Exit(syncLock);
#endif

            if (this.InstanceStarted != null)
                this.InstanceStarted(this, EventArgs.Empty);

            if (this.isIntervalEnabled)
            {
                while (this.isStarted)
                {
                    Thread.Sleep(this.interval);

                    if (this.Replicated != null)
                        this.Replicated(this, EventArgs.Empty);
                }
            }
            else
            {
                if (this.Replicated != null)
                    this.Replicated(this, EventArgs.Empty);

                this.StopInstance();
            }
        }

        /// <summary>
        /// Stop replication
        /// </summary>
        public void StopInstance()
        {
#if TEST
            this.isStarted = false;
#else
            Monitor.Enter(syncLock);
            this.isStarted = false;
            Monitor.Exit(syncLock);
#endif

            if (this.InstanceStopped != null)
                this.InstanceStopped(this, EventArgs.Empty);
        }
        #endregion

        #region properties
        /// <summary>
        /// Property for the DFS directories
        /// </summary>
        public IEnumerable<Folder> Folders
        {
            get
            {
                return (IEnumerable<Folder>)this.folders;
            }
        }

        /// <summary>
        /// Property for the Interval
        /// Interval for checking replication directories
        /// </summary>
        public int Interval
        {
            get
            {
                return this.interval;
            }
        }
        #endregion
    }
}
