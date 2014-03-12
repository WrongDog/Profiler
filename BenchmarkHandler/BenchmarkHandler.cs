using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using TraceWrapper;

namespace BenchmarkHandler
{
    public class BenchmarkHandler:IResultHandler
    {
        public event ResultEventHandler OnResultChange;
        protected TraceDefinitionFile traceDefinition = new TraceDefinitionFile();
        public DataSet Result
        {
            get;
            protected set;
        }
        protected List<string> HeavyOperations= new List<string>();
        //internal enum HeavyOperations
        //{
        //    TableScan,//Occurs when the corresponding table does not have a clustered index. Most likely, creating a clustered index or defragmenting index will enable you to get rid of it.
        //    ClusteredIndexScan,//Sometimes considered equivalent to Table Scan. Takes place when a non-clustered index on an eligible column is not available. Most of the time, creating a non-clustered index will enable you to get rid of it.
        //    RIDLookup,//Takes place when you have a non-clustered index but the same table does not have any clustered index. In this case, the database engine has to look up the actual row using the row ID, which is an expensive operation. Creating a clustered index on the corresponding table would enable you to get rid of it.
        //    HashJoin, //The most expensive joining methodology. This takes place when the joining columns between two tables are not indexed. Creating indexes on those columns will enable you to get rid of it.
        //    //NestedLoops, Most cases, this happens when a non-clustered index does not include (Cover) a column that is used in the SELECT column list. In this case, for each member in the non-clustered index column, the database server has to seek into the clustered index to retrieve the other column value specified in the SELECT list. Creating a covered index will enable you to get rid of it.
        //    Sort,
        //    KeyLookup//
        //}
        private DataSet InitResult()
        {
            DataSet result = new DataSet("Result"+DateTime.UtcNow.ToString().Replace(":",""));
            DataTable tableSp = new DataTable("StoredProcedure");
            tableSp.Columns.Add("Object");
            tableSp.Columns.Add("Count");

            tableSp.Columns.Add("EntryLineNumber");
            tableSp.Columns.Add("FirstCaptureTime",typeof(DateTime));
            tableSp.Columns.Add("LastCaptureTime",typeof(DateTime));

            DataTable tableOperation = new DataTable("Operation");
            tableOperation.Columns.Add("Object");
            tableOperation.Columns.Add("Table");
            tableOperation.Columns.Add("Operation");
            tableOperation.Columns.Add("Count");
            tableOperation.Columns.Add("FirstCaptureTime",typeof(DateTime));
            tableOperation.Columns.Add("LastCaptureTime", typeof(DateTime));

            result.Tables.Add(tableSp);
            result.Tables.Add(tableOperation);
            result.Relations.Add(tableSp.Columns["Object"], tableOperation.Columns["Object"]);
            return result;
        }
        public BenchmarkHandler()
        {
            Init();
        }
        public BenchmarkHandler(string heavyOperationFile)
        {
            DataTable heavyOperationDT = new DataTable();
            heavyOperationDT.ReadXml(heavyOperationFile);
            var list = from dr in heavyOperationDT.Rows.OfType<DataRow>()
                       select Convert.ToString(dr[0]);
            Init();
            InitHeavyOperation(list.ToList<string>());
        }
        public BenchmarkHandler(List<string> heavyOperationDT)
        {
            Init();
            InitHeavyOperation(heavyOperationDT);
        }
        private void InitHeavyOperation(List<string> heavyOperationDT)
        {
            HeavyOperations = new List<string>();
            foreach (string operation in heavyOperationDT)
            {
                if (!HeavyOperations.Contains(operation.Replace(" ", ""))) HeavyOperations.Add(operation.Replace(" ", ""));
            }
        }
        private void Init()
        {
            List<TraceColumn> columns = new List<TraceColumn>();
            columns.Add(TraceColumn.EventClass);
            columns.Add(TraceColumn.ObjectName);
            columns.Add(TraceColumn.LineNumber);
            columns.Add(TraceColumn.NestLevel);
            columns.Add(TraceColumn.SPID);
            columns.Add(TraceColumn.TextData);
            columns.Add(TraceColumn.StartTime);
            //columns.Add(TraceColumn.CPU);
            //columns.Add(TraceColumn.Reads);
            //columns.Add(TraceColumn.Writes);


            traceDefinition.Add(TraceEventEnum.ShowplanXMLStatisticsProfile, columns);
            //TraceDefinitionFile.ColumnFilter filter = new TraceDefinitionFile.ColumnFilter(TraceColumn.ObjectName, true);
            TraceDefinitionFile.ColumnFilter filter = new TraceDefinitionFile.ColumnFilter(TraceColumn.ObjectName,false);
            filter.Operations.Add(new TraceDefinitionFile.ColumnFilter.Operation(TraceDefinitionFile.ColumnFilter.OperatorEnum.Like, "SP%"));
            filter.Operations.Add(new TraceDefinitionFile.ColumnFilter.Operation(TraceDefinitionFile.ColumnFilter.OperatorEnum.NotLike, "%PgicCasinoLink30CommunicationsMessageBridge%"));
            traceDefinition.ColumnFilters.Add(filter);
            Result = InitResult();
           
        
        }
        public void OnNewTraceEvent(TraceEvent traceEvent)
        {

            if (traceEvent["EventClass"].ToString().StartsWith("Showplan") || traceEvent["EventClass"].ToString() == "146")
            {
                string objectname = traceEvent["ObjectName"].ToString();
                string textdata = traceEvent["TextData"].ToString();
                XmlDocument xdoc = new XmlDocument();
                string header = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";
                try
                {
                    xdoc.LoadXml(header + textdata.Replace("xmlns=\"http://schemas.microsoft.com/sqlserver/2004/07/showplan\"", ""));
                    

                    DataRow[] rows = Result.Tables["StoredProcedure"].Select(" Object ='" + objectname + "'");
                    DataRow row;
                    if (rows.Length > 0)
                    {
                        row = rows[0];
                        int lineNumber = Convert.ToInt32(traceEvent["LineNumber"].ToString());
                        int entryLineNumber = Convert.ToInt32(row["EntryLineNumber"].ToString());
                        if (lineNumber <= entryLineNumber) row["Count"] = Convert.ToInt32(row["Count"]) + 1;
                        if (lineNumber < entryLineNumber) row["EntryLineNumber"] = lineNumber.ToString();
                        row["LastCaptureTime"] = Convert.ToDateTime(traceEvent["StartTime"]);
                    }
                    else
                    {
                
                        object[] newrow = new object[5];
                        newrow[0] = objectname;
                        newrow[1] = 1;
                        newrow[2] = traceEvent["LineNumber"].ToString();
                        newrow[3] = Convert.ToDateTime(traceEvent["StartTime"]);
                        newrow[4] = Convert.ToDateTime(traceEvent["StartTime"]);
           
                        row = Result.Tables["StoredProcedure"].Rows.Add(newrow);
                    }

                    bool hasheavy = false;
                    foreach (XmlNode relop in xdoc.SelectNodes("//RelOp"))
                    {
                        if (relop.Attributes["PhysicalOp"] != null)
                        {
                       
                            string operation =relop.Attributes["PhysicalOp"].Value.Replace(" ", "");

                            if (HeavyOperations.Contains(operation) || HeavyOperations.Count==0)
                            {
                                string table = string.Empty;
                                XmlNode scanobject = relop.SelectSingleNode(".//Object");
                                if (scanobject != null && scanobject.Attributes["Table"] != null) table = scanobject.Attributes["Table"].Value.Substring(1,scanobject.Attributes["Table"].Value.Length-2);
                                if (table != string.Empty)
                                {
                                    DataRow[] oprows = Result.Tables["Operation"].Select(" Object ='" + objectname + "' and Table='" + table + "' and Operation='"+operation+"'");
                                    DataRow oprow;
                                    if (oprows.Length > 0)
                                    {
                                        oprow = oprows[0];
                                        oprow["Count"] = Convert.ToInt32(oprow["Count"]) + 1;
                                        oprow["LastCaptureTime"] = Convert.ToDateTime(traceEvent["StartTime"]);
                                    }
                                    else
                                    {

                                        object[] newrow = new object[6];
                                        newrow[0] = objectname;
                                        newrow[1] = table;
                                        newrow[2] = operation;
                                        newrow[3] = 1;
                                        newrow[4] = Convert.ToDateTime(traceEvent["StartTime"]);
                                        newrow[5] = Convert.ToDateTime(traceEvent["StartTime"]);
                                        Result.Tables["Operation"].Rows.Add(newrow);
                                    }
                                    hasheavy = true;
                                }
                            }
                        }
                    }
                    if (hasheavy)
                    {
                        if (OnResultChange != null) OnResultChange(this.GetType().Name, new Result() { Content = Result, Action = ResultAction.Refresh });
                    }
              
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }
            }
        }
        protected int GetCount(string source, string target)
        {
            if (target.Length == 0) return 0;
            return (source.Length - source.ToLower().Replace(target.ToLower(), "").Length) / target.Length;
        }
        public TraceDefinitionFile TraceDefinition
        {
            get { return traceDefinition; }
        }
        public string Name
        {
            get;
            set;
        }
        public void SaveResult(IResultSaveAdapter resultSaveAdapter)
        {
            resultSaveAdapter.Save(this.Name  +DateTime.UtcNow.ToString("yyyy_MM_dd HH_mm_ss"), Result);
        }
    }
}
