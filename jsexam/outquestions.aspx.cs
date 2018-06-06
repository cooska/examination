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
    public partial class outquestions : System.Web.UI.Page
    {
        public DataTable ModuleTb { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        void LoadData()
        {
            ModuleTb = c_mian<c_module>.Instans.GetModuleInfo();
        }
    }
}