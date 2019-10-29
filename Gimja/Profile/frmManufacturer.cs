using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using GimjaBL;
using DevExpress.XtraLayout;

namespace Gimja
{
    public partial class frmManufacturer : DevExpress.XtraEditors.XtraForm
    {
        bool isRequiredFieldBlank = false;

        //used to temporarily store telephones/faxes
        private List<TelephoneFaxData> tempTelephoneFaxList;

        private ManufacturerBL manufacturerBL;
        private ManufacturerData manufacturerData;

        private AddressBL addressBL;
        private AddressData addressData;

        private TelephoneFaxBL telephoneFaxBL;
        private TelephoneFaxData telephoneFaxData;

        private TelephoneFaxTypeBL telephoneFaxTypeBL;

        public frmManufacturer()
        {
            InitializeComponent();

            manufacturerBL = new ManufacturerBL();
            addressBL = new AddressBL();
            telephoneFaxBL = new TelephoneFaxBL();
            telephoneFaxTypeBL = new TelephoneFaxTypeBL();

            tempTelephoneFaxList = new List<TelephoneFaxData>();

            manufacturerBL.IsUpdate = false;

            PopulateTelephoneFaxType();

            PopulateCountries();

            PopulateManufacturers();

            grdViewManufacturer_RowCellClick(null, null);
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

            grdManufacturer.Enabled = val;
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

        private void LoadManufacturerData()
        {
            ////displays Manufacturer info
            txtName.Text = manufacturerData.Name;
            txtContactPerson.Text = manufacturerData.ContactPerson;
            txtDescription.Text = manufacturerData.Description;
            chkIsActive.CheckState = (manufacturerData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;

            if (manufacturerData.AddressID != Guid.Empty)
            {
                var _addressData = addressBL.GetAddress((Guid)manufacturerData.AddressID);

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

            var _telephoneFax = telephoneFaxBL.GetTelephoneFax(manufacturerData.ManufacturerID);
            
            grdTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(_telephoneFax);
        }

        private void PopulateManufacturers()
        {
            grdManufacturer.DataSource = manufacturerBL.GetData();
            manufacturerBL.HasData();
        }

        private void PopulateTelephoneFaxType()
        {
            repLKTelephoneFaxType.DataSource = telephoneFaxTypeBL.GetData(true);
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

        private void RefreshManufacturerData()
        {
            SetControlsReadOnly(layoutControl1, true);
            SetControlsReadOnly(layoutControl2, true);
            SetControlsReadOnly(layoutControl2, true);

            EnableDisableButtons(layoutControl1, "Load");
            PopulateManufacturers();
            grdViewManufacturer_RowCellClick(null, null);
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
        
        private void btnAdd_Click(object sender, EventArgs e)
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

                manufacturerBL.IsUpdate = false;

                manufacturerData = null;

                ////sets manufacturer ID to empty
                //manufacturerData.ManufacturerID = String.Empty;

                ////sets address ID to empty so that a new Guid can be generated while saving the data
                //manufacturerData.AddressID = Guid.Empty;

                //tempTelephoneFaxList has to be reset
                tempTelephoneFaxList = new List<TelephoneFaxData>();

                grdTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(tempTelephoneFaxList);

                txtName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtName.Text) &&
                !String.IsNullOrWhiteSpace(txtContactPerson.Text) &&
                !String.IsNullOrWhiteSpace(txtPrimaryEmail.Text) &&
                !String.IsNullOrWhiteSpace(txtTownCity.Text) &&
                !String.IsNullOrWhiteSpace(txtRegionState.Text) &&
                !lkeCountry.EditValue.Equals(0))
                {
                    dynamic _manufacturerID;
                    dynamic _addressID;

                    //sets manufacturer and address IDs when the operation is update
                    if (manufacturerData != null)
                    {
                        _manufacturerID = manufacturerData.ManufacturerID;
                        _addressID = manufacturerData.AddressID;
                    }
                    else //operation is to add new manufacturer
                    {
                        _manufacturerID = string.Empty;
                        _addressID = Guid.NewGuid();
                    }

                    manufacturerData = new ManufacturerData()
                    {
                        ManufacturerID = _manufacturerID,
                        Name = txtName.Text.Trim(),
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

                    manufacturerData.Address = addressData;

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

                    manufacturerData.TelephoneFax = tempTelephoneFaxList;

                    if (manufacturerBL.IsUpdate)
                    {
                        if (manufacturerBL.Update(manufacturerData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshManufacturerData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (manufacturerBL.Add(manufacturerData))
                        {
                            GimjaHelper.ShowInformation("Data successfully added");
                            RefreshManufacturerData();
                        }
                        else
                            GimjaHelper.ShowError("Data can not successfully added. Please try again");
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    isRequiredFieldBlank = true;
                    txtName.Focus();
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
                    if (manufacturerBL.Delete(manufacturerData.ManufacturerID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateManufacturers();

                        if (manufacturerBL.IsDataAvailable)
                        {
                            grdViewManufacturer_RowCellClick(null, null);
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

        private void grdViewManufacturer_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (manufacturerBL.IsDataAvailable)
                {
                    manufacturerData = new ManufacturerData()
                    {
                        ManufacturerID = grdViewManufacturer.GetRowCellValue(grdViewManufacturer.FocusedRowHandle, "ManufacturerID").ToString(),
                        Name = grdViewManufacturer.GetRowCellValue(grdViewManufacturer.FocusedRowHandle, "Name").ToString(),
                        ContactPerson = grdViewManufacturer.GetRowCellValue(grdViewManufacturer.FocusedRowHandle, "ContactPerson").ToString(),
                        Description = grdViewManufacturer.GetRowCellValue(grdViewManufacturer.FocusedRowHandle, "Description").ToString(),
                        IsActive = Convert.ToBoolean(grdViewManufacturer.GetRowCellValue(grdViewManufacturer.FocusedRowHandle, "IsActive")),
                        AddressID = grdViewManufacturer.GetRowCellValue(grdViewManufacturer.FocusedRowHandle, "AddressID") != null ? Guid.Parse(grdViewManufacturer.GetRowCellValue(grdViewManufacturer.FocusedRowHandle, "AddressID").ToString()) : Guid.Empty
                    };

                    LoadManufacturerData();

                    EnableDisableButtons(layoutControl1, "Load");
                }
                else
                {
                    manufacturerData = new ManufacturerData();
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

            manufacturerBL.IsUpdate = true;

            //sets tempTelephoneFax to null
            tempTelephoneFaxList = new List<TelephoneFaxData>();

            txtName.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (manufacturerBL.IsDataAvailable)
            {
                grdViewManufacturer_RowCellClick(null, null);
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

        private void txtName_Leave(object sender, EventArgs e)
        {
            if (txtName.Text.Equals(""))
            {
                dxErrorProvider.SetError(txtName, "required field");
                txtName.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void frmManufacturer_FormClosing(object sender, FormClosingEventArgs e)
        {
            string _name = txtName.Text.Trim();
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

            //TODO: check telephoneFax

            if (manufacturerData != null)
            {
                if (manufacturerData.Name != _name ||
                    manufacturerData.ContactPerson != _contactPerson ||
                    manufacturerData.Description != _description ||
                    manufacturerData.IsActive != _IsActive)
                {
                    SaveBeforeClosing(e);
                }

                if (addressData != null)
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

            else if (manufacturerBL.IsUpdate == false &&
                    (_name != String.Empty || _contactPerson != String.Empty ||
                        _description != String.Empty  ||
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