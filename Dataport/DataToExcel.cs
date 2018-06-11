#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：Dataport
* 项目描述 ：
* 类 名 称 ：DataToExcel
* 类 描 述 ：
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Administrator
* 创建时间 ：2018/6/11 15:30:19
* 更新时间 ：2018/6/11 15:30:19
*******************************************************************
* Copyright @ 湖南教育出版社-贝壳网. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Dataport
{
    public class DataToExcel
    {
        public DataToExcel()
        {

        }
        public delegate void Dlg_ShowExportPercent(int ct, int SumCt);
        public event Dlg_ShowExportPercent Evnet_ShowExportPercent;
        /// <summary>调用Excel打印数据
        /// strFile包含路径的模板文件名，strSheetName工作表名称，nQshh起始行号，nQslh起始列号，strBbmc报表名称，strDwmc单位名称，strZbr制表人
        /// </summary>
        public static void ExportExcelTemplate(ExportContion Export,string strFileName, string strSheetName, int nQshh, int nQslh, string strBbmc, string strDwmc, string strZbr)
        {
            // 导出
            string filename = HttpUtility.UrlEncode("考试成绩查询表.xls");// PinyinHelper.GetPinyin(Path.GetFileName(strFileName));
            HttpContext curContext = HttpContext.Current;
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
            curContext.Response.Charset = "UTF-8";
            curContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            curContext.Response.Clear();

            //读取模板文件
            FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read);
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
            //计算导出相关数据


            GenerateData(Export,hssfworkbook,strSheetName, nQshh, nQslh, strBbmc, strDwmc, strZbr);

            //导出
            curContext.Response.BinaryWrite(WriteToStream(hssfworkbook).GetBuffer());
            curContext.Response.End();
            return;
        }

        /// <summary>填充Excel数据
        /// strSheetName工作表名称，nQshh起始行号，nQslh起始列号，nZzlh终止列号，strBbmc报表名称，strDwmc单位名称，strZbr制表人
        /// </summary>
        private static void GenerateData(ExportContion Export, HSSFWorkbook hssfworkbook, string strSheetName, int nQshh, int nQslh, string strBbmc, string strDwmc, string strZbr)
        {
            HSSFSheet sheet = hssfworkbook.GetSheet(strSheetName);

            #region 替换模板参数
            //for (int row = 0; row <= nQshh - 1; row++)
            //{
            //    for (int col = 0; col <= nZzlh - 1; col++)
            //    {
            //        HSSFRow frow = sheet.GetRow(row);
            //        if (frow != null)
            //        {
            //            HSSFCell cell = frow.GetCell(col);
            //            if (cell != null)
            //            {
            //                string strCellValue = cell.StringCellValue;
            //                if (strCellValue.IndexOf("[BBMC]") != -1)
            //                    cell.SetCellValue(strCellValue.Replace("[BBMC]", strBbmc));
            //                if (strCellValue.IndexOf("[DWMC]") != -1)
            //                    cell.SetCellValue(strCellValue.Replace("[DWMC]", strDwmc));
            //                if (strCellValue.IndexOf("[ZBSJ]") != -1)
            //                    cell.SetCellValue(strCellValue.Replace("[ZBSJ]", DateTime.Now.ToLongDateString()));
            //                if (strCellValue.IndexOf("[ZBR]") != -1)
            //                    cell.SetCellValue(strCellValue.Replace("[ZBR]", strZbr));
            //                if (strCellValue.IndexOf("[CSUM]") != -1)
            //                    cell.SetCellValue(strCellValue.Replace("[CSUM]", "=SUM(R[1]C:R[" + dt.Rows.Count + "]C)"));
            //            }
            //        }
            //    }
            //}
            #endregion

            //边框
            HSSFCellStyle style = hssfworkbook.CreateCellStyle();
            style.BorderBottom = HSSFCellStyle.BORDER_THIN;
            style.BorderLeft = HSSFCellStyle.BORDER_THIN;
            style.BorderRight = HSSFCellStyle.BORDER_THIN;
            style.BorderTop = HSSFCellStyle.BORDER_THIN;
            style.VerticalAlignment = HSSFCellStyle.ALIGN_CENTER;
            //填充表格内容
            int RowIdx = 0;//设置开始行号 该变量用于存储总行数
            for (int i = 0; i < Export.SumPage; i++)
            {
                int Rang = i * Export.EvePage;
                string limit = string.Format("limit {0},{1}", Rang, Export.EvePage);
                string sql = string.Format("{0} {1} {2}",Export.sql,Export.condtion,limit);
                DataTable dt = DataCenter.Instans.SearchTb(sql);
                //终止列号
                int nZzlh = dt.Columns.Count;
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    HSSFRow frow = sheet.CreateRow(nQshh + RowIdx - 1);
                    for (int col = 0; col < nZzlh; col++)
                    {
                        HSSFCell cell = frow.CreateCell(col);
                        string strCell = null;
                        if (col < dt.Columns.Count)
                        {//小于DataTable的列数时
                            string rws = dt.Rows[row][col].ToString() == "0" ? "" : dt.Rows[row][col].ToString();
                            try
                            {
                                strCell = DateTime.Parse(rws).ToShortDateString();
                            }
                            catch (Exception ex)
                            {
                                strCell = rws;
                            }

                        }
                        else
                            strCell = "";
                        if (strCell == null || strCell == System.DBNull.Value.ToString())
                            strCell = " ";
                        {
                            cell.SetCellValue(strCell);
                            cell.CellStyle = style;
                        }
                    }
                    //if (Evnet_ShowExportPercent!=null)
                    //{
                    //    Evnet_ShowExportPercent(RowIdx,Export.DataCount);
                    //}
                    RowIdx++;
                }
            }
            sheet.ForceFormulaRecalculation = true;
        }

        private static MemoryStream WriteToStream(HSSFWorkbook hssfworkbook)
        {
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            return file;
        }
    }
    public class ExportContion
    {
        public string sql { get; set; }
        public string condtion { get; set; }

        public int DataCount { get; set; }
        public int SumPage { get; set; }
        public int EvePage { get; set; }
    }
}
