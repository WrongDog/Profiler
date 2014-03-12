using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using TraceWrapper;

namespace AverageDurationHandler
{
   
    public class AverageDurationHandler:IResultHandler
    {
        private object resultlock = new object();
        private Dictionary<Regex, Func<string, bool>> regexs = new Dictionary<Regex, Func<string, bool>>();
        private int top;
        protected DataTable result;
        protected TraceDefinitionFile tracedefinition = TraceDefinitionFile.Common;
        protected Regex textDataMatchFilter;
        protected Regex textDataNotMatchFilter;
        public AverageDurationHandler(int keeptop,string textDataMatchFilterString,string textDataNotMatchFilterString)
        {
            TraceDefinitionFile.ColumnFilter filter = new TraceDefinitionFile.ColumnFilter(TraceColumn.TextData,true);
            TraceDefinitionFile.ColumnFilter filterDuration = new TraceDefinitionFile.ColumnFilter(TraceColumn.Duration, false);
            filterDuration.Operations.Add( new TraceDefinitionFile.ColumnFilter.Operation(TraceDefinitionFile.ColumnFilter.OperatorEnum.GreaterOrEqual,(Int64)1));
            tracedefinition.ColumnFilters.Add(filter);
            tracedefinition.ColumnFilters.Add(filterDuration);//1 e8 03 2 d0 07

            //tracedefinition =  TraceDefinitionFile.Common;
            //tracedefinition.Remove(TraceEvent.RPCCompleted);

            this.top = keeptop;
            result = InitResult();
            if(!string.IsNullOrWhiteSpace(textDataMatchFilterString)) textDataMatchFilter = new Regex(textDataMatchFilterString,RegexOptions.IgnoreCase);
            if (!string.IsNullOrWhiteSpace(textDataNotMatchFilterString)) textDataNotMatchFilter = new Regex(textDataNotMatchFilterString, RegexOptions.IgnoreCase);
            regexs.Add(new Regex(@"(?<key>[@]{0,1}[\w|\d]+?)[\s|\n]*[=|>|<]+[\s|\n]*(?<value>[\S|\s]+?)[,|and]{1}"),
            (s) =>
            {
                decimal decimalvalue = 0;
                Decimal.TryParse(s, out decimalvalue);
                return s.Contains('.') && decimalvalue.ToString() != s && !s.Contains("'");
            }
            );
      

            regexs.Add(new Regex(@"in[\s|\n]*[(]{1}(?<value>[\S|\s]+?)[)]{1}"),
                (s) =>
                {
                    return s.ToLower().Contains("select");
                }
            );
        }

        public void OnNewTraceEvent(TraceEvent traceEvent)
        {

            try
            {
                Int64 duration = Convert.ToInt64(traceEvent["Duration"].ToString());
                if (duration == 0) return;
                string textdata = traceEvent["TextData"].ToString();
                if (textDataMatchFilter != null && !textDataMatchFilter.IsMatch(textdata)) return;
                if (textDataNotMatchFilter != null && textDataNotMatchFilter.IsMatch(textdata)) return;
                textdata = FormatTextWithOutParameters(textdata);

                Int64 reads;
                Int64 writes;
                Int64 cpu;
                Int64.TryParse(traceEvent["Reads"].ToString(), out reads);
                Int64.TryParse(traceEvent["Writes"].ToString(), out writes);
                Int64.TryParse(traceEvent["CPU"].ToString(), out cpu);

                lock (resultlock)
                {
                    DataRow[] drarray = (from dr in result.Rows.Cast<DataRow>()
                                         where dr["TextData"].ToString() == textdata 
                                         select dr).ToArray<DataRow>();

                    if (drarray != null && drarray.Length > 0)
                    {
                        if (Convert.ToDateTime(drarray[0]["LastCaptureTime"])!= Convert.ToDateTime(traceEvent["StartTime"]) ||
                           Convert.ToInt64(drarray[0]["LastDuration"]) != duration)
                        {
                            drarray[0]["AverageDuration"] = (Convert.ToInt64(drarray[0]["Count"]) * Convert.ToInt64(drarray[0]["AverageDuration"]) + duration)
                                / (Convert.ToInt64(drarray[0]["Count"]) + 1);

                            drarray[0]["AverageReads"] = (Convert.ToInt64(drarray[0]["Count"]) * Convert.ToInt64(drarray[0]["AverageReads"]) + reads)
                               / (Convert.ToInt64(drarray[0]["Count"]) + 1);

                            drarray[0]["AverageWrites"] = (Convert.ToInt64(drarray[0]["Count"]) * Convert.ToInt64(drarray[0]["AverageWrites"]) + writes)
                               / (Convert.ToInt64(drarray[0]["Count"]) + 1);

                            drarray[0]["AverageCPU"] = (Convert.ToInt64(drarray[0]["Count"]) * Convert.ToInt64(drarray[0]["AverageCPU"]) + cpu)
                              / (Convert.ToInt64(drarray[0]["Count"]) + 1);


                            drarray[0]["Count"] = Convert.ToInt64(drarray[0]["Count"]) + 1;

                            if (Convert.ToInt64(drarray[0]["MinDuration"]) > duration) drarray[0]["MinDuration"] = duration;
                            if (Convert.ToInt64(drarray[0]["MaxDuration"]) < duration) drarray[0]["MaxDuration"] = duration;

                            if (Convert.ToInt64(drarray[0]["MinReads"]) > reads) drarray[0]["MinReads"] = reads;
                            if (Convert.ToInt64(drarray[0]["MaxReads"]) < reads) drarray[0]["MaxReads"] = reads;

                            if (Convert.ToInt64(drarray[0]["MinWrites"]) > writes) drarray[0]["MinWrites"] = writes;
                            if (Convert.ToInt64(drarray[0]["MaxWrites"]) < writes) drarray[0]["MaxWrites"] = writes;

                            if (Convert.ToInt64(drarray[0]["MinCPU"]) > cpu) drarray[0]["MinCPU"] = cpu;
                            if (Convert.ToInt64(drarray[0]["MaxCPU"]) < cpu) drarray[0]["MaxCPU"] = cpu;

                            drarray[0]["LastCaptureTime"] = Convert.ToDateTime(traceEvent["StartTime"]);
                            drarray[0]["LastDuration"] = duration;
                        }

                    }
                    else
                    {

                        result.Rows.Add(new object[]{
                        textdata,
                        1,
                        duration,
                        duration,
                        duration,
                        Convert.ToDateTime(traceEvent["StartTime"]),
                        Convert.ToDateTime(traceEvent["StartTime"]),
                        duration,
                        reads,
                        reads,
                        reads,
                        writes,
                        writes,
                        writes,
                        cpu,
                        cpu,
                        cpu

                    });

                        if (result.Rows.Count > top)
                        {
                            var drdel = from dr in result.Rows.Cast<DataRow>()
                                        orderby Convert.ToDecimal(dr["AverageDuration"]) ascending
                                        select dr;
                            result.Rows.Remove(drdel.ToList<DataRow>()[0]);
                        }
                    }
                }
                OnResultChange(this.GetType().Name, new Result() { Action = ResultAction.Refresh, Content = result });
            }
            catch
            {
            }

        }
        private DataTable InitResult()
        {
            DataTable result = new DataTable("AverageDuration");
            result.Columns.Add("TextData");
            result.Columns.Add("Count",typeof(Int64));
            result.Columns.Add("AverageDuration", typeof(Int64));
            result.Columns.Add("MinDuration", typeof(Int64));
            result.Columns.Add("MaxDuration", typeof(Int64));
            result.Columns.Add("FirstCaptureTime",typeof(DateTime));
            result.Columns.Add("LastCaptureTime", typeof(DateTime));
            result.Columns.Add("LastDuration", typeof(Int64));
            result.Columns.Add("AverageReads", typeof(Int64));
            result.Columns.Add("MinReads", typeof(Int64));
            result.Columns.Add("MaxReads", typeof(Int64));
            result.Columns.Add("AverageWrites", typeof(Int64));
            result.Columns.Add("MinWrites", typeof(Int64));
            result.Columns.Add("MaxWrites", typeof(Int64));
            result.Columns.Add("AverageCPU", typeof(Int64));
            result.Columns.Add("MinCPU", typeof(Int64));
            result.Columns.Add("MaxCPU", typeof(Int64));
            return result;
        }
        //protected
        protected string FormatTextWithOutParameters(string textData)
        {

            string header = string.Empty;
            string condition = string.Empty;
            if (!textData.StartsWith("exec", StringComparison.InvariantCultureIgnoreCase))
            {
                int havingidx = textData.IndexOf("having ", StringComparison.InvariantCultureIgnoreCase);
                int whereidx = textData.IndexOf("where ", StringComparison.InvariantCultureIgnoreCase);
                if (whereidx > 0)
                {
                    header = textData.Substring(0, whereidx - 1);
                    condition = textData.Substring(whereidx, textData.Length - whereidx) + ",";
                }
                else
                {
                    condition = textData + ",";
                }
            }
            else
            {
                condition = textData + ",";
            }

            foreach (KeyValuePair<Regex, Func<string, bool>> kvpair in regexs)
            {
                condition = kvpair.Key.ReplaceGroup(condition, "value", ".", kvpair.Value);
            }
            return header+condition;
            
            
        }
        public void SaveResult(IResultSaveAdapter resultSaveAdapter)
        {
            resultSaveAdapter.Save(this.Name + DateTime.UtcNow.ToString("yyyy_MM_dd HH_mm_ss"), result);
        }



        public event ResultEventHandler OnResultChange;


        public TraceDefinitionFile TraceDefinition
        {
            get { return tracedefinition; }
        }
        public string Name
        {
            get;
            set;
        }
    }
}
