using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UBoxCore.Server.Utils
{
    public class HttpHelper
    {

        public static string Upfile(string requestUri, string filename, Dictionary<string, string> values)
        {
            HttpClient httpRequestClient = new HttpClient();

            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            foreach (var keyValuePair in values)
            {
                multiContent.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
            }

            var payload =  File.ReadAllBytes(filename);
            multiContent.Add(new ByteArrayContent(payload), "myfiles", Path.GetFileName(filename)); // name must be "files"
            var response = httpRequestClient.PostAsync(requestUri, multiContent).Result;

            return  response.Content.ReadAsStringAsync().Result;

        }


    }
}
