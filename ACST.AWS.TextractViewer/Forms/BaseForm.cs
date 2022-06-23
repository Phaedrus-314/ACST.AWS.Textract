using ACST.AWS.Common;
using System;
using System.Drawing;
using System.Threading;
using Timer = System.Threading.Timer;
using System.Windows.Forms;

namespace ACST.AWS.TextractViewer
{

    // ACST Code uses ImageBox developed by:

    // Cyotek ImageBox
    // Copyright (c) 2010-2015 Cyotek Ltd.
    // http://cyotek.com
    // http://cyotek.com/blog/tag/imagebox

    // Licensed under the MIT License. See license.txt for the full text.

    // If you use this control in your applications, attribution, donations or contributions are welcome.

    internal partial class BaseForm : Form
    {
        #region Public Constructors

        public BaseForm()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Overridden Methods

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                this.Font = SystemFonts.MessageBoxFont;
            }

            base.OnLoad(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Cursor.Current = Cursors.Default;
        }

        #endregion

        #region Protected Members

        protected string FormatPoint(Point point)
        {
            return string.Format("X:{0}, Y:{1}", point.X, point.Y);
        }

        protected string FormatRectangle(RectangleF rect)
        {
            return string.Format("X:{0}, Y:{1}, W:{2}, H:{3}", (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        protected string FormatRectangleF(RectangleF rect)
        {
            return string.Format("X:{0}, Y:{1}, W:{2}, H:{3}", rect.X, rect.Y, rect.Width, rect.Height);
        }

        protected string FormatRectangleFCenter(RectangleF rect)
        {
            return string.Format("Xc:{0}, Yc:{1}", rect.X + rect.Width/2, rect.Y + rect.Height/2);
        }

        protected virtual void StatusMessage(string Message)
        {
            throw new NotImplementedException();
        }

        protected virtual void StatusMessage()
        {
            this.StatusMessage(null);
        }

        protected void StatusMessage(string Message, int SecondsToClear)
        {
            if (SecondsToClear == 0)
                SecondsToClear = 5;

            this.StatusMessage(Message);

            Timer TimeThread = new Timer(new TimerCallback(StatusMessage), null, SecondsToClear * 1000, 0);
        }

        protected void StatusMessage(object state)
        {
            try
            {
                this.BeginInvoke(new ThreadStart(this.StatusMessage), null);
            }
            catch { }
        }

        #endregion
    }
}
