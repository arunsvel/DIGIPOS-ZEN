using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventorSync.Forms;
using InventorSync.InventorBL.Master;
using InventorSync.InventorBL.Accounts;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;
using Syncfusion.WinForms.DataGrid;

namespace InventorSync
{
    public partial class frmStockVoucher : Form
    {
        public frmStockVoucher()
        {
            InitializeComponent();
        }

        #region "VARIABLES --------------------------------------------- >>"

        Control ctrl;
        String MVchType, mstrOldData, defaultcriteria, mConvertionTYpe;
        long MVchTypeID, MParentVchTypeID;
        Boolean mblnNoStartTrans, mblnCancelled, BlnHeldBillForSave=false, IsChanged;
        int PrintCopies, SaveCount = 0;
        string sEditCelltext, sEditItemCode, sBatchCode, sUnit;
        string sEditedValueonKeyPress;
        int iSelFromRowItemID;

        Common Comm = new Common();
        UspGetItemMasterInfo GetItmMstinfo = new UspGetItemMasterInfo();
        UspGetEmployeeInfo GetEmpInfo = new UspGetEmployeeInfo();
        UspGetCostCentreInfo GetCctinfo = new UspGetCostCentreInfo();
        UspGetTaxModeInfo GetTaxMinfo = new UspGetTaxModeInfo();
        UspGetAgentinfo GetAgentinfo = new UspGetAgentinfo();
        UspGetLedgerInfo GetLedinfo = new UspGetLedgerInfo();
        UspGetUnitInfo GetUnitInfo = new UspGetUnitInfo();
        
        

        clsItemMaster clsItmMst = new clsItemMaster();
        clsEmployee clsEmp = new clsEmployee();
        clsCostCentre clscct = new clsCostCentre();
        clsTaxMode clsTax = new clsTaxMode();
        clsAgentMaster clsAgent = new clsAgentMaster();
        clsLedger clsLedg = new clsLedger();
        clsUnitMaster clsUnit = new clsUnitMaster();

        DataTable dtItemPublic = new DataTable();
        DataTable dtUnitPublic = new DataTable();
        DataTable dtBatchCode = new DataTable();
        DataTable dtBatchCodeData = new DataTable();

        enum GridColumns
        {
            CItemCode, //0
            CItemName,
            CUnit,
            cBarCode,
            CExpiry,
            cMRP,
            cPrate,
            cQty,
            cFree,
            cSRate1Per,
            cSRate1,
            cSRate2Per,
            cSRate2,
            cSRate3Per,
            cSRate3,
            cSRate4Per,
            cSRate4,
            cSRate5Per,
            cSRate5,
            cRateinclusive,
            //cRate,
            //cRateDiscPer,
            cGrossAmt,
            cDiscPer,
            cDiscAmount,
            cBillDisc,
            cCrate,
            cCRateWithTax,
            ctaxable,
            ctaxPer,
            ctax,
            cIGST,
            cSGST,
            cCGST,
            cNetAmount,
            cItemID,
            cGrossValueAfterRateDiscount,
            cNonTaxable,
            cCCessPer,
            cCCompCessQty,
            cFloodCessPer,
            cFloodCessAmt,
            cStockMRP,
            cAgentCommPer,
            cCoolie,
            cBlnOfferItem,
            cStrOfferDetails,
            cBatchMode
        }

        enum GridBottomColumns
        {
            QtyTotal, //0
            GrossAmt,
            GrossAfterRateDiscount,
            RateDiscountTotal,
            BillDisc,
            GrossAfterItemDiscount,
            ItemDiscountTotal,
            TaxableAmount,
            NonTaxableAmount,
            TaxAmount,
            VatTotal,
            INTERSTATE,
            GSTType,
            CGST,
            SGST,
            IGST,
            CessAmount,
            FloodCessTotal,
            QtyCompCessAmount,
            NetAmount,
            AgentCommission,
            AgentCommissionMode,
            Coolie,
            Savings

        }

        #endregion

        #region "EVENTS ------------------------------------------------ >>"

        private DataGridViewTextBoxEditingControl editingControl;
        private void dgvPurchase_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            editingControl = (DataGridViewTextBoxEditingControl)e.Control;
            editingControl.TextChanged += new EventHandler(editingControl_TextChanged);
        }

        private void frmStockVoucher_Load(object sender, EventArgs e)
        {
            FillTaxMode();
            FillCostCentre();
            FillEmployee();
            FillAgent();
            FillStates();
            AddColumnsToGrid();
            GridInitialize();
            //BottomGridInitialize();

            cboPayment.SelectedIndex = 0;
            txtPrefix.Text = "PR";
            txtInvAutoNo.Text = Comm.gfnGetNextSerialNo("tblPurchase", "AutoNum").ToString();

            Application.DoEvents();
            txtReferenceNo.Focus();
            txtReferenceNo.Select();
        }

        void editingControl_TextChanged(object sender, EventArgs e)
        {
            //string sQuery = "SELECT top 10 Category,ItemCode,ItemName,IGSTTaxPer as [GST %],ItemID,I.CategoryID FROM tblItemMaster I INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID";
            //new frmCompactSearch(GetFromItemSearch, sQuery, "ItemName", dgvPurchase.Location.X + 250, dgvPurchase.Location.Y + 150, 4, 0, editingControl.Text, 4, 0, "ORDER BY ItemName ASC").ShowDialog();
            //editingControl.TextChanged -= editingControl_TextChanged;
            //editingControl.Text = sEditCelltext;
            //editingControl.TextChanged += editingControl_TextChanged;
        }

        private void dgvPurchase_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            editingControl.TextChanged -= new EventHandler(editingControl_TextChanged);
            editingControl = null;
        }

        private void sfDataGridStkVoucher_CurrentCellEndEdit(object sender, Syncfusion.WinForms.DataGrid.Events.CurrentCellEndEditEventArgs e)
        {
            //if (e.DataColumn.GridColumn.MappingName == "CItemName")
            //{
            //    int rowIndex = sfDataGridStkVoucher.CurrentCell.RowIndex;
            //    //int columnIndex = sfDataGridStkVoucher.TableControl.ResolveToGridVisibleColumnIndex(2);
            //    int columnIndex = sfDataGridStkVoucher.CurrentCell.ColumnIndex;
            //    if (columnIndex < 0)
            //        return;
            //    var mappingName = sfDataGridStkVoucher.Columns[columnIndex - 1].MappingName;
            //    var ItmCodemappingName = sfDataGridStkVoucher.Columns[columnIndex - 2].MappingName;
            //    var recordIndex = sfDataGridStkVoucher.TableControl.ResolveToRecordIndex(rowIndex);
            //    var ItemCoderecordIndex = sfDataGridStkVoucher.TableControl.ResolveToRecordIndex(rowIndex);
            //    if (recordIndex < 0)
            //        return;
            //    object data;
            //    object ItemCodedata;

            //    if (sfDataGridStkVoucher.View.TopLevelGroup != null)
            //    {
            //        var record = sfDataGridStkVoucher.View.TopLevelGroup.DisplayElements[recordIndex];
            //        var ItmCoderecord = sfDataGridStkVoucher.View.TopLevelGroup.DisplayElements[ItemCoderecordIndex];
            //        if (!record.IsRecords)
            //            return;
            //        data = (record as Syncfusion.Data.RecordEntry).Data;
            //        ItemCodedata = (ItmCoderecord as Syncfusion.Data.RecordEntry).Data;
            //        //cellVaue = (data.GetType().GetProperty(mappingName).GetValue(data, null).ToString());
            //    }
            //    else
            //    {
            //        data = sfDataGridStkVoucher.View.Records.GetItemAt(recordIndex);
            //        ItemCodedata = sfDataGridStkVoucher.View.Records.GetItemAt(ItemCoderecordIndex);
            //        //cellVaue = (data.GetType().GetProperty(mappingName).GetValue(data, null).ToString());
            //    }
            //    //Get the cell value            
            //    var cellValue = this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().GetValue(e.DataRow.RowData, e.DataColumn.GridColumn.MappingName).ToString();

            //    if (sfDataGridStkVoucher.CurrentCell != null)
            //    {
            //        // Get the CurrentCellValue
            //        var currentCellValue = sfDataGridStkVoucher.CurrentCell.CellRenderer.GetControlValue();
            //        MessageBox.Show(currentCellValue.ToString(), "Current Cell Value");
            //    }

            //    string sQuery = "SELECT top 10 Category,ItemCode,ItemName,IGSTTaxPer as [GST %],ItemID,I.CategoryID FROM tblItemMaster I INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID";
            //    new frmCompactSearch(GetFromItemSearch, sQuery, "ItemName", sfDataGridStkVoucher.Location.X + 226, sfDataGridStkVoucher.Location.Y + 108, 4, 0, cellValue, 4, 0, "ORDER BY ItemName ASC").ShowDialog();
            //    this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().SetValue(data, mappingName, sEditCelltext);
            //    this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().SetValue(ItemCodedata, ItmCodemappingName, sEditItemCode);
            //}
        }

        private void sfDataGridStkVoucher_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //Get the row index of current cell             
            var rowindex = sfDataGridStkVoucher.CurrentCell.RowIndex;
            if (rowindex < (sfDataGridStkVoucher.RowCount - 1))
            {
                //Get the column index of current cell 
                var colindex = sfDataGridStkVoucher.CurrentCell.ColumnIndex;
                if (e.KeyCode == Keys.Enter)
                {
                    //Need to move the selection into next row of current column  
                    //sfDataGridStkVoucher.CurrentCell. (rowindex + 1, colindex);
                    //e.Handled = true;
                }
            }
        }

        private void GridFillAsperItemCompact(string sMappingName, int iCurrRowIndex, int iCurrColIndex)
        {
            //string sActMappingName = "";
            //int ActrecordIndex = "";
            //if (sMappingName == "CItemName")
            //{
            //    if (iCurrColIndex > 0)
            //    {
            //        sActMappingName = sfDataGridStkVoucher.Columns[(int)GridColumns.CItemName].MappingName;
            //        ActrecordIndex = sfDataGridStkVoucher.TableControl.ResolveToRecordIndex(iCurrRowIndex);

            //        if (sfDataGridStkVoucher.View.TopLevelGroup != null)
            //        {
            //            var record = sfDataGridStkVoucher.View.TopLevelGroup.DisplayElements[sfDataGridStkVoucher.TableControl.ResolveToRecordIndex(iCurrRowIndex)];
            //            var ItmCoderecord = sfDataGridStkVoucher.View.TopLevelGroup.DisplayElements[ItemCoderecordIndex];
            //            if (!record.IsRecords)
            //                return;
            //            data = (record as Syncfusion.Data.RecordEntry).Data;
            //            ItemCodedata = (ItmCoderecord as Syncfusion.Data.RecordEntry).Data;
            //        }
            //        else
            //        {
            //            data = sfDataGridStkVoucher.View.Records.GetItemAt(recordIndex);
            //            ItemCodedata = sfDataGridStkVoucher.View.Records.GetItemAt(ItemCoderecordIndex);
            //        }
            //    }
            //}

            // OLD Code ============================ >>
            //int rowIndex = sfDataGridStkVoucher.CurrentCell.RowIndex;
            ////int columnIndex = sfDataGridStkVoucher.TableControl.ResolveToGridVisibleColumnIndex(2);
            //int columnIndex = sfDataGridStkVoucher.CurrentCell.ColumnIndex;
            //if (columnIndex < 0)
            //    return;
            //var mappingName = sfDataGridStkVoucher.Columns[columnIndex - 1].MappingName;
            //var ItmCodemappingName = sfDataGridStkVoucher.Columns[columnIndex - 2].MappingName;
            //var recordIndex = sfDataGridStkVoucher.TableControl.ResolveToRecordIndex(rowIndex);
            //var ItemCoderecordIndex = sfDataGridStkVoucher.TableControl.ResolveToRecordIndex(rowIndex);
            //if (recordIndex < 0)
            //    return;
            //object data;
            //object ItemCodedata;

            //if (sfDataGridStkVoucher.View.TopLevelGroup != null)
            //{
            //    var record = sfDataGridStkVoucher.View.TopLevelGroup.DisplayElements[recordIndex];
            //    var ItmCoderecord = sfDataGridStkVoucher.View.TopLevelGroup.DisplayElements[ItemCoderecordIndex];
            //    if (!record.IsRecords)
            //        return;
            //    data = (record as Syncfusion.Data.RecordEntry).Data;
            //    ItemCodedata = (ItmCoderecord as Syncfusion.Data.RecordEntry).Data;
            //}
            //else
            //{
            //    data = sfDataGridStkVoucher.View.Records.GetItemAt(recordIndex);
            //    ItemCodedata = sfDataGridStkVoucher.View.Records.GetItemAt(ItemCoderecordIndex);
            //}
        }

        private void sfDataGridStkVoucher_CurrentCellKeyPress(object sender, Syncfusion.WinForms.DataGrid.Events.CurrentCellKeyPressEventArgs e)
        {
            sEditedValueonKeyPress = e.KeyPressEventArgs.KeyChar.ToString();
        }

        private object GiveObjectAsperRecord(int iRowIndex)
        {
            object objRet;
            if (sfDataGridStkVoucher.View.TopLevelGroup != null)
            {
                objRet = (sfDataGridStkVoucher.View.TopLevelGroup.DisplayElements[sfDataGridStkVoucher.TableControl.ResolveToRecordIndex(iRowIndex)] as Syncfusion.Data.RecordEntry).Data;
            }
            else
            {
                objRet = sfDataGridStkVoucher.View.Records.GetItemAt(sfDataGridStkVoucher.TableControl.ResolveToRecordIndex(iRowIndex));
            }
            return objRet;
        }

        private string GiveMappingNameAsperRecord(int iColIndex)
        {
            return sfDataGridStkVoucher.Columns[iColIndex - 1].MappingName;
        }

        private void FillBatchCode(int itemID = 0)
        {
            string sQuery = "select StockID,BatchCode from tblStock WHERE TenantID = " + Global.gblTenantID + " and ItemID = " + itemID + "";
            //string sQuery = "select StockID,BatchCode from tblStock";
            dtBatchCode = Comm.fnGetData(sQuery).Tables[0];
            //if (dtBatchCode.Rows.Count == 0)
            //{
            //    //dtBatchCode.Columns.Add("StockID");
            //    //dtBatchCode.Columns.Add("BatchCode");
            //    dtBatchCode.Rows.Add(1, "None");
            //}
        }

        private DataTable LoadBatchCode(int iBatchCode =0)
        {
            string sQuery = "select StockID,BatchCode from tblStock WHERE TenantID = " + Global.gblTenantID + " and StockID = " + iBatchCode + "";
            return Comm.fnGetData(sQuery).Tables[0];
            //if (dtBatchCode.Rows.Count == 0)
            //{
            //    //dtBatchCode.Columns.Add("StockID");
            //    //dtBatchCode.Columns.Add("BatchCode");
            //    dtBatchCode.Rows.Add(1, "None");
            //}
        }

        private void sfDataGridStkVoucher_CurrentCellBeginEdit(object sender, Syncfusion.WinForms.DataGrid.Events.CurrentCellBeginEditEventArgs e)
        {
            string sQuery;
            this.sfDataGridStkVoucher.EditorSelectionBehavior = Syncfusion.WinForms.DataGrid.Enums.EditorSelectionBehavior.Default;
            if (e.DataColumn.GridColumn.MappingName == "CItemName")
            {
                this.sfDataGridStkVoucher.EndUpdate();
                //Get the cell value            
                var cellValue = this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().GetValue(e.DataRow.RowData, e.DataColumn.GridColumn.MappingName).ToString();
                sQuery = "SELECT (ItemCode+ItemName+CONVERT(VARCHAR,ISNULL(IGSTTaxPer,0))) as AnyWhere,ItemCode,ItemName,IGSTTaxPer as [GST %],ItemID,I.CategoryID FROM tblItemMaster I INNER JOIN tblCategories C ON C.CategoryID = I.CategoryID";
                new frmCompactSearch(GetFromItemSearch, sQuery, "Anywhere|ItemCode|ItemName|CONVERT(VARCHAR,ISNULL(IGSTTaxPer,0))", sfDataGridStkVoucher.Location.X + 126, sfDataGridStkVoucher.Location.Y + 108, 3, 0, sEditedValueonKeyPress, 3, 0, "ORDER BY ItemName ASC", 0, 0, "Item Name Search...", 0, "100,200,100,0,0").ShowDialog();

                if (dtItemPublic.Rows.Count > 0)
                {
                    //this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().SetValue(GiveObjectAsperRecord(sfDataGridStkVoucher.CurrentCell.RowIndex), GiveMappingNameAsperRecord(sfDataGridStkVoucher.CurrentCell.ColumnIndex), dtItemPublic.Rows[0]["ItemName"].ToString());
                    this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().SetValue(GiveObjectAsperRecord(sfDataGridStkVoucher.CurrentCell.RowIndex), GiveMappingNameAsperRecord((int)GridColumns.CItemName + 1), dtItemPublic.Rows[0]["ItemName"].ToString());
                    this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().SetValue(GiveObjectAsperRecord(sfDataGridStkVoucher.CurrentCell.RowIndex), GiveMappingNameAsperRecord((int)GridColumns.CItemCode + 1), dtItemPublic.Rows[0]["ItemCode"].ToString());
                    this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().SetValue(GiveObjectAsperRecord(sfDataGridStkVoucher.CurrentCell.RowIndex), GiveMappingNameAsperRecord((int)GridColumns.cBarCode + 1), dtItemPublic.Rows[0]["BatchCode"].ToString());
                    this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().SetValue(GiveObjectAsperRecord(sfDataGridStkVoucher.CurrentCell.RowIndex), GiveMappingNameAsperRecord((int)GridColumns.CUnit + 1), dtItemPublic.Rows[0]["Unit"].ToString());
                    this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().SetValue(GiveObjectAsperRecord(sfDataGridStkVoucher.CurrentCell.RowIndex), GiveMappingNameAsperRecord((int)GridColumns.cMRP + 1), dtItemPublic.Rows[0]["MRP"].ToString());
                    this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().SetValue(GiveObjectAsperRecord(sfDataGridStkVoucher.CurrentCell.RowIndex), GiveMappingNameAsperRecord((int)GridColumns.cPrate + 1), dtItemPublic.Rows[0]["PRate"].ToString());
                    FillBatchCode(Convert.ToInt32(dtItemPublic.Rows[0]["ItemID"].ToString()));
                    iSelFromRowItemID = Convert.ToInt32(dtItemPublic.Rows[0]["ItemID"].ToString());
                    this.sfDataGridStkVoucher.EditorSelectionBehavior = Syncfusion.WinForms.DataGrid.Enums.EditorSelectionBehavior.SelectAll;
                    this.sfDataGridStkVoucher.Focus();
                    this.sfDataGridStkVoucher.TableControl.Select();
                    this.sfDataGridStkVoucher.MoveToCurrentCell(new Syncfusion.WinForms.GridCommon.ScrollAxis.RowColumnIndex(sfDataGridStkVoucher.CurrentCell.RowIndex, (int)GridColumns.cBarCode + 1));
                }
            }
            else if (e.DataColumn.GridColumn.MappingName == "cBarCode")
            {
                var cellValue = this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().GetValue(e.DataRow.RowData, e.DataColumn.GridColumn.MappingName).ToString();
                sQuery = "select BatchCode,StockID from tblStock WHERE TenantID = " + Global.gblTenantID + " and ItemID = " + iSelFromRowItemID + "";
                new frmCompactSearch(GetFromBatchCodeSearch, sQuery, "BatchCode", sfDataGridStkVoucher.Location.X + 226, sfDataGridStkVoucher.Location.Y + 108, 1, 0, sEditedValueonKeyPress, 1, 0, "ORDER BY StockID ASC", 0, 0, "Batchcode Search...", 0, "100,0").ShowDialog();

                if (dtBatchCodeData.Rows.Count > 0)
                {
                    this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().SetValue(GiveObjectAsperRecord(sfDataGridStkVoucher.CurrentCell.RowIndex), GiveMappingNameAsperRecord((int)GridColumns.cBarCode + 1), dtBatchCode.Rows[0]["BatchCode"].ToString());
                    this.sfDataGridStkVoucher.EditorSelectionBehavior = Syncfusion.WinForms.DataGrid.Enums.EditorSelectionBehavior.SelectAll;
                    this.sfDataGridStkVoucher.Focus();
                    this.sfDataGridStkVoucher.TableControl.Select();
                    this.sfDataGridStkVoucher.MoveToCurrentCell(new Syncfusion.WinForms.GridCommon.ScrollAxis.RowColumnIndex(sfDataGridStkVoucher.CurrentCell.RowIndex, (int)GridColumns.CExpiry + 1));
                }
            }
            //else if (e.DataColumn.GridColumn.MappingName == "CUnit")
            //{
            //    var cellValue = this.sfDataGridStkVoucher.View.GetPropertyAccessProvider().GetValue(e.DataRow.RowData, e.DataColumn.GridColumn.MappingName).ToString();
            //    sQuery = "Select (UnitShortName+UnitName) as AnyWhere,UnitShortName as [Unit],UnitName,UnitID from tblUnit WHERE TenantID = " + Global.gblTenantID + "";
            //    new frmCompactSearch(GetFromUnitSearch, sQuery, "Anywhere|UnitShortName", sfDataGridStkVoucher.Location.X + 226, sfDataGridStkVoucher.Location.Y + 108, 3, 0, sEditedValueonKeyPress, 3, 0, "ORDER BY UnitShortName ASC", 0, 0, "Unit Search...", 0, "100,0,0").ShowDialog();

            //    if (dtUnitPublic.Rows.Count > 0)
            //    {

            //    }
            //}
        }

        private Boolean GetFromBatchCodeSearch(string sReturn)
        {
            string[] sCompSearchData = sReturn.Split('|');
            if (sCompSearchData.Length > 0)
            {
                if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                {
                    dtBatchCodeData.Clear();
                    dtBatchCodeData = LoadBatchCode(Convert.ToInt32(sCompSearchData[0].ToString()));
                     return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSupplier_TextChanged(object sender, EventArgs e)
        {
            if (txtSupplier.Text != "")
            {
                string sQuery = "SELECT  LedgerName+LedgerCode+Phone+MobileNo+Address as AnyWhere,LedgerCode as [Supplier Code],LedgerName as [Supplier Name] ,MobileNo ,Address,LID  FROM tblLedger where UPPER(groupName)='SUPPLIER' AND TenantID=" + Global.gblTenantID + "";
                new frmCompactSearch(GetFromSupplierSearch, sQuery, "AnyWhere|LedgerCode|LedgerName|MobileNo|Address", txtSupplier.Location.X + 800, txtSupplier.Location.Y - 20, 4, 0, txtSupplier.Text, 4, 0, "ORDER BY LedgerName ASC",0,0,"Supplier Search ...",0,"100,200,100,200,0").ShowDialog();
            }
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ctrl = (Control)sender;
                if (ctrl is TextBox)
                {
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                    {
                        this.SelectNextControl(ctrl, true, true, true, true);
                    }
                    else if (e.KeyCode == Keys.Up)
                    {
                        this.SelectNextControl(ctrl, false, true, true, true);
                    }
                    else
                        return;
                }
                else
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        this.SelectNextControl(ctrl, true, true, true, true);

                    }
                    else if (e.KeyCode == Keys.Up && e.Control)
                    {
                        this.SelectNextControl(ctrl, false, true, true, true);

                    }
                    else
                        return;
                }
                Cursor.Current = Cursors.Default;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Input box order is properly working ?" + "\n" + ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
            }

        }

        private Boolean GetFromSupplierSearch(string LstIDandText)
        {
            string[] sCompSearchData = LstIDandText.Split('|');
            DataTable dtSupp = new DataTable();

            if (sCompSearchData.Length > 0)
            {
                if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                {
                    GetLedinfo.LID = Convert.ToInt32(sCompSearchData[0].ToString());
                    GetLedinfo.TenantID = Global.gblTenantID;
                    GetLedinfo.GroupName = "SUPPLIER";
                    dtSupp = clsLedg.GetLedger(GetLedinfo);
                    if (dtSupp.Rows.Count > 0)
                    {
                        this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                        txtSupplier.Text = dtSupp.Rows[0]["LName"].ToString();
                        this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                        lblLID.Text = dtSupp.Rows[0]["LID"].ToString();
                        txtAddress1.Text = dtSupp.Rows[0]["Address"].ToString();
                        txtMobile.Text = dtSupp.Rows[0]["MobileNo"].ToString();
                        //txtTaxRegn.Text = dtSupp.Rows[0]["Address"].ToString();
                        FillStates(Convert.ToInt32(dtSupp.Rows[0]["StateID"].ToString()));
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    this.txtSupplier.TextChanged -= this.txtSupplier_TextChanged;
                    txtSupplier.Text = sCompSearchData[1].ToString();
                    this.txtSupplier.TextChanged += this.txtSupplier_TextChanged;
                    return true;
                }
            }
            else
                return false;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void txtReferenceNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                cboPayment.Focus();
        }

        private void txtTaxRegn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                cboState.Focus();
        }

        private void txtAddress1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.sfDataGridStkVoucher.Focus();
                this.sfDataGridStkVoucher.TableControl.Select();
                this.sfDataGridStkVoucher.MoveToCurrentCell(new Syncfusion.WinForms.GridCommon.ScrollAxis.RowColumnIndex(1, 0));
            }
        }

        #endregion

        #region "METHODS ----------------------------------------------- >>"

        private void FillEmployee(int iSelID = 0)
        {
            DataTable dtEmp = new DataTable();
            GetEmpInfo.EmpID = 0;
            GetEmpInfo.TenantID = Global.gblTenantID;
            GetEmpInfo.blnSalesStaff = true;
            dtEmp = clsEmp.GetEmployee(GetEmpInfo);
            if (dtEmp.Rows.Count > 0)
            {
                Comm.LoadControl(cboSalesStaff, dtEmp, "", false, false, "Name", "EmpID");
                if (iSelID != 0)
                    cboSalesStaff.SelectedValue = iSelID;
            }
        }

        private void FillCostCentre(int iSelID = 0)
        {
            DataTable dtCct = new DataTable();
            GetCctinfo.CCID = 0;
            GetCctinfo.TenantID = Global.gblTenantID;
            dtCct = clscct.GetCostCentre(GetCctinfo);
            if (dtCct.Rows.Count > 0)
            {
                Comm.LoadControl(cboCostCentre, dtCct, "",false,false, "CCName", "CCID");
                if (iSelID != 0)
                    cboCostCentre.SelectedValue = iSelID;
            }
        }

        private void FillTaxMode(int iSelID = 0)
        {
            DataTable dtTax = new DataTable();
            GetTaxMinfo.TaxModeID = 0;
            GetTaxMinfo.TenantID = Global.gblTenantID;
            dtTax = clsTax.GetTaxMode(GetTaxMinfo);
            if (dtTax.Rows.Count > 0)
            {
                Comm.LoadControl(cboTaxMode, dtTax, "", false, false, "TaxMode", "TaxModeID");
                if (iSelID != 0)
                    cboTaxMode.SelectedValue = iSelID;
            }
        }

        private void FillAgent(int iSelID = 0)
        {
            DataTable dtAgent = new DataTable();
            GetAgentinfo.AgentID = 0;
            GetAgentinfo.TenantID = Global.gblTenantID;
            dtAgent = clsAgent.GetAgentMaster(GetAgentinfo);
            if (dtAgent.Rows.Count > 0)
            {
                Comm.LoadControl(cboAgent, dtAgent, "", false, false, "Agent Name", "AgentID");
                if (iSelID != 0)
                    cboAgent.SelectedValue = iSelID;
            }
        }

        private void FillStates(int iSelID = 0)
        {
            DataTable dtState = new DataTable();
            dtState = Comm.fnGetData("SELECT StateCode,State,StateId FROM tblStates WHERE TenantID =" + Global.gblTenantID + "").Tables[0];
            if (dtState.Rows.Count > 0)
            {
                Comm.LoadControl(cboState, dtState, "", false, false, "State", "StateId");
                if (iSelID != 0)
                {
                    cboState.SelectedValue = iSelID;
                    foreach (System.Data.DataRow row in dtState.Rows)
                    {
                        if (Convert.ToInt32(row["StateId"].ToString()) == iSelID)
                        {
                            lblStateCode.Text = row["StateCode"].ToString();
                        }
                    }
                }
            }
        }

        private Boolean GetFromItemSearch(string sReturn)
        {
            string[] sCompSearchData = sReturn.Split('|');
            if (sCompSearchData.Length > 0)
            {
                if (Convert.ToInt32(sCompSearchData[0].ToString()) != 0)
                {
                    GetItmMstinfo.ItemID = Convert.ToInt32(sCompSearchData[0].ToString());
                    GetItmMstinfo.TenantID = Global.gblTenantID;
                    //Commented due to error --> 14Dec
                    //dtItemPublic = clsItmMst.GetItemMaster(GetItmMstinfo);
                    //if (dtItemPublic.Rows.Count > 0)
                    //{
                    //    sEditCelltext = dtItemPublic.Rows[0]["ItemName"].ToString();
                    //    sEditItemCode = dtItemPublic.Rows[0]["ItemCode"].ToString();
                    //    sBatchCode = dtItemPublic.Rows[0]["BatchCode"].ToString();
                    //    sUnit = dtItemPublic.Rows[0]["Unit"].ToString();
                    //}

                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        private void AddColumnsToGrid()
        {
            //FillBatchCode(1);
            //this.sfDataGridStkVoucher.Columns.Add(new GridTextColumn() { MappingName = "CSlNo", HeaderText = "Serial No" }); //0
            this.sfDataGridStkVoucher.Columns.Add(new GridTextColumn() { MappingName = "CItemCode", HeaderText = "Item Code" }); //1
            this.sfDataGridStkVoucher.Columns.Add(new GridTextColumn() { MappingName = "CItemName", HeaderText = "Item" }); //2
            this.sfDataGridStkVoucher.Columns.Add(new GridTextColumn() { MappingName = "CUnit", HeaderText = "Unit" }); //3
            this.sfDataGridStkVoucher.Columns.Add(new GridTextColumn() { MappingName = "cBarCode", HeaderText = "Batch Code" }); //4
            //this.sfDataGridStkVoucher.Columns.Add(new GridComboBoxColumn() { MappingName = "cBarCode", HeaderText = "Batch Code", DisplayMember = "BatchCode", ValueMember = "StockID", DropDownStyle = Syncfusion.WinForms.ListView.Enums.DropDownStyle.DropDown, AutoCompleteMode = AutoCompleteMode.SuggestAppend, DataSource = dtBatchCode }); //4
            this.sfDataGridStkVoucher.Columns.Add(new GridDateTimeColumn() { MappingName = "CExpiry", HeaderText = "Expiry Date" }); //5
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cMRP", HeaderText = "MRP" }); //6
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cPrate", HeaderText = "PRate" }); //7
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cQty", HeaderText = "Qty" }); //8
            //this.sfDataGridStkVoucher.Columns.Add(new GridTextColumn() { MappingName = "CSerialNo", HeaderText = "Serial No" });
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cFree", HeaderText = "Free" }); //9
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cSRate1Per", HeaderText = "SRate 1 %" }); //10
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cSRate1", HeaderText = "SRate 1" }); //11
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cSRate2Per", HeaderText = "SRate 2 %" }); //12
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cSRate2", HeaderText = "SRate 2" }); //13
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cSRate3Per", HeaderText = "SRate 3 %" }); //14
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cSRate3", HeaderText = "SRate 3" }); //15
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cSRate4Per", HeaderText = "SRate 4 %" }); //16
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cSRate4", HeaderText = "SRate 4" }); //17
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cSRate5Per", HeaderText = "SRate 5 %" }); //18
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cSRate5", HeaderText = "SRate 5" }); //19
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cRateinclusive", HeaderText = "Rate Inc." }); //20

            //this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cRate", HeaderText = "Rate" }); //21
            //this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cRateDiscPer", HeaderText = "Rate Discount %" }); //22
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cGrossAmt", HeaderText = "Gross Amt" }); //23
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cDiscPer", HeaderText = "Discount %" }); //24
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cDiscAmount", HeaderText = "Discount Amt" }); //25
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cBillDisc", HeaderText = "Bill Discount" }); //26
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cCrate", HeaderText = "CRate" }); //27
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cCRateWithTax", HeaderText = "CRate With Tax" }); //28
            
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "ctaxable", HeaderText = "Taxable" }); //29
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "ctaxPer", HeaderText = "Tax %" }); //30
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "ctax", HeaderText = "Tax" }); //31
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cIGST", HeaderText = "IGST" }); //32
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cSGST", HeaderText = "SGST" }); //33
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cCGST", HeaderText = "CGST" }); //34
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cNetAmount", HeaderText = "Net Amt" }); //35
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cItemID", HeaderText = "ItemID" }); //36
            
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cGrossValueAfterRateDiscount", HeaderText = "Gross Val" }); //37
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cNonTaxable", HeaderText = "Non Taxable" }); //38
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cCCessPer", HeaderText = "Cess %" }); //39
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cCCompCessQty", HeaderText = "Comp Cess Qty" }); //40
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cFloodCessPer", HeaderText = "Flood Cess %" }); //41
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cFloodCessAmt", HeaderText = "Flood Cess Amt" }); //42
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cStockMRP", HeaderText = "Stock MRP" }); //43
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cAgentCommPer", HeaderText = "Agent Comm. %" }); //44
            this.sfDataGridStkVoucher.Columns.Add(new GridNumericColumn() { MappingName = "cCoolie", HeaderText = "Coolie" }); //45
            this.sfDataGridStkVoucher.Columns.Add(new GridCheckBoxColumn() { MappingName = "cBlnOfferItem", HeaderText = "Offer Item" }); //46
            this.sfDataGridStkVoucher.Columns.Add(new GridTextColumn() { MappingName = "cStrOfferDetails", HeaderText = "Offer Det." }); //47
            this.sfDataGridStkVoucher.Columns.Add(new GridTextColumn() { MappingName = "cBatchMode", HeaderText = "Batch Mode" }); //48

            this.sfDataGridStkVoucher.Style.BorderStyle = BorderStyle.None;
            this.sfDataGridStkVoucher.Style.HeaderStyle.BackColor = Color.FromArgb(65, 85, 104);
            this.sfDataGridStkVoucher.Style.HeaderStyle.TextColor = Color.White;

            //sfDataGridStkVoucher.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            //sfDataGridStkVoucher.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(65, 85, 104);
            //sfDataGridStkVoucher.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void GridInitialize()
        {
            string sColName = "";
            string sData = "";
            DataTable dt = new DataTable("ItemDetails");
            for (int i = 0; i < Enum.GetNames(typeof(GridColumns)).Length; i++)
            {
                sColName = Enum.GetName(typeof(GridColumns), i);
                dt.Columns.Add(sColName);
            }
            dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

            // Hide Columns
            for (int k = 34; k < Enum.GetNames(typeof(GridColumns)).Length; k++)
            {
                this.sfDataGridStkVoucher.Columns[k].Visible = false;
            }

            sfDataGridStkVoucher.DataSource = dt;
            //sfDataGridStkVoucher.AddNewRowPosition = Syncfusion.WinForms.DataGrid.Enums.RowPosition.FixedTop;
        }

        //private void FillBatchCode(int itemID = 0)
        //{
        //    string sQuery = "select StockID,BatchCode from tblStock WHERE TenantID = " + Global.gblTenantID + " and ItemID = " + itemID + "";
        //    dtBatchCode = Comm.fnGetData(sQuery).Tables[0];

        //}

        //private void BottomGridInitialize()
        //{
        //    string sColName = "";
        //    DataTable dt = new DataTable("BottomData");
        //    for (int j = 0; j < Enum.GetNames(typeof(GridBottomColumns)).Length; j++)
        //    {
        //        sColName = Enum.GetName(typeof(GridBottomColumns), j);
        //        dt.Columns.Add(sColName);
        //    }
        //    dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

        //    //// Hide Columns
        //    //for (int k = 34; k < Enum.GetNames(typeof(GridColumns)).Length; k++)
        //    //{
        //    //    this.dgvBottomControls.Columns[k].Visible = false;
        //    //}

        //    dgvBottomControls.DataSource = dt;
        //    //sfDataGridStkVoucher.AddNewRowPosition = Syncfusion.WinForms.DataGrid.Enums.RowPosition.FixedTop;
        //}

        #endregion

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
