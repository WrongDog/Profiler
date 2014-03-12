using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceWrapper
{
    public interface IResultSaveAdapter
    {
        void Save(string tag,object result);
    }
}
