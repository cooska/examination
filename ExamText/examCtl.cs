#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：ExamTextServer
* 项目描述 ：
* 类 名 称 ：examCtl
* 类 描 述 ：
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Administrator
* 创建时间 ：2018/6/21 10:09:01
* 更新时间 ：2018/6/21 10:09:01
*******************************************************************
* Copyright @ 湖南教育出版社-贝壳网. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExamTextServer
{
    public class examCtl
    {
      
    }
    public class root
    {
        /// <summary>
        /// 考试剩余时间(分钟)
        /// </summary>
        public sbyte fz { get; set; }
        /// <summary>
        /// 考试剩余时间(秒钟)
        /// </summary>
        public sbyte mz { get; set; }
        public sbyte model_type { get; set; }
        public userinfo user_info { get; set; }
        public List<question_list> question_list { get; set; }
        /// <summary>
        /// 考试总分数
        /// </summary>
        public sbyte score { get; set; }

    }
    public class userinfo
    {
        public int id { get; set; }
        public string user_name { get; set; }
        public string user_card { get; set; }
        public string user_head_img { get; set; }
        public string user_work_str { get; set; }
        public string user_place_str { get; set; }
        public string start_time { get; set; }
        public string user_phone { get; set; }
        public string user_sex { get; set; }
        public string exam_card { get; set; }
    }
    public class Qlist
    {
        /// <summary>
        /// 123
        /// </summary>
        public string anwser { get; set; }
        /// <summary>
        /// true
        /// </summary>
        public string isright { get; set; }
        bool _anright = false;
        /// <summary>
        /// 回答
        /// </summary>
        public bool anright { get { return _anright; } set { _anright = value; } }
        public int ckTime { get; set; }
    }
    
    public class question_list
    {
        /// <summary>
        /// 索引
        /// </summary>
       public sbyte Idx { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Qtype
        /// </summary>
        public int qtype { get; set; }
        /// <summary>
        /// 简单的小情歌
        /// </summary>
        public string qcontent { get; set; }
        /// <summary>
        /// Userid
        /// </summary>
        public int userid { get; set; }
        /// <summary>
        /// Qlist
        /// </summary>
        public List<Qlist> qlist { get; set; }
        /// <summary>
        /// 2018-06-14T17:51:19
        /// </summary>
        public string addtime { get; set; }
        /// <summary>
        /// Md_id
        /// </summary>
        public int md_id { get; set; }
        /// <summary>
        /// Kng_id
        /// </summary>
        public int kng_id { get; set; }
        /// <summary>
        /// 单个试题得分
        /// </summary>
        public sbyte score { get; set; }
    }
}
