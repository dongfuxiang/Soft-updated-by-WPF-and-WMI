/*----------------------------------------------------------------
// Copyright © 2021 HYC. All Rights Reserved
// 版权所有。
//
// =================================
// CLR版本     ：4.0.30319.42000
// 命名空间    ：AppUpdate.Converters
// 文件名称    ：StepIdConverter
// =================================
// 创 建 者    ：dongfx
// 创建日期    ：2024/3/14 13:41:15
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
using AppUpdate.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AppUpdate.Converters
{
    class StepIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                UpdateStep step = (UpdateStep)value;
                if (parameter != null)
                {
                    string param = (string)parameter;
                    switch (param)
                    {
                        case "TextBlock":
                            switch (step)
                            {
                                case UpdateStep.ConnectWMI: return "连接到WMI……";
                                case UpdateStep.CheckProcessOn: return "检查进程是否打开……";
                                case UpdateStep.ShutDownProcess: return "关闭进程……";
                                case UpdateStep.Update: return "执行更新……";
                                case UpdateStep.CheckProcessDown: return "检查进程是否关闭……";
                                case UpdateStep.CreateProcess: return "打开进程……";
                                case UpdateStep.CheeckPing: return "检查连接……";
                                case UpdateStep.FindUpdateFiles: return "查找更新文件……";
                                default:
                                    break;
                            }
                            break;
                        case "Grid_IsEnabled":
                            return (int)step == -1 ? true : false;
                        case "Loading_Visibility":
                            return (int)step == -1 ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
