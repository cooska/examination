using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
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
        #region 变量定义
        /// <summary>
        /// 是否跳出计时
        /// </summary>
        bool IsBreak = false;
        /// <summary>
        /// 考试总时间(分钟)
        /// </summary>
        public int SumTime = 60;
        /// <summary>
        /// 剩余分钟
        /// </summary>
        public int fz = 0;
        //剩余秒钟
        public int mz = 0;
        /// <summary>
        /// 按钮背景颜色
        /// </summary>
        Color color = (Color)ColorConverter.ConvertFromString("#1f4ba4");
        /// <summary>
        /// 当前试题按钮颜色
        /// </summary>
        Color Curt_color = (Color)ColorConverter.ConvertFromString("#00FFFF");

        /// <summary>
        /// 按钮初始化背景颜色
        /// </summary>
        Color Iintcolor = (Color)ColorConverter.ConvertFromString("#e0e0e0");

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
        /// 全局选项点击次数
        /// </summary>
        int click_ct = 0;
        /// <summary>
        /// 是否是新题目的选项集合
        /// </summary>
        bool IsNewChoseList = true;
        /// <summary>
        /// 是否开始考试
        /// </summary>
        bool IsStartExam = false;
        /// <summary>
        /// 考试时间
        /// </summary>
        DateTime ExamTime;
        /// <summary>
        /// 是否重新连接服务器
        /// </summary>
        bool IsReConServer = false;
        #endregion
        void Post_ActionTime(dlg_ActionTime hd)
        {
            hd.BeginInvoke(ActoinTimeCallBack, hd);
        }
        void ActoinTimeCallBack(IAsyncResult ars)
        {
            dlg_ActionTime hd = (dlg_ActionTime)ars.AsyncState;
            hd.EndInvoke(ars);
            //不是重新连接服务器状态提交试卷
            if (!IsReConServer)
            {
                SubMit();
            }
           
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
            IsReConServer = false;//默认不要重连
            Post_IintData(ConServer);
            btn_style = (Style)this.FindResource("BtnIcon");
        }
        void ConServer()
        {
            string[] ips = GetCfg.Split(':');
            ExTCP = new examTCP(ips[0], int.Parse(ips[1]));
            ExTCP.On_isConToServer += ExTCP_On_isConToServer;
            ExTCP.On_isGetUserInfo += ExTCP_On_isGetUserInfo;
            ExTCP.On_ReConServer += ExTCP_On_ReConServer;
            ExTCP.Connect();
        }
        /// <summary>
        /// 作答中与服务器断开连节再连接的处理
        /// </summary>
        private void ExTCP_On_ReConServer()
        {
            //先设置为要重连状态
            IsReConServer = true;
            //在跳出计时
            IsBreak = true;
            MessageBox.Show("与服务器连节断开,请监考老师重新打开程序,继续作答!");
            ResStartApp();//重新打开程序
        }

        string GetCfg
        {
            get
            {
                var item = ConfigurationManager.AppSettings["ip"];
                return item;
            }
        }
        private void ExTCP_On_isGetUserInfo(root Info)
        {
            if (Info.user_info != null)//用户信息不等于空就设置用户信息
            {
                SetUserInfo(Info.user_info);
                //如果本地文件不存在则问服务器获取考试试题
                if (!ExTCP.HasQuseFile)
                {
                  ExTCP.SendMsg("{\"model_type\":4,\"data\":\"ask\"}");
                }
                else
                {//如果本地考试文件存在就直接加载已保存试题信息
                    Info.question_list = new List<question_list>();
                    Info.question_list = ExTCP.GET_ques_list.question_list;
                    fz = ExTCP.GET_ques_list.fz;
                    mz = ExTCP.GET_ques_list.mz;
                    AddQuesBtn(Info.question_list);
                }
            }
            else//直接获取考试信息加载
            {
                fz = 0;
                mz = 0;
                AddQuesBtn(Info.question_list);
            }
        }
        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="userinfo"></param>
        void SetUserInfo(userinfo userinfo)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    byte[] arr = Convert.FromBase64String(userinfo.user_head_img);
                    ks_img.Source = LoadImage(arr);
                    ks_name.Text = userinfo.user_name;
                    ks_xb.Text = userinfo.user_sex;
                    ks_sfz.Text = userinfo.user_card;
                    ks_dw.Text = userinfo.user_work_str;
                    ks_sd.Text = userinfo.user_place_str;
                    ks_zkzh.Text = userinfo.exam_card;
                    ks_zwh.Text = userinfo.exam_card.Substring(userinfo.exam_card.Length - 2, 2);
                    if (!String.IsNullOrEmpty(userinfo.start_time))
                    {
                        ExamTime = DateTime.Parse(userinfo.start_time);//DateTime.Parse("2018-07-01 16:30:00");
                    }
                    this.Title = "湘西州专业技术人员公需科目考试作答系统V1.0 [正在获取试题信息...]";
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("考试信息获取失败,请监考老师重新打开程序获取考生信息!");
                ResStartApp();
            }

        }
        void ResStartApp()
        {
            GC.Collect();
            Environment.Exit(0);
        }
        public BitmapImage LoadImage(byte[] imageData)
        {
            var image = new BitmapImage();
            if (imageData == null || imageData.Length == 0) return null;
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
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Title = "湘西州专业技术人员公需科目考试作答系统V1.0 [正在连接考试服务器....]";
                }));
            }
            else
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Title = "湘西州专业技术人员公需科目考试作答系统V1.0 [已连接服务器,正获取考生信息...]";
                }));
            }
        }

        string SplitTime(double fz)
        {
            var t = new TimeSpan(0, 0, 0, (int)fz);
            return string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
        }
        void ActionTime()
        {
            DateTime fiveM = DateTime.Parse("01:00:01");
            var rstTime = ExamTime.Subtract(DateTime.Now);
            if (rstTime.TotalMinutes < 0&& rstTime.TotalMinutes>-SumTime)
            {
                int Minute = (int)DateTime.Now.Subtract(ExamTime).TotalMinutes;
                Minute = fz == 0 ? (SumTime - Minute) : fz;//计算剩余时间
                int Second = mz == 0 ? DateTime.Now.Second : mz;//计算剩余秒钟
                fiveM = DateTime.Parse(string.Format("00:{0}:{1}", Minute, Second));
            }
            while (true)
            {
                ks_time.Dispatcher.BeginInvoke(new Action(() =>
                {
                    fiveM = fiveM.AddSeconds(-1);
                    ks_time.Text = string.Format("{0}分{1}秒", fiveM.Minute.ToString("00"), fiveM.Second.ToString("00"));
                    //不退出时写入考试信息
                    if (!IsBreak)
                    {
                        WriteQuse((sbyte)fiveM.Minute, (sbyte)fiveM.Second);
                    }
                    if (fiveM.Hour == 0 && fiveM.Minute == 0 && fiveM.Second == 0)
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
        /// <summary>
        /// 写入考试信息
        /// </summary>
        void WriteQuse(sbyte fz, sbyte mz)
        {
            root rt = new root() { fz = fz, mz = mz, model_type = 2, question_list = QuseList };
            ExTCP.WriteQuse(rt);
        }
        void AddQuesBtn(List<question_list> list)
        {
            QuseList = list;
            SolidColorBrush cor = null;
            btn_idx.Dispatcher.BeginInvoke(new Action(() =>
            {
                sbyte i = 0;
                btn_idx.Children.Clear();
                foreach (var item in list)
                {
                    //如果题目已作答启动作答后的颜色
                    if (item.qlist.Find(s => s.anright == true) == null)
                    {
                        cor = new SolidColorBrush(Iintcolor);
                    }
                    else
                    {
                        cor = new SolidColorBrush(color);
                    }
                    Button xx = new Button() { Style = btn_style, Width = 26, Height = 26, Tag = string.Format("{0},{1}", item.id, i), Content = (i + 1).ToString(), Background = cor, Margin = new Thickness(6, 6, 10, 0) };
                    btn_idx.Children.Add(xx);
                    i++;
                }
                CurQueIdx = 0;//设置默认当前试题索引
                this.Title = "湘西州专业技术人员公需科目考试作答系统V1.0 [试题信息已获取]";
                btn_start.Visibility = Visibility.Visible;
                q_list.IsEnabled = false;
                btn_idx.IsEnabled = false;
                btn_up.IsEnabled = false;
                //WriteQuse(60,00);//写入试题信息
                SetQustion(list[0], 0, (sbyte)list.Count);
            }));
        }
        string[] qidx_arr = new string[] { "A、", "B、", "C、", "D、", "E、", "F、", "G、", "H、" };
        /// <summary>
        /// 设置题目
        /// </summary>
        /// <param name="item">题目集合</param>
        /// <param name="idx">题目索引</param>
        /// <param name="ct">题目总数</param>
        void SetQustion(question_list item, sbyte idx, sbyte ct)
        {
            q_title.Text = "";
            q_title.Inlines.Add(string.Format("{0}、", (idx + 1)));
            string lx = item.qtype == 1 ? "单选题" : (item.qtype == 2 ? "多选题" : "判断题");
            q_title.Inlines.Add(new Run(string.Format("({0}) ", lx)) { Foreground = new SolidColorBrush(color), FontWeight = FontWeights.Bold });
            item.qcontent = HttpUtility.HtmlDecode(item.qcontent).Replace("??", "\t")
            .Replace("?(", "(").Replace(")?", ")").Replace("(?", "(").Replace("?)", ")");//  item.qcontent.Replace("&nbsp;"," ").Replace("<br>","\r\n");
            q_title.Inlines.Add(item.qcontent);
            q_list.Children.Clear();//清除原有集合
            sbyte i = 0;
            foreach (var qitem in item.qlist)
            {
                QuestionItem qq = new QuestionItem() { Margin = new Thickness(0, 20, 0, 0) };
                qq.Q_ID = item.id;
                qq.Q_idx = qidx_arr[i];
                qq.Q_title = qitem.anwser.Replace("&nbsp;", " ");
                qq.Q_isCheack = qitem.anright;
                qq.ckTime = qitem.ckTime;
                qq.On_dlg_Cheacked += Qq_On_dlg_Cheacked;
                q_list.Children.Add(qq);
                i++;
            }
            if (idx == 0)
            {
                btn_up.Visibility = Visibility.Collapsed;
                if (IsStartExam)
                {
                    btn_dwom.Visibility = Visibility.Visible;
                }
            }
            else if (idx > 0 && idx < (ct - 1))
            {
                btn_up.Visibility = Visibility.Visible;
                btn_dwom.Visibility = Visibility.Visible;
            }
            else if (idx == (ct - 1))
            {
                btn_dwom.Visibility = Visibility.Collapsed;
                btn_up.Visibility = Visibility.Visible;
            }
            else if (idx < (ct - 1))
            {
                btn_dwom.Visibility = Visibility.Visible;
                btn_up.Visibility = Visibility.Collapsed;
            }

        }
        //作答
        private void Qq_On_dlg_Cheacked(int id)
        {
            click_ct++;
            //计算得分
            if (GetScore(QuseList[CurQueIdx]))
            {
                SetBtnsColor(CurQueIdx);//启用已作答试题Button按钮
            }
            else
            {
                SetBtnsColor(CurQueIdx, 0);//恢复未作答Button按钮
            }
        }

        private void btn_idx_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;
            string[] ids = btn.Tag.ToString().Split(',');
            CurQueIdx = sbyte.Parse(ids[1]);//索引赋值
            IsNewChoseList = true;//设置为有新试题加载
            SetQustion(QuseList[CurQueIdx], CurQueIdx, (sbyte)QuseList.Count);//对应试题跳转
            click_ct = 0;//点击清零
        }
        /// <summary>
        /// 开始考试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsStartExam = true;
            q_list.IsEnabled = true;
            btn_idx.IsEnabled = true;
            btn_dwom.IsEnabled = true;
            btn_up.IsEnabled = true;
            btn_start.Visibility = Visibility.Collapsed;
            time_paner.Visibility = Visibility.Visible;
            btn_dwom.Visibility = Visibility.Visible;
            ExTCP.isAnwser = true;//设置为开始作答状态
            Post_ActionTime(ActionTime);//启动考试结束时间
        }


        void SetBtnsColor(sbyte idx, sbyte type = 1)
        {
            for (int i = 0; i < btn_idx.Children.Count; i++)
            {
                if (i == idx)
                {
                    Button item = btn_idx.Children[i] as Button;
                    // var xx = item.Background;
                    if (type == 1)
                    {
                        item.Background = new SolidColorBrush(color);
                    }
                    else
                    {
                        item.Background = new SolidColorBrush(Iintcolor);
                    }
                    break;
                }
            }
        }
        private void btn_dwom_Click(object sender, RoutedEventArgs e)
        {
            CurQueIdx++;
            IsNewChoseList = true;//设置为有新试题加载
            SetQustion(QuseList[CurQueIdx], CurQueIdx, (sbyte)QuseList.Count);
            click_ct = 0;
        }

        private void btn_up_Click(object sender, RoutedEventArgs e)
        {
            CurQueIdx--;
            IsNewChoseList = true;
            SetQustion(QuseList[CurQueIdx], CurQueIdx, (sbyte)QuseList.Count);
            click_ct = 0;
        }


        /// <summary>
        /// 计算得分
        /// </summary>
        /// <param name="item"></param>
        bool GetScore(question_list item)
        {
            if (IsNewChoseList)
            {
                TempScorelist.Clear();
            }
            sbyte idx = 0;
            var xx = q_list.Children;
            foreach (QuestionItem qcItem in q_list.Children)
            {
                item.qlist[idx].anright = qcItem.Q_isCheack;
                item.qlist[idx].ckTime = qcItem.ckTime;
                if (IsNewChoseList)
                {
                    TempScorelist.Add(new Qlist() { anwser = qcItem.Q_title, anright = qcItem.Q_isCheack, ckTime = qcItem.ckTime });
                }
                else
                {
                    TempScorelist[idx] = new Qlist() { anwser = qcItem.Q_title, anright = qcItem.Q_isCheack, ckTime = qcItem.ckTime };
                }
                idx++;
            }
            //控制控件选中答案 单选题，判断题 必须单个选中
            if (item.qtype == 1 || item.qtype == 3)
            {
                var list = TempScorelist.FindAll(s => s.anright == true);
                list = list.OrderByDescending(s => s.ckTime).ToList();
                for (int i = 1; i < list.Count; i++)
                {
                    list[i].anright = false;
                    idx = 0;
                    foreach (QuestionItem qcItem in q_list.Children)
                    {
                        if (list[i].ckTime == qcItem.ckTime)
                        {
                            qcItem.Q_isCheack = false;
                            item.qlist[idx].anright = false;
                            break;
                        }
                        idx++;
                    }
                }
            }
            idx = 0;
            //计算得分 单选题和判断题
            if (item.qtype == 1 || item.qtype == 3)
            {
                var qitem = item.qlist.Find(s => s.isright.ToLower() == "true");
                var tqitem = TempScorelist.Find(s => s.anright == true);
                if (tqitem == null)
                {
                    item.score = 0;
                }
                else if (qitem.anwser == tqitem.anwser)//如果答对记1分
                {
                    item.score = 1;
                }
                else
                {
                    item.score = 0;
                }

            }
            else//多选题
            {
                var qitem = item.qlist.FindAll(s => s.isright.ToLower() == "true");
                var tqitem = TempScorelist.FindAll(s => s.anright == true);
                if (tqitem == null)
                {
                    item.score = 0;
                }
                else if (qitem.Count != tqitem.Count)
                {
                    item.score = 0;
                }
                else if (qitem.Count == tqitem.Count)
                {
                    idx = 0;
                    sbyte dx = 0;
                    foreach (var tt_item in qitem)
                    {
                        if (tt_item.anwser == tqitem[idx].anwser)//如果答对记1分
                        {
                            dx++;
                        }
                        idx++;
                    }
                    if (qitem.Count == dx)//多选必须每一道题答对才记一分
                    {
                        item.score = 1;
                    }
                    else
                    {
                        item.score = 0;
                    }
                }
            }
            IsNewChoseList = false;//加载后不是新题了
            if (TempScorelist.Find(s => s.anright == true) == null)
            {
                return false;
            }
            return true;
        }

        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("已经认真检查好,确定交卷！", "操作提示!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //跳出倒计时 自动提交考试成绩
                IsBreak = true;
            }
        }
        /// <summary>
        /// 提交试卷
        /// </summary>
        void SubMit()
        {
            ExTCP.DeQuseFile();
            sbyte SumNum = (sbyte)QuseList.FindAll(s => s.score == 1).Count;
            //提交得分
            ExTCP.SendMsg("{\"model_type\":3,\"score\":\"" + SumNum + "\"}");

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.IsEnabled = false;
            }));
        }

    }
}
