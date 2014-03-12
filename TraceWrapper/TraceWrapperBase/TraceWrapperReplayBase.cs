using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceWrapper
{
    public abstract class TraceWrapperProgressBase:TraceWrapperBase
    {
        public delegate void ProgressUpdateDele(int current, int max);
        public event ProgressUpdateDele ProgressUpdated;
        public virtual void UpdateProgress(int current, int max)
        {
            if (ProgressUpdated != null) ProgressUpdated(current, max);
        }
        public TraceWrapperProgressBase(List<IResultHandler> handlers = null)
            : base(handlers)
        {
        }
    }
}
