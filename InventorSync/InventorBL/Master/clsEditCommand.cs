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
    public class clsEditCommand : DBConnection
    {
        Common Comm = new Common();
        public DataTable GetCategories(UspGetCategoriesinfo GetCat)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daCat = new SqlDataAdapter("UspGetCategories", sqlcon))
                    {
                        daCat.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daCat.SelectCommand.Parameters.Add("@CategoryID", SqlDbType.Int).Value = GetCat.CategoryID;
                        daCat.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Int).Value = GetCat.TenantId;
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

        public DataTable GetManufacturer(UspGetManufacturerInfo GetManf)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daManf = new SqlDataAdapter("UspGetManufacturer", sqlcon))
                    {
                        daManf.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daManf.SelectCommand.Parameters.Add("@MnfID", SqlDbType.Int).Value = GetManf.MnfID;
                        daManf.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Int).Value = GetManf.TenantID;
                        daManf.Fill(dtbl);
                    }
                }
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return dtbl;
        }
        public DataTable GetHSNCode(UspGetHSNInfo GetHSN)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daHSN = new SqlDataAdapter("UspGetHSN", sqlcon))
                    {
                        daHSN.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daHSN.SelectCommand.Parameters.Add("@HID", SqlDbType.Int).Value = GetHSN.HID;
                        daHSN.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Int).Value = GetHSN.TenantID;
                        daHSN.Fill(dtbl);
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
