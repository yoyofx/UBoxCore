using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }


        [Test]
        public void upfileTest()
        {

            var json = "{ \"callRecordsId\":\"462\",  \"dealerId\":\"50002218\"}";
            var dic =  JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            //var dic = new Dictionary<string, string> {
            //    {"callRecordsId","462"  },
            //    {"dealerId", "50002218" }
            //};

            string res = HttpHelper.Upfile("http://chexian.ubox.cn/gateway/boxapi/callrecords/upload", "d:\\mp3\\588.mp3",dic);
            dynamic ret = JsonConvert.DeserializeObject(res);


            Assert.AreEqual(ret.result.Value, 1);
        }

       


    }
}
