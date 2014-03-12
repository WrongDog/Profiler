using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Trace;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.Threading;

namespace TraceWrapper
{
   

    /// <summary>
    /// 1.if duration is short it may get two same completed records instead of 1 start and 1 completed
    /// 2. not able to trace deadlock and the one survived will be recieved twice
    /// </summary>
    public class TraceWrapperTraceServer : TraceWrapperBase
    {
        private SqlConnectionInfo connectionInfo;
        private string traceDefinitionFile;
        private TraceServer trace = new TraceServer();
        private BackgroundWorker backWorker= new BackgroundWorker();
        protected DataTable schema ;
        public TraceWrapperTraceServer(ConnectionInfo connectionInfo, List<IResultHandler> handlers = null):base(handlers)
        {
            this.connectionInfo = FromConnectionInfo(connectionInfo);

        
            TraceDefinitionFile.SaveAsTDF("mytrace.tdf");
            traceDefinitionFile = "mytrace.tdf";

            this.State = TraceState.NotInitiated;
            
            this.backWorker.WorkerReportsProgress = true;
            this.backWorker.WorkerSupportsCancellation = true;
            this.backWorker.ProgressChanged += new ProgressChangedEventHandler(backWorker_ProgressChanged);
            this.backWorker.DoWork += new DoWorkEventHandler(backWorker_DoWork);
           
        }
      
        protected virtual SqlConnectionInfo FromConnectionInfo(ConnectionInfo info)
        {
            SqlConnectionInfo connectionInfo = new SqlConnectionInfo();
            connectionInfo.ServerName = info.ServerName;
            connectionInfo.UseIntegratedSecurity = info.UseIntegratedSecurity;
            if (!connectionInfo.UseIntegratedSecurity)
            {
                connectionInfo.UserName = info.UserName;
                connectionInfo.Password = info.Password;
            }
            return connectionInfo;
        }
        void backWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            StartTrace();
        }

        void backWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object[] userState = e.UserState as object[];
            DataRow traceevent = userState[0] as DataRow;
            try
            {
                DataTable dt = (DataTable)traceevent.Table.Clone();
                base.HandleEventData((TraceEvent)dt.Rows.Add(traceevent.ItemArray));
                //base.HandleEventData(traceevent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        public static DataTable FromTraceReaderSchema(DataTable schema)
        {
            DataTable result = new DataTable();
            foreach (DataRow dr in schema.Rows)
            {
                result.Columns.Add(dr["ColumnName"].ToString(), Type.GetType(dr["DataType"].ToString()));
                //if (Type.GetType(dr["DataType"].ToString()) != typeof(System.Byte[]))
                //{
                //    result.Columns.Add(dr["ColumnName"].ToString(), Type.GetType(dr["DataType"].ToString()));
                //}
                //else
                //{
                //    result.Columns.Add(dr["ColumnName"].ToString());
                //}
            }
            return result;
        }
        public static DataRow ReaderToDataRow(IDataReader traceReader, DataTable schema)
        {
            

            
            object[] datarow = new object[schema.Columns.Count];
            int columnidx = 0;
            foreach (DataColumn dc in schema.Columns)
            {
                try
                {
                    //if (dc.DataType == typeof(System.Byte[]) && traceReader[dc.ColumnName] != null)
                    //{
                        
                    //    datarow[columnidx] = Convert.ToBase64String((Byte[])traceReader[dc.ColumnName]);

                    //}
                    //else
                    //{
                        datarow[columnidx] = traceReader[dc.ColumnName];
                    //}
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }
                finally
                {
                    columnidx++;
                }
            }
            schema.Rows.Add(datarow);
            return schema.Rows[0];
        }
        public override void Start()
        {
            if (!this.backWorker.IsBusy) this.backWorker.RunWorkerAsync();
        }
        protected void StartTrace()
        {
            try
            {
                trace = new TraceServer();
                trace.InitializeAsReader(connectionInfo, traceDefinitionFile);
                object[] userState = new object[2];
                userState[0] = this.trace;
                this.State = TraceState.Started;
                DataTable schema=null;
                while (this.trace.Read())
                {
                    try
                    {
                        if (schema == null) schema = FromTraceReaderSchema(this.trace.GetSchemaTable());
                        schema = (DataTable)schema.Clone();
                        //schmea.Clear();
                        //schema.AcceptChanges();
                        //schema = FromTraceReaderSchema(this.trace.GetSchemaTable());
                        userState[0] = ReaderToDataRow(this.trace, schema);
                        userState[1] = this.State;
                        this.backWorker.ReportProgress(0, userState);
                    }
                    catch (Exception ex)//System.AccessViolationException in Microsoft.SqlServer.ConnectionInfoExtended.dll
                    {//Microsoft.SqlServer.Management.Trace.SqlTraceException
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }
                this.State = TraceState.Stopped;
                userState[1] = this.State;
                
            }
            catch (Exception ex)
            {
                trace = null;
                this.State = TraceState.NotInitiated;
                throw ex;
            }
        }
        /// <summary>
        /// not able to restart
        /// </summary>
        [Obsolete]
        public void Pause()
        {
            try
            {
                if (trace != null) trace.Pause();
                
                this.State = TraceState.Paused;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// not working
        /// </summary>
        [Obsolete]
        public void Restart()
        {
            try
            {
                if (trace != null) trace.Restart();
                this.State = TraceState.Started;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override void Stop()
        {
            try
            {
                if (trace != null)
                {
                    trace.Stop();
                    trace.Close();
                }
                this.backWorker.CancelAsync();
                this.State = TraceState.Stopped;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public event WriteNotifyEventHandler WriteNotifyEventHandler;
        // Optional Notify handler
        //void writer_WriteNotify(object sender, TraceEventArgs args)
        //{
        //    this.toolStripStatusLabel1.Text = string.Format("Writing: {0}", args.CurrentRecord[0]);
        //    args.SkipRecord = false;
        //}
        public void Save(string filename)
        {
            TraceFile writer = new TraceFile();
            writer.InitializeAsWriter(this.trace, filename);
            //if (WriteNotifyEventHandler != null) writer.WriteNotify += WriteNotifyEventHandler;
            while (writer.Write()) ;
            writer.Close();
        }
    }
}
