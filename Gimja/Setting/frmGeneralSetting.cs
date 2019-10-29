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
    public partial class frmGeneralSetting : DevExpress.XtraEditors.XtraForm
    {
        private TelephoneFaxTypeBL telephoneFaxTypeBL;
        private TelephoneFaxTypeData telephoneFaxTypeData;

        private RoleBL roleBL;
        private RoleData roleData;

        private CreditStatusBL creditStatusBL;
        private CreditStatusData creditStatusData;

        private const int MOVE_MENU_UP = -1;
        private const int MOVE_MENU_DOWN = 1;

        public frmGeneralSetting()
        {
            InitializeComponent();

            telephoneFaxTypeBL = new TelephoneFaxTypeBL();
            roleBL = new RoleBL();
            creditStatusBL = new CreditStatusBL();

            telephoneFaxTypeBL.IsUpdate = false;
            roleBL.IsUpdate = false;

            PopulateTelephoneFaxTypes();
            PopulateRoles();
            PopulateCreditStatuses();
            
            grdViewTelephoneFaxType_RowCellClick(null, null);
        }

        #region Methods
        
        private void PopulateTelephoneFaxTypes()
        {
            grdTelephoneFaxType.DataSource = telephoneFaxTypeBL.GetData();
            telephoneFaxTypeBL.HasData();
        }
        
        private void PopulateRoles()
        {
            grdRole.DataSource = roleBL.GetData(false);
            roleBL.HasData();
        }

        private void PopulateCreditStatuses()
        {
            grdCreditStatus.DataSource = creditStatusBL.GetData();
            creditStatusBL.HasData();
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

            if (tabGeneralSetting.SelectedTabPage == tabPageRole)
                grdRole.Enabled = val;
            else if (tabGeneralSetting.SelectedTabPage == tabPageTelephoneFaxType)
                grdTelephoneFaxType.Enabled = val;
            else if (tabGeneralSetting.SelectedTabPage == tabPageCreditStatus)
                grdCreditStatus.Enabled = val;
            
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

        private void LoadTelephoneFaxData()
        {
            txtTelephoneFaxTypeName.Text = telephoneFaxTypeData.Name;
            chkTelephoneFaxTypeIsActive.CheckState = (telephoneFaxTypeData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;
        }

        private void LoadRoleData()
        {
            txtRoleName.Text = roleData.RoleName;
            txtRoleDescription.Text = roleData.Description;
            chkRoleIsActive.CheckState = (roleData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;
        }

        private void LoadCreditStatusData()
        {
            txtCreditStatusName.Text = creditStatusData.Name;
            txtCreditStatusDescription.Text = creditStatusData.Description;
            chkCreditStatusIsActive.CheckState = (creditStatusData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;
        }

        private void RefreshTelephonFaxTypeData()
        {
            SetControlsReadOnly(layoutControl2, true);
            EnableDisableTabs(true);
            EnableDisableButtons(layoutControl2, "Load");
            PopulateTelephoneFaxTypes();
            grdViewTelephoneFaxType_RowCellClick(null, null);
        }

        private void RefreshUserRoleData()
        {
            SetControlsReadOnly(layoutControl1, true);
            SetControlsReadOnly(layoutControl2, true);
            SetControlsReadOnly(layoutControl3, true);

            EnableDisableButtons(layoutControl3, "Load");
            EnableDisableTabs(true);
            PopulateRoles();
            grdViewRole_RowCellClick(null, null);
        }

        private void RefreshCreditStatusData()
        {
            SetControlsReadOnly(layoutControl4, true);

            EnableDisableButtons(layoutControl4, "Load");
            EnableDisableTabs(true);
            PopulateCreditStatuses();
            grdViewCreditStatus_RowCellClick(null, null);
        }

        /// <summary>
        /// This method enables/disables tabs when adding new records
        /// </summary>
        /// <param name="enableAll">if true, enables all tabs</param>
        private void EnableDisableTabs(bool enableAll = false)
        {
            if (enableAll)
                tabPageTelephoneFaxType.PageEnabled = tabPageRole.PageEnabled = tabPageCreditStatus.PageEnabled = true;
            else
            {
                if (tabGeneralSetting.SelectedTabPage == tabPageTelephoneFaxType)
                {
                    tabPageRole.PageEnabled = tabPageCreditStatus.PageEnabled = false;
                }
                else if (tabGeneralSetting.SelectedTabPage == tabPageRole)
                {
                    tabPageTelephoneFaxType.PageEnabled = tabPageCreditStatus.PageEnabled = false;
                }
                else if (tabGeneralSetting.SelectedTabPage == tabPageCreditStatus)
                {
                    tabPageTelephoneFaxType.PageEnabled = tabPageRole.PageEnabled = false;
                }
            }
        }
        
        #endregion

        #region Event Handlers

        private void tabGeneralSetting_Click(object sender, EventArgs e)
        {
            if (tabGeneralSetting.SelectedTabPage == tabPageTelephoneFaxType && btnTelephoneFaxTypeAdd.Enabled)
                grdViewTelephoneFaxType_RowCellClick(null, null);
            else if (tabGeneralSetting.SelectedTabPage == tabPageRole && btnRoleAdd.Enabled)
                grdViewRole_RowCellClick(null, null);
            else if (tabGeneralSetting.SelectedTabPage == tabPageCreditStatus && btnCreditStatusAdd.Enabled)
                grdViewCreditStatus_RowCellClick(null, null);
        }

        #region Telephone/Fax
        
        private void btnTelephoneFaxAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl1);
                ClearControlValues(layoutControl2);
                ClearControlValues(layoutControl3);

                EnableDisableButtons(layoutControl2, "Add");

                SetControlsReadOnly(layoutControl1, false);
                SetControlsReadOnly(layoutControl2, false);
                SetControlsReadOnly(layoutControl3, false);

                EnableDisableTabs();

                telephoneFaxTypeBL.IsUpdate = false;

                //sets customer ID to empty
                telephoneFaxTypeData.TelephoneFaxTypeID = 0;

                txtTelephoneFaxTypeName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void btnTelephoneFaxTypeSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtTelephoneFaxTypeName.Text))
                {
                    #region telephone/fax info
                    //sets user id
                    var _telephoneFaxTypeID = telephoneFaxTypeData.TelephoneFaxTypeID == 0 ? 0 : telephoneFaxTypeData.TelephoneFaxTypeID;

                    telephoneFaxTypeData = new TelephoneFaxTypeData()
                    {
                        TelephoneFaxTypeID = (short)_telephoneFaxTypeID,
                        Name = txtTelephoneFaxTypeName.Text.Trim(),
                        IsActive = (chkTelephoneFaxTypeIsActive.CheckState == CheckState.Checked ? true : false)
                    };

                    #endregion

                    if (telephoneFaxTypeBL.IsUpdate)
                    {
                        if (telephoneFaxTypeBL.Update(telephoneFaxTypeData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshTelephonFaxTypeData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (telephoneFaxTypeBL.IsValid(telephoneFaxTypeData.Name))
                        {
                            if (telephoneFaxTypeBL.Add(telephoneFaxTypeData))
                            {
                                GimjaHelper.ShowInformation("Data successfully added");
                                RefreshTelephonFaxTypeData();
                            }
                            else
                                GimjaHelper.ShowWarning("Data can not successfully added. Please try again");
                        }
                        else
                        {
                            GimjaHelper.ShowWarning(String.Format("The name <{0}> is already taken. Please use a different name", telephoneFaxTypeData.Name));
                            txtTelephoneFaxTypeName.Focus();
                        }
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    txtTelephoneFaxTypeName.Focus();
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message);
            }
        }

        private void btnTelephoneFaxTypeEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl1, false);
            SetControlsReadOnly(layoutControl2, false);
            SetControlsReadOnly(layoutControl3, false);

            EnableDisableButtons(layoutControl2, "Edit");

            EnableDisableTabs();

            telephoneFaxTypeBL.IsUpdate = true;

            txtTelephoneFaxTypeName.Focus();
        }

        private void btnTelephoneFaxTypeDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult _ans = GimjaHelper.ShowQuestion("Are you sure you want to delete the data?");
                if (_ans == DialogResult.Yes)
                {
                    if (telephoneFaxTypeBL.Delete(telephoneFaxTypeData.TelephoneFaxTypeID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted");

                        PopulateTelephoneFaxTypes();

                        if (telephoneFaxTypeBL.IsDataAvailable)
                        {
                            grdViewTelephoneFaxType_RowCellClick(null, null);
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

        private void btnTelephoneFaxTypeCancel_Click(object sender, EventArgs e)
        {
            if (telephoneFaxTypeBL.IsDataAvailable)
            {
                grdViewTelephoneFaxType_RowCellClick(null, null);
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

        private void grdViewTelephoneFaxType_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (telephoneFaxTypeBL.IsDataAvailable)
                {
                    telephoneFaxTypeData = new TelephoneFaxTypeData()
                    {
                        TelephoneFaxTypeID = Convert.ToInt16(grdViewTelephoneFaxType.GetRowCellValue(grdViewTelephoneFaxType.FocusedRowHandle, "TelephoneFaxTypeID").ToString()),
                        Name = grdViewTelephoneFaxType.GetRowCellValue(grdViewTelephoneFaxType.FocusedRowHandle, "Name").ToString(),
                        IsActive = Convert.ToBoolean(grdViewTelephoneFaxType.GetRowCellValue(grdViewTelephoneFaxType.FocusedRowHandle, "IsActive")),
                    };

                    LoadTelephoneFaxData();

                    EnableDisableButtons(layoutControl1, "Load");
                }
                else
                {
                    telephoneFaxTypeData = new TelephoneFaxTypeData();
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
        
        private void txtTelephoneFaxTypeName_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtTelephoneFaxTypeName.Text))
            {
                dxErrorProvider.SetError(txtTelephoneFaxTypeName, "required field");
                txtTelephoneFaxTypeName.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtTelephoneFaxTypeName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        #endregion

        #region User Role

        private void btnRoleAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl1);
                ClearControlValues(layoutControl2);
                ClearControlValues(layoutControl3);

                EnableDisableButtons(layoutControl3, "Add");
                
                EnableDisableTabs();

                SetControlsReadOnly(layoutControl1, false);
                SetControlsReadOnly(layoutControl2, false);
                SetControlsReadOnly(layoutControl3, false);

                roleBL.IsUpdate = false;

                //sets role ID to empty
                roleData.ID = 0;

                txtRoleName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void btnRoleSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtRoleName.Text) && !String.IsNullOrWhiteSpace(txtRoleDescription.Text))
                {
                    //sets role id
                    var _roleID = roleData.ID == 0 ? 0 : roleData.ID;

                    roleData = new RoleData()
                    {
                        ID = _roleID,
                        RoleName = txtRoleName.Text.Trim(),
                        Description = txtRoleDescription.Text.Trim(),
                        IsActive = (chkRoleIsActive.CheckState == CheckState.Checked ? true : false)
                    };

                    if (roleBL.IsUpdate)
                    {
                        if (roleBL.Update(roleData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshUserRoleData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (roleBL.IsValid(txtRoleName.Text.Trim()))
                        {
                            if (roleBL.Add(roleData))
                            {
                                GimjaHelper.ShowInformation("Data successfully added");
                                RefreshUserRoleData();
                            }
                            else
                                GimjaHelper.ShowError("Data can not successfully added. Please try again");
                        }
                        else
                        {
                            GimjaHelper.ShowWarning("Role name already exists");
                            txtRoleName.SelectAll();
                            txtRoleName.Focus();
                        }
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    txtRoleName.Focus();
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message);
            }
        }

        private void btnRoleEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl1, false);
            SetControlsReadOnly(layoutControl2, false);
            SetControlsReadOnly(layoutControl3, false);

            EnableDisableButtons(layoutControl3, "Edit");
            
            EnableDisableTabs();

            roleBL.IsUpdate = true;

            txtRoleName.Focus();
        }

        private void btnRoleDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int _userRecords = roleBL.UserAccountsWithRole(roleData.ID);

                DialogResult _ans = (_userRecords > 0) ? GimjaHelper.ShowQuestion(String.Format("{0} user record(s) with this role will also be deleted. Do you want to continue?", _userRecords)) :
                                                         GimjaHelper.ShowQuestion("Are you sure you want to delete the role?");
                if (_ans == DialogResult.Yes)
                {
                    if (roleBL.Delete(roleData.ID))
                        GimjaHelper.ShowInformation("Data successfully deleted");
                    
                    PopulateRoles();

                    if (roleBL.IsDataAvailable)
                    {
                        grdViewRole_RowCellClick(null, null);
                        EnableDisableButtons(layoutControl3, "Load");
                    }
                    else
                    {
                        ClearControlValues(layoutControl1);
                        EnableDisableButtons(layoutControl3, "Disable All");
                    }

                    EnableDisableTabs(true);
                    SetControlsReadOnly(layoutControl1, true);
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be deleted. \n" + ex.Message);
            }
        }

        private void btnRoleCancel_Click(object sender, EventArgs e)
        {
            if (roleBL.IsDataAvailable)
            {
                grdViewRole_RowCellClick(null, null);
                EnableDisableButtons(layoutControl3, "Load");
            }
            else
            {
                ClearControlValues(layoutControl1);
                EnableDisableButtons(layoutControl3, "Disable All");
            }

            EnableDisableTabs(true);
            dxErrorProvider.ClearErrors();
            SetControlsReadOnly(layoutControl1, true);
        }

        private void grdViewRole_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                //if (roleBL.IsDataAvailable)
                //{
                    roleData = grdViewRole.GetFocusedRow() as RoleData;
                if(roleData !=null)
                {
                //roleData = new RoleData()
                    //{
                    //    ID = Convert.ToInt32(grdViewRole.GetRowCellValue(grdViewRole.FocusedRowHandle, "ID").ToString()),
                    //    RoleName = grdViewRole.GetRowCellValue(grdViewRole.FocusedRowHandle, "RoleName").ToString(),
                    //    Priority = Convert.ToInt16(grdViewRole.GetRowCellValue(grdViewRole.FocusedRowHandle, "Priority").ToString()),
                    //    Description = grdViewRole.GetRowCellValue(grdViewRole.FocusedRowHandle, "Description").ToString(),
                    //    IsActive = Convert.ToBoolean(grdViewRole.GetRowCellValue(grdViewRole.FocusedRowHandle, "IsActive")),
                    //};

                    LoadRoleData();

                    EnableDisableButtons(layoutControl3, "Load");
                    //if (roleData.Priority == 1)
                    //{
                    //    btnUp.Enabled = false;
                    //    btnDown.Enabled = true;
                    //}
                    //else
                    //    btnUp.Enabled = btnDown.Enabled = true;
                }
                else
                {
                    roleData = new RoleData();
                    EnableDisableButtons(layoutControl3, "Disable All");
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

        private void txtRoleName_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtRoleName.Text))
            {
                dxErrorProvider.SetError(txtRoleName, "required field");
                txtRoleName.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtRoleName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void txtRoleDescription_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtRoleDescription.Text))
            {
                dxErrorProvider.SetError(txtRoleDescription, "required field");
                txtRoleDescription.Focus();
            }
            else
                dxErrorProvider.SetErrorType(txtRoleDescription, DevExpress.XtraEditors.DXErrorProvider.ErrorType.None);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            //if (roleBL.UpdateRolePriority(roleData, MOVE_MENU_UP))
            //    PopulateRoles();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            //if (roleBL.UpdateRolePriority(roleData, MOVE_MENU_DOWN))
            //    PopulateRoles();
        }

        #endregion

        #region Credit Status

        private void btnCreditStatusAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl4);

                EnableDisableButtons(layoutControl4, "Add");
                EnableDisableTabs();

                SetControlsReadOnly(layoutControl4, false);

                creditStatusBL.IsUpdate = false;

                //sets creditStatus ID to empty
                creditStatusData.CreditStatusID = 0;

                txtCreditStatusName.Focus();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void btnCreditStatusSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtCreditStatusName.Text) && !String.IsNullOrWhiteSpace(txtCreditStatusDescription.Text))
                {
                    //sets credit status id
                    var _creditStatusID = creditStatusData.CreditStatusID == 0 ? (short)0 : creditStatusData.CreditStatusID;

                    creditStatusData = new CreditStatusData()
                    {
                        CreditStatusID = _creditStatusID,
                        Name = txtCreditStatusName.Text.Trim(),
                        Description = txtCreditStatusDescription.Text.Trim(),
                        IsActive = (chkCreditStatusIsActive.CheckState == CheckState.Checked ? true : false)
                    };

                    if (creditStatusBL.IsUpdate)
                    {
                        if (creditStatusBL.Update(creditStatusData))
                        {
                            GimjaHelper.ShowInformation("Data successfully updated");
                            RefreshCreditStatusData();
                        }
                        else
                            GimjaHelper.ShowWarning("Data can not be successfully updated");
                    }
                    else
                    {
                        if (creditStatusBL.IsValid(txtCreditStatusName.Text.Trim()))
                        {
                            if (creditStatusBL.Add(creditStatusData))
                            {
                                GimjaHelper.ShowInformation("Data successfully added");
                                RefreshCreditStatusData();
                            }
                            else
                                GimjaHelper.ShowError("Data can not successfully added. Please try again");
                        }
                        else
                        {
                            GimjaHelper.ShowWarning("Role name already exists");
                            txtCreditStatusName.SelectAll();
                            txtCreditStatusName.Focus();
                        }
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields");
                    txtCreditStatusName.Focus();
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message);
            }
        }

        private void btnCreditStatusEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl4, false);

            EnableDisableButtons(layoutControl4, "Edit");
            
            EnableDisableTabs();

            creditStatusBL.IsUpdate = true;

            txtCreditStatusName.Focus();
        }

        private void btnCreditStatusDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult _ans = GimjaHelper.ShowQuestion("Are you sure you want to delete the credit status?");
                if (_ans == DialogResult.Yes)
                {
                    if (creditStatusBL.Delete(creditStatusData.CreditStatusID))
                        GimjaHelper.ShowInformation("Data successfully deleted");

                    PopulateCreditStatuses();

                    if (creditStatusBL.IsDataAvailable)
                    {
                        grdViewCreditStatus_RowCellClick(null, null);
                        EnableDisableButtons(layoutControl4, "Load");
                    }
                    else
                    {
                        ClearControlValues(layoutControl1);
                        EnableDisableButtons(layoutControl4, "Disable All");
                    }

                    EnableDisableTabs(true);
                    SetControlsReadOnly(layoutControl1, true);
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be deleted. \n" + ex.Message);
            }
        }

        private void btnCreditStatusCancel_Click(object sender, EventArgs e)
        {
            if (creditStatusBL.IsDataAvailable)
            {
                grdViewCreditStatus_RowCellClick(null, null);
                EnableDisableButtons(layoutControl4, "Load");
            }
            else
            {
                ClearControlValues(layoutControl1);
                EnableDisableButtons(layoutControl4, "Disable All");
            }

            EnableDisableTabs(true);
            dxErrorProvider.ClearErrors();
            SetControlsReadOnly(layoutControl1, true);
        }

        private void grdViewCreditStatus_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (creditStatusBL.IsDataAvailable)
                {
                    creditStatusData = new CreditStatusData()
                    {
                        CreditStatusID = Convert.ToInt16(grdViewCreditStatus.GetRowCellValue(grdViewCreditStatus.FocusedRowHandle, "CreditStatusID").ToString()),
                        Name = grdViewCreditStatus.GetRowCellValue(grdViewCreditStatus.FocusedRowHandle, "Name").ToString(),
                        Description = grdViewCreditStatus.GetRowCellValue(grdViewCreditStatus.FocusedRowHandle, "Description").ToString(),
                        IsActive = Convert.ToBoolean(grdViewCreditStatus.GetRowCellValue(grdViewCreditStatus.FocusedRowHandle, "IsActive")),
                    };

                    LoadCreditStatusData();

                    EnableDisableButtons(layoutControl4, "Load");
                }
                else
                {
                    creditStatusData = new CreditStatusData();
                    EnableDisableButtons(layoutControl4, "Disable All");
                }

                //SetControlsReadOnly(layoutControl1, true);
                //SetControlsReadOnly(layoutControl2, true);
                SetControlsReadOnly(layoutControl4, true);
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message);
            }
        }

        #endregion

        #endregion
    }
}