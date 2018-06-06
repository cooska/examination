using Dataport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace jsexam.control
{
    public class c_knowledge
    {
        public DataTable GetModelList
        {
            get {
                return DataCenter.Instans.SearchTb("SELECT a.id,a.module_name,(SELECT COUNT(b.id) FROM knowledge_info b WHERE b.mid = a.id) AS COUNT,(SELECT count(c.id) FROM knowledge_info c) as csum FROM `module_info` a ORDER BY COUNT DESC");
            }
        }
        public int AddKnowled(int mid,string mstr,string list)
        {
            List<knowledgeLine> KonwlngeList = DataCenter.Instans.getJsonObject<List<knowledgeLine>>(list);
            string sql = "";
            int rst = 0;
            foreach (var item in KonwlngeList)
            {
                sql += string.Format("INSERT INTO knowledge_info SET MID={0},mstr='{1}',content='{2}';\r\n",mid,mstr,item.content);
            }
            rst = DataCenter.Instans.ExecSql(sql);
            return rst;
        }
        public string GetKnowledList(int mid)
        {
            string sql = "";
            sql = string.Format("select id, mid, content from knowledge_info where mid = {0}",mid);
            sql = DataCenter.Instans.GetJsonData(sql);
            return sql;
        }
        public string GetKnowledList_Pro(int mid)
        {
            string sql = "";
            sql = string.Format("CALL getknglist_by_mid({0})", mid);
            sql = DataCenter.Instans.GetJsonData(sql);
            return sql;
        }


        public int deklgtion(int deid)
        {
            int rst = 0;
            if (HttpContext.Current.Session["usinfo"] != null)
            {
                string sql = string.Format("delete from knowledge_info where id={0}",
                deid
                );
                rst = DataCenter.Instans.ExecSql(sql);
                return rst;
            }
            else
            {
                HttpContext.Current.Response.Redirect("../index.aspx");
                return rst;
            }
        }
        public int UpKnowled(int upid, int mid, string mstr,string content)
        {
            int rst = 0;
            string sql = string.Format("update knowledge_info SET MID={0},mstr='{1}',content='{2}' where id={3};\r\n", mid, mstr, content,upid);
            rst = DataCenter.Instans.ExecSql(sql);
            return rst;
        }
    }
    public class knowledgeLine
    {
        public string content { get; set; }
    }
}