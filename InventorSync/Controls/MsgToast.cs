using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigiposZen.Controls
{
    public partial class MsgToast : Form
    {
        string strCaption, strMessage, strLocfrom;
        int XPosition, YPosition, intWaitTimeinSeconds;

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public MsgToast(string sCaption = "", string sMessage = "",string sLocFrom = "", int iWaitTimeinSec = 3, int XPos = 0, int YPos = 0)
        {
            InitializeComponent();

            strCaption = sCaption;
            strMessage = sMessage;
            strLocfrom = sLocFrom;

            XPosition = XPos;
            YPosition = YPos;

            intWaitTimeinSeconds = iWaitTimeinSec;
        }

        private void MsgToast_Load(object sender, EventArgs e)
        {
            decimal dMsgLine = 0;
            decimal dHeight = this.Height;
            lblToastHeading.Text = strCaption;
            lblToastMessage.Text = strMessage;
            tmrToast.Enabled = true;

            if (strMessage.Length == 0) strMessage = "_";
            dMsgLine = Math.Round(Convert.ToDecimal(strMessage.Length / 30), 0);
            this.Height = Convert.ToInt32(dHeight + (dMsgLine * 20));

            Screen scr = Screen.FromPoint(this.Location);
            if(strLocfrom.ToUpper() == "TOP-RIGHT")
                this.Location = new Point(scr.WorkingArea.Right - this.Width, scr.WorkingArea.Top);
            else if (strLocfrom.ToUpper() == "TOP-LEFT")
                this.Location = new Point(scr.WorkingArea.Left, scr.WorkingArea.Top);
            else if(strLocfrom.ToUpper() == "BOTTOM-RIGHT")
                this.Location = new Point(scr.WorkingArea.Right - this.Width, scr.WorkingArea.Bottom - this.Height);
            else if (strLocfrom.ToUpper() == "BOTTOM-LEFT")
                this.Location = new Point(scr.WorkingArea.Left, scr.WorkingArea.Bottom - this.Height);
            else if(strLocfrom.ToUpper() == "")
                this.Location = new Point(XPosition, YPosition);
        }

        private void tmrToast_Tick(object sender, EventArgs e)
        {
            if (lblTmrVal.Text == "") lblTmrVal.Text = "0";
            lblTmrVal.Text = (Convert.ToInt32(lblTmrVal.Text) + 100).ToString();
            if (Convert.ToInt32(lblTmrVal.Text) == intWaitTimeinSeconds * 100)
            {
                tmrToast.Enabled = false;
                tmrClose.Enabled = true;
            }
        }

        private void tmrClose_Tick(object sender, EventArgs e)
        {
            if (this.Height < 20)
            {
                tmrClose.Enabled = false;
                this.Close();
            }
            else
                this.Height = this.Height - 10;
        }
    }
}
