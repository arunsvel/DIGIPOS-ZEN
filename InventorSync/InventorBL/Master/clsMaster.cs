using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DigiposZen.InventorBL.Helper;
using DigiposZen.Info;

namespace DigiposZen.InventorBL.Master
{
    public class clsMaster : DBConnection
    {
        Common Comm = new Common();

        public DataTable GetChildCount(string HID)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daCat = new SqlDataAdapter("Usp_GetCountChildnode", sqlcon))
                    {
                        daCat.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daCat.SelectCommand.Parameters.Add("@HID", SqlDbType.VarChar).Value = HID;
                        daCat.Fill(dtbl);
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return dtbl;
        }
        public DataTable GetColumnIDsData(UspGetMasterInfo Getcl)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daCat = new SqlDataAdapter("Usp_GetColumnIDExists", sqlcon))
                    {
                        daCat.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daCat.SelectCommand.Parameters.Add("@Type", SqlDbType.VarChar).Value = Getcl.TYPE;
                        daCat.SelectCommand.Parameters.Add("@ID", SqlDbType.VarChar).Value = Getcl.ID;
                        daCat.Fill(dtbl);
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return dtbl;
        }
    }
}
