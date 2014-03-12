using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBengine;

namespace BenchmarkResultApplication
{
    public enum Interval{
        Hour,
        Day,
        Week
    }
    public class BenchmarkResultHelper
    {
        public static DataTable CreateTableWeight()
        {

            DataTable tableWeight = new DataTable("TableWeight");
            tableWeight.Columns.Add("Table");
            tableWeight.Columns.Add("Weight", typeof(decimal));
            return tableWeight;
        }
        public static DataTable CreateTableWeight(string connectioninfo)
        {

            DataTable tableWeight = CreateTableWeight();
            DBengine.DBengine dbeng = new DBengine.DBengine();
            try
            {
                dbeng.DBConnect(connectioninfo, DataBaseType.sqlserver);

                foreach (string tablename in dbeng.GetTableName())
                {
                    int count = (int)dbeng.ExecuteScalar("select count(*) from " + tablename);
                    tableWeight.Rows.Add(new object[] { tablename, count });
                }
                return tableWeight;

            }
            catch (Exception)
            {

            }
            finally
            {
                dbeng.Close();
            }
            return tableWeight;
        }
        public static DataTable CreateOperationWeight()
        {
            DataTable operationWeight = new DataTable("OperationWeight");
            operationWeight.Columns.Add("Operation");
            operationWeight.Columns.Add("Weight", typeof(decimal));
            operationWeight.Rows.Add(new object[] { "TableScan", 1 });
            operationWeight.Rows.Add(new object[] { "ClusteredIndexScan", 1 });
            operationWeight.Rows.Add(new object[] { "RIDLookup", 1 });
            operationWeight.Rows.Add(new object[] { "HashJoin", 1 });
            operationWeight.Rows.Add(new object[] { "Sort", 1 });
            operationWeight.Rows.Add(new object[] { "KeyLookup", 1 });
            operationWeight.Rows.Add(new object[] { "NestedLoops", 1 });
            return operationWeight;
        }

        protected Dictionary<string, decimal> tableWeight ;
        protected Dictionary<string, decimal> operationWeight;
        public BenchmarkResultHelper(DataTable tableWeightTable, DataTable operationWeightTable)
        {
            tableWeight = GetDictionaryFromTable(tableWeightTable);
            operationWeight = GetDictionaryFromTable(operationWeightTable);
        }
        protected Dictionary<string, decimal> GetDictionaryFromTable(DataTable table)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            foreach (DataRow dr in table.Rows)
            {
                result.Add(dr[0].ToString(), Convert.ToDecimal(dr[1]));
            }
            return result;
        }
        protected Dictionary<string, decimal> CalculateWeight(DataRow[] executionPlan)
        {
 
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            var objectnames = from dr in executionPlan select dr["Object"].ToString();
            foreach (string  objectname  in objectnames.Distinct<string>().ToArray<string>() )
            {
                decimal weight = 0;
                foreach (DataRow opdr in from dr in executionPlan where dr["Object"].ToString()==objectname select dr )
                {
                    decimal tableweight = 0;
                    decimal operationweight = 0;
                    if (tableWeight.ContainsKey(opdr["Table"].ToString())) tableWeight.TryGetValue(opdr["Table"].ToString(), out tableweight);
                    if (operationWeight.ContainsKey(opdr["Operation"].ToString())) operationWeight.TryGetValue(opdr["Operation"].ToString(), out operationweight);
                    weight += tableweight * operationweight * Convert.ToDecimal(opdr["Count"].ToString());
                }
                result.Add(objectname, weight);
                //result.Rows.Add(new object[] { spdr["Object"].ToString(), weight });
            }
            return result;
        }
        public void CalculateWeight(List<string> SourceFiles, Interval interval, string excel)
        {


            DataTable tableOperation = new DataTable("Operation");
            tableOperation.Columns.Add("Object");
            tableOperation.Columns.Add("CaptureTime");
            tableOperation.Columns.Add("Table");
            tableOperation.Columns.Add("Operation");
            tableOperation.Columns.Add("Count",typeof(decimal));



            DateTime startTime = DateTime.MaxValue;
            DateTime endTime = DateTime.MinValue;
            foreach (string file in SourceFiles)
            {
                DataSet ds = new DataSet();
                ds.ReadXml(file);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string captureTime = dr["FirstCaptureTime"].ToString();
                    if (startTime >Convert.ToDateTime( captureTime)) startTime = Convert.ToDateTime(captureTime);
                    if (endTime < Convert.ToDateTime(captureTime)) endTime = Convert.ToDateTime(captureTime);

                    string objectName = dr["Object"].ToString();
                    int count = Convert.ToInt32(dr["Count"].ToString());
                    foreach (DataRow opdr in ds.Tables[1].Select(" Object='"+objectName+"'"))
                    {
                        decimal avgcount = Convert.ToDecimal(opdr["Count"].ToString())/count;
                        tableOperation.Rows.Add(new object[] { objectName, captureTime, opdr["Table"].ToString(), opdr["Operation"].ToString(),avgcount });
                    }
                }
            }

            TimeSpan step= new TimeSpan(1,0,0);
            switch (interval)
            {
                case Interval.Hour:
                    step = new TimeSpan(1, 0, 0);
                    break;
                case Interval.Day:
                    step = new TimeSpan(24, 0, 0);
                    break;
                case Interval.Week:
                    step = new TimeSpan(24*7, 0, 0);
                    break;
            }

            using (ExcelHelper.ExcelHelper excelHelper = new ExcelHelper.ExcelHelper(excel))
            {
                for (DateTime timerange = startTime; timerange < endTime; timerange = timerange.Add(step))
                {
                    var drarray = from dr
                           in tableOperation.Select()
                                  where Convert.ToDateTime(dr["CaptureTime"].ToString()) >= timerange && Convert.ToDateTime(dr["CaptureTime"].ToString()) < timerange.Add(step)
                                  select dr;

                    Dictionary<string, decimal> result = CalculateWeight(drarray.ToArray<DataRow>());
                    excelHelper.Append("Date" + timerange.ToString("yyyy/MM/dd HH:mm:ss"), result);
                }
            }
            
        }

        public void CalculateWeight(string connectionstring, string criteria, Interval interval, string excel)
        {


            DBengine.DBengine dbeng = new DBengine.DBengine();
            try
            {
                dbeng.DBConnect(connectionstring, DataBaseType.sqlserver);
                if (!string.IsNullOrEmpty(criteria)) criteria = " where " + criteria;
                DateTime startTime = Convert.ToDateTime(dbeng.ExecuteScalar("select min(convert(datetime,FirstCaptureTime)) from StoredProcedure "+ criteria));
                DateTime endTime = Convert.ToDateTime(dbeng.ExecuteScalar("select max(convert(datetime,FirstCaptureTime)) from StoredProcedure " + criteria));

                TimeSpan step = new TimeSpan(1, 0, 0);
                switch (interval)
                {
                    case Interval.Hour:
                        step = new TimeSpan(1, 0, 0);
                        break;
                    case Interval.Day:
                        step = new TimeSpan(24, 0, 0);
                        break;
                    case Interval.Week:
                        step = new TimeSpan(24 * 7, 0, 0);
                        break;
                }
                using (ExcelHelper.ExcelHelper excelHelper = new ExcelHelper.ExcelHelper(excel))
                {
                    for (DateTime timerange = startTime; timerange < endTime; timerange = timerange.Add(step))
                    {
                        DataTable dt = dbeng.QuerySQL("SELECT [dbo].[Operation].[Object]" +
"      ,[dbo].[Operation].[Table]" +
"      ,[dbo].[Operation].[Operation]" +
"      ,CONVERT(decimal, Sum([dbo].[Operation].[Count]))/Sum([dbo].[StoredProcedure].[Count]) as Count" +
"  FROM [dbo].[Operation] inner join [dbo].[StoredProcedure] on [dbo].[Operation].[Object]=[dbo].[StoredProcedure].[Object] and [dbo].[Operation].[FirstCaptureTime]=[dbo].[StoredProcedure].[FirstCaptureTime] " +
"  where convert(datetime,[dbo].[Operation].[FirstCaptureTime])>='" + timerange.ToString("yyyy/MM/dd HH:mm:ss") + "' and " +
" convert(datetime,[dbo].[Operation].[FirstCaptureTime])<'" + timerange.Add(step).ToString("yyyy/MM/dd HH:mm:ss") + "'" +
" group by   [dbo].[Operation].[Object],[dbo].[Operation].[Table],[dbo].[Operation].[Operation]");
                        Dictionary<string, decimal> result = CalculateWeight(dt.Select());
                        excelHelper.Append("Date" + timerange.ToString("yyyy/MM/dd HH:mm:ss"), result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            finally
            {
                dbeng.Close();
            }

           
          

           

           
        }
    }
}
