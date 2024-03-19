/*----------------------------------------------------------------
// Copyright © 2021 HYC. All Rights Reserved
// 版权所有。
//
// =================================
// CLR版本     ：4.0.30319.42000
// 命名空间    ：AppUpdate.Tools
// 文件名称    ：Helper
// =================================
// 创 建 者    ：dongfx
// 创建日期    ：2024/3/14 14:21:17
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
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppUpdate.Tools
{
    class Helper
    {
        /// <summary>
        /// 对象序列化成Json文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="pth"></param>
        public static void ObjectToJson<T>(T t, string path) where T : class
        {
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T));
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                formatter.WriteObject(stream, t);
            }
        }
        /// <summary>
        /// json字符串转成对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="json">json格式字符串</param>
        /// <returns>对象</returns>
        public static T JsonToObject<T>(string path) where T : class
        {
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T));
            //获取Json字符串
            string json = File.ReadAllText(path);
            using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            {
                T result = formatter.ReadObject(stream) as T;
                return result;
            }
        }

        /// <summary>
        /// 执行Ping，检测目标IP是否可用
        /// </summary>
        /// <param name="strIP"></param>
        /// <returns></returns>
        public static bool PingIP(string strIP)
        {
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(strIP, 120);//第一个参数为ip地址，第二个参数为ping的时间
            if (reply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }
    }
}
