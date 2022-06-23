namespace ACST.AWS.TextractViewer
{
    partial class ConfirmationDialog
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbExport = new System.Windows.Forms.RadioButton();
            this.rbSave = new System.Windows.Forms.RadioButton();
            this.rbExportOpenNext = new System.Windows.Forms.RadioButton();
            this.cbShowConfirmation = new System.Windows.Forms.CheckBox();
            this.chkRememberChoice = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(219, 195);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(54, 31);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Tag = "Cancel";
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(282, 195);
            this.btnOK.Margin = new System.Windows.Forms.Padding(1);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(54, 31);
            this.btnOK.TabIndex = 1;
            this.btnOK.Tag = "OK";
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Textract Claim, Save and Export";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbExportOpenNext);
            this.groupBox1.Controls.Add(this.rbExport);
            this.groupBox1.Controls.Add(this.rbSave);
            this.groupBox1.Location = new System.Drawing.Point(20, 47);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(316, 133);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // rbExport
            // 
            this.rbExport.AutoSize = true;
            this.rbExport.Location = new System.Drawing.Point(20, 59);
            this.rbExport.Margin = new System.Windows.Forms.Padding(1);
            this.rbExport.Name = "rbExport";
            this.rbExport.Size = new System.Drawing.Size(275, 17);
            this.rbExport.TabIndex = 13;
            this.rbExport.Tag = "Export";
            this.rbExport.Text = "Save && Export Claim to BizTalk for CAMS processing.";
            this.rbExport.UseVisualStyleBackColor = true;
            // 
            // rbSave
            // 
            this.rbSave.AutoSize = true;
            this.rbSave.Location = new System.Drawing.Point(20, 26);
            this.rbSave.Margin = new System.Windows.Forms.Padding(1);
            this.rbSave.Name = "rbSave";
            this.rbSave.Size = new System.Drawing.Size(276, 17);
            this.rbSave.TabIndex = 12;
            this.rbSave.Tag = "Save";
            this.rbSave.Text = "Save Textract Claim results to archive for future edits.";
            this.rbSave.UseVisualStyleBackColor = true;
            // 
            // rbExportOpenNext
            // 
            this.rbExportOpenNext.AutoSize = true;
            this.rbExportOpenNext.Location = new System.Drawing.Point(20, 90);
            this.rbExportOpenNext.Margin = new System.Windows.Forms.Padding(1);
            this.rbExportOpenNext.Name = "rbExportOpenNext";
            this.rbExportOpenNext.Size = new System.Drawing.Size(152, 17);
            this.rbExportOpenNext.TabIndex = 14;
            this.rbExportOpenNext.Tag = "ExportAndOpen";
            this.rbExportOpenNext.Text = "Save, Export && Open Next ";
            this.rbExportOpenNext.UseVisualStyleBackColor = true;
            // 
            // cbShowConfirmation
            // 
            this.cbShowConfirmation.AutoSize = true;
            this.cbShowConfirmation.Checked = global::ACST.AWS.TextractViewer.Properties.Settings.Default.ShowSaveConfirmation;
            this.cbShowConfirmation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowConfirmation.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ACST.AWS.TextractViewer.Properties.Settings.Default, "ShowSaveConfirmation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbShowConfirmation.Enabled = false;
            this.cbShowConfirmation.Location = new System.Drawing.Point(45, 203);
            this.cbShowConfirmation.Margin = new System.Windows.Forms.Padding(1);
            this.cbShowConfirmation.Name = "cbShowConfirmation";
            this.cbShowConfirmation.Size = new System.Drawing.Size(163, 17);
            this.cbShowConfirmation.TabIndex = 11;
            this.cbShowConfirmation.Tag = "ShowConfirmation";
            this.cbShowConfirmation.Text = "Show this confirmation dialog";
            this.cbShowConfirmation.UseVisualStyleBackColor = true;
            // 
            // chkRememberChoice
            // 
            this.chkRememberChoice.AutoSize = true;
            this.chkRememberChoice.Checked = global::ACST.AWS.TextractViewer.Properties.Settings.Default.ConfirmationSaveChoice;
            this.chkRememberChoice.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ACST.AWS.TextractViewer.Properties.Settings.Default, "ConfirmationSaveChoice", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkRememberChoice.Location = new System.Drawing.Point(45, 184);
            this.chkRememberChoice.Margin = new System.Windows.Forms.Padding(1);
            this.chkRememberChoice.Name = "chkRememberChoice";
            this.chkRememberChoice.Size = new System.Drawing.Size(131, 17);
            this.chkRememberChoice.TabIndex = 9;
            this.chkRememberChoice.Tag = "RememberChoice";
            this.chkRememberChoice.Text = "Remember my choice.";
            this.chkRememberChoice.UseVisualStyleBackColor = true;
            this.chkRememberChoice.Visible = false;
            // 
            // ConfirmationDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(359, 249);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbShowConfirmation);
            this.Controls.Add(this.chkRememberChoice);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfirmationDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Confirmation";
            this.Load += new System.EventHandler(this.ConfirmationDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkRememberChoice;
        private System.Windows.Forms.CheckBox cbShowConfirmation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbExportOpenNext;
        private System.Windows.Forms.RadioButton rbExport;
        private System.Windows.Forms.RadioButton rbSave;
    }
}