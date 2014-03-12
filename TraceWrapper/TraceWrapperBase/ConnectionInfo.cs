using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceWrapper
{
    public class ConnectionInfo
    {
        public ConnectionInfo()
        {
            this["DataBase"] = "master";
        }
        public string ServerName { get; set; }
        public bool UseIntegratedSecurity { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        //public string DataBase { get; set; }
        protected Dictionary<string, string> ExtendedInfo = new Dictionary<string, string>();
        public string this[string setting]
        {
            get
            {
                if (!ExtendedInfo.ContainsKey(setting)) return null;
                string value;
                ExtendedInfo.TryGetValue(setting, out value);
                return value;
            }
            set
            {
                if (ExtendedInfo.ContainsKey(setting)) ExtendedInfo.Remove(setting);

                ExtendedInfo.Add(setting, value);
            }
        }
        

        public string AsConnectionString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Server=" + ServerName + ";Database=" + this["DataBase"] + ";");
                if (UseIntegratedSecurity)
                {
                    sb.Append("Integrated Security=SSPI;");
                }
                else
                {
                    sb.Append("User ID=" + UserName + ";Password=" + Password + ";");
                }
                return sb.ToString();
            }
        }
    }
}
