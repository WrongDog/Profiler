using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Data;
using TraceWrapper;
using DBengine;

namespace DataBaseResultSaveAdapter
{
   
    public class DataBaseResultSaveAdapter : ResultSaveAdapterBase
    {
        protected string connectionstring;
        protected DataBaseType databasetype;
        public DataBaseResultSaveAdapter(string connectionstring, string databasetype)
        {
            this.connectionstring = connectionstring;
            try
            {
                this.databasetype = (DataBaseType)Enum.Parse(typeof(DataBaseType), databasetype, true);
            }
            catch
            {
                this.databasetype = DataBaseType.unknown;
            }
        }
        protected override void Worker(string tag, string result)
        {
            DBengine.DBengine dbeng = new DBengine.DBengine();
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(result)));

                dbeng.DBConnect(connectionstring, databasetype);
                foreach (DataTable dt in ds.Tables)
                {
                    dbeng.UpdateTable(dt);
                }
                
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
            finally{
                dbeng.Close();
            }
        }
    }
}
