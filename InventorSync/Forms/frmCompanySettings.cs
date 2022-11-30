using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventorSync.InventorBL.Master;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;
using InventorSync.Forms;
using Syncfusion.Windows.Forms.Tools;
using System.Runtime.InteropServices;
using System.IO;

namespace InventorSync.Forms
{
    public partial class frmCompanySettings : Form, IMessageFilter
    {
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        public frmCompanySettings()
        {
            InitializeComponent();

            Application.AddMessageFilter(this);

            try
            {
                Comm.TransparentControls(this);
                Comm.SetControlColours(this);

                tlpMain.BackColor = Color.FromArgb(249, 246, 238);
                this.BackColor = Color.Black;
                this.Padding = new Padding(1);

                //Comm.LoadBGImage(this, picBackground);

                lblHeading.Font = new Font("Tahoma", 21, FontStyle.Regular, GraphicsUnit.Pixel);
                lblSave.Font = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);

                lblSave.ForeColor = Color.Black;

                btnSave.Image = global::InventorSync.Properties.Resources.save240402;
                btnMinimize.Image = global::InventorSync.Properties.Resources.minimize_finalised;
                btnClose.Image = global::InventorSync.Properties.Resources.logout_Final;

                controlsToMove.Add(this);
                controlsToMove.Add(this.lblHeading);//Add whatever controls here you want to move the form when it is clicked and dragged

                Cursor.Current = Cursors.Default;

                LoadModelComapnies();

            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN &&
                        controlsToMove.Contains(Control.FromHandle(m.HWnd)))
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                return true;
            }
            return false;
        }

        private void LoadModelComapnies()
        {
            try
            {
                sqlControl rs = new sqlControl();
                string pwd = "";
                string uname = "";

                rs.Open("Select username, password from startup.dbo.tblusers where userid=" + Global.gblSuperUserID);
                if (!rs.eof())
                {
                    uname = rs.fields("username").ToString();
                    pwd = rs.fields("password").ToString();
                }

                if (Global.gblSuperUserID == 0 && Global.gblUserName == "DIGIPOS")
                {
                    Comm.LoadControl(cboExistingCompany, new DataTable(), @"select 0 as CompanyID, '<DEFAULT>' AS CompanyCode, '<DEFAULT>' AS CompanyName, 0 as sortorder 
                                                                         union select startup.dbo.tblCompany.CompanyID, startup.dbo.tblCompany.CompanyCode AS CompanyCode, startup.dbo.tblCompany.CompanyName, 1 as sortorder from startup.dbo.tblCompany, startup.dbo.tblUsers where startup.dbo.tblCompany.ParentID = startup.dbo.tblUsers.CompanyID order by sortorder, CompanyCode ", false, false, "CompanyName", "CompanyCode");
                    //from startup.dbo.tblCompany, startup.dbo.tblUsers where startup.dbo.tblCompany.ParentID = startup.dbo.tblUsers.CompanyID 
                }
                else
                {
                    Comm.LoadControl(cboExistingCompany, new DataTable(), @"select 0 as CompanyID, '<DEFAULT>' AS CompanyCode, '<DEFAULT>' AS CompanyName, 0 as sortorder 
                                                                         union select startup.dbo.tblCompany.CompanyID, startup.dbo.tblCompany.CompanyCode AS CompanyCode, startup.dbo.tblCompany.CompanyName, 1 as sortorder from startup.dbo.tblCompany, startup.dbo.tblUsers where startup.dbo.tblCompany.ParentID = startup.dbo.tblUsers.CompanyID and LTRIM(RTRIM(startup.dbo.tblUsers.UserName)) = '" + uname + "' and LTRIM(RTRIM(startup.dbo.tblUsers.Password)) = '" + pwd + "' order by sortorder, CompanyCode ", false, false, "CompanyName", "CompanyCode");
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //restore as new company
                if (Directory.Exists(@"C:\DIGIDATA") == false)
                    Directory.CreateDirectory(@"C:\DIGIDATA");

                if (Directory.Exists(@"C:\DIGIDATA") == false)
                { 
                    MessageBox.Show(@"Directory C:\DIGIDATA doesn't exist. Please check permissions and try again.","Create Company", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (Directory.Exists(@"C:\DIGIDATA\Data") == false)
                    Directory.CreateDirectory(@"C:\DIGIDATA\Data");

                if (Directory.Exists(@"C:\DIGIDATA\Data") == false)
                { 
                    MessageBox.Show(@"Directory C:\DIGIDATA\Data doesn't exist. Please check permissions and try again.", "Create Company", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("Password and Confirm Password should be identical. Please re-enter both passwords and try again.", "Create Company", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                string SqlFolderName = "";

                SqlFolderName = Global.SqlServerName.Replace(@"\", "").Replace(".", "");

                if (Directory.Exists(@"C:\DIGIDATA" + SqlFolderName) == false)
                    Directory.CreateDirectory(@"C:\DIGIDATA" + SqlFolderName);

                if (Directory.Exists(@"C:\DIGIDATA" + SqlFolderName) == false)
                {
                    MessageBox.Show(@"Cannot create directory C:\DIGIDATA" + SqlFolderName + ". Please check permissions and try again.", "Create Company", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                string SourceCompanyName = "";

                if (cboExistingCompany.SelectedIndex == 0)
                    SourceCompanyName = "DigiposModel";
                else
                    SourceCompanyName = cboExistingCompany.SelectedValue.ToString();
                
                string mdfName = @"C:\DIGIDATA\" + txtCompanyCode.Text + ".mdf";
                string ldfName = @"C:\DIGIDATA\" + txtCompanyCode.Text + ".ldf";

                string strQuery = @" CREATE DATABASE " + txtCompanyCode.Text.ToString() + " ON ( NAME = " + txtCompanyCode.Text + "_dat,FILENAME = '" + mdfName + "',SIZE = 10,MAXSIZE = UNLIMITED,FILEGROWTH = 5 ) " +
                                 " LOG ON ( NAME = " + txtCompanyCode.Text + "_log,   FILENAME = '" + ldfName + "',     SIZE = 5MB,     MAXSIZE = UNLIMITED,    FILEGROWTH = 5MB ) ";

                sqlControl cnn = new sqlControl("password=#infinitY@279;User ID=sa;Initial Catalog=Startup;Data Source=" + Global.SqlServerName);

                cnn.Execute(strQuery);

                if (cnn.Exception != "")
                {
                    MessageBox.Show(cnn.Exception + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Comm.Transferdatabase(cnn, SourceCompanyName, txtCompanyCode.Text.ToString(), @"C:\DIGIDATA\" + SqlFolderName);

                //update company details in ewstored database
                strQuery = "Select max(CompanyID) + 1 as newCompID from startup.dbo.tblCompany ";

                sqlControl Rs = new sqlControl();
                Rs.Open(strQuery);

                int lngCompID = 0;

                if (!Rs.eof())
                {
                    lngCompID = Comm.ToInt32(Rs.fields("newCompID"));
                    if (lngCompID < 300) lngCompID = 300;
                }

                if (lngCompID < 1) lngCompID = 300;

                if (!Rs.eof())
                {
                    strQuery = "INSERT INTO tblCompany (CompanyID, ParentID, CompanyCode, CompanyName, Lock,Application,ApplicationID,ACTIVE,ClientID) VALUES (" + lngCompID + ", " + lngCompID + ", '" + txtCompanyCode.Text + "', '" + txtCompanyCode.Text.Replace("'", "''") + "', '0','DIGIPOS',101,1,'" + Global.ClientID + "')"; 
                    cnn.Execute(strQuery);

                    int userid = 0;
                    string Password = "";
                    Rs.Open("Select max(userid) + 1 as newuserid from startup.dbo.tblUsers "); 
                    if(!Rs.eof())
                    {
                        userid = Comm.ToInt32(Rs.fields("newuserid"));
                    }
                    //if (cboExistingCompany.SelectedIndex > 0)
                    //{
                    //    Rs.Open("Select Password From startup.dbo.tblUsers, startup.dbo.tblCompany Where startup.dbo.tblUsers.CompanyID=startup.dbo.tblCompany.CompanyID and username='admin' and companycode='" + cboExistingCompany.SelectedValue.ToString() + "' ");
                    //    if (!Rs.eof())
                    //        Password = Rs.fields("Password");
                    //}
                    //else
                    //{
                    //    Password = "admin";
                    //}


                    Password = txtPassword.Text;


                    strQuery = "INSERT INTO startup.dbo.tblUsers (UserID, UserName, Password, CompanyID) VALUES (" + userid + ", 'admin', '" + Password + "', " + lngCompID + ")"; 
                    cnn.Execute(strQuery);

                    strQuery = "update " + txtCompanyCode.Text + ".dbo.tblUserMaster set pwd='" + Password + "' where username='admin'"; 
                    cnn.Execute(strQuery);
                }

                MessageBox.Show("Company Created Successfully.", "DIGIPOS", MessageBoxButtons.OK, MessageBoxIcon.Information); 
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
