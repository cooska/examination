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
    public partial class Results : System.Web.UI.Page
    {
        #region 变量定义
        public int SumCount = 0;
        const sbyte EvPage = 10;
        public string ExcelPath = AppDomain.CurrentDomain.BaseDirectory + "template\\kscj.xls";

        delegate void dlg_Export();
        void Post_Export(dlg_Export hd)
        {
            hd.BeginInvoke(Post_ExportCallback,hd);
        }
        void Post_ExportCallback(IAsyncResult rst)
        {
            dlg_Export hd = (dlg_Export)rst.AsyncState;
            hd.EndInvoke(rst);
        }
        public string fy { get; set; }
        public DataTable tb { get; set; }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["export"]))//执行导出
            {
                ExpoertExcel();
            }
            else {
                LoadData();
            }
        }
        string GetCondtion()
        {
            var req = Request;
            string qmodule = string.IsNullOrEmpty(req.QueryString["exami_module"]) == true ? "" : string.Format(" and exami_module = {0}", req.QueryString["exami_module"]);
            string qwork = string.IsNullOrEmpty(req.QueryString["work_id"]) == true ? "" : string.Format(" and work_id = {0}", req.QueryString["work_id"]);
            string qdate = string.IsNullOrEmpty(req.QueryString["start_date"]) == true ? "" : req.QueryString["start_date"];
            string kssj = string.Format("{0}-01-01 00:00:00", qdate);
            string jssj = string.Format("{0}-12-31 23:59:59", qdate);
            qdate = qdate == "" ? "" : string.Format("and start_date >= '{0}' and start_date<='{1}'", kssj, jssj);
            string qsfz = string.IsNullOrEmpty(req.QueryString["user_card"]) == true ? "" : string.Format(" and user_card like '{0}%'", req.QueryString["user_card"]);
            string tj = qmodule + qwork + qdate + qsfz;
            tj = tj == "" ? "" : "where" + tj.Substring(tj.IndexOf('a') + 3);
            return tj;
        }
        void LoadData()
        {
            ModuleList = c_mian<c_knowledge>.Instans.GetModelList;
            HaveTime = c_results.GetHaveDate;
            Works = c_results.GetWorks;
            int pgidx = string.IsNullOrEmpty(Request.QueryString["page"]) == true ? 1 : int.Parse(Request.QueryString["page"]);
            int Rang = (pgidx - 1) * EvPage;
            string limit = string.Format("limit {0},{1}", Rang, EvPage);
            string condtion = GetCondtion();
            string sql = string.Format(@"select a.id,
            (select module_name from module_info where id = (select exami_module from exam_layout where id = a.layout_id)) as module_name,
            (select user_name from user_info where id =a.user_id ) as user_name,
            (select user_sex from user_info where id = a.user_id) as user_sex,
            a.user_card,
            (select work_name from work_info where id = a.work_id) as work_name,
            (select exami_name from exam_layout where id = a.layout_id ) as exami_name,
            a.score from exami_info a {0} {1}", condtion, limit);
            tb = c_mian<c_qusetoin>.Instans.GetAllQusetion(sql);
            string url = Request.Url.PathAndQuery;
            url = url.Substring(1);
            fy = c_glob.Instans.LoadSpliPage(EvPage, "id", "exami_info", url, pgidx, condtion, ref SumCount);
         }
        void ExpoertExcel()
        {
            //Response.Write("<script>CallWriteExportStat('12');</script>");
            //Response.Write("<script>CallWriteExportStat(12,35)</script>");
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"xx","alert('12');");
           // return;
            string condtion = GetCondtion();
            string sql = @"select
            (select module_name from module_info where id = (select exami_module from exam_layout where id = a.layout_id)) as module_name,
            (select user_name from user_info where id =a.user_id ) as user_name,
            (select user_sex from user_info where id = a.user_id) as user_sex,
            a.user_card,
            (select work_name from work_info where id = a.work_id) as work_name,
            (select exami_name from exam_layout where id = a.layout_id ) as exami_name,
            a.score from exami_info a";
            //计算统计总数，总分页数
            DataTable Art_Table = DataCenter.Instans.SearchTb(string.Format("select count(id) as ct from exami_info {0}",condtion));
            int Datacount = int.Parse(Art_Table.Rows[0]["ct"].ToString());
            double dx = ((double)Datacount / EvPage);
            int SumPage = (int)Math.Ceiling(dx);//总页数
            //组合导出条件
            var item = new ExportContion() { sql= sql, condtion = condtion, DataCount = Datacount, SumPage = SumPage,EvePage = EvPage };
            DataToExcel.ExportExcelTemplate(item,ExcelPath,"公考成绩查询统计表", 3,1,"","","");
        }

        private void Export_Evnet_ShowExportPercent(int ct, int SumCt)
        {
            //Response.Write("<script>parent.CallWriteExportStat("+ct+","+SumCt+")</script>");
        }

        public DataTable ModuleList { get; set; }
        public List<int> HaveTime { get; set; }
        public List<Key_Val> Works { get; set; }
    }
}