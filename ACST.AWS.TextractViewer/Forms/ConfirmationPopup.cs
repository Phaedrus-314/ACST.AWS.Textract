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
    public partial class ConfirmationPopup : Form
    {
        public int SecondsToClear { get; set; }

        public string Message { get; set; }

        public bool Warning { get; set; }

        public ConfirmationPopup()
        {
            InitializeComponent();
        }

        public ConfirmationPopup(string message, bool warning = false, int secondsToClear = 0) : this()
        {
            this.Message = message;
            this.Warning = warning;
            this.SecondsToClear = secondsToClear;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            txtMessage.Text = Message;
            txtMessage.ForeColor = this.Warning ? Color.Red : Color.Black;

            if (SecondsToClear != 0)
            {
                Timer t = new Timer();
                t.Interval = SecondsToClear * 1000;
                t.Tick += new EventHandler(TimerTick);
                t.Start();
            }

            btnOK.Visible = SecondsToClear == 0;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
