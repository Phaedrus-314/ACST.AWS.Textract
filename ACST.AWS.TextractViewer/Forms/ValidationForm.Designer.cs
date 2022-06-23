namespace ACST.AWS.TextractViewer
{
    partial class ValidationForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.dgvValidation = new System.Windows.Forms.DataGridView();
            this.txtBoxValidationInfo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvValidation)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(360, 404);
            this.btnOK.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(54, 31);
            this.btnOK.TabIndex = 3;
            this.btnOK.Tag = "OK";
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(293, 404);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(54, 31);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Tag = "Cancel";
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 74);
            this.textBox1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(404, 79);
            this.textBox1.TabIndex = 4;
            // 
            // dgvValidation
            // 
            this.dgvValidation.AllowUserToAddRows = false;
            this.dgvValidation.AllowUserToDeleteRows = false;
            this.dgvValidation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvValidation.BackgroundColor = System.Drawing.Color.GhostWhite;
            this.dgvValidation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvValidation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvValidation.Location = new System.Drawing.Point(12, 174);
            this.dgvValidation.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.dgvValidation.MultiSelect = false;
            this.dgvValidation.Name = "dgvValidation";
            this.dgvValidation.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvValidation.ShowCellToolTips = false;
            this.dgvValidation.Size = new System.Drawing.Size(402, 213);
            this.dgvValidation.TabIndex = 19;
            this.dgvValidation.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvValidation_CellEndEdit);
            // 
            // txtBoxValidationInfo
            // 
            this.txtBoxValidationInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxValidationInfo.Location = new System.Drawing.Point(12, 16);
            this.txtBoxValidationInfo.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.txtBoxValidationInfo.Multiline = true;
            this.txtBoxValidationInfo.Name = "txtBoxValidationInfo";
            this.txtBoxValidationInfo.ReadOnly = true;
            this.txtBoxValidationInfo.Size = new System.Drawing.Size(404, 47);
            this.txtBoxValidationInfo.TabIndex = 20;
            this.txtBoxValidationInfo.Text = "Textract Imageg was saved but not exported due to the following missing requireme" +
    "nts.  Correct the data below, then try Export again.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(12, 413);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(284, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "This dialog info will move to Validation tab in context panel.";
            // 
            // ValidationForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(426, 445);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBoxValidationInfo);
            this.Controls.Add(this.dgvValidation);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.Name = "ValidationForm";
            this.Text = "ValidationForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvValidation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView dgvValidation;
        private System.Windows.Forms.TextBox txtBoxValidationInfo;
        private System.Windows.Forms.Label label1;
    }
}