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
    public class clsCostCentre : DBConnection
    {
        Common Comm = new Common();
        public DataTable GetAreaCheckedList(UspGetAreaCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    //using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetCostCenterCheckedList", sqlcon))
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("Select AreaID, Area From tblArea where AreaID in (" + Info.IDs + ") Order By Area", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.Text;
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

        public DataTable GetAccountGroupCheckedList(UspGetAccountGroupCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    //using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetCostCenterCheckedList", sqlcon))
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("Select AccountGroupID, AccountGroup From tblAccountGroup where AccountGroupID in (" + Info.IDs + ") Order By AccountGroup", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.Text;
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

        public DataTable GetCostCentre(UspGetCostCentreInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetCostCentre", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@CCID", SqlDbType.Decimal).Value = Info.CCID;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Decimal).Value = Info.TenantID;
                        sqlda.SelectCommand.Parameters.Add("@CCIDs", SqlDbType.VarChar).Value = Info.CCIDs;
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
        public string InsertUpdateDeleteCostCentre(Info.UspCostCentreInsertInfo CostCentreInsertInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspCostCentreInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@CCID", SqlDbType.Decimal);
                        SpParam.Value = CostCentreInsertInfo.CCID;
                        SpParam = sqlCmd.Parameters.Add("@CCName", SqlDbType.VarChar);
                        SpParam.Value = CostCentreInsertInfo.CCName;
                        SpParam = sqlCmd.Parameters.Add("@InCharge", SqlDbType.VarChar);
                        SpParam.Value = CostCentreInsertInfo.InCharge;
                        SpParam = sqlCmd.Parameters.Add("@Description1", SqlDbType.VarChar);
                        SpParam.Value = CostCentreInsertInfo.Description1;
                        SpParam = sqlCmd.Parameters.Add("@Description2", SqlDbType.VarChar);
                        SpParam.Value = CostCentreInsertInfo.Description2;
                        SpParam = sqlCmd.Parameters.Add("@Description3", SqlDbType.VarChar);
                        SpParam.Value = CostCentreInsertInfo.Description3;
                        SpParam = sqlCmd.Parameters.Add("@BLNDAMAGED", SqlDbType.Decimal);
                        SpParam.Value = CostCentreInsertInfo.BLNDAMAGED;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = CostCentreInsertInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = CostCentreInsertInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime);
                        SpParam.Value = CostCentreInsertInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = CostCentreInsertInfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Decimal);
                        SpParam.Value = CostCentreInsertInfo.TenantID;
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
                            sResult = sResult + " | " + dtResult.Rows[0]["ErrorMessage"].ToString();
                            Comm.WritetoSqlErrorLog(dtResult, Global.gblUserName);
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
        public DataTable GetCostCeneterCheckedList(UspGetCostCenterCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    //using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetCostCenterCheckedList", sqlcon))
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("Select CCID, CCName From tblCostCentre where CCID in (" + Info.IDs + ") Order By CCName", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.Text;
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
        public DataTable GetMnfCheckedList(UspGetMnfCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetMnfCheckedList", sqlcon))
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
        public DataTable GetPtypeCheckedList(UspGetPtypeCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetPtypeCheckedList", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@IDs", SqlDbType.NVarChar).Value = Info.IDs;
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
        public DataTable GetTaxModeCheckedList(UspGetTaxModeCheckedListtInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetTaxModeCheckedList", sqlcon))
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
        public DataTable GetCheckedListStaff(UspGetStaffCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("Select EmpID, Name From tblEmployee where EmpID in (" + Info.IDs + ") Order By Name", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.Text;
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
        public DataTable GetCheckedListAgent(UspGetAgentCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetAgentCheckedList", sqlcon))
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
        public DataTable GetCheckedListBill(UspGetBillCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetBillCheckedList", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@IDs", SqlDbType.NVarChar).Value = Info.IDs;
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
        
        public DataTable GetCheckedListMop(UspGetMopCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
             {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("Select distinct mop as MID, mop as MopName from tblPurchase where mop in (" + Info.IDs + ") Order By MOP", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.Text;
                        //sqlda.SelectCommand.Parameters.Add("@IDs", SqlDbType.NVarChar).Value = Info.IDs;
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
        public DataTable GetCheckedListCat(UspGetCategoryCheckedListInfo Info)
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
        public DataTable GetCheckedListUser(UspGetUserCheckedListInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetUserCheckedList", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@IDs", SqlDbType.NVarChar).Value = Info.IDs;
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
