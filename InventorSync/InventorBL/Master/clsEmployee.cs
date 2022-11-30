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
    public class clsEmployee : DBConnection
    {
        Common Comm = new Common();
        public DataTable GetEmployee(UspGetEmployeeInfo Info)
        {
            DataTable dtbl = new DataTable();
            try
            {
                using (var sqlcon = GetDBConnection())
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter("UspGetEmployee", sqlcon))
                    {
                        sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
                        sqlda.SelectCommand.Parameters.Add("@EmpID", SqlDbType.Decimal).Value = Info.EmpID;
                        sqlda.SelectCommand.Parameters.Add("@TenantID", SqlDbType.Int).Value = Info.TenantID;
                        //sqlda.SelectCommand.Parameters.Add("@blnSalesStaff", SqlDbType.Bit).Value = Info.blnSalesStaff;
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
        public string InsertUpdateDeleteEmployee(Info.UspEmployeeInsertInfo EmployeeInsertInfo, int iAction = 0)
        {
            DataTable dtResult = new DataTable();
            string sResult = "";
            using (var sqlConn = GetDBConnection())
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("UspEmployeeInsert", sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter SpParam = new SqlParameter();
                        SpParam = sqlCmd.Parameters.Add("@EmpID", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.EmpID;
                        SpParam = sqlCmd.Parameters.Add("@Name", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.Name;
                        SpParam = sqlCmd.Parameters.Add("@Address", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.Address;
                        SpParam = sqlCmd.Parameters.Add("@NameOfFather", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.NameOfFather;
                        SpParam = sqlCmd.Parameters.Add("@PhNo", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.PhNo;
                        SpParam = sqlCmd.Parameters.Add("@MaritialStatus", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.MaritialStatus;
                        SpParam = sqlCmd.Parameters.Add("@NoOfFamilyMembers", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.NoOfFamilyMembers;
                        SpParam = sqlCmd.Parameters.Add("@NameOFNominee", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.NameOFNominee;
                        SpParam = sqlCmd.Parameters.Add("@Spouse", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.Spouse;
                        SpParam = sqlCmd.Parameters.Add("@SpouseEmployed", SqlDbType.Bit);
                        SpParam.Value = EmployeeInsertInfo.SpouseEmployed;
                        SpParam = sqlCmd.Parameters.Add("@OwnerOfResidence", SqlDbType.Bit);
                        SpParam.Value = EmployeeInsertInfo.OwnerOfResidence;
                        SpParam = sqlCmd.Parameters.Add("@PANNo", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.PANNo;
                        SpParam = sqlCmd.Parameters.Add("@BloodGroup", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.BloodGroup;
                        SpParam = sqlCmd.Parameters.Add("@Designation", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.Designation;
                        SpParam = sqlCmd.Parameters.Add("@Qualification", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.Qualification;
                        SpParam = sqlCmd.Parameters.Add("@Sex", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.Sex;
                        SpParam = sqlCmd.Parameters.Add("@DOB", SqlDbType.DateTime);
                        SpParam.Value = EmployeeInsertInfo.DOB;
                        SpParam = sqlCmd.Parameters.Add("@DOJ", SqlDbType.DateTime);
                        SpParam.Value = EmployeeInsertInfo.DOJ;
                        SpParam = sqlCmd.Parameters.Add("@DOI", SqlDbType.DateTime);
                        SpParam.Value = EmployeeInsertInfo.DOI;
                        SpParam = sqlCmd.Parameters.Add("@PensionAccNo", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.PensionAccNo;
                        SpParam = sqlCmd.Parameters.Add("@GPFAccNo", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.GPFAccNo;
                        SpParam = sqlCmd.Parameters.Add("@GSLIAccNo", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.GSLIAccNo;
                        SpParam = sqlCmd.Parameters.Add("@LICPolicyNo", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.LICPolicyNo;
                        SpParam = sqlCmd.Parameters.Add("@LICMonthlyPremium", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.LICMonthlyPremium;
                        SpParam = sqlCmd.Parameters.Add("@LICDateofMaturity", SqlDbType.DateTime);
                        SpParam.Value = EmployeeInsertInfo.LICDateofMaturity;
                        SpParam = sqlCmd.Parameters.Add("@CategoryID", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.CategoryID;
                        SpParam = sqlCmd.Parameters.Add("@DateofPromotion", SqlDbType.DateTime);
                        SpParam.Value = EmployeeInsertInfo.DateofPromotion;
                        SpParam = sqlCmd.Parameters.Add("@DateofRetirement", SqlDbType.DateTime);
                        SpParam.Value = EmployeeInsertInfo.DateofRetirement;
                        SpParam = sqlCmd.Parameters.Add("@GISAccNo", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.GISAccNo;
                        SpParam = sqlCmd.Parameters.Add("@BankAccNo", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.BankAccNo;
                        SpParam = sqlCmd.Parameters.Add("@Commission", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.Commission;
                        SpParam = sqlCmd.Parameters.Add("@CommissionAmt", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.CommissionAmt;
                        SpParam = sqlCmd.Parameters.Add("@EmpFname", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.EmpFname;
                        SpParam = sqlCmd.Parameters.Add("@blnSalesStaff", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.blnSalesStaff;
                        SpParam = sqlCmd.Parameters.Add("@PhotoPath", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.PhotoPath;
                        SpParam = sqlCmd.Parameters.Add("@InsCompany", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.InsCompany;
                        SpParam = sqlCmd.Parameters.Add("@CommissionCondition", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.CommissionCondition;
                        SpParam = sqlCmd.Parameters.Add("@EmpCode", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.EmpCode;
                        SpParam = sqlCmd.Parameters.Add("@blnStatus", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.blnStatus;
                        SpParam = sqlCmd.Parameters.Add("@DrivingLicenceNo", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.DrivingLicenceNo;
                        SpParam = sqlCmd.Parameters.Add("@DrivingLicenceExpiry", SqlDbType.DateTime);
                        SpParam.Value = EmployeeInsertInfo.DrivingLicenceExpiry;
                        SpParam = sqlCmd.Parameters.Add("@PassportNo", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.PassportNo;
                        SpParam = sqlCmd.Parameters.Add("@PassportExpiry", SqlDbType.DateTime);
                        SpParam.Value = EmployeeInsertInfo.PassportExpiry;
                        SpParam = sqlCmd.Parameters.Add("@Active", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.Active;
                        SpParam = sqlCmd.Parameters.Add("@SortOrder", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.SortOrder;
                        SpParam = sqlCmd.Parameters.Add("@EnrollNo", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.EnrollNo;
                        SpParam = sqlCmd.Parameters.Add("@TargetAmount", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.TargetAmount;
                        SpParam = sqlCmd.Parameters.Add("@IncentivePer", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.IncentivePer;
                        SpParam = sqlCmd.Parameters.Add("@PWD", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.PWD;
                        SpParam = sqlCmd.Parameters.Add("@Holidays", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.Holidays;
                        SpParam = sqlCmd.Parameters.Add("@LID", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.LID;
                        SpParam = sqlCmd.Parameters.Add("@salarypermonth", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.salarypermonth;
                        SpParam = sqlCmd.Parameters.Add("@SystemName", SqlDbType.VarChar);
                        SpParam.Value = EmployeeInsertInfo.SystemName;
                        SpParam = sqlCmd.Parameters.Add("@UserID", SqlDbType.Decimal);
                        SpParam.Value = EmployeeInsertInfo.UserID;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime);
                        SpParam.Value = EmployeeInsertInfo.LastUpdateDate;
                        SpParam = sqlCmd.Parameters.Add("@LastUpdateTime", SqlDbType.DateTime);
                        SpParam.Value = EmployeeInsertInfo.LastUpdateTime;
                        SpParam = sqlCmd.Parameters.Add("@TenantID", SqlDbType.Int);
                        SpParam.Value = EmployeeInsertInfo.TenantID;
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
    }
}
