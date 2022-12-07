using InventorSync.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.InventorBL.Helper
{
    public class Global
    {


        static Global()
        {
            SetTenantID(1);
            SetUserName("Administrator");
            Setpwd("");
            SetUserId(1);
            SetUserGroupId(1);
            SetSuperUserId(1);
            SetMessageCaption("Inventor");
            SetSystemName("Standard");
            SetFormBorderColor(Color.FromArgb(64,64,64));
        }

        public static SqlTransaction Transaction { get; private set; }
        public static void GSqlTransaction(ref SqlConnection cn,  int Mode)
        {
            if(Transaction != null)
            {
                if (Mode == 1)
                    Transaction = cn.BeginTransaction();
                else if(Mode == 2)
                    Transaction.Commit();
                else if(Mode == 3)
                    Transaction.Rollback();
                else if(Mode == 4)
                    Transaction = null;
            }
        }

        //public SqlConnection mycn = new SqlConnection(Settings.Default.ConnectionString);
        //public SqlConnection myOnlinecn = new SqlConnection();

        public static SqlConnection mycn { get; set; }
        public static void Setmycn(string ConnectionString)
        {
            mycn = new SqlConnection(ConnectionString);
        }

        public static string SqlServerName { get; private set; }
        public static void SetSqlServerName(string sqlServerName)
        {
            SqlServerName = sqlServerName;
        }

        public static string SqlServerName2 { get; private set; }
        public static void SetSqlServerName2(string sqlServerName)
        {
            SqlServerName2 = sqlServerName;
        }

        public static DateTime FyStartDate { get; private set; }
        public static void SetFyStartDate(DateTime fyStartDate)
        {
            FyStartDate = fyStartDate;
        }

        public static DateTime FyEndDate { get; private set; }
        public static void SetFyEndDate(DateTime fyEndDate)
        {
            FyEndDate = fyEndDate;
        }

        public static string CompanyCode { get; private set; }
        public static void SetCompanyCode(string companyCode)
        {
            CompanyCode = companyCode;
        }
        
        public static string ComputerName { get; private set; }
        public static void SetComputerName(string computerName)
        {
            ComputerName = computerName;
        }
        
        public static bool blnTrialExpired { get; private set; }
        public static void SetTrialExpired(bool TrialExpired)
        {
            blnTrialExpired = TrialExpired;
        }

        public static int gblUserGroupID { get; private set; }
        public static void SetUserGroupId(int iUGID)
        {
            gblUserGroupID = iUGID;
        }

        public static int gblUserID { get; private set; }
        public static void SetUserId(int iUID)
        {
            gblUserID = iUID;
        }

        public static int gblSuperUserID { get; private set; }
        public static void SetSuperUserId(int iUID)
        {
            gblSuperUserID = iUID;
        }

        public static string ClientID { get; private set; }
        public static void SetClientID(int iUID)
        {
            gblSuperUserID = iUID;
        }

        public static string gblUserName { get; private set; }
        public static void SetUserName(string sUName)
        {
            gblUserName = sUName;
        }

        public static string gblpwd { get; private set; }
        public static void Setpwd(string sUName)
        {
            gblpwd = sUName;
        }

        public static string gblMessageCaption { get; private set; }
        public static void SetMessageCaption(string sMsgCaption)
        {
            gblMessageCaption = sMsgCaption;
        }

        public static int gblTenantID { get; private set; }
        public static void SetTenantID(int iTentID)
        {
            gblTenantID = iTentID;
        }

        public static string gblSystemName { get; private set; }
        public static void SetSystemName(string sSysName)
        {
            gblSystemName = sSysName;
        }


        public static int gblThemeID { get; private set; }
        public static void SetThemeID(int gintTheme)
        {
            gblThemeID = gintTheme;
        }
        public static Color gblFormBorderColor { get; private set; }
        public static void SetFormBorderColor(Color gfrmColor)
        {
            gblFormBorderColor = gfrmColor;
        }
    }
}
