using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using GimjaBL;
using DevExpress.XtraLayout;

namespace Gimja
{
    public partial class frmItemSettings : DevExpress.XtraEditors.XtraForm
    {
        BrandBL brandBL;
        BrandData brandData;

        CategoryBL categoryBL;
        CategoryData categoryData;

        TaxTypeBL taxTypeBL;
        TaxTypeData taxTypeData;

        UnitBL unitBL;
        UnitData unitData;

        public frmItemSettings()
        {
            InitializeComponent();

            brandBL = new BrandBL();
            categoryBL = new CategoryBL();
            taxTypeBL = new TaxTypeBL();
            unitBL = new UnitBL();

            brandBL.IsUpdate = false;
            categoryBL.IsUpdate = false;
            taxTypeBL.IsUpdate = false;
            unitBL.IsUpdate = false;

            PopulateBrands();
            PopulateCategories();
            PopulateTaxTypes();
            PopulateUnits();

            grdViewBrand_RowCellClick(null, null);
        }

        #region Methods

        public void ClearControlValues(LayoutControl controlCollection)
        {
            foreach (var ctrl in controlCollection.Controls)
            {
                if (ctrl is TextEdit)
                {
                    TextEdit txt = ctrl as TextEdit;
                    if (txt != null)
                        txt.Text = string.Empty;
                }
                else if (ctrl is MemoEdit)
                {
                    MemoEdit txtMemo = ctrl as MemoEdit;
                    if (txtMemo != null)
                        txtMemo.Text = string.Empty;
                }
                else if (ctrl is CheckEdit)
                {
                    CheckEdit chk = ctrl as CheckEdit;
                    if (chk != null)
                        chk.Checked = false;
                }
                else if (ctrl is LookUpEdit)
                {
                    LookUpEdit lk = ctrl as LookUpEdit;
                    if (lk != null)
                        lk.SelectedText = string.Empty;
                }
                else if (ctrl is ComboBoxEdit)
                {

                }
                else if (ctrl is DateEdit)
                {
                    DateEdit dt = ctrl as DateEdit;
                    if (dt != null)
                        dt.EditValue = DateTime.Today;
                }

                dxErrorProvider.ClearErrors();
            }
        }

        private void PopulateBrands()
        {
            grdBrand.DataSource = brandBL.GetData();
            brandBL.HasData();
        }

        private void PopulateCategories()
        {
            grdCategory.DataSource = categoryBL.GetData();
            categoryBL.HasData();
        }

        private void PopulateTaxTypes()
        {
            grdTaxType.DataSource = taxTypeBL.GetData();
            taxTypeBL.HasData();
        }

        private void PopulateUnits()
        {
            grdUnit.DataSource = unitBL.GetData();
            unitBL.HasData();
        }

        private void SetControlsReadOnly(LayoutControl layoutControl, bool val)
        {
            foreach (var ctrl in layoutControl.Controls)
            {
                if (ctrl is TextEdit)
                {
                    TextEdit txt = ctrl as TextEdit;
                    if (txt != null)
                        txt.Properties.ReadOnly = val;
                }
                else if (ctrl is MemoEdit)
                {
                    MemoEdit txtMemo = ctrl as MemoEdit;
                    if (txtMemo != null)
                        txtMemo.Properties.ReadOnly = val;
                }
                else if (ctrl is CheckEdit)
                {
                    CheckEdit chk = ctrl as CheckEdit;
                    if (chk != null)
                        chk.Properties.ReadOnly = val;
                }
                else if (ctrl is LookUpEdit)
                {
                    LookUpEdit lke = ctrl as LookUpEdit;
                    if (lke != null)
                        lke.Properties.ReadOnly = val;
                }

                else if (ctrl is DateEdit)
                {
                    DateEdit dt = ctrl as DateEdit;
                    if (dt != null)
                        dt.Properties.ReadOnly = val;
                }
            }

            grdBrand.Enabled = grdCategory.Enabled = grdTaxType.Enabled = grdUnit.Enabled = val;
        }

        private void EnableDisableButtons(LayoutControl controlCollection, string action)
        {
            foreach (Control ctrl in controlCollection.Controls)
            {
                if (ctrl is SimpleButton)
                {
                    SimpleButton btn = ctrl as SimpleButton;
                    if (btn != null)
                    {
                        switch (action)
                        {
                            case "Add":
                                if (btn.Text.Equals("&Save") || btn.Text.Equals("&Cancel"))
                                    btn.Enabled = true;

                                if (btn.Text.Equals("&Delete") || btn.Text.Equals("&Edit") || btn.Text.Contains("&Add "))
                                    btn.Enabled = false;

                                break;
                            case "Edit":
                                if (btn.Text.Contains("&Save") || btn.Text.Contains("&Cancel") || btn.Text.Contains("&Delete"))
                                    btn.Enabled = true;
                                if (btn.Text.Contains("&Edit") || btn.Text.Contains("&Add "))
                                    btn.Enabled = false;

                                break;

                            case "Load":

                                if (btn.Text.Contains("&Save") || btn.Text.Contains("&Delete") || btn.Text.Contains("&Cancel"))
                                    btn.Enabled = false;

                                if (btn.Text.Contains("&Edit") || btn.Text.Contains("&Add "))
                                    btn.Enabled = true;

                                break;
                            case "Disable All":

                                if (btn.Text.Contains("&Save") || btn.Text.Contains("&Edit") || btn.Text.Contains("&Delete") || btn.Text.Contains("&Cancel"))
                                    btn.Enabled = false;
                                if (btn.Text.Contains("&Add "))
                                    btn.Enabled = true;
                                break;
                        }
                    }
                }
            }

            //switch (action)
            //{
            //    case "Add":
            //        btnBrandSave.Enabled = btnBrandCancel.Enabled = true;
            //        btnBrandDelete.Enabled = btnBrandEdit.Enabled = false;
            //        break;
            //    case "Edit":
            //        btnBrandSave.Enabled = btnBrandDelete.Enabled = btnBrandCancel.Enabled = true;
            //        btnBrandEdit.Enabled = false;
            //        break;
            //    case "Load":
            //        btnBrandSave.Enabled = btnBrandDelete.Enabled = btnBrandCancel.Enabled = false;
            //        btnBrandEdit.Enabled = true;
            //        break;
            //    case "Disable All":
            //        btnBrandSave.Enabled = btnBrandEdit.Enabled = btnBrandDelete.Enabled = btnBrandCancel.Enabled = false;
            //        break;
            //}
        }

        private void LoadBrands()
        {
            txtBrandName.Text = brandData.Name;
            txtBrandDescription.Text = brandData.Description;
            chkBrandIsActive.CheckState = (brandData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;
        }

        private void LoadCategories()
        {
            txtCategoryName.Text = categoryData.Name;
            txtCategoryDescription.Text = categoryData.Description;
            chkCategoryIsActive.CheckState = (categoryData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;
        }

        private void LoadTaxTypes()
        {
            txtTaxTypeName.Text = taxTypeData.TaxTypeName;
            txtTaxTypeRate.Text = taxTypeData.Rate.ToString();
            chkTaxTypeIsActive.CheckState = (taxTypeData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;
        }

        private void LoadUnits()
        {
            txtUnitName.Text = unitData.UnitName;
            txtUnitDescription.Text = unitData.Description;
            chkUnitIsActive.CheckState = (unitData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;
        }

        private void RefreshCategoryData()
        {
            SetControlsReadOnly(layoutControl2, true);
            EnableDisableTabs(true);
            EnableDisableButtons(layoutControl2, "Load");
            PopulateCategories();
            grdViewCategory_RowCellClick(null, null);
        }

        private void RefreshBrandData()
        {
            SetControlsReadOnly(layoutControl2, true);
            EnableDisableTabs(true);
            EnableDisableButtons(layoutControl2, "Load");
            PopulateBrands();
            grdViewBrand_RowCellClick(null, null);
        }

        private void RefreshUnitData()
        {
            SetControlsReadOnly(layoutControl2, true);
            EnableDisableTabs(true);
            EnableDisableButtons(layoutControl2, "Load");
            PopulateUnits();
            grdViewUnit_RowCellClick(null, null);
        }

        private void RefreshTaxTypeData()
        {
            SetControlsReadOnly(layoutControl2, true);
            EnableDisableTabs(true);
            EnableDisableButtons(layoutControl2, "Load");
            PopulateTaxTypes();
            grdViewTaxType_RowCellClick(null, null);
        }


        /// <summary>
        /// This method enables/disables tabs when adding new records
        /// </summary>
        /// <param name="enableAll">if true, enables all tabs</param>
        private void EnableDisableTabs(bool enableAll = false)
        {
            if (enableAll)
                tabPageBrand.PageEnabled = tabPageCategory.PageEnabled = tabPageTaxType.PageEnabled = tabPageUnit.PageEnabled = true;
            else
            {
                if (tabItemSettings.SelectedTabPage == tabPageBrand)
                    tabPageCategory.PageEnabled = tabPageTaxType.PageEnabled = tabPageUnit.PageEnabled = false;

                else if (tabItemSettings.SelectedTabPage == tabPageCategory)
                    tabPageBrand.PageEnabled = tabPageTaxType.PageEnabled = tabPageUnit.PageEnabled = false;

                else if (tabItemSettings.SelectedTabPage == tabPageTaxType)
                    tabPageCategory.PageEnabled = tabPageBrand.PageEnabled = tabPageUnit.PageEnabled = false;

                else if (tabItemSettings.SelectedTabPage == tabPageUnit)
                    tabPageCategory.PageEnabled = tabPageTaxType.PageEnabled = tabPageBrand.PageEnabled = false;
            }
        }

        #endregion

        #region Event Handlers

        private void tabItemSettings_Click(object sender, EventArgs e)
        {
            if (tabItemSettings.SelectedTabPage == tabPageCategory && btnCategoryAdd.Enabled)
                grdViewCategory_RowCellClick(null, null);
            else if (tabItemSettings.SelectedTabPage == tabPageBrand && btnBrandAdd.Enabled)
                grdViewBrand_RowCellClick(null, null);
            else if (tabItemSettings.SelectedTabPage == tabPageTaxType && btnTaxTypeAdd.Enabled)
                grdViewTaxType_RowCellClick(null, null);
            else if (tabItemSettings.SelectedTabPage == tabPageUnit && btnUnitAdd.Enabled)
                grdViewUnit_RowCellClick(null, null);
        }

        #region Brand Tab

        private void btnBrandAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl2);

                EnableDisableButtons(layoutControl2, "Add");
                SetControlsReadOnly(layoutControl2, false);

                brandBL.IsUpdate = false;

                EnableDisableTabs();

                brandData.BrandID = 0;
                txtBrandName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void btnBrandSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtBrandName.Text.Trim()))
                {
                    var _brandID = brandData.BrandID != 0 ? brandData.BrandID : 0;

                    brandData = new BrandData()
                    {
                        BrandID = _brandID,
                        Name = txtBrandName.Text.Trim(),
                        Description = txtBrandDescription.Text.Trim(),
                        IsActive = (chkBrandIsActive.CheckState == CheckState.Checked ? true : false)
                    };

                    if (brandBL.IsUpdate)
                    {
                        if (brandBL.Update(brandData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshBrandData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (brandBL.IsValid(brandData.Name))
                        {
                            if (brandBL.Add(brandData))
                            {
                                GimjaHelper.ShowInformation("Data successfully added");
                                RefreshBrandData();
                            }
                            else
                                GimjaHelper.ShowWarning("Data can not successfully added. Please try again");
                        }
                        else
                        {
                            GimjaHelper.ShowWarning(String.Format("The name <{0}> is already taken. Please use a different name", brandData.Name));
                            txtBrandName.Focus();
                        }
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    txtBrandName.Focus();
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message);
            }
        }

        private void btnBrandEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl2, false);

            EnableDisableButtons(layoutControl2, "Edit");
            EnableDisableTabs();
            brandBL.IsUpdate = true;

            txtBrandName.Focus();
        }

        private void btnBrandDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult _ans = GimjaHelper.ShowQuestion("Are you sure you want to delete the data?");
                if (_ans == DialogResult.Yes)
                {
                    if (brandBL.Delete(brandData.BrandID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateBrands();

                        if (brandBL.IsDataAvailable)
                        {
                            grdViewBrand_RowCellClick(null, null);
                            EnableDisableButtons(layoutControl2, "Load");
                        }
                        else
                        {
                            ClearControlValues(layoutControl1);
                            EnableDisableButtons(layoutControl2, "Disable All");
                        }

                        EnableDisableTabs(true);
                        SetControlsReadOnly(layoutControl1, true);
                    }
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be deleted. \n" + ex.Message);
            }
        }

        private void btnBrandCancel_Click(object sender, EventArgs e)
        {
            if (brandBL.IsDataAvailable)
            {
                grdViewBrand_RowCellClick(null, null);
                EnableDisableButtons(layoutControl2, "Load");
            }
            else
            {
                ClearControlValues(layoutControl2);
                EnableDisableButtons(layoutControl2, "Disable All");
            }

            EnableDisableTabs(true);

            dxErrorProvider.ClearErrors();
            SetControlsReadOnly(layoutControl2, true);
        }

        private void grdViewBrand_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            brandData = grdViewBrand.GetFocusedRow() as BrandData;

            if(brandData != null)
            {
            //if (brandBL.IsDataAvailable)
            //{
            //    brandBL.BrandID = Convert.ToInt32(grdViewBrand.GetRowCellValue(grdViewBrand.FocusedRowHandle, "BrandID"));
            //    brandBL.Name = grdViewBrand.GetRowCellValue(grdViewBrand.FocusedRowHandle, "Name").ToString();
            //    brandBL.Description = grdViewBrand.GetRowCellValue(grdViewBrand.FocusedRowHandle, "Description").ToString();
            //    brandBL.IsActive = Convert.ToBoolean(grdViewBrand.GetRowCellValue(grdViewBrand.FocusedRowHandle, "IsActive"));

                LoadBrands();

                EnableDisableButtons(layoutControl2, "Load");
            }
            else
                EnableDisableButtons(layoutControl2, "Disable All");

            SetControlsReadOnly(layoutControl2, true);
        }

        private void txtBrandName_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtBrandName.Text))
            {
                dxErrorProvider.SetError(txtBrandName, "required field");
                txtBrandName.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtBrandName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        #endregion

        #region Category Tab

        private void btnCategoryAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl3);

                EnableDisableButtons(layoutControl3, "Add");
                SetControlsReadOnly(layoutControl3, false);

                categoryBL.IsUpdate = false;
                
                EnableDisableTabs();

                categoryData.CategoryID = 0;

                txtCategoryName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void btnCategorySave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtCategoryName.Text))
                {
                    var _categoryID = categoryData.CategoryID != 0 ? categoryData.CategoryID : 0;

                    categoryData = new CategoryData()
                    {
                        CategoryID = _categoryID,
                        Name = txtCategoryName.Text.Trim(),
                        Description = txtCategoryDescription.Text.Trim(),
                        IsActive = (chkCategoryIsActive.CheckState == CheckState.Checked ? true : false)
                    };
                    
                    if (categoryBL.IsUpdate)
                    {
                        if (categoryBL.Update(categoryData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");

                            RefreshCategoryData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (categoryBL.IsValid(categoryData.Name))
                        {
                            if (categoryBL.Add(categoryData))
                            {
                                GimjaHelper.ShowInformation("Data successfully added");
                                RefreshCategoryData();
                            }
                            else
                                GimjaHelper.ShowWarning("Data can not successfully added. Please try again");
                        }
                        else
                        {
                            GimjaHelper.ShowWarning(String.Format("The name <{0}> is already taken. Please use a different name", categoryData.Name));
                            txtCategoryName.Focus();
                        }
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    txtCategoryName.Focus();
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message);
            }
        }

        private void btnCategoryEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl3, false);

            EnableDisableButtons(layoutControl3, "Edit");
            EnableDisableTabs();
            categoryBL.IsUpdate = true;

            txtCategoryName.Focus();
        }

        private void btnCategoryDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult _ans = GimjaHelper.ShowQuestion("Are you sure you want to delete the data?");
                if (_ans == DialogResult.Yes)
                {
                    if (categoryBL.Delete(categoryData.CategoryID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateCategories();

                        if (categoryBL.IsDataAvailable)
                        {
                            grdViewCategory_RowCellClick(null, null);
                            EnableDisableButtons(layoutControl2, "Load");
                        }
                        else
                        {
                            ClearControlValues(layoutControl1);
                            EnableDisableButtons(layoutControl2, "Disable All");
                        }

                        EnableDisableTabs(true);
                        SetControlsReadOnly(layoutControl1, true);
                    }
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be deleted. \n" + ex.Message);
            }
        }

        private void btnCategoryCancel_Click(object sender, EventArgs e)
        {
            if (categoryBL.IsDataAvailable)
            {
                grdViewCategory_RowCellClick(null, null);
                EnableDisableButtons(layoutControl3, "Load");
            }
            else
            {
                ClearControlValues(layoutControl3);
                EnableDisableButtons(layoutControl3, "Disable All");
            }

            EnableDisableTabs(true);
            dxErrorProvider.ClearErrors();
            SetControlsReadOnly(layoutControl3, true);
        }

        private void grdViewCategory_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            categoryData = grdViewCategory.GetFocusedRow() as CategoryData;

            if(categoryData !=null)
            {
            //if (categoryBL.IsDataAvailable)
            //{
            //    categoryBL.CategoryID = Convert.ToInt32(grdViewCategory.GetRowCellValue(grdViewCategory.FocusedRowHandle, "CategoryID"));
            //    categoryBL.Name = grdViewCategory.GetRowCellValue(grdViewCategory.FocusedRowHandle, "Name").ToString();
            //    categoryBL.Description = grdViewCategory.GetRowCellValue(grdViewCategory.FocusedRowHandle, "Description").ToString();
            //    categoryBL.IsActive = Convert.ToBoolean(grdViewCategory.GetRowCellValue(grdViewCategory.FocusedRowHandle, "IsActive"));

                LoadCategories();

                EnableDisableButtons(layoutControl3, "Load");
            }
            else
                EnableDisableButtons(layoutControl3, "Disable All");

            SetControlsReadOnly(layoutControl3, true);
        }

        private void txtCategoryName_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                dxErrorProvider.SetError(txtCategoryName, "required field");
                txtCategoryName.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtCategoryName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        #endregion

        #region Tax Type Tab

        private void btnTaxTypeAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl4);

                EnableDisableButtons(layoutControl4, "Add");
                SetControlsReadOnly(layoutControl4, false);

                taxTypeBL.IsUpdate = false;
                
                EnableDisableTabs();

                taxTypeData.TaxTypeID = 0;

                txtTaxTypeName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void btnTaxTypeSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtTaxTypeName.Text) && !String.IsNullOrWhiteSpace(txtTaxTypeRate.Text))
                {
                    var _taxTypeID = taxTypeData.TaxTypeID != 0 ? taxTypeData.TaxTypeID : 0;

                    taxTypeData = new TaxTypeData()
                    {
                        TaxTypeID = _taxTypeID,
                        TaxTypeName = txtTaxTypeName.Text.Trim(),
                        Rate = Convert.ToDouble(txtTaxTypeRate.Text.Trim()),
                        IsActive = (chkTaxTypeIsActive.CheckState == CheckState.Checked ? true : false)
                    };

                    if (taxTypeBL.IsUpdate)
                    {
                        if (taxTypeBL.Update(taxTypeData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshTaxTypeData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (taxTypeBL.IsValid(taxTypeData.TaxTypeName))
                        {
                            if (taxTypeBL.Add(taxTypeData))
                            {
                                GimjaHelper.ShowInformation("Data successfully added");
                                RefreshTaxTypeData();
                            }
                            else
                                GimjaHelper.ShowWarning("Data can not successfully added. Please try again");
                        }
                        else
                        {
                            GimjaHelper.ShowWarning(String.Format("The name <{0}> is already taken. Please use a different name", taxTypeData.TaxTypeName));
                            txtTaxTypeName.Focus();
                        }
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    txtTaxTypeName.Focus();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Data could not be saved. \n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTaxTypeEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl4, false);

            EnableDisableButtons(layoutControl4, "Edit");
            EnableDisableTabs();
            taxTypeBL.IsUpdate = true;

            txtTaxTypeName.Focus();
        }

        private void btnTaxTypeDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult _ans = GimjaHelper.ShowQuestion("Are you sure you want to delete the data?");
                if (_ans == DialogResult.Yes)
                {
                    if (taxTypeBL.Delete(taxTypeData.TaxTypeID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateTaxTypes();

                        if (taxTypeBL.IsDataAvailable)
                        {
                            grdViewTaxType_RowCellClick(null, null);
                            EnableDisableButtons(layoutControl2, "Load");
                        }
                        else
                        {
                            ClearControlValues(layoutControl1);
                            EnableDisableButtons(layoutControl2, "Disable All");
                        }

                        EnableDisableTabs(true);
                        SetControlsReadOnly(layoutControl1, true);
                    }
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be deleted. \n" + ex.Message);
            }
        }

        private void btnTaxTypeCancel_Click(object sender, EventArgs e)
        {
            if (taxTypeBL.IsDataAvailable)
            {
                grdViewTaxType_RowCellClick(null, null);
                EnableDisableButtons(layoutControl4, "Load");
            }
            else
            {
                ClearControlValues(layoutControl4);
                EnableDisableButtons(layoutControl4, "Disable All");
            }

            EnableDisableTabs(true);
            dxErrorProvider.ClearErrors();
            SetControlsReadOnly(layoutControl4, true);
        }

        private void grdViewTaxType_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            taxTypeData = grdViewTaxType.GetFocusedRow() as TaxTypeData;

            if(taxTypeData != null)
            {
            //if (taxTypeBL.IsDataAvailable)
            //{
            //    taxTypeBL.TaxTypeID = Convert.ToInt32(grdViewTaxType.GetRowCellValue(grdViewTaxType.FocusedRowHandle, "TaxTypeID"));
            //    taxTypeBL.TaxTypeName = grdViewTaxType.GetRowCellValue(grdViewTaxType.FocusedRowHandle, "TaxTypeName").ToString();
            //    taxTypeBL.Rate = Convert.ToDouble(grdViewTaxType.GetRowCellValue(grdViewTaxType.FocusedRowHandle, "Rate"));
            //    taxTypeBL.IsActive = Convert.ToBoolean(grdViewTaxType.GetRowCellValue(grdViewTaxType.FocusedRowHandle, "IsActive"));

                LoadTaxTypes();

                EnableDisableButtons(layoutControl4, "Load");
            }
            else
                EnableDisableButtons(layoutControl4, "Disable All");

            SetControlsReadOnly(layoutControl4, true);
        }

        private void txtTaxTypeName_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtTaxTypeName.Text))
            {
                dxErrorProvider.SetError(txtTaxTypeName, "required field");
                txtTaxTypeName.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtTaxTypeName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void txtTaxTypeRate_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtTaxTypeRate.Text))
            {
                dxErrorProvider.SetError(txtTaxTypeRate, "required field");
                txtTaxTypeRate.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtTaxTypeRate, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        #endregion

        #region Unit Tab

        private void btnUnitAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl5);

                EnableDisableButtons(layoutControl5, "Add");
                SetControlsReadOnly(layoutControl5, false);

                unitBL.IsUpdate = false;

                EnableDisableTabs();

                unitData.UnitID = 0;

                txtUnitName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void btnUnitSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtUnitName.Text) && !String.IsNullOrWhiteSpace(txtUnitDescription.Text))
                {
                    var _unitID = unitData.UnitID != 0 ? unitData.UnitID : 0;

                    unitData = new UnitData()
                    {
                        UnitID = _unitID,
                        UnitName = txtUnitName.Text.Trim(),
                        Description = txtUnitDescription.Text.Trim(),
                        IsActive = (chkUnitIsActive.CheckState == CheckState.Checked ? true : false)
                    };

                    if (unitBL.IsUpdate)
                    {
                        if (unitBL.Update(unitData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshUnitData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (unitBL.IsValid(unitData.UnitName))
                        {
                            if (unitBL.Add(unitData))
                            {
                                GimjaHelper.ShowInformation("Data successfully added");
                                RefreshUnitData();
                            }
                            else
                                GimjaHelper.ShowWarning("Data can not successfully added. Please try again");
                        }
                        else
                        {
                            GimjaHelper.ShowWarning(String.Format("The name <{0}> is already taken. Please use a different name", unitData.UnitName));
                            txtUnitName.Focus();
                        }
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    txtUnitName.Focus();
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message);
            }
        }

        private void btnUnitEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl5, false);

            EnableDisableButtons(layoutControl5, "Edit");
            EnableDisableTabs();
            unitBL.IsUpdate = true;

            txtUnitName.Focus();
        }

        private void btnUnitDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult _ans = GimjaHelper.ShowQuestion("Are you sure you want to delete the data?");
                if (_ans == DialogResult.Yes)
                {
                    if (unitBL.Delete(unitData.UnitID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateUnits();

                        if (unitBL.IsDataAvailable)
                        {
                            grdViewUnit_RowCellClick(null, null);
                            EnableDisableButtons(layoutControl2, "Load");
                        }
                        else
                        {
                            ClearControlValues(layoutControl1);
                            EnableDisableButtons(layoutControl2, "Disable All");
                        }

                        SetControlsReadOnly(layoutControl1, true);
                    }
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be deleted. \n" + ex.Message);
            }
        }

        private void btnUnitCancel_Click(object sender, EventArgs e)
        {
            if (unitBL.IsDataAvailable)
            {
                grdViewUnit_RowCellClick(null, null);
                EnableDisableButtons(layoutControl5, "Load");
            }
            else
            {
                ClearControlValues(layoutControl5);
                EnableDisableButtons(layoutControl5, "Disable All");
            }

            EnableDisableTabs(true);
            dxErrorProvider.ClearErrors();
            SetControlsReadOnly(layoutControl5, true);
        }

        private void grdViewUnit_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            unitData = grdViewUnit.GetFocusedRow() as UnitData;

            if(unitData !=null)
            {
            //if (unitBL.IsDataAvailable)
            //{
            //    unitBL.UnitID = Convert.ToInt32(grdViewUnit.GetRowCellValue(grdViewUnit.FocusedRowHandle, "UnitID"));
            //    unitBL.UnitName = grdViewUnit.GetRowCellValue(grdViewUnit.FocusedRowHandle, "UnitName").ToString();
            //    unitBL.Description = grdViewUnit.GetRowCellValue(grdViewUnit.FocusedRowHandle, "Description").ToString();
            //    unitBL.IsActive = Convert.ToBoolean(grdViewUnit.GetRowCellValue(grdViewUnit.FocusedRowHandle, "IsActive"));

                LoadUnits();

                EnableDisableButtons(layoutControl5, "Load");
            }
            else
                EnableDisableButtons(layoutControl5, "Disable All");

            SetControlsReadOnly(layoutControl5, true);
        }

        private void txtUnitName_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtUnitName.Text))
            {
                dxErrorProvider.SetError(txtUnitName, "required field");
                txtUnitName.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtUnitName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        #endregion

        #endregion
    }
}