namespace ACST.AWS.TextractViewer
{
    partial class ContextViewer
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvJsonExplorer = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.rtbSearchResults = new System.Windows.Forms.RichTextBox();
            this.btnSearchByID = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 402);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.statusStrip1.Size = new System.Drawing.Size(815, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(54, 17);
            this.tsslStatus.Text = "tsslStatus";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(815, 402);
            this.panel1.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvJsonExplorer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(815, 402);
            this.splitContainer1.SplitterDistance = 394;
            this.splitContainer1.TabIndex = 0;
            // 
            // tvJsonExplorer
            // 
            this.tvJsonExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvJsonExplorer.Location = new System.Drawing.Point(0, 0);
            this.tvJsonExplorer.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.tvJsonExplorer.Name = "tvJsonExplorer";
            this.tvJsonExplorer.Size = new System.Drawing.Size(394, 402);
            this.tvJsonExplorer.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.richTextBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel2);
            this.splitContainer2.Size = new System.Drawing.Size(417, 402);
            this.splitContainer2.SplitterDistance = 268;
            this.splitContainer2.TabIndex = 0;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(417, 268);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnExit);
            this.panel2.Controls.Add(this.rtbSearchResults);
            this.panel2.Controls.Add(this.btnSearchByID);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(417, 130);
            this.panel2.TabIndex = 0;
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(189, 14);
            this.btnExit.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(52, 17);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Visible = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // rtbSearchResults
            // 
            this.rtbSearchResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbSearchResults.Location = new System.Drawing.Point(17, 72);
            this.rtbSearchResults.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.rtbSearchResults.Name = "rtbSearchResults";
            this.rtbSearchResults.Size = new System.Drawing.Size(380, 40);
            this.rtbSearchResults.TabIndex = 3;
            this.rtbSearchResults.Text = "";
            // 
            // btnSearchByID
            // 
            this.btnSearchByID.Location = new System.Drawing.Point(291, 31);
            this.btnSearchByID.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.btnSearchByID.Name = "btnSearchByID";
            this.btnSearchByID.Size = new System.Drawing.Size(56, 23);
            this.btnSearchByID.TabIndex = 2;
            this.btnSearchByID.Text = "Search";
            this.btnSearchByID.UseVisualStyleBackColor = true;
            this.btnSearchByID.Click += new System.EventHandler(this.btnSearchByID_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ID:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(30, 35);
            this.textBox1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(244, 20);
            this.textBox1.TabIndex = 0;
            // 
            // ContextViewer
            // 
            this.AcceptButton = this.btnSearchByID;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(815, 424);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.Name = "ContextViewer";
            this.Text = "ContextViewer";
            this.Load += new System.EventHandler(this.ContextViewer_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvJsonExplorer;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox rtbSearchResults;
        private System.Windows.Forms.Button btnSearchByID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnExit;
    }
}