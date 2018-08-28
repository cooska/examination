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
    public partial class knowledgepoint : System.Web.UI.Page
    {
        #region 变量定义
        public int SumCount = 0;
        public string fy { get; set; }
        public DataTable tb { get; set; }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        sbyte EvPage = 10;
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        string GetCondtion()
        {
            var req = Request;
            string qtype = string.IsNullOrEmpty(req.QueryString["mid"]) == true ? "" : string.Format(" and mid = {0}", req.QueryString["mid"]);
            string qcontent = string.IsNullOrEmpty(req.QueryString["zsd"]) == true ? "" : string.Format(" and content like '%{0}%'", HttpUtility.UrlDecode(HttpUtility.HtmlDecode(req.QueryString["zsd"]), Encoding.UTF8));
            EvPage = string.IsNullOrEmpty(req.QueryString["dtct"]) == true ? (sbyte)10 : sbyte.Parse(req.QueryString["dtct"]);
            string tj = qtype + qcontent;
            tj = tj == "" ? "" : "where" + tj.Substring(tj.IndexOf('a') + 3);
            return tj;
        }
        void LoadData()
        {
            ModuleList = c_mian<c_knowledge>.Instans.GetModelList;
            int pgidx = string.IsNullOrEmpty(Request.QueryString["page"]) == true ? 1 : int.Parse(Request.QueryString["page"]);
            //函数调用顺序不要改变
            string condtion = GetCondtion();
            int Rang = (pgidx - 1) * EvPage;
            string limit = string.Format("limit {0},{1}", Rang, EvPage);
            string sql = string.Format("select a.id,a.mid,a.mstr,a.content from knowledge_info a INNER JOIN (select id from knowledge_info {0} order by id desc {1}) b on a.id = b.id", condtion, limit);
            tb = c_mian<c_qusetoin>.Instans.GetAllQusetion(sql);
            string url = Request.Url.PathAndQuery;
            url = url.Substring(1);
            fy = c_glob.Instans.LoadSpliPage(EvPage, "id", "knowledge_info", url, pgidx, condtion,ref SumCount);
        }
        public DataTable ModuleList { get; set; }
    }
}