using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.Data;
namespace TraceWrapper
{
   
    public enum TraceState
    {
        NotInitiated,
        Started,
        Paused,
        Stopped
    }
    public delegate void ExceptionNotifierDele(string message);
    public delegate void StateChangeNotifierDele(TraceState newstate);
    public abstract class TraceWrapperBase
    {
        public event ExceptionNotifierDele OnException;
        public event StateChangeNotifierDele OnStateChanged;
        public void FireException(string message)
        {
            if(this.OnException!= null) this.OnException(message);
        }
        public TraceDefinitionFile TraceDefinitionFile { get; set; }
        public TraceWrapperBase(List<IResultHandler> handlers = null)
        {
            #region Handlers
            Handlers = new List<IResultHandler>();
            if (handlers != null)
            {
                foreach (IResultHandler handler in handlers)
                {
                    if (Handlers.Count<IResultHandler>((h) => h.GetType() == handler.GetType()) == 0)
                    {
                        Handlers.Add(handler);
                    }
                }
            }
            else
            {
                string prefix = "IResultHandler";
                foreach (string handlername in ConfigurationManager.AppSettings.AllKeys
                         .Where(key => key.StartsWith(prefix))
                         .ToArray())
                {

                    IResultHandler handler = ReflectionUtil.CreateInstance<IResultHandler>(ConfigurationManager.AppSettings[handlername]); //CreateHandler(handlerSetting);
                    if (handler != null) handler.Name = handlername.Substring(prefix.Length, handlername.Length - prefix.Length);
                    if (handler != null && Handlers.Count<IResultHandler>((h) => h.Name == handler.Name) == 0)
                    {
                        Handlers.Add(handler);
                    }
                }

                if (Handlers.Count == 0) Handlers.Add(new DefaultEventHandler());
            }
            #endregion
            #region TraceDefinitionFile
            TraceDefinitionFile = new TraceDefinitionFile();
            foreach (IResultHandler handler in Handlers)
            {
                TraceDefinitionFile.Merge(handler.TraceDefinition);
            }
            #endregion
            #region IResultSaveAdapter
            string resultSaveAdapterSetting = ConfigurationManager.AppSettings["IResultSaveAdapter"];
            if (!string.IsNullOrEmpty(resultSaveAdapterSetting))
            {
                resultSaveAdapter = ReflectionUtil.CreateInstance<IResultSaveAdapter>(resultSaveAdapterSetting);
                if (resultSaveAdapter == null) resultSaveAdapter = new FileResultSaveAdapter();
            }
            #endregion
        }
        public abstract void Start();
        public abstract void Stop();

        protected TraceState state;
        public TraceState State
        {
            get
            {
                return state;
            }
            protected set
            {
                state = value;
                if (OnStateChanged != null) OnStateChanged(value);
            }
        }

        #region SaveResult
        protected IResultSaveAdapter resultSaveAdapter = new FileResultSaveAdapter();
        public TimeSpan Interval { get; set; }
        protected DateTime sentinal = DateTime.MaxValue;
        #endregion
        #region Handlers
        public List<IResultHandler> Handlers { get; set; }
        
        
        protected void HandleEventData(TraceEvent traceevent)
        {
            if (Handlers != null)
            {
                try
                {
                    if (Interval.Ticks != 0)
                    {
                        if (sentinal == DateTime.MaxValue) sentinal = DateTime.UtcNow;
                        if (DateTime.UtcNow.Subtract(sentinal) > Interval)
                        {
                            foreach (IResultHandler handler in Handlers)
                            {
                                try
                                {
                                    handler.SaveResult(resultSaveAdapter);
                                }
                                catch
                                {
                                }
                            }
                            sentinal = DateTime.UtcNow;
                        }
                    }
                   
                    if (traceevent.EventClass.HasValue)
                    {
                        foreach (IResultHandler handler in Handlers)
                        {
                            if (handler.TraceDefinition.ContainsKey(traceevent.EventClass.Value)) handler.OnNewTraceEvent(traceevent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    FireException(ex.Message);
                }

            }
        }
        #endregion
    }
}
