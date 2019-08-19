using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using AutoUpdater.model;

namespace AutoUpdater
{
    public class AutoUpdate
    {
        UpdateConfigModel _model = new UpdateConfigModel();
        private LocalConf _local;
        string _appBaseDir = "";//本地应用程序根目录
        string _remoteDir = "";//服务器更新文件根目录
        string _backDir = "";//本地文件备份路径

        /// <summary>
        /// 公共方法
        /// </summary>
        /// <param name="func">方法体</param>
        /// <param name="workName">当前方法描述</param>
        /// <param name="faildMessage">当前方法执行失败描述</param>
        /// <returns></returns>
        private bool Work(Func<bool> func, string workName, string faildMessage)
        {
            try
            {
                if (!string.IsNullOrEmpty(workName))
                    Console.WriteLine($"正在执行 --> {workName}");

                var flag = func();

                if (!flag)
                {
                    if (!string.IsNullOrEmpty(faildMessage))
                        Console.WriteLine(faildMessage);
                    return false;
                }

                if (!string.IsNullOrEmpty(workName))
                    Console.WriteLine($"结束执行 --> {workName}");


                Console.WriteLine("...");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 开始
        /// </summary>
        public bool Start()
        {
            try
            {
                _local = new LocalConf();

                //更新
                bool reUpdate = Update();
                if (!reUpdate)
                {
                    return false;
                }

                //重启
                ReStart();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public bool Update()
        {
            Console.WriteLine("***************开始更新****************");

            //1. 检查更新，获取UpdateConfig.xml,从而获得需要新增，更新，删除的工作清单 
            if (!Work(CheckUpdate, "检查更新", "不需要更新")) return false;


            //2. 检查更新文件数量
            if (!Work(HasUpdateFiles, "", "没有可更新的文件")) return false;


            //3. 如果程序已经打开，进程kill掉。
            if (!Work(IsAppClose, "检查应用程序启动状态", "未关闭应用程序，更新停止")) return false;


            //4. 备份本地程序
            if (!Work(BackUp, "备份程序", "未能备份应用程序，更新停止")) return false;


            //5. 开始更新，从任务清单获取任务列表依次下载文件，然后执行相关操作（增删改）
            if (Work(DoUpdate, "更新文件", ""))
            {
                //6.1 为true时，更新本地版本号和更新时间
                _local.Version = _model.Version;
                _local.UpdateTime = _model.UpdateTime;
            }
            else
            {
                //6.2 为false时，还原本地程序
                if (!Work(ReBack, "系统还原", "系统还原失败")) return false;
            }


            //7. 删除备份程序
            Work(DeleteBackFiles, "删除备份文件", "备份文件删除失败");


            Console.WriteLine("***************更新完毕！****************");

            return true;
        }

        /// <summary>
        /// 重新启动
        /// </summary>
        public bool ReStart()
        {
            if (_model.ReStart)
            {
                Console.WriteLine("正在重启程序...");
                Thread.Sleep(1000);
                try
                {
                    bool winAutoUpdaterRuning = false;
                    Process[] processList = Process.GetProcesses();
                    foreach (Process process in processList)
                    {
                        if (process.ProcessName == _model.AppName || process.ProcessName == _model.AppName + ".exe")
                        {
                            winAutoUpdaterRuning = true;
                        }
                    }
                    if (!winAutoUpdaterRuning)
                    {
                        Process.Start(Path.Combine(_appBaseDir, _model.AppName));
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("重启应用程序失败：" + ex.Message);
                    return false;
                }
            }
            return true;
        }


        #region 1. 检查更新

        /// <summary>
        /// 检查更新
        /// </summary>
        /// <returns></returns>
        private bool CheckUpdate()
        {
            bool result;
            try
            {
                string localVersion = GetLocalVersion();//获取本地版本
                string serviceVersion = GetServiceVersion();//获取服务器版本
                result = localVersion != serviceVersion;

                //_appBaseDir = Path.GetDirectoryName(ConfigurationManager.AppSettings["AppPath"]);
                _appBaseDir = AppDomain.CurrentDomain.BaseDirectory;
                _remoteDir = _model.UrlAddress;

                if (result && _model.UpdateYes)
                {
                    //if (MessageBox.Show(_model.UpdateYesMsg, "提醒", MessageBoxButtons.YesNo) == DialogResult.No)
                    //{
                    //    result = false;
                    //}
                }
            }
            catch (Exception ex) { throw ex; }
            return result;
        }

        /// <summary>
        /// 获取服务器版本
        /// </summary>
        /// <returns></returns>
        private string GetServiceVersion()
        {
            string serverConfig = _local.UpdateConfig;
            UpdateConfigModel model = new UpdateConfigModel();
            _model = model.GetRemoteModel(serverConfig);
            return _model.Version;
        }
        
        /// <summary>
        /// 获取本地版本
        /// </summary>
        /// <returns></returns>
        private string GetLocalVersion()
        {
            return _local.Version;
        }


        #endregion

        # region 2. 检查更新的文件数量
        private bool HasUpdateFiles()
        {
            int count = _model.UpdateFileList.Count + _model.DeleteFileList.Count;
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        # region 3. 检查应用程序是否已打开，如果打开就提醒关闭
        private bool IsAppClose()
        {
            string appPath = _local.AppPath;
            string appName = Path.GetFileNameWithoutExtension(appPath);

            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                if (process.ProcessName == appName || process.ProcessName == appName + ".exe")
                {
                    //if (MessageBox.Show("应用程序已经打开，是否现在关闭进行系统更新？", "提醒", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //{
                    //    process.Kill();
                    //    return true;
                    //}
                    //return false;
                    // todo 目前强制关闭
                    process.Kill();
                    return true;
                }
            }
            return true;
        }

        #endregion

        #region 4. 备份本地程序

        //备份本地程序
        private bool BackUp()
        {
            string appPath = _local.AppPath;
            var directoryInfo = new FileInfo(appPath).Directory;
            if (directoryInfo?.Parent != null) _backDir = directoryInfo.Parent.FullName + "\\_BackFiles";
            return CopyDir(_appBaseDir, _backDir);
        }

        /// <summary>
        /// 备份文件处理
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="aimPath"></param>
        /// <returns></returns>
        private bool CopyDir(string srcPath, string aimPath)
        {
            string name = Process.GetCurrentProcess().ProcessName;
            string[] filters = { $"{name}.exe", $"{name}.exe.config", $"{name}.pdb" };

            int count = 0;
            //string logPath = "";

            bool copyFile = false;
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加 
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                {
                    aimPath += Path.DirectorySeparatorChar;
                }
                // 判断目标目录是否存在如果不存在则新建 
                if (!Directory.Exists(aimPath))
                {
                    Directory.CreateDirectory(aimPath);
                }
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组 
                // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法 
                // string[] fileList = Directory.GetFiles（srcPath）； 
                string[] fileList = Directory.GetFileSystemEntries(srcPath);

                string str = "";
                if (count == 0)
                {
                    str = DateTime.Now.ToString(CultureInfo.InvariantCulture) + "------";
                }

                // 遍历所有的文件和目录 
                foreach (string file in fileList)
                {
                    // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件 
                    if (Directory.Exists(file))
                    {
                        CopyDir(file, aimPath + Path.GetFileName(file));
                        if (count == 0)
                        {
                            str += file + ",";
                        }
                    }
                    // 否则直接Copy文件 
                    else
                    {
                        bool flag = false;
                        foreach (var filter in filters)
                        {
                            if (Path.GetFileName(file) == filter) flag = true;
                        }
                        if (!flag)
                            File.Copy(file, aimPath + Path.GetFileName(file), true);

                        if (count == 0)
                        {
                            str += file + ",";
                        }
                    }
                }

                //if (System.IO.File.Exists(logPath))
                //{
                //    using (StreamWriter sw = new StreamWriter(logPath, true, Encoding.Default))
                //    {
                //        sw.WriteLine(str);
                //    }
                //}
                //else
                //{
                //    File.Create(logPath).Close();

                //    using (StreamWriter sw = new StreamWriter(logPath, true, Encoding.Default))
                //    {
                //        sw.WriteLine(str);
                //    }
                //}
                copyFile = true;

            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
            return copyFile;
        }

        #endregion

        #region 5. 执行更新文件

        //执行更新文件
        private bool DoUpdate()
        {
            int count = _model.UpdateFileList.Count + _model.DeleteFileList.Count;
            int i = 0;
            int err = 0;
            _model.UpdateFileList.ForEach(t =>
            {
                if (err > 0) return;

                i++;
                if (!DoOneUpdate(t, UpdateType.Update)) err++;

                Console.WriteLine($"正在更新-->{t} ...{((double)i / count):P} ");
            });

            _model.DeleteFileList.ForEach(t =>
            {
                if (err > 0) return;

                i++;
                if (!DoOneUpdate(t, UpdateType.Delete)) err++;

                Console.WriteLine($"正在更新-->{t} ...{((double)i / count):P} ");
            });

            if (err > 0)
                return false;
            else
                return true;
        }

        //执行单个文件更新
        private bool DoOneUpdate(string fileName, UpdateType upType)
        {
            try
            {

                string filePath = Path.Combine(_appBaseDir, fileName);

                if (upType == UpdateType.Update)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());

                        WebRequest.Create(Path.Combine(_remoteDir, fileName));

                        webClient.DownloadFile(Path.Combine(_remoteDir, fileName), filePath);

                        return true;
                    }
                }
                if (File.Exists(filePath))
                    File.Delete(filePath);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新[{fileName}]失败: {ex.Message}");
                return false;
            }

        }


        #endregion

        /// <summary>
        /// 还原备份文件
        /// </summary>
        /// <returns></returns>
        private bool ReBack()
        {
            DelectDir(_appBaseDir);

            return CopyDir(_backDir, _appBaseDir);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcPath"></param>
        private void DelectDir(string srcPath)
        {

            string name = Process.GetCurrentProcess().ProcessName;
            string[] filters = { $"{name}.exe", $"{name}.exe.config", $"{name}.pdb" };
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo file in fileinfo)
                {
                    if (file is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(file.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        bool flag = false;
                        foreach (var filter in filters)
                        {
                            if (file.Name == filter) flag = true;
                        }
                        if (!flag)
                            File.Delete(file.FullName);      //删除指定文件
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 最后，删除备份
        /// </summary>
        /// <returns></returns>
        private bool DeleteBackFiles()
        {
            try
            {
                string appPath = _local.AppPath;
                var directoryInfo = new FileInfo(appPath).Directory;
                if (directoryInfo?.Parent != null)
                {
                    string backPath = directoryInfo.Parent.FullName + "\\_BackFiles";
                    Directory.Delete(backPath, true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public enum UpdateType
        {
            Update,
            Delete
        }
    }
}
