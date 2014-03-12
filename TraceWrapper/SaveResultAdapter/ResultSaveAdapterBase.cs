using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Threading.Tasks;

namespace TraceWrapper
{
    public abstract class ResultSaveAdapterBase:IResultSaveAdapter
    {
        public virtual void Save(string tag, object result)
        {
            try
            {

                using (MemoryStream ms = new MemoryStream())
                {

                    if (result.GetType() == typeof(DataTable))
                    {

                        ((DataTable)result).WriteXml(ms, XmlWriteMode.WriteSchema);
                        ((DataTable)result).Clear();
                    }
                    else if (result.GetType() == typeof(DataSet))
                    {
                        ((DataSet)result).WriteXml(ms,XmlWriteMode.WriteSchema);
                        ((DataSet)result).Clear();
                    }
                    Task.Factory.StartNew(() => Worker(tag, System.Text.Encoding.UTF8.GetString(ms.ToArray())));
                    ms.Close();
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }
        protected abstract void Worker(string tag, string result);
    }
}
