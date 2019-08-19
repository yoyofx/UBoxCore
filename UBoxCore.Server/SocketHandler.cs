using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UBoxCore.Server.RPC;
using UBoxCore.Server.Utils;


namespace UBoxCore.Server
{
    public class SocketHandler
    {
        public static BlockingCollection<SocketHandler> socketsList = new BlockingCollection<SocketHandler>();


        public const int BufferSize = 4096;

        WebSocket socket;
        static readonly Encoding utf8 = Encoding.UTF8;

        SocketHandler(WebSocket socket)
        {
            this.socket = socket;
        }

        private async Task EchoLoop()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);

            while (this.socket.State == WebSocketState.Open)
            {
                var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);

                // show how to use the input and change the response
                var input = utf8.GetString(buffer, 0, incoming.Count);  // convert buffer to string

                var ret = callMethod(socket, input);

            }
        }

    


        private async ValueTask<int> callMethod(WebSocket socket, string data)
        {
            try
            {
                RPCRequest request = JsonConvert.DeserializeObject<RPCRequest>(data);

                switch (request.Method)
                {
                    case "getStatus":
                        sendMessage(new RpcResponse { Event = "OnDeviceStatus", Data = Program.Recorder.getCurrentChannel() != null ? "0" : "-1" });
                        break;
       
                    default:
                        var response = RpcService.Invoke(request);
                        sendMessage(response);
                        break;
                

                }
            }
            catch (Exception ex)
            {
                sendMessage(new RpcResponse { Event = "OnError", Data = ex.Message });
            }



            return 0;
        }

        internal static void OnLog(object arg1, string arg2)
        {
            sendMessage(new RpcResponse { Event = "OnLog", Data = arg2 });
        }

        internal static void OnClosed(object obj)
        {
            Console.WriteLine("设备断开,请检查USB连接.并重新启动此程序!!");
            sendMessage(new RpcResponse { Event = "OnClosed" });
        }

        internal static void OnHangUp(object obj)
        {
            sendMessage(new RpcResponse { Event = "OnHangUp" });
        }

        internal static void OnLineVoltage(object arg1, int arg2)
        {
            sendMessage(new RpcResponse { Event = "OnLineVoltage", Data = arg2.ToString() });
        }

        internal static void OnRingCancel(object obj)
        {
            sendMessage(new RpcResponse { Event = "OnRingCancel" });
        }

        internal static void OnRinging(object obj)
        {
            sendMessage(new RpcResponse { Event = "OnRinging" });
        }

        internal static void OnHookOff(object obj)
        {
            sendMessage(new RpcResponse { Event = "OnHookOff" });
        }

        internal static void OnError(object arg1, string arg2)
        {
            sendMessage(new RpcResponse { Event = "OnError", Data = arg2 });
            Console.WriteLine("设备错误..."+ "请退出程序,并重新插拔一下USB，重新运行软件，看是否正常；如还有异常请重启电脑。错误码: " + arg2);
            Win32.MessageBox(IntPtr.Zero , 
                "退出软件，重新插拔一下电脑端USB口或换一个USB口，重新运行软件，看是否正常；如还有异常请重启电脑。错误码:" + arg2,
                "错误", (uint)0x00000010L);
            
        }

        internal static void OnConnected(object channel, string type)
        {
            Console.WriteLine("设备正常,已连接.请务关闭此窗口,并将此窗口最少化.");
            sendMessage(new RpcResponse { Event = "OnConnected" });
        }


        internal static void OnCall(object channel,string phone)
        {
            sendMessage(new RpcResponse { Event = "OnCall" , Data = phone });
        }



        static void sendMessage(WebSocket socket, string msg)
        {
            var b = utf8.GetBytes(msg);            // create outgoing bytes
            var outgoing = new ArraySegment<byte>(b);
            socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static void sendMessage<T>(T msg)
        {
            string json = JsonConvert.SerializeObject(msg);
            foreach(var current in socketsList)
            {
                sendMessage(current.socket, json);
            }


            //if (currentSocket != null)
            //{
            //    sendMessage(currentSocket, json);
            //}
        }


       // private static WebSocket currentSocket;

        static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var socket = await hc.WebSockets.AcceptWebSocketAsync();
            //if(currentSocket!=null)
            //    currentSocket.Dispose();
            //currentSocket = socket;
            var h = new SocketHandler(socket);
            socketsList.TryAdd(h);
            await h.EchoLoop();
            socketsList.TryTake(out h);
            


        }

        /// <summary>
        /// branches the request pipeline for this SocketHandler usage
        /// </summary>
        /// <param name="app"></param>
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(SocketHandler.Acceptor);
        }
    }
}
