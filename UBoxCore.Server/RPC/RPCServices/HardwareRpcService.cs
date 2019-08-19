using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using UBoxCore.Server.RPC.Models;
using System.Runtime.InteropServices;
using System.Diagnostics;
using UBoxCore.Server.Utils;

namespace UBoxCore.Server.RPC.RPCServices
{
    [RpcService]
    public class HardwareRpcService
    {
        /// <summary>
        /// 获取磁盘空间(MB)
        /// </summary>
        /// <param name="str_HardDiskName">C,D,E</param>
        /// <returns></returns>
        [RpcFunc(Name = "getHardDiskSpace")]
        public long GetHardDiskSpace(string str_HardDiskName) {

            long totalSize = 0;
            str_HardDiskName = str_HardDiskName + ":\\";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    totalSize = drive.TotalFreeSpace / (1024 * 1024);
                }
            }
            return totalSize;
        }

        [RpcFunc(Name = "getDirectories")]
        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }


        [RpcFunc(Name = "getDirFiles")]
        public string[] GetDirFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        [RpcFunc(Name = "getFileSize")]

        public long GetFileSize(string path)
        {
            var fileinfo = new FileInfo(path);
            return fileinfo.Length / 1024;
        }

        [RpcFunc(Name = "deleteFile")]
        public bool DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch { return false; }
            return true;
        }


        [RpcFunc(Name = "getSystemInfo")]
        public SystemInfo GetSystemInfo()
        {
            var systeminfo = new SystemInfo
            {
                OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
                OSDescription = RuntimeInformation.OSDescription,
                ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                ProcessorCount = Environment.ProcessorCount,
                OSVersion = Environment.OSVersion,
                MemWorkingSet = Environment.WorkingSet,
                LogicalDrives = string.Join(',', Environment.GetLogicalDrives())

            };

            return systeminfo;
        }


        [RpcFunc(Name = "shellRun")]
        public bool ShellRun(string path,string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.FileName = path;


            if (string.IsNullOrEmpty(arguments))
                Process.Start(startInfo);
            else
            {
                startInfo.Arguments = arguments;
                Process.Start(startInfo);
            }
            return true;
        }

        [RpcFunc(Name = "openFileDialog")]
        public string OpenFileDialog(string title)
        {
            return Win32.OpenFileDialog(title);
        }



        [RpcFunc(Name = "saveFileDialog")]
        public string SaveFileDialog(string title)
        {
            return Win32.SaveFileDialog(title);
        }


        [RpcFunc(Name = "createDirectory")]
        public bool CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch { return false; }
            return true;
        }

        [RpcFunc(Name = "writeTextToFile")]
        public bool WriteTextToFile(string path,string content)
        {
            try
            {
                File.WriteAllText(path, content);
            }
            catch { return false; }
            return true;
        }

        [RpcFunc(Name = "readTextFormFile")]
        public string ReadTextFormFile(string path)
        {
            return File.ReadAllText(path);
        }



    }
}
