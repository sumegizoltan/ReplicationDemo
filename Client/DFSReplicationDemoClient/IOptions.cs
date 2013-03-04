using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DFSReplicationDemoClient
{
    public interface IOptions
    {
        bool IsIntervalEnabled { get; }
        int TimerInterval  { get; }
        Folder[] Folders    { get; }
    }
}
