using Dataport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace jsexam.control
{
    public class c_exam
    {
        /// <summary>
        /// 获取代考信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetExamiInfo()
        {
            DataTable tb = null;
            string sql =string.Format(@"SELECT a.id,a.exami_name,a.exami_time,a.exami_num,a.exami_place,a.exami_hold,a.exami_module,
           (SELECT b.module_name FROM  module_info b WHERE b.id = a.exami_module) AS exami_module_str FROM `exam_layout` a
            WHERE a.`exami_time` >= '{0} 00:00:00'",DateTime.Now.ToShortDateString());
            tb = DataCenter.Instans.SearchTb(sql);
            return tb;
        }
    }
}