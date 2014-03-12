using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using Microsoft.SqlServer.Management.Trace;

namespace TraceWrapper
{
    public class TraceWrapperTraceFile:TraceWrapperBase
    {
        protected string traceFileName;
        protected Thread readerthread;
        public TraceWrapperTraceFile(ConnectionInfo connectionInfo)
            : base(null)
        {
            this.traceFileName = connectionInfo["TraceFile"];
          

        }
        public TraceWrapperTraceFile(ConnectionInfo connectionInfo, List<IResultHandler> handlers)
            : base(handlers)
        {
            this.traceFileName = connectionInfo["TraceFile"];
           
           
        }
        public override void Start()
        {
            this.State = TraceState.Started;
             readerthread = new Thread(this.Read);
             readerthread.Start();
           
        }
        private void Read()
        {
            try
            {

                TraceFile traceFile = new TraceFile();
                traceFile.InitializeAsReader(this.traceFileName);
                while (traceFile.Read())
                {
                    base.HandleEventData((TraceEvent)TraceWrapperTraceServer.ReaderToDataRow(traceFile, TraceWrapperTraceServer.FromTraceReaderSchema(traceFile.GetSchemaTable())));
                }
                
            }
            catch (Exception ex)
            {
                base.FireException(ex.Message);
            }
            finally
            {
                this.State = TraceState.Stopped;
            }


        }
        public override void Stop()
        {
            if (readerthread != null && readerthread.ThreadState == ThreadState.Running)
            {
                readerthread.Abort();
            }
            this.State = TraceState.Stopped;
            
        }
    }
}
