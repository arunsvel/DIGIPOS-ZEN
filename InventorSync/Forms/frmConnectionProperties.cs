using DigiposZen.InventorBL.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigiposZen.Forms
{
    public partial class frmConnectionProperties : Form
    {
        public frmConnectionProperties(object MDIParent = null)
        {
            try
            {
                InitializeComponent();

                frmMDI form = (frmMDI)MDIParent;
                this.MdiParent = form;
                int l = this.Width;
                int t = this.Height;

                int x = form.ClientSize.Width - this.Width;
                int y = 10;

                this.SetBounds(x, y, l, t);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void frmConnectionProperties_Load(object sender, EventArgs e)
        {
            txtSqlServer.Text = Global.SqlServerName;
            //txtServerClient.Text = Global.x;
            txtCompanyCode.Text = Global.CompanyCode;
            txtUserName.Text = Global.gblUserName;
            //txtCDKey.Text = Global.x;
            //txtLicensedTo.Text = Global.x;
        }

        private void lblSqlServer_Click(object sender, EventArgs e)
        {

        }

        private void lblServerClient_Click(object sender, EventArgs e)
        {

        }

        private void lblCompanyCode_Click(object sender, EventArgs e)
        {

        }

        private void lblCDKey_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void txtCompanyCode_Click(object sender, EventArgs e)
        {

        }

        private void txtServerClient_Click(object sender, EventArgs e)
        {

        }

        private void txtSqlServer_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblOK_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void lblLogOut_Click(object sender, EventArgs e)
        {
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void frmConnectionProperties_Shown(object sender, EventArgs e)
        {
            if (btnOK.Enabled && btnOK.Visible)
                btnOK.Focus();
        }
    }
}
