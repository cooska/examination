using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Dataport
{
    public class DataCenter
    {
        const string constr = @"server='120.76.157.73'; userid='root'; password='wdxqm.exam';database='exami';Charset=utf8;";
        //const string constr = @"server='sql.t249.vhostgo.com'; userid='coos45'; password='westdata.45';database='coos45';Charset=utf8;port=3306;";
        static DataCenter _Instans = null;
        public static DataCenter Instans
        {
            get { return _Instans == null ? _Instans = new DataCenter() : _Instans; }
        }
        static MySqlConnection _cn = null;
        static MySqlConnection cn
        {
            get
            {
                if (_cn == null)
                {
                    _cn = new MySqlConnection(constr);
                    _cn.Open();
                }
                return _cn;
            }
        }
        MySqlCommand cm = new MySqlCommand();
        MySqlCommand cmproce = null;
        internal MySqlCommand BulidCMD(string sql)
        {
            cm.CommandText = "set names utf8;" + sql;
            cm.Connection = cn;
            return cm;
        }
        
        MySqlDataAdapter BuildDa(string sql)
        {
            return new MySqlDataAdapter(BulidCMD(sql));
        }
        public DataTable SearchTb(string sql)
        {
            DataTable tb = new DataTable();
            MySqlDataAdapter da = BuildDa(sql);
            da.Fill(tb);
            da.Dispose();
            return tb;
        }
        public int ExecSql(string sql)
        {
            return BulidCMD(sql).ExecuteNonQuery();
        }

        
       
        /// <summary>
        /// 获取最后插入ID
        /// </summary>
        /// <param name="tbname"></param>
        /// <param name="zdm"></param>
        /// <returns></returns>
        public int GetLastID(string tbname, string zdm)
        {
            DataTable tb = SearchTb("select last_insert_id(" + zdm + ") as gs from " + tbname + " order by gs DESC LIMIT 1");
            return int.Parse(tb.Rows[0]["gs"].ToString());
        }
        /// <summary>
        /// 获取json数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string GetJsonData(string sql)
        {
            DataTable tb = SearchTb(sql);
            return DataTableToJson(tb);
        }
        public string DataTableToJson(DataTable table)
        {
            var JsonString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
            }
            table.Dispose();
            return JsonString.ToString();
        }
        /// <summary>
        /// 将对象转json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string getJsonByObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public T getJsonObject<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
        public string GetDataRowJson_Columns(DataRow rw)
        {
            string str = "[{\"Columns\":\"{th}\",";
            string cols = "";
            for (int i = 0; i < rw.Table.Columns.Count; i++)
            {
                string key = rw.Table.Columns[i].ColumnName.ToLower();
                cols += key + ",";
                string val = rw[key].ToString();
                string Line = string.Format("\"{0}\":\"{1}\",", key, val);
                str += Line;
            }
            cols = cols.TrimEnd(',');
            str = str.Replace("{th}", cols).TrimEnd(',');
            return str += "}]";
        }
        public string GetDataRowJson(DataRow rw)
        {
            string str = "[{";
            for (int i = 0; i < rw.Table.Columns.Count; i++)
            {
                string key = rw.Table.Columns[i].ColumnName.ToLower();
                string val = rw[key].ToString();
                string Line = string.Format("\"{0}\":\"{1}\",", key, val);
                str += Line;
            }

            str = str.TrimEnd(',');
            return str += "}]";
            // return HttpUtility.UrlEncode(str);
        }

    }

}
