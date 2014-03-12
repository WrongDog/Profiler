namespace BenchmarkResultApplication
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBox_files = new System.Windows.Forms.ListBox();
            this.button_append = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_week = new System.Windows.Forms.RadioButton();
            this.radioButton_hour = new System.Windows.Forms.RadioButton();
            this.radioButton_day = new System.Windows.Forms.RadioButton();
            this.textBox_excel = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.button_usetablerowcount = new System.Windows.Forms.Button();
            this.dataGridView_table = new System.Windows.Forms.DataGridView();
            this.textBox_connection = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dataGridView_operation = new System.Windows.Forms.DataGridView();
            this.tabControl_source = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_sourceconnection = new System.Windows.Forms.TextBox();
            this.textBox_sourcecriteria = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_table)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_operation)).BeginInit();
            this.tabControl_source.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(563, 338);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.statusStrip1);
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(555, 312);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ExecutionPlanSource";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl_source);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.button_append);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.textBox_excel);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(549, 306);
            this.splitContainer1.SplitterDistance = 221;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBox_files
            // 
            this.listBox_files.AllowDrop = true;
            this.listBox_files.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_files.FormattingEnabled = true;
            this.listBox_files.Location = new System.Drawing.Point(3, 3);
            this.listBox_files.Name = "listBox_files";
            this.listBox_files.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox_files.Size = new System.Drawing.Size(535, 189);
            this.listBox_files.TabIndex = 0;
            this.listBox_files.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_files_DragDrop);
            this.listBox_files.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox_files_DragEnter);
            this.listBox_files.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox_files_MouseDown);
            // 
            // button_append
            // 
            this.button_append.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_append.Location = new System.Drawing.Point(464, 20);
            this.button_append.Name = "button_append";
            this.button_append.Size = new System.Drawing.Size(80, 23);
            this.button_append.TabIndex = 4;
            this.button_append.Text = "Append";
            this.button_append.UseVisualStyleBackColor = true;
            this.button_append.Click += new System.EventHandler(this.button_append_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_week);
            this.groupBox1.Controls.Add(this.radioButton_hour);
            this.groupBox1.Controls.Add(this.radioButton_day);
            this.groupBox1.Location = new System.Drawing.Point(5, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(187, 51);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Interval";
            // 
            // radioButton_week
            // 
            this.radioButton_week.AutoSize = true;
            this.radioButton_week.Location = new System.Drawing.Point(110, 19);
            this.radioButton_week.Name = "radioButton_week";
            this.radioButton_week.Size = new System.Drawing.Size(54, 17);
            this.radioButton_week.TabIndex = 5;
            this.radioButton_week.TabStop = true;
            this.radioButton_week.Text = "Week";
            this.radioButton_week.UseVisualStyleBackColor = true;
            // 
            // radioButton_hour
            // 
            this.radioButton_hour.AutoSize = true;
            this.radioButton_hour.Checked = true;
            this.radioButton_hour.Location = new System.Drawing.Point(6, 19);
            this.radioButton_hour.Name = "radioButton_hour";
            this.radioButton_hour.Size = new System.Drawing.Size(48, 17);
            this.radioButton_hour.TabIndex = 4;
            this.radioButton_hour.TabStop = true;
            this.radioButton_hour.Text = "Hour";
            this.radioButton_hour.UseVisualStyleBackColor = true;
            // 
            // radioButton_day
            // 
            this.radioButton_day.AutoSize = true;
            this.radioButton_day.Location = new System.Drawing.Point(60, 19);
            this.radioButton_day.Name = "radioButton_day";
            this.radioButton_day.Size = new System.Drawing.Size(44, 17);
            this.radioButton_day.TabIndex = 2;
            this.radioButton_day.Text = "Day";
            this.radioButton_day.UseVisualStyleBackColor = true;
            // 
            // textBox_excel
            // 
            this.textBox_excel.AllowDrop = true;
            this.textBox_excel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_excel.Location = new System.Drawing.Point(201, 22);
            this.textBox_excel.Name = "textBox_excel";
            this.textBox_excel.Size = new System.Drawing.Size(262, 20);
            this.textBox_excel.TabIndex = 1;
            this.textBox_excel.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox_excel_DragDrop);
            this.textBox_excel.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBox_excel_DragEnter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(198, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Result Excel";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.button_usetablerowcount);
            this.tabPage2.Controls.Add(this.dataGridView_table);
            this.tabPage2.Controls.Add(this.textBox_connection);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(555, 312);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "TableWeight";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "ConnectionString";
            // 
            // button_usetablerowcount
            // 
            this.button_usetablerowcount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_usetablerowcount.Location = new System.Drawing.Point(429, 4);
            this.button_usetablerowcount.Name = "button_usetablerowcount";
            this.button_usetablerowcount.Size = new System.Drawing.Size(118, 23);
            this.button_usetablerowcount.TabIndex = 2;
            this.button_usetablerowcount.Text = "UseTableRowCount";
            this.button_usetablerowcount.UseVisualStyleBackColor = true;
            this.button_usetablerowcount.Click += new System.EventHandler(this.button_usetablerowcount_Click);
            // 
            // dataGridView_table
            // 
            this.dataGridView_table.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_table.Location = new System.Drawing.Point(3, 32);
            this.dataGridView_table.Name = "dataGridView_table";
            this.dataGridView_table.Size = new System.Drawing.Size(549, 280);
            this.dataGridView_table.TabIndex = 1;
            // 
            // textBox_connection
            // 
            this.textBox_connection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_connection.Location = new System.Drawing.Point(101, 6);
            this.textBox_connection.Name = "textBox_connection";
            this.textBox_connection.Size = new System.Drawing.Size(322, 20);
            this.textBox_connection.TabIndex = 0;
            this.textBox_connection.Text = "Server=.;Database=Casinolink;Trusted_Connection=True;";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dataGridView_operation);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(555, 312);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "OperationWeight";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridView_operation
            // 
            this.dataGridView_operation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_operation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_operation.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_operation.Name = "dataGridView_operation";
            this.dataGridView_operation.Size = new System.Drawing.Size(555, 312);
            this.dataGridView_operation.TabIndex = 0;
            // 
            // tabControl_source
            // 
            this.tabControl_source.Controls.Add(this.tabPage4);
            this.tabControl_source.Controls.Add(this.tabPage5);
            this.tabControl_source.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_source.Location = new System.Drawing.Point(0, 0);
            this.tabControl_source.Name = "tabControl_source";
            this.tabControl_source.SelectedIndex = 0;
            this.tabControl_source.Size = new System.Drawing.Size(549, 221);
            this.tabControl_source.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.listBox_files);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(541, 195);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "XmlFile";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.label4);
            this.tabPage5.Controls.Add(this.textBox_sourcecriteria);
            this.tabPage5.Controls.Add(this.textBox_sourceconnection);
            this.tabPage5.Controls.Add(this.label3);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(541, 195);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "DataBase";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "ConnectionString";
            // 
            // textBox_sourceconnection
            // 
            this.textBox_sourceconnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_sourceconnection.Location = new System.Drawing.Point(100, 9);
            this.textBox_sourceconnection.Name = "textBox_sourceconnection";
            this.textBox_sourceconnection.Size = new System.Drawing.Size(435, 20);
            this.textBox_sourceconnection.TabIndex = 1;
            // 
            // textBox_sourcecriteria
            // 
            this.textBox_sourcecriteria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_sourcecriteria.Location = new System.Drawing.Point(100, 35);
            this.textBox_sourcecriteria.Name = "textBox_sourcecriteria";
            this.textBox_sourcecriteria.Size = new System.Drawing.Size(435, 20);
            this.textBox_sourcecriteria.TabIndex = 2;
            this.textBox_sourcecriteria.Text = "FirstCaptureTime>\'2013/08/13\'";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "SP Criteria";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(3, 287);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(549, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 338);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "BenchmarkResult";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_table)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_operation)).EndInit();
            this.tabControl_source.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBox_files;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_hour;
        private System.Windows.Forms.RadioButton radioButton_day;
        private System.Windows.Forms.TextBox textBox_excel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_usetablerowcount;
        private System.Windows.Forms.DataGridView dataGridView_table;
        private System.Windows.Forms.TextBox textBox_connection;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dataGridView_operation;
        private System.Windows.Forms.Button button_append;
        private System.Windows.Forms.RadioButton radioButton_week;
        private System.Windows.Forms.TabControl tabControl_source;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_sourcecriteria;
        private System.Windows.Forms.TextBox textBox_sourceconnection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

