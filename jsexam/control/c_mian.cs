using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jsexam.control
{
    public class c_mian<T> where T : new()
    {
        static T _instans;
        public static T Instans
        {
            get
            {
                return _instans == null ? _instans = new T() : _instans;
            }
        }

    }
}