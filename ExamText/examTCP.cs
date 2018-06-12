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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ExamTextServer
{
    public class examTCP
    {
        
        string Path = AppDomain.CurrentDomain.BaseDirectory + "Err.txt";
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
                //获取服务端消息
                Thread threadServer = new Thread(new ThreadStart(GetServerMsg));
                threadServer.IsBackground = true;
                threadServer.Start();               
            }
            catch (Exception ex)
            {
                WriteErr("连接服务器出错：" + Environment.NewLine + "Connect" + ex.ToString());
            }
            //与后台保持连接
            Thread threadHeart = new Thread(new ThreadStart(SendHeart));
            threadHeart.IsBackground = true;
            threadHeart.Start();
        }
        void GetServerMsg()
        {
           
            while (true)
            {
                try
                {
                    NetworkStream networkStream = tcpClient.GetStream();
                    //将网络流作为二进制读写对象
                    br = new BinaryReader(networkStream);
                    bw = new BinaryWriter(networkStream);
                    string sReader = br.ReadString(); //接收消息  
                    string xxx = sReader;
                    string sWriter = "接收到消息";
                    bw.Write(sWriter);   //向对方发送消息  
                }
                catch(Exception ex)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
        }

        public void WriteErr(string msg)
        {
            if (!File.Exists(Path))
            {
                File.Create(Path);
            }
            StreamWriter sw = new StreamWriter(Path, true, Encoding.UTF8);
            sw.WriteLine(msg);
            sw.Close();
            sw.Dispose();
        }
        /// <summary>
        /// 重连
        /// </summary>
        public void Reconnect()
        {
            try
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
                tcpClient = new TcpClient(IP, port);
                //获取网络流
                NetworkStream networkStream = tcpClient.GetStream();
                //将网络流作为二进制读写对象
                br = new BinaryReader(networkStream);
                bw = new BinaryWriter(networkStream);
            }
            catch (Exception ex)
            {
                WriteErr("重连服务器出错：" + Environment.NewLine + "Reconnect" + ex.ToString());
            }
        }

        /// <summary>
        /// 给服务器心跳，10秒一次
        /// </summary>
        private void SendHeart()
        {
            while (true)
            {
                Thread.Sleep(10000);
                SendMsg("0000");
            }
        }

        /// <summary>
        /// 发送消息到服务器的方法，带发送长度
        /// </summary>
        /// <param name="msg">消息内容</param>
        public void SendMsg(string msgs)
        {
            try
            {
                byte[] msg = Encoding.UTF8.GetBytes(msgs);
                int length = msg.Length;
                short lengthall = (short)(length + 2);
                byte[] lengthByte = System.BitConverter.GetBytes(lengthall);
                byte[] all = lengthByte.Concat(msg).ToArray();
                //然后将字节数组写入网络流
                if (bw != null && tcpClient.Connected == true)
                {
                    bw.Write(all);
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
                    this.Reconnect();
                }
            }
            catch (Exception ex)
            {
                WriteErr("发送消息到服务器出错：" + Environment.NewLine + "SendMsg" + ex.ToString());
            }
        }
    }
}
