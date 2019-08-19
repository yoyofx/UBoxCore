using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UBoxCore.Server.RPC.Models
{
    public class SystemInfo
    {
        public string OSArchitecture { set; get; }

        public string OSDescription { set; get; }

        public string ProcessArchitecture { set; get; }

        public bool Is64BitOperatingSystem { set; get; }
        public int ProcessorCount { get; internal set; }
        public OperatingSystem OSVersion { get; internal set; }
        public long MemWorkingSet { get; internal set; }
        public string LogicalDrives { get; internal set; }
    }
}
