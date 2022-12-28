using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DigiposZen.InventorBL.Helper;
using Newtonsoft.Json;
using System.IO;
using DigiposZen.Forms;
using System.Data.SqlClient;

namespace DigiposZen
{
    public partial class Login : Form
    {
        string mPrimaryServer = "";
        private bool mblnOnceActivated = false;
        public Login()
        {
            mblnOnceActivated = false;

            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        Common Comm = new Common();
        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            try
            {
                if (txtUsername.Text.ToLower() == "user name")
                    txtUsername.Text = "";
                else
                    txtUsername.Select(1, txtUsername.Text.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            //if (txtPassword.Text.ToLower() == "password")
            //    txtPassword.Text = "";
            //else
            //    txtPassword.Select(1, txtPassword.Text.Length);
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtUsername.Text == "")
                    txtUsername.Text = "User Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            //if (txtPassword.Text == "")
            //    txtPassword.Text = "Password";
        }

        private void pnlLogin_Paint(object sender, PaintEventArgs e)
        {
            //pnlLogin.Location = new Point(ClientSize.Width / 2 - pnlLogin.Size.Width / 2, ClientSize.Height / 2 - pnlLogin.Height / 2);
            //pnlLogin.Anchor = AnchorStyles.None;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            //try
            //{
            //    this.tblpLoginScreen.ColumnStyles[0].SizeType = SizeType.Percent;
            //    this.tblpLoginScreen.ColumnStyles[0].Width = 100;

            //    this.tblpLoginScreen.ColumnStyles[1].SizeType = SizeType.Absolute;
            //    this.tblpLoginScreen.ColumnStyles[1].Width = 0;

            //    FillTimeZone();
            //    rdoComputerName.Checked = true;
            //    rdoUser.Checked = true;
            //    ShowFormsAsperClick(1);
            //    cboRoleofSystem.SelectedIndex = 0;


            //    string sJason = File.ReadAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "LoginSettings.txt", Encoding.UTF8);

            //    clsLoginSettings LogSett = new clsLoginSettings();

            //    LogSett = JsonConvert.DeserializeObject<clsLoginSettings>(sJason);

            //    if (LogSett != null)
            //    {
            //        if (LogSett.ROLEOFSYSTEM != "")
            //            cboRoleofSystem.SelectedIndex = Convert.ToInt32(LogSett.ROLEOFSYSTEM);

            //        txtClientID.Text = LogSett.CLIENTID;

            //        if (LogSett.SQLCONNCONFIG == 1)
            //            rdoComputerName.Checked = true;
            //        else if (LogSett.SQLCONNCONFIG == 2)
            //            rdoIPAdress.Checked = true;
            //        else if (LogSett.SQLCONNCONFIG == 3)
            //            rdoCloudServer.Checked = true;

            //        cboPrimaryServer.Text = LogSett.PRIMSERVERNAME;
            //        cboSecondaryServer.Text = LogSett.SECOSERVERNAME;
            //        cboTimeZone.Text = LogSett.TIMEZONE;

            //        DigiposZen.Properties.Settings.Default.server = cboPrimaryServer.Text.ToString(); //fso.FileOperation(Application.StartupPath + "Resources\\Config.ini", true);
            //        DigiposZen.Properties.Settings.Default.ConnectionString = "Data Source=" + cboPrimaryServer.Text.ToString() + ";Initial Catalog=DIGIPOSDEMO;User ID=sa;Password=#infinitY@279";


            //        MakeViewForUserDatabase();
            //        ReadWriteLoginCredentials(false);
            //    }
            //}

            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void FillTimeZone()
        {
            try
            {
                foreach (TimeZoneInfo tzi in TimeZoneInfo.GetSystemTimeZones())
                {
                    cboTimeZone.Items.Add(tzi.DisplayName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowFormsAsperClick(int iColIndex = 1)
        {
            try
            {
                for (int g = 0; g < this.tblpForms.ColumnCount; g++)
                {
                    if (iColIndex == g + 1)
                    {
                        this.tblpForms.ColumnStyles[g].SizeType = SizeType.Percent;
                        this.tblpForms.ColumnStyles[g].Width = 100;
                    }
                    else
                    {
                        this.tblpForms.ColumnStyles[g].SizeType = SizeType.Absolute;
                        this.tblpForms.ColumnStyles[g].Width = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rdoUser_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(1);
        }

        private void rdoFavorite_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(2);
        }

        private void rdoLock_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(3);
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            ShowFormsAsperClick(4);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void rdoComputerName_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cboPrimaryServer.Text = "";
                cboSecondaryServer.Text = "";
                if (rdoComputerName.Checked == true)
                {
                    cboPrimaryServer.Text = Environment.MachineName.ToString() + "\\" + "DIGIPOS";
                    cboSecondaryServer.Text = GetIPAddress() + "\\" + "DIGIPOS";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rdoIPAdress_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cboPrimaryServer.Text = "";
                cboSecondaryServer.Text = "";
                if (rdoIPAdress.Checked == true)
                {
                    cboPrimaryServer.Text = GetIPAddress() + "/" + "SQLEXPRESS";
                    cboSecondaryServer.Text = Environment.MachineName.ToString() + "/" + "SQLEXPRESS";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetIPAddress()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                String strHostName = string.Empty;
                strHostName = System.Net.Dns.GetHostName();
                //sb.Append("The Local Machine Host Name: " + strHostName);
                //sb.AppendLine();
                System.Net.IPHostEntry ipHostEntry = System.Net.Dns.GetHostEntry(strHostName);
                System.Net.IPAddress[] address = ipHostEntry.AddressList;
                sb.Append(address[1].ToString());
                sb.AppendLine();
                return sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveServerSettings();
        }
        private void SaveServerSettings()
        {
            try
            {
                clsLoginSettings LogSett = new clsLoginSettings();
                LogSett.ROLEOFSYSTEM = cboRoleofSystem.SelectedIndex.ToString();
                LogSett.CLIENTID = txtClientID.Text;

                if (rdoComputerName.Checked == true)
                    LogSett.SQLCONNCONFIG = 1;
                else if (rdoIPAdress.Checked == true)
                    LogSett.SQLCONNCONFIG = 2;
                else if (rdoCloudServer.Checked == true)
                    LogSett.SQLCONNCONFIG = 3;

                LogSett.PRIMSERVERNAME = cboPrimaryServer.Text;
                LogSett.SECOSERVERNAME = cboSecondaryServer.Text;
                LogSett.TIMEZONE = cboTimeZone.Text;

                string sJason = JsonConvert.SerializeObject(LogSett);
                File.WriteAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "LoginSettings.txt", sJason);

                Comm.MessageboxToasted("Inventor", "Settings Updated !!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void changedbpwd()
        {
            try
            {
                string CnString = "Data Source=" + mPrimaryServer + ";Initial Catalog=master;User ID=sa;Password=changeme007$";

                sqlControl rs = new sqlControl(CnString, false);
                if (rs.connection.State == ConnectionState.Open)
                    rs.Execute(" sp_password 'changeme007$' ,'#infinitY@279', 'sa'");
            }
            catch
            { }
        }

        private void FirstRun()
        {
            try
            {
                try
                {
                    changedbpwd();
                }
                catch
                { }

                string CnString = "Data Source=" + mPrimaryServer + ";Initial Catalog=master;User ID=sa;Password=#infinitY@279";

                bool BlnFirstRun = true;

                sqlControl rs = new sqlControl(CnString);

                if (rs.connection.State != ConnectionState.Open)
                {
                    MessageBox.Show("A connection could not be established to the server. Please check sql server installation and try again.", "DIGIPOS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                lblIniitialize.Text = "Iniitialising database. Please wait ... ";
                prgInitialize.Minimum = 10;
                prgInitialize.Maximum = 100;

                rs.Open("select name,database_id,create_date from sys.databases where (name = 'STARTUP')");
                if (!rs.eof())
                {
                    BlnFirstRun = false;
                }

                try
                {
                    if (Directory.Exists(@"c:\digisql") == false)
                    {
                        Directory.CreateDirectory(@"c:\digisql");
                        Directory.CreateDirectory(@"c:\digisql\sqlTemp");
                    }
                    else
                    {
                        if (Directory.Exists(@"c:\digisql\sqlTemp") == false)
                        {
                            Directory.CreateDirectory(@"c:\digisql\sqlTemp");
                        }
                    }
                }
                catch
                { }

                try
                {
                    File.Copy(Application.StartupPath + @"\Resources\DigiposDemo.bak", @"c:\digisql\sqlTemp\DigiposDemo.bak", true);
                    prgInitialize.Value = 20;
                }
                catch
                { }

                try
                {
                    File.Copy(Application.StartupPath + @"\Resources\Startup.bak", @"c:\digisql\sqlTemp\Startup.bak", true);
                    prgInitialize.Value = 30;
                }
                catch
                { }

                try
                {
                    File.Copy(Application.StartupPath + @"\Resources\DigiposModel.bak", @"c:\digisql\sqlTemp\DigiposModel.bak", true);
                    prgInitialize.Value = 40;
                }
                catch
                { }

                Application.DoEvents();

                //C:\DIGIDATA

                try
                {
                    if (BlnFirstRun == true)
                        rs.Execute("Create DATABASE Startup");
                }
                catch
                { }

                try
                {
                    string SqlFolderName = mPrimaryServer.Replace(@"\", @"") + @"\";
                    if (Directory.Exists(@"C:\DIGIDATA\Data\" + SqlFolderName) == false)
                    {
                        Directory.CreateDirectory(@"C:\DIGIDATA\Data\" + SqlFolderName);
                    }
                }
                catch
                { }

                try
                {
                    if (BlnFirstRun == true)
                    {
                        Comm.RESTOREDB("Startup", @"c:\digisql\sqlTemp\Startup.bak", "");
                        prgInitialize.Value = 50;
                    }
                }
                catch
                { }

                try
                {
                    Comm.RESTOREDB("DigiposDemo", @"c:\digisql\sqlTemp\DigiposDemo.bak", "");
                    prgInitialize.Value = 70;
                }
                catch
                { }

                try
                {
                    Comm.RESTOREDB("DigiposModel", @"c:\digisql\sqlTemp\DigiposModel.bak", "");
                    prgInitialize.Value = 90;
                }
                catch
                { }

                try
                {
                    CnString = "Data Source=" + mPrimaryServer + ";Initial Catalog=master;User ID=sa;Password=#infinitY@279";
                    sqlControl cnn = new sqlControl(CnString);

                    cnn.Open(" SELECT count(database_id) as Nos FROM master.sys.databases           WHERE name in('Startup','DigiposDemo','DigiposModel')");
                    if (!cnn.eof())
                    {
                        if (Convert.ToInt32(cnn.fields("NOS")) < 3)
                        {
                            return;
                            MessageBox.Show("Sorry . We couldn't restore the files to the database. make sure your SQL is running.");
                        }
                        else
                        {
                            if (File.Exists(Application.StartupPath + @"\Resources\InitLog.ini") == false)
                            {
                                File.WriteAllText(Application.StartupPath + @"\Resources\InitLog.ini", "Default databases restored successfully to server " + mPrimaryServer);
                            }
                            return;
                        }
                    }
                }
                catch
                { }
            }
            catch
            { }
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(Application.StartupPath + @"\Resources"))
                {
                    if (File.Exists(Application.StartupPath + @"\Resources\InitLog.ini") == false)
                    {
                        //var frm = new FirstRun(cboPrimaryServer.Text.ToString());
                        //frm.ShowDialog();
                        //this.Close();
                        //Environment.Exit(0);


                        lblIniitialize.Visible = true;
                        prgInitialize.Visible = true;

                        mPrimaryServer = cboPrimaryServer.Text.ToString();
                        Global.SetSqlServerName(mPrimaryServer);
                        Global.SetROLEOFSYSTEM(cboRoleofSystem.Text.ToString());

                        FirstRun();
                    }
                }

                MakeViewForUserDatabase();

                if (DateTime.Today > Convert.ToDateTime("30-Jan-2023"))
                {
                    MessageBox.Show("Trial period is expired. Exiting application. Please contact customer care.");
                    Global.SetTrialExpired (true);
                    this.Close();
                    Environment.Exit(0);
                }
                if (DateTime.Today > Convert.ToDateTime("20-Jan-2023"))
                {
                    MessageBox.Show("Trial period is about to expire. Please contact customer care.");
                }

                if (CheckUserisValid(txtUsername.Text.Trim(), txtPassword.Text.Trim()) == true)
                {
                    Global.SetSqlServerName(cboPrimaryServer.Text.ToString());
                    Global.SetSqlServerName2(cboSecondaryServer.Text.ToString());
                    Global.SetROLEOFSYSTEM(cboRoleofSystem.Text.ToString());

                    this.tblpLoginScreen.ColumnStyles[0].SizeType = SizeType.Absolute;
                    this.tblpLoginScreen.ColumnStyles[0].Width = 0;

                    this.tblpLoginScreen.ColumnStyles[1].SizeType = SizeType.Percent;
                    this.tblpLoginScreen.ColumnStyles[1].Width = 100;

                    FillTreeview();

                    //Comm.LoadAppSettings();
                    ReadWriteLoginCredentials();
                    dtpProcessDate.Focus();
                }
                else
                {
                    MessageBox.Show("Incorrect user name or password. Check the credentials and try again.", "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ReadWriteLoginCredentials(bool bIsWrite = true)
        {
            try
            {
                string sFolderName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                string sErrFileName = sFolderName + "\\" + "LoginCredential.txt";
                if (bIsWrite == true)
                {
                    if (File.Exists(sErrFileName) == false)
                    {
                        File.Create(sErrFileName);
                    }
                    File.WriteAllText(sErrFileName, String.Empty);
                    if (pboxcheck.Visible == true)
                    {
                        using (StreamWriter sw = File.AppendText(sErrFileName))
                        {

                            sw.WriteLine("Username:" + txtUsername.Text);
                            sw.WriteLine("Password: " + txtPassword.Text);
                            if (pboxcheck.Visible == true)
                                sw.WriteLine("RememberMe:" + "1");
                            else
                                sw.WriteLine("RememberMe : " + "0");
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = File.AppendText(sErrFileName))
                        {
                            sw.WriteLine("Username:");
                            sw.WriteLine("Password:");
                            sw.WriteLine("RememberMe : " + "0");
                        }
                    }
                }
                else
                {
                    if (File.Exists(sErrFileName) == true)
                    {
                        string[] lines = File.ReadAllLines(sErrFileName);
                        if (lines.Length > 0)
                        {
                            string[] sData = lines[2].Split(':');
                            if (Convert.ToInt32(sData[1].ToString()) == 1)
                            {
                                string[] sCred = lines[0].Split(':');
                                txtUsername.Text = sCred[1].ToString();

                                string[] sCred1 = lines[1].Split(':');
                                txtPassword.Text = sCred1[1].ToString().Trim();

                                pboxcheck.Visible = true;
                                pboxUncheck.Visible = false;
                                txtPassword.Focus();
                            }
                            else
                                txtUsername.Focus();
                        }
                        else
                            txtUsername.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckUserisValid(string sUserName, string sPwd)
        {
            try
            {
                
                bool bresult = false;

                ClsFileOperation fso = new ClsFileOperation();

                DigiposZen.Properties.Settings.Default.server = cboPrimaryServer.Text.ToString(); //fso.FileOperation(Application.StartupPath + "Resources\\Config.ini", true);
                DigiposZen.Properties.Settings.Default.ConnectionString = "Data Source=" + DigiposZen.Properties.Settings.Default.server + ";Initial Catalog=Startup;User ID=sa;Password=#infinitY@279";

                if (sUserName == "digipos" && sPwd == "praesidium") //means protection in latin
                {
                    Global.SetSuperUserId(0);
                    Global.SetUserId(0);
                    Global.SetUserGroupId(0);
                    Global.SetUserName("DIGIPOS");
                    Global.Setpwd("");
                    Global.SetSystemName("Standard");

                    return true;
                }

                DataTable dtUser = Comm.fnGetData("SELECT userid,username,0 AS groupid FROM tblUsers WHERE LTRIM(RTRIM(UserName)) = '" + sUserName + "' AND LTRIM(RTRIM(Password)) = '" + sPwd + "'").Tables[0];
                if (dtUser.Rows.Count > 0)
                {
                    Global.SetSuperUserId(Convert.ToInt32(dtUser.Rows[0]["userid"]));
                    Global.SetUserId(Convert.ToInt32(dtUser.Rows[0]["userid"]));
                    Global.SetUserGroupId(Convert.ToInt32(dtUser.Rows[0]["groupid"]));
                    Global.SetUserName(dtUser.Rows[0]["username"].ToString());
                    Global.Setpwd(sPwd.ToString());

                    bresult = true;
                }
                return bresult;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void pboxUncheck_Click(object sender, EventArgs e)
        {
            try
            {
                pboxcheck.Visible = true;
                pboxUncheck.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pboxcheck_Click(object sender, EventArgs e)
        {
            try
            {
                pboxcheck.Visible = false;
                pboxUncheck.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblRememberMe_Click(object sender, EventArgs e)
        {
            try
            {
                if (pboxcheck.Visible == true)
                {
                    pboxcheck.Visible = false;
                    pboxUncheck.Visible = true;
                }
                else
                {
                    pboxcheck.Visible = true;
                    pboxUncheck.Visible = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                pnlMain.Location = new Point(ClientSize.Width / 2 - pnlMain.Size.Width / 2, ClientSize.Height / 2 - pnlMain.Height / 2);
                pnlMain.Anchor = AnchorStyles.None;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FillTreeview()
        {
            try
            {
                DataSet ds = new DataSet();
                TreeNode parentNode;
                if (txtUsername.Text.Trim().ToUpper() == "DIGIPOS")
                    ds = Comm.fnGetData("SELECT tblCompany.CompanyID,tblCompany.CompanyCode,CompanyName FROM tblCompany, tblUsers WHERE tblCompany.CompanyID=tblUsers.CompanyID and tblCompany.ParentID=tblCompany.CompanyID ");
                else
                    ds = Comm.fnGetData("SELECT tblCompany.CompanyID,tblCompany.CompanyCode,CompanyName FROM tblCompany, tblUsers WHERE tblCompany.CompanyID=tblUsers.CompanyID and tblCompany.ParentID=tblCompany.CompanyID and LTRIM(RTRIM(UserName)) = '" + txtUsername.Text.Trim() + "' AND LTRIM(RTRIM(Password)) = '" + txtPassword.Text.Trim() + "'");
                
                tvwUserCompany.Nodes.Clear();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    parentNode = tvwUserCompany.Nodes.Add(dr["CompanyCode"].ToString(), dr["CompanyName"].ToString());
                    PopulateTreeView(Convert.ToInt32(dr["CompanyID"].ToString()), parentNode);
                }

                tvwUserCompany.ExpandAll();
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
                if (txtUsername.Text.Trim() == "DIGIPOS")
                    dtgetData = Comm.fnGetData("SELECT C.CompanyID,C.CompanyCode,REPLACE((CONVERT(VARCHAR,FyStartDate,103) + '-' + CONVERT(VARCHAR,FyEndDate,103)),'','-') as FinYear FROM tblCompany C, tblUsers U WHERE C.CompanyID=U.CompanyID AND C.ParentID = " + parentId + "").Tables[0];
                else
                    dtgetData = Comm.fnGetData("SELECT C.CompanyID,C.CompanyCode,REPLACE((CONVERT(VARCHAR,FyStartDate,103) + '-' + CONVERT(VARCHAR,FyEndDate,103)),'','-') as FinYear FROM tblCompany C, tblUsers U WHERE C.CompanyID=U.CompanyID and LTRIM(RTRIM(UserName)) = '" + txtUsername.Text.Trim() + "' AND LTRIM(RTRIM(Password)) = '" + txtPassword.Text.Trim() + "' AND C.ParentID = " + parentId + "").Tables[0];

                TreeNode childNode;
                foreach (DataRow dr in dtgetData.Rows)
                {
                    if (parentNode == null)
                    {
                        childNode = tvwUserCompany.Nodes.Add(dr["CompanyCode"].ToString(), dr["FinYear"].ToString());
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

        private void MakeViewForUserDatabase()
        { 
            try
            {
                if (Comm.fnExecuteNonQuery("DROP VIEW ViewForUserDatabase", false) >=0)
                {
                    string sQuery = "";
                    StringBuilder strBuild = new StringBuilder();
                    DataTable dtDB = Comm.fnGetData("select name from sys.Databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb'); ").Tables[0];
                    if (dtDB.Rows.Count > 0)
                    {
                        strBuild.Append("CREATE VIEW " + "ViewForUserDatabase" + " As ");
                        strBuild.AppendLine();
                        for (int i = 0; i < dtDB.Rows.Count; i++)
                        {
                            if (Comm.fnGetData("select name from " + dtDB.Rows[i]["name"].ToString() + "." + "dbo" + "." + "sysobjects WHERE name = 'tblUserMaster' AND xtype = 'U'").Tables[0].Rows.Count > 0)
                            {
                                strBuild.Append("Select '" + dtDB.Rows[i]["name"].ToString() + "' as CompanyCode,U.UserId,U.UserName,U.Pwd,U.GroupID,U.Status,G.GroupName,G.AccessLevel,G.StrCCID,G.ID FROM ");
                                strBuild.Append(dtDB.Rows[i]["name"].ToString() + "." + "dbo" + "." + "tblUserMaster U");
                                strBuild.Append(" INNER JOIN " + dtDB.Rows[i]["name"].ToString() + "." + "dbo" + "." + "tblUserGroupMaster G");
                                strBuild.Append(" ON " + "U.GroupID = G.ID");
                                strBuild.AppendLine();
                                strBuild.Append("UNION ALL");
                                strBuild.AppendLine();
                            }
                        }
                        sQuery = strBuild.ToString();
                        sQuery = sQuery.Substring(0, sQuery.Length - 11);
                    }

                    //Comm.fnExecuteNonQuery("EXEC sp_executesql '" + sQuery + "'");
                    Comm.fnExecuteNonQuery(sQuery);

                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Login",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    txtPassword.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnLogin.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBacktoLogin_Click(object sender, EventArgs e)
        {
            try
            {
                this.tblpLoginScreen.ColumnStyles[0].SizeType = SizeType.Percent;
                this.tblpLoginScreen.ColumnStyles[0].Width = 100;

                this.tblpLoginScreen.ColumnStyles[1].SizeType = SizeType.Absolute;
                this.tblpLoginScreen.ColumnStyles[1].Width = 0;

                if (pboxcheck.Visible == false)
                {
                    txtUsername.Clear();
                    txtPassword.Clear();

                    txtUsername.Focus();
                }
                else
                    txtPassword.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCompanyUserOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvwUserCompany.SelectedNode == null)
                {
                    MessageBox.Show("Please select the company.", "User Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DigiposZen.Properties.Settings.Default.server = cboPrimaryServer.Text.ToString();
                DigiposZen.Properties.Settings.Default.ConnectionString = "Data Source=" + DigiposZen.Properties.Settings.Default.server + ";Initial Catalog=" + tvwUserCompany.SelectedNode.Name + ";User ID=sa;Password=#infinitY@279";

                AppSettings.CompanyCode = tvwUserCompany.SelectedNode.Name;
                
                if (txtUsername.Text.Trim().ToUpper() == "DIGIPOS")
                {
                    Global.SetUserId(0);
                    Global.SetUserGroupId(0);
                    Global.SetUserName("DIGIPOS");
                    Global.Setpwd("");
                    Global.SetCompanyCode(tvwUserCompany.SelectedNode.Name);
                    Global.SetCompanyID(tvwUserCompany.SelectedNode.Tag.ToString());
                }
                else
                { 
                    DataTable dtUser = Comm.fnGetData("SELECT userid,username,groupid FROM tblUserMaster WHERE LTRIM(RTRIM(UserName)) = '" + txtUsername.Text.ToString() + "' AND LTRIM(RTRIM(pwd)) = '" + txtPassword.Text.ToString() + "' ").Tables[0];
                    if (dtUser.Rows.Count > 0)
                    {
                        Global.SetUserId(Convert.ToInt32(dtUser.Rows[0]["userid"]));
                        Global.SetUserGroupId(Convert.ToInt32(dtUser.Rows[0]["groupid"]));
                        Global.SetUserName(dtUser.Rows[0]["username"].ToString());
                        Global.Setpwd(txtPassword.Text.ToString());
                        Global.SetCompanyCode(tvwUserCompany.SelectedNode.Name);
                        if (tvwUserCompany.SelectedNode.Tag != null)
                            Global.SetCompanyID(tvwUserCompany.SelectedNode.Tag.ToString());
                        else
                            Global.SetCompanyID(tvwUserCompany.SelectedNode.Parent.Tag.ToString());
                    }
                    else
                    {
                        MessageBox.Show("User name and password mismatch in local database and super user database.", "User Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dtUser.Dispose();
                        return;
                    }
                    dtUser.Dispose();
                }

                SaveServerSettings();

                using (SqlConnection cn = new SqlConnection(DigiposZen.Properties.Settings.Default.ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Select FyStartDate, FyEndDate From tblCompanyMaster", cn))
                    {
                        sqlCmd.CommandType = CommandType.Text;
                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        DataTable dt = new DataTable();
                        sqlDa.Fill(dt);

                        Global.SetFyStartDate(Convert.ToDateTime(dt.Rows[0]["FyStartDate"].ToString()));
                        Global.SetFyEndDate(Convert.ToDateTime(dt.Rows[0]["FyEndDate"].ToString()));
                    }
                }

                Comm.DBUpdate();
                Comm.CreateViewsAndProcudures();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }

        private void Login_Activated(object sender, EventArgs e)
        {
            try
            {
                if (mblnOnceActivated == true) return;

                mblnOnceActivated = true;
            }
            catch
            {

            }
            try
            {
                this.tblpLoginScreen.ColumnStyles[0].SizeType = SizeType.Percent;
                this.tblpLoginScreen.ColumnStyles[0].Width = 100;

                this.tblpLoginScreen.ColumnStyles[1].SizeType = SizeType.Absolute;
                this.tblpLoginScreen.ColumnStyles[1].Width = 0;

                FillTimeZone();
                rdoComputerName.Checked = true;
                rdoUser.Checked = true;
                ShowFormsAsperClick(1);
                cboRoleofSystem.SelectedIndex = 0;

                if (Directory.Exists(Application.StartupPath + @"\Resources") == false)
                    Directory.CreateDirectory(Application.StartupPath + @"\Resources");

                string sJason = "";
                try
                {
                    sJason = File.ReadAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "LoginSettings.txt", Encoding.UTF8);
                }
                catch
                { }

                clsLoginSettings LogSett = new clsLoginSettings();

                LogSett = JsonConvert.DeserializeObject<clsLoginSettings>(sJason);

                if (LogSett != null)
                {
                    if (LogSett.ROLEOFSYSTEM != "")
                        cboRoleofSystem.SelectedIndex = Convert.ToInt32(LogSett.ROLEOFSYSTEM);

                    txtClientID.Text = LogSett.CLIENTID;

                    if (LogSett.SQLCONNCONFIG == 1)
                        rdoComputerName.Checked = true;
                    else if (LogSett.SQLCONNCONFIG == 2)
                        rdoIPAdress.Checked = true;
                    else if (LogSett.SQLCONNCONFIG == 3)
                        rdoCloudServer.Checked = true;

                    cboPrimaryServer.Text = LogSett.PRIMSERVERNAME;
                    cboSecondaryServer.Text = LogSett.SECOSERVERNAME;
                    cboTimeZone.Text = LogSett.TIMEZONE;

                    DigiposZen.Properties.Settings.Default.server = cboPrimaryServer.Text.ToString(); //fso.FileOperation(Application.StartupPath + "Resources\\Config.ini", true);
                    DigiposZen.Properties.Settings.Default.ConnectionString = "Data Source=" + cboPrimaryServer.Text.ToString() + ";Initial Catalog=DIGIPOSDEMO;User ID=sa;Password=#infinitY@279";


                    ReadWriteLoginCredentials(false);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnCloseFromSettings_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
                Environment.Exit(0);
            }
            catch
            { }
        }
    }

    public class clsLoginSettings
    {
        public string ROLEOFSYSTEM { get; set; }
        public string CLIENTID { get; set; }
        public int SQLCONNCONFIG { get; set; }
        public string PRIMSERVERNAME { get; set; }
        public string SECOSERVERNAME { get; set; }
        public string TIMEZONE { get; set; }
    }
}
