using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TraceWrapper
{
    //TraceWrapper.TraceWrapper trace = new TraceWrapper.TraceWrapper(new TraceWrapper.ConnectionInfo() { ServerName = "localhost", UseIntegratedSecurity = true },
    //    "Standard.tdf");
    //trace.Start();
    //TraceWrapper.TraceManager manager = new TraceWrapper.TraceManager(50);
    //manager.Watch(trace);
    /// <summary>
    /// to do : watch status for trace 
    /// </summary>
    public class TraceManager
    {
        private float maxMemory;
        public TraceManager(float maxMemoryMB)
        {
            this.maxMemory = maxMemoryMB*1024 * 1024;
        }
        public void Watch(TraceWrapperTraceServer trace)
        {
            PerformanceCounter counter = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);
      
            while(trace.State== TraceState.Started ){
                    if (counter.NextValue() > maxMemory)
                    {
                        System.Diagnostics.Trace.WriteLine(String.Format("Memory usage exceded:{0} Current:{1}", maxMemory, counter.NextValue()));
                        trace.Stop();
                        GC.Collect();
                       // if (trace.Handlers != null) foreach (IResultHandler handler in trace.Handlers) handler.SaveResult();//System.Threading.Tasks.Parallel.ForEach<IResultHandler>(trace.Handlers,(handler)=>handler.SaveResult());
                        trace.Start();
                        System.Threading.Thread.Sleep(10000);
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine(String.Format("Memory usage {0} ", counter.NextValue()));
                    }
                    System.Threading.Thread.Sleep(10000);
            }

        }
    }
}
