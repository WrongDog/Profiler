using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
namespace TraceWrapper
{
    public enum TraceEventEnum
    {
        RPCCompleted = 10,
        RPCStarting = 11,
        SQLBatchCompleted = 12,
        SQLBatchStarting = 13,
        AuditLogin = 14,
        AuditLogout = 15,
        Attention = 16,
        ExistingConnection = 17,
        AuditServerStartsandStops = 18,
        DTCTransaction = 19,
        AuditLoginFailed = 20,
        EventLog = 21,
        ErrorLog = 22,
        LockReleased = 23,
        LockAcquired = 24,
        LockDeadlock = 25,
        LockCancel = 26,
        LockTimeout = 27,
        DegreeofParallelismEvent = 28,
        Exception = 33,
        SPCacheMiss = 34,
        SPCacheInsert = 35,
        SPCacheRemove = 36,
        SPRecompile = 37,
        SPCacheHit = 38,
        Deprecated = 39,
        SQLStmtStarting = 40,
        SQLStmtCompleted = 41,
        SPStarting = 42,
        SPCompleted = 43,
        SPStmtStarting = 44,
        SPStmtCompleted = 45,
        ObjectCreated = 46,
        ObjectDeleted = 47,
        SQLTransaction = 50,
        ScanStarted = 51,
        ScanStopped = 52,
        CursorOpen = 53,
        TransactionLog = 54,
        HashWarning = 55,
        AutoStats = 58,
        LockDeadlockChain = 59,
        LockEscalation = 60,
        OLEDBErrors = 61,
        ExecutionWarnings = 67,
        ShowplanTextUnencoded = 68,
        SortWarnings = 69,
        CursorPrepare = 70,
        PrepareSQL = 71,
        ExecPreparedSQL = 72,
        UnprepareSQL = 73,
        CursorExecute = 74,
        CursorRecompile = 75,
        CursorImplicitConversion = 76,
        CursorUnprepare = 77,
        CursorClose = 78,
        MissingColumnStatistics = 79,
        MissingJoinPredicate = 80,
        ServerMemoryChange = 81,
        DataFileAutoGrow = 92,
        LogFileAutoGrow = 93,
        DataFileAutoShrink = 94,
        LogFileAutoShrink = 95,
        ShowplanText = 96,
        ShowplanAll = 97,
        ShowplanStatisticsProfile = 98,
        RPCOutputParameter = 100,
        AuditStatementGDREvent = 102,
        AuditObjectGDREvent = 103,
        AuditAddLoginEvent = 104,
        AuditLoginGDREvent = 105,
        AuditLoginChangePropertyEvent = 106,
        AuditLoginChangePasswordEvent = 107,
        AuditAddLogintoServerRoleEvent = 108,
        AuditAddDBUserEvent = 109,
        AuditAddMembertoDBRoleEvent = 110,
        AuditAddRoleEvent = 111,
        AuditAppRoleChangePasswordEvent = 112,
        AuditStatementPermissionEvent = 113,
        AuditSchemaObjectAccessEvent = 114,
        AuditBackupRestoreEvent = 115,
        AuditDBCCEvent = 116,
        AuditChangeAuditEvent = 117,
        AuditObjectDerivedPermissionEvent = 118,
        OLEDBCallEvent = 119,
        OLEDBQueryInterfaceEvent = 120,
        OLEDBDataReadEvent = 121,
        ShowplanXML = 122,
        SQLFullTextQuery = 123,
        BrokerConversation = 124,
        DeprecationAnnouncement = 125,
        DeprecationFinalSupport = 126,
        ExchangeSpillEvent = 127,
        AuditDatabaseManagementEvent = 128,
        AuditDatabaseObjectManagementEvent = 129,
        AuditDatabasePrincipalManagementEvent = 130,
        AuditSchemaObjectManagementEvent = 131,
        AuditServerPrincipalImpersonationEvent = 132,
        AuditDatabasePrincipalImpersonationEvent = 133,
        AuditServerObjectTakeOwnershipEvent = 134,
        AuditDatabaseObjectTakeOwnershipEvent = 135,
        BrokerConversationGroup = 136,
        BlockedProcessReport = 137,
        BrokerConnection = 138,
        BrokerForwardedMessageSent = 139,
        BrokerForwardedMessageDropped = 140,
        BrokerMessageClassify = 141,
        BrokerTransmission = 142,
        BrokerQueueDisabled = 143,
        ShowplanXMLStatisticsProfile = 146,
        DeadlockGraph = 148,
        BrokerRemoteMessageAcknowledgement = 149,
        TraceFileClose = 150,
        AuditChangeDatabaseOwner = 152,
        AuditSchemaObjectTakeOwnershipEvent = 153,
        FTCrawlStarted = 155,
        FTCrawlStopped = 156,
        FTCrawlAborted = 157,
        AuditBrokerConversation = 158,
        AuditBrokerLogin = 159,
        BrokerMessageUndeliverable = 160,
        BrokerCorruptedMessage = 161,
        UserErrorMessage = 162,
        BrokerActivation = 163,
        ObjectAltered = 164,
        Performancestatistics = 165,
        SQLStmtRecompile = 166,
        DatabaseMirroringStateChange = 167,
        ShowplanXMLForQueryCompile = 168,
        ShowplanAllForQueryCompile = 169,
        AuditServerScopeGDREvent = 170,
        AuditServerObjectGDREvent = 171,
        AuditDatabaseObjectGDREvent = 172,
        AuditServerOperationEvent = 173,
        AuditServerAlterTraceEvent = 175,
        AuditServerObjectManagementEvent = 176,
        AuditServerPrincipalManagementEvent = 177,
        AuditDatabaseOperationEvent = 178,
        AuditDatabaseObjectAccessEvent = 180,
        TMBeginTranstarting = 181,
        TMBeginTrancompleted = 182,
        TMPromoteTranstarting = 183,
        TMPromoteTrancompleted = 184,
        TMCommitTranstarting = 185,
        TMCommitTrancompleted = 186,
        TMRollbackTranstarting = 187,
        TMRollbackTrancompleted = 188,
        LockTimeoutHasValue = 189,
        ProgressReportOnlineIndexOperation = 190,
        TMSaveTranstarting = 191,
        TMSaveTrancompleted = 192,
        BackgroundJobError = 193,
        OLEDBProviderInformation = 194,
        MountTape = 195,
        AssemblyLoad = 196,
        XQueryStaticType = 198,
        QNsubscription = 199,
        QNparametertable = 200,
        QNtemplate = 201,
        QNdynamics = 202

    }
    public enum TraceColumn
    {
        TextData =1,
        BinaryData=2,
        DatabaseID=3,
        TransactionID=4,
        LineNumber=5,
        NTUserName=6,
        NTDomainName=7,
        HostName=8,
        ClientProcessID=9,
        ApplicationName=10,
        LoginName=11,
        SPID=12,
        Duration=13,
        StartTime=14,
        EndTime=15,
        Reads=16,
        Writes=17,
        CPU=18,
        Permissions=19,
        Severity=20,
        EventSubClass=21,
        ObjectID=22,
        Success=23,
        IndexID=24,
        IntegerData=25,
        ServerName=26,
        EventClass=27,
        ObjectType=28,
        NestLevel=29,
        State=30,
        Error=31,
        Mode=32,
        Handle=33,
        ObjectName=34,
        DatabaseName=35,
        FileName=36,
        OwnerName=37,
        RoleName=38,
        TargetUserName=39,
        DBUserName=40,
        LoginSid=41,
        TargetLoginName=42,
        TargetLoginSid=43,
        ColumnPermissions=44,
        LinkedServerName=45,
        ProviderName=46,
        MethodName=47,
        RowCounts=48,
        RequestID=49,
        XactSequence=50,
        EventSequence=51,
        BigintData1=52,
        BigintData2=53,
        GUID=54,
        IntegerData2=55,
        ObjectID2=56,
        Type=57,
        OwnerID=58,
        ParentName=59,
        IsSystem=60,
        Offset=61,
        SourceDatabaseID=62,
        SqlHandle=63,
        SessionLoginName=64
    }
    public class TraceDefinitionFile : Dictionary<TraceEventEnum, List<TraceColumn>>
    {
        public class ColumnFilter
        {
            public ColumnFilter(TraceColumn Column, bool ExcludeRowThatDoNotContainValues = false)
            {
                this.Column = Column;
                this.ExcludeRowThatDoNotContainValues = ExcludeRowThatDoNotContainValues;
                this.Operations = new List<Operation>();
            }
            public enum OperatorEnum
            {
                Equals=0,
                NotEqualTo=1,
                GreaterOrEqual=4,
                LessOrEqual = 5,
                Like=6,
                NotLike=7
            }
            public class Operation
            {
                public Operation(OperatorEnum Operator, object Value)
                {
                    this.Operator = Operator;
                    this.Value = Value;
                }

                public OperatorEnum Operator { get; set; }
                public object Value { get; set; }
                public int ValueLength
                {
                    get
                    {
                        if (this.Operator == OperatorEnum.Like ||
                            this.Operator == OperatorEnum.NotLike)
                        {
                            return (((string)Value).Length + 1)*2;
                        }
                        else if (Value.GetType()== typeof(Int32))
                        {
                            return 4;
                        }
                        else if (Value.GetType() == typeof(Int64))
                        {
                            return 8;
                        }
                        return (((string)Value).Length + 1) * 2; 
                    }
                } 
            } 
            public TraceColumn Column { get; set; }
            public List<Operation> Operations { get; set; }
            public bool ExcludeRowThatDoNotContainValues { get; set; }
            public int Length
            {
                get
                {
                    int result = 0;
                    foreach (Operation op in Operations)
                    {
                        result += op.ValueLength+2+1+4;//column,operation,length
                    }
                    if (ExcludeRowThatDoNotContainValues) result += 7;
                    return result;
                }
            }
            
        }
        public class ColumnFilterList:List<ColumnFilter>
        {
            public new void Add(ColumnFilter filter)
            {
                if (!this.Exists((f) => f.Column == filter.Column))
                {
                    base.Add(filter);
                }
                else
                {
                    ColumnFilter existsfilter= this.Find((f) => f.Column == filter.Column);
                    foreach (ColumnFilter.Operation op in filter.Operations)
                    {
                        if (!existsfilter.Operations.Exists((o) => o.Operator == op.Operator))
                        {
                            existsfilter.Operations.Add(op);
                        }
                        //else ?
                    }
                }
            }
        }
        //public List<ColumnFilter> ColumnFilters = new List<ColumnFilter>();//Todo column and operation should be unique key
        public ColumnFilterList ColumnFilters = new ColumnFilterList();
        #region Static
        public static TraceDefinitionFile Common
        {
            get
            {
                TraceDefinitionFile traceDefinitionCommon = new TraceDefinitionFile();
                traceDefinitionCommon.Add(TraceEventEnum.SQLBatchCompleted, commoncolumns);
                traceDefinitionCommon.Add(TraceEventEnum.SQLStmtCompleted, commoncolumns);
                traceDefinitionCommon.Add(TraceEventEnum.RPCCompleted, commoncolumns);
                return traceDefinitionCommon;
            }
        }
        public static TraceDefinitionFile Deadlock
        {
            get
            {
                TraceDefinitionFile traceDefinitionDeadlock = new TraceDefinitionFile();
                traceDefinitionDeadlock.Add(TraceEventEnum.LockDeadlock, commoncolumns);
                traceDefinitionDeadlock.Add(TraceEventEnum.LockDeadlockChain, commoncolumns);
                traceDefinitionDeadlock.Add(TraceEventEnum.DeadlockGraph, commoncolumns);
                return traceDefinitionDeadlock;
            }
        }
        public static TraceDefinitionFile Performance
        {
            get
            {
                TraceDefinitionFile traceDefinitionDeadlock = new TraceDefinitionFile();
                traceDefinitionDeadlock.Add(TraceEventEnum.ShowplanXMLStatisticsProfile, commoncolumns);
                traceDefinitionDeadlock.Add(TraceEventEnum.ShowplanXML, commoncolumns);
                traceDefinitionDeadlock.Add(TraceEventEnum.DeadlockGraph, commoncolumns);
                return traceDefinitionDeadlock;
            }
        }
        protected static byte[] header;
        protected static List<TraceColumn> commoncolumns = CommonColumns;
        public static List<TraceColumn> CommonColumns
        {
            get
            {
                List<TraceColumn> commoncolumnslist = new List<TraceColumn>();
                commoncolumnslist.Add(TraceColumn.EventClass);
                commoncolumnslist.Add(TraceColumn.TextData);
                commoncolumnslist.Add(TraceColumn.RowCounts);
                commoncolumnslist.Add(TraceColumn.Duration);
                commoncolumnslist.Add(TraceColumn.StartTime);
                commoncolumnslist.Add(TraceColumn.EndTime);
                commoncolumnslist.Add(TraceColumn.Reads);
                commoncolumnslist.Add(TraceColumn.Writes);
                commoncolumnslist.Add(TraceColumn.CPU);
                commoncolumnslist.Add(TraceColumn.SPID);
                commoncolumnslist.Add(TraceColumn.ApplicationName);
                commoncolumnslist.Add(TraceColumn.DBUserName);
                commoncolumnslist.Add(TraceColumn.TransactionID);
                return commoncolumnslist;
            }
        }
        static TraceDefinitionFile()
        {
            header = CreateHeader();
        }
        protected static byte[] CreateHeader()
        {
            string headerstring = "fffe900209004d006900630072006f0073006f00660074002000530051004c0020005300650072007600650072";
            byte[] result = new byte[659];
            for (int idx = 0; idx < result.Length; idx++) result[idx] = 0;
            for (int idx = 0; idx < headerstring.Length / 2; idx++)
            {
                result[idx] = Convert.ToByte(headerstring.Substring(idx * 2, 2), 16);
            }
            result[390] = Convert.ToByte("0a", 16);
            result[391] = Convert.ToByte("32", 16);
            result[654] = Convert.ToByte("fa", 16);
            result[655] = Convert.ToByte("fb", 16);
            result[656] = Convert.ToByte("ed", 16);
            result[657] = Convert.ToByte("fb", 16);
            result[658] = 0;
            return result;
        }
        #endregion
        /// <summary>
        /// will use common cloumns
        /// </summary>
        /// <param name="tevent"></param>
        public void Add(TraceEventEnum tevent)
        {
            Add(tevent, CommonColumns);
        }
        public void Add(TraceEventEnum tevent, TraceColumn[] tcolumns)
        {
            this.Add(tevent, new List<TraceColumn>(tcolumns));
        }
        public new void Add(TraceEventEnum tevent, List<TraceColumn> tcolumns)
        {
             if (!tcolumns.Contains(TraceColumn.EventClass)) tcolumns.Add(TraceColumn.EventClass);
             if (this.ContainsKey(tevent))
             {
                 List<TraceColumn> stored;
                 this.TryGetValue(tevent, out stored);
                 foreach (TraceColumn tc in tcolumns)
                 {
                     if (!stored.Contains(tc)) stored.Add(tc);
                 }
             }
             else
             {
                 base.Add(tevent, tcolumns);
             }
        }
        public void Merge(TraceDefinitionFile traceDefinitionFile)
        {
            foreach (KeyValuePair<TraceEventEnum, List<TraceColumn>> kvpair in traceDefinitionFile)
            {
                this.Add(kvpair.Key, kvpair.Value);
            }
            foreach (ColumnFilter filter in traceDefinitionFile.ColumnFilters)
            {
                this.ColumnFilters.Add(filter);
            }
        }
    
      
        public void ReadFromXml(string filename)
        {
            
            string content = "";
            using (StreamReader reader = new StreamReader(filename))
            {
                content = reader.ReadToEnd();
                reader.Close();
            }
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(content);
            this.Clear();
            foreach (XmlNode traceevent in xdoc.SelectNodes("./TraceDefinition/TraceEvent"))
            {
                TraceEventEnum tevent = (TraceEventEnum)Enum.Parse(typeof(TraceEventEnum), traceevent.Attributes["Name"].Value, true);
                List<TraceColumn> tcolumns = new List<TraceColumn>();
                foreach (XmlNode tracecolumn in traceevent.SelectNodes("./Column"))
                {
                    tcolumns.Add((TraceColumn)Enum.Parse(typeof(TraceColumn),  tracecolumn.Attributes["Name"].Value,true));
                }
                this.Add(tevent, tcolumns);

                foreach (XmlNode columnfilter in traceevent.SelectNodes("./ColumnFilter"))
                {
                    ColumnFilter filter = new ColumnFilter(
                        (TraceColumn)Enum.Parse(typeof(TraceColumn), columnfilter.Attributes["ColumnName"].Value, true),
                        Convert.ToBoolean(columnfilter.Attributes["ExcludeRowThatDoNotContainValues"].Value));
                    foreach (XmlNode columnfilteroperation in columnfilter.SelectNodes("./Operation"))
                    {
                        filter.Operations.Add(new ColumnFilter.Operation(  
                            (ColumnFilter.OperatorEnum)Enum.Parse(typeof(ColumnFilter.OperatorEnum), columnfilteroperation.Attributes["Operator"].Value, true),
                            columnfilteroperation.Attributes["Value"].Value));
                        //columnfilteroperation.Attributes["Type"].Value
                    }
                    this.ColumnFilters.Add(filter);
                }
                
            }
            
        }
        public void SaveAsTDF(string filename)
        {
            if (File.Exists(filename)) File.Delete(filename);
            List<TraceColumn> totalColumns = new List<TraceColumn>();
            using (BinaryWriter bw = new BinaryWriter(new FileStream(filename,FileMode.Create)))
            {
                bw.Write(header);
                foreach (KeyValuePair<TraceEventEnum, List<TraceColumn>> kvpair in this)//event and its columns
                {
                    bw.Write(Convert.ToByte("fc", 16));
                    bw.Write(Convert.ToByte("ff", 16));
                    bw.Write(Convert.ToByte(kvpair.Value.Count*2+2));
                    bw.Write(Convert.ToInt16(kvpair.Key));
                    foreach (TraceColumn tc in kvpair.Value)
                    {
                        bw.Write(Convert.ToInt16(tc));
                        if (!totalColumns.Contains(tc)) totalColumns.Add(tc);
                    }
                }

                bw.Write(Convert.ToByte("fc", 16));//all columns
                bw.Write(Convert.ToByte("fb", 16));
                bw.Write(Convert.ToByte(totalColumns.Count * 2 + 2));
                bw.Write(Convert.ToInt16(0x1b));
                foreach (TraceColumn tc in totalColumns)
                {
                    bw.Write(Convert.ToInt16(tc));
                }
                if (ColumnFilters.Count == 0)//filters
                {
                    bw.Write(Convert.ToByte("fb", 16));
                    bw.Write(Convert.ToByte("fb", 16));
                    bw.Write((byte)0);
                }
                else
                {
                    bw.Write(Convert.ToByte("fb", 16));
                    bw.Write(Convert.ToByte("fb", 16));
                    int length = 0;
                    foreach (ColumnFilter filter in ColumnFilters) length += filter.Length;
                    if (length < 256)
                    {
                        bw.Write(Convert.ToByte(length));//totoal length
                    }
                    else
                    {
                        bw.Write(length);
                    }
                    foreach (ColumnFilter filter in ColumnFilters)
                    {
                        foreach (ColumnFilter.Operation op in filter.Operations)
                        {
                            bw.Write(Convert.ToByte(filter.Column));
                            bw.Write((byte)0);
                            bw.Write(Convert.ToByte(op.Operator));
                            bw.Write(Convert.ToInt32(op.ValueLength));
                            if (op.Operator == ColumnFilter.OperatorEnum.Like ||
                                op.Operator == ColumnFilter.OperatorEnum.NotLike)
                            {
                                foreach (char c in ((string)op.Value).ToCharArray())
                                {
                                    bw.Write(Convert.ToInt16(c));
                                }
                                bw.Write((byte)0);
                                bw.Write((byte)0);
                            }
                            else if (op.Value.GetType()==typeof(Int32))
                            {
                                bw.Write(Convert.ToInt32(op.Value));
                            }
                            else if (op.Value.GetType() == typeof(Int64))
                            {
                                bw.Write(Convert.ToInt64(op.Value)*1000);
                            }
                        }
                        if (filter.ExcludeRowThatDoNotContainValues)
                        {
                            bw.Write(Convert.ToByte(filter.Column));
                            bw.Write((byte)0);
                            bw.Write((byte)0x01);
                            bw.Write((byte)0);
                            bw.Write((byte)0);
                            bw.Write((byte)0);
                            bw.Write((byte)0);
                        }

                    }
                   
                }
                bw.Flush();
                bw.Close();
            }
        }
        public void SaveAsXml(string filename)
        {
            XmlTextWriter tw = new XmlTextWriter(new FileStream(filename, FileMode.Create),Encoding.UTF8);
            tw.WriteStartDocument();
            tw.WriteStartElement("TraceDefinition");

            foreach (KeyValuePair<TraceEventEnum, List<TraceColumn>> kvpair in this)
            {
                tw.WriteStartElement("TraceEvent");
                tw.WriteAttributeString("Name",kvpair.Key.ToString());
                foreach (TraceColumn tc in kvpair.Value)
                {
                    tw.WriteStartElement("Column");
                    tw.WriteAttributeString("Name", tc.ToString());
                    tw.WriteEndElement();
                }
                foreach (ColumnFilter filter in this.ColumnFilters)
                {
                    tw.WriteStartElement("ColumnFilter");
                    tw.WriteAttributeString("ColumnName",Enum.GetName(typeof(TraceColumn), filter.Column));
                    tw.WriteAttributeString("ExcludeRowThatDoNotContainValues", filter.ExcludeRowThatDoNotContainValues.ToString());
                    foreach (ColumnFilter.Operation op in filter.Operations)
                    {
                        tw.WriteStartElement("Operation");
                        tw.WriteAttributeString("Operator", Enum.GetName(typeof(ColumnFilter.OperatorEnum), op.Operator));
                        tw.WriteAttributeString("Value", (string)op.Value);
                        tw.WriteAttributeString("Type", op.Value.GetType().Name);
                        tw.WriteEndElement();
                    }
                    tw.WriteEndElement();
                }
                tw.WriteEndElement();//TraceEvent
            }
            tw.WriteEndElement();//TraceDefinition
            tw.WriteEndDocument();
            tw.Flush();
            tw.Close();
            
        }
        
    }
}
