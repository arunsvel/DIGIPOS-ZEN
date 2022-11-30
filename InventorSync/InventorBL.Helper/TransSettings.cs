using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.InventorBL.Helper
{
    public class TransSettings
    {
        public int  IntSmartSearchBehavourMode
        {
            get;
            set;
        }
        public int  intEnterKeyBehavourMode
        {
            get;
            set;
        }
        public int  IntdefaultFocusColumnID
        {
            get;
            set;
        }
        public bool  BlnBillDiscAmtEntry
        {
            get;
            set;
        }
        public int  BlntouchScreen
        {
            get;
            set;
        }
        public bool BlnHideZeroQtyFromVirtuaSearch
        {
            get;
            set;
        }
        public bool blnShowSecondScreen
        {
            get;
            set;
        }
        public int  Numberingmode
        {
            get;
            set;
        }
        public bool BlnPreserveOriginalInvNo
        {
            get;
            set;
        }
        public bool BlnLoaded
        {
            get;
            set;
        }
        public int  intDefGodownID
        {
            get;
            set;
        }
        public int  ConsolidatedLedgerID
        {
            get;
            set;
        }
        public bool BLNSCANNER
        {
            get;
            set;
        }
        public bool blnBlockDuplicate
        {
            get;
            set;
        }
        public bool BlnCaptureQty
        {
            get;
            set;
        }
        public bool BlnEditMode
        {
            get;
            set;
        }
        public string  StrgridColor
        {
            get;
            set;
        }
        public string  StrgridHeaderColor
        {
            get;
            set;
        }
        public string  ExtraAccountsGroupIDS
        {
            get;
            set;
        }
        public string  defaultcriteria
        {
            get;
            set;
        }
        public string  CreditGroupIDs
        {
            get;
            set;
        }
        public string  DebitGroupIDs
        {
            get;
            set;
        }
        public string  Prefix
        {
            get;
            set;
        }
        public string  Suffix
        {
            get;
            set;
        }
        public bool blnMobileVoucher
        {
            get;
            set;
        }
        public string  BCstring
        {
            get;
            set;
        }
        public string  QRCodestring
        {
            get;
            set;
        }
        public string  QRCodeSplitChar
        {
            get;
            set;
        }

        public string  InvSqlInjectionCodestring
        {
            get;
            set;
        }
        public string  InvSqlInjectionSplitChar
        {
            get;
            set;
        }


        public bool PD_BlnPoleDisplayEnabled
        {
            get;
            set;
        }
        public string  PD_Port
        {
            get;
            set;
        }
        public int  PD_BitPerSecond
        {
            get;
            set;
        }
        public int  PD_DataBit
        {
            get;
            set;
        }
        public string  PD_Parity
        {
            get;
            set;
        }
        public int  PD_BitStop
        {
            get;
            set;
        }
        public string  PD_FlowControl
        {
            get;
            set;
        }
        public int  PD_CharacterWidth
        {
            get;
            set;
        }
        public string  PD_TestMessage
        {
            get;
            set;
        }


        public bool WM_BlnWeighingMachineEnabled
        {
            get;
            set;
        }
        public string  WM_Port
        {
            get;
            set;
        }
        public int  WM_BitPerSecond
        {
            get;
            set;
        }
        public int  WM_DataBit
        {
            get;
            set;
        }
        public string  WM_Parity
        {
            get;
            set;
        }
        public string  WM_BitStop
        {
            get;
            set;
        }
        public string  WM_FlowControl
        {
            get;
            set;
        }

        public int  IntFillItemDiscountNonePMC
        {
            get;
            set;
        }
        public int  IntFillBillDiscountNoneCA
        {
            get;
            set;
        }


        public bool BlnEnableCustomFormColor
        {
            get;
            set;
        }
        public int  returnvchtypeID
        {
            get;
            set;
        }


        public string  MVchType
        {
            get;
            set;
        }
        public int  MVchTypeID
        {
            get;
            set;
        }
        public int  MParentVchTypeID
        {
            get;
            set;
        }

        //------Color
        public string  WindowBackColor
        {
            get;
            set;
        }
        public string  ContrastBackColor
        {
            get;
            set;
        }
        public string  GridColor
        {
            get;
            set;
        }
        public string  GridHeaderColor
        {
            get;
            set;
        }
        public string  GridselectedRow
        {
            get;
            set;
        }
        public string  GridHeaderFont
        {
            get;
            set;
        }
        public string  GridBackColor
        {
            get;
            set;
        }
        public string  GridAlternatCellColor
        {
            get;
            set;
        }
        public string  GridCellColor
        {
            get;
            set;
        }
        public string  GridFontColor
        {
            get;
            set;
        }

        public bool blnCancelled
        {
            get;
            set;
        }

        public string  mstrOldData
        {
            get;
            set;
        }
        public int  MaxCreditdays
        {
            get;
            set;
        }
        public int  MaxCreditBills
        {
            get;
            set;
        }
        public int  IntRoundOffMode
        {
            get;
            set;
        }
        Double DBLRoundOffBlock
        {
            get;
            set;
        }

        public bool blnHomeCountry
        {
            get;
            set;
        }

        public bool blnDisablebegintrans
        {
            get;
            set;
        }

        public string  StrItemClassIDS2
        {
            get;
            set;
        }
        public string  strSecondaryCCIDS
        {
            get;
            set;
        }
        public string  strPrimaryCCIDS
        {
            get;
            set;
        }


        public string  strOrderVchTypeIDS
        {
            get;
            set;
        }
        public string  strNoteVchTypeIDS
        {
            get;
            set;
        }
        public string  strQuotationVchTypeIDS
        {
            get;
            set;
        }


        public int  intDEFMOPID
        {
            get;
            set;
        }

        public bool BLNLOCKMOP
        {
            get;
            set;
        }
        public int  intDEFTAXMODEID
        {
            get;
            set;
        }
        public bool BLNLOCKTAXMODE
        {
            get;
            set;
        }
        public int  intDEFAGENTID
        {
            get;
            set;
        }
        public bool BLNLOCKAGENT
        {
            get;
            set;
        }
        public int  intDEFPRICELISTID
        {
            get;
            set;
        }
        public bool BLNLOCKPRICELIST
        {
            get;
            set;
        }
        public int  DEFSALESMANID
        {
            get;
            set;
        }
        public bool BLNLOCKSALESMAN
        {
            get;
            set;
        }
        public bool DEFPRINTSCH
        {
            get;
            set;
        }


        public int  IntPRINTSCHID
        {
            get;
            set;
        }
        public int  IntPRINTSCHID2
        {
            get;
            set;
        }
        public bool BLNLOCKDATE
        {
            get;
            set;
        }
        public bool BLNLOCKPRINT
        {
            get;
            set;
        }
        public bool BLNLOCKPRINT2
        {
            get;
            set;
        }

        public bool BlnBillWiseDisc
        {
            get;
            set;
        }
        public bool BlnItemWisePerDisc
        {
            get;
            set;
        }
        public bool BlnItemWiseAmtDisc
        {
            get;
            set;
        }
        public string  strsmartsearchQuery
        {
            get;
            set;
        }
        public string[] StrColWidth;

        public bool blnCalcQtyTotal
        {
            get;
            set;
        }
        public bool blnCalcGrossAmt
        {
            get;
            set;
        }
        public bool blnCalcGrossAfterRateDiscount
        {
            get;
            set;
        }
        public bool blnCalcRateDiscountTotal
        {
            get;
            set;
        }
        public bool blnCalcGrossAfterBillDiscount
        {
            get;
            set;
        }
        public bool blnCalcGrossAfterItemDiscount
        {
            get;
            set;
        }
        public bool blnCalcItemDiscountTotal
        {
            get;
            set;
        }
        public bool blnCalcTaxableAmount
        {
            get;
            set;
        }
        public bool blnCalcNonTaxableAmount
        {
            get;
            set;
        }
        public bool blnCalcTaxAmount
        {
            get;
            set;
        }
        public bool blnCalcVatTotal
        {
            get;
            set;
        }
        public bool blnCalcINTERSTATE
        {
            get;
            set;
        }
        public bool blnCalcCGST
        {
            get;
            set;
        }
        public bool blnCalcSGST
        {
            get;
            set;
        }
        public bool blnCalcIGST
        {
            get;
            set;
        }
        public bool blnCalcCessAmount
        {
            get;
            set;
        }
        public bool blnfloodCessTot
        {
            get;
            set;
        }
        public bool blnCalcQtyCompCessAmount
        {
            get;
            set;
        }
        public bool blnCalcNetAmount
        {
            get;
            set;
        }
        public bool blnCalcAgentCommission
        {
            get;
            set;
        }
        public bool blnCalcCoolie
        {
            get;
            set;
        }
        public bool BlnShowSavings
        {
            get;
            set;
        }


        public bool BLNALLOWDUPLICATEITEMS
        {
            get;
            set;
        }
        public bool BLNALLOWDUPLICATELEDGERS
        {
            get;
            set;
        }
        public bool BLNANYWHEREITEMSEARCH
        {
            get;
            set;
        }
        public bool BLNAUTOCHANGERATEONPRICELIST
        {
            get;
            set;
        }
        public bool BLNBATCHWITHQOH
        {
            get;
            set;
        }
        public bool BLNCHEQUEDETAILS
        {
            get;
            set;
        }
        public bool BLNCUSTOMERRATES
        {
            get;
            set;
        }


        public bool BLNEDITMRPRATE
        {
            get;
            set;
        }
        public bool BLNEDITSALERATE
        {
            get;
            set;
        }
        public bool BLNENABLECASHDISCOUNT
        {
            get;
            set;
        }
        public bool BLNPOSTONEFFECTIVEDATE
        {
            get;
            set;
        }
        public bool BLNCASHDESK
        {
            get;
            set;
        }
        public bool BLNENABLERATEDISCOUNT
        {
            get;
            set;
        }
        public bool BLNFOCUSTOFIRSTCOLUMN
        {
            get;
            set;
        }
        public bool BLNPARTYDETAILS
        {
            get;
            set;
        }
        public bool blnShowEfectiveDate
        {
            get;
            set;
        }
        public bool BLNPRINTCONFIRMATION
        {
            get;
            set;
        }
        public bool blnShowOfferinPopup
        {
            get;
            set;
        }
        public bool blnShowRateFixer
        {
            get;
            set;
        }

        public bool blnSearchButtonDisplayTodaysEntryOnly
        {
            get;
            set;
        }
        public bool blnShowReferenceNo
        {
            get;
            set;
        }
        public bool BLNRECALCULATESalesRatesOnPercentage
        {
            get;
            set;
        }
        public bool BlnEnableTracking
        {
            get;
            set;
        }
        public bool BlnAllowExpiredStock
        {
            get;
            set;
        }
        public bool blnSummariseItemsWhileEntering
        {
            get;
            set;
        }
        public bool blnShowItemProfitPer
        {
            get;
            set;
        }
        public int  IntPrintCopies
        {
            get;
            set;
        }
        public bool BLNCASHDRAWER
        {
            get;
            set;
        }
        public bool BLNHOLDBILL
        {
            get;
            set;
        }
        public bool BLNDELETEBILL
        {
            get;
            set;
        }
        public bool BLNCANCELBILL
        {
            get;
            set;
        }
        public bool intnegativeAWB
        {
            get;
            set;
        }
        public bool BLNCASHANDCARD
        {
            get;
            set;
        }
        public bool BLNSHOWPARTYDETAILS
        {
            get;
            set;
        }
        public bool BLNPRINTIMMEDIATELY
        {
            get;
            set;
        }

        public bool BLNRESTRICTREPRINT
        {
            get;
            set;
        }
        public bool BLNSHOWBILLNARRATION
        {
            get;
            set;
        }
        public bool blnshowQty
        {
            get;
            set;
        }
        public bool BLNSHOWCUSTOMERPOINTS
        {
            get;
            set;
        }
        public bool BLNSHOWEXTRAACCOUNTS
        {
            get;
            set;
        }
        public bool BLNSHOWFREEQUANTITY
        {
            get;
            set;
        }
        public bool BLNAPPLYOFFER
        {
            get;
            set;
        }
        public bool BLNAPPLYGIFTVOUCHER
        {
            get;
            set;
        }
        public bool blnShowItemCalcGrid
        {
            get;
            set;
        }
        public bool BLNSHOWLEDGERBALANCES
        {
            get;
            set;
        }
        public bool BLNSHOWOTHEREXPENSE
        {
            get;
            set;
        }
        public bool blnEditTaxPer
        {
            get;
            set;
        }
        public bool blnAutoSendSMS
        {
            get;
            set;
        }
        public bool blnAutoSendSMSOnCustomerSelection
        {
            get;
            set;
        }
        public bool blnSummariseDuplicateItemsInPrint
        {
            get;
            set;
        }
        public bool BLNSHOWPREVIEW
        {
            get;
            set;
        }
        public bool blnNameandMObilenoMandatory
        {
            get;
            set;
        }
        public bool BLNEnableBarCodeWeighingMachine
        {
            get;
            set;
        }
        public string  StrBarCodeWeighingMachineValue
        {
            get;
            set;
        }
        public bool BLNSHOWPRICELIST
        {
            get;
            set;
        }
        public bool BLNSHOWPROFIT
        {
            get;
            set;
        }
        public bool BLNSUMMARISEDUPLICATEITEMS
        {
            get;
            set;
        }
        public bool BLNSUMMARISEITEMS
        {
            get;
            set;
        }
        public bool CHEQUEPRINTING
        {
            get;
            set;
        }
        public bool BLNPOSADVANCED
        {
            get;
            set;
        }
        public bool BLNPOSCALCULATION
        {
            get;
            set;
        }
        public bool BLNHIDEZEROQTY
        {
            get;
            set;
        }
        public bool blnAutoFillRawMaterial
        {
            get;
            set;
        }
        public bool blnRejectZeroValueItem
        {
            get;
            set;
        }
        public bool blnRejectZeroValueBill
        {
            get;
            set;
        }
        public bool blnKFCEExclusiveOfTax
        {
            get;
            set;
        }
        public bool blnshowscrap
        {
            get;
            set;
        }
    }
}
