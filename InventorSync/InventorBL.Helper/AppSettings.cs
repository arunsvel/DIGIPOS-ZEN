using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.InventorBL.Helper
{
     public class AppSettings
    {
         public static string STRWMIDENTIFIER { get; set; }
         public static string STRWMBARCODELENGTH { get; set; }
         public static string STRWMQTYLENGTH { get; set; }
         public static string BarcodePrefix { get; set; }
         public static string MajorCurrency { get; set; }
         public static string MinorCurrency { get; set; }
         public static string MajorSymbol { get; set; }
         public static string MinorSymbol { get; set; }
         public static string CompAddress { get; set; }
         public static string CompName { get; set; }
         public static string CompanyCode { get; set; }
         public static bool TaxEnabled { get; set; }
         public static double Cess { get; set; }
         public static bool NeedToByDayBook { get; set; }
         public static double VerticalAccFormat { get; set; }
         public static double ThemeIndex { get; set; }
         public static bool AutoBackupOnLogin { get; set; }

         public static bool NeedAgent { get; set; }
         public static int CessMode { get; set; }
        
         public static bool BLNSRATEINC { get; set; }
         public static bool BLNPRATEINC { get; set; }

         public static string StateCode { get; set; }
         public static string CompGSTIN { get; set; }
         public static string ECommerceNo { get; set; }
         public static string AVAILABLETAXPER { get; set; }
         public static string CompStreet { get; set; }
         public static string CompContact { get; set; }
         public static string CompEmail { get; set; }

         public static int CurrencyDecimals { get; set; }
         public static int QtyDecimals { get; set; }

         public static DateTime FinYearStart { get; set; }
         public static DateTime FinYearEnd { get; set; }

         public static bool NeedTaxCollectSourcet { get; set; }
         public static bool BLNBARCODE { get; set; }
         public static bool NeedAdvanced { get; set; }
         public static bool NeedExternalDevConnt { get; set; }
         public static bool NeedOffersLoyalty { get; set; }
         public static bool NeedDiscGrouping { get; set; }
         public static bool NeedSize { get; set; }
         public static bool NeedColor { get; set; }
         public static bool NeedBrand { get; set; }
         public static bool NeedTheme { get; set; }
         public static string LakhsOrMillion { get; set; }

         public static bool NeedAutobackupOnExit { get; set; }
         public static bool NeedCostCenter { get; set; }
         public static string BackUpPath1 { get; set; }
         public static string BackUpPath2 { get; set; }
         public static string BackUpPath3 { get; set; }
         public static int CasingID { get; set; }

         public static string FormMainBackClr { get; set; }
         public static string FormHeadBackClr { get; set; }
         public static string FormFooterBackClr { get; set; }
         public static string FormLeftBackClr { get; set; }
         public static string FormRightBackClr { get; set; }
         public static string FormHeadTextClr { get; set; }

         public static string FormHighlight1Clr { get; set; }
         public static string FormHighlight2Clr { get; set; }
         public static string FormHighlight3Clr { get; set; }

         public static string GridBackClr { get; set; }
         public static string GridHeadBackClr { get; set; }
         public static string GridHeadTextClr { get; set; }
         public static string GridHeadTextBold { get; set; }
         public static string GridHeadTextFnt { get; set; }

         public static string GridAltBackClr { get; set; }
         public static string GridAltTextClr { get; set; }
         public static string GridAltTextBold { get; set; }
         public static string GridAltTextFnt { get; set; }

         public static string GridSelRwBackClr { get; set; }
         public static string GridSelRwTextClr { get; set; }
         public static string GridSelRwTextBold { get; set; }
         public static string GridSelRwTextFnt { get; set; }

         public static string GridNorRwTextClr { get; set; }
         public static string GridNorRwTextBold { get; set; }
         public static string GridNorRwTextFnt { get; set; }

         public static string FontforApplication { get; set; }
         public static string FormHeadingFntSiz { get; set; }
         public static string FormNorFntSiz { get; set; }
         public static string FormDescFntSiz { get; set; }

         public static int PLCALCULATION { get; set; }

         public static bool IsActiveSRate1 { get; set; }
         public static string SRate1Name { get; set; }
         public static bool IsActiveSRate2 { get; set; }
         public static string SRate2Name { get; set; }
         public static bool IsActiveSRate3 { get; set; }
         public static string SRate3Name { get; set; }
         public static bool IsActiveSRate4 { get; set; }
          public static string SRate4Name { get; set; }

        public static bool NeedCustArea { get; set; }

        //Added By anjitha 16-02-2022 04:30 PM
        public static bool IsActiveSRate5 { get; set; }
        public static string SRate5Name { get; set; }
        public static bool IsActiveMRP { get; set; }
        public static string MRPName { get; set; }
        public static int TaxMode { get; set; }

        public static string QtyDecimalFormat { get; set; }
        public static string CurrDecimalFormat { get; set; }
        public static bool BLNRECALCULATESalesRatesOnPercentage { get; set; }
    }
}
