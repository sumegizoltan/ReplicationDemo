using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace DFSReplicationDemoClient
{
    /// <summary> 
    /// DFS Replication Demo 
    /// 
    /// Demonstrate the Distributed File System functionality
    /// </summary>
    public class Program
    {
        #region Main
        /// <summary> 
        /// The program entry point 
        /// </summary>
        static void Main(string[] args)
        {
            InitDFS();
            StartReplication();

            Console.WriteLine("");
            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }
        #endregion

        #region public methods
        /// <summary> 
        /// Initialize Replicator instance and console messages for error handling 
        /// </summary>
        public static void InitDFS() 
        {
            Replicator dfs = null;
            Options options = null;

            ErrorHandler.InitConsoleMessages();

            options = Common.LoadSettings();

            if (options != default(Options))
            {

                dfs = Replicator.GetReplicator(options);

                dfs.InstanceStarted += new ReplicatorEventHandler(OnInstanceStarted);
                dfs.Replicated += new ReplicatorEventHandler(OnReplicationIntervalStarted);
                dfs.Replicated += new ReplicatorEventHandler(Replicator.OnReplicated);
                dfs.Replicated += new ReplicatorEventHandler(OnReplicationIntervalFinished);
                dfs.InstanceStopped += new ReplicatorEventHandler(OnInstanceStopped);
            }
        }

        /// <summary> 
        /// Start replication progress 
        /// </summary>
        public static void StartReplication()
        {
            Replicator dfs = Replicator.GetReplicator();

            dfs.StartInstance();
        }

        /// <summary> 
        /// Stop replication progress
        /// </summary>
        public static void StopReplication()
        {
            Replicator dfs = Replicator.GetReplicator();

            dfs.StopInstance();
        }
        #endregion

        #region private methods
        /// <summary> 
        /// Replication instance started
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Event arguments</param>
        static void OnInstanceStarted(object sender, EventArgs e)
        {
            Console.WriteLine("The DFS instance started. - {0}", DateTime.Now);
        }

        /// <summary> 
        /// Replication instance stopped
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Event arguments</param>
        static void OnInstanceStopped(object sender, EventArgs e)
        {
            Console.WriteLine("The DFS instance stopped. - {0}", DateTime.Now);
        }

        /// <summary> 
        /// A replication interval started 
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Event arguments</param>
        static void OnReplicationIntervalStarted(object sender, EventArgs e)
        {
            Console.WriteLine("A replication interval started. - {0}", DateTime.Now);
        }

        /// <summary> 
        /// A replication interval started 
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Event arguments</param>
        static void OnReplicationIntervalFinished(object sender, EventArgs e)
        {
            Console.WriteLine("A replication interval finished.");

            Replicator dfs = (Replicator)sender;

            dfs.StopInstance();
        }
        #endregion
    }
}
