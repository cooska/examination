using Maticsoft.Common.DEncrypt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Dataport
{
    public class uslogin
    {
        static uslogin _instans;
        public static uslogin Instans
        {
            get { return _instans == null ? _instans = new uslogin() : _instans; }
        }
        /// <summary>
        /// 用户登录（0:无此账号,1:登录成功,2:密码错误,3:授权码错误）
        /// </summary>
        /// <param name="usname"></param>
        /// <param name="uspass"></param>
        /// <param name="sqm"></param>
        /// <returns>1:成功,2:密码错误,0:没有该用户</returns>
        public sbyte UserLogin(string usname, string uspass)
        {
            string sql = "select * from user_info where user_name='" + usname + "'";
            DataTable tb = DataCenter.Instans.SearchTb(sql);
            if (tb.Rows.Count > 0)
            {
                if (DESEncrypt.Encrypt(uspass) == tb.Rows[0]["user_password"].ToString())
                {
                    HttpContext.Current.Session.Timeout = 120;//2小时过期
                    UserRow = tb.Rows[0];
                    HttpContext.Current.Session["usinfo"] = UserRow;
                    return 1;//登录成功
                }
                return 2;//密码错误
            }
            return 0;
        }
        public sbyte UserLogout()
        {
            HttpContext.Current.Session["usinfo"] = null;
            HttpContext.Current.Session.Clear();
            return 1;
        }
        public static DataRow UserRow { get; set; }
    }

}


