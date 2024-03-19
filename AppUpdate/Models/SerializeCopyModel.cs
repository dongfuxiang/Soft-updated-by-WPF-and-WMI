/*----------------------------------------------------------------
// Copyright © 2021 HYC. All Rights Reserved
// 版权所有。
//
// =================================
// CLR版本     ：4.0.30319.42000
// 命名空间    ：AppUpdate.Models
// 文件名称    ：SerializeCopyModel
// =================================
// 创 建 者    ：dongfx
// 创建日期    ：2024/3/8 11:07:50
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppUpdate.Models
{
    /// <summary>
    /// 此类用来序列化CopyModel，因为继承ObservableObject的模型序列化有问题
    /// </summary>
    public class SerializeCopyModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 源文件路径
        /// </summary>
        public string SourcePath { get; set; }
        /// <summary>
        /// 目标路径
        /// </summary>
        public string TargetPath { get; set; }
        /// <summary>
        /// 目标程序路径
        /// </summary>
        public string ExePath { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
    }
}
