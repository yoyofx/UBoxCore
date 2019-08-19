using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UBoxCore.Server.RPC.Models;
using UBoxCore.Server.RPC.RPCServices;

namespace UBoxCore.Server.RPC
{
    public class RpcService
    {
        private static List<Type> serviceTypes = new List<Type> {
            typeof(UBoxRpcServices) , typeof(HardwareRpcService), typeof(CustomRpcService) ,
            typeof(NetworkRpcService)
        };

        private static Dictionary<string, MethodInfo> serviceMethods = new Dictionary<string, MethodInfo>();


        private static bool hasAttribute<TAttr>(Type serviceType)
        {
            var attrs = serviceType.GetCustomAttributes(typeof(TAttr), false);
            return attrs != null && attrs.Length > 0;
        }


        public static void LoadServices()
        {
            foreach(var service in serviceTypes)
            {
                if (hasAttribute<RpcServiceAttribute>(service))
                {
                    var methods = service.GetMethods();
                    foreach (var method in methods)
                    {
                        var attrs = method.GetCustomAttributes(typeof(RpcFuncAttribute), false);
                        if (attrs != null && attrs.Length > 0)
                        {
                            var rpcattr = attrs[0] as RpcFuncAttribute;
                            try
                            {
                                serviceMethods.Add(rpcattr.Name, method);
                            }
                            catch { }
                        }
                    }
                }


            }
        }


        public static RpcResponse Invoke(RPCRequest request)
        {
            string callbackName = request.Method + "Callback";
            var ret = new RpcResponse { Event = callbackName, Sucess = false, Message = "", Data = null };

            if (serviceMethods.ContainsKey(request.Method))
            {
                var method = serviceMethods[request.Method];

                object instance = Activator.CreateInstance(method.DeclaringType);
                try
                {
                    object methodRet = method.Invoke(instance, request.Parameters.ToArray());

                    ret.Data = methodRet;
                    ret.Sucess = true;
                    ret.Message = "Sucess";


                    if (typeof(IAsyncTaskCallback).IsAssignableFrom(methodRet.GetType()))
                    {
                        var asyncCallback = methodRet as IAsyncTaskCallback;
                        asyncCallback.AsyncProgressChanged += (s, e) =>
                        {
                            ret.Data = e;
                            ret.Event = asyncCallback.CallbackName + "ProgressChanged";
                            SocketHandler.sendMessage(ret);
                        };
                        asyncCallback.AsyncCompleted += (s, e) => {
                            ret.Data = "Complated";
                            ret.Event = asyncCallback.CallbackName + "Complated";
                            SocketHandler.sendMessage(ret);
                        };
                    }

                }
                catch (Exception ex)
                {
                    ret.Message = ex.Message;
                }

            }


            return ret;
        }

       
    }
}
