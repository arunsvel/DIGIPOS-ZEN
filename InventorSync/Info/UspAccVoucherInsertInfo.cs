using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiposZen.Info
{
    public class UspAccVoucherInsertInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public int InvId
        {
            get;
            set;
        }
        public string InvNo
        {
            get;
            set;
        }
        public int AutoNum
        {
            get;
            set;
        }
        public string ReferenceAutoNO
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
        public string DebitCredit
        {
            get;
            set;
        }
        public int ACCLedgerID
        {
            get;
            set;
        }
        public string TaxModeID
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
        public int Cancelled
        {
            get;
            set;
        }
        public int SalesManID
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
        public string UserNarration
        {
            get;
            set;
        }
        public int SortNumber
        {
            get;
            set;
        }
        public int VchTypeID
        {
            get;
            set;
        }
        public int CCID
        {
            get;
            set;
        }
        public int CurrencyID
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
        public decimal CashDiscount
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
        public int blnWaitforAuthorisation
        {
            get;
            set;
        }
        public int UserIDAuth
        {
            get;
            set;
        }
        public DateTime BillTime
        {
            get;
            set;
        }
        public int StateID
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
        public int blnHold
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
        public int REconciled
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
        public string ChequeNo
        {
            get;
            set;
        }
        public string BankName
        {
            get;
            set;
        }
        public string SalesTaxtype
        {
            get;
            set;
        }
        public string TransType
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
        public int CounterID
        {
            get;
            set;
        }
        public string AdavancedMode
        {
            get;
            set;
        }
        public DateTime ChequeDate
        {
            get;
            set;
        }
        public string JsonData
        {
            get;
            set;
        }
        public int TenantID
        {
            get;
            set;
        }
        public int Action
        {
            get;
            set;
        }
        #endregion
    }
}
