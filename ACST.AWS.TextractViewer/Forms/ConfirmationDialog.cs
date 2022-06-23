using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACST.AWS.TextractViewer
{
    public partial class ConfirmationDialog : Form
    {
        public bool Export { get { return rbExportOpenNext.Checked | rbExport.Checked; } }

        public bool OpenNext { get { return rbExportOpenNext.Checked; } }

        public ConfirmationDialog()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Properties.Settings.Default.ConfirmationSaveChoice = rbSave.Checked;
            Properties.Settings.Default.ConfirmationSaveAndExport = rbExport.Checked;
            Properties.Settings.Default.ConfirmationSaveExportAndOpen = rbExportOpenNext.Checked;

            //if (Properties.Settings.Default.SaveSettingsOnExit)
            Properties.Settings.Default.Save();

            base.OnClosing(e);
        }
        private void ConfirmationDialog_Load(object sender, EventArgs e)
        {

            rbSave.Checked = Properties.Settings.Default.ConfirmationSaveChoice;
            rbExport.Checked = Properties.Settings.Default.ConfirmationSaveAndExport;
            rbExportOpenNext.Checked = Properties.Settings.Default.ConfirmationSaveExportAndOpen;
            //rbExportOpenNext.Checked = Properties.Settings.Default.;



            //if (!chkRememberChoice.Checked)
            //{
            //    rbSave.Checked = true;
            //}
        }
    }
}
