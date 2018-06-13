using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExamTextServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 考试总时间(分钟)
        /// </summary>
        public sbyte SumTime =60;
        delegate void dlg_ActionTime();
        void Post_ActionTime(dlg_ActionTime hd)
        {
            hd.BeginInvoke(ActoinTimeCallBack,hd);
        }
        void ActoinTimeCallBack(IAsyncResult ars)
        {
            dlg_ActionTime hd = (dlg_ActionTime)ars.AsyncState;
            hd.EndInvoke(ars);
            MessageBox.Show("考试结束");
        }
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //new examTCP("192.168.131.22",11118).Connect();
            LoadStudentInfo();
        }
        void LoadStudentInfo()
        {
            ks_img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory+"timg.jpg",UriKind.Absolute));//strImagePath 就绝对路径
            Post_ActionTime(ActionTime);
        }
        void ActionTime()
        {
            DateTime fiveM = DateTime.Parse(string.Format("00:{0}:59", (SumTime-1)));
            bool IsBreak = false;
            while (true)
            {
                this.Dispatcher.BeginInvoke(new Action(()=> {
                    fiveM = fiveM.AddSeconds(-1);
                    ks_time.Text = string.Format("{0}分{1}秒", fiveM.Minute.ToString("00"), fiveM.Second.ToString("00"));
                    if (ks_time.Text=="00分00秒")
                    {
                        IsBreak = true;
                    }
                }));
                if (IsBreak)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
