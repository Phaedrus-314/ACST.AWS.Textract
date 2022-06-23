using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACST.AWS.TextractViewer
{
    internal partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.MaximizeOnOpen = cbMaximizeOnOpen.Checked;
            Properties.Settings.Default.HighlightOnTab = chkHighlightCurrentField.Checked;

            Properties.Settings.Default.Save();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            this.Close();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            txtHighConfidence.ForeColor = Color.Black;
            txtBorderlineConfidence.ForeColor = Color.Black;
            txtLowConfidence.ForeColor = Color.Black;

            txtHighConfidence.BackColor = TextractClaimImageStyle.ocrConfidencePass;
            txtBorderlineConfidence.BackColor = TextractClaimImageStyle.ocrConfidenceBorderline;
            txtLowConfidence.BackColor = TextractClaimImageStyle.ocrConfidenceFail;

            txtDefinedInSpecLegend.ForeColor = TextractClaimImageStyle.matchedFieldValueColor;
            txtMappedToClaimLegend.ForeColor = TextractClaimImageStyle.mappedFieldValueColor;
            txtNotDefinedInSpecLegend.ForeColor = TextractClaimImageStyle.notMatchedFieldColor;
            txtOcrLegend.ForeColor = TextractClaimImageStyle.ocrFieldValueColor;

            cbMaximizeOnOpen.Checked = Properties.Settings.Default.MaximizeOnOpen;
            chkHighlightCurrentField.Checked = Properties.Settings.Default.HighlightOnTab;

            //var index = dgvKeyboardShortcuts.Rows.Add();
            //dgvKeyboardShortcuts.Rows[index].Cells[0].Value = "Ctrl + S";
            //dgvKeyboardShortcuts.Rows[index].Cells[1].Value = "Save and or Export";
            //index = dgvKeyboardShortcuts.Rows.Add();
            //dgvKeyboardShortcuts.Rows[index].Cells[0].Value = "Ctrl + H";
            //dgvKeyboardShortcuts.Rows[index].Cells[1].Value = @"Show / Hide field highlights";

        }

    }
}
