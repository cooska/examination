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
    public partial class knowledgeadd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        void LoadData()
        {
            ModuleList = c_mian<c_knowledge>.Instans.GetModelList;
        }

        public DataTable ModuleList { get; set; }
    }
}