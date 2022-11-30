using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;

namespace InventorSync.InventorBL.Master
{
    public class clsStockDetails : DBConnection
    {
        Common Comm = new Common();
        public DataTable GetStockDetails(UspGetStockDetailsInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetStockDetails", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@StockID", SqlDbType.Decimal).Value = Info.StockID;
                        sqlda.SelectCommand.Parameters.Add("@BatchCode", SqlDbType.VarChar).Value = Info.BatchCode;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
                        sqlda.SelectCommand.Parameters.Add("@ItemID", SqlDbType.Decimal).Value = Info.ItemID;
                        sqlda.SelectCommand.Parameters.Add("@CCID", SqlDbType.Decimal).Value = Info.CCID;
                        sqlda.SelectCommand.Parameters.Add("@BatchUnique", SqlDbType.VarChar).Value = Info.BatchUnique;
                        sqlda.Fill(dtbl);
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
