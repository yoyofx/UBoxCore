using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UBoxCore.Server.RPC
{
    public class RpcResponse
    {
        public string Event { set; get; }

        public object Data { set; get; }

        public string Message { set; get; }
        public bool Sucess { set; get; }

    }
}
