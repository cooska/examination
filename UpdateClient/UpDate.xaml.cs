using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UpdateClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpDate : Window
    {
        WebClient wb = new WebClient();
        string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        delegate void dlg_ActionWork();
        string TextCfg = "";
        string cfg_str = "[ver]\r\nvalue={th}\r\n[ck]\r\nvalue=http://coos45.gotoip11.com/update/version.txt\r\n[up]\r\nvalue=http://coos45.gotoip11.com/update/upclient.zip";
        void AsyncActionWork(dlg_ActionWork hd)
        {
            hd.BeginInvoke(ActionCallBack, hd);
        }
        void ActionCallBack(IAsyncResult rs)
        {
            dlg_ActionWork hd = (dlg_ActionWork)rs.AsyncState;
            hd.EndInvoke(rs);
        }
        public UpDate()
        {
            InitializeComponent();
            ReadCfg();
            CheackVersion();
        }
        /// <summary>
        /// 读取配置文件
        /// </summary>
        void ReadCfg()
        {
            TextCfg = File.ReadAllText(AppPath+ "UpConfig.txt");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (btn_down.Content.ToString() != "点击启动")
            {
                tbk_msg.Text = "下载中..";
                AsyncActionWork(DownFile);
            }
            else {
                StartExt();
            }
        }
        private void StartExt()
        {
            //执行开启主程序
            Process prc = new Process();
            prc.StartInfo.FileName = AppPath + "ExamTextServer.exe";
            prc.Start();
            App.Current.Shutdown();//退出主进程

        }
        void DownFile()
        {
            try
            {
                string SavePath = AppPath + "up";
                if (!Directory.Exists(SavePath))
                {
                    Directory.CreateDirectory(SavePath);
                }
                SavePath += "\\upclient.zip";
                wb.DownloadFile(GetUpUrl, SavePath);
                //解压文件 到up文件夹下
                bool IsOk = UnZipFile(SavePath, "upclient.zip");
                if (IsOk)
                {
                    this.Dispatcher.BeginInvoke(new Action(()=> {
                        //写入cfg
                        File.WriteAllText(AppPath + "UpConfig.txt", cfg_str);
                        tbk_msg.Text = "文件更新完毕!";
                        btn_down.Content = "点击启动";
                    }) );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("下载失败请重试!");
            }
        }
        
      
        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <param name="versionName"></param>
        /// <returns></returns>
        public bool UnZipFile(string zipFilePath, string versionName)
        {
            try
            {
                if (!File.Exists(zipFilePath))
                {
                    return false;
                }
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        Console.WriteLine(theEntry.Name);
                        string directoryName = System.IO.Path.GetDirectoryName(theEntry.Name);
                        string fileName = System.IO.Path.GetFileName(theEntry.Name);
                        // create directory
                        if (directoryName.Length > 0 && string.IsNullOrEmpty(fileName))
                        {
                            Directory.CreateDirectory(string.Format(@"up\{0}", theEntry.Name));
                        }
                        if (fileName != String.Empty)
                        {
                            //组装下载路径并且下载
                            using (FileStream streamWriter = File.Create(string.Format(@"up\{0}", theEntry.Name)))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                            }
                        }
                    }
                    //删除下载的压缩包文件
                    File.Delete(zipFilePath);
                    foreach (var item in Directory.GetFiles(AppPath + "up"))
                    {
                        string FileName = item.Substring(item.LastIndexOf("\\") + 1);
                        File.Copy(item, AppPath + FileName, true);
                    }
                    Directory.Delete(AppPath + "up",true);
                }
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }
        /// <summary>
        /// 检测版本
        /// </summary>
        /// <returns></returns>
        void CheackVersion()
        {
            string CkUrl = GetCkUrl;
            string SavePath = AppPath + "version.txt";
            wb.DownloadFile(CkUrl, SavePath);
            string msg = File.ReadAllText(SavePath);
            string Ver = GetVersionText("[Client]", msg);
            cfg_str = cfg_str.Replace("{th}",Ver);
            string Cur_Ver = GetCurVer;
            //如果版本相同直接打开
            if (Ver == Cur_Ver)
            {
                //删除已下载的文件
                File.Delete(SavePath);
                StartExt();
            }
            else
            {
                //删除已下载的文件
                File.Delete(SavePath);
                tbk_msg.Text = "请点击下载更新文件!";
                btn_down.Visibility = Visibility.Visible;
            }
        }
        string GetVersionText(string key, string content)
        {
            string txt = content.Substring((content.IndexOf(key) + key.Length + 2));
            return txt.Split('=')[1].Trim().Replace("\r\n", "$").Split('$')[0];
        }
        string GetCkUrl
        {
            get
            {
                var item = GetVersionText("[ck]", TextCfg);
                return item;
            }
        }
        string GetUpUrl
        {
            get
            {
                var item = GetVersionText("[up]", TextCfg);
                return item;
            }
        }
        /// <summary>
        /// 获取当前版本
        /// </summary>
        string GetCurVer
        {
            get
            {
                var item = GetVersionText("[ver]", TextCfg);
                return item;
            }
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        string GetDownUrl()
        {
            return "";
        }

    }

}
