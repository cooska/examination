using Dataport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace jsexam.control
{
    public class c_results
    {
        /// <summary>
        /// 获取所有考试时间
        /// </summary>
        public static List<int> GetHaveDate
        {
            get
            {
                List<int> arr = new List<int>();
                string sql = "select DISTINCT(DATE_FORMAT(start_date, '%Y' )) start_date from exami_info order by start_date ASC";
                DataTable tb = DataCenter.Instans.SearchTb(sql);
                if (tb.Rows.Count==0)
                {
                    return new List<int>() { DateTime.Now.Year};
                }
                int MinTime = int.Parse(tb.Rows[0][0].ToString());
                int NowTime = DateTime.Now.Year;
                int rst = NowTime - MinTime;
                if (rst==0)
                {
                    arr.Add(MinTime);
                    return arr;
                }
                arr.Add(MinTime);
                for (int i = 1; i < tb.Rows.Count; i++)
                {
                    arr.Add((arr[i - 1] + 1));
                }
                return arr;
            }
        }
        /// <summary>
        /// 获取所有单位信息
        /// </summary>
        public static List<Key_Val> GetWorks
        {
            get
            {
                List<Key_Val> list = new List<Key_Val>();
                string sql = "select * from work_info";
                DataTable tb = DataCenter.Instans.SearchTb(sql);
                foreach (DataRow rw in tb.Rows)
                {
                    list.Add(new Key_Val() { key = int.Parse(rw["id"].ToString()), val = rw["work_name"].ToString() });
                }
                tb.Dispose();
                return list;
            }
        }
    }
}