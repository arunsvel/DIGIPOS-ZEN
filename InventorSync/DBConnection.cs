using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DigiposZen
{
    public class DBConnection
    {
        //public DBConnection()
        //{
        //    SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString());
        //    if (SqlConn.State == System.Data.ConnectionState.Closed || SqlConn.State == System.Data.ConnectionState.Broken)
        //        SqlConn.Open();
        //}

        public SqlConnection GetDBConnection() //string Server, string DBName)
        {
            try
            {
                //Data Source=DESKTOP-CKAPAK6\DIGIPOS;Initial Catalog=DigiposDemo;User ID=sa;Password=#infinitY@279
                //SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString());
                //DESKTOP-CKAPAK6\\DIGIPOS
                //DigiposDemo

                SqlConnection SqlConn = new SqlConnection(DigiposZen.Properties.Settings.Default.ConnectionString);
                if (SqlConn.State == System.Data.ConnectionState.Closed || SqlConn.State == System.Data.ConnectionState.Broken)
                    SqlConn.Open();

                return SqlConn;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Digipos connection");
                return new SqlConnection();
            }
        }
    }
}
