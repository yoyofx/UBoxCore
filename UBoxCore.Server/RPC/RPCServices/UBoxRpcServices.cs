using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UBoxCore.Server.Utils;
using UBoxCoreLib;

namespace UBoxCore.Server.RPC.RPCServices
{
    [RpcService]
    public class UBoxRpcServices
    {

        [RpcFunc(Name = "upfiles")]
        public bool Upfiles(string url,string filename,string callRecordsId, string dealerid)
        {
            var dic = new Dictionary<string, string> {
                {"callRecordsId",callRecordsId  },
                {"dealerId", dealerid }
            };
            try
            {
                string res = HttpHelper.Upfile("http://chexian.ubox.cn/gateway/boxapi/callrecords/upload", "d:\\mp3\\588.mp3", dic);

                dynamic ret = JsonConvert.DeserializeObject(res);

                if(ret.result.Value == 1)
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }

     

        [RpcFunc(Name = "callPhone")]
        public bool CallPhone(string phone)
        {
            Program.Recorder.CallPhone(phone);
            return true;
        }


        [RpcFunc(Name = "startRecordVoice")]
        public bool StartRecordVoice(string path)
        {
            return Program.Recorder.StartRecordVoice(path);
        }



        [RpcFunc(Name = "stopRecordVoice")]
        public bool StopRecordVoice()
        {
            Program.Recorder.StopRecordVoice();
            return true;
        }

        [RpcFunc(Name = "startReadLineVoltage")]
        public bool StartReadLineVoltage()
        {
            Program.Recorder.StartReadLineVoltage();
            return true;
        }

        [RpcFunc(Name = "stopReadLineVoltage")]
        public bool StopReadLineVoltage()
        {
            Program.Recorder.StopReadLineVoltage();
            return true;
        }


        [RpcFunc(Name = "setHookoffThreshold")]
        public bool SetHookoffThreshold(long v)
        {
            Program.Recorder.SetHookoffThreshold((int)v);
            return true;
        }


        [RpcFunc(Name = "connectDevice")]
        public bool ConnectDevice()
        {
            return Program.Recorder.ConnectDevice();
        }


        [RpcFunc(Name = "closeDevice")]
        public bool CloseDevice()
        {
            Program.Recorder.CloseDevice();
            return true;
        }


        [RpcFunc(Name = "getHookStatus")]

        public int GetHookStatus()
        {
            var channel = Program.Recorder.getCurrentChannel() as Channel;
            if (channel != null)
            {
                return channel._updown;
            }

            return 0;
        }


        [RpcFunc(Name = "getRecordStatus")]

        public bool GetRecordStatus()
        {
            var channel = Program.Recorder.getCurrentChannel() as Channel;
            if (channel != null)
            {
                return channel._bRecording;
            }

            return false;
        }

        [RpcFunc(Name = "getDeviceStatus")]
        public bool GetDeviceStatus()
        {
            return Program.Recorder.getCurrentChannel() != null;
        }





    }
}
