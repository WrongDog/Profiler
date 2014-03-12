using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BenchmarkResultApplication
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

      

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("TableWeight.xml"))
            {
                DataTable dt = new DataTable();
                dt.ReadXml("TableWeight.xml");
                dataGridView_table.DataSource = dt;
            }
            else
            {
                dataGridView_table.DataSource = BenchmarkResultHelper.CreateTableWeight();
            }
            if (File.Exists("OperationWeight.xml"))
            {
                DataTable dt = new DataTable();
                dt.ReadXml("OperationWeight.xml");
                dataGridView_operation.DataSource = dt;
            }
            else
            {
                dataGridView_operation.DataSource = BenchmarkResultHelper.CreateOperationWeight();
            }

        }

        private void button_usetablerowcount_Click(object sender, EventArgs e)
        {
            dataGridView_table.DataSource = BenchmarkResultHelper.CreateTableWeight(textBox_connection.Text);
        }

        private void button_append_Click(object sender, EventArgs e)
        {
            ((DataTable)dataGridView_table.DataSource).WriteXml("TableWeight.xml",XmlWriteMode.WriteSchema);
            ((DataTable)dataGridView_operation.DataSource).WriteXml("OperationWeight.xml", XmlWriteMode.WriteSchema);

            BenchmarkResultHelper helper = new BenchmarkResultHelper(((DataTable)dataGridView_table.DataSource),
                       ((DataTable)dataGridView_operation.DataSource));
            Interval interval = Interval.Hour;
            if (radioButton_hour.Checked)
            {
                interval = Interval.Hour;
            }
            else if (radioButton_day.Checked)
            {
                interval = Interval.Day;
            }
            else if (radioButton_week.Checked)
            {
                interval = Interval.Week;
            }

            if (tabControl_source.SelectedIndex == 0)
            {//file

                if (listBox_files.Items.Count > 0 && textBox_excel.Text != string.Empty)
                {
                    List<string> files = new List<string>();
                    foreach (object item in listBox_files.Items)
                    {
                        files.Add(item.ToString());
                    }
                    helper.CalculateWeight(files, interval, textBox_excel.Text);
                    toolStripStatusLabel1.Text = "Finished";
                }
                else
                {
                    toolStripStatusLabel1.Text = "missing parameters";
                }
            }
            else
            {//db
                try
                {
                    helper.CalculateWeight(textBox_sourceconnection.Text,textBox_sourcecriteria.Text, interval, textBox_excel.Text);
                    toolStripStatusLabel1.Text = "Finished";
                }
                catch (Exception ex)
                {
                    toolStripStatusLabel1.Text = ex.Message;
                }
            }

        }

        private void listBox_files_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void listBox_files_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                   if(file.EndsWith(".xml"))listBox_files.Items.Add(file);
                }
            }
        }

        private void textBox_excel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && files[0].EndsWith(".xlsx"))
                {
                    e.Effect = DragDropEffects.All;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void textBox_excel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                textBox_excel.Text = files[0];
            }
        }

 

        private void listBox_files_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && listBox_files.SelectedItems.Count > 0)
            {
                for (int i = listBox_files.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    listBox_files.Items.RemoveAt(listBox_files.SelectedIndices[i]);
                }
            }
        }
    }
}
