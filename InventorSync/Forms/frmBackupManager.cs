using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DigiposZen.InventorBL.Master;
using DigiposZen.InventorBL.Helper;
using DigiposZen.Info;
using System.Runtime.InteropServices;
using System.IO;

namespace DigiposZen
{
    public partial class frmBackupManager : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;
        string olddata = "";
        string newdata = "";
        string oldvalue = "";

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        // ======================================================== >>
        // Description:             Backup Manager
        // Developed By:            Arun S
        // Completed Date & Time:   27/12/2022 6.30 PM
        // Last Edited By:          
        // Last Edited Date & Time: 
        // ======================================================== >>
        public frmBackupManager(int iAreaID = 0, bool bFromEdit = false, Control Controlpassed = null, bool blnDisableMinimize = false)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
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
                    lblSave.Font = new Font("Tahoma", 9, FontStyle.Regular, GraphicsUnit.Point);
                    lblDelete.Font = new Font("Tahoma", 9, FontStyle.Regular, GraphicsUnit.Point);

                    lblSave.ForeColor = Color.Black;
                    lblDelete.ForeColor = Color.Black;

                    btnSave.Image = global::DigiposZen.Properties.Resources.save240402;
                    btnDelete.Image = global::DigiposZen.Properties.Resources.delete340402;
                    btnMinimize.Image = global::DigiposZen.Properties.Resources.minimize_finalised;
                    btnClose.Image = global::DigiposZen.Properties.Resources.logout_Final;

                    FillTreeview();
                }
                catch (Exception ex)
                {
                    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                }

                controlsToMove.Add(this);
                controlsToMove.Add(this.lblHeading);//Add whatever controls here you want to move the form when it is clicked and dragged

                CtrlPassed = Controlpassed;

                this.BackColor = Global.gblFormBorderColor;

                if (blnDisableMinimize == true) btnMinimize.Enabled = false;

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load backup manager"+"\n"+ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region "VARIABLES  -------------------------------------------- >>"
        InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
        clsMaster clsMaster = new clsMaster();

        bool dragging = false;
        int xOffset = 0;
        int yOffset = 0;
        int iAction = 0;
        Control ctrl;
        string strCheck;
        string strSelectNodeName = "";
        Control CtrlPassed;
        #endregion

        #region "EVENTS ------------------------------------------------ >>"
        //For Drag Form
        private void tlpHeader_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void tlpHeader_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            xOffset = Cursor.Position.X - this.Location.X;
            yOffset = Cursor.Position.Y - this.Location.Y;
        }
        private void tlpHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                this.Location = new Point(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset);
                this.Update();
            }
        }

        private void frmBackupManager_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ClearAll();
                this.Show();
                Application.DoEvents();

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Backup manager couldn't load ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }
        }
        private void frmBackupManager_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (e.KeyCode == Keys.Escape)//Close
                        this.Close();
                else if (e.KeyCode == Keys.F5)//Save
                {
                }
                else if (e.KeyCode == Keys.F7)//Delete
                {
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Short Keys not working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if (e.Shift == true && e.KeyCode == Keys.Enter)
                    {
                        this.SelectNextControl(ctrl, false, false, false, false);
                    }
                    else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                    {
                        this.SelectNextControl(ctrl, true, true, true, true);
                    }
                    else if (e.KeyCode == Keys.Up)
                    {
                        this.SelectNextControl(ctrl, false, true, true, true);
                    }
                    else
                        return;
                }
                else
                {
                    if (e.Shift == true && e.KeyCode == Keys.Enter)
                    {
                        this.SelectNextControl(ctrl, false, true, true, true);
                    }
                    else if (e.KeyCode == Keys.Enter)
                    {
                        this.SelectNextControl(ctrl, true, true, true, true);
                    }
                    else if (e.KeyCode == Keys.Up && e.Control)
                    {
                        this.SelectNextControl(ctrl, false, true, true, true);

                    }
                    else
                        return;
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex )
            {
                MessageBox.Show("Enter Key Press not working" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.gblROLEOFSYSTEM.ToUpper().Contains("CLIENT") == true)
                { 
                    MessageBox.Show("Unable to create backup form client system. " + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Cursor.Current = Cursors.WaitCursor;

                SelectedNodes = "";
                string Companies = GetCheckedNodes(tvwUserCompanyBackup.Nodes);

                if (MessageBox.Show("Following companies are selected for backup..." + "\n" + Companies + "." + "\n\n" + "Are you sure to continue?" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                Comm.BACKUPDB(ref progressBar1, "bfb", @"C:\backup");

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save..."+"\n"+ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                DialogResult dlgResult = MessageBox.Show("Are you sure to delete the company [" + strSelectNodeName + "] Permanently ?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dlgResult.Equals(DialogResult.Yes))
                    DeleteCompany();

                //Comm.writeuserlog(Common.UserActivity.Delete_Entry, newdata, olddata, "Deleted company " + AreaInfo.Area, 521, 521, AreaInfo.Area, Comm.ToInt32(AreaInfo.AreaID), "Area");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Delete..." + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = MessageBox.Show("Your are in the middle of an Entry. Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dlgResult.Equals(DialogResult.Yes))
                this.Close();
        }
        #endregion

        #region "METHODS ----------------------------------------------- >>"
        //Description : Validating the Mandatory Fields Before Save Functionality
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

        string SelectedNodes = "";

        public string GetCheckedNodes(TreeNodeCollection nodes)
        {
            foreach (System.Windows.Forms.TreeNode aNode in nodes)
            {
                //edit
                if (!aNode.Checked)
                {
                    if (aNode.Nodes.Count != 0)
                        GetCheckedNodes(aNode.Nodes);

                    continue;
                }

                SelectedNodes += aNode.Text + ",";

                if (aNode.Nodes.Count != 0)
                    GetCheckedNodes(aNode.Nodes);
            }

            return SelectedNodes;
        }

        /*

        public void DatabaseFunctionality(string str, string CompanyCode, string CompanyName)
        {
            try
            {
                // MsgBox("sTARTED")
                if (str == "backup")
                {
                    progressBar1.Visible = true;
                    Progress(true, 10);

                    Progress(true, 30);
                    if (File.Exists(txtBackupPath.Text) == true)
                    {
                        MessageBox.Show("File already exists. Please choose a new file to backup.", MsgBoxStyle.Information);
                        return;
                    }
                    Progress(true, 50);
                    Cursor = Cursors.WaitCursor;

                    string CurYear = DateTime.Today.Year.ToString();
                    string CurMonth = DateTime.Today.Month.ToString();
                    string CurDate = DateTime.Today.Date.ToString();



                    if (Directory.Exists(@"C:\SQLBK\" + CompanyCode) == true)
                        Directory.Delete(@"C:\SQLBK\" + CompanyCode);
                    // MsgBox("2")
                    if (Directory.Exists(@"C:\SQLBK\" + CompanyCode) == false)
                        Directory.CreateDirectory(@"C:\SQLBK\" + CompanyCode);
                    // MsgBox("3")
                    if (Directory.Exists(@"C:\SQLBK\" + CompanyCode) == false)
                    {
                        MessageBox.Show("Path not found. Could not create temporary file or directory for backup creation.");
                        Cursor = Cursors.Default;
                        return;
                    }

                    bool blnFailed = false;
                    if (File.Exists(@"C:\SQLBK\" + CompanyCode + @"\" + CompanyCode + ".bak") == true)
                        File.Delete(@"C:\SQLBK\" + CompanyCode + @"\" + CompanyCode + ".bak");

                    if (Comm.query("backup database " + CompanyCode + " to disk='" + @"C:\SQLBK\" + CompanyCode + @"\" + CompanyCode + ".bak'") == true)
                    {
                        if (BackupFile.CreateBakupFile(cmbBackupDatabase.Text) == false)
                        {
                            MsgBox("Failed to backup database. Could not create file to " + txtBackupPath.Text.ToString);
                            blnFailed = true;
                        }
                        else
                        {
                            MsgBox("Backup process completed successfully. File copied to " + txtBackupPath.Text.ToString, MsgBoxStyle.Information);
                            writeuserlog(UserActivity.new_Entry, "BackUp", "", "Successfully completed backup process", 0, 0, DCSApp.CompanyCode, 0, "BACKUP DATABASE");
                            progressBar1.Visible = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to backup database. Backup process aboted abnormally.");
                        blnFailed = true;
                    }

                    if (blnFailed)
                    {
                        Progress(true, 0);
                        progressBar1.Visible = false;
                        Cursor = Cursors.Default;
                        return;
                    }

                    Progress(true, 100);
                    progressBar1.Visible = false;
                }
                else if (str == "restore")
                {
                    Progress(true, 10);
                    Application.DoEvents();
                    if (File.Exists(txtRestoreFilePath.Text) == false)
                    {
                        MessageBox.Show("File not found. Please select a backup file.", MsgBoxStyle.Exclamation);
                        Cursor = Cursors.Default;
                        return;
                    }
                    if (System.IO.Path.GetExtension(txtRestoreFilePath.Text).ToString.ToLower == ".back" | System.IO.Path.GetExtension(txtRestoreFilePath.Text).ToString.ToLower == ".zip" | System.IO.Path.GetExtension(txtRestoreFilePath.Text).ToString.ToLower == ".bac")
                    {
                    }
                    else
                    {
                        MessageBox.Show("Please open a valid backup file to continue.");
                        Cursor = Cursors.Default;
                        return;
                    }

                    Progress(true, 30);
                    Application.DoEvents();
                    string BakFilePath = "";
                    string dbname = RestoreFile.CreateRestoreFile(cmbRestoreDatabase.Text.ToString, BakFilePath);
                    Progress(true, 70);
                    Application.DoEvents();
                    if (dbname == "")
                    {
                        MessageBox.Show("Unable to restore database. Restore operation aborts unexpectedly.");
                        Cursor = Cursors.Default;
                        return;
                    }

                    string RestorePath;
                    RestorePath = txtRestorePath.Text.ToString;
                    if (Strings.Mid(RestorePath, Strings.Len(RestorePath) - 1, 1) != @"\")
                        RestorePath = RestorePath + @"\";
                    // query("RESTORE DATABASE " & dbname & " FROM disk='" & BakFilePath & "'")
                    // '''''https://www.mssqltips.com/sqlservertutorial/122/retore-sql-server-database-to-different-filenames-and-locations/

                    string DatName = "";
                    string LogName = "";
                    bool blnRestored = false;

                    Progress(true, 60);
                    Application.DoEvents();
                    cmd = new SqlCommand("RESTORE FILELISTONLY FROM DISK = N'" + BakFilePath + "'", con);
                    cmd.CommandTimeout = 0;
                    Progress(true, 70);
                    SqlDataReader dr = cmd.ExecuteReader();
                    Progress(true, 80);
                    cmd.Clone();
                    cmd.Dispose();
                    while (dr.Read)
                    {
                        if (Mid(dr.Item("physicalname"), dr.Item("physicalname").ToString.Length - 2, 3).ToUpper == "DAT" | Mid(dr.Item("physicalname"), dr.Item("physicalname").ToString.Length - 2, 3).ToUpper == "MDF")
                            DatName = dr.Item("logicalname").ToString;
                        if (Mid(dr.Item("physicalname"), dr.Item("physicalname").ToString.Length - 2, 3).ToUpper == "LDF" | Mid(dr.Item("physicalname"), dr.Item("physicalname").ToString.Length - 2, 3).ToUpper == "LOG")
                            LogName = dr.Item("logicalname").ToString;
                    }
                    dr.Close();

                    if (DatName != "" & LogName != "")
                    {
                        if (query("RESTORE DATABASE " + dbname + " FROM disk='" + BakFilePath + "' " + "With Move '" + DatName + "' TO '" + RestorePath + dbname + "_Dat.mdf', " + "Move '" + LogName + "' TO '" + RestorePath + dbname + "_Log.ldf' "))
                        {
                            RestoreFile.ClearRestoreFiles();
                            Progress(true, 100);
                            blnRestored = true;

                            int CompanyID = 0;
                            string strQuery = "password=NEWTECH007$;User ID=sa;Initial Catalog=dcsStartup;Data Source=" + My.Settings.SQLServerName;
                            sqlControl cnn = new sqlControl(strQuery);
                            cnn.Open("Select * from tblCompany Where CompanyCode='" + dbname + "' ");
                            // If there is no such a company
                            if (cnn.eof)
                            {
                                cnn.Open("Select max(companyid)+1 as companyid from tblcompany");

                                if (cnn.eof)
                                    CompanyID = 301;
                                else
                                    CompanyID = cnn.companyid;

                                if (CompanyID <= 300)
                                    CompanyID = 301;
                                string ClientID = DCSApp.ClientID;
                                if (ClientID == "")
                                    ClientID = "DEFAULT";
                                strQuery = "INSERT INTO tblCompany (CompanyID, CompanyCode, CompanyName, Lock,Application,ApplicationID,ACTIVE,ClientID) VALUES (" + CompanyID + ", '" + dbname + "', '" + Strings.Replace(dbname, "'", "''") + "', '0','IBizNexera',101,1,'" + ClientID + "')";
                                cnn.Execute(strQuery);
                            }

                            MessageBox.Show("Restored database successfully.", MsgBoxStyle.Information);
                            // writeuserlog(UserActivity.new_Entry, "Restore", "Successfully completed restore process", 0, 0, DCSApp.CompanyCode, 0, "RESTORE DATABASE")
                            ProgressBar2.Visible = false;
                        }
                    }

                    if (blnRestored == false)
                        MessageBox.Show("Database could not be restored.", MsgBoxStyle.Information);
                }
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        */

        private void DeleteCompany()
        {
            try
            {

            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to delete company" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private bool IsValidate()
        {
            bool bValidate = true;
            return bValidate;
        }

        private void FillTreeview()
        {
            try
            {
                DataSet ds = new DataSet();
                TreeNode parentNode;
                if (Global.gblUserName.Trim().ToUpper() == "DIGIPOS")
                    ds = Comm.fnGetData("SELECT tblCompany.CompanyID,tblCompany.CompanyCode,CompanyName FROM startup.dbo.tblCompany as tblCompany, startup.dbo.tblUsers as tblUsers WHERE tblCompany.CompanyID=tblUsers.CompanyID and tblCompany.ParentID=tblCompany.CompanyID ");
                else
                    ds = Comm.fnGetData("SELECT tblCompany.CompanyID,tblCompany.CompanyCode,CompanyName FROM startup.dbo.tblCompany as tblCompany, startup.dbo.tblUsers as tblUsers WHERE tblCompany.CompanyID=tblUsers.CompanyID and tblCompany.ParentID=tblCompany.CompanyID and LTRIM(RTRIM(tblUsers.UserName)) = '" + Global.gblUserName.ToString() + "' AND LTRIM(RTRIM(tblUsers.Password)) = '" + Global.gblpwd.ToString() + "' ");

                tvwUserCompanyBackup.Nodes.Clear();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    parentNode = tvwUserCompanyBackup.Nodes.Add(dr["CompanyCode"].ToString(), dr["CompanyName"].ToString());
                    PopulateTreeView(Convert.ToInt32(dr["CompanyID"].ToString()), parentNode);
                }

                tvwUserCompanyBackup.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateTreeView(int parentId, TreeNode parentNode)
        {
            try
            {
                DataTable dtgetData = new DataTable();
                if (Global.gblUserName.Trim().ToUpper() == "DIGIPOS")
                    dtgetData = Comm.fnGetData("SELECT C.CompanyID,C.CompanyCode,REPLACE((CONVERT(VARCHAR,FyStartDate,103) + '-' + CONVERT(VARCHAR,FyEndDate,103)),'','-') as FinYear FROM startup.dbo.tblCompany C, startup.dbo.tblUsers U WHERE C.CompanyID=U.CompanyID AND C.ParentID = " + parentId + "").Tables[0];
                else
                    dtgetData = Comm.fnGetData("SELECT C.CompanyID,C.CompanyCode,REPLACE((CONVERT(VARCHAR,FyStartDate,103) + '-' + CONVERT(VARCHAR,FyEndDate,103)),'','-') as FinYear FROM startup.dbo.tblCompany C, startup.dbo.tblUsers U WHERE C.CompanyID=U.CompanyID and U.UserID = " + Global.gblSuperUserID + " AND C.ParentID = " + parentId + "").Tables[0];

                TreeNode childNode;
                foreach (DataRow dr in dtgetData.Rows)
                {
                    if (parentNode == null)
                    {
                        childNode = tvwUserCompanyBackup.Nodes.Add(dr["CompanyCode"].ToString(), dr["FinYear"].ToString());
                    }
                    else
                    {
                        parentNode.Tag = dr["CompanyID"].ToString();
                        childNode = parentNode.Nodes.Add(dr["CompanyCode"].ToString(), dr["FinYear"].ToString());
                    }
                    //PopulateTreeView(Convert.ToInt32(dr["CategoryID"].ToString()), childNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Description : Clear Data from Form
        private void ClearAll()
        {
            btnDelete.Enabled = false;
        }
        #endregion

        private void frmBackupManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dlgResult = MessageBox.Show("Do you want to exit backup / restore manager?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dlgResult.Equals(DialogResult.No))
                e.Cancel = true;
        }
    }
}

