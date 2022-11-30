using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using InventorSync.InventorBL.Helper;
using InventorSync.InventorBL.Master;
using Microsoft.VisualBasic;

namespace InventorSync.JsonClass
{
    public class clsJSonCommon
    {
        Common Comm = new Common();
        //Changed By Dipu
        public clsJsonVoucherType GetVoucherType(int iVchtypID = 0)
        {
            string sVchJson = "";
            clsJsonVoucherType clsVchTyp = new clsJsonVoucherType();
            DataTable dtVchFeatures = new DataTable();
            DataTable dtVoucherTp = Comm.fnGetData("SELECT VchJson FROM tblVchType WHERE VchTypeID = " + iVchtypID + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
            if (dtVoucherTp.Rows.Count > 0)
            {
                sVchJson = dtVoucherTp.Rows[0]["VchJson"].ToString();
                clsVchTyp = JsonConvert.DeserializeObject<clsJsonVoucherType>(sVchJson);
            }
            if (clsVchTyp == null) 
                return new clsJsonVoucherType();
            else
                return clsVchTyp;
        }
        //Changed By Dipu
        public clsGetStockInVoucherSettings GetVoucherTypeGeneralSettings(int iVchtypID = 0, int iUserGrpId = 0)
        {
            clsGetStockInVoucherSettings clsGetSetting = new clsGetStockInVoucherSettings();
            DataTable dtGet = new DataTable();
            dtGet = Comm.fnGetData("SELECT distinct SettingsName,BlnEnabled FROM tblvchtypeGenSettings WHERE vchtypeID=" + iVchtypID + " AND UserID = " + iUserGrpId + " order by SettingsName").Tables[0];
            if (dtGet.Rows.Count > 0)
            {
                for (int i = 0; i < dtGet.Rows.Count; i++)
                {
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNPOSTCASHENTRY") clsGetSetting.BLNPOSTCASHENTRY = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNEDITMRPRATE") clsGetSetting.BLNEDITMRPRATE = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNEDITSALERATE") clsGetSetting.blneditsalerate = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNEDITTAXPER") clsGetSetting.blnEditTaxPer = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNENABLECASHDISCOUNT") clsGetSetting.blnenablecashdiscount = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNENABLEEFFECIVEDATE") clsGetSetting.blnenableEffeciveDate = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNPARTYDETAILS") clsGetSetting.blnpartydetails = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNRECALCULATESALESRATESONPERCENTAGE") clsGetSetting.BLNRECALCULATESalesRatesOnPercentage = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNSHOWBILLNARRATION") clsGetSetting.blnshowbillnarration = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNSHOWFREEQUANTITY") clsGetSetting.BLNSHOWFREEQUANTITY = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNSHOWITEMCALCGRID") clsGetSetting.blnShowItemCalcGrid = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNSHOWITEMPROFITPER") clsGetSetting.blnShowItemProfitPer = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNSHOWOTHEREXPENSE") clsGetSetting.blnshowotherexpense = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNSHOWPREVIEW") clsGetSetting.blnshowpreview = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNPRINTIMMEDIATELY".ToUpper()) clsGetSetting.blnprintimmediately = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNPRINTCONFIRMATION") clsGetSetting.blnprintconfirmation = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));

                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNSHOWRATEFIXER") clsGetSetting.blnShowRateFixer = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNSHOWREFERENCENO") clsGetSetting.blnShowReferenceNo = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNSUMMARISEITEMSWHILEENTERING") clsGetSetting.blnSummariseItemsWhileEntering = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNWARNIFSRATELESSTHANPRATE") clsGetSetting.blnWarnifSRatelessthanPrate = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "blnDualEntryMode".ToUpper()) clsGetSetting.blnDualEntryMode = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));

                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNALLOWDUPLICATELEDGERS".ToUpper()) clsGetSetting.BLNALLOWDUPLICATELEDGERS = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNCHEQUEDETAILS".ToUpper()) clsGetSetting.BLNCHEQUEDETAILS = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNDISABLEANYWHERESEARCHINGOFLEDGERS".ToUpper()) clsGetSetting.BLNDISABLEANYWHERESEARCHINGOFLEDGERS = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNDISPLAYLEDGERBALANCE".ToUpper()) clsGetSetting.BLNDISPLAYLEDGERBALANCE = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNENABLEEFFECIVEDATE".ToUpper()) clsGetSetting.BLNENABLEEFFECIVEDATE = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNPARTYDETAILS".ToUpper()) clsGetSetting.BLNPARTYDETAILS = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "BLNPOSTONEFFECTIVEDATE".ToUpper()) clsGetSetting.BLNPOSTONEFFECTIVEDATE = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));
                    if (dtGet.Rows[i]["SettingsName"].ToString().ToUpper() == "CHEQUEPRINTING".ToUpper()) clsGetSetting.CHEQUEPRINTING = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["BlnEnabled"].ToString()));

                }
                if (clsGetSetting == null)
                    return new clsGetStockInVoucherSettings();
                else
                    return clsGetSetting;
            }
            else
            {
                if (clsGetSetting == null)
                    return new clsGetStockInVoucherSettings();
                else
                    return clsGetSetting;
            }
        }

        public double RoundOffAmount(double Amount, int NoneNormalUpDown0123 = 4, double MinRndLimit = 0, int DecPlc = 2)
        {
            ;/* Cannot convert OnErrorResumeNextStatementSyntax, CONVERSION ERROR: Conversion for OnErrorResumeNextStatement not implemented, please report this issue in 'On Error Resume Next' at character 205
   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.DefaultVisit(SyntaxNode node)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitOnErrorResumeNextStatement(OnErrorResumeNextStatementSyntax node)
   at Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

Input: 
        On Error Resume Next

 */
            double ReturnValue = 0;
            string MyCurrencyFormat;
            string DecimalPart;
            double IntPart;
            double ModVal;
            double TempMinRndLimit;
            // None = 0
            // Normal = 1
            // upward = 2
            // downward = 3
            // auto = 4


            // 111 TO ROUND TO .125 AND .131 TO .150 N 156 TO .175 N 188 TO 200
            // 211 TO ROUND TO .225 AND .231 TO .250 N 256 TO .275 N 288 TO 300
            // 311 TO ROUND TO .325 AND .331 TO .350 N 356 TO .375 N 388 TO 400
            // 411 TO ROUND TO .425 AND .431 TO .450 N 456 TO .475 N 488 TO 500
            // 511 TO ROUND TO .525 AND .531 TO .550 N 556 TO .575 N 588 TO 600
            // 611 TO ROUND TO .625 AND .631 TO .650 N 656 TO .675 N 688 TO 700
            // 711 TO ROUND TO .725 AND .731 TO .750 N 756 TO .775 N 788 TO 800
            // 811 TO ROUND TO .825 AND .831 TO .850 N 856 TO .875 N 888 TO 900
            // 911 TO ROUND TO .925 AND .931 TO .950 N 956 TO .975 N 988 TO 1



            TempMinRndLimit = MinRndLimit;
            if (MinRndLimit == 0)
                MinRndLimit = 0.5;

            MyCurrencyFormat = "#0" + Interaction.IIf(DecPlc > 0, ".", "") + Strings.Replace(Strings.Space(DecPlc), " ", "0");

            if (NoneNormalUpDown0123 == 4)
            {
                // when parameters are not specified
                NoneNormalUpDown0123 = 1;
                MyCurrencyFormat = "#0.00";
                MinRndLimit = 0.50; // gStgDblRoundOffAmt
            }

            if (Amount == 0)
            {
                ReturnValue = 0;
                return ReturnValue;
            }

            DecimalPart = ((Conversion.Val(Amount) - Conversion.Int(Conversion.Val(Amount))) * Math.Pow(10, DecPlc)).ToString();  // Converting Fractional Part into Integer
            if (MinRndLimit < 5 & Convert.ToDouble( DecimalPart ) == 0)
            {
                ReturnValue = Amount;
                return ReturnValue;
            }

            switch (NoneNormalUpDown0123)
            {
                case 0:
                    {
                        ReturnValue = Convert.ToDouble(Microsoft.VisualBasic.Strings.Format(Amount, MyCurrencyFormat));
                        break;
                    }

                case 1:
                    {
                        DecimalPart = ((Conversion.Val(Amount) - Conversion.Int(Conversion.Val(Amount))) * Math.Pow(10, DecPlc)).ToString(); // Converting Fractional Part into Integer
                        IntPart = Convert.ToDouble(Strings.Left(System.Convert.ToString(Amount), Strings.InStr(1, System.Convert.ToString(Amount), ".") - 1));
                        // IntPart = CLng(Val(Amount))
                        // 50 paise round off
                        // MinRndLimit is ignored
                        if (Convert.ToDouble(DecimalPart) == 0)
                            ReturnValue = Amount;
                        else
                        {
                            // If DCSApp.BlnMiddleEastCurrency = False Then
                            double Addfactor = 0;
                            string DecimalBlock = "0";
                            string wholepart = "";
                            SplitDecimal(MinRndLimit.ToString(), ref wholepart, ref DecimalBlock, 2);
                            // DecimalBlock = MinRndLimit
                            if (MinRndLimit < 1)
                            {
                                for (var i = 1; i <= Conversion.Val(1 / MinRndLimit); i++)
                                {
                                    if (Conversion.Val(DecimalPart) < (Conversion.Val(DecimalBlock) * i))
                                    {
                                        // need to check if half fo decimal achieved or not
                                        int MinX = Convert.ToInt16(Conversion.Val(DecimalBlock) * (i - 1));
                                        int MaxX = Convert.ToInt16(Conversion.Val(DecimalBlock) * (i));
                                        if ((MinX + (Conversion.Val(DecimalBlock) / (double)2)) > Conversion.Val(DecimalPart))
                                            Addfactor = MinX / (double)100;
                                        else
                                            Addfactor = MaxX / (double)100;
                                        break;
                                    }
                                }
                                // If DecimalPart > DecimalBlock Then
                                // Addfactor = DecimalBlock * 2 / 100 '
                                // ElseIf DecimalPart <= DecimalBlock Then
                                // Addfactor = 0
                                // ElseIf DecimalPart = DecimalBlock Then
                                // Addfactor = 0.5
                                // End If
                                ReturnValue = Convert.ToDouble(Microsoft.VisualBasic.Strings.Format(IntPart + Addfactor, MyCurrencyFormat));
                            }
                            else if (MinRndLimit == 1)
                            {
                                // Switch(DecimalPart > 50, 1, DecimalPart <= 50, 0)
                                if (Conversion.Val(DecimalPart) > 50)
                                    Addfactor = 1;
                                else if (Conversion.Val(DecimalPart) <= 50)
                                    Addfactor = 0;
                                ReturnValue = Convert.ToDouble(Microsoft.VisualBasic.Strings.Format(IntPart + Addfactor, MyCurrencyFormat));
                            }
                            else if (MinRndLimit == 2)
                            {
                                if (Conversion.Val(DecimalPart) >= 50)
                                    Addfactor = 1;
                                else if (Conversion.Val(DecimalPart) < 50)
                                    Addfactor = 0;
                                ReturnValue = Convert.ToDouble(Microsoft.VisualBasic.Strings.Format(IntPart + Addfactor, MyCurrencyFormat));
                            }
                            else
                                ReturnValue = Convert.ToDouble(Microsoft.VisualBasic.Strings.Format(Amount, MyCurrencyFormat));
                        }

                        break;
                    }

                case 2:
                    {
                        // MinRndLimit is considered
                        // Dim DecimalPart As Long
                        string tempstr;
                        DecimalPart = System.Convert.ToString((Conversion.Val(Amount) - Conversion.Int(Conversion.Val(Amount))) * Math.Pow(10, DecPlc));  // Converting Fractional Part into Integer
                        if (Conversion.Val(DecimalPart) == 0)
                            Amount = Amount + Conversion.Val(".0000000001");
                        // IntPart = CLng(Val(Amount))
                        IntPart = Conversion.Val(Strings.Left(System.Convert.ToString(Amount), Strings.InStr(1, System.Convert.ToString(Amount), ".") - 1));
                        if (MinRndLimit > 1)
                        {
                            tempstr = Microsoft.VisualBasic.Strings.Format(Amount / MinRndLimit, MyCurrencyFormat);
                            string comparevalue = Microsoft.VisualBasic.Strings.Format(Amount - (Conversion.Val(tempstr) * MinRndLimit), MyCurrencyFormat);

                            ModVal = (double)Interaction.IIf(Conversion.Val(comparevalue) >= 1, Convert.ToDouble(1), Convert.ToDouble(0));
                            if (ModVal != 0)
                                ReturnValue = Convert.ToDouble(Microsoft.VisualBasic.Strings.Format(Amount + (MinRndLimit - (Amount - (Conversion.Val(tempstr) * MinRndLimit))), MyCurrencyFormat));
                            else if (Strings.InStr(1, tempstr, ".", Constants.vbTextCompare) > 0)
                                ReturnValue = Convert.ToDouble(Microsoft.VisualBasic.Strings.Format(MinRndLimit * (Conversion.Val(tempstr) + (double)Interaction.IIf((Conversion.Val(tempstr) - ((int)Conversion.Val(tempstr))) > 0, Convert.ToDouble(1), Convert.ToDouble(0))), MyCurrencyFormat));
                            else
                                ReturnValue = Convert.ToDouble(Microsoft.VisualBasic.Strings.Format(IntPart, MyCurrencyFormat));
                        }
                        else
                        {
                            tempstr = Microsoft.VisualBasic.Strings.Format(Amount / MinRndLimit, MyCurrencyFormat);
                            ModVal = Conversion.Val("0." + Strings.Mid(tempstr, Strings.InStr(tempstr, ".") + 1, Strings.Len(tempstr) - Strings.InStr(tempstr, ".")));
                            if (Strings.InStr(1, tempstr, ".", CompareMethod.Text) > 0)
                                ReturnValue = Convert.ToDouble(Microsoft.VisualBasic.Strings.Format(IntPart + (double)Interaction.IIf(Conversion.Val("0." + Conversion.Val(Conversion.Int(DecimalPart)).ToString()) < MinRndLimit, MinRndLimit, (MinRndLimit * 2)), MyCurrencyFormat));
                        }

                        break;
                    }

                case 3:
                    {
                        // seeking downward roundoff
                        string tempstr;
                        DecimalPart = (System.Convert.ToDouble(Conversion.Val(Amount) - Conversion.Int(Conversion.Val(Amount))) * Math.Pow(10, DecPlc)).ToString();  // Converting Fractional Part into Integer
                                                                                                                                                        // IntPart = Int(Val(Amount))
                        if (Conversion.Val(DecimalPart) == 0)
                            Amount = Amount + Conversion.Val(".0000000001");
                        IntPart = Conversion.Val(Strings.Left(System.Convert.ToString(Amount), Strings.InStr(1, System.Convert.ToString(Amount), ".") - 1));
                        if (Conversion.Val(DecimalPart) == 0)
                            Amount = Amount - 0.0000000001;
                        if (MinRndLimit > 1)
                        {
                            tempstr = Microsoft.VisualBasic.Strings.Format(Amount / MinRndLimit, (Interaction.IIf(Conversion.Val(DecimalPart) == 0, "00.00", MyCurrencyFormat)).ToString());
                            // Modval = Amount Mod MinRndLimit

                            ModVal = Conversion.Val(Interaction.IIf(Conversion.Val(Microsoft.VisualBasic.Strings.Format(Amount - (Conversion.Val(tempstr) * MinRndLimit), MyCurrencyFormat)) >= 1, 1, 0));
                            if (ModVal != 0)
                                ReturnValue = Conversion.Val(Microsoft.VisualBasic.Strings.Format(Math.Round((Amount - Amount % MinRndLimit), 0), MyCurrencyFormat).ToString());
                            else if (Strings.InStr(1, tempstr, ".", Constants.vbTextCompare) > 0)
                                ReturnValue = Conversion.Val(Microsoft.VisualBasic.Strings.Format(MinRndLimit * (Conversion.Val(tempstr) + (double)Interaction.IIf((Conversion.Val(tempstr) - ((int)Conversion.Val(tempstr))) > 0, Convert.ToDouble(0), Convert.ToDouble(1))), MyCurrencyFormat).ToString());
                            else
                                ReturnValue = Conversion.Val(Microsoft.VisualBasic.Strings.Format(IntPart, MyCurrencyFormat).ToString());

                            if (Conversion.Val(Microsoft.VisualBasic.Strings.Format(Amount - (Conversion.Val(tempstr) * MinRndLimit), MyCurrencyFormat)) == 0)
                                ReturnValue = Conversion.Val(Microsoft.VisualBasic.Strings.Format(IntPart, MyCurrencyFormat));
                        }
                        else
                        {
                            tempstr = Microsoft.VisualBasic.Strings.Format(Amount / MinRndLimit, MyCurrencyFormat);
                            ModVal = Conversion.Val("0." + Strings.Mid(tempstr, Strings.InStr(tempstr, ".") + 1, Strings.Len(tempstr) - Strings.InStr(tempstr, ".")));
                            if (Strings.InStr(1, tempstr, ".", CompareMethod.Text) > 0)
                                ReturnValue = Convert.ToDouble(Microsoft.VisualBasic.Strings.Format(IntPart - (double)Interaction.IIf(Conversion.Val("0." + Conversion.Val(Conversion.Int(DecimalPart)).ToString()) <= MinRndLimit, Convert.ToDouble(0), -MinRndLimit), MyCurrencyFormat));
                            //ReturnValue = Convert.ToDouble(Microsoft.VisualBasic.Strings.Format(IntPart + (double)Interaction.IIf(Conversion.Val("0." + Conversion.Val(Conversion.Int(DecimalPart)).ToString()) < MinRndLimit, MinRndLimit, (MinRndLimit * 2)), MyCurrencyFormat));

                        }

                        break;
                    }

                case 4:
                    {
                        break;
                    }
            }

            return ReturnValue;

        }

        public void SplitDecimal(string number, ref string wholePart, ref string fractionalPart, int DecimalCount)
        {
            if (Strings.InStr(number, ".") > 0)
            {
                fractionalPart = ((Conversion.Val(number) - Conversion.Int(Conversion.Val(number))) * Math.Pow(10, DecimalCount)).ToString(); // Converting Fractional Part into Integer
                wholePart = Strings.Left(System.Convert.ToString(number), Strings.InStr(1, System.Convert.ToString(number), ".") - 1);
            }
            else
            {
                fractionalPart = "0";
                wholePart = System.Convert.ToString(number);
            }
        }

        //public List<clsJsonVchTypeFeatures> GetVoucherTypeFeatureList(int iVchtypID = 0, int iUserGrpId = 0)
        //{
        //    string sFeaturesJson = "";
        //    List<clsJsonVchTypeFeatures> lstVchTyp = new List<clsJsonVchTypeFeatures>();
        //    DataTable dtVchFeatures = new DataTable();
        //    DataTable dtVoucherTp = Comm.fnGetData("SELECT FeaturesJson FROM tblVchType WHERE VchTypeID = " + iVchtypID + " AND TenantID = " + Global.gblTenantID + "").Tables[0];
        //    if (dtVoucherTp.Rows.Count > 0)
        //    {
        //        sFeaturesJson = dtVoucherTp.Rows[0]["FeaturesJson"].ToString();
        //        lstVchTyp = JsonConvert.DeserializeObject<List<clsJsonVchTypeFeatures>>(sFeaturesJson);
        //        return lstVchTyp.Where(s => s.UserID.Equals(iUserGrpId)).ToList();
        //        //dtVchFeatures = lstVchTyp.ToDataTable();
        //    }
        //    else
        //        return null;
        //}

    }
}
