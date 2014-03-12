using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TraceWrapper
{
    public class TraceEvent 
    {
        protected DataRow dataRow;       
        public TraceEvent(DataRow dr)
        {
            this.dataRow = ((DataTable)dr.Table.Clone()).Rows.Add(dr.ItemArray);
        }
        public static explicit operator TraceEvent(DataRow dr)
        {
            return new TraceEvent(dr);
        }
        public static implicit operator DataRow(TraceEvent traceEvent)
        {
            return traceEvent.dataRow;
        }
        public object this[string ColumnName]
        {
            get
            {
                return dataRow[ColumnName];
            }
        }

        public Int32? RowNumber
        {
            get
            {
                if (this["RowNumber"] == null) return null;
                return Convert.ToInt32(this["RowNumber"]);
            }
        }
        public TraceEventEnum? EventClass
        {
            get
            {
                if (this["EventClass"] == null) return null;
                string eventname = this["EventClass"].ToString();
                int eventnumber = 0;
                int.TryParse(eventname, out eventnumber);
                if (eventnumber > 0)
                {
                    eventname = Enum.GetName(typeof(TraceEventEnum), eventnumber);
                }
                else
                {
                    eventname = eventname.Replace(":", "").Replace(" ", "").Replace("/", "");
                }
                TraceEventEnum tevent;
                if (Enum.TryParse<TraceEventEnum>(eventname, out tevent))
                {
                    return tevent;
                }


                return null;

            }
        }
        public String ObjectName
        {
            get
            {
                if (this["ObjectName"] == null) return null;
                return Convert.ToString(this["ObjectName"]);
            }
        }
        public Int32? LineNumber
        {
            get
            {
                if (this["LineNumber"] == null) return null;
                return Convert.ToInt32(this["LineNumber"]);
            }
        }
        public Int32? NestLevel
        {
            get
            {
                if (this["NestLevel"] == null) return null;
                return Convert.ToInt32(this["NestLevel"]);
            }
        }
        public Int32? SPID
        {
            get
            {
                if (this["SPID"] == null) return null;
                return Convert.ToInt32(this["SPID"]);
            }
        }
        public String TextData
        {
            get
            {
                if (this["TextData"] == null) return null;
                return Convert.ToString(this["TextData"]);
            }
        }
        public DateTime? StartTime
        {
            get
            {
                if (this["StartTime"] == null) return null;
                return Convert.ToDateTime(this["StartTime"]);
            }
        }
        public Int64? RowCounts
        {
            get
            {
                if (this["RowCounts"] == null) return null;
                return Convert.ToInt64(this["RowCounts"]);
            }
        }
        public Int64? Duration
        {
            get
            {
                if (this["Duration"] == null) return null;
                return Convert.ToInt64(this["Duration"]);
            }
        }
        public DateTime? EndTime
        {
            get
            {
                if (this["EndTime"] == null) return null;
                return Convert.ToDateTime(this["EndTime"]);
            }
        }
        public Int64? Reads
        {
            get
            {
                if (this["Reads"] == null) return null;
                return Convert.ToInt64(this["Reads"]);
            }
        }
        public Int64? Writes
        {
            get
            {
                if (this["Writes"] == null) return null;
                return Convert.ToInt64(this["Writes"]);
            }
        }
        public String ApplicationName
        {
            get
            {
                if (this["ApplicationName"] == null) return null;
                return Convert.ToString(this["ApplicationName"]);
            }
        }
        public String DBUserName
        {
            get
            {
                if (this["DBUserName"] == null) return null;
                return Convert.ToString(this["DBUserName"]);
            }
        }
        public Int64? TransactionID
        {
            get
            {
                if (this["TransactionID"] == null) return null;
                return Convert.ToInt64(this["TransactionID"]);
            }
        }
        public Byte[] BinaryData
        {
            get
            {
                if (this["BinaryData"] == null) return null;
                return System.Text.Encoding.UTF8.GetBytes(this["BinaryData"].ToString());
            }
        }
        

    }
    public enum ResultAction
    {
        Add,
        Delete,
        Modify,
        Refresh
    }
    public class Result
    {
        public object Content { get; set; }
        public ResultAction Action { get; set; }
    }
    public delegate void ResultEventHandler(string sender,Result resultEvent);
    public interface IResultHandler
    {
        /// <summary>
        /// used to update UI
        /// </summary>
        event ResultEventHandler OnResultChange;
        /// <summary>
        /// handle trace event
        /// </summary>
        /// <param name="traceEvent"></param>
        void OnNewTraceEvent(TraceEvent traceEvent);
        /// <summary>
        /// 
        /// </summary>
        TraceDefinitionFile TraceDefinition { get; }
        /// <summary>
        /// 
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        void SaveResult(IResultSaveAdapter resultSaveAdapter);

    }
}
