using Dataport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace jsexam.control
{
    public class c_qusetoin
    {
        /// <summary>
        /// 新增试题
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="Anwserlist"></param>
        /// <returns></returns>
        public int AddQusetion(sbyte type, string content, string Anwserlist,int mid,int kngid)
        {
            int rst = 0;
            if (HttpContext.Current.Session["usinfo"] != null)
            {
                string sql = string.Format("insert into question_info set qtype={0},qcontent='{1}',userid={2},qlist='{3}',addtime='{4}',md_id={5},kng_id={6}",
                type,
                content,
                uslogin.UserRow["id"].ToString(),
                Anwserlist,
                DateTime.Now.ToString(),
                mid,
                kngid
                );
                rst = DataCenter.Instans.ExecSql(sql);
                return rst;
            }
            else {
                HttpContext.Current.Response.Redirect("../index.aspx");
                return rst;
            }
        }
        /// <summary>
        /// 修改试题
        /// </summary>
        /// <param name="upid"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="Anwserlist"></param>
        /// <returns></returns>
        public int upQusetion(int upid,sbyte type, string content, string Anwserlist,string mid,string kng_id)
        {
            int rst = 0;
            if (HttpContext.Current.Session["usinfo"] != null)
            {
                string sql = string.Format("update question_info set qtype={0},qcontent='{1}',userid={2},qlist='{3}',addtime='{4}',md_id={5},kng_id={6} where id={7}",
                type,
                content,
                uslogin.UserRow["id"].ToString(),
                Anwserlist,
                DateTime.Now.ToString(),
                mid,
                kng_id,
                upid
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
        public int deQusetion(int deid)
        {
            int rst = 0;
            if (HttpContext.Current.Session["usinfo"] != null)
            {
                string sql = string.Format("delete from question_info where id={0}",
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
        /// <summary>
        /// 获取所有试题集合
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllQusetion(string sql)
        {
            DataTable tb = null;
            tb = DataCenter.Instans.SearchTb(sql);
            return tb;
        }
    }
}