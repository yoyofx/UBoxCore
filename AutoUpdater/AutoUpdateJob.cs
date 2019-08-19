using System;
using System.Threading.Tasks;
using AutoUpdater.model;
using Quartz;

namespace AutoUpdater
{
    [PersistJobDataAfterExecution]//保存执行状态  
    [DisallowConcurrentExecution]  //不允许并发执行 
    public class AutoUpdateJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            //MessageBox.Show("我开始执行了");
            //const string jobName = "自动升级Job";
            LocalConf local = new LocalConf();
            try
            {
                //ErrorRetryHelper.Handle(local.ReTry, () =>
                //{
                //    AutoUpdate au = new AutoUpdate();
                //    var flag = au.Start();
                //    return flag;
                //});
                AutoUpdate au = new AutoUpdate();
                au.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

  
    }
}
