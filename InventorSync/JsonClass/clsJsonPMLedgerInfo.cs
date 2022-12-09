using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiposZen.JsonClass
{
    public class clsJsonPMLedgerInfo
    {
        public decimal LID { get; set; }
        public string LName { get; set; }
        public string LAliasName { get; set; }
        public string GroupName { get; set; }
        public string Type { get; set; }
        public decimal OpBalance { get; set; }
        public string AppearIn { get; set; }
        public string Address { get; set; }
        public string CreditDays { get; set; }
        public string Phone { get; set; }
        public string TaxNo { get; set; }
        public decimal AccountGroupID { get; set; }
        public decimal RouteID { get; set; }
        public string Area { get; set; }
        public string Notes { get; set; }
        public decimal TargetAmt { get; set; }
        public decimal SMSSchID { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public decimal DiscPer { get; set; }
        public decimal InterestPer { get; set; }
        public string DummyLName { get; set; }
        public decimal BlnBank{  get; set; }
        public decimal CurrencyID {  get;  set; }
        public decimal AreaID { get; set; }
        public decimal PLID { get; set; }
        public decimal ActiveStatus { get; set; }
        public string EmailAddress { get; set; }
        public DateTime EntryDate { get; set; }
        public decimal blnBillWise { get; set; }
        public decimal CustomerCardID { get; set; }
        public decimal TDSPer { get; set; }
        public DateTime DOB { get; set; }
        public decimal StateID{ get; set; }
        public string CCIDS{ get; set; }
        public decimal CurrentBalance { get; set; }
        public string LedgerName { get; set; }
        public string LedgerCode { get; set; }
        public decimal BlnWallet { get; set; }
        public decimal blnCoupon { get; set; }
        public decimal TransComn{ get; set; }
        public decimal BlnSmsWelcome { get; set; }
        public string DLNO { get; set; }
        public decimal TDS{ get; set; }
        public string LedgerNameUnicode { get; set; }
        public string LedgerAliasNameUnicode { get; set; }
        public string ContactPerson { get; set; }
        public string TaxParameter { get; set; }
        public string TaxParameterType { get; set; }
        public string HSNCODE{ get; set; }
        public decimal CGSTTaxPer{ get; set; }
        public decimal SGSTTaxPer { get; set; }
        public decimal IGSTTaxPer { get; set; }
        public decimal HSNID{  get; set; }
        public decimal BankAccountNo { get; set; }
        public string BankIFSCCode { get; set; }
        public string BankNote{ get; set; }
        public decimal WhatsAppNo { get; set; }
        //Dipu 21-03-2022 ------- >>
        //public string SystemName { get; set; }
        //public decimal UserID {  get; set; }
        //public DateTime LastUpdateDate { get; set; }
        //public DateTime LastUpdateTime { get;  set; }
        public decimal TenantID {  get; set; }
        public int Action { get; set; }
        public string GSTType { get; set; }
        public decimal AgentID { get; set; }
    }
}
