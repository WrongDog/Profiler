using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
namespace TraceWrapper
{
    public class TraceWrapperServerSide:TraceWrapperBase
    {
        protected string fileTRC = string.Empty;
        protected ConnectionInfo connectionInfo;
        public TraceWrapperServerSide(ConnectionInfo connectionInfo,string servertrcfile, List<IResultHandler> handlers = null)
            : base(handlers)
        {            
            fileTRC = servertrcfile;
            this.connectionInfo = connectionInfo;
        }
        /// <summary>
        /// </summary>
        /// <param name="trcFile"></param>
        /// <returns></returns>
        public string AsServerSideStoredProcedureStart(TraceDefinitionFile tracefile,string trcFile)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("            declare @TraceId int \n");
            sb.Append("            declare @maxfilesize  bigint \n");
            sb.Append("            set @maxfilesize = 20 \n");
            sb.Append("            -- create the trace \n");
            sb.Append("            exec sp_trace_create @TraceId output, 2, N'" + trcFile + "',@maxfilesize \n");
            sb.Append("            -- set event and columns to trace \n");
            foreach (KeyValuePair<TraceEventEnum, List<TraceColumn>> kvpair in tracefile)
            {
                foreach (TraceColumn tc in kvpair.Value)
                {
                    sb.Append("            exec sp_trace_setevent @TraceId, " + ((int)kvpair.Key).ToString() + "," + ((int)tc).ToString() + ",1 \n");
                }
            }
            sb.Append("            -- start the trace \n");
            sb.Append("            exec sp_trace_setstatus @TraceId, 1 \n");
            sb.Append("            -- check status for our trace \n");
            sb.Append("            select value from fn_trace_getinfo(@TraceId) where property =5 \n");
            sb.Append("            go \n");
            return sb.ToString();

        }

        public string AsServerSideStoredProcedureStop(string trcFile)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("declare @TraceToStop int");
            sb.Append("select @TraceToStop = TraceId from fn_trace_getinfo(default) where value = N'" + trcFile+".trc" + "'");
            sb.Append("exec sp_trace_setstatus @TraceToStop, 0");
            sb.Append("exec sp_trace_setstatus @TraceToStop, 2");
            return sb.ToString();
        }
        public override void Start()
        {
            string sp = AsServerSideStoredProcedureStart(this.TraceDefinitionFile,fileTRC);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionInfo.AsConnectionString))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(sp, conn);
                        string state = Convert.ToString(command.ExecuteScalar());
                        if (state == "1")
                        {
                            this.State = TraceState.Started;
                        }
  
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

        }

        public override void Stop()
        {
            try
            {
                string sp = AsServerSideStoredProcedureStop(fileTRC);
                using (SqlConnection conn = new SqlConnection(connectionInfo.AsConnectionString))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(sp, conn);
                        command.ExecuteNonQuery();
                        this.State = TraceState.Stopped;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
    }
}
