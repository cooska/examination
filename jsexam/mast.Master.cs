using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace jsexam
{
    public partial class mast : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["usinfo"]==null)
            {
                Response.Redirect("index.aspx");
                return;
            }
            SetMesg();
        }
        /// <summary>
        /// 设置一些必要的信息
        /// </summary>
        void SetMesg()
        {
            UsName = (Session["usinfo"] as DataRow)["user_name"].ToString();
        }

        public string UsName { get; set; }
       
    }
}