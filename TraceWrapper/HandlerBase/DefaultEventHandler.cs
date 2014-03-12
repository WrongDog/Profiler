using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TraceWrapper
{
    public class DefaultEventHandler:IResultHandler
    {
        public event ResultEventHandler OnResultChange;
        protected TraceDefinitionFile traceDefinition = TraceDefinitionFile.Common;
        protected Regex textDataMatchFilter;
        protected Regex textDataNotMatchFilter;
        public DefaultEventHandler()
        {
            this.Name = "DefaultEventHandler";
        }
        public DefaultEventHandler(string textDataMatchFilterString, string textDataNotMatchFilterString)
            : this()
        {
            if (!string.IsNullOrWhiteSpace(textDataMatchFilterString)) textDataMatchFilter = new Regex(textDataMatchFilterString, RegexOptions.IgnoreCase);
            if (!string.IsNullOrWhiteSpace(textDataNotMatchFilterString)) textDataNotMatchFilter = new Regex(textDataNotMatchFilterString, RegexOptions.IgnoreCase);
        }

        public void OnNewTraceEvent(TraceEvent traceEvent)
        {
            if (textDataMatchFilter != null || textDataNotMatchFilter != null)
            {
                string textdata = traceEvent.TextData;// traceEvent["TextData"].ToString();
                if (textDataMatchFilter != null && !textDataMatchFilter.IsMatch(textdata)) return;
                if (textDataNotMatchFilter != null && textDataNotMatchFilter.IsMatch(textdata)) return;
            }
            if (OnResultChange != null) OnResultChange(this.GetType().Name, new Result() { Content = (System.Data.DataRow)traceEvent, Action = ResultAction.Add });
        }

        public void SaveResult(IResultSaveAdapter resultSaveAdapter)
        {
            //nothing
        }


        public TraceDefinitionFile TraceDefinition
        {
            get
            {
                return traceDefinition;
            }
 
        }


        public string Name
        {
            get;
            set;
        }
    }
}
