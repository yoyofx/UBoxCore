using System;
using UBoxCoreLib;

namespace UBoxCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("devices init...");

            IRecorder recorder = new HPRecorder();

          
            recorder.OnLog((t, log) =>
            {
                Console.WriteLine(log);
            });


         

            recorder.OnDeviceConnected((t, c) => {

                Console.WriteLine("devices connected.");
            });


            recorder.OnCall((c, phone) => {

                Console.WriteLine("On Call Phone: " + phone);
            });


            if (recorder.ConnectDevice())
            {
                Console.WriteLine("devices connecting...");
            }



            //Console.Write("Input send phone number:");
            //var phonenumber = Console.ReadLine();
            //recorder.StartRecordVoice("D:\\mp3\\tttt.mp3");
            //recorder.CallPhone("0018633317303");


            var key = Console.ReadKey();


            recorder.StartReadLineVoltage();




            var key1 = Console.ReadKey();

            recorder.StopReadLineVoltage();
            recorder.StopRecordVoice();
            recorder.CloseDevice();

        }



     
    }
}
