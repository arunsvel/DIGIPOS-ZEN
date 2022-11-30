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
    class clsItemAnalysis : DBConnection
    {
        Common Comm = new Common();
        public DataTable GetStockHistory(UspGetStockHistoryInfo info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetStockHistory", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@ItemID", SqlDbType.Decimal).Value = info.ItemID;
                        sqlda.SelectCommand.Parameters.Add("@VchTypeID", SqlDbType.Decimal).Value = info.VchTypID;
                        sqlda.SelectCommand.Parameters.Add("@BatchUnique", SqlDbType.Decimal).Value = info.BatchUnique;
                        sqlda.SelectCommand.Parameters.Add("@CostCentreID", SqlDbType.Decimal).Value = info.CostCentreID;
                        sqlda.SelectCommand.Parameters.Add("@FromDate", SqlDbType.VarChar).Value = info.FromDate;
                        sqlda.SelectCommand.Parameters.Add("@ToDate", SqlDbType.VarChar).Value = info.ToDate;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = info.TenantID;
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
        public DataTable GetStockHistoryTotal(UspGetStockHistoryInfo info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetStockHistoryTotal", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@ItemID", SqlDbType.Decimal).Value = info.ItemID;
                        sqlda.SelectCommand.Parameters.Add("@VchTypeID", SqlDbType.Decimal).Value = info.VchTypID;
                        sqlda.SelectCommand.Parameters.Add("@BatchUnique", SqlDbType.Decimal).Value = info.BatchUnique;
                        sqlda.SelectCommand.Parameters.Add("@CostCentreID", SqlDbType.Decimal).Value = info.CostCentreID;
                        sqlda.SelectCommand.Parameters.Add("@FromDate", SqlDbType.VarChar).Value = info.FromDate;
                        sqlda.SelectCommand.Parameters.Add("@ToDate", SqlDbType.VarChar).Value = info.ToDate;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = info.TenantID;
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
        public DataTable GetStockReport(UspGetStockHistoryInfo info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetStockReport", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@ToDate", SqlDbType.VarChar).Value = info.ToDate;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = info.TenantID;
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
