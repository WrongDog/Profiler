using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraceWrapper;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace TraceWrapperSQLDB
{
    public class TraceWrapperSQLDB : TraceWrapperBase
    {
        protected SqlConnection connection;
        protected string traceTable;
        protected Thread readerthread;
        public TraceWrapperSQLDB(ConnectionInfo connectionInfo, List<IResultHandler> handlers = null):base(handlers)
        {
            this.traceTable = connectionInfo.SourceTable;
            this.connection = new SqlConnection(connectionInfo.AsConnectionString);
           
        }
        public override void Start()
        {
           
             readerthread = new Thread(this.Read);
             readerthread.Start();
           
        }
        private void Read()
        {
            try
            {
                connection.Open();
                SqlDataAdapter da = new SqlDataAdapter("select * from " + traceTable, connection);
                DataTable result = new DataTable("result");
                da.Fill(result);
                connection.Close();
                foreach (DataRow dr in result.Rows)
                {
                    base.HandleEventData(dr);
                }
            }
            catch (Exception ex)
            {
                base.FireException(ex.Message);
            }


        }
        public override void Stop()
        {
            if (readerthread != null && readerthread.ThreadState == ThreadState.Running)
            {
                readerthread.Abort();
            }
            if (connection != null && connection.State == ConnectionState.Open) connection.Close();
        }
    }
}
