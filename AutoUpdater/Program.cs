using System;
using System.Threading;
using System.Threading.Tasks;
using AutoUpdater.model;
using Quartz;
using Quartz.Impl;

namespace AutoUpdater
{
    class Program
    {
        static async Task Main(string[] args)
        {
          

            #region Quartz
            try
            {
                var cancelSource = new CancellationTokenSource();

                LocalConf local = new LocalConf();
                //从工厂中获取一个调度器实例化
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                
                Console.WriteLine(local.QuartzNow);
                if (local.QuartzNow)
                {
                    IJobDetail job2 = JobBuilder.Create<AutoUpdateJob>()
                        .WithIdentity("WinAutoUpdaterJob2", "JobGroup")
                        .Build();
                    ITrigger trigger2 = TriggerBuilder.Create()
                        .WithIdentity("WinAutoUpdaterJobTrigger2", "JobTriggerGroup")
                        .StartNow()
                        .Build();
                    await scheduler.ScheduleJob(job2, trigger2);
                }

                IJobDetail job = JobBuilder.Create<AutoUpdateJob>()
                    .WithIdentity("WinAutoUpdaterJob", "JobGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("WinAutoUpdaterJobTrigger", "JobTriggerGroup")
                    .WithCronSchedule(local.QuartzCron)
                    .Build();
                await scheduler.ScheduleJob(job, trigger);

                await scheduler.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            #endregion

            Console.ReadKey();

        }
    }
}
