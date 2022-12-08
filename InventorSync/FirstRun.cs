using InventorSync.InventorBL.Helper;
using DigiposZen.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorSync.Forms
{
    public partial class FirstRun : Form
    {
        string mPrimaryServer = "";
        public FirstRun(string PrimaryServer = "")
        {
            InitializeComponent();

            mPrimaryServer = PrimaryServer;
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
        private void FirstRun_Shown(object sender, EventArgs e)
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
                    RESTOREDB("Startup", @"c:\digisql\sqlTemp\Startup.bak", "");
                    prgInitialize.Value = 50;
                }
                }
                catch
                { }

                try
                { 
                RESTOREDB("DigiposDemo", @"c:\digisql\sqlTemp\DigiposDemo.bak", "");
                prgInitialize.Value = 70;
                }
                catch
                { }

                try
                { 
                RESTOREDB("DigiposModel", @"c:\digisql\sqlTemp\DigiposModel.bak", "");
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

        public void RESTOREDB(string bDNAME, string SRCDbPATH, string DestDbPath)
        {
            try
            {
                string Cnstring;
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
    }
}
