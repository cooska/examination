using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ExamTextServer
{
    /// <summary>
    /// QuestionItem.xaml 的交互逻辑
    /// </summary>
    public partial class QuestionItem : UserControl
    {
        public delegate void dlg_Cheacked(int id);
        public event dlg_Cheacked On_dlg_Cheacked;
        public QuestionItem()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 试题编号
        /// </summary>
        public int Q_ID { get; set; }
        /// <summary>
        /// 选型编号
        /// </summary>
        public sbyte Q_Idx { get; set; }
        string _Q_idx = "";
        public string Q_idx
        {
            get { return _Q_idx; }
            set { _Q_idx = value;
                q_idx.Text = value;
            }
        }
        public static readonly DependencyProperty C_QidxProperty =
       DependencyProperty.Register("Q_idx", typeof(string), typeof(QuestionItem), new PropertyMetadata(null));
        string _Q_title = "";
        public string Q_title
        {
            get { return _Q_title; }
            set { _Q_title = value;
                q_item.Text = value;
            }
        }
        public static readonly DependencyProperty C_QtitleProperty =
       DependencyProperty.Register("Q_title", typeof(string), typeof(QuestionItem), new PropertyMetadata(null));
        bool _Q_isCheack = false;
        public bool Q_isCheack
        {
            get { return (bool)q_ck.IsChecked; }
            set { _Q_isCheack = value;
                q_ck.IsChecked = value;
            }
        }
        public static readonly DependencyProperty C_Q_isCheackProperty =
       DependencyProperty.Register("Q_isCheack", typeof(bool), typeof(QuestionItem), new PropertyMetadata(null));

        private void q_ck_Click(object sender, RoutedEventArgs e)
        {
            if ( On_dlg_Cheacked != null )
            {
                On_dlg_Cheacked(Q_ID);
            }
        }
    }
}
