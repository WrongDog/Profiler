using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.IO;
using System.Xml.Linq;
using Microsoft.SqlServer.Management.Smo;
using TraceWrapper;

namespace TraceWrapperSMO
{
    /// <summary>
    /// using wmi very unstable but can trace deadlock
    /// not able to trace batchcompleted odd
    /// </summary>
    public class TraceWrapperSMO:TraceWrapperBase
    {
        protected Server server;
        protected ServerTraceEventSet eventSetServer;
        protected DatabaseEventSet eventSetDatabase;
        public TraceWrapperSMO(ConnectionInfo connectionInfo, List<IResultHandler> handlers = null)
            : base(handlers)
        {
            server = new Server(connectionInfo.ServerName);



            eventSetServer = new ServerTraceEventSet(ServerTraceEvent.LockDeadlock,
             ServerTraceEvent.LockDeadlockChain,
             ServerTraceEvent.DeadlockGraph);
            //TraceDefinitionFile to eventSetServer


            //eventSetServer = new ServerTraceEventSet();
            //foreach (PropertyInfo pi in typeof(ServerTraceEvent).GetProperties())
            //{
            //    if (pi.PropertyType == typeof(ServerTraceEvent))
            //    {
            //        eventSetServer.Add((ServerTraceEvent)pi.GetValue(null, null));
            //    }
            //}
           
              
        }
        public override void Start()
        {
            server.Events.SubscribeToEvents(eventSetServer, OnEvents);

            server.Events.StartEvents();
        }
        private void OnEvents(object sender, ServerEventArgs e)
        {
            DataTable result = GetResultDataTable();
            List<object> data = new List<object>();
            foreach (DataColumn dc in result.Columns)
            {
                EventProperty ep = e.Properties.FirstOrDefault((prop) => prop.Name == dc.ColumnName);
                if (ep != null)
                {
                    //if (dc.DataType == typeof(DateTime))
                    //{
                    //    DateTime datetime;
                    //    DateTime.TryParse(ep.Value.ToString(), out datetime);
                    //    data.Add(datetime);
                    //}
                    //else
                    //{
                    //    data.Add(ep.Value);
                    //}
                    data.Add(ep.Value);
                }
                else
                {
                    data.Add(null);
                }
            }
            //foreach (EventProperty ep in e.Properties.ToList<EventProperty>())
            //{
            //    if (ep.Value != null)
            //    {
            //        if (ep.Value.GetType() == typeof(System.Byte[]))
            //        {
            //            result.Columns.Add(ep.Name);
            //            data.Add(Convert.ToBase64String((Byte[])ep.Value));
            //        }
            //        else
            //        {
            //            result.Columns.Add(ep.Name, ep.Value.GetType());
            //            data.Add(ep.Value);
            //        }
            //    }
            //    else
            //    {
            //        result.Columns.Add(ep.Name);
            //        data.Add(null);
            //    }
            //}
            result.Rows.Add(data.ToArray());
            if (Handlers != null)
            {
                foreach (IResultHandler handler in Handlers) handler.OnNewTraceEvent((TraceEvent)result.Rows[0]);
                //why not working?locked?
                
            }
        }
        public override void Stop()
        {
            server.Events.StopEvents();

            server.Events.UnsubscribeFromEvents(eventSetServer);
        }
        private DataTable GetResultDataTable()
        {
            DataTable result = new DataTable();
            result.Columns.Add("EventClass");
            result.Columns.Add("TextData");
            result.Columns.Add("ApplicationName");
            result.Columns.Add("NTUserName" );
            result.Columns.Add("LoginName");
            result.Columns.Add("CPU",typeof(int) );
            result.Columns.Add("Reads", typeof(Int64));
            result.Columns.Add("Writes", typeof(Int64));
            result.Columns.Add("Duration", typeof(Int64));
            result.Columns.Add("ClientProcessID", typeof(int));
            result.Columns.Add("SPID", typeof(int));
            //result.Columns.Add("StartTime", typeof(DateTime));
            //result.Columns.Add("EndTime", typeof(DateTime));
            result.Columns.Add("StartTime");
            result.Columns.Add("EndTime");
            result.Columns.Add("BinaryData" );

            return result;
        }
        #region deadlock
        private static void OnDeadlock(object sender, ServerEventArgs e)
        {
            switch (e.EventType)
            {
                case EventType.DeadlockGraph:
                    DeadlockGraph(e);
                    break;
                case EventType.LockDeadlockChain:
                    LockDeadlockChain(e);
                    break;
                case EventType.LockDeadlock:
                    Console.WriteLine("{0:dd/MM/yyyy HH:mm}", e.PostTime);
                    Console.WriteLine("SPID {0}", e.Spid);
                    break;
            }

            Console.WriteLine();
        }

        private static void LockDeadlockChain(ServerEventArgs e)
        {
            var textData = e.Properties.FirstOrDefault(prop => prop.Name == "TextData");

            if (textData == null) return;

            Console.WriteLine("{0:dd/MM/yyyy HH:mm}", e.PostTime);
            Console.WriteLine(((string)textData.Value).TrimEnd());
        }

        private static void DeadlockGraph(ServerEventArgs e)
        {
            var path = string.Format(@"C:\TEMP\{0}.xdl", Guid.NewGuid());

            var textData = e.Properties.FirstOrDefault(prop => prop.Name == "TextData");

            if (textData == null) return;

            var xml = XDocument.Parse(((string)textData.Value))
                .Elements("TextData")
                .Elements("deadlock-list")
                .FirstOrDefault();

            if (xml == null) return;

            using (var file = new StreamWriter(path)) file.Write(xml.ToString(SaveOptions.DisableFormatting));

        }
        #endregion
    }
}
