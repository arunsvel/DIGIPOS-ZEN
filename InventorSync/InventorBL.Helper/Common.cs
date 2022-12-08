using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using System.Drawing;
using System.Globalization;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO.Compression;
using InventorSync.Info;
using InventorSync.InventorBL.Master;

namespace InventorSync.InventorBL.Helper
{
    public class Common : DBConnection
    {
        public enum PermissionType
        {
            View,
            New,
            Edit,
            Archive,
            Delete,
            Print,
            DateChange
        }

        public enum UserActivity
        { 
            new_Entry = 1,
            UpdateEntry = 2,
            Delete_Entry = 3,
            CancelEntry = 4,
            DisplayWindow = 5,
            Printinvoice = 6,
            DateChange = 7,
            WaitForAuthorisation = 8,
            LoggedIn = 9,
            Loggedout = 10
        }

        public bool CheckUserPermission(UserActivity Activity, string WindowCaption, bool BlnSupressMessage = false, string CustomAccessString = "")
        {
            // If Exists in string=It is to be rejected
            // User Actions exists in AccessString = This action is to rejected for this user
            // CheckUserPermission = True
            // Exit Function

            if (Global.gblUserID == 1 | Global.gblUserID == 0)
            {
                return true;
            }

            if (Global.gblUserGroupID == 1)
            {
                return true;
            }

            //clsfeaturecontrol FC = new clsfeaturecontrol();

            Common Comm = new Common();

            DataTable dt = Comm.fnGetData("Select AccessLevel From tblUserGroupMaster Where ID=" + Global.gblUserGroupID.ToString()).Tables[0];

            string MyMstrAccessString = "";

            if (dt.Rows.Count > 0)
                MyMstrAccessString = dt.Rows[0]["AccessLevel"].ToString();

            if (Strings.Left(MyMstrAccessString, 1) != "Ü")
                MyMstrAccessString = "^" + MyMstrAccessString.ToUpper();

            MyMstrAccessString = Strings.Replace(MyMstrAccessString.ToUpper(), "Ü", "^");

            if (Strings.Trim(CustomAccessString) != "")
                MyMstrAccessString = Strings.Replace(Strings.UCase(CustomAccessString), "Ü", "^");

            recheckrights:
            if (Activity == UserActivity.new_Entry || Activity == UserActivity.UpdateEntry || Activity == UserActivity.Delete_Entry || Activity == UserActivity.CancelEntry)
            {
            }

            if (WindowCaption == "0Key")
            {
                return true;
            }
            if (Strings.Trim(CustomAccessString) == "")
            {
                if ((Global.gblUserName.ToString() == "ADMIN" || Global.gblUserName.ToString() == "DIGIPOS"))
                {
                    return true;
                }
            }

            if (Strings.Left(MyMstrAccessString, 1) != "><")
                MyMstrAccessString = "><" + Strings.UCase(MyMstrAccessString);
            MyMstrAccessString = Strings.UCase(MyMstrAccessString);
            WindowCaption = Strings.UCase(WindowCaption);
            if (Strings.InStr(1, MyMstrAccessString, "><" + WindowCaption + "|") == 0)
            {
                if (BlnSupressMessage == false)
                    Comm.MessageboxToasted("User Permission Module", "User Permission denied for all Activities.");

                return false;
            }

            string Stractivity;
            string[] SPLITSTR;
            SPLITSTR = Strings.Split(Strings.UCase(MyMstrAccessString), "><" + WindowCaption + "|");
            Stractivity = "|" + Strings.Left(SPLITSTR[1], Strings.InStr(1, SPLITSTR[1], "><") - 1);


            if (Activity == UserActivity.Delete_Entry)
            {
                if (Strings.InStr(1, Stractivity, "|D") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for Deletion.", Constants.vbCritical);
                    return false;
                }
            }
            else if (Activity == UserActivity.CancelEntry)
            {
                if (Strings.InStr(1, Stractivity, "|C") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for Cancellation.", Constants.vbCritical);
                    return false;
                }
            }
            else if (Activity == UserActivity.UpdateEntry)
            {
                if (Strings.InStr(1, Stractivity, "|E") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for Updation.", Constants.vbCritical);
                    return false;
                }
            }
            else if (Activity == UserActivity.new_Entry)
            {
                if (Strings.InStr(1, Stractivity, "|N") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for Creating New.", Constants.vbCritical);
                    return false;
                }

                if (Global.blnTrialExpired)
                {
                    Interaction.MsgBox("30 day Trial Period Expired. you can't create new entries", Constants.vbCritical, "Permission");
                    return false;
                }
            }
            else if (Activity == UserActivity.DateChange)
            {
                if (Strings.InStr(1, Stractivity, "|A") > 0)
                    return false;
            }
            else if (Activity == UserActivity.Printinvoice)
            {
                if (Strings.InStr(1, Stractivity, "|P") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for printing.", Constants.vbCritical);
                    return false;
                }
            }
            else if (Activity == UserActivity.WaitForAuthorisation)
            {
                if (Strings.InStr(1, Stractivity, "|W") > 0)
                    return false;
            }
            else if (Activity == UserActivity.DisplayWindow)
            {
                if (Strings.InStr(1, Stractivity, "|V") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for Updation.", Constants.vbCritical);
                    return false;
                }
            }
            
            return true;

        }

        public void writeuserlog(UserActivity useractivity, string NewData, string OldData, string ActionDescription, int VchTypeId, int ParentVchTypeId, string UniqueField, int RefID, string WindowName)
        {
            try
            {
                string StrAction = "";
                sqlControl cn = new sqlControl();


                switch (useractivity)
                {
                    case UserActivity.new_Entry:
                        {
                            StrAction = "Insert";
                            break;
                        }

                    case UserActivity.UpdateEntry:
                        {
                            StrAction = "Update";
                            break;
                        }

                    case UserActivity.Delete_Entry:
                        {
                            StrAction = "Delete";
                            break;
                        }

                    case UserActivity.CancelEntry:
                        {
                            StrAction = "Cancel";
                            break;
                        }

                    case UserActivity.DisplayWindow:
                        {
                            StrAction = "Dislpay";
                            break;
                        }

                    case UserActivity.Printinvoice:
                        {
                            StrAction = "Print";
                            break;
                        }

                    case UserActivity.DateChange:
                        {
                            StrAction = "DateChanged";
                            break;
                        }

                    case UserActivity.WaitForAuthorisation:
                        {
                            StrAction = "Authorisation";
                            break;
                        }

                    case UserActivity.LoggedIn:
                        {
                            StrAction = "LoggedIn";
                            break;
                        }

                    case UserActivity.Loggedout:
                        {
                            StrAction = "Loggedout";
                            break;
                        }
                }

                cn.Execute(" exec dbo.fnInsertUserLog  '" + NewData.Replace("'", "''") + "','" + OldData.Replace("'", "''") + "','" + StrAction + "','" + ActionDescription.Replace("'", "''") + "', " + VchTypeId + " ," + ParentVchTypeId + ", '" + UniqueField + "' , " + RefID + ", " + Global.gblUserID + " ,'" + Global.ComputerName.Replace("'", "''") + "','" + WindowName.Replace("'", "''") + "'");
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "WrituserLog");
            }
        }

        //public bool CheckUserPermission(string WindowName = "", PermissionType Permission = PermissionType.View)
        //{
        //    try 
        //    {
        //        UspGetUserGroupMasterInfo GetuserInfo = new UspGetUserGroupMasterInfo();
        //        clsUserGroup clsuser = new clsUserGroup();

        //        sqlControl rs = new sqlControl();

        //        String strAccessLevel = "";

        //        DataTable dtLoad = new DataTable();
        //        GetuserInfo.GroupID = Convert.ToDecimal(Global.gblUserGroupID);
        //        GetuserInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
        //        dtLoad = clsuser.GetUserGroupMaster(GetuserInfo);
        //        if (dtLoad.Rows.Count > 0)
        //        {
        //            strAccessLevel = dtLoad.Rows[0]["AccessLevel"].ToString();
        //        }
        //        return true;
        //    }

        //    catch 
        //    {
        //        return false;
        //    }
        //}

        public bool Transferdatabase(sqlControl cn, string SRCDbName, string DestDbName, string DestDbpath)
        {
            try
            {
            string strQuery;
            strQuery = "password=#infinitY@279;User ID=sa;Initial Catalog=Startup;Data Source=" + Global.SqlServerName;
            sqlControl cnn = new sqlControl(strQuery);

            // =======================Default path===========
            if (DestDbpath == "")
            {
                string Sql;
                string SqlFolderName;
                SqlFolderName = Global.SqlServerName.Replace(@"\", "" + @"\");
                SqlFolderName = Strings.Replace(SqlFolderName, ".", "");
                SqlFolderName = Strings.Replace(SqlFolderName, ",", "");

                DestDbpath = @"C:\DIGIDATA\Data\";
            }
            // =======================Default path===========
            // Start Main script
            sqlControl Rs = new sqlControl();
            string MDFLOgicalName = "";
            string LDFLOgicalName = "";
            string SRCMDFLOgicalName = "";
            string SRCLDFLOgicalName = "";
            string SRCPath = "";
            DestDbpath = Strings.Replace(DestDbpath, ".", "");
            DestDbpath = Strings.Replace(DestDbpath, ",", "");
            MDFLOgicalName = DestDbpath + DestDbName + "_DAT.mdf";
            LDFLOgicalName = DestDbpath + DestDbName + "_LOG.ldf";
            Rs = null/* TODO Change to default(_) if this is not a reference type */;
            cnn.Open("  SELECT     name, filename From sys.sysdatabases where Name='" + SRCDbName + "'");
            // getting physical file Name
            if (!cnn.eof())
            {
                SRCPath = cnn.fields("FileName");
                SRCPath = Strings.Left(SRCPath, Strings.InStrRev(SRCPath, @"\"));
            }

            cn.Execute("BACKUP DATABASE [" + SRCDbName + @"] TO  DISK = N'C:\DIGIDATA\Data\" + SRCDbName + ".BAK' WITH NOFORMAT, INIT,  NAME = N'" + SRCDbName + "-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 30");
            // CN.Execute " CREATE DATABASE " & DestDbName
            Rs = null/* TODO Change to default(_) if this is not a reference type */;
            cnn.Open(@" Restore filelistonly FROM         disk = 'C:\DIGIDATA\Data\" + SRCDbName + ".BAK'");
            if (!cnn.eof())
            {
                SRCMDFLOgicalName = cnn.fields("LogicalName");
                cnn.MoveNext();
                SRCLDFLOgicalName = cnn.fields("LogicalName");
            }
            cn.Execute(" RESTORE DATABASE [" + DestDbName + @"] FROM  DISK = N'C:\DIGIDATA\Data\" + SRCDbName + ".BAK' WITH  FILE = 1, " + " MOVE N'" + SRCMDFLOgicalName + "' TO N'" + MDFLOgicalName + "'," + " MOVE N'" + SRCLDFLOgicalName + "' TO N'" + LDFLOgicalName + "',  NOUNLOAD,  REPLACE,  STATS = 10");

            // getting physical file Name
            cn.Execute(" ALTER DATABASE [" + DestDbName + "] MODIFY FILE (NAME=N'" + SRCMDFLOgicalName + "', NEWNAME=N'" + DestDbName + "_DAT')  ");
            cn.Execute(" ALTER DATABASE [" + DestDbName + "] MODIFY FILE (NAME=N'" + SRCLDFLOgicalName + "', NEWNAME=N'" + DestDbName + "_Log') ");


            return true;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return false;
            }
        }

        private bool query(string que)
        {
            try
            {
                SqlConnection con = GetDBConnection();
                SqlCommand cmd = new SqlCommand(que, con);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return false;
            }
        }

        public bool CreateZipFile(string FilePath, string FileName, string ZipFilePath, string ZipFileName)
        {
            try
            {
                if (File.Exists(FilePath + FileName) == true)
                {
                    ZipFileName = FilePath + ZipFileName;

                    if (ZipFileName == "")
                        return false;
                    ZipFile.CreateFromDirectory(FilePath + FileName, ZipFileName);
                    File.Copy(ZipFileName, ZipFilePath + ZipFileName);
                    File.Delete(ZipFileName);
                    File.Delete(FilePath + FileName);
                }

                return true;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return false;
            }
        }

        public bool BACKUPDB(string str, string BackupCompany = "", string BackupPath = "")
        {
            try
            {
                if (BackupCompany == "")
                {
                    Interaction.MsgBox("Select a company to backup", MsgBoxStyle.Information);
                    return false;
                }
                if (BackupPath == "")
                {
                    Interaction.MsgBox("Select a filename to backup", MsgBoxStyle.Information);
                    return false;
                }
                if (File.Exists(BackupPath) == true)
                {
                    Interaction.MsgBox("File already exists. Please choose a new file to backup.", MsgBoxStyle.Information);
                    return false;
                }
                // MsgBox("1")

                if (Directory.Exists(@"C:\SQLBK\" + BackupCompany) == true)
                    Directory.Delete(@"C:\SQLBK\" + BackupCompany);
                // MsgBox("2")
                if (Directory.Exists(@"C:\SQLBK\" + BackupCompany) == false)
                    Directory.CreateDirectory(@"C:\SQLBK\" + BackupCompany);
                // MsgBox("3")
                if (Directory.Exists(@"C:\SQLBK\" + BackupCompany) == false)
                {
                    Interaction.MsgBox("Path not found. Could not create temporary file or directory for backup creation.");
                    return false;
                }

                bool blnFailed = false;
                if (File.Exists(@"C:\SQLBK\" + BackupCompany + @"\" + BackupCompany + ".bak") == true)
                    File.Delete(@"C:\SQLBK\" + BackupCompany + @"\" + BackupCompany + ".bak");

                if (query("backup database " + BackupCompany + " to disk='" + @"C:\SQLBK\" + BackupCompany + @"\" + BackupCompany + ".bak'") == true)
                {
                    if (CreateZipFile(@"C:\SQLBK\" + BackupCompany + @"\" , BackupCompany + ".bak'", BackupPath, BackupCompany + DateTime.Now.ToString("ddMMMyyyy_hh_mm_ss_tt") + ".zip'") == false)
                    {
                        Interaction.MsgBox("Failed to backup database. Could not create file to " + BackupPath);
                        blnFailed = true;
                    }
                    else
                    {
                        Interaction.MsgBox("Backup process completed successfully. File copied to " + BackupPath, MsgBoxStyle.Information);
                        blnFailed = false;
                    }
                }
                else
                {
                    Interaction.MsgBox("Failed to backup database. Backup process aboted abnormally.");
                    blnFailed = true;
                }

                if (blnFailed)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return false;
            }
        }

        public void RESTOREDB(string bDNAME, string SRCDbPATH, string DestDbPath)
        {
            try
            {
                string Cnstring;
                string mPrimaryServer = Global.SqlServerName;

                Cnstring = "Data Source=" + mPrimaryServer + ";Initial Catalog=master;User ID=sa;Password=#infinitY@279";
                sqlControl cnn = new sqlControl(Cnstring);

                string SqlFolderName;
                if (DestDbPath == "")
                {
                    SqlFolderName = mPrimaryServer.Replace(@"\", "") + @"\";
                    if (Directory.Exists(@"C:\DIGIDATA\Data\" + SqlFolderName) == false)
                        Directory.CreateDirectory(@"C:\DIGIDATA\Data\" + SqlFolderName);
                    DestDbPath = @"C:\DIGIDATA\Data\" + SqlFolderName;
                }
                string MDFLogicalName;
                string LDFLogicalName;

                cnn.Open(" Restore  filelistonly from  disk = '" + SRCDbPATH + "'");
                if (!cnn.eof())
                {
                    MDFLogicalName = cnn.fields("logicalName");
                    cnn.MoveNext();
                    LDFLogicalName = cnn.fields("logicalName");
                    cnn.Execute("RESTORE DATABASE [" + bDNAME + "] from DISK =N'" + SRCDbPATH + "' WITH FILE = 1, MOVE N'" + MDFLogicalName + "' TO N'" + DestDbPath + MDFLogicalName + ".DAT', " + " MOVE N'" + LDFLogicalName + "' TO N'" + DestDbPath + LDFLogicalName + ".ldf', NOUNLOAD, REPLACE,STATS = 10");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("First Run " + ex.Message, Global.gblMessageCaption);
            }
        }

        public void ResizeForm(Form frm)
        {
            frm.Width = 1000;
            frm.Height = 1000;
        }

        public decimal ToDecimal(string Number)
        {
            try
            {
                if (Number == null) return 0;

                decimal ParsedNumber = 0;

                bool canConvert = decimal.TryParse(Number, out ParsedNumber);
                if (canConvert == true)
                    return ParsedNumber;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }
        public decimal ToDecimal(object Number)
        {
            try
            {
                if (Number == null) return 0;

                decimal ParsedNumber = 0;

                bool canConvert = decimal.TryParse(Number.ToString(), out ParsedNumber);
                if (canConvert == true)
                    return ParsedNumber;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public double ToDouble(string Number)
        {
            try
            {
                if (Number == null) return 0;

                double ParsedNumber = 0;

                bool canConvert = double.TryParse(Number, out ParsedNumber);
                if (canConvert == true)
                    return ParsedNumber;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }
        public double ToDouble(object Number)
        {
            try
            {
                if (Number == null) return 0;

                double ParsedNumber = 0;

                bool canConvert = double.TryParse(Number.ToString(), out ParsedNumber);
                if (canConvert == true)
                    return ParsedNumber;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public Int32 ToInt32(string Number)
        {
            try
            {
                if (Number == null) return 0;
                if (Number == "") return 0;

                Int32 ParsedNumber = 0;

                if (Number.ToString().Contains("."))
                {
                    return decimal.ToInt32(Convert.ToDecimal(Number.ToString()));
                }
                else
                {
                    bool canConvert = Int32.TryParse(Number, out ParsedNumber);
                    if (canConvert == true)
                        return ParsedNumber;
                    else
                        return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
        public Int32 ToInt32(object Number)
        {
            try
            {
                if (Number == null) return 0;

                Int32 ParsedNumber = 0;

                if (Number.ToString().Contains("."))
                {
                    return decimal.ToInt32(Convert.ToDecimal(Number.ToString()));
                }
                else
                { 
                    bool canConvert = Int32.TryParse(Number.ToString(), out ParsedNumber);
                    if (canConvert == true)
                        return ParsedNumber;
                    else
                        return 0;
                } 
            }
            catch
            {
                return 0;
            }
        }



        //Description : Format the Amount using Supplied Values
        public string FormatAmt(double myValue, string myFormat)
        {
            //https://msdn.microsoft.com/en-us/library/microsoft.visualbasic.strings.format(v=vs.110).aspx
            //DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss")
            //"29-Jan-2018 10:16:16"
            //FormatAmt = String.Format("{0:N3}", Val(myValue))
            //FormatAmt = Format(Val(myValue), "f" & DCSApp.Gdecimal.ToString & "")

            if (myFormat == "")
                myFormat = "#.00";
            return ToDouble(myValue).ToString(myFormat);
        }
        public string FormatAmt(decimal myValue, string myFormat)
        {
            //https://msdn.microsoft.com/en-us/library/microsoft.visualbasic.strings.format(v=vs.110).aspx
            //DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss")
            //"29-Jan-2018 10:16:16"
            //FormatAmt = String.Format("{0:N3}", Val(myValue))
            //FormatAmt = Format(Val(myValue), "f" & DCSApp.Gdecimal.ToString & "")

            if (myFormat == "")
                myFormat = "#.00";
            return ToDecimal(myValue).ToString(myFormat);
        }

        //Description : Format Values like Currency/Quantity to the Formated Values asper App Settings
        public string FormatValue(double myValue, bool blnIsCurrency = true, string sMyFormat = "")
        {
            string myFormat = "";
            if (blnIsCurrency == true)
                myFormat = AppSettings.CurrDecimalFormat;
            else
                myFormat = AppSettings.QtyDecimalFormat;

            if (myFormat == "")
                myFormat = "#.00";

            if (sMyFormat != "")
                myFormat = sMyFormat;

            return ToDouble(myValue).ToString(myFormat);
        }
        public string FormatValue(decimal myValue, bool blnIsCurrency = true, string sMyFormat = "")
        {
            string myFormat = "";
            if (blnIsCurrency == true)
                myFormat = AppSettings.CurrDecimalFormat;
            else
                myFormat = AppSettings.QtyDecimalFormat;

            if (myFormat == "")
                myFormat = "#.00";

            if (sMyFormat != "")
                myFormat = sMyFormat;

            return ToDecimal(myValue).ToString(myFormat);
        }

        //Description : Convert to Int32 of Decimal Value
        public int ConvertI32(decimal dVal)
        {
            return Convert.ToInt32(dVal);
        }

        public bool CheckNumeric(object sender, KeyPressEventArgs e, bool NegativeAllowed = false, bool IsLeave = false)
        {
            try
            {
                if (IsLeave == true)
                {
                    string stringNumber = ((TextBox)sender).Text.ToString();
                    bool isNumber = int.TryParse(stringNumber, out _);
                    return isNumber;
                }
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
                {
                    return true;
                }
                if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                {
                    return true;
                }
                if (NegativeAllowed == true)
                {
                    if ((e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static Image ScaleByPercent(Image imgPhoto, int Percent)
        {
            float nPercent = ((float)Percent / 100);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight,
                                     PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                    imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public void LoadBGImage(Form frm, PictureBox picBackground)
        {
            picBackground.WaitOnLoad = false;
            picBackground.Size = frm.Size;
            picBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            picBackground.LoadAsync(Application.StartupPath + @"\Resources\WallPaper2.jpeg");
        }

        public bool TransparentControls(Control parentctrl)
        {
            try
            {
                foreach (Control ctrl in parentctrl.Controls)
                {
                    if (ctrl.GetType() != typeof(Form))
                    {
                        if (GetControlStyle(ctrl, ControlStyles.SupportsTransparentBackColor) == true)
                        {
                            ctrl.BackColor = Color.Transparent;
                            if (ctrl.Controls.Count > 0)
                            {
                                TransparentControls(ctrl);
                            }
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SetControlColours(Control parentctrl)
        {
            try
            {
                foreach (Control ctrl in parentctrl.Controls)
                {
                    if (ctrl.GetType() == typeof(Label))
                    {
                        ctrl.ForeColor = Color.Black;
                    }
                    if (ctrl.Controls.Count > 0)
                    {
                        SetControlColours(ctrl);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GetControlStyle(Control control, ControlStyles flags)
        {
            Type type = control.GetType();
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo method = type.GetMethod("GetStyle", bindingFlags);
            object[] param = { flags };
            return (bool)method.Invoke(control, param);
        }

        public int gfnGetNextSerialNo(string sTableName, string sColumnName, string sCondition = "")
        {
            // --------------------------------------------------------- >>
            // Description: gfnGetNextSerialNo, is to get next serial number using the tablename, columnname and if have any conditions.
            // Created By : Dipu Joseph
            // Create On  : 03-09-2021
            // Last Edited By :
            // Last Edited On :
            // --------------------------------------------------------- >>

            using (var sqlConn = GetDBConnection())
            {
                string sQuery = "";
                int iResult = 1;

                sQuery = "SELECT ISNULL(MAX(" + sColumnName + "), 0) + 1 FROM " + sTableName;
                if (sCondition != "")
                {
                    sQuery = sQuery + " WHERE " + sCondition;
                }

                SqlDataAdapter daSerial = new SqlDataAdapter(sQuery, sqlConn);
                DataTable dtSerial = new DataTable();
                daSerial.Fill(dtSerial);
                if (dtSerial.Rows.Count > 0)
                {
                    iResult = Convert.ToInt32(dtSerial.Rows[0][0].ToString());
                }
                sqlConn.Close();
                return iResult;
            }
        }

        public void WritetoErrorLog(Exception ex, string sEventTarget)
        {
            // --------------------------------------------------------- >>
            // Description: WritetoErrorLog, is to write the error logs from forms and project.
            // Created By : Dipu Joseph
            // Create On  : 03-09-2021
            // Last Edited By :
            // Last Edited On :
            // --------------------------------------------------------- >>

            try
            {
                string sDate = DateTime.Today.ToString("dd-MMM-yyyy");
                string sFolderName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\LogError";

                if (!Directory.Exists(sFolderName))
                    Directory.CreateDirectory(sFolderName);

                string sErrFileName = sFolderName + "\\" + "iError_" + sDate + ".txt";
                if (File.Exists(sErrFileName) == false)
                {
                    File.Create(sErrFileName);
                }

                using (StreamWriter sw = File.AppendText(sErrFileName))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine("EventTarget:" + sEventTarget);
                    sw.WriteLine(ex.Message);
                    sw.WriteLine(Convert.ToString(ex.InnerException));
                    sw.WriteLine(ex.Source);
                    sw.WriteLine("User Name: " + "Administrator");
                    sw.WriteLine("------------------------------------------------------------------------------");
                }
            }
            catch (Exception)
            {
            }
        }

        public void WritetoSqlErrorLog(DataTable dtErrorResult, string sUsername)
        {
            // --------------------------------------------------------- >>
            // Description: WritetoSqlErrorLog, is to write the error logs from sql stored procedures
            // Created By : Dipu Joseph
            // Create On  : 03-09-2021
            // Last Edited By :
            // Last Edited On :
            // --------------------------------------------------------- >>

            try
            {
                string sDate = DateTime.Today.ToString("dd-MMM-yyyy");

                string sErrFileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\LogError" + "\\" + "SqlError_" + sDate + ".txt";
                if (File.Exists(sErrFileName) == false)
                {
                    File.Create(sErrFileName);
                }

                using (StreamWriter sw = File.AppendText(sErrFileName))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine("ERROR_NUMBER:" + dtErrorResult.Rows[0]["ErrorNumber"].ToString());
                    sw.WriteLine("ERROR_STATE:" + dtErrorResult.Rows[0]["ErrorState"].ToString());
                    sw.WriteLine("ERROR_SEVERITY:" + dtErrorResult.Rows[0]["ErrorSeverity"].ToString());
                    sw.WriteLine("ERROR_PROCEDURE:" + dtErrorResult.Rows[0]["ErrorProcedure"].ToString());
                    sw.WriteLine("ERROR_LINE:" + dtErrorResult.Rows[0]["ErrorLine"].ToString());
                    sw.WriteLine("ERROR_MESSAGE:" + dtErrorResult.Rows[0]["ErrorMessage"].ToString());
                    sw.WriteLine("User Name: " + sUsername);
                    sw.WriteLine("------------------------------------------------------------------------------");
                }
            }
            catch (Exception)
            {
            }
        }

        public DataSet fnGetData(string sQuery)
        {
            // --------------------------------------------------------- >>
            // Description: fnGetData, is to get data from database using sql script (query/procedure)
            // Created By : Dipu Joseph
            // Create On  : 03-09-2021
            // Last Edited By :
            // Last Edited On :
            // --------------------------------------------------------- >>
            try
            {
                SqlConnection sqlconn = GetDBConnection();
                //MessageBox.Show(sqlconn.ConnectionString);
                try
                {
                    //sqlconn.Open();
                    string sStr = sQuery;
                    SqlDataAdapter sqlda = new SqlDataAdapter(sStr, sqlconn);
                    DataSet ds = new DataSet();
                    sqlda.Fill(ds);
                    sqlda.Dispose();
                    return ds;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (sqlconn != null)
                        sqlconn.Close();
                    return new DataSet();
                }
                finally
                {
                    if (sqlconn != null)
                        sqlconn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataSet();
            }
        }

        public int fnExecuteNonQuery(string sQuery, bool blnShowErrorMessage = true, SqlConnection sqlconn = null, SqlTransaction trans = null)
        {
            int iRet = 0;
            if (sqlconn == null)
                sqlconn = GetDBConnection();

            try
            {
                //sqlconn.Open();
                string sStr = sQuery;

                SqlCommand sqlCmd;

                if (trans == null)
                    sqlCmd = new SqlCommand(sStr, sqlconn);
                else
                    sqlCmd = new SqlCommand(sStr, sqlconn, trans);

                iRet = sqlCmd.ExecuteNonQuery();

                if (trans == null)
                    sqlconn.Close();

                return iRet;
            }
            catch (Exception ex)
            {
                if (sqlconn != null)
                    sqlconn.Close();

                if (blnShowErrorMessage == true)
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

                return -1;
            }
            finally
            {
                if (trans == null)
                {
                    if (sqlconn != null)
                        sqlconn.Close();
                }
            }
        }

        public void LoadGrdiControl(Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl ctl, DataTable dtLoad, bool bShowFilterbar = false, bool bAllowEdit = false, string sColWidth = "",int isetfrmwidth=0)
        {
            int iTotGridWidth = 0, idtColCount = 0, iGridColWidth = 0, iSerilBalWidth = 0;
            Syncfusion.GridHelperClasses.GridDynamicFilter filter = new Syncfusion.GridHelperClasses.GridDynamicFilter();


            ctl.DataSource = null;
            if (dtLoad.Rows.Count > 0)
            {
                ctl.DataSource = dtLoad;
                ctl.Refresh();

                ctl.TopLevelGroupOptions.ShowAddNewRecordBeforeDetails = false;
                ctl.TopLevelGroupOptions.ShowCaption = false;
                ctl.NestedTableGroupOptions.ShowAddNewRecordBeforeDetails = false;
                ctl.TableModel.EnableLegacyStyle = false;
                ctl.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.Office2016White;
                ctl.TableOptions.ListBoxSelectionMode = SelectionMode.MultiExtended;
                ctl.TableControl.DpiAware = true;
                ctl.WantTabKey = false;

                // Settings
                ctl.TopLevelGroupOptions.ShowFilterBar = bShowFilterbar;
                if (bAllowEdit == false)
                    ctl.ActivateCurrentCellBehavior = GridCellActivateAction.None;
                else
                    ctl.ActivateCurrentCellBehavior = GridCellActivateAction.ClickOnCell;

                iTotGridWidth = ctl.Width;
                idtColCount = dtLoad.Columns.Count;
                iGridColWidth = iTotGridWidth / (dtLoad.Columns.Count - 1);

                //Added by Anjitha 14/03/2022 5:33 PM
                if (sColWidth != "")
                {
                    SetGridColumnWidth(ctl, iTotGridWidth, sColWidth, bShowFilterbar, isetfrmwidth);
                }
                else
                {
                    for (int i = 0; i < ctl.TableDescriptor.Columns.Count; i++)
                    {
                        if (i == 0)
                            ctl.TableDescriptor.Columns[i].Width = 0;
                        else
                        {
                            if (ctl.TableDescriptor.Columns[i].HeaderText == "Serial No")
                            {
                                ctl.TableDescriptor.Columns[i].Width = iGridColWidth / 2;
                                iSerilBalWidth = iGridColWidth / 2;
                                ctl.TableDescriptor.Columns[i].Appearance.AnyRecordFieldCell.HorizontalAlignment = Syncfusion.Windows.Forms.Grid.GridHorizontalAlignment.Center;
                            }
                            else if (i == ctl.TableDescriptor.Columns.Count - 1)
                            {
                                ctl.TableDescriptor.Columns[i].Width = iGridColWidth - 20;
                            }
                            else
                            {
                                ctl.TableDescriptor.Columns[i].Width = iGridColWidth + iSerilBalWidth;
                                iSerilBalWidth = 0;
                            }
                            filter.WireGrid(ctl);
                            ctl.TableDescriptor.Columns[i].AllowFilter = bShowFilterbar;
                            //}
                        }
                    }
                }

                Syncfusion.Grouping.Record rec = ctl.Table.Records[0];
                ctl.TableModel.Selections.Clear();

                ctl.TableModel.Selections.Add(GridRangeInfo.Row(rec.GetRowIndex()));
                int rowIndex = ctl.Table.DisplayElements.IndexOf(rec);
                ctl.TableControl.CurrentCell.MoveTo(rowIndex, 1, GridSetCurrentCellOptions.ScrollInView);
            }
            ctl.Refresh();
        }

        private void SetGridColumnWidth(Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl ctrl, int iTotalGridwidt, string sColumnGridWidth = "", bool bShowFilterbar = false,int isetfrmwidth=0)
        {
            try
            {
                string[] sArrColWidth;
                int[] iArrayWidth;
                int iColWidthTot = 0;
                Syncfusion.GridHelperClasses.GridDynamicFilter filter = new Syncfusion.GridHelperClasses.GridDynamicFilter();
                //if (iTotalGridwidt == 840)
                //    iTotalGridwidt = 1306;
                if (iTotalGridwidt < 1000)
                    iTotalGridwidt = isetfrmwidth - 230;
                if (sColumnGridWidth != "")
                {
                    sArrColWidth = sColumnGridWidth.Split(',');
                    iArrayWidth = Array.ConvertAll(sArrColWidth, s => int.Parse(s));
                    int isum = iArrayWidth.Sum();
                    iColWidthTot = isum + 1;

                    for (int j = 0; j < sArrColWidth.Length; j++)
                    {
                        if (sArrColWidth[j].ToString() == "-1")
                        {
                            ctrl.TableDescriptor.Columns[j].Width = (iTotalGridwidt - iColWidthTot) - 5;
                        }
                        else
                        {
                            ctrl.TableDescriptor.Columns[j].Width = Convert.ToInt32(sArrColWidth[j].ToString());
                        }

                        ctrl.TableDescriptor.Columns[j].AllowFilter = bShowFilterbar;
                        filter.WireGrid(ctrl);
                    }
                    if (iColWidthTot < iTotalGridwidt)
                        ctrl.TableControl.HScrollBehavior = Syncfusion.Windows.Forms.Grid.GridScrollbarMode.Disabled;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "SetGridColumn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void LoadComboboxControl(ComboBox ctl, string sTableName, string sNameField, string sCodeField, string sDummy = "", string sCondition = "", string sOrderField = "", string sSortBy = "")
        {
            SqlConnection sqlConn = GetDBConnection();
            sqlConn.Open();
            string sQuery = "";

            if (sDummy != "")
            {
                sQuery = "SELECT 0 as  " + sCodeField + ",'" + sDummy + "' as " + sNameField + " FROM " + sTableName + " UNION ";
            }

            sQuery = sQuery + "SELECT DISTINCT " + sCodeField + " as " + sCodeField + "," + sNameField + " as " + sNameField + " FROM " + sTableName;
            if (sCondition != "")
            {
                sQuery = sQuery + " WHERE " + sCondition;
            }

            if (sOrderField != "")
            {
                sQuery = sQuery + " ORDER BY " + sOrderField;
                if (sSortBy != "")
                {
                    sQuery = sQuery + " " + sSortBy.ToUpper();
                }
            }

            SqlDataAdapter daPop = new SqlDataAdapter(sQuery, sqlConn);
            DataTable dtPop = new DataTable();
            daPop.Fill(dtPop);
            sqlConn.Close();

            ctl.DataSource = null;
            if (dtPop.Rows.Count > 0)
            {
                ctl.DataSource = dtPop;
                ctl.DisplayMember = sNameField;
                ctl.ValueMember = sCodeField;
            }
        }

        public string GetCheckedData(CheckedListBox ctl)
        {
            try
            {
                string returnvalue = "";

                foreach (var obj in ctl.CheckedItems)
                {
                    DataRowView castedItem = obj as DataRowView;
                    string comapnyName = castedItem["CompanyName"].ToString();
                    string id = castedItem["ID"].ToString();

                    returnvalue += id + ", ";
                }

                return returnvalue;
            }
            catch(Exception ex)
            {
                WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
        }

        public void LoadControl(object ctl, DataTable dtLoad, string Query = "", bool blnMultiSelect = false, bool blnAllowSelectAll = false, string sDisplayField = "", string sValueField = "", bool blnMultiColumn = true, bool btnHideValueMember = false, bool blnProgress = false, bool BlnAppendToCurrent = false, bool BlnAutoComplete = true, bool BlnFastFill = false, bool bShowGridFilterbar = false, bool bAllowGridEdit = false)
        {
            // --------------------------------------------------------- >>
            // Description: LoadControl, is for load or fill the control in a form from database
            // Created By : Dipu Joseph
            // Create On  : 13-09-2021
            // Last Edited By :
            // Last Edited On :
            // --------------------------------------------------------- >>

            try
            {
                DataTable dtLoadQuery = new DataTable();
                Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl CtlGridGroupControl = new Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl();
                Syncfusion.WinForms.ListView.SfComboBox CtlSFComboBox = new Syncfusion.WinForms.ListView.SfComboBox();
                Syncfusion.Windows.Forms.Tools.ComboDropDown cboddl = new Syncfusion.Windows.Forms.Tools.ComboDropDown();
                Syncfusion.Windows.Forms.Tools.MultiColumnComboBox multcbo = new Syncfusion.Windows.Forms.Tools.MultiColumnComboBox();

                ComboBox cbo = new ComboBox();

                int iTotGridWidth = 0, idtColCount = 0, iGridColWidth = 0, iSerilBalWidth = 0;
                Syncfusion.GridHelperClasses.GridDynamicFilter filter = new Syncfusion.GridHelperClasses.GridDynamicFilter();

                if (Query == "")
                    dtLoadQuery = dtLoad;
                else
                    dtLoadQuery = fnGetData(Query).Tables[0];

                if (ctl.GetType().ToString().ToUpper().Contains("GRIDGROUPINGCONTROL") == true)
                    CtlGridGroupControl = (Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl)ctl;
                else if (ctl.GetType().ToString().ToUpper().Contains("SFCOMBOBOX") == true)
                    CtlSFComboBox = (Syncfusion.WinForms.ListView.SfComboBox)ctl;
                else if (ctl.GetType().FullName.ToString().ToUpper() == "SYNCFUSION.WINDOWS.FORMS.TOOLS.MULTICOLUMNCOMBOBOX")
                    multcbo = (Syncfusion.Windows.Forms.Tools.MultiColumnComboBox)ctl;
                else if (ctl.GetType().FullName.ToString().ToUpper() == "SYSTEM.WINDOWS.FORMS.COMBOBOX")
                    cbo = (ComboBox)ctl;

                if (BlnAppendToCurrent == false)
                {
                    if (ctl.GetType().ToString().ToUpper().Contains("GRIDGROUPINGCONTROL") == true)
                        CtlGridGroupControl.DataSource = null;
                    else if (ctl.GetType().ToString().ToUpper().Contains("SFCOMBOBOX") == true)
                        CtlSFComboBox.DataSource = null;
                    else if (ctl.GetType().FullName.ToString().ToUpper() == "SYNCFUSION.WINDOWS.FORMS.TOOLS.MULTICOLUMNCOMBOBOX")
                        multcbo.DataSource = null;
                    else if (ctl.GetType().FullName.ToString().ToUpper() == "SYSTEM.WINDOWS.FORMS.COMBOBOX")
                        cbo.DataSource = null;

                }

                if (dtLoadQuery.Rows.Count > 0)
                {
                    if (ctl.GetType().ToString().ToUpper().Contains("GRIDGROUPINGCONTROL") == true)
                    {
                        CtlGridGroupControl.DataSource = null;
                        if (dtLoad.Rows.Count > 0)
                        {
                            CtlGridGroupControl.DataSource = dtLoad;
                            CtlGridGroupControl.Refresh();

                            CtlGridGroupControl.TopLevelGroupOptions.ShowAddNewRecordBeforeDetails = false;
                            CtlGridGroupControl.TopLevelGroupOptions.ShowCaption = false;
                            CtlGridGroupControl.NestedTableGroupOptions.ShowAddNewRecordBeforeDetails = false;
                            CtlGridGroupControl.TableModel.EnableLegacyStyle = false;
                            CtlGridGroupControl.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.Office2016White;
                            CtlGridGroupControl.TableOptions.ListBoxSelectionMode = SelectionMode.MultiExtended;
                            CtlGridGroupControl.TableControl.DpiAware = true;
                            CtlGridGroupControl.WantTabKey = false;

                            // Settings
                            CtlGridGroupControl.TopLevelGroupOptions.ShowFilterBar = bShowGridFilterbar;
                            if (bAllowGridEdit == false)
                                CtlGridGroupControl.ActivateCurrentCellBehavior = GridCellActivateAction.None;
                            else
                                CtlGridGroupControl.ActivateCurrentCellBehavior = GridCellActivateAction.ClickOnCell;

                            iTotGridWidth = CtlGridGroupControl.Width;
                            idtColCount = dtLoad.Columns.Count;
                            iGridColWidth = iTotGridWidth / (dtLoad.Columns.Count - 1);

                            for (int i = 0; i < CtlGridGroupControl.TableDescriptor.Columns.Count; i++)
                            {
                                if (i == 0)
                                    CtlGridGroupControl.TableDescriptor.Columns[i].Width = 0;
                                else
                                {
                                    if (CtlGridGroupControl.TableDescriptor.Columns[i].HeaderText == "Serial No")
                                    {
                                        CtlGridGroupControl.TableDescriptor.Columns[i].Width = iGridColWidth / 2;
                                        iSerilBalWidth = iGridColWidth / 2;
                                        CtlGridGroupControl.TableDescriptor.Columns[i].Appearance.AnyRecordFieldCell.HorizontalAlignment = Syncfusion.Windows.Forms.Grid.GridHorizontalAlignment.Center;
                                    }
                                    else if (i == CtlGridGroupControl.TableDescriptor.Columns.Count - 1)
                                    {
                                        CtlGridGroupControl.TableDescriptor.Columns[i].Width = iGridColWidth - 20;
                                    }
                                    else
                                    {
                                        CtlGridGroupControl.TableDescriptor.Columns[i].Width = iGridColWidth + iSerilBalWidth;
                                        iSerilBalWidth = 0;
                                    }
                                    filter.WireGrid(CtlGridGroupControl);
                                    CtlGridGroupControl.TableDescriptor.Columns[i].AllowFilter = bShowGridFilterbar;
                                }
                            }
                        }
                    }
                    else if (ctl.GetType().ToString().ToUpper().Contains("SFCOMBOBOX") == true)
                    {
                        CtlSFComboBox.DataSource = dtLoadQuery;

                        if (CtlSFComboBox.Name == "sfcboDiscGroup" || CtlSFComboBox.Name == "sfcboDepmnt")
                        {
                            CtlSFComboBox.DisplayMember = dtLoadQuery.Columns[1].ColumnName;
                        }
                        else
                        {
                            if (sDisplayField == "")
                                CtlSFComboBox.DisplayMember = dtLoadQuery.Columns[2].ColumnName;
                            else
                                CtlSFComboBox.DisplayMember = sDisplayField;
                        }

                        if (sValueField == "")
                            CtlSFComboBox.ValueMember = dtLoadQuery.Columns[0].ColumnName;
                        else
                            CtlSFComboBox.ValueMember = sValueField;

                        if (blnMultiSelect == true)
                        {
                            CtlSFComboBox.ComboBoxMode = Syncfusion.WinForms.ListView.Enums.ComboBoxMode.MultiSelection;
                            if (blnAllowSelectAll == true)
                                CtlSFComboBox.AllowSelectAll = true;

                            //CtlSFComboBox.DropDownControl.ShowButtons = false;
                            CtlSFComboBox.ShowToolTip = true;
                            CtlSFComboBox.ToolTipOption.InitialDelay = 3000;
                            CtlSFComboBox.ToolTipOption.AutoPopDelay = 2000;
                            //CtlSFComboBox.ShowClearButton = true;

                        }
                    }
                    else if (ctl.GetType().FullName.ToString().ToUpper() == "SYNCFUSION.WINDOWS.FORMS.TOOLS.MULTICOLUMNCOMBOBOX")
                    {
                        multcbo.DataSource = dtLoadQuery;

                        if (sDisplayField == "")
                            multcbo.DisplayMember = dtLoadQuery.Columns[1].ColumnName;
                        else
                            multcbo.DisplayMember = sDisplayField;

                        if (sValueField == "")
                            multcbo.ValueMember = dtLoadQuery.Columns[0].ColumnName;
                        else
                            multcbo.ValueMember = sValueField;

                        if (blnMultiColumn == true)
                            multcbo.MultiColumn = true;

                        multcbo.ShowColumnHeader = true;
                        multcbo.AlphaBlendSelectionColor = System.Drawing.Color.LightBlue;
                        multcbo.DropDownWidth = multcbo.Width;

                        if (btnHideValueMember == true)
                            multcbo.ListBox.Grid.Model.Cols.Hidden[multcbo.ValueMember] = true;
                    }
                    else if (ctl.GetType().FullName.ToString().ToUpper() == "SYSTEM.WINDOWS.FORMS.COMBOBOX")
                    {
                        cbo.DataSource = dtLoadQuery;

                        if (sValueField == "")
                            cbo.ValueMember = dtLoadQuery.Columns[0].ColumnName;
                        else
                            cbo.ValueMember = sValueField;
                        if (sDisplayField == "")
                        {
                            if (cbo.Name == "cboDiscGroup" || cbo.Name == "cboDepmnt")
                                cbo.DisplayMember = dtLoadQuery.Columns[1].ColumnName;
                            else
                                cbo.DisplayMember = dtLoadQuery.Columns[2].ColumnName;
                        }
                        else
                            cbo.DisplayMember = sDisplayField;

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public string Val(string sText, bool bRetIsNum = true)
        {
            string sRetVal = "";

            if (sText == "")
                sRetVal = "0";
            else
            {
                sRetVal = sText;
            }
            return sRetVal;
        }

        public DataTable CompactSearch(string strQuery = "", string strSearchFieldName = "", string strSearchData = "", string strOrderBy = "")
        {
            SqlConnection sqlconn = GetDBConnection();
            try
            {
                string sQuery = strQuery;

                if (strSearchData != "")
                {
                    if (strQuery.ToLower().Contains("where") == true)
                    {
                        sQuery = sQuery + " AND ";
                    }
                    else
                    {
                        sQuery = sQuery + " WHERE ";
                    }
                    if (strSearchFieldName != "")
                    {
                        if (strSearchData != "")
                        {
                            strSearchData = strSearchData.Replace("\r\n", "");
                            sQuery = sQuery + strSearchFieldName + " LIKE " + "'%" + strSearchData + "%'";
                        }
                        else
                        {
                            strSearchData = strSearchData.Replace("\r\n", "");
                            sQuery = sQuery + strSearchFieldName + " LIKE " + "'" + strSearchData + "'";
                        }
                    }
                }

                if (strOrderBy != "")
                {
                    sQuery = sQuery + " " + strOrderBy;
                }

                SqlDataAdapter daSearch = new SqlDataAdapter(sQuery, sqlconn);
                DataTable dtCompSearch = new DataTable();

                daSearch.Fill(dtCompSearch);
                return dtCompSearch;
            }
            catch (Exception ex)
            {
                if (sqlconn != null)
                    sqlconn.Close();
                return null;
            }
            finally
            {
                if (sqlconn != null)
                    sqlconn.Close();
            }
        }

        public TreeNode GetNodeByText(TreeNodeCollection nodes, string searchtext)
        {
            TreeNode n_found_node = null;
            bool b_node_found = false;

            foreach (TreeNode node in nodes)
            {

                if (node.Text == searchtext)
                {
                    b_node_found = true;
                    n_found_node = node;

                    return n_found_node;
                }

                if (!b_node_found)
                {
                    n_found_node = GetNodeByText(node.Nodes, searchtext);

                    if (n_found_node != null)
                    {
                        return n_found_node;
                    }
                }
            }
            return null;
        }

        public string FormatSQL(string StrField)
        {
            int GStgIntDecimals;
            GStgIntDecimals = 2;
            return " case " + StrField + " when 0.0 then null else  convert(decimal(20," + GStgIntDecimals + "), " + StrField + ") end";
        }

        public int SalesPrint(string FILEpATH, string PrinterName)
        {
            ClsFileOperation FSO = new ClsFileOperation();

            string AppPath = Application.StartupPath;

            if (File.Exists(Application.StartupPath + "\\Print.exe") == false)
                Interaction.MsgBox("Print file is missing", MsgBoxStyle.Exclamation);

            Interaction.Shell(Application.StartupPath + "\\Print.exe " + PrinterName + "æ" + FILEpATH, AppWinStyle.NormalFocus);

            return 0;
        }

        public string GetCheckedNodesTextForChkCompact(TreeNodeCollection nodes)
        {
            string sCheckedNodes = "";
            foreach (System.Windows.Forms.TreeNode aNode in nodes)
            {
                //edit
                if (aNode.Checked)
                {

                    sCheckedNodes = sCheckedNodes + aNode.Text + ",";
                    //Console.WriteLine(aNode.Text);

                    //if (aNode.Nodes.Count != 0)
                    //    GetCheckedNodes(aNode.Nodes);
                }
            }
            return sCheckedNodes.Substring(0, sCheckedNodes.Length - 1);
        }

        public void MessageboxToasted(string sCaption, string sMessage, int DelayMlSeconds = 1)
        {
            //new Controls.MsgToast(sCaption, sMessage, "TOP-RIGHT", DelayMlSeconds).ShowDialog();
            Controls.MsgToast Msgtst = new Controls.MsgToast(sCaption, sMessage, "TOP-RIGHT", DelayMlSeconds);
            Msgtst.Show();
        }

        public bool IsDiscountPercentageOutofLimit(decimal dEnteredDiscountper, string strFormName, decimal dLimitValue = 99, bool bChangeLimitValtoText = true)
        {
            bool bResult = false;
            string sMessage = "";

            if (dEnteredDiscountper > dLimitValue)
            {
                if (bChangeLimitValtoText == true)
                    sMessage = "You are trying to enter the value greater than " + dLimitValue + ". Automatically changing it to " + dLimitValue + "%.";
                else
                    sMessage = "You are trying to enter the value greater than " + dLimitValue + ".";
                MessageboxToasted(strFormName, sMessage);
                bResult = true;
            }

            return bResult;
        }

        public bool IsCursorOnEmptyLine(TextBox targetTextBox)
        {
            var cursorPosition = targetTextBox.SelectionStart;
            var positionBefore = targetTextBox.Text.LastIndexOf('\n', cursorPosition == 0 ? 0 : cursorPosition - 1);
            var positionAfter = targetTextBox.Text.IndexOf('\r', cursorPosition);
            if (positionBefore == -1) positionBefore = 0;
            if (positionAfter == -1) positionAfter = targetTextBox.Text.Length;
            return targetTextBox.Text.Substring(positionBefore, positionAfter - positionBefore).Trim() == "";
        }

        public void SaveInAppSettings(string sKeyName = "", string sValue = "")
        {
            int iID = 0;
            string sQuery = "";

            if (sKeyName != "")
            {
                sQuery = "UPDATE tblAppSettings SET ValueName='" + sValue + "' WHERE LTRIM(RTRIM(UPPER(KeyName)))='" + sKeyName.ToUpper().Trim() + "'";
                if (fnExecuteNonQuery(sQuery) == 0)
                {
                    sQuery = "INSERT INTO tblAppSettings(KeyName,ValueName,TenantID) VALUES('" + sKeyName.ToUpper() + "','" + sValue + "'," + Global.gblTenantID + ")";
                    fnExecuteNonQuery(sQuery);
                }
                if (sKeyName == "BLNSHOWCOMPANYNAME")
                {
                    sQuery = "update startup.dbo.tblcompany set companyname='" + sValue + "' where companycode='" + AppSettings.CompanyCode + "'";
                    fnExecuteNonQuery(sQuery);

                    sQuery = "update tblCompanyMaster set companyname='" + sValue + "' ";
                    fnExecuteNonQuery(sQuery);
                }
                if (sKeyName == "FSTARTDATE")
                {
                    sQuery = "update startup.dbo.tblcompany set fystartdate='" + sValue + "' where companycode='" + AppSettings.CompanyCode + "'";
                    fnExecuteNonQuery(sQuery);

                    sQuery = "update tblCompanyMaster set fystartdate='" + sValue + "' ";
                    fnExecuteNonQuery(sQuery);
                }
                if (sKeyName == "FENDDATE")
                {
                    sQuery = "update startup.dbo.tblcompany set fyenddate='" + sValue + "' where companycode='" + AppSettings.CompanyCode + "'";
                    fnExecuteNonQuery(sQuery);

                    sQuery = "update tblCompanyMaster set fyenddate='" + sValue + "' ";
                    fnExecuteNonQuery(sQuery);
                }

            }
        }

        public DataTable RetieveFromDBInAppSettings(double dTenanID)
        {
            DataTable dtData = new DataTable();
            dtData = fnGetData("SELECT UPPER(KeyName) as KeyName,ValueName,ID FROM tblAppSettings WHERE TenantID = " + dTenanID + "").Tables[0];
            if (dtData.Rows.Count > 0)
                return dtData;
            else
                return null;
        }

        public void LoadThemeAsperThemeID()
        {
            clsTheme cTheme = new clsTheme();
            DataTable dtGet = fnGetData("SELECT KeyName,ValueName, FROM tblAppSettings WHERE TenantID = " + Global.gblTenantID + "").Tables[0];
            if (dtGet.Rows.Count > 0)
            {
                for (int i = 0; i < dtGet.Rows.Count; i++)
                {
                    switch (dtGet.Rows[i]["KeyName"].ToString().Trim().ToUpper())
                    {
                        case "FORMMAINBCKCLR":
                            cTheme.FORMMAINBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHDRBCKCLR":
                            cTheme.FORMHDRBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMFTRBCKCLR":
                            cTheme.FORMFTRBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORLFTBCKCLR":
                            cTheme.FORLFTBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMRHTBCKCLR":
                            cTheme.FORMRHTBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHDRTXTCLR":
                            cTheme.FORMHDRTXTCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "FORMHILTCLR1":
                            cTheme.FORMHILTCLR1 = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHILTCLR2":
                            cTheme.FORMHILTCLR2 = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHILTCLR3":
                            cTheme.FORMHILTCLR3 = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDBCKCLR":
                            cTheme.GRIDBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRBCKCLR":
                            cTheme.GRIDHDRBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTCLR":
                            cTheme.GRIDHDRTXTCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTBLD":
                            cTheme.GRIDHDRTXTBLD = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTFNT":
                            cTheme.GRIDHDRTXTFNT = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDALTRWBCKCLR":
                            cTheme.GRIDALTRWBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTCLR":
                            cTheme.GRIDALTRWTXTCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTBLD":
                            cTheme.GRIDALTRWTXTBLD = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTFNT":
                            cTheme.GRIDALTRWTXTFNT = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDSELRWBCKCLR":
                            cTheme.GRIDSELRWBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTCLR":
                            cTheme.GRIDSELRWTXTCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTBLD":
                            cTheme.GRIDSELRWTXTBLD = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTFNT":
                            cTheme.GRIDSELRWTXTFNT = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDNORRWTXTCLR":
                            cTheme.GRIDNORRWTXTCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDNORRWTXTBLD":
                            cTheme.GRIDNORRWTXTBLD = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDNORRWTXTFNT":
                            cTheme.GRIDNORRWTXTFNT = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FONTFORAPP":
                            cTheme.FONTFORAPP = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "HEADFNTSIZ":
                            cTheme.HEADFNTSIZ = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "NORFNTSIZ":
                            cTheme.NORFNTSIZ = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "DESCFNTSIZ":
                            cTheme.DESCFNTSIZ = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        
                    }
                }
            }
        }

        //public void LoadAppSettings()
        //{
        //    AppSettings AppSet = new AppSettings();
        //    DataTable dtGet = RetieveFromDBInAppSettings(Global.gblTenantID);
        //    if (dtGet.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dtGet.Rows.Count; i++)
        //        {
        //            switch (dtGet.Rows[i]["KeyName"].ToString().Trim().ToUpper())
        //            {
        //                case "STRBATCODEPREFIXSUFFIX":
        //                    AppSet.BarcodePrefix = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "MAJORCURRENCY":
        //                    AppSet.MajorCurrency = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "MINORCURRENCY":
        //                    AppSet.MinorCurrency = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "MAJORSYMBOL":
        //                    AppSet.MajorSymbol = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "MINORSYMBOL":
        //                    AppSet.MinorSymbol = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "BLNSHOWCOMPANYADDRESS":
        //                    AppSet.CompAddress = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "BLNSHOWCOMPANYNAME":
        //                    AppSet.CompName = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "BLNTAXENABLED":
        //                    AppSet.TaxEnabled = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "DBLCESS":
        //                    AppSet.Cess = Convert.ToDouble(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTOBYINDAYBOOK":
        //                    AppSet.NeedToByDayBook = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNVERTICALACCFORMAT":
        //                    AppSet.VerticalAccFormat = Convert.ToDouble(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "EXPLORERSKININDEX":
        //                    AppSet.ThemeIndex = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNAUTOBACKUP":
        //                    AppSet.AutoBackupOnLogin = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "INTCESSMODE":
        //                    AppSet.CessMode = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNAGENT":
        //                    AppSet.NeedAgent = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "INTIMPLEMENTINGSTATECODE":
        //                    AppSet.StateCode = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "MYGSTIN":
        //                    AppSet.CompGSTIN = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "MYECOMMERCEGSTIN":
        //                    AppSet.ECommerceNo = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "CURRENCYDECIMALS":
        //                    AppSet.CurrencyDecimals = Convert.ToDecimal(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "QTYDECIMALFORMAT":
        //                    AppSet.QtyDecimals = Convert.ToDecimal(dtGet.Rows[i]["ValueName"].ToString());
        //                    break; 
        //                case "STRSTREET":
        //                    AppSet.CompStreet = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "STRCONTACT":
        //                    AppSet.CompContact = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "STEMAIL":
        //                    AppSet.CompEmail = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FSTARTDATE":
        //                    AppSet.FinYearStart = Convert.ToDateTime(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "FENDDATE":
        //                    AppSet.FinYearEnd = Convert.ToDateTime(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTAXCOLLSOURCE":
        //                    AppSet.NeedTaxCollectSourcet = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNADVANCED":
        //                    AppSet.NeedAdvanced = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNEXTDEVCONN":
        //                    AppSet.NeedExternalDevConnt = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNOFFERLOY":
        //                    AppSet.NeedOffersLoyalty = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNDISCGROUP":
        //                    AppSet.NeedDiscGrouping = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTEXSIZE":
        //                    AppSet.NeedSize = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTEXCOLOR":
        //                    AppSet.NeedColor = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTEXBRAND":
        //                    AppSet.NeedBrand = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTHEME":
        //                    AppSet.NeedTheme = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "STRLAKHORMILL":
        //                    AppSet.LakhsOrMillion = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "BLNSHOWBKONLOG":
        //                    AppSet.AutoBackupOnLogin = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNAUTOBACKUPEXIT":
        //                    AppSet.NeedAutobackupOnExit = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNCOSTCENTRE":
        //                    AppSet.NeedCostCenter = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "STRBACKUPSTRING":
        //                    AppSet.BackUpPath1 = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "STRBACKUPSTRING2":
        //                    AppSet.BackUpPath2 = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "STRBACKUPSTRING3":
        //                    AppSet.BackUpPath3 = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "INTCASINGID":
        //                    AppSet.CasingID = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;

        //                case "FORMMAINBCKCLR":
        //                    AppSet.FormMainBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMHDRBCKCLR":
        //                    AppSet.FormHeadBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMFTRBCKCLR":
        //                    AppSet.FormFooterBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORLFTBCKCLR":
        //                    AppSet.FormLeftBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMRHTBCKCLR":
        //                    AppSet.FormRightBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMHDRTXTCLR":
        //                    AppSet.FormHeadTextClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;

        //                case "FORMHILTCLR1":
        //                    AppSet.FormHighlight1Clr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMHILTCLR2":
        //                    AppSet .FormHighlight2Clr= dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMHILTCLR3":
        //                    AppSet.FormHighlight3Clr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;

        //                case "GRIDBCKCLR":
        //                    AppSet.GridBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDHDRBCKCLR":
        //                    AppSet.GridHeadBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDHDRTXTCLR":
        //                    AppSet.GridHeadTextClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDHDRTXTBLD":
        //                    AppSet.GridHeadTextBold = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDHDRTXTFNT":
        //                    AppSet.GridHeadTextFnt = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDALTRWBCKCLR":
        //                    AppSet.GridAltBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDALTRWTXTCLR":
        //                    AppSet.GridAltTextClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDALTRWTXTBLD":
        //                    AppSet.GridAltTextBold = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDALTRWTXTFNT[i]["":
        //                    AppSet.GridAltTextFnt = dtGet.RowsValueName"].ToString();
        //                    break;

        //                case "GRIDSELRWBCKCLR":
        //                    AppSet.GridSelRwBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDSELRWTXTCLR":
        //                    AppSet.GridSelRwTextClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDSELRWTXTBLD":
        //                    AppSet.GridSelRwTextBold = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDSELRWTXTFNT":
        //                    AppSet.GridSelRwTextFnt = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;

        //            }
        //        }
        //    }
        //}

        public void LoadAppSettings()
        {
            // AppSettings AppSet = new AppSettings();
            DataTable dtGet = RetieveFromDBInAppSettings(Global.gblTenantID);
            if (dtGet.Rows.Count > 0)
            {
                for (int i = 0; i < dtGet.Rows.Count; i++)
                {

                    switch (dtGet.Rows[i]["KeyName"].ToString().Trim().ToUpper())
                    {
                        case "STRWMIDENTIFIER":
                            AppSettings.STRWMIDENTIFIER = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRWMBARCODELENGTH":
                            AppSettings.STRWMBARCODELENGTH = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRWMQTYLENGTH":
                            AppSettings.STRWMQTYLENGTH = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRBATCODEPREFIXSUFFIX":
                            AppSettings.BarcodePrefix = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MAJORCURRENCY":
                            AppSettings.MajorCurrency = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MINORCURRENCY":
                            AppSettings.MinorCurrency = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MAJORSYMBOL":
                            AppSettings.MajorSymbol = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MINORSYMBOL":
                            AppSettings.MinorSymbol = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNSHOWCOMPANYADDRESS":
                            AppSettings.CompAddress = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNSHOWCOMPANYNAME":
                            AppSettings.CompName = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNTAXENABLED":
                            AppSettings.TaxEnabled = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"]));
                            break;
                        case "DBLCESS":
                            AppSettings.Cess = Convert.ToDouble(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNTOBYINDAYBOOK":
                            AppSettings.NeedToByDayBook = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNVERTICALACCFORMAT":
                            AppSettings.VerticalAccFormat = Convert.ToDouble(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "EXPLORERSKININDEX":
                            AppSettings.ThemeIndex = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNAUTOBACKUP":
                            AppSettings.AutoBackupOnLogin = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "INTCESSMODE":
                            AppSettings.CessMode = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNSRATEINC":
                            AppSettings.BLNSRATEINC = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNPRATEINC":
                            AppSettings.BLNPRATEINC = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNAGENT":
                            AppSettings.NeedAgent = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "INTIMPLEMENTINGSTATECODE":
                            AppSettings.StateCode = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MYGSTIN":
                            AppSettings.CompGSTIN = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MYECOMMERCEGSTIN":
                            AppSettings.ECommerceNo = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "AVAILABLETAXPER":
                            AppSettings.AVAILABLETAXPER = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "CURRENCYDECIMALS":
                            if (dtGet.Rows[i]["ValueName"].ToString() == "")
                                AppSettings.CurrencyDecimals = 2;
                            else
                            {
                                AppSettings.CurrDecimalFormat = "";
                                AppSettings.CurrencyDecimals = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());

                                for (int j = 0; j < Convert.ToInt32(AppSettings.CurrencyDecimals); j++)
                                    AppSettings.CurrDecimalFormat = AppSettings.CurrDecimalFormat + "0";

                                AppSettings.CurrDecimalFormat = "#0." + AppSettings.CurrDecimalFormat;
                            }
                            break;
                        case "QTYDECIMALFORMAT":
                            if (dtGet.Rows[i]["ValueName"].ToString() == "")
                                AppSettings.QtyDecimals = 2;
                            else
                            {
                                AppSettings.QtyDecimalFormat = "";
                                AppSettings.QtyDecimals = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());

                                for (int k = 0; k < Convert.ToInt32(AppSettings.QtyDecimals); k++)
                                    AppSettings.QtyDecimalFormat = AppSettings.QtyDecimalFormat + "0";

                                AppSettings.QtyDecimalFormat = "#0." + AppSettings.QtyDecimalFormat;
                            }
                            break;

                        ////case "CURRENCYDECIMALS":
                        ////    AppSettings.CurrencyDecimals = Convert.ToDecimal(dtGet.Rows[i]["ValueName"].ToString());
                        ////    break;
                        ////case "QTYDECIMALFORMAT":
                        ////    AppSettings.QtyDecimals = Convert.ToDecimal(dtGet.Rows[i]["ValueName"].ToString());
                        ////    break;
                        case "STRSTREET":
                            AppSettings.CompStreet = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRCONTACT":
                            AppSettings.CompContact = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STEMAIL":
                            AppSettings.CompEmail = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FSTARTDATE":
                            AppSettings.FinYearStart = Convert.ToDateTime(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "FENDDATE":
                            AppSettings.FinYearEnd = Convert.ToDateTime(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNTAXCOLLSOURCE":
                            AppSettings.NeedTaxCollectSourcet = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNBARCODE":
                            AppSettings.BLNBARCODE = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNADVANCED":
                            AppSettings.NeedAdvanced = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNEXTDEVCONN":
                            AppSettings.NeedExternalDevConnt = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNOFFERLOY":
                            AppSettings.NeedOffersLoyalty = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNDISCGROUP":
                            AppSettings.NeedDiscGrouping = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNTEXSIZE":
                            AppSettings.NeedSize = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNTEXCOLOR":
                            AppSettings.NeedColor = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNTEXBRAND":
                            AppSettings.NeedBrand = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNTHEME":
                            AppSettings.NeedTheme = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "STRLAKHORMILL":
                            AppSettings.LakhsOrMillion = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNSHOWBKONLOG":
                            AppSettings.AutoBackupOnLogin = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNAUTOBACKUPEXIT":
                            AppSettings.NeedAutobackupOnExit = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNCOSTCENTRE":
                            AppSettings.NeedCostCenter = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "STRBACKUPSTRING":
                            AppSettings.BackUpPath1 = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRBACKUPSTRING2":
                            AppSettings.BackUpPath2 = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRBACKUPSTRING3":
                            AppSettings.BackUpPath3 = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "INTCASINGID":
                            AppSettings.CasingID = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;

                        case "FORMMAINBCKCLR":
                            AppSettings.FormMainBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHDRBCKCLR":
                            AppSettings.FormHeadBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMFTRBCKCLR":
                            AppSettings.FormFooterBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORLFTBCKCLR":
                            AppSettings.FormLeftBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMRHTBCKCLR":
                            AppSettings.FormRightBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHDRTXTCLR":
                            AppSettings.FormHeadTextClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "FORMHILTCLR1":
                            AppSettings.FormHighlight1Clr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHILTCLR2":
                            AppSettings.FormHighlight2Clr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHILTCLR3":
                            AppSettings.FormHighlight3Clr = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDBCKCLR":
                            AppSettings.GridBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRBCKCLR":
                            AppSettings.GridHeadBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTCLR":
                            AppSettings.GridHeadTextClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTBLD":
                            AppSettings.GridHeadTextBold = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTFNT":
                            AppSettings.GridHeadTextFnt = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWBCKCLR":
                            AppSettings.GridAltBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTCLR":
                            AppSettings.GridAltTextClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTBLD":
                            AppSettings.GridAltTextBold = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTFNT":
                            AppSettings.GridAltTextFnt = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDSELRWBCKCLR":
                            AppSettings.GridSelRwBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTCLR":
                            AppSettings.GridSelRwTextClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTBLD":
                            AppSettings.GridSelRwTextBold = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTFNT":
                            AppSettings.GridSelRwTextFnt = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        //Added Anjitha 14/02/2022 2:30 PM
                        case "GRIDNORRWTXTCLR":
                            AppSettings.GridNorRwTextClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDNORRWTXTBLD":
                            AppSettings.GridNorRwTextBold = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDNORRWTXTFNT":
                            AppSettings.GridNorRwTextFnt = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FONTFORAPP":
                            AppSettings.FontforApplication = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "HEADFNTSIZ":
                            AppSettings.FormHeadingFntSiz = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "NORFNTSIZ":
                            AppSettings.FormNorFntSiz = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "DESCFNTSIZ":
                            AppSettings.FormDescFntSiz = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "PLCALCULATION":
                            AppSettings.PLCALCULATION = Convert.ToInt32(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;

                        case "SRATE1ACT":
                            AppSettings.IsActiveSRate1 = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "SRATE1NAME":
                            AppSettings.SRate1Name = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "SRATE2ACT":
                            AppSettings.IsActiveSRate2 = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "SRATE2NAME":
                            AppSettings.SRate2Name = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "SRATE3ACT":
                            AppSettings.IsActiveSRate3 = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "SRATE3NAME":
                            AppSettings.SRate3Name = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "SRATE4ACT":
                            AppSettings.IsActiveSRate4 = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "SRATE4NAME":
                            AppSettings.SRate4Name = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNCUSTAREA":
                            AppSettings.NeedCustArea = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;

                        //Added By Anjitha 16-Feb-2022 04:55 PM
                        case "SRATE5ACT":
                            AppSettings.IsActiveSRate5 = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "SRATE5NAME":
                            AppSettings.SRate5Name = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MRPACT":
                            AppSettings.IsActiveMRP = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "MRPNAME":
                            AppSettings.MRPName = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "INTTAXMODE":
                            AppSettings.TaxMode = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                    }
                }
            }
        }

        public string StockInsert(string sAction, decimal dItemID, string sBatchCode, decimal dQty, decimal dMRP, decimal dCostRateInc, decimal dCostRateExcl, decimal dPRateExcl, decimal dPrateInc, decimal dTaxPer, decimal dSRate1, decimal dSRate2, decimal dSRate3, decimal dSRate4, decimal dSRate5, int iBatchMode, string dVchType, DateTime dtVchDate, DateTime dtExpDt, double dRefID, double dVchTypeID, double dCCID = 1, double dTenantID = 1, bool bAutoCode = false, bool bBatchCode = false, bool bExpiry = false, decimal dPRate = 0)
        {
            string sMessage = "";
            //string sBarCode = "";
            DataTable dtStkIns = new DataTable();
            DataSet ds = new DataSet();
            if (dVchTypeID == 0)
            {//dtStkIns = fnGetData("EXEC UspTransStockUpdateFromItem " + dItemID + ",'" + sBatchCode + "',''," + dQty + "," + dMRP + "," + dCostRateInc + "," + dCostRateExcl + "," + dPRateExcl + "," + dPrateInc + "," + dTaxPer + "," + dSRate1 + "," + dSRate2 + "," + dSRate3 + "," + dSRate4 + "," + dSRate5 + "," + iBatchMode + ",'" + dVchType + "','" + dtVchDate.ToString("dd-MMM-yyyy") + "','" + dtExpDt.ToString("dd-MMM-yyyy") + "','" + sAction + "'," + dRefID + "," + dVchTypeID + "," + dCCID + "," + dTenantID + "").Tables[0];
                ds = fnGetData("EXEC UspTransStockUpdateFromItem " + dItemID + ",'" + sBatchCode + "',''," + dQty + "," + dMRP + "," + dCostRateInc + "," + dCostRateExcl + "," + dPRateExcl + "," + dPrateInc + "," + dTaxPer + "," + dSRate1 + "," + dSRate2 + "," + dSRate3 + "," + dSRate4 + "," + dSRate5 + "," + iBatchMode + ",'" + dVchType + "','" + dtVchDate.ToString("dd-MMM-yyyy") + "','" + dtExpDt.ToString("dd-MMM-yyyy") + "','" + sAction + "'," + dRefID + "," + dVchTypeID + "," + dCCID + "," + dTenantID + "," + dPRate + " ");
                if (ds != null)
                    if(ds.Tables.Count > 0)
                    {
                        dtStkIns = ds.Tables[0];
                    }
            }
            else
            {
                if (dVchTypeID == 0)
                {
                    sMessage = "-1" + "|" + "Voucher type not identified. Please re open the window";
                    return sMessage;
                }
                dtStkIns = fnGetData("EXEC UspTransStockUpdate " + dItemID + ",'" + sBatchCode + "',''," + dQty + "," + dMRP + "," + dCostRateInc + "," + dCostRateExcl + "," + dPRateExcl + "," + dPrateInc + "," + dTaxPer + "," + dSRate1 + "," + dSRate2 + "," + dSRate3 + "," + dSRate4 + "," + dSRate5 + "," + iBatchMode + ",'" + dVchType + "','" + dtVchDate.ToString("dd-MMM-yyyy") + "','" + dtExpDt.ToString("dd-MMM-yyyy") + "','" + sAction + "'," + dRefID + "," + dVchTypeID + "," + dCCID + "," + dTenantID + "").Tables[0];
            }
            if (dtStkIns.Rows.Count > 0)
            {
                sMessage = dtStkIns.Rows[0][0].ToString() + "|" + "";
            }

            return sMessage;
        }

        public void ControlEnterLeave(Control ctrl, Boolean blnIsLeave = false, Boolean blnEnableFormat = true)
        {
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

            if (ctrl is TextBox)
            {
                if (blnIsLeave == true)//Enter
                {
                    ctrl.BackColor = System.Drawing.Color.LightBlue;
                    //ctrl.Select();
                }
                else//Leave
                {
                    if (blnEnableFormat == true)
                    {
                        if (AppSettings.CasingID == 1)
                            ctrl.Text = ctrl.Text.ToUpper();
                        else if (AppSettings.CasingID == 2)
                            ctrl.Text = myTI.ToTitleCase(ctrl.Text.ToLower());

                    }

                    ctrl.BackColor = System.Drawing.SystemColors.Window;
                }
            }
            else
            {
                if (blnIsLeave == true)//Enter
                {
                    ctrl.BackColor = System.Drawing.Color.LightBlue;
                }
                else//Leave
                {
                    if (blnEnableFormat == true)
                    {
                        if (AppSettings.CasingID == 1)
                            ctrl.Text = ctrl.Text.ToUpper();
                        else if (AppSettings.CasingID == 2)
                            ctrl.Text = myTI.ToTitleCase(ctrl.Text.ToLower());

                    }

                    ctrl.BackColor = System.Drawing.SystemColors.Window;
                }
            }

        }

        public string CheckDBNullOrEmpty(string sValue)
        {
            bool IsNumeric = int.TryParse(sValue, out int numericValue);
            if (sValue != "")
            {
                if (string.IsNullOrEmpty(sValue) == true)
                {
                    if (IsNumeric == true)
                        return "0";
                    else
                        return "";
                }
                else
                {
                    return sValue;
                }
            }
            else
            {
                return "0"; //udayippu
            }
        }

        public string GetTableValue(string tableName, string FieldToSearch, string Condition = "")
        {
            string StrVal = "";
            sqlControl rs = new sqlControl();
            rs.Open("Select " + FieldToSearch + " as Field1 from " + tableName + " " + Condition);

            if (!rs.eof())
                StrVal = rs.fields("field1").ToString();
            if (StrVal == "")
                return "";
            else
                return StrVal;
        }

        public string chkChangeValuetoZero(string strVal = "")
        {
            string strRet = "";
            if (strVal != "")
            {
                if (strVal == ".00")
                {
                    strRet = "0";
                }
                else
                {
                    strRet = strVal;
                }
            }
            return strRet;
        }

        public bool VoucherInsert(int CCID, int vchtypeID, DateTime VchDate, DateTime vchtime, decimal LedgerID, decimal drlid, decimal crlid, long RefID, string VchNo, string CHECKINGTAG, double AmountD, double AmountC, long AgentID, long SalesmanID, int Optionalfield, long currencyID, bool BlnReconciled = false, string usernarration = "", string DestConnectionString = "")
        {
            try
            {
                long VchID;
                sqlControl rs = new sqlControl();
                if (DestConnectionString != "")
                    rs = new sqlControl(DestConnectionString);

                VchID = Convert.ToInt64(gfnGetNextSerialNo("tblVoucher", "VChID").ToString());

                if (VchID <= 0)
                    VchID = Convert.ToInt64(gfnGetNextSerialNo("tblVoucher", "VChID").ToString());

                if (usernarration == null)
                    usernarration = "";
                usernarration = usernarration.Replace("'", "''");

                double Amount;
                Amount = AmountD + AmountC;

                if (Amount <= 0)
                    return false;

                if (BlnReconciled)
                {
                    rs.Execute("INSERT INTO tblvoucher (vchid, VchDate, vchtime, LedgerID, drlid, crlid, RefID, VchNo, AmountD, AmountC, Amount, AgentID, SalesmanID, usernarration, optional, currencyID, Conversion, vchtypeID, CCID, myNarration) VALUES     (" + VchID + ",'" + Microsoft.VisualBasic.Strings.Format(VchDate, "dd/MMM/yyyy") + "','" + Microsoft.VisualBasic.Strings.Format(vchtime, "HH:mm:ss") + "'," + LedgerID + "," + drlid + "," + crlid + "," + Conversion.Val(RefID) + ",'" + VchNo.Replace("'", "''") + "'," + Amount + "," + AmountC + "," + (AmountD + AmountC) + "," + AgentID + "," + SalesmanID + ",'" + CHECKINGTAG.Replace("'", "''") + "'," + Optionalfield + "," + currencyID + ",0," + vchtypeID + "," + CCID + ",'" + usernarration.Replace("'", "''") + "')");
                    //rs.Execute("INSERT INTO tblvoucher (vchid, VchDate, vchtime, drlid, crlid, RefID, VchNo, AmountD, AmountC, AgentID, SalesmanID, usernarration, optional, currencyID, Conversion, vchtypeID, CCID, myNarration) VALUES     (" + VchID + ",'" + Strings.Format(VchDate, "dd/MMM/yyyy") + "','" + Strings.Format(vchtime, "HH:mm:ss") + "'," + crlid + "," + drlid + "," + Conversion.Val(RefID) + ",'" + VchNo.Replace("'", "''") + "',0," + Amount + "," + AgentID + "," + SalesmanID + ",'" + CHECKINGTAG.Replace("'", "''") + "'," + Optionalfield + "," + currencyID + ",0," + vchtypeID + "," + CCID + ",'" + usernarration.Replace("'", "''") + "')");
                }
                else
                {
                    rs.Execute("INSERT INTO tblvoucher (vchid, VchDate, vchtime, LedgerID, drlid, crlid, RefID, VchNo, AmountD, AmountC, Amount, AgentID, SalesmanID, usernarration, optional, currencyID, Conversion, vchtypeID, CCID, myNarration) VALUES     (" + VchID + ",'" + Microsoft.VisualBasic.Strings.Format(VchDate, "dd/MMM/yyyy") + "','" + Microsoft.VisualBasic.Strings.Format(vchtime, "HH:mm:ss") + "'," + LedgerID + "," + drlid + "," + crlid + "," + Conversion.Val(RefID) + ",'" + VchNo.Replace("'", "''") + "'," + AmountD + "," + AmountC + "," + (AmountD + AmountC) + "," + AgentID + "," + SalesmanID + ",'" + CHECKINGTAG.Replace("'", "''") + "'," + Optionalfield + "," + currencyID + ",0," + vchtypeID + "," + CCID + ",'" + usernarration.Replace("'", "''") + "')");
                    //rs.Execute("INSERT INTO tblvoucher (vchid, VchDate, vchtime, drlid, crlid, RefID, VchNo, AmountD, AmountC, AgentID, SalesmanID, usernarration, optional, currencyID, Conversion, vchtypeID, CCID, myNarration) VALUES     (" + VchID + ",'" + Strings.Format(VchDate, "dd/MMM/yyyy") + "','" + Strings.Format(vchtime, "HH:mm:ss") + "'," + crlid + "," + drlid + "," + Conversion.Val(RefID) + ",'" + VchNo.Replace("'", "''") + "',0," + Amount + "," + AgentID + "," + SalesmanID + ",'" + CHECKINGTAG.Replace("'", "''") + "'," + Optionalfield + "," + currencyID + ",0," + vchtypeID + "," + CCID + ",'" + usernarration.Replace("'", "''") + "')");
                }

                if (rs.RecordCount > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message + "Insert  to voucher function", MsgBoxStyle.Critical);
                return false;
            }
        }

        public void GridDefaultsStyleAccounts(DataGridView dvg)
        {
            try
            {
                // R:210, G:189, B:172
                // R:132, G:180, B:130
                // R:241, G:241, B:229
                // R:218, G:222, B:187
                // R:233, G:206, B:179
                // R:189, G:196, B:129
                // R:234, G:212, B:191
                dvg.EnableHeadersVisualStyles = false;

                DataGridViewCellStyle rowStyle;
                rowStyle = dvg.Rows[0].HeaderCell.Style;
                rowStyle.BackColor = Color.FromArgb(210, 189, 172);
                rowStyle.ForeColor = Color.Black;
                dvg.Rows[0].HeaderCell.Style = rowStyle;

                // R:241, G:241, B:229
                //dvg.DefaultCellStyle.SelectionBackColor = My.Settings.MyContrastColor;
                //dvg.DefaultCellStyle.SelectionForeColor = Color.Black;

                // Set RowHeadersDefaultCellStyle.SelectionBackColor so that its default
                // value won't override DataGridView.DefaultCellStyle.SelectionBackColor.
                dvg.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Empty;

                // Set the background color for all rows and for alternating rows. 
                // The value for alternating rows overrides the value for all rows. 
                dvg.RowsDefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255); // R:218, G:222, B:187
                                                                                    // R:189, G:196, B:129
                dvg.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 255, 240); // Color.FromArgb(189, 196, 129)

                // Set the row and column header styles.

                dvg.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dvg.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
                dvg.RowHeadersVisible = false;
                dvg.BackgroundColor = Color.FromArgb(255, 255, 255);

                int CustomColWidth;
                try
                {
                    CustomColWidth = (dvg.Width - 20) / (dvg.Columns.GetColumnCount(DataGridViewElementStates.Visible));
                }
                catch
                {
                    CustomColWidth = 30;
                }

                if (dvg.ColumnCount > 1)
                {
                    for (var i = 1; i <= dvg.ColumnCount - 1; i++)
                        dvg.Columns[i].Width = CustomColWidth;
                }
            }
            catch 
            {
            }
        }
    }
}
