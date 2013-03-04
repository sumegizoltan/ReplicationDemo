using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DFSReplicationDemoClient;
using System.IO;

namespace DFSReplicationDemoTest
{
    [TestClass]
    public class UnitTest1
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        ///A test for DFS Instance
        ///</summary>
        [TestMethod()]
        public void DFSTest()
        {
            Replicator replicator = null;

            replicator = Replicator.GetReplicator();

            Assert.IsNotNull(replicator);
        }

        /// <summary>
        ///A test for DFS Instance is Singleton Instance
        ///</summary>
        [TestMethod()]
        public void DFSSingletonTest()
        {
            Replicator replicator = null;
            Replicator replicator2 = null;

            replicator = Replicator.GetReplicator();
            replicator2 = Replicator.GetReplicator();

            Assert.AreSame(replicator, replicator2);
        }

        /// <summary>
        ///A test for InitDFS
        ///</summary>
        [TestMethod()]
        public void InitDFSTest()
        {
            int foldersCount = 0;
            Options options = null;
            Replicator replicator = null;

            options = Common.LoadSettings();
            replicator = Replicator.GetReplicator(options);

            if (replicator.Folders != null)
                foldersCount = replicator.Folders.Count();

            Assert.AreNotEqual(0, foldersCount);
        }

        /// <summary>
        ///A test for StartReplication
        ///</summary>
        [TestMethod()]
        public void StartReplicationTest()
        {
            bool isReplicatedNewExists = false;
            string file = String.Empty;
            string fileDestination = String.Empty;
            string folder = String.Empty;
            string folderDestination = String.Empty;
            Options options = null;
            Replicator replicator = null;

            options = Common.LoadSettings();
            options.IsIntervalEnabled = false;
            replicator = Replicator.GetReplicator(options);
            replicator.Replicated += new ReplicatorEventHandler(Replicator.OnReplicated);

            if (replicator.Folders != null)
            {
                folder = replicator.Folders.First().Source;
                folderDestination = replicator.Folders.First().Destination;
                file = folder + "UnitTest1.txt";
                fileDestination = folderDestination + "UnitTest1.txt";
                File.AppendAllText(file, "--- Unit test ---");

                if (File.Exists(fileDestination))
                    File.Delete(fileDestination);

                replicator.StartInstance();

                isReplicatedNewExists = File.Exists(fileDestination);
            }

            Assert.IsTrue(isReplicatedNewExists);
        }
    }
}
