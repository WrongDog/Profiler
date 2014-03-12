using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using TraceWrapper;
using System.Data.SqlClient;
namespace BenchmarkResult
{
    public class BenchmarkResult
    {
        public delegate void progressdele(int current, int max);
        public delegate void resultdele(Dictionary<string, decimal> result);
        public event progressdele ProgressUpdated;
        public event resultdele ResultUpdated;
        public static DataTable CreateTableWeight(ConnectionInfo connectioninfo)
        {
            //DataSet ds = new DataSet("TableWeight");
            DataTable casinolink = new DataTable("TableWeight");
            casinolink.Columns.Add("Table");
            casinolink.Columns.Add("Weight",typeof(decimal));
            SqlConnection sqlconnection = new SqlConnection();
            try
            {
                sqlconnection.ConnectionString = connectioninfo.AsConnectionString;
                sqlconnection.Open();
                string sqlstr = "select Name from dbo.sysobjects where  OBJECTPROPERTY(id,N'IsUserTable')=1";
                SqlCommand command = sqlconnection.CreateCommand();
                command.CommandText = sqlstr;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow tabledr in dt.Rows)
                {
                    string tablename = tabledr[0].ToString();
                    SqlCommand commands = sqlconnection.CreateCommand();
                    commands.CommandText = "select count(*) from "+tablename;
                    int count= (int)commands.ExecuteScalar();
                    casinolink.Rows.Add(new object[] {tablename,count });
                }


                return casinolink;

            }
            catch (Exception)
            {
 
            }
            finally
            {
                sqlconnection.Close();
            }
            //casinolink.Rows.Add(new object[] { "PatronPoint",1});
            //casinolink.Rows.Add(new object[] { "PatronPointDetail", 1.5 });
            //casinolink.Rows.Add(new object[] { "PatronPointPending", 0.01 });
            //casinolink.Rows.Add(new object[] { "PatronPointPlayDailyUsed", 0.1 });

            return casinolink;
        }
        public static DataTable CreateOperationWeight()
        {
            DataTable operationWeight = new DataTable("OperationWeight");
            operationWeight.Columns.Add("Operation");
            operationWeight.Columns.Add("Weight", typeof(decimal));
            operationWeight.Rows.Add(new object[] { "TableScan", 1 });
            operationWeight.Rows.Add(new object[] { "ClusteredIndexScan", 1 });
            operationWeight.Rows.Add(new object[] { "RIDLookup", 1 });
            operationWeight.Rows.Add(new object[] { "HashJoin", 1 });
            operationWeight.Rows.Add(new object[] { "Sort", 1 });
            operationWeight.Rows.Add(new object[] { "KeyLookup", 1 });
            operationWeight.Rows.Add(new object[] { "NestedLoops", 1 });
            return operationWeight;
        }

        protected Dictionary<string,decimal> tableWeight;
        protected Dictionary<string, decimal> operationWeight;
        protected ConnectionInfo connectionInfo;
        protected BenchmarkHandler.BenchmarkHandler handler = new BenchmarkHandler.BenchmarkHandler();
        TraceWrapperSQLDB traceDB;
        public BenchmarkResult(ConnectionInfo connection, DataTable tableWeight, DataTable operationWeight)
        {
            this.connectionInfo = connection;
            this.tableWeight = GetDictionaryFromTable(tableWeight);
            this.operationWeight = GetDictionaryFromTable(operationWeight);

            List<IResultHandler> handlers = new List<IResultHandler>();
            handlers.Add(handler);
            traceDB = new TraceWrapperSQLDB(connectionInfo, handlers);
            traceDB.ProgressUpdated +=new TraceWrapperProgressBase.ProgressUpdateDele(traceDB_ProgressUpdated); 
            traceDB.OnStateChanged += new StateChangeNotifierDele(traceDB_OnStateChanged);
        }
        protected Dictionary<string, decimal> GetDictionaryFromTable(DataTable table)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            foreach (DataRow dr in table.Rows)
            {
                result.Add(dr[0].ToString(), Convert.ToDecimal(dr[1]));
            }
            return result;
        }

        public void Start()
        {
              
            traceDB.Start();
        }

        void traceDB_OnStateChanged(TraceState newstate)
        {
            if (newstate == TraceState.Stopped)
            {
                DataSet handlerresult = handler.Result;
                handlerresult.WriteXml("BenchmarkHandler" + DateTime.UtcNow.ToString().Replace(":", "_").Replace(@"/", "_") + ".xml", XmlWriteMode.WriteSchema);

                Dictionary<string, decimal> result = new Dictionary<string, decimal>();

                foreach (DataRow spdr in handlerresult.Tables[0].Rows)
                {
                    decimal weight = 0;
                    foreach (DataRow opdr in handlerresult.Tables[1].Select(" Object ='" + spdr["Object"].ToString() + "'"))
                    {
                        decimal tableweight = 0;
                        decimal operationweight = 0;
                        if (tableWeight.ContainsKey(opdr["Table"].ToString())) tableWeight.TryGetValue(opdr["Table"].ToString(), out tableweight);
                        if (operationWeight.ContainsKey(opdr["Operation"].ToString())) operationWeight.TryGetValue(opdr["Operation"].ToString(), out operationweight);
                        weight += tableweight * operationweight * Convert.ToInt32(opdr["Count"].ToString());
                    }
                    weight = weight / Convert.ToInt32(spdr["Count"].ToString());
                    result.Add(spdr["Object"].ToString(), weight);
                    //result.Rows.Add(new object[] { spdr["Object"].ToString(), weight });
                }




                if (ResultUpdated != null) ResultUpdated(result);
        
            }
        }
      

        void traceDB_ProgressUpdated(int current, int max)
        {
            //Console.WriteLine(string.Format("{0}/{1}",current,max));
            //throw new NotImplementedException();
            if (this.ProgressUpdated != null) ProgressUpdated(current, max);
        }
    }
}
