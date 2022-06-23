
namespace ACST.AWS.TextractViewer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;

    using ACST.AWS.Common;
    using ACST.AWS.TextractClaimMapper.OCR;

    internal partial class MDITextractViewer
        : BaseForm
    {
        bool autoOpen = false;

        const string testNewForm = @"C:\BTS Folders\Dental\Output\Archive\Dental Claims 060820 1.zip";

        const bool RebuildClaim = false;

        public MDITextractViewer()
        {
            InitializeComponent();

            UpdateStatusBar();
            //ToolStripPanel tspTop = new ToolStripPanel();
            //tspTop.Dock = DockStyle.Top;

            //ToolStrip tsTop = new ToolStrip();
            //tsTop.Items.Add("Top");
            //tspTop.Join(tsTop);
        }

        #region Form Overrides

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ReBuildToolStripButton.Checked = RebuildClaim;

            ////this.WindowState = Properties.Settings.Default.WindowsState;
            ////this.Location = Properties.Settings.Default.WindowsLocation;
            ////this.Size = Properties.Settings.Default.MinimumSize;

            //// Handle upraded .Net version
            //if (Properties.Settings.Default.MinimumSize.Width == 0) Properties.Settings.Default.Upgrade();

            //if (Properties.Settings.Default.MinimumSize.Width == 0 || Properties.Settings.Default.MinimumSize.Height == 0)
            //{
            //    this.Size = new System.Drawing.Size(2000, 1400);
            //}
            //else
            //{
            //    this.WindowState = Properties.Settings.Default.WindowsState;

            //    if (this.WindowState == FormWindowState.Minimized)
            //        this.WindowState = FormWindowState.Normal;

            //    this.Location = Properties.Settings.Default.WindowsLocation;
            //    this.Size = Properties.Settings.Default.MinimumSize;
            //}

            if (autoOpen)
            {
                string compressedOCRFileName = testNewForm;
                ViewTractClaimImage(compressedOCRFileName);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //Properties.Settings.Default.WindowsState = this.WindowState;
            //if (this.WindowState == FormWindowState.Normal)
            //{
            //    Properties.Settings.Default.WindowsLocation = this.Location;
            //    Properties.Settings.Default.MinimumSize = this.Size;
            //}
            //else
            //{
            //    Properties.Settings.Default.WindowsLocation = this.RestoreBounds.Location;
            //    Properties.Settings.Default.MinimumSize = this.RestoreBounds.Size;
            //}

            //if (Properties.Settings.Default.SaveSettingsOnExit)
            //    Properties.Settings.Default.Save();

            base.OnClosing(e);
        }
        #endregion

        TextractClaimImage LoadTextractClaimImage(string compressedOCRFileName,  bool remapClaim = false)
        {
            var textractClaimImage = new TextractClaimImage();

            try
            {
                if (ReBuildToolStripButton.Checked)
                    textractClaimImage.BuildADA(compressedOCRFileName);
                else
                    textractClaimImage.OpenADA(compressedOCRFileName);
            }
            catch (Exception ex)
            {
                Logger.TraceError(ex);
                MessageBox.Show(string.Format("{0}", ex.Message));
            }

            return textractClaimImage;
        }

        void ViewTractClaimImage(string compressedOCRFileName)
        {
            var textractClaimImage = LoadTextractClaimImage(compressedOCRFileName);

            if (textractClaimImage.Claim == null || !textractClaimImage.Claim.IsValid)
            {
                //UpdateStatusBar("Required values did not map...", 4);
            }

            if (textractClaimImage == null || textractClaimImage.SourceImage == null)
            {
                MessageBox.Show("Error loading TextractClaimImage");
            }
            else
            {
                ViewerJpg childViewer = new ViewerJpg(textractClaimImage);
                childViewer.MdiParent = this;
                childViewer.Text = $"Textract Viewer {System.IO.Path.GetFileNameWithoutExtension(compressedOCRFileName)}";
                childViewer.Show();
            }
        }

        #region Status Methods

        protected override void StatusMessage(string Message)
        {
            if (Message.IsNullOrEmpty())
                this.StatusToolStripStatusLabel.Text = "Ready...";
            else
                this.StatusToolStripStatusLabel.Text = Message;

            this.statusStrip.Refresh();
        }

        protected override void StatusMessage()
        {
            base.StatusMessage();
        }

        void UpdateStatusBar(string appStatus = null, int secondsToClear = 0)
        {
            //ComboBox_ZoomLevel.Text = string.Format("{0}%", imageBox.Zoom);
            //zoomToolStripStatusLabel.Text = string.Format("{0}%", imageBox.Zoom);
            //autoScrollPositionToolStripStatusLabel.Text = this.FormatPoint(imageBox.AutoScrollPosition);
            //imageSizeToolStripStatusLabel.Text = this.FormatRectangle(imageBox.GetImageViewPort());
            //tsslMousePosition.Text = $"Image: [{e.X},{e.Y}]";

            this.StatusMessage(appStatus, secondsToClear);
        }
        #endregion

        #region MDI Handlers

        #region Misc
        private void ShowNewForm(object sender, EventArgs e)
        {
            string compressedOCRFileName = testNewForm;
            ViewTractClaimImage(compressedOCRFileName);
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void MDITextractViewer_MdiChildActivate(object sender, EventArgs e)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.toolstripmanager?redirectedfrom=MSDN&view=netcore-3.1

            //if ((Viewer)this.ActiveMdiChild != null)
            //    ToolStripManager.Merge(((Viewer)this.ActiveMdiChild).toolStrip, toolStrip);
        }
        #endregion

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.InitialDirectory = @"C:\temp\dental\_Archive\";
            openFileDialog.Filter = "Text Files (*.zip)|*.zip|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                ViewTractClaimImage(openFileDialog.FileName);
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options options = new Options();
            options.Text = "Textract Viewer - Options";
            options.Show(this);
        }

        private void ReBuildToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
        }
        
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            Form activeChild = this.ActiveMdiChild;

            if (activeChild != null)
            {
                try
                {
                    if (activeChild is ViewerJpg)
                    {
                        ConfirmationDialog confirmation = new ConfirmationDialog();
                        DialogResult result = confirmation.ShowDialog(this);

                        if (result == DialogResult.Cancel) return;

                        var tt = ((ViewerJpg)activeChild).TextractClaimImage;

                        tt.Save(confirmation.Export);

                        if (tt.Results.Count() > 0)
                        {
                            ValidationForm vf = new ValidationForm(tt);
                            vf.Show(this);
                        }
                        else 
                        {
                            string filler = confirmation.Export ? " and Exported" : "";
                            ConfirmationPopup popup = new ConfirmationPopup();
                            popup.Message = $"Textract Claim Image successfully Saved{filler}.";
                            popup.Show(this);
                        }

                    }
                    //if (activeChild is zViewerPdf)
                    //{ 
                    //    ((zViewerPdf)activeChild).TextractClaimImage.Save();
                    //}
                }
                catch (Exception ex)
                {
                    Logger.TraceError(ex);
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

    }
}
