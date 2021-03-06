﻿#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：ExamTextServer
* 项目描述 ：
* 类 名 称 ：examTCP
* 类 描 述 ：
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Administrator
* 创建时间 ：2018/6/12 15:26:44
* 更新时间 ：2018/6/12 15:26:44
*******************************************************************
* Copyright @ 湖南教育出版社-贝壳网. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion
using Maticsoft.Common.DEncrypt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace ExamTextServer
{
    public class examTCP
    {
        #region 变量定义
        /// <summary>
        /// 链接锁
        /// </summary>
        static object ConServer = new object();
        /// <summary>
        /// 模型id
        /// </summary>
        public static int module_id { get; set; }
        public static string local_id { get; set; }
        public static int exam_time { get; set; }
        public static int exam_id { get; set; }
        public delegate void dlg_isConToServer(bool YesOrNo);
        public event dlg_isConToServer On_isConToServer;
        public delegate void dlg_isGetUserInfo(string title); 
        public event dlg_isGetUserInfo On_isGetUserInfo;
        public delegate void dlg_ReConServer();
        public event dlg_ReConServer On_ReConServer;
        public delegate void dlg_NextExma();
        public event dlg_ReConServer On_NextExma;
        string ErrPath = AppDomain.CurrentDomain.BaseDirectory + "Err.txt";
        string LogPath = AppDomain.CurrentDomain.BaseDirectory + "log.txt";
        string QPath = AppDomain.CurrentDomain.BaseDirectory + "cfg.txt";
        delegate void dlg_ActionWork();
        NetworkStream networkStream = null;
        StringBuilder sb = new StringBuilder();
        Thread[] threadHeart = new Thread[2];
        /// <summary>
        /// 是否开始作答
        /// </summary>
        public bool isAnwser = false;
        #endregion

        // tcp通信对象
        private TcpClient tcpClient;
        // tcp通信中读取数据的对象
        private BinaryReader br;
        // tcp通信中写数据的对象
        private BinaryWriter bw;
        // 通信的远程服务器ip
        private string IP;
        // 通信的远程服务器端口
        private int port;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <param name="port">服务器开放的通信端口号</param>
        public examTCP(string ip, int port)
        {
            this.IP = ip;
            this.port = port;
        }

        public examTCP()
        {

        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void Connect()
        {
            try
            {
                //如果线程处于开启状态关掉
                //foreach (var item in threadHeart)
                //{
                //    if (item != null)
                //    {
                //        item.Abort();
                //    }
                //}
                //实例连接对象
                tcpClient = new TcpClient(IP, port);
                if (On_isConToServer != null){
                    On_isConToServer(true);
                }
                ActionWork();
            }
            catch (Exception ex)
            {
                if (On_isConToServer != null){
                    On_isConToServer(false);
                }
                Thread.Sleep(1000);
                Reconnect();
            }
        }
        /// <summary>
        /// 开启工作
        /// </summary>
        void ActionWork()
        {
            for (int i = 0; i < threadHeart.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        //获取消息
                        threadHeart[i] = new Thread(new ThreadStart(GetServerMsg));
                        break;
                    case 1:
                        //发送消息
                        threadHeart[i] = new Thread(new ThreadStart(SendHeart));
                        break;
                }
                threadHeart[i].IsBackground = true;
                threadHeart[i].Start();
                Thread.Sleep(500);
            }
        }
        void GetServerMsg()
        {
            try
            {
                while (tcpClient.Connected)
                {
                    Byte[] buffer = new Byte[512];
                    networkStream = tcpClient.GetStream();
                    bw = new BinaryWriter(networkStream, Encoding.BigEndianUnicode);
                    //将网络流作为二进制读写对象
                    sb.Clear();
                    int bytesRead = 0;
                    do
                    {
                        Thread.Sleep(10);
                        bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                        sb.Append(Encoding.BigEndianUnicode.GetString(buffer, 0, bytesRead));
                    } while (bytesRead == 512);
                    bw.Flush();
                    DoActionByMes(sb.ToString());
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Reconnect();
                return;
            }

        }
        Regex rg = new Regex("@@@(.*?)###", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        void DoActionByMes(string msg)
        {
            try
            {
                MatchCollection mcl = rg.Matches(msg);
                foreach (Match item in mcl)
                {
                    if (item.Value.IndexOf("@@@") >= 0 && item.Value.LastIndexOf("###") != -1)
                    {
                        string Newmsg = item.Value.Substring(3, (msg.Length - 6));
                        if (Newmsg != "hello" && Newmsg != "" && Newmsg.ToLower().IndexOf("_title_") == -1)
                        {
                            var Jsonitem = JsonConvert.DeserializeObject<root>(Newmsg);
                            switch (Jsonitem.model_type)
                            {
                                case 1://获取用户基础信息
                                    if (On_isGetUserInfo != null)
                                    {
                                        module_id = Jsonitem.module_id;
                                        local_id = Jsonitem.local_ip;
                                        exam_time = Jsonitem.exam_time;
                                        exam_id = Jsonitem.exam_id;
                                        On_isGetUserInfo(Jsonitem.module_name);
                                        //写日志
                                        WriteLog(string.Format("获取考生信息:module_id=>{0}",module_id));
                                    }
                                    break;
                                case 5://切换下一场考试
                                    exam_id = Jsonitem.exam_id;
                                    if (On_NextExma != null)
                                    {
                                        //删除当前试题缓存
                                        DeQuseFile();
                                        //函数执行顺序不能变 先执行下一场请求 再获取用户信息
                                        On_NextExma();
                                    }
                                    if (On_isGetUserInfo!=null)
                                    {
                                        On_isGetUserInfo(Jsonitem.module_name);
                                        //写日志
                                        WriteLog(string.Format("获取下一场考生信息:module_id=>{0}", module_id));
                                    }
                                  
                                    break;
                            } 
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return;
            }

        }
        static object Lck = new object();
        public void WriteErr(string msg)
        {
            lock (Lck)
            {
                File.AppendAllText(ErrPath, msg);
            }
        }
        public void WriteLog(string msg)
        {
            lock (Lck)
            {
                File.AppendAllText(LogPath, string.Format("[{0}]\r\n{1}", DateTime.Now.ToString(), msg));
            }
        }
        static object LckQ = new object();
        /// <summary>
        /// 时时写入考试作答情况
        /// </summary>
        /// <param name="rt"></param>
        public void WriteQuse(root rt)
        {
            lock (LckQ)
            {
                rt.user_info = null;
                string msg = JsonConvert.SerializeObject(rt);
                msg = DESEncrypt.Encrypt(msg);
                File.WriteAllText(QPath, msg);

                //msg = DESEncrypt.BinkEncrypt(msg);
                //FileStream fs = new FileStream(QPath, FileMode.OpenOrCreate,FileAccess.Write); //可以指定盘符，也可以指定任意文件名，还可以为word等文件
                //StreamWriter sw = new StreamWriter(fs,Encoding.UTF8); // 创建写入流
                //sw.Write(msg);
                //sw.Close();
                //fs.Close();
            }
        }
        /// <summary>
        /// 删除考试记录文件
        /// </summary>
        public void DeQuseFile()
        {
            File.Delete(QPath);
        }
        /// <summary>
        /// 判断是否存在考试记录文件
        /// </summary>
        public bool HasQuseFile
        {
            get
            {
                return File.Exists(QPath);
            }
        }
        public root GET_ques_list
        {
            get
            {
                lock (LckQ)
                {
                    root rst = new root();
                    string msg = File.ReadAllText(QPath);
                    msg = DESEncrypt.Decrypt(msg);
                    //FileStream fs = new FileStream(QPath, FileMode.Open, FileAccess.Read); //可以指定盘符，也可以指定任意文件名，还可以为word等文件
                    //StreamReader sw = new StreamReader(fs, Encoding.UTF8); // 创建写入流
                    //string msg = sw.ReadToEnd();
                    //sw.Close();
                    //fs.Close();
                    //msg = DESEncrypt.BinkDecrypt(msg);
                    rst = JsonConvert.DeserializeObject<root>(msg);
                    return rst;
                }
            }
        }
        /// <summary>
        /// 重连
        /// </summary>
        public void Reconnect()
        {
            lock (ConServer)
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();//关闭连接在重新连
                }
                //等待3秒后重连
                Thread.Sleep(2000);
                //如果用户没有作答则可一种后台重连
                if (!isAnwser)
                {
                    Connect();
                }
                else
                {
                    //如果已作答则通知界面重连
                    if (On_ReConServer != null){
                        On_ReConServer();
                    }
                }
            }
        }

        /// <summary>
        /// 给服务器心跳，1秒一次
        /// </summary>
        private void SendHeart()
        {
            while (true)
            {
                SendMsg("0000");
                Thread.Sleep(3000);
            }
        }
        static object lck_Send = new object();
        /// <summary>
        /// 发送消息到服务器的方法，带发送长度
        /// </summary>
        /// <param name="msg">消息内容</param>
        public void SendMsg(string msgs)
        {
            lock (lck_Send)
            {
                try
                {
                    msgs = string.Format("@@@{0}###", msgs);
                    byte[] msg = Encoding.BigEndianUnicode.GetBytes(msgs);
                    //然后将字节数组写入网络流
                    var xx = tcpClient.Connected;
                    bw.Write(msg);
                    bw.Flush();
                    if (msgs == "0000")//心跳写单独的文件
                    {
                        WriteErr(Environment.NewLine + "成功发送数据：" + msgs);
                    }
                    else
                    {
                        WriteErr(Environment.NewLine + "成功发送数据：" + msgs);
                    }
                }
                catch (Exception ex)
                {
                    this.Reconnect();
                    WriteErr("发送消息到服务器出错：" + Environment.NewLine + "SendMsg" + ex.ToString());
                }
            }
        }
    }
}
