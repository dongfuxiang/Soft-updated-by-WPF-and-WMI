/*----------------------------------------------------------------
// Copyright © 2021 HYC. All Rights Reserved
// 版权所有。
//
// =================================
// CLR版本     ：4.0.30319.42000
// 命名空间    ：AppUpdate.ViewModels
// 文件名称    ：MainWindowViewModel
// =================================
// 创 建 者    ：dongfx
// 创建日期    ：2024/2/26 17:08:20
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
using AppUpdate.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = HandyControl.Controls.MessageBox;

namespace AppUpdate.ViewModels
{
    class MainWindowViewModel : ObservableObject
    {
        #region 属性
        /// <summary>
        /// 源与目标
        /// </summary>
        private ObservableCollection<CopyModel> copyModels;

        public ObservableCollection<CopyModel> CopyModels
        {
            get => copyModels;
            set => SetProperty(ref copyModels, value);
        }


        /// <summary>
        /// 所有待复制的文件
        /// </summary>
        private ObservableCollection<FileInfo> filesToCopy;

        public ObservableCollection<FileInfo> FilesToCopy
        {
            get => filesToCopy;
            set => SetProperty(ref filesToCopy, value);
        }

        /// <summary>
        /// 已经复制的文件
        /// </summary>
        private ObservableCollection<FileInfo> copiedFiles;

        public ObservableCollection<FileInfo> CopiedFiles
        {
            get => copiedFiles;
            set => SetProperty(ref copiedFiles, value);
        }

        /// <summary>
        /// 更新步骤
        /// </summary>
        private UpdateStep stepId;
        public UpdateStep StepId
        {
            get => stepId;
            set => SetProperty(ref stepId, value);
        }

        #endregion

        #region 命令

        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand<CopyModel> CopyCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand<CopyModel> RemoveCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand<CopyModel> CheckUpdateCommand { get; set; }

        #endregion
        public MainWindowViewModel()
        {
            LoadedCommand = new RelayCommand(Loaded);
            CopyCommand = new RelayCommand<CopyModel>(CopyAsync);
            AddCommand = new RelayCommand(Add);
            RemoveCommand = new RelayCommand<CopyModel>(Remove);
            SaveCommand = new RelayCommand(Save);
            CheckUpdateCommand = new RelayCommand<CopyModel>(CheckUpdateAsync);
        }

        /// <summary>
        /// 窗体启动事件
        /// </summary>
        public void Loaded()
        {
            Load();
        }
        /// <summary>
        /// 更新
        /// </summary>
        public async void CopyAsync(CopyModel model)
        {
            try
            {
                ProcessOperation wop = new ProcessOperation(model);
                //是否本地
                if (!wop.CheckIsLocal())
                {  //1.连接到WMI
                    StepId = UpdateStep.ConnectWMI;
                    await Task.Delay(200);
                    await wop.ConnectWMIAsync();
                }

                //2.检查进程是否打开
                StepId = UpdateStep.CheckProcessOn;
                await Task.Delay(200);

                bool chkOnRes = await wop.CheckProcessOn();
                if (chkOnRes)
                {
                    //3.进程若打开，则关闭进程
                    StepId = UpdateStep.ShutDownProcess;
                    await Task.Delay(200);
                    wop.RemoveProcess();
                }
                //4.执行更新
                StepId = UpdateStep.Update;
                await Task.Delay(200);

                //更新
                //执行Update
                if (FilesToCopy == null) return;
                CopiedFiles = new ObservableCollection<FileInfo>();
                string targetFilePath = model.TargetPath;
                if (FilesToCopy.Count > 0)
                {
                    //复制文件
                    await Task.Run(() =>
                      {
                          foreach (FileInfo item in filesToCopy)
                          {
                              File.Copy(item.FullName, Path.Combine(targetFilePath, item.Name), true);
                              CopiedFiles.Add(item);
                          }
                      });
                }

                var res = MessageBox.Show("更新完毕，是否打开软件？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    //5.打开进程
                    StepId = UpdateStep.CreateProcess;
                    await Task.Delay(200);
                    wop.CreateProcess();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                StepId = UpdateStep.None;
            }
        }
        /// <summary>
        /// 检查更新
        /// </summary>
        public async void CheckUpdateAsync(CopyModel model)
        {
            try
            {
                if (model == null) return;
                StepId = UpdateStep.CheeckPing;
                await Task.Delay(500);
                bool pingRes = await Ping(model);
                if (!pingRes)
                {
                    StepId = UpdateStep.None;
                    return;
                }
                StepId = UpdateStep.FindUpdateFiles;
                await Task.Delay(500);
                //WMIOperation.CreatProcess(model);
                //拉取远程更新目录的文件
                //源文件路径
                string sourceFilePath = model.SourcePath;
                //string sourceFilePath = $@"\\{model.SourceIp}{model.SourcePath.Split(':')[1]}";
                string targetFilePath = model.TargetPath;
                //string targetFilePath = $@"\\{model.TargetIP}{model.TargetPath.Split(':')[1]}";
                //源所有文件
                FileInfo[] sourceFiles = new DirectoryInfo(sourceFilePath).GetFiles();
                //目标所有文件
                FileInfo[] targetFiles = new DirectoryInfo(targetFilePath).GetFiles();

                //查找需要复制的文件
                FilesToCopy = new ObservableCollection<FileInfo>();
                CopiedFiles = new ObservableCollection<FileInfo>();
                foreach (FileInfo srcFile in sourceFiles)
                {
                    if ((srcFile.Attributes & FileAttributes.Hidden) > 0) continue;

                    var lst = targetFiles.Where(p => p.Name.ToLower() == srcFile.Name.ToLower()).ToList();
                    if (lst.Count > 0)
                    {
                        FileInfo dstFile = lst[0];
                        if (srcFile.LastWriteTime > dstFile.LastWriteTime)
                        {
                            FilesToCopy.Add(srcFile);
                            continue;
                        }
                    }
                    else
                    {
                        FilesToCopy.Add(srcFile);
                        continue;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                StepId = UpdateStep.None;
            }
        }


        /// <summary>
        /// 添加一个CopyModel
        /// </summary>
        public void Add()
        {
            try
            {
                if (CopyModels != null)
                {
                    CopyModels.Add(new CopyModel());
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 移除一个CopyModel
        /// </summary>
        /// <param name="model"></param>
        public void Remove(CopyModel model)
        {
            try
            {
                if (CopyModels != null)
                {
                    CopyModels.Remove(model);
                    MessageBox.Show("完成");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 保存所有CopyModel
        /// </summary>
        public void Save()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CopyModels.Json");

                List<SerializeCopyModel> models = new List<SerializeCopyModel>();
                foreach (CopyModel item in CopyModels)
                {
                    models.Add(new SerializeCopyModel
                    {
                        Name = item.Name,
                        SourcePath = item.SourcePath,
                        TargetPath = item.TargetPath,
                        ExePath = item.ExePath,
                        UserName = item.UserName,
                        PassWord = item.PassWord
                    });
                }

                Helper.ObjectToJson(models, path);
                MessageBox.Show("完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 加载CopyModel
        /// </summary>
        public void Load()
        {
            try
            {
                CopyModels = new ObservableCollection<CopyModel>();
                List<SerializeCopyModel> models = new List<SerializeCopyModel>();
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CopyModels.Json");
                if (!File.Exists(path))
                {
                    Helper.ObjectToJson(models, path);
                }

                models = Helper.JsonToObject<List<SerializeCopyModel>>(path);
                foreach (SerializeCopyModel item in models)
                {
                    CopyModels.Add(new CopyModel
                    {
                        Name = item.Name,
                        SourcePath = item.SourcePath,
                        TargetPath = item.TargetPath,
                        ExePath = item.ExePath,
                        UserName = item.UserName,
                        PassWord = item.PassWord
                    });
                }
                StepId = UpdateStep.None;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }


        /// <summary>
        /// 执行Ping，检测目标IP是否可用
        /// </summary>
        /// <param name="strIP"></param>
        /// <returns></returns>
        private async Task<bool> Ping(CopyModel model)
        {
            return await Task.Run(() =>
             {
                 string sourIP = model.SourcePath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];
                 if (!Helper.PingIP(sourIP))
                 {
                     MessageBox.Show($"源IP {sourIP} 无法访问！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                     return false;
                 }
                 string targetIP = model.TargetPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];
                 if (!Helper.PingIP(targetIP))
                 {
                     MessageBox.Show($"目标IP {targetIP} 无法访问！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                     return false;
                 }
                 return true;
             });


        }
    }


}
