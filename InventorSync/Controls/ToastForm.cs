using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace InventorSync.Controls
{
    public partial class ToastForm : Form
    {
        public ToastForm(int iLifeTime, string sMessage)
        {
            InitializeComponent();
            lifeTimer.Interval = iLifeTime;
            lblToastMessage.Text = sMessage;
        }

        #region "Variables --------------------------- >>"

        private static List<ToastForm> openForms = new List<ToastForm>();
        private bool allowFocus = false;
        //private FormAnimator animator;
        private IntPtr currentForegroundWindow;

        #endregion

        #region " Constructors ----------------------- >>"

        #endregion
    }
}
