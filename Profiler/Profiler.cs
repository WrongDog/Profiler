using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using TraceWrapper;

namespace Profiler
{
    public partial class Profiler : Form
    {
        private int maxdisplayed = 100;
        private TraceWrapper.TraceWrapperBase trace;
        private Dictionary<string, DataGridView> dgdic = new Dictionary<string, DataGridView>();
        public Profiler()
        {
            InitializeComponent();
            
        }

        private void checkBox_useintegratedsecrity_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_useintegratedsecrity.Checked)
            {
                textBox_user.Enabled = false;
                textBox_password.Enabled = false;
            }
            else
            {
                textBox_user.Enabled = true;
                textBox_password.Enabled = true;
            }
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            if (this.trace == null || this.trace.State == TraceState.NotInitiated || this.trace.State == TraceState.Stopped)
            {
   
                ConnectionInfo connectioninfo = new ConnectionInfo();
                connectioninfo.ServerName = textBox_server.Text;
                connectioninfo.UseIntegratedSecurity = checkBox_useintegratedsecrity.Checked;
                if (!connectioninfo.UseIntegratedSecurity)
                {
                    connectioninfo.UserName = textBox_user.Text;
                    connectioninfo.Password = textBox_password.Text;
                }
                try
                {
                
                    trace = TraceWrapperFactory.Create(connectioninfo);
                    if (checkBox_Interval.Checked) trace.Interval = new TimeSpan(0,Convert.ToInt32(textBox_Interval.Text),0);
                    tabControl1.TabPages.Clear();
                    dgdic.Clear();
                    foreach (IResultHandler handler in trace.Handlers)
                    {
                        handler.OnResultChange += new ResultEventHandler(trace_TraceEventHandler);
                        TabPage tabpage = new TabPage(handler.GetType().Name);
                        DataGridView dg = new DataGridView();
                        dg.MouseDown += new MouseEventHandler(dg_MouseDown);
                        dg.AllowUserToAddRows = false;
                        dg.Name= "dg"+handler.GetType().Name;
                        dg.Dock = DockStyle.Fill;
                        tabpage.Controls.Add(dg);
                        tabControl1.TabPages.Add(tabpage);
                        dgdic.Add(handler.GetType().Name, dg);
                    }
                    trace.Start();
                    this.toolStripStatusLabel1.Text = "started";
                    
                }
                catch (Exception ex)
                {
                    this.toolStripStatusLabel1.Text = ex.Message;
                    return;
                }
              
            }
 
            //button_pause.Enabled = true;
            button_stop.Enabled = true;
            button_start.Enabled = false;
            
        }

        void dg_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right) return;
            if (((DataGridView)sender).DataSource != null)
            {
                saveFileDialog1.ShowDialog();
                if (!string.IsNullOrEmpty(saveFileDialog1.FileName))
                {
                    object result = ((DataGridView)sender).DataSource;
                    if (result.GetType() == typeof(DataSet))
                    {
                        if(((DataSet)result).DataSetName == string.Empty)((DataSet)result).DataSetName = ((DataGridView)sender).Name;
                        ((DataSet)result).WriteXml(saveFileDialog1.FileName, XmlWriteMode.WriteSchema);
                    }
                    else
                    {
                        if (((DataGridView)sender).SelectedRows.Count > 0)
                        {
                            DataTable saveing = ((DataTable)((DataGridView)sender).DataSource).Clone();
                            foreach (DataGridViewRow row in ((DataGridView)sender).SelectedRows)
                            {
                                saveing.Rows.Add(((DataRowView)row.DataBoundItem).Row.ItemArray);
                            }
                            if(saveing.TableName==string.Empty)saveing.TableName = ((DataGridView)sender).Name;
                            saveing.WriteXml(saveFileDialog1.FileName, XmlWriteMode.WriteSchema);
                        }
                        else if (((DataGridView)sender).SelectedCells.Count > 0)
                        {
                            using (StreamWriter output = new StreamWriter(saveFileDialog1.FileName))
                            {
                                foreach (DataGridViewCell dc in ((DataGridView)sender).SelectedCells)
                                {
                                    output.WriteLine(dc.Value);
                                }
                                output.Close();
                            }
                        }
                        else
                        {
                                if (((DataSet)result).DataSetName == string.Empty) ((DataTable)result).TableName = ((DataGridView)sender).Name;
                                ((DataTable)result).WriteXml(saveFileDialog1.FileName, XmlWriteMode.WriteSchema);
                        }
                    }
                }
            }
        }

        void trace_TraceEventHandler(string sender,Result traceEvent)
        {
            DataGridView dg=null;
            dgdic.TryGetValue(sender, out dg);
            if (dg.InvokeRequired)
            {
                dg.Invoke((ResultEventHandler)UpdateDataSource, new object[] { sender, traceEvent });
            }
            else
            {
                UpdateDataSource(sender,traceEvent);
            }
        }
        private void UpdateDataSource(string sender, Result traceEvent)
        {
            try
            {
                DataGridView dg = null;
                dgdic.TryGetValue(sender, out dg);
                if (traceEvent.Content.GetType() == typeof(DataRow))
                {
                    DataTable displayed = (DataTable)dg.DataSource;
                    DataRow dr = (DataRow)traceEvent.Content;
                    if (displayed == null) displayed = (DataTable)dr.Table.Clone();
                    switch (traceEvent.Action)
                    {
                        case ResultAction.Add:
                            displayed.Rows.Add(dr.ItemArray.ToArray<object>());
                            break;
                        case ResultAction.Delete:
                            break;
                        case ResultAction.Modify:
                            break;
                    }

                    if (displayed.Rows.Count > maxdisplayed) displayed.Rows[0].Delete();
                    displayed.AcceptChanges();
                    dg.DataSource = displayed;
                }
                else if (traceEvent.Content.GetType() == typeof(DataTable))
                {
                    switch (traceEvent.Action)
                    {
                        case ResultAction.Refresh:
                            dg.DataSource = (DataTable)traceEvent.Content;
                            break;
                    }
                }
                else if (traceEvent.Content.GetType() == typeof(DataSet))
                {
                    switch (traceEvent.Action)
                    {
                        case ResultAction.Refresh:
                            dg.DataSource = (DataSet)traceEvent.Content;
                            dg.DataMember = ((DataSet)traceEvent.Content).Tables[0].TableName;
                            
                            break;
                    }
                }

                
            }
            catch (Exception ex)
            {
                this.toolStripStatusLabel1.Text = ex.Message;
            }
        }
      
        

        private void button_stop_Click(object sender, EventArgs e)
        {
            trace.Stop();
            //button_pause.Enabled = false;
            button_stop.Enabled = false;
            button_start.Enabled = true;
            this.toolStripStatusLabel1.Text = "stopped";

        }

        private void Profiler_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (trace != null && trace.State == TraceState.Started) trace.Stop();
        }
     

       

       

       

        
    }
}
