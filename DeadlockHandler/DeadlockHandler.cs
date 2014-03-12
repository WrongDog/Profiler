using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraceWrapper;

namespace DeadlockHandler
{
    public class DeadlockHandler:IResultHandler
    {
        public event ResultEventHandler OnResultChange;
        protected TraceDefinitionFile traceDefinition = TraceDefinitionFile.Deadlock;
        public void OnNewTraceEvent(TraceEvent traceEvent)
        {
            //throw new NotImplementedException();
            if (OnResultChange != null) OnResultChange(this.GetType().Name, new Result() { Content = traceEvent, Action = ResultAction.Add });
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
