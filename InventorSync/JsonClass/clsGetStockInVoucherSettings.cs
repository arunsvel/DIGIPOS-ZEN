using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.JsonClass
{
    public class clsGetStockInVoucherSettings
    {
        public bool BLNPOSTCASHENTRY { get; set; }
        public bool BLNEDITMRPRATE { get; set; }
        public bool blneditsalerate { get; set; }
        public bool blnEditTaxPer { get; set; }
        public bool blnenablecashdiscount { get; set; }
        public bool blnenableEffeciveDate { get; set; }
        public bool blnpartydetails { get; set; }
        public bool blnprintconfirmation { get; set; }
        public bool blnprintimmediately { get; set; }
        public bool BLNRECALCULATESalesRatesOnPercentage { get; set; }
        public bool blnshowbillnarration { get; set; }
        public bool BLNSHOWFREEQUANTITY { get; set; }
        public bool blnShowItemCalcGrid { get; set; }
        public bool blnShowItemProfitPer { get; set; }
        public bool blnshowotherexpense { get; set; }
        public bool blnshowpreview { get; set; }
        public bool blnShowRateFixer { get; set; }
        public bool blnShowReferenceNo { get; set; }
        public bool blnSummariseDuplicateItemsInPrint { get; set; }
        public bool blnSummariseItemsWhileEntering { get; set; }
        public bool blnWarnifSRatelessthanPrate { get; set; }
        public bool BLNALLOWDUPLICATELEDGERS { get; set; }
        public bool BLNCHEQUEDETAILS { get; set; }
        public bool BLNDISABLEANYWHERESEARCHINGOFLEDGERS { get; set; }
        public bool BLNDISPLAYLEDGERBALANCE { get; set; }
        public bool blnDualEntryMode { get; set; }
        public bool BLNENABLEEFFECIVEDATE { get; set; }
        public bool BLNPARTYDETAILS { get; set; }
        public bool BLNPOSTONEFFECTIVEDATE { get; set; }
        public bool CHEQUEPRINTING { get; set; }
        //public string PrintSettings { get; set; }

        public clsGetStockInVoucherSettings()
        {
        BLNEDITMRPRATE = false;
        blneditsalerate = false;
        blnEditTaxPer = false;
        blnenablecashdiscount = false;
        blnenableEffeciveDate = false;
        blnpartydetails = false;
        blnprintconfirmation = false;
        blnprintimmediately = false;
        BLNRECALCULATESalesRatesOnPercentage = false;
        blnshowbillnarration = false;
        BLNSHOWFREEQUANTITY = false;
        blnShowItemCalcGrid = false;
        blnShowItemProfitPer = false;
        blnshowotherexpense = false;
        blnshowpreview = false;
        blnShowRateFixer = false;
        blnShowReferenceNo = false;
        blnSummariseDuplicateItemsInPrint = false;
        blnSummariseItemsWhileEntering = false;
        blnWarnifSRatelessthanPrate = false;
        BLNALLOWDUPLICATELEDGERS = false;
        BLNCHEQUEDETAILS = false;
        BLNDISABLEANYWHERESEARCHINGOFLEDGERS = false;
        BLNDISPLAYLEDGERBALANCE = false;
        blnDualEntryMode = false;
        BLNENABLEEFFECIVEDATE = false;
        BLNPARTYDETAILS = false;
        BLNPOSTONEFFECTIVEDATE = false;
        CHEQUEPRINTING = false;
            //PrintSettings = "";
    }
}
}
