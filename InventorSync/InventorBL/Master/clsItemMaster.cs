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
    public class clsItemMaster : DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteItemMasterInsert(Info.UspItemMasterInsertInfo ItemMasterInsertInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspItemMasterInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@ItemID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.ItemID;
                        SpParam = sqlCmd.Parameters.Add("@ItemCode", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.ItemCode;
                        SpParam = sqlCmd.Parameters.Add("@ItemName", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.ItemName;
                        SpParam = sqlCmd.Parameters.Add("@CategoryID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.CategoryID;
                        SpParam = sqlCmd.Parameters.Add("@Description", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.Description;
                        SpParam = sqlCmd.Parameters.Add("@PRate", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.PRate;
                        SpParam = sqlCmd.Parameters.Add("@SrateCalcMode", SqlDbType.Int);
                        SpParam.Value = ItemMasterInsertInfo.SrateCalcMode;
                        SpParam = sqlCmd.Parameters.Add("@CRateAvg", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.CRateAvg;
                        SpParam = sqlCmd.Parameters.Add("@Srate1Per", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.Srate1Per;
                        SpParam = sqlCmd.Parameters.Add("@SRate1", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.SRate1;
                        SpParam = sqlCmd.Parameters.Add("@Srate2Per", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.Srate2Per;
                        SpParam = sqlCmd.Parameters.Add("@SRate2", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.SRate2;
                        SpParam = sqlCmd.Parameters.Add("@Srate3Per", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.Srate3Per;
                        SpParam = sqlCmd.Parameters.Add("@SRate3", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.SRate3;
                        SpParam = sqlCmd.Parameters.Add("@Srate4", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.Srate4;
                        SpParam = sqlCmd.Parameters.Add("@Srate4Per", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.Srate4Per;
                        SpParam = sqlCmd.Parameters.Add("@SRate5", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.SRate5;
                        SpParam = sqlCmd.Parameters.Add("@Srate5Per", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.Srate5Per;
                        SpParam = sqlCmd.Parameters.Add("@MRP", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.MRP;
                        SpParam = sqlCmd.Parameters.Add("@ROL", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.ROL;
                        SpParam = sqlCmd.Parameters.Add("@Rack", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.Rack;
                        SpParam = sqlCmd.Parameters.Add("@Manufacturer", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.Manufacturer;
                        SpParam = sqlCmd.Parameters.Add("@ActiveStatus", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.ActiveStatus;
                        SpParam = sqlCmd.Parameters.Add("@IntLocal", SqlDbType.Int);
                        SpParam.Value = ItemMasterInsertInfo.IntLocal;
                        SpParam = sqlCmd.Parameters.Add("@ProductType", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.ProductType;
                        SpParam = sqlCmd.Parameters.Add("@ProductTypeID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.ProductTypeID;
                        SpParam = sqlCmd.Parameters.Add("@LedgerID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.LedgerID;
                        SpParam = sqlCmd.Parameters.Add("@UNITID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.UNITID;
                        SpParam = sqlCmd.Parameters.Add("@Notes", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.Notes;
                        SpParam = sqlCmd.Parameters.Add("@agentCommPer", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.agentCommPer;
                        SpParam = sqlCmd.Parameters.Add("@BlnExpiryItem", SqlDbType.Int);
                        SpParam.Value = ItemMasterInsertInfo.BlnExpiryItem;
                        SpParam = sqlCmd.Parameters.Add("@Coolie", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.Coolie;
                        SpParam = sqlCmd.Parameters.Add("@FinishedGoodID", SqlDbType.Int);
                        SpParam.Value = ItemMasterInsertInfo.FinishedGoodID;
                        SpParam = sqlCmd.Parameters.Add("@MinRate", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.MinRate;
                        SpParam = sqlCmd.Parameters.Add("@MaxRate", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.MaxRate;
                        SpParam = sqlCmd.Parameters.Add("@PLUNo", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.PLUNo;
                        SpParam = sqlCmd.Parameters.Add("@HSNID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.HSNID;
                        SpParam = sqlCmd.Parameters.Add("@iCatDiscPer", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.iCatDiscPer;
                        SpParam = sqlCmd.Parameters.Add("@IPGDiscPer", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.IPGDiscPer;
                        SpParam = sqlCmd.Parameters.Add("@ImanDiscPer", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.ImanDiscPer;
                        SpParam = sqlCmd.Parameters.Add("@ItemNameUniCode", SqlDbType.NVarChar);
                        SpParam.Value = ItemMasterInsertInfo.ItemNameUniCode;
                        SpParam = sqlCmd.Parameters.Add("@Minqty", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.Minqty;
                        SpParam = sqlCmd.Parameters.Add("@MNFID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.MNFID;
                        SpParam = sqlCmd.Parameters.Add("@PGID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.PGID;
                        SpParam = sqlCmd.Parameters.Add("@ItemCodeUniCode", SqlDbType.NVarChar);
                        SpParam.Value = ItemMasterInsertInfo.ItemCodeUniCode;
                        SpParam = sqlCmd.Parameters.Add("@UPC", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.UPC;
                        SpParam = sqlCmd.Parameters.Add("@BatchMode", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.BatchMode;
                        SpParam = sqlCmd.Parameters.Add("@blnExpiry", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.blnExpiry;
                        SpParam = sqlCmd.Parameters.Add("@Qty", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.Qty;
                        SpParam = sqlCmd.Parameters.Add("@MaxQty", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.MaxQty;
                        SpParam = sqlCmd.Parameters.Add("@IntNoOrWeight", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.IntNoOrWeight;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime);
                        SpParam.Value = ItemMasterInsertInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = ItemMasterInsertInfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.TenantID;
                        SpParam = sqlCmd.Parameters.Add("@blnCessOnTax", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.blnCessOnTax;
                        SpParam = sqlCmd.Parameters.Add("@CompCessQty", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.CompCessQty;
                        SpParam = sqlCmd.Parameters.Add("@CGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.CGSTTaxPer;
                        SpParam = sqlCmd.Parameters.Add("@SGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.SGSTTaxPer;
                        SpParam = sqlCmd.Parameters.Add("@IGSTTaxPer", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.IGSTTaxPer;
                        SpParam = sqlCmd.Parameters.Add("@CessPer", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.CessPer;
                        SpParam = sqlCmd.Parameters.Add("@VAT", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.VAT;
                        SpParam = sqlCmd.Parameters.Add("@CategoryIDs", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.CategoryIDs;
                        SpParam = sqlCmd.Parameters.Add("@ColorIDs", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.ColorIDs;
                        SpParam = sqlCmd.Parameters.Add("@SizeIDs", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.SizeIDs;
                        SpParam = sqlCmd.Parameters.Add("@BrandDisPer", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.BrandDisPer;
                        SpParam = sqlCmd.Parameters.Add("@DGroupID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.DGroupID;
                        SpParam = sqlCmd.Parameters.Add("@DGroupDisPer", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.DGroupDisPer;
                        SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                        SpParam.Value = iAction;

                        SpParam = sqlCmd.Parameters.Add("@BatchCode", SqlDbType.VarChar);
                        SpParam.Value = ItemMasterInsertInfo.BatchCode;
                        SpParam = sqlCmd.Parameters.Add("@CostRateInc", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.CostRateInc;
                        SpParam = sqlCmd.Parameters.Add("@CostRateExcl", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.CostRateExcl;
                        SpParam = sqlCmd.Parameters.Add("@PRateExcl", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.PRateExcl;
                        SpParam = sqlCmd.Parameters.Add("@PrateInc", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.PrateInc;
                        SpParam = sqlCmd.Parameters.Add("@BrandID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.BrandID;

                        SpParam = sqlCmd.Parameters.Add("@AltUnitID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.AltUnitID;
                        SpParam = sqlCmd.Parameters.Add("@ConvFactor", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.ConvFactor;
                        SpParam = sqlCmd.Parameters.Add("@ShelfLife", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.Shelflife;
                        SpParam = sqlCmd.Parameters.Add("@SRateInclusive", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.SRIncl;
                        SpParam = sqlCmd.Parameters.Add("@PRateInclusive", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.PRIncl;
                        SpParam = sqlCmd.Parameters.Add("@Slabsys", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.SlabSys;
                        SpParam = sqlCmd.Parameters.Add("@DiscPer", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.DiscPer;
                        SpParam = sqlCmd.Parameters.Add("@DepartmentID", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.DepartmentID;
                        SpParam = sqlCmd.Parameters.Add("@DefaultExpInDays", SqlDbType.Decimal);
                        SpParam.Value = ItemMasterInsertInfo.DefaultExpInDays;

                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        DataSet dsCommon = new DataSet();
                        sqlDa.Fill(dsCommon);
                        dtResult = dsCommon.Tables[0];
                        if (dtResult.Rows.Count > 0)
                            sResult = dtResult.Rows[0]["SqlSpResult"].ToString();

                        if (Convert.ToInt32(sResult) == -1)
                        {
                            sResult = sResult + "|" + dtResult.Rows[0]["ErrorMessage"].ToString();
                            Comm.WritetoSqlErrorLog(dtResult, Global.gblUserName);
                        }
                        else
                        {
                            sResult = "";
                            foreach (DataColumn cl in dtResult.Columns)
                            {
                                sResult = sResult + "|" + dtResult.Rows[0][cl].ToString();
                            }
                            sResult = sResult.Substring(1, sResult.Length - 1);
                        }
                        return sResult;
                    }
                }
                catch (Exception ex)
                {
                    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return " - 1" + "| " + ex.Message;
                }
                finally
                {
                    sqlConn.Close();
                }

            }
        }
        public DataTable GetItemMaster(UspGetItemMasterInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetItemMaster", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@ItemID", SqlDbType.Decimal).Value = Info.ItemID;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
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
        public DataTable GetItemMasterFromStock(UspGetItemMasterFromStockInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetItemMasterFromStock", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@StockID", SqlDbType.Decimal).Value = Info.StockID;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
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
        public DataTable GetItemMasterBatchUnique(UspgetitemmasterBatchUniqueInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspgetitemmasterBatchUnique", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@BatchUnique", SqlDbType.VarChar).Value = Info.BatchUnique;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
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
        public DataTable GetHSNFromItemMaster(UspGetHSNFromItemMasterInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetHSNFromItemMaster", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@ItemID", SqlDbType.Decimal).Value = Info.ItemID;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
                        sqlda.SelectCommand.Parameters.Add("@HSNID", SqlDbType.Decimal).Value = Info.HSNID;
                        sqlda.SelectCommand.Parameters.Add("@IGSTTaxPer", SqlDbType.Decimal).Value = Info.IGSTTaxPer;
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
