using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using System.Drawing;
using System.Globalization;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO.Compression;
using DigiposZen.Info;
using DigiposZen.InventorBL.Master;

namespace DigiposZen.InventorBL.Helper
{
    public class Common : DBConnection
    {
        public enum PermissionType
        {
            View,
            New,
            Edit,
            Archive,
            Delete,
            Print,
            DateChange
        }

        public enum UserActivity
        { 
            new_Entry = 1,
            UpdateEntry = 2,
            Delete_Entry = 3,
            CancelEntry = 4,
            DisplayWindow = 5,
            Printinvoice = 6,
            DateChange = 7,
            WaitForAuthorisation = 8,
            LoggedIn = 9,
            Loggedout = 10
        }

        public bool DBUpdate()
        {
            try
            {
                string strdt = "01/Jan/1999";
                DateTime dt = DateTime.Today; //Last DBUpdateDateTime
                strdt = RetieveFromDBInAppSettings(Global.gblTenantID, "DBUPDATEDATE");
                if (strdt != null)
                    if (strdt == "") strdt = "01/Jan/1999";
                
                dt = Convert.ToDateTime(strdt);

                if (dt == Convert.ToDateTime("13/Dec/2022"))
                    return false;

                SaveInAppSettings("DBUPDATEDATE", "13/Dec/2022");
            }
            catch
            { }
            
            string sQuery = "";

            try
            {
                sQuery = @"update tblbrand set brandName='DEFAULT', brandShortName='DEFAULT' where brandID = 1";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblhsncode add [TenantID] [numeric](18, 0) NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [HSNType] [varchar](200) NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add 	[CGSTTaxPer] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add 	[SGSTTaxPer] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [IGSTTaxPer] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [CessPer] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [CompCessQty] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [HID] [int] IDENTITY(1,1) NOT NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [CGSTTaxPer1] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [SGSTTaxPer1] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [IGSTTaxPer1] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [CGSTTaxPer2] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [SGSTTaxPer2] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [IGSTTaxPer2] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [CGSTTaxPer3] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [SGSTTaxPer3] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [IGSTTaxPer3] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [CGSTTaxPer4] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [SGSTTaxPer4] [float] NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"alter table tblhsncode add [IGSTTaxPer4] [float] NULL";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"UPDATE tblhsncode SET [TenantID] = 1 WHERE [TenantID] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [HSNType] = 'GOODS' WHERE [HSNType] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [CGSTTaxPer] = 0 WHERE [CGSTTaxPer] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [SGSTTaxPer] = 0 WHERE [SGSTTaxPer] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [IGSTTaxPer] = 0 WHERE [IGSTTaxPer] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [CessPer] = 0 WHERE [CessPer] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [CompCessQty] = 0 WHERE [CompCessQty] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [CGSTTaxPer1] = 0 WHERE [CGSTTaxPer1] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [SGSTTaxPer1] = 0 WHERE [SGSTTaxPer1] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [IGSTTaxPer1] = 0 WHERE [IGSTTaxPer1] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [CGSTTaxPer2] = 0 WHERE [CGSTTaxPer2] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [SGSTTaxPer2] = 0 WHERE [SGSTTaxPer2] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [IGSTTaxPer2] = 0 WHERE [IGSTTaxPer2] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [CGSTTaxPer3] = 0 WHERE [CGSTTaxPer3] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [SGSTTaxPer3] = 0 WHERE [SGSTTaxPer3] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [IGSTTaxPer3] = 0 WHERE [IGSTTaxPer3] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [CGSTTaxPer4] = 0 WHERE [CGSTTaxPer4] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [SGSTTaxPer4] = 0 WHERE [SGSTTaxPer4] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [IGSTTaxPer4] = 0 WHERE [IGSTTaxPer4] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"UPDATE tblhsncode SET [HID] = HSNID WHERE [HID] IS NULL";
                fnExecuteNonQuery(sQuery, false);

                try
                {
                    sQuery = @"UPDATE tblhsncode SET [HID] = HSNCODE WHERE [HID] IS NULL";
                    fnExecuteNonQuery(sQuery, false);
                }
                catch
                { }

                sQuery = @"UPDATE tblhsncode SET [HID] = 0 WHERE [HID] IS NULL";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"INSERT INTO [dbo].[tblHSNCode]
                        ([HSNID],[HSNCODE],[HSNDECRIPTION],[TaxClassID],[StockOrService],[TaxClassID1],[TaxClassID2],[TaxClassID3],[TaxClassID4],[ValueStartSB1]
                        ,[ValueStartSB2],[ValueStartSB3],[ValueStartSB4],[ValueEndSB1],[ValueEndSB2],[ValueEndSB3],[ValueEndSB4],[blnSlabSystem],[slabtaxString]
                        ,[HSNCODEUnique],[NoofTaxClass],[SystemName],[UserID],[LastUpdateDate],[LastUpdateTime],[HSNType],[CGSTTaxPer],[SGSTTaxPer],[IGSTTaxPer]
                        ,[CessPer],[CompCessQty],[CGSTTaxPer1],[SGSTTaxPer1],[IGSTTaxPer1],[CGSTTaxPer2],[SGSTTaxPer2],[IGSTTaxPer2],[CGSTTaxPer3],[SGSTTaxPer3]
                        ,[IGSTTaxPer3],[CGSTTaxPer4],[SGSTTaxPer4],[IGSTTaxPer4],[TenantID])
                        Select  distinct  [HSNID],[HSNID] as [HSNCODE],[HSNID] as [HSNDECRIPTION],0 as [TaxClassID],'GOODS' AS [StockOrService],0 as [TaxClassID1]
                        ,0 as [TaxClassID2],0 as [TaxClassID3],0 as [TaxClassID4],0 as [ValueStartSB1],0 as [ValueStartSB2],0 as [ValueStartSB3],0 as [ValueStartSB4]
                        ,0 as [ValueEndSB1],0 as [ValueEndSB2],0 as [ValueEndSB3],0 as [ValueEndSB4],0 as [blnSlabSystem],'' as [slabtaxString],[HSNID] as [HSNCODEUnique]
                        ,1 as [NoofTaxClass],'' as [SystemName],1 as [UserID],'' as [LastUpdateDate],'' as [LastUpdateTime],'GOODS' AS [HSNType],[CGSTTaxPer]
                        ,[SGSTTaxPer],[IGSTTaxPer],[CessPer],[CompCessQty],0 as [CGSTTaxPer1],0 as [SGSTTaxPer1],0 as [IGSTTaxPer1],0 as [CGSTTaxPer2],0 as [SGSTTaxPer2]
                        ,0 as [IGSTTaxPer2],0 as [CGSTTaxPer3],0 as [SGSTTaxPer3],0 as [IGSTTaxPer3],0 as [CGSTTaxPer4],0 as [SGSTTaxPer4],0 as [IGSTTaxPer4], [TenantID] 
                        From tblItemmaster";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblsales add coolie decimal";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblsales set coolie = 0 where coolie is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblsales add DelNoteNo varchar(200)";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblsales add DelNoteDate DateTime";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblsales add DelNoteRefNo varchar(200)";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblsales add DelNoteRefDate DateTime";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblsales add OtherRef varchar(200)";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblsales add BuyerOrderNo varchar(200)";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblsales add BuyerOrderDate DateTime";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblsales add DispatchDocNo varchar(200)";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblsales add LRRRNo varchar(200)";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblsales add MotorVehicleNo varchar(200)";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }


            try
            {
                sQuery = @"update tblsales set DelNoteNo = '' where DelNoteNo is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblsales set DelNoteDate = '01-Jan-1901' where DelNoteDate is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblsales set DelNoteRefNo = '' where DelNoteRefNo is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblsales set DelNoteRefDate = '01-Jan-1901' where DelNoteRefDate is null";
                fnExecuteNonQuery(sQuery, false);

            }
            catch
            { }

            try
            {
                sQuery = @"update tblsales set OtherRef = '' where OtherRef is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblsales set BuyerOrderNo = '' where BuyerOrderNo is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblsales set BuyerOrderDate = '01-Jan-1901' where BuyerOrderDate is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblsales set DispatchDocNo = '' where DispatchDocNo is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblsales set LRRRNo = '' where LRRRNo is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblsales set MotorVehicleNo = '' where MotorVehicleNo is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"ALTER TABLE TBLLEDGER ADD FAX VARCHAR(50)";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"INSERT INTO [dbo].[tblLedger] ([LID],[LName],[LAliasName],[GroupName],[Type],[OpBalance],[AppearIn],[Address],[CreditDays],[Phone],[TaxNo],[AccountGroupID]
                        ,[RouteID],[Area],[Notes],[TargetAmt],[SMSSchID],[Email],[MobileNo],[DiscPer],[InterestPer],[DummyLName],[BlnBank],[CurrencyID]
                        ,[AreaID],[PLID],[ActiveStatus],[EmailAddress],[EntryDate],[blnBillWise],[CustomerCardID],[TDSPer],[DOB],[StateID],[CCIDS]
                        ,[CurrentBalance],[LedgerName],[LedgerCode],[BlnWallet],[blnCoupon],[TransComn],[BlnSmsWelcome],[DLNO],[TDS],[LedgerNameUnicode]
                        ,[LedgerAliasNameUnicode],[ContactPerson],[TaxParameter],[TaxParameterType],[HSNCODE],[CGSTTaxPer],[SGSTTaxPer],[IGSTTaxPer]
                        ,[HSNID],[BankAccountNo],[BankIFSCCode],[BankNote],[WhatsAppNo],[SystemName],[UserID],[LastUpdateDate],[LastUpdateTime],[TenantID]
                        ,[GSTType],[AgentID],[FAX])
                        VALUES (58,'COOLIE','COOLIE','InDirect Expense','Cr',0,NULL,'',0,'','',9,NULL,'NONE', '', 0,NULL,NULL,'',NULL ,NULL, '', NULL ,0,1,0,1,'',NULL ,NULL ,0,NULL ,NULL ,NULL, '' ,NULL ,'COOLIE','COOLIE',NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,'COOLIE','COOLIE',NULL ,'DEFAULT',NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,1,NULL ,NULL,NULL)";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"INSERT INTO [dbo].[tblLedger] ([LID],[LName],[LAliasName],[GroupName],[Type],[OpBalance],[AppearIn],[Address],[CreditDays],[Phone],[TaxNo],[AccountGroupID]
                        ,[RouteID],[Area],[Notes],[TargetAmt],[SMSSchID],[Email],[MobileNo],[DiscPer],[InterestPer],[DummyLName],[BlnBank],[CurrencyID]
                        ,[AreaID],[PLID],[ActiveStatus],[EmailAddress],[EntryDate],[blnBillWise],[CustomerCardID],[TDSPer],[DOB],[StateID],[CCIDS]
                        ,[CurrentBalance],[LedgerName],[LedgerCode],[BlnWallet],[blnCoupon],[TransComn],[BlnSmsWelcome],[DLNO],[TDS],[LedgerNameUnicode]
                        ,[LedgerAliasNameUnicode],[ContactPerson],[TaxParameter],[TaxParameterType],[HSNCODE],[CGSTTaxPer],[SGSTTaxPer],[IGSTTaxPer]
                        ,[HSNID],[BankAccountNo],[BankIFSCCode],[BankNote],[WhatsAppNo],[SystemName],[UserID],[LastUpdateDate],[LastUpdateTime],[TenantID]
                        ,[GSTType],[AgentID],[FAX])
                        VALUES (59,'AGENT COMMISSION','AGENT COMMISSION','Direct Expense','Cr',0,NULL,'',0,'','',6,NULL,'NONE', '', 0,NULL,NULL,'',NULL ,NULL, '', NULL ,0,1,0,1,'',NULL ,NULL ,0,NULL ,NULL ,NULL, '' ,NULL ,'AGENT COMMISSION','AGENT COMMISSION',NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,'AGENT COMMISSION','AGENT COMMISSION',NULL ,'DEFAULT',NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,NULL ,1,NULL ,NULL,NULL)";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"INSERT INTO [dbo].[tblVchType]
                            ([VchTypeID],[VchType],[ShortKey],[EasyKey],[SortOrder],[ActiveStatus],[ParentID],[Description],[numberingCode],[Prefix],[Sufix],[ItemClassIDS]
                            ,[CreditGroupIDs],[DebitGroupIDs],[ProductTypeIDs],[GeneralSettings],[NegativeBalance],[RoundOffBlock],[RoundOffMode],[ItemClassIDS2],[SecondaryCCIDS]
                            ,[PrimaryCCIDS],[OrderVchTypeIDS],[NoteVchTypeIDS],[QuotationVchTypeIDS],[DEFMOPID],[BLNLOCKMOP],[DEFTAXMODEID],[BLNLOCKTAXMODE],[DEFAGENTID]
                            ,[BLNLOCKAGENT],[DEFPRICELISTID],[BLNLOCKPRICELIST],[DEFSALESMANID],[BLNLOCKSALESMAN],[DEFPRINTID],[BLNLOCKPRINT],[ColwidthStr],[gridColor],[DefaultGodownID]
                            ,[ActCFasCostLedger],[ActCFasCostLedger4],[gridHeaderColor],[BLNUseForClientSync],[rateInclusiveIndex],[BlnBillWiseDisc],[BlnItemWisePerDisc],[BlnItemWiseAmtDisc]
                            ,[gridselectedRow],[GridHeaderFont],[GridBackColor],[GridAlternatCellColor],[GridCellColor],[GridFontColor],[Metatag],[DefaultCriteria],[SearchSql]
                            ,[SmartSearchBehavourMode],[criteriaconfig],[intEnterKeyBehavourMode],[BlnBillDiscAmtEntry],[blnRateDiscount],[IntdefaultFocusColumnID],[BlnTouchScreen]
                            ,[StrTouchSetting],[StrCalculationFields],[CRateCalMethod],[MMRPSortOrder],[ItemDiscountFrom],[DEFPRINTID2],[BLNLOCKPRINT2],[BillDiscountFrom],[WindowBackColor]
                            ,[ContrastBackColor],[BlnEnableCustomFormColor],[returnVchtypeID],[PrintCopies],[SystemName],[UserID],[LastUpdateDate],[LastUpdateTime],[BlnMobileVoucher],[SearchSQLSettings]
                            ,[AdvancedSearchSQLEnabled],[TenantID],[VchJson],[FeaturesJson],[GridSettingsJson],[DEFTAXINCLUSIVEID],[BLNLOCKTAXINCLUSIVE],[InvScheme1],[InvScheme2]
                            ,[PrintSettings],[BoardRateExportType],[BoardRateFileName],[BoardRateQuery])
                            VALUES
                            (20,'Repacking','','',1020,1,20,'Repacking',2,'','','','','','','',0,1,4,'','',1,'','','',1,0,3,1,1,0,0,0,1,1,0,0,'','',0,0,0,'',0,0,0,0,0,'','','','',
                            '','','','','',0,'',0,0,1,0,0,'','',0,1,0,0,0,0,'','',0,0,0,'',1,'12-Nov-2022','12-Nov-2022',0,'',0,1,
                            '{""TransactionName"":""Repacking"",""ParentID"":20.0,""TransactionNumberingValue"":2.0,""TransactionPrefix"":"""",""ReferenceNumberingValue"":0.0,""ReferencePrefix"":""P"",""TransactinSortOrder"":1020.0,""CursorNavigationOrderList"":"""",""PrimaryCCValue"":1.0,""blnPrimaryLockWithSelection"":0.0,""SecondaryCCValue"":0.0,""blnSecondaryLockWithSelection"":0.0,""DefaultSearchMethodValue"":1.0,""blnUseSpaceforRateSearch"":0.0,""btnShowItmSearchByDefault"":1.0,""blnMovetoNextRowAfterSelection"":1.0,""blnHideNegativeorExpiredItmsfromMRRPSubWindow"":0.0,""MMRPSubWindowsSortModeValue"":1.0,""blnShowSearchWindowByDefault"":1.0,""blnBillWiseDiscPercentage"":0.0,""btnBillWiseDiscAmount"":1.0,""blnBillWiseDiscPercentageandAmt"":0.0,""BillWiseDiscFillXtraDiscFromValue"":1.0,""blnItmWiseDiscPercentage"":1.0,""blnItmWiseDiscAmount"":0.0,""blnItmWiseDiscPercentageandAmt"":0.0,""ItmWiseDiscFillXtraDiscFromValue"":1.0,""RoundOffMode"":4,""RoundOffBlock"":1.0,""blnRateDiscount"":1.0,""DefaultTaxModeValue"":3.0,""blnTaxModeLockWSel"":0.0,""DefaultModeofPaymentValue"":1.0,""blnModeofPaymentLockWSel"":0.0,""DefaultSaleStaffValue"":1.0,""blnSaleStaffLockWSel"":0.0,""DefaultAgentValue"":1.0,""blnAgentLockWSel"":0.0,""DefaultTaxInclusiveValue"":1.0,""DefaultBarcodeMode"":0.0,""blnTaxInclusiveLockWSel"":0.0,""ProductClassList"":"""",""ItemCategoriesList"":"""",""CustomerSupplierAccGroupList"":"""",""DebitAccGroupList"":"""",""CreditAccGroupList"":"""",""ActiveStatus"":1,""PrintSettings"":""""}',
                            '[{""VchTypeID"":20.0,""SettingsName"":""BLNRECALCULATESALESRATESONPERCENTAGE"",""SettingsDescription"":""Recalculate Sales Rates On Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNSHOWRATEFIXER"",""SettingsDescription"":""Enable Rate Fixer"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNCASHDESK"",""SettingsDescription"":""Enable Cash Desk"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNWARNIFSRATELESSTHANPRATE"",""SettingsDescription"":""Warn If Sales Rate Is Less Than Purchase Rate"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNEDITMRPRATE"",""SettingsDescription"":""Allow User To Edit MRP"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNEDITSALERATE"",""SettingsDescription"":""Allow User To Edit Rate"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNSHOWFREEQUANTITY"",""SettingsDescription"":""Allow User To Enter Free Qty"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNEDITTAXPER"",""SettingsDescription"":""Allow User To Edit Tax Percentage"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNPARTYDETAILS"",""SettingsDescription"":""Show Party Details"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNENABLECASHDISCOUNT"",""SettingsDescription"":""Show Cash Discount"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNENABLEEFFECIVEDATE"",""SettingsDescription"":""Show Effective Date"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNSHOWREFERENCENO"",""SettingsDescription"":""Show Reference No"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNSHOWOTHEREXPENSE"",""SettingsDescription"":""Show Other Expenses"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNSHOWBILLNARRATION"",""SettingsDescription"":""Show Bill Narration"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNSHOWITEMCALCGRID"",""SettingsDescription"":""Show Item Calculation Grid"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNSHOWITEMPROFITPER"",""SettingsDescription"":""Show Product Profit Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNSHOWPREVIEW"",""SettingsDescription"":""Show Preview Before Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNPRINTCONFIRMATION"",""SettingsDescription"":""Ask Confirmation On Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNPRINTIMMEDIATELY"",""SettingsDescription"":""Send Bill To Printer On Save"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNSUMMARISEDUPLICATEITEMSINPRINT"",""SettingsDescription"":""Summarise Duplicate Items In Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0},{""VchTypeID"":2.0,""SettingsName"":""BLNSUMMARISEITEMSWHILEENTERING"",""SettingsDescription"":""Summarise Duplicate Items While Entering"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-02T00:00:00+05:30"",""LastUpdateTime"":""2022-11-02T16:15:48.2116845+05:30"",""TenantID"":1.0}]',
                            '[{""blnVisible"":true,""sName"":""Sl.No"",""iWidth"":50,""sColName"":""cSlNo""},{""blnVisible"":true,""sName"":""Item Code"",""iWidth"":211,""sColName"":""CItemCode""},{""blnVisible"":true,""sName"":""Item Name"",""iWidth"":151,""sColName"":""CItemName""},{""blnVisible"":false,""sName"":""Unit"",""iWidth"":50,""sColName"":""CUnit""},{""blnVisible"":true,""sName"":""Batch Code"",""iWidth"":217,""sColName"":""cBarCode""},{""blnVisible"":false,""sName"":""Expiry Date"",""iWidth"":50,""sColName"":""CExpiry""},{""blnVisible"":true,""sName"":""MRP"",""iWidth"":88,""sColName"":""cMRP""},{""blnVisible"":true,""sName"":""PRate"",""iWidth"":110,""sColName"":""cPrate""},{""blnVisible"":false,""sName"":""Rate Inc."",""iWidth"":80,""sColName"":""cRateinclusive""},{""blnVisible"":true,""sName"":""Qty"",""iWidth"":50,""sColName"":""cQty""},{""blnVisible"":false,""sName"":""Free"",""iWidth"":50,""sColName"":""cFree""},{""blnVisible"":true,""sName"":""SRATE1 %"",""iWidth"":80,""sColName"":""cSRate1Per""},{""blnVisible"":true,""sName"":""SRATE1"",""iWidth"":80,""sColName"":""cSRate1""},{""blnVisible"":false,""sName"":""Disc Rs %"",""iWidth"":80,""sColName"":""cSRate2Per""},{""blnVisible"":false,""sName"":""Disc Rs"",""iWidth"":80,""sColName"":""cSRate2""},{""blnVisible"":false,""sName"":""SRate 3 %"",""iWidth"":80,""sColName"":""cSRate3Per""},{""blnVisible"":false,""sName"":""SRate 3"",""iWidth"":80,""sColName"":""cSRate3""},{""blnVisible"":false,""sName"":""WS %"",""iWidth"":80,""sColName"":""cSRate4Per""},{""blnVisible"":false,""sName"":""WS"",""iWidth"":80,""sColName"":""cSRate4""},{""blnVisible"":false,""sName"":""Retail Rs %"",""iWidth"":80,""sColName"":""cSRate5Per""},{""blnVisible"":false,""sName"":""Retail Rs"",""iWidth"":80,""sColName"":""cSRate5""},{""blnVisible"":true,""sName"":""Gross Amt"",""iWidth"":80,""sColName"":""cGrossAmt""},{""blnVisible"":true,""sName"":""Discount %"",""iWidth"":80,""sColName"":""cDiscPer""},{""blnVisible"":true,""sName"":""Discount Amt"",""iWidth"":80,""sColName"":""cDiscAmount""},{""blnVisible"":true,""sName"":""Bill Discount"",""iWidth"":80,""sColName"":""cBillDisc""},{""blnVisible"":true,""sName"":""CRate"",""iWidth"":70,""sColName"":""cCrate""},{""blnVisible"":false,""sName"":""CRate With Tax"",""iWidth"":50,""sColName"":""cCRateWithTax""},{""blnVisible"":true,""sName"":""Taxable"",""iWidth"":80,""sColName"":""ctaxable""},{""blnVisible"":true,""sName"":""Tax %"",""iWidth"":80,""sColName"":""ctaxPer""},{""blnVisible"":true,""sName"":""Tax"",""iWidth"":80,""sColName"":""ctax""},{""blnVisible"":true,""sName"":""IGST"",""iWidth"":80,""sColName"":""cIGST""},{""blnVisible"":true,""sName"":""SGST"",""iWidth"":80,""sColName"":""cSGST""},{""blnVisible"":true,""sName"":""CGST"",""iWidth"":80,""sColName"":""cCGST""},{""blnVisible"":true,""sName"":""Net Amt"",""iWidth"":181,""sColName"":""cNetAmount""},{""blnVisible"":false,""sName"":""ItemID"",""iWidth"":80,""sColName"":""cItemID""},{""blnVisible"":false,""sName"":""Gross Val"",""iWidth"":100,""sColName"":""cGrossValueAfterRateDiscount""},{""blnVisible"":false,""sName"":""Non Taxable"",""iWidth"":100,""sColName"":""cNonTaxable""},{""blnVisible"":false,""sName"":""Cess %"",""iWidth"":100,""sColName"":""cCCessPer""},{""blnVisible"":false,""sName"":""Comp Cess Qty"",""iWidth"":100,""sColName"":""cCCompCessQty""},{""blnVisible"":false,""sName"":""Flood Cess %"",""iWidth"":100,""sColName"":""cFloodCessPer""},{""blnVisible"":false,""sName"":""Flood Cess Amt"",""iWidth"":100,""sColName"":""cFloodCessAmt""},{""blnVisible"":false,""sName"":""Stock MRP"",""iWidth"":100,""sColName"":""cStockMRP""},{""blnVisible"":false,""sName"":""Agent Comm. %"",""iWidth"":100,""sColName"":""cAgentCommPer""},{""blnVisible"":false,""sName"":""Coolie"",""iWidth"":100,""sColName"":""cCoolie""},{""blnVisible"":false,""sName"":""Offer Item"",""iWidth"":100,""sColName"":""cBlnOfferItem""},{""blnVisible"":false,""sName"":""Offer Det."",""iWidth"":100,""sColName"":""cStrOfferDetails""},{""blnVisible"":false,""sName"":""Batch Mode"",""iWidth"":100,""sColName"":""cBatchMode""},{""blnVisible"":false,""sName"":""ID"",""iWidth"":100,""sColName"":""cID""},{""blnVisible"":false,""sName"":"""",""iWidth"":40,""sColName"":""cImgDel""}]',
                            1,0,NULL,NULL,'','','','')";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"ALTER TABLE tblRepackingItem ADD SRATE1 DECIMAL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"ALTER TABLE tblRepackingItem ADD SRATE2 DECIMAL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"ALTER TABLE tblRepackingItem ADD SRATE3 DECIMAL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"ALTER TABLE tblRepackingItem ADD SRATE4 DECIMAL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"ALTER TABLE tblRepackingItem ADD SRATE5 DECIMAL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"ALTER TABLE tblRepackingItem ADD Srate1Per DECIMAL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"ALTER TABLE tblRepackingItem ADD SRATE2PER DECIMAL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"ALTER TABLE tblRepackingItem ADD SRATE3PER DECIMAL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"ALTER TABLE tblRepackingItem ADD SRATE4PER DECIMAL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"ALTER TABLE tblRepackingItem ADD SRATE5PER DECIMAL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblRepackingItem SET SRATE1 = 0 WHERE SRATE1 IS NULL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblRepackingItem SET SRATE2 = 0 WHERE SRATE2 IS NULL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblRepackingItem SET SRATE3 = 0 WHERE SRATE3 IS NULL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblRepackingItem SET SRATE4 = 0 WHERE SRATE4 IS NULL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblRepackingItem SET SRATE5 = 0 WHERE SRATE5 IS NULL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblRepackingItem SET Srate1Per = 0 WHERE Srate1Per IS NULL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblRepackingItem SET SRATE2PER = 0 WHERE Srate2Per IS NULL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblRepackingItem SET SRATE3PER = 0 WHERE Srate3Per IS NULL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblRepackingItem SET SRATE4PER = 0 WHERE Srate4Per IS NULL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblRepackingItem SET SRATE5PER = 0 WHERE Srate5Per IS NULL";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblrepacking add JSONData Varchar(Max)";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepacking set JSONData = '' where JSONData is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepacking add PartyGSTIN Varchar(100)";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepacking set PartyGSTIN = '' where PartyGSTIN is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add Discount decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set Discount = 0 where Discount is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add Free decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set Free = 0 where Free is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add ItemDiscount decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set ItemDiscount = 0 where ItemDiscount is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add ItemDiscountPer decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set ItemDiscountPer = 0 where ItemDiscountPer is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add CGSTTaxPer decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set CGSTTaxPer = 0 where CGSTTaxPer is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add CGSTTaxAmt decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set CGSTTaxAmt = 0 where CGSTTaxAmt is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add SGSTTaxPer decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set SGSTTaxPer = 0 where SGSTTaxPer is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add SGSTTaxAmt decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set SGSTTaxAmt = 0 where SGSTTaxAmt is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add IGSTTaxPer decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set IGSTTaxPer = 0 where IGSTTaxPer is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add IGSTTaxAmt decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set IGSTTaxAmt = 0 where IGSTTaxAmt is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add iRateDiscPer decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set iRateDiscPer = 0 where iRateDiscPer is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add iRateDiscount decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set iRateDiscount = 0 where iRateDiscount is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add BalQty decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set BalQty = 0 where BalQty is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add GrossAmount decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set GrossAmount = 0 where GrossAmount is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add iFloodCessPer decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set iFloodCessPer = 0 where iFloodCessPer is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add iFloodCessAmt decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set iFloodCessAmt = 0 where iFloodCessAmt is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add Costrate decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set Costrate = 0 where Costrate is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add CostValue decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set CostValue = 0 where CostValue is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add Profit decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set Profit = 0 where Profit is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add ProfitPer decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set ProfitPer = 0 where ProfitPer is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblrepackingitem add DiscMode decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblrepackingitem set DiscMode = 0 where DiscMode is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add ReferenceAutoNO varchar(50)";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set ReferenceAutoNO = '0' where ReferenceAutoNO is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add party varchar(200)";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set party = '' where party  is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add MobileNo varchar(100)";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set MobileNo = '' where MobileNo is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add GSTType varchar(50)";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set GSTType = '' where GSTType is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add PartyAddress varchar(500)";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set PartyAddress = '' where PartyAddress is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add TenantID numeric";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set TenantID = 1 where TenantID is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add AgentID numeric";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set AgentID = 1 where AgentID is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add StateID numeric";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set StateID = 32 where StateID is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add ItemDiscountTotal float";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set ItemDiscountTotal = 0 where ItemDiscountTotal is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add DiscPer decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set DiscPer = 0 where DiscPer is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add Discount decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set Discount = 0 where Discount is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add OtherExpense decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set OtherExpense = 0 where OtherExpense is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add CashDiscount decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set CashDiscount = 0 where CashDiscount is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add CashDisPer decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set CashDisPer = 0 where CashDisPer is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add CostFactor decimal";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set CostFactor = 0 where CostFactor is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblRepacking add LedgerId numeric";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"update tblRepacking set LedgerId = 100 where LedgerId is null";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }


            try
            {
                sQuery = @"CREATE TABLE [dbo].[tblWMDetails](	[ID] [numeric](18, 0) IDENTITY(1,1) NOT NULL,	[ModelName] [nvarchar](50) NULL,	[SqlQuery] [nvarchar](max) NULL,	[DefaultField] [nvarchar](50) NULL) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter TABLE [dbo].[tblWMDetails] add CSVPath varchar(1000)";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblWMDetails add BlnSkipHeader numeric ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"Alter Table tblWMDetails Add  ExportFileType varchar(50) Null ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"Alter Table tblWMDetails Add  ExportFileFormat varchar(50) Null ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"Alter Table tblBoardRateMaster Add TenantID numeric ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblBoardRateMaster set TenantID = 1 where TenantID is null ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblvchtype set vchjson='{""TransactionName"":""Board Rate Updator"",""ParentID"":40.0,""TransactionNumberingValue"":1.0,""TransactionPrefix"":""PT"",""ReferenceNumberingValue"":1.0,""ReferencePrefix"":""PT"",""TransactinSortOrder"":1.0,""CursorNavigationOrderList"":"""",""PrimaryCCValue"":1.0,""blnPrimaryLockWithSelection"":0.0,""SecondaryCCValue"":1.0,""blnSecondaryLockWithSelection"":0.0,""DefaultSearchMethodValue"":1.0,""blnUseSpaceforRateSearch"":0.0,""btnShowItmSearchByDefault"":1.0,""blnMovetoNextRowAfterSelection"":0.0,""blnHideNegativeorExpiredItmsfromMRRPSubWindow"":0.0,""MMRPSubWindowsSortModeValue"":1.0,""blnShowSearchWindowByDefault"":1.0,""blnBillWiseDiscPercentage"":1.0,""btnBillWiseDiscAmount"":0.0,""blnBillWiseDiscPercentageandAmt"":0.0,""BillWiseDiscFillXtraDiscFromValue"":2.0,""blnItmWiseDiscPercentage"":1.0,""blnItmWiseDiscAmount"":0.0,""blnItmWiseDiscPercentageandAmt"":0.0,""ItmWiseDiscFillXtraDiscFromValue"":1.0,""RoundOffMode"":1,""RoundOffBlock"":1.0,""blnRateDiscount"":0.0,""DefaultTaxModeValue"":3.0,""blnTaxModeLockWSel"":0.0,""DefaultModeofPaymentValue"":1.0,""blnModeofPaymentLockWSel"":0.0,""DefaultSaleStaffValue"":1.0,""blnSaleStaffLockWSel"":0.0,""DefaultAgentValue"":1.0,""blnAgentLockWSel"":0.0,""DefaultTaxInclusiveValue"":1.0,""DefaultBarcodeMode"":0.0,""blnTaxInclusiveLockWSel"":0.0,""ProductClassList"":"""",""ItemCategoriesList"":"""",""CustomerSupplierAccGroupList"":""10"",""DebitAccGroupList"":"""",""CreditAccGroupList"":"""",""ActiveStatus"":1,""PrintSettings"":""""}' where vchtypeid=40 and vchjson is null";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set featuresjson='[{""VchTypeID"":41.0,""SettingsName"":""BLNRECALCULATESALESRATESONPERCENTAGE"",""SettingsDescription"":""Recalculate Sales Rates On Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWRATEFIXER"",""SettingsDescription"":""Enable Rate Fixer"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNCASHDESK"",""SettingsDescription"":""Enable Cash Desk"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNWARNIFSRATELESSTHANPRATE"",""SettingsDescription"":""Warn If Sales Rate Is Less Than Purchase Rate"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNEDITMRPRATE"",""SettingsDescription"":""Allow User To Edit MRP"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNEDITSALERATE"",""SettingsDescription"":""Allow User To Edit Rate"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWFREEQUANTITY"",""SettingsDescription"":""Allow User To Enter Free Qty"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNEDITTAXPER"",""SettingsDescription"":""Allow User To Edit Tax Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNPARTYDETAILS"",""SettingsDescription"":""Show Party Details"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNENABLECASHDISCOUNT"",""SettingsDescription"":""Show Cash Discount"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNENABLEEFFECIVEDATE"",""SettingsDescription"":""Show Effective Date"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWREFERENCENO"",""SettingsDescription"":""Show Reference No"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWOTHEREXPENSE"",""SettingsDescription"":""Show Other Expenses"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWBILLNARRATION"",""SettingsDescription"":""Show Bill Narration"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWITEMCALCGRID"",""SettingsDescription"":""Show Item Calculation Grid"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWITEMPROFITPER"",""SettingsDescription"":""Show Product Profit Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWPREVIEW"",""SettingsDescription"":""Show Preview Before Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNPRINTCONFIRMATION"",""SettingsDescription"":""Ask Confirmation On Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNPRINTIMMEDIATELY"",""SettingsDescription"":""Send Bill To Printer On Save"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSUMMARISEDUPLICATEITEMSINPRINT"",""SettingsDescription"":""Summarise Duplicate Items In Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSUMMARISEITEMSWHILEENTERING"",""SettingsDescription"":""Summarise Duplicate Items While Entering"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0}]' where vchtypeid=40 and featuresjson is null";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set featuresjson='[{""VchTypeID"":41.0,""SettingsName"":""BLNRECALCULATESALESRATESONPERCENTAGE"",""SettingsDescription"":""Recalculate Sales Rates On Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWRATEFIXER"",""SettingsDescription"":""Enable Rate Fixer"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNCASHDESK"",""SettingsDescription"":""Enable Cash Desk"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNWARNIFSRATELESSTHANPRATE"",""SettingsDescription"":""Warn If Sales Rate Is Less Than Purchase Rate"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNEDITMRPRATE"",""SettingsDescription"":""Allow User To Edit MRP"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNEDITSALERATE"",""SettingsDescription"":""Allow User To Edit Rate"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWFREEQUANTITY"",""SettingsDescription"":""Allow User To Enter Free Qty"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNEDITTAXPER"",""SettingsDescription"":""Allow User To Edit Tax Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNPARTYDETAILS"",""SettingsDescription"":""Show Party Details"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNENABLECASHDISCOUNT"",""SettingsDescription"":""Show Cash Discount"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNENABLEEFFECIVEDATE"",""SettingsDescription"":""Show Effective Date"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWREFERENCENO"",""SettingsDescription"":""Show Reference No"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWOTHEREXPENSE"",""SettingsDescription"":""Show Other Expenses"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWBILLNARRATION"",""SettingsDescription"":""Show Bill Narration"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWITEMCALCGRID"",""SettingsDescription"":""Show Item Calculation Grid"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWITEMPROFITPER"",""SettingsDescription"":""Show Product Profit Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWPREVIEW"",""SettingsDescription"":""Show Preview Before Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNPRINTCONFIRMATION"",""SettingsDescription"":""Ask Confirmation On Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNPRINTIMMEDIATELY"",""SettingsDescription"":""Send Bill To Printer On Save"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSUMMARISEDUPLICATEITEMSINPRINT"",""SettingsDescription"":""Summarise Duplicate Items In Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSUMMARISEITEMSWHILEENTERING"",""SettingsDescription"":""Summarise Duplicate Items While Entering"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0}]' where vchtypeid=40 and featuresjson = '' ";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set featuresjson='[{""VchTypeID"":41.0,""SettingsName"":""BLNRECALCULATESALESRATESONPERCENTAGE"",""SettingsDescription"":""Recalculate Sales Rates On Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWRATEFIXER"",""SettingsDescription"":""Enable Rate Fixer"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNCASHDESK"",""SettingsDescription"":""Enable Cash Desk"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNWARNIFSRATELESSTHANPRATE"",""SettingsDescription"":""Warn If Sales Rate Is Less Than Purchase Rate"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNEDITMRPRATE"",""SettingsDescription"":""Allow User To Edit MRP"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNEDITSALERATE"",""SettingsDescription"":""Allow User To Edit Rate"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWFREEQUANTITY"",""SettingsDescription"":""Allow User To Enter Free Qty"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNEDITTAXPER"",""SettingsDescription"":""Allow User To Edit Tax Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNPARTYDETAILS"",""SettingsDescription"":""Show Party Details"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNENABLECASHDISCOUNT"",""SettingsDescription"":""Show Cash Discount"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNENABLEEFFECIVEDATE"",""SettingsDescription"":""Show Effective Date"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWREFERENCENO"",""SettingsDescription"":""Show Reference No"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWOTHEREXPENSE"",""SettingsDescription"":""Show Other Expenses"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWBILLNARRATION"",""SettingsDescription"":""Show Bill Narration"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWITEMCALCGRID"",""SettingsDescription"":""Show Item Calculation Grid"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWITEMPROFITPER"",""SettingsDescription"":""Show Product Profit Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSHOWPREVIEW"",""SettingsDescription"":""Show Preview Before Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNPRINTCONFIRMATION"",""SettingsDescription"":""Ask Confirmation On Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNPRINTIMMEDIATELY"",""SettingsDescription"":""Send Bill To Printer On Save"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSUMMARISEDUPLICATEITEMSINPRINT"",""SettingsDescription"":""Summarise Duplicate Items In Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0},{""VchTypeID"":41.0,""SettingsName"":""BLNSUMMARISEITEMSWHILEENTERING"",""SettingsDescription"":""Summarise Duplicate Items While Entering"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-10-27T00:00:00+05:30"",""LastUpdateTime"":""2022-10-27T19:28:38.6112165+05:30"",""TenantID"":1.0}]' where vchtypeid=40 and featuresjson = '[]' ";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set gridsettingsjson='[{""blnVisible"":true,""sName"":""Sl.No"",""iWidth"":50,""sColName"":""cSlNo""},{""blnVisible"":true,""sName"":""Item Code"",""iWidth"":130,""sColName"":""CItemCode""},{""blnVisible"":true,""sName"":""Item Name"",""iWidth"":200,""sColName"":""CItemName""},{""blnVisible"":true,""sName"":""Unit"",""iWidth"":50,""sColName"":""CUnit""},{""blnVisible"":true,""sName"":""Batch Code"",""iWidth"":200,""sColName"":""cBarCode""},{""blnVisible"":true,""sName"":""Expiry Date"",""iWidth"":120,""sColName"":""CExpiry""},{""blnVisible"":true,""sName"":""MRP"",""iWidth"":80,""sColName"":""cMRP""},{""blnVisible"":true,""sName"":""SRate"",""iWidth"":80,""sColName"":""cSrate""},{""blnVisible"":false,""sName"":""Rate Inc."",""iWidth"":80,""sColName"":""cRateinclusive""},{""blnVisible"":true,""sName"":""Qty"",""iWidth"":80,""sColName"":""cQty""},{""blnVisible"":true,""sName"":""QOH"",""iWidth"":80,""sColName"":""cQOH""},{""blnVisible"":false,""sName"":""SRATE1 % "",""iWidth"":80,""sColName"":""cSRate1Per""},{""blnVisible"":false,""sName"":""SRATE1"",""iWidth"":80,""sColName"":""cSRate1""},{""blnVisible"":false,""sName"":""Disc Rs % "",""iWidth"":80,""sColName"":""cSRate2Per""},{""blnVisible"":false,""sName"":""Disc Rs"",""iWidth"":80,""sColName"":""cSRate2""},{""blnVisible"":false,""sName"":""SRate 3 % "",""iWidth"":80,""sColName"":""cSRate3Per""},{""blnVisible"":false,""sName"":""SRate 3"",""iWidth"":80,""sColName"":""cSRate3""},{""blnVisible"":false,""sName"":""WS % "",""iWidth"":80,""sColName"":""cSRate4Per""},{""blnVisible"":false,""sName"":""WS"",""iWidth"":80,""sColName"":""cSRate4""},{""blnVisible"":false,""sName"":""Retail Rs % "",""iWidth"":80,""sColName"":""cSRate5Per""},{""blnVisible"":false,""sName"":""Retail Rs"",""iWidth"":80,""sColName"":""cSRate5""},{""blnVisible"":true,""sName"":""Gross Amt"",""iWidth"":105,""sColName"":""cGrossAmt""},{""blnVisible"":false,""sName"":""Discount % "",""iWidth"":50,""sColName"":""cDiscPer""},{""blnVisible"":true,""sName"":""Discount Amt"",""iWidth"":119,""sColName"":""cDiscAmount""},{""blnVisible"":false,""sName"":""Bill Discount"",""iWidth"":50,""sColName"":""cBillDisc""},{""blnVisible"":true,""sName"":""CRate"",""iWidth"":80,""sColName"":""cCrate""},{""blnVisible"":false,""sName"":""CRate With Tax"",""iWidth"":50,""sColName"":""cCRateWithTax""},{""blnVisible"":false,""sName"":""Taxable"",""iWidth"":80,""sColName"":""ctaxable""},{""blnVisible"":false,""sName"":""Tax % "",""iWidth"":50,""sColName"":""ctaxPer""},{""blnVisible"":false,""sName"":""Tax"",""iWidth"":50,""sColName"":""ctax""},{""blnVisible"":false,""sName"":""IGST"",""iWidth"":50,""sColName"":""cIGST""},{""blnVisible"":false,""sName"":""SGST"",""iWidth"":50,""sColName"":""cSGST""},{""blnVisible"":false,""sName"":""CGST"",""iWidth"":50,""sColName"":""cCGST""},{""blnVisible"":true,""sName"":""Net Amt"",""iWidth"":100,""sColName"":""cNetAmount""},{""blnVisible"":false,""sName"":""ItemID"",""iWidth"":80,""sColName"":""cItemID""},{""blnVisible"":false,""sName"":""Gross Val"",""iWidth"":100,""sColName"":""cGrossValueAfterRateDiscount""},{""blnVisible"":false,""sName"":""Non Taxable"",""iWidth"":100,""sColName"":""cNonTaxable""},{""blnVisible"":false,""sName"":""Cess % "",""iWidth"":100,""sColName"":""cCCessPer""},{""blnVisible"":false,""sName"":""Comp Cess Qty"",""iWidth"":100,""sColName"":""cCCompCessQty""},{""blnVisible"":false,""sName"":""Flood Cess % "",""iWidth"":100,""sColName"":""cFloodCessPer""},{""blnVisible"":false,""sName"":""Flood Cess Amt"",""iWidth"":100,""sColName"":""cFloodCessAmt""},{""blnVisible"":false,""sName"":""Stock MRP"",""iWidth"":100,""sColName"":""cStockMRP""},{""blnVisible"":false,""sName"":""Agent Comm. % "",""iWidth"":100,""sColName"":""cAgentCommPer""},{""blnVisible"":false,""sName"":""Coolie"",""iWidth"":100,""sColName"":""cCoolie""},{""blnVisible"":false,""sName"":""Offer Item"",""iWidth"":100,""sColName"":""cBlnOfferItem""},{""blnVisible"":false,""sName"":""Offer Det."",""iWidth"":100,""sColName"":""cStrOfferDetails""},{""blnVisible"":false,""sName"":""Batch Mode"",""iWidth"":100,""sColName"":""cBatchMode""},{""blnVisible"":false,""sName"":""ID"",""iWidth"":100,""sColName"":""cID""}]' where vchtypeid=40 and gridsettingsjson is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"update tblvchtype set vchjson='{""TransactionName"":""Opening"",""ParentID"":1005.0,""TransactionNumberingValue"":1.0,""TransactionPrefix"":""PT"",""ReferenceNumberingValue"":1.0,""ReferencePrefix"":""PT"",""TransactinSortOrder"":1.0,""CursorNavigationOrderList"":"""",""PrimaryCCValue"":1.0,""blnPrimaryLockWithSelection"":0.0,""SecondaryCCValue"":1.0,""blnSecondaryLockWithSelection"":0.0,""DefaultSearchMethodValue"":1.0,""blnUseSpaceforRateSearch"":0.0,""btnShowItmSearchByDefault"":1.0,""blnMovetoNextRowAfterSelection"":0.0,""blnHideNegativeorExpiredItmsfromMRRPSubWindow"":0.0,""MMRPSubWindowsSortModeValue"":1.0,""blnShowSearchWindowByDefault"":1.0,""blnBillWiseDiscPercentage"":1.0,""btnBillWiseDiscAmount"":0.0,""blnBillWiseDiscPercentageandAmt"":0.0,""BillWiseDiscFillXtraDiscFromValue"":2.0,""blnItmWiseDiscPercentage"":1.0,""blnItmWiseDiscAmount"":0.0,""blnItmWiseDiscPercentageandAmt"":0.0,""ItmWiseDiscFillXtraDiscFromValue"":1.0,""RoundOffMode"":1,""RoundOffBlock"":1.0,""blnRateDiscount"":0.0,""DefaultTaxModeValue"":3.0,""blnTaxModeLockWSel"":0.0,""DefaultModeofPaymentValue"":1.0,""blnModeofPaymentLockWSel"":0.0,""DefaultSaleStaffValue"":1.0,""blnSaleStaffLockWSel"":0.0,""DefaultAgentValue"":1.0,""blnAgentLockWSel"":0.0,""DefaultTaxInclusiveValue"":1.0,""DefaultBarcodeMode"":0.0,""blnTaxInclusiveLockWSel"":0.0,""ProductClassList"":"""",""ItemCategoriesList"":"""",""CustomerSupplierAccGroupList"":""10"",""DebitAccGroupList"":"""",""CreditAccGroupList"":"""",""ActiveStatus"":1,""PrintSettings"":""""}' where vchtypeid=1005 and vchjson is null";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set vchjson='{""TransactionName"":""Opening"",""ParentID"":1005.0,""TransactionNumberingValue"":1.0,""TransactionPrefix"":""PT"",""ReferenceNumberingValue"":1.0,""ReferencePrefix"":""PT"",""TransactinSortOrder"":1.0,""CursorNavigationOrderList"":"""",""PrimaryCCValue"":1.0,""blnPrimaryLockWithSelection"":0.0,""SecondaryCCValue"":1.0,""blnSecondaryLockWithSelection"":0.0,""DefaultSearchMethodValue"":1.0,""blnUseSpaceforRateSearch"":0.0,""btnShowItmSearchByDefault"":1.0,""blnMovetoNextRowAfterSelection"":0.0,""blnHideNegativeorExpiredItmsfromMRRPSubWindow"":0.0,""MMRPSubWindowsSortModeValue"":1.0,""blnShowSearchWindowByDefault"":1.0,""blnBillWiseDiscPercentage"":1.0,""btnBillWiseDiscAmount"":0.0,""blnBillWiseDiscPercentageandAmt"":0.0,""BillWiseDiscFillXtraDiscFromValue"":2.0,""blnItmWiseDiscPercentage"":1.0,""blnItmWiseDiscAmount"":0.0,""blnItmWiseDiscPercentageandAmt"":0.0,""ItmWiseDiscFillXtraDiscFromValue"":1.0,""RoundOffMode"":1,""RoundOffBlock"":1.0,""blnRateDiscount"":0.0,""DefaultTaxModeValue"":3.0,""blnTaxModeLockWSel"":0.0,""DefaultModeofPaymentValue"":1.0,""blnModeofPaymentLockWSel"":0.0,""DefaultSaleStaffValue"":1.0,""blnSaleStaffLockWSel"":0.0,""DefaultAgentValue"":1.0,""blnAgentLockWSel"":0.0,""DefaultTaxInclusiveValue"":1.0,""DefaultBarcodeMode"":0.0,""blnTaxInclusiveLockWSel"":0.0,""ProductClassList"":"""",""ItemCategoriesList"":"""",""CustomerSupplierAccGroupList"":""10"",""DebitAccGroupList"":"""",""CreditAccGroupList"":"""",""ActiveStatus"":1,""PrintSettings"":""""}' where vchtypeid=1005 and vchjson = '[]'";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set vchjson='{""TransactionName"":""Opening"",""ParentID"":1005.0,""TransactionNumberingValue"":1.0,""TransactionPrefix"":""PT"",""ReferenceNumberingValue"":1.0,""ReferencePrefix"":""PT"",""TransactinSortOrder"":1.0,""CursorNavigationOrderList"":"""",""PrimaryCCValue"":1.0,""blnPrimaryLockWithSelection"":0.0,""SecondaryCCValue"":1.0,""blnSecondaryLockWithSelection"":0.0,""DefaultSearchMethodValue"":1.0,""blnUseSpaceforRateSearch"":0.0,""btnShowItmSearchByDefault"":1.0,""blnMovetoNextRowAfterSelection"":0.0,""blnHideNegativeorExpiredItmsfromMRRPSubWindow"":0.0,""MMRPSubWindowsSortModeValue"":1.0,""blnShowSearchWindowByDefault"":1.0,""blnBillWiseDiscPercentage"":1.0,""btnBillWiseDiscAmount"":0.0,""blnBillWiseDiscPercentageandAmt"":0.0,""BillWiseDiscFillXtraDiscFromValue"":2.0,""blnItmWiseDiscPercentage"":1.0,""blnItmWiseDiscAmount"":0.0,""blnItmWiseDiscPercentageandAmt"":0.0,""ItmWiseDiscFillXtraDiscFromValue"":1.0,""RoundOffMode"":1,""RoundOffBlock"":1.0,""blnRateDiscount"":0.0,""DefaultTaxModeValue"":3.0,""blnTaxModeLockWSel"":0.0,""DefaultModeofPaymentValue"":1.0,""blnModeofPaymentLockWSel"":0.0,""DefaultSaleStaffValue"":1.0,""blnSaleStaffLockWSel"":0.0,""DefaultAgentValue"":1.0,""blnAgentLockWSel"":0.0,""DefaultTaxInclusiveValue"":1.0,""DefaultBarcodeMode"":0.0,""blnTaxInclusiveLockWSel"":0.0,""ProductClassList"":"""",""ItemCategoriesList"":"""",""CustomerSupplierAccGroupList"":""10"",""DebitAccGroupList"":"""",""CreditAccGroupList"":"""",""ActiveStatus"":1,""PrintSettings"":""""}' where vchtypeid=1005 and vchjson = ''";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set featuresjson='[{""VchTypeID"":1005.0,""SettingsName"":""BLNRECALCULATESALESRATESONPERCENTAGE"",""SettingsDescription"":""Recalculate Sales Rates On Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWRATEFIXER"",""SettingsDescription"":""Enable Rate Fixer"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNCASHDESK"",""SettingsDescription"":""Enable Cash Desk"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNWARNIFSRATELESSTHANPRATE"",""SettingsDescription"":""Warn If Sales Rate Is Less Than Purchase Rate"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNEDITMRPRATE"",""SettingsDescription"":""Allow User To Edit MRP"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNEDITSALERATE"",""SettingsDescription"":""Allow User To Edit Rate"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWFREEQUANTITY"",""SettingsDescription"":""Allow User To Enter Free Qty"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNEDITTAXPER"",""SettingsDescription"":""Allow User To Edit Tax Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNPARTYDETAILS"",""SettingsDescription"":""Show Party Details"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNENABLECASHDISCOUNT"",""SettingsDescription"":""Show Cash Discount"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNENABLEEFFECIVEDATE"",""SettingsDescription"":""Show Effective Date"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWREFERENCENO"",""SettingsDescription"":""Show Reference No"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWOTHEREXPENSE"",""SettingsDescription"":""Show Other Expenses"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWBILLNARRATION"",""SettingsDescription"":""Show Bill Narration"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWITEMCALCGRID"",""SettingsDescription"":""Show Item Calculation Grid"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWITEMPROFITPER"",""SettingsDescription"":""Show Product Profit Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWPREVIEW"",""SettingsDescription"":""Show Preview Before Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNPRINTCONFIRMATION"",""SettingsDescription"":""Ask Confirmation On Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNPRINTIMMEDIATELY"",""SettingsDescription"":""Send Bill To Printer On Save"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSUMMARISEDUPLICATEITEMSINPRINT"",""SettingsDescription"":""Summarise Duplicate Items In Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSUMMARISEITEMSWHILEENTERING"",""SettingsDescription"":""Summarise Duplicate Items While Entering"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0}]' where vchtypeid=1005 and featuresjson is null";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set featuresjson='[{""VchTypeID"":1005.0,""SettingsName"":""BLNRECALCULATESALESRATESONPERCENTAGE"",""SettingsDescription"":""Recalculate Sales Rates On Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWRATEFIXER"",""SettingsDescription"":""Enable Rate Fixer"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNCASHDESK"",""SettingsDescription"":""Enable Cash Desk"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNWARNIFSRATELESSTHANPRATE"",""SettingsDescription"":""Warn If Sales Rate Is Less Than Purchase Rate"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNEDITMRPRATE"",""SettingsDescription"":""Allow User To Edit MRP"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNEDITSALERATE"",""SettingsDescription"":""Allow User To Edit Rate"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWFREEQUANTITY"",""SettingsDescription"":""Allow User To Enter Free Qty"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNEDITTAXPER"",""SettingsDescription"":""Allow User To Edit Tax Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNPARTYDETAILS"",""SettingsDescription"":""Show Party Details"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNENABLECASHDISCOUNT"",""SettingsDescription"":""Show Cash Discount"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNENABLEEFFECIVEDATE"",""SettingsDescription"":""Show Effective Date"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWREFERENCENO"",""SettingsDescription"":""Show Reference No"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWOTHEREXPENSE"",""SettingsDescription"":""Show Other Expenses"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWBILLNARRATION"",""SettingsDescription"":""Show Bill Narration"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWITEMCALCGRID"",""SettingsDescription"":""Show Item Calculation Grid"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWITEMPROFITPER"",""SettingsDescription"":""Show Product Profit Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWPREVIEW"",""SettingsDescription"":""Show Preview Before Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNPRINTCONFIRMATION"",""SettingsDescription"":""Ask Confirmation On Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNPRINTIMMEDIATELY"",""SettingsDescription"":""Send Bill To Printer On Save"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSUMMARISEDUPLICATEITEMSINPRINT"",""SettingsDescription"":""Summarise Duplicate Items In Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSUMMARISEITEMSWHILEENTERING"",""SettingsDescription"":""Summarise Duplicate Items While Entering"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0}]' where vchtypeid=1005 and featuresjson = '' ";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set featuresjson='[{""VchTypeID"":1005.0,""SettingsName"":""BLNRECALCULATESALESRATESONPERCENTAGE"",""SettingsDescription"":""Recalculate Sales Rates On Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWRATEFIXER"",""SettingsDescription"":""Enable Rate Fixer"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNCASHDESK"",""SettingsDescription"":""Enable Cash Desk"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNWARNIFSRATELESSTHANPRATE"",""SettingsDescription"":""Warn If Sales Rate Is Less Than Purchase Rate"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNEDITMRPRATE"",""SettingsDescription"":""Allow User To Edit MRP"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNEDITSALERATE"",""SettingsDescription"":""Allow User To Edit Rate"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWFREEQUANTITY"",""SettingsDescription"":""Allow User To Enter Free Qty"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNEDITTAXPER"",""SettingsDescription"":""Allow User To Edit Tax Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNPARTYDETAILS"",""SettingsDescription"":""Show Party Details"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNENABLECASHDISCOUNT"",""SettingsDescription"":""Show Cash Discount"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNENABLEEFFECIVEDATE"",""SettingsDescription"":""Show Effective Date"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWREFERENCENO"",""SettingsDescription"":""Show Reference No"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWOTHEREXPENSE"",""SettingsDescription"":""Show Other Expenses"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWBILLNARRATION"",""SettingsDescription"":""Show Bill Narration"",""BlnEnabled"":1.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWITEMCALCGRID"",""SettingsDescription"":""Show Item Calculation Grid"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWITEMPROFITPER"",""SettingsDescription"":""Show Product Profit Percentage"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSHOWPREVIEW"",""SettingsDescription"":""Show Preview Before Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNPRINTCONFIRMATION"",""SettingsDescription"":""Ask Confirmation On Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNPRINTIMMEDIATELY"",""SettingsDescription"":""Send Bill To Printer On Save"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSUMMARISEDUPLICATEITEMSINPRINT"",""SettingsDescription"":""Summarise Duplicate Items In Print"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0},{""VchTypeID"":1005.0,""SettingsName"":""BLNSUMMARISEITEMSWHILEENTERING"",""SettingsDescription"":""Summarise Duplicate Items While Entering"",""BlnEnabled"":0.0,""UserID"":1.0,""SystemName"":""Standard"",""UserID1"":1.0,""LastUpdateDate"":""2022-11-05T00:00:00+05:30"",""LastUpdateTime"":""2022-11-05T11:21:46.9437823+05:30"",""TenantID"":1.0}]' where vchtypeid=1005 and featuresjson = '[]' ";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set gridsettingsjson='[{""blnVisible"":true,""sName"":""Sl.No"",""iWidth"":50,""sColName"":""cSlNo""},{""blnVisible"":true,""sName"":""Item Code"",""iWidth"":130,""sColName"":""CItemCode""},{""blnVisible"":true,""sName"":""Item Name"",""iWidth"":200,""sColName"":""CItemName""},{""blnVisible"":true,""sName"":""Unit"",""iWidth"":69,""sColName"":""CUnit""},{""blnVisible"":true,""sName"":""Batch Code"",""iWidth"":200,""sColName"":""cBarCode""},{""blnVisible"":true,""sName"":""Expiry Date"",""iWidth"":120,""sColName"":""CExpiry""},{""blnVisible"":true,""sName"":""MRP"",""iWidth"":80,""sColName"":""cMRP""},{""blnVisible"":true,""sName"":""PRate"",""iWidth"":80,""sColName"":""cPrate""},{""blnVisible"":false,""sName"":""Rate Inc."",""iWidth"":80,""sColName"":""cRateinclusive""},{""blnVisible"":true,""sName"":""Qty"",""iWidth"":80,""sColName"":""cQty""},{""blnVisible"":false,""sName"":""Free"",""iWidth"":80,""sColName"":""cFree""},{""blnVisible"":false,""sName"":""SRATE1 %"",""iWidth"":80,""sColName"":""cSRate1Per""},{""blnVisible"":false,""sName"":""SRATE1"",""iWidth"":80,""sColName"":""cSRate1""},{""blnVisible"":false,""sName"":""Disc Rs %"",""iWidth"":80,""sColName"":""cSRate2Per""},{""blnVisible"":false,""sName"":""Disc Rs"",""iWidth"":80,""sColName"":""cSRate2""},{""blnVisible"":false,""sName"":""SRate 3 %"",""iWidth"":80,""sColName"":""cSRate3Per""},{""blnVisible"":false,""sName"":""SRate 3"",""iWidth"":80,""sColName"":""cSRate3""},{""blnVisible"":false,""sName"":""WS %"",""iWidth"":80,""sColName"":""cSRate4Per""},{""blnVisible"":false,""sName"":""WS"",""iWidth"":80,""sColName"":""cSRate4""},{""blnVisible"":false,""sName"":""Retail Rs %"",""iWidth"":80,""sColName"":""cSRate5Per""},{""blnVisible"":false,""sName"":""Retail Rs"",""iWidth"":80,""sColName"":""cSRate5""},{""blnVisible"":true,""sName"":""Gross Amt"",""iWidth"":105,""sColName"":""cGrossAmt""},{""blnVisible"":false,""sName"":""Discount %"",""iWidth"":50,""sColName"":""cDiscPer""},{""blnVisible"":false,""sName"":""Discount Amt"",""iWidth"":119,""sColName"":""cDiscAmount""},{""blnVisible"":false,""sName"":""Bill Discount"",""iWidth"":50,""sColName"":""cBillDisc""},{""blnVisible"":true,""sName"":""CRate"",""iWidth"":80,""sColName"":""cCrate""},{""blnVisible"":false,""sName"":""CRate With Tax"",""iWidth"":50,""sColName"":""cCRateWithTax""},{""blnVisible"":false,""sName"":""Taxable"",""iWidth"":80,""sColName"":""ctaxable""},{""blnVisible"":false,""sName"":""Tax %"",""iWidth"":50,""sColName"":""ctaxPer""},{""blnVisible"":false,""sName"":""Tax"",""iWidth"":50,""sColName"":""ctax""},{""blnVisible"":false,""sName"":""IGST"",""iWidth"":50,""sColName"":""cIGST""},{""blnVisible"":false,""sName"":""SGST"",""iWidth"":50,""sColName"":""cSGST""},{""blnVisible"":false,""sName"":""CGST"",""iWidth"":50,""sColName"":""cCGST""},{""blnVisible"":true,""sName"":""Net Amt"",""iWidth"":100,""sColName"":""cNetAmount""},{""blnVisible"":false,""sName"":""ItemID"",""iWidth"":80,""sColName"":""cItemID""},{""blnVisible"":false,""sName"":""Gross Val"",""iWidth"":100,""sColName"":""cGrossValueAfterRateDiscount""},{""blnVisible"":false,""sName"":""Non Taxable"",""iWidth"":100,""sColName"":""cNonTaxable""},{""blnVisible"":false,""sName"":""Cess %"",""iWidth"":100,""sColName"":""cCCessPer""},{""blnVisible"":false,""sName"":""Comp Cess Qty"",""iWidth"":100,""sColName"":""cCCompCessQty""},{""blnVisible"":false,""sName"":""Flood Cess %"",""iWidth"":100,""sColName"":""cFloodCessPer""},{""blnVisible"":false,""sName"":""Flood Cess Amt"",""iWidth"":100,""sColName"":""cFloodCessAmt""},{""blnVisible"":false,""sName"":""Stock MRP"",""iWidth"":100,""sColName"":""cStockMRP""},{""blnVisible"":false,""sName"":""Agent Comm. %"",""iWidth"":100,""sColName"":""cAgentCommPer""},{""blnVisible"":false,""sName"":""Coolie"",""iWidth"":100,""sColName"":""cCoolie""},{""blnVisible"":false,""sName"":""Offer Item"",""iWidth"":100,""sColName"":""cBlnOfferItem""},{""blnVisible"":false,""sName"":""Offer Det."",""iWidth"":100,""sColName"":""cStrOfferDetails""},{""blnVisible"":false,""sName"":""Batch Mode"",""iWidth"":100,""sColName"":""cBatchMode""},{""blnVisible"":false,""sName"":""ID"",""iWidth"":100,""sColName"":""cID""},{""blnVisible"":false,""sName"":"""",""iWidth"":40,""sColName"":""cImgDel""}]' where vchtypeid=1005 and gridsettingsjson is null";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set gridsettingsjson='[{""blnVisible"":true,""sName"":""Sl.No"",""iWidth"":50,""sColName"":""cSlNo""},{""blnVisible"":true,""sName"":""Item Code"",""iWidth"":130,""sColName"":""CItemCode""},{""blnVisible"":true,""sName"":""Item Name"",""iWidth"":200,""sColName"":""CItemName""},{""blnVisible"":true,""sName"":""Unit"",""iWidth"":69,""sColName"":""CUnit""},{""blnVisible"":true,""sName"":""Batch Code"",""iWidth"":200,""sColName"":""cBarCode""},{""blnVisible"":true,""sName"":""Expiry Date"",""iWidth"":120,""sColName"":""CExpiry""},{""blnVisible"":true,""sName"":""MRP"",""iWidth"":80,""sColName"":""cMRP""},{""blnVisible"":true,""sName"":""PRate"",""iWidth"":80,""sColName"":""cPrate""},{""blnVisible"":false,""sName"":""Rate Inc."",""iWidth"":80,""sColName"":""cRateinclusive""},{""blnVisible"":true,""sName"":""Qty"",""iWidth"":80,""sColName"":""cQty""},{""blnVisible"":false,""sName"":""Free"",""iWidth"":80,""sColName"":""cFree""},{""blnVisible"":false,""sName"":""SRATE1 %"",""iWidth"":80,""sColName"":""cSRate1Per""},{""blnVisible"":false,""sName"":""SRATE1"",""iWidth"":80,""sColName"":""cSRate1""},{""blnVisible"":false,""sName"":""Disc Rs %"",""iWidth"":80,""sColName"":""cSRate2Per""},{""blnVisible"":false,""sName"":""Disc Rs"",""iWidth"":80,""sColName"":""cSRate2""},{""blnVisible"":false,""sName"":""SRate 3 %"",""iWidth"":80,""sColName"":""cSRate3Per""},{""blnVisible"":false,""sName"":""SRate 3"",""iWidth"":80,""sColName"":""cSRate3""},{""blnVisible"":false,""sName"":""WS %"",""iWidth"":80,""sColName"":""cSRate4Per""},{""blnVisible"":false,""sName"":""WS"",""iWidth"":80,""sColName"":""cSRate4""},{""blnVisible"":false,""sName"":""Retail Rs %"",""iWidth"":80,""sColName"":""cSRate5Per""},{""blnVisible"":false,""sName"":""Retail Rs"",""iWidth"":80,""sColName"":""cSRate5""},{""blnVisible"":true,""sName"":""Gross Amt"",""iWidth"":105,""sColName"":""cGrossAmt""},{""blnVisible"":false,""sName"":""Discount %"",""iWidth"":50,""sColName"":""cDiscPer""},{""blnVisible"":false,""sName"":""Discount Amt"",""iWidth"":119,""sColName"":""cDiscAmount""},{""blnVisible"":false,""sName"":""Bill Discount"",""iWidth"":50,""sColName"":""cBillDisc""},{""blnVisible"":true,""sName"":""CRate"",""iWidth"":80,""sColName"":""cCrate""},{""blnVisible"":false,""sName"":""CRate With Tax"",""iWidth"":50,""sColName"":""cCRateWithTax""},{""blnVisible"":false,""sName"":""Taxable"",""iWidth"":80,""sColName"":""ctaxable""},{""blnVisible"":false,""sName"":""Tax %"",""iWidth"":50,""sColName"":""ctaxPer""},{""blnVisible"":false,""sName"":""Tax"",""iWidth"":50,""sColName"":""ctax""},{""blnVisible"":false,""sName"":""IGST"",""iWidth"":50,""sColName"":""cIGST""},{""blnVisible"":false,""sName"":""SGST"",""iWidth"":50,""sColName"":""cSGST""},{""blnVisible"":false,""sName"":""CGST"",""iWidth"":50,""sColName"":""cCGST""},{""blnVisible"":true,""sName"":""Net Amt"",""iWidth"":100,""sColName"":""cNetAmount""},{""blnVisible"":false,""sName"":""ItemID"",""iWidth"":80,""sColName"":""cItemID""},{""blnVisible"":false,""sName"":""Gross Val"",""iWidth"":100,""sColName"":""cGrossValueAfterRateDiscount""},{""blnVisible"":false,""sName"":""Non Taxable"",""iWidth"":100,""sColName"":""cNonTaxable""},{""blnVisible"":false,""sName"":""Cess %"",""iWidth"":100,""sColName"":""cCCessPer""},{""blnVisible"":false,""sName"":""Comp Cess Qty"",""iWidth"":100,""sColName"":""cCCompCessQty""},{""blnVisible"":false,""sName"":""Flood Cess %"",""iWidth"":100,""sColName"":""cFloodCessPer""},{""blnVisible"":false,""sName"":""Flood Cess Amt"",""iWidth"":100,""sColName"":""cFloodCessAmt""},{""blnVisible"":false,""sName"":""Stock MRP"",""iWidth"":100,""sColName"":""cStockMRP""},{""blnVisible"":false,""sName"":""Agent Comm. %"",""iWidth"":100,""sColName"":""cAgentCommPer""},{""blnVisible"":false,""sName"":""Coolie"",""iWidth"":100,""sColName"":""cCoolie""},{""blnVisible"":false,""sName"":""Offer Item"",""iWidth"":100,""sColName"":""cBlnOfferItem""},{""blnVisible"":false,""sName"":""Offer Det."",""iWidth"":100,""sColName"":""cStrOfferDetails""},{""blnVisible"":false,""sName"":""Batch Mode"",""iWidth"":100,""sColName"":""cBatchMode""},{""blnVisible"":false,""sName"":""ID"",""iWidth"":100,""sColName"":""cID""},{""blnVisible"":false,""sName"":"""",""iWidth"":40,""sColName"":""cImgDel""}]' where vchtypeid=40 and gridsettingsjson is null";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"update tblvchtype set gridsettingsjson='[{""blnVisible"":true,""sName"":""Sl.No"",""iWidth"":50,""sColName"":""cSlNo""},{""blnVisible"":true,""sName"":""Item Code"",""iWidth"":130,""sColName"":""CItemCode""},{""blnVisible"":true,""sName"":""Item Name"",""iWidth"":200,""sColName"":""CItemName""},{""blnVisible"":true,""sName"":""Unit"",""iWidth"":69,""sColName"":""CUnit""},{""blnVisible"":true,""sName"":""Batch Code"",""iWidth"":200,""sColName"":""cBarCode""},{""blnVisible"":true,""sName"":""Expiry Date"",""iWidth"":120,""sColName"":""CExpiry""},{""blnVisible"":true,""sName"":""MRP"",""iWidth"":80,""sColName"":""cMRP""},{""blnVisible"":true,""sName"":""PRate"",""iWidth"":80,""sColName"":""cPrate""},{""blnVisible"":false,""sName"":""Rate Inc."",""iWidth"":80,""sColName"":""cRateinclusive""},{""blnVisible"":true,""sName"":""Qty"",""iWidth"":80,""sColName"":""cQty""},{""blnVisible"":false,""sName"":""Free"",""iWidth"":80,""sColName"":""cFree""},{""blnVisible"":false,""sName"":""SRATE1 %"",""iWidth"":80,""sColName"":""cSRate1Per""},{""blnVisible"":false,""sName"":""SRATE1"",""iWidth"":80,""sColName"":""cSRate1""},{""blnVisible"":false,""sName"":""Disc Rs %"",""iWidth"":80,""sColName"":""cSRate2Per""},{""blnVisible"":false,""sName"":""Disc Rs"",""iWidth"":80,""sColName"":""cSRate2""},{""blnVisible"":false,""sName"":""SRate 3 %"",""iWidth"":80,""sColName"":""cSRate3Per""},{""blnVisible"":false,""sName"":""SRate 3"",""iWidth"":80,""sColName"":""cSRate3""},{""blnVisible"":false,""sName"":""WS %"",""iWidth"":80,""sColName"":""cSRate4Per""},{""blnVisible"":false,""sName"":""WS"",""iWidth"":80,""sColName"":""cSRate4""},{""blnVisible"":false,""sName"":""Retail Rs %"",""iWidth"":80,""sColName"":""cSRate5Per""},{""blnVisible"":false,""sName"":""Retail Rs"",""iWidth"":80,""sColName"":""cSRate5""},{""blnVisible"":true,""sName"":""Gross Amt"",""iWidth"":105,""sColName"":""cGrossAmt""},{""blnVisible"":false,""sName"":""Discount %"",""iWidth"":50,""sColName"":""cDiscPer""},{""blnVisible"":false,""sName"":""Discount Amt"",""iWidth"":119,""sColName"":""cDiscAmount""},{""blnVisible"":false,""sName"":""Bill Discount"",""iWidth"":50,""sColName"":""cBillDisc""},{""blnVisible"":true,""sName"":""CRate"",""iWidth"":80,""sColName"":""cCrate""},{""blnVisible"":false,""sName"":""CRate With Tax"",""iWidth"":50,""sColName"":""cCRateWithTax""},{""blnVisible"":false,""sName"":""Taxable"",""iWidth"":80,""sColName"":""ctaxable""},{""blnVisible"":false,""sName"":""Tax %"",""iWidth"":50,""sColName"":""ctaxPer""},{""blnVisible"":false,""sName"":""Tax"",""iWidth"":50,""sColName"":""ctax""},{""blnVisible"":false,""sName"":""IGST"",""iWidth"":50,""sColName"":""cIGST""},{""blnVisible"":false,""sName"":""SGST"",""iWidth"":50,""sColName"":""cSGST""},{""blnVisible"":false,""sName"":""CGST"",""iWidth"":50,""sColName"":""cCGST""},{""blnVisible"":true,""sName"":""Net Amt"",""iWidth"":100,""sColName"":""cNetAmount""},{""blnVisible"":false,""sName"":""ItemID"",""iWidth"":80,""sColName"":""cItemID""},{""blnVisible"":false,""sName"":""Gross Val"",""iWidth"":100,""sColName"":""cGrossValueAfterRateDiscount""},{""blnVisible"":false,""sName"":""Non Taxable"",""iWidth"":100,""sColName"":""cNonTaxable""},{""blnVisible"":false,""sName"":""Cess %"",""iWidth"":100,""sColName"":""cCCessPer""},{""blnVisible"":false,""sName"":""Comp Cess Qty"",""iWidth"":100,""sColName"":""cCCompCessQty""},{""blnVisible"":false,""sName"":""Flood Cess %"",""iWidth"":100,""sColName"":""cFloodCessPer""},{""blnVisible"":false,""sName"":""Flood Cess Amt"",""iWidth"":100,""sColName"":""cFloodCessAmt""},{""blnVisible"":false,""sName"":""Stock MRP"",""iWidth"":100,""sColName"":""cStockMRP""},{""blnVisible"":false,""sName"":""Agent Comm. %"",""iWidth"":100,""sColName"":""cAgentCommPer""},{""blnVisible"":false,""sName"":""Coolie"",""iWidth"":100,""sColName"":""cCoolie""},{""blnVisible"":false,""sName"":""Offer Item"",""iWidth"":100,""sColName"":""cBlnOfferItem""},{""blnVisible"":false,""sName"":""Offer Det."",""iWidth"":100,""sColName"":""cStrOfferDetails""},{""blnVisible"":false,""sName"":""Batch Mode"",""iWidth"":100,""sColName"":""cBatchMode""},{""blnVisible"":false,""sName"":""ID"",""iWidth"":100,""sColName"":""cID""},{""blnVisible"":false,""sName"":"""",""iWidth"":40,""sColName"":""cImgDel""}]' where vchtypeid=40 and gridsettingsjson is null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblitemmaster alter column PLUNo numeric(18,0) not null";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblstock add autobatchid numeric(18,0) not null default 0";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }



            //BoardRateQuery = @BoardRateQuery, BoardRateFileName = @BoardRateFileName, BoardRateExportType = @BoardRateExportType
            try
            {
                sQuery = @"alter table tblvchtype add BoardRateExportType varchar(2000)";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblvchtype SET BoardRateExportType = '' WHERE BoardRateExportType IS NULL";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblvchtype add BoardRateFileName varchar(2000)";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblvchtype SET BoardRateFileName = '' WHERE BoardRateFileName IS NULL";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblvchtype add BoardRateQuery varchar(2000)";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblvchtype SET BoardRateQuery = '' WHERE BoardRateQuery IS NULL";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"alter table tblvchtype add PrintSettings varchar(2000)";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblvchtype SET PrintSettings = '' WHERE PrintSettings IS NULL";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }


            try
            {
                sQuery = @"CREATE TABLE [dbo].[tblCashDeskMaster](
                            [PaymentID][numeric](18, 0) NOT NULL,
                            [PaymentType] [varchar](50) NULL,
	                        [LedgerID] [numeric](18, 0) NULL
                        ) ON[PRIMARY]";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"CREATE TABLE [dbo].[tblCashDeskItems](
	                        [ID] [numeric](18, 0) NULL,
	                        [PaymentType] [varchar](500) NULL,
	                        [PaymentID] [numeric](18, 0) NULL,
	                        [LedgerID] [numeric](18, 0) NULL,
	                        [Amount] [float] NULL
                        ) ON [PRIMARY]";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblCashDeskItems add PreviousBalance decimal ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblCashDeskItems add TotalOutstanting decimal ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblCashDeskItems add CurrentReceipt decimal ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"alter table tblCashDeskItems add CurrentBalance  decimal ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"UPDATE tblCashDeskItems SET PreviousBalance = 0 WHERE PreviousBalance IS NULL ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblCashDeskItems SET TotalOutstanting = 0 WHERE TotalOutstanting IS NULL ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblCashDeskItems SET CurrentReceipt = 0 WHERE CurrentReceipt IS NULL ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"UPDATE tblCashDeskItems SET CurrentBalance = 0 WHERE CurrentBalance IS NULL ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"ALTER TABLE TBLCASHDESKMASTER ADD CONSTRAINT PK_TBLCASHDESKMASTER primary key (PaymentID) ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"ALTER TABLE TBLCASHDESKMASTER ADD CONSTRAINT IX_TBLCASHDESKMASTER unique nonclustered (PaymentType) ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"ALTER TABLE TBLCASHDESKMASTER ADD CONSTRAINT PK_TBLCASHDESKMASTER primary key (PaymentID) ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"INSERT INTO tblCashDeskMaster(PaymentID,PaymentType,LedgerID)
                            VALUES (1,'CASH',3)";
                fnExecuteNonQuery(sQuery, false);

                sQuery = @"INSERT INTO tblCashDeskMaster(PaymentID,PaymentType,LedgerID)
                            VALUES (2,'BANK',4)";
                fnExecuteNonQuery(sQuery, false);

                //sQuery = @"INSERT INTO tblCashDeskMaster(PaymentID,PaymentType,LedgerID)
                //            VALUES (3,'CREDIT',0)";
                //fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }



            CreateViewsAndProcudures();


            return true;
        }

        public void CreateViewsAndProcudures()
        {
            string sQuery = "";

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[trgUpdateQOHInStock] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"create TRIGGER [dbo].[trgUpdateQOHInStock]
                           ON  [dbo].[tblStockHistory]
                           AFTER INSERT,DELETE,UPDATE 
                        AS 
                        BEGIN
	                        -- SET NOCOUNT ON added to prevent extra result sets from
	                        -- interfering with SELECT statements.
	                        SET NOCOUNT OFF;

	                        DECLARE @ItemID NUMERIC
	                        DECLARE @BatchUnique VARCHAR(500)
	                        DECLARE @CCID NUMERIC
	                        DECLARE @TenantID NUMERIC

	                        DECLARE @Action as char(1);
                            SET @Action = ''
	                        if EXISTS(SELECT * FROM INSERTED) AND EXISTS(SELECT * FROM DELETED)
		                        set @Action = 'U'  
	                        else
		                        if EXISTS(SELECT * FROM INSERTED) 
			                        set @Action = 'I'  
		                        else
			                        if EXISTS(SELECT * FROM DELETED)
				                        set @Action = 'D'  

	                        -- For Getting the ID
	                        if  @Action = 'D'
	                            SELECT @ItemID = ItemID, @BatchUnique = BatchUnique, @CCID = CCID, @TenantID = TenantID FROM deleted; 
	                        else -- Insert or Update
	                            SELECT @ItemID = ItemID, @BatchUnique = BatchUnique, @CCID = CCID, @TenantID = TenantID FROM inserted; 

	                        update tblStock set QOH = ISNULL((select sum(qtyin)-sum(qtyout) from tblstockhistory as h 
		                        where h.ItemID=@ItemID and h.BatchUnique=@BatchUnique and h.CCID=@CCID 
			                        and h.TenantID=@TenantID),0) 
	                        where tblStock.ItemID = @ItemID and tblStock.BatchUnique = @BatchUnique and tblStock.CCID = @CCID 
		                        and tblStock.TenantID = @TenantID 
                        END";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[UspItemMasterInsert] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspItemMasterInsert] (@ItemID           NUMERIC (18, 0),
                                             @ItemCode         VARCHAR (100),
                                             @ItemName         VARCHAR (100),
                                             @CategoryID       NUMERIC (18, 0),
                                             @Description      VARCHAR (1000),
                                             @PRate            MONEY,
                                             @SrateCalcMode    INT,
                                             @CRateAvg         MONEY,
                                             @Srate1Per        MONEY,
                                             @SRate1           MONEY,
                                             @Srate2Per        MONEY,
                                             @SRate2           MONEY,
                                             @Srate3Per        MONEY,
                                             @SRate3           MONEY,
                                             @Srate4           MONEY,
                                             @Srate4Per        MONEY,
                                             @SRate5           MONEY,
                                             @Srate5Per        MONEY,
                                             @MRP              MONEY,
                                             @ROL              FLOAT,
                                             @Rack             VARCHAR (100),
                                             @Manufacturer     VARCHAR (100),
                                             @ActiveStatus     NUMERIC (1, 0),
                                             @IntLocal         INT,
                                             @ProductType      VARCHAR (50),
                                             @ProductTypeID    FLOAT,
                                             @LedgerID         NUMERIC (18, 0),
                                             @UNITID           NUMERIC (18, 0),
                                             @Notes            VARCHAR (1000),
                                             @agentCommPer     FLOAT,
                                             @BlnExpiryItem    INT,
                                             @Coolie           NUMERIC (18, 0),
                                             @FinishedGoodID   INT,
                                             @MinRate          FLOAT,
                                             @MaxRate          FLOAT,
                                             @PLUNo            NUMERIC(18, 0),
                                             @HSNID            NUMERIC (18, 0),
                                             @iCatDiscPer      FLOAT,
                                             @IPGDiscPer       FLOAT,
                                             @ImanDiscPer      FLOAT,
                                             @ItemNameUniCode  NVARCHAR (500),
                                             @Minqty           FLOAT,
                                             @MNFID            NUMERIC (18, 0),
                                             @PGID             NUMERIC (18, 0),
                                             @ItemCodeUniCode  NVARCHAR (50),
                                             @UPC              VARCHAR (50),
                                             @BatchMode        VARCHAR (50),
                                             @blnExpiry        NUMERIC (1, 0),
                                             @Qty              FLOAT,
                                             @MaxQty           FLOAT,
                                             @IntNoOrWeight    NUMERIC (18, 0),
                                             @SystemName       VARCHAR (50),
                                             @UserID           NUMERIC (18, 0),
                                             @LastUpdateDate   DATETIME,
                                             @LastUpdateTime   DATETIME,
                                             @TenantID         NUMERIC (18, 0),
                                             @blnCessOnTax     NUMERIC (18, 0),
                                             @CompCessQty      FLOAT,
                                             @CGSTTaxPer       FLOAT,
                                             @SGSTTaxPer       FLOAT,
                                             @IGSTTaxPer       FLOAT,
                                             @CessPer          FLOAT,
                                             @VAT              FLOAT,
                                             @CategoryIDs      VARCHAR (1000),
                                             @ColorIDs         VARCHAR (1000),
                                             @SizeIDs          VARCHAR (1000),
                                             @BrandDisPer      FLOAT,
                                             @DGroupID         NUMERIC (18, 0),
                                             @DGroupDisPer     FLOAT,
                                             @Action           INT=0,
                                             @BatchCode        VARCHAR(50),
                                             @CostRateInc      FLOAT,
                                             @CostRateExcl     FLOAT,
                                             @PRateExcl        FLOAT,
                                             @PrateInc         FLOAT,
                                             @BrandID          NUMERIC (18, 0),
                                             @AltUnitID        NUMERIC (18, 0),
                                             @ConvFactor       NUMERIC (18, 5),
                                             @ShelfLife        NUMERIC (18, 0),
                                             @SRateInclusive   NUMERIC (1, 0),
                                             @PRateInclusive   NUMERIC (1, 0),
                                             @Slabsys          NUMERIC (1, 0),
                                             @DiscPer          FLOAT,
                                             @DepartmentID     NUMERIC (18, 0),
                                             @DefaultExpInDays NUMERIC (18, 0))
                                AS
                                  BEGIN
                                      DECLARE @RetResult INT
                                      DECLARE @BatchID NUMERIC(18, 0)
                                      DECLARE @StockID NUMERIC(18, 0)
                                      DECLARE @ExpDt DATETIME = Dateadd(year, 8, Getdate())
                                      DECLARE @LastInvDt DATETIME = Getdate()
                                      DECLARE @GetPLUNo NUMERIC(18, 0)
                                      DECLARE @TransType CHAR(1)

                                      BEGIN try
                                          BEGIN TRANSACTION;

                                          IF NOT EXISTS(SELECT *
                                                        FROM   tblonetimemaster
                                                        WHERE  otmtype = 'ITMRACK'
                                                               AND Ltrim(Rtrim(otmdata)) = @Rack)
                                            BEGIN
                                                INSERT INTO tblonetimemaster
                                                            (otmdata,
                                                             otmvalue,
                                                             otmdescription,
                                                             otmtype,
                                                             tenantid)
                                                VALUES     (@Rack,
                                                            0,
                                                            'Rack Details',
                                                            'ITMRACK',
                                                            @TenantID)
                                            END

                                          IF NOT EXISTS(SELECT *
                                                        FROM   tblonetimemaster
                                                        WHERE  otmtype = 'PRODCLASS'
                                                               AND Ltrim(Rtrim(otmdata)) = @ProductType)
                                            BEGIN
                                                INSERT INTO tblonetimemaster
                                                            (otmdata,
                                                             otmvalue,
                                                             otmdescription,
                                                             otmtype,
                                                             tenantid)
                                                VALUES     (@ProductType,
                                                            0,
                                                            'Product Class Details',
                                                            'PRODCLASS',
                                                            @TenantID)
                                            END

                                          SELECT @iCatDiscPer = Isnull(catdiscper, 0)
                                          FROM   tblcategories
                                          WHERE  tenantid = @TenantID
                                                 AND categoryid = @CategoryID

                                          SELECT @ImanDiscPer = Isnull(discper, 0)
                                          FROM   tblmanufacturer
                                          WHERE  tenantid = @TenantID
                                                 AND mnfid = @MNFID

                                          SELECT @BrandDisPer = Isnull(discper, 0)
                                          FROM   tblbrand
                                          WHERE  tenantid = @TenantID
                                                 AND brandid = @BrandID

                                          SELECT @DGroupDisPer = Isnull(discper, 0)
                                          FROM   tbldiscountgroup
                                          WHERE  tenantid = @TenantID
                                                 AND discountgroupid = @DGroupID

                                          IF @Action = 0
                                            BEGIN
                                                SELECT @GetPLUNo = Isnull(Max(pluno) + 1, 0)
                                                FROM   tblitemmaster

                                                IF @BatchMode = 3
                                                  BEGIN
                                                      IF @PLUNo > 0
                                                        BEGIN
                                                            SET @PLUNo = @PLUNo
                                                        END
                                                      ELSE
                                                        BEGIN
                                                            SET @PLUNo = @GetPLUNo
                                                        END
                                                  END
                                                ELSE
                                                  BEGIN
                                                      SET @PLUNo = 0
                                                  END

                                                IF NOT EXISTS(SELECT *
                                                              FROM   tblstock
                                                              WHERE  tenantid = @TenantID)
                                                  BEGIN
                                                      SET @StockID = 1
                                                      SET @BatchID = 1
                                                  END
                                                ELSE
                                                  BEGIN
                                                      SELECT @StockID = Isnull(Max(stockid) + 1, 0)
                                                      FROM   tblstock
                                                      WHERE  tenantid = @TenantID

                                                      SELECT @BatchID = Isnull(Max(batchid) + 1, 0)
                                                      FROM   tblstock
                                                      WHERE  tenantid = @TenantID
                                                  END

                                                INSERT INTO tblitemmaster
                                                            (itemid,
                                                             itemcode,
                                                             itemname,
                                                             categoryid,
                                                             description,
                                                             prate,
                                                             sratecalcmode,
                                                             crateavg,
                                                             srate1per,
                                                             srate1,
                                                             srate2per,
                                                             srate2,
                                                             srate3per,
                                                             srate3,
                                                             srate4,
                                                             srate4per,
                                                             srate5,
                                                             srate5per,
                                                             mrp,
                                                             rol,
                                                             rack,
                                                             manufacturer,
                                                             activestatus,
                                                             intlocal,
                                                             producttype,
                                                             producttypeid,
                                                             ledgerid,
                                                             unitid,
                                                             notes,
                                                             agentcommper,
                                                             blnexpiryitem,
                                                             coolie,
                                                             finishedgoodid,
                                                             minrate,
                                                             maxrate,
                                                             pluno,
                                                             hsnid,
                                                             icatdiscper,
                                                             ipgdiscper,
                                                             imandiscper,
                                                             itemnameunicode,
                                                             minqty,
                                                             mnfid,
                                                             pgid,
                                                             itemcodeunicode,
                                                             upc,
                                                             batchmode,
                                                             blnexpiry,
                                                             qty,
                                                             maxqty,
                                                             intnoorweight,
                                                             systemname,
                                                             userid,
                                                             lastupdatedate,
                                                             lastupdatetime,
                                                             tenantid,
                                                             blncessontax,
                                                             compcessqty,
                                                             cgsttaxper,
                                                             sgsttaxper,
                                                             igsttaxper,
                                                             cessper,
                                                             vat,
                                                             categoryids,
                                                             colorids,
                                                             sizeids,
                                                             branddisper,
                                                             dgroupid,
                                                             dgroupdisper,
                                                             brandid,
                                                             altunitid,
                                                             convfactor,
                                                             shelflife,
                                                             srateinclusive,
                                                             prateinclusive,
                                                             slabsys,
                                                             discper,
                                                             departmentid,
                                                             defaultexpindays)
                                                VALUES     (@ItemID,
                                                            @ItemCode,
                                                            @ItemName,
                                                            @CategoryID,
                                                            @Description,
                                                            @PRate,
                                                            @SrateCalcMode,
                                                            @CRateAvg,
                                                            @Srate1Per,
                                                            @SRate1,
                                                            @Srate2Per,
                                                            @SRate2,
                                                            @Srate3Per,
                                                            @SRate3,
                                                            @Srate4,
                                                            @Srate4Per,
                                                            @SRate5,
                                                            @Srate5Per,
                                                            @MRP,
                                                            @ROL,
                                                            @Rack,
                                                            @Manufacturer,
                                                            @ActiveStatus,
                                                            @IntLocal,
                                                            @ProductType,
                                                            @ProductTypeID,
                                                            @LedgerID,
                                                            @UNITID,
                                                            @Notes,
                                                            @agentCommPer,
                                                            @BlnExpiryItem,
                                                            @Coolie,
                                                            @FinishedGoodID,
                                                            @MinRate,
                                                            @MaxRate,
                                                            @PLUNo,
                                                            @HSNID,
                                                            @iCatDiscPer,
                                                            @IPGDiscPer,
                                                            @ImanDiscPer,
                                                            @ItemNameUniCode,
                                                            @Minqty,
                                                            @MNFID,
                                                            @PGID,
                                                            @ItemCodeUniCode,
                                                            @UPC,
                                                            @BatchMode,
                                                            @blnExpiry,
                                                            @Qty,
                                                            @MaxQty,
                                                            @IntNoOrWeight,
                                                            @SystemName,
                                                            @UserID,
                                                            @LastUpdateDate,
                                                            @LastUpdateTime,
                                                            @TenantID,
                                                            @blnCessOnTax,
                                                            @CompCessQty,
                                                            @CGSTTaxPer,
                                                            @SGSTTaxPer,
                                                            @IGSTTaxPer,
                                                            @CessPer,
                                                            @VAT,
                                                            @CategoryIDs,
                                                            @ColorIDs,
                                                            @SizeIDs,
                                                            @BrandDisPer,
                                                            @DGroupID,
                                                            @DGroupDisPer,
                                                            @BrandID,
                                                            @AltUnitID,
                                                            @ConvFactor,
                                                            @ShelfLife,
                                                            @SRateInclusive,
                                                            @PRateInclusive,
                                                            @Slabsys,
                                                            @DiscPer,
                                                            @DepartmentID,
                                                            @DefaultExpInDays)

                                                SET @RetResult = 1;
                                                SET @TransType = 'S';
                                            END

                                          IF @Action = 1
                                            BEGIN
                                                SELECT @GetPLUNo = Isnull(Max(pluno) + 1, 0)
                                                FROM   tblitemmaster

                                                IF @BatchMode = 3
                                                  BEGIN
                                                      IF @PLUNo = 0
                                                        BEGIN
                                                            SET @PLUNo = @GetPLUNo
                                                        END
                                                  END

                                                IF @ActiveStatus = 0
                                                  BEGIN
                                                      UPDATE tblitemmaster
                                                      SET    activestatus = @ActiveStatus
                                                      WHERE  itemid = @ItemID
                                                             AND tenantid = @TenantID
                                                  END
                                                ELSE
                                                  BEGIN
                                                      UPDATE tblitemmaster
                                                      SET    itemcode = @ItemCode,
                                                             itemname = @ItemName,
                                                             categoryid = @CategoryID,
                                                             description = @Description,
                                                             prate = @PRate,
                                                             sratecalcmode = @SrateCalcMode,
                                                             crateavg = @CRateAvg,
                                                             srate1per = @Srate1Per,
                                                             srate1 = @SRate1,
                                                             srate2per = @Srate2Per,
                                                             srate2 = @SRate2,
                                                             srate3per = @Srate3Per,
                                                             srate3 = @SRate3,
                                                             srate4 = @Srate4,
                                                             srate4per = @Srate4Per,
                                                             srate5 = @SRate5,
                                                             srate5per = @Srate5Per,
                                                             mrp = @MRP,
                                                             rol = @ROL,
                                                             rack = @Rack,
                                                             manufacturer = @Manufacturer,
                                                             activestatus = @ActiveStatus,
                                                             intlocal = @IntLocal,
                                                             producttype = @ProductType,
                                                             producttypeid = @ProductTypeID,
                                                             ledgerid = @LedgerID,
                                                             unitid = @UNITID,
                                                             notes = @Notes,
                                                             agentcommper = @agentCommPer,
                                                             blnexpiryitem = @BlnExpiryItem,
                                                             coolie = @Coolie,
                                                             finishedgoodid = @FinishedGoodID,
                                                             minrate = @MinRate,
                                                             maxrate = @MaxRate,
                                                             pluno = @PLUNo,
                                                             hsnid = @HSNID,
                                                             icatdiscper = @iCatDiscPer,
                                                             ipgdiscper = @IPGDiscPer,
                                                             imandiscper = @ImanDiscPer,
                                                             itemnameunicode = @ItemNameUniCode,
                                                             minqty = @Minqty,
                                                             mnfid = @MNFID,
                                                             pgid = @PGID,
                                                             itemcodeunicode = @ItemCodeUniCode,
                                                             upc = @UPC,
                                                             batchmode = @BatchMode,
                                                             blnexpiry = @blnExpiry,
                                                             qty = @Qty,
                                                             maxqty = @MaxQty,
                                                             intnoorweight = @IntNoOrWeight,
                                                             systemname = @SystemName,
                                                             userid = @UserID,
                                                             lastupdatedate = @LastUpdateDate,
                                                             lastupdatetime = @LastUpdateTime,
                                                             blncessontax = @blnCessOnTax,
                                                             compcessqty = @CompCessQty,
                                                             cgsttaxper = @CGSTTaxPer,
                                                             sgsttaxper = @SGSTTaxPer,
                                                             igsttaxper = @IGSTTaxPer,
                                                             cessper = @CessPer,
                                                             vat = @VAT,
                                                             categoryids = @CategoryIDs,
                                                             colorids = @ColorIDs,
                                                             sizeids = @SizeIDs,
                                                             branddisper = @BrandDisPer,
                                                             dgroupid = @DGroupID,
                                                             dgroupdisper = @DGroupDisPer,
                                                             brandid = @BrandID,
                                                             altunitid = @AltUnitID,
                                                             convfactor = @ConvFactor,
                                                             shelflife = @ShelfLife,
                                                             srateinclusive = @SRateInclusive,
                                                             prateinclusive = @PRateInclusive,
                                                             slabsys = @Slabsys,
                                                             discper = @DiscPer,
                                                             departmentid = @DepartmentID,
                                                             defaultexpindays = @DefaultExpInDays
                                                      WHERE  itemid = @ItemID
                                                             AND tenantid = @TenantID

                                                      SELECT @StockID = stockid
                                                      FROM   tblstock
                                                      WHERE  batchcode = @BatchCode
                                                             AND tenantid = @TenantID
                                                             AND itemid = @ItemID

                                                      SET @StockID=Isnull(@StockID, 0)
                                                  END

                                                SET @RetResult = 1;
                                                SET @TransType = 'E';
                                            END

                                          IF @Action = 2
                                            BEGIN

                                                DELETE FROM tblStockHistory
                                                WHERE  itemid = @ItemID AND QtyIn=0 AND QtyOut=0 
                                                       AND tenantid = @TenantID

                                                DELETE FROM tblstock
                                                WHERE  itemid = @ItemID
                                                       AND tenantid = @TenantID

                                                DELETE FROM tblitemmaster
                                                WHERE  itemid = @ItemID
                                                       AND tenantid = @TenantID

                                                SET @RetResult = 0;
                                                SET @TransType = 'D';
                                            END

                                          COMMIT TRANSACTION;

                                          SELECT @RetResult AS SqlSpResult,
                                                 @ItemID    AS TransID,
                                                 @TransType AS TransactType
                                      END try

                                      BEGIN catch
                                          ROLLBACK;

                                          SELECT -1                AS SqlSpResult,
                                                 Error_number()    AS ErrorNumber,
                                                 Error_state()     AS ErrorState,
                                                 Error_severity()  AS ErrorSeverity,
                                                 Error_procedure() AS ErrorProcedure,
                                                 Error_line()      AS ErrorLine,
                                                 Error_message()   AS ErrorMessage;
                                      END catch;
                                  END ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[UspGetItemMaster] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspGetItemMaster] (@ItemID   NUMERIC (18, 0),
                                          @TenantID NUMERIC (18, 0))
                            AS
                              BEGIN
                                  DECLARE @CatIDsNames VARCHAR(1000)
                                  DECLARE @CatIDs VARCHAR(1000)

                                  IF @ItemID <> 0
                                    BEGIN
                                        SELECT @CatIDs = categoryids
                                        FROM   tblitemmaster
                                        WHERE  itemid = @ItemID
                                               AND tenantid = @TenantID

                                        SELECT @CatIDsNames = COALESCE(@CatIDsNames + ',', '') + category
                                        FROM   tblcategories
                                        WHERE  tenantid = @TenantID
                                               AND ',' + @CatIDs + ',' LIKE '%,' + CONVERT(VARCHAR(50),
                                                                            categoryid
                                                                            )
                                                                            +
                                                                            ',%';

                                        SELECT I.itemid,
                                               itemcode,
                                               itemname,
                                               I.categoryid,
                                               description,
                                               Isnull(I.prate, 0)          AS PRate,
                                               Isnull(sratecalcmode, 0)    AS SrateCalcMode,
                                               crateavg,
				                               CostRateInc,
				                               CostRateExcl,
                                               srate1per,
                                               I.srate1,
                                               srate2per,
                                               I.srate2,
                                               srate3per,
                                               I.srate3,
                                               I.srate4,
                                               srate4per,
                                               I.srate5,
                                               srate5per,
                                               Isnull(I.mrp, 0)            AS MRP,
                                               rol,
                                               rack,
                                               manufacturer,
                                               activestatus,
                                               intlocal,
                                               producttype,
                                               producttypeid,
                                               ledgerid,
                                               I.unitid,
                                               notes,
                                               agentcommper,
                                               blnexpiryitem,
                                               coolie,
                                               finishedgoodid,
                                               minrate,
                                               maxrate,
                                               pluno,
                                               i.hsnid,
				                               h.HSNCODE,
                                               icatdiscper,
                                               ipgdiscper,
                                               imandiscper,
                                               itemnameunicode,
                                               minqty,
                                               mnfid,
                                               pgid,
                                               itemcodeunicode,
                                               upc,
                                               batchmode,
                                               blnexpiry,
                                               qty,
                                               maxqty,
                                               intnoorweight,
                                               I.systemname,
                                               I.userid,
                                               I.lastupdatedate,
                                               I.lastupdatetime,
                                               I.tenantid,
                                               blncessontax,
                                               i.compcessqty,
                                               i.cgsttaxper,
                                               i.sgsttaxper,
                                               i.igsttaxper,
                                               i.cessper,
                                               vat,
                                               categoryids,
                                               colorids,
                                               sizeids,
                                               branddisper,
                                               dgroupid,
                                               dgroupdisper,
                                               @CatIDsNames                AS Categories,
                                               U.unitshortname             AS [Unit],
                                               Isnull(batchcode, 0)        AS BatchCode,
                                               brandid,
                                               Isnull(altunitid, 0)        AS AltUnitID,
                                               Isnull(convfactor, 0)       AS ConvFactor,
                                               Isnull(shelflife, 0)        AS Shelflife,
                                               Isnull(srateinclusive, 0)   AS SRateInclusive,
                                               Isnull(prateinclusive, 0)   AS PRateInclusive,
                                               Isnull(slabsys, 0)          AS Slabsystem,
                                               batchmode,
                                               Isnull(discper, 0)          AS DiscPer,
                                               S.batchunique,
                                               S.stockid,
                                               Isnull(departmentid, 0)     AS DepartmentID,
                                               Isnull(i.compcessqty, 0)      AS CompCessQty,
                                               Isnull(defaultexpindays, 0) AS DefaultExpInDays
                                        FROM   tblitemmaster I
                                               INNER JOIN tblcategories C
                                                       ON C.categoryid = I.categoryid
                                               LEFT JOIN tblunit U
                                                      ON U.unitid = I.unitid
                                                         AND U.tenantid = @TenantID
                                               LEFT JOIN tblHSNCode h
                                                      ON h.HSNID = I.HSNID 
                                                         AND h.tenantid = @TenantID
                                               LEFT JOIN tblstock S
                                                      ON S.itemid = I.itemid
                                        WHERE  I.itemid = @ItemID
                                               AND I.tenantid = @TenantID
                                    END
                                  ELSE
                                    BEGIN
                                        SELECT I.itemid,
                                               itemcode             AS [Item Code],
                                               itemname             AS [Item],
                                               U.unitshortname      AS [Unit],
                                               C.category,
                                               description,
                                               I.mrp,
                                               hsnid                AS [HSN Code],
                                               ( CASE
                                                   WHEN activestatus = 1 THEN 'Active'
                                                   ELSE 'In Active'
                                                 END )              AS Status,
                                               Isnull(batchcode, 0) AS BatchCode
                                        FROM   tblitemmaster I
                                               INNER JOIN tblcategories C
                                                       ON C.categoryid = I.categoryid
                                               LEFT JOIN tblunit U
                                                      ON U.unitid = I.unitid
                                                         AND U.tenantid = @TenantID
                                               LEFT JOIN tblstock S
                                                      ON S.itemid = I.itemid
                                        WHERE  I.tenantid = @TenantID
                                    END
                              END ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[UspHSNMasterInsert] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspHSNMasterInsert](
                             @HID  NUMERIC  (18,0),
                             @HSNCODE  VARCHAR  (50),
                             @HSNDECRIPTION    VARCHAR  (50),
	                         @HSNType        VARCHAR  (50),
                             @CGSTTaxPer FLOAT,
                             @SGSTTaxPer  FLOAT,
                             @IGSTTaxPer  FLOAT,
                             @CessPer   FLOAT,
                             @CompCessQty  FLOAT,
	                         @CGSTTaxPer1 FLOAT,
                             @SGSTTaxPer1 FLOAT,
                             @IGSTTaxPer1 FLOAT,
                             @CGSTTaxPer2 FLOAT,
                             @SGSTTaxPer2 FLOAT,
                             @IGSTTaxPer2 FLOAT,
                             @CGSTTaxPer3 FLOAT,
                             @SGSTTaxPer3 FLOAT,
                             @IGSTTaxPer3 FLOAT,
                             @CGSTTaxPer4 FLOAT,
                             @SGSTTaxPer4 FLOAT,
                             @IGSTTaxPer4 FLOAT,
	                         @ValueStartSB1  NUMERIC  (18,0),
                             @ValueStartSB2  NUMERIC  (18,0),
                             @ValueStartSB3  NUMERIC  (18,0),
                             @ValueStartSB4  NUMERIC  (18,0),
                             @ValueEndSB1 NUMERIC  (18,0),
                             @ValueEndSB2 NUMERIC  (18,0),
                             @ValueEndSB3 NUMERIC  (18,0),
                             @ValueEndSB4 NUMERIC  (18,0),
	                         @blnSlabSystem NUMERIC  (18,0),
                             @TenantID   NUMERIC  (18,0),
                        @Action             INT=0
                        )
                        AS
                        BEGIN
                        DECLARE @RetResult      INT
                        BEGIN TRY
                        BEGIN TRANSACTION;
                        IF @Action = 0
                        BEGIN
                             INSERT INTO tblHSNCode(
	                         HSNID,
	                         HSNCODE,
	                         HSNDECRIPTION,
	                         HSNType,
                             CGSTTaxPer ,
                             SGSTTaxPer  ,
                             IGSTTaxPer  ,
                             CessPer   ,
                             CompCessQty  ,
	                         CGSTTaxPer1 ,
                             SGSTTaxPer1 ,
                             IGSTTaxPer1 ,
                             CGSTTaxPer2 ,
                             SGSTTaxPer2 ,
                             IGSTTaxPer2 ,
                             CGSTTaxPer3 ,
                             SGSTTaxPer3 ,
                             IGSTTaxPer3 ,
                             CGSTTaxPer4 ,
                             SGSTTaxPer4 ,
                             IGSTTaxPer4 ,
	                         ValueStartSB1  ,
                             ValueStartSB2  ,
                             ValueStartSB3  ,
                             ValueStartSB4  ,
                             ValueEndSB1 ,
                             ValueEndSB2 ,
                             ValueEndSB3 ,
                             ValueEndSB4 ,
	                         blnSlabSystem,
	                         TenantID)
                             VALUES(
	                         @HID,
	                         @HSNCODE,
	                         @HSNDECRIPTION,
	                         @HSNType,
                             @CGSTTaxPer ,
                             @SGSTTaxPer  ,
                             @IGSTTaxPer  ,
                             @CessPer   ,
                             @CompCessQty  ,
	                         @CGSTTaxPer1 ,
                             @SGSTTaxPer1 ,
                             @IGSTTaxPer1 ,
                             @CGSTTaxPer2 ,
                             @SGSTTaxPer2 ,
                             @IGSTTaxPer2 ,
                             @CGSTTaxPer3 ,
                             @SGSTTaxPer3 ,
                             @IGSTTaxPer3 ,
                             @CGSTTaxPer4 ,
                             @SGSTTaxPer4 ,
                             @IGSTTaxPer4 ,
	                         @ValueStartSB1  ,
                             @ValueStartSB2  ,
                             @ValueStartSB3  ,
                             @ValueStartSB4  ,
                             @ValueEndSB1 ,
                             @ValueEndSB2 ,
                             @ValueEndSB3 ,
                             @ValueEndSB4 ,
	                         @blnSlabSystem,
	                         @TenantID )
                             SET @RetResult = 1;
                        END
                        IF @Action = 1
                        BEGIN
                             UPDATE tblHSNCode SET 
	                         HSNID=@HID,
	                         HSNCODE=@HSNCODE,
	                         HSNDECRIPTION=@HSNDECRIPTION,
	                         HSNType=@HSNType,
                             CGSTTaxPer=@CGSTTaxPer ,
                             SGSTTaxPer=@SGSTTaxPer  ,
                             IGSTTaxPer=@IGSTTaxPer  ,
                             CessPer =@CessPer   ,
                             CompCessQty=@CompCessQty  ,
	                         CGSTTaxPer1=@CGSTTaxPer1 ,
                             SGSTTaxPer1=@SGSTTaxPer1 ,
                             IGSTTaxPer1=@IGSTTaxPer1 ,
                             CGSTTaxPer2=@CGSTTaxPer2 ,
                             SGSTTaxPer2=@SGSTTaxPer2 ,
                             IGSTTaxPer2=@IGSTTaxPer2 ,
                             CGSTTaxPer3=@CGSTTaxPer3 ,
                             SGSTTaxPer3=@SGSTTaxPer3 ,
                             IGSTTaxPer3=@IGSTTaxPer3 ,
                             CGSTTaxPer4=@CGSTTaxPer4 ,
                             SGSTTaxPer4=@SGSTTaxPer4 ,
                             IGSTTaxPer4=@IGSTTaxPer4 ,
	                         ValueStartSB1=@ValueStartSB1  ,
                             ValueStartSB2=@ValueStartSB2  ,
                             ValueStartSB3=@ValueStartSB3  ,
                             ValueStartSB4=@ValueStartSB4  ,
                             ValueEndSB1=@ValueEndSB1 ,
                             ValueEndSB2=@ValueEndSB2 ,
                             ValueEndSB3=@ValueEndSB3 ,
                             ValueEndSB4=@ValueEndSB4 ,
	                         blnSlabSystem=@blnSlabSystem,
	                         TenantID=@TenantID
                             WHERE HID=@HID
                             SET @RetResult = 1;
                        END
                        IF @Action = 2
                        BEGIN
                             DELETE FROM tblHSNCode WHERE HID=@HID
                             SET @RetResult = 0;
                        END
                        COMMIT TRANSACTION;
                        SELECT @RetResult as SqlSpResult
                        END TRY
                        BEGIN CATCH
                        ROLLBACK;
                        SELECT
                        - 1 as SqlSpResult,
                        ERROR_NUMBER() AS ErrorNumber,
                        ERROR_STATE() AS ErrorState,
                        ERROR_SEVERITY() AS ErrorSeverity,
                        ERROR_PROCEDURE() AS ErrorProcedure,
                        ERROR_LINE() AS ErrorLine,
                        ERROR_MESSAGE() AS ErrorMessage;
                        END CATCH;
                        END";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[UspGetHSNFromItemMaster] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspGetHSNFromItemMaster] ( 
                        @TenantID		NUMERIC(18,0), 
						@HSNCODE		NUMERIC(18,0), 
                        @IGSTTaxPer     NUMERIC(18,0) ) 
                        AS  
                        BEGIN 
	                        IF @HSNCODE <> 0  
	                        BEGIN 
		                        SELECT Distinct HSNCODE,HSNID,IGSTTaxPer as IGSTTaxPer,SGSTTaxPer as SGSTTaxPer,CGSTTaxPer as CGSTTaxPer, CessPer 
		                        FROM tblHSNCode I 
		                        WHERE HSNID=@HSNCODE AND I.TenantID = @TenantID  
	                        END  
	                        ELSE  
		                        BEGIN 
		                        IF @HSNCODE <> 0  
		                        BEGIN 
			                        SELECT Distinct HSNCODE,HSNID,IGSTTaxPer,CessPer,CGSTTaxPer,SGSTTaxPer 
			                        FROM tblHSNCode I 
			                        WHERE I.TenantID = @TenantID AND HSNID=@HSNCODE 
		                        END  
		                        ELSE  
		                        BEGIN 
			                        SELECT Distinct HSNCODE as [HSN Code],HSNID,IGSTTaxPer as [IGST %],CessPer as [Cess %] 
			                        FROM tblHSNCode I 
			                        WHERE I.TenantID = @TenantID 
		                        END  
	                        END  
                        END  ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[UspGetHSN] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspGetHSN](
	                        @HID		NUMERIC   (18,0),
	                        @TenantID		NUMERIC   (18,0))
                        AS
                        BEGIN
                             IF @HID <> 0 
                             BEGIN
                                 SELECT * FROM tblHSNCode
                                 WHERE HID = @HID AND TenantID = @TenantID
		                         ORDER BY HSNCODE ASC
                             END
                             ELSE
                             BEGIN
		                        SELECT *
		                        FROM tblColor WHERE TenantID = @TenantID
		                        ORDER BY ColorName ASC
		
                             END
                        END";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[UspLedgerInsert] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspLedgerInsert](
                             @LID    DECIMAL,
                             @LName    VARCHAR  (50),
                             @LAliasName    VARCHAR  (50),
                             @GroupName    VARCHAR  (50),
                             @Type    VARCHAR  (50),
                             @OpBalance    FLOAT,
                             @AppearIn    VARCHAR  (50)=null,
                             @Address    VARCHAR  (500),
                             @CreditDays    VARCHAR  (50)=null,
                             @Phone    VARCHAR  (50)=null,
                             @TaxNo    VARCHAR  (50),
                             @AccountGroupID    DECIMAL,
                             @RouteID    DECIMAL,
                             @Area    VARCHAR  (50)=null,
                             @Notes    VARCHAR  (2000)=null,
                             @TargetAmt    FLOAT,
                             @SMSSchID    NUMERIC  (18,0),
                             @Email    VARCHAR  (100),
                             @MobileNo    VARCHAR  (100),
                             @DiscPer    FLOAT,
                             @InterestPer    FLOAT,
                             @DummyLName    VARCHAR  (500)=NULL,
                             @BlnBank    NUMERIC  (18,0),
                             @CurrencyID    NUMERIC  (18,0),
                             @AreaID    NUMERIC  (18,0),
                             @PLID    NUMERIC  (18,0),
                             @ActiveStatus    NUMERIC  (18,0),
                             @EmailAddress    VARCHAR  (100)=NULL,
                             @EntryDate    DATETIME,
                             @blnBillWise    NUMERIC  (18,0),
                             @CustomerCardID    NUMERIC  (18,0),
                             @TDSPer    FLOAT,
                             @CurrentBalance    FLOAT,
                             @StateID    NUMERIC  (18,0),
                             @CCIDS    VARCHAR  (100)=NULL,
                             @DOB    DATETIME,
                             --@LedgerName    VARCHAR  (50),
                             --@LedgerCode    VARCHAR  (50),
                             @BlnWallet    NUMERIC  (18,0),
                             @blnCoupon    NUMERIC  (18,0),
                             @TransComn    NUMERIC  (18,0),
                             @BlnSmsWelcome    NUMERIC  (18,0),
                             @DLNO    VARCHAR  (50)=NULL,
                             @TDS    FLOAT,
                             @LedgerNameUnicode    NVARCHAR  (50)=NULL,
                             @LedgerAliasNameUnicode    NVARCHAR  (50)=NULL,
                             @ContactPerson    VARCHAR  (50)=NULL,
                             @TaxParameter    VARCHAR  (50)=NULL,
                             @TaxParameterType    VARCHAR  (50)=NULL,
                             @HSNCODE    VARCHAR  (50)=NULL,
                             @CGSTTaxPer    FLOAT,
                             @SGSTTaxPer    FLOAT,
                             @IGSTTaxPer    FLOAT,
                             --@HSNID    NUMERIC  (18,0)=1,
                             @BankAccountNo    NUMERIC  (18,0),
                             @BankIFSCCode    VARCHAR  (50)=NULL,
                             @BankNote    VARCHAR  (100)=NULL,
                             @WhatsAppNo    NUMERIC  (18,0),
                             @SystemName    VARCHAR  (50),
                             @UserID    NUMERIC  (18,0),
                             @LastUpdateDate    DATETIME,
                             @LastUpdateTime    DATETIME,
                             @TenantID   NUMERIC  (18,0),
                             @Action             INT=0,
	                         @GSTType VARCHAR  (50),
	                         @AgentID NUMERIC  (18,0)
                        )
                        AS
                        BEGIN
                        DECLARE @RetResult      INT
                        DECLARE @TransType		CHAR(1)
                        BEGIN TRY
                        BEGIN TRANSACTION;
                        IF @Action = 0
                        BEGIN
                             --INSERT INTO tblLedger(LID,LName,LAliasName,GroupName,Type,OpBalance,AppearIn,Address,CreditDays,Phone,TaxNo,AccountGroupID,RouteID,Area,Notes,TargetAmt,SMSSchID,Email,MobileNo,DiscPer,InterestPer,DummyLName,BlnBank,CurrencyID,AreaID,PLID,ActiveStatus,EmailAddress,EntryDate,blnBillWise,CustomerCardID,TDSPer,DOB,StateID,CCIDS,CurrentBalance,LedgerName,LedgerCode,BlnWallet,blnCoupon,TransComn,BlnSmsWelcome,DLNO,TDS,LedgerNameUnicode,LedgerAliasNameUnicode,ContactPerson,TaxParameter,TaxParameterType,HSNCODE,CGSTTaxPer,SGSTTaxPer,IGSTTaxPer,HSNID,BankAccountNo,BankIFSCCode,BankNote,WhatsAppNo,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID)
                             --VALUES(@LID,@LName,@LAliasName,@GroupName,@Type,@OpBalance,@AppearIn,@Address,@CreditDays,@Phone,@TaxNo,@AccountGroupID,@RouteID,@Area,@Notes,@TargetAmt,@SMSSchID,@Email,@MobileNo,@DiscPer,@InterestPer,@DummyLName,@BlnBank,@CurrencyID,@AreaID,@PLID,@ActiveStatus,@EmailAddress,@EntryDate,@blnBillWise,@CustomerCardID,@TDSPer,@DOB,@StateID,@CCIDS,@CurrentBalance,@LedgerName,@LedgerCode,@BlnWallet,@blnCoupon,@TransComn,@BlnSmsWelcome,@DLNO,@TDS,@LedgerNameUnicode,@LedgerAliasNameUnicode,@ContactPerson,@TaxParameter,@TaxParameterType,@HSNCODE,@CGSTTaxPer,@SGSTTaxPer,@IGSTTaxPer,@HSNID,@BankAccountNo,@BankIFSCCode,@BankNote,@WhatsAppNo,@SystemName,@UserID,@LastUpdateDate,@LastUpdateTime,@TenantID)
    
                            INSERT INTO tblLedger(LID,LName,LAliasName,GroupName,Type,OpBalance,AppearIn,Address,CreditDays,Phone,TaxNo,AccountGroupID,RouteID,Area,Notes,TargetAmt,SMSSchID,Email,MobileNo,DiscPer,InterestPer,DummyLName,BlnBank,CurrencyID,AreaID,PLID,ActiveStatus,EmailAddress,EntryDate,blnBillWise,CustomerCardID,TDSPer,DOB,StateID,CCIDS,CurrentBalance,BlnWallet,blnCoupon,TransComn,BlnSmsWelcome,DLNO,TDS,LedgerNameUnicode,LedgerAliasNameUnicode,ContactPerson,TaxParameter,TaxParameterType,HSNCODE,CGSTTaxPer,SGSTTaxPer,IGSTTaxPer,BankAccountNo,BankIFSCCode,BankNote,WhatsAppNo,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID,GSTType,AgentID)
                             VALUES(@LID,@LName,@LAliasName,@GroupName,@Type,@OpBalance,@AppearIn,@Address,@CreditDays,@Phone,@TaxNo,@AccountGroupID,@RouteID,@Area,@Notes,@TargetAmt,@SMSSchID,@Email,@MobileNo,@DiscPer,@InterestPer,@DummyLName,@BlnBank,@CurrencyID,@AreaID,@PLID,@ActiveStatus,@EmailAddress,@EntryDate,@blnBillWise,@CustomerCardID,@TDSPer,@DOB,@StateID,@CCIDS,@CurrentBalance,@BlnWallet,@blnCoupon,@TransComn,@BlnSmsWelcome,@DLNO,@TDS,@LedgerNameUnicode,@LedgerAliasNameUnicode,@ContactPerson,@TaxParameter,@TaxParameterType,@HSNCODE,@CGSTTaxPer,@SGSTTaxPer,@IGSTTaxPer,@BankAccountNo,@BankIFSCCode,@BankNote,@WhatsAppNo,@SystemName,@UserID,@LastUpdateDate,@LastUpdateTime,@TenantID,@GSTType,@AgentID)
 
	
	                         SET @RetResult = 1;
	                          SET @TransType = 'S';
                        END
                        IF @Action = 1
                        BEGIN
	                        IF @LID > 1000
                             BEGIN
		                         --UPDATE tblLedger SET LName=@LName,LAliasName=@LAliasName,GroupName=@GroupName,Type=@Type,OpBalance=@OpBalance,AppearIn=@AppearIn,Address=@Address,CreditDays=@CreditDays,Phone=@Phone,TaxNo=@TaxNo,AccountGroupID=@AccountGroupID,RouteID=@RouteID,Area=@Area,Notes=@Notes,TargetAmt=@TargetAmt,SMSSchID=@SMSSchID,Email=@Email,MobileNo=@MobileNo,DiscPer=@DiscPer,InterestPer=@InterestPer,DummyLName=@DummyLName,BlnBank=@BlnBank,CurrencyID=@CurrencyID,AreaID=@AreaID,PLID=@PLID,ActiveStatus=@ActiveStatus,EmailAddress=@EmailAddress,EntryDate=@EntryDate,blnBillWise=@blnBillWise,CustomerCardID=@CustomerCardID,TDSPer=@TDSPer,DOB=@DOB,StateID=@StateID,CCIDS=@CCIDS,CurrentBalance=@CurrentBalance,LedgerName=@LedgerName,LedgerCode=@LedgerCode,BlnWallet=@BlnWallet,blnCoupon=@blnCoupon,TransComn=@TransComn,BlnSmsWelcome=@BlnSmsWelcome,DLNO=@DLNO,TDS=@TDS,LedgerNameUnicode=@LedgerNameUnicode,LedgerAliasNameUnicode=@LedgerAliasNameUnicode,ContactPerson=@ContactPerson,TaxParameter=@TaxParameter,TaxParameterType=@TaxParameterType,HSNCODE=@HSNCODE,CGSTTaxPer=@CGSTTaxPer,SGSTTaxPer=@SGSTTaxPer,IGSTTaxPer=@IGSTTaxPer,HSNID=@HSNID,BankAccountNo=@BankAccountNo,BankIFSCCode=@BankIFSCCode,BankNote=@BankNote,WhatsAppNo=@WhatsAppNo,SystemName=@SystemName,UserID=@UserID,LastUpdateDate=@LastUpdateDate,LastUpdateTime=@LastUpdateTime,TenantID=@TenantID
		                         UPDATE tblLedger SET LName=@LName,LAliasName=@LAliasName,GroupName=@GroupName,Type=@Type,OpBalance=@OpBalance,AppearIn=@AppearIn,Address=@Address,CreditDays=@CreditDays,Phone=@Phone,TaxNo=@TaxNo,AccountGroupID=@AccountGroupID,RouteID=@RouteID,Area=@Area,Notes=@Notes,TargetAmt=@TargetAmt,SMSSchID=@SMSSchID,Email=@Email,MobileNo=@MobileNo,DiscPer=@DiscPer,InterestPer=@InterestPer,DummyLName=@DummyLName,BlnBank=@BlnBank,CurrencyID=@CurrencyID,AreaID=@AreaID,PLID=@PLID,ActiveStatus=@ActiveStatus,EmailAddress=@EmailAddress,EntryDate=@EntryDate,blnBillWise=@blnBillWise,CustomerCardID=@CustomerCardID,TDSPer=@TDSPer,DOB=@DOB,StateID=@StateID,CCIDS=@CCIDS,CurrentBalance=@CurrentBalance,BlnWallet=@BlnWallet,blnCoupon=@blnCoupon,TransComn=@TransComn,BlnSmsWelcome=@BlnSmsWelcome,DLNO=@DLNO,TDS=@TDS,LedgerNameUnicode=@LedgerNameUnicode,LedgerAliasNameUnicode=@LedgerAliasNameUnicode,ContactPerson=@ContactPerson,TaxParameter=@TaxParameter,TaxParameterType=@TaxParameterType,HSNCODE=@HSNCODE,CGSTTaxPer=@CGSTTaxPer,SGSTTaxPer=@SGSTTaxPer,IGSTTaxPer=@IGSTTaxPer,BankAccountNo=@BankAccountNo,BankIFSCCode=@BankIFSCCode,BankNote=@BankNote,WhatsAppNo=@WhatsAppNo,SystemName=@SystemName,UserID=@UserID,LastUpdateDate=@LastUpdateDate,LastUpdateTime=@LastUpdateTime,TenantID=@TenantID,GSTType=@GSTType,AgentID=@AgentID
		                         WHERE LID=@LID
		                         SET @RetResult = 1;
		                         SET @TransType = 'E';
	                         END
	                         ELSE
	                         BEGIN
		                         UPDATE tblLedger SET Type=@Type,OpBalance=@OpBalance 
		                         WHERE LID=@LID

		                         SET @RetResult = -1;
		                         SET @TransType = 'F';
	                         END
                        END
                        IF @Action = 2
                        BEGIN
	                        IF @LID > 1000
                             BEGIN
		                         DELETE FROM tblLedger WHERE LID=@LID

		                         SET @RetResult = 0;
		                         SET @TransType = 'D';
	                         END
	                         ELSE
	                         BEGIN
		                         SET @RetResult = -1;
		                         SET @TransType = 'F';
	                         END
                        END
                        COMMIT TRANSACTION;

                        IF @RetResult <> -1
	                        SELECT @RetResult as SqlSpResult,@LID as TransID,@TransType as TransactType
                        ELSE
	                        SELECT -1 as SqlSpResult, -1 AS ErrorNumber, 'TRANSACTION FAILED' AS ErrorState, 'CRITICAL' AS ErrorSeverity,
		                        'UspLedgerInsert' AS ErrorProcedure, -1 AS ErrorLine, 'DEFAULT LEDGERS CANNOT BE EDITED OR DELETED' AS ErrorMessage;

                        END TRY
                        BEGIN CATCH
                        ROLLBACK;
                        SELECT
                        - 1 as SqlSpResult,
                        ERROR_NUMBER() AS ErrorNumber,
                        ERROR_STATE() AS ErrorState,
                        ERROR_SEVERITY() AS ErrorSeverity,
                        ERROR_PROCEDURE() AS ErrorProcedure,
                        ERROR_LINE() AS ErrorLine,
                        ERROR_MESSAGE() AS ErrorMessage;
                        END CATCH;
                        END

                        ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[UspSalesInsert] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspSalesInsert] (@InvId                        NUMERIC(18, 0),
                                                                    @InvNo                        VARCHAR(100),
                                                                    @AutoNum                      NUMERIC(18, 0),
                                                                    @Prefix                       VARCHAR(50),
                                                                    @InvDate                        DATETIME,
                                                                    @VchType                      VARCHAR(100),
                                                                    @MOP                          VARCHAR(100),
                                                                    @TaxModeID                    NUMERIC(18, 0),
                                                                    @LedgerId                     NUMERIC(18, 0),
                                                                    @Party                        VARCHAR(100),
                                                                    @Discount                     FLOAT,
										                            @dSteadyBillDiscPerc			 FLOAT,
										                            @dSteadyBillDiscAmt			 FLOAT,
                                                                    @TaxAmt                       FLOAT,
                                                                    @GrossAmt                     FLOAT,
                                                                    @BillAmt                      FLOAT,
                                                                    @Cancelled                    NUMERIC(18, 0),
                                                                    @OtherExpense                 FLOAT,
                                                                    @Coolie                       FLOAT,
                                                                    @SalesManID                   NUMERIC(18, 0),
                                                                    @Taxable                      FLOAT,
                                                                    @NonTaxable                   FLOAT,
                                                                    @ItemDiscountTotal            FLOAT,
                                                                    @RoundOff                     FLOAT,
                                                                    @UserNarration                VARCHAR(500),
                                                                    @SortNumber                   NUMERIC(18, 0),
                                                                    @DiscPer                      FLOAT,
                                                                    @VchTypeID                    NUMERIC(18, 0),
                                                                    @CCID                         NUMERIC(18, 0),
                                                                    @CurrencyID                   NUMERIC(18, 0),
                                                                    @PartyAddress                 VARCHAR(500),
                                                                    @UserID                       INT,
                                                                    @AgentID                      NUMERIC(18, 0),
                                                                    @CashDiscount                 FLOAT,
                                                                    @DPerType_ManualCalc_Customer NUMERIC(18, 0),
                                                                    @NetAmount                    FLOAT,
                                                                    @RefNo                        VARCHAR(100),
                                                                    @CashPaid                     NUMERIC(18, 0),
                                                                    @CardPaid                     NUMERIC(18, 0),
                                                                    @blnWaitforAuthorisation      NUMERIC(18, 0),
                                                                    @UserIDAuth                   NUMERIC(18, 0),
                                                                    @BillTime                     DATETIME,
                                                                    @StateID                      NUMERIC(18, 0),
                                                                    @ImplementingStateCode        VARCHAR(50),
                                                                    @GSTType                      VARCHAR(50),
                                                                    @CGSTTotal                    FLOAT,
                                                                    @SGSTTotal                    FLOAT,
                                                                    @IGSTTotal                    FLOAT,
                                                                    @DelNoteNo                   VARCHAR(200),
                                                                    @DelNoteDate                   DATETIME,
                                                                    @DelNoteRefNo                   VARCHAR(200),
                                                                    @DelNoteRefDate                   DATETIME,
                                                                    @OtherRef                   VARCHAR(200),
                                                                    @BuyerOrderNo                   VARCHAR(200),
                                                                    @BuyerOrderDate                   DATETIME,
                                                                    @DispatchDocNo                   VARCHAR(200),
                                                                    @LRRRNo                   VARCHAR(200),
                                                                    @MotorVehicleNo                   VARCHAR(200),
                                                                    @PartyGSTIN                   VARCHAR(50),
                                                                    @BillType                     VARCHAR(50),
                                                                    @blnHold                      NUMERIC(18, 0),
                                                                    @PriceListID                  NUMERIC(18, 0),
                                                                    @EffectiveDate                        DATETIME,
                                                                    @partyCode                    VARCHAR(150),
                                                                    @MobileNo                     VARCHAR(20),
                                                                    @Email                        VARCHAR(100),
                                                                    @TaxType                      VARCHAR(50),
                                                                    @QtyTotal                     FLOAT,
                                                                    @DestCCID                     NUMERIC(18, 0),
                                                                    @AgentCommMode                VARCHAR(50),
                                                                    @AgentCommAmount              FLOAT,
                                                                    @AgentLID                     NUMERIC(18, 0),
                                                                    @BlnStockInsert               NUMERIC(18, 0),
                                                                    @BlnConverted                 NUMERIC(18, 0),
                                                                    @ConvertedParentVchTypeID     NUMERIC(18, 0),
                                                                    @ConvertedVchTypeID           NUMERIC(18, 0),
                                                                    @ConvertedVchNo               VARCHAR(50),
                                                                    @ConvertedVchID               NUMERIC(18, 0),
                                                                    @DeliveryNoteDetails          VARCHAR(500),
                                                                    @OrderDetails                 VARCHAR(500),
                                                                    @IntegrityStatus              VARCHAR(50),
                                                                    @BalQty                       FLOAT,
                                                                    @CustomerpointsSettled        FLOAT,
                                                                    @blnCashPaid                  NUMERIC(18, 0),
                                                                    @originalsalesinvid           NUMERIC(18, 0),
                                                                    @retuninvid                   NUMERIC(18, 0),
                                                                    @returnamount                 FLOAT,
                                                                    @SystemName                   VARCHAR(50),
                                                                    @LastUpdateDate                        DATETIME,
                                                                    @LastUpdateTime                        DATETIME,
                                                                    @DeliveryDetails              VARCHAR(max),
                                                                    @DespatchDetails              VARCHAR(max),
                                                                    @TermsOfDelivery              VARCHAR(max),
                                                                    @FloodCessTot                 FLOAT,
                                                                    @CounterID                    NUMERIC(18, 0),
                                                                    @ExtraCharges                 FLOAT,
                                                                    @ReferenceAutoNO              VARCHAR(50),
                                                                    @CashDiscPer                  FLOAT,
                                                                    @CostFactor                   NUMERIC(18, 0),
                                                                    @TenantID                     NUMERIC(18, 0),
                                                                    @JsonData                     VARCHAR(max),
                                                                    @Action                       INT=0)
                        AS
                            BEGIN
                                DECLARE @RetResult INT

                                BEGIN try
                                    --BEGIN TRANSACTION;

                                    IF @Action = 0
                                    BEGIN
                                        INSERT INTO tblSales
                                                    (invid,
                                                        invno,
                                                        autonum,
                                                        prefix,
                                                        invdate,
                                                        vchtype,
                                                        mop,
                                                        taxmodeid,
                                                        ledgerid,
                                                        party,
                                                        discount,
							                            dSteadyBillDiscPerc,
							                            dSteadyBillDiscAmt,
                                                        taxamt,
                                                        grossamt,
                                                        billamt,
                                                        cancelled,
                                                        otherexpense,
                                                        Coolie,
                                                        salesmanid,
                                                        taxable,
                                                        nontaxable,
                                                        itemdiscounttotal,
                                                        roundoff,
                                                        usernarration,
                                                        sortnumber,
                                                        discper,
                                                        vchtypeid,
                                                        ccid,
                                                        currencyid,
                                                        partyaddress,
                                                        userid,
                                                        agentid,
                                                        cashdiscount,
                                                        dpertype_manualcalc_customer,
                                                        netamount,
                                                        refno,
                                                        cashpaid,
                                                        cardpaid,
                                                        blnwaitforauthorisation,
                                                        useridauth,
                                                        billtime,
                                                        stateid,
                                                        implementingstatecode,
                                                        gsttype,
                                                        cgsttotal,
                                                        sgsttotal,
                                                        igsttotal,
							                        DelNoteNo,
							                        DelNoteDate,
							                        DelNoteRefNo,
							                        DelNoteRefDate,
							                        OtherRef,
							                        BuyerOrderNo,
							                        BuyerOrderDate,
							                        DispatchDocNo,
							                        LRRRNo,
							                        MotorVehicleNo,
                                                        partygstin,
                                                        billtype,
                                                        blnhold,
                                                        pricelistid,
                                                        effectivedate,
                                                        partycode,
                                                        mobileno,
                                                        email,
                                                        taxtype,
                                                        qtytotal,
                                                        destccid,
                                                        agentcommmode,
                                                        agentcommamount,
                                                        agentlid,
                                                        blnstockinsert,
                                                        blnconverted,
                                                        convertedparentvchtypeid,
                                                        convertedvchtypeid,
                                                        convertedvchno,
                                                        convertedvchid,
                                                        deliverynotedetails,
                                                        orderdetails,
                                                        integritystatus,
                                                        balqty,
                                                        customerpointssettled,
                                                        blncashpaid,
                                                        originalsalesinvid,
                                                        retuninvid,
                                                        returnamount,
                                                        systemname,
                                                        lastupdatedate,
                                                        lastupdatetime,
                                                        deliverydetails,
                                                        despatchdetails,
                                                        termsofdelivery,
                                                        floodcesstot,
                                                        counterid,
                                                        extracharges,
                                                        referenceautono,
                                                        cashdisper,
                                                        costfactor,
                                                        tenantid,
                                                        jsondata)
                                        VALUES     (@InvId,
                                                    @InvNo,
                                                    @AutoNum,
                                                    @Prefix,
                                                    @InvDate,
                                                    @VchType,
                                                    @MOP,
                                                    @TaxModeID,
                                                    @LedgerId,
                                                    @Party,
                                                    @Discount,
							                        @dSteadyBillDiscPerc,
							                        @dSteadyBillDiscAmt,
                                                    @TaxAmt,
                                                    @GrossAmt,
                                                    @BillAmt,
                                                    @Cancelled,
                                                    @OtherExpense,
                                                    @Coolie,
                                                    @SalesManID,
                                                    @Taxable,
                                                    @NonTaxable,
                                                    @ItemDiscountTotal,
                                                    @RoundOff,
                                                    @UserNarration,
                                                    @SortNumber,
                                                    @DiscPer,
                                                    @VchTypeID,
                                                    @CCID,
                                                    @CurrencyID,
                                                    @PartyAddress,
                                                    @UserID,
                                                    @AgentID,
                                                    @CashDiscount,
                                                    @DPerType_ManualCalc_Customer,
                                                    @NetAmount,
                                                    @RefNo,
                                                    @CashPaid,
                                                    @CardPaid,
                                                    @blnWaitforAuthorisation,
                                                    @UserIDAuth,
                                                    @BillTime,
                                                    @StateID,
                                                    @ImplementingStateCode,
                                                    @GSTType,
                                                    @CGSTTotal,
                                                    @SGSTTotal,
                                                    @IGSTTotal,
							                        @DelNoteNo,
							                        @DelNoteDate,
							                        @DelNoteRefNo,
							                        @DelNoteRefDate,
							                        @OtherRef,
							                        @BuyerOrderNo,
							                        @BuyerOrderDate,
							                        @DispatchDocNo,
							                        @LRRRNo,
							                        @MotorVehicleNo,
                                                    @PartyGSTIN,
                                                    @BillType,
                                                    @blnHold,
                                                    @PriceListID,
                                                    @EffectiveDate,
                                                    @partyCode,
                                                    @MobileNo,
                                                    @Email,
                                                    @TaxType,
                                                    @QtyTotal,
                                                    @DestCCID,
                                                    @AgentCommMode,
                                                    @AgentCommAmount,
                                                    @AgentLID,
                                                    @BlnStockInsert,
                                                    @BlnConverted,
                                                    @ConvertedParentVchTypeID,
                                                    @ConvertedVchTypeID,
                                                    @ConvertedVchNo,
                                                    @ConvertedVchID,
                                                    @DeliveryNoteDetails,
                                                    @OrderDetails,
                                                    @IntegrityStatus,
                                                    @BalQty,
                                                    @CustomerpointsSettled,
                                                    @blnCashPaid,
                                                    @originalsalesinvid,
                                                    @retuninvid,
                                                    @returnamount,
                                                    @SystemName,
                                                    @LastUpdateDate,
                                                    @LastUpdateTime,
                                                    @DeliveryDetails,
                                                    @DespatchDetails,
                                                    @TermsOfDelivery,
                                                    @FloodCessTot,
                                                    @CounterID,
                                                    @ExtraCharges,
                                                    @ReferenceAutoNO,
                                                    @CashDiscPer,
                                                    @CostFactor,
                                                    @TenantID,
                                                    @JsonData)

                                        SET @RetResult = 1;
                                    END

                                    IF @Action = 1
                                    BEGIN
                                        UPDATE tblSales
                                        SET    invno = @InvNo,
                                                autonum = @AutoNum,
                                                prefix = @Prefix,
                                                invdate = @InvDate,
                                                vchtype = @VchType,
                                                mop = @MOP,
                                                taxmodeid = @TaxModeID,
                                                ledgerid = @LedgerId,
                                                party = @Party,
                                                discount = @Discount,
					                            dSteadyBillDiscPerc = @dSteadyBillDiscPerc,
					                            dSteadyBillDiscAmt = @dSteadyBillDiscAmt,
                                                taxamt = @TaxAmt,
                                                grossamt = @GrossAmt,
                                                billamt = @BillAmt,
                                                cancelled = @Cancelled,
                                                otherexpense = @OtherExpense,
                                                Coolie = @Coolie,
                                                salesmanid = @SalesManID,
                                                taxable = @Taxable,
                                                nontaxable = @NonTaxable,
                                                itemdiscounttotal = @ItemDiscountTotal,
                                                roundoff = @RoundOff,
                                                usernarration = @UserNarration,
                                                sortnumber = @SortNumber,
                                                discper = @DiscPer,
                                                vchtypeid = @VchTypeID,
                                                ccid = @CCID,
                                                currencyid = @CurrencyID,
                                                partyaddress = @PartyAddress,
                                                userid = @UserID,
                                                agentid = @AgentID,
                                                cashdiscount = @CashDiscount,
                                                dpertype_manualcalc_customer =
                                                @DPerType_ManualCalc_Customer,
                                                netamount = @NetAmount,
                                                refno = @RefNo,
                                                cashpaid = @CashPaid,
                                                cardpaid = @CardPaid,
                                                blnwaitforauthorisation = @blnWaitforAuthorisation,
                                                useridauth = @UserIDAuth,
                                                billtime = @BillTime,
                                                stateid = @StateID,
                                                implementingstatecode = @ImplementingStateCode,
                                                gsttype = @GSTType,
                                                cgsttotal = @CGSTTotal,
                                                sgsttotal = @SGSTTotal,
                                                igsttotal = @IGSTTotal,
						                        DelNoteNo = @DelNoteNo,
						                        DelNoteDate = @DelNoteDate,
						                        DelNoteRefNo = @DelNoteRefNo,
						                        DelNoteRefDate = @DelNoteRefDate,
						                        OtherRef = @OtherRef,
						                        BuyerOrderNo = @BuyerOrderNo,
						                        BuyerOrderDate = @BuyerOrderDate,
						                        DispatchDocNo = @DispatchDocNo,
						                        LRRRNo = @LRRRNo,
						                        MotorVehicleNo = @MotorVehicleNo,
                                                partygstin = @PartyGSTIN,
                                                billtype = @BillType,
                                                blnhold = @blnHold,
                                                pricelistid = @PriceListID,
                                                effectivedate = @EffectiveDate,
                                                partycode = @partyCode,
                                                mobileno = @MobileNo,
                                                email = @Email,
                                                taxtype = @TaxType,
                                                qtytotal = @QtyTotal,
                                                destccid = @DestCCID,
                                                agentcommmode = @AgentCommMode,
                                                agentcommamount = @AgentCommAmount,
                                                agentlid = @AgentLID,
                                                blnstockinsert = @BlnStockInsert,
                                                blnconverted = @BlnConverted,
                                                convertedparentvchtypeid = @ConvertedParentVchTypeID,
                                                convertedvchtypeid = @ConvertedVchTypeID,
                                                convertedvchno = @ConvertedVchNo,
                                                convertedvchid = @ConvertedVchID,
                                                deliverynotedetails = @DeliveryNoteDetails,
                                                orderdetails = @OrderDetails,
                                                integritystatus = @IntegrityStatus,
                                                balqty = @BalQty,
                                                customerpointssettled = @CustomerpointsSettled,
                                                blncashpaid = @blnCashPaid,
                                                originalsalesinvid = @originalsalesinvid,
                                                retuninvid = @retuninvid,
                                                returnamount = @returnamount,
                                                systemname = @SystemName,
                                                lastupdatedate = @LastUpdateDate,
                                                lastupdatetime = @LastUpdateTime,
                                                deliverydetails = @DeliveryDetails,
                                                despatchdetails = @DespatchDetails,
                                                termsofdelivery = @TermsOfDelivery,
                                                floodcesstot = @FloodCessTot,
                                                counterid = @CounterID,
                                                extracharges = @ExtraCharges,
                                                referenceautono = @ReferenceAutoNO,
                                                cashdisper = @CashDiscPer,
                                                costfactor = @CostFactor,
                                                jsondata = @JsonData
                                        WHERE  invid = @InvId
                                                AND tenantid = @TenantID

                                        SET @RetResult = 1;
                                    END

                                    IF @Action = 2
                                    BEGIN
                                        DELETE FROM tblSales
                                        WHERE  invid = @InvId
                                                AND tenantid = @TenantID

                                        SET @RetResult = 0;
                                    END

                                    IF @Action = 3
                                    BEGIN
                                        UPDATE tblSales
                                        SET    cancelled = 1
                                        WHERE  invid = @InvId
                                                AND tenantid = @TenantID

                                        SET @RetResult = 3;
                                    END

                                    --COMMIT TRANSACTION;

                                    SELECT @RetResult AS SqlSpResult
                                END try

                                BEGIN catch
                                    --ROLLBACK;

                                    SELECT -1                AS SqlSpResult,
                                            Error_number()    AS ErrorNumber,
                                            Error_state()     AS ErrorState,
                                            Error_severity()  AS ErrorSeverity,
                                            Error_procedure() AS ErrorProcedure,
                                            Error_line()      AS ErrorLine,
                                            Error_message()   AS ErrorMessage;
                                END catch;
                            END ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[Createtaxledger] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[Createtaxledger] @vatPer        FLOAT,
                                        @TaxType       VARCHAR (50),
                                        @TaxMode       VARCHAR(50),
                                        @TaxParameter  VARCHAR(50),
                                        @intAccGroupId INT
                                        AS
                                          BEGIN
                                              DECLARE @Maxid AS INT
                                              DECLARE @StrType AS VARCHAR (50)

                                              IF @intAccGroupId = 0
                                                BEGIN
                                                    SET @intAccGroupId = 15
                                                END

                                              IF @TaxParameter = 'INPUTTAX'
                                                BEGIN
                                                    SET @StrType = 'Debit (Customers, Assets)'
                                                END
                                              ELSE
                                                BEGIN
                                                    SET @StrType = 'Credit (Suppliers, Liabilities)'
                                                END

                                              SELECT lname
                                              FROM   tblledger
                                              WHERE  Upper(Replace(lname, ' ', '')) = Upper(
                                                     Replace(( Cast((@vatPer) AS
                                                               VARCHAR(12))
                                                               + @TaxType ), ' ', ''))

                                              IF @@ROWCOUNT = 0
                                                BEGIN
                                                    SELECT @maxId = Max(lid) + 1
                                                    FROM   tblledger

                                                    INSERT INTO tblledger(lid,lname,laliasname,creditdays,targetamt,emailaddress,phone,mobileno,opbalance,discper,fax,[address],blnbank,[type],activestatus,dob,blnwallet,blncoupon,transcomn,blnsmswelcome,dlno,tds,areaid,plid,groupname,accountgroupid,blnbillwise,ledgernameunicode,ledgeraliasnameunicode,contactperson,stateid,hsncode,cgsttaxper,sgsttaxper,igsttaxper,taxparameter,taxparametertype) VALUES      
                                                    (@Maxid,Cast((@vatPer) AS VARCHAR(12)) + ' '+ @TaxType,Cast((@vatPer) AS VARCHAR(12)) + ' '+ @TaxType,0,0,'','','',0,0,'','',0,@Strtype,1,Getdate(),0,0,0,0,'','0',1,1,'LEDGER',@intAccGroupId,0,N'',N'','',40,'',0,0,@vatPer,@TaxParameter,@TaxMode )
                                                END
                                          END ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[spTaxLedgerInsert] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[spTaxLedgerInsert]
		                    @vatPer decimal,
		                    @KFCESS decimal,
		                    @CGSTper decimal,
		                    @SGSTPer decimal,
		                    @IGSTPer decimal,
		                    @CessPer decimal,
		                    @CompCessPer decimal
                            AS
                              BEGIN
                                  DECLARE @TaxHalf AS FLOAT

                                  IF @vatPer <> 0
                                    BEGIN
                                        SET @taxHalf = @vatPer / 2

                                        EXEC Createtaxledger
                                          @VATPER,
                                          'SALES TAX',
                                          'SALES',
                                          'TAX',
                                          15

                                        EXEC Createtaxledger
                                          @VATPER,
                                          'SALES TAXABLE',
                                          'SALES',
                                          'TAXABLE',
                                          13

                                        EXEC Createtaxledger
                                          @VATPER,
                                          'PURCHASE TAX',
                                          'PURCHASE',
                                          'TAX',
                                          15

                                        EXEC Createtaxledger
                                          @VATPER,
                                          'PURCHASE TAXABLE',
                                          'PURCHASE',
                                          'TAXABLE',
                                          12

                                        EXEC Createtaxledger
                                          @VATPER,
                                          'SALES RETURN TAX',
                                          'SALESRETURN',
                                          'TAX',
                                          15

                                        EXEC Createtaxledger
                                          @VATPER,
                                          'SALES RETURN TAXABLE',
                                          'SALESRETURN',
                                          'TAXABLE',
                                          27

                                        EXEC Createtaxledger
                                          @VATPER,
                                          'PURCHASE RETURN TAX',
                                          'PURCHASERETURN',
                                          'TAX',
                                          15

                                        EXEC Createtaxledger
                                          @VATPER,
                                          'PURCHASE RETURN TAXABLE',
                                          'PURCHASERETURN',
                                          'TAXABLE',
                                          28
                                    END

                                  IF ( @CGSTPer <> 0
                                       AND @sGSTPer <> 0 )
                                      OR ( @iGSTPer <> 0 )
                                    BEGIN
                                        EXEC Createtaxledger
                                          @taxHalf,
                                          'SALES TAX CGST',
                                          'SALES',
                                          'TAXCGST',
                                          15

                                        EXEC Createtaxledger
                                          @taxHalf,
                                          'SALES TAX SGST',
                                          'SALES',
                                          'TAXSGST',
                                          15

                                        EXEC Createtaxledger
                                          @vatPer,
                                          'SALES TAX IGST',
                                          'SALES',
                                          'TAXIGST',
                                          15

                                        EXEC Createtaxledger
                                          @vatPer,
                                          'SALES TAXABLE',
                                          'SALES',
                                          'TAXABLE',
                                          13

                                        EXEC Createtaxledger
                                          @taxHalf,
                                          'PURCHASE TAX CGST',
                                          'PURCHASE',
                                          'TAXCGST',
                                          15

                                        EXEC Createtaxledger
                                          @taxHalf,
                                          'PURCHASE TAX SGST',
                                          'PURCHASE',
                                          'TAXSGST',
                                          15

                                        EXEC Createtaxledger
                                          @vatPer,
                                          'PURCHASE TAX IGST',
                                          'PURCHASE',
                                          'TAXIGST',
                                          15

                                        EXEC Createtaxledger
                                          @vatPer,
                                          'PURCHASE TAXABLE ',
                                          'PURCHASE',
                                          'TAXABLE',
                                          12

                                        EXEC Createtaxledger
                                          @taxHalf,
                                          'SALES RETURN TAX CGST',
                                          'SALESRETURN',
                                          'TAXCGST',
                                          15

                                        EXEC Createtaxledger
                                          @taxHalf,
                                          'SALES RETURN TAX SGST',
                                          'SALESRETURN',
                                          'TAXSGST',
                                          15

                                        EXEC Createtaxledger
                                          @vatPer,
                                          'SALES RETURN TAX IGST',
                                          'SALESRETURN',
                                          'TAXIGST',
                                          15

                                        EXEC Createtaxledger
                                          @vatPer,
                                          'SALES RETURN TAXABLE',
                                          'SALESRETURN',
                                          'TAXABLE',
                                          27

                                        EXEC Createtaxledger
                                          @taxHalf,
                                          'PURCHASE RETURN TAX CGST',
                                          'PURCHASERETURN',
                                          'TAXCGST',
                                          15

                                        EXEC Createtaxledger
                                          @taxHalf,
                                          'PURCHASE RETURN TAX SGST',
                                          'PURCHASERETURN',
                                          'TAXSGST',
                                          15

                                        EXEC Createtaxledger 
                                          @vatPer,
                                          'PURCHASE RETURN TAX IGST',
                                          'PURCHASERETURN',
                                          'TAXIGST',
                                          15

                                        EXEC Createtaxledger
                                          @vatPer,
                                          'PURCHASE RETURN TAXABLE',
                                          'PURCHASERETURN',
                                          'TAXABLE',
                                          28
                                    END

                                  IF @CessPer <> 0
                                    BEGIN
                                        EXEC Createtaxledger
                                          @CessPer,
                                          'SALES CESS',
                                          'SALES',
                                          'TAXCESS',
                                          15

                                        EXEC Createtaxledger
                                          @CessPer,
                                          'PURCHASE CESS',
                                          'PURCHASE',
                                          'TAXCESS',
                                          15
            
                                        EXEC Createtaxledger
                                          @CessPer,
                                          'SALES RETURN CESS',
                                          'SALESRETURN',
                                          'TAXCESS',
                                          15
                                        EXEC Createtaxledger
                                          @CessPer,
                                          'PURCHASE RETURN CESS',
                                          'PURCHASERETURN',
                                          'TAXCESS',
                                          15
                                    END

                                  IF @KFCESS <> 0
                                    BEGIN
                                        EXEC Createtaxledger
                                          @KFCESS,
                                          'SALES FLOOD CESS',
                                          'SALES',
                                          'TAXFLOODCESS',
                                          15

                                        EXEC Createtaxledger
                                          @KFCess,
                                          'PURCHASE FLOOD CESS',
                                          'PURCHASE',
                                          'TAXFLOODCESS',
                                          15
                                        EXEC Createtaxledger
                                          @KFCess,
                                          'SALES RETURN FLOOD CESS',
                                          'SALESRETURN',
                                          'TAXFLOODCESS',
                                          15
                                        EXEC Createtaxledger
                                          @KFCess,
                                          'PURCHASE RETURN FLOOD CESS',
                                          'PURCHASERETURN',
                                          'TAXFLOODCESS',
                                          15
                                    END

                                  IF @CompCessPer <> 0
                                    BEGIN
                                        EXEC Createtaxledger
                                          @CompCessPer,
                                          'SALES COMPCESS',
                                          'SALES',
                                          'TAXCOMPCESS',
                                          15

                                        EXEC Createtaxledger
                                          @CompCessPer,
                                          'PURCHASE COMPCESS',
                                          'PURCHASE',
                                          'TAXCOMPCESS',
                                          15
			
			                            EXEC Createtaxledger
                                          @KFCess,
                                          'SALES RETURN COMPCESS',
                                          'SALESRETURN',
                                          'TAXCOMPCESS',
                                          15
                                        EXEC Createtaxledger
                                          @KFCess,
                                          'PURCHASE RETURN COMPCESS',
                                          'PURCHASERETURN',
                                          'TAXCOMPCESS',
                                          15        
                                   END
                              END ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[UspGetCashDeskMaster] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspGetCashDeskMaster](
	                        @PaymentID	NUMERIC   (18,0),
	                        @Paymentids		VARCHAR(Max) = '' )
                        AS
                        BEGIN
                             IF @PaymentID <> 0 
                             BEGIN
                                 SELECT PaymentID,PaymentType,LedgerID FROM tblCashDeskMaster
                                 WHERE PaymentID = @PaymentID 
		                         ORDER BY PaymentType ASC
                             END
                             ELSE
                             BEGIN
		                        IF @Paymentids <> ''
		                        BEGIN
			                        DECLARE @CashDeskMaster VARCHAR(1000)
			                        SELECT @CashDeskMaster = COALESCE(@CashDeskMaster + ',', '') + PaymentType 
			                        FROM tblCashDeskMaster 
			                        WHERE ','+@Paymentids+',' LIKE '%,'+CONVERT(VARCHAR(50),PaymentID)+',%';
			                        select @CashDeskMaster
		                        END
		                        ELSE
		                        BEGIN
			                        SELECT PaymentID,PaymentType  
			                        FROM tblCashDeskMaster 
			                        ORDER BY PaymentType ASC
		                        END
                             END
                        END";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"DROP PROCEDURE [dbo].[UspCashDeskMasterInsert] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspCashDeskMasterInsert] (
                                @PaymentID    NUMERIC  (18,0),
                                @PaymentType    VARCHAR  (50),
                                @LedgerID    NUMERIC  (18,0),
                                @Action             INT=0 )
                        AS
                        BEGIN
                        DECLARE @RetResult      INT
                        BEGIN TRY
                        BEGIN TRANSACTION;
                        IF @Action = 0
                        BEGIN
                                INSERT INTO tblCashDeskMaster(PaymentID,PaymentType,LedgerID)
                                VALUES(@PaymentID,@PaymentType,@LedgerID)
                                SET @RetResult = 1;
                        END
                        IF @Action = 1
                        BEGIN
                                UPDATE tblCashDeskMaster SET PaymentType=@PaymentType,LedgerID=@LedgerID
                                WHERE PaymentID=@PaymentID
                                SET @RetResult = 1;
                        END
                        IF @Action = 2
                        BEGIN
                                DELETE FROM tblCashDeskMaster WHERE PaymentID=@PaymentID

                                SET @RetResult = 0;
                        END
                        COMMIT TRANSACTION;
                        SELECT @RetResult as SqlSpResult
                        END TRY
                        BEGIN CATCH
                        ROLLBACK;
                        SELECT
                        - 1 as SqlSpResult,
                        ERROR_NUMBER() AS ErrorNumber,
                        ERROR_STATE() AS ErrorState,
                        ERROR_SEVERITY() AS ErrorSeverity,
                        ERROR_PROCEDURE() AS ErrorProcedure,
                        ERROR_LINE() AS ErrorLine,
                        ERROR_MESSAGE() AS ErrorMessage;
                        END CATCH;
                        END";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"drop TRIGGER [dbo].[trgTotalTrigger] ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE TRIGGER [dbo].[trgTotalTrigger] 
                            ON  [dbo].[tblSalesItem] 
                            AFTER INSERT,DELETE,UPDATE
                        AS 
                        BEGIN
	                        SET NOCOUNT ON;
	                        declare @invid numeric
	                        select @invid = invid from inserted
	                        update tblsales set qtytotal = ( select sum(qty + free) from tblSalesItem where InvID = @invid ) where InvID = @invid 
	                        update tblsales set SGSTTotal = ( select sum(SGSTTaxAmt) from tblSalesItem where InvID = @invid ) where InvID = @invid 
	                        update tblsales set CGSTTotal = ( select sum(CGSTTaxAmt) from tblSalesItem where InvID = @invid ) where InvID = @invid 
	                        update tblsales set IGSTTotal = ( select sum(IGSTTaxAmt) from tblSalesItem where InvID = @invid ) where InvID = @invid 
                        END";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"drop PROCEDURE UspRepackingInsert";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                //UspRepackingInsert
                sQuery = @"CREATE PROCEDURE [dbo].[UspRepackingInsert] (@InvId                        NUMERIC
                            (18, 0),
                                                                       @InvNo                        VARCHAR
                            (100),
                                                                       @AutoNum                      NUMERIC
                            (18, 0),
                                                                       @Prefix                       VARCHAR
                            (50),
                                                                       @InvDate
                            DATETIME,
                                                                       @VchType                      VARCHAR
                            (100),
                                                                       @MOP                          VARCHAR
                            (100),
                                                                       @TaxModeID                    NUMERIC
                            (18, 0),
                                                                       @LedgerId                     NUMERIC
                            (18, 0),
                                                                       @Party                        VARCHAR
                            (100),
                                                                       @Discount                     FLOAT,
                                                                       @TaxAmt                       FLOAT,
                                                                       @GrossAmt                     FLOAT,
                                                                       @BillAmt                      FLOAT,
                                                                       @Cancelled                    NUMERIC
                            (18, 0),
                                                                       @OtherExpense                 FLOAT,
                                                                       @SalesManID                   NUMERIC
                            (18, 0),
                                                                       @Taxable                      FLOAT,
                                                                       @NonTaxable                   FLOAT,
                                                                       @ItemDiscountTotal            FLOAT,
                                                                       @RoundOff                     FLOAT,
                                                                       @UserNarration                VARCHAR
                            (500),
                                                                       @SortNumber                   NUMERIC
                            (18, 0),
                                                                       @DiscPer                      FLOAT,
                                                                       @VchTypeID                    NUMERIC
                            (18, 0),
                                                                       @CCID                         NUMERIC
                            (18, 0),
                                                                       @CurrencyID                   NUMERIC
                            (18, 0),
                                                                       @PartyAddress                 VARCHAR
                            (500),
                                                                       @UserID                       INT,
                                                                       @AgentID                      NUMERIC
                            (18, 0),
                                                                       @CashDiscount                 FLOAT,
                                                                       @DPerType_ManualCalc_Customer NUMERIC
                            (18, 0),
                                                                       @NetAmount                    FLOAT,
                                                                       @RefNo                        VARCHAR
                            (100),
                                                                       @CashPaid                     NUMERIC
                            (18, 0),
                                                                       @CardPaid                     NUMERIC
                            (18, 0),
                                                                       @blnWaitforAuthorisation      NUMERIC
                            (18, 0),
                                                                       @UserIDAuth                   NUMERIC
                            (18, 0),
                                                                       @BillTime
                            DATETIME,
                                                                       @StateID                      NUMERIC
                            (18, 0),
                                                                       @ImplementingStateCode        VARCHAR
                            (50),
                                                                       @GSTType                      VARCHAR
                            (50),
                                                                       @CGSTTotal                    FLOAT,
                                                                       @SGSTTotal                    FLOAT,
                                                                       @IGSTTotal                    FLOAT,
                                                                       @PartyGSTIN                   VARCHAR
                            (50),
                                                                       @BillType                     VARCHAR
                            (50),
                                                                       @blnHold                      NUMERIC
                            (18, 0),
                                                                       @PriceListID                  NUMERIC
                            (18, 0),
                                                                       @EffectiveDate
                            DATETIME,
                                                                       @partyCode                    VARCHAR
                            (150),
                                                                       @MobileNo                     VARCHAR
                            (20),
                                                                       @Email                        VARCHAR
                            (100),
                                                                       @TaxType                      VARCHAR
                            (50),
                                                                       @QtyTotal                     FLOAT,
                                                                       @DestCCID                     NUMERIC
                            (18, 0),
                                                                       @AgentCommMode                VARCHAR
                            (50),
                                                                       @AgentCommAmount              FLOAT,
                                                                       @AgentLID                     NUMERIC
                            (18, 0),
                                                                       @BlnStockInsert               NUMERIC
                            (18, 0),
                                                                       @BlnConverted                 NUMERIC
                            (18, 0),
                                                                       @ConvertedParentVchTypeID     NUMERIC
                            (18, 0),
                                                                       @ConvertedVchTypeID           NUMERIC
                            (18, 0),
                                                                       @ConvertedVchNo               VARCHAR
                            (50),
                                                                       @ConvertedVchID               NUMERIC
                            (18, 0),
                                                                       @DeliveryNoteDetails          VARCHAR
                            (500),
                                                                       @OrderDetails                 VARCHAR
                            (500),
                                                                       @IntegrityStatus              VARCHAR
                            (50),
                                                                       @BalQty                       FLOAT,
                                                                       @CustomerpointsSettled        FLOAT,
                                                                       @blnCashPaid                  NUMERIC
                            (18, 0),
                                                                       @originalsalesinvid           NUMERIC
                            (18, 0),
                                                                       @retuninvid                   NUMERIC
                            (18, 0),
                                                                       @returnamount                 FLOAT,
                                                                       @SystemName                   VARCHAR
                            (50),
                                                                       @LastUpdateDate
                            DATETIME,
                                                                       @LastUpdateTime
                            DATETIME,
                                                                       @DeliveryDetails              VARCHAR
                            (max),
                                                                       @DespatchDetails              VARCHAR
                            (max),
                                                                       @TermsOfDelivery              VARCHAR
                            (max),
                                                                       @FloodCessTot                 FLOAT,
                                                                       @CounterID                    NUMERIC
                            (18, 0),
                                                                       @ExtraCharges                 FLOAT,
                                                                       @ReferenceAutoNO              VARCHAR
                            (50),
                                                                       @CashDiscPer                  FLOAT,
                                                                       @CostFactor                   NUMERIC
                            (18, 0),
                                                                       @TenantID                     NUMERIC
                            (18, 0),
                                                                       @JsonData                     VARCHAR
                            (max),
                                                                       @Action                       INT=0)
                            AS
                              BEGIN
                                  DECLARE @RetResult INT

                                  BEGIN try
                                      --BEGIN TRANSACTION;

                                      IF @Action = 0
                                        BEGIN
                                            INSERT INTO tblRepacking
                                                        (invid,
                                                         invno,
                                                         autonum,
                                                         prefix,
                                                         invdate,
                                                         vchtype,
                                                         mop,
                                                         taxmodeid,
                                                         ledgerid,
                                                         party,
                                                         discount,
                                                         taxamt,
                                                         grossamt,
                                                         billamt,
                                                         cancelled,
                                                         otherexpense,
                                                         salesmanid,
                                                         taxable,
                                                         nontaxable,
                                                         itemdiscounttotal,
                                                         roundoff,
                                                         usernarration,
                                                         sortnumber,
                                                         discper,
                                                         vchtypeid,
                                                         ccid,
                                                         currencyid,
                                                         partyaddress,
                                                         userid,
                                                         agentid,
                                                         cashdiscount,
                                                         netamount,
                                                         refno,
                                                         blnwaitforauthorisation,
                                                         useridauth,
                                                         billtime,
                                                         stateid,
                                                         gsttype,
                                                         partygstin,
                                                         blnhold,
                                                         pricelistid,
                                                         effectivedate,
                                                         mobileno,
                                                         qtytotal,
                                                         blnstockinsert,
                                                         systemname,
                                                         lastupdatedate,
                                                         lastupdatetime,
                                                         referenceautono,
                                                         cashdisper,
                                                         costfactor,
                                                         tenantid,
                                                         jsondata)
                                            VALUES     (@InvId,
                                                        @InvNo,
                                                        @AutoNum,
                                                        @Prefix,
                                                        @InvDate,
                                                        @VchType,
                                                        @MOP,
                                                        @TaxModeID,
                                                        @LedgerId,
                                                        @Party,
                                                        @Discount,
                                                        @TaxAmt,
                                                        @GrossAmt,
                                                        @BillAmt,
                                                        @Cancelled,
                                                        @OtherExpense,
                                                        @SalesManID,
                                                        @Taxable,
                                                        @NonTaxable,
                                                        @ItemDiscountTotal,
                                                        @RoundOff,
                                                        @UserNarration,
                                                        @SortNumber,
                                                        @DiscPer,
                                                        @VchTypeID,
                                                        @CCID,
                                                        @CurrencyID,
                                                        @PartyAddress,
                                                        @UserID,
                                                        @AgentID,
                                                        @CashDiscount,
                                                        @NetAmount,
                                                        @RefNo,
                                                        @blnWaitforAuthorisation,
                                                        @UserIDAuth,
                                                        @BillTime,
                                                        @StateID,
                                                        @GSTType,
                                                        @PartyGSTIN,
                                                        @blnHold,
                                                        @PriceListID,
                                                        @EffectiveDate,
                                                        @MobileNo,
                                                        @QtyTotal,
                                                        @BlnStockInsert,
                                                        @SystemName,
                                                        @LastUpdateDate,
                                                        @LastUpdateTime,
                                                        @ReferenceAutoNO,
                                                        @CashDiscPer,
                                                        @CostFactor,
                                                        @TenantID,
                                                        @JsonData)

                                            SET @RetResult = 1;
                                        END

                                      IF @Action = 1
                                        BEGIN
                                            UPDATE tblRepacking
                                            SET    invno = @InvNo,
                                                   autonum = @AutoNum,
                                                   prefix = @Prefix,
                                                   invdate = @InvDate,
                                                   vchtype = @VchType,
                                                   mop = @MOP,
                                                   taxmodeid = @TaxModeID,
                                                   ledgerid = @LedgerId,
                                                   party = @Party,
                                                   discount = @Discount,
                                                   taxamt = @TaxAmt,
                                                   grossamt = @GrossAmt,
                                                   billamt = @BillAmt,
                                                   cancelled = @Cancelled,
                                                   otherexpense = @OtherExpense,
                                                   salesmanid = @SalesManID,
                                                   taxable = @Taxable,
                                                   nontaxable = @NonTaxable,
                                                   itemdiscounttotal = @ItemDiscountTotal,
                                                   roundoff = @RoundOff,
                                                   usernarration = @UserNarration,
                                                   sortnumber = @SortNumber,
                                                   discper = @DiscPer,
                                                   vchtypeid = @VchTypeID,
                                                   ccid = @CCID,
                                                   currencyid = @CurrencyID,
                                                   partyaddress = @PartyAddress,
                                                   userid = @UserID,
                                                   agentid = @AgentID,
                                                   cashdiscount = @CashDiscount,
                                                   netamount = @NetAmount,
                                                   refno = @RefNo,
                                                   blnwaitforauthorisation = @blnWaitforAuthorisation,
                                                   useridauth = @UserIDAuth,
                                                   billtime = @BillTime,
                                                   stateid = @StateID,
                                                   gsttype = @GSTType,
                                                   partygstin = @PartyGSTIN,
                                                   blnhold = @blnHold,
                                                   pricelistid = @PriceListID,
                                                   effectivedate = @EffectiveDate,
                                                   mobileno = @MobileNo,
                                                   qtytotal = @QtyTotal,
                                                   blnstockinsert = @BlnStockInsert,
                                                   systemname = @SystemName,
                                                   lastupdatedate = @LastUpdateDate,
                                                   lastupdatetime = @LastUpdateTime,
                                                   referenceautono = @ReferenceAutoNO,
                                                   cashdisper = @CashDiscPer,
                                                   costfactor = @CostFactor,
                                                   jsondata = @JsonData
                                            WHERE  invid = @InvId
                                                   AND tenantid = @TenantID

                                            SET @RetResult = 1;
                                        END

                                      IF @Action = 2
                                        BEGIN
                                            DELETE FROM tblRepacking
                                            WHERE  invid = @InvId
                                                   AND tenantid = @TenantID

                                            SET @RetResult = 0;
                                        END

                                      IF @Action = 3
                                        BEGIN
                                            UPDATE tblRepacking
                                            SET    cancelled = 1
                                            WHERE  invid = @InvId
                                                   AND tenantid = @TenantID

                                            SET @RetResult = 3;
                                        END

                                      --COMMIT TRANSACTION;

                                      SELECT @RetResult AS SqlSpResult
                                  END try

                                  BEGIN catch
                                      --ROLLBACK;

                                      SELECT -1                AS SqlSpResult,
                                             Error_number()    AS ErrorNumber,
                                             Error_state()     AS ErrorState,
                                             Error_severity()  AS ErrorSeverity,
                                             Error_procedure() AS ErrorProcedure,
                                             Error_line()      AS ErrorLine,
                                             Error_message()   AS ErrorMessage;
                                  END catch;
                              END ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"drop PROCEDURE UspRepackingItemInsert";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                //UspRepackingItemInsert
                sQuery = @"CREATE PROCEDURE [dbo].[UspRepackingItemInsert] (  
   	                            @InvID NUMERIC(18, 0)  
   	                            ,@ItemId NUMERIC(18, 0)  
   	                            ,@Qty FLOAT  
   	                            ,@Rate FLOAT  
   	                            ,@UnitId NUMERIC(18, 0)  
   	                            ,@Batch VARCHAR(50)  
   	                            ,@TaxPer FLOAT  
   	                            ,@TaxAmount FLOAT  
   	                            ,@Discount FLOAT  
   	                            ,@MRP FLOAT  
   	                            ,@SlNo NUMERIC(18, 0)  
   	                            ,@Prate FLOAT  
   	                            ,@Free FLOAT  
   	                            ,@SerialNos VARCHAR(5000)  
   	                            ,@ItemDiscount FLOAT  
   	                            ,@BatchCode VARCHAR(50)  
   	                            ,@iCessOnTax FLOAT  
   	                            ,@blnCessOnTax NUMERIC(18, 0)  
   	                            ,@Expiry DATETIME  
   	                            ,@ItemDiscountPer FLOAT  
   	                            ,@RateInclusive NUMERIC(18, 0)  
   	                            ,@ITaxableAmount FLOAT  
   	                            ,@INetAmount FLOAT  
   	                            ,@CGSTTaxPer FLOAT  
   	                            ,@CGSTTaxAmt FLOAT  
   	                            ,@SGSTTaxPer FLOAT  
   	                            ,@SGSTTaxAmt FLOAT  
   	                            ,@IGSTTaxPer FLOAT  
   	                            ,@IGSTTaxAmt FLOAT  
   	                            ,@iRateDiscPer FLOAT  
   	                            ,@iRateDiscount FLOAT  
   	                            ,@BatchUnique VARCHAR(150)  
   	                            ,@blnQtyIN NUMERIC(18, 0)  
   	                            ,@CRate FLOAT  
	                            ,@CRateWithTax FLOAT  
   	                            ,@Unit VARCHAR(50)  
   	                            ,@ItemStockID NUMERIC(18, 0)  
   	                            ,@IcessPercent FLOAT  
   	                            ,@IcessAmt FLOAT  
   	                            ,@IQtyCompCessPer FLOAT  
   	                            ,@IQtyCompCessAmt FLOAT  
   	                            ,@StockMRP FLOAT  
   	                            ,@BaseCRate FLOAT  
   	                            ,@InonTaxableAmount FLOAT  
   	                            ,@IAgentCommPercent FLOAT  
   	                            ,@BlnDelete NUMERIC(18, 0)  
   	                            ,@Id NUMERIC(18, 0)  
   	                            ,@StrOfferDetails VARCHAR(100)  
   	                            ,@BlnOfferItem FLOAT  
   	                            ,@BalQty FLOAT  
   	                            ,@GrossAmount FLOAT  
   	                            ,@iFloodCessPer FLOAT  
   	                            ,@iFloodCessAmt FLOAT  
   	                            ,@Srate1 FLOAT  
   	                            ,@Srate2 FLOAT  
   	                            ,@Srate3 FLOAT  
   	                            ,@Srate4 FLOAT  
   	                            ,@Srate5 FLOAT  
   	                            ,@Costrate FLOAT  
   	                            ,@CostValue FLOAT  
   	                            ,@Profit FLOAT  
   	                            ,@ProfitPer FLOAT  
   	                            ,@DiscMode NUMERIC(18, 0)  
   	                            ,@Srate1Per FLOAT  
   	                            ,@Srate2Per FLOAT  
   	                            ,@Srate3Per FLOAT  
   	                            ,@Srate4Per FLOAT  
   	                            ,@Srate5Per FLOAT  
   	                            ,@Action INT = 0  
   	                            )  
                               AS  
                               BEGIN  
   	                            DECLARE @RetResult INT  
   	                            DECLARE @RetID INT  
   	                            DECLARE @VchType VARCHAR(50)  
   	                            DECLARE @VchTypeID NUMERIC(18, 0)  
   	                            DECLARE @BatchMode VARCHAR(50)  
   	                            DECLARE @VchDate DATETIME  
   	                            DECLARE @CCID NUMERIC(18, 0)  
   	                            DECLARE @TenantID NUMERIC(18, 0)  
   	                            DECLARE @BarCode_out VARCHAR(2000)  
   	                            DECLARE @VchParentID NUMERIC(18, 0)  
   	                            DECLARE @FreeQty NUMERIC(18, 5)  
   	                            BEGIN TRY  
   		                            --BEGIN TRANSACTION;  

   		                            SET @FreeQty = @Qty + @Free  

   		                            SELECT @VchType = VchType  
   			                            ,@VchTypeID = VchTypeID  
   			                            ,@VchDate = InvDate  
   			                            ,@CCID = CCID  
   			                            ,@TenantID = TenantID  
   		                            FROM tblRepacking  
   		                            WHERE InvId = @InvID  

   		                            SELECT @BatchMode = BatchMode  
   		                            FROM tblItemMaster  
   		                            WHERE ItemID = @ItemId  
   		                            SELECT @VchParentID = ParentID  
   		                            FROM tblVchType  
   		                            WHERE VchTypeID = @VchTypeID  

   		                            IF @Action = 0  
   		                            BEGIN  
   			                            IF @VchParentID = 20 
   			                            BEGIN  
											IF @blnQtyIN = 1 
   				                            BEGIN
												EXEC UspTransStockUpdate @ItemId  
   													,@BatchCode  
   													,@BatchUnique  
   													,@FreeQty  
   													,@MRP  
   													,@CRateWithTax  
   													,@CRate  
   													,@Prate  
   													,@Prate  
   													,@TaxPer  
   													,@Srate1  
   													,@Srate2  
   													,@Srate3  
   													,@Srate4  
   													,@Srate5  
   													,@BatchMode  
   													,@VchType  
   													,@VchDate  
   													,@Expiry  
   													,'STOCKADD'  
   													,@InvID  
   													,@VchTypeID  
   													,@CCID  
   													,@TenantID  
   													,@Prate  
   													,@BarCode_out OUTPUT  

												IF CHARINDEX('-1|ERROR|CRITICAL|', @BarCode_out) > 0  
   												BEGIN
													SELECT - 1 AS SqlSpResult  
   														,@RetID AS PID  
   														,-1 AS ErrorNumber  
   														,'' AS ErrorState  
   														,'' AS ErrorSeverity  
   														,'' AS ErrorProcedure  
   														,-1 AS ErrorLine  
   														,'The barcode is assigned with another MRP. Multiple MRP not allowed for <Auto Barcode>.' AS ErrorMessage;  
						
														RETURN;
												END

   												IF CHARINDEX('@', @BarCode_out) > 0  
   												BEGIN  
   													SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))  
   												END  
											END
											ELSE IF @blnQtyIN = 0 
   				                            BEGIN
												EXEC UspTransStockUpdate @ItemId  
   													,@BatchCode  
   													,@BatchUnique  
   													,@FreeQty  
   													,@MRP  
   													,@CRateWithTax  
   													,@CRate  
   													,@Prate  
   													,@Prate  
   													,@TaxPer  
   													,@Srate1  
   													,@Srate2  
   													,@Srate3  
   													,@Srate4  
   													,@Srate5  
   													,@BatchMode  
   													,@VchType  
   													,@VchDate  
   													,@Expiry  
   													,'STOCKLESS'  
   													,@InvID  
   													,@VchTypeID  
   													,@CCID  
   													,@TenantID  
   													,@Prate  
   													,@BarCode_out OUTPUT  

												IF CHARINDEX('-1|ERROR|CRITICAL|', @BarCode_out) > 0  
   												BEGIN
													SELECT - 1 AS SqlSpResult  
   														,@RetID AS PID  
   														,-1 AS ErrorNumber  
   														,'' AS ErrorState  
   														,'' AS ErrorSeverity  
   														,'' AS ErrorProcedure  
   														,-1 AS ErrorLine  
   														,'The barcode is assigned with another MRP. Multiple MRP not allowed for <Auto Barcode>.' AS ErrorMessage;  
						
														RETURN;
												END

   												IF CHARINDEX('@', @BarCode_out) > 0  
   												BEGIN  
   													SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))  
   												END  
											END
   			                            END  
   			                            ELSE  
   			                            BEGIN  
   				                            SET @BatchCode = @BarCode_out  
   			                            END  
   			                            INSERT INTO tblRepackingItem (  
   				                            InvID  
   				                            ,ItemId  
   				                            ,Qty  
   				                            ,Rate  
   				                            ,UnitId  
   				                            ,Batch  
   				                            ,TaxPer  
   				                            ,TaxAmount  
   				                            ,Discount  
   				                            ,MRP  
   				                            ,SlNo  
   				                            ,Prate  
   				                            ,Free  
   				                            ,SerialNos  
   				                            ,ItemDiscount  
   				                            ,BatchCode  
   				                            ,iCessOnTax  
   				                            ,blnCessOnTax  
   				                            ,Expiry  
   				                            ,ItemDiscountPer  
   				                            ,RateInclusive  
   				                            ,ITaxableAmount  
   				                            ,INetAmount  
   				                            ,CGSTTaxPer  
   				                            ,CGSTTaxAmt  
   				                            ,SGSTTaxPer  
   				                            ,SGSTTaxAmt  
   				                            ,IGSTTaxPer  
   				                            ,IGSTTaxAmt  
   				                            ,iRateDiscPer  
   				                            ,iRateDiscount  
   				                            ,BatchUnique  
   				                            ,blnQtyIN  
   				                            ,CRate  
   				                            ,Unit  
   				                            ,ItemStockID  
   				                            ,IcessPercent  
   				                            ,IcessAmt  
   				                            ,IQtyCompCessPer  
   				                            ,IQtyCompCessAmt  
   				                            ,StockMRP  
   				                            ,BaseCRate  
   				                            ,InonTaxableAmount  
   				                            ,IAgentCommPercent  
   				                            ,BlnDelete  
   				                            ,StrOfferDetails  
   				                            ,BlnOfferItem  
   				                            ,BalQty  
   				                            ,GrossAmount  
   				                            ,iFloodCessPer  
   				                            ,iFloodCessAmt  
   				                            ,Srate1  
   				                            ,Srate2  
   				                            ,Srate3  
   				                            ,Srate4  
   				                            ,Srate5  
   				                            ,Costrate  
   				                            ,CostValue  
   				                            ,Profit  
   				                            ,ProfitPer  
   				                            ,DiscMode  
   				                            ,Srate1Per  
   				                            ,Srate2Per  
   				                            ,Srate3Per  
   				                            ,Srate4Per  
   				                            ,Srate5Per  
   				                            )  
   			                            VALUES (  
   				                            @InvID  
   				                            ,@ItemId  
   				                            ,@Qty  
   				                            ,@Rate  
   				                            ,@UnitId  
   				                            ,@Batch  
   				                            ,@TaxPer  
   				                            ,@TaxAmount  
   				                            ,@Discount  
   				                            ,@MRP  
   				                            ,@SlNo  
   				                            ,@Prate  
   				                            ,@Free  
   				                            ,@SerialNos  
   				                            ,@ItemDiscount  
   				                            ,@BatchCode  
   				                            ,@iCessOnTax  
   				                            ,@blnCessOnTax  
   				                            ,@Expiry  
   				                            ,@ItemDiscountPer  
   				                            ,@RateInclusive  
   				                            ,@ITaxableAmount  
   				                            ,@INetAmount  
   				                            ,@CGSTTaxPer  
   				                            ,@CGSTTaxAmt  
   				                            ,@SGSTTaxPer  
   				                            ,@SGSTTaxAmt  
   				                            ,@IGSTTaxPer  
   				                            ,@IGSTTaxAmt  
   				                            ,@iRateDiscPer  
   				                            ,@iRateDiscount  
   				                            ,@BarCode_out  
   				                            ,@blnQtyIN  
   				                            ,@CRate  
   				                            ,@Unit  
   				                            ,@ItemStockID  
   				                            ,@IcessPercent  
   				                            ,@IcessAmt  
   				                            ,@IQtyCompCessPer  
   				                            ,@IQtyCompCessAmt  
   				                            ,@StockMRP  
   				                            ,@BaseCRate  
   				                            ,@InonTaxableAmount  
   				                            ,@IAgentCommPercent  
   				                            ,@BlnDelete  
   				                            ,@StrOfferDetails  
   				                            ,@BlnOfferItem  
   				                            ,@BalQty  
   				                            ,@GrossAmount  
   				                            ,@iFloodCessPer  
   				                            ,@iFloodCessAmt  
   				                            ,@Srate1  
   				                            ,@Srate2  
   				                            ,@Srate3  
   				                            ,@Srate4  
   				                            ,@Srate5  
   				                            ,@Costrate  
   				                            ,@CostValue  
   				                            ,@Profit  
   				                            ,@ProfitPer  
   				                            ,@DiscMode  
   				                            ,@Srate1Per  
   				                            ,@Srate2Per  
   				                            ,@Srate3Per  
   				                            ,@Srate4Per  
   				                            ,@Srate5Per  
   				                            )  
   			                            SET @RetResult = 1;  
   		                            END  
   		                            ELSE IF @Action = 2  
   		                            BEGIN  
   			                            EXEC UspTransStockUpdate @ItemId  
   				                            ,@BatchCode  
   				                            ,@BatchUnique  
   				                            ,@FreeQty  
   				                            ,@MRP  
   				                            ,@CRateWithTax  
   				                            ,@CRate  
   				                            ,@Prate  
   				                            ,@Prate  
   				                            ,@TaxPer  
   				                            ,@Srate1  
   				                            ,@Srate2  
   				                            ,@Srate3  
   				                            ,@Srate4  
   				                            ,@Srate5  
   				                            ,@BatchMode  
   				                            ,@VchType  
   				                            ,@VchDate  
   				                            ,@Expiry  
   				                            ,'STOCKDEL'  
   				                            ,@InvID  
   				                            ,@VchTypeID  
   				                            ,@CCID  
   				                            ,@TenantID  
   				                            ,@Prate  
   				                            ,@BarCode_out OUTPUT  

			                            DELETE  
   			                            FROM tblRepackingItem  
   			                            WHERE InvID = @InvID  

   			                            SET @RetResult = 0;  
   		                            END  
   		                            ELSE IF @Action = 3  
   		                            BEGIN  
   			                            EXEC UspTransStockUpdate @ItemId  
   				                            ,@BatchCode  
   				                            ,@BatchUnique  
   				                            ,@FreeQty  
   				                            ,@MRP  
   				                            ,@CRateWithTax  
   				                            ,@CRate  
   				                            ,@Prate  
   				                            ,@Prate  
   				                            ,@TaxPer  
   				                            ,@Srate1  
   				                            ,@Srate2  
   				                            ,@Srate3  
   				                            ,@Srate4  
   				                            ,@Srate5  
   				                            ,@BatchMode  
   				                            ,@VchType  
   				                            ,@VchDate  
   				                            ,@Expiry  
   				                            ,'STOCKDEL'  
   				                            ,@InvID  
   				                            ,@VchTypeID  
   				                            ,@CCID  
   				                            ,@TenantID  
   				                            ,@Prate  
   				                            ,@BarCode_out OUTPUT  
   			                            SET @RetResult = 0;  
   		                            END  
   		                            --COMMIT TRANSACTION;  
   		                            SELECT @RetResult AS SqlSpResult  
   			                            ,@RetID AS PID  
   	                            END TRY  
   	                            BEGIN CATCH  
   		                            --ROLLBACK;  
   		                            SELECT - 1 AS SqlSpResult  
   			                            ,@RetID AS PID  
   			                            ,ERROR_NUMBER() AS ErrorNumber  
   			                            ,ERROR_STATE() AS ErrorState  
   			                            ,ERROR_SEVERITY() AS ErrorSeverity  
   			                            ,ERROR_PROCEDURE() AS ErrorProcedure  
   			                            ,ERROR_LINE() AS ErrorLine  
   			                            ,ERROR_MESSAGE() AS ErrorMessage;  
   	                            END CATCH;  
                               END ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"drop PROCEDURE UspGetRepackingMaster";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                //UspGetRepackingMaster
                sQuery = @"CREATE PROCEDURE [dbo].[UspGetRepackingMaster] (  
   	                        @InvId NUMERIC(18, 0)  
   	                        ,@TenantID NUMERIC(18, 0)  
   	                        ,@VchTypeID NUMERIC(18, 0)  
   	                        ,@blnPrevNext BIT = 0  
   	                        )  
                           AS  
                           BEGIN  
   	                        DECLARE @PrevVoucherNo INT  
   	                        DECLARE @NextVoucherNo INT  
   	                        DECLARE @InvId_Org INT  
   	                        IF @InvId <> 0  
   	                        BEGIN  
   		                        IF @blnPrevNext = 0  
   		                        BEGIN  
   			                        SELECT party  
   				                        ,InvId  
   				                        ,InvNo  
   				                        ,AutoNum  
   				                        ,Prefix  
   				                        ,convert(VARCHAR(10), InvDate, 105) AS InvDate  
   				                        ,convert(VARCHAR(10), EffectiveDate, 105) AS EffectiveDate  
   				                        ,RefNo  
   				                        ,ReferenceAutoNO  
   				                        ,MOP  
   				                        ,TaxModeID  
   				                        ,CCID  
   				                        ,SalesManID  
   				                        ,AgentID  
   				                        ,MobileNo  
   				                        ,StateID  
   				                        ,GSTType  
   				                        ,PartyAddress  
   				                        ,GrossAmt  
   				                        ,ItemDiscountTotal  
   				                        ,DiscPer  
   				                        ,Discount  
   				                        ,Taxable  
   				                        ,NonTaxable  
   				                        ,TaxAmt  
   				                        ,OtherExpense  
   				                        ,NetAmount  
   				                        ,CashDiscount  
   				                        ,RoundOff  
   				                        ,UserNarration  
   				                        ,BillAmt  
   				                        ,PartyGSTIN  
   				                        ,Isnull(CashDisPer, 0) AS CashDisPer  
   				                        ,Isnull(CostFactor, 0) AS CostFactor  
   				                        ,LedgerId  
   				                        ,Cancelled  
   				                        ,JsonData  
   			                        FROM tblRepacking  
   			                        WHERE InvId = @InvId  
   				                        AND TenantID = @TenantID  
   				                        AND VchTypeID = @VchTypeID  
   		                        END  
   		                        ELSE  
   		                        BEGIN  
   			                        SELECT @InvId_Org = InvId  
   			                        FROM tblRepacking  
   			                        WHERE InvNo = @InvId  
   				                        AND TenantID = @TenantID  
   				                        AND VchTypeID = @VchTypeID  
   			                        SELECT TOP 1 @PrevVoucherNo = InvId  
   			                        FROM tblRepacking  
   			                        WHERE InvId < @InvId_Org  
   				                        AND VchTypeID = @VchTypeID  
   			                        ORDER BY InvId DESC  
   			                        SELECT TOP 1 @NextVoucherNo = InvId  
   			                        FROM tblRepacking  
   			                        WHERE InvId > @InvId_Org  
   				                        AND VchTypeID = @VchTypeID  
   			                        ORDER BY InvId ASC  
   			                        SELECT ISNULL(@PrevVoucherNo, 0) AS PrevVoucherNo  
   				                        ,ISNULL(@NextVoucherNo, 0) AS NextVoucherNo  
   		                        END  
   	                        END  
   	                        ELSE  
   	                        BEGIN  
   		                        SELECT InvId  
   			                        ,InvNo AS [Invoice No]  
   			                        ,CONVERT(VARCHAR(12), InvDate) AS [Invoice Date]  
   			                        ,ISNULL(RefNo, '') + CONVERT(VARCHAR, ReferenceAutoNO) AS [Reference No]  
   			                        ,MOP  
   			                        ,Party AS [Supplier]  
   			                        ,MobileNo AS [Supplier Contact]  
   			                        --,RoundOff AS [RoundOff]  
   			                        ,BillAmt AS [Bill Amount]  
   			                        ,(  
   				                        CASE   
   					                        WHEN ISNULL(Cancelled, 0) = 0  
   						                        THEN 'Active'  
   					                        ELSE 'Cancelled'  
   					                        END  
   				                        ) AS [Bill Status]  
   		                        FROM tblRepacking  
   		                        WHERE TenantID = @TenantID  
   			                        AND VchTypeID = @VchTypeID  
   		                        ORDER BY InvID ASC  
   	                        END  
                           END ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"drop PROCEDURE UspGetRepackingDetailItem ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                //UspGetRepackingDetailItem
                sQuery = @" CREATE PROCEDURE [dbo].[UspGetRepackingDetailItem]--UspGetRepackingDetailItem 1
                            (
                            @InvID    NUMERIC   (18,0)
                            )
                            AS
                            BEGIN
                                    IF @InvID <> 0 
                                    BEGIN
                                        SELECT InvID,ItemId,Qty,Rate,UnitId,Batch,TaxPer,TaxAmount,Discount,MRP,SlNo,Prate,Free,SerialNos,ItemDiscount,BatchCode,iCessOnTax,blnCessOnTax, 
		                                --Expiry,--convert(varchar(10), Expiry, 105) as Expiry,
		                                (Format(Expiry, 'dd/MMM/yyyy ')) as Expiry,
		                                ItemDiscountPer,RateInclusive,ITaxableAmount,INetAmount,CGSTTaxPer,CGSTTaxAmt,SGSTTaxPer,SGSTTaxAmt,IGSTTaxPer,IGSTTaxAmt,iRateDiscPer,iRateDiscount,
		                                BatchUnique,blnQtyIN,CRate,Unit,ItemStockID,IcessPercent,IcessAmt,IQtyCompCessPer,IQtyCompCessAmt,StockMRP,BaseCRate,InonTaxableAmount,IAgentCommPercent,
		                                BlnDelete,Id,StrOfferDetails,BlnOfferItem,BalQty,GrossAmount,iFloodCessPer,iFloodCessAmt,Srate1,Srate2,Srate3,Srate4,Srate5,Costrate,CostValue,Profit,
		                                ProfitPer,DiscMode,Srate1Per,Srate2Per,Srate3Per,Srate4Per,Srate5Per 
		                                ,(select itemcode from tblItemMaster TM where TM.ItemID=PD.ItemID) as itemcode
		                                ,(select ItemName from tblItemMaster TM where TM.ItemID=PD.ItemID) as ItemName
		                                ,(select Unit from tblItemMaster TM where TM.ItemID=PD.ItemID) as UnitName
		                                ,(select ReferenceAutoNO from tblRepacking P where P.InvId=PD.InvID) as RefAutoNum
		                                ,(select VchTypeID from tblRepacking P where P.InvId=PD.InvID) as VchTypeID
		                                ,(select InvDate from tblRepacking P where P.InvId=PD.InvID) as InvDate
		                                ,(select Expiry from tblRepacking P where P.InvId=PD.InvID) as Expiry
		                                ,(select BatchMode from tblItemMaster TM where TM.ItemID=PD.ItemID) as BatchMode
		                                --,(select CostRateExcl from tblStock TS where TS.ItemID=PD.ItemID) as CostRateExcl
		                                --,(select CostRateInc from tblStock TS where TS.ItemID=PD.ItemID) as CostRateInc
		                                --,(select PRateExcl from tblStock TS where TS.ItemID=PD.ItemID) as PRateExcl
		                                --,(select PrateInc from tblStock TS where TS.ItemID=PD.ItemID) as PrateInc

		                                FROM tblRepackingItem PD
                                        WHERE InvID = @InvID 
                                    END
                                    ELSE
                                    BEGIN
                                        SELECT InvID,ItemId,Qty,Rate,UnitId,Batch,TaxPer,TaxAmount,Discount,MRP,SlNo,Prate,Free,SerialNos,ItemDiscount,BatchCode,iCessOnTax,blnCessOnTax,Expiry,ItemDiscountPer,RateInclusive,ITaxableAmount,INetAmount,CGSTTaxPer,CGSTTaxAmt,SGSTTaxPer,SGSTTaxAmt,IGSTTaxPer,IGSTTaxAmt,iRateDiscPer,iRateDiscount,BatchUnique,blnQtyIN,CRate,Unit,ItemStockID,IcessPercent,IcessAmt,IQtyCompCessPer,IQtyCompCessAmt,StockMRP,BaseCRate,InonTaxableAmount,IAgentCommPercent,BlnDelete,Id,StrOfferDetails,BlnOfferItem,BalQty,GrossAmount,iFloodCessPer,iFloodCessAmt,Srate1,Srate2,Srate3,Srate4,Srate5,Costrate,CostValue,Profit,ProfitPer,DiscMode,Srate1Per,Srate2Per,Srate3Per,Srate4Per,Srate5Per FROM tblRepackingItem
		                                order by InvID asc
                                    END
                            END";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }


            try
            {
                sQuery = @"drop PROCEDURE CLEARMASTERS ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"drop PROCEDURE CLEARTRANSACTIONS ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                //CLEARTRANSACTIONS
                sQuery = @"CREATE PROCEDURE [dbo].[CLEARTRANSACTIONS] AS 
                            BEGIN
                                DELETE FROM [dbo].[tblAccVoucherItem]
                                DELETE FROM [dbo].[tblAccVoucher]
                                DELETE FROM [dbo].[tblBoardRateDetail]
                                DELETE FROM [dbo].[tblBoardRateMaster]
                                DELETE FROM [dbo].[tblPriceListDetail]
                                DELETE FROM [dbo].[tblPriceListMaster]
                                DELETE FROM [dbo].[tblPurchaseItem]
                                DELETE FROM [dbo].[tblPurchase]
                                DELETE FROM [dbo].[tblRepackingItem]
                                DELETE FROM [dbo].[tblRepacking]
                                DELETE FROM [dbo].[tblSalesItem]
                                DELETE FROM [dbo].[tblSales]
                                DELETE FROM [dbo].[tblStockHistory]
                                DELETE FROM [dbo].[tblStock] WHERE 
									ItemID IN (SELECT ItemID FROM tblItemMaster WHERE BatchMode = 2) 
                                DELETE FROM [dbo].[tblStock] WHERE 
									BatchUnique NOT IN (
										SELECT BATCHUNIQUE FROM (SELECT ItemID , 
											(SELECT TOP 1 BatchUnique FROM tblStock WHERE tblItemMaster.ItemID = tblStock.ItemID 
												ORDER BY BatchID) AS BatchUnique FROM tblItemMaster) X )

                                UPDATE [dbo].[tblStock] SET QOH = 0 

                                DELETE FROM [dbo].[tblStockJournalItem]
                                DELETE FROM [dbo].[tblStockJournal]
                                DELETE FROM [dbo].[tblTransactionPause]
                                DELETE FROM [dbo].[tblVoucher]
                                DELETE FROM [dbo].[tblVoucherMaster]
								DELETE FROM [dbo].[tblCashDeskdetails]
								DELETE FROM [dbo].[tblCashDeskItems]
                            END";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                //clearmasters
                sQuery = @"CREATE PROCEDURE CLEARMASTERS AS 
                            BEGIN
                                --CLEAR STOCK AND ITEM MASTERS
                                DELETE FROM [dbo].[tblStock] 
                                DELETE FROM [dbo].[tblItemMaster]  --WHERE ITEMID NOT IN (SELECT DISTINCT ITEMID FROM [dbo].[tblStock]) 
                                DELETE FROM [dbo].[tblArea] where AreaID > 1
                                DELETE FROM [dbo].[tblBrand] where brandID > 1
                                DELETE FROM [dbo].[tblColor] where ColorID > 1
                                DELETE FROM [dbo].[tblSize] where SizeID > 1
                                DELETE FROM [dbo].[tblCategories] where CategoryID > 1
                                DELETE FROM [dbo].[tblCostCentre] where CCID > 1
                                DELETE FROM [dbo].[tblDepartment] where DepartmentID > 2
                                DELETE FROM [dbo].[tblLedger] where LID > 1000
                                DELETE FROM [dbo].[tblManufacturer] where MnfID > 1
                                DELETE FROM [dbo].[tblVchType] where VchTypeID > 1005
                            END";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"drop VIEW VWBOARDRATE";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"CREATE VIEW [dbo].[VWBOARDRATE]
                            AS
                          SELECT m.itemid,
                                 m.batchunique,
                                 M.ccid,
                                 M.invdate AS Invdate,M.invid,
                                 N.srate1,
                                 N.srate2,
                                 N.srate3,
                                 N.srate4,
                                 N.srate5,
                                 N.mrp
                          FROM   ( (SELECT A1.InvID,B1.itemid,
                                         B1.batchunique,
                                         A1.ccid                                 AS CCID,
                                         Max(( Cast(a1.invdate AS DATETIME)
                                               + Cast(a1.invtime AS DATETIME) )) AS Invdate,
                                         NULL                                    AS Srate1,
                                         NULL                                    AS Srate2,
                                         NULL                                    AS Srate3,
                                         NULL                                    AS Srate4,
                                         NULL                                    AS Srate5,
                                         NULL                                    AS Mrp
                                  FROM   tblboardratemaster A1,
                                         tblboardratedetail B1
                                  WHERE  A1.invid = B1.invid
                                  GROUP  BY A1.InvID,B1.itemid,
                                            B1.batchunique,
                                            A1.ccid) M
                                   INNER JOIN (SELECT A2.InvID,b2.itemid                          AS ItemId,
                                                      B2.batchunique,
                                                      A2.ccid                            AS CCID,
                                                      ( Cast(A2.invdate AS DATETIME)
                                                        + Cast(A2.invtime AS DATETIME) ) AS Invdate,
                                                      B2.srate1,
                                                      B2.srate2,
                                                      B2.srate3,
                                                      B2.srate4,
                                                      B2.srate5,
                                                      B2.mrp
                                               FROM   tblboardratemaster A2,
                                                      tblboardratedetail B2
                                               WHERE  A2.invid = B2.invid) N
                                           ON M.invdate = N.invdate
                                              AND M.itemid = N.itemid
                                              AND M.ccid = N.ccid
                                              AND M.batchunique = N.batchunique) ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"drop VIEW vwBoardRatePLU";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"CREATE VIEW [dbo].[vwBoardRatePLU]
                        AS
                          SELECT TOP (100) PERCENT dbo.tblitemmaster.itemid,
                           dbo.tblitemmaster.itemcode,
                           dbo.tblitemmaster.itemname,
                           dbo.tblstock.batchunique, dbo.tblStock.BatchCode, 
                           dbo.tblitemmaster.pluno              AS PLUNumber,
                           Isnull ((SELECT TOP (1) mrp
                                    FROM   dbo.vwboardrate AS pit
                                    WHERE  ( dbo.tblstock.batchunique =
                                             batchunique )
                                           AND ( ccid = 1 )
                                    ORDER  BY invdate DESC, invid desc), 0) AS MRP,
                           Isnull ((SELECT TOP (1) srate1
                                    FROM   dbo.vwboardrate AS pit
                                    WHERE  ( dbo.tblstock.batchunique =
                                             batchunique )
                                           AND ( ccid = 1 )
                                    ORDER  BY invdate DESC, invid desc), 0) AS SRATE1,
                           Isnull ((SELECT TOP (1) srate2
                                    FROM   dbo.vwboardrate AS pit
                                    WHERE  ( dbo.tblstock.batchunique =
                                             batchunique )
                                           AND ( ccid = 1 )
                                    ORDER  BY invdate DESC, invid desc), 0) AS SRATE2,
                           Isnull ((SELECT TOP (1) srate3
                                    FROM   dbo.vwboardrate AS pit
                                    WHERE  ( dbo.tblstock.batchunique =
                                             batchunique )
                                           AND ( ccid = 1 )
                                    ORDER  BY invdate DESC, invid desc), 0) AS SRATE3,
                           Isnull ((SELECT TOP (1) srate4
                                    FROM   dbo.vwboardrate AS pit
                                    WHERE  ( dbo.tblstock.batchunique =
                                             batchunique )
                                           AND ( ccid = 1 )
                                    ORDER  BY invdate DESC, invid desc), 0) AS SRATE4,
                           Isnull ((SELECT TOP (1) srate5
                                    FROM   dbo.vwboardrate AS pit
                                    WHERE  ( dbo.tblstock.batchunique =
                                             batchunique )
                                           AND ( ccid = 1 )
                                    ORDER  BY invdate DESC, invid desc), 0) AS SRATE5,
                           dbo.tblitemmaster.intnoorweight      AS Unit,
                           Isnull(dbo.tblstock.QOH, 0)      AS QOH,
                           Isnull(dbo.tblstock.ExpiryDate, '31-Dec-2050')      AS Expiry
  FROM   dbo.tblitemmaster
         INNER JOIN dbo.tblstock
                 ON dbo.tblstock.itemid = dbo.tblitemmaster.itemid
  WHERE  ( dbo.tblitemmaster.activestatus = 1 )
         AND ( dbo.tblitemmaster.batchmode = 3 )
  ORDER  BY dbo.tblitemmaster.itemcode ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"drop PROCEDURE UspBoardRateMasterInsert";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspBoardRateMasterInsert]
                    (
                         @InvID    NUMERIC(18, 0),
                         @InvNo    VARCHAR(50),
                         @InvDate    DATETIME,
                         @InvTime    DATETIME,
                         @VchtypeID    NUMERIC(18, 0),
                         @CCID    NUMERIC(18, 0),
                         @MachineModel    NUMERIC(18, 0),
                         @DisplayRate    NUMERIC(18, 0),
                         @MachineModelId    NUMERIC(18, 0),
                         @Action             INT = 0
                    )
                    AS
                    BEGIN
                    DECLARE @RetResult      INT
                    BEGIN TRY
                    IF @Action = 0
                    BEGIN
                         INSERT INTO tblBoardRateMaster(InvID, InvNo, InvDate, VchtypeID, CCID, MachineModel, InvTime, DisplayRate, MachineModelId)
                         VALUES(@InvID, @InvNo, @InvDate, @VchtypeID, @CCID, @MachineModel, @InvTime, @DisplayRate, @MachineModelId)
                         SET @RetResult = 1;
                                    END
                                    IF @Action = 1
                    BEGIN
                         UPDATE tblBoardRateMaster SET InvNo = @InvNo,InvDate = @InvDate,VchtypeID = @VchtypeID,CCID = @CCID,MachineModel = @MachineModel,InvTime = @InvTime,DisplayRate = @DisplayRate,MachineModelId = @MachineModelId
                         WHERE InvID = @InvID
                         SET @RetResult = 1;
                                    END
                                    IF @Action = 2
                    BEGIN
                         DELETE FROM tblBoardRateMaster WHERE InvID = @InvID

                         SET @RetResult = 0;
                                    END
                                    SELECT @RetResult as SqlSpResult
                    END TRY
                    BEGIN CATCH
                    SELECT
                    - 1 as SqlSpResult,
                    ERROR_NUMBER() AS ErrorNumber,
                    ERROR_STATE() AS ErrorState,
                    ERROR_SEVERITY() AS ErrorSeverity,
                    ERROR_PROCEDURE() AS ErrorProcedure,
                    ERROR_LINE() AS ErrorLine,
                    ERROR_MESSAGE() AS ErrorMessage;
                    END CATCH;
                    END";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"drop PROCEDURE UspPurchaseItemInsert";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspPurchaseItemInsert] (  
   	                            @InvID NUMERIC(18, 0)  
   	                            ,@ItemId NUMERIC(18, 0)  
   	                            ,@Qty FLOAT  
   	                            ,@Rate FLOAT  
   	                            ,@UnitId NUMERIC(18, 0)  
   	                            ,@Batch VARCHAR(50)  
   	                            ,@TaxPer FLOAT  
   	                            ,@TaxAmount FLOAT  
   	                            ,@Discount FLOAT  
   	                            ,@MRP FLOAT  
   	                            ,@SlNo NUMERIC(18, 0)  
   	                            ,@Prate FLOAT  
   	                            ,@Free FLOAT  
   	                            ,@SerialNos VARCHAR(5000)  
   	                            ,@ItemDiscount FLOAT  
   	                            ,@BatchCode VARCHAR(50)  
   	                            ,@iCessOnTax FLOAT  
   	                            ,@blnCessOnTax NUMERIC(18, 0)  
   	                            ,@Expiry DATETIME  
   	                            ,@ItemDiscountPer FLOAT  
   	                            ,@RateInclusive NUMERIC(18, 0)  
   	                            ,@ITaxableAmount FLOAT  
   	                            ,@INetAmount FLOAT  
   	                            ,@CGSTTaxPer FLOAT  
   	                            ,@CGSTTaxAmt FLOAT  
   	                            ,@SGSTTaxPer FLOAT  
   	                            ,@SGSTTaxAmt FLOAT  
   	                            ,@IGSTTaxPer FLOAT  
   	                            ,@IGSTTaxAmt FLOAT  
   	                            ,@iRateDiscPer FLOAT  
   	                            ,@iRateDiscount FLOAT  
   	                            ,@BatchUnique VARCHAR(150)  
   	                            ,@blnQtyIN NUMERIC(18, 0)  
   	                            ,@CRate FLOAT  
	                            ,@CRateWithTax FLOAT  
   	                            ,@Unit VARCHAR(50)  
   	                            ,@ItemStockID NUMERIC(18, 0)  
   	                            ,@IcessPercent FLOAT  
   	                            ,@IcessAmt FLOAT  
   	                            ,@IQtyCompCessPer FLOAT  
   	                            ,@IQtyCompCessAmt FLOAT  
   	                            ,@StockMRP FLOAT  
   	                            ,@BaseCRate FLOAT  
   	                            ,@InonTaxableAmount FLOAT  
   	                            ,@IAgentCommPercent FLOAT  
   	                            ,@BlnDelete NUMERIC(18, 0)  
   	                            ,@Id NUMERIC(18, 0)  
   	                            ,@StrOfferDetails VARCHAR(100)  
   	                            ,@BlnOfferItem FLOAT  
   	                            ,@BalQty FLOAT  
   	                            ,@GrossAmount FLOAT  
   	                            ,@iFloodCessPer FLOAT  
   	                            ,@iFloodCessAmt FLOAT  
   	                            ,@Srate1 FLOAT  
   	                            ,@Srate2 FLOAT  
   	                            ,@Srate3 FLOAT  
   	                            ,@Srate4 FLOAT  
   	                            ,@Srate5 FLOAT  
   	                            ,@Costrate FLOAT  
   	                            ,@CostValue FLOAT  
   	                            ,@Profit FLOAT  
   	                            ,@ProfitPer FLOAT  
   	                            ,@DiscMode NUMERIC(18, 0)  
   	                            ,@Srate1Per FLOAT  
   	                            ,@Srate2Per FLOAT  
   	                            ,@Srate3Per FLOAT  
   	                            ,@Srate4Per FLOAT  
   	                            ,@Srate5Per FLOAT  
   	                            ,@Action INT = 0  
   	                            )  
                               AS  
                               BEGIN  
   	                            DECLARE @RetResult INT  
   	                            DECLARE @RetID INT  
   	                            DECLARE @VchType VARCHAR(50)  
   	                            DECLARE @VchTypeID NUMERIC(18, 0)  
   	                            DECLARE @BatchMode VARCHAR(50)  
   	                            DECLARE @VchDate DATETIME  
   	                            DECLARE @CCID NUMERIC(18, 0)  
   	                            DECLARE @TenantID NUMERIC(18, 0)  
   	                            DECLARE @BarCode_out VARCHAR(2000)  
   	                            DECLARE @VchParentID NUMERIC(18, 0)  
   	                            DECLARE @FreeQty NUMERIC(18, 5)  
   	                            BEGIN TRY  
   		                            --BEGIN TRANSACTION;  

   		                            SET @FreeQty = @Qty + @Free  

   		                            SELECT @VchType = VchType  
   			                            ,@VchTypeID = VchTypeID  
   			                            ,@VchDate = InvDate  
   			                            ,@CCID = CCID  
   			                            ,@TenantID = TenantID  
   		                            FROM tblPurchase  
   		                            WHERE InvId = @InvID  

   		                            SELECT @BatchMode = BatchMode  
   		                            FROM tblItemMaster  
   		                            WHERE ItemID = @ItemId  
   		                            SELECT @VchParentID = ParentID  
   		                            FROM tblVchType  
   		                            WHERE VchTypeID = @VchTypeID  

   		                            IF @Action = 0  
   		                            BEGIN  
   			                            IF @VchParentID = 2 
   			                            BEGIN  
   				                            EXEC UspTransStockUpdate @ItemId  
   					                            ,@BatchCode  
   					                            ,@BatchUnique  
   					                            ,@FreeQty  
   					                            ,@MRP  
   					                            ,@CRateWithTax  
   					                            ,@CRate  
   					                            ,@Prate  
   					                            ,@Prate  
   					                            ,@TaxPer  
   					                            ,@Srate1  
   					                            ,@Srate2  
   					                            ,@Srate3  
   					                            ,@Srate4  
   					                            ,@Srate5  
   					                            ,@BatchMode  
   					                            ,@VchType  
   					                            ,@VchDate  
   					                            ,@Expiry  
   					                            ,'STOCKADD'  
   					                            ,@InvID  
   					                            ,@VchTypeID  
   					                            ,@CCID  
   					                            ,@TenantID  
   					                            ,@Prate  
   					                            ,@BarCode_out OUTPUT  

				                            IF CHARINDEX('-1|ERROR|CRITICAL|', @BarCode_out) > 0  
   				                            BEGIN
					                            SELECT - 1 AS SqlSpResult  
   						                            ,@RetID AS PID  
   						                            ,-1 AS ErrorNumber  
   						                            ,'' AS ErrorState  
   						                            ,'' AS ErrorSeverity  
   						                            ,'' AS ErrorProcedure  
   						                            ,-1 AS ErrorLine  
   						                            ,'The barcode is assigned with another MRP. Multiple MRP not allowed for <Auto Barcode>.' AS ErrorMessage;  
						
						                            RETURN;
				                            END

   				                            IF CHARINDEX('@', @BarCode_out) > 0  
   				                            BEGIN  
   					                            SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))  
   				                            END  
   			                            END  
   			                            ELSE IF @VchParentID = 1005 
   			                            BEGIN  
				                            IF @FreeQty > 0
   				                            BEGIN
					                            EXEC UspTransStockUpdate @ItemId  
   						                            ,@BatchCode  
   						                            ,@BatchUnique  
   						                            ,@FreeQty  
   						                            ,@MRP  
   						                            ,@CRateWithTax  
   						                            ,@CRate  
   						                            ,@Prate  
   						                            ,@Prate  
   						                            ,@TaxPer  
   						                            ,@Srate1  
   						                            ,@Srate2  
   						                            ,@Srate3  
   						                            ,@Srate4  
   						                            ,@Srate5  
   						                            ,@BatchMode  
   						                            ,@VchType  
   						                            ,@VchDate  
   						                            ,@Expiry  
   						                            ,'STOCKADD'  
   						                            ,@InvID  
   						                            ,@VchTypeID  
   						                            ,@CCID  
   						                            ,@TenantID  
   						                            ,@Prate  
   						                            ,@BarCode_out OUTPUT  
				                            END
				                            ELSE
				                            BEGIN
					                            EXEC UspTransStockUpdate @ItemId  
   						                            ,@BatchCode  
   						                            ,@BatchUnique  
   						                            ,@FreeQty  
   						                            ,@MRP  
   						                            ,@CRateWithTax  
   						                            ,@CRate  
   						                            ,@Prate  
   						                            ,@Prate  
   						                            ,@TaxPer  
   						                            ,@Srate1  
   						                            ,@Srate2  
   						                            ,@Srate3  
   						                            ,@Srate4  
   						                            ,@Srate5  
   						                            ,@BatchMode  
   						                            ,@VchType  
   						                            ,@VchDate  
   						                            ,@Expiry  
   						                            ,'STOCKLESS'  
   						                            ,@InvID  
   						                            ,@VchTypeID  
   						                            ,@CCID  
   						                            ,@TenantID  
   						                            ,@Prate  
   						                            ,@BarCode_out OUTPUT  
				                            END

				                            IF CHARINDEX('-1|ERROR|CRITICAL|', @BarCode_out) > 0  
   				                            BEGIN
					                            SELECT - 1 AS SqlSpResult  
   						                            ,@RetID AS PID  
   						                            ,-1 AS ErrorNumber  
   						                            ,'' AS ErrorState  
   						                            ,'' AS ErrorSeverity  
   						                            ,'' AS ErrorProcedure  
   						                            ,-1 AS ErrorLine  
   						                            ,'The barcode is assigned with another MRP. Multiple MRP not allowed for <Auto Barcode>.' AS ErrorMessage;  
						
						                            RETURN;
				                            END

   				                            IF CHARINDEX('@', @BarCode_out) > 0  
   				                            BEGIN  
   					                            SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))  
   				                            END  
   			                            END  
   			                            ELSE IF @VchParentID = 4  
   			                            BEGIN  
   				                            EXEC UspTransStockUpdate @ItemId  
   					                            ,@BatchCode  
   					                            ,@BatchUnique  
   					                            ,@FreeQty  
   					                            ,@MRP  
   					                            ,@CRateWithTax  
   					                            ,@CRate  
   					                            ,@Prate  
   					                            ,@Prate  
   					                            ,@TaxPer  
   					                            ,@Srate1  
   					                            ,@Srate2  
   					                            ,@Srate3  
   					                            ,@Srate4  
   					                            ,@Srate5  
   					                            ,@BatchMode  
   					                            ,@VchType  
   					                            ,@VchDate  
   					                            ,@Expiry  
   					                            ,'STOCKLESS'  
   					                            ,@InvID  
   					                            ,@VchTypeID  
   					                            ,@CCID  
   					                            ,@TenantID  
   					                            ,@Prate  
   					                            ,@BarCode_out OUTPUT  

				                            IF CHARINDEX('-1|ERROR|CRITICAL|', @BarCode_out) > 0  
   				                            BEGIN
					                            SELECT - 1 AS SqlSpResult  
   						                            ,@RetID AS PID  
   						                            ,-1 AS ErrorNumber  
   						                            ,'' AS ErrorState  
   						                            ,'' AS ErrorSeverity  
   						                            ,'' AS ErrorProcedure  
   						                            ,-1 AS ErrorLine  
   						                            ,'The barcode is assigned with another MRP. Multiple MRP not allowed for <Auto Barcode>.' AS ErrorMessage;  
						
						                            RETURN;
				                            END

   				                            IF CHARINDEX('@', @BarCode_out) > 0  
   				                            BEGIN  
   					                            SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))  
   				                            END  
   			                            END  
   			                            ELSE IF @VchParentID = 6  
   			                            BEGIN  
   				                            EXEC UspTransStockUpdate @ItemId  
   					                            ,@BatchCode  
   					                            ,@BatchUnique  
   					                            ,@FreeQty  
   					                            ,@MRP  
   					                            ,@CRateWithTax  
   					                            ,@CRate  
   					                            ,@Prate  
   					                            ,@Prate  
   					                            ,@TaxPer  
   					                            ,@Srate1  
   					                            ,@Srate2  
   					                            ,@Srate3  
   					                            ,@Srate4  
   					                            ,@Srate5  
   					                            ,@BatchMode  
   					                            ,@VchType  
   					                            ,@VchDate  
   					                            ,@Expiry  
   					                            ,'STOCKADD'  
   					                            ,@InvID  
   					                            ,@VchTypeID  
   					                            ,@CCID  
   					                            ,@TenantID  
   					                            ,@Prate  
   					                            ,@BarCode_out OUTPUT  

				                            IF CHARINDEX('-1|ERROR|CRITICAL|', @BarCode_out) > 0  
   				                            BEGIN
					                            SELECT - 1 AS SqlSpResult  
   						                            ,@RetID AS PID  
   						                            ,-1 AS ErrorNumber  
   						                            ,'CRITICAL' AS ErrorState  
   						                            ,'CRITICAL' AS ErrorSeverity  
   						                            ,'usptransstockupdate VIA UspPurchaseItemInsert' AS ErrorProcedure  
   						                            ,-1 AS ErrorLine  
   						                            ,@BarCode_out AS ErrorMessage;  
						
						                            RETURN;
				                            END

   				                            IF CHARINDEX('@', @BarCode_out) > 0  
   				                            BEGIN  
   					                            SET @BatchCode = SUBSTRING(@BarCode_out, 0, CHARINDEX('@', @BarCode_out))  
   				                            END  
   			                            END  
   			                            ELSE  
   			                            BEGIN  
   				                            SET @BatchCode = @BarCode_out  
   			                            END  
   			                            INSERT INTO tblPurchaseItem (  
   				                            InvID  
   				                            ,ItemId  
   				                            ,Qty  
   				                            ,Rate  
   				                            ,UnitId  
   				                            ,Batch  
   				                            ,TaxPer  
   				                            ,TaxAmount  
   				                            ,Discount  
   				                            ,MRP  
   				                            ,SlNo  
   				                            ,Prate  
   				                            ,Free  
   				                            ,SerialNos  
   				                            ,ItemDiscount  
   				                            ,BatchCode  
   				                            ,iCessOnTax  
   				                            ,blnCessOnTax  
   				                            ,Expiry  
   				                            ,ItemDiscountPer  
   				                            ,RateInclusive  
   				                            ,ITaxableAmount  
   				                            ,INetAmount  
   				                            ,CGSTTaxPer  
   				                            ,CGSTTaxAmt  
   				                            ,SGSTTaxPer  
   				                            ,SGSTTaxAmt  
   				                            ,IGSTTaxPer  
   				                            ,IGSTTaxAmt  
   				                            ,iRateDiscPer  
   				                            ,iRateDiscount  
   				                            ,BatchUnique  
   				                            ,blnQtyIN  
   				                            ,CRate  
				                            ,CRateWithTax
   				                            ,Unit  
   				                            ,ItemStockID  
   				                            ,IcessPercent  
   				                            ,IcessAmt  
   				                            ,IQtyCompCessPer  
   				                            ,IQtyCompCessAmt  
   				                            ,StockMRP  
   				                            ,BaseCRate  
   				                            ,InonTaxableAmount  
   				                            ,IAgentCommPercent  
   				                            ,BlnDelete  
   				                            ,StrOfferDetails  
   				                            ,BlnOfferItem  
   				                            ,BalQty  
   				                            ,GrossAmount  
   				                            ,iFloodCessPer  
   				                            ,iFloodCessAmt  
   				                            ,Srate1  
   				                            ,Srate2  
   				                            ,Srate3  
   				                            ,Srate4  
   				                            ,Srate5  
   				                            ,Costrate  
   				                            ,CostValue  
   				                            ,Profit  
   				                            ,ProfitPer  
   				                            ,DiscMode  
   				                            ,Srate1Per  
   				                            ,Srate2Per  
   				                            ,Srate3Per  
   				                            ,Srate4Per  
   				                            ,Srate5Per  
   				                            )  
   			                            VALUES (  
   				                            @InvID  
   				                            ,@ItemId  
   				                            ,@Qty  
   				                            ,@Rate  
   				                            ,@UnitId  
   				                            ,@Batch  
   				                            ,@TaxPer  
   				                            ,@TaxAmount  
   				                            ,@Discount  
   				                            ,@MRP  
   				                            ,@SlNo  
   				                            ,@Prate  
   				                            ,@Free  
   				                            ,@SerialNos  
   				                            ,@ItemDiscount  
   				                            ,@BatchCode  
   				                            ,@iCessOnTax  
   				                            ,@blnCessOnTax  
   				                            ,@Expiry  
   				                            ,@ItemDiscountPer  
   				                            ,@RateInclusive  
   				                            ,@ITaxableAmount  
   				                            ,@INetAmount  
   				                            ,@CGSTTaxPer  
   				                            ,@CGSTTaxAmt  
   				                            ,@SGSTTaxPer  
   				                            ,@SGSTTaxAmt  
   				                            ,@IGSTTaxPer  
   				                            ,@IGSTTaxAmt  
   				                            ,@iRateDiscPer  
   				                            ,@iRateDiscount  
   				                            ,@BarCode_out  
   				                            ,@blnQtyIN  
   				                            ,@CRate  
				                            ,@CRateWithTax
   				                            ,@Unit  
   				                            ,@ItemStockID  
   				                            ,@IcessPercent  
   				                            ,@IcessAmt  
   				                            ,@IQtyCompCessPer  
   				                            ,@IQtyCompCessAmt  
   				                            ,@StockMRP  
   				                            ,@BaseCRate  
   				                            ,@InonTaxableAmount  
   				                            ,@IAgentCommPercent  
   				                            ,@BlnDelete  
   				                            ,@StrOfferDetails  
   				                            ,@BlnOfferItem  
   				                            ,@BalQty  
   				                            ,@GrossAmount  
   				                            ,@iFloodCessPer  
   				                            ,@iFloodCessAmt  
   				                            ,@Srate1  
   				                            ,@Srate2  
   				                            ,@Srate3  
   				                            ,@Srate4  
   				                            ,@Srate5  
   				                            ,@Costrate  
   				                            ,@CostValue  
   				                            ,@Profit  
   				                            ,@ProfitPer  
   				                            ,@DiscMode  
   				                            ,@Srate1Per  
   				                            ,@Srate2Per  
   				                            ,@Srate3Per  
   				                            ,@Srate4Per  
   				                            ,@Srate5Per  
   				                            )  
   			                            SET @RetResult = 1;  
   		                            END  
   		                            ELSE IF @Action = 2  
   		                            BEGIN  
   			                            EXEC UspTransStockUpdate @ItemId  
   				                            ,@BatchCode  
   				                            ,@BatchUnique  
   				                            ,@FreeQty  
   				                            ,@MRP  
   				                            ,@CRateWithTax  
   				                            ,@CRate  
   				                            ,@Prate  
   				                            ,@Prate  
   				                            ,@TaxPer  
   				                            ,@Srate1  
   				                            ,@Srate2  
   				                            ,@Srate3  
   				                            ,@Srate4  
   				                            ,@Srate5  
   				                            ,@BatchMode  
   				                            ,@VchType  
   				                            ,@VchDate  
   				                            ,@Expiry  
   				                            ,'STOCKDEL'  
   				                            ,@InvID  
   				                            ,@VchTypeID  
   				                            ,@CCID  
   				                            ,@TenantID  
   				                            ,@Prate  
   				                            ,@BarCode_out OUTPUT  

			                            DELETE  
   			                            FROM tblPurchaseItem  
   			                            WHERE InvID = @InvID  

   			                            SET @RetResult = 0;  
   		                            END  
   		                            ELSE IF @Action = 3  
   		                            BEGIN  
   			                            EXEC UspTransStockUpdate @ItemId  
   				                            ,@BatchCode  
   				                            ,@BatchUnique  
   				                            ,@FreeQty  
   				                            ,@MRP  
   				                            ,@CRateWithTax  
   				                            ,@CRate  
   				                            ,@Prate  
   				                            ,@Prate  
   				                            ,@TaxPer  
   				                            ,@Srate1  
   				                            ,@Srate2  
   				                            ,@Srate3  
   				                            ,@Srate4  
   				                            ,@Srate5  
   				                            ,@BatchMode  
   				                            ,@VchType  
   				                            ,@VchDate  
   				                            ,@Expiry  
   				                            ,'STOCKDEL'  
   				                            ,@InvID  
   				                            ,@VchTypeID  
   				                            ,@CCID  
   				                            ,@TenantID  
   				                            ,@Prate  
   				                            ,@BarCode_out OUTPUT  
   			                            SET @RetResult = 0;  
   		                            END  
   		                            --COMMIT TRANSACTION;  
   		                            SELECT @RetResult AS SqlSpResult  
   			                            ,@RetID AS PID  
   	                            END TRY  
   	                            BEGIN CATCH  
   		                            --ROLLBACK;  
   		                            SELECT - 1 AS SqlSpResult  
   			                            ,@RetID AS PID  
   			                            ,ERROR_NUMBER() AS ErrorNumber  
   			                            ,ERROR_STATE() AS ErrorState  
   			                            ,ERROR_SEVERITY() AS ErrorSeverity  
   			                            ,ERROR_PROCEDURE() AS ErrorProcedure  
   			                            ,ERROR_LINE() AS ErrorLine  
   			                            ,ERROR_MESSAGE() AS ErrorMessage;  
   	                            END CATCH;  
                               END ";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }


            try
            {
                sQuery = @"DROP PROCEDURE UspStockInsert";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspStockInsert] (    
      	                        @StockID NUMERIC(18, 0)    
      	                        ,@TenantID NUMERIC(18, 0)    
      	                        ,@CCID NUMERIC(18, 0)    
      	                        ,@BatchCode VARCHAR(100)    
      	                        ,@BatchUnique VARCHAR(50)    
      	                        ,@BatchID NUMERIC(18, 0)    
      	                        ,@MRP NUMERIC(18, 5)    
      	                        ,@ExpiryDate DATE    
      	                        ,@CostRateInc DECIMAL(18, 2)    
      	                        ,@CostRateExcl DECIMAL(18, 2)    
      	                        ,@PRateExcl DECIMAL(18, 2)    
      	                        ,@PrateInc DECIMAL(18, 2)    
      	                        ,@TaxPer DECIMAL(18, 2)    
      	                        ,@SRate1 DECIMAL(18, 2)    
      	                        ,@SRate2 DECIMAL(18, 2)    
      	                        ,@SRate3 DECIMAL(18, 2)    
      	                        ,@SRate4 DECIMAL(18, 2)    
      	                        ,@SRate5 DECIMAL(18, 2)    
      	                        ,@QOH DECIMAL(18, 5)      
      	                        ,@LastInvDate DATE    
      	                        ,@LastInvNo VARCHAR(50)    
      	                        ,@LastSupplierID NUMERIC(18, 0)    
      	                        ,@Action INT = 0    
      	                        ,@ItemID NUMERIC(18, 0)    
      	                        ,@BatchMode VARCHAR(100)    
      	                        ,@PRate NUMERIC(18, 5)    
		                        ,@AutoBatchID NUMERIC(18, 0)    
      	                        )    
                              AS    
                              BEGIN    
      	                        DECLARE @RetResult INT    
      	                        DECLARE @TransType CHAR(1)    
      	                        DECLARE @blnExpiry NUMERIC(18, 0)    
      	                        BEGIN TRY    
									SET @AutoBatchID = ISNULL(@AutoBatchID,0)
      		                        --BEGIN TRANSACTION;    
      		                        IF @BatchMode = 0    
      		                        BEGIN   
                                        SELECT @blnExpiry = ISNULL(blnExpiry, 0) FROM tblItemMaster WHERE ItemID = @ItemID    
      			                        IF @BatchCode = '' 
                                            SELECT @BatchCode = ItemCode, @blnExpiry = ISNULL(blnExpiry, 0) FROM tblItemMaster WHERE ItemID = @ItemID    
      		                        END    
      		                        IF @Action = 0    
      		                        BEGIN    
      			                        IF @BatchMode = 0    
      			                        BEGIN /*None*/    
      				                        INSERT INTO tblStock (    
      					                        StockID    
      					                        ,TenantID    
      					                        ,CCID    
      					                        ,BatchCode    
      					                        ,BatchUnique    
      					                        ,BatchID    
						                        ,AutoBatchID
      					                        ,MRP    
      					                        ,ExpiryDate    
      					                        ,CostRateInc    
      					                        ,CostRateExcl    
      					                        ,PRateExcl    
      					                        ,PrateInc    
      					                        ,TaxPer    
      					                        ,SRate1    
      					                        ,SRate2    
      					                        ,SRate3    
      					                        ,SRate4    
      					                        ,SRate5    
      					                        ,QOH    
      					                        ,LastInvDate    
      					                        ,LastInvNo    
      					                        ,LastSupplierID    
      					                        ,ItemID    
      					                        ,PRate    
      					                        )    
      				                        VALUES (    
      					                        @StockID    
      					                        ,@TenantID    
      					                        ,@CCID    
      					                        ,@BatchCode    
      					                        ,@BatchUnique    
      					                        ,@BatchID
						                        ,@AutoBatchID
      					                        ,@MRP    
      					                        ,@ExpiryDate    
      					                        ,@CostRateInc    
      					                        ,@CostRateExcl    
      					                        ,@PRateExcl    
      					                        ,@PrateInc    
      					                        ,@TaxPer    
      					                        ,@SRate1    
      					                        ,@SRate2    
      					                        ,@SRate3    
      					                        ,@SRate4    
      					                        ,@SRate5    
      					                        ,ABS(@QOH)    
      					                        ,@LastInvDate    
      					                        ,@LastInvNo    
      					                        ,@LastSupplierID    
      					                        ,@ItemID    
      					                        ,@PRate    
      					                        )    
      				                        SET @RetResult = 1;    
      				                        SET @TransType = 'S';    
      			                        END    
      			                        ELSE IF @BatchMode = 1    
      			                        BEGIN    
      				                        INSERT INTO tblStock (    
      					                        StockID    
      					                        ,TenantID    
      					                        ,CCID    
      					                        ,BatchCode    
      					                        ,BatchUnique    
      					                        ,BatchID    
						                        ,AutoBatchID
      					                        ,MRP    
      					                        ,ExpiryDate    
      					                        ,CostRateInc    
      					                        ,CostRateExcl    
      					                        ,PRateExcl    
      					                        ,PrateInc    
      					                        ,TaxPer    
      					                        ,SRate1    
      					                        ,SRate2    
      					                        ,SRate3    
      					                        ,SRate4    
      					                        ,SRate5    
      					                        ,QOH    
      					                        ,LastInvDate    
      					                        ,LastInvNo    
      					                        ,LastSupplierID    
      					                        ,ItemID    
      					                        ,PRate    
      					                        )    
      				                        VALUES (    
      					                        @StockID    
      					                        ,@TenantID    
      					                        ,@CCID    
      					                        ,@BatchCode    
      					                        ,@BatchUnique    
      					                        ,@BatchID    
						                        ,@AutoBatchID
      					                        ,@MRP    
      					                        ,@ExpiryDate    
      					                        ,@CostRateInc    
      					                        ,@CostRateExcl    
      					                        ,@PRateExcl    
      					                        ,@PrateInc    
      					                        ,@TaxPer    
      					                        ,@SRate1    
      					                        ,@SRate2    
      					                        ,@SRate3    
      					                        ,@SRate4    
      					                        ,@SRate5    
      					                        ,ABS(@QOH)    
      					                        ,@LastInvDate    
      					                        ,@LastInvNo    
      					                        ,@LastSupplierID    
      					                        ,@ItemID    
      					                        ,@PRate    
      					                        )    
      				                        SET @RetResult = 1;    
      				                        SET @TransType = 'S';    
      			                        END    
      			                        ELSE IF @BatchMode = 2    
      				                        AND @BatchCode <> ''    
      			                        BEGIN /*Auto*/    
      				                        INSERT INTO tblStock (    
      					                        StockID    
      					                        ,TenantID    
      					                        ,CCID    
      					                        ,BatchCode    
      					                        ,BatchUnique    
      					                        ,BatchID    
						                        ,AutoBatchID
      					                        ,MRP    
      					                        ,ExpiryDate    
      					                        ,CostRateInc    
      					                        ,CostRateExcl    
      					                        ,PRateExcl    
      					                        ,PrateInc    
      					                        ,TaxPer    
      					                        ,SRate1    
      					                        ,SRate2    
      					                        ,SRate3    
      					                        ,SRate4    
      					                        ,SRate5    
      					                        ,QOH    
      					                        ,LastInvDate    
      					                        ,LastInvNo    
      					                        ,LastSupplierID    
      					                        ,ItemID    
      					                        ,PRate    
      					                        )    
      				                        VALUES (    
      					                        @StockID    
      					                        ,@TenantID    
      					                        ,@CCID    
      					                        ,@BatchCode    
      					                        ,@BatchUnique    
      					                        ,@BatchID    
						                        ,@AutoBatchID
      					                        ,@MRP    
      					                        ,@ExpiryDate    
      					                        ,@CostRateInc    
      					                        ,@CostRateExcl    
      					                        ,@PRateExcl    
      					                        ,@PrateInc    
      					                        ,@TaxPer    
      					                        ,@SRate1    
      					                        ,@SRate2    
      					                        ,@SRate3    
      					                        ,@SRate4    
      					                        ,@SRate5    
      					                        ,ABS(@QOH)    
      					                        ,@LastInvDate    
      					                        ,@LastInvNo    
      					                        ,@LastSupplierID    
      					                        ,@ItemID    
      					                        ,@PRate    
      					                        )    
      				                        SET @RetResult = 1;    
      				                        SET @TransType = 'S';    
      			                        END    
      			                        ELSE IF @BatchMode = 3    
      			                        BEGIN    
      				                        INSERT INTO tblStock (    
      					                        StockID    
      					                        ,TenantID    
      					                        ,CCID    
      					                        ,BatchCode    
      					                        ,BatchUnique    
      					                        ,BatchID    
						                        ,AutoBatchID
      					                        ,MRP    
      					                        ,ExpiryDate    
      					                        ,CostRateInc    
      					                        ,CostRateExcl    
      					                        ,PRateExcl    
      					                        ,PrateInc    
      					                        ,TaxPer    
      					                        ,SRate1    
      					                        ,SRate2    
      					                        ,SRate3    
      					                        ,SRate4    
      					                        ,SRate5    
      					                        ,QOH    
      					                        ,LastInvDate    
      					                        ,LastInvNo    
      					                        ,LastSupplierID    
      					                        ,ItemID    
      					                        ,PRate    
      					                        )    
      				                        VALUES (    
      					                        @StockID    
      					                        ,@TenantID    
      					                        ,@CCID    
      					                        ,@BatchCode    
      					                        ,@BatchUnique    
      					                        ,@BatchID    
						                        ,@AutoBatchID
      					                        ,@MRP    
      					                        ,@ExpiryDate    
      					                        ,@CostRateInc    
      					                        ,@CostRateExcl    
      					                        ,@PRateExcl    
      					                        ,@PrateInc    
      					                        ,@TaxPer    
      					                        ,@SRate1    
      					                        ,@SRate2    
      					                        ,@SRate3    
      					                        ,@SRate4    
      					                        ,@SRate5    
      					                        ,ABS(@QOH)    
      					                        ,@LastInvDate    
      					                        ,@LastInvNo    
      					                        ,@LastSupplierID    
      					                        ,@ItemID    
      					                        ,@PRate    
      					                        )    
      				                        SET @RetResult = 1;    
      				                        SET @TransType = 'S';    
      			                        END    
      		                        END    
      		                        IF @Action = 1    
      		                        BEGIN    
										IF @BatchMode = 3 
										BEGIN
      										UPDATE tblStock    
      										SET BatchID = @BatchID    , AutoBatchID = @AutoBatchID 
      											,MRP = @MRP    
      											,ExpiryDate = @ExpiryDate    
      											,CostRateInc = @CostRateInc    
      											,CostRateExcl = @CostRateExcl    
      											,PRateExcl = @PRateExcl    
      											,PrateInc = @PrateInc    
      											,TaxPer = @TaxPer    
      											,QOH = QOH + @QOH    
      											,LastInvDate = @LastInvDate    
      											,LastInvNo = @LastInvNo    
      											,LastSupplierID = @LastSupplierID    
      											,PRate = @PRate    
      										WHERE ItemID = @ItemID    
      											AND CCID = @CCID    
      											AND BatchCode = @BatchCode    
      											AND BatchUnique = @BatchUnique    
      											AND TenantID = @TenantID    
										END
										ELSE
										BEGIN
      										UPDATE tblStock    
      										SET BatchID = @BatchID    , AutoBatchID = @AutoBatchID 
      											,MRP = @MRP    
      											,ExpiryDate = @ExpiryDate    
      											,CostRateInc = @CostRateInc    
      											,CostRateExcl = @CostRateExcl    
      											,PRateExcl = @PRateExcl    
      											,PrateInc = @PrateInc    
      											,TaxPer = @TaxPer    
      											,SRate1 = @SRate1    
      											,SRate2 = @SRate2    
      											,SRate3 = @SRate3    
      											,SRate4 = @SRate4    
      											,SRate5 = @SRate5    
      											,QOH = QOH + @QOH    
      											,LastInvDate = @LastInvDate    
      											,LastInvNo = @LastInvNo    
      											,LastSupplierID = @LastSupplierID    
      											,PRate = @PRate    
      										WHERE ItemID = @ItemID    
      											AND CCID = @CCID    
      											AND BatchCode = @BatchCode    
      											AND BatchUnique = @BatchUnique    
      											AND TenantID = @TenantID    
										END
				                        IF @@ROWCOUNT = 0 
				                        BEGIN
      				                        INSERT INTO tblStock (    
      					                        StockID    
      					                        ,TenantID    
      					                        ,CCID    
      					                        ,BatchCode    
      					                        ,BatchUnique    
      					                        ,BatchID    
						                        ,AutoBatchID
      					                        ,MRP    
      					                        ,ExpiryDate    
      					                        ,CostRateInc    
      					                        ,CostRateExcl    
      					                        ,PRateExcl    
      					                        ,PrateInc    
      					                        ,TaxPer    
      					                        ,SRate1    
      					                        ,SRate2    
      					                        ,SRate3    
      					                        ,SRate4    
      					                        ,SRate5    
      					                        ,QOH    
      					                        ,LastInvDate    
      					                        ,LastInvNo    
      					                        ,LastSupplierID    
      					                        ,ItemID    
      					                        ,PRate    
      					                        )    
      				                        VALUES (    
      					                        @StockID    
      					                        ,@TenantID    
      					                        ,@CCID    
      					                        ,@BatchCode    
      					                        ,@BatchUnique    
      					                        ,@BatchID    
						                        ,@AutoBatchID
      					                        ,@MRP    
      					                        ,@ExpiryDate    
      					                        ,@CostRateInc    
      					                        ,@CostRateExcl    
      					                        ,@PRateExcl    
      					                        ,@PrateInc    
      					                        ,@TaxPer    
      					                        ,@SRate1    
      					                        ,@SRate2    
      					                        ,@SRate3    
      					                        ,@SRate4    
      					                        ,@SRate5    
      					                        ,ABS(@QOH)    
      					                        ,@LastInvDate    
      					                        ,@LastInvNo    
      					                        ,@LastSupplierID    
      					                        ,@ItemID    
      					                        ,@PRate    
      					                        )    
				                        END

      			                        SET @RetResult = 1;    
      			                        SET @TransType = 'E';    
      		                        END    
      		                        IF @Action = 2    
      		                        BEGIN    
										IF @BatchMode = 3 
										BEGIN
      										UPDATE tblStock    
      										SET CCID = @CCID    
      											,BatchCode = @BatchCode    
      											,BatchUnique = @BatchUnique    
      											,BatchID = @BatchID    
												,AutoBatchID = @AutoBatchID
      											,MRP = @MRP    
      											,ExpiryDate = @ExpiryDate    
      											,CostRateInc = @CostRateInc    
      											,CostRateExcl = @CostRateExcl    
      											,PRateExcl = @PRateExcl    
      											,PrateInc = @PrateInc    
      											,TaxPer = @TaxPer    
      											,QOH = QOH + @QOH    
      											,LastInvDate = @LastInvDate    
      											,LastInvNo = @LastInvNo    
      											,LastSupplierID = @LastSupplierID    
      											,PRate = @PRate    
      										WHERE ItemID = @ItemID    
      											AND CCID = @CCID    
      											AND BatchCode = @BatchCode    
      											AND BatchUnique = @BatchUnique    
      											AND TenantID = @TenantID    
      			                        END
										ELSE
										BEGIN
      										UPDATE tblStock    
      										SET CCID = @CCID    
      											,BatchCode = @BatchCode    
      											,BatchUnique = @BatchUnique    
      											,BatchID = @BatchID    
												,AutoBatchID = @AutoBatchID
      											,MRP = @MRP    
      											,ExpiryDate = @ExpiryDate    
      											,CostRateInc = @CostRateInc    
      											,CostRateExcl = @CostRateExcl    
      											,PRateExcl = @PRateExcl    
      											,PrateInc = @PrateInc    
      											,TaxPer = @TaxPer    
      											,SRate1 = @SRate1    
      											,SRate2 = @SRate2    
      											,SRate3 = @SRate3    
      											,SRate4 = @SRate4    
      											,SRate5 = @SRate5    
      											,QOH = QOH + @QOH    
      											,LastInvDate = @LastInvDate    
      											,LastInvNo = @LastInvNo    
      											,LastSupplierID = @LastSupplierID    
      											,PRate = @PRate    
      										WHERE ItemID = @ItemID    
      											AND CCID = @CCID    
      											AND BatchCode = @BatchCode    
      											AND BatchUnique = @BatchUnique    
      											AND TenantID = @TenantID    
										END

										SET @RetResult = 0;    
      			                        SET @TransType = 'D';    
      		                        END    
      		                        --COMMIT TRANSACTION;    
      		                        SELECT @RetResult AS SqlSpResult    
      			                        ,@StockID AS TransID    
      			                        ,@TransType AS TransactType    
      	                        END TRY    
      	                        BEGIN CATCH    
      		                        --ROLLBACK;    
      		                        SELECT - 1 AS SqlSpResult    
      			                        ,ERROR_NUMBER() AS ErrorNumber    
      			                        ,ERROR_STATE() AS ErrorState    
      			                        ,ERROR_SEVERITY() AS ErrorSeverity    
      			                        ,ERROR_PROCEDURE() AS ErrorProcedure    
      			                        ,ERROR_LINE() AS ErrorLine    
      			                        ,ERROR_MESSAGE() AS ErrorMessage;    
      	                        END CATCH;    
                              END  ";


                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }


            try
            {
                sQuery = @"drop PROCEDURE UspTransStockUpdateFromItem";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspTransStockUpdateFromItem] (@ItemID       NUMERIC(18, 0),
	                    @BatchCode    VARCHAR(50),	@BatchUniq    VARCHAR(50),	@Qty          NUMERIC(18, 5),	@MRP          NUMERIC(18, 5),	@CostRateInc  NUMERIC(18, 5),	@CostRateExcl NUMERIC(18, 5),
	                    @PRateExcl    NUMERIC(18, 5),	@PrateInc     NUMERIC(18, 5),	@TaxPer       NUMERIC(18, 5),	@SRate1       NUMERIC(18, 5),	@SRate2       NUMERIC(18, 5),	@SRate3       NUMERIC(18, 5),
	                    @SRate4       NUMERIC(18, 5),	@SRate5       NUMERIC(18, 5),	@BatchMode    INT,	@VchType      VARCHAR(100),	@VchDate      DATETIME,	@ExpDt        DATETIME,	@Action       VARCHAR(20),
	                    @RefID        NUMERIC(18, 0),	@VchTypeID    NUMERIC(18, 0),	@CCID         NUMERIC(18, 0),	@TenantID     NUMERIC(18, 0),	@PRate        NUMERIC(18, 5))
                    AS
                      BEGIN
                          DECLARE @BatchID NUMERIC(18, 0)
	                      DECLARE @AutoBatchID NUMERIC(18,0)
                          DECLARE @StockID NUMERIC(18, 0)
                          DECLARE @LastInvDt DATETIME = Getdate()
                          DECLARE @STOCKHISID NUMERIC(18, 0)
                          DECLARE @PRFXBATCH VARCHAR(10)
                          DECLARE @Stock NUMERIC(18, 5)
                          DECLARE @BarCode VARCHAR(50)
                          DECLARE @BarUniq VARCHAR(100)
                          DECLARE @CalcQOH NUMERIC(18, 5)
                          DECLARE @BLNADVANCED INT
                          DECLARE @blnExpiry BIT

                          SET @BarCode = @BatchCode

                          SELECT @StockID = Isnull(Max(stockid) + 1, 0)
                          FROM   tblstock
                          WHERE  tenantid = @TenantID

                          SELECT @BatchID = Isnull(Max(batchid) + 1, 0)
                          FROM   tblstock
                          WHERE  tenantid = @TenantID

                          SELECT @STOCKHISID = Isnull(Max(stockhisid) + 1, 0)
                          FROM   tblstockhistory
                          WHERE  tenantid = @TenantID

                          SELECT @BLNADVANCED = Isnull(valuename, 0)
                          FROM   [tblappsettings]
                          WHERE  Upper(Ltrim(Rtrim(keyname))) = 'BLNADVANCED'

                          SELECT @Stock = Isnull(qoh, 0)
                          FROM   tblstock
                          WHERE  itemid = @ItemID             AND batchcode = @BatchCode             AND tenantid = @TenantID

                          SELECT @blnExpiry = Isnull(blnexpiry, 0)
                          FROM   tblitemmaster
                          WHERE  itemid = @ItemID

                          IF @StockID = 0
                            BEGIN
                                SET @StockID = 1
                            END

                          IF @STOCKHISID = 0
                            BEGIN
                                SET @STOCKHISID = 1
                            END

                          IF @BatchID = 0
                            BEGIN
                                SET @BatchID = 1
                            END

		                    SET @AutoBatchID = 0

                          IF @Action = 'STOCKADD'
                            BEGIN
                                IF @BatchCode = '<Auto Barcode>'
                                  BEGIN
                                      DECLARE @Prefix VARCHAR(50)
                                      DECLARE @BatchPrefix VARCHAR(50)

				                      SELECT @AutoBatchID = Isnull( Max(AutoBatchID), 0 ) + 1  FROM   tblstock  WHERE  tenantid = @TenantID 
				                      IF @AutoBatchID = 0
					                    BEGIN
						                    SET @AutoBatchID = 1
					                    END

                                      IF @BLNADVANCED = 1
                                        BEGIN
						                    SELECT 'ENTER ADVANCED'
                        
						                    SELECT @BatchPrefix = valuename
                                            FROM   tblappsettings
                                            WHERE  keyname = 'STRBATCODEPREFIXSUFFIX'

                                            SET @BatchPrefix = (SELECT Parsename(Replace(@BatchPrefix,'ƒ','') , 1))

                                            IF( @BatchPrefix = '<YEARMONTH>' )
                                              BEGIN
                                                  SELECT @Prefix = (SELECT  [dbo].[Ufnbatchcodeprefixsuffix](@BatchPrefix))

							                      SET @BatchCode = @Prefix + CONVERT(VARCHAR, @AutoBatchID)

							                    SELECT @BatchCode	,'WITH PREFIX'

						                      END
						                    ELSE
						                      BEGIN
							                    SELECT @BatchPrefix = ISNULL(valuename,'')  FROM   tblappsettings  WHERE  keyname = 'STRBATCODEPREFIXSUFFIX'

							                    SET @BatchPrefix = ISNULL((SELECT  Parsename(Replace(@BatchPrefix,'ƒ','') , 1)),'')

							                    SELECT @BatchID = Isnull( Max(batchid), 0 ) + 1  FROM   tblstock  WHERE  tenantid = @TenantID
							                    SELECT @AutoBatchID = Isnull( Max(AutoBatchID), 0 ) + 1  FROM   tblstock  WHERE  tenantid = @TenantID
							
							                    SET @BatchCode = @BatchPrefix + CAST(@AutoBatchID AS varchar)

						                      END
					                    END
				                      ELSE
					                    BEGIN
					                      --SELECT @BatchCode = Isnull( Max(batchid), 0 ) + 1  FROM   tblstock  WHERE  tenantid = @TenantID
					                      SELECT @BatchCode = Isnull( Max(AutoBatchID), 0 ) + 1  FROM   tblstock  WHERE  tenantid = @TenantID
					                    END

				                    IF @blnExpiry = 1
				                    BEGIN
					                    SET @BatchUniq = @BatchCode + '@' + CONVERT( VARCHAR(22), CONVERT( NUMERIC(18, 2), @MRP ) ) + '@' + Replace( CONVERT( VARCHAR(10), Format(@ExpDt, 'dd-MM-yy') ), '-', '' )
					                    SET @BarUniq = @BarCode + '@' + CONVERT( VARCHAR(22), CONVERT( NUMERIC(18, 2), @MRP ) ) + '@' + Replace( CONVERT( VARCHAR(10), Format(@ExpDt, 'dd-MM-yy') ) , '-', '' )
				                    END
				                    ELSE
				                    BEGIN
					                    SET @BatchUniq = @BatchCode + '@' + CONVERT( VARCHAR(22), CONVERT( NUMERIC(18, 2), @MRP ) ) 
					                    SET @BarUniq = @BarCode + '@' + CONVERT( VARCHAR(22), CONVERT( NUMERIC(18, 2), @MRP ) ) 
				                    END

					                    select @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BarUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID 

				                      IF EXISTS(SELECT *  FROM   tblstock  WHERE  itemid = @ItemID  AND batchcode = @BarCode  AND tenantid = @TenantID  AND batchunique = @BatchUniq  AND ccid = @CCID)
				                      BEGIN
					                    IF @VchTypeID = 0
					                    BEGIN
						                    EXEC Uspstockinsert  @StockID,  @TenantID,  @CCID,  @BarCode,  @BatchUniq,  @BatchID,  @MRP,  @ExpDt,  @CostRateInc,  @CostRateExcl,  @PRateExcl,  @PrateInc,  @TaxPer,  @SRate1,  @SRate2,  @SRate3,  @SRate4,  @SRate5,  @Qty,  @LastInvDt,  '',  NULL,  1,  @ItemID,  @BatchMode,  @PRate, @AutoBatchID

						                    INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,costrateinc,costrateexcl,prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,stockhisid,tenantid,stockid)
						                      VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )
					                    END
				                      END
				                      ELSE
				                      BEGIN
					                    IF EXISTS(SELECT *  FROM   tblstock  WHERE  itemid = @ItemID      AND batchcode = @BarCode      AND tenantid = @TenantID      AND ccid = @CCID)
					                    BEGIN
						                    IF @VchTypeID = 0
						                    BEGIN
							

							                    EXEC Uspstockinsert @StockID,@TenantID,@CCID,@BarCode,@BarUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode,@PRate,@AutoBatchID


							                    INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,costrateinc,costrateexcl,prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,stockhisid,tenantid,stockid)
							                    VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BarUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )
						                    END
					                    END
					                    ELSE
					                    BEGIN
						                    IF @VchTypeID = 0
						                    BEGIN
							                    EXEC Uspstockinsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,0,@ItemID,@BatchMode,@PRate,@AutoBatchID

							                    INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,costrateinc,costrateexcl,prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,stockhisid,tenantid,stockid)
							                    VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )
						                    END
					                    END
				                    END
			                    END
			                    ELSE
			                    BEGIN
			                    IF @BatchMode = 0 OR @BatchMode = 3
			                    BEGIN
                                    if @BatchCode = ''
				                        SELECT @BatchCode = itemcode  FROM   tblitemmaster  WHERE  itemid = @ItemID

				                    SET @BatchUniq = @BatchCode
				                    SET @BarCode = @BatchCode
			                    END
			                    ELSE
			                    BEGIN
				                    SET @BatchUniq = @BatchCode + '@'+ CONVERT( VARCHAR(22), CONVERT( NUMERIC(18, 2), @MRP ) )

				                    IF Charindex('@', @BatchUniq) = 0
				                    BEGIN
					                    IF @blnExpiry = 1
					                    BEGIN
						                    SET @BatchUniq = @BatchCode + '@'+ CONVERT( VARCHAR(22), CONVERT(NUMERIC(18,2),@MRP) )+ '@'+ Replace( CONVERT( VARCHAR(10),Format(@ExpDt,'dd-MM-yy')),'-', '' )
					                    END
					                    ELSE
					                    BEGIN
						                    SET @BatchUniq = @BatchCode + '@'+ CONVERT( VARCHAR(22), CONVERT(NUMERIC(18,2),@MRP) )
					                    END
				                    END
			                    END

			                    IF EXISTS(SELECT *  FROM   tblstock  WHERE  itemid = @ItemID  AND batchcode = @BarCode  AND tenantid = @TenantID  AND batchunique = @BatchUniq  AND ccid = @CCID)
			                    BEGIN
				                    select 'Exists',@StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,1,@ItemID,@BatchMode,@PRate
				                    EXEC Uspstockinsert @StockID,@TenantID,@CCID,@BatchCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@Qty,@LastInvDt,'',NULL,1,@ItemID,@BatchMode,@PRate,@AutoBatchID

				                    IF @VchTypeID = 0
				                    BEGIN
					                    INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,costrateinc,costrateexcl,prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,stockhisid,tenantid,stockid)
					                    VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )
				                    END
			                    END
			                    ELSE
			                    BEGIN
				                    select 'Not Exists',@StockID,  @TenantID,  @CCID,  @BatchCode,  @BatchUniq,  @BatchID,  @MRP,  @ExpDt,  @CostRateInc,  @CostRateExcl,  @PRateExcl,  @PrateInc,  @TaxPer,  @SRate1,  @SRate2,  @SRate3,  @SRate4,  @SRate5,  @Qty,  @LastInvDt,  '',  NULL,  0,  @ItemID,  @BatchMode,  @PRate
				                    EXEC Uspstockinsert  @StockID,  @TenantID,  @CCID,  @BatchCode,  @BatchUniq,  @BatchID,  @MRP,  @ExpDt,  @CostRateInc,  @CostRateExcl,  @PRateExcl,  @PrateInc,  @TaxPer,  @SRate1,  @SRate2,  @SRate3,  @SRate4,  @SRate5,  @Qty,  @LastInvDt,  '',  NULL,  0,  @ItemID,  @BatchMode,  @PRate,@AutoBatchID

				                    IF @VchTypeID = 0
				                    BEGIN
					                    INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,costrateinc,costrateexcl,prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,stockhisid,tenantid,stockid)
					                    VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )
				                    END
			                    END
		                    END

		                    SET @BatchCode = @BarCode
	                    END

	                    IF @Action = 'STOCKLESS'
	                    BEGIN
		                    SET @Qty = @Qty * -1;
	                    END

	                    IF @Action = 'STOCKDEL'
	                    BEGIN
		                    SELECT @CalcQOH = qoh FROM   tblstock WHERE  itemid = @ItemID AND ccid = @CCID AND batchcode = @BatchCode AND batchunique = @BatchUniq AND tenantid = @TenantID

		                    UPDATE tblstock SET    qoh = qoh - @CalcQOH
                                WHERE  itemid = @ItemID AND ccid = @CCID AND batchcode = @BatchCode AND batchunique = @BatchUniq AND tenantid = @TenantID

		                    DELETE FROM tblstockhistory
                                WHERE  refid = @RefID AND itemid = @ItemID	AND batchcode = @BatchCode	AND vchtypeid = @VchTypeID	AND ccid = @CCID	AND tenantid = @TenantID
	                    END

	                    SELECT @BatchCode
                    END ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }

            try
            {
                sQuery = @"drop PROCEDURE UspTransStockUpdate";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspTransStockUpdate] (@ItemID       NUMERIC(18, 0),
                                             @BatchCode    VARCHAR(50),
                                             @BatchUniq    VARCHAR(50),
                                             @Qty          NUMERIC(18, 5),
                                             @MRP          NUMERIC(18, 5),
                                             @CostRateInc  NUMERIC(18, 5),
                                             @CostRateExcl NUMERIC(18, 5),
                                             @PRateExcl    NUMERIC(18, 5),
                                             @PrateInc     NUMERIC(18, 5),
                                             @TaxPer       NUMERIC(18, 5),
                                             @SRate1       NUMERIC(18, 5),
                                             @SRate2       NUMERIC(18, 5),
                                             @SRate3       NUMERIC(18, 5),
                                             @SRate4       NUMERIC(18, 5),
                                             @SRate5       NUMERIC(18, 5),
                                             @BatchMode    INT,
                                             @VchType      VARCHAR(100),
                                             @VchDate      DATETIME,
                                             @ExpDt        DATETIME,
                                             @Action       VARCHAR(20),
                                             @RefID        NUMERIC(18, 0),
                                             @VchTypeID    NUMERIC(18, 0),
                                             @CCID         NUMERIC(18, 0),
                                             @TenantID     NUMERIC(18, 0),
                                             @ActPrate     NUMERIC(18, 5),
                                             @BarCode_out  VARCHAR(2000) output)
                        AS
                          BEGIN
                          BEGIN TRY    
                              DECLARE @BatchID NUMERIC(18, 0)
	                          DECLARE @AutoBatchID NUMERIC(18,0)
                              DECLARE @StockID NUMERIC(18, 0)
                              DECLARE @LastInvDt DATETIME = Getdate()
                              DECLARE @STOCKHISID NUMERIC(18, 0)
                              DECLARE @PRFXBATCH VARCHAR(10)
                              DECLARE @Stock NUMERIC(18, 5)
                              DECLARE @INVID NUMERIC(18, 0)
                              DECLARE @BarCode VARCHAR(50)
                              DECLARE @BarUniq VARCHAR(100)
                              DECLARE @CalcQOH NUMERIC(18, 5)
                              DECLARE @BLNADVANCED INT
                              DECLARE @blnExpiry BIT
                              DECLARE @LessQty NUMERIC(18, 5)
	                          DECLARE @ParentID NUMERIC(18, 5)
							  DECLARE @WSUPDATED BIT

							  SET @WSUPDATED = 0

                              SET @BarCode = @BatchCode

	                          SELECT @ParentID=ParentID FROM tblVchType WHERE VchTypeID = @VchTypeID

                              SELECT @StockID = Isnull(Max(stockid), 0) + 1
                              FROM   tblstock
                              WHERE  tenantid = @TenantID

                              SELECT @BatchID = Max( Isnull(batchid, 0) ) + 1
                              FROM   tblstock
                              WHERE  tenantid = @TenantID

	                          SELECT @BatchID = ISNULL(@BatchID,1)



                              SELECT @STOCKHISID = Max( Isnull(stockhisid, 0) ) + 1
                              FROM   tblstockhistory
                              WHERE  tenantid = @TenantID

                              SELECT @BLNADVANCED = Isnull(valuename, 0)
                              FROM   tblappsettings
                              WHERE  Upper(Ltrim(Rtrim(keyname))) = 'BLNADVANCED'

                              SELECT @Stock = Isnull(qoh, 0)
                              FROM   tblstock
                              WHERE  itemid = @ItemID
                                     AND batchcode = @BatchCode
                                     AND tenantid = @TenantID

                              SELECT @blnExpiry = Isnull(blnexpiry, 0)
                              FROM   tblitemmaster
                              WHERE  itemid = @ItemID



                              SET @LessQty = -1

                              select @StockID = isnull(@StockID,0)

                              IF @StockID = 0 
                                BEGIN
                                    SET @StockID = 1
                                END

                              IF @STOCKHISID = 0
                                BEGIN
                                    SET @STOCKHISID = 1
                                END

                              IF @BatchID = 0
                                BEGIN
                                    SET @BatchID = 1
                                END

	                        IF @Action = 'STOCKADD'
	                        BEGIN
		                        IF @BatchMode = 2
		                        BEGIN
			                        IF EXISTS(SELECT *  FROM   tblstock  WHERE  batchcode = @BarCode  AND  tenantid = @TenantID	AND  MRP <> @MRP)
      		                        BEGIN
				                        SET @BarCode_out = '-1|ERROR|CRITICAL|usptransstockupdate|The barcode is assigned with another MRP. Multiple MRP not allowed for auto barcode.'
				                        return;
			                        END
		                        END

		                        IF @BatchCode = '<Auto Barcode>'
		                        BEGIN
			                        DECLARE @Prefix VARCHAR(50)
			                        DECLARE @BatchPrefix VARCHAR(50)

			                        SELECT @AutoBatchID = Isnull( Max(AutoBatchID), 0 ) + 1  FROM   tblstock  WHERE  tenantid = @TenantID 

								CreateBatchAgain:
									
			                        IF @AutoBatchID = 0
			                        BEGIN
				                        SET @AutoBatchID = 1
			                        END

			                        IF @BLNADVANCED = 1
			                        BEGIN
				                        SELECT @BatchPrefix = valuename  FROM   tblappsettings   WHERE  keyname = 'STRBATCODEPREFIXSUFFIX'

				                        SET @BatchPrefix = (SELECT  Parsename(Replace(@BatchPrefix,'ƒ',''),1))

				                        IF( @BatchPrefix = '<YEARMONTH>' )
				                        BEGIN
					                        SELECT @Prefix = (SELECT  [dbo].[Ufnbatchcodeprefixsuffix](@BatchPrefix))
							  
					                        --SET @BatchCode = @Prefix + CONVERT(VARCHAR, @BatchID)
					                        SET @BatchCode = @Prefix + CONVERT(VARCHAR, @AutoBatchID)
				                        END
				                        ELSE
				                        BEGIN
					                        SELECT @BatchPrefix = valuename FROM   tblappsettings WHERE  keyname = 'STRBATCODEPREFIXSUFFIX'
					
					                        SET @BatchPrefix = (SELECT Parsename(Replace(@BatchPrefix,'ƒ',''),1))

					                        --SELECT @BatchCode = Isnull( Max(batchid), 0 ) + 1  FROM   tblstock  WHERE  tenantid = @TenantID
					                        SELECT @BatchCode = CONVERT(VARCHAR, @AutoBatchID)  
											--FROM   tblstock  WHERE  tenantid = @TenantID

					                        IF @BatchPrefix <> ''
					                        BEGIN
						                        SET @BatchCode = @BatchPrefix + CONVERT(VARCHAR,@BatchCode)
					                        END
				                        END
			                        END
			                        ELSE
			                        BEGIN
				                        --SELECT @BatchCode = Isnull( Max(batchid), 0 ) + 1  FROM   tblstock  WHERE  tenantid = @TenantID
				                        SELECT @BatchCode = CONVERT(VARCHAR, @AutoBatchID)  
										--FROM   tblstock  WHERE  tenantid = @TenantID
			                        END

			                        SET @BarCode = @BatchCode

			                        IF @blnExpiry = 1
			                        BEGIN
				                        SET @BatchUniq = @BatchCode + '@' + CONVERT( VARCHAR(22), CONVERT( NUMERIC(18,2), @MRP)) + '@'  + Replace( CONVERT( VARCHAR(10), Format(@ExpDt,'dd-MM-yy')),'-', '' )
				
				                        SET @BarUniq = @BarCode + '@' + CONVERT( VARCHAR(22), CONVERT( NUMERIC(18, 2), @MRP)) + '@' + Replace( CONVERT( VARCHAR(10), Format(@ExpDt,'dd-MM-yy')), '-', '' )
			                        END
			                        ELSE
			                        BEGIN
				                        SET @BatchUniq = @BatchCode + '@' + CONVERT( VARCHAR(22), CONVERT( NUMERIC(18, 2), @MRP))
				
				                        SET @BarUniq = @BarCode + '@' + CONVERT( VARCHAR(22), CONVERT( NUMERIC(18, 2), @MRP))
			                        END

									if exists(Select ItemID,BatchCode From tblStock Where LTRIM(RTRIM(BatchCode)) = @BarCode and ItemID <> @ItemID and TenantID = @TenantID )
									Begin
										SET @AutoBatchID = @AutoBatchID + 1
										goto CreateBatchAgain
									End

									IF EXISTS(SELECT *  FROM   tblstock  WHERE  itemid = @ItemID  AND batchcode = @BarCode  AND tenantid = @TenantID
										AND batchunique = @BatchUniq  AND ccid = @CCID)
									BEGIN
										IF @VchTypeID <> 0
										BEGIN
											if @ParentID = 2 or @ParentID = 20 or @ParentID = 6 or @ParentID = 1005
											EXEC Uspstockinsert  @StockID,  @TenantID,  @CCID,  @BarCode,  @BatchUniq,  @BatchID,  @MRP,  @ExpDt,  @CostRateInc,
												@CostRateExcl,  @PRateExcl,  @PrateInc,  @TaxPer,  @SRate1,  @SRate2,  @SRate3,  @SRate4,  @SRate5,  @Qty,
												@LastInvDt,  '',  NULL,  1,  @ItemID,  @BatchMode,  @ActPrate, @AutoBatchID

										INSERT INTO tblstockhistory (vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,costrateinc,
											costrateexcl,prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,stockhisid,tenantid,
											stockid)	VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,
											@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,
											@TenantID,@StockID )
										END
									END
									ELSE
									BEGIN
										IF EXISTS(SELECT *  FROM   tblstock  WHERE  itemid = @ItemID      AND batchcode = @BarCode      AND tenantid = @TenantID
											AND ccid = @CCID)
										BEGIN
											IF @VchTypeID <> 0
											BEGIN
												if @ParentID = 2 or @ParentID = 20 or @ParentID = 6 or @ParentID = 1005
												EXEC Uspstockinsert   @StockID,   @TenantID,   @CCID,   @BarCode,   @BarUniq,   @BatchID,   @MRP,   @ExpDt,
													@CostRateInc,   @CostRateExcl,   @PRateExcl,   @PrateInc,   @TaxPer,   @SRate1,   @SRate2,   @SRate3,
													@SRate4,   @SRate5,   @Qty,   @LastInvDt,   '',   NULL,   0,   @ItemID,   @BatchMode,   @ActPrate, @AutoBatchID

												INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,costrateinc,
													costrateexcl,prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,stockhisid,tenantid,stockid)
												VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BarCode,@BarUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,
													@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )
											END
										END
										ELSE
										BEGIN
											IF @VchTypeID <> 0
											BEGIN
												if @ParentID = 2 or @ParentID = 20 or @ParentID = 6 or @ParentID = 1005
												EXEC Uspstockinsert   @StockID,   @TenantID,   @CCID,   @BatchCode,   @BatchUniq,   @BatchID,   @MRP,   @ExpDt,
													@CostRateInc,   @CostRateExcl,   @PRateExcl,   @PrateInc,   @TaxPer,   @SRate1,   @SRate2,   @SRate3,   @SRate4,
													@SRate5,   @Qty,   @LastInvDt,   '',   NULL,   0,   @ItemID,   @BatchMode,   @ActPrate, @AutoBatchID

												INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,costrateinc,
													costrateexcl,prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,stockhisid,tenantid,stockid)
												VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,
													@PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )
											END
										END
									END
		                        END
		                        ELSE
		                        BEGIN
			                        IF @BatchMode = 0 OR @BatchMode = 3 
			                        BEGIN
										if @BatchCode = ''
											SELECT @BatchCode = itemcode  FROM   tblitemmaster  WHERE  itemid = @ItemID
			
				                        SET @BarCode = @BatchCode

				                        IF EXISTS(SELECT *  FROM   tblstock  WHERE  itemid = @ItemID      AND batchcode = @BarCode      AND tenantid = @TenantID
						                        AND batchunique = @BatchUniq      AND ccid = @CCID)
				                        BEGIN
					                        if @ParentID = 2 or @ParentID = 20 or @ParentID = 6 or @ParentID = 1005
					                        EXEC Uspstockinsert  @StockID,  @TenantID,  @CCID,  @BatchCode,  @BatchUniq,  @BatchID,  @MRP,  @ExpDt,  @CostRateInc,
						                        @CostRateExcl,  @PRateExcl,  @PrateInc,  @TaxPer,  @SRate1,  @SRate2,  @SRate3,  @SRate4,  @SRate5,
						                        @Qty,  @LastInvDt,  '',  NULL,  1,  @ItemID,  @BatchMode,  @ActPrate, @AutoBatchID

					                        IF @VchTypeID <> 0
					                        BEGIN
						                        INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,
							                        costrateinc,costrateexcl,prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,
							                        stockhisid,tenantid,stockid)
						                        VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,
							                        @PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )
					                        END
				                        END
				                        ELSE
				                        BEGIN
					                        if @ParentID = 2 or @ParentID = 20 or @ParentID = 6 or @ParentID = 1005
					                        EXEC Uspstockinsert  @StockID,  @TenantID,  @CCID,  @BatchCode,  @BatchUniq,  @BatchID,  @MRP,  @ExpDt,  @CostRateInc,
						                        @CostRateExcl,  @PRateExcl,  @PrateInc,  @TaxPer,  @SRate1,  @SRate2,  @SRate3,  @SRate4,  @SRate5,  @Qty,  @LastInvDt,
						                        '',  NULL,  0,  @ItemID,  @BatchMode,  @ActPrate, @AutoBatchID

					                        IF @VchTypeID <> 0
					                        BEGIN
						                        INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,costrateinc,costrateexcl,
							                        prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,stockhisid,tenantid,stockid)
						                        VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,
							                        @TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )
					                        END
				                        END
			                        END
			                        ELSE
			                        BEGIN
				
				                        DECLARE @EXPIRYCONDITION AS VARCHAR(500)
				
				                        IF Charindex('@', @BatchUniq) = 0
				                        BEGIN
					                        IF @blnExpiry = 1
					                        BEGIN
						                        SET @BatchUniq = @BatchCode + '@' + CONVERT( VARCHAR(22), CONVERT(NUMERIC(18,2),@MRP)) + '@' + Replace( CONVERT( VARCHAR(10),Format(@ExpDt,'dd-MM-yy')),'-', '' )
						                        SET @EXPIRYCONDITION = ' AND Format(expirydate, ''dd-MM-yyyy'') = ''' + CONVERT( VARCHAR(12),Format(CONVERT(DATE, @ExpDt),'dd-MM-yyyy')) + ''''
					                        END
					                        ELSE
					                        BEGIN
						                        SET @BatchUniq = @BatchCode + '@'+ CONVERT( VARCHAR(22), CONVERT(NUMERIC(18,2),@MRP))
						                        SET @EXPIRYCONDITION = ' '
					                        END
				                        END
				                        ELSE
				                        BEGIN
					                        IF @blnExpiry = 1
					                        BEGIN
						                        SET @BatchUniq = @BatchCode + '@' + CONVERT( VARCHAR(22), CONVERT(NUMERIC(18,2), @MRP)) + '@' + Replace( CONVERT( VARCHAR(10),Format(@ExpDt,'dd-MM-yy')),'-', '' )
						                        SET @EXPIRYCONDITION = ' AND Format(expirydate, ''dd-MM-yyyy'') = ''' + CONVERT( VARCHAR(12),Format(CONVERT(DATE, @ExpDt),'dd-MM-yyyy')) + ''''
					                        END
					                        ELSE
					                        BEGIN
						                        SET @BatchUniq = @BatchCode + '@' + CONVERT( VARCHAR(22), CONVERT(NUMERIC(18,2),@MRP))
						                        SET @EXPIRYCONDITION = ' '
					                        END
				                        END

				                        DECLARE @QUERY VARCHAR(3000)

				                        SET @QUERY = 'SELECT *  FROM   tblstock  WHERE  itemid = ' + CONVERT(VARCHAR(100), @ItemID) + ' AND batchcode = ''' + @BarCode + '''' + @EXPIRYCONDITION + ' AND tenantid = ' + CONVERT(VARCHAR(100), @TenantID) +
				                        ' AND batchunique = ''' + @BatchUniq + ''' AND ccid = ' + CONVERT(VARCHAR(100), @CCID) + ' AND mrp = ' + CONVERT(VARCHAR(100), @MRP  )

				                        EXEC (@QUERY)

				                        --IF EXISTS(SELECT *  FROM   tblstock  WHERE  itemid = @ItemID AND batchcode = @BarCode AND tenantid = @TenantID
				                        --	AND batchunique = @BatchUniq AND ccid = @CCID AND mrp = @MRP AND Format(expirydate, 'dd-MM-yy') = Format(@ExpDt, 'dd-MM-yy'))
				                        IF @@ROWCOUNT > 0
				                        BEGIN
					                        if @ParentID = 2 or @ParentID = 20 or @ParentID = 6 or @ParentID = 1005

											SELECT  @AutoBatchID = autobatchid  FROM   tblstock  WHERE  itemid = @ItemID AND tenantid = @TenantID AND batchunique = @BatchUniq  

					                        EXEC Uspstockinsert  @StockID,  @TenantID,  @CCID,  @BatchCode,  @BatchUniq,  @BatchID,  @MRP,  @ExpDt,  @CostRateInc,  @CostRateExcl,
						                        @PRateExcl,  @PrateInc,  @TaxPer,  @SRate1,  @SRate2,  @SRate3,  @SRate4,  @SRate5,  @Qty,  @LastInvDt,  '',  NULL,  1,  @ItemID,
						                        @BatchMode,  @ActPrate, @AutoBatchID

					                        IF @VchTypeID <> 0
					                        BEGIN
						                        INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,
							                        costrateinc,costrateexcl,prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,
							                        stockhisid,tenantid,stockid)
						                        VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,
							                        @PRateExcl,@PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )
					                        END
				                        END
				                        ELSE
				                        BEGIN
					                        if @ParentID = 2 or @ParentID = 20 or @ParentID = 6 or @ParentID = 1005
					                        EXEC Uspstockinsert  @StockID,  @TenantID,  @CCID,  @BatchCode,  @BatchUniq,  @BatchID,  @MRP,  @ExpDt,  @CostRateInc,
						                        @CostRateExcl,  @PRateExcl,  @PrateInc,  @TaxPer,  @SRate1,  @SRate2,  @SRate3,  @SRate4,  @SRate5,  @Qty,  @LastInvDt,  '',
						                        NULL,  0,  @ItemID,  @BatchMode,  @ActPrate, @AutoBatchID

					                        IF @VchTypeID <> 0
					                        BEGIN
						                        INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,costrateinc,costrateexcl,
							                        prateexcl,prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,stockhisid,tenantid,stockid)
						                        VALUES      ( @VchType,@VchDate,@RefID,@ItemID,@Qty,0,@BatchCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,
							                        @TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )
					                        END
				                        END
			                        END
		                        END

		                        SET @BatchCode = @BarCode
	                        END

	                        IF @Action = 'STOCKLESS'
	                        BEGIN
		                        --SELECT @ItemID , @BarCode , @TenantID , @BatchUniq , @CCID , @VchTypeID 

		                        IF EXISTS(SELECT * FROM   tblstock WHERE  itemid = @ItemID AND batchcode = @BarCode AND tenantid = @TenantID
			                        AND batchunique = @BatchUniq AND ccid = @CCID)
		                        BEGIN
			                        IF @VchTypeID <> 0
			                        BEGIN
				                        SET @LessQty = @LessQty * @Qty

				                        --select 'Stock updation' as f1

				                        if @ParentID = 2 or @ParentID = 20 or @ParentID = 6 or @ParentID = 1005
				                        EXEC Uspstockinsert @StockID,@TenantID,@CCID,@BarCode,@BatchUniq,@BatchID,@MRP,@ExpDt,@CostRateInc,@CostRateExcl,@PRateExcl,
					                        @PrateInc,@TaxPer,@SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@LessQty,@LastInvDt,'',NULL,1,@ItemID,@BatchMode,@ActPrate, @AutoBatchID

				                        --select 'Stock history updation' as f1

				                        INSERT INTO tblstockhistory(vchtype,vchdate,refid,itemid,qtyin,qtyout,batchcode,batchunique,expiry,mrp,costrateinc,costrateexcl,prateexcl,
					                        prateinc,taxper,srate1,srate2,srate3,srate4,srate5,vchtypeid,ccid,stockhisid,tenantid,stockid)
				                        VALUES      ( @VchType,@VchDate,@RefID,@ItemID,0,@Qty,@BarCode,@BatchUniq,@ExpDt,@MRP,@CostRateInc,@CostRateExcl,@PRateExcl,@PrateInc,@TaxPer,
					                        @SRate1,@SRate2,@SRate3,@SRate4,@SRate5,@VchTypeID,@CCID,@STOCKHISID,@TenantID,@StockID )

					                        --select @@ROWCOUNT as insertedrows

			                        END
		                        END
	                        END

	                        IF @Action = 'STOCKDEL'
	                        BEGIN
		                        SELECT @CalcQOH = qoh   FROM   tblstock   WHERE  itemid = @ItemID  AND ccid = @CCID   AND batchunique = @BatchUniq  AND tenantid = @TenantID

		                        UPDATE tblstock  SET    qoh = qoh - @CalcQOH  WHERE  itemid = @ItemID AND ccid = @CCID AND batchunique = @BatchUniq AND tenantid = @TenantID

		                        DELETE FROM tblstockhistory  WHERE  refid = @RefID and VchTypeID = @VchTypeID --AND ccid = @CCID AND tenantid = @TenantID 
	                        END

	                        SET @BarCode_out = @BatchUniq

	                        SELECT @BarCode_out

	                        END TRY    
                            BEGIN CATCH    
      	                        --ROLLBACK;    
      	                        SELECT - 1 AS SqlSpResult    
      		                        ,ERROR_NUMBER() AS ErrorNumber    
      		                        ,ERROR_STATE() AS ErrorState    
      		                        ,ERROR_SEVERITY() AS ErrorSeverity    
      		                        ,ERROR_PROCEDURE() AS ErrorProcedure    
      		                        ,ERROR_LINE() AS ErrorLine    
      		                        ,ERROR_MESSAGE() AS ErrorMessage;    
                            END CATCH;    
                        END  ";

                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }


            try
            {
                sQuery = @"drop PROCEDURE UspVchTypeInsert";
                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
            try
            {
                sQuery = @"CREATE PROCEDURE [dbo].[UspVchTypeInsert] (@VchTypeID                NUMERIC(18,0),
                                          @VchType                  VARCHAR(50),
                                          @ShortKey                 VARCHAR(50),
                                          @EasyKey                  VARCHAR(50),
                                          @SortOrder                NUMERIC(18,0),
                                          @ActiveStatus             NUMERIC(18,0),
                                          @ParentID                 NUMERIC(18,0),
                                          @Description              VARCHAR(250),
                                          @numberingCode            NUMERIC(18,0),
                                          @Prefix                   VARCHAR(10),
                                          @Sufix                    VARCHAR(10),
                                          @ItemClassIDS             VARCHAR(100),
                                          @CreditGroupIDs           VARCHAR(3000),
                                          @DebitGroupIDs            VARCHAR(3000),
                                          @ProductTypeIDs           VARCHAR(100),
                                          @GeneralSettings          VARCHAR(1000),
                                          @NegativeBalance          NUMERIC(18,0),
                                          @RoundOffBlock            FLOAT,
                                          @RoundOffMode             NUMERIC(18,0),
                                          @ItemClassIDS2            VARCHAR(500),
                                          @SecondaryCCIDS           VARCHAR(500),
                                          @PrimaryCCIDS             VARCHAR(500),
                                          @OrderVchTypeIDS          VARCHAR(1000),
                                          @NoteVchTypeIDS           VARCHAR(1000),
                                          @QuotationVchTypeIDS      VARCHAR(1000),
                                          @DEFMOPID                 INT,
                                          @BLNLOCKMOP               INT,
                                          @DEFTAXMODEID             INT,
                                          @BLNLOCKTAXMODE           INT,
                                          @DEFAGENTID               INT,
                                          @BLNLOCKAGENT             INT,
                                          @DEFPRICELISTID           INT,
                                          @BLNLOCKPRICELIST         INT,
                                          @DEFSALESMANID            INT,
                                          @BLNLOCKSALESMAN          INT,
                                          @DEFPRINTID               INT,
                                          @BLNLOCKPRINT             INT,
                                          @ColwidthStr              VARCHAR(2000),
                                          @gridColor                VARCHAR(50),
                                          @DefaultGodownID          NUMERIC(18,0),
                                          @ActCFasCostLedger        NUMERIC(18,0),
                                          @ActCFasCostLedger4       NUMERIC(18,0),
                                          @gridHeaderColor          VARCHAR(50),
                                          @BLNUseForClientSync      NUMERIC(18,0),
                                          @rateInclusiveIndex       NUMERIC(18,0),
                                          @BlnBillWiseDisc          NUMERIC(18,0),
                                          @BlnItemWisePerDisc       NUMERIC(18,0),
                                          @BlnItemWiseAmtDisc       NUMERIC(18,0),
                                          @gridselectedRow          VARCHAR(50),
                                          @GridHeaderFont           VARCHAR(50),
                                          @GridBackColor            VARCHAR(50),
                                          @GridAlternatCellColor    VARCHAR(50),
                                          @GridCellColor            VARCHAR(50),
                                          @GridFontColor            VARCHAR(50),
                                          @Metatag                  NVARCHAR(3000),
                                          @DefaultCriteria          NVARCHAR(50),
                                          @SearchSql                NVARCHAR(max),
                                          @SmartSearchBehavourMode  NUMERIC(18,0),
                                          @criteriaconfig           VARCHAR(max),
                                          @intEnterKeyBehavourMode  NUMERIC(18,0),
                                          @BlnBillDiscAmtEntry      NUMERIC(18,0),
                                          @blnRateDiscount          NUMERIC(18,0),
                                          @IntdefaultFocusColumnID  NUMERIC(18,0),
                                          @BlnTouchScreen           NUMERIC(18,0),
                                          @StrTouchSetting          VARCHAR(max),
                                          @StrCalculationFields     VARCHAR(max),
                                          @CRateCalMethod           INT,
                                          @MMRPSortOrder            NUMERIC(18,0),
                                          @ItemDiscountFrom         NUMERIC(18,0),
                                          @DEFPRINTID2              NUMERIC(18,0),
                                          @BLNLOCKPRINT2            NUMERIC(18,0),
                                          @BillDiscountFrom         NUMERIC(18,0),
                                          @WindowBackColor          VARCHAR(50),
                                          @ContrastBackColor        VARCHAR(50),
                                          @BlnEnableCustomFormColor NUMERIC(18,0),
                                          @returnVchtypeID          NUMERIC(18,0),
                                          @PrintCopies              NUMERIC(18,0),
                                          @SystemName               VARCHAR(50),
                                          @UserID                   NUMERIC(18,0),
                                          @LastUpdateDate           DATETIME,
                                          @LastUpdateTime           DATETIME,
                                          @BlnMobileVoucher         NUMERIC(18,0),
                                          @SearchSQLSettings        VARCHAR(max),
                                          @AdvancedSearchSQLEnabled NUMERIC(18,0),
                                          @TenantID                 NUMERIC(18,0),
                                          @VchJson                  VARCHAR(max),
                                          @FeaturesJson             VARCHAR(max),
                                          @DEFTaxInclusiveID        INT,
                                          @BLNLOCKTaxInclusive      INT,
										  @PrintSettings             VARCHAR(2000),
                                          @BoardRateQuery           VARCHAR(2000),
                                          @BoardRateFileName        VARCHAR(1000),
                                          @BoardRateExportType      INT,
										  @Action                   INT = 0)
                AS
                  BEGIN
                      DECLARE @RetResult INT

                      BEGIN try
                          BEGIN TRANSACTION;

                          IF @Action = 0
                            BEGIN
                                INSERT INTO tblvchtype
                                            (vchtypeid,
                                             vchtype,
                                             shortkey,
                                             easykey,
                                             sortorder,
                                             activestatus,
                                             parentid,
                                             description,
                                             numberingcode,
                                             prefix,
                                             sufix,
                                             itemclassids,
                                             creditgroupids,
                                             debitgroupids,
                                             producttypeids,
                                             generalsettings,
                                             negativebalance,
                                             roundoffblock,
                                             roundoffmode,
                                             itemclassids2,
                                             secondaryccids,
                                             primaryccids,
                                             ordervchtypeids,
                                             notevchtypeids,
                                             quotationvchtypeids,
                                             defmopid,
                                             blnlockmop,
                                             deftaxmodeid,
                                             blnlocktaxmode,
                                             defagentid,
                                             blnlockagent,
                                             defpricelistid,
                                             blnlockpricelist,
                                             defsalesmanid,
                                             blnlocksalesman,
                                             defprintid,
                                             blnlockprint,
                                             colwidthstr,
                                             gridcolor,
                                             defaultgodownid,
                                             actcfascostledger,
                                             actcfascostledger4,
                                             gridheadercolor,
                                             blnuseforclientsync,
                                             rateinclusiveindex,
                                             blnbillwisedisc,
                                             blnitemwiseperdisc,
                                             blnitemwiseamtdisc,
                                             gridselectedrow,
                                             gridheaderfont,
                                             gridbackcolor,
                                             gridalternatcellcolor,
                                             gridcellcolor,
                                             gridfontcolor,
                                             metatag,
                                             defaultcriteria,
                                             searchsql,
                                             smartsearchbehavourmode,
                                             criteriaconfig,
                                             intenterkeybehavourmode,
                                             blnbilldiscamtentry,
                                             blnratediscount,
                                             intdefaultfocuscolumnid,
                                             blntouchscreen,
                                             strtouchsetting,
                                             strcalculationfields,
                                             cratecalmethod,
                                             mmrpsortorder,
                                             itemdiscountfrom,
                                             defprintid2,
                                             blnlockprint2,
                                             billdiscountfrom,
                                             windowbackcolor,
                                             contrastbackcolor,
                                             blnenablecustomformcolor,
                                             returnvchtypeid,
                                             printcopies,
                                             systemname,
                                             userid,
                                             lastupdatedate,
                                             lastupdatetime,
                                             blnmobilevoucher,
                                             searchsqlsettings,
                                             advancedsearchsqlenabled,
                                             tenantid,
                                             vchjson,
                                             featuresjson,
                                             deftaxinclusiveid,
                                             blnlocktaxinclusive,
							                 PrintSettings,
                                             BoardRateQuery,
                                             BoardRateFileName,
                                             BoardRateExportType)
                                VALUES     (@VchTypeID,
                                            @VchType,
                                            @ShortKey,
                                            @EasyKey,
                                            @SortOrder,
                                            @ActiveStatus,
                                            @ParentID,
                                            @Description,
                                            @numberingCode,
                                            @Prefix,
                                            @Sufix,
                                            @ItemClassIDS,
                                            @CreditGroupIDs,
                                            @DebitGroupIDs,
                                            @ProductTypeIDs,
                                            @GeneralSettings,
                                            @NegativeBalance,
                                            @RoundOffBlock,
                                            @RoundOffMode,
                                            @ItemClassIDS2,
                                            @SecondaryCCIDS,
                                            @PrimaryCCIDS,
                                            @OrderVchTypeIDS,
                                            @NoteVchTypeIDS,
                                            @QuotationVchTypeIDS,
                                            @DEFMOPID,
                                            @BLNLOCKMOP,
                                            @DEFTAXMODEID,
                                            @BLNLOCKTAXMODE,
                                            @DEFAGENTID,
                                            @BLNLOCKAGENT,
                                            @DEFPRICELISTID,
                                            @BLNLOCKPRICELIST,
                                            @DEFSALESMANID,
                                            @BLNLOCKSALESMAN,
                                            @DEFPRINTID,
                                            @BLNLOCKPRINT,
                                            @ColwidthStr,
                                            @gridColor,
                                            @DefaultGodownID,
                                            @ActCFasCostLedger,
                                            @ActCFasCostLedger4,
                                            @gridHeaderColor,
                                            @BLNUseForClientSync,
                                            @rateInclusiveIndex,
                                            @BlnBillWiseDisc,
                                            @BlnItemWisePerDisc,
                                            @BlnItemWiseAmtDisc,
                                            @gridselectedRow,
                                            @GridHeaderFont,
                                            @GridBackColor,
                                            @GridAlternatCellColor,
                                            @GridCellColor,
                                            @GridFontColor,
                                            @Metatag,
                                            @DefaultCriteria,
                                            @SearchSql,
                                            @SmartSearchBehavourMode,
                                            @criteriaconfig,
                                            @intEnterKeyBehavourMode,
                                            @BlnBillDiscAmtEntry,
                                            @blnRateDiscount,
                                            @IntdefaultFocusColumnID,
                                            @BlnTouchScreen,
                                            @StrTouchSetting,
                                            @StrCalculationFields,
                                            @CRateCalMethod,
                                            @MMRPSortOrder,
                                            @ItemDiscountFrom,
                                            @DEFPRINTID2,
                                            @BLNLOCKPRINT2,
                                            @BillDiscountFrom,
                                            @WindowBackColor,
                                            @ContrastBackColor,
                                            @BlnEnableCustomFormColor,
                                            @returnVchtypeID,
                                            @PrintCopies,
                                            @SystemName,
                                            @UserID,
                                            @LastUpdateDate,
                                            @LastUpdateTime,
                                            @BlnMobileVoucher,
                                            @SearchSQLSettings,
                                            @AdvancedSearchSQLEnabled,
                                            @TenantID,
                                            @VchJson,
                                            @FeaturesJson,
                                            @DEFTaxInclusiveID,
                                            @BLNLOCKTAXINCLUSIVE,
							                @PrintSettings,
                                          @BoardRateQuery,
                                          @BoardRateFileName,
                                          @BoardRateExportType)

                                SET @RetResult = 1;
                            END

                          IF @Action = 1
                            BEGIN
				                IF @VchTypeID > 1005
				                BEGIN
					                UPDATE tblvchtype
					                SET    vchtype = @VchType,shortkey = @ShortKey,easykey = @EasyKey,sortorder = @SortOrder,activestatus = @ActiveStatus,parentid = @ParentID,description = @Description,numberingcode = @numberingCode,prefix = @Prefix,sufix = @Sufix,itemclassids = @ItemClassIDS,creditgroupids = @CreditGroupIDs,debitgroupids = @DebitGroupIDs,producttypeids = @ProductTypeIDs,generalsettings = @GeneralSettings,negativebalance = @NegativeBalance,roundoffblock = @RoundOffBlock,roundoffmode = @RoundOffMode,itemclassids2 = @ItemClassIDS2,secondaryccids = @SecondaryCCIDS,primaryccids = @PrimaryCCIDS,ordervchtypeids = @OrderVchTypeIDS,notevchtypeids = @NoteVchTypeIDS,quotationvchtypeids = @QuotationVchTypeIDS,
						                defmopid = @DEFMOPID,blnlockmop = @BLNLOCKMOP,deftaxmodeid = @DEFTAXMODEID,blnlocktaxmode = @BLNLOCKTAXMODE,defagentid = @DEFAGENTID,blnlockagent = @BLNLOCKAGENT,defpricelistid = @DEFPRICELISTID,blnlockpricelist = @BLNLOCKPRICELIST,defsalesmanid = @DEFSALESMANID,blnlocksalesman = @BLNLOCKSALESMAN,defprintid = @DEFPRINTID,blnlockprint = @BLNLOCKPRINT,colwidthstr = @ColwidthStr,gridcolor = @gridColor,defaultgodownid = @DefaultGodownID,actcfascostledger = @ActCFasCostLedger,actcfascostledger4 = @ActCFasCostLedger4,gridheadercolor = @gridHeaderColor,blnuseforclientsync = @BLNUseForClientSync,rateinclusiveindex = @rateInclusiveIndex,blnbillwisedisc = @BlnBillWiseDisc,
						                blnitemwiseperdisc = @BlnItemWisePerDisc,blnitemwiseamtdisc = @BlnItemWiseAmtDisc,gridselectedrow = @gridselectedRow,gridheaderfont = @GridHeaderFont,gridbackcolor = @GridBackColor,gridalternatcellcolor = @GridAlternatCellColor,gridcellcolor = @GridCellColor,gridfontcolor = @GridFontColor,metatag = @Metatag,defaultcriteria = @DefaultCriteria,searchsql = @SearchSql,smartsearchbehavourmode = @SmartSearchBehavourMode,criteriaconfig = @criteriaconfig,intenterkeybehavourmode = @intEnterKeyBehavourMode,blnbilldiscamtentry = @BlnBillDiscAmtEntry,blnratediscount = @blnRateDiscount,intdefaultfocuscolumnid = @IntdefaultFocusColumnID,blntouchscreen = @BlnTouchScreen,strtouchsetting = @StrTouchSetting,
						                strcalculationfields = @StrCalculationFields,cratecalmethod = @CRateCalMethod,mmrpsortorder = @MMRPSortOrder,itemdiscountfrom = @ItemDiscountFrom,defprintid2 = @DEFPRINTID2,blnlockprint2 = @BLNLOCKPRINT2,billdiscountfrom = @BillDiscountFrom,windowbackcolor = @WindowBackColor,contrastbackcolor = @ContrastBackColor,blnenablecustomformcolor = @BlnEnableCustomFormColor,returnvchtypeid = @returnVchtypeID,printcopies = @PrintCopies,systemname = @SystemName,userid = @UserID,lastupdatedate = @LastUpdateDate,lastupdatetime = @LastUpdateTime,blnmobilevoucher = @BlnMobileVoucher,searchsqlsettings = @SearchSQLSettings,advancedsearchsqlenabled = @AdvancedSearchSQLEnabled,vchjson = @VchJson,
						                featuresjson = @FeaturesJson,deftaxinclusiveid = @DEFTaxInclusiveID,blnlocktaxinclusive = @BLNLOCKTAXINCLUSIVE, PrintSettings = @PrintSettings, BoardRateQuery = @BoardRateQuery, BoardRateFileName = @BoardRateFileName, BoardRateExportType = @BoardRateExportType 
					                WHERE  vchtypeid = @VchTypeID
						                AND tenantid = @TenantID
				                END
				                ELSE
				                BEGIN
					                UPDATE tblvchtype
					                SET    shortkey = @ShortKey,easykey = @EasyKey,sortorder = @SortOrder,activestatus = @ActiveStatus,description = @Description,numberingcode = @numberingCode,prefix = @Prefix,sufix = @Sufix,itemclassids = @ItemClassIDS,creditgroupids = @CreditGroupIDs,debitgroupids = @DebitGroupIDs,producttypeids = @ProductTypeIDs,generalsettings = @GeneralSettings,negativebalance = @NegativeBalance,roundoffblock = @RoundOffBlock,roundoffmode = @RoundOffMode,itemclassids2 = @ItemClassIDS2,secondaryccids = @SecondaryCCIDS,primaryccids = @PrimaryCCIDS,ordervchtypeids = @OrderVchTypeIDS,notevchtypeids = @NoteVchTypeIDS,quotationvchtypeids = @QuotationVchTypeIDS,
						                defmopid = @DEFMOPID,blnlockmop = @BLNLOCKMOP,deftaxmodeid = @DEFTAXMODEID,blnlocktaxmode = @BLNLOCKTAXMODE,defagentid = @DEFAGENTID,blnlockagent = @BLNLOCKAGENT,defpricelistid = @DEFPRICELISTID,blnlockpricelist = @BLNLOCKPRICELIST,defsalesmanid = @DEFSALESMANID,blnlocksalesman = @BLNLOCKSALESMAN,defprintid = @DEFPRINTID,blnlockprint = @BLNLOCKPRINT,colwidthstr = @ColwidthStr,gridcolor = @gridColor,defaultgodownid = @DefaultGodownID,actcfascostledger = @ActCFasCostLedger,actcfascostledger4 = @ActCFasCostLedger4,gridheadercolor = @gridHeaderColor,blnuseforclientsync = @BLNUseForClientSync,rateinclusiveindex = @rateInclusiveIndex,blnbillwisedisc = @BlnBillWiseDisc,
						                blnitemwiseperdisc = @BlnItemWisePerDisc,blnitemwiseamtdisc = @BlnItemWiseAmtDisc,gridselectedrow = @gridselectedRow,gridheaderfont = @GridHeaderFont,gridbackcolor = @GridBackColor,gridalternatcellcolor = @GridAlternatCellColor,gridcellcolor = @GridCellColor,gridfontcolor = @GridFontColor,metatag = @Metatag,defaultcriteria = @DefaultCriteria,searchsql = @SearchSql,smartsearchbehavourmode = @SmartSearchBehavourMode,criteriaconfig = @criteriaconfig,intenterkeybehavourmode = @intEnterKeyBehavourMode,blnbilldiscamtentry = @BlnBillDiscAmtEntry,blnratediscount = @blnRateDiscount,intdefaultfocuscolumnid = @IntdefaultFocusColumnID,blntouchscreen = @BlnTouchScreen,strtouchsetting = @StrTouchSetting,
						                strcalculationfields = @StrCalculationFields,cratecalmethod = @CRateCalMethod,mmrpsortorder = @MMRPSortOrder,itemdiscountfrom = @ItemDiscountFrom,defprintid2 = @DEFPRINTID2,blnlockprint2 = @BLNLOCKPRINT2,billdiscountfrom = @BillDiscountFrom,windowbackcolor = @WindowBackColor,contrastbackcolor = @ContrastBackColor,blnenablecustomformcolor = @BlnEnableCustomFormColor,returnvchtypeid = @returnVchtypeID,printcopies = @PrintCopies,systemname = @SystemName,userid = @UserID,lastupdatedate = @LastUpdateDate,lastupdatetime = @LastUpdateTime,blnmobilevoucher = @BlnMobileVoucher,searchsqlsettings = @SearchSQLSettings,advancedsearchsqlenabled = @AdvancedSearchSQLEnabled,vchjson = @VchJson,
						                featuresjson = @FeaturesJson,deftaxinclusiveid = @DEFTaxInclusiveID,blnlocktaxinclusive = @BLNLOCKTAXINCLUSIVE, PrintSettings = @PrintSettings , BoardRateQuery = @BoardRateQuery, BoardRateFileName = @BoardRateFileName, BoardRateExportType = @BoardRateExportType 
					                WHERE  vchtypeid = @VchTypeID
						                AND tenantid = @TenantID
				                END

				                SET @RetResult = 1;
                            END

                          IF @Action = 2
                            BEGIN
				                IF @VchTypeID > 1005
				                BEGIN
					                DELETE FROM tblvchtype
					                WHERE  vchtypeid = @VchTypeID
                                       AND tenantid = @TenantID

					                SET @RetResult = 0;
				                END
				                ELSE
				                BEGIN
					                SET @RetResult = -1;
				                END
                            END

                          COMMIT TRANSACTION;

			                IF @RetResult <> -1
				                SELECT @RetResult AS SqlSpResult
			                ELSE
				                SELECT -1 as SqlSpResult, -1 AS ErrorNumber, 'TRANSACTION FAILED' AS ErrorState, 'CRITICAL' AS ErrorSeverity,
					                'Uspvchtypeinsert' AS ErrorProcedure, -1 AS ErrorLine, 'DEFAULT VCHTYPE CANNOT BE DELETED' AS ErrorMessage;
                      END try

                      BEGIN catch
                          ROLLBACK;

                          SELECT -1                AS SqlSpResult,
                                 Error_number()    AS ErrorNumber,
                                 Error_state()     AS ErrorState,
                                 Error_severity()  AS ErrorSeverity,
                                 Error_procedure() AS ErrorProcedure,
                                 Error_line()      AS ErrorLine,
                                 Error_message()   AS ErrorMessage;
                      END catch;
                  END ";


                fnExecuteNonQuery(sQuery, false);
            }
            catch
            { }
        }


        public double GetLedgerBalance(long LedgerID, DateTime AsOnDate, DateTime StartDate = default(DateTime), string VchtypeID = "", string strCCIDsql = "")
        {
            try
            {
                double returnvalue = 0;
                sqlControl rs = new sqlControl();
                string StrSqlDate = "";
                string strVchtypeID = "";
                if (Strings.Len(VchtypeID) > 0)
                {
                    if (Conversion.Val(VchtypeID) > 0)
                        strVchtypeID = " And vchTypeID In(" + VchtypeID + ") ";
                }

                if (Strings.InStr(1, StartDate.ToString(), "00") > 1)
                    StrSqlDate = " And vchDate <='" + AsOnDate.ToString("dd/MMM/yyyy") + "'";
                else
                    StrSqlDate = " and vchDate between '" + AsOnDate.ToString("dd/MMM/yyyy") + "' and '" + StartDate.ToString("dd/MMM/yyyy") + "'";
                // =================


                string StrCCSQL1 = "";
                if (strCCIDsql != "")
                    StrCCSQL1 = strCCIDsql.Replace("and", "") + " AND ";

                rs.Open("Select Sum(AmountD)-Sum(AmountC) as Balance from tblVoucher where " + StrCCSQL1 + " Optional=0  " + strVchtypeID + " and LedgerID=" + LedgerID + StrSqlDate);

                if (!rs.eof())
                {
                    if (rs.fields("Balance") != null)
                        returnvalue = ToDouble(rs.fields("Balance"));
                }
                return returnvalue;
            }
            catch (Exception ex)
            {
                WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return 0;
            }
        }

        public bool CheckUserPermission(UserActivity Activity, string WindowCaption, bool BlnSupressMessage = false, string CustomAccessString = "")
        {
            // If Exists in string=It is to be rejected
            // User Actions exists in AccessString = This action is to rejected for this user
            // CheckUserPermission = True
            // Exit Function

            if (Global.gblUserID == 1 | Global.gblUserID == 0)
            {
                return true;
            }

            if (Global.gblUserGroupID == 1)
            {
                return true;
            }

            //clsfeaturecontrol FC = new clsfeaturecontrol();

            Common Comm = new Common();

            DataTable dt = Comm.fnGetData("Select AccessLevel From tblUserGroupMaster Where ID=" + Global.gblUserGroupID.ToString()).Tables[0];

            string MyMstrAccessString = "";

            if (dt.Rows.Count > 0)
                MyMstrAccessString = dt.Rows[0]["AccessLevel"].ToString();

            if (Strings.Left(MyMstrAccessString, 1) != "Ü")
                MyMstrAccessString = "^" + MyMstrAccessString.ToUpper();

            MyMstrAccessString = Strings.Replace(MyMstrAccessString.ToUpper(), "Ü", "^");

            if (Strings.Trim(CustomAccessString) != "")
                MyMstrAccessString = Strings.Replace(Strings.UCase(CustomAccessString), "Ü", "^");

            recheckrights:
            if (Activity == UserActivity.new_Entry || Activity == UserActivity.UpdateEntry || Activity == UserActivity.Delete_Entry || Activity == UserActivity.CancelEntry)
            {
            }

            if (WindowCaption == "0Key")
            {
                return true;
            }
            if (Strings.Trim(CustomAccessString) == "")
            {
                if ((Global.gblUserName.ToString() == "ADMIN" || Global.gblUserName.ToString() == "DIGIPOS"))
                {
                    return true;
                }
            }

            if (Strings.Left(MyMstrAccessString, 1) != "><")
                MyMstrAccessString = "><" + Strings.UCase(MyMstrAccessString);
            MyMstrAccessString = Strings.UCase(MyMstrAccessString);
            WindowCaption = Strings.UCase(WindowCaption);
            if (Strings.InStr(1, MyMstrAccessString, "><" + WindowCaption + "|") == 0)
            {
                if (BlnSupressMessage == false)
                    Comm.MessageboxToasted("User Permission Module", "User Permission denied for all Activities.");

                return false;
            }

            string Stractivity;
            string[] SPLITSTR;
            SPLITSTR = Strings.Split(Strings.UCase(MyMstrAccessString), "><" + WindowCaption + "|");
            Stractivity = "|" + Strings.Left(SPLITSTR[1], Strings.InStr(1, SPLITSTR[1], "><") - 1);


            if (Activity == UserActivity.Delete_Entry)
            {
                if (Strings.InStr(1, Stractivity, "|D") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for Deletion.", Constants.vbCritical);
                    return false;
                }
            }
            else if (Activity == UserActivity.CancelEntry)
            {
                if (Strings.InStr(1, Stractivity, "|C") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for Cancellation.", Constants.vbCritical);
                    return false;
                }
            }
            else if (Activity == UserActivity.UpdateEntry)
            {
                if (Strings.InStr(1, Stractivity, "|E") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for Updation.", Constants.vbCritical);
                    return false;
                }
            }
            else if (Activity == UserActivity.new_Entry)
            {
                if (Strings.InStr(1, Stractivity, "|N") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for Creating New.", Constants.vbCritical);
                    return false;
                }

                if (Global.blnTrialExpired)
                {
                    Interaction.MsgBox("30 day Trial Period Expired. you can't create new entries", Constants.vbCritical, "Permission");
                    return false;
                }
            }
            else if (Activity == UserActivity.DateChange)
            {
                if (Strings.InStr(1, Stractivity, "|A") > 0)
                    return false;
            }
            else if (Activity == UserActivity.Printinvoice)
            {
                if (Strings.InStr(1, Stractivity, "|P") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for printing.", Constants.vbCritical);
                    return false;
                }
            }
            else if (Activity == UserActivity.WaitForAuthorisation)
            {
                if (Strings.InStr(1, Stractivity, "|W") > 0)
                    return false;
            }
            else if (Activity == UserActivity.DisplayWindow)
            {
                if (Strings.InStr(1, Stractivity, "|V") > 0)
                {
                    if (BlnSupressMessage == false)
                        Interaction.MsgBox("User Permission denied for Updation.", Constants.vbCritical);
                    return false;
                }
            }
            
            return true;

        }

        public void writeuserlog(UserActivity useractivity, string NewData, string OldData, string ActionDescription, int VchTypeId, int ParentVchTypeId, string UniqueField, int RefID, string WindowName)
        {
            try
            {
                string StrAction = "";
                sqlControl cn = new sqlControl();


                switch (useractivity)
                {
                    case UserActivity.new_Entry:
                        {
                            StrAction = "Insert";
                            break;
                        }

                    case UserActivity.UpdateEntry:
                        {
                            StrAction = "Update";
                            break;
                        }

                    case UserActivity.Delete_Entry:
                        {
                            StrAction = "Delete";
                            break;
                        }

                    case UserActivity.CancelEntry:
                        {
                            StrAction = "Cancel";
                            break;
                        }

                    case UserActivity.DisplayWindow:
                        {
                            StrAction = "Dislpay";
                            break;
                        }

                    case UserActivity.Printinvoice:
                        {
                            StrAction = "Print";
                            break;
                        }

                    case UserActivity.DateChange:
                        {
                            StrAction = "DateChanged";
                            break;
                        }

                    case UserActivity.WaitForAuthorisation:
                        {
                            StrAction = "Authorisation";
                            break;
                        }

                    case UserActivity.LoggedIn:
                        {
                            StrAction = "LoggedIn";
                            break;
                        }

                    case UserActivity.Loggedout:
                        {
                            StrAction = "Loggedout";
                            break;
                        }
                }

                cn.Execute(" exec dbo.fnInsertUserLog  '" + NewData.Replace("'", "''") + "','" + OldData.Replace("'", "''") + "','" + StrAction + "','" + ActionDescription.Replace("'", "''") + "', " + VchTypeId + " ," + ParentVchTypeId + ", '" + UniqueField + "' , " + RefID + ", " + Global.gblUserID + " ,'" + Global.ComputerName.Replace("'", "''") + "','" + WindowName.Replace("'", "''") + "'");
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "WrituserLog");
            }
        }

        //public bool CheckUserPermission(string WindowName = "", PermissionType Permission = PermissionType.View)
        //{
        //    try 
        //    {
        //        UspGetUserGroupMasterInfo GetuserInfo = new UspGetUserGroupMasterInfo();
        //        clsUserGroup clsuser = new clsUserGroup();

        //        sqlControl rs = new sqlControl();

        //        String strAccessLevel = "";

        //        DataTable dtLoad = new DataTable();
        //        GetuserInfo.GroupID = Convert.ToDecimal(Global.gblUserGroupID);
        //        GetuserInfo.TenantID = Convert.ToDecimal(Global.gblTenantID);
        //        dtLoad = clsuser.GetUserGroupMaster(GetuserInfo);
        //        if (dtLoad.Rows.Count > 0)
        //        {
        //            strAccessLevel = dtLoad.Rows[0]["AccessLevel"].ToString();
        //        }
        //        return true;
        //    }

        //    catch 
        //    {
        //        return false;
        //    }
        //}

        public bool Transferdatabase(sqlControl cn, string SRCDbName, string DestDbName, string DestDbpath)
        {
            try
            {
            string strQuery;
            strQuery = "password=#infinitY@279;User ID=sa;Initial Catalog=Startup;Data Source=" + Global.SqlServerName;
            sqlControl cnn = new sqlControl(strQuery);

            // =======================Default path===========
            if (DestDbpath == "")
            {
                string Sql;
                string SqlFolderName;
                SqlFolderName = Global.SqlServerName.Replace(@"\", "" + @"\");
                SqlFolderName = Strings.Replace(SqlFolderName, ".", "");
                SqlFolderName = Strings.Replace(SqlFolderName, ",", "");

                DestDbpath = @"C:\DIGIDATA\Data\";
            }
            // =======================Default path===========
            // Start Main script
            sqlControl Rs = new sqlControl();
            string MDFLOgicalName = "";
            string LDFLOgicalName = "";
            string SRCMDFLOgicalName = "";
            string SRCLDFLOgicalName = "";
            string SRCPath = "";
            DestDbpath = Strings.Replace(DestDbpath, ".", "");
            DestDbpath = Strings.Replace(DestDbpath, ",", "");
            MDFLOgicalName = DestDbpath + DestDbName + "_DAT.mdf";
            LDFLOgicalName = DestDbpath + DestDbName + "_LOG.ldf";
            Rs = null/* TODO Change to default(_) if this is not a reference type */;
            cnn.Open("  SELECT     name, filename From sys.sysdatabases where Name='" + SRCDbName + "'");
            // getting physical file Name
            if (!cnn.eof())
            {
                SRCPath = cnn.fields("FileName");
                SRCPath = Strings.Left(SRCPath, Strings.InStrRev(SRCPath, @"\"));
            }

            cn.Execute("BACKUP DATABASE [" + SRCDbName + @"] TO  DISK = N'C:\DIGIDATA\Data\" + SRCDbName + ".BAK' WITH NOFORMAT, INIT,  NAME = N'" + SRCDbName + "-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 30");
            // CN.Execute " CREATE DATABASE " & DestDbName
            Rs = null/* TODO Change to default(_) if this is not a reference type */;
            cnn.Open(@" Restore filelistonly FROM         disk = 'C:\DIGIDATA\Data\" + SRCDbName + ".BAK'");
            if (!cnn.eof())
            {
                SRCMDFLOgicalName = cnn.fields("LogicalName");
                cnn.MoveNext();
                SRCLDFLOgicalName = cnn.fields("LogicalName");
            }
            cn.Execute(" RESTORE DATABASE [" + DestDbName + @"] FROM  DISK = N'C:\DIGIDATA\Data\" + SRCDbName + ".BAK' WITH  FILE = 1, " + " MOVE N'" + SRCMDFLOgicalName + "' TO N'" + MDFLOgicalName + "'," + " MOVE N'" + SRCLDFLOgicalName + "' TO N'" + LDFLOgicalName + "',  NOUNLOAD,  REPLACE,  STATS = 10");

            // getting physical file Name
            cn.Execute(" ALTER DATABASE [" + DestDbName + "] MODIFY FILE (NAME=N'" + SRCMDFLOgicalName + "', NEWNAME=N'" + DestDbName + "_DAT')  ");
            cn.Execute(" ALTER DATABASE [" + DestDbName + "] MODIFY FILE (NAME=N'" + SRCLDFLOgicalName + "', NEWNAME=N'" + DestDbName + "_Log') ");


            return true;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return false;
            }
        }

        private bool query(string que)
        {
            try
            {
                SqlConnection con = GetDBConnection();
                SqlCommand cmd = new SqlCommand(que, con);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return false;
            }
        }

        public bool CreateZipFile(string FilePath, string FileName, string ZipFilePath, string ZipFileName)
        {
            try
            {
                if (File.Exists(FilePath + FileName) == true)
                {
                    ZipFileName = FilePath + ZipFileName;

                    if (ZipFileName == "")
                        return false;
                    ZipFile.CreateFromDirectory(FilePath + FileName, ZipFileName);
                    File.Copy(ZipFileName, ZipFilePath + ZipFileName);
                    File.Delete(ZipFileName);
                    File.Delete(FilePath + FileName);
                }

                return true;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return false;
            }
        }

        public bool BACKUPDB(string str, string BackupCompany = "", string BackupPath = "")
        {
            try
            {
                if (BackupCompany == "")
                {
                    Interaction.MsgBox("Select a company to backup", MsgBoxStyle.Information);
                    return false;
                }
                if (BackupPath == "")
                {
                    Interaction.MsgBox("Select a filename to backup", MsgBoxStyle.Information);
                    return false;
                }
                if (File.Exists(BackupPath) == true)
                {
                    Interaction.MsgBox("File already exists. Please choose a new file to backup.", MsgBoxStyle.Information);
                    return false;
                }
                // MsgBox("1")

                if (Directory.Exists(@"C:\SQLBK\" + BackupCompany) == true)
                    Directory.Delete(@"C:\SQLBK\" + BackupCompany);
                // MsgBox("2")
                if (Directory.Exists(@"C:\SQLBK\" + BackupCompany) == false)
                    Directory.CreateDirectory(@"C:\SQLBK\" + BackupCompany);
                // MsgBox("3")
                if (Directory.Exists(@"C:\SQLBK\" + BackupCompany) == false)
                {
                    Interaction.MsgBox("Path not found. Could not create temporary file or directory for backup creation.");
                    return false;
                }

                bool blnFailed = false;
                if (File.Exists(@"C:\SQLBK\" + BackupCompany + @"\" + BackupCompany + ".bak") == true)
                    File.Delete(@"C:\SQLBK\" + BackupCompany + @"\" + BackupCompany + ".bak");

                if (query("backup database " + BackupCompany + " to disk='" + @"C:\SQLBK\" + BackupCompany + @"\" + BackupCompany + ".bak'") == true)
                {
                    if (CreateZipFile(@"C:\SQLBK\" + BackupCompany + @"\" , BackupCompany + ".bak'", BackupPath, BackupCompany + DateTime.Now.ToString("ddMMMyyyy_hh_mm_ss_tt") + ".zip'") == false)
                    {
                        Interaction.MsgBox("Failed to backup database. Could not create file to " + BackupPath);
                        blnFailed = true;
                    }
                    else
                    {
                        Interaction.MsgBox("Backup process completed successfully. File copied to " + BackupPath, MsgBoxStyle.Information);
                        blnFailed = false;
                    }
                }
                else
                {
                    Interaction.MsgBox("Failed to backup database. Backup process aboted abnormally.");
                    blnFailed = true;
                }

                if (blnFailed)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return false;
            }
        }

        public void RESTOREDB(string bDNAME, string SRCDbPATH, string DestDbPath)
        {
            try
            {
                string Cnstring;
                string mPrimaryServer = Global.SqlServerName;

                Cnstring = "Data Source=" + mPrimaryServer + ";Initial Catalog=master;User ID=sa;Password=#infinitY@279";
                sqlControl cnn = new sqlControl(Cnstring);

                string SqlFolderName;
                if (DestDbPath == "")
                {
                    SqlFolderName = mPrimaryServer.Replace(@"\", "") + @"\";
                    if (Directory.Exists(@"C:\DIGIDATA\Data\" + SqlFolderName) == false)
                        Directory.CreateDirectory(@"C:\DIGIDATA\Data\" + SqlFolderName);
                    DestDbPath = @"C:\DIGIDATA\Data\" + SqlFolderName;
                }
                string MDFLogicalName;
                string LDFLogicalName;

                cnn.Open(" Restore  filelistonly from  disk = '" + SRCDbPATH + "'");
                if (!cnn.eof())
                {
                    MDFLogicalName = cnn.fields("logicalName");
                    cnn.MoveNext();
                    LDFLogicalName = cnn.fields("logicalName");
                    cnn.Execute("RESTORE DATABASE [" + bDNAME + "] from DISK =N'" + SRCDbPATH + "' WITH FILE = 1, MOVE N'" + MDFLogicalName + "' TO N'" + DestDbPath + MDFLogicalName + ".DAT', " + " MOVE N'" + LDFLogicalName + "' TO N'" + DestDbPath + LDFLogicalName + ".ldf', NOUNLOAD, REPLACE,STATS = 10");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("First Run " + ex.Message, Global.gblMessageCaption);
            }
        }

        public void ResizeForm(Form frm)
        {
            frm.Width = 1000;
            frm.Height = 1000;
        }

        public decimal ToDecimal(string Number)
        {
            try
            {
                if (Number == null) return 0;

                decimal ParsedNumber = 0;

                bool canConvert = decimal.TryParse(Number, out ParsedNumber);
                if (canConvert == true)
                    return ParsedNumber;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }
        public decimal ToDecimal(object Number)
        {
            try
            {
                if (Number == null) return 0;

                decimal ParsedNumber = 0;

                bool canConvert = decimal.TryParse(Number.ToString(), out ParsedNumber);
                if (canConvert == true)
                    return ParsedNumber;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public double ToDouble(string Number)
        {
            try
            {
                if (Number == null) return 0;

                double ParsedNumber = 0;

                bool canConvert = double.TryParse(Number, out ParsedNumber);
                if (canConvert == true)
                    return ParsedNumber;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }
        public double ToDouble(object Number)
        {
            try
            {
                if (Number == null) return 0;

                double ParsedNumber = 0;

                bool canConvert = double.TryParse(Number.ToString(), out ParsedNumber);
                if (canConvert == true)
                    return ParsedNumber;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public Int32 ToInt32(string Number)
        {
            try
            {
                if (Number == null) return 0;
                if (Number == "") return 0;

                Int32 ParsedNumber = 0;

                if (Number.ToString().Contains("."))
                {
                    return decimal.ToInt32(Convert.ToDecimal(Number.ToString()));
                }
                else
                {
                    bool canConvert = Int32.TryParse(Number, out ParsedNumber);
                    if (canConvert == true)
                        return ParsedNumber;
                    else
                        return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
        public Int32 ToInt32(object Number)
        {
            try
            {
                if (Number == null) return 0;

                Int32 ParsedNumber = 0;

                if (Number.ToString().Contains("."))
                {
                    return decimal.ToInt32(Convert.ToDecimal(Number.ToString()));
                }
                else
                { 
                    bool canConvert = Int32.TryParse(Number.ToString(), out ParsedNumber);
                    if (canConvert == true)
                        return ParsedNumber;
                    else
                        return 0;
                } 
            }
            catch
            {
                return 0;
            }
        }



        //Description : Format the Amount using Supplied Values
        public string FormatAmt(double myValue, string myFormat)
        {
            //https://msdn.microsoft.com/en-us/library/microsoft.visualbasic.strings.format(v=vs.110).aspx
            //DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss")
            //"29-Jan-2018 10:16:16"
            //FormatAmt = String.Format("{0:N3}", Val(myValue))
            //FormatAmt = Format(Val(myValue), "f" & DCSApp.Gdecimal.ToString & "")

            if (myFormat == "")
                myFormat = "#.00";
            return ToDouble(myValue).ToString(myFormat);
        }
        public string FormatAmt(decimal myValue, string myFormat)
        {
            //https://msdn.microsoft.com/en-us/library/microsoft.visualbasic.strings.format(v=vs.110).aspx
            //DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss")
            //"29-Jan-2018 10:16:16"
            //FormatAmt = String.Format("{0:N3}", Val(myValue))
            //FormatAmt = Format(Val(myValue), "f" & DCSApp.Gdecimal.ToString & "")

            if (myFormat == "")
                myFormat = "#.00";
            return ToDecimal(myValue).ToString(myFormat);
        }

        //Description : Format Values like Currency/Quantity to the Formated Values asper App Settings
        public string FormatValue(double myValue, bool blnIsCurrency = true, string sMyFormat = "")
        {
            string myFormat = "";
            if (blnIsCurrency == true)
                myFormat = AppSettings.CurrDecimalFormat;
            else
                myFormat = AppSettings.QtyDecimalFormat;

            if (myFormat == "")
                myFormat = "#.00";

            if (sMyFormat != "")
                myFormat = sMyFormat;

            return ToDouble(myValue).ToString(myFormat);
        }
        public string FormatValue(decimal myValue, bool blnIsCurrency = true, string sMyFormat = "")
        {
            string myFormat = "";
            if (blnIsCurrency == true)
                myFormat = AppSettings.CurrDecimalFormat;
            else
                myFormat = AppSettings.QtyDecimalFormat;

            if (myFormat == "")
                myFormat = "#.00";

            if (sMyFormat != "")
                myFormat = sMyFormat;

            return ToDecimal(myValue).ToString(myFormat);
        }

        //Description : Convert to Int32 of Decimal Value
        public int ConvertI32(decimal dVal)
        {
            return Convert.ToInt32(dVal);
        }

        public bool CheckNumeric(object sender, KeyPressEventArgs e, bool NegativeAllowed = false, bool IsLeave = false)
        {
            try
            {
                if (IsLeave == true)
                {
                    string stringNumber = ((TextBox)sender).Text.ToString();
                    bool isNumber = int.TryParse(stringNumber, out _);
                    return isNumber;
                }
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
                {
                    return true;
                }
                if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                {
                    return true;
                }
                if (NegativeAllowed == true)
                {
                    if ((e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static Image ScaleByPercent(Image imgPhoto, int Percent)
        {
            float nPercent = ((float)Percent / 100);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight,
                                     PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                    imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public void LoadBGImage(Form frm, PictureBox picBackground)
        {
            picBackground.WaitOnLoad = false;
            picBackground.Size = frm.Size;
            picBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            picBackground.LoadAsync(Application.StartupPath + @"\Resources\WallPaper2.jpeg");
        }

        public bool TransparentControls(Control parentctrl)
        {
            try
            {
                foreach (Control ctrl in parentctrl.Controls)
                {
                    if (ctrl.GetType() != typeof(Form))
                    {
                        if (GetControlStyle(ctrl, ControlStyles.SupportsTransparentBackColor) == true)
                        {
                            ctrl.BackColor = Color.Transparent;
                            if (ctrl.Controls.Count > 0)
                            {
                                TransparentControls(ctrl);
                            }
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SetControlColours(Control parentctrl)
        {
            try
            {
                foreach (Control ctrl in parentctrl.Controls)
                {
                    if (ctrl.GetType() == typeof(Label))
                    {
                        ctrl.ForeColor = Color.Black;
                    }
                    if (ctrl.Controls.Count > 0)
                    {
                        SetControlColours(ctrl);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GetControlStyle(Control control, ControlStyles flags)
        {
            Type type = control.GetType();
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo method = type.GetMethod("GetStyle", bindingFlags);
            object[] param = { flags };
            return (bool)method.Invoke(control, param);
        }

        public int gfnGetNextSerialNo(string sTableName, string sColumnName, string sCondition = "")
        {
            // --------------------------------------------------------- >>
            // Description: gfnGetNextSerialNo, is to get next serial number using the tablename, columnname and if have any conditions.
            // Created By : Dipu Joseph
            // Create On  : 03-09-2021
            // Last Edited By :
            // Last Edited On :
            // --------------------------------------------------------- >>

            using (var sqlConn = GetDBConnection())
            {
                string sQuery = "";
                int iResult = 1;

                sQuery = "SELECT ISNULL(MAX(" + sColumnName + "), 0) + 1 FROM " + sTableName;
                if (sCondition != "")
                {
                    sQuery = sQuery + " WHERE " + sCondition;
                }

                SqlDataAdapter daSerial = new SqlDataAdapter(sQuery, sqlConn);
                DataTable dtSerial = new DataTable();
                daSerial.Fill(dtSerial);
                if (dtSerial.Rows.Count > 0)
                {
                    iResult = Convert.ToInt32(dtSerial.Rows[0][0].ToString());
                }
                sqlConn.Close();
                return iResult;
            }
        }

        public void WritetoErrorLog(Exception ex, string sEventTarget)
        {
            // --------------------------------------------------------- >>
            // Description: WritetoErrorLog, is to write the error logs from forms and project.
            // Created By : Dipu Joseph
            // Create On  : 03-09-2021
            // Last Edited By :
            // Last Edited On :
            // --------------------------------------------------------- >>

            try
            {
                string sDate = DateTime.Today.ToString("dd-MMM-yyyy");
                string sFolderName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\LogError";

                if (!Directory.Exists(sFolderName))
                    Directory.CreateDirectory(sFolderName);

                string sErrFileName = sFolderName + "\\" + "iError_" + sDate + ".txt";
                if (File.Exists(sErrFileName) == false)
                {
                    File.Create(sErrFileName);
                }

                using (StreamWriter sw = File.AppendText(sErrFileName))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine("EventTarget:" + sEventTarget);
                    sw.WriteLine(ex.Message);
                    sw.WriteLine(Convert.ToString(ex.InnerException));
                    sw.WriteLine(ex.Source);
                    sw.WriteLine("User Name: " + "Administrator");
                    sw.WriteLine("------------------------------------------------------------------------------");
                }
            }
            catch (Exception)
            {
            }
        }

        public void WritetoSqlErrorLog(DataTable dtErrorResult, string sUsername)
        {
            // --------------------------------------------------------- >>
            // Description: WritetoSqlErrorLog, is to write the error logs from sql stored procedures
            // Created By : Dipu Joseph
            // Create On  : 03-09-2021
            // Last Edited By :
            // Last Edited On :
            // --------------------------------------------------------- >>

            try
            {
                string sDate = DateTime.Today.ToString("dd-MMM-yyyy");

                string sErrFileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\LogError" + "\\" + "SqlError_" + sDate + ".txt";
                if (File.Exists(sErrFileName) == false)
                {
                    File.Create(sErrFileName);
                }

                using (StreamWriter sw = File.AppendText(sErrFileName))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine("ERROR_NUMBER:" + dtErrorResult.Rows[0]["ErrorNumber"].ToString());
                    sw.WriteLine("ERROR_STATE:" + dtErrorResult.Rows[0]["ErrorState"].ToString());
                    sw.WriteLine("ERROR_SEVERITY:" + dtErrorResult.Rows[0]["ErrorSeverity"].ToString());
                    sw.WriteLine("ERROR_PROCEDURE:" + dtErrorResult.Rows[0]["ErrorProcedure"].ToString());
                    sw.WriteLine("ERROR_LINE:" + dtErrorResult.Rows[0]["ErrorLine"].ToString());
                    sw.WriteLine("ERROR_MESSAGE:" + dtErrorResult.Rows[0]["ErrorMessage"].ToString());
                    sw.WriteLine("User Name: " + sUsername);
                    sw.WriteLine("------------------------------------------------------------------------------");
                }
            }
            catch (Exception)
            {
            }
        }

        public DataSet fnGetData(string sQuery)
        {
            // --------------------------------------------------------- >>
            // Description: fnGetData, is to get data from database using sql script (query/procedure)
            // Created By : Dipu Joseph
            // Create On  : 03-09-2021
            // Last Edited By :
            // Last Edited On :
            // --------------------------------------------------------- >>
            try
            {
                SqlConnection sqlconn = GetDBConnection();
                //MessageBox.Show(sqlconn.ConnectionString);
                try
                {
                    //sqlconn.Open();
                    string sStr = sQuery;
                    SqlDataAdapter sqlda = new SqlDataAdapter(sStr, sqlconn);
                    DataSet ds = new DataSet();
                    sqlda.Fill(ds);
                    sqlda.Dispose();
                    return ds;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (sqlconn != null)
                        sqlconn.Close();
                    return new DataSet();
                }
                finally
                {
                    if (sqlconn != null)
                        sqlconn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataSet();
            }
        }

        public int fnExecuteNonQuery(string sQuery, bool blnShowErrorMessage = true, SqlConnection sqlconn = null, SqlTransaction trans = null)
        {
            int iRet = 0;
            if (sqlconn == null)
                sqlconn = GetDBConnection();

            try
            {
                //sqlconn.Open();
                string sStr = sQuery;

                SqlCommand sqlCmd;

                if (trans == null)
                    sqlCmd = new SqlCommand(sStr, sqlconn);
                else
                    sqlCmd = new SqlCommand(sStr, sqlconn, trans);

                iRet = sqlCmd.ExecuteNonQuery();

                if (trans == null)
                    sqlconn.Close();

                return iRet;
            }
            catch (Exception ex)
            {
                if (sqlconn != null)
                    sqlconn.Close();

                if (blnShowErrorMessage == true)
                    MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

                return -1;
            }
            finally
            {
                if (trans == null)
                {
                    if (sqlconn != null)
                        sqlconn.Close();
                }
            }
        }

        public void LoadGrdiControl(Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl ctl, DataTable dtLoad, bool bShowFilterbar = false, bool bAllowEdit = false, string sColWidth = "",int isetfrmwidth=0)
        {
            int iTotGridWidth = 0, idtColCount = 0, iGridColWidth = 0, iSerilBalWidth = 0;
            Syncfusion.GridHelperClasses.GridDynamicFilter filter = new Syncfusion.GridHelperClasses.GridDynamicFilter();


            ctl.DataSource = null;
            if (dtLoad.Rows.Count > 0)
            {
                ctl.DataSource = dtLoad;
                ctl.Refresh();

                ctl.TopLevelGroupOptions.ShowAddNewRecordBeforeDetails = false;
                ctl.TopLevelGroupOptions.ShowCaption = false;
                ctl.NestedTableGroupOptions.ShowAddNewRecordBeforeDetails = false;
                ctl.TableModel.EnableLegacyStyle = false;
                ctl.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.Office2016White;
                ctl.TableOptions.ListBoxSelectionMode = SelectionMode.MultiExtended;
                ctl.TableControl.DpiAware = true;
                ctl.WantTabKey = false;

                // Settings
                ctl.TopLevelGroupOptions.ShowFilterBar = bShowFilterbar;
                if (bAllowEdit == false)
                    ctl.ActivateCurrentCellBehavior = GridCellActivateAction.None;
                else
                    ctl.ActivateCurrentCellBehavior = GridCellActivateAction.ClickOnCell;

                iTotGridWidth = ctl.Width;
                idtColCount = dtLoad.Columns.Count;
                iGridColWidth = iTotGridWidth / (dtLoad.Columns.Count - 1);

                //Added by Anjitha 14/03/2022 5:33 PM
                if (sColWidth != "")
                {
                    SetGridColumnWidth(ctl, iTotGridWidth, sColWidth, bShowFilterbar, isetfrmwidth);
                }
                else
                {
                    for (int i = 0; i < ctl.TableDescriptor.Columns.Count; i++)
                    {
                        if (i == 0)
                            ctl.TableDescriptor.Columns[i].Width = 0;
                        else
                        {
                            if (ctl.TableDescriptor.Columns[i].HeaderText == "Serial No")
                            {
                                ctl.TableDescriptor.Columns[i].Width = iGridColWidth / 2;
                                iSerilBalWidth = iGridColWidth / 2;
                                ctl.TableDescriptor.Columns[i].Appearance.AnyRecordFieldCell.HorizontalAlignment = Syncfusion.Windows.Forms.Grid.GridHorizontalAlignment.Center;
                            }
                            else if (i == ctl.TableDescriptor.Columns.Count - 1)
                            {
                                ctl.TableDescriptor.Columns[i].Width = iGridColWidth - 20;
                            }
                            else
                            {
                                ctl.TableDescriptor.Columns[i].Width = iGridColWidth + iSerilBalWidth;
                                iSerilBalWidth = 0;
                            }
                            filter.WireGrid(ctl);
                            ctl.TableDescriptor.Columns[i].AllowFilter = bShowFilterbar;
                            //}
                        }
                    }
                }

                Syncfusion.Grouping.Record rec = ctl.Table.Records[0];
                ctl.TableModel.Selections.Clear();

                ctl.TableModel.Selections.Add(GridRangeInfo.Row(rec.GetRowIndex()));
                int rowIndex = ctl.Table.DisplayElements.IndexOf(rec);
                ctl.TableControl.CurrentCell.MoveTo(rowIndex, 1, GridSetCurrentCellOptions.ScrollInView);
            }
            ctl.Refresh();
        }

        private void SetGridColumnWidth(Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl ctrl, int iTotalGridwidt, string sColumnGridWidth = "", bool bShowFilterbar = false,int isetfrmwidth=0)
        {
            try
            {
                string[] sArrColWidth;
                int[] iArrayWidth;
                int iColWidthTot = 0;
                Syncfusion.GridHelperClasses.GridDynamicFilter filter = new Syncfusion.GridHelperClasses.GridDynamicFilter();
                //if (iTotalGridwidt == 840)
                //    iTotalGridwidt = 1306;
                if (iTotalGridwidt < 1000)
                    iTotalGridwidt = isetfrmwidth - 230;
                if (sColumnGridWidth != "")
                {
                    sArrColWidth = sColumnGridWidth.Split(',');
                    iArrayWidth = Array.ConvertAll(sArrColWidth, s => int.Parse(s));
                    int isum = iArrayWidth.Sum();
                    iColWidthTot = isum + 1;

                    for (int j = 0; j < sArrColWidth.Length; j++)
                    {
                        if (sArrColWidth[j].ToString() == "-1")
                        {
                            ctrl.TableDescriptor.Columns[j].Width = (iTotalGridwidt - iColWidthTot) - 5;
                        }
                        else
                        {
                            ctrl.TableDescriptor.Columns[j].Width = Convert.ToInt32(sArrColWidth[j].ToString());
                        }

                        ctrl.TableDescriptor.Columns[j].AllowFilter = bShowFilterbar;
                        filter.WireGrid(ctrl);
                    }
                    if (iColWidthTot < iTotalGridwidt)
                        ctrl.TableControl.HScrollBehavior = Syncfusion.Windows.Forms.Grid.GridScrollbarMode.Disabled;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "SetGridColumn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void LoadComboboxControl(ComboBox ctl, string sTableName, string sNameField, string sCodeField, string sDummy = "", string sCondition = "", string sOrderField = "", string sSortBy = "")
        {
            SqlConnection sqlConn = GetDBConnection();
            sqlConn.Open();
            string sQuery = "";

            if (sDummy != "")
            {
                sQuery = "SELECT 0 as  " + sCodeField + ",'" + sDummy + "' as " + sNameField + " FROM " + sTableName + " UNION ";
            }

            sQuery = sQuery + "SELECT DISTINCT " + sCodeField + " as " + sCodeField + "," + sNameField + " as " + sNameField + " FROM " + sTableName;
            if (sCondition != "")
            {
                sQuery = sQuery + " WHERE " + sCondition;
            }

            if (sOrderField != "")
            {
                sQuery = sQuery + " ORDER BY " + sOrderField;
                if (sSortBy != "")
                {
                    sQuery = sQuery + " " + sSortBy.ToUpper();
                }
            }

            SqlDataAdapter daPop = new SqlDataAdapter(sQuery, sqlConn);
            DataTable dtPop = new DataTable();
            daPop.Fill(dtPop);
            sqlConn.Close();

            ctl.DataSource = null;
            if (dtPop.Rows.Count > 0)
            {
                ctl.DataSource = dtPop;
                ctl.DisplayMember = sNameField;
                ctl.ValueMember = sCodeField;
            }
        }

        public string GetCheckedData(CheckedListBox ctl)
        {
            try
            {
                string returnvalue = "";

                foreach (var obj in ctl.CheckedItems)
                {
                    DataRowView castedItem = obj as DataRowView;
                    string comapnyName = castedItem["CompanyName"].ToString();
                    string id = castedItem["ID"].ToString();

                    returnvalue += id + ", ";
                }

                return returnvalue;
            }
            catch(Exception ex)
            {
                WritetoErrorLog(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
        }

        public void LoadControl(object ctl, DataTable dtLoad, string Query = "", bool blnMultiSelect = false, bool blnAllowSelectAll = false, string sDisplayField = "", string sValueField = "", bool blnMultiColumn = true, bool btnHideValueMember = false, bool blnProgress = false, bool BlnAppendToCurrent = false, bool BlnAutoComplete = true, bool BlnFastFill = false, bool bShowGridFilterbar = false, bool bAllowGridEdit = false)
        {
            // --------------------------------------------------------- >>
            // Description: LoadControl, is for load or fill the control in a form from database
            // Created By : Dipu Joseph
            // Create On  : 13-09-2021
            // Last Edited By :
            // Last Edited On :
            // --------------------------------------------------------- >>

            try
            {
                DataTable dtLoadQuery = new DataTable();
                Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl CtlGridGroupControl = new Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl();
                Syncfusion.WinForms.ListView.SfComboBox CtlSFComboBox = new Syncfusion.WinForms.ListView.SfComboBox();
                Syncfusion.Windows.Forms.Tools.ComboDropDown cboddl = new Syncfusion.Windows.Forms.Tools.ComboDropDown();
                Syncfusion.Windows.Forms.Tools.MultiColumnComboBox multcbo = new Syncfusion.Windows.Forms.Tools.MultiColumnComboBox();

                ComboBox cbo = new ComboBox();

                int iTotGridWidth = 0, idtColCount = 0, iGridColWidth = 0, iSerilBalWidth = 0;
                Syncfusion.GridHelperClasses.GridDynamicFilter filter = new Syncfusion.GridHelperClasses.GridDynamicFilter();

                if (Query == "")
                    dtLoadQuery = dtLoad;
                else
                    dtLoadQuery = fnGetData(Query).Tables[0];

                if (ctl.GetType().ToString().ToUpper().Contains("GRIDGROUPINGCONTROL") == true)
                    CtlGridGroupControl = (Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl)ctl;
                else if (ctl.GetType().ToString().ToUpper().Contains("SFCOMBOBOX") == true)
                    CtlSFComboBox = (Syncfusion.WinForms.ListView.SfComboBox)ctl;
                else if (ctl.GetType().FullName.ToString().ToUpper() == "SYNCFUSION.WINDOWS.FORMS.TOOLS.MULTICOLUMNCOMBOBOX")
                    multcbo = (Syncfusion.Windows.Forms.Tools.MultiColumnComboBox)ctl;
                else if (ctl.GetType().FullName.ToString().ToUpper() == "SYSTEM.WINDOWS.FORMS.COMBOBOX")
                    cbo = (ComboBox)ctl;

                if (BlnAppendToCurrent == false)
                {
                    if (ctl.GetType().ToString().ToUpper().Contains("GRIDGROUPINGCONTROL") == true)
                        CtlGridGroupControl.DataSource = null;
                    else if (ctl.GetType().ToString().ToUpper().Contains("SFCOMBOBOX") == true)
                        CtlSFComboBox.DataSource = null;
                    else if (ctl.GetType().FullName.ToString().ToUpper() == "SYNCFUSION.WINDOWS.FORMS.TOOLS.MULTICOLUMNCOMBOBOX")
                        multcbo.DataSource = null;
                    else if (ctl.GetType().FullName.ToString().ToUpper() == "SYSTEM.WINDOWS.FORMS.COMBOBOX")
                        cbo.DataSource = null;

                }

                if (dtLoadQuery.Rows.Count > 0)
                {
                    if (ctl.GetType().ToString().ToUpper().Contains("GRIDGROUPINGCONTROL") == true)
                    {
                        CtlGridGroupControl.DataSource = null;
                        if (dtLoad.Rows.Count > 0)
                        {
                            CtlGridGroupControl.DataSource = dtLoad;
                            CtlGridGroupControl.Refresh();

                            CtlGridGroupControl.TopLevelGroupOptions.ShowAddNewRecordBeforeDetails = false;
                            CtlGridGroupControl.TopLevelGroupOptions.ShowCaption = false;
                            CtlGridGroupControl.NestedTableGroupOptions.ShowAddNewRecordBeforeDetails = false;
                            CtlGridGroupControl.TableModel.EnableLegacyStyle = false;
                            CtlGridGroupControl.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.Office2016White;
                            CtlGridGroupControl.TableOptions.ListBoxSelectionMode = SelectionMode.MultiExtended;
                            CtlGridGroupControl.TableControl.DpiAware = true;
                            CtlGridGroupControl.WantTabKey = false;

                            // Settings
                            CtlGridGroupControl.TopLevelGroupOptions.ShowFilterBar = bShowGridFilterbar;
                            if (bAllowGridEdit == false)
                                CtlGridGroupControl.ActivateCurrentCellBehavior = GridCellActivateAction.None;
                            else
                                CtlGridGroupControl.ActivateCurrentCellBehavior = GridCellActivateAction.ClickOnCell;

                            iTotGridWidth = CtlGridGroupControl.Width;
                            idtColCount = dtLoad.Columns.Count;
                            iGridColWidth = iTotGridWidth / (dtLoad.Columns.Count - 1);

                            for (int i = 0; i < CtlGridGroupControl.TableDescriptor.Columns.Count; i++)
                            {
                                if (i == 0)
                                    CtlGridGroupControl.TableDescriptor.Columns[i].Width = 0;
                                else
                                {
                                    if (CtlGridGroupControl.TableDescriptor.Columns[i].HeaderText == "Serial No")
                                    {
                                        CtlGridGroupControl.TableDescriptor.Columns[i].Width = iGridColWidth / 2;
                                        iSerilBalWidth = iGridColWidth / 2;
                                        CtlGridGroupControl.TableDescriptor.Columns[i].Appearance.AnyRecordFieldCell.HorizontalAlignment = Syncfusion.Windows.Forms.Grid.GridHorizontalAlignment.Center;
                                    }
                                    else if (i == CtlGridGroupControl.TableDescriptor.Columns.Count - 1)
                                    {
                                        CtlGridGroupControl.TableDescriptor.Columns[i].Width = iGridColWidth - 20;
                                    }
                                    else
                                    {
                                        CtlGridGroupControl.TableDescriptor.Columns[i].Width = iGridColWidth + iSerilBalWidth;
                                        iSerilBalWidth = 0;
                                    }
                                    filter.WireGrid(CtlGridGroupControl);
                                    CtlGridGroupControl.TableDescriptor.Columns[i].AllowFilter = bShowGridFilterbar;
                                }
                            }
                        }
                    }
                    else if (ctl.GetType().ToString().ToUpper().Contains("SFCOMBOBOX") == true)
                    {
                        CtlSFComboBox.DataSource = dtLoadQuery;

                        if (CtlSFComboBox.Name == "sfcboDiscGroup" || CtlSFComboBox.Name == "sfcboDepmnt")
                        {
                            CtlSFComboBox.DisplayMember = dtLoadQuery.Columns[1].ColumnName;
                        }
                        else
                        {
                            if (sDisplayField == "")
                                CtlSFComboBox.DisplayMember = dtLoadQuery.Columns[2].ColumnName;
                            else
                                CtlSFComboBox.DisplayMember = sDisplayField;
                        }

                        if (sValueField == "")
                            CtlSFComboBox.ValueMember = dtLoadQuery.Columns[0].ColumnName;
                        else
                            CtlSFComboBox.ValueMember = sValueField;

                        if (blnMultiSelect == true)
                        {
                            CtlSFComboBox.ComboBoxMode = Syncfusion.WinForms.ListView.Enums.ComboBoxMode.MultiSelection;
                            if (blnAllowSelectAll == true)
                                CtlSFComboBox.AllowSelectAll = true;

                            //CtlSFComboBox.DropDownControl.ShowButtons = false;
                            CtlSFComboBox.ShowToolTip = true;
                            CtlSFComboBox.ToolTipOption.InitialDelay = 3000;
                            CtlSFComboBox.ToolTipOption.AutoPopDelay = 2000;
                            //CtlSFComboBox.ShowClearButton = true;

                        }
                    }
                    else if (ctl.GetType().FullName.ToString().ToUpper() == "SYNCFUSION.WINDOWS.FORMS.TOOLS.MULTICOLUMNCOMBOBOX")
                    {
                        multcbo.DataSource = dtLoadQuery;

                        if (sDisplayField == "")
                            multcbo.DisplayMember = dtLoadQuery.Columns[1].ColumnName;
                        else
                            multcbo.DisplayMember = sDisplayField;

                        if (sValueField == "")
                            multcbo.ValueMember = dtLoadQuery.Columns[0].ColumnName;
                        else
                            multcbo.ValueMember = sValueField;

                        if (blnMultiColumn == true)
                            multcbo.MultiColumn = true;

                        multcbo.ShowColumnHeader = true;
                        multcbo.AlphaBlendSelectionColor = System.Drawing.Color.LightBlue;
                        multcbo.DropDownWidth = multcbo.Width;

                        if (btnHideValueMember == true)
                            multcbo.ListBox.Grid.Model.Cols.Hidden[multcbo.ValueMember] = true;
                    }
                    else if (ctl.GetType().FullName.ToString().ToUpper() == "SYSTEM.WINDOWS.FORMS.COMBOBOX")
                    {
                        cbo.DataSource = dtLoadQuery;

                        if (sValueField == "")
                            cbo.ValueMember = dtLoadQuery.Columns[0].ColumnName;
                        else
                            cbo.ValueMember = sValueField;
                        if (sDisplayField == "")
                        {
                            if (cbo.Name == "cboDiscGroup" || cbo.Name == "cboDepmnt")
                                cbo.DisplayMember = dtLoadQuery.Columns[1].ColumnName;
                            else
                                cbo.DisplayMember = dtLoadQuery.Columns[2].ColumnName;
                        }
                        else
                            cbo.DisplayMember = sDisplayField;

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public string Val(string sText, bool bRetIsNum = true)
        {
            string sRetVal = "";

            if (sText == "")
                sRetVal = "0";
            else
            {
                sRetVal = sText;
            }
            return sRetVal;
        }

        public DataTable CompactSearch(string strQuery = "", string strSearchFieldName = "", string strSearchData = "", string strOrderBy = "")
        {
            SqlConnection sqlconn = GetDBConnection();
            try
            {
                string sQuery = strQuery;

                if (strSearchData != "")
                {
                    if (strQuery.ToLower().Contains("where") == true)
                    {
                        sQuery = sQuery + " AND ";
                    }
                    else
                    {
                        sQuery = sQuery + " WHERE ";
                    }
                    if (strSearchFieldName != "")
                    {
                        if (strSearchData != "")
                        {
                            strSearchData = strSearchData.Replace("\r\n", "");
                            sQuery = sQuery + strSearchFieldName + " LIKE " + "'%" + strSearchData + "%'";
                        }
                        else
                        {
                            strSearchData = strSearchData.Replace("\r\n", "");
                            sQuery = sQuery + strSearchFieldName + " LIKE " + "'" + strSearchData + "'";
                        }
                    }
                }

                if (strOrderBy != "")
                {
                    sQuery = sQuery + " " + strOrderBy;
                }

                SqlDataAdapter daSearch = new SqlDataAdapter(sQuery, sqlconn);
                DataTable dtCompSearch = new DataTable();

                daSearch.Fill(dtCompSearch);
                return dtCompSearch;
            }
            catch (Exception ex)
            {
                if (sqlconn != null)
                    sqlconn.Close();
                return null;
            }
            finally
            {
                if (sqlconn != null)
                    sqlconn.Close();
            }
        }

        public TreeNode GetNodeByText(TreeNodeCollection nodes, string searchtext)
        {
            TreeNode n_found_node = null;
            bool b_node_found = false;

            foreach (TreeNode node in nodes)
            {

                if (node.Text == searchtext)
                {
                    b_node_found = true;
                    n_found_node = node;

                    return n_found_node;
                }

                if (!b_node_found)
                {
                    n_found_node = GetNodeByText(node.Nodes, searchtext);

                    if (n_found_node != null)
                    {
                        return n_found_node;
                    }
                }
            }
            return null;
        }

        public string FormatSQL(string StrField)
        {
            int GStgIntDecimals;
            GStgIntDecimals = 2;
            return " case " + StrField + " when 0.0 then null else  convert(decimal(20," + GStgIntDecimals + "), " + StrField + ") end";
        }

        public int SalesPrint(string FILEpATH, string PrinterName)
        {
            ClsFileOperation FSO = new ClsFileOperation();

            string AppPath = Application.StartupPath;

            if (File.Exists(Application.StartupPath + "\\Print.exe") == false)
                Interaction.MsgBox("Print file is missing", MsgBoxStyle.Exclamation);

            Interaction.Shell(Application.StartupPath + "\\Print.exe " + PrinterName + "æ" + FILEpATH, AppWinStyle.NormalFocus);

            return 0;
        }

        public string GetCheckedNodesTextForChkCompact(TreeNodeCollection nodes)
        {
            string sCheckedNodes = "";
            foreach (System.Windows.Forms.TreeNode aNode in nodes)
            {
                //edit
                if (aNode.Checked)
                {

                    sCheckedNodes = sCheckedNodes + aNode.Text + ",";
                    //Console.WriteLine(aNode.Text);

                    //if (aNode.Nodes.Count != 0)
                    //    GetCheckedNodes(aNode.Nodes);
                }
            }
            return sCheckedNodes.Substring(0, sCheckedNodes.Length - 1);
        }

        public void MessageboxToasted(string sCaption, string sMessage, int DelayMlSeconds = 1)
        {
            //new Controls.MsgToast(sCaption, sMessage, "TOP-RIGHT", DelayMlSeconds).ShowDialog();
            Controls.MsgToast Msgtst = new Controls.MsgToast(sCaption, sMessage, "TOP-RIGHT", DelayMlSeconds);
            Msgtst.Show();
        }

        public bool IsDiscountPercentageOutofLimit(decimal dEnteredDiscountper, string strFormName, decimal dLimitValue = 99, bool bChangeLimitValtoText = true)
        {
            bool bResult = false;
            string sMessage = "";

            if (dEnteredDiscountper > dLimitValue)
            {
                if (bChangeLimitValtoText == true)
                    sMessage = "You are trying to enter the value greater than " + dLimitValue + ". Automatically changing it to " + dLimitValue + "%.";
                else
                    sMessage = "You are trying to enter the value greater than " + dLimitValue + ".";
                MessageboxToasted(strFormName, sMessage);
                bResult = true;
            }

            return bResult;
        }

        public bool IsCursorOnEmptyLine(TextBox targetTextBox)
        {
            var cursorPosition = targetTextBox.SelectionStart;
            var positionBefore = targetTextBox.Text.LastIndexOf('\n', cursorPosition == 0 ? 0 : cursorPosition - 1);
            var positionAfter = targetTextBox.Text.IndexOf('\r', cursorPosition);
            if (positionBefore == -1) positionBefore = 0;
            if (positionAfter == -1) positionAfter = targetTextBox.Text.Length;
            return targetTextBox.Text.Substring(positionBefore, positionAfter - positionBefore).Trim() == "";
        }

        public void SaveInAppSettings(string sKeyName = "", string sValue = "")
        {
            int iID = 0;
            string sQuery = "";

            if (sKeyName != "")
            {
                sQuery = "UPDATE tblAppSettings SET ValueName='" + sValue + "' WHERE LTRIM(RTRIM(UPPER(KeyName)))='" + sKeyName.ToUpper().Trim() + "'";
                if (fnExecuteNonQuery(sQuery) == 0)
                {
                    sQuery = "INSERT INTO tblAppSettings(KeyName,ValueName,TenantID) VALUES('" + sKeyName.ToUpper() + "','" + sValue + "'," + Global.gblTenantID + ")";
                    fnExecuteNonQuery(sQuery);
                }
                if (sKeyName == "BLNSHOWCOMPANYNAME")
                {
                    sQuery = "update startup.dbo.tblcompany set companyname='" + sValue + "' where companycode='" + AppSettings.CompanyCode + "'";
                    fnExecuteNonQuery(sQuery);

                    sQuery = "update tblCompanyMaster set companyname='" + sValue + "' ";
                    fnExecuteNonQuery(sQuery);
                }
                if (sKeyName == "FSTARTDATE")
                {
                    sQuery = "update startup.dbo.tblcompany set fystartdate='" + sValue + "' where companycode='" + AppSettings.CompanyCode + "'";
                    fnExecuteNonQuery(sQuery);

                    sQuery = "update tblCompanyMaster set fystartdate='" + sValue + "' ";
                    fnExecuteNonQuery(sQuery);
                }
                if (sKeyName == "FENDDATE")
                {
                    sQuery = "update startup.dbo.tblcompany set fyenddate='" + sValue + "' where companycode='" + AppSettings.CompanyCode + "'";
                    fnExecuteNonQuery(sQuery);

                    sQuery = "update tblCompanyMaster set fyenddate='" + sValue + "' ";
                    fnExecuteNonQuery(sQuery);
                }
                if (sKeyName == "BLNBARCODE")
                {
                    sQuery = "update tblVchtype set Activestatus=0 Where ParentID in (40,88)";
                    fnExecuteNonQuery(sQuery);
                }

            }
        }

        public DataTable RetieveFromDBInAppSettings(double dTenanID)
        {
            DataTable dtData = new DataTable();
            dtData = fnGetData("SELECT UPPER(KeyName) as KeyName,ValueName,ID FROM tblAppSettings WHERE TenantID = " + dTenanID + "").Tables[0];
            if (dtData.Rows.Count > 0)
                return dtData;
            else
                return null;
        }

        public string RetieveFromDBInAppSettings(double dTenanID, string KeyName)
        {
            DataTable dtData = new DataTable();
            dtData = fnGetData("SELECT UPPER(KeyName) as KeyName,ValueName,ID FROM tblAppSettings WHERE UPPER(KeyName) = '" + KeyName.ToUpper() + "' and TenantID = " + dTenanID + "").Tables[0];
            if (dtData.Rows.Count > 0)
                return dtData.Rows[0]["ValueName"].ToString();
            else
                return "";
        }

        public void LoadThemeAsperThemeID()
        {
            clsTheme cTheme = new clsTheme();
            DataTable dtGet = fnGetData("SELECT KeyName,ValueName, FROM tblAppSettings WHERE TenantID = " + Global.gblTenantID + "").Tables[0];
            if (dtGet.Rows.Count > 0)
            {
                for (int i = 0; i < dtGet.Rows.Count; i++)
                {
                    switch (dtGet.Rows[i]["KeyName"].ToString().Trim().ToUpper())
                    {
                        case "FORMMAINBCKCLR":
                            cTheme.FORMMAINBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHDRBCKCLR":
                            cTheme.FORMHDRBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMFTRBCKCLR":
                            cTheme.FORMFTRBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORLFTBCKCLR":
                            cTheme.FORLFTBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMRHTBCKCLR":
                            cTheme.FORMRHTBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHDRTXTCLR":
                            cTheme.FORMHDRTXTCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "FORMHILTCLR1":
                            cTheme.FORMHILTCLR1 = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHILTCLR2":
                            cTheme.FORMHILTCLR2 = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHILTCLR3":
                            cTheme.FORMHILTCLR3 = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDBCKCLR":
                            cTheme.GRIDBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRBCKCLR":
                            cTheme.GRIDHDRBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTCLR":
                            cTheme.GRIDHDRTXTCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTBLD":
                            cTheme.GRIDHDRTXTBLD = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTFNT":
                            cTheme.GRIDHDRTXTFNT = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDALTRWBCKCLR":
                            cTheme.GRIDALTRWBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTCLR":
                            cTheme.GRIDALTRWTXTCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTBLD":
                            cTheme.GRIDALTRWTXTBLD = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTFNT":
                            cTheme.GRIDALTRWTXTFNT = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDSELRWBCKCLR":
                            cTheme.GRIDSELRWBCKCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTCLR":
                            cTheme.GRIDSELRWTXTCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTBLD":
                            cTheme.GRIDSELRWTXTBLD = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTFNT":
                            cTheme.GRIDSELRWTXTFNT = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDNORRWTXTCLR":
                            cTheme.GRIDNORRWTXTCLR = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDNORRWTXTBLD":
                            cTheme.GRIDNORRWTXTBLD = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDNORRWTXTFNT":
                            cTheme.GRIDNORRWTXTFNT = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FONTFORAPP":
                            cTheme.FONTFORAPP = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "HEADFNTSIZ":
                            cTheme.HEADFNTSIZ = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "NORFNTSIZ":
                            cTheme.NORFNTSIZ = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "DESCFNTSIZ":
                            cTheme.DESCFNTSIZ = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        
                    }
                }
            }
        }

        //public void LoadAppSettings()
        //{
        //    AppSettings AppSet = new AppSettings();
        //    DataTable dtGet = RetieveFromDBInAppSettings(Global.gblTenantID);
        //    if (dtGet.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dtGet.Rows.Count; i++)
        //        {
        //            switch (dtGet.Rows[i]["KeyName"].ToString().Trim().ToUpper())
        //            {
        //                case "STRBATCODEPREFIXSUFFIX":
        //                    AppSet.BarcodePrefix = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "MAJORCURRENCY":
        //                    AppSet.MajorCurrency = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "MINORCURRENCY":
        //                    AppSet.MinorCurrency = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "MAJORSYMBOL":
        //                    AppSet.MajorSymbol = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "MINORSYMBOL":
        //                    AppSet.MinorSymbol = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "BLNSHOWCOMPANYADDRESS":
        //                    AppSet.CompAddress = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "BLNSHOWCOMPANYNAME":
        //                    AppSet.CompName = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "BLNTAXENABLED":
        //                    AppSet.TaxEnabled = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "DBLCESS":
        //                    AppSet.Cess = Convert.ToDouble(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTOBYINDAYBOOK":
        //                    AppSet.NeedToByDayBook = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNVERTICALACCFORMAT":
        //                    AppSet.VerticalAccFormat = Convert.ToDouble(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "EXPLORERSKININDEX":
        //                    AppSet.ThemeIndex = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNAUTOBACKUP":
        //                    AppSet.AutoBackupOnLogin = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "INTCESSMODE":
        //                    AppSet.CessMode = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNAGENT":
        //                    AppSet.NeedAgent = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "INTIMPLEMENTINGSTATECODE":
        //                    AppSet.StateCode = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "MYGSTIN":
        //                    AppSet.CompGSTIN = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "MYECOMMERCEGSTIN":
        //                    AppSet.ECommerceNo = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "CURRENCYDECIMALS":
        //                    AppSet.CurrencyDecimals = Convert.ToDecimal(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "QTYDECIMALFORMAT":
        //                    AppSet.QtyDecimals = Convert.ToDecimal(dtGet.Rows[i]["ValueName"].ToString());
        //                    break; 
        //                case "STRSTREET":
        //                    AppSet.CompStreet = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "STRCONTACT":
        //                    AppSet.CompContact = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "STEMAIL":
        //                    AppSet.CompEmail = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FSTARTDATE":
        //                    AppSet.FinYearStart = Convert.ToDateTime(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "FENDDATE":
        //                    AppSet.FinYearEnd = Convert.ToDateTime(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTAXCOLLSOURCE":
        //                    AppSet.NeedTaxCollectSourcet = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNADVANCED":
        //                    AppSet.NeedAdvanced = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNEXTDEVCONN":
        //                    AppSet.NeedExternalDevConnt = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNOFFERLOY":
        //                    AppSet.NeedOffersLoyalty = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNDISCGROUP":
        //                    AppSet.NeedDiscGrouping = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTEXSIZE":
        //                    AppSet.NeedSize = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTEXCOLOR":
        //                    AppSet.NeedColor = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTEXBRAND":
        //                    AppSet.NeedBrand = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNTHEME":
        //                    AppSet.NeedTheme = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "STRLAKHORMILL":
        //                    AppSet.LakhsOrMillion = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "BLNSHOWBKONLOG":
        //                    AppSet.AutoBackupOnLogin = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNAUTOBACKUPEXIT":
        //                    AppSet.NeedAutobackupOnExit = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "BLNCOSTCENTRE":
        //                    AppSet.NeedCostCenter = Convert.ToBoolean(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;
        //                case "STRBACKUPSTRING":
        //                    AppSet.BackUpPath1 = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "STRBACKUPSTRING2":
        //                    AppSet.BackUpPath2 = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "STRBACKUPSTRING3":
        //                    AppSet.BackUpPath3 = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "INTCASINGID":
        //                    AppSet.CasingID = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
        //                    break;

        //                case "FORMMAINBCKCLR":
        //                    AppSet.FormMainBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMHDRBCKCLR":
        //                    AppSet.FormHeadBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMFTRBCKCLR":
        //                    AppSet.FormFooterBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORLFTBCKCLR":
        //                    AppSet.FormLeftBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMRHTBCKCLR":
        //                    AppSet.FormRightBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMHDRTXTCLR":
        //                    AppSet.FormHeadTextClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;

        //                case "FORMHILTCLR1":
        //                    AppSet.FormHighlight1Clr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMHILTCLR2":
        //                    AppSet .FormHighlight2Clr= dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "FORMHILTCLR3":
        //                    AppSet.FormHighlight3Clr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;

        //                case "GRIDBCKCLR":
        //                    AppSet.GridBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDHDRBCKCLR":
        //                    AppSet.GridHeadBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDHDRTXTCLR":
        //                    AppSet.GridHeadTextClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDHDRTXTBLD":
        //                    AppSet.GridHeadTextBold = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDHDRTXTFNT":
        //                    AppSet.GridHeadTextFnt = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDALTRWBCKCLR":
        //                    AppSet.GridAltBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDALTRWTXTCLR":
        //                    AppSet.GridAltTextClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDALTRWTXTBLD":
        //                    AppSet.GridAltTextBold = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDALTRWTXTFNT[i]["":
        //                    AppSet.GridAltTextFnt = dtGet.RowsValueName"].ToString();
        //                    break;

        //                case "GRIDSELRWBCKCLR":
        //                    AppSet.GridSelRwBackClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDSELRWTXTCLR":
        //                    AppSet.GridSelRwTextClr = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDSELRWTXTBLD":
        //                    AppSet.GridSelRwTextBold = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;
        //                case "GRIDSELRWTXTFNT":
        //                    AppSet.GridSelRwTextFnt = dtGet.Rows[i]["ValueName"].ToString();
        //                    break;

        //            }
        //        }
        //    }
        //}

        public void LoadAppSettings()
        {
            // AppSettings AppSet = new AppSettings();
            DataTable dtGet = RetieveFromDBInAppSettings(Global.gblTenantID);
            if (dtGet.Rows.Count > 0)
            {
                for (int i = 0; i < dtGet.Rows.Count; i++)
                {

                    switch (dtGet.Rows[i]["KeyName"].ToString().Trim().ToUpper())
                    {
                        case "STRWMIDENTIFIER":
                            AppSettings.STRWMIDENTIFIER = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRWMBARCODELENGTH":
                            AppSettings.STRWMBARCODELENGTH = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRWMQTYLENGTH":
                            AppSettings.STRWMQTYLENGTH = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRBATCODEPREFIXSUFFIX":
                            AppSettings.BarcodePrefix = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MAJORCURRENCY":
                            AppSettings.MajorCurrency = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MINORCURRENCY":
                            AppSettings.MinorCurrency = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MAJORSYMBOL":
                            AppSettings.MajorSymbol = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MINORSYMBOL":
                            AppSettings.MinorSymbol = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNSHOWCOMPANYADDRESS":
                            AppSettings.CompAddress = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNSHOWCOMPANYNAME":
                            AppSettings.CompName = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNTAXENABLED":
                            AppSettings.TaxEnabled = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"]));
                            break;
                        case "DBLCESS":
                            AppSettings.Cess = Convert.ToDouble(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNTOBYINDAYBOOK":
                            AppSettings.NeedToByDayBook = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNVERTICALACCFORMAT":
                            AppSettings.VerticalAccFormat = Convert.ToDouble(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "EXPLORERSKININDEX":
                            AppSettings.ThemeIndex = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNAUTOBACKUP":
                            AppSettings.AutoBackupOnLogin = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "INTCESSMODE":
                            AppSettings.CessMode = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNSRATEINC":
                            AppSettings.BLNSRATEINC = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNPRATEINC":
                            AppSettings.BLNPRATEINC = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNAGENT":
                            AppSettings.NeedAgent = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "INTIMPLEMENTINGSTATECODE":
                            AppSettings.StateCode = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MYGSTIN":
                            AppSettings.CompGSTIN = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MYECOMMERCEGSTIN":
                            AppSettings.ECommerceNo = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "AVAILABLETAXPER":
                            AppSettings.AVAILABLETAXPER = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "CURRENCYDECIMALS":
                            if (dtGet.Rows[i]["ValueName"].ToString() == "")
                                AppSettings.CurrencyDecimals = 2;
                            else
                            {
                                AppSettings.CurrDecimalFormat = "";
                                AppSettings.CurrencyDecimals = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());

                                for (int j = 0; j < Convert.ToInt32(AppSettings.CurrencyDecimals); j++)
                                    AppSettings.CurrDecimalFormat = AppSettings.CurrDecimalFormat + "0";

                                AppSettings.CurrDecimalFormat = "#0." + AppSettings.CurrDecimalFormat;
                            }
                            break;
                        case "QTYDECIMALFORMAT":
                            if (dtGet.Rows[i]["ValueName"].ToString() == "")
                                AppSettings.QtyDecimals = 2;
                            else
                            {
                                AppSettings.QtyDecimalFormat = "";
                                AppSettings.QtyDecimals = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());

                                for (int k = 0; k < Convert.ToInt32(AppSettings.QtyDecimals); k++)
                                    AppSettings.QtyDecimalFormat = AppSettings.QtyDecimalFormat + "0";

                                AppSettings.QtyDecimalFormat = "#0." + AppSettings.QtyDecimalFormat;
                            }
                            break;

                        ////case "CURRENCYDECIMALS":
                        ////    AppSettings.CurrencyDecimals = Convert.ToDecimal(dtGet.Rows[i]["ValueName"].ToString());
                        ////    break;
                        ////case "QTYDECIMALFORMAT":
                        ////    AppSettings.QtyDecimals = Convert.ToDecimal(dtGet.Rows[i]["ValueName"].ToString());
                        ////    break;
                        case "STRSTREET":
                            AppSettings.CompStreet = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRCONTACT":
                            AppSettings.CompContact = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STEMAIL":
                            AppSettings.CompEmail = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FSTARTDATE":
                            AppSettings.FinYearStart = Convert.ToDateTime(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "FENDDATE":
                            AppSettings.FinYearEnd = Convert.ToDateTime(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                        case "BLNTAXCOLLSOURCE":
                            AppSettings.NeedTaxCollectSourcet = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNBARCODE":
                            AppSettings.BLNBARCODE = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNADVANCED":
                            AppSettings.NeedAdvanced = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNEXTDEVCONN":
                            AppSettings.NeedExternalDevConnt = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNOFFERLOY":
                            AppSettings.NeedOffersLoyalty = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNDISCGROUP":
                            AppSettings.NeedDiscGrouping = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNTEXSIZE":
                            AppSettings.NeedSize = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNTEXCOLOR":
                            AppSettings.NeedColor = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNTEXBRAND":
                            AppSettings.NeedBrand = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNTHEME":
                            AppSettings.NeedTheme = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "STRLAKHORMILL":
                            AppSettings.LakhsOrMillion = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNSHOWBKONLOG":
                            AppSettings.AutoBackupOnLogin = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNAUTOBACKUPEXIT":
                            AppSettings.NeedAutobackupOnExit = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "BLNCOSTCENTRE":
                            AppSettings.NeedCostCenter = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "STRBACKUPSTRING":
                            AppSettings.BackUpPath1 = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRBACKUPSTRING2":
                            AppSettings.BackUpPath2 = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "STRBACKUPSTRING3":
                            AppSettings.BackUpPath3 = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "INTCASINGID":
                            AppSettings.CasingID = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;

                        case "FORMMAINBCKCLR":
                            AppSettings.FormMainBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHDRBCKCLR":
                            AppSettings.FormHeadBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMFTRBCKCLR":
                            AppSettings.FormFooterBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORLFTBCKCLR":
                            AppSettings.FormLeftBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMRHTBCKCLR":
                            AppSettings.FormRightBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHDRTXTCLR":
                            AppSettings.FormHeadTextClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "FORMHILTCLR1":
                            AppSettings.FormHighlight1Clr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHILTCLR2":
                            AppSettings.FormHighlight2Clr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FORMHILTCLR3":
                            AppSettings.FormHighlight3Clr = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDBCKCLR":
                            AppSettings.GridBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRBCKCLR":
                            AppSettings.GridHeadBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTCLR":
                            AppSettings.GridHeadTextClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTBLD":
                            AppSettings.GridHeadTextBold = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDHDRTXTFNT":
                            AppSettings.GridHeadTextFnt = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWBCKCLR":
                            AppSettings.GridAltBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTCLR":
                            AppSettings.GridAltTextClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTBLD":
                            AppSettings.GridAltTextBold = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDALTRWTXTFNT":
                            AppSettings.GridAltTextFnt = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "GRIDSELRWBCKCLR":
                            AppSettings.GridSelRwBackClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTCLR":
                            AppSettings.GridSelRwTextClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTBLD":
                            AppSettings.GridSelRwTextBold = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDSELRWTXTFNT":
                            AppSettings.GridSelRwTextFnt = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        //Added Anjitha 14/02/2022 2:30 PM
                        case "GRIDNORRWTXTCLR":
                            AppSettings.GridNorRwTextClr = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDNORRWTXTBLD":
                            AppSettings.GridNorRwTextBold = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "GRIDNORRWTXTFNT":
                            AppSettings.GridNorRwTextFnt = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "FONTFORAPP":
                            AppSettings.FontforApplication = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "HEADFNTSIZ":
                            AppSettings.FormHeadingFntSiz = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "NORFNTSIZ":
                            AppSettings.FormNorFntSiz = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "DESCFNTSIZ":
                            AppSettings.FormDescFntSiz = dtGet.Rows[i]["ValueName"].ToString();
                            break;

                        case "PLCALCULATION":
                            AppSettings.PLCALCULATION = Convert.ToInt32(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;

                        case "SRATE1ACT":
                            AppSettings.IsActiveSRate1 = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "SRATE1NAME":
                            AppSettings.SRate1Name = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "SRATE2ACT":
                            AppSettings.IsActiveSRate2 = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "SRATE2NAME":
                            AppSettings.SRate2Name = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "SRATE3ACT":
                            AppSettings.IsActiveSRate3 = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "SRATE3NAME":
                            AppSettings.SRate3Name = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "SRATE4ACT":
                            AppSettings.IsActiveSRate4 = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "SRATE4NAME":
                            AppSettings.SRate4Name = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "BLNCUSTAREA":
                            AppSettings.NeedCustArea = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;

                        //Added By Anjitha 16-Feb-2022 04:55 PM
                        case "SRATE5ACT":
                            AppSettings.IsActiveSRate5 = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "SRATE5NAME":
                            AppSettings.SRate5Name = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "MRPACT":
                            AppSettings.IsActiveMRP = Convert.ToBoolean(Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString()));
                            break;
                        case "MRPNAME":
                            AppSettings.MRPName = dtGet.Rows[i]["ValueName"].ToString();
                            break;
                        case "INTTAXMODE":
                            AppSettings.TaxMode = Convert.ToInt32(dtGet.Rows[i]["ValueName"].ToString());
                            break;
                    }
                }
            }
        }

        public string StockInsert(string sAction, decimal dItemID, string sBatchCode, decimal dQty, decimal dMRP, decimal dCostRateInc, decimal dCostRateExcl, decimal dPRateExcl, decimal dPrateInc, decimal dTaxPer, decimal dSRate1, decimal dSRate2, decimal dSRate3, decimal dSRate4, decimal dSRate5, int iBatchMode, string dVchType, DateTime dtVchDate, DateTime dtExpDt, double dRefID, double dVchTypeID, double dCCID = 1, double dTenantID = 1, bool bAutoCode = false, bool bBatchCode = false, bool bExpiry = false, decimal dPRate = 0)
        {
            string sMessage = "";
            //string sBarCode = "";
            DataTable dtStkIns = new DataTable();
            DataSet ds = new DataSet();
            if (dVchTypeID == 0)
            {//dtStkIns = fnGetData("EXEC UspTransStockUpdateFromItem " + dItemID + ",'" + sBatchCode + "',''," + dQty + "," + dMRP + "," + dCostRateInc + "," + dCostRateExcl + "," + dPRateExcl + "," + dPrateInc + "," + dTaxPer + "," + dSRate1 + "," + dSRate2 + "," + dSRate3 + "," + dSRate4 + "," + dSRate5 + "," + iBatchMode + ",'" + dVchType + "','" + dtVchDate.ToString("dd-MMM-yyyy") + "','" + dtExpDt.ToString("dd-MMM-yyyy") + "','" + sAction + "'," + dRefID + "," + dVchTypeID + "," + dCCID + "," + dTenantID + "").Tables[0];
                ds = fnGetData("EXEC UspTransStockUpdateFromItem " + dItemID + ",'" + sBatchCode + "',''," + dQty + "," + dMRP + "," + dCostRateInc + "," + dCostRateExcl + "," + dPRateExcl + "," + dPrateInc + "," + dTaxPer + "," + dSRate1 + "," + dSRate2 + "," + dSRate3 + "," + dSRate4 + "," + dSRate5 + "," + iBatchMode + ",'" + dVchType + "','" + dtVchDate.ToString("dd-MMM-yyyy") + "','" + dtExpDt.ToString("dd-MMM-yyyy") + "','" + sAction + "'," + dRefID + "," + dVchTypeID + "," + dCCID + "," + dTenantID + "," + dPRate + " ");
                if (ds != null)
                    if(ds.Tables.Count > 0)
                    {
                        dtStkIns = ds.Tables[0];
                    }
            }
            else
            {
                if (dVchTypeID == 0)
                {
                    sMessage = "-1" + "|" + "Voucher type not identified. Please re open the window";
                    return sMessage;
                }
                dtStkIns = fnGetData("EXEC UspTransStockUpdate " + dItemID + ",'" + sBatchCode + "',''," + dQty + "," + dMRP + "," + dCostRateInc + "," + dCostRateExcl + "," + dPRateExcl + "," + dPrateInc + "," + dTaxPer + "," + dSRate1 + "," + dSRate2 + "," + dSRate3 + "," + dSRate4 + "," + dSRate5 + "," + iBatchMode + ",'" + dVchType + "','" + dtVchDate.ToString("dd-MMM-yyyy") + "','" + dtExpDt.ToString("dd-MMM-yyyy") + "','" + sAction + "'," + dRefID + "," + dVchTypeID + "," + dCCID + "," + dTenantID + "").Tables[0];
            }
            if (dtStkIns.Rows.Count > 0)
            {
                sMessage = dtStkIns.Rows[0][0].ToString() + "|" + "";
            }

            return sMessage;
        }

        public void ControlEnterLeave(Control ctrl, Boolean blnIsLeave = false, Boolean blnEnableFormat = true)
        {
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

            if (ctrl is TextBox)
            {
                if (blnIsLeave == true)//Enter
                {
                    ctrl.BackColor = System.Drawing.Color.LightBlue;
                    //ctrl.Select();
                }
                else//Leave
                {
                    if (blnEnableFormat == true)
                    {
                        if (AppSettings.CasingID == 1)
                            ctrl.Text = ctrl.Text.ToUpper();
                        else if (AppSettings.CasingID == 2)
                            ctrl.Text = myTI.ToTitleCase(ctrl.Text.ToLower());

                    }

                    ctrl.BackColor = System.Drawing.SystemColors.Window;
                }
            }
            else
            {
                if (blnIsLeave == true)//Enter
                {
                    ctrl.BackColor = System.Drawing.Color.LightBlue;
                }
                else//Leave
                {
                    if (blnEnableFormat == true)
                    {
                        if (AppSettings.CasingID == 1)
                            ctrl.Text = ctrl.Text.ToUpper();
                        else if (AppSettings.CasingID == 2)
                            ctrl.Text = myTI.ToTitleCase(ctrl.Text.ToLower());

                    }

                    ctrl.BackColor = System.Drawing.SystemColors.Window;
                }
            }

        }

        public string CheckDBNullOrEmpty(string sValue)
        {
            bool IsNumeric = int.TryParse(sValue, out int numericValue);
            if (sValue != "")
            {
                if (string.IsNullOrEmpty(sValue) == true)
                {
                    if (IsNumeric == true)
                        return "0";
                    else
                        return "";
                }
                else
                {
                    return sValue;
                }
            }
            else
            {
                return "0"; //udayippu
            }
        }

        public string GetTableValue(string tableName, string FieldToSearch, string Condition = "")
        {
            string StrVal = "";
            sqlControl rs = new sqlControl();
            rs.Open("Select " + FieldToSearch + " as Field1 from " + tableName + " " + Condition);

            if (!rs.eof())
                StrVal = rs.fields("field1").ToString();
            if (StrVal == "")
                return "";
            else
                return StrVal;
        }

        public string chkChangeValuetoZero(string strVal = "")
        {
            string strRet = "";
            if (strVal != "")
            {
                if (strVal == ".00")
                {
                    strRet = "0";
                }
                else
                {
                    strRet = strVal;
                }
            }
            return strRet;
        }

        public bool VoucherInsert(int CCID, int vchtypeID, DateTime VchDate, DateTime vchtime, decimal LedgerID, decimal drlid, decimal crlid, long RefID, string VchNo, string CHECKINGTAG, double AmountD, double AmountC, long AgentID, long SalesmanID, int Optionalfield, long currencyID, bool BlnReconciled = false, string usernarration = "", string DestConnectionString = "")
        {
            try
            {
                long VchID;
                sqlControl rs = new sqlControl();
                if (DestConnectionString != "")
                    rs = new sqlControl(DestConnectionString);

                VchID = Convert.ToInt64(gfnGetNextSerialNo("tblVoucher", "VChID").ToString());

                if (VchID <= 0)
                    VchID = Convert.ToInt64(gfnGetNextSerialNo("tblVoucher", "VChID").ToString());

                if (usernarration == null)
                    usernarration = "";
                usernarration = usernarration.Replace("'", "''");

                double Amount;
                Amount = AmountD + AmountC;

                if (Amount <= 0)
                    return false;

                if (BlnReconciled)
                {
                    rs.Execute("INSERT INTO tblvoucher (vchid, VchDate, vchtime, LedgerID, drlid, crlid, RefID, VchNo, AmountD, AmountC, Amount, AgentID, SalesmanID, usernarration, optional, currencyID, Conversion, vchtypeID, CCID, myNarration) VALUES     (" + VchID + ",'" + Microsoft.VisualBasic.Strings.Format(VchDate, "dd/MMM/yyyy") + "','" + Microsoft.VisualBasic.Strings.Format(vchtime, "HH:mm:ss") + "'," + LedgerID + "," + drlid + "," + crlid + "," + Conversion.Val(RefID) + ",'" + VchNo.Replace("'", "''") + "'," + Amount + "," + AmountC + "," + (AmountD + AmountC) + "," + AgentID + "," + SalesmanID + ",'" + CHECKINGTAG.Replace("'", "''") + "'," + Optionalfield + "," + currencyID + ",0," + vchtypeID + "," + CCID + ",'" + usernarration.Replace("'", "''") + "')");
                    //rs.Execute("INSERT INTO tblvoucher (vchid, VchDate, vchtime, drlid, crlid, RefID, VchNo, AmountD, AmountC, AgentID, SalesmanID, usernarration, optional, currencyID, Conversion, vchtypeID, CCID, myNarration) VALUES     (" + VchID + ",'" + Strings.Format(VchDate, "dd/MMM/yyyy") + "','" + Strings.Format(vchtime, "HH:mm:ss") + "'," + crlid + "," + drlid + "," + Conversion.Val(RefID) + ",'" + VchNo.Replace("'", "''") + "',0," + Amount + "," + AgentID + "," + SalesmanID + ",'" + CHECKINGTAG.Replace("'", "''") + "'," + Optionalfield + "," + currencyID + ",0," + vchtypeID + "," + CCID + ",'" + usernarration.Replace("'", "''") + "')");
                }
                else
                {
                    rs.Execute("INSERT INTO tblvoucher (vchid, VchDate, vchtime, LedgerID, drlid, crlid, RefID, VchNo, AmountD, AmountC, Amount, AgentID, SalesmanID, usernarration, optional, currencyID, Conversion, vchtypeID, CCID, myNarration) VALUES     (" + VchID + ",'" + Microsoft.VisualBasic.Strings.Format(VchDate, "dd/MMM/yyyy") + "','" + Microsoft.VisualBasic.Strings.Format(vchtime, "HH:mm:ss") + "'," + LedgerID + "," + drlid + "," + crlid + "," + Conversion.Val(RefID) + ",'" + VchNo.Replace("'", "''") + "'," + AmountD + "," + AmountC + "," + (AmountD + AmountC) + "," + AgentID + "," + SalesmanID + ",'" + CHECKINGTAG.Replace("'", "''") + "'," + Optionalfield + "," + currencyID + ",0," + vchtypeID + "," + CCID + ",'" + usernarration.Replace("'", "''") + "')");
                    //rs.Execute("INSERT INTO tblvoucher (vchid, VchDate, vchtime, drlid, crlid, RefID, VchNo, AmountD, AmountC, AgentID, SalesmanID, usernarration, optional, currencyID, Conversion, vchtypeID, CCID, myNarration) VALUES     (" + VchID + ",'" + Strings.Format(VchDate, "dd/MMM/yyyy") + "','" + Strings.Format(vchtime, "HH:mm:ss") + "'," + crlid + "," + drlid + "," + Conversion.Val(RefID) + ",'" + VchNo.Replace("'", "''") + "',0," + Amount + "," + AgentID + "," + SalesmanID + ",'" + CHECKINGTAG.Replace("'", "''") + "'," + Optionalfield + "," + currencyID + ",0," + vchtypeID + "," + CCID + ",'" + usernarration.Replace("'", "''") + "')");
                }

                if (rs.RecordCount > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message + "Insert  to voucher function", MsgBoxStyle.Critical);
                return false;
            }
        }

        public void GridDefaultsStyleAccounts(DataGridView dvg)
        {
            try
            {
                // R:210, G:189, B:172
                // R:132, G:180, B:130
                // R:241, G:241, B:229
                // R:218, G:222, B:187
                // R:233, G:206, B:179
                // R:189, G:196, B:129
                // R:234, G:212, B:191
                dvg.EnableHeadersVisualStyles = false;

                DataGridViewCellStyle rowStyle;
                rowStyle = dvg.Rows[0].HeaderCell.Style;
                rowStyle.BackColor = Color.FromArgb(210, 189, 172);
                rowStyle.ForeColor = Color.Black;
                dvg.Rows[0].HeaderCell.Style = rowStyle;

                // R:241, G:241, B:229
                //dvg.DefaultCellStyle.SelectionBackColor = My.Settings.MyContrastColor;
                //dvg.DefaultCellStyle.SelectionForeColor = Color.Black;

                // Set RowHeadersDefaultCellStyle.SelectionBackColor so that its default
                // value won't override DataGridView.DefaultCellStyle.SelectionBackColor.
                dvg.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Empty;

                // Set the background color for all rows and for alternating rows. 
                // The value for alternating rows overrides the value for all rows. 
                dvg.RowsDefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255); // R:218, G:222, B:187
                                                                                    // R:189, G:196, B:129
                dvg.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 255, 240); // Color.FromArgb(189, 196, 129)

                // Set the row and column header styles.

                dvg.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dvg.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
                dvg.RowHeadersVisible = false;
                dvg.BackgroundColor = Color.FromArgb(255, 255, 255);

                int CustomColWidth;
                try
                {
                    CustomColWidth = (dvg.Width - 20) / (dvg.Columns.GetColumnCount(DataGridViewElementStates.Visible));
                }
                catch
                {
                    CustomColWidth = 30;
                }

                if (dvg.ColumnCount > 1)
                {
                    for (var i = 1; i <= dvg.ColumnCount - 1; i++)
                        dvg.Columns[i].Width = CustomColWidth;
                }
            }
            catch 
            {
            }
        }
    }
}
