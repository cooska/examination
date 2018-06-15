using Dataport;
using jsexam.control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace jsexam
{
    public partial class action : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ActionType = Request.QueryString["actp"];
            string rst = "", p1 = "", p2 = "", p3 = "", p4 = "",p5="",p6="";
            switch (ActionType)
            {
                case "login"://用户登陆
                   rst = c_mian<c_login>.Instans.CheckLogin(Request.QueryString["name"],Request.QueryString["pass"]).ToString();
                    break;
                case "loginout":
                    rst = c_mian<c_login>.Instans.SigOut().ToString();
                    break;
                case "addques"://添加试题
                    p1 = Request.QueryString["qtype"];
                    p2 = Request.QueryString["tg"];
                    p2 = HttpUtility.UrlDecode(p2);
                    p2 = p2.Replace("'", "\\'");
                    p3 = Request.QueryString["list"];
                    p3 = HttpUtility.UrlDecode(p3);
                    p3 = p3.Replace("'", "\\'");
                    p4= Request.QueryString["mid"];
                    p5= Request.QueryString["kngid"];
                    rst = c_mian<c_qusetoin>.Instans.AddQusetion(sbyte.Parse(p1),p2,p3,int.Parse(p4),int.Parse(p5)).ToString();
                    break;
                case "upques"://修改试题
                    p1 = Request.QueryString["upid"];
                    p2 = Request.QueryString["qtype"];
                    p3 = Request.QueryString["tg"];
                    p3 = HttpUtility.UrlDecode(p3);
                    p3 = p3.Replace("'", "\\'");
                    p4 = Request.QueryString["list"];
                    p4 = HttpUtility.UrlDecode(p4);
                    p4 = p4.Replace("'", "\\'");
                    p5= Request.QueryString["mid"];
                    p6 = Request.QueryString["kng_id"];
                    rst = c_mian<c_qusetoin>.Instans.upQusetion(int.Parse(p1),sbyte.Parse(p2), p3, p4,p5,p6).ToString();
                    break;
                case "deques"://删除知识点
                    p1 = Request.QueryString["deid"];
                    rst = c_mian<c_qusetoin>.Instans.deQusetion(int.Parse(p1)).ToString();
                    break;
                case "addknowledge"://新增知识点
                    p1 = Request.QueryString["mid"];
                    p2 = Request.QueryString["mstr"];
                    p2 = HttpUtility.UrlDecode(p2);
                    p3 = Request.QueryString["list"];
                    p3 = HttpUtility.UrlDecode(p3);
                    p3 = p3.Replace("'", "\\'");
                    rst = c_mian<c_knowledge>.Instans.AddKnowled(int.Parse(p1),p2,p3).ToString();
                    break;
                case "deklg"://删除知识点
                    p1 = Request.QueryString["deid"];
                    rst = c_mian<c_knowledge>.Instans.deklgtion(int.Parse(p1)).ToString();
                    break;
                case "upklg"://修改知识点
                    p1 = Request.QueryString["upid"];
                    p2 = Request.QueryString["mid"];
                    p3 = Request.QueryString["mstr"];
                    p3 = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(p3,Encoding.UTF8));
                    p3 = p3.Replace("'", "\\'");
                    p4 = Request.QueryString["content"];
                    p4 = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(p4,Encoding.UTF8));
                    p4 = p4.Replace("'", "\\'");
                    rst = c_mian<c_knowledge>.Instans.UpKnowled(int.Parse(p1), sbyte.Parse(p2), p3, p4).ToString();
                    break;
                case "getknglist":
                    p1 = Request.QueryString["mid"];
                    rst = c_mian<c_knowledge>.Instans.GetKnowledList(int.Parse(p1));
                    break;
                case "getknglist_pro":
                    p1 = Request.QueryString["mid"];
                    rst = c_mian<c_knowledge>.Instans.GetKnowledList_Pro(int.Parse(p1));
                    break;
                case "upmdlist":
                    p1 = Request.QueryString["upid"];
                    p2 = Request.QueryString["nkg_list"];
                    p2 = HttpUtility.UrlDecode(p2);
                    p2 = p2.Replace("'", "\\'");
                   // p2 = p2.Replace("\"","\"\"");
                    rst = c_mian<c_module>.Instans.UpKng_list(int.Parse(p1),p2).ToString();
                    break;
            }
            Response.Write(rst);
        }
    }
}