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
    public class clsVoucherType : DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteVchTypeInsert(Info.UspVchTypeInsertInfo VchTypeInsertInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspVchTypeInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@VchTypeID", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.VchTypeID;
                        SpParam = sqlCmd.Parameters.Add("@VchType", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.VchType;
                        SpParam = sqlCmd.Parameters.Add("@ShortKey", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.ShortKey;
                        SpParam = sqlCmd.Parameters.Add("@EasyKey", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.EasyKey;
                        SpParam = sqlCmd.Parameters.Add("@SortOrder", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.SortOrder;
                        SpParam = sqlCmd.Parameters.Add("@ActiveStatus", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.ActiveStatus;
                        SpParam = sqlCmd.Parameters.Add("@ParentID", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.ParentID;
                        SpParam = sqlCmd.Parameters.Add("@Description", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.Description;
                        SpParam = sqlCmd.Parameters.Add("@numberingCode", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.numberingCode;
                        SpParam = sqlCmd.Parameters.Add("@Prefix", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.Prefix;
                        SpParam = sqlCmd.Parameters.Add("@Sufix", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.Sufix;
                        SpParam = sqlCmd.Parameters.Add("@ItemClassIDS", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.ItemClassIDS;
                        SpParam = sqlCmd.Parameters.Add("@CreditGroupIDs", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.CreditGroupIDs;
                        SpParam = sqlCmd.Parameters.Add("@DebitGroupIDs", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.DebitGroupIDs;
                        SpParam = sqlCmd.Parameters.Add("@ProductTypeIDs", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.ProductTypeIDs;
                        SpParam = sqlCmd.Parameters.Add("@GeneralSettings", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.GeneralSettings;
                        SpParam = sqlCmd.Parameters.Add("@NegativeBalance", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.NegativeBalance;
                        SpParam = sqlCmd.Parameters.Add("@RoundOffBlock", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.RoundOffBlock;
                        SpParam = sqlCmd.Parameters.Add("@RoundOffMode", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.RoundOffMode;
                        SpParam = sqlCmd.Parameters.Add("@ItemClassIDS2", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.ItemClassIDS2;
                        SpParam = sqlCmd.Parameters.Add("@SecondaryCCIDS", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.SecondaryCCIDS;
                        SpParam = sqlCmd.Parameters.Add("@PrimaryCCIDS", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.PrimaryCCIDS;
                        SpParam = sqlCmd.Parameters.Add("@OrderVchTypeIDS", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.OrderVchTypeIDS;
                        SpParam = sqlCmd.Parameters.Add("@NoteVchTypeIDS", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.NoteVchTypeIDS;
                        SpParam = sqlCmd.Parameters.Add("@QuotationVchTypeIDS", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.QuotationVchTypeIDS;
                        SpParam = sqlCmd.Parameters.Add("@DEFMOPID", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.DEFMOPID;
                        SpParam = sqlCmd.Parameters.Add("@BLNLOCKMOP", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.BLNLOCKMOP;
                        SpParam = sqlCmd.Parameters.Add("@DEFTAXMODEID", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.DEFTAXMODEID;
                        SpParam = sqlCmd.Parameters.Add("@BLNLOCKTAXMODE", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.BLNLOCKTAXMODE;
                        SpParam = sqlCmd.Parameters.Add("@DEFAGENTID", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.DEFAGENTID;
                        SpParam = sqlCmd.Parameters.Add("@BLNLOCKAGENT", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.BLNLOCKAGENT;
                        SpParam = sqlCmd.Parameters.Add("@DEFPRICELISTID", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.DEFPRICELISTID;
                        SpParam = sqlCmd.Parameters.Add("@BLNLOCKPRICELIST", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.BLNLOCKPRICELIST;
                        SpParam = sqlCmd.Parameters.Add("@DEFSALESMANID", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.DEFSALESMANID;
                        SpParam = sqlCmd.Parameters.Add("@BLNLOCKSALESMAN", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.BLNLOCKSALESMAN;
                        SpParam = sqlCmd.Parameters.Add("@DEFPRINTID", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.DEFPRINTID;
                        SpParam = sqlCmd.Parameters.Add("@BLNLOCKPRINT", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.BLNLOCKPRINT;
                        SpParam = sqlCmd.Parameters.Add("@ColwidthStr", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.ColwidthStr;
                        SpParam = sqlCmd.Parameters.Add("@gridColor", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.gridColor;
                        SpParam = sqlCmd.Parameters.Add("@DefaultGodownID", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.DefaultGodownID;
                        SpParam = sqlCmd.Parameters.Add("@ActCFasCostLedger", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.ActCFasCostLedger;
                        SpParam = sqlCmd.Parameters.Add("@ActCFasCostLedger4", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.ActCFasCostLedger4;
                        SpParam = sqlCmd.Parameters.Add("@gridHeaderColor", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.gridHeaderColor;
                        SpParam = sqlCmd.Parameters.Add("@BLNUseForClientSync", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.BLNUseForClientSync;
                        SpParam = sqlCmd.Parameters.Add("@rateInclusiveIndex", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.rateInclusiveIndex;
                        SpParam = sqlCmd.Parameters.Add("@BlnBillWiseDisc", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.BlnBillWiseDisc;
                        SpParam = sqlCmd.Parameters.Add("@BlnItemWisePerDisc", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.BlnItemWisePerDisc;
                        SpParam = sqlCmd.Parameters.Add("@BlnItemWiseAmtDisc", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.BlnItemWiseAmtDisc;
                        SpParam = sqlCmd.Parameters.Add("@gridselectedRow", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.gridselectedRow;
                        SpParam = sqlCmd.Parameters.Add("@GridHeaderFont", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.GridHeaderFont;
                        SpParam = sqlCmd.Parameters.Add("@GridBackColor", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.GridBackColor;
                        SpParam = sqlCmd.Parameters.Add("@GridAlternatCellColor", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.GridAlternatCellColor;
                        SpParam = sqlCmd.Parameters.Add("@GridCellColor", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.GridCellColor;
                        SpParam = sqlCmd.Parameters.Add("@GridFontColor", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.GridFontColor;
                        SpParam = sqlCmd.Parameters.Add("@Metatag", SqlDbType.NVarChar);
                        SpParam.Value = VchTypeInsertInfo.Metatag;
                        SpParam = sqlCmd.Parameters.Add("@DefaultCriteria", SqlDbType.NVarChar);
                        SpParam.Value = VchTypeInsertInfo.DefaultCriteria;
                        SpParam = sqlCmd.Parameters.Add("@SearchSql", SqlDbType.NVarChar);
                        SpParam.Value = VchTypeInsertInfo.SearchSql;
                        SpParam = sqlCmd.Parameters.Add("@SmartSearchBehavourMode", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.SmartSearchBehavourMode;
                        SpParam = sqlCmd.Parameters.Add("@criteriaconfig", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.criteriaconfig;
                        SpParam = sqlCmd.Parameters.Add("@intEnterKeyBehavourMode", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.intEnterKeyBehavourMode;
                        SpParam = sqlCmd.Parameters.Add("@BlnBillDiscAmtEntry", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.BlnBillDiscAmtEntry;
                        SpParam = sqlCmd.Parameters.Add("@blnRateDiscount", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.blnRateDiscount;
                        SpParam = sqlCmd.Parameters.Add("@IntdefaultFocusColumnID", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.IntdefaultFocusColumnID;
                        SpParam = sqlCmd.Parameters.Add("@BlnTouchScreen", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.BlnTouchScreen;
                        SpParam = sqlCmd.Parameters.Add("@StrTouchSetting", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.StrTouchSetting;
                        SpParam = sqlCmd.Parameters.Add("@StrCalculationFields", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.StrCalculationFields;
                        SpParam = sqlCmd.Parameters.Add("@CRateCalMethod", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.CRateCalMethod;
                        SpParam = sqlCmd.Parameters.Add("@MMRPSortOrder", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.MMRPSortOrder;
                        SpParam = sqlCmd.Parameters.Add("@ItemDiscountFrom", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.ItemDiscountFrom;
                        SpParam = sqlCmd.Parameters.Add("@DEFPRINTID2", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.DEFPRINTID2;
                        SpParam = sqlCmd.Parameters.Add("@BLNLOCKPRINT2", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.BLNLOCKPRINT2;
                        SpParam = sqlCmd.Parameters.Add("@BillDiscountFrom", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.BillDiscountFrom;
                        SpParam = sqlCmd.Parameters.Add("@WindowBackColor", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.WindowBackColor;
                        SpParam = sqlCmd.Parameters.Add("@ContrastBackColor", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.ContrastBackColor;
                        SpParam = sqlCmd.Parameters.Add("@BlnEnableCustomFormColor", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.BlnEnableCustomFormColor;
                        SpParam = sqlCmd.Parameters.Add("@returnVchtypeID", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.returnVchtypeID;
                        SpParam = sqlCmd.Parameters.Add("@PrintCopies", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.PrintCopies;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime);
                        SpParam.Value = VchTypeInsertInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = VchTypeInsertInfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@BlnMobileVoucher", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.BlnMobileVoucher;
                        SpParam = sqlCmd.Parameters.Add("@SearchSQLSettings", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.SearchSQLSettings;
                        SpParam = sqlCmd.Parameters.Add("@AdvancedSearchSQLEnabled", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.AdvancedSearchSQLEnabled;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = VchTypeInsertInfo.TenantID;
                        SpParam = sqlCmd.Parameters.Add("@VchJson", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.VchJson;
                        SpParam = sqlCmd.Parameters.Add("@FeaturesJson", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.FeaturesJson;
                        SpParam = sqlCmd.Parameters.Add("@DEFTaxInclusiveID", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.DEFTAXINCLUSIVEID;
                        SpParam = sqlCmd.Parameters.Add("@BLNLOCKTaxInclusive", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.BLNLOCKTAXINCLUSIVE;
                        SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                        SpParam.Value = iAction;
                        SpParam = sqlCmd.Parameters.Add("@PrintSettings", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.PrintSettings;
                        SpParam = sqlCmd.Parameters.Add("@BoardRateQuery", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.BoardRateQuery;
                        SpParam = sqlCmd.Parameters.Add("@BoardRateFileName", SqlDbType.VarChar);
                        SpParam.Value = VchTypeInsertInfo.BoardRateFileName;
                        SpParam = sqlCmd.Parameters.Add("@BoardRateExportType", SqlDbType.Int);
                        SpParam.Value = VchTypeInsertInfo.BoardRateExportType;
                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        DataSet dsCommon = new DataSet();
                        sqlDa.Fill(dsCommon);
                        dtResult = dsCommon.Tables[0];
                        if (dtResult.Rows.Count > 0)
                            sResult = dtResult.Rows[0]["SqlSpResult"].ToString();

                        if (Convert.ToInt32(sResult) == -1)
                        {
                            sResult = sResult + " | " + dtResult.Rows[0]["ErrorMessage"].ToString();
                            Comm.WritetoSqlErrorLog(dtResult, Global.gblUserName);
                        }
                        return sResult;
                    }
                }
                catch (Exception ex)
                {
                    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    Comm.MessageboxToasted("VoucherType", ex.Message);
                    return " - 1" + "| " + ex.Message;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public DataTable GetVchType(UspGetVchTypeInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetVchType", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@VchTypeID", SqlDbType.Decimal).Value = Info.VchTypeID;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
                        sqlda.SelectCommand.Parameters.Add("@VchTypeIDs", SqlDbType.VarChar).Value = Info.VchTypeIDs;
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
        public DataTable GetVoucherCheckedList(UspGetVoucherCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetVoucherCheckedList", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@IDs", SqlDbType.NVarChar).Value = Info.IDs;
                        sqlda.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Int).Value = Info.TenantId;
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
