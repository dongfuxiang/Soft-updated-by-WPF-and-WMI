/*----------------------------------------------------------------
// Copyright © 2021 HYC. All Rights Reserved
// 版权所有。
//
// =================================
// CLR版本     ：4.0.30319.42000
// 命名空间    ：AppUpdate.Models
// 文件名称    ：CopyModel
// =================================
// 创 建 者    ：dongfx
// 创建日期    ：2024/3/7 19:06:57
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
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppUpdate.Models
{

    public class CopyModel : ObservableObject
    {
        /// <summary>
        /// 名称
        /// </summary>
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }


        /// <summary>
        /// 源文件路径
        /// </summary>
        private string sourcePath;

        public string SourcePath
        {
            get => sourcePath;
            set => SetProperty(ref sourcePath, value);
        }

        /// <summary>
        /// 目标路径
        /// </summary>
        private string targetPath;
        public string TargetPath
        {
            get => targetPath;
            set => SetProperty(ref targetPath, value);
        }

        /// <summary>
        /// 目标程序路径
        /// </summary>
        private string exePath;

        public string ExePath
        {
            get => exePath;
            set => SetProperty(ref exePath, value);
        }

        /// <summary>
        /// 用户名
        /// </summary>
        private string userName;

        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        /// <summary>
        /// 密码
        /// </summary>
        private string passWord;

        public string PassWord
        {
            get => passWord;
            set => SetProperty(ref passWord, value);
        }

    }
}
