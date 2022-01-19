using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UBoxCore.Server.RPC.Models;

namespace UBoxCore.Server.RPC.RPCServices
{
    [RpcService]
    public class NetworkRpcService
    {

        [RpcFunc(Name = "pingNetwork")]
        public long PingNetwork()
        {
            //构造Ping实例
            Ping pingSender = new Ping();
            //Ping 选项设置
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            //测试数据
            string data = "test data hello world aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int size = buffer.Length;

            //设置超时时间
            int timeout = 1000;
            //调用同步 send 方法发送数据,将返回结果保存至PingReply实例
            PingReply reply = pingSender.Send("chexian.ubox.cn", timeout, buffer, options);
            if(reply.Status == IPStatus.Success)
            {
             
                return reply.RoundtripTime;
            }

            return 0;
        }


        [RpcFunc(Name = "downloadFile")]
        public IAsyncTaskCallback DownloadFile(string url,string localfile)
        {
            IAsyncTaskCallback asyncTaskCallback = new DownloadAsyncTaskCallback(url, localfile);
            asyncTaskCallback.InvokeAsync();
            return asyncTaskCallback;
        }

       
    }
}
