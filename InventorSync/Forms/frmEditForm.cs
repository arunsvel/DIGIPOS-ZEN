using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventorSync.InventorBL.Helper;
using InventorSync.InventorBL.Master;
using InventorSync.Info;

namespace InventorSync.Forms
{
    public partial class frmEditForm : Form
    {
        // ======================================================== >>
        // Description: The Edit Form for Edit/Delete Changes
        // Developed By: Dipu Joseph
        // Completed Date & Time: 09-Sep-2021 8.00 PM
        // Last Edited By:
        // Last Edited Date & Time:
        // ======================================================== >>

        #region "VARIABLES -------------------------------------------- >>"

        Common Comm = new Common();
        clsEditCommand EdtComm = new clsEditCommand();
        clsBrandMaster clsBrand = new clsBrandMaster();
        clsColorMaster clsColor = new clsColorMaster();
        clsSizeMaster clssize = new clsSizeMaster();
        clsDiscountGroup clsDiscG = new clsDiscountGroup();

        UspGetCategoriesinfo Catinfo = new UspGetCategoriesinfo();
        UspGetManufacturerInfo Manfinfo = new UspGetManufacturerInfo();
        UspGetBrandinfo GetBrandInfo = new UspGetBrandinfo();
        UspGetColorInfo GetcolorInfo = new UspGetColorInfo();
        UspGetSizeInfo Getsizeinfo = new UspGetSizeInfo();
        UspGetDiscountGroupInfo GetDiscGinfo = new UspGetDiscountGroupInfo();
        DataTable dtgetData = new DataTable();

        #endregion

        #region "EVENTS ----------------------------------------------- >>"

        public frmEditForm(string sMenuType = "")
        {
            InitializeComponent();
            if (sMenuType.ToUpper() == "CATEGORIES")
                rdoCategories.Checked = true;
            else if (sMenuType.ToUpper() == "MANUFACTURER")
                rdoManufacturer.Checked = true;
            else if (sMenuType.ToUpper() == "BRAND")
                rdoBrand.Checked = true;
            else if (sMenuType.ToUpper() == "COLOR")
                rdoColor.Checked = true;
            else if (sMenuType.ToUpper() == "SIZE")
                rdoSize.Checked = true;

            GetDataAsperMenuClick(sMenuType.ToUpper().Trim());
            //if (dtgetData.Rows.Count > 0)
            //{
            //    gridGroupingControlSearch.DataSource = dtgetData;
            //    GridSettings(sMenuType.ToUpper().Trim());
            //    SampleCustomization();
            //}
        }

        private void frmEditForm_Load(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void gridGroupingControlSearch_FilterBarSelectedItemChanging(object sender, Syncfusion.Windows.Forms.Grid.Grouping.FilterBarSelectedItemChangingEventArgs e)
        {
            e.Cancel = false;
        }

        private void rdoCategories_Click(object sender, EventArgs e)
        {
            if (rdoCategories.Checked == true)
            {
                GetDataAsperMenuClick("CATEGORIES");
            }
        }

        private void rdoManufacturer_Click(object sender, EventArgs e)
        {
            if (rdoManufacturer.Checked == true)
            {
                GetDataAsperMenuClick("MANUFACTURER");
            }
        }

        private void rdoBrand_Click(object sender, EventArgs e)
        {
            if (rdoBrand.Checked == true)
            {
                GetDataAsperMenuClick("BRAND");
            }
        }

        private void rdoSize_Click(object sender, EventArgs e)
        {
            if (rdoSize.Checked == true)
            {
                GetDataAsperMenuClick("SIZE");
            }
        }

        private void rdoColor_Click(object sender, EventArgs e)
        {
            if (rdoColor.Checked == true)
            {
                GetDataAsperMenuClick("COLOR");
            }
        }

        private void rdoDiscGoup_Click(object sender, EventArgs e)
        {
            if (rdoDiscGoup.Checked == true)
            {
                GetDataAsperMenuClick("DISCGROUP");
            }
        }

        private void gridGroupingControlSearch_QueryCellStyleInfo(object sender, Syncfusion.Windows.Forms.Grid.Grouping.GridTableCellStyleInfoEventArgs e)
        {
            if (e.TableCellIdentity.ColIndex == 0)
            {
                e.Style.ReadOnly = true;
            }
        }

        #endregion

        #region "METHODS ---------------------------------------------- >>"

        private void GetDataAsperMenuClick(string sMenuType = "")
        {
            DataTable dtResult = new DataTable();
            if (sMenuType.ToUpper() == "CATEGORIES")
            {
                Catinfo.CategoryID = 0;
                Catinfo.TenantId = Global.gblTenantID;
                dtResult = EdtComm.GetCategories(Catinfo);
            }
            else if (sMenuType.ToUpper() == "MANUFACTURER")
            {
                Manfinfo.MnfID = 0;
                Manfinfo.TenantID = Global.gblTenantID;
                dtResult = EdtComm.GetManufacturer(Manfinfo);
            }
            else if (sMenuType.ToUpper() == "BRAND")
            {
                GetBrandInfo.brandID = 0;
                GetBrandInfo.TenantID = Global.gblTenantID;
                dtResult = clsBrand.GetBrandMaster(GetBrandInfo);
            }
            else if (sMenuType.ToUpper() == "COLOR")
            {
                GetcolorInfo.ColorID = 0;
                GetcolorInfo.TenantID = Global.gblTenantID;
                dtResult = clsColor.GetColorMaster(GetcolorInfo);
            }
            else if (sMenuType.ToUpper() == "SIZE")
            {
                Getsizeinfo.SizeID = 0;
                Getsizeinfo.TenantID = Global.gblTenantID;
                dtResult = clssize.GetSizeMaster(Getsizeinfo);
            }
            else if (sMenuType.ToUpper() == "DISCGROUP")
            {
                GetDiscGinfo.DiscountGroupID = 0;
                GetDiscGinfo.TenantID = Global.gblTenantID;
                dtResult = clsDiscG.GetDiscountGroup(GetDiscGinfo);
            }
            Comm.LoadGrdiControl(gridGroupingControlSearch, dtResult,true);
        }


        //void GridSettings(string sMenuType = "")
        //{
        //    this.gridGroupingControlSearch.TopLevelGroupOptions.ShowAddNewRecordBeforeDetails = false;
        //    this.gridGroupingControlSearch.TopLevelGroupOptions.ShowCaption = false;
        //    this.gridGroupingControlSearch.NestedTableGroupOptions.ShowAddNewRecordBeforeDetails = false;
        //    this.BackColor = Color.White;
        //    this.gridGroupingControlSearch.TableModel.EnableLegacyStyle = false;
        //    this.gridGroupingControlSearch.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.Office2016White;
        //    this.gridGroupingControlSearch.TableOptions.ListBoxSelectionMode = SelectionMode.MultiExtended;
        //    //this.gridGroupingControlSearch.GetTable("Orders").DefaultRecordRowHeight = this.gridGroupingControlSearch.Table.DefaultRecordRowHeight;
        //    this.gridGroupingControlSearch.TableControl.DpiAware = true;
        //    this.gridGroupingControlSearch.WantTabKey = false;
        //    this.gridGroupingControlSearch.TopLevelGroupOptions.ShowFilterBar = true;

        //    if (sMenuType.ToUpper() == "CATEGORIES")
        //    {
        //        // Header Text name changes.
        //        if (this.gridGroupingControlSearch.TableDescriptor.Columns.Count > 0)
        //        {
        //            this.gridGroupingControlSearch.TableDescriptor.Columns["CategoryID"].HeaderText = "CategoryID";
        //            this.gridGroupingControlSearch.TableDescriptor.Columns["CategoryID"].Width = 0;
        //            this.gridGroupingControlSearch.TableDescriptor.Columns["Category"].HeaderText = "Category Name";
        //            this.gridGroupingControlSearch.TableDescriptor.Columns["Category"].Width = 200;
        //            this.gridGroupingControlSearch.TableDescriptor.Columns["ParentCategory"].HeaderText = "Parent Category";
        //            this.gridGroupingControlSearch.TableDescriptor.Columns["ParentCategory"].Width = 200;
        //            this.gridGroupingControlSearch.TableDescriptor.Columns["CatDiscPer"].HeaderText = "Discount %";
        //            this.gridGroupingControlSearch.TableDescriptor.Columns["CatDiscPer"].Width = 100;
        //            this.gridGroupingControlSearch.TableDescriptor.Columns["Remarks"].HeaderText = "Remarks";
        //            this.gridGroupingControlSearch.TableDescriptor.Columns["Remarks"].Width = (this.gridGroupingControlSearch.Width - 520);
        //        }
        //    }
        //    else if (sMenuType.ToUpper() == "MANUFACTURER")
        //    {
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["MnfID"].HeaderText = "ManufactureID";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["MnfID"].Width = 0;
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["MnfName"].HeaderText = "Manufacturer";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["MnfName"].Width = (this.gridGroupingControlSearch.Width - 340);
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["MnfShortName"].HeaderText = "Manufacturer Shortname";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["MnfShortName"].Width = 200;
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["DiscPer"].HeaderText = "Discount %";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["DiscPer"].Width = 100;
        //    }
        //    else if (sMenuType.ToUpper() == "BRAND")
        //    {
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["brandID"].HeaderText = "BrandID";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["brandID"].Width = 0;
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["brandName"].HeaderText = "Brand";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["brandName"].Width = (this.gridGroupingControlSearch.Width - 340);
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["brandShortName"].HeaderText = "Brand Shortname";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["brandShortName"].Width = 200;
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["DiscPer"].HeaderText = "Discount %";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["DiscPer"].Width = 100;
        //    }
        //    else if (sMenuType.ToUpper() == "COLOR")
        //    {
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["ColorID"].HeaderText = "ColorID";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["ColorID"].Width = 0;
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["ColorName"].HeaderText = "Color";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["ColorName"].Width = 200;
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["ColorHexCode"].HeaderText = "Color HexCode";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["ColorHexCode"].Width = (this.gridGroupingControlSearch.Width - 240);
        //    }
        //    else if (sMenuType.ToUpper() == "SIZE")
        //    {
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["SizeID"].HeaderText = "SizeID";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["SizeID"].Width = 0;
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["SizeName"].HeaderText = "Size";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["SizeName"].Width = (this.gridGroupingControlSearch.Width - 340);
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["SizeNameShort"].HeaderText = "Size Shortname";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["SizeNameShort"].Width = 200;
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["SortOrder"].HeaderText = "Sort Order";
        //        this.gridGroupingControlSearch.TableDescriptor.Columns["SortOrder"].Width = 100;
        //    }

        //    for (int i = 0; i < gridGroupingControlSearch.TableDescriptor.Columns.Count; i++)
        //    {
        //        gridGroupingControlSearch.TableDescriptor.Columns[i].AllowFilter = true;
        //        gridGroupingControlSearch.TableDescriptor.Columns[i].FilterRowOptions.FilterMode = Syncfusion.Windows.Forms.Grid.Grouping.FilterMode.DisplayText;
        //    }
        //}

        //private void GridGroupControlSearchRowDelete()
        //{
        //    int count = gridGroupingControlSearch.TableDescriptor.Columns.Count;
        //    Syncfusion.Grouping.SelectedRecordsCollection selRecords;
        //    selRecords = this.gridGroupingControlSearch.Table.SelectedRecords;
        //    Syncfusion.Grouping.Record[] r = new Syncfusion.Grouping.Record[count];
        //    selRecords.CopyTo(r, 0);
        //    this.gridGroupingControlSearch.Table.SelectedRecords.Clear();

        //    this.gridGroupingControlSearch.Engine.Table.BeginEdit();

        //    for (int i = count - 1; i >= 0; i--)
        //    {
        //        this.gridGroupingControlSearch.Table.DeleteRecord(r[i]);
        //    }
        //    this.gridGroupingControlSearch.Engine.Table.EndEdit();
        //    this.gridGroupingControlSearch.Engine.Table.TableDirty = true;
        //    this.gridGroupingControlSearch.Update();
        //}

        //void SampleCustomization()
        //{
        //    this.gridGroupingControlSearch.FilterBarSelectedItemChanging += new Syncfusion.Windows.Forms.Grid.Grouping.FilterBarSelectedItemChangingEventHandler(gridGroupingControlSearch_FilterBarSelectedItemChanging);
        //    //this.gridGroupingControlSearch.FilterBarSelectedItemChanged += new Syncfusion.Windows.Forms.Grid.Grouping.FilterBarSelectedItemChangedEventHandler(gridGroupingControlSearch_FilterBarSelectedItemChanged);
        //}

        #endregion

        
    }
}
