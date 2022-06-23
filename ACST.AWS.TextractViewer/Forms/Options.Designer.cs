namespace ACST.AWS.TextractViewer
{
    partial class Options
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
            this.tcOptions = new System.Windows.Forms.TabControl();
            this.tabPage_KbdShortcuts = new System.Windows.Forms.TabPage();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabPage_Legend = new System.Windows.Forms.TabPage();
            this.txtLowConfidence = new System.Windows.Forms.TextBox();
            this.txtBorderlineConfidence = new System.Windows.Forms.TextBox();
            this.txtHighConfidence = new System.Windows.Forms.TextBox();
            this.txtUpdatedClaimLegend = new System.Windows.Forms.TextBox();
            this.txtMappedToClaimLegend = new System.Windows.Forms.TextBox();
            this.txtNotDefinedInSpecLegend = new System.Windows.Forms.TextBox();
            this.txtDefinedInSpecLegend = new System.Windows.Forms.TextBox();
            this.txtOcrLegend = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage_Environment = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbMaximizeOnOpen = new System.Windows.Forms.CheckBox();
            this.cbShowConfirmation = new System.Windows.Forms.CheckBox();
            this.chkHighlightCurrentField = new System.Windows.Forms.CheckBox();
            this.cbSaveSettingOnExit = new System.Windows.Forms.CheckBox();
            this.tcOptions.SuspendLayout();
            this.tabPage_KbdShortcuts.SuspendLayout();
            this.tabPage_Legend.SuspendLayout();
            this.tabPage_Environment.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcOptions
            // 
            this.tcOptions.Controls.Add(this.tabPage_KbdShortcuts);
            this.tcOptions.Controls.Add(this.tabPage_Legend);
            this.tcOptions.Controls.Add(this.tabPage_Environment);
            this.tcOptions.Location = new System.Drawing.Point(10, 10);
            this.tcOptions.Margin = new System.Windows.Forms.Padding(1);
            this.tcOptions.Name = "tcOptions";
            this.tcOptions.SelectedIndex = 0;
            this.tcOptions.Size = new System.Drawing.Size(318, 382);
            this.tcOptions.TabIndex = 0;
            // 
            // tabPage_KbdShortcuts
            // 
            this.tabPage_KbdShortcuts.Controls.Add(this.textBox2);
            this.tabPage_KbdShortcuts.Controls.Add(this.textBox1);
            this.tabPage_KbdShortcuts.Location = new System.Drawing.Point(4, 22);
            this.tabPage_KbdShortcuts.Name = "tabPage_KbdShortcuts";
            this.tabPage_KbdShortcuts.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_KbdShortcuts.Size = new System.Drawing.Size(310, 356);
            this.tabPage_KbdShortcuts.TabIndex = 2;
            this.tabPage_KbdShortcuts.Text = "Keyboard Shortcuts";
            this.tabPage_KbdShortcuts.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(119, 18);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(185, 287);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = "Next OCR Field\r\nPrevious OCR Field\r\n\r\nToggle Field Highlights\r\nToggle Field Conte" +
    "xt\r\nOpen Next Document\r\nSave / Export Textract\r\n\r\nNext Open Document\r\nPrevious O" +
    "pen Document\r\n\r\nZoom In\r\nZoom Out\r\n";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(16, 18);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(89, 287);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "Tab\r\nShift + Tab\r\n\r\nCtrl + H\r\nCtrl + R\r\nCtrl + N\r\nCtrl + S\r\n\r\nCtrl + Tab\r\nShift+C" +
    "trl+Tab\r\n\r\nShift+\r\nShift -\r\n\r\n";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tabPage_Legend
            // 
            this.tabPage_Legend.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_Legend.Controls.Add(this.txtLowConfidence);
            this.tabPage_Legend.Controls.Add(this.txtBorderlineConfidence);
            this.tabPage_Legend.Controls.Add(this.txtHighConfidence);
            this.tabPage_Legend.Controls.Add(this.txtUpdatedClaimLegend);
            this.tabPage_Legend.Controls.Add(this.txtMappedToClaimLegend);
            this.tabPage_Legend.Controls.Add(this.txtNotDefinedInSpecLegend);
            this.tabPage_Legend.Controls.Add(this.txtDefinedInSpecLegend);
            this.tabPage_Legend.Controls.Add(this.txtOcrLegend);
            this.tabPage_Legend.Controls.Add(this.label3);
            this.tabPage_Legend.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Legend.Margin = new System.Windows.Forms.Padding(1);
            this.tabPage_Legend.Name = "tabPage_Legend";
            this.tabPage_Legend.Padding = new System.Windows.Forms.Padding(1);
            this.tabPage_Legend.Size = new System.Drawing.Size(310, 356);
            this.tabPage_Legend.TabIndex = 1;
            this.tabPage_Legend.Text = "Legend";
            // 
            // txtLowConfidence
            // 
            this.txtLowConfidence.BackColor = System.Drawing.Color.White;
            this.txtLowConfidence.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLowConfidence.Location = new System.Drawing.Point(22, 82);
            this.txtLowConfidence.Margin = new System.Windows.Forms.Padding(1);
            this.txtLowConfidence.Multiline = true;
            this.txtLowConfidence.Name = "txtLowConfidence";
            this.txtLowConfidence.ReadOnly = true;
            this.txtLowConfidence.Size = new System.Drawing.Size(266, 31);
            this.txtLowConfidence.TabIndex = 15;
            this.txtLowConfidence.Text = "Required Field with Low confidence - Outlined in Red";
            this.txtLowConfidence.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBorderlineConfidence
            // 
            this.txtBorderlineConfidence.BackColor = System.Drawing.Color.White;
            this.txtBorderlineConfidence.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBorderlineConfidence.Location = new System.Drawing.Point(22, 50);
            this.txtBorderlineConfidence.Margin = new System.Windows.Forms.Padding(1);
            this.txtBorderlineConfidence.Multiline = true;
            this.txtBorderlineConfidence.Name = "txtBorderlineConfidence";
            this.txtBorderlineConfidence.ReadOnly = true;
            this.txtBorderlineConfidence.Size = new System.Drawing.Size(266, 31);
            this.txtBorderlineConfidence.TabIndex = 14;
            this.txtBorderlineConfidence.Text = "Required Field with Borderline confidence - Outlined in Yellow";
            this.txtBorderlineConfidence.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtHighConfidence
            // 
            this.txtHighConfidence.BackColor = System.Drawing.Color.White;
            this.txtHighConfidence.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHighConfidence.Location = new System.Drawing.Point(22, 18);
            this.txtHighConfidence.Margin = new System.Windows.Forms.Padding(1);
            this.txtHighConfidence.Multiline = true;
            this.txtHighConfidence.Name = "txtHighConfidence";
            this.txtHighConfidence.ReadOnly = true;
            this.txtHighConfidence.Size = new System.Drawing.Size(266, 31);
            this.txtHighConfidence.TabIndex = 13;
            this.txtHighConfidence.Text = "Required Field with High confidence - Outlined in Green";
            this.txtHighConfidence.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtUpdatedClaimLegend
            // 
            this.txtUpdatedClaimLegend.BackColor = System.Drawing.Color.LightGreen;
            this.txtUpdatedClaimLegend.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUpdatedClaimLegend.ForeColor = System.Drawing.Color.Black;
            this.txtUpdatedClaimLegend.Location = new System.Drawing.Point(22, 298);
            this.txtUpdatedClaimLegend.Margin = new System.Windows.Forms.Padding(1);
            this.txtUpdatedClaimLegend.Multiline = true;
            this.txtUpdatedClaimLegend.Name = "txtUpdatedClaimLegend";
            this.txtUpdatedClaimLegend.ReadOnly = true;
            this.txtUpdatedClaimLegend.Size = new System.Drawing.Size(266, 31);
            this.txtUpdatedClaimLegend.TabIndex = 12;
            this.txtUpdatedClaimLegend.Text = "Mapped claim value updated or approved by user";
            this.txtUpdatedClaimLegend.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtMappedToClaimLegend
            // 
            this.txtMappedToClaimLegend.BackColor = System.Drawing.SystemColors.Control;
            this.txtMappedToClaimLegend.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMappedToClaimLegend.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtMappedToClaimLegend.Location = new System.Drawing.Point(22, 262);
            this.txtMappedToClaimLegend.Margin = new System.Windows.Forms.Padding(1);
            this.txtMappedToClaimLegend.Multiline = true;
            this.txtMappedToClaimLegend.Name = "txtMappedToClaimLegend";
            this.txtMappedToClaimLegend.ReadOnly = true;
            this.txtMappedToClaimLegend.Size = new System.Drawing.Size(266, 31);
            this.txtMappedToClaimLegend.TabIndex = 11;
            this.txtMappedToClaimLegend.Text = "Mapped to Claim, defined in ADA, & OCR Recognition";
            this.txtMappedToClaimLegend.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtNotDefinedInSpecLegend
            // 
            this.txtNotDefinedInSpecLegend.BackColor = System.Drawing.SystemColors.Control;
            this.txtNotDefinedInSpecLegend.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNotDefinedInSpecLegend.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtNotDefinedInSpecLegend.Location = new System.Drawing.Point(22, 226);
            this.txtNotDefinedInSpecLegend.Margin = new System.Windows.Forms.Padding(1);
            this.txtNotDefinedInSpecLegend.Multiline = true;
            this.txtNotDefinedInSpecLegend.Name = "txtNotDefinedInSpecLegend";
            this.txtNotDefinedInSpecLegend.ReadOnly = true;
            this.txtNotDefinedInSpecLegend.Size = new System.Drawing.Size(266, 31);
            this.txtNotDefinedInSpecLegend.TabIndex = 10;
            this.txtNotDefinedInSpecLegend.Text = "Not found in Specification, OCR Recognition";
            this.txtNotDefinedInSpecLegend.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtDefinedInSpecLegend
            // 
            this.txtDefinedInSpecLegend.BackColor = System.Drawing.SystemColors.Control;
            this.txtDefinedInSpecLegend.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDefinedInSpecLegend.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtDefinedInSpecLegend.Location = new System.Drawing.Point(22, 190);
            this.txtDefinedInSpecLegend.Margin = new System.Windows.Forms.Padding(1);
            this.txtDefinedInSpecLegend.Multiline = true;
            this.txtDefinedInSpecLegend.Name = "txtDefinedInSpecLegend";
            this.txtDefinedInSpecLegend.ReadOnly = true;
            this.txtDefinedInSpecLegend.Size = new System.Drawing.Size(266, 31);
            this.txtDefinedInSpecLegend.TabIndex = 9;
            this.txtDefinedInSpecLegend.Text = "Defined in ADA Specification, OCR Recognition";
            this.txtDefinedInSpecLegend.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtOcrLegend
            // 
            this.txtOcrLegend.BackColor = System.Drawing.SystemColors.Control;
            this.txtOcrLegend.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtOcrLegend.Location = new System.Drawing.Point(22, 153);
            this.txtOcrLegend.Margin = new System.Windows.Forms.Padding(1);
            this.txtOcrLegend.Multiline = true;
            this.txtOcrLegend.Name = "txtOcrLegend";
            this.txtOcrLegend.ReadOnly = true;
            this.txtOcrLegend.Size = new System.Drawing.Size(266, 31);
            this.txtOcrLegend.TabIndex = 8;
            this.txtOcrLegend.Text = "Optical Character Recognition (OCR) by Amazon Textract";
            this.txtOcrLegend.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 168);
            this.label3.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 4;
            // 
            // tabPage_Environment
            // 
            this.tabPage_Environment.Controls.Add(this.cbMaximizeOnOpen);
            this.tabPage_Environment.Controls.Add(this.cbShowConfirmation);
            this.tabPage_Environment.Controls.Add(this.chkHighlightCurrentField);
            this.tabPage_Environment.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Environment.Margin = new System.Windows.Forms.Padding(1);
            this.tabPage_Environment.Name = "tabPage_Environment";
            this.tabPage_Environment.Size = new System.Drawing.Size(310, 356);
            this.tabPage_Environment.TabIndex = 0;
            this.tabPage_Environment.Text = "Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(244, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Visual Experience";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(255, 412);
            this.btnOk.Margin = new System.Windows.Forms.Padding(1);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(72, 31);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(170, 412);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 31);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbMaximizeOnOpen
            // 
            this.cbMaximizeOnOpen.AutoSize = true;
            this.cbMaximizeOnOpen.Location = new System.Drawing.Point(29, 124);
            this.cbMaximizeOnOpen.Name = "cbMaximizeOnOpen";
            this.cbMaximizeOnOpen.Size = new System.Drawing.Size(113, 17);
            this.cbMaximizeOnOpen.TabIndex = 15;
            this.cbMaximizeOnOpen.Text = "Maximize on Open";
            this.cbMaximizeOnOpen.UseVisualStyleBackColor = true;
            // 
            // cbShowConfirmation
            // 
            this.cbShowConfirmation.AutoSize = true;
            this.cbShowConfirmation.Checked = global::ACST.AWS.TextractViewer.Properties.Settings.Default.ShowSaveConfirmation;
            this.cbShowConfirmation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowConfirmation.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ACST.AWS.TextractViewer.Properties.Settings.Default, "ShowSaveConfirmation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbShowConfirmation.Location = new System.Drawing.Point(29, 84);
            this.cbShowConfirmation.Name = "cbShowConfirmation";
            this.cbShowConfirmation.Size = new System.Drawing.Size(175, 17);
            this.cbShowConfirmation.TabIndex = 14;
            this.cbShowConfirmation.Text = "Show Save Confirmation Dialog";
            this.cbShowConfirmation.UseVisualStyleBackColor = true;
            // 
            // chkHighlightCurrentField
            // 
            this.chkHighlightCurrentField.AutoSize = true;
            this.chkHighlightCurrentField.Checked = true;
            this.chkHighlightCurrentField.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHighlightCurrentField.Location = new System.Drawing.Point(29, 47);
            this.chkHighlightCurrentField.Name = "chkHighlightCurrentField";
            this.chkHighlightCurrentField.Size = new System.Drawing.Size(218, 17);
            this.chkHighlightCurrentField.TabIndex = 0;
            this.chkHighlightCurrentField.Text = "Highight and Center current Field on Tab";
            this.chkHighlightCurrentField.UseVisualStyleBackColor = true;
            // 
            // cbSaveSettingOnExit
            // 
            this.cbSaveSettingOnExit.AutoSize = true;
            this.cbSaveSettingOnExit.Checked = global::ACST.AWS.TextractViewer.Properties.Settings.Default.SaveSettingsOnExit;
            this.cbSaveSettingOnExit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSaveSettingOnExit.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ACST.AWS.TextractViewer.Properties.Settings.Default, "SaveSettingsOnExit", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbSaveSettingOnExit.Location = new System.Drawing.Point(71, 137);
            this.cbSaveSettingOnExit.Name = "cbSaveSettingOnExit";
            this.cbSaveSettingOnExit.Size = new System.Drawing.Size(309, 36);
            this.cbSaveSettingOnExit.TabIndex = 1;
            this.cbSaveSettingOnExit.Text = "Save Setting on Exit";
            this.cbSaveSettingOnExit.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(346, 451);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tcOptions);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.tcOptions.ResumeLayout(false);
            this.tabPage_KbdShortcuts.ResumeLayout(false);
            this.tabPage_KbdShortcuts.PerformLayout();
            this.tabPage_Legend.ResumeLayout(false);
            this.tabPage_Legend.PerformLayout();
            this.tabPage_Environment.ResumeLayout(false);
            this.tabPage_Environment.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcOptions;
        private System.Windows.Forms.TabPage tabPage_Environment;
        private System.Windows.Forms.CheckBox cbSaveSettingOnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage_Legend;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUpdatedClaimLegend;
        private System.Windows.Forms.TextBox txtMappedToClaimLegend;
        private System.Windows.Forms.TextBox txtNotDefinedInSpecLegend;
        private System.Windows.Forms.TextBox txtDefinedInSpecLegend;
        private System.Windows.Forms.TextBox txtOcrLegend;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtLowConfidence;
        private System.Windows.Forms.TextBox txtBorderlineConfidence;
        private System.Windows.Forms.TextBox txtHighConfidence;
        private System.Windows.Forms.CheckBox chkHighlightCurrentField;
        private System.Windows.Forms.TabPage tabPage_KbdShortcuts;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox cbShowConfirmation;
        private System.Windows.Forms.CheckBox cbMaximizeOnOpen;
    }
}