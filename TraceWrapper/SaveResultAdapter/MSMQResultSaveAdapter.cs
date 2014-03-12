using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Messaging;
using System.Threading.Tasks;
using System.Data;

namespace TraceWrapper
{
    public class MSMQResultSaveAdapter : ResultSaveAdapterBase
    {
        protected string url ;
        public MSMQResultSaveAdapter(string url)
        {           
            if (!MessageQueue.Exists(url)) MessageQueue.Create(url);
            this.url = url;
        }


        protected override void Worker(string tag, string result)
        {
            try
            {
                // open the queue
                MessageQueue mq = new MessageQueue(url);
                // set the message to durable.
                mq.DefaultPropertiesToSend.Recoverable = true;
                // set the formatter to Binary if needed, default is XML
                //mq.Formatter = new BinaryMessageFormatter();
                // send the job object
                mq.Send(result, tag);
                mq.Close();
 
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }
    }
}
