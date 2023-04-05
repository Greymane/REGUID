
namespace ReGUID
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelFixedCount = new System.Windows.Forms.Label();
            this.labelTotalCreatures = new System.Windows.Forms.Label();
            this.labelHighestGuid = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.buttonStart = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBoxOutput = new System.Windows.Forms.ListBox();
            this.checkBoxHaltOnError = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Total Creatures :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(223, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Highest GUID :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(449, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fixed :";
            // 
            // labelFixedCount
            // 
            this.labelFixedCount.AutoSize = true;
            this.labelFixedCount.Location = new System.Drawing.Point(506, 13);
            this.labelFixedCount.Name = "labelFixedCount";
            this.labelFixedCount.Size = new System.Drawing.Size(17, 20);
            this.labelFixedCount.TabIndex = 3;
            this.labelFixedCount.Text = "0";
            // 
            // labelTotalCreatures
            // 
            this.labelTotalCreatures.AutoSize = true;
            this.labelTotalCreatures.Location = new System.Drawing.Point(134, 13);
            this.labelTotalCreatures.Name = "labelTotalCreatures";
            this.labelTotalCreatures.Size = new System.Drawing.Size(17, 20);
            this.labelTotalCreatures.TabIndex = 4;
            this.labelTotalCreatures.Text = "0";
            // 
            // labelHighestGuid
            // 
            this.labelHighestGuid.AutoSize = true;
            this.labelHighestGuid.Location = new System.Drawing.Point(335, 13);
            this.labelHighestGuid.Name = "labelHighestGuid";
            this.labelHighestGuid.Size = new System.Drawing.Size(17, 20);
            this.labelHighestGuid.TabIndex = 5;
            this.labelHighestGuid.Text = "0";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 44);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(546, 29);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 6;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(565, 44);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(116, 29);
            this.buttonStart.TabIndex = 7;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxOutput);
            this.groupBox1.Location = new System.Drawing.Point(13, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(546, 201);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // listBoxOutput
            // 
            this.listBoxOutput.FormattingEnabled = true;
            this.listBoxOutput.ItemHeight = 20;
            this.listBoxOutput.Location = new System.Drawing.Point(7, 27);
            this.listBoxOutput.Name = "listBoxOutput";
            this.listBoxOutput.Size = new System.Drawing.Size(533, 164);
            this.listBoxOutput.TabIndex = 0;
            // 
            // checkBoxHaltOnError
            // 
            this.checkBoxHaltOnError.AutoSize = true;
            this.checkBoxHaltOnError.Checked = true;
            this.checkBoxHaltOnError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHaltOnError.Location = new System.Drawing.Point(565, 97);
            this.checkBoxHaltOnError.Name = "checkBoxHaltOnError";
            this.checkBoxHaltOnError.Size = new System.Drawing.Size(116, 24);
            this.checkBoxHaltOnError.TabIndex = 9;
            this.checkBoxHaltOnError.Text = "Halt on Error";
            this.checkBoxHaltOnError.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(565, 252);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(116, 29);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.Enabled = false;
            this.buttonExport.Location = new System.Drawing.Point(566, 211);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(115, 35);
            this.buttonExport.TabIndex = 11;
            this.buttonExport.Text = "Export output";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 293);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.checkBoxHaltOnError);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.labelHighestGuid);
            this.Controls.Add(this.labelTotalCreatures);
            this.Controls.Add(this.labelFixedCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "ReGUID by Greymane/Elioth";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelFixedCount;
        private System.Windows.Forms.Label labelTotalCreatures;
        private System.Windows.Forms.Label labelHighestGuid;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonStart;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBoxOutput;
        private System.Windows.Forms.CheckBox checkBoxHaltOnError;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonExport;
    }
}