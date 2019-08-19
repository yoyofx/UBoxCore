using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UBoxCore.Server.RPC.RPCServices
{
    [RpcService]
    public class CustomRpcService
    {

        [RpcFunc(Name = "getUserInfo")]

        public string GetUserInfo(string id, string name)
        {
            return id + name;
        }

    }
}
