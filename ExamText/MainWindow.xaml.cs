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
        public sbyte SumTime = 60;
        /// <summary>
        /// 按钮背景颜色
        /// </summary>
        Color color = (Color)ColorConverter.ConvertFromString("#1f4ba4");
        /// <summary>
        /// 按钮样式
        /// </summary>
        Style btn_style = null;
        delegate void dlg_ActionTime();
        void Post_ActionTime(dlg_ActionTime hd)
        {
            hd.BeginInvoke(ActoinTimeCallBack, hd);
        }
        void ActoinTimeCallBack(IAsyncResult ars)
        {
            dlg_ActionTime hd = (dlg_ActionTime)ars.AsyncState;
            hd.EndInvoke(ars);
            MessageBox.Show("考试结束");
        }
        void Post_IintData(dlg_ActionTime hd)
        {
            hd.BeginInvoke(ActionIintDataCallBack, hd);
        }
        void ActionIintDataCallBack(IAsyncResult ars)
        {
            dlg_ActionTime hd = (dlg_ActionTime)ars.AsyncState;
            hd.EndInvoke(ars);
        }
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Post_IintData(ConServer);
            btn_style = (Style)this.FindResource("BtnIcon");
            Post_IintData(InitFormData);
           
        }
        void ConServer()
        {
            new examTCP("192.168.131.22", 11118).Connect();
            this.Dispatcher.BeginInvoke(new Action(()=> {
                this.Title = "考试作答系统V1.0 [已成功连接考试服务器]";
            }));
        }
        void InitFormData()
        {
            LoadStudentInfo();
        }
        void LoadStudentInfo()
        {
            left_paner.Dispatcher.BeginInvoke(new Action(() =>
            {
                ks_img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "timg.jpg", UriKind.Absolute));//strImagePath 就绝对路径
                
            }));

        }
        void ActionTime()
        {
            DateTime fiveM = DateTime.Parse(string.Format("00:{0}:59", (SumTime - 1)));
            bool IsBreak = false;
            while (true)
            {
                ks_time.Dispatcher.BeginInvoke(new Action(() =>
                {
                    fiveM = fiveM.AddSeconds(-1);
                    ks_time.Text = string.Format("{0}分{1}秒", fiveM.Minute.ToString("00"), fiveM.Second.ToString("00"));
                    if (ks_time.Text == "00分00秒")
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
        void AddQuesBtn()
        {
           
            btn_idx.Dispatcher.BeginInvoke(new Action(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Button xx = new Button() { Width = 26, Height = 26, Content = (i + 1).ToString(), Background = new SolidColorBrush(color), Margin = new Thickness(6, 6, 10, 0), Style = btn_style };
                    btn_idx.Children.Add(xx);
                }
            }));

        }
        private void btn_idx_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 开始考试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            btn_start.Visibility = Visibility.Collapsed;
            time_paner.Visibility = Visibility.Visible;
            //先获取数据
            Post_ActionTime(ActionTime);//启动结束时间
            Post_IintData(AddQuesBtn);//启动题号
        }
    }
}
