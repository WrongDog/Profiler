using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraceWrapper;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace TraceWrapper
{
    public class TraceWrapperSQLDB : TraceWrapperProgressBase
    {
        protected SqlConnection connection;
        protected string traceTable;
        protected string traceTableCriteria;
        protected Thread readerthread;
        public TraceWrapperSQLDB(ConnectionInfo connectionInfo)
            : base(null)
        {
            LoadSettings(connectionInfo);
        }
        public TraceWrapperSQLDB(ConnectionInfo connectionInfo, List<IResultHandler> handlers):base(handlers)
        {
            LoadSettings(connectionInfo);
        }
        private void LoadSettings(ConnectionInfo connectionInfo)
        {
            this.traceTable = connectionInfo["SourceTable"];
            this.traceTableCriteria = connectionInfo["Criteria"];
            this.connection = new SqlConnection(connectionInfo.AsConnectionString);
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
                connection.Open();
                string commandText = " from " + traceTable;
                if (!string.IsNullOrEmpty(this.traceTableCriteria)) commandText += " where " + this.traceTableCriteria;
                ////may out of memery
                //SqlDataAdapter da = new SqlDataAdapter(commandText, connection);
                //DataTable result = new DataTable("result");
                //da.Fill(result);
                //foreach (DataRow dr in result.Rows)
                //{
                //    base.HandleEventData(dr);
                //}

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "select count(*) "+commandText;
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "select * " + commandText;
                int index = 0;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    DataTable schema = TraceWrapper.TraceWrapperTraceServer.FromTraceReaderSchema( reader.GetSchemaTable());    
                    while (reader.Read())
                    {
                        schema = (DataTable)schema.Clone();
                        index++;
                        UpdateProgress(index, count);
                        base.HandleEventData((TraceEvent)TraceWrapper.TraceWrapperTraceServer.ReaderToDataRow(reader, schema));
                    }
                }
                
            }
            catch (Exception ex)
            {
                base.FireException(ex.Message);
            }
            finally
            {
                connection.Close();
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
            if (connection != null && connection.State == ConnectionState.Open) connection.Close();
        }
    }
}
