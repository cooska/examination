using Excel;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Common
{
    public class DataToExcel
    {
        /// <summary>调用Excel打印数据
        /// strFile包含路径的模板文件名，strSheetName工作表名称，nQshh起始行号，nQslh起始列号，nZzlh终止列号，strBbmc报表名称，strDwmc单位名称，strZbr制表人
        /// </summary>
        public static void ExportExcelTemplate(System.Data.DataTable dt, string strFileName, string strSheetName, int nQshh, int nQslh, int nZzlh, string strBbmc, string strDwmc, string strZbr)
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

            GenerateData(hssfworkbook, dt, strSheetName, nQshh, nQslh, nZzlh, strBbmc, strDwmc, strZbr);

            //导出
            curContext.Response.BinaryWrite(WriteToStream(hssfworkbook).GetBuffer());
            curContext.Response.End();
        }

        /// <summary>填充Excel数据
        /// strSheetName工作表名称，nQshh起始行号，nQslh起始列号，nZzlh终止列号，strBbmc报表名称，strDwmc单位名称，strZbr制表人
        /// </summary>
        private static void GenerateData(HSSFWorkbook hssfworkbook, System.Data.DataTable dt, string strSheetName, int nQshh, int nQslh, int nZzlh, string strBbmc, string strDwmc, string strZbr)
        {
            HSSFSheet sheet = hssfworkbook.GetSheet(strSheetName);
            //替换模板参数
            for (int row = 0; row <= nQshh - 1; row++)
            {
                for (int col = 0; col <= nZzlh - 1; col++)
                {
                    HSSFRow frow = sheet.GetRow(row);
                    if (frow != null)
                    {
                        HSSFCell cell = frow.GetCell(col);
                        if (cell != null)
                        {
                            string strCellValue = cell.StringCellValue;
                            if (strCellValue.IndexOf("[BBMC]") != -1)
                                cell.SetCellValue(strCellValue.Replace("[BBMC]", strBbmc));
                            if (strCellValue.IndexOf("[DWMC]") != -1)
                                cell.SetCellValue(strCellValue.Replace("[DWMC]", strDwmc));
                            if (strCellValue.IndexOf("[ZBSJ]") != -1)
                                cell.SetCellValue(strCellValue.Replace("[ZBSJ]", DateTime.Now.ToLongDateString()));
                            if (strCellValue.IndexOf("[ZBR]") != -1)
                                cell.SetCellValue(strCellValue.Replace("[ZBR]", strZbr));
                            if (strCellValue.IndexOf("[CSUM]") != -1)
                                cell.SetCellValue(strCellValue.Replace("[CSUM]", "=SUM(R[1]C:R[" + dt.Rows.Count + "]C)"));
                        }
                    }
                }
            }

            //边框
            HSSFCellStyle style = hssfworkbook.CreateCellStyle();
            style.BorderBottom = HSSFCellStyle.BORDER_THIN;
            style.BorderLeft = HSSFCellStyle.BORDER_THIN;
            style.BorderRight = HSSFCellStyle.BORDER_THIN;
            style.BorderTop = HSSFCellStyle.BORDER_THIN;
            style.VerticalAlignment = HSSFCellStyle.ALIGN_CENTER;
            //填充表格内容
            for (int row = 0; row < dt.Rows.Count; row++)
            {
                HSSFRow frow = sheet.CreateRow(nQshh + row - 1);
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
}
