using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.JsonClass
{
    public class clsJsonPMInfo
    {
 
        #region "Parameters--------------------------------------------------- >> "

        public decimal InvId
        {
            get;
            set;
        }
        public string InvNo
        {
            get;
            set;
        }
        public decimal AutoNum
        {
            get;
            set;
        }
        public string Prefix
        {
            get;
            set;
        }
        public DateTime InvDate
        {
            get;
            set;
        }
        public string VchType
        {
            get;
            set;
        }
        public string MOP
        {
            get;
            set;
        }
        public decimal TaxModeID
        {
            get;
            set;
        }
        public decimal LedgerId
        {
            get;
            set;
        }
        public string Party
        {
            get;
            set;
        }
        public decimal Discount
        {
            get;
            set;
        }
        public decimal dSteadyBillDiscPerc
        {
            get;
            set;
        }
        public decimal dSteadyBillDiscAmt
        {
            get;
            set;
        }
        public decimal TaxAmt
        {
            get;
            set;
        }
        public decimal GrossAmt
        {
            get;
            set;
        }
        public decimal BillAmt
        {
            get;
            set;
        }
        public decimal CoolieTotal
        {
            get;
            set;
        }
        public decimal Cancelled
        {
            get;
            set;
        }
        public decimal OtherExpense
        {
            get;
            set;
        }
        public decimal SalesManID
        {
            get;
            set;
        }
        public decimal Taxable
        {
            get;
            set;
        }
        public decimal NonTaxable
        {
            get;
            set;
        }
        public decimal ItemDiscountTotal
        {
            get;
            set;
        }
        public decimal RoundOff
        {
            get;
            set;
        }
        public string UserNarration
        {
            get;
            set;
        }
        public decimal SortNumber
        {
            get;
            set;
        }
        public decimal DiscPer
        {
            get;
            set;
        }
        public decimal VchTypeID
        {
            get;
            set;
        }
        public decimal CCID
        {
            get;
            set;
        }
        public decimal CurrencyID
        {
            get;
            set;
        }
        public string PartyAddress
        {
            get;
            set;
        }
        public int UserID
        {
            get;
            set;
        }
        public decimal AgentID
        {
            get;
            set;
        }
        public decimal CashDiscount
        {
            get;
            set;
        }
        public decimal DPerType_ManualCalc_Customer
        {
            get;
            set;
        }
        public decimal NetAmount
        {
            get;
            set;
        }
        public string RefNo
        {
            get;
            set;
        }
        public decimal CashPaid
        {
            get;
            set;
        }
        public decimal CardPaid
        {
            get;
            set;
        }
        public decimal blnWaitforAuthorisation
        {
            get;
            set;
        }
        public decimal UserIDAuth
        {
            get;
            set;
        }
        public DateTime BillTime
        {
            get;
            set;
        }
        public decimal StateID
        {
            get;
            set;
        }
        public string ImplementingStateCode
        {
            get;
            set;
        }
        public string GSTType
        {
            get;
            set;
        }
        public decimal CGSTTotal
        {
            get;
            set;
        }
        public decimal SGSTTotal
        {
            get;
            set;
        }
        public decimal IGSTTotal
        {
            get;
            set;
        }
        public string DelNoteNo
        {
            get;
            set;
        }
        public DateTime DelNoteDate
        {
            get;
            set;
        }
        public string DelNoteRefNo
        {
            get;
            set;
        }
        public DateTime DelNoteRefDate
        {
            get;
            set;
        }
        public string OtherRef
        {
            get;
            set;
        }
        public string BuyerOrderNo
        {
            get;
            set;
        }
        public DateTime BuyerOrderDate
        {
            get;
            set;
        }
        public string DispatchDocNo
        {
            get;
            set;
        }
        public string LRRRNo
        {
            get;
            set;
        }
        public string MotorVehicleNo
        {
            get;
            set;
        }
        public string PartyGSTIN
        {
            get;
            set;
        }
        public string BillType
        {
            get;
            set;
        }
        public decimal blnHold
        {
            get;
            set;
        }
        public decimal PriceListID
        {
            get;
            set;
        }
        public DateTime EffectiveDate
        {
            get;
            set;
        }
        public string partyCode
        {
            get;
            set;
        }
        public string MobileNo
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public string TaxType
        {
            get;
            set;
        }
        public decimal QtyTotal
        {
            get;
            set;
        }
        public decimal FreeTotal
        {
            get;
            set;
        }
        public decimal DestCCID
        {
            get;
            set;
        }
        public string AgentCommMode
        {
            get;
            set;
        }
        public decimal AgentCommAmount
        {
            get;
            set;
        }
        public decimal AgentLID
        {
            get;
            set;
        }
        public decimal BlnStockInsert
        {
            get;
            set;
        }
        public decimal BlnConverted
        {
            get;
            set;
        }
        public decimal ConvertedParentVchTypeID
        {
            get;
            set;
        }
        public decimal ConvertedVchTypeID
        {
            get;
            set;
        }
        public string ConvertedVchNo
        {
            get;
            set;
        }
        public decimal ConvertedVchID
        {
            get;
            set;
        }
        public string DeliveryNoteDetails
        {
            get;
            set;
        }
        public string OrderDetails
        {
            get;
            set;
        }
        public string IntegrityStatus
        {
            get;
            set;
        }
        public decimal BalQty
        {
            get;
            set;
        }
        public decimal CustomerpointsSettled
        {
            get;
            set;
        }
        public decimal blnCashPaid
        {
            get;
            set;
        }
        public decimal originalsalesinvid
        {
            get;
            set;
        }
        public decimal retuninvid
        {
            get;
            set;
        }
        public decimal returnamount
        {
            get;
            set;
        }
        public string SystemName
        {
            get;
            set;
        }
        public DateTime LastUpdateDate
        {
            get;
            set;
        }
        public DateTime LastUpdateTime
        {
            get;
            set;
        }
        public string DeliveryDetails
        {
            get;
            set;
        }
        public string DespatchDetails
        {
            get;
            set;
        }
        public string TermsOfDelivery
        {
            get;
            set;
        }
        public decimal FloodCessTot
        {
            get;
            set;
        }
        public decimal CounterID
        {
            get;
            set;
        }
        public decimal ExtraCharges
        {
            get;
            set;
        }
        public string ReferenceAutoNO
        {
            get;
            set;
        }
        public decimal CashDisPer
        {
            get;
            set;
        }
        public decimal CostFactor
        {
            get;
            set;
        }
        public decimal TenantID
        {
            get;
            set;
        }
        public string JsonData
        {
            get;
            set;
        }
        

        #endregion
    }
}
