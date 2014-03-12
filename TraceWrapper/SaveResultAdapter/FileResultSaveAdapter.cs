using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace TraceWrapper
{
    public class FileResultSaveAdapter : ResultSaveAdapterBase
    {
    
        protected override void Worker(string tag, string result)
        {
            try
            {
                using (StreamWriter sout = new StreamWriter(tag + ".xml"))
                {
                    sout.WriteLine(result);
                    sout.Close();
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }
    }
}
