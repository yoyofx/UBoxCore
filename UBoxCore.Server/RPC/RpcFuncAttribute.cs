using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UBoxCore.Server.RPC
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcFuncAttribute : Attribute
    {
        public string Name { set; get; }
    }
}
