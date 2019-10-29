using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using GimjaBL;
using DevExpress.XtraLayout;

namespace Gimja
{
    public partial class frmSupplier : DevExpress.XtraEditors.XtraForm
    {
        private bool isRequiredFieldBlank = false;

        //used to temporarily store telephones/faxes
        private List<TelephoneFaxData> tempTelephoneFaxList;

        private SupplierBL supplierBL;
        private SupplierData supplierData;

        private AddressBL addressBL;
        private AddressData addressData;

        private TelephoneFaxBL telephoneFaxBL;
        private TelephoneFaxData telephoneFaxData;

        private TelephoneFaxTypeBL telephoneFaxTypeBL;

        public frmSupplier()
        {
            InitializeComponent();

            supplierBL = new SupplierBL();
            addressBL = new AddressBL();
            telephoneFaxBL = new TelephoneFaxBL();
            telephoneFaxTypeBL = new TelephoneFaxTypeBL();

            tempTelephoneFaxList = new List<TelephoneFaxData>();

            supplierBL.IsUpdate = false;

            PopulateTelephoneFaxType();

            PopulateCountries();

            PopulateSuppliers();

            grdViewSupplier_RowCellClick(null, null);
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
                    {
                        txt.Text = string.Empty;
                        txt.EditValue = null;
                    }
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
                    LookUpEdit lke = ctrl as LookUpEdit;
                    if (lke != null)
                        lke.EditValue = null;
                }

                dxErrorProvider.ClearErrors();
            }

            grdTelephoneFax.DataSource = null;
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

            grdSupplier.Enabled = val;
            grdTelephoneFax.Enabled = !val;
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

        private void LoadSupplierData()
        {
            ////displays Manufacturer info
            txtCompanyName.Text = supplierData.CompanyName;
            txtContactPerson.Text = supplierData.ContactPerson;
            txtDescription.Text = supplierData.Description;
            chkIsActive.CheckState = (supplierData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;

            if (supplierData.AddressID != Guid.Empty)
            {
                var _addressData = addressBL.GetAddress((Guid)supplierData.AddressID);

                //displays Address info
                txtHouseNo.Text = _addressData.HouseNo;
                txtKebele.Text = _addressData.Kebele;
                txtWoreda.Text = _addressData.Woreda;
                txtSubcity.Text = _addressData.Subcity;
                txtStreet.Text = _addressData.Street;
                txtTownCity.Text = _addressData.City_Town;
                txtRegionState.Text = _addressData.State_Region;
                lkeCountry.EditValue = _addressData.Country;
                txtPrimaryEmail.Text = _addressData.PrimaryEmail;
                txtSecondaryEmail.Text = _addressData.SecondaryEmail;
                txtPoBox.Text = _addressData.PoBox;
                txtPostalZipCode.Text = _addressData.ZipCode_PostalCode;
                txtAdditionalInfo.Text = _addressData.AdditionalInfo;
            }
            else //leaves the controls blank
            {
                txtHouseNo.Text = txtKebele.Text = txtWoreda.Text = txtSubcity.Text = txtStreet.Text = txtTownCity.Text = txtRegionState.Text =
                txtPrimaryEmail.Text = txtSecondaryEmail.Text = txtPoBox.Text = txtPostalZipCode.Text = txtAdditionalInfo.Text = String.Empty;
                lkeCountry.EditValue = 0;
            }

            var _telephoneFax = telephoneFaxBL.GetTelephoneFax(supplierData.SupplierID);

            grdTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(_telephoneFax);
        }

        private void PopulateSuppliers()
        {
            grdSupplier.DataSource = supplierBL.GetData();
            supplierBL.HasData();
        }

        private void PopulateTelephoneFaxType()
        {
            repLKETelephoneFaxType.DataSource = telephoneFaxTypeBL.GetData(true);
        }

        private void PopulateCountries()
        {
            try
            {
                lkeCountry.Properties.DataSource = GimjaBL.CommonBL.GetCountriesList();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void RefreshSupplierData()
        {
            SetControlsReadOnly(layoutControl1, true);
            SetControlsReadOnly(layoutControl2, true);
            EnableDisableButtons(layoutControl1, "Load");
            PopulateSuppliers();

            grdViewSupplier_RowCellClick(null, null);
        }

        private void SaveBeforeClosing(FormClosingEventArgs e)
        {
            DialogResult _result = GimjaHelper.ShowQuestionWithYesNoCancelButtons("Would you like to save the changes?");

            if (_result == DialogResult.Yes)
            {
                btnSave_Click(null, null);

                if (isRequiredFieldBlank)
                    e.Cancel = true;
            }
            else if (_result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region Event Handlers
        
        private void txtName_Leave(object sender, EventArgs e)
        {
            if (txtCompanyName.Text.Equals(""))
            {
                dxErrorProvider.SetError(txtCompanyName, "required field");
                txtCompanyName.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtCompanyName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void txtContactPerson_Leave(object sender, EventArgs e)
        {
            if (txtContactPerson.Text.Equals(""))
            {
                dxErrorProvider.SetError(txtContactPerson, "required field");
                txtContactPerson.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtContactPerson, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void btnAddSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl1);
                ClearControlValues(layoutControl2);
                ClearControlValues(layoutControl3);

                EnableDisableButtons(layoutControl1, "Add");

                SetControlsReadOnly(layoutControl1, false);
                SetControlsReadOnly(layoutControl2, false);
                SetControlsReadOnly(layoutControl3, false);

                supplierBL.IsUpdate = false;

                supplierData = null;

                ////sets role ID to empty
                //supplierData.SupplierID = String.Empty;

                ////sets address ID to empty so that a new Guid can be generated while saving the data
                //supplierData.AddressID = Guid.Empty;

                //tempTelephoneFaxList has to be reset
                tempTelephoneFaxList = new List<TelephoneFaxData>();

                grdTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(tempTelephoneFaxList);

                txtCompanyName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void txtDescription_Leave(object sender, EventArgs e)
        {
            //if (txtDescription.Text.Equals(""))
            //{
            //    dxErrorProvider.SetError(txtDescription, "required field");
            //    txtDescription.Focus();
            //}
            //else
            //    dxErrorProvider.SetErrorType(txtDescription, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtCompanyName.Text) &&
                !String.IsNullOrWhiteSpace(txtContactPerson.Text) &&
                !String.IsNullOrWhiteSpace(txtPrimaryEmail.Text) &&
                !String.IsNullOrWhiteSpace(txtTownCity.Text) &&
                !String.IsNullOrWhiteSpace(txtRegionState.Text) &&
                !lkeCountry.EditValue.Equals(0))
                {
                    dynamic _supplierID;
                    dynamic _addressID;

                    if (supplierData != null) //updating manufacturer record
                    {
                        _supplierID = supplierData.SupplierID;
                        _addressID = supplierData.AddressID;
                    }
                    else //adding new manufacturer record
                    {
                        _supplierID = string.Empty;
                        _addressID = Guid.NewGuid();
                    }

                    supplierData = new SupplierData()
                    {
                        SupplierID = _supplierID,
                        CompanyName = txtCompanyName.Text.Trim(),
                        ContactPerson = txtContactPerson.Text.Trim(),
                        Description = txtDescription.Text.Trim(),
                        AddressID = _addressID,
                        IsActive = (chkIsActive.CheckState == CheckState.Checked ? true : false),
                        CreatedBy = Singleton.Instance.UserID,
                        CreatedDate = DateTime.Now
                    };

                    addressData = new AddressData
                    {
                        ID = (Guid)_addressID,
                        HouseNo = txtHouseNo.Text,
                        Kebele = txtKebele.Text,
                        Woreda = txtWoreda.Text,
                        State_Region = txtRegionState.Text,
                        Subcity = txtSubcity.Text,
                        Street = txtStreet.Text,
                        City_Town = txtTownCity.Text,
                        PoBox = txtPoBox.Text,
                        PrimaryEmail = txtPrimaryEmail.Text,
                        SecondaryEmail = txtSecondaryEmail.Text,
                        Country = lkeCountry.Text,
                        ZipCode_PostalCode = txtPostalZipCode.Text,
                        AdditionalInfo = txtAdditionalInfo.Text
                    };

                    supplierData.Address = addressData;

                    tempTelephoneFaxList = new List<TelephoneFaxData>();
                    for (int c = 0; c < grdViewTelephoneFax.RowCount; c++)
                    {
                        if (grdViewTelephoneFax.GetRowCellValue(c, "Number") != null && grdViewTelephoneFax.GetRowCellValue(c, "Type") != null)
                        {
                            telephoneFaxData = new TelephoneFaxData()
                            {
                                ID = Convert.ToInt32(grdViewTelephoneFax.GetRowCellValue(c, "ID")),
                                Number = grdViewTelephoneFax.GetRowCellValue(c, "Number").ToString(),
                                Type = Convert.ToInt16(grdViewTelephoneFax.GetRowCellValue(c, "Type")),
                                IsActive = Convert.ToBoolean(grdViewTelephoneFax.GetRowCellValue(c, "IsActive"))
                            };

                            tempTelephoneFaxList.Add(telephoneFaxData);
                        }
                    }

                    if (tempTelephoneFaxList == null || tempTelephoneFaxList.Count == 0)
                        tempTelephoneFaxList = null;

                    supplierData.TelephoneFax = tempTelephoneFaxList;

                    if (supplierBL.IsUpdate)
                    {
                        if (supplierBL.Update(supplierData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshSupplierData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (supplierBL.Add(supplierData))
                        {
                            GimjaHelper.ShowInformation("Data successfully added");
                            RefreshSupplierData();
                        }
                        else
                            GimjaHelper.ShowError("Data can not successfully added. Please try again");
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    isRequiredFieldBlank = true;
                    txtCompanyName.Focus();
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
                    if (supplierBL.Delete(supplierData.SupplierID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateSuppliers();

                        if (supplierBL.IsDataAvailable)
                        {
                            grdViewSupplier_RowCellClick(null, null);
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

        private void grdViewSupplier_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (supplierBL.IsDataAvailable)
                {
                    supplierData = new SupplierData()
                    {
                        SupplierID = grdViewSupplier.GetRowCellValue(grdViewSupplier.FocusedRowHandle, "SupplierID").ToString(),
                        CompanyName = grdViewSupplier.GetRowCellValue(grdViewSupplier.FocusedRowHandle, "CompanyName").ToString(),
                        ContactPerson = grdViewSupplier.GetRowCellValue(grdViewSupplier.FocusedRowHandle, "ContactPerson").ToString(),
                        Description = grdViewSupplier.GetRowCellValue(grdViewSupplier.FocusedRowHandle, "Description").ToString(),
                        IsActive = Convert.ToBoolean(grdViewSupplier.GetRowCellValue(grdViewSupplier.FocusedRowHandle, "IsActive")),
                        AddressID = grdViewSupplier.GetRowCellValue(grdViewSupplier.FocusedRowHandle, "AddressID") != null ? Guid.Parse(grdViewSupplier.GetRowCellValue(grdViewSupplier.FocusedRowHandle, "AddressID").ToString()) : Guid.Empty
                    };

                    LoadSupplierData();

                    EnableDisableButtons(layoutControl1, "Load");
                }
                else
                {
                    supplierData = new SupplierData();
                    EnableDisableButtons(layoutControl1, "Disable All");
                }

                SetControlsReadOnly(layoutControl1, true);
                SetControlsReadOnly(layoutControl2, true);
                SetControlsReadOnly(layoutControl3, true);
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl1, false);
            SetControlsReadOnly(layoutControl2, false);
            SetControlsReadOnly(layoutControl3, false);

            EnableDisableButtons(layoutControl1, "Edit");

            supplierBL.IsUpdate = true;

            //sets tempTelephoneFax to null
            tempTelephoneFaxList = new List<TelephoneFaxData>();

            txtCompanyName.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (supplierBL.IsDataAvailable)
            {
                grdViewSupplier_RowCellClick(null, null);
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

        private void frmSupplier_FormClosing(object sender, FormClosingEventArgs e)
        {
            string _companyName = txtCompanyName.Text.Trim();
            string _contactPerson = txtContactPerson.Text.Trim();
            string _description = txtDescription.Text.Trim();
            bool _IsActive = (chkIsActive.CheckState == CheckState.Checked ? true : false);

            string _houseNo = txtHouseNo.Text.Trim();
            string _kebele = txtKebele.Text.Trim();
            string _woreda = txtWoreda.Text.Trim();
            string _state_Region = txtRegionState.Text.Trim();
            string _subcity = txtSubcity.Text.Trim();
            string _street = txtStreet.Text.Trim();
            string _city_Town = txtTownCity.Text.Trim();
            string _poBox = txtPoBox.Text.Trim();
            string _primaryEmail = txtPrimaryEmail.Text.Trim();
            string _secondaryEmail = txtSecondaryEmail.Text.Trim();
            string _country = lkeCountry.Text.Trim();
            string _zipCode_PostalCode = txtPostalZipCode.Text.Trim();
            string _additionalInfo = txtAdditionalInfo.Text.Trim();

            //TODO: telephoneFax

            if (supplierData != null)
            {
                if (supplierData.CompanyName != _companyName ||
                    supplierData.ContactPerson != _contactPerson ||
                    supplierData.Description != _description)
                {
                    SaveBeforeClosing(e);
                }

                else if (addressData != null)
                {
                    if (
                    addressData.HouseNo != _houseNo ||
                    addressData.Kebele != _kebele ||
                    addressData.Woreda != _woreda ||
                    addressData.State_Region != _state_Region ||
                    addressData.Subcity != _subcity ||
                    addressData.Street != _street ||
                    addressData.City_Town != _city_Town ||
                    addressData.PoBox != _poBox ||
                    addressData.PrimaryEmail != _primaryEmail ||
                    addressData.SecondaryEmail != _secondaryEmail ||
                    addressData.Country != _country ||
                    addressData.ZipCode_PostalCode != _zipCode_PostalCode ||
                    addressData.AdditionalInfo != _additionalInfo)
                    {
                        SaveBeforeClosing(e);
                    }
                }
            }

            else if (supplierBL.IsUpdate == false &&
                    (_companyName != String.Empty || _contactPerson != String.Empty ||
                        _description != String.Empty ||
                        _kebele != String.Empty ||
                        _woreda != String.Empty ||
                        _state_Region != String.Empty ||
                        _subcity != String.Empty ||
                        _street != String.Empty ||
                        _city_Town != String.Empty ||
                        _poBox != String.Empty ||
                        _primaryEmail != String.Empty ||
                        _secondaryEmail != String.Empty ||
                        _country != String.Empty ||
                        _zipCode_PostalCode != String.Empty ||
                        _additionalInfo != String.Empty))
            {
                SaveBeforeClosing(e);
            }
        }

        #endregion
    }
}