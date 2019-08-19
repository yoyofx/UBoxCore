using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UBoxCore.Server
{
    public class FlashPolicyfileHandler
    {
        private static bool serverFlag = false;

        public static void Cancel()
        {
            serverFlag = false;
        }


        public static Task StartAsync()
        {
            return Task.Factory.StartNew(() => {

                serverFlag = true;
                string host = "127.0.0.1";
                int port = 843;
                byte[] data;
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);
                Socket sc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sc.Bind(ipe);
                sc.Listen(5);
                while (serverFlag)
                {
                    Thread.Sleep(100);
                    data = new byte[1024];
                    Socket clientSocket = sc.Accept();

                    int count = clientSocket.Receive(data, data.Length, 0);//接收的字节                
                    string recstr = Encoding.UTF8.GetString(data, 0, count);//byte[]转换成string 
                    if (recstr.IndexOf("<policy-file-request/>") >= 0)
                    {
                        byte[] datas = System.Text.Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\" /></cross-domain-policy>\0");
                        clientSocket.Send(datas);
                        clientSocket.Close();
                        continue;
                    }
                }


            });

           
        }
    }
}
