﻿namespace KIMI_Sim
{
    partial class Results_KineticModel
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
            this.plot1 = new OxyPlot.WindowsForms.PlotView();
            this.button_export = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.button_plot = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.trackBar_dist = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_dinamic = new System.Windows.Forms.RadioButton();
            this.button_ms = new System.Windows.Forms.Button();
            this.radioButton_ms = new System.Windows.Forms.RadioButton();
            this.radioButton_kinetic = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_log = new System.Windows.Forms.CheckBox();
            this.button_t_x = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_conc = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_dist = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBar_conc = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_dist)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_conc)).BeginInit();
            this.SuspendLayout();
            // 
            // plot1
            // 
            this.plot1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plot1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.plot1.Location = new System.Drawing.Point(169, 0);
            this.plot1.Name = "plot1";
            this.plot1.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plot1.Size = new System.Drawing.Size(915, 553);
            this.plot1.TabIndex = 13;
            this.plot1.Text = "plot1";
            this.plot1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plot1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plot1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // button_export
            // 
            this.button_export.Location = new System.Drawing.Point(8, 519);
            this.button_export.Name = "button_export";
            this.button_export.Size = new System.Drawing.Size(74, 25);
            this.button_export.TabIndex = 3;
            this.button_export.Text = "Export";
            this.button_export.UseVisualStyleBackColor = true;
            this.button_export.Click += new System.EventHandler(this.button_export_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.BackColor = System.Drawing.SystemColors.Control;
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(8, 105);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(150, 154);
            this.checkedListBox1.TabIndex = 1;
            // 
            // button_plot
            // 
            this.button_plot.Location = new System.Drawing.Point(87, 519);
            this.button_plot.Name = "button_plot";
            this.button_plot.Size = new System.Drawing.Size(74, 25);
            this.button_plot.TabIndex = 2;
            this.button_plot.Text = "Plot";
            this.button_plot.UseVisualStyleBackColor = true;
            this.button_plot.Click += new System.EventHandler(this.button_plot_Click);
            // 
            // trackBar_dist
            // 
            this.trackBar_dist.Enabled = false;
            this.trackBar_dist.LargeChange = 1;
            this.trackBar_dist.Location = new System.Drawing.Point(8, 305);
            this.trackBar_dist.Name = "trackBar_dist";
            this.trackBar_dist.Size = new System.Drawing.Size(150, 45);
            this.trackBar_dist.TabIndex = 16;
            this.trackBar_dist.Value = 10;
            this.trackBar_dist.Scroll += new System.EventHandler(this.trackBar_dist_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_dinamic);
            this.groupBox1.Controls.Add(this.button_ms);
            this.groupBox1.Controls.Add(this.radioButton_ms);
            this.groupBox1.Controls.Add(this.radioButton_kinetic);
            this.groupBox1.Location = new System.Drawing.Point(8, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(150, 90);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data type";
            // 
            // radioButton_dinamic
            // 
            this.radioButton_dinamic.AutoSize = true;
            this.radioButton_dinamic.Enabled = false;
            this.radioButton_dinamic.Location = new System.Drawing.Point(6, 65);
            this.radioButton_dinamic.Name = "radioButton_dinamic";
            this.radioButton_dinamic.Size = new System.Drawing.Size(118, 17);
            this.radioButton_dinamic.TabIndex = 2;
            this.radioButton_dinamic.Text = "Kinetick model - f(n)";
            this.radioButton_dinamic.UseVisualStyleBackColor = true;
            this.radioButton_dinamic.CheckedChanged += new System.EventHandler(this.radioButton_dinamic_CheckedChanged);
            // 
            // button_ms
            // 
            this.button_ms.Enabled = false;
            this.button_ms.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button_ms.Location = new System.Drawing.Point(104, 42);
            this.button_ms.Name = "button_ms";
            this.button_ms.Size = new System.Drawing.Size(40, 18);
            this.button_ms.TabIndex = 20;
            this.button_ms.Text = "C-ON";
            this.button_ms.UseVisualStyleBackColor = true;
            this.button_ms.Click += new System.EventHandler(this.button_ms_Click);
            // 
            // radioButton_ms
            // 
            this.radioButton_ms.AutoSize = true;
            this.radioButton_ms.Location = new System.Drawing.Point(6, 42);
            this.radioButton_ms.Name = "radioButton_ms";
            this.radioButton_ms.Size = new System.Drawing.Size(96, 17);
            this.radioButton_ms.TabIndex = 1;
            this.radioButton_ms.Text = "Mass spectrum";
            this.radioButton_ms.UseVisualStyleBackColor = true;
            this.radioButton_ms.CheckedChanged += new System.EventHandler(this.radioButton_ms_CheckedChanged);
            // 
            // radioButton_kinetic
            // 
            this.radioButton_kinetic.AutoSize = true;
            this.radioButton_kinetic.Checked = true;
            this.radioButton_kinetic.Location = new System.Drawing.Point(6, 19);
            this.radioButton_kinetic.Name = "radioButton_kinetic";
            this.radioButton_kinetic.Size = new System.Drawing.Size(134, 17);
            this.radioButton_kinetic.TabIndex = 0;
            this.radioButton_kinetic.TabStop = true;
            this.radioButton_kinetic.Text = "Kinetick model - f(x)/f(t)";
            this.radioButton_kinetic.UseVisualStyleBackColor = true;
            this.radioButton_kinetic.CheckedChanged += new System.EventHandler(this.radioButton_kinetic_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.button_plot);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.button_export);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.textBox_conc);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textBox_dist);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.trackBar_conc);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.trackBar_dist);
            this.panel1.Controls.Add(this.checkedListBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(168, 553);
            this.panel1.TabIndex = 19;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_log);
            this.groupBox2.Controls.Add(this.button_t_x);
            this.groupBox2.Location = new System.Drawing.Point(8, 445);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(154, 68);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            // 
            // checkBox_log
            // 
            this.checkBox_log.AutoSize = true;
            this.checkBox_log.Checked = true;
            this.checkBox_log.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_log.Location = new System.Drawing.Point(6, 16);
            this.checkBox_log.Name = "checkBox_log";
            this.checkBox_log.Size = new System.Drawing.Size(75, 17);
            this.checkBox_log.TabIndex = 21;
            this.checkBox_log.Text = "Log. scale";
            this.checkBox_log.UseVisualStyleBackColor = true;
            this.checkBox_log.CheckedChanged += new System.EventHandler(this.checkBox_log_CheckedChanged);
            // 
            // button_t_x
            // 
            this.button_t_x.Enabled = false;
            this.button_t_x.Location = new System.Drawing.Point(93, 13);
            this.button_t_x.Name = "button_t_x";
            this.button_t_x.Size = new System.Drawing.Size(55, 20);
            this.button_t_x.TabIndex = 21;
            this.button_t_x.Text = "t --> x";
            this.button_t_x.UseVisualStyleBackColor = true;
            this.button_t_x.Click += new System.EventHandler(this.button_t_x_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 353);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(157, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "--------------------------------------------------";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 262);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(157, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "--------------------------------------------------";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(139, 429);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "max";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(139, 337);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "max";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 429);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "min";
            // 
            // textBox_conc
            // 
            this.textBox_conc.Enabled = false;
            this.textBox_conc.Location = new System.Drawing.Point(87, 371);
            this.textBox_conc.Name = "textBox_conc";
            this.textBox_conc.Size = new System.Drawing.Size(71, 20);
            this.textBox_conc.TabIndex = 24;
            this.textBox_conc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_conc_KeyDown);
            this.textBox_conc.Leave += new System.EventHandler(this.textBox_conc_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 337);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "min";
            // 
            // textBox_dist
            // 
            this.textBox_dist.Enabled = false;
            this.textBox_dist.Location = new System.Drawing.Point(87, 279);
            this.textBox_dist.Name = "textBox_dist";
            this.textBox_dist.Size = new System.Drawing.Size(71, 20);
            this.textBox_dist.TabIndex = 21;
            this.textBox_dist.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_conc_KeyDown);
            this.textBox_dist.Leave += new System.EventHandler(this.textBox_dist_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 374);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Concentration:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 282);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Time:";
            // 
            // trackBar_conc
            // 
            this.trackBar_conc.Enabled = false;
            this.trackBar_conc.LargeChange = 1;
            this.trackBar_conc.Location = new System.Drawing.Point(8, 397);
            this.trackBar_conc.Name = "trackBar_conc";
            this.trackBar_conc.Size = new System.Drawing.Size(150, 45);
            this.trackBar_conc.TabIndex = 22;
            this.trackBar_conc.Value = 10;
            this.trackBar_conc.Scroll += new System.EventHandler(this.trackBar_conc_Scroll);
            this.trackBar_conc.Leave += new System.EventHandler(this.trackBar_conc_Leave);
            // 
            // Results_KineticModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 553);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.plot1);
            this.Name = "Results_KineticModel";
            this.Text = "Results_KineticModel";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_dist)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_conc)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private OxyPlot.WindowsForms.PlotView plot1;
        private System.Windows.Forms.Button button_export;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button button_plot;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TrackBar trackBar_dist;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_dinamic;
        private System.Windows.Forms.RadioButton radioButton_ms;
        private System.Windows.Forms.RadioButton radioButton_kinetic;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_conc;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox textBox_dist;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBar_conc;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_t_x;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBox_log;
        private System.Windows.Forms.Button button_ms;
    }
}