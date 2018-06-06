using Dataport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jsexam.control
{
    public class c_login
    {
        /// <summary>
        /// 检测用户登陆
        /// </summary>
       public sbyte CheckLogin(string name,string pass)
        {
            sbyte rst = uslogin.Instans.UserLogin(name, pass);
            return rst;
        }
        /// <summary>
        /// 退出登陆
        /// </summary>
        public sbyte SigOut()
        {
            sbyte rst = uslogin.Instans.UserLogout();
            return rst;
        }
    }
}