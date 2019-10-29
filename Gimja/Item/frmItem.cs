using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using GimjaBL;
using DevExpress.XtraLayout;

namespace Gimja
{
    public partial class frmItem : DevExpress.XtraEditors.XtraForm
    {
        ItemBL itemBL;
        ItemData itemData;
        BrandBL brandBL;
        CategoryBL categoryBL;
        UnitBL unitBL;
        TaxTypeBL taxTypeBL;

        bool isRequiredFieldBlank = false;

        public frmItem()
        {
            InitializeComponent();

            itemBL = new ItemBL();
            brandBL = new BrandBL();
            categoryBL = new CategoryBL();
            unitBL = new UnitBL();
            taxTypeBL = new TaxTypeBL();

            itemBL.IsUpdate = false;

            PopulateItems();

            PopulateBrand();

            PopulateTaxType();

            PopulateUnit();

            PopulateCountries();

            grdViewItem_RowCellClick(null, null);
        }

        #region Methods

        public void ClearControlValues(LayoutControl controlCollection)
        {
            #region commented
            //foreach (var ctrl in controlCollection.Controls)
            //{
            //    if (ctrl is TextEdit)
            //    {
            //        TextEdit txt = ctrl as TextEdit;
            //        if (txt != null)
            //        {
            //            txt.Text = string.Empty;
            //            //txt.EditValue = null;
            //        }
            //    }
            //    else if (ctrl is MemoEdit)
            //    {
            //        MemoEdit txtMemo = ctrl as MemoEdit;
            //        if (txtMemo != null)
            //            txtMemo.Text = string.Empty;
            //    }
            //    else if (ctrl is CheckEdit)
            //    {
            //        CheckEdit chk = ctrl as CheckEdit;
            //        if (chk != null)
            //            chk.Checked = false;
            //    }
            //    else if (ctrl is LookUpEdit)
            //    {
            //        LookUpEdit lke = ctrl as LookUpEdit;
            //        if (lke != null)
            //            lke.EditValue = null;
            //    }

            #endregion

            if (!itemBL.IsUpdate)
            {
                txtItemID.Text = String.Empty;
                lkeBrand.EditValue = null;
                lkeCategory.EditValue = null;
                lkeOrigin.EditValue = null;
            }

            lkeUnit.EditValue = null;
            lkeTaxType.EditValue = null;
            chkIsTaxExempted.CheckState = CheckState.Unchecked;
            chkIsActive.CheckState = CheckState.Checked;
            txtUnitPrice.Text = txtOrderQty.Text = txtPickFaceQty.Text = txtReorderLevel.Text = txtDescription.Text = String.Empty;

            dxErrorProvider.ClearErrors();
            //}
        }

        private void SetControlsReadOnly(LayoutControl layoutControl, bool val)
        {
            #region commented
            //foreach (var ctrl in layoutControl.Controls)
            //{
            //    if (ctrl is TextEdit)
            //    {
            //        TextEdit txt = ctrl as TextEdit;
            //        bool _dontEnable = txt.Tag != null ? true : false;

            //        if (txt != null && !_dontEnable && !itemBL.IsUpdate)
            //            txt.Properties.ReadOnly = val;
            //    }
            //    else if (ctrl is MemoEdit)
            //    {
            //        MemoEdit txtMemo = ctrl as MemoEdit;
            //        if (txtMemo != null)
            //            txtMemo.Properties.ReadOnly = val;
            //    }
            //    else if (ctrl is CheckEdit)
            //    {
            //        CheckEdit chk = ctrl as CheckEdit;
            //        if (chk != null)
            //            chk.Properties.ReadOnly = val;
            //    }
            //    else if (ctrl is LookUpEdit)
            //    {
            //        LookUpEdit lke = ctrl as LookUpEdit;
            //        bool _dontEnable = lke.Tag != null ? true : false;

            //        if (lke != null && !_dontEnable && !itemBL.IsUpdate)
            //            lke.Properties.ReadOnly = val;
            //    }

            //    else if (ctrl is DateEdit)
            //    {
            //        DateEdit dt = ctrl as DateEdit;
            //        if (dt != null)
            //            dt.Properties.ReadOnly = val;
            //    }
            //}
            #endregion

            if (!itemBL.IsUpdate && !val)
                lkeBrand.Enabled = lkeCategory.Enabled = lkeOrigin.Enabled = true;
            else
                lkeBrand.Enabled = lkeCategory.Enabled = lkeOrigin.Enabled = false;

            lkeUnit.Properties.ReadOnly =
            lkeTaxType.Properties.ReadOnly =
            chkIsTaxExempted.Properties.ReadOnly =
            chkIsActive.Properties.ReadOnly =
            txtUnitPrice.Properties.ReadOnly =
            txtOrderQty.Properties.ReadOnly =
            txtPickFaceQty.Properties.ReadOnly =
            txtReorderLevel.Properties.ReadOnly =
            txtDescription.Properties.ReadOnly = val;

            grdItem.Enabled = val;
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
        }

        private void SetItemID()
        {
            if (!itemBL.IsUpdate)
            {
                if (Convert.ToInt32(lkeBrand.EditValue) != 0 && Convert.ToInt32(lkeCategory.EditValue) != 0 && !string.IsNullOrWhiteSpace(lkeOrigin.EditValue.ToString()))
                {
                    int _brandID = Convert.ToInt32(lkeBrand.EditValue);
                    int _categoryID = Convert.ToInt32(lkeCategory.EditValue);
                    string _origin = Convert.ToString(lkeOrigin.EditValue);

                    string _newItemID = itemBL.GenerateItemID(_brandID, _categoryID, _origin);
                    
                    if (!string.IsNullOrEmpty(_newItemID))
                        txtItemID.Text = _newItemID;

                    else
                        GimjaHelper.ShowError("Gimja could not be able to generate new Item ID. Please try again");
                }
                //else
                //    GimjaHelper.ShowWarning("Brand, Category and/or Origin can not be blank");
            }
        }

        private void PopulateUnit()
        {
            lkeUnit.Properties.DataSource = unitBL.GetData(true);
            lkeUnit.Properties.DisplayMember = "UnitName";
            lkeUnit.Properties.ValueMember = "UnitID";
        }

        private void PopulateTaxType()
        {
            lkeTaxType.Properties.DataSource = taxTypeBL.GetData(true);
            lkeTaxType.Properties.DisplayMember = "TaxTypeName";
            lkeTaxType.Properties.ValueMember = "TaxTypeID";
        }

        private void PopulateBrand()
        {
            lkeBrand.Properties.DataSource = brandBL.GetData(true);
            lkeBrand.Properties.DisplayMember = "Name";
            lkeBrand.Properties.ValueMember = "BrandID";
        }

        private void PopulateCategory(string status)
        {
            lkeCategory.Properties.DataSource = status == "Filtered" ? categoryBL.GetFilteredCategories(itemData.BrandID) : categoryBL.GetData(true);
            lkeCategory.Properties.DisplayMember = "Name";
            lkeCategory.Properties.ValueMember = "CategoryID";
        }

        private void PopulateItems()
        {
            grdItem.DataSource = itemBL.GetData();
            itemBL.HasData();
        }

        private void LoadData()
        {
            txtOrderQty.Text = itemData.OrderQuantity.ToString();
            txtDescription.Text = itemData.Description.ToString();
            txtPickFaceQty.Text = itemData.PickFaceQty.ToString();
            txtUnitPrice.Text = itemData.UnitPrice.ToString();
            lkeBrand.EditValue = itemData.BrandID;
            lkeCategory.EditValue = itemData.CategoryID;
            lkeOrigin.EditValue = itemData.Origin;
            txtItemID.Text = itemData.ItemID;
            lkeTaxType.EditValue = itemData.TaxTypeID;
            lkeUnit.EditValue = itemData.UnitID;
            txtReorderLevel.Text = itemData.ReorderLevel.ToString();
            chkIsTaxExempted.CheckState = (itemData.IsTaxExempted == true) ? CheckState.Checked : CheckState.Unchecked;
            chkIsActive.CheckState = (itemData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;
        }

        private void PopulateCountries()
        {
            try
            {
                lkeOrigin.Properties.DataSource = GimjaBL.CommonBL.GetCountriesList();
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message);
            }
        }

        private void RefreshItemData()
        {
            SetControlsReadOnly(layoutControl1, true);
            EnableDisableButtons(layoutControl1, "Load");
            PopulateItems();
            grdViewItem_RowCellClick(null, null);
        }

        private void SaveBeforeClosing(FormClosingEventArgs e)
        {
                            DialogResult _result = GimjaHelper.ShowQuestionWithYesNoCancelButtons("Would you like to save your changes?");

                if (_result == DialogResult.Yes)
                {
                    btnSave_Click(null, null);

                    if (isRequiredFieldBlank)
                        e.Cancel = true;
                }
                else if (_result == DialogResult.Cancel)
                {
                    // Stop the closing and return to the form
                    e.Cancel = true;
                }
        }

        #endregion

        #region Event Handlers

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                itemBL.IsUpdate = false;
                
                ClearControlValues(layoutControl1);

                EnableDisableButtons(layoutControl1, "Add");

                SetControlsReadOnly(layoutControl1, false);

                itemData = null;

                //sets item ID to empty
                //itemData.ItemID = String.Empty;

                lkeBrand.Focus();
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message);
            }
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtItemID.Text) &&
                    !String.IsNullOrWhiteSpace(txtOrderQty.Text) &&
                    !String.IsNullOrWhiteSpace(txtDescription.Text) &&
                    !String.IsNullOrWhiteSpace(txtPickFaceQty.Text) &&
                    !String.IsNullOrWhiteSpace(txtUnitPrice.Text) &&
                    !lkeBrand.EditValue.Equals(0) &&
                    !lkeCategory.EditValue.Equals(0) &&
                    !lkeTaxType.EditValue.Equals(0) &&
                    !lkeUnit.EditValue.Equals(0) &&
                    !String.IsNullOrWhiteSpace(txtReorderLevel.Text) &&
                    !lkeOrigin.EditValue.Equals(0))
                {
                    itemData = new ItemData()
                    {
                        ItemID = txtItemID.Text.Trim(),
                        OrderQuantity = Convert.ToDouble(txtOrderQty.Text.Trim()),
                        Description = txtDescription.Text.Trim(),
                        PickFaceQty = Convert.ToDouble(txtPickFaceQty.Text.Trim()),
                        UnitPrice = Convert.ToDouble(txtUnitPrice.Text.Trim()),
                        BrandID = Convert.ToInt32(lkeBrand.EditValue),
                        CategoryID = Convert.ToInt32(lkeCategory.EditValue),
                        TaxTypeID = Convert.ToInt32(lkeTaxType.EditValue),
                        UnitID = Convert.ToInt32(lkeUnit.EditValue),
                        ReorderLevel = Convert.ToDouble(txtReorderLevel.Text.Trim()),
                        IsTaxExempted = (chkIsTaxExempted.CheckState == CheckState.Checked ? true : false),
                        IsActive = (chkIsActive.CheckState == CheckState.Checked ? true : false),
                        Origin = lkeOrigin.EditValue.ToString()
                    };


                    if (itemBL.IsUpdate)
                    {
                        if (itemBL.Update(itemData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshItemData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (itemBL.IsValid(itemData.ItemID))
                        {
                            if (itemBL.Add(itemData))
                            {
                                GimjaHelper.ShowInformation("Data successfully added");
                                RefreshItemData();
                            }
                            else
                                GimjaHelper.ShowWarning("Data can not successfully added. Please try again");
                        }
                        else
                            GimjaHelper.ShowWarning(String.Format("The item id  <{0}> is already already taken. Please use a different item ID", itemData.ItemID));
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    lkeBrand.Focus();
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult _ans = GimjaHelper.ShowQuestion("Are you sure you want to delete the data?");
                if (_ans == DialogResult.Yes)
                {
                    if (itemBL.Delete(itemData.ItemID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateItems();

                        if (itemBL.IsDataAvailable)
                        {
                            grdViewItem_RowCellClick(null, null);
                            EnableDisableButtons(layoutControl1, "Load");
                        }
                        else
                        {
                            ClearControlValues(layoutControl1);
                            EnableDisableButtons(layoutControl1, "Disable All");
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

        private void grdViewItem_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            itemData = grdViewItem.GetFocusedRow() as ItemData;
            if (itemData != null)
            {
                //if (itemBL.IsDataAvailable)
                //{
//                    itemBL.IsLoadData = true;

                    //itemBL.ItemID = grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "ItemID").ToString();
                    //itemBL.OrderQuantity = Convert.ToDouble(grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "OrderQuantity").ToString());
                    //itemBL.PickFaceQty = Convert.ToDouble(grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "PickFaceQty").ToString());
                    //itemBL.Description = grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "Description").ToString();
                    //itemBL.UnitPrice = Convert.ToDouble(grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "UnitPrice").ToString());
                    //itemBL.BrandID = Convert.ToInt32(grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "BrandID").ToString());

                    //itemBL.CategoryID = Convert.ToInt32(grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "CategoryID").ToString());
                    //itemBL.TaxTypeID = Convert.ToInt32(grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "TaxTypeID").ToString());
                    //itemBL.UnitID = Convert.ToInt32(grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "UnitID").ToString());
                    //itemBL.ReorderLevel = Convert.ToDouble(grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "ReorderLevel").ToString());
                    //itemBL.IsTaxExempted = Convert.ToBoolean(grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "IsTaxExempted").ToString());
                    //itemBL.IsActive = Convert.ToBoolean(grdViewItem.GetRowCellValue(grdViewItem.FocusedRowHandle, "IsActive"));

                    PopulateCategory("All");
                    LoadData();

                    EnableDisableButtons(layoutControl1, "Load");
                }
                else
                {
                    itemData = new ItemData();
                    EnableDisableButtons(layoutControl1, "Disable All");
                }
            //}

            SetControlsReadOnly(layoutControl1, true);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            itemBL.IsUpdate = true;
            
            SetControlsReadOnly(layoutControl1, false);

            EnableDisableButtons(layoutControl1, "Edit");

            lkeUnit.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (itemBL.IsDataAvailable)
            {
                PopulateItems();
                grdViewItem_RowCellClick(null, null);
                EnableDisableButtons(layoutControl1, "Load");
            }
            else
            {
                ClearControlValues(layoutControl1);
                EnableDisableButtons(layoutControl1, "Disable All");
            }

            dxErrorProvider.ClearErrors();
            SetControlsReadOnly(layoutControl1, true);
        }

        private void lkeCategory_Leave(object sender, EventArgs e)
        {
            //if (lkeCategory.EditValue.Equals(0))
            //{
            //    dxErrorProvider.SetError(lkeCategory, "required field");
            //    lkeCategory.Focus();
            //}
            //else
            //    dxErrorProvider.SetErrorType(lkeCategory, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void lkeBrand_Leave(object sender, EventArgs e)
        {
            if (lkeBrand.EditValue.Equals(0))
            {
                dxErrorProvider.SetError(lkeBrand, "required field");
                lkeBrand.Focus();
            }
            else
                dxErrorProvider.SetErrorType(lkeBrand, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        private void txtUnitPrice_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtUnitPrice.Text))
            {
                dxErrorProvider.SetError(txtUnitPrice, "required field");
                txtUnitPrice.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtUnitPrice, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void txtReorderLevel_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtReorderLevel.Text))
            {
                dxErrorProvider.SetError(txtReorderLevel, "required field");
                txtReorderLevel.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtReorderLevel, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        private void txtPickFaceQty_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtPickFaceQty.Text))
            {
                dxErrorProvider.SetError(txtPickFaceQty, "required field");
                txtPickFaceQty.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtPickFaceQty, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        private void lkeUnit_Leave(object sender, EventArgs e)
        {
            //if (lkeUnit.EditValue.Equals(0))
            //{
            //    dxErrorProvider.SetError(lkeUnit, "required field");
            //    lkeUnit.Focus();
            //}
            //else
            //    dxErrorProvider.SetErrorType(lkeUnit, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        private void txtOrderQty_Leave(object sender, EventArgs e)
        {
            //if (String.IsNullOrWhiteSpace(txtOrderQty.Text))
            //{
            //    dxErrorProvider.SetError(txtOrderQty, "required field");
            //    txtOrderQty.Focus();
            //}
            //else
            //    dxErrorProvider.SetErrorType(txtOrderQty, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void lkeTaxType_Leave(object sender, EventArgs e)
        {
            //if (lkeTaxType.EditValue.Equals(0))
            //{
            //    dxErrorProvider.SetError(lkeTaxType, "required field");
            //    lkeTaxType.Focus();
            //}
            //else
            //    dxErrorProvider.SetErrorType(lkeTaxType, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        private void lkeCategory_EditValueChanged(object sender, EventArgs e)
        {
            
        }

        private void lkeBrand_EditValueChanged(object sender, EventArgs e)
        {
            //if (!itemBL.IsUpdate)
            //{
            //    if (lkeBrand.EditValue != null)
            //    {
            //        itemData.BrandID = Convert.ToInt32(lkeBrand.EditValue);
            //        if (!itemBL.IsUpdate)
            //        {
            //            PopulateCategory("Filtered");
            //            lkeCategory.EditValue = null;
            //            txtItemID.Text = String.Empty;
            //        }
            //        else
            //        {
            //            PopulateCategory("All");
            //        }
            //    }
            //}
            //else
            //    lkeCategory.EditValue = itemData.CategoryID;
        }

        private void lkeOrigin_EditValueChanged(object sender, EventArgs e)
        {
            if (!itemBL.IsUpdate)
            {
                SetItemID();
            }
        }

        private void frmItem_FormClosing(object sender, FormClosingEventArgs e)
        {
            string _itemID = txtItemID.Text.Trim();
            double _orderQuantity = txtOrderQty.Text.Trim() !=string.Empty ? Convert.ToDouble(txtOrderQty.Text.Trim()) : 0;
            string _description = txtDescription.Text.Trim();
            double _pickFaceQty = txtPickFaceQty.Text.Trim() !=string.Empty ?  Convert.ToDouble(txtPickFaceQty.Text.Trim()) : 0;
            double _unitPrice = txtUnitPrice.Text.Trim() != string.Empty ? Convert.ToDouble(txtUnitPrice.Text.Trim()) : 0;
            int _brandID = lkeBrand.EditValue !=null ? Convert.ToInt32(lkeBrand.EditValue) : 0;
            int _categoryID = lkeCategory.EditValue !=null ? Convert.ToInt32(lkeCategory.EditValue) : 0;
            int _taxTypeID = lkeTaxType.EditValue !=null ? Convert.ToInt32(lkeTaxType.EditValue) : 0;
            int _unitID = lkeUnit.EditValue !=null ? Convert.ToInt32(lkeUnit.EditValue) : 0;
            double _reorderLevel = txtReorderLevel.Text.Trim() !=string.Empty ? Convert.ToDouble(txtReorderLevel.Text.Trim()) : 0;
            bool _isTaxExempted = (chkIsTaxExempted.CheckState == CheckState.Checked ? true : false);
            bool _isActive = (chkIsActive.CheckState == CheckState.Checked ? true : false);
            string _origin = lkeOrigin.EditValue !=null ? lkeOrigin.EditValue.ToString() : string.Empty;

            if (itemData != null)
            {
                if (itemData.ItemID != _itemID ||
                    itemData.OrderQuantity != _orderQuantity ||
                    itemData.Description != _description ||
                    itemData.PickFaceQty != _pickFaceQty ||
                    itemData.UnitPrice != _unitPrice ||
                    itemData.UnitID != _unitID ||
                    itemData.BrandID != _brandID ||
                    itemData.CategoryID != _categoryID ||
                    itemData.TaxTypeID != _taxTypeID ||
                    itemData.ReorderLevel != _reorderLevel ||
                    itemData.IsTaxExempted != _isTaxExempted ||
                    itemData.IsActive != _isActive ||
                    itemData.Origin != _origin)
                {
                    SaveBeforeClosing(e);
                }
            }
            else if (itemBL.IsUpdate == false &&
                (_itemID != string.Empty ||
                _orderQuantity != 0 ||
                _description != string.Empty ||
                _pickFaceQty != 0 ||
                _unitPrice != 0 ||
                _unitID != 0 ||
                _brandID != 0 ||
                _categoryID != 0 ||
                _taxTypeID != 0 ||
                _reorderLevel != 0 ||
                _origin != string.Empty))
            {
                SaveBeforeClosing(e);
            }
        }
        
        #endregion
    }
}