using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;

namespace TraceWrapper
{
    public abstract class TraceWrapperFactory
    {
        public static TraceWrapperBase Create(ConnectionInfo connection)
        {
            string prefix="ConnectionInfoExtended";
            foreach (string Extended in ConfigurationManager.AppSettings.AllKeys
                            .Where(key => key.StartsWith(prefix))                         
                            .ToArray())
            {

                connection[Extended.Substring(prefix.Length, Extended.Length - prefix.Length)] = ConfigurationManager.AppSettings[Extended];
               
            }
            //string dataBase = ConfigurationManager.AppSettings["DataBase"];
            //if (!string.IsNullOrEmpty(dataBase)) connection.DataBase = dataBase;
            //string sourceTable = ConfigurationManager.AppSettings["SourceTable"];
            //if (!string.IsNullOrEmpty(sourceTable)) connection.SourceTable = sourceTable;
            string traceWrapperType = ConfigurationManager.AppSettings["TraceWrapperBase"];
            if (!string.IsNullOrEmpty(traceWrapperType))
            {
                Type type = ReflectionUtil.CreateType(traceWrapperType);
                return (TraceWrapperBase)Activator.CreateInstance(type, new object[] { connection });
            }
            else
            {
                return new TraceWrapperTraceServer(connection);
            }
 
        }
    }
}
