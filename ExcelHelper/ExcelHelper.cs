using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace ExcelHelper
{
    public class ExcelHelper:IDisposable
    {
        protected Microsoft.Office.Interop.Excel.Application excelApp;
        protected Workbook excelWorkbook;
        public ExcelHelper(string excelFile)
        {
            // Create the Excel Application object
		    excelApp = new Application();

		    excelWorkbook = excelApp.Workbooks.Open(excelFile);

        }
        public void Append(string label, Dictionary<string, decimal> weight)
        {
            Worksheet worksheet1 = (Worksheet)excelWorkbook.Worksheets[1];
            string text;
            int columnidx = 2;
            for (columnidx = 2; columnidx < worksheet1.Columns.Count; columnidx++)
            {
                text=((Range)worksheet1.Cells[1, columnidx]).Text;
                if (string.IsNullOrWhiteSpace(text) || text == label) break;
            }
            worksheet1.Cells[1, columnidx] = label;
            foreach (KeyValuePair<string, decimal> kv in weight)
            {
                int rowindex = 2;
                for ( rowindex = 2; rowindex < worksheet1.Rows.Count; rowindex++)
                {
                    text=((Range)worksheet1.Cells[rowindex, 1]).Text;
                    if (text == kv.Key ||
                        string.IsNullOrWhiteSpace(text)) break;
                }
                worksheet1.Cells[rowindex, 1] = kv.Key;
                worksheet1.Cells[rowindex, columnidx] = kv.Value;
            }

            excelWorkbook.Save();
        }

        public void Dispose()
        {
            // Release the Application object
            excelApp.Quit();
            excelApp = null;
        }
    }
}
