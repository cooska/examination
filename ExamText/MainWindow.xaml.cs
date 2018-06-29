using System;
using System.Collections.Generic;
using System.Configuration;
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
        #region 变量定义
        /// <summary>
        /// 考试总时间(分钟)
        /// </summary>
        public sbyte SumTime = 60;
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
        #endregion
        void Post_ActionTime(dlg_ActionTime hd)
        {
            hd.BeginInvoke(ActoinTimeCallBack, hd);
        }
        void ActoinTimeCallBack(IAsyncResult ars)
        {
            dlg_ActionTime hd = (dlg_ActionTime)ars.AsyncState;
            hd.EndInvoke(ars);
            SubMit();
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
            string[] ips = GetCfg.Split(':');
            ExTCP = new examTCP(ips[0],int.Parse(ips[1]));
            ExTCP.On_isConToServer += ExTCP_On_isConToServer;
            ExTCP.On_isGetUserInfo += ExTCP_On_isGetUserInfo;
            ExTCP.Connect();
        }
        string GetCfg
        {
            get {
                var item = ConfigurationManager.AppSettings["ip"];
                return item;
            }
        }
        private void ExTCP_On_isGetUserInfo(root Info)
        {
            if (Info.user_info != null)//用户信息不等于设置用户信息
            {
                SetUserInfo(Info.user_info);
                //获取完用户信息马上获取考试信息
                ExTCP.SendMsg("{\"model_type\":4,\"data\":\"ask\"}");
            }
            else//设置考试信息
            {
                //if (ExTCP.HasQuseFile)
                //{

                //}
                //else {
                   AddQuesBtn(Info.question_list);
                //}
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
                ks_zkzh.Text = userinfo.exam_card;
                ks_zwh.Text = userinfo.exam_card.Substring(userinfo.exam_card.Length-2,2);
                this.Title = "湘西州专业技术人员公需科目考试作答系统V1.0 [正在获取试题信息...]";
            }));
        }
        public BitmapImage LoadImage(byte[] imageData)
        {
            var image = new BitmapImage();
            try {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("链接失败,请重新打开程序!");
                GC.Collect();
                Environment.Exit(0);
                return null;
            }
            return image;
        }

        private void ExTCP_On_isConToServer(bool YesOrNo)
        {
            if (!YesOrNo)
            {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    this.Title = "湘西州专业技术人员公需科目考试作答系统V1.0 [正在连接考试服务器....]";
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
            bool IsBreak = false;
            while (true)
            {
                ks_time.Dispatcher.BeginInvoke(new Action(() =>
                {
                    fiveM = fiveM.AddSeconds(-1);
                    ks_time.Text = string.Format("{0}分{1}秒", fiveM.Minute.ToString("00"), fiveM.Second.ToString("00"));
                    WriteQuse((sbyte)fiveM.Minute,(sbyte)fiveM.Second);
                    if (fiveM.Hour ==0 && fiveM.Minute==0 && fiveM.Second ==0)
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
        void WriteQuse(sbyte fz,sbyte mz)
        {
           root rt = new root() { fz = fz,mz = mz, model_type= 2, score = 0, question_list = QuseList };
           ExTCP.WriteQuse(rt);
        }
        void AddQuesBtn(List<question_list> list)
        {
            QuseList = list;
            btn_idx.Dispatcher.BeginInvoke(new Action(() =>
            {
                sbyte i = 0;
                btn_idx.Children.Clear();
                foreach (var item in list)
                {
                    Button xx = new Button() { Style = btn_style, Width = 26, Height = 26, Tag=string.Format("{0},{1}", item.id,i),Content = (i + 1).ToString(), Background = new SolidColorBrush(Iintcolor), Margin = new Thickness(6, 6, 10, 0) };
                    btn_idx.Children.Add(xx);
                    i++;
                }
                CurQueIdx = 0;//设置默认当前试题索引
                this.Title = "湘西州专业技术人员公需科目考试作答系统V1.0 [试题信息已获取]";
                btn_start.Visibility = Visibility.Visible;
                q_list.IsEnabled = false;
                btn_idx.IsEnabled = false;
                btn_up.IsEnabled = false;
                WriteQuse(60,00);//写入试题信息
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
        void SetQustion(question_list item,sbyte idx,sbyte ct)
        {
            q_title.Text = "";
            q_title.Inlines.Add(string.Format("{0}、", (idx+1)));
            string lx = item.qtype == 1?"单选题" :(item.qtype == 2?"多选题":"判断题");
            q_title.Inlines.Add(new Run(string.Format("({0}) ", lx)) { Foreground = new SolidColorBrush(color), FontWeight = FontWeights.Bold });
            item.qcontent = item.qcontent.Replace("&nbsp;"," ").Replace("<br>","\r\n");
            q_title.Inlines.Add(item.qcontent);
            q_list.Children.Clear();//清除原有集合
            sbyte i = 0;
            foreach (var qitem in item.qlist)
            {
                QuestionItem qq = new QuestionItem() { Margin = new Thickness(0,20,0,0) };
                qq.Q_ID = item.id;
                qq.Q_idx = qidx_arr[i];
                qq.Q_title = qitem.anwser.Replace("&nbsp;"," ");
                qq.Q_isCheack = qitem.anright;
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
            else if (idx > 0 && idx<(ct-1))
            { btn_up.Visibility = Visibility.Visible;
                btn_dwom.Visibility = Visibility.Visible;
            }
            else if (idx == (ct-1))
            {
                btn_dwom.Visibility = Visibility.Collapsed;
                btn_up.Visibility = Visibility.Visible;
            }
            else if(idx<(ct-1)) {
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
            Post_ActionTime(ActionTime);//启动考试结束时间
            IsStartExam = true;
            q_list.IsEnabled = true;
            btn_idx.IsEnabled = true;
            btn_dwom.IsEnabled = true;
            btn_up.IsEnabled = true;
            btn_start.Visibility = Visibility.Collapsed;
            time_paner.Visibility = Visibility.Visible;
            btn_dwom.Visibility = Visibility.Visible;

          
        }

      
        void SetBtnsColor(sbyte idx)
        {
            for (int i = 0; i < btn_idx.Children.Count; i++)
            {
                if (i==idx)
                {
                    Button item = btn_idx.Children[i] as Button;
                   // var xx = item.Background;
                    item.Background = new SolidColorBrush(color);
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
                if (IsNewChoseList)
                {
                    TempScorelist.Add(new Qlist() { anwser = qcItem.Q_title, anright = qcItem.Q_isCheack, ckTime = qcItem.ckTime });
                }
                else {
                    TempScorelist[idx] = new Qlist() { anwser = qcItem.Q_title, anright = qcItem.Q_isCheack, ckTime = qcItem.ckTime };
                }
                idx++;
            }
            //控制控件选中答案 单选题，判断题 必须单个选中
            if ( item.qtype == 1 || item.qtype == 3 )
            {
                var list = TempScorelist.FindAll(s => s.anright == true);
                list = list.OrderByDescending(s => s.ckTime).ToList();
                for (int i = 1; i < list.Count; i++)
                {
                    list[i].anright = false;
                    foreach (QuestionItem qcItem in q_list.Children)
                    {
                        if (list[i].ckTime == qcItem.ckTime)
                        {
                            qcItem.Q_isCheack = false;
                            break;
                        }
                    }
                }
            }
            idx = 0;
            //计算得分 单选题和判断题
            if (item.qtype == 1 || item.qtype == 3)
            {
                var qitem = item.qlist.Find(s =>s.isright.ToLower() == "true");
                var tqitem = TempScorelist.Find(s=>s.anright==true);
                if (tqitem==null)
                {
                    item.score = 0;
                }
                else if (bool.Parse(qitem.isright) == tqitem.anright)//如果答对记1分
                {
                    item.score = 1;
                }
                else {
                   
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
                else if(qitem.Count==tqitem.Count) {
                    idx = 0;
                    sbyte dx = 0;
                    foreach (var tt_item in qitem)
                    {
                        if (bool.Parse(tt_item.isright) == tqitem[idx].anright)//如果答对记1分
                        {
                            tt_item.anright = true;
                            dx++;
                        }
                        idx++;
                    }
                    if (qitem.Count == dx)//多选必须每一道题答对才记一分
                    {
                        item.score = 1;
                    }
                    else {
                        item.score = 0;
                    }
                }
            }

       
            IsNewChoseList = false;//加载后不是新题了
            return true;
        }
    
        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("已经认真检查好,确定交卷！","操作提示!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SubMit();
            }
        }
        /// <summary>
        /// 提交试卷
        /// </summary>
        void SubMit()
        {
            ExTCP.DeQuseFile();
            MessageBox.Show("家里了");
            this.IsEnabled = false;
        }
        
    }
}
