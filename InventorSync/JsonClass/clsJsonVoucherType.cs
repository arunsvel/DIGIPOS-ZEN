using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorSync.JsonClass
{
    public class clsJsonVoucherType
    {
        private double roundOffMode;

        //Settings 

        public string TransactionName { get; set; }
        public decimal ParentID { get; set; }
        public decimal TransactionNumberingValue { get; set; }
        public string TransactionPrefix { get; set; }
        public decimal ReferenceNumberingValue { get; set; }
        public string ReferencePrefix { get; set; }
        public decimal TransactinSortOrder { get; set; }
        public string CursorNavigationOrderList { get; set; }
        public decimal PrimaryCCValue { get; set; }
        public decimal blnPrimaryLockWithSelection { get; set; }
        public decimal SecondaryCCValue { get; set; }
        public decimal blnSecondaryLockWithSelection { get; set; }

        //SearchMethod

        public decimal DefaultSearchMethodValue { get; set; }
        public decimal blnUseSpaceforRateSearch { get; set; }
        public decimal btnShowItmSearchByDefault { get; set; }
        public decimal blnMovetoNextRowAfterSelection { get; set; }
        public decimal blnHideNegativeorExpiredItmsfromMRRPSubWindow { get; set; }
        public decimal MMRPSubWindowsSortModeValue { get; set; }
        public decimal blnShowSearchWindowByDefault { get; set; }

        //Discount

        public decimal blnBillWiseDiscPercentage { get; set; }
        public decimal btnBillWiseDiscAmount { get; set; }
        public decimal blnBillWiseDiscPercentageandAmt { get; set; }
        public decimal BillWiseDiscFillXtraDiscFromValue { get; set; }
        public decimal blnItmWiseDiscPercentage { get; set; }
        public decimal blnItmWiseDiscAmount { get; set; }
        public decimal blnItmWiseDiscPercentageandAmt { get; set; }
        public decimal ItmWiseDiscFillXtraDiscFromValue { get; set; }
        public int RoundOffMode
        {
            get => Convert.ToInt32(roundOffMode);
            set => roundOffMode = value;
        }

        public double RoundOffBlock { get; set; }
        public decimal blnRateDiscount { get; set; }

        //Defaults

        public decimal DefaultTaxModeValue { get; set; }
        public decimal blnTaxModeLockWSel { get; set; }
        public decimal DefaultModeofPaymentValue { get; set; }
        public decimal blnModeofPaymentLockWSel { get; set; }
        public decimal DefaultPriceList { get; set; }
        public decimal blnPriceListLockWSel { get; set; }
        public decimal DefaultSaleStaffValue { get; set; }
        public decimal blnSaleStaffLockWSel { get; set; }
        public decimal DefaultAgentValue { get; set; }
        public decimal blnAgentLockWSel { get; set; }
        public decimal DefaultTaxInclusiveValue { get; set; }
        public decimal DefaultBarcodeMode { get; set; }
        public decimal blnTaxInclusiveLockWSel { get; set; }

        //Filters

        public string ProductClassList { get; set; }
        public string ItemCategoriesList { get; set; }
        public string CustomerSupplierAccGroupList { get; set; }
        public string DebitAccGroupList { get; set; }
        public string CreditAccGroupList { get; set; }
        public int ActiveStatus { get; set; }
        public string PrintSettings { get; set; }
        public int BoardRateExportType { get; set; }
        public string BoardRateQuery { get; set; }
        public string BoardRateFileName { get; set; }

        public clsJsonVoucherType()
        {
            InventorBL.Helper.Common Comm = new InventorBL.Helper.Common();
            try
            {
                roundOffMode = 0;
                TransactionName = "";
                ParentID = 0;
                TransactionNumberingValue = 0;
                TransactionPrefix = "";
                ReferenceNumberingValue = 0;
                ReferencePrefix = "";
                TransactinSortOrder = 0;
                CursorNavigationOrderList = "";
                PrimaryCCValue = 0;
                blnPrimaryLockWithSelection = 0;
                SecondaryCCValue = 0;
                blnSecondaryLockWithSelection = 0;
                DefaultSearchMethodValue = 0;
                blnUseSpaceforRateSearch  = 0;
                btnShowItmSearchByDefault  = 0;
                blnMovetoNextRowAfterSelection  = 0;
                blnHideNegativeorExpiredItmsfromMRRPSubWindow  = 0;
                MMRPSubWindowsSortModeValue  = 0;
                blnShowSearchWindowByDefault  = 0;

                //Discount

                blnBillWiseDiscPercentage  = 0;
                btnBillWiseDiscAmount  = 0;
                blnBillWiseDiscPercentageandAmt  = 0;
                BillWiseDiscFillXtraDiscFromValue  = 0;
                blnItmWiseDiscPercentage  = 0;
                blnItmWiseDiscAmount  = 0;
                blnItmWiseDiscPercentageandAmt  = 0;
                ItmWiseDiscFillXtraDiscFromValue  = 0;
                RoundOffMode = 0;

                RoundOffBlock  = 0;
                blnRateDiscount  = 0;

                //Defaults

                DefaultTaxModeValue  = 0;
                blnTaxModeLockWSel  = 0;
                DefaultModeofPaymentValue  = 0;
                blnModeofPaymentLockWSel  = 0;
                DefaultPriceList = 0;
                blnPriceListLockWSel = 0;
                DefaultSaleStaffValue  = 0;
                blnSaleStaffLockWSel  = 0;
                DefaultAgentValue  = 0;
                blnAgentLockWSel  = 0;
                DefaultTaxInclusiveValue  = 0;
                blnTaxInclusiveLockWSel  = 0;

                //Filters

                ProductClassList  = "";
                ItemCategoriesList  = "";
                CustomerSupplierAccGroupList  = "";
                DebitAccGroupList = "";
                CreditAccGroupList = "";

                ActiveStatus = 0;

                PrintSettings = "";

                BoardRateExportType = 0;
                BoardRateFileName = "";
                BoardRateQuery = "";
            }
            catch (Exception ex)
            {
                Comm.WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, "DIGIPOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
