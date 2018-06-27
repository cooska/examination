using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Dataport
{
    public class c_glob
    {
        private static c_glob _instans = null;
        public static c_glob Instans
        {
            get {
                return _instans == null ? _instans = new c_glob() : _instans;
            }
        }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public const int EVERY_PAGE = 20;
        public string CurentPath
        {
            get
            {
                string url = HttpContext.Current.Request.Url.Host;
                string port = HttpContext.Current.Request.Url.Port.ToString();
                url += port == "80" ? "" : ":" + port;
                url = "http://" + url;
                if (url.IndexOf("443") > 0)
                {
                    url = url.Replace("http", "https").Replace(":443", "");
                }
                return url;
            }
        }
        /// <summary>
        /// 装载分页
        /// </summary>
        /// <param name="EvePageCount"></param>
        /// <param name="zd"></param>
        /// <param name="TbName"></param>
        /// <param name="gourlname"></param>
        /// <param name="CurentIdx"></param>
        /// <param name="condtoin"></param>
        /// <returns></returns>
        public string LoadSpliPage(int EvePageCount, string zd, string TbName, string gourlname, int CurentIdx, string condtoin,ref int count)
        {
            DataTable Art_Table = DataCenter.Instans.SearchTb(string.Format("select count({0}) as ct from {1} {2}", zd, TbName, condtoin));
            count = int.Parse(Art_Table.Rows[0]["ct"].ToString());
            double dx = ((double)count / EvePageCount);
            int SumPage = (int)Math.Ceiling(dx);
            SplicPage sp = new SplicPage(EvePageCount, gourlname, SumPage);
            return sp.GreatSplitPageTakeQs(CurentIdx);
        }
    }
    public class SplicPage
    {
        public int SumPage = 0;//共多少页
        public int EvPageCount = 0;//每页显示多少行
        public string PageName = "";//跳转页名称
        public string Condtion = "";
        List<SpIdx> Arr_spidx = new List<SpIdx>();
        public SplicPage(int _EvPageCount, string _PageName, int _SumPage, string condtion = "")
        {
            SumPage = _SumPage;
            EvPageCount = _EvPageCount;
            PageName = _PageName;
            Condtion = condtion;
            //8至少分8页显示
            for (int i = 0; i < _SumPage; i += 8)
            {
                Arr_spidx.Add(new SpIdx(i, (i + 8)));
            }
        }
        public string GreatSplitPageTakeQs(int _CurentPage)
        {
            string page = "<div class=\"fy\"><ul>";
            PageName = HttpUtility.UrlDecode(PageName);
            int cx_idx = PageName.IndexOf("?cx");
            if (PageName.IndexOf("?page") != -1 || PageName.IndexOf("&page") != -1)
            {
                PageName = PageName.Replace("?page", "^").Replace("&page", "^");
                PageName = PageName.Split('^')[0];
            }
            string pagestr = (cx_idx == -1 ? "?page=" : "&page=");
            page += _CurentPage == 1 ? "" : "<li class=\"p_sy\"><a href=\"" + string.Format("{0}/{1}" + pagestr + "{2}", c_glob.Instans.CurentPath, PageName, (_CurentPage - 1)) + "\"><img id=\"tp_sy\" src=\"" + c_glob.Instans.CurentPath + "/img/sy_img.png\" />上一页</a></li>";
            for (int i = 0; i < Arr_spidx.Count; i++)
            {
                int min = Arr_spidx[i].Min;
                int max = Arr_spidx[i].Max >= SumPage ? SumPage : Arr_spidx[i].Max;
                if (_CurentPage > min && _CurentPage <= max)
                {
                    for (int j = (min + 1); j <= max; j++)
                    {

                        if (_CurentPage == j)//显示样式
                        {
                            string nav_Url = string.Format("{0}/{1}" + pagestr + "{2}", c_glob.Instans.CurentPath, PageName, j);
                            page += "<li class=\"fya\" style=\"background:#BF4E6A;width:30px;text-align:center;border:1px solid #d7d7d7\"><a href=\"" + nav_Url + "\">" + j + "</a></li>";
                        }
                        else
                        {
                            page += "<li class=\"p_ymsz\"><a href=\"" + string.Format("{0}/{1}" + pagestr + "{2}", c_glob.Instans.CurentPath, PageName, j) + "\">" + j + "</a></li>";
                        }
                    }
                }
            }
            page += "<li class=\"p_sy\">共:" + SumPage + "页</li>";
            page += _CurentPage == SumPage ? "" : "<li class=\"p_sy\"><a href=\"" + string.Format("{0}/{1}" + pagestr + "{2}", c_glob.Instans.CurentPath, PageName, (_CurentPage + 1)) + "\">下一页<img id=\"tp_xy\" src=\"" + c_glob.Instans.CurentPath + "/img/xy_img.png\" /></a></li>";
            page += "<li class=\"p_tzym\"><span>向</span><input id=\"tzym_t\" type=\"text\" onfocus=\"pdjd(this);\" value =\"\"><span>页</span><span><div id=\"ymtz_a\" onclick=\"SearchGoPage('" + PageName + "');\">跳转</div></span></li></ul></div>";
            return page;
        }
        public string Idx_GreatSplitPageTakeQs(int _CurentPage)
        {
            string page = "<div class=\"fy\"><ul>";
            PageName = HttpUtility.UrlDecode(PageName);
            page += _CurentPage == 1 ? "" : "<li class=\"p_sy\"><a href=\"" + string.Format("{0}/{1}/{2}{3}", c_glob.Instans.CurentPath, PageName, (_CurentPage - 1), Condtion) + "\">上一页</a></li>";
            for (int i = 0; i < Arr_spidx.Count; i++)
            {
                int min = Arr_spidx[i].Min;
                int max = Arr_spidx[i].Max >= SumPage ? SumPage : Arr_spidx[i].Max;
                if (_CurentPage > min && _CurentPage <= max)
                {
                    for (int j = (min + 1); j <= max; j++)
                    {

                        if (_CurentPage == j)//显示样式
                        {
                            page += "<li class=\"fya\" style=\"color:#31abff;width:30px;text-align:center;border:1px solid #dcdbdb\"><a href=\"" + string.Format("{0}/{1}/{2}{3}", c_glob.Instans.CurentPath, PageName, j, Condtion) + "\">" + j + "</a></li>";
                        }
                        else
                        {
                            page += "<li class=\"p_ymsz\"><a href=\"" + string.Format("{0}/{1}/{2}{3}", c_glob.Instans.CurentPath, PageName, j, Condtion) + "\">" + j + "</a></li>";
                        }
                    }
                }
            }
            page += "<li class=\"p_sy\">共:" + SumPage + "页</li>";
            page += _CurentPage == SumPage ? "" : "<li class=\"p_sy\"><a href=\"" + string.Format("{0}/{1}/{2}{3}", c_glob.Instans.CurentPath, PageName, (_CurentPage + 1), Condtion) + "\">下一页</a></li>";
            return page;
        }
    }
    public class SpIdx
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public SpIdx(int mi, int mx)
        {
            Min = mi;
            Max = mx;
        }
    }
}
