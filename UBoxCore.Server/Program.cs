using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;


using UBoxCoreLib;



namespace UBoxCore.Server
{
    public class Program
    {

        public static IRecorder Recorder;

        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Bitauto智能电话助手");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Recorder = new HPRecorder();

            var config = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddEnvironmentVariables()
                          .AddJsonFile("certificate.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"certificate.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                          .Build();

            var certificateSettings = config.GetSection("certificateSettings");
            string certificateFileName = certificateSettings.GetValue<string>("filename");
            string certificatePassword = certificateSettings.GetValue<string>("password");

            var certificate = new X509Certificate2(certificateFileName, certificatePassword);

            var host = new WebHostBuilder()
                .UseKestrel(
                    options =>
                    {
                        options.AddServerHeader = false;
                        options.Listen(IPAddress.Loopback, 44321, listenOptions =>
                        {
                            listenOptions.UseHttps(certificate);
                        });
                    }
                )
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("https://localhost:44321")
                .Build();

            //System.Diagnostics.Process.Start("https://localhost:44321/web/index.html");

            host.Run();
            

           
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //下面的代码使用了C# 7中新增的模式匹配语法
            if (e.ExceptionObject is Exception ex)
            {
                Console.WriteLine($"捕获到未处理异常：{ex.GetType()}");
                Console.WriteLine($"异常信息：{ex.Message}");
                Console.WriteLine($"异常堆栈：{ex.StackTrace}");
                Console.WriteLine($"CLR即将退出：{e.IsTerminating}");
            }

            Console.ReadKey();
        }
    }
}
