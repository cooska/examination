#region << 版 本 注 释 >>
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

namespace ExamTextServer
{
    public class examTCP
    {

        public delegate void dlg_isConToServer(bool YesOrNo);
        public event dlg_isConToServer On_isConToServer;
        public delegate void dlg_isGetUserInfo(root userinfo);
        public event dlg_isGetUserInfo On_isGetUserInfo;
        string Path = AppDomain.CurrentDomain.BaseDirectory + "Err.txt";
        string QPath = AppDomain.CurrentDomain.BaseDirectory + "cfg.txt";
        delegate void dlg_ActionWork();
        NetworkStream networkStream = null;
        StringBuilder sb = new StringBuilder();
        Thread threadHeart = null;
        void PostActionWork(dlg_ActionWork hd)
        {
            hd.BeginInvoke(CallBackActionWork, hd);
        }
        void CallBackActionWork(IAsyncResult ast)
        {
            dlg_ActionWork hd = (dlg_ActionWork)ast.AsyncState;
            hd.EndInvoke(ast);
        }
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
                //实例对象
                tcpClient = new TcpClient(IP, port);
                if (On_isConToServer != null)
                {
                    On_isConToServer(true);
                }
            }
            catch (Exception ex)
            {
                if (On_isConToServer != null)
                {
                    On_isConToServer(false);
                }
                //WriteErr("连接服务器出错：" + Environment.NewLine + "Connect" + ex.ToString());
                Thread.Sleep(1000);
                Connect();
            }
            //增开线程维护消息收发
            threadHeart = new Thread(new ThreadStart(ActionWork));
            threadHeart.IsBackground = true;
            threadHeart.Start();
        }
        void ActionWork()
        {
            PostActionWork(GetServerMsg);//收
            PostActionWork(SendHeart);//心跳发送
        }
        void GetServerMsg()
        {
            while (tcpClient.Connected)
            {
                try
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
                }
                catch (Exception ex)
                {
                    continue;
                }
                Thread.Sleep(1000);
            }
        }
        void DoActionByMes(string msg)
        {
            if (msg.IndexOf("@@@") >= 0 && msg.LastIndexOf("###") != -1)
            {
                msg = msg.Substring(3, (msg.Length - 6));
                if (msg != "hello" && msg != "")
                {
                    var item = JsonConvert.DeserializeObject<root>(msg);
                    if (On_isGetUserInfo != null)
                    {
                        On_isGetUserInfo(item);
                    }
                }
            }
        }
        static object Lck = new object();
        public void WriteErr(string msg)
        {
            if (!File.Exists(Path))
            {
                File.Create(Path);
            }
            lock (Lck)
            {
                StreamWriter sw = new StreamWriter(Path, true, Encoding.UTF8);
                sw.WriteLine(msg);
                sw.Close();
                sw.Dispose();
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
                FileStream fs = new FileStream(QPath, FileMode.OpenOrCreate, FileAccess.ReadWrite); //可以指定盘符，也可以指定任意文件名，还可以为word等文件
                StreamWriter sw = new StreamWriter(fs); // 创建写入流
                sw.WriteLine(msg);
                 sw.Close(); 
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
        /// <summary>
        /// 重连
        /// </summary>
        public void Reconnect()
        {
            Connect();
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
                    if (bw != null && tcpClient.Connected == true)
                    {
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
                    else
                    {
                        threadHeart.Abort();
                        tcpClient.Close();//关闭连接在重新连
                        this.Reconnect();
                    }
                }
                catch (Exception ex)
                {
                    threadHeart.Abort();
                    tcpClient.Close();//关闭连接在重新连
                    this.Reconnect();
                    WriteErr("发送消息到服务器出错：" + Environment.NewLine + "SendMsg" + ex.ToString());
                }
            }
        }
    }
}
