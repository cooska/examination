using Dataport;
using jsexam.control;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace jsexam
{
    public partial class questionlist : System.Web.UI.Page
    {
        public int SumCount = 0;
        /// <summary>
        /// 每页显示条数
        /// </summary>
        sbyte EvPage = 10;
        public DataTable tb { get; set; }
        public DataTable ModuleList { get; set;}
        public DataTable QuseTypeList { get; set; }
        public string fy { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        string GetCondtion()
        {
            var req = Request;
            string qtype = string.IsNullOrEmpty(req.QueryString["tp"]) == true ? "" : string.Format(" and qtype = {0}", req.QueryString["tp"]);
            string qcontent = string.IsNullOrEmpty(req.QueryString["tg"]) == true ? "" : string.Format(" and qcontent like '%{0}%'", HttpUtility.UrlDecode(HttpUtility.HtmlDecode(req.QueryString["tg"]), Encoding.UTF8));
            string mid = string.IsNullOrEmpty(req.QueryString["mid"]) == true ? "" : string.Format(" and md_id = {0}", req.QueryString["mid"]);
            string kngid = string.IsNullOrEmpty(req.QueryString["kng_id"]) == true ? "" : string.Format(" and kng_id = {0}", req.QueryString["kng_id"]);
            EvPage = string.IsNullOrEmpty(req.QueryString["dtct"]) == true ? (sbyte)10 : sbyte.Parse(req.QueryString["dtct"]);
            string tj = qtype + qcontent+mid+kngid;
            tj = tj == "" ? "" : "where" + tj.Substring(tj.IndexOf('a') + 3);
            return tj;
        }
        void LoadData()
        {
            ModuleList = c_mian<c_knowledge>.Instans.GetModelList;//获取模块信息
            QuseTypeList = DataCenter.Instans.SearchTb("SELECT a.qtype,COUNT(id) AS ct,(SELECT COUNT(c.id) FROM question_info c) AS csum FROM question_info a GROUP BY a.`qtype`");//获取试题类型集合
            int pgidx = string.IsNullOrEmpty(Request.QueryString["page"]) == true ? 1 : int.Parse(Request.QueryString["page"]);
            //函数调用顺序不要变
            string condtion = GetCondtion();
            int Rang = (pgidx - 1) * EvPage;
            string limit = string.Format("limit {0},{1}", Rang, EvPage); 
            //string sql = string.Format("select a.*,(select b.module_name from module_info b where a.md_id = b.id) as md_str from question_info a INNER JOIN (select id from question_info {0} order by id desc {1}) b on a.id = b.id", condtion, limit);
            string sql = string.Format("select a.* from question_info a INNER JOIN (select id from question_info {0} order by id desc {1}) b on a.id = b.id", condtion, limit);
            tb = c_mian<c_qusetoin>.Instans.GetAllQusetion(sql);
            string url = Request.Url.PathAndQuery;
            url = url.Substring(1);
            fy = c_glob.Instans.LoadSpliPage(EvPage, "id", "question_info", url, pgidx, condtion,ref SumCount);
        } 
    }
}