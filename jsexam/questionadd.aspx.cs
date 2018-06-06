using jsexam.control;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace jsexam
{
    public partial class questionadd : System.Web.UI.Page
    {
        public DataTable ModuleList { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            ModuleList = c_mian<c_knowledge>.Instans.GetModelList;
        }

    }
}