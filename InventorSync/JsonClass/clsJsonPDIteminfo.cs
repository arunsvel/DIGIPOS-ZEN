using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.JsonClass
{
    public class clsJsonPDIteminfo
    {
        public decimal ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int CategoryID { get;  set; }
        public string Description { get; set; }
        public decimal PRate { get; set; }
        public int SrateCalcMode { get; set; }
        public decimal CRateAvg { get; set; }
        public decimal Srate1Per { get; set; }
        public decimal SRate1 { get; set; }
        public decimal Srate2Per { get; set; }
        public decimal SRate2 { get; set; }
        public decimal Srate3Per { get; set; }
        public decimal SRate3 { get; set; }
        public decimal Srate4 { get; set; }
        public decimal Srate4Per { get;set; }
        public decimal SRate5 { get; set; }
        public decimal Srate5Per { get; set; }
        public decimal MRP { get; set; }
        public decimal ROL { get; set; }
        public string Rack { get; set; }
        public string Manufacturer { get; set; }
        public int ActiveStatus { get; set; }
        public int IntLocal { get; set; }
        public string ProductType { get; set; }
        public decimal ProductTypeID { get; set; }
        public decimal LedgerID { get; set; }
        public decimal UNITID { get; set; }
        public string Notes { get; set; }
        public decimal agentCommPer { get; set; }
        public int BlnExpiryItem { get; set; }
        public int Coolie { get; set; }
        public int FinishedGoodID { get; set; }
        public decimal MinRate { get; set; }
        public decimal MaxRate { get; set; }
        public decimal PLUNo { get; set; }
        public int HSNID { get; set; }
        public decimal iCatDiscPer { get; set; }
        public decimal IPGDiscPer { get; set; }
        public decimal ImanDiscPer { get; set; }
        public string ItemNameUniCode { get; set; }
        public decimal Minqty { get; set; }
        public int MNFID { get; set; }
        public int PGID { get; set; }
        public string ItemCodeUniCode { get; set; }
        public string UPC { get; set; }
        public string BatchMode { get; set; }
        public int blnExpiry{ get; set; }
        public decimal Qty { get; set; }
        public decimal MaxQty { get; set; }
        public int IntNoOrWeight { get; set; }
        public string SystemName { get; set; }
        public decimal UserID { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public decimal TenantID { get; set; }
        public int blnCessOnTax { get; set; }
        public decimal CompCessQty { get; set; }
        public decimal CGSTTaxPer { get; set; }
        public decimal SGSTTaxPer { get; set; }
        public decimal IGSTTaxPer { get; set; }
        public decimal CessPer { get;  set; }
        public decimal VAT { get; set; }
        public string CategoryIDs { get; set; }
        public string ColorIDs { get; set; }
        public string SizeIDs { get; set; }
        public decimal BrandDisPer { get; set; }
        public int DGroupID { get; set; }
        public decimal DGroupDisPer { get; set; }
        public string BatchCode { get; set; }
        public decimal CostRateInc { get; set; }
        public decimal CostRateExcl { get; set; }
        public decimal PRateExcl { get; set; }
        public decimal PrateInc { get; set;  }
        public int BrandID { get; set; }
        public decimal AltUnitID { get; set; }
        public decimal ConvFactor { get; set; }
        public decimal Shelflife { get; set; }
        public decimal SRIncl { get; set; }
        public decimal PRIncl { get; set; }
        public decimal SlabSys { get; set; }
        public decimal SRateInclusive { get; set; }
        public decimal PRateInclusive { get; set; }
        public decimal Slabsys { get; set; }
        public decimal ParentID { get; set; }
        public decimal ParentConv { get; set; }
        public int blnParentEqlRate { get; set; }
        public string ItmConvType { get; set; }
        public decimal DiscPer { get; set; }
    }
}
