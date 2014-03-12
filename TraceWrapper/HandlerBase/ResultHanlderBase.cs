using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceWrapper
{
    /// <summary>
    /// to do: allow each handler to have its own interval and save adapter
    /// </summary>
    public abstract class ResultHanlderBase:IResultHandler
    {

        public event ResultEventHandler OnResultChange;

        public abstract void OnNewTraceEvent(TraceEvent traceEvent);
      
        public abstract TraceDefinitionFile TraceDefinition
        {
            get ;
        }
        public string Name
        {
            get;
            set;
        }
        public void SaveResult( IResultSaveAdapter resultSaveAdapter)
        {
            throw new NotImplementedException();
        }
    }
}
