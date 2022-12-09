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
    public class clsCategory : DBConnection
    {
        Common Comm = new Common();
        public string InsertUpdateDeleteCategory(Info.UspInsertCategoryInfo UspCatinfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";

            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspCategoriesInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();

                        SpParam = sqlCmd.Parameters.Add("@CategoryID", SqlDbType.Decimal);
                        SpParam.Value = UspCatinfo.CategoryID;
                        SpParam = sqlCmd.Parameters.Add("@Category", SqlDbType.VarChar);
                        SpParam.Value = UspCatinfo.Category;
                        SpParam = sqlCmd.Parameters.Add("@Remarks", SqlDbType.VarChar);
                        SpParam.Value = UspCatinfo.Remarks;
                        SpParam = sqlCmd.Parameters.Add("@ParentID", SqlDbType.VarChar);
                        SpParam.Value = UspCatinfo.ParentID;
                        SpParam = sqlCmd.Parameters.Add("@HID", SqlDbType.VarChar);
                        SpParam.Value = UspCatinfo.HID;
                        SpParam = sqlCmd.Parameters.Add("@CatDiscPer", SqlDbType.Decimal);
                        SpParam.Value = UspCatinfo.CatDiscPer;

                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = UspCatinfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = UspCatinfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@TenantId", SqlDbType.Decimal);
                        SpParam.Value = UspCatinfo.TenantId;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.Date);
                        SpParam.Value = UspCatinfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = UspCatinfo.LastUpdateTime;

                        SpParam = sqlCmd.Parameters.Add("@Action", SqlDbType.Int);
                        SpParam.Value = iAction;

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
                        return sResult;
                    }
                }
                catch (Exception ex)
                {
                    Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return "-1" + "|" + ex.Message;
                }
                finally
                {
                    sqlConn.Close();
                }

                
            }
        }

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
                        daCat.SelectCommand.Parameters.Add("@CategoryID", SqlDbType.Decimal).Value = GetCat.CategoryID;
                        daCat.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = GetCat.TenantId;
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
        public DataTable CheckParentIDExists(decimal parentID,decimal TenantID)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter daCat = new SqlDataAdapter("UspGetCategoriesParentid", sqlcon))
                    {
                        daCat.SelectCommand.CommandType = CommandType.StoredProcedure;
                        daCat.SelectCommand.Parameters.Add("@CategoryID", SqlDbType.Decimal).Value = parentID;
                        daCat.SelectCommand.Parameters.Add("@TenantId", SqlDbType.Decimal).Value = TenantID;
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

        public DataTable GetCategoryCheckedList(UspGetCategoryCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetCategoryCheckedList", sqlcon))
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
