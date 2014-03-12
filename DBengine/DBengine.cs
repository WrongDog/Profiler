using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Data.OleDb;

namespace DBengine
{
    public enum DataBaseType
    {
        sqlserver = 0,
        oledb = 1,
        oracle = 2,
        unknown = 3
    }
    public class DBengine
    {

        public DBengine()
        {
        }
        #region ENGstructure
        const string SqlNull = "Null";
        //private bool SQLnotOLE=true;

        public DataBaseType DBtype = DataBaseType.unknown;

        private DbConnection Conn;
        public string ConnectionString;
        static DBengine UDBengine = new DBengine();
        static public DBengine instance()
        {
            return UDBengine;
        }
        public void DBConnectFromFile(string cfg)
        {
            string connstr = "";


            try
            {
                XmlDocument configfile = new XmlDocument();
                configfile.Load(cfg);
                XmlNodeList configlist = configfile.GetElementsByTagName("configuration");
                foreach (XmlNode onode in configlist)
                {
                    foreach (XmlNode ochild in onode.ChildNodes)
                    {

                        if (ochild.Name == "connectionstring")
                        {
                            connstr = ochild.InnerXml;
                        }
                        else if (ochild.Name == "dbtype")
                        {//db type
                            switch (ochild.InnerXml.ToLower())
                            {
                                case "sqlserver":
                                    DBtype = DataBaseType.sqlserver;
                                    break;
                                case "oledb":
                                    DBtype = DataBaseType.oledb;
                                    break;
                                case "oracle":
                                    DBtype = DataBaseType.oracle;
                                    break;
                                default:
                                    DBtype = DataBaseType.unknown;
                                    break;


                            }

                        }
                    }
                }
                if (!DBConnect(connstr, DBtype))
                {
                    throw (new Exception("Á¬½ÓÊý¾Ý¿âÊ§°Ü"));
                }
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }
        }
        /// <summary>
        /// Á¬½ÓÊý¾Ý¿â
        /// </summary>
        /// <param name="connString">Á¬½Ó×Ö·û´®</param>
        /// <param name="SqlServer">Ê¹ÓÃSQLConn»¹ÊÇOLEDB</param>
        /// <returns></returns>
        public bool DBConnect(string connString, DataBaseType databasetype)
        {
            DBtype = databasetype;
            if (connString == "")
            {
                return false;
            }
            else
            {


                if (DBtype == DataBaseType.sqlserver)
                {
                    try
                    {
                        Conn = new SqlConnection(connString);
                        Conn.Open();
                        ConnectionString = connString;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                        return false;
                    }
                }
                else if (DBtype == DataBaseType.oledb)
                {
                    try
                    {
                        Conn = new OleDbConnection(connString);
                        Conn.Open();
                        ConnectionString = connString;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                        return false;
                    }

                }
                else if (DBtype == DataBaseType.oracle)
                {
                    try
                    {
                        Conn = new OracleConnection(connString);
                        Conn.Open();
                        ConnectionString = connString;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                        return false;
                    }

                }
                else
                {//unknown
                    try
                    {
                        Conn = new SqlConnection(connString);
                        Conn.Open();
                        ConnectionString = connString;
                        DBtype = DataBaseType.sqlserver;
                        return true;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            Conn = new OleDbConnection(connString);
                            Conn.Open();
                            ConnectionString = connString;
                            DBtype = DataBaseType.oledb;
                            return true;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                Conn = new OracleConnection(connString);
                                Conn.Open();
                                ConnectionString = connString;
                                DBtype = DataBaseType.oracle;
                                return true;
                            }
                            catch (Exception)
                            {

                                return false;
                            }
                        }
                    }
                }
            }

        }
        public void Close()
        {

            if (Conn != null) Conn.Close();

        }
        /// <summary>
        /// ½«²éÑ¯×÷ÎªÊÂÎñÖ´ÐÐ
        /// </summary>
        /// <param name="Sqlstr">²éÑ¯</param>
        /// <returns>·µ»ØÓ°ÏìµÄÊý¾Ý¼ÇÂ¼¸öÊý</returns>
        public int ExecuteNonQuery(string Sqlstr)
        {
            int rev;

            DbTransaction sqt = Conn.BeginTransaction();
            try
            {
                DbCommand cmd;
                if (DBtype == DataBaseType.sqlserver)
                {
                    cmd = new SqlCommand(Sqlstr, (SqlConnection)Conn, (SqlTransaction)sqt);
                }
                else if (DBtype == DataBaseType.oracle)
                {
                    cmd = new OracleCommand(Sqlstr, (OracleConnection)Conn, (OracleTransaction)sqt);
                }
                else
                {
                    cmd = new OleDbCommand(Sqlstr, (OleDbConnection)Conn, (OleDbTransaction)sqt);
                }

                rev = cmd.ExecuteNonQuery();
                sqt.Commit();
                return rev;
            }
            catch (Exception ex)
            {
                sqt.Rollback();
                throw (new Exception(ex.Message));
            }

        }

        public void ExecuteTransaction(string[] Sqlstr)
        {
            Queue<string> sqlq = new Queue<string>();
            for (int p = 0; p <= Sqlstr.GetUpperBound(0); p++)
            {
                sqlq.Enqueue(Sqlstr[p]);
            }
            ExecuteTransaction(sqlq);

        }

        public void ExecuteTransaction(Queue<string> Sqlstr)
        {


            DbTransaction sqt = Conn.BeginTransaction();
            DbCommand cmd;
            if (DBtype == DataBaseType.sqlserver)
            {
                cmd = new SqlCommand("", (SqlConnection)Conn, (SqlTransaction)sqt);
            }
            else if (DBtype == DataBaseType.oracle)
            {
                cmd = new OracleCommand("", (OracleConnection)Conn, (OracleTransaction)sqt);
            }
            else
            {
                cmd = new OleDbCommand("", (OleDbConnection)Conn, (OleDbTransaction)sqt);
            }
            cmd.Transaction = sqt;
            try
            {
                while (Sqlstr.Count > 0)
                {
                    cmd.CommandText = Convert.ToString(Sqlstr.Dequeue());
                    cmd.ExecuteNonQuery();
                }
                sqt.Commit();
            }
            catch (Exception ex)
            {
                sqt.Rollback();
                throw (new Exception(cmd.CommandText + " " + ex.Message));

            }

        }

        public object ExecuteScalar(string Sqlstr)
        {
            object rev;

            DbTransaction sqt = Conn.BeginTransaction();
            try
            {
                DbCommand cmd;
                if (DBtype == DataBaseType.sqlserver)
                {
                    cmd = new SqlCommand(Sqlstr, (SqlConnection)Conn, (SqlTransaction)sqt);
                }
                else if (DBtype == DataBaseType.oracle)
                {
                    cmd = new OracleCommand(Sqlstr, (OracleConnection)Conn, (OracleTransaction)sqt);
                }
                else
                {
                    cmd = new OleDbCommand(Sqlstr, (OleDbConnection)Conn, (OleDbTransaction)sqt);
                }
                rev = cmd.ExecuteScalar();
                sqt.Commit();
                return rev;
            }
            catch (Exception ex)
            {
                sqt.Rollback();
                throw (new Exception(ex.Message));
            }

        }
        public DataTable QuerySQL(string Sqlstr)
        {
            return QuerySQL(Sqlstr, "QueryResult");
        }
        public DataTable QuerySQL_MAX(string Sqlstr, int start, int max)
        {
            DataTable Tempdt = new DataTable("QueryResult");

            try
            {
                DbDataAdapter Da;
                if (DBtype == DataBaseType.sqlserver)
                {
                    Da = new SqlDataAdapter(Sqlstr, (SqlConnection)Conn);
                }
                else if (DBtype == DataBaseType.oracle)
                {
                    Da = new OracleDataAdapter(Sqlstr, (OracleConnection)Conn);
                }
                else
                {
                    Da = new OleDbDataAdapter(Sqlstr, (OleDbConnection)Conn);
                }
                Da.Fill(start, max, Tempdt);
                return Tempdt;
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }

        }

        public DataTable QuerySQL(string Sqlstr, string resultname)
        {
            DataTable Tempdt = new DataTable(resultname);

            try
            {
                DbDataAdapter Da;
                if (DBtype == DataBaseType.sqlserver)
                {
                    Da = new SqlDataAdapter(Sqlstr, (SqlConnection)Conn);
                }
                else if (DBtype == DataBaseType.oracle)
                {
                    Da = new OracleDataAdapter(Sqlstr, (OracleConnection)Conn);
                }
                else
                {
                    Da = new OleDbDataAdapter(Sqlstr, (OleDbConnection)Conn);
                }
                Da.Fill(Tempdt);
                Da.FillSchema(Tempdt, SchemaType.Mapped);
                return Tempdt;
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }

        }

        #endregion


        #region Gfunction
        public List<string> GetTableName()
        {
            List<string> rev = new List<string>();


            DataTable schemaTable;
            string Sqlstr;
            //Oracle SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME
            //MySQL SHOW TABLES
            //MS SQL Server select name from sysobjects where type = N¡¯U¡¯ order by name 
            //DB2 UDB SELECT NAME FROM SYSIBM.SYSTABLES WHERE TYPE = ¡®T¡¯ AND CREATOR != ¡®SYSIBM¡¯ ORDER BY NAME 

            if (DBtype == DataBaseType.sqlserver)
            {
                Sqlstr = "select Name as TABLE_NAME from dbo.sysobjects where  OBJECTPROPERTY(id,N'IsUserTable')=1 order by Name";
                schemaTable = this.QuerySQL(Sqlstr);

            }
            else if (DBtype == DataBaseType.oracle)
            {
                Sqlstr = "SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME  ";
                schemaTable = this.QuerySQL(Sqlstr);
            }
            else
            {
                schemaTable = ((OleDbConnection)Conn).GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                   new object[] { null, null, null, "TABLE" });
            }

            foreach (DataRow row in schemaTable.Rows)
            {
                rev.Add(row["TABLE_NAME"].ToString());
            }

            return rev;

        }
        public List<string> GetTableKey(string Tablename)
        {
            List<string> rev = new List<string>();

            if (DBtype == DataBaseType.sqlserver)
            {
                string Sqlstr = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME='" + Tablename + "'";
                DataTable dt = this.QuerySQL(Sqlstr);
                if (dt.Rows.Count == 0) return null;

                foreach (DataRow row in dt.Rows)
                {
                    rev.Add(row["COLUMN_NAME"].ToString());

                }
            }
            else if (DBtype == DataBaseType.oracle)
            {
                string Sqlstr = "select   *   from    user_cons_columns    where    constraint_name   =    (select    constraint_name   from    user_constraints      where    table_name   =    upper('" + Tablename + "')  and    constraint_type   ='P') ";
                DataTable dt = this.QuerySQL(Sqlstr);
                if (dt.Rows.Count == 0) return null;
                foreach (DataRow row in dt.Rows)
                {
                    rev.Add(row["COLUMN_NAME"].ToString());
                }
            }
            else
            {
                DataTable OLEDBschemaTable = ((OleDbConnection)Conn).GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new Object[] { null, null, Tablename });
                if (OLEDBschemaTable.Rows.Count == 0) return null;
                for (int i = 0; i < OLEDBschemaTable.Rows.Count; i++)
                {
                    rev.Add(OLEDBschemaTable.Rows[i].ItemArray[3].ToString());

                }

            }


            return rev;

        }
        public string[,] GetColType(string Tablename)
        {
            DataTable dt = this.QuerySQL("select * from " + Tablename + " where 1=2");
            string[,] rev = new string[dt.Columns.Count, 2];
            int p = 0;
            foreach (DataColumn mycol in dt.Columns)
            {
                rev[p, 0] = mycol.ColumnName.ToString();
                rev[p, 1] = mycol.DataType.ToString();
                p++;
            }
            return rev;
        }
        private bool IsRowInTable(DataTable dt, DataRow dr)
        {
            object[] ob = dr.ItemArray;
            object[] oldob;
            bool same = true;
            foreach (DataRow odr in dt.Rows)
            {
                oldob = odr.ItemArray;
                if (oldob.GetUpperBound(0) == ob.GetUpperBound(0))
                {
                    same = true;
                    for (int p = 0; p <= ob.GetUpperBound(0); p++)
                    {
                        if (Convert.ToString(oldob[p]) != Convert.ToString(ob[p]))
                        {
                            same = false;
                            break;
                        }
                    }
                    if (same) return true;
                }
            }
            return false;
        }

        public void UpdateTable(DataTable dt)
        {
            Queue<string> sqlq = new Queue<string>();
            string sql;
            foreach (DataRow dr in dt.Rows)
            {
                if (DBtype == DataBaseType.oracle)
                {
                    sql = UpdateRowsora(dr, dt.TableName);
                }
                else
                {
                    sql = UpdateRowssql(dr, dt.TableName);
                }
                if (sql != null)
                {
                    sqlq.Enqueue(sql);
                }

            }
            if (sqlq.Count > 0) this.ExecuteTransaction(sqlq);

        }
        public static string UpdateRowssql(DataRow dr, string tablename)
        {
            string operation = "";
            switch (dr.RowState)
            {
                case DataRowState.Modified:
                    operation = "update";
                    break;
                case DataRowState.Added:
                    operation = "insert";
                    break;
                case DataRowState.Deleted:
                    operation = "delete";
                    break;
            }
            return UpdateRows(dr, DataBaseType.sqlserver, tablename, operation);
        }
        public static string UpdateRowsora(DataRow dr, string tablename)
        {
            string operation = "";
            switch (dr.RowState)
            {
                case DataRowState.Modified:
                    operation = "update";
                    break;
                case DataRowState.Added:
                    operation = "insert";
                    break;
                case DataRowState.Deleted:
                    operation = "delete";
                    break;
            }
            return UpdateRows(dr, DataBaseType.oracle, tablename, operation);
        }
        protected static string FormatSQLValue(object value)
        {
            string valuestring = value.ToString();
            if (!valuestring.Contains("'")) return valuestring;
            Regex reg = new Regex("[']{1}");
            return reg.Replace(valuestring, "''");
        }
        protected static string FormatTableColumnName(string name)
        {
            return "[" + name + "]";
        }
        protected static object GetOriginalContent(DataRow dr, string columnName)
        {
            object data = dr[columnName];
            try
            {
                data = dr[columnName, DataRowVersion.Original];
            }
            catch (System.Data.VersionNotFoundException)
            {
            }
            return data;
        }
        public static string UpdateRows(DataRow dr, DataBaseType dbtype, string tablename, string operation)
        {
            string sqlhead = "";
            string sqlcdt = "";
            string sqlcdt2 = "";
            object data;
            string rev = "";
            switch (operation.ToLower())
            {
                case "select":
                    sqlhead = "select ";
                    sqlcdt = "";
                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        data = GetOriginalContent(dr,dc.ColumnName);
                       


                        sqlcdt += "," + dc.ColumnName;

                        if (data == DBNull.Value)
                        {
                            sqlcdt2 += " and " + FormatTableColumnName(dc.ColumnName) + " is null ";
                        }
                        else if (dc.DataType.ToString().IndexOf("String") > 0 ||
                          dc.DataType.ToString().IndexOf("Char") > 0)
                        {

                            sqlcdt2 += " and " + FormatTableColumnName(dc.ColumnName) + " = '" + FormatSQLValue(data) + "'";
                        }
                        else if (dc.DataType.ToString().IndexOf("DateTime") > 0)//"yyyy/MM/dd HH:mm:ss.fffffffK"
                        {

                            sqlcdt2 += " and " + FormatTableColumnName(dc.ColumnName) + " = ";
                            if (dbtype == DataBaseType.oracle) sqlcdt2 += "to_date(";
                            sqlcdt2 += "'" + Convert.ToDateTime(data).ToString("yyyy/MM/dd HH:mm:ss") + "'";
                            if (dbtype == DataBaseType.oracle) sqlcdt2 += ",'yyyy/mm/dd hh24:mi:ss')";



                        }
                        else
                        {

                            sqlcdt2 += " and " + FormatTableColumnName(dc.ColumnName) + " = '" + data + "'";

                        }
                    }
                    sqlcdt = sqlcdt.Substring(1, sqlcdt.Length - 1);
                    sqlcdt2 = sqlcdt2.Substring(5, sqlcdt2.Length - 5);
                    rev = sqlhead + sqlcdt + " from " + tablename + " where " + sqlcdt2;
                    break;
                case "update":
                    sqlhead = "update " + tablename + " set ";
                    sqlcdt = "";
                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        data = GetOriginalContent(dr, dc.ColumnName);

                        if (data == DBNull.Value)
                        {
                            sqlcdt2 += " and " + FormatTableColumnName(dc.ColumnName) + " is null ";
                        }
                        else if (dc.DataType.ToString().IndexOf("String") > 0 ||
                          dc.DataType.ToString().IndexOf("Char") > 0)
                        {
                            //sqlcdt += "," + dc.ColumnName + " = '" + dr[dc.ColumnName] + "'";
                            sqlcdt2 += " and " + FormatTableColumnName(dc.ColumnName) + " = '" + FormatSQLValue(data) + "'";
                        }
                        else if (dc.DataType.ToString().IndexOf("DateTime") > 0)//"yyyy/MM/dd HH:mm:ss.fffffffK"
                        {
                            //sqlcdt += "," + dc.ColumnName + " = '" + Convert.ToDateTime(dr[dc.ColumnName]).ToString("yyyy/MM/dd HH:mm:ss") + "'";
                            sqlcdt2 += " and " + FormatTableColumnName(dc.ColumnName) + " = ";
                            if (dbtype == DataBaseType.oracle) sqlcdt2 += "to_date(";
                            sqlcdt2 += "'" + Convert.ToDateTime(data).ToString("yyyy/MM/dd HH:mm:ss") + "'";
                            if (dbtype == DataBaseType.oracle) sqlcdt2 += ",'yyyy/mm/dd hh24:mi:ss')";

                        }
                        else
                        {
                            //sqlcdt += "," + dc.ColumnName + " = " + dr[dc.ColumnName] + "";
                            sqlcdt2 += " and " + FormatTableColumnName(dc.ColumnName) + " = '" + data + "'";

                        }

                        if (dr[dc.ColumnName] == DBNull.Value)
                        {
                            sqlcdt += "," + FormatTableColumnName(dc.ColumnName) + " = null ";
                        }
                        else if (dc.DataType.ToString().IndexOf("String") > 0 ||
                          dc.DataType.ToString().IndexOf("Char") > 0)
                        {
                            sqlcdt += "," + FormatTableColumnName(dc.ColumnName) + " = '" + FormatSQLValue(dr[dc.ColumnName]) + "'";

                        }
                        else if (dc.DataType.ToString().IndexOf("DateTime") > 0)//"yyyy/MM/dd HH:mm:ss.fffffffK"
                        {
                            sqlcdt += "," + FormatTableColumnName(dc.ColumnName) + " = ";
                            if (dbtype == DataBaseType.oracle) sqlcdt += "to_date(";
                            sqlcdt += "'" + Convert.ToDateTime(dr[dc.ColumnName]).ToString("yyyy/MM/dd HH:mm:ss") + "'";
                            if (dbtype == DataBaseType.oracle) sqlcdt += ",'yyyy/mm/dd hh24:mi:ss')";


                        }
                        else
                        {
                            sqlcdt += "," + FormatTableColumnName(dc.ColumnName) + " = " + dr[dc.ColumnName] + "";

                        }
                    }
                    sqlcdt = sqlcdt.Substring(1, sqlcdt.Length - 1);
                    sqlcdt2 = sqlcdt2.Substring(5, sqlcdt2.Length - 5);
                    rev = sqlhead + sqlcdt + " where " + sqlcdt2;
                    break;
                case "insert":
                    sqlhead = "insert into " + tablename;
                    sqlcdt = "";
                    sqlcdt2 = "";
                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        data = GetOriginalContent(dr, dc.ColumnName);

                        sqlcdt += "," + FormatTableColumnName(dc.ColumnName);

                        if (data == DBNull.Value)
                        {
                            sqlcdt2 += ", null ";
                        }
                        else if (dc.DataType.ToString().IndexOf("String") > 0 ||
                           dc.DataType.ToString().IndexOf("Char") > 0)
                        {
                            sqlcdt2 += ",'" + FormatSQLValue(dr[dc.ColumnName]) + "'";
                        }
                        else if (dc.DataType.ToString().IndexOf("DateTime") > 0)
                        {
                            sqlcdt2 += ",";
                            if (dbtype == DataBaseType.oracle) sqlcdt2 += "to_date(";
                            sqlcdt2 += "'" + Convert.ToDateTime(dr[dc.ColumnName]).ToString("yyyy/MM/dd HH:mm:ss") + "'";
                            if (dbtype == DataBaseType.oracle) sqlcdt2 += ",'yyyy/mm/dd hh24:mi:ss')";
                        }
                        else
                        {
                            sqlcdt2 += "," + dr[dc.ColumnName] + "";

                        }
                    }
                    sqlcdt = sqlcdt.Substring(1, sqlcdt.Length - 1);
                    sqlcdt = "(" + sqlcdt + ") values ";
                    sqlcdt2 = sqlcdt2.Substring(1, sqlcdt2.Length - 1);
                    sqlcdt2 = "(" + sqlcdt2 + ")";

                    rev = sqlhead + sqlcdt + sqlcdt2;
                    break;
                case "delete":
                    sqlhead = "delete " + tablename + " where ";
                    sqlcdt = "";

                    DataColumn[] cols;
                    if (dr.Table.PrimaryKey.Length == 0)
                    {
                        cols = new DataColumn[dr.Table.Columns.Count];
                        for (int idx = 0; idx < dr.Table.Columns.Count; idx++)
                        {
                            cols[idx] = dr.Table.Columns[idx];
                        }
                    }
                    else
                    {
                        cols = dr.Table.PrimaryKey;
                    }

                    foreach (DataColumn dc in cols)
                    {

                        data = GetOriginalContent(dr, dc.ColumnName);

                        if (data == DBNull.Value)
                        {
                            sqlcdt += " and " + FormatTableColumnName(dc.ColumnName) + " is null ";
                        }
                        else if (dc.DataType.ToString().IndexOf("String") > 0 ||
                           dc.DataType.ToString().IndexOf("Char") > 0)
                        {
                            sqlcdt += " and " + FormatTableColumnName(dc.ColumnName) + " = '" + FormatSQLValue(data) + "'";
                        }
                        else if (dc.DataType.ToString().IndexOf("DateTime") > 0)
                        {
                            sqlcdt += " and " + FormatTableColumnName(dc.ColumnName) + " = ";
                            if (dbtype == DataBaseType.oracle) sqlcdt += "to_date(";
                            sqlcdt += "'" + Convert.ToDateTime(data).ToString("yyyy/MM/dd HH:mm:ss") + "'";
                            if (dbtype == DataBaseType.oracle) sqlcdt += ",'yyyy/mm/dd hh24:mi:ss')";
                        }
                        else
                        {
                            sqlcdt += " and " + FormatTableColumnName(dc.ColumnName) + " = " + data + "";

                        }
                    }
                    sqlcdt = sqlcdt.Substring(5, sqlcdt.Length - 5);
                    rev = sqlhead + sqlcdt;
                    break;
            }
            if (dbtype == DataBaseType.oracle) rev = rev.Replace(System.Environment.NewLine, "'||chr(10)||chr(13)||'");
            return rev;
        }
        public static string InsertSQL(DataSet ds, DataBaseType dbtype, bool deletefirst)
        {
            string rev = "";
            if (deletefirst)
            {
                foreach (DataTable dt in ds.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dbtype == DataBaseType.oracle)
                        {
                            rev += UpdateRows(dr, dbtype, dt.TableName, "delete") + System.Environment.NewLine + "/" + System.Environment.NewLine;
                        }
                        else
                        {
                            rev += UpdateRows(dr, dbtype, dt.TableName, "delete") + System.Environment.NewLine;
                        }
                    }
                }

            }

            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dbtype == DataBaseType.oracle)
                    {
                        rev += UpdateRows(dr, dbtype, dt.TableName, "insert") + System.Environment.NewLine + "/" + System.Environment.NewLine;
                    }
                    else
                    {
                        rev += UpdateRows(dr, dbtype, dt.TableName, "insert") + System.Environment.NewLine;
                    }
                }
            }
            return rev;
        }
        public DataTable Getschema(string tablename)
        {

            string Sqlstr = "select * from " + tablename + " where 1=2";
            DataTable Tempdt = new DataTable(tablename);


            try
            {
                DbDataAdapter Da;
                if (DBtype == DataBaseType.sqlserver)
                {
                    Da = new SqlDataAdapter(Sqlstr, (SqlConnection)Conn);
                }
                else if (DBtype == DataBaseType.oracle)
                {
                    Da = new OracleDataAdapter(Sqlstr, (OracleConnection)Conn);
                }
                else
                {
                    Da = new OleDbDataAdapter(Sqlstr, (OleDbConnection)Conn);
                }


                //Da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                //Da.Fill(Tempdt);
                Da.FillSchema(Tempdt, SchemaType.Mapped);
                return Tempdt;
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }


        }
        #endregion






    }
}
