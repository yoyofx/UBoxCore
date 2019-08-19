using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace UBoxCore.Server.RPC.Models
{
    public class DownloadAsyncTaskCallback : IAsyncTaskCallback
    {
        private Uri uri;
        private string filePath;
        private System.Net.WebClient webclient;
        private Stopwatch sw;

        public string CallbackName => "Download";


        public DownloadAsyncTaskCallback(string url,string file)
        {
            uri = new Uri(url);
            filePath = file;
            sw = new Stopwatch();
        }

        public event AsyncCompletedEventHandler AsyncCompleted;
        public event AsyncProgressChangedEventHandler AsyncProgressChanged;

        public void InvokeAsync()
        {
            var progressChangeInfo = new ProgressChangeInfo();

            webclient = new System.Net.WebClient();
            sw.Start();

            webclient.DownloadFileCompleted += (s, e) => {
                sw.Reset();
                if (AsyncCompleted != null)
                {
                    AsyncCompleted.Invoke(this, e);
                }
            };

            webclient.DownloadProgressChanged += (s, e) => {
                string value = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

                if (e.ProgressPercentage == 100)
                {
                    sw.Reset();
                }


                if (AsyncProgressChanged != null)
                {
                    progressChangeInfo.Percentage = e.ProgressPercentage;
                    progressChangeInfo.Speed = value;

                    AsyncProgressChanged.Invoke(this, progressChangeInfo);
                }

            };


            webclient.DownloadFileAsync(uri, filePath);
         

        }




    }
}
