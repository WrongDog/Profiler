using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TraceWrapper;
using System.Configuration;

namespace BenchmarkFormsApplication
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        protected string excelname;
        protected string label;
        public static void SaveSettings(List<Type> controlTypes, Control parent)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach (Control control in parent.Controls)
            {
                if (controlTypes.Contains(control.GetType()))
                {

                    config.AppSettings.Settings.Remove(control.Name);
                    config.AppSettings.Settings.Add(control.Name, control.Text);

                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                else
                {
                    SaveSettings(controlTypes, control);
                }
            }
        }
        public static void LoadSettings(List<Type> controlTypes, Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (controlTypes.Contains(control.GetType()))
                {
                    control.Text = ConfigurationManager.AppSettings[control.Name];

                }
                else
                {
                    LoadSettings(controlTypes, control);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSettings(new List<Type>() { typeof(TextBox) }, this);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings(new List<Type>() { typeof(TextBox) }, this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable operationWeight = new DataTable();
            operationWeight.ReadXml("OperationWeight.xml");
            DataTable tableWeight = new DataTable();
            tableWeight.ReadXml("TableWeight.xml");
            ConnectionInfo connection = new ConnectionInfo();
            connection.ServerName = textBox_server.Text;
            connection.UserName = textBox_user.Text;
            connection["DataBase"] = textBox_database.Text;
            connection.Password = textBox_password.Text;
            connection.UseIntegratedSecurity = checkBox_useintegratedsecrity.Checked;
            connection["SourceTable"] = textBox_table.Text;
            connection["Criteria"] = textBox_Criteria.Text;


            excelname = textBox_excel.Text;
            label = textBox_label.Text;


            BenchmarkResult.BenchmarkResult br = new BenchmarkResult.BenchmarkResult(connection, tableWeight, operationWeight);
            br.ProgressUpdated += new BenchmarkResult.BenchmarkResult.progressdele(br_ProgressUpdated);
            br.ResultUpdated += new BenchmarkResult.BenchmarkResult.resultdele(br_ResultUpdated);
            br.Start();
            //Dictionary<string, decimal> result = br.GetResult();
            //using (ExcelHelper.ExcelHelper excelHelper = new ExcelHelper.ExcelHelper(textBox_excel.Text))
            //{
            //    excelHelper.Append(textBox_label.Text, result);
            //}
        }
        private string GetExcelFile()
        {
            return textBox_excel.Text;
        }
        void br_ResultUpdated(Dictionary<string, decimal> result)
        {

            using (ExcelHelper.ExcelHelper excelHelper = new ExcelHelper.ExcelHelper(excelname))
            {
                excelHelper.Append(label, result);
            }
            br_ProgressUpdated(0, 10);
        }

    
        void br_ProgressUpdated(int current, int max)
        {
            if (this.progressBar1.InvokeRequired)
            {
                this.progressBar1.Invoke( new Action<int,int>(this.UpdateProgress), new object[] { current, max });
            }
            else
            {
                UpdateProgress(current, max);
            }
        }
        private void UpdateProgress(int current, int max)
        {
            this.progressBar1.Maximum = max;
            this.progressBar1.Value = current;

        }
    }
}
