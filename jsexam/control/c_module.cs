using Dataport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace jsexam.control
{
    public class c_module
    {
        /// <summary>
        /// 获取模块信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetModuleInfo()
        {
            DataTable tb = null;
            string sql = "SELECT a.id,a.module_name,a.kng_list,(SELECT COUNT(b.id) FROM knowledge_info b WHERE b.mid = a.id) AS COUNT,(SELECT COUNT(c.id) FROM knowledge_info c) AS csum FROM `module_info` a ORDER BY COUNT DESC";
            tb = DataCenter.Instans.SearchTb(sql);
            return tb;
        }
        public int UpKng_list(int id,string kng_list)
        {
            string sql = string.Format("update module_info set kng_list='{0}' where id = {1}",kng_list,id);
            return DataCenter.Instans.ExecSql(sql);
        }
    }
}