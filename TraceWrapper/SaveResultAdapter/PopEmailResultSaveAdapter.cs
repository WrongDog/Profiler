using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Net.Mail;

namespace TraceWrapper
{
    public class PopEmailResultSaveAdapter : ResultSaveAdapterBase
    {
        protected string mailto;
        protected string mailfrom;
        protected string subject;
        protected string smtpserver;
        public PopEmailResultSaveAdapter(string mailto,string smtpserver,
         string mailfrom,
         string subject)
        {
            this.mailto = mailto;
            this.mailfrom = mailfrom;
            this.subject = subject;
            this.smtpserver = smtpserver;
        }


        protected override void Worker(string tag, string body)
        {
            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.To.Add(mailto);
                message.Subject = subject + " " + tag;
                message.From = new System.Net.Mail.MailAddress(mailfrom);
                message.Body = body;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpserver);
                smtp.Send(message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }
    }
}
