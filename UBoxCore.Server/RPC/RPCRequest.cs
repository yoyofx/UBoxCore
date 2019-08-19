using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UBoxCore.Server.RPC
{
    public class RPCRequest
    {
        public string Method { set; get; }

        public List<object> Parameters { set; get; }

    }
}
