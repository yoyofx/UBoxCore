using System;
using System.IO;
using AutoUpdater.utils;

namespace AutoUpdater.model
{
    class LocalConf
    {
        private static readonly string LocalConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localconfig.xml");
        //private static readonly string LocalConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localconfig.xml");

        public string UpdateTime
        {
            get => XmlHelper.Read(LocalConfig, "/AutoUpdater/UpdateInfo/UpdateTime", "Date");
            set => XmlHelper.Update(LocalConfig, "/AutoUpdater/UpdateInfo/UpdateTime", "Date", value);
        }

        public string Version
        {
            get => XmlHelper.Read(LocalConfig, "/AutoUpdater/UpdateInfo/Version", "Num");
            set => XmlHelper.Update(LocalConfig, "/AutoUpdater/UpdateInfo/Version", "Num", value);
        }

        public string UpdateConfig
        {
            get => XmlHelper.Read(LocalConfig, "/AutoUpdater/UpdateInfo/UpdateConfig", "Url");
            set => XmlHelper.Update(LocalConfig, "/AutoUpdater/UpdateInfo/UpdateConfig", "Url", value);
        }

        public string AppPath
        {
            get => XmlHelper.Read(LocalConfig, "/AutoUpdater/UpdateInfo/AppPath", "Name");
            set => XmlHelper.Update(LocalConfig, "/AutoUpdater/UpdateInfo/AppPath", "Name", value);
        }

        public string ReTry
        {
            get => XmlHelper.Read(LocalConfig, "/AutoUpdater/UpdateInfo/ReTryTimes", "Times");
            set => XmlHelper.Update(LocalConfig, "/AutoUpdater/UpdateInfo/ReTryTimes", "Times", value);
        }

        public bool QuartzNow
        {
            get => Boolean.Parse(XmlHelper.Read(LocalConfig, "/AutoUpdater/UpdateInfo/Quartz", "StartNow"));
            set => XmlHelper.Update(LocalConfig, "/AutoUpdater/UpdateInfo/Quartz", "StartNow", value.ToString());
        }

        public string QuartzCron
        {
            get => XmlHelper.Read(LocalConfig, "/AutoUpdater/UpdateInfo/Quartz", "Cron");
            set => XmlHelper.Update(LocalConfig, "/AutoUpdater/UpdateInfo/Quartz", "Cron", value);
        }
    }
}
