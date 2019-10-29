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
    public partial class frmUser : DevExpress.XtraEditors.XtraForm
    {
        private bool isRequiredFieldBlank = false;

        private UserBL userBL;
        private HashPasswordBL hashPasswordBL;

        //used to temporarily store telephones/faxes
        private List<TelephoneFaxData> tempTelephoneFaxList;
        //private List<RoleData> tempRoleList;

        private UserData userData;
        private RoleBL roleBL;
        private RoleData roleData;

        private AddressBL addressBL;
        private AddressData addressData;

        private TelephoneFaxBL telephoneFaxBL;
        private TelephoneFaxData telephoneFaxData;

        private TelephoneFaxTypeBL telephoneFaxTypeBL;

        public frmUser()
        {
            InitializeComponent();

            userBL = new UserBL();
            hashPasswordBL = new HashPasswordBL();

            roleBL = new RoleBL();
            roleData = new RoleData();
            
            addressBL = new AddressBL();
            
            userBL.IsUpdate = false;

            PopulateUsers();

            PopulateUserRoles();

            telephoneFaxBL = new TelephoneFaxBL();
            telephoneFaxTypeBL = new TelephoneFaxTypeBL();

            tempTelephoneFaxList = new List<TelephoneFaxData>();

            PopulateTelephoneFaxType();

            PopulateCountries();
            
            grdViewUser_RowCellClick(null, null);
        }

        #region Methods

        private void PopulateUsers()
        {
            grdUser.DataSource = userBL.GetData();
        }

        private void PopulateUserRoles()
        {
            lkeRole.Properties.DataSource = userBL.GetUserRole();
        }

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

            //User Roles info
            lkeRole.Properties.DataSource = userBL.GetUserRole();
        }

        private void SetControlsReadOnly(LayoutControl layoutControl, bool val)
        {
            foreach (var ctrl in layoutControl.Controls)
            {
                if (ctrl is TextEdit)
                {
                    TextEdit txt = ctrl as TextEdit;
                    bool _isUserID = txt.Tag != null ? true : false;

                    if (txt != null && _isUserID && userBL.IsUpdate)
                    {
                        txt.Properties.ReadOnly = true;
                    }
                    else
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

            grdUser.Enabled = val;
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

        private void LoadUserData()
        {
            ////displays User info
            txtUserID.Text = userData.UserID;
            txtFullName.Text = userData.FullName;
            txtPassword.Text = txtConfirmPassword.Text = userData.Password;
            chkIsActive.CheckState = (userData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;

            if (userData.AddressID != Guid.Empty)
            {
                var _addressData = addressBL.GetAddress((Guid)userData.AddressID);

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

            var _telephoneFax = telephoneFaxBL.GetTelephoneFax(userData.UserID);
            grdTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(_telephoneFax);

            lkeRole.EditValue = userBL.GetRoleID(userData.UserID);
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

        private void RefreshUserData()
        {
            SetControlsReadOnly(layoutControl1, true);
            SetControlsReadOnly(layoutControl2, true);
            SetControlsReadOnly(layoutControl3, true);
            EnableDisableButtons(layoutControl1, "Load");
            PopulateUsers();
            grdViewUser_RowCellClick(null, null);
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
                
                userBL.IsUpdate = false;
                
                SetControlsReadOnly(layoutControl1, false);
                SetControlsReadOnly(layoutControl2, false);
                SetControlsReadOnly(layoutControl3, false);

                

                //sets user ID to empty
                //userData.UserID = String.Empty;

                //sets address ID to empty so that a new Guid can be generated while saving the data
                userData.AddressID = Guid.Empty;

                //tempTelephoneFaxList has to be reset
                tempTelephoneFaxList = new List<TelephoneFaxData>();
                grdTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(tempTelephoneFaxList);
                
                txtUserID.Focus();
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
                if (!String.IsNullOrWhiteSpace(txtFullName.Text.Trim()) &&
                !String.IsNullOrWhiteSpace(txtUserID.Text.Trim()) &&
                !String.IsNullOrWhiteSpace(txtPassword.Text.Trim()) &&
                !String.IsNullOrWhiteSpace(txtConfirmPassword.Text.Trim()) &&
                !String.IsNullOrWhiteSpace(txtKebele.Text.Trim()) &&
                !String.IsNullOrWhiteSpace(txtWoreda.Text.Trim()) &&
                !String.IsNullOrWhiteSpace(txtSubcity.Text.Trim()) &&
                !String.IsNullOrWhiteSpace(txtTownCity.Text.Trim()) &&
                !String.IsNullOrWhiteSpace(txtRegionState.Text.Trim()) &&
                !lkeCountry.EditValue.Equals(0) &&
                !lkeRole.EditValue.Equals(0)) 
                {
                    if (txtPassword.Text.Trim() == txtConfirmPassword.Text.Trim())
                    {
                        #region User info
                        dynamic _userID;
                        dynamic _addressID;
                        //dynamic _password;

                        //when updating user record
                        if (userData != null)
                        {
                            _userID = userData.UserID;
                            _addressID = userData.AddressID;
                            
                            //TODO: change the password on if it's new or modified
                            //if(userData.Password == hashPasswordBL.GetHashedPassword(txtPassword.Text.Trim()))
                            //    _password = userData.Password;
                            //else
                            //    _password = hashPasswordBL.GetHashedPassword(txtPassword.Text.Trim());
                        }
                        else //for new user record
                        {
                            userData.UserID = String.Empty;
                            _addressID = Guid.NewGuid();
                            //_password = hashPasswordBL.GetHashedPassword(txtPassword.Text.Trim());
                        }

                        userData = new UserData()
                        {
                            UserID = txtUserID.Text.Trim().ToLower(),
                            Password = hashPasswordBL.GetHashedPassword(txtPassword.Text.Trim()),
                            FullName = txtFullName.Text.Trim(),
                            AddressID = _addressID,
                            RoleID = Convert.ToInt32(lkeRole.EditValue),
                            IsActive = (chkIsActive.CheckState == CheckState.Checked ? true : false),
                            CreatedBy = Singleton.Instance.UserID,
                            CreatedDate = DateTime.Now
                        };
                        #endregion

                        #region Address Info
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

                        userData.Address = addressData;
                        #endregion

                        #region TelephoneFax info
                        //sets telephoneFax list
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

                        userData.TelephoneFax = tempTelephoneFaxList;
                        #endregion

                        #region User Role info
                        ////sets the selected roleID(s)
                        //tempRoleList = new List<RoleData>();
                        //for (int c = 0; c < grdViewUserRole.RowCount; c++)
                        //{
                        //    if ((bool)grdViewUserRole.GetRowCellValue(c, "Applies") == true)
                        //    {
                        //        roleData = new RoleData()
                        //        {
                        //            ID = Convert.ToInt32(grdViewUserRole.GetRowCellValue(c, "RoleID"))
                        //        };

                        //        tempRoleList.Add(roleData);
                        //    }
                        //}

                        //if (tempRoleList == null || tempRoleList.Count == 0)
                        //    tempRoleList = null;

                        //userData.Role = tempRoleList;
                        #endregion

                        if (userBL.IsUpdate)
                        {
                            if (userBL.Update(userData))
                            {
                                GimjaHelper.ShowInformation("Data successfully updated");
                                RefreshUserData();
                            }
                            else
                                GimjaHelper.ShowWarning("Data can not be successfully updated");
                        }
                        else
                        {
                            if (userBL.IsValid(userData.UserID))
                            {
                                if (userBL.Add(userData))
                                {
                                    GimjaHelper.ShowInformation("Data successfully added");
                                    RefreshUserData();
                                }
                                else
                                    GimjaHelper.ShowWarning("Data can not successfully added. Please try again");
                            }
                            else
                                GimjaHelper.ShowWarning(String.Format("The user id  <{0}> is already taken. Please use a different user ID", userData.UserID));
                        }
                    }
                    else
                    {
                        GimjaHelper.ShowWarning("The password and confirm password values do not match");
                        isRequiredFieldBlank = true;
                        txtPassword.Focus();
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    isRequiredFieldBlank = true;
                    txtUserID.Focus();
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
                    if (userBL.Delete(userData.UserID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateUsers();

                        if (userBL.IsDataAvailable)
                        {
                            grdViewUser_RowCellClick(null, null);
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

        private void grdViewUser_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var _focusedRow = (UserData)grdViewUser.GetFocusedRow();

                if(_focusedRow!=null)
                {
                    userData = _focusedRow;

                    LoadUserData();

                    EnableDisableButtons(layoutControl1, "Load");
                }
                else
                {
                    userData = new UserData();
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
            userBL.IsUpdate = true;

            SetControlsReadOnly(layoutControl1, false);
            SetControlsReadOnly(layoutControl2, false);
            SetControlsReadOnly(layoutControl3, false);

            EnableDisableButtons(layoutControl1, "Edit");

            //sets tempTelephoneFax to null
            tempTelephoneFaxList = new List<TelephoneFaxData>();

            txtFullName.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (userBL.IsDataAvailable)
            {
                grdViewUser_RowCellClick(null, null);
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

        private void txtFullName_Leave(object sender, EventArgs e)
        {
            //if (txtFullName.Text.Equals(""))
            //{
            //    dxErrorProvider.SetError(txtFullName, "required field");
            //    txtFullName.Focus();
            //}
            //else
            //    dxErrorProvider.SetErrorType(txtFullName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void txtUserID_Leave(object sender, EventArgs e)
        {
            //if (txtUserID.Text.Equals(""))
            //{
            //    dxErrorProvider.SetError(txtUserID, "required field");
            //    txtUserID.Focus();
            //}
            //else
            //    dxErrorProvider.SetErrorType(txtUserID, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            //if (txtPassword.Text.Equals(""))
            //{
            //    dxErrorProvider.SetError(txtPassword, "required field");
            //    txtPassword.Focus();
            //}
            //else
            //    dxErrorProvider.SetErrorType(txtPassword, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void txtConfirmPassword_Leave(object sender, EventArgs e)
        {
            if (txtConfirmPassword.Text.Equals(""))
            {
                dxErrorProvider.SetError(txtConfirmPassword, "required field");
                txtConfirmPassword.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtConfirmPassword, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);

            if (!txtConfirmPassword.Text.Equals(txtPassword.Text))
            {
                XtraMessageBox.Show("The passwords do not match", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirmPassword.Focus();
            }
        }

        private void frmUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            string _userID = txtUserID.Text.Trim();
            string _fullName = txtFullName.Text.Trim();
            string _password = txtPassword.Text.Trim();
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

            //TODO: add telephoneFax here

            if (userData != null)
            {
                if (userData.UserID != _userID ||
                    userData.FullName != _fullName ||
                    userData.Password != _password ||
                    userData.IsActive != _IsActive)
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

            else if (userBL.IsUpdate == false &&
                    (_userID != String.Empty || _fullName != String.Empty ||
                        _password != String.Empty ||
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