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
using System.IO;

namespace Gimja
{
    public partial class frmCompanyInfo : DevExpress.XtraEditors.XtraForm
    {
        //used to temporarily store telephones/faxes
        private List<TelephoneFaxData> tempTelephoneFaxList;

        private CompanyProfileBL companyProfileBL;
        private CompanyProfileData companyProfileData;

        private WarehouseBL warehouseBL;
        private WarehouseData warehouseData;

        private BranchBL branchBL;
        private BranchData branchData;

        private AddressBL addressBL;
        private AddressData addressData;

        private TelephoneFaxBL telephoneFaxBL;
        private TelephoneFaxData telephoneFaxData;

        private TelephoneFaxTypeBL telephoneFaxTypeBL;

        byte[] companyLogo = null;

        public frmCompanyInfo()
        {
            InitializeComponent();

            companyProfileBL = new CompanyProfileBL();
            warehouseBL = new WarehouseBL();
            branchBL = new BranchBL();

            addressBL = new AddressBL();
            telephoneFaxBL = new TelephoneFaxBL();
            telephoneFaxTypeBL = new TelephoneFaxTypeBL();

            tempTelephoneFaxList = new List<TelephoneFaxData>();

            companyProfileBL.IsUpdate = false;
            warehouseBL.IsUpdate = false;
            branchBL.IsUpdate = false; 

            PopulateTelephoneFaxType();
            PopulateCountries();

            PopulateCompanyProfile();

            PopulateWarehouses();
            PopulateActiveNotDefaultWarehouses();

            PopulateBranches();
            PopulateActiveNotDefaultBranches();

            grdViewCompany_RowCellClick(null, null);
        }

        #region Company Profile Methods

        private void PopulateCompanyProfile()
        {
            grdCompany.DataSource = companyProfileBL.GetData();
            companyProfileBL.HasData();
        }

        private void LoadCompanyProfileData()
        {
            txtCompanyAmharicName.Text = companyProfileData.AmharicName;
            txtCompanyEnglishName.Text = companyProfileData.EnglishName;
            txtCompanyTINNo.Text = companyProfileData.TINNumber;
            txtCompanyVATRegNo.Text = companyProfileData.VATRegistrationNo;
            dtCompanyVATRegDate.EditValue = companyProfileData.VATRegistrationDate != null ? companyProfileData.VATRegistrationDate : null;
            picComapnyLogo.Image = companyProfileData.Logo != null ? Image.FromStream(ConvertBytesToImageBL.ByteToImage(companyProfileData.Logo)) : null;

            if (companyProfileData.AddressID != Guid.Empty)
            {
                var _addressData = addressBL.GetAddress((Guid)companyProfileData.AddressID);

                //displays Address info
                txtCompanyHouseNo.Text = _addressData.HouseNo;
                txtCompanyKebele.Text = _addressData.Kebele;
                txtCompanyWoreda.Text = _addressData.Woreda;
                txtCompanySubcity.Text = _addressData.Subcity;
                txtCompanyStreet.Text = _addressData.Street;
                txtCompanyTownCity.Text = _addressData.City_Town;
                txtCompanyRegionState.Text = _addressData.State_Region;
                lkeCompanyCountry.EditValue = _addressData.Country;
                txtCompanyPrimaryEmail.Text = _addressData.PrimaryEmail;
                txtCompanySecondaryEmail.Text = _addressData.SecondaryEmail;
                txtCompanyPoBox.Text = _addressData.PoBox;
                txtCompanyPostalZipCode.Text = _addressData.ZipCode_PostalCode;
                txtCompanyAdditionalInfo.Text = _addressData.AdditionalInfo;
            }
            else //leaves the controls blank
            {
                txtCompanyHouseNo.Text = txtCompanyKebele.Text = txtCompanyWoreda.Text = txtCompanySubcity.Text = txtCompanyStreet.Text = txtCompanyTownCity.Text = txtCompanyRegionState.Text =
                txtCompanyPrimaryEmail.Text = txtCompanySecondaryEmail.Text = txtCompanyPoBox.Text = txtCompanyPostalZipCode.Text = txtCompanyAdditionalInfo.Text = String.Empty;
                lkeBranchCountry.EditValue = 0;
            }

            var _telephoneFax = telephoneFaxBL.GetTelephoneFax(companyProfileData.ID);

            grdCompanyTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(_telephoneFax);
        }

        private void RefreshCompanyProfileData()
        {
            SetControlsReadOnly(layoutControl1, true);
            SetControlsReadOnly(layoutControl2, true);
            SetControlsReadOnly(layoutControl3, true);
            EnableDisableButtons(layoutControl2, "Load");
            EnableDisableTabs(true);
            PopulateCompanyProfile();
            tabCompanyAddress_TelephoneFax.SelectedTabPage = tabPageCompanyAddress;
        }

        #endregion

        #region Warehouse Methods

        private void PopulateWarehouses()
        {
            grdWarehouse.DataSource = warehouseBL.GetData();
            warehouseBL.HasData();
        }

        private void LoadWarehouseData()
        {
            txtWarehouseName.Text = warehouseData.Name;
            txtWarehouseDescription.Text = warehouseData.Description;
            chkWarehouseIsActive.Checked = warehouseData.IsActive? true: false;

            if (warehouseData.AddressID != Guid.Empty)
            {
                var _addressData = addressBL.GetAddress((Guid)warehouseData.AddressID);

                //displays Address info
                txtWarehouseHouseNo.Text = _addressData.HouseNo;
                txtWarehouseKebele.Text = _addressData.Kebele;
                txtWarehouseWoreda.Text = _addressData.Woreda;
                txtWarehouseSubcity.Text = _addressData.Subcity;
                txtWarehouseStreet.Text = _addressData.Street;
                txtWarehouseTownCity.Text = _addressData.City_Town;
                txtWarehouseRegionState.Text = _addressData.State_Region;
                lkeWarehouseCountry.EditValue = _addressData.Country;
                txtWarehousePrimaryEmail.Text = _addressData.PrimaryEmail;
                txtWarehouseSecondaryEmail.Text = _addressData.SecondaryEmail;
                txtWarehousePoBox.Text = _addressData.PoBox;
                txtWarehousePostalZipCode.Text = _addressData.ZipCode_PostalCode;
                txtWarehouseAdditionalInfo.Text = _addressData.AdditionalInfo;
            }
            else //leaves the controls blank
            {
                txtWarehouseHouseNo.Text = txtWarehouseKebele.Text = txtWarehouseWoreda.Text = txtWarehouseSubcity.Text = txtWarehouseStreet.Text = txtWarehouseTownCity.Text = txtWarehouseRegionState.Text =
                txtWarehousePrimaryEmail.Text = txtWarehouseSecondaryEmail.Text = txtWarehousePoBox.Text = txtWarehousePostalZipCode.Text = txtWarehouseAdditionalInfo.Text = String.Empty;
                lkeWarehouseCountry.EditValue = 0;
            }

            var _telephoneFax = telephoneFaxBL.GetTelephoneFax(warehouseData.WarehouseID);

            grdWarehouseTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(_telephoneFax);
        }

        private void PopulateActiveNotDefaultWarehouses()
        {
            lkeWarehouses.Properties.DataSource = warehouseBL.GetActiveNotDefaultWarehouses();
        }

        private void RefreshWarehouseData()
        {
            SetControlsReadOnly(layoutControl5, true);
            SetControlsReadOnly(layoutControl7, true);

            EnableDisableButtons(layoutControl5, "Load");
            EnableDisableTabs(true);
            PopulateWarehouses();
            PopulateActiveNotDefaultBranches();
            tabWarehouseAddress_TelephoneFax.SelectedTabPage = tabPageWarehouseAddress;
        }

        #endregion

        #region Branch Methods

        private void PopulateBranches()
        {
            grdBranch.DataSource = branchBL.GetData();
            branchBL.HasData();
        }

        private void LoadBranchData()
        {
            txtBranchName.Text = branchData.Name;
            txtBranchDescription.Text = branchData.Description;
            chkBranchIsActive.Checked = branchData.IsActive ? true : false;

            if (branchData.AddressID != Guid.Empty)
            {
                var _addressData = addressBL.GetAddress((Guid)branchData.AddressID);

                //displays Address info
                txtBranchHouseNo.Text = _addressData.HouseNo;
                txtBranchKebele.Text = _addressData.Kebele;
                txtBranchWoreda.Text = _addressData.Woreda;
                txtBranchSubcity.Text = _addressData.Subcity;
                txtBranchStreet.Text = _addressData.Street;
                txtBranchTownCity.Text = _addressData.City_Town;
                txtBranchRegionState.Text = _addressData.State_Region;
                lkeBranchCountry.EditValue = _addressData.Country;
                txtBranchPrimaryEmail.Text = _addressData.PrimaryEmail;
                txtBranchSecondaryEmail.Text = _addressData.SecondaryEmail;
                txtBranchPoBox.Text = _addressData.PoBox;
                txtBranchPostalZipCode.Text = _addressData.ZipCode_PostalCode;
                txtBranchAdditionalInfo.Text = _addressData.AdditionalInfo;
            }
            else //leaves the controls blank
            {
                txtBranchHouseNo.Text = txtBranchKebele.Text = txtBranchWoreda.Text = txtBranchSubcity.Text = txtBranchStreet.Text = txtBranchTownCity.Text = txtBranchRegionState.Text =
                txtBranchPrimaryEmail.Text = txtBranchSecondaryEmail.Text = txtBranchPoBox.Text = txtBranchPostalZipCode.Text = txtBranchAdditionalInfo.Text = String.Empty;
                lkeBranchCountry.EditValue = 0;
            }

            var _telephoneFax = telephoneFaxBL.GetTelephoneFax(branchData.ID);

            grdBranchTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(_telephoneFax);
        }

        private void PopulateActiveNotDefaultBranches()
        {
            lkeBranch.Properties.DataSource = branchBL.GetActiveNotDefaultBranches();
        }

        private void RefreshBranchData()
        {
            SetControlsReadOnly(layoutControl6, true);
            SetControlsReadOnly(layoutControl9, true);

            EnableDisableButtons(layoutControl6, "Load");
            EnableDisableTabs(true);
            PopulateBranches();
            PopulateActiveNotDefaultBranches();
            tabBranchAddress_TelephoneFax.SelectedTabPage = tabPageBranchAddress;
        }

        #endregion

        #region Common Methods

        public void ClearControlValues(LayoutControl controlCollection)
        {
            foreach (var ctrl in controlCollection.Controls)
            {
                if (ctrl is TextEdit)
                {
                    TextEdit txt = ctrl as TextEdit;
                    bool _lkeDontDisable = txt.Tag != null ? false : true;
                
                    if (txt != null && _lkeDontDisable)
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
                    bool _lkeDontDisable = lke.Tag != null ? false : true;

                    if (lke != null && _lkeDontDisable)
                        lke.EditValue = null;
                }
                else if (ctrl is PictureEdit)
                {
                    PictureEdit pic = ctrl as PictureEdit;
                    if (pic != null)
                        pic.Image = null;
                }

                dxErrorProvider.ClearErrors();
            }
        }

        private void SetControlsReadOnly(LayoutControl layoutControl, bool val)
        {
            foreach (var ctrl in layoutControl.Controls)
            {
                if (ctrl is TextEdit)
                {
                    TextEdit txt = ctrl as TextEdit;
                    bool _lkeDontDisable = txt.Tag != null ? false : true;

                    if (txt != null && _lkeDontDisable)
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
                    bool _lkeDontDisable = lke.Tag != null ? false : true;

                    if (lke != null && _lkeDontDisable)
                        lke.Properties.ReadOnly = val;
                }

                else if (ctrl is DateEdit)
                {
                    DateEdit dt = ctrl as DateEdit;
                    if (dt != null)
                        dt.Properties.ReadOnly = val;
                }

                else if (ctrl is PictureEdit)
                {
                    PictureEdit pic = ctrl as PictureEdit;
                    if (pic != null)
                        pic.Properties.ReadOnly = val;
                }
            }

            if (tabCompanyInfo.SelectedTabPage == tabPageCompany)
            {
                grdCompany.Enabled = val;
                grdCompanyTelephoneFax.Enabled = !val;
            }
            else if (tabCompanyInfo.SelectedTabPage == tabPageBranch)
            {
                grdBranch.Enabled = val;
                grdBranchTelephoneFax.Enabled = !val;
            }
            else if (tabCompanyInfo.SelectedTabPage == tabPageWarehouse)
            {
                grdWarehouse.Enabled = val;
                grdWarehouseTelephoneFax.Enabled = !val;
            }
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
                        bool btnLogo = btn.Tag == null ? false : true;

                        switch (action)
                        {
                            case "Add":
                                if (btn.Text.Equals("&Save") || btn.Text.Equals("&Cancel") || btnLogo)
                                    btn.Enabled = true;

                                if (btn.Text.Equals("&Delete") || btn.Text.Equals("&Edit") || btn.Text.Contains("&Add "))
                                    btn.Enabled = false;

                                break;
                            case "Edit":
                                if (btn.Text.Contains("&Save") || btn.Text.Contains("&Cancel") || btn.Text.Contains("&Delete") || btnLogo)
                                    btn.Enabled = true;
                                if (btn.Text.Contains("&Edit") || btn.Text.Contains("&Add "))
                                    btn.Enabled = false;

                                break;

                            case "Load":

                                if (btn.Text.Contains("&Save") || btn.Text.Contains("&Delete") || btn.Text.Contains("&Cancel") || btnLogo)
                                    btn.Enabled = false;

                                if (btn.Text.Contains("&Edit") || btn.Text.Contains("&Add "))
                                    btn.Enabled = true;

                                break;
                            case "Disable All":

                                if (btn.Text.Contains("&Save") || btn.Text.Contains("&Edit") || btn.Text.Contains("&Delete") || btn.Text.Contains("&Cancel") || btnLogo)
                                    btn.Enabled = false;
                                if (btn.Text.Contains("&Add "))
                                    btn.Enabled = true;
                                break;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// This method enables/disables tabs when adding new records
        /// </summary>
        /// <param name="enableAll">if true, enables all tabs</param>
        private void EnableDisableTabs(bool enableAll = false)
        {
            if (enableAll)
                tabPageCompany.PageEnabled = tabPageWarehouse.PageEnabled = tabPageBranch.PageEnabled = true;
            else
            {
                if (tabCompanyInfo.SelectedTabPage == tabPageCompany)
                    tabPageWarehouse.PageEnabled = tabPageBranch.PageEnabled = false;

                else if (tabCompanyInfo.SelectedTabPage == tabPageWarehouse)
                    tabPageCompany.PageEnabled = tabPageBranch.PageEnabled = false;

                else if (tabCompanyInfo.SelectedTabPage == tabPageBranch)
                    tabPageCompany.PageEnabled = tabPageWarehouse.PageEnabled = false; 
            }
        }

        private void PopulateTelephoneFaxType()
        {
            repLkeCompanyTelephoneFaxType.DataSource = repLkeWarehouseTelephoneFaxType.DataSource = repLkeBranchTelephoneFaxType.DataSource = telephoneFaxTypeBL.GetData(true);
        }

        private void PopulateCountries()
        {
            try
            {
                lkeCompanyCountry.Properties.DataSource = lkeWarehouseCountry.Properties.DataSource = lkeBranchCountry.Properties.DataSource = GimjaBL.CommonBL.GetCountriesList();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Company Profile Event Handlers

        private void btnCompanyAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl1);
                ClearControlValues(layoutControl2);
                ClearControlValues(layoutControl3);

                EnableDisableButtons(layoutControl2, "Add");
                EnableDisableTabs();

                SetControlsReadOnly(layoutControl1, false);
                SetControlsReadOnly(layoutControl2, false);
                SetControlsReadOnly(layoutControl3, false);

                companyProfileBL.IsUpdate = false;

                //sets role ID to empty
                companyProfileData.ID = String.Empty;

                //sets address ID to empty so that a new Guid can be generated while saving the data
                companyProfileData.AddressID = Guid.Empty;

                //tempTelephoneFaxList has to be reset
                tempTelephoneFaxList = new List<TelephoneFaxData>();

                grdCompanyTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(tempTelephoneFaxList);

                txtCompanyAmharicName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        private void btnCompanySave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtCompanyEnglishName.Text) &&
                !String.IsNullOrWhiteSpace(txtCompanyAmharicName.Text) &&
                !String.IsNullOrWhiteSpace(txtCompanyTINNo.Text) &&
                !String.IsNullOrWhiteSpace(txtCompanyHouseNo.Text) &&
                !String.IsNullOrWhiteSpace(txtCompanyWoreda.Text) &&
                !String.IsNullOrWhiteSpace(txtCompanySubcity.Text) &&
                !String.IsNullOrWhiteSpace(txtCompanyTownCity.Text) &&
                !String.IsNullOrWhiteSpace(txtCompanyRegionState.Text) &&
                !lkeCompanyCountry.EditValue.Equals(0))
                {
                    //sets company profile id
                    var _companyProfileID = companyProfileData.ID == String.Empty ? String.Empty : companyProfileData.ID;

                    //generates address ID only if the operation is to add a new address record
                    var _addressID = companyProfileData.AddressID == Guid.Empty ? Guid.NewGuid() : companyProfileData.AddressID;

                    companyProfileData = new CompanyProfileData()
                    {
                        ID = _companyProfileID,
                        AmharicName = txtCompanyAmharicName.Text.Trim(),
                        EnglishName = txtCompanyEnglishName.Text.Trim(),
                        TINNumber = txtCompanyTINNo.Text.Trim(),
                        VATRegistrationNo = txtCompanyVATRegNo.Text.Trim(),
                        Logo = companyLogo,
                        AddressID = _addressID
                    };

                    if(dtCompanyVATRegDate.EditValue != null )
                        companyProfileData.VATRegistrationDate = dtCompanyVATRegDate.DateTime;

                    addressData = new AddressData
                    {
                        ID = (Guid)_addressID,
                        HouseNo = txtCompanyHouseNo.Text,
                        Kebele = txtCompanyKebele.Text,
                        Woreda = txtCompanyWoreda.Text,
                        State_Region = txtCompanyRegionState.Text,
                        Subcity = txtCompanySubcity.Text,
                        Street = txtCompanyStreet.Text,
                        City_Town = txtCompanyTownCity.Text,
                        PoBox = txtCompanyPoBox.Text,
                        PrimaryEmail = txtCompanyPrimaryEmail.Text,
                        SecondaryEmail = txtCompanySecondaryEmail.Text,
                        Country = lkeCompanyCountry.Text,
                        ZipCode_PostalCode = txtCompanyPostalZipCode.Text,
                        AdditionalInfo = txtCompanyAdditionalInfo.Text
                    };

                    companyProfileData.Address = addressData;

                    tempTelephoneFaxList = new List<TelephoneFaxData>();
                    for (int c = 0; c < grdViewCompanyTelephoneFax.RowCount; c++)
                    {
                        if (grdViewCompanyTelephoneFax.GetRowCellValue(c, "Number") != null && grdViewCompanyTelephoneFax.GetRowCellValue(c, "Type") != null)
                        {
                            telephoneFaxData = new TelephoneFaxData()
                            {
                                ID = Convert.ToInt32(grdViewCompanyTelephoneFax.GetRowCellValue(c, "ID")),
                                Number = grdViewCompanyTelephoneFax.GetRowCellValue(c, "Number").ToString(),
                                Type = Convert.ToInt16(grdViewCompanyTelephoneFax.GetRowCellValue(c, "Type")),
                                IsActive = Convert.ToBoolean(grdViewCompanyTelephoneFax.GetRowCellValue(c, "IsActive"))
                            };

                            tempTelephoneFaxList.Add(telephoneFaxData);
                        }
                    }

                    if (tempTelephoneFaxList == null || tempTelephoneFaxList.Count == 0)
                        tempTelephoneFaxList = null;

                    companyProfileData.TelephoneFax = tempTelephoneFaxList;

                    if (companyProfileBL.IsUpdate)
                    {
                        if (companyProfileBL.Update(companyProfileData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshCompanyProfileData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (companyProfileBL.Add(companyProfileData))
                        {
                            GimjaHelper.ShowInformation("Data successfully added");
                            RefreshCompanyProfileData();
                        }
                        else
                            GimjaHelper.ShowError("Data can not successfully added. Please try again");
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    txtCompanyAmharicName.Focus();
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message);
            }
        }

        private void btnCompanyEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl1, false);
            SetControlsReadOnly(layoutControl2, false);
            SetControlsReadOnly(layoutControl3, false);

            EnableDisableButtons(layoutControl2, "Edit");

            EnableDisableTabs();

            companyProfileBL.IsUpdate = true;

            //sets tempTelephoneFax to null
            tempTelephoneFaxList = new List<TelephoneFaxData>();

            txtCompanyEnglishName.Focus();
        }

        private void btnCompanyCancel_Click(object sender, EventArgs e)
        {
            if (companyProfileBL.IsDataAvailable)
            {
                grdViewCompany_RowCellClick(null, null);
                EnableDisableButtons(layoutControl2, "Load");
            }
            else
            {
                ClearControlValues(layoutControl1);
                EnableDisableButtons(layoutControl2, "Disable All");
            }

            EnableDisableTabs(true);
            dxErrorProvider.ClearErrors();
            SetControlsReadOnly(layoutControl1, true);
        }

        private void grdViewCompany_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (companyProfileBL.IsDataAvailable)
                {
                    companyProfileData = new CompanyProfileData()
                    {
                        ID = grdViewCompany.GetRowCellValue(grdViewCompany.FocusedRowHandle, "ID").ToString(),
                        AmharicName = grdViewCompany.GetRowCellValue(grdViewCompany.FocusedRowHandle, "AmharicName").ToString(),
                        EnglishName = grdViewCompany.GetRowCellValue(grdViewCompany.FocusedRowHandle, "EnglishName").ToString(),
                        TINNumber = grdViewCompany.GetRowCellValue(grdViewCompany.FocusedRowHandle, "TINNumber").ToString(),
                        VATRegistrationNo = grdViewCompany.GetRowCellValue(grdViewCompany.FocusedRowHandle, "VATRegistrationNo").ToString(),
                        Logo = (byte[])grdViewCompany.GetRowCellValue(grdViewCompany.FocusedRowHandle, "Logo"),
                        AddressID = grdViewCompany.GetRowCellValue(grdViewCompany.FocusedRowHandle, "AddressID") != null ? Guid.Parse(grdViewCompany.GetRowCellValue(grdViewCompany.FocusedRowHandle, "AddressID").ToString()) : Guid.Empty
                    };

                    if (grdViewCompany.GetRowCellValue(grdViewCompany.FocusedRowHandle, "VATRegistrationDate") != null)
                        companyProfileData.VATRegistrationDate = Convert.ToDateTime(grdViewCompany.GetRowCellValue(grdViewCompany.FocusedRowHandle, "VATRegistrationDate").ToString());

                    LoadCompanyProfileData();

                    EnableDisableButtons(layoutControl2, "Load");
                }
                else
                {
                    companyProfileData = new CompanyProfileData();
                    EnableDisableButtons(layoutControl2, "Disable All");
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

        private void btnCompanyLogoAdd_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.ico; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.ico; *.png";
            openFileDialog.FileName = "";
            openFileDialog.ShowDialog();

            string strFileName = openFileDialog.FileName;

            if (!strFileName.Equals(""))
            {
                try
                {
                    picComapnyLogo.Image = Image.FromFile(strFileName);

                    FileInfo fiImage = new FileInfo(strFileName);
                    long photoFileLength = fiImage.Length;
                    FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    
                    companyLogo = new byte[Convert.ToInt32(photoFileLength)];
                    
                    int ibytesread = fs.Read(companyLogo, 0, Convert.ToInt32(photoFileLength));

                    fs.Close();
                }
                catch (Exception ex)
                {
                    GimjaHelper.ShowError(ex.Message, Application.ProductName);
                }
            }
            else
                companyLogo = null;

        }

        #endregion

        private void tabCompanyInfo_Click(object sender, EventArgs e)
        {
            if (tabCompanyInfo.SelectedTabPage == tabPageCompany && btnCompanyAdd.Enabled)
                grdViewCompany_RowCellClick(null, null);
            else if (tabCompanyInfo.SelectedTabPage == tabPageWarehouse && btnWarehouseAdd.Enabled)
                grdViewWarehouse_RowCellClick(null, null);
            else if (tabCompanyInfo.SelectedTabPage == tabPageBranch && btnBranchAdd.Enabled)
                grdViewBranch_RowCellClick(null, null);
        }

        #region Warehouse Event Handlers

        private void btnWarehouseAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl5);
                ClearControlValues(layoutControl7);

                EnableDisableButtons(layoutControl5, "Add");
                EnableDisableTabs();

                SetControlsReadOnly(layoutControl5, false);
                SetControlsReadOnly(layoutControl7, false);

                warehouseBL.IsUpdate = false;

                //sets warehouse ID to empty
                warehouseData.WarehouseID = String.Empty;

                //sets address ID to empty so that a new Guid can be generated while saving the data
                warehouseData.AddressID = Guid.Empty;

                //tempTelephoneFaxList has to be reset
                tempTelephoneFaxList = new List<TelephoneFaxData>();

                grdWarehouseTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(tempTelephoneFaxList);

                txtWarehouseName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void btnWarehouseSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtWarehouseName.Text) &&
                !String.IsNullOrWhiteSpace(txtWarehouseWoreda.Text) &&
                !String.IsNullOrWhiteSpace(txtWarehouseHouseNo.Text) &&
                !String.IsNullOrWhiteSpace(txtWarehouseSubcity.Text) &&
                !String.IsNullOrWhiteSpace(txtWarehouseTownCity.Text) &&
                !String.IsNullOrWhiteSpace(txtWarehouseRegionState.Text) &&
                !lkeWarehouseCountry.EditValue.Equals(0))
                {
                    //sets warehouse id
                    var _warehouseID = warehouseData.WarehouseID == String.Empty ? String.Empty : warehouseData.WarehouseID;

                    //generates address ID only if the operation is to add a new address record
                    var _addressID = warehouseData.AddressID == Guid.Empty ? Guid.NewGuid() : warehouseData.AddressID;

                    warehouseData = new WarehouseData()
                    {
                        WarehouseID = _warehouseID,
                        Name = txtWarehouseName.Text.Trim(),
                        Description= txtWarehouseDescription.Text.Trim(),
                        IsActive = chkWarehouseIsActive.CheckState == CheckState.Checked? true:false,
                        AddressID = _addressID
                    };

                    addressData = new AddressData
                    {
                        ID = (Guid)_addressID,
                        HouseNo = txtWarehouseHouseNo.Text,
                        Kebele = txtWarehouseKebele.Text,
                        Woreda = txtWarehouseWoreda.Text,
                        State_Region = txtWarehouseRegionState.Text,
                        Subcity = txtWarehouseSubcity.Text,
                        Street = txtWarehouseStreet.Text,
                        City_Town = txtWarehouseTownCity.Text,
                        PoBox = txtWarehousePoBox.Text,
                        PrimaryEmail = txtWarehousePrimaryEmail.Text,
                        SecondaryEmail = txtWarehouseSecondaryEmail.Text,
                        Country = lkeWarehouseCountry.Text,
                        ZipCode_PostalCode = txtWarehousePostalZipCode.Text,
                        AdditionalInfo = txtWarehouseAdditionalInfo.Text
                    };

                    warehouseData.Address = addressData;

                    tempTelephoneFaxList = new List<TelephoneFaxData>();
                    for (int c = 0; c < grdViewWarehouseTelephoneFax.RowCount; c++)
                    {
                        if (grdViewWarehouseTelephoneFax.GetRowCellValue(c, "Number") != null && grdViewWarehouseTelephoneFax.GetRowCellValue(c, "Type") != null)
                        {
                            telephoneFaxData = new TelephoneFaxData()
                            {
                                ID = Convert.ToInt32(grdViewWarehouseTelephoneFax.GetRowCellValue(c, "ID")),
                                Number = grdViewWarehouseTelephoneFax.GetRowCellValue(c, "Number").ToString(),
                                Type = Convert.ToInt16(grdViewWarehouseTelephoneFax.GetRowCellValue(c, "Type")),
                                IsActive = Convert.ToBoolean(grdViewWarehouseTelephoneFax.GetRowCellValue(c, "IsActive"))
                            };

                            tempTelephoneFaxList.Add(telephoneFaxData);
                        }
                    }

                    if (tempTelephoneFaxList == null || tempTelephoneFaxList.Count == 0)
                        tempTelephoneFaxList = null;

                    warehouseData.TelephoneFax = tempTelephoneFaxList;

                    if (warehouseBL.IsUpdate)
                    {
                        if (warehouseBL.Update(warehouseData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshWarehouseData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (warehouseBL.IsValid(warehouseData.Name))
                        {
                            if (warehouseBL.Add(warehouseData))
                            {
                                GimjaHelper.ShowInformation("Data successfully added");
                                RefreshWarehouseData();
                            }
                            else
                                GimjaHelper.ShowError("Data can not successfully added. Please try again");
                        }
                        else
                            GimjaHelper.ShowError(String.Format("A warehouse by the name <{0}> already exists. Please try again", warehouseData.Name));
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    txtWarehouseName.Focus();
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message);
            }
        }

        private void btnWarehouseEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl5, false);
            SetControlsReadOnly(layoutControl7, false);

            EnableDisableButtons(layoutControl5, "Edit");

            EnableDisableTabs();

            warehouseBL.IsUpdate = true;

            //sets tempTelephoneFax to null
            tempTelephoneFaxList = new List<TelephoneFaxData>();

            txtWarehouseName.Focus();
        }

        private void btnWarehouseDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult _ans = GimjaHelper.ShowQuestion("Are you sure you want to delete the data?");
                if (_ans == DialogResult.Yes)
                {
                    if (warehouseBL.Delete(warehouseData.WarehouseID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateWarehouses();
                        PopulateActiveNotDefaultWarehouses();

                        if (warehouseBL.IsDataAvailable)
                        {
                            grdViewWarehouse_RowCellClick(null, null);
                            EnableDisableButtons(layoutControl1, "Load");
                        }
                        else
                        {
                            ClearControlValues(layoutControl1);
                            EnableDisableButtons(layoutControl1, "Disable All");
                        }

                        SetControlsReadOnly(layoutControl5, true);
                        SetControlsReadOnly(layoutControl7, true);
                    }
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be deleted. \n" + ex.Message);
            }
        }

        private void btnWarehouseCancel_Click(object sender, EventArgs e)
        {
            if (warehouseBL.IsDataAvailable)
            {
                grdViewWarehouse_RowCellClick(null, null);
                EnableDisableButtons(layoutControl7, "Load");
            }
            else
            {
                ClearControlValues(layoutControl7);
                EnableDisableButtons(layoutControl5, "Disable All");
            }

            dxErrorProvider.ClearErrors();
            EnableDisableTabs(true);
            SetControlsReadOnly(layoutControl5, true);
            SetControlsReadOnly(layoutControl7, true);
        }

        private void grdViewWarehouse_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (warehouseBL.IsDataAvailable)
                {
                    warehouseData = new WarehouseData()
                    {
                        WarehouseID = grdViewWarehouse.GetRowCellValue(grdViewWarehouse.FocusedRowHandle, "WarehouseID").ToString(),
                        Name = grdViewWarehouse.GetRowCellValue(grdViewWarehouse.FocusedRowHandle, "Name").ToString(),
                        Description = grdViewWarehouse.GetRowCellValue(grdViewWarehouse.FocusedRowHandle, "Description")!=null ? grdViewWarehouse.GetRowCellValue(grdViewWarehouse.FocusedRowHandle, "Description").ToString():String.Empty,
                        IsActive = Convert.ToBoolean( grdViewWarehouse.GetRowCellValue(grdViewWarehouse.FocusedRowHandle, "IsActive")),
                        IsDefault = Convert.ToBoolean(grdViewWarehouse.GetRowCellValue(grdViewWarehouse.FocusedRowHandle, "IsDefault")),
                        AddressID = grdViewWarehouse.GetRowCellValue(grdViewWarehouse.FocusedRowHandle, "AddressID") != null ? Guid.Parse(grdViewWarehouse.GetRowCellValue(grdViewWarehouse.FocusedRowHandle, "AddressID").ToString()) : Guid.Empty
                    };

                    LoadWarehouseData();

                    EnableDisableButtons(layoutControl5, "Load");
                }
                else
                {
                    warehouseData = new WarehouseData();
                    EnableDisableButtons(layoutControl5, "Disable All");
                }

                SetControlsReadOnly(layoutControl5, true);
                SetControlsReadOnly(layoutControl7, true);
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message);
            }
        }

        private void btnWarehouseSetDefault_Click(object sender, EventArgs e)
        {
            try
            {
                string _warehouseID = lkeWarehouses.EditValue.ToString();
                if (String.IsNullOrWhiteSpace(_warehouseID) || String.IsNullOrEmpty(_warehouseID))
                    GimjaHelper.ShowWarning("Please select a warehouse to set to default");
                else
                {
                    if (warehouseBL.SetDefault(_warehouseID))
                    {
                        PopulateActiveNotDefaultWarehouses();
                        PopulateWarehouses();
                    }
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Error occurred while setting a default warehoouse. Please try again");
            }
        }

        #endregion

        #region Branch Event Handlers

        private void btnBranchAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl6);
                ClearControlValues(layoutControl9);

                EnableDisableButtons(layoutControl6, "Add");
                EnableDisableTabs();

                SetControlsReadOnly(layoutControl6, false);
                SetControlsReadOnly(layoutControl9, false);

                branchBL.IsUpdate = false;

                //sets branch ID to empty
                branchData.ID = String.Empty;

                //sets address ID to empty so that a new Guid can be generated while saving the data
                branchData.AddressID = Guid.Empty;

                //tempTelephoneFaxList has to be reset
                tempTelephoneFaxList = new List<TelephoneFaxData>();

                grdBranchTelephoneFax.DataSource = new BindingList<TelephoneFaxData>(tempTelephoneFaxList);

                txtBranchName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void btnBranchSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtBranchName.Text) &&
                !String.IsNullOrWhiteSpace(txtBranchHouseNo.Text) &&
                !String.IsNullOrWhiteSpace(txtBranchWoreda.Text) &&
                !String.IsNullOrWhiteSpace(txtBranchSubcity.Text) &&
                !String.IsNullOrWhiteSpace(txtBranchTownCity.Text) &&
                !String.IsNullOrWhiteSpace(txtBranchRegionState.Text) &&
                !lkeBranchCountry.EditValue.Equals(0))
                {
                    //sets branch id
                    var _branchID = branchData.ID == String.Empty ? String.Empty : branchData.ID;

                    //generates address ID only if the operation is to add a new address record
                    var _addressID = branchData.AddressID == Guid.Empty ? Guid.NewGuid() : branchData.AddressID;

                    branchData = new BranchData()
                    {
                        ID = _branchID,
                        Name = txtBranchName.Text.Trim(),
                        Description = txtBranchDescription.Text.Trim(),
                        IsActive = chkBranchIsActive.CheckState == CheckState.Checked ? true : false,
                        AddressID = _addressID
                    };

                    addressData = new AddressData
                    {
                        ID = (Guid)_addressID,
                        HouseNo = txtBranchHouseNo.Text,
                        Kebele = txtBranchKebele.Text,
                        Woreda = txtBranchWoreda.Text,
                        State_Region = txtBranchRegionState.Text,
                        Subcity = txtBranchSubcity.Text,
                        Street = txtBranchStreet.Text,
                        City_Town = txtBranchTownCity.Text,
                        PoBox = txtBranchPoBox.Text,
                        PrimaryEmail = txtBranchPrimaryEmail.Text,
                        SecondaryEmail = txtBranchSecondaryEmail.Text,
                        Country = lkeBranchCountry.Text,
                        ZipCode_PostalCode = txtBranchPostalZipCode.Text,
                        AdditionalInfo = txtBranchAdditionalInfo.Text
                    };

                    branchData.Address = addressData;

                    tempTelephoneFaxList = new List<TelephoneFaxData>();
                    for (int c = 0; c < grdViewBranchTelephoneFax.RowCount; c++)
                    {
                        if (grdViewBranchTelephoneFax.GetRowCellValue(c, "Number") != null && grdViewBranchTelephoneFax.GetRowCellValue(c, "Type") != null)
                        {
                            telephoneFaxData = new TelephoneFaxData()
                            {
                                ID = Convert.ToInt32(grdViewBranchTelephoneFax.GetRowCellValue(c, "ID")),
                                Number = grdViewBranchTelephoneFax.GetRowCellValue(c, "Number").ToString(),
                                Type = Convert.ToInt16(grdViewBranchTelephoneFax.GetRowCellValue(c, "Type")),
                                IsActive = Convert.ToBoolean(grdViewBranchTelephoneFax.GetRowCellValue(c, "IsActive"))
                            };

                            tempTelephoneFaxList.Add(telephoneFaxData);
                        }
                    }

                    if (tempTelephoneFaxList == null || tempTelephoneFaxList.Count == 0)
                        tempTelephoneFaxList = null;

                    branchData.TelephoneFax = tempTelephoneFaxList;

                    if (branchBL.IsUpdate)
                    {
                        if (branchBL.Update(branchData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshBranchData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (branchBL.IsValid(branchData.Name))
                        {
                            if (branchBL.Add(branchData))
                            {
                                GimjaHelper.ShowInformation("Data successfully added");
                                RefreshBranchData();
                            }
                            else
                                GimjaHelper.ShowError("Data can not successfully added. Please try again");
                        }
                        else
                            GimjaHelper.ShowError(String.Format("A branch by the name <{0}> already exists. Please try again", branchData.Name));
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    txtBranchName.Focus();
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message);
            }
        }

        private void btnBranchEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl6, false);
            SetControlsReadOnly(layoutControl9, false);

            EnableDisableButtons(layoutControl6, "Edit");

            EnableDisableTabs();

            branchBL.IsUpdate = true;

            //sets tempTelephoneFax to null
            tempTelephoneFaxList = new List<TelephoneFaxData>();

            txtBranchName.Focus();
        }

        private void btnBranchDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult _ans = GimjaHelper.ShowQuestion("Are you sure you want to delete the data?");
                if (_ans == DialogResult.Yes)
                {
                    if (branchBL.Delete(branchData.ID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateBranches();
                        PopulateActiveNotDefaultBranches();

                        if (branchBL.IsDataAvailable)
                        {
                            grdViewBranch_RowCellClick(null, null);
                            EnableDisableButtons(layoutControl1, "Load");
                        }
                        else
                        {
                            ClearControlValues(layoutControl1);
                            EnableDisableButtons(layoutControl1, "Disable All");
                        }

                        SetControlsReadOnly(layoutControl6, true);
                        SetControlsReadOnly(layoutControl9, true);
                    }
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be deleted. \n" + ex.Message);
            }
        }

        private void btnBranchCancel_Click(object sender, EventArgs e)
        {
            if (branchBL.IsDataAvailable)
            {
                grdViewBranch_RowCellClick(null, null);
                EnableDisableButtons(layoutControl9, "Load");
            }
            else
            {
                ClearControlValues(layoutControl9);
                EnableDisableButtons(layoutControl6, "Disable All");
            }

            dxErrorProvider.ClearErrors();
            EnableDisableTabs(true);
            SetControlsReadOnly(layoutControl6, true);
            SetControlsReadOnly(layoutControl9, true);
        }

        private void grdViewBranch_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (branchBL.IsDataAvailable)
                {
                    branchData = new BranchData()
                    {
                        ID = grdViewBranch.GetRowCellValue(grdViewBranch.FocusedRowHandle, "ID").ToString(),
                        Name = grdViewBranch.GetRowCellValue(grdViewBranch.FocusedRowHandle, "Name").ToString(),
                        Description = grdViewBranch.GetRowCellValue(grdViewBranch.FocusedRowHandle, "Description") != null ? grdViewBranch.GetRowCellValue(grdViewBranch.FocusedRowHandle, "Description").ToString() : String.Empty,
                        IsActive = Convert.ToBoolean(grdViewBranch.GetRowCellValue(grdViewBranch.FocusedRowHandle, "IsActive")),
                        IsDefault = Convert.ToBoolean(grdViewBranch.GetRowCellValue(grdViewBranch.FocusedRowHandle, "IsDefault")),
                        AddressID = grdViewBranch.GetRowCellValue(grdViewBranch.FocusedRowHandle, "AddressID") != null ? Guid.Parse(grdViewBranch.GetRowCellValue(grdViewBranch.FocusedRowHandle, "AddressID").ToString()) : Guid.Empty
                    };

                    LoadBranchData();

                    EnableDisableButtons(layoutControl6, "Load");
                }
                else
                {
                    branchData = new BranchData();
                    EnableDisableButtons(layoutControl6, "Disable All");
                }

                SetControlsReadOnly(layoutControl6, true);
                SetControlsReadOnly(layoutControl9, true);
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message);
            }
        }
        
        private void btnBranchSetDefault_Click(object sender, EventArgs e)
        {
            try
            {
                string _branchID = lkeBranch.EditValue.ToString();
                if (String.IsNullOrWhiteSpace(_branchID) || String.IsNullOrEmpty(_branchID))
                    GimjaHelper.ShowWarning("Please select a branch to set to default");
                else
                {
                    if (branchBL.SetDefault(_branchID))
                    {
                        PopulateActiveNotDefaultBranches();
                        PopulateBranches();
                    }
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Error occurred while setting a default warehoouse. Please try again");
            }
        }

        #endregion

    }
}