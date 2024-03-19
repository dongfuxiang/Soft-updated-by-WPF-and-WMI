/*----------------------------------------------------------------
// Copyright © 2021 HYC. All Rights Reserved
// 版权所有。
//
// =================================
// CLR版本     ：4.0.30319.42000
// 命名空间    ：AppUpdate.Tools
// 文件名称    ：Enums
// =================================
// 创 建 者    ：dongfx
// 创建日期    ：2024/3/14 13:27:24
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

namespace AppUpdate.Tools
{
    public enum UpdateStep
    {

        None = -1,
        /// <summary>
        /// 检查连接
        /// </summary>
        CheeckPing = 1,
        /// <summary>
        /// 查找可更新文件
        /// </summary>
        FindUpdateFiles = 2,
        /// <summary>
        /// 连接到WMI
        /// </summary>
        ConnectWMI = 3,
        /// <summary>
        /// 检查进程是否打开
        /// </summary>
        CheckProcessOn = 4,
        /// <summary>
        /// 关闭进程
        /// </summary>
        ShutDownProcess = 5,
        /// <summary>
        /// 更新
        /// </summary>
        Update = 6,
        /// <summary>
        /// 检查进程是否打开
        /// </summary>
        CheckProcessDown = 7,
        /// <summary>
        /// 打开进程
        /// </summary>
        CreateProcess = 8

    }

}
