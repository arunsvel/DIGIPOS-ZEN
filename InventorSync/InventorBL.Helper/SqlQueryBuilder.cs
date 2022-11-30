using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace InventorSync.InventorBL.Helper
{
    public class SqlQueryBuilder
    {
        Common Comm = new Common();

        public void SqlQueryExecutor(int iExecCode)
        {

            //if (iExecCode == 1)
            //{
            //    sQuery = "BEGIN TRANSACTION" +
            //    "SET QUOTED_IDENTIFIER ON" +
            //    "SET ARITHABORT ON" +
            //    "SET NUMERIC_ROUNDABORT OFF" +
            //    "SET CONCAT_NULL_YIELDS_NULL ON" +
            //    "SET ANSI_NULLS ON" +
            //    "SET ANSI_PADDING ON" +
            //    "SET ANSI_WARNINGS ON" +
            //    "COMMIT" +
            //    "BEGIN TRANSACTION" +
            //    "GO" +
            //    "ALTER TABLE dbo.tblCategories ADD" +
            //    "	TenantID numeric(18, 0) NULL" +
            //    "GO" +
            //    "ALTER TABLE dbo.tblCategories" +
            //    "	DROP CONSTRAINT AK_Categories" +
            //    "GO" +
            //    "DROP INDEX IX_tblCategories ON dbo.tblCategories" +
            //    "GO" +
            //    "CREATE UNIQUE NONCLUSTERED INDEX IX_tblCategories_Category_TenantID ON dbo.tblCategories" +
            //    "	(" +
            //    "	Category," +
            //    "	TenantID" +
            //    "	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
            //    "GO" +
            //    "CREATE UNIQUE NONCLUSTERED INDEX UK_tblCategories_ ON dbo.tblCategories" +
            //    "	(" +
            //    "	CategoryID" +
            //    "	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
            //    "GO" +
            //    "ALTER TABLE dbo.tblCategories SET (LOCK_ESCALATION = TABLE)" +
            //    "GO" +
            //    "COMMIT";
            //    Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            //}
            //else if (iExecCode == 2)
            //{
            //    sQuery = "CREATE PROCEDURE UspCategoriesInsert" +
            //    "(" +
            //    "     @CategoryID    NUMERIC  (18,0)," +
            //    "     @Category    VARCHAR  (50)," +
            //    "     @Remarks    VARCHAR  (50)," +
            //    "     @ParentID    VARCHAR  (100)," +
            //    "     @HID    VARCHAR  (100)," +
            //    "     @CatDiscPer    FLOAT," +
            //    "     @SystemName    VARCHAR  (50)," +
            //    "     @UserID    NUMERIC  (18,0)," +
            //    "     @LastUpdateDate    DATETIME," +
            //    "     @LastUpdateTime    DATETIME," +
            //    "     @TenantID   NUMERIC  (18,0)," +
            //    "	@Action             INT=0" +
            //    ")" +
            //    "AS" +
            //    "BEGIN" +
            //    "DECLARE @RetResult      INT" +
            //    "BEGIN TRY" +
            //    "BEGIN TRANSACTION;" +
            //    "IF @Action = 0" +
            //    "BEGIN" +
            //    "     INSERT INTO tblCategories(CategoryID,Category,Remarks,ParentID,HID,CatDiscPer,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID)" +
            //    "     VALUES(@CategoryID,@Category,@Remarks,@ParentID,@HID,@CatDiscPer,@SystemName,@UserID,@LastUpdateDate,@LastUpdateTime,@TenantID)" +
            //    "     SET @RetResult = 1;" +
            //    "END" +
            //    "IF @Action = 1" +
            //    "BEGIN" +
            //    "     UPDATE tblCategories SET Category=@Category,Remarks=@Remarks,ParentID=@ParentID,HID=@HID,CatDiscPer=@CatDiscPer,SystemName=@SystemName,UserID=@UserID,LastUpdateDate=@LastUpdateDate,LastUpdateTime=@LastUpdateTime" +
            //    "     WHERE CategoryID=@CategoryID AND TenantID=@TenantID" +
            //    "     SET @RetResult = 1;" +
            //    "END" +
            //    "IF @Action = 2" +
            //    "BEGIN" +
            //    "     DELETE FROM tblCategories WHERE CategoryID=@CategoryID AND TenantID=@TenantID" +
            //    "     SET @RetResult = 0;" +
            //    "END" +
            //    "COMMIT TRANSACTION;" +
            //    "SELECT @RetResult as SqlSpResult" +
            //    "END TRY" +
            //    "BEGIN CATCH" +
            //    "ROLLBACK;" +
            //    "SELECT" +
            //    "- 1 as SqlSpResult," +
            //    "ERROR_NUMBER() AS ErrorNumber," +
            //    "ERROR_STATE() AS ErrorState," +
            //    "ERROR_SEVERITY() AS ErrorSeverity," +
            //    "ERROR_PROCEDURE() AS ErrorProcedure," +
            //    "ERROR_LINE() AS ErrorLine," +
            //    "ERROR_MESSAGE() AS ErrorMessage;" +
            //    "END CATCH;" +
            //    "END";
            //    Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            //}
            //else if (iExecCode == 3)
            //{
            //    sQuery = "CREATE PROCEDURE UspGetCategories" +
            //    "(" +
            //    "@CategoryID    NUMERIC   (18,0)," +
            //    "@TenantID     NUMERIC   (18,0)" +
            //    ")" +
            //    "AS" +
            //    "BEGIN" +
            //    "     IF @CategoryID <> 0 " +
            //    "     BEGIN" +
            //    "         SELECT CategoryID,Category,Remarks,ParentID,HID,CatDiscPer,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblCategories" +
            //    "         WHERE CategoryID = @CategoryID AND TenantID = @TenantID ORDER BY Category ASC" +
            //    "     END" +
            //    "     ELSE" +
            //    "     BEGIN" +
            //    "         SELECT CategoryID,Category, (SELECT ISNULL(Category,'') FROM tblCategories WHERE CategoryID = C.ParentID) as [Parent Category],CatDiscPer as [Discount %],Remarks --,HID,ParentID,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID " +
            //    "		 FROM tblCategories C WHERE TenantID = @TenantID" +
            //    "     END" +
            //    "END";
            //    Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            //}

            string sQuery = "";
            if (iExecCode == 1)
            {
                sQuery = "DROP PROCEDURE UspGetCategories" +
                "GO" +
                "CREATE PROCEDURE UspGetCategories" +
                "(" +
                "@CategoryID    NUMERIC   (18,0)," +
                "@TenantID     NUMERIC   (18,0)" +
                ")" +
                "AS" +
                "BEGIN" +
                "     IF @CategoryID <> 0 " +
                "     BEGIN" +
                "         SELECT CategoryID,(ROW_NUMBER() OVER(ORDER BY CategoryID ASC)) as [Serial No],Category,Remarks,ParentID,HID,CatDiscPer,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblCategories" +
                "         WHERE CategoryID = @CategoryID AND TenantID = @TenantID" +
                "     END" +
                "     ELSE" +
                "     BEGIN" +
                "         SELECT CategoryID,(ROW_NUMBER() OVER(ORDER BY CategoryID ASC)) as [Serial No],Category,(SELECT Category WHERE CategoryID = C.ParentID) as [Parent Category],CatDiscPer as [Discount %],Remarks--,ParentID,HID,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID " +
                "		 FROM tblCategories C WHERE TenantID = @TenantID" +
                "     END" +
                "END";
                Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            }
            else if (iExecCode == 2)
            {
                sQuery = "DROP PROCEDURE UspGetBrand)" +
                "GO" +
                "CREATE PROCEDURE [dbo].[UspGetBrand]" +
                "(" +
                "	@brandID    NUMERIC   (18,0)," +
                "	@TenantID	NUMERIC   (18,0)" +
                ")" +
                "AS" +
                "BEGIN" +
                "     IF @brandID <> 0 " +
                "     BEGIN" +
                "         SELECT brandID,brandName,brandShortName,DiscPer,SystemName,UserID,TenantID,LastUpdateDate,LastUpdateTime FROM tblBrand" +
                "         WHERE brandID = @brandID AND TenantID = @TenantID " +
                "     END" +
                "     ELSE" +
                "     BEGIN" +
                "         SELECT brandID,brandName,brandShortName,DiscPer--,SystemName,UserID,TenantID,LastUpdateDate,LastUpdateTime " +
                "		 FROM tblBrand" +
                "		 WHERE TenantID = @TenantID " +
                "     END" +
                "END";
                Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            }
            else if (iExecCode == 3)
            {
                sQuery = "BEGIN TRANSACTION" +
                "SET QUOTED_IDENTIFIER ON" +
                "SET ARITHABORT ON" +
                "SET NUMERIC_ROUNDABORT OFF" +
                "SET CONCAT_NULL_YIELDS_NULL ON" +
                "SET ANSI_NULLS ON" +
                "SET ANSI_PADDING ON" +
                "SET ANSI_WARNINGS ON" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster ADD" +
                "	TenantID numeric(18, 0) NULL" +
                "GO" +
                "CREATE UNIQUE NONCLUSTERED INDEX UK_tblItemMaster ON dbo.tblItemMaster" +
                "	(" +
                "	ItemID" +
                "	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                "GO" +
                "CREATE NONCLUSTERED INDEX UK_tblItemMaster_ItemName_TenantID ON dbo.tblItemMaster" +
                "	(" +
                "	ItemName," +
                "	TenantID" +
                "	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT";
                Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            }
            else if (iExecCode == 4)
            {
                sQuery = "BEGIN TRANSACTION" +
                "SET QUOTED_IDENTIFIER ON" +
                "SET ARITHABORT ON" +
                "SET NUMERIC_ROUNDABORT OFF" +
                "SET CONCAT_NULL_YIELDS_NULL ON" +
                "SET ANSI_NULLS ON" +
                "SET ANSI_PADDING ON" +
                "SET ANSI_WARNINGS ON" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT FK_tblItemMaster_tblProductGroup" +
                "GO" +
                "ALTER TABLE dbo.tblProductGroup SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT FK_tblItemMaster_tblUnit" +
                "GO" +
                "ALTER TABLE dbo.tblUnit SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT FK_tblItemMaster_tblHSNCODE" +
                "GO" +
                "ALTER TABLE dbo.tblHSNCode SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT FK_tblItemMaster_tblManufacturer" +
                "GO" +
                "ALTER TABLE dbo.tblManufacturer SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT FK_tblItemMaster_tblCategories" +
                "GO" +
                "ALTER TABLE dbo.tblCategories SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT DF_tblItemMaster_IntLocal" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT DF__tblItemMa__Ledge__1975C517" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT DF__tblItemMa__BlnEx__48EFCE0F" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT DF__tblItemMa__Cooli__49E3F248" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT DF__tblItemMa__Finis__4AD81681" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT DF__tblItemMa__MinRa__5555A4F4" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster" +
                "	DROP CONSTRAINT DF__tblItemMa__MaxRa__573DED66" +
                "GO" +
                "CREATE TABLE dbo.Tmp_tblItemMaster" +
                "	(" +
                "	ItemID numeric(18, 0) NOT NULL," +
                "	ItemCode varchar(100) NOT NULL," +
                "	ItemName varchar(100) NULL," +
                "	CategoryID numeric(18, 0) NULL," +
                "	Description varchar(1000) NULL," +
                "	Unit varchar(100) NULL," +
                "	MRP money NULL," +
                "	ROL float(53) NULL," +
                "	Rack varchar(100) NULL," +
                "	Manufacturer varchar(100) NULL," +
                "	ActiveStatus numeric(1, 0) NULL," +
                "	IntLocal int NULL," +
                "	ProductType varchar(50) NULL," +
                "	ProductTypeID float(53) NULL," +
                "	LedgerID numeric(18, 0) NULL," +
                "	UNITID numeric(18, 0) NULL," +
                "	Notes varchar(1000) NULL," +
                "	agentCommPer float(53) NULL," +
                "	BlnExpiryItem int NULL," +
                "	Coolie numeric(18, 0) NULL," +
                "	FinishedGoodID int NULL," +
                "	MinRate float(53) NULL," +
                "	MaxRate float(53) NULL," +
                "	PLUNo varchar(50) NULL," +
                "	HSNID numeric(18, 0) NULL," +
                "	iCatDiscPer float(53) NULL," +
                "	IPGDiscPer float(53) NULL," +
                "	ImanDiscPer float(53) NULL," +
                "	ItemNameUniCode nvarchar(500) NULL," +
                "	Minqty float(53) NULL," +
                "	MNFID numeric(18, 0) NULL," +
                "	ItemCodeUniCode nvarchar(50) NULL," +
                "	UPC varchar(50) NULL," +
                "	BatchMode varchar(50) NULL," +
                "	Qty float(53) NULL," +
                "	MaxQty float(53) NULL," +
                "	IntNoOrWeight numeric(18, 0) NULL," +
                "	SystemName varchar(50) NULL," +
                "	UserID numeric(18, 0) NULL," +
                "	LastUpdateDate datetime NULL," +
                "	LastUpdateTime datetime NULL" +
                "	)  ON [PRIMARY]" +
                "GO" +
                "ALTER TABLE dbo.Tmp_tblItemMaster SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "ALTER TABLE dbo.Tmp_tblItemMaster ADD CONSTRAINT" +
                "	DF_tblItemMaster_IntLocal DEFAULT ((0)) FOR IntLocal" +
                "GO" +
                "ALTER TABLE dbo.Tmp_tblItemMaster ADD CONSTRAINT" +
                "	DF__tblItemMa__Ledge__1975C517 DEFAULT ('101') FOR LedgerID" +
                "GO" +
                "ALTER TABLE dbo.Tmp_tblItemMaster ADD CONSTRAINT" +
                "	DF__tblItemMa__BlnEx__48EFCE0F DEFAULT ((0)) FOR BlnExpiryItem" +
                "GO" +
                "ALTER TABLE dbo.Tmp_tblItemMaster ADD CONSTRAINT" +
                "	DF__tblItemMa__Cooli__49E3F248 DEFAULT ((0)) FOR Coolie" +
                "GO" +
                "ALTER TABLE dbo.Tmp_tblItemMaster ADD CONSTRAINT" +
                "	DF__tblItemMa__Finis__4AD81681 DEFAULT ((-1)) FOR FinishedGoodID" +
                "GO" +
                "ALTER TABLE dbo.Tmp_tblItemMaster ADD CONSTRAINT" +
                "	DF__tblItemMa__MinRa__5555A4F4 DEFAULT ((0)) FOR MinRate" +
                "GO" +
                "ALTER TABLE dbo.Tmp_tblItemMaster ADD CONSTRAINT" +
                "	DF__tblItemMa__MaxRa__573DED66 DEFAULT ((0)) FOR MaxRate" +
                "GO" +
                "IF EXISTS(SELECT * FROM dbo.tblItemMaster)" +
                "	 EXEC('INSERT INTO dbo.Tmp_tblItemMaster (ItemID, ItemCode, ItemName, CategoryID, Description, Unit, MRP, ROL, Rack, Manufacturer, ActiveStatus, IntLocal, ProductType, ProductTypeID, LedgerID, UNITID, Notes, agentCommPer, BlnExpiryItem, Coolie, FinishedGoodID, MinRate, MaxRate, PLUNo, HSNID, iCatDiscPer, IPGDiscPer, ImanDiscPer, ItemNameUniCode, Minqty, MNFID, ItemCodeUniCode, UPC, BatchMode, Qty, MaxQty, IntNoOrWeight, SystemName, UserID, LastUpdateDate, LastUpdateTime)" +
                "		SELECT ItemID, ItemCode, ItemName, CategoryID, Description, Unit, MRP, ROL, Rack, Manufacturer, ActiveStatus, IntLocal, ProductType, ProductTypeID, LedgerID, UNITID, Notes, agentCommPer, BlnExpiryItem, CONVERT(numeric(18, 0), Coolie), FinishedGoodID, MinRate, MaxRate, PLUNo, HSNID, iCatDiscPer, IPGDiscPer, ImanDiscPer, ItemNameUniCode, Minqty, MNFID, ItemCodeUniCode, UPC, BatchMode, Qty, MaxQty, IntNoOrWeight, SystemName, UserID, LastUpdateDate, LastUpdateTime FROM dbo.tblItemMaster WITH (HOLDLOCK TABLOCKX)')" +
                "GO" +
                "ALTER TABLE dbo.tblGroupItemMaster" +
                "	DROP CONSTRAINT FK_tblGroupItemMaster_tblItemMaster" +
                "GO" +
                "ALTER TABLE dbo.tblGroupItemMaster" +
                "	DROP CONSTRAINT FK_tblGroupItemMaster_tblItemMasterRaw" +
                "GO" +
                "ALTER TABLE dbo.tblRepackingItem" +
                "	DROP CONSTRAINT FK_tblRepackingItem_tblitenmaster" +
                "GO" +
                "ALTER TABLE dbo.tblStockJournalItem" +
                "	DROP CONSTRAINT FK_tblStockJournalItem_tblitenmaster" +
                "GO" +
                "ALTER TABLE dbo.tblSalesItem" +
                "	DROP CONSTRAINT FK_tblSalesItem_tblitenmaster" +
                "GO" +
                "ALTER TABLE dbo.tblOrderItem" +
                "	DROP CONSTRAINT FK_tblOrderItem_tblitenmaster" +
                "GO" +
                "ALTER TABLE dbo.tblPurchaseItem" +
                "	DROP CONSTRAINT FK_tblPurchaseItem_tblitenmaster" +
                "GO" +
                "ALTER TABLE dbo.tblItemStock" +
                "	DROP CONSTRAINT FK_tblItemStock_tblItemMaster" +
                "GO" +
                "DROP TABLE dbo.tblItemMaster" +
                "GO" +
                "EXECUTE sp_rename N'dbo.Tmp_tblItemMaster', N'tblItemMaster', 'OBJECT' " +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster ADD CONSTRAINT" +
                "	PK_tblItemMaster PRIMARY KEY NONCLUSTERED " +
                "	(" +
                "	ItemID" +
                "	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                "GO" +
                "CREATE UNIQUE CLUSTERED INDEX CDX_tblItemMaster ON dbo.tblItemMaster" +
                "	(" +
                "	ItemID," +
                "	ItemCode," +
                "	ItemName," +
                "	MRP" +
                "	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster ADD CONSTRAINT" +
                "	IX_tblItemMaster UNIQUE NONCLUSTERED " +
                "	(" +
                "	ItemCode" +
                "	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster ADD CONSTRAINT" +
                "	FK_tblItemMaster_tblCategories FOREIGN KEY" +
                "	(" +
                "	CategoryID" +
                "	) REFERENCES dbo.tblCategories" +
                "	(" +
                "	CategoryID" +
                "	) ON UPDATE  NO ACTION " +
                "	 ON DELETE  NO ACTION " +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster ADD CONSTRAINT" +
                "	FK_tblItemMaster_tblManufacturer FOREIGN KEY" +
                "	(" +
                "	MNFID" +
                "	) REFERENCES dbo.tblManufacturer" +
                "	(" +
                "	MnfID" +
                "	) ON UPDATE  NO ACTION " +
                "	 ON DELETE  NO ACTION " +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster ADD CONSTRAINT" +
                "	FK_tblItemMaster_tblHSNCODE FOREIGN KEY" +
                "	(" +
                "	HSNID" +
                "	) REFERENCES dbo.tblHSNCode" +
                "	(" +
                "	HSNID" +
                "	) ON UPDATE  NO ACTION " +
                "	 ON DELETE  NO ACTION " +
                "GO" +
                "ALTER TABLE dbo.tblItemMaster ADD CONSTRAINT" +
                "	FK_tblItemMaster_tblUnit FOREIGN KEY" +
                "	(" +
                "	UNITID" +
                "	) REFERENCES dbo.tblUnit" +
                "	(" +
                "	UnitID" +
                "	) ON UPDATE  NO ACTION " +
                "	 ON DELETE  NO ACTION " +
                "GO" +
                "create trigger [dbo].[tg_deleteimage] on dbo.tblItemMaster after delete as Declare @ItemId as numeric Declare @Itemcode as varchar(500) select @itemid = itemid ,@Itemcode= itemcode from deleted delete from dbo.tblImageData where sourcewindow='Item' and imagename=@ItemCode and TemplateID=@ItemID" +
                "GO" +
                "Create trigger [dbo].[Trg_ItemmasterUserLog]  ON  dbo.tblItemMaster for Delete  as  Declare @ItemName Varchar(100)  Declare @ItemId Int  Declare @ItemCode varchar(50)  DECLARE @Action as char(1)   Declare @UserID int  Declare @Systemname Varchar(50)  Declare @Lastdate Datetime  Declare @Lasttime Datetime  Declare @Username Varchar(50)      SET @Action = (CASE WHEN EXISTS(SELECT *  FROM INSERTED)                                    AND EXISTS(SELECT * FROM DELETED)                                 THEN 'U' 			WHEN EXISTS(SELECT * FROM INSERTED)                                     THEN 'I'					                       WHEN EXISTS(SELECT * FROM DELETED)                               THEN 'D'           END) 	if @Action = 'U' or @Action = 'I'    	begin		 Select @ItemName = Itemname , @ItemCode = ItemCode , @ItemId = itemID ,@UserID =userID , @Systemname = SystemName , @Lastdate = LastUpdateDate , @Lasttime = LastUpdateTime  from  INSERTED  	 select @Username = username from tbluserMaster where userID = @UserId 	end 	if @Action = 'D'     	begin		 Select @ItemName = Itemname , @ItemCode = ItemCode , @ItemId = itemID,@UserID =userID , @Systemname = SystemName , @Lastdate = LastUpdateDate , @Lasttime = LastUpdateTime   from  Deleted  	  select @Username = username from tbluserMaster where userID = @UserId 	 Insert into tbluserLog (NewData,DateOf,Timeof,[Action],ActionDescription,VchtypeId , parentVchtypeId,UniqueFiledValue,RefId,UserId,SystemName,WindowName,Username) values (@ItemName , @Lastdate ,@LastTime ,'Delete','Deleted Item ' + @Itemname , 502 , 502,@ItemCode,@ItemId , @UserID ,@Systemname,'ItemMaster' ,@Username )  	end 	if @Action = 'I'  	begin 	Insert into tbluserLog (NewData,DateOf,Timeof,[Action],ActionDescription,VchtypeId , parentVchtypeId,UniqueFiledValue,RefId,UserId , SystemName,WindowName,Username) values (@ItemName , @Lastdate ,@LastTime  ,'Insert','Insert New Item ' + @Itemname ,502,502,@ItemCode,@ItemId , @UserID ,@Systemname,'ItemMaster',@Username)  	end 	if @Action = 'U'  	begin 	Insert into tbluserLog (NewData,DateOf,Timeof,[Action],ActionDescription,VchtypeId , parentVchtypeId,UniqueFiledValue,RefId,UserId , SystemName,WindowName,Username) values (@ItemName ,  @Lastdate ,@LastTime ,'Update','Updated Item ' + @Itemname,502,502,@ItemCode,@ItemId  , @UserID ,@Systemname,'ItemMaster',@Username)  	end" +
                "GO" +
                "create trigger [dbo].[Trig_ItemCatDiscperUpdateInsert]  ON  dbo.tblItemMaster for insert,update as  declare @LCatDiscPer Float  declare @LCategoryID Float  declare @lItemID Float  select  @lItemID=ItemID,@LCategoryID=CategoryID from inserted  select  @LCatDiscPer=isnull(CatDiscPer,0)  from tblcategories where CategoryID = @LCategoryID  update tblItemMaster set ICatDiscPer = @LCatDiscPer where ItemID = @LItemID" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblItemStock ADD CONSTRAINT" +
                "	FK_tblItemStock_tblItemMaster FOREIGN KEY" +
                "	(" +
                "	ItemID" +
                "	) REFERENCES dbo.tblItemMaster" +
                "	(" +
                "	ItemID" +
                "	) ON UPDATE  NO ACTION " +
                "	 ON DELETE  NO ACTION " +
                "GO" +
                "ALTER TABLE dbo.tblItemStock SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblPurchaseItem ADD CONSTRAINT" +
                "	FK_tblPurchaseItem_tblitenmaster FOREIGN KEY" +
                "	(" +
                "	ItemId" +
                "	) REFERENCES dbo.tblItemMaster" +
                "	(" +
                "	ItemID" +
                "	) ON UPDATE  CASCADE " +
                "	 ON DELETE  NO ACTION " +
                "GO" +
                "ALTER TABLE dbo.tblPurchaseItem SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblOrderItem ADD CONSTRAINT" +
                "	FK_tblOrderItem_tblitenmaster FOREIGN KEY" +
                "	(" +
                "	ItemId" +
                "	) REFERENCES dbo.tblItemMaster" +
                "	(" +
                "	ItemID" +
                "	) ON UPDATE  CASCADE " +
                "	 ON DELETE  NO ACTION " +
                "GO" +
                "ALTER TABLE dbo.tblOrderItem SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblSalesItem ADD CONSTRAINT" +
                "	FK_tblSalesItem_tblitenmaster FOREIGN KEY" +
                "	(" +
                "	ItemId" +
                "	) REFERENCES dbo.tblItemMaster" +
                "	(" +
                "	ItemID" +
                "	) ON UPDATE  CASCADE " +
                "	 ON DELETE  NO ACTION " +
                "GO" +
                "ALTER TABLE dbo.tblSalesItem SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblStockJournalItem ADD CONSTRAINT" +
                "	FK_tblStockJournalItem_tblitenmaster FOREIGN KEY" +
                "	(" +
                "	ItemId" +
                "	) REFERENCES dbo.tblItemMaster" +
                "	(" +
                "	ItemID" +
                "	) ON UPDATE  CASCADE " +
                "	 ON DELETE  NO ACTION " +
                "GO" +
                "ALTER TABLE dbo.tblStockJournalItem SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblRepackingItem ADD CONSTRAINT" +
                "	FK_tblRepackingItem_tblitenmaster FOREIGN KEY" +
                "	(" +
                "	ItemId" +
                "	) REFERENCES dbo.tblItemMaster" +
                "	(" +
                "	ItemID" +
                "	) ON UPDATE  CASCADE " +
                "	 ON DELETE  NO ACTION " +
                "GO" +
                "ALTER TABLE dbo.tblRepackingItem SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT" +
                "BEGIN TRANSACTION" +
                "GO" +
                "ALTER TABLE dbo.tblGroupItemMaster ADD CONSTRAINT" +
                "	FK_tblGroupItemMaster_tblItemMaster FOREIGN KEY" +
                "	(" +
                "	FinishedItemID" +
                "	) REFERENCES dbo.tblItemMaster" +
                "	(" +
                "	ItemID" +
                "	) ON UPDATE  NO ACTION " +
                "	 ON DELETE  CASCADE " +
                "GO" +
                "ALTER TABLE dbo.tblGroupItemMaster ADD CONSTRAINT" +
                "	FK_tblGroupItemMaster_tblItemMasterRaw FOREIGN KEY" +
                "	(" +
                "	RawMaterialItemID" +
                "	) REFERENCES dbo.tblItemMaster" +
                "	(" +
                "	ItemID" +
                "	) ON UPDATE  NO ACTION " +
                "	 ON DELETE  NO ACTION " +
                "GO" +
                "ALTER TABLE dbo.tblGroupItemMaster SET (LOCK_ESCALATION = TABLE)" +
                "GO" +
                "COMMIT";
                Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            }
            else if (iExecCode == 5)
            {
                sQuery = "DROP PROCEDURE UspItemMasterInsert)" +
                "GO" +
                "CREATE PROCEDURE UspItemMasterInsert" +
                "(" +
                "     @ItemID    NUMERIC  (18,0)," +
                "     @ItemCode    VARCHAR  (100)," +
                "     @ItemName    VARCHAR  (100)," +
                "     @CategoryID    NUMERIC  (18,0)," +
                "     @Description    VARCHAR  (1000)," +
                "     @Unit    VARCHAR  (100)," +
                "     @MRP    MONEY," +
                "     @ROL    FLOAT," +
                "     @Rack    VARCHAR  (100)," +
                "     @Manufacturer    VARCHAR  (100)," +
                "     @ActiveStatus    NUMERIC  (1,0)," +
                "     @IntLocal    INT," +
                "     @ProductType    VARCHAR  (50)," +
                "     @ProductTypeID    FLOAT," +
                "     @LedgerID    NUMERIC  (18,0)," +
                "     @UNITID    NUMERIC  (18,0)," +
                "     @Notes    VARCHAR  (1000)," +
                "     @agentCommPer    FLOAT," +
                "     @BlnExpiryItem    INT," +
                "     @Coolie    NUMERIC  (18,0)," +
                "     @FinishedGoodID    INT," +
                "     @MinRate    FLOAT," +
                "     @MaxRate    FLOAT," +
                "     @PLUNo    VARCHAR  (50)," +
                "     @HSNID    NUMERIC  (18,0)," +
                "     @iCatDiscPer    FLOAT," +
                "     @IPGDiscPer    FLOAT," +
                "     @ImanDiscPer    FLOAT," +
                "     @ItemNameUniCode    NVARCHAR  (500)," +
                "     @Minqty    FLOAT," +
                "     @MNFID    NUMERIC  (18,0)," +
                "     @ItemCodeUniCode    NVARCHAR  (50)," +
                "     @UPC    VARCHAR  (50)," +
                "     @BatchMode    VARCHAR  (50)," +
                "     @Qty    FLOAT," +
                "     @MaxQty    FLOAT," +
                "     @IntNoOrWeight    NUMERIC  (18,0)," +
                "     @SystemName    VARCHAR  (50)," +
                "     @UserID    NUMERIC  (18,0)," +
                "     @LastUpdateDate    DATETIME," +
                "     @LastUpdateTime    DATETIME," +
                "     @TenantID   NUMERIC  (18,0)," +
                "@Action             INT=0" +
                ")" +
                "AS" +
                "BEGIN" +
                "DECLARE @RetResult      INT" +
                "BEGIN TRY" +
                "BEGIN TRANSACTION;" +
                "IF @Action = 0" +
                "BEGIN" +
                "     INSERT INTO tblItemMaster(ItemID,ItemCode,ItemName,CategoryID,Description,Unit,MRP,ROL,Rack,Manufacturer,ActiveStatus,IntLocal,ProductType,ProductTypeID,LedgerID,UNITID,Notes,agentCommPer,BlnExpiryItem,Coolie,FinishedGoodID,MinRate,MaxRate,PLUNo,HSNID,iCatDiscPer,IPGDiscPer,ImanDiscPer,ItemNameUniCode,Minqty,MNFID,ItemCodeUniCode,UPC,BatchMode,Qty,MaxQty,IntNoOrWeight,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID)" +
                "     VALUES(@ItemID,@ItemCode,@ItemName,@CategoryID,@Description,@Unit,@MRP,@ROL,@Rack,@Manufacturer,@ActiveStatus,@IntLocal,@ProductType,@ProductTypeID,@LedgerID,@UNITID,@Notes,@agentCommPer,@BlnExpiryItem,@Coolie,@FinishedGoodID,@MinRate,@MaxRate,@PLUNo,@HSNID,@iCatDiscPer,@IPGDiscPer,@ImanDiscPer,@ItemNameUniCode,@Minqty,@MNFID,@ItemCodeUniCode,@UPC,@BatchMode,@Qty,@MaxQty,@IntNoOrWeight,@SystemName,@UserID,@LastUpdateDate,@LastUpdateTime,@TenantID)" +
                "     SET @RetResult = 1;" +
                "END" +
                "IF @Action = 1" +
                "BEGIN" +
                "     UPDATE tblItemMaster SET ItemCode=@ItemCode,ItemName=@ItemName,CategoryID=@CategoryID,Description=@Description,Unit=@Unit,MRP=@MRP,ROL=@ROL,Rack=@Rack,Manufacturer=@Manufacturer,ActiveStatus=@ActiveStatus,IntLocal=@IntLocal,ProductType=@ProductType,ProductTypeID=@ProductTypeID,LedgerID=@LedgerID,UNITID=@UNITID,Notes=@Notes,agentCommPer=@agentCommPer,BlnExpiryItem=@BlnExpiryItem,Coolie=@Coolie,FinishedGoodID=@FinishedGoodID,MinRate=@MinRate,MaxRate=@MaxRate,PLUNo=@PLUNo,HSNID=@HSNID,iCatDiscPer=@iCatDiscPer,IPGDiscPer=@IPGDiscPer,ImanDiscPer=@ImanDiscPer,ItemNameUniCode=@ItemNameUniCode,Minqty=@Minqty,MNFID=@MNFID,ItemCodeUniCode=@ItemCodeUniCode,UPC=@UPC,BatchMode=@BatchMode,Qty=@Qty,MaxQty=@MaxQty,IntNoOrWeight=@IntNoOrWeight,SystemName=@SystemName,UserID=@UserID,LastUpdateDate=@LastUpdateDate,LastUpdateTime=@LastUpdateTime,TenantID=@TenantID" +
                "     WHERE ItemID=@ItemID AND IntLocal = @IntLocal" +
                "     SET @RetResult = 1;" +
                "END" +
                "IF @Action = 2" +
                "BEGIN" +
                "     DELETE FROM tblItemMaster WHERE ItemID=@ItemID" +
                "     SET @RetResult = 0;" +
                "END" +
                "COMMIT TRANSACTION;" +
                "SELECT @RetResult as SqlSpResult" +
                "END TRY" +
                "BEGIN CATCH" +
                "ROLLBACK;" +
                "SELECT" +
                "- 1 as SqlSpResult," +
                "ERROR_NUMBER() AS ErrorNumber," +
                "ERROR_STATE() AS ErrorState," +
                "ERROR_SEVERITY() AS ErrorSeverity," +
                "ERROR_PROCEDURE() AS ErrorProcedure," +
                "ERROR_LINE() AS ErrorLine," +
                "ERROR_MESSAGE() AS ErrorMessage;" +
                "END CATCH;" +
                "END" +
                "";
                Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            }
            else if (iExecCode == 6)
            {
                sQuery = "DROP PROCEDURE UspGetItemMaster)" +
                "GO" +
                "CREATE PROCEDURE UspGetItemMaster" +
                "(" +
                "@ItemID    NUMERIC   (18,0)," +
                "@TenantID     NUMERIC   (18,0)" +
                ")" +
                "AS" +
                "BEGIN" +
                "     IF @ItemID <> 0 " +
                "     BEGIN" +
                "         SELECT ItemID,(ROW_NUMBER() OVER(ORDER BY ItemID ASC)) as [Serial No],ItemCode,ItemName,CategoryID,Description,Unit,MRP,ROL,Rack,Manufacturer,ActiveStatus,IntLocal,ProductType,ProductTypeID,LedgerID,UNITID,Notes,agentCommPer,BlnExpiryItem,Coolie,FinishedGoodID,MinRate,MaxRate,PLUNo,HSNID,iCatDiscPer,IPGDiscPer,ImanDiscPer,ItemNameUniCode,Minqty,MNFID,ItemCodeUniCode,UPC,BatchMode,Qty,MaxQty,IntNoOrWeight,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblItemMaster" +
                "         WHERE ItemID = @ItemID AND TenantID = @TenantID" +
                "     END" +
                "     ELSE" +
                "     BEGIN" +
                "         SELECT ItemID,(ROW_NUMBER() OVER(ORDER BY ItemID ASC)) as [Serial No],ItemCode,ItemName,CategoryID,Description,Unit,MRP,ROL,Rack,Manufacturer,ActiveStatus,IntLocal,ProductType,ProductTypeID,LedgerID,UNITID,Notes,agentCommPer,BlnExpiryItem,Coolie,FinishedGoodID,MinRate,MaxRate,PLUNo,HSNID,iCatDiscPer,IPGDiscPer,ImanDiscPer,ItemNameUniCode,Minqty,MNFID,ItemCodeUniCode,UPC,BatchMode,Qty,MaxQty,IntNoOrWeight,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblItemMaster WHERE TenantID = @TenantID" +
                "     END" +
                "END" +
                "";
                Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            }
            else if (iExecCode == 7)
            {
                sQuery = "DROP TABLE [dbo].[tblOnetimeMaster]" +
                "GO" +
                "CREATE TABLE [dbo].[tblOnetimeMaster](" +
                "	[OtmID] [int] IDENTITY(1,1) NOT NULL," +
                "	[OtmData] [nvarchar](1000) NULL," +
                "	[OtmValue] [decimal](18, 2) NULL," +
                "	[OtmDescription] [nvarchar](1000) NULL," +
                "	[OtmType] [char](10) NULL," +
                "	[TenantID] [numeric](18, 0) NULL," +
                " CONSTRAINT [PK_tblOnetimeMaster] PRIMARY KEY CLUSTERED " +
                "(" +
                "	[OtmID] ASC" +
                ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                ") ON [PRIMARY]" +
                "GO" +
                "CREATE UNIQUE NONCLUSTERED INDEX UK_tblOnetimeMaster ON dbo.tblOnetimeMaster" +
                "	(" +
                "	OtmID" +
                "	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                "GO" +
                "CREATE UNIQUE NONCLUSTERED INDEX UK_tblOnetimeMaster_OtmDataXTenantID ON dbo.tblOnetimeMaster" +
                "	(" +
                "	OtmID" +
                "	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                "GO" +
                "SET ANSI_PADDING OFF" +
                "GO";
                Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            }
            else if (iExecCode == 8)
            {
                sQuery = "DROP PROCEDURE UspGetOnetimeMaster)" +
                "GO" +
                "CREATE PROCEDURE UspGetOnetimeMaster" +
                "(" +
                "	@OtmID			INT," +
                "	@TenantID		NUMERIC   (18,0)," +
                "	@OtmType		CHAR(10)" +
                ")" +
                "AS" +
                "BEGIN" +
                "     IF @OtmID <> 0 " +
                "     BEGIN" +
                "         SELECT OtmID,(ROW_NUMBER() OVER(ORDER BY OtmID ASC)) as [Serial No],OtmData,OtmValue,OtmDescription,TenantID FROM tblOnetimeMaster" +
                "         WHERE OtmID = @OtmID AND OtmType = @TenantID AND OtmType = @OtmType" +
                "     END" +
                "     ELSE" +
                "     BEGIN" +
                "         SELECT OtmID,(ROW_NUMBER() OVER(ORDER BY OtmID ASC)) as [Serial No],OtmData,OtmValue,OtmDescription,OtmType,TenantID FROM tblOnetimeMaster " +
                "		 WHERE OtmType = @TenantID AND OtmType = @OtmType" +
                "     END" +
                "END" +
                "";
                Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            }
            else if (iExecCode == 9)
            {
                sQuery = "DROP PROCEDURE UspGetUnit" +
                "GO" +
                "CREATE PROCEDURE UspGetUnit" +
                "(" +
                "@UnitID    NUMERIC   (18,0)," +
                "@TenantID     NUMERIC   (18,0)" +
                ")" +
                "AS" +
                "BEGIN" +
                "     IF @UnitID <> 0 " +
                "     BEGIN" +
                "         SELECT UnitID,(ROW_NUMBER() OVER(ORDER BY UnitID ASC)) as [Serial No],UnitName,UnitShortName,TenantID FROM tblUnit" +
                "         WHERE UnitID = @UnitID AND TenantID = @TenantID" +
                "     END" +
                "     ELSE" +
                "     BEGIN" +
                "         SELECT UnitID,(ROW_NUMBER() OVER(ORDER BY UnitID ASC)) as [Serial No],UnitName,UnitShortName,TenantID FROM tblUnit WHERE TenantID = @TenantID" +
                "     END" +
                "END";
                Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            }
            else if (iExecCode == 10)
            {
                sQuery = "DROP PROCEDURE UspGetItemMaster" +
                "GO" +
                "CREATE PROCEDURE [dbo].[UspGetItemMaster]" +
                "(" +
                "@ItemID    NUMERIC   (18,0)," +
                "@TenantID     NUMERIC   (18,0)" +
                ")" +
                "AS" +
                "BEGIN" +
                "     IF @ItemID <> 0 " +
                "     BEGIN" +
                "         SELECT ItemID,(ROW_NUMBER() OVER(ORDER BY ItemID ASC)) as [Serial No],ItemCode,ItemName,CategoryID,Description,Unit,MRP,ROL,Rack,Manufacturer,ActiveStatus,IntLocal,ProductType,ProductTypeID,LedgerID,UNITID,Notes,agentCommPer,BlnExpiryItem,Coolie,FinishedGoodID,MinRate,MaxRate,PLUNo,HSNID,iCatDiscPer,IPGDiscPer,ImanDiscPer,ItemNameUniCode,Minqty,MNFID,ItemCodeUniCode,UPC,BatchMode,Qty,MaxQty,IntNoOrWeight,SystemName,UserID,LastUpdateDate,LastUpdateTime,TenantID FROM tblItemMaster" +
                "         WHERE ItemID = @ItemID AND TenantID = @TenantID" +
                "     END" +
                "     ELSE" +
                "     BEGIN" +
                "         SELECT ItemID,(ROW_NUMBER() OVER(ORDER BY ItemID ASC)) as [Serial No],ItemCode,ItemName,Unit,MRP FROM tblItemMaster WHERE TenantID = @TenantID" +
                "     END" +
                "END";
                Comm.fnExecuteNonQuery("EXECUTE sp_executesql " + sQuery);
            }
        }

    }
}
