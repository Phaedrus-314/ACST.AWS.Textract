
namespace ACST.AWS.TextractViewer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using ACST.AWS.Common;

    internal partial class MDITextractViewer
        : BaseForm
    {

        #region Fields & Properties
        
        bool autoOpen = false;
        ////const string testNewForm = "";

        const string testNewForm = @"C:\BTS Folders\Dental\Working\Review\Dental Claims 1005201024_5.zip";
        //const string testNewForm = @"C:\BTS Folders\Dental\Working\Review\Dental Claims 1005201024_65.zip";
        //const string testNewForm = @"C:\BTS Folders\Dental\Working\Review\Dental Claims 0828201024_43.zip";
        //const string testNewForm = @"C:\BTS Folders\Dental\Working\Review\ScanToJPEG-08282020080956.zip"; // Upside down image
        //const string testNewForm = @"C:\BTS Folders\Dental\Working\Review\bemOnly_Dental Claims 0828201024_13.zip"; // 2012
        //const string testNewForm = @"C:\BTS Folders\Dental\Working\Review\bemOnly_Dental Claims 0828201024_7.zip"; // 2006

        const string DisplayNamePrefix = "Textract Viewer ";
        const string ArchiveFileExtension = ".zip";
        const string OpenDialogFilters = "Text Files (*.zip)|*.zip|All Files (*.*)|*.*";
        const bool RebuildClaim = false;

        string currentArchivePath;
        #endregion

        #region Constructors
        
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
        #endregion

        #region Form Overrides

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ReBuildToolStripButton.Checked = RebuildClaim;

            if (Properties.Settings.Default.MaximizeOnOpen)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;

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
                nextAvaliableToolStripButton.Enabled = true;
                previousAvaliableToolStripButton.Enabled = true;
                nextAvaliableToolStripMenuItem.Enabled = true;
                saveToolStripButton.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
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

        #region Textract Methods

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form activeChild = this.ActiveMdiChild;

            if (activeChild != null)
                activeChild.Close();
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        TextractClaimImage LoadTextractClaimImage(string compressedOCRFileName, bool remapClaim = false)
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

        IEnumerable<string> OpenFileNames ()
        {
            return MdiChildren.Select(m => $"{m.Text.Replace(DisplayNamePrefix, string.Empty)}{ArchiveFileExtension}");
        }

        private void PreviousForm()
        {
            try
            {
                Form PrevForm = null;
                foreach (Form ChildForm in this.MdiChildren)
                {
                    if (ChildForm.Equals(this.ActiveMdiChild))
                    {
                        if ((PrevForm != null))
                        {
                            this.ActivateMdiChild(PrevForm);
                            PrevForm.Focus();
                            break;
                        }
                    }
                    PrevForm = ChildForm;
                }
                PrevForm = null;

            }
            catch (Exception ex)
            {
            }

        }

        private void OpenNextForm(object sender, EventArgs e)
        {

            if (currentArchivePath.IsNullOrWhiteSpace()) return;

            ToolStripButton tsButton = (ToolStripButton)sender;

            DirectoryInfo info = new DirectoryInfo(currentArchivePath);
            FileInfo[] files = info.GetFiles()
                                    .Where(p => p.Extension == ArchiveFileExtension)
                                    .OrderBy(p => p.CreationTime)
                                    .ToArray();
            string fn = null;
            var t = OpenFileNames();

            switch (tsButton.Tag)
            {
                case "Previous":
                    PreviousForm();
                    return;
                    //fn = files.FirstOrDefault(p => !OpenFileNames().Contains(p.Name))?.FullName;
                    //break;

                case "Next":
                    fn = files.LastOrDefault(p => !OpenFileNames().Contains(p.Name))?.FullName;
                    break;

                default:
                    break;
            }
                        
            if (fn == null)
            {
                ConfirmationPopup popup = new ConfirmationPopup($"There are no more files avaliable for review.", true, 4);
                popup.Show(this);
            }
            else
                ViewTractClaimImage(fn);
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = OpenDialogFilters;

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                nextAvaliableToolStripButton.Enabled = true;
                previousAvaliableToolStripButton.Enabled = true;
                nextAvaliableToolStripMenuItem.Enabled = true;
                saveToolStripButton.Enabled = true;
                saveToolStripMenuItem.Enabled = true;

                currentArchivePath = Path.GetDirectoryName(openFileDialog.FileName) + @"\";
                
                var F = MdiChildren.SingleOrDefault(m => m.Text == $"DisplayNamePrefix{Path.GetFileNameWithoutExtension(openFileDialog.FileName)}");

                if (F == null)
                {
                    UpdateStatusBar("Open and Validate Claim...");
                    ViewTractClaimImage(openFileDialog.FileName);
                }
                else
                    F.Activate();
            }
            UpdateStatusBar();
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

            UpdateStatusBar("Saving...");
            Cursor.Current = Cursors.WaitCursor;

            if (activeChild != null)
            {
                try
                {
                    if (activeChild is ViewerJpg)
                    {
                        ConfirmationDialog confirmation = new ConfirmationDialog();
                        DialogResult result = confirmation.ShowDialog(this);

                        if (result == DialogResult.Cancel) return;

                        var textractClaimImage = ((ViewerJpg)activeChild).TextractClaimImage;

                        UpdateStatusBar("Validating...");

                        textractClaimImage.Save(confirmation.Export);
                        
                        string filler = confirmation.Export ? $" and Exported with ClaimNo: {textractClaimImage.MetaData.ClaimNo}" : string.Empty;

                        ConfirmationPopup popup = new ConfirmationPopup($"Textract Claim Image Successfully Saved{filler}.", false, 5);

                        if (confirmation.Export)
                        {
                            if (textractClaimImage.Claim.IsValid)
                            {
                                UpdateStatusBar($"Exporting Claim No: {textractClaimImage.MetaData.ClaimNo}", 8);
                                textractClaimImage.Archive();
                                popup.Show(activeChild);
                                activeChild.Close();

                                if (confirmation.OpenNext)
                                    nextAvaliableToolStripButton.PerformClick();
                            }
                            else
                            {
                                popup = new ConfirmationPopup($"Please correct validation errors and then Export again.", true, 4);
                                popup.Show(activeChild);
                                ((ViewerJpg)activeChild).CreateValidationResultContext();
                            }
                        }
                        else
                            popup.Show(activeChild);

                    }
                }
                catch (Exception ex)
                {
                    Logger.TraceError(ex);
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    UpdateStatusBar("Ready...");
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        void ViewTractClaimImage(string compressedOCRFileName)
        {
            if (File.Exists(compressedOCRFileName))
            {
                var textractClaimImage = LoadTextractClaimImage(compressedOCRFileName);

                //if (textractClaimImage.Claim == null || !textractClaimImage.Claim.IsValid)
                //{
                //    //UpdateStatusBar("Required values did not map...", 4);
                //}

                if (textractClaimImage == null || textractClaimImage.SourceImage == null)
                {
                    MessageBox.Show("Error loading TextractClaimImage", "Source Error");
                }
                else
                {
                    ViewerJpg childViewer = new ViewerJpg(textractClaimImage);
                    childViewer.MdiParent = this;
                    childViewer.Text = $"Textract Viewer {System.IO.Path.GetFileNameWithoutExtension(compressedOCRFileName)}";
                    childViewer.Show();
                }
            }
            else
            {
                MessageBox.Show($"OCR Archive file not found:\r\r{compressedOCRFileName}","Source Error");
            }
        }
        #endregion

        #region MDI Handlers

        private void nextAvaliableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nextAvaliableToolStripButton.PerformClick();
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

        private void MDITextractViewer_MdiChildActivate(object sender, EventArgs e)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.toolstripmanager?redirectedfrom=MSDN&view=netcore-3.1

            //if ((Viewer)this.ActiveMdiChild != null)
            //    ToolStripManager.Merge(((Viewer)this.ActiveMdiChild).toolStrip, toolStrip);
        }
        #endregion

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
    }
}
