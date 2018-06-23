using System;
using System.Collections.Generic;
using System.IO;
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
        examTCP ExTCP = null;
        /// <summary>
        /// 接收存储试题（网络/xml）
        /// </summary>
        List<question_list> QuseList = null;
        /// <summary>
        /// 当前试题索引
        /// </summary>
        sbyte CurQueIdx = 0;
        /// <summary>
        /// 计算分数用
        /// </summary>
        List<Qlist> TempScorelist = new List<Qlist>();
        /// <summary>
        /// 考试得分
        /// </summary>
        sbyte SumScore = 0;
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
        }
        void ConServer()
        {
            ExTCP = new examTCP("192.168.131.22", 11118);
            ExTCP.On_isConToServer += ExTCP_On_isConToServer;
            ExTCP.On_isGetUserInfo += ExTCP_On_isGetUserInfo;
            ExTCP.Connect();
        }

        private void ExTCP_On_isGetUserInfo(root Info)
        {
            if (Info.user_info != null)//用户信息不等于设置用户信息
            {
                SetUserInfo(Info.user_info);
            }
            else//设置考试信息
            {
                AddQuesBtn(Info.question_list);
            }
        }
        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="userinfo"></param>
        void SetUserInfo(userinfo userinfo)
        {
            this.Dispatcher.BeginInvoke(new Action(() => {
                byte[] arr = Convert.FromBase64String(userinfo.user_head_img);
                ks_img.Source = LoadImage(arr);
                ks_name.Text = userinfo.user_name;
                ks_xb.Text = userinfo.user_sex;
                ks_sfz.Text = userinfo.user_card;
                ks_dw.Text = userinfo.user_work_str;
                ks_sd.Text = userinfo.user_place_str;
                btn_start.IsEnabled = true;
                this.Title = "湘西州专业技术人员公需科目考试作答系统V1.0 [已成功连接考试服务器]";
            }));
        }
        public BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void ExTCP_On_isConToServer(bool YesOrNo)
        {
            if (!YesOrNo)
            {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    this.Title = "湘西州专业技术人员公需科目考试作答系统V1.0 [正在连接考试服务器....]";
                    btn_start.IsEnabled = false;
                }));
            }
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
        void AddQuesBtn(List<question_list> list)
        {
            QuseList = list;
            btn_idx.Dispatcher.BeginInvoke(new Action(() =>
            {
                sbyte i = 0;
                foreach (var item in list)
                {
                    Button xx = new Button() { Style = btn_style, IsEnabled = false, Width = 26, Height = 26, Tag=string.Format("{0},{1}", item.id,i),Content = (i + 1).ToString(), Background = new SolidColorBrush(color), Margin = new Thickness(6, 6, 10, 0) };
                    btn_idx.Children.Add(xx);
                    i++;
                }
                btn_start.Visibility = Visibility.Collapsed;
                time_paner.Visibility = Visibility.Visible;
                btn_dwom.Visibility = Visibility.Visible;
                btn_up.Visibility = Visibility.Visible;
                CurQueIdx = 0;//设置默认当前试题索引
                SetQustion(list[0], 0, (sbyte)list.Count);
                Post_ActionTime(ActionTime);//启动考试结束时间
            }));
        }
        string[] qidx_arr = new string[] { "A、", "B、", "C、", "D、", "E、", "F、", "G、", "H、" };
        /// <summary>
        /// 设置题目
        /// </summary>
        /// <param name="item">题目集合</param>
        /// <param name="idx">题目索引</param>
        /// <param name="ct">题目总数</param>
        void SetQustion(question_list item,sbyte idx,sbyte ct)
        {
            q_title.Text = "";
            q_title.Inlines.Add(string.Format("{0}、", (idx+1)));
            string lx = item.qtype == 1?"单选题" :(item.qtype == 2?"多选题":"判断题");
            q_title.Inlines.Add(new Run(string.Format("({0})", lx)) { Foreground = new SolidColorBrush(color), FontWeight = FontWeights.Bold });
            q_title.Inlines.Add(item.qcontent);
            q_list.Children.Clear();//清除原有集合
            sbyte i = 0;
            foreach (var qitem in item.qlist)
            {
                QuestionItem qq = new QuestionItem() { Margin = new Thickness(0,20,0,0) };
                qq.Q_idx = qidx_arr[i];
                qq.Q_title = qitem.anwser;
                q_list.Children.Add(qq);
                i++;
            }
            if (idx == 0)
            {
                btn_up.Visibility = Visibility.Collapsed;
            }
            else if (idx > 0)
            { btn_up.Visibility = Visibility.Visible; }
            else if (idx == (ct-1))
            {
                btn_dwom.Visibility = Visibility.Collapsed;
            }
            else if(idx<(ct-1)) {
                btn_dwom.Visibility = Visibility.Visible;
            }

        }
        private void btn_idx_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;
            string[] ids = btn.Tag.ToString().Split(',');
            GetScore(QuseList[CurQueIdx]);//先计算当前得分
            var item = QuseList.Find(s => s.id == int.Parse(ids[0]));
            CurQueIdx = sbyte.Parse(ids[1]);//索引赋值
            SetQustion(QuseList[CurQueIdx], CurQueIdx, (sbyte)QuseList.Count);//对应试题跳转

        }
        /// <summary>
        /// 开始考试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExTCP.SendMsg("ask");
        }

        private void btn_dwom_Click(object sender, RoutedEventArgs e)
        {
            GetScore(QuseList[CurQueIdx]);//先计算分数
            CurQueIdx++;
            SetQustion(QuseList[CurQueIdx], CurQueIdx,(sbyte)QuseList.Count);
        }
        
        private void btn_up_Click(object sender, RoutedEventArgs e)
        {
            GetScore(QuseList[CurQueIdx]);//先计算分数
            CurQueIdx--;
            SetQustion(QuseList[CurQueIdx], CurQueIdx, (sbyte)QuseList.Count);
        }
        
        /// <summary>
        /// 计算得分
        /// </summary>
        /// <param name="item"></param>
        void GetScore(question_list item)
        {
            TempScorelist.Clear();
            foreach (var c in q_list.Children)
            {
                QuestionItem qc = (QuestionItem)c;
                TempScorelist.Add(new Qlist() { anwser = qc.Q_title, isright = qc.Q_isCheack.ToString() });
            }
            bool rst = item.qlist.Equals(TempScorelist);
            if (rst)
            {
                SumScore++;
            }
        }
    }
}
