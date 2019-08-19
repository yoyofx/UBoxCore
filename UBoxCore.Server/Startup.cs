using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using UBoxCore.Server;
using UBoxCore.Server.RPC;

namespace UBoxCore.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
           

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
                RequestPath = "/Web"
            });


            app.Map("/ws", SocketHandler.Map);

            lifetime.ApplicationStarted.Register(OnAppStarted);

            lifetime.ApplicationStopping.Register(OnAppStopping);

            lifetime.ApplicationStopped.Register(OnAppStopped);


            FlashPolicyfileHandler.StartAsync();
        }


        public void OnAppStarted()
        {

            RpcService.LoadServices();

            Program.Recorder.OnCall(SocketHandler.OnCall);

            Program.Recorder.OnDeviceConnected(SocketHandler.OnConnected);
            Program.Recorder.OnDeviceClosed(SocketHandler.OnClosed);

            Program.Recorder.OnDeviceError(SocketHandler.OnError);

            Program.Recorder.OnHangUp(SocketHandler.OnHangUp);
            Program.Recorder.OnHookOff(SocketHandler.OnHookOff);

            Program.Recorder.OnLineVoltage(SocketHandler.OnLineVoltage);

            Program.Recorder.OnRinging(SocketHandler.OnRinging);

            Program.Recorder.OnRingCancel(SocketHandler.OnRingCancel);

            Program.Recorder.OnLog(SocketHandler.OnLog);


            if (Program.Recorder.ConnectDevice())
            {
                Console.WriteLine("devices connecting...");
            }


           
            Process.Start(Path.Combine(Directory.GetCurrentDirectory(), "App", "UBoxCoreApp.exe"));

        }


        public void OnAppStopping()
        {
            FlashPolicyfileHandler.Cancel();

            Program.Recorder.CloseDevice();

        }


        public void OnAppStopped()
        {
            string contents = $"App stopped at {DateTime.Now}";
            Console.WriteLine(contents);

        }

    }
}
