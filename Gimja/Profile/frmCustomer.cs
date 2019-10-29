using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using GimjaBL;
using DevExpress.XtraLayout;


namespace Gimja
{
    public partial class frmCustomer : DevExpress.XtraEditors.XtraForm
    {
        bool isRequiredFieldBlank = false;

        //used to temporarily store telephones/faxes
        private List<TelephoneFaxData> tempTelephoneFaxList;

        private CustomerBL customerBL;
        private CustomerData customerData;

        private AddressBL addressBL;
        private AddressData addressData;

        private TelephoneFaxBL telephoneFaxBL;
        private TelephoneFaxData telephoneFaxData;

        private TelephoneFaxTypeBL telephoneFaxTypeBL;

        public frmCustomer()
        {
            InitializeComponent();

            customerBL = new CustomerBL();
            addressBL = new AddressBL();
            telephoneFaxBL = new TelephoneFaxBL();
            telephoneFaxTypeBL = new TelephoneFaxTypeBL();

            tempTelephoneFaxList = new List<TelephoneFaxData>();

            customerBL.IsUpdate = false;

            PopulateTelephoneFaxType();

            PopulateCountries();

            PopulateCustomers();

            grdViewCustomer_RowCellClick(null, null);
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

            grdCustomer.Enabled = val;
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

        private void LoadCustomerData()
        {
            ////displays Customer info
            txtName.Text = customerData.Name;
            txtFatherName.Text = customerData.FatherName;
            txtGrandfatherName.Text = customerData.GrandfatherName;
            txtCompanyName.Text = customerData.CompanyName;
            txtTINNo.Text = customerData.TINNo;
            txtVATRegNo.Text = customerData.VATRegistrationNo;
            dtVATRegDate.EditValue = customerData.VATRegistrationDate;
            chkIsActive.CheckState = (customerData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;

            if (customerData.AddressID != Guid.Empty)
            {
                addressData = addressBL.GetAddress((Guid)customerData.AddressID);

                //displays Address info
                txtHouseNo.Text = addressData.HouseNo;
                txtKebele.Text = addressData.Kebele;
                txtWoreda.Text = addressData.Woreda;
                txtSubcity.Text = addressData.Subcity;
                txtStreet.Text = addressData.Street;
                txtTownCity.Text = addressData.City_Town;
                txtRegionState.Text = addressData.State_Region;
                lkeCountry.EditValue = addressData.Country;
                txtPrimaryEmail.Text = addressData.PrimaryEmail;
                txtSecondaryEmail.Text = addressData.SecondaryEmail;
                txtPoBox.Text = addressData.PoBox;
                txtPostalZipCode.Text = addressData.ZipCode_PostalCode;
                txtAdditionalInfo.Text = addressData.AdditionalInfo;
            }
            else //leaves the controls blank
            {
                txtHouseNo.Text = txtKebele.Text = txtWoreda.Text = txtSubcity.Text = txtStreet.Text = txtTownCity.Text = txtRegionState.Text =
                txtPrimaryEmail.Text = txtSecondaryEmail.Text = txtPoBox.Text = txtPostalZipCode.Text = txtAdditionalInfo.Text = String.Empty;
                lkeCountry.EditValue = 0;
            }

            var _telephoneFax = telephoneFaxBL.GetTelephoneFax(customerData.ID);

            grdTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(_telephoneFax);
        }

        private void PopulateCustomers()
        {
            grdCustomer.DataSource = customerBL.GetData();
            customerBL.HasData();
        }

        private void PopulateTelephoneFaxType()
        {
            repLkeTelephoneFaxType.DataSource = telephoneFaxTypeBL.GetData(true);
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

        private void RefreshCustomerData()
        {
            SetControlsReadOnly(layoutControl1, true);
            SetControlsReadOnly(layoutControl2, true);
            SetControlsReadOnly(layoutControl3, true);
            EnableDisableButtons(layoutControl1, "Load");
            PopulateCustomers();
            grdViewCustomer_RowCellClick(null, null);
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

                customerBL.IsUpdate = false;

                customerData = null;

                ////sets customer ID to empty
                //customerData.ID = String.Empty;

                ////sets address ID to empty so that a new Guid can be generated while saving the data
                //customerData.AddressID = Guid.Empty;

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
                !String.IsNullOrWhiteSpace(txtFatherName.Text) &&
                !String.IsNullOrWhiteSpace(txtGrandfatherName.Text) &&
                //!String.IsNullOrWhiteSpace(txtKebele.Text) &&
                //!String.IsNullOrWhiteSpace(txtWoreda.Text) &&
                //!String.IsNullOrWhiteSpace(txtSubcity.Text) &&
                !String.IsNullOrWhiteSpace(txtTownCity.Text) &&
                !String.IsNullOrWhiteSpace(txtRegionState.Text) &&
                !lkeCountry.EditValue.Equals(0))
                {
                    dynamic _customerID;
                    dynamic _addressID;

                    //sets customer and address IDs when the operation is update
                    if (customerData != null)
                    {
                        _customerID = customerData.ID;
                        _addressID = customerData.AddressID;
                    }
                    else //operation is to add new customer record
                    {
                        _customerID = string.Empty;

                        //generates address ID
                        _addressID = Guid.NewGuid();
                    }

                    customerData = new CustomerData()
                    {
                        ID = _customerID,
                        Name = txtName.Text.Trim(),
                        FatherName = txtFatherName.Text.Trim(),
                        GrandfatherName = txtGrandfatherName.Text.Trim(),
                        CompanyName = txtCompanyName.Text.Trim(),
                        AddressID = _addressID,
                        TINNo = txtTINNo.Text.Trim(),
                        VATRegistrationNo = txtVATRegNo.Text.Trim(),
                        VATRegistrationDate = Convert.ToDateTime(dtVATRegDate.Text).Date,
                        IsActive = (chkIsActive.CheckState == CheckState.Checked ? true : false),
                        CreatedBy = Singleton.Instance.UserID,
                        CreatedDate = DateTime.Now
                    };

                    addressData = new AddressData
                    {
                        ID = (Guid)_addressID,
                        HouseNo = txtHouseNo.Text.Trim(),
                        Kebele = txtKebele.Text.Trim(),
                        Woreda = txtWoreda.Text.Trim(),
                        State_Region = txtRegionState.Text.Trim(),
                        Subcity = txtSubcity.Text.Trim(),
                        Street = txtStreet.Text.Trim(),
                        City_Town = txtTownCity.Text.Trim(),
                        PoBox = txtPoBox.Text.Trim(),
                        PrimaryEmail = txtPrimaryEmail.Text.Trim(),
                        SecondaryEmail = txtSecondaryEmail.Text.Trim(),
                        Country = lkeCountry.Text.Trim(),
                        ZipCode_PostalCode = txtPostalZipCode.Text.Trim(),
                        AdditionalInfo = txtAdditionalInfo.Text.Trim()
                    };

                    customerData.Address = addressData;

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

                    customerData.TelephoneFax = tempTelephoneFaxList;

                    if (customerBL.IsUpdate)
                    {
                        if (customerBL.Update(customerData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshCustomerData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (customerBL.Add(customerData))
                        {
                            GimjaHelper.ShowInformation("Data successfully added");
                            RefreshCustomerData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not successfully added. Please try again");
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
                    if (customerBL.Delete(customerData.ID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateCustomers();

                        if (customerBL.IsDataAvailable)
                        {
                            grdViewCustomer_RowCellClick(null, null);
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

        private void grdViewCustomer_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                customerData = grdViewCustomer.GetFocusedRow() as CustomerData;
                
                if(customerData !=null)
                {
                ////if (customerBL.IsDataAvailable)
                ////{
                //    customerData = new CustomerData()
                //    {
                //        ID = grdViewCustomer.GetRowCellValue(grdViewCustomer.FocusedRowHandle, "ID").ToString(),
                //        Name = grdViewCustomer.GetRowCellValue(grdViewCustomer.FocusedRowHandle, "Name").ToString(),
                //        FatherName = grdViewCustomer.GetRowCellValue(grdViewCustomer.FocusedRowHandle, "FatherName").ToString(),
                //        GrandfatherName = grdViewCustomer.GetRowCellValue(grdViewCustomer.FocusedRowHandle, "GrandfatherName").ToString(),
                //        CompanyName = grdViewCustomer.GetRowCellValue(grdViewCustomer.FocusedRowHandle, "CompanyName").ToString(),
                //        TINNo = grdViewCustomer.GetRowCellValue(grdViewCustomer.FocusedRowHandle, "TINNo").ToString(),
                //        VATRegistrationNo = grdViewCustomer.GetRowCellValue(grdViewCustomer.FocusedRowHandle, "VATRegistrationNo").ToString(),
                //        VATRegistrationDate = Convert.ToDateTime(grdViewCustomer.GetRowCellValue(grdViewCustomer.FocusedRowHandle, "VATRegistrationDate")).Date,
                //        IsActive = Convert.ToBoolean(grdViewCustomer.GetRowCellValue(grdViewCustomer.FocusedRowHandle, "IsActive")),
                //        AddressID = grdViewCustomer.GetRowCellValue(grdViewCustomer.FocusedRowHandle, "AddressID") != null ? Guid.Parse(grdViewCustomer.GetRowCellValue(grdViewCustomer.FocusedRowHandle, "AddressID").ToString()) : Guid.Empty
                //    };

                    LoadCustomerData();

                    EnableDisableButtons(layoutControl1, "Load");
                }
                else
                {
                    customerData = new CustomerData();
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

            customerBL.IsUpdate = true;

            //sets tempTelephoneFax to null
            tempTelephoneFaxList = new List<TelephoneFaxData>();

            txtName.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (customerBL.IsDataAvailable)
            {
                grdViewCustomer_RowCellClick(null, null);
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

        private void txtFatherName_Leave(object sender, EventArgs e)
        {
            if (txtFatherName.Text.Equals(""))
            {
                dxErrorProvider.SetError(txtFatherName, "required field");
                txtFatherName.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtFatherName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        private void txtGrandfatherName_Leave(object sender, EventArgs e)
        {
            if (txtGrandfatherName.Text.Equals(""))
            {
                dxErrorProvider.SetError(txtGrandfatherName, "required field");
                txtGrandfatherName.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtGrandfatherName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        private void txtCompanyName_Leave(object sender, EventArgs e)
        {
            //if (txtCompanyName.Text.Equals(""))
            //{
            //    dxErrorProvider.SetError(txtCompanyName, "required field");
            //    txtCompanyName.Focus();
            //}
            //else
            //    dxErrorProvider.SetErrorType(txtCompanyName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        private void txtTINNo_Leave(object sender, EventArgs e)
        {
            if (txtTINNo.Text.Equals(""))
            {
                dxErrorProvider.SetError(txtTINNo, "required field");
                txtTINNo.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtTINNo, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        private void txtVATRegNo_Leave(object sender, EventArgs e)
        {
            if (txtVATRegNo.Text.Equals(""))
            {
                dxErrorProvider.SetError(txtVATRegNo, "required field");
                txtVATRegNo.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtVATRegNo, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        private void dtVATRegDate_Leave(object sender, EventArgs e)
        {
            if (dtVATRegDate.EditValue.Equals(0))
            {
                dxErrorProvider.SetError(dtVATRegDate, "required field");
                dtVATRegDate.Focus();
            }
            else
                dxErrorProvider.SetErrorType(dtVATRegDate, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

        }

        private void frmCustomer_FormClosing(object sender, FormClosingEventArgs e)
        {
            string _name = txtName.Text.Trim();
            string _fatherName = txtFatherName.Text.Trim();
            string _grandfatherName = txtGrandfatherName.Text.Trim();
            string _companyName = txtCompanyName.Text.Trim();
            string _TINNo = txtTINNo.Text.Trim();
            string _VATRegistrationNo = txtVATRegNo.Text.Trim();

            DateTime? _VATRegistrationDate = dtVATRegDate.Text != String.Empty ? Convert.ToDateTime(dtVATRegDate.Text).Date : (Nullable<DateTime>)null;
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

            //List<TelephoneFaxData> _telephoneFaxList = grdViewCustomer.getv as List<TelephoneFaxData>;
            
            //for (int c = 0; c < grdViewTelephoneFax.RowCount; c++)
            //{
            //    if (grdViewTelephoneFax.GetRowCellValue(c, "Number") != null && grdViewTelephoneFax.GetRowCellValue(c, "Type") != null)
            //    {
            //        telephoneFaxData = new TelephoneFaxData()
            //        {
            //            ID = Convert.ToInt32(grdViewTelephoneFax.GetRowCellValue(c, "ID")),
            //            Number = grdViewTelephoneFax.GetRowCellValue(c, "Number").ToString(),
            //            Type = Convert.ToInt16(grdViewTelephoneFax.GetRowCellValue(c, "Type")),
            //            IsActive = Convert.ToBoolean(grdViewTelephoneFax.GetRowCellValue(c, "IsActive"))
            //        };
            //        tempTelephoneFaxList.Add(telephoneFaxData);
            //    }
            //}

            if (customerData != null)
            {
                if (customerData.Name != _name ||
                    customerData.FatherName != _fatherName ||
                    customerData.GrandfatherName != _grandfatherName ||
                    customerData.CompanyName != _companyName ||
                    customerData.TINNo != _TINNo ||
                    customerData.VATRegistrationNo != _VATRegistrationNo ||
                    customerData.VATRegistrationDate != _VATRegistrationDate ||
                    customerData.IsActive != _IsActive)
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

            else if (customerBL.IsUpdate == false &&
                    (_name != String.Empty || _fatherName != String.Empty ||
                        _grandfatherName != String.Empty || _companyName != String.Empty ||
                        _TINNo != String.Empty || _VATRegistrationNo != String.Empty ||
                        _VATRegistrationDate != null ||
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
            //else if (telephoneFaxData != null && (tempTelephoneFaxList != _telephoneFaxList))
            //{
            //    SaveBeforeClosing(e);
            //}
        }

        #endregion
    }
}