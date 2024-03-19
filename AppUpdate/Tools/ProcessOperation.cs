/*----------------------------------------------------------------
// Copyright © 2021 HYC. All Rights Reserved
// 版权所有。
//
// =================================
// CLR版本     ：4.0.30319.42000
// 命名空间    ：AppUpdate.Tools
// 文件名称    ：WMIOperation
// =================================
// 创 建 者    ：dongfx
// 创建日期    ：2024/3/13 15:16:17
// 功能描述    ：……
// 使用说明    ：
//
//
// 创建标识：
//
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/
using AppUpdate.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Threading.Tasks;

//System.Management 命名空间是.NET 框架中的 WMI 命名空间。此命名空间包括下列类对象：
//ManagementObject 或 ManagementClass：分别为单个管理对象或类。
//ManagementObjectSearcher：用于根据指定的查询或枚举检索 ManagementObject 或 ManagementClass 对象的集合。
//ManagementEventWatcher：用于预订来自 WMI 的事件通知。
//ManagementQuery：用作所有查询类的基础
namespace AppUpdate.Tools
{
    public class ProcessOperation
    {

        private List<string> list = new List<string>();
        private ManagementScope Ms = new ManagementScope();
        private ManagementPath RemoteNameSpace = null;
        public CopyModel model = new CopyModel();

        public ProcessOperation(CopyModel md)
        {
            model = md;

        }

        public async Task ConnectWMIAsync()
        {
            await Task.Run(() =>
             {
                 //ManagementScope类能够建立和远程计算机（或者本地计算机）的WMI连接，表示管理可操作范围
                 Ms = new ManagementScope();
                 string targetIP = model.TargetPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];
                 RemoteNameSpace = new ManagementPath($@"\\{targetIP}\root\cimv2");
                 //ConnectionOptions 为建立的WMI连接提供所需的所有设置
                 ConnectionOptions connOP = new ConnectionOptions();
                 //设定用于WMI连接操作的用户名
                 connOP.Username = model.UserName;
                 //设定用户的口令
                 connOP.Password = model.PassWord;
                 Ms.Path = RemoteNameSpace;
                 Ms.Options = connOP;
                 //连接到实际操作的WMI范围
                 Ms.Connect();
             });

        }

        /// <summary>
        /// 检查进程是否打开
        /// </summary>
        public async Task<bool> CheckProcessOn()
        {
            return await Task.Run(() =>
              {
                  //ObjectQuery类或其派生类用于在ManagementObjectSearcher中指定查询。
                  ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Process");
                  //WQL语句，设定的WMI查询内容和WMI的操作范围，检索WMI对象集合
                  ManagementObjectSearcher searcher = new ManagementObjectSearcher(Ms, query);
                  ManagementObjectCollection returnCollection = searcher.Get();

                  foreach (var service in returnCollection)
                  {
                      if (service["Name"].ToString() == model.Name)
                      {
                          return true;
                      }
                  }
                  return false;
              });

        }


        /// <summary>
        /// 打开一个进程
        /// </summary>
        public void CreateProcess()
        {
            bool isLocal = CheckIsLocal();
            if (isLocal)
            {
                //打开远程进程
                CreatRemoteProcess();
            }
            else
            {
                //打开本地进程
                CreatLocalProcess();
            }
        }
        /// <summary>
        /// 关闭一个进程
        /// </summary>
        public void RemoveProcess()
        {
            bool isLocal = CheckIsLocal();
            if (isLocal)
            {
                //关闭远程进程
                RemoveRemoteProcess();
            }
            else
            {
                //关闭本地进程
                RemoveLocalProcess();
            }
        }
        /// <summary>
        /// 打开一个远程进程
        /// </summary>
        /// <param name="model"></param>
        public void CreatRemoteProcess()
        {
            //建立对远程的进程操作对象
            ManagementClass processClass = new ManagementClass(Ms, new ManagementPath("Win32_Process"), null);

            //获得用来提供参数的对象
            ManagementBaseObject inParams = processClass.GetMethodParameters("Create");

            inParams["CommandLine"] = "schtasks /delete /tn \"cid\" /F";
            //删除任务
            ManagementBaseObject outParams1 = processClass.InvokeMethod("Create", inParams, null);

            //(只能启动进程，没有UI界面)
            //inParams["CommandLine"] = "F://CID//Line//CID.Line.exe";
            string path = Path.Combine(model.ExePath, model.Name);
            inParams["CommandLine"] = $"schtasks /create /tn cid /tr F:\\CID\\Main\\CID.Main.exe /sc once /st 11:11";
            //inParams["CommandLine"] = $"schtasks /create /tn cid /tr F:\\CID\\Line\\CID.Line.exe /sc once /st 11:11";
            //创建任务
            ManagementBaseObject outParams2 = processClass.InvokeMethod("Create", inParams, null);

            inParams["CommandLine"] = "schtasks /run /tn \"cid\"";
            //执行任务
            ManagementBaseObject outParams3 = processClass.InvokeMethod("Create", inParams, null);

            inParams["CommandLine"] = "schtasks /delete /tn \"cid\" /F";
            //删除任务
            ManagementBaseObject outParams4 = processClass.InvokeMethod("Create", inParams, null);

        }

        /// <summary>
        /// 关闭一个远程进程
        /// </summary>
        /// <param name="model"></param>
        public void RemoveRemoteProcess()
        {
            //ObjectQuery类或其派生类用于在ManagementObjectSearcher中指定查询。
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Process");
            //WQL语句，设定的WMI查询内容和WMI的操作范围，检索WMI对象集合
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(Ms, query);
            ManagementObjectCollection returnCollection = searcher.Get();

            foreach (ManagementObject service in returnCollection)
            {
                if (service["Name"].ToString() == model.Name)
                {
                    object[] obj = new object[] { 0 };
                    service.InvokeMethod("Terminate", obj);
                    break;
                }
            }

        }

        /// <summary>
        /// 打开一个本地进程
        /// </summary>
        public void CreatLocalProcess()
        {
            string path = Path.Combine(model.ExePath, model.Name);
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.Start();
        }

        /// <summary>
        /// 关闭一个本地进程
        /// </summary>
        public void RemoveLocalProcess()
        {
            //关闭当前程序的进程，App.exe
            string path = Path.Combine(model.ExePath, model.Name);
            Process[] pp = Process.GetProcessesByName(path);
            for (int j = 0; j < pp.Length; j++)
            {
                pp[j].Kill();
            }
        }
        /// <summary>
        /// 检查目标IP是否本地
        /// </summary>
        /// <returns></returns>
        public bool CheckIsLocal()
        {
            //判断是否为本地软件
            List<IPAddress> localIps = Dns.GetHostAddresses(Dns.GetHostName()).ToList();
            string targetIP = model.TargetPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];
            foreach (var item in localIps)
            {
                if (item.ToString() == targetIP)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
