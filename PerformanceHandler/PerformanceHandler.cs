using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Xml.XPath;
using TraceWrapper;

namespace PerformanceHandler
{
    /// <summary>
    /// not able to get StatementText
    /// SELECT cp.objtype AS ObjectType,
    ///OBJECT_NAME(st.objectid,st.dbid) AS ObjectName,
    ///cp.usecounts AS ExecutionCount,
    ///st.TEXT AS QueryText,
    ///qp.query_plan AS QueryPlan
    ///FROM sys.dm_exec_cached_plans AS cp
    ///CROSS APPLY sys.dm_exec_query_plan(cp.plan_handle) AS qp
    ///CROSS APPLY sys.dm_exec_sql_text(cp.plan_handle) AS st
    /// </summary>
    public class PerformanceHandler:IResultHandler
    {
        public event ResultEventHandler OnResultChange;
        protected TraceDefinitionFile traceDefinition = new TraceDefinitionFile();
        protected DataTable result;
        internal enum HeavyOperations
        {
            TableScan,//Occurs when the corresponding table does not have a clustered index. Most likely, creating a clustered index or defragmenting index will enable you to get rid of it.
            ClusteredIndexScan,//Sometimes considered equivalent to Table Scan. Takes place when a non-clustered index on an eligible column is not available. Most of the time, creating a non-clustered index will enable you to get rid of it.
            RIDLookup,//Takes place when you have a non-clustered index but the same table does not have any clustered index. In this case, the database engine has to look up the actual row using the row ID, which is an expensive operation. Creating a clustered index on the corresponding table would enable you to get rid of it.
            HashJoin, //The most expensive joining methodology. This takes place when the joining columns between two tables are not indexed. Creating indexes on those columns will enable you to get rid of it.
            //NestedLoops, Most cases, this happens when a non-clustered index does not include (Cover) a column that is used in the SELECT column list. In this case, for each member in the non-clustered index column, the database server has to seek into the clustered index to retrieve the other column value specified in the SELECT list. Creating a covered index will enable you to get rid of it.
            Sort,
            KeyLookup//
        }
        private DataTable InitResult()
        {
            DataTable result = new DataTable();
            result.Columns.Add("Object");
            result.Columns.Add("Statement");
            result.Columns.Add("XmlPlan");
            result.Columns.Add("TransactionID");
            foreach (HeavyOperations heavyop in Enum.GetValues(typeof(HeavyOperations)))
            {
                result.Columns.Add(Enum.GetName(typeof(HeavyOperations), heavyop)+"Tables");
                result.Columns.Add(Enum.GetName(typeof(HeavyOperations), heavyop)+"Count", typeof(int));
            }


            return result;
        }
        public PerformanceHandler()
        {
            List<TraceColumn> columns = TraceDefinitionFile.CommonColumns;
            columns.Add(TraceColumn.ObjectName);
            columns.Add(TraceColumn.TransactionID);
            traceDefinition.Add(TraceEventEnum.SQLBatchCompleted, columns);
            //traceDefinition = TraceDefinitionFile.Common;
            traceDefinition.Add(TraceEventEnum.ShowplanXMLStatisticsProfile, columns);
            result = InitResult();
        }
        public void OnNewTraceEvent(TraceEvent traceEvent)
        {
            
            if (traceEvent["EventClass"].ToString().StartsWith("Showplan"))
            {
                string textdata = traceEvent["TextData"].ToString();
                XmlDocument xdoc = new XmlDocument();
                string header = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";
                try
                {
                    xdoc.LoadXml(header + textdata.Replace("xmlns=\"http://schemas.microsoft.com/sqlserver/2004/07/showplan\"", ""));
                    //var nsmgr = new XmlNamespaceManager(xdoc.NameTable);
                    //nsmgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    //nsmgr.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
                    //XmlNode statementNode = xdoc.SelectSingleNode("//StmtSimple", nsmgr);
                    //string statement = "Cached";
                    //if (statementNode.Attributes["StatementText"] != null)
                    //{
                    //    statement = statementNode.Attributes["StatementText"].Value;
                    //}

                    DataRow[] rows = result.Select(" TransactionID ='" + traceEvent["TransactionID"].ToString() + "'");
                    DataRow row;
                    bool newrowflag = false;
                    if (rows.Length > 0)
                    {
                        row = rows[0];
                    }
                    else
                    {
                        newrowflag = true;
                        string objectname = traceEvent["ObjectName"].ToString();
                        object[] newrow = new object[result.Columns.Count];
                        newrow[0] = objectname;
                        newrow[2] = textdata;
                        newrow[3] = traceEvent["TransactionID"].ToString();

                        for (int idx = 5; idx < newrow.Length; idx += 2) newrow[idx] = 0;
                        row = result.Rows.Add(newrow);
                    }

                    bool hasheavy = false;
                    foreach (XmlNode relop in xdoc.SelectNodes("//RelOp"))
                    {
                        if (relop.Attributes["PhysicalOp"] != null)
                        {
                            HeavyOperations heavyop;
                            bool isheavy = Enum.TryParse<HeavyOperations>(relop.Attributes["PhysicalOp"].Value.Replace(" ", ""), out heavyop);

                            if (isheavy)
                            {
                                string table = string.Empty;
                                XmlNode scanobject = relop.SelectSingleNode(".//Object");
                                if (scanobject != null && scanobject.Attributes["Table"] != null) table = scanobject.Attributes["Table"].Value;
                                if (table != string.Empty)
                                {
                                    row[Enum.GetName(typeof(HeavyOperations), heavyop) + "Count"] = int.Parse(row[Enum.GetName(typeof(HeavyOperations), heavyop) + "Count"].ToString()) + 1;
                                    if (!row[Enum.GetName(typeof(HeavyOperations), heavyop) + "Tables"].ToString().Contains(table))
                                    {
                                        row[Enum.GetName(typeof(HeavyOperations), heavyop) + "Tables"] = row[Enum.GetName(typeof(HeavyOperations), heavyop) + "Tables"].ToString() + table;
                                    }
                                    row.AcceptChanges();
                                    hasheavy = true;
                                }
                            }
                        }
                    }
                    if (hasheavy)
                    {
                        if (OnResultChange != null) OnResultChange(this.GetType().Name, new Result() { Content = result, Action = ResultAction.Refresh });
                    }
                    else
                    {
                        if (newrowflag) result.Rows.Remove(row);
                    }
                }
                catch (Exception)
                {
                }
            }//other trace event
            else if (traceEvent["EventClass"].ToString().Contains("Completed"))
            {
                if (result.Rows.Count > 0)
                {
                    result.Rows[result.Rows.Count-1][1] = traceEvent["TextData"].ToString();
                    if (OnResultChange != null) OnResultChange(this.GetType().Name, new Result() { Content = result, Action = ResultAction.Refresh });
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
            throw new NotImplementedException();
        }
    }
}
