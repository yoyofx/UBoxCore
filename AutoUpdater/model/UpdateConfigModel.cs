using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using AutoUpdater.utils;

namespace AutoUpdater.model
{
    /// <summary>
    /// UpdateConfig 配置文件 处理类
    /// </summary>
    public class UpdateConfigModel
    {
        /// <summary>
        /// 升级文件所在地址
        /// </summary>
        public string UrlAddress { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime { get; set; }

        /// <summary>
        /// 当前运行的版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public bool UpdateYes { get; set; }

        /// <summary>
        /// 非强制更新时的提示语
        /// </summary>
        public string UpdateYesMsg { get; set; }

        /// <summary>
        /// 需要更新文件列表
        /// </summary>
        public List<string> UpdateFileList = new List<string>();

        /// <summary>
        /// 需要删除文件列表
        /// </summary>
        public List<string> DeleteFileList = new List<string>();

        /// <summary>
        /// 更新成功后是否自动重新启动
        /// </summary>
        public bool ReStart { get; set; }

        /// <summary>
        /// 自动重启的程序名称(主程序名称)
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 获取服务器上的 UpdateConfig 配置
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public UpdateConfigModel GetRemoteModel(string xmlPath)
        {
            string localPath;
            using (WebClient webClient = new WebClient())
            {
                string fileName = xmlPath.Substring(xmlPath.LastIndexOf("/", StringComparison.Ordinal) + 1);
                string localDir = Directory.GetCurrentDirectory() + "\\Download\\";
                if (!Directory.Exists(localDir))
                    Directory.CreateDirectory(localDir);
                localPath = localDir + fileName;
                try
                {
                    WebRequest.Create(xmlPath);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                try
                {
                    webClient.DownloadFile(xmlPath, localPath);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            UpdateConfigModel model = new UpdateConfigModel
            {
                UrlAddress = XmlHelper.Read(localPath, $"/AutoUpdater/URLAddress", "URL"),
                UpdateTime = XmlHelper.Read(localPath, $"/AutoUpdater/UpdateInfo/UpdateTime", "Date"),
                Version = XmlHelper.Read(localPath, $"/AutoUpdater/UpdateInfo/Version", "Num"),
                UpdateYes = Boolean.Parse(XmlHelper.Read(localPath, $"/AutoUpdater/UpdateInfo/UpdateYes", "Value")),
                UpdateYesMsg = XmlHelper.Read(localPath, $"/AutoUpdater/UpdateInfo/UpdateYes", "Msg"),
                ReStart = Convert.ToBoolean(XmlHelper.Read(localPath, $"/AutoUpdater/RestartApp/ReStart", "Allow")),
                AppName = XmlHelper.Read(localPath, $"/AutoUpdater/RestartApp/AppName", "Name")
            };


            XmlHelper helper = new XmlHelper(localPath);
            {
                var children = helper.ReadAllChild($"/AutoUpdater/UpdateFileList");
                foreach (XmlNode child in children)
                {
                    if (child.NodeType == XmlNodeType.Element)
                        if (child.Attributes?["FileName"] != null)
                            model.UpdateFileList.Add(child.Attributes["FileName"].Value);
                }
            }
            {
                var children = helper.ReadAllChild($"/AutoUpdater/DeleteFileList");
                foreach (XmlNode child in children)
                {
                    if (child.NodeType == XmlNodeType.Element)
                        if (child.Attributes?["FileName"] != null)
                            model.DeleteFileList.Add(child.Attributes["FileName"].Value);
                }
            }

            //删除下载的配置文件
            {
                if (File.Exists(localPath))
                    File.Delete(localPath);
            }


            return model;

        }
    }
}
