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
    public class VoucherType : DBConnection
    {
        Common Comm = new Common();
        public DataTable UspGetVoucherType(UspGetVoucherType GetVchType)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daVouchTyp = new SqlDataAdapter("UspGetVoucherType", sqlcon))
                    {
                        daVouchTyp.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daVouchTyp.SelectCommand.Parameters.Add("@Prefix", SqlDbType.VarChar).Value = GetVchType.Prefix;
                        daVouchTyp.Fill(dtbl);
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
