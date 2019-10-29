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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;

namespace Gimja
{
    public partial class frmMenuSetting : DevExpress.XtraEditors.XtraForm
    {
        private MenuTypeBL menuTypeBL;
        private MenuTypeData menuTypeData;
        private MenuRoleData menuRoleData;
        private FormData formData;

        private MenuBL menuBL;
        private MenuData menuData;

        int menuID;
        private const int MOVE_MENU_UP = -1;
        private const int MOVE_MENU_DOWN = 1;

        private RoleBL roleBL;

        /// <summary>
        /// stores selected Menu IDs from Associate Menu<->Role
        /// </summary>
        private List<int> selectedMenus = new List<int>();

        /// <summary>
        /// selected Menu<->Form IDs to delete
        /// </summary>
        private Dictionary<int, object> selectedMenuForms = new Dictionary<int,object>();

        public frmMenuSetting()
        {
            InitializeComponent();
            menuTypeBL = new MenuTypeBL();
            menuTypeBL.IsMenuTypeUpdate = false;

            menuBL = new MenuBL();
            menuBL.IsMenuUpdate = false;

            roleBL = new RoleBL();

            InitializeMenuType();

            InitializeMenu();

            InitializeMenuRole();

            InitializeMenuForm();

            grdViewMenuType_RowCellClick(null, null);
        }

        #region Common Methods

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
                else if (ctrl is LookUpEdit)
                {
                    LookUpEdit lke = ctrl as LookUpEdit;
                    if (lke != null)
                        lke.EditValue = null;
                }
                else if (ctrl is MemoEdit)
                {
                    MemoEdit txt = ctrl as MemoEdit;
                    if (txt != null)
                        txt.Text = string.Empty;
                }
                else if (ctrl is CheckEdit)
                {
                    CheckEdit chk = ctrl as CheckEdit;
                    if (chk != null)
                        chk.Checked = false;
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
                else if (ctrl is PictureEdit)
                {
                    PictureEdit pic = ctrl as PictureEdit;
                    if (pic != null)
                        pic.Properties.ReadOnly = val;
                }
            }

            grdMenuType.Enabled = val; //grdMenu.Enabled=grd
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
                        bool btnIcon = btn.Tag == null ? false : true;

                        switch (action)
                        {
                            case "Add":
                                if (btn.Text.Equals("&Save") || btn.Text.Equals("&Cancel") || btnIcon)
                                    btn.Enabled = true;

                                if (btn.Text.Equals("&Delete") || btn.Text.Equals("&Edit") || btn.Text.Contains("&Add "))
                                    btn.Enabled = false;

                                break;
                            case "Edit":
                                if (btn.Text.Contains("&Save") || btn.Text.Contains("&Cancel") || btn.Text.Contains("&Delete") || btnIcon)
                                    btn.Enabled = true;
                                if (btn.Text.Contains("&Edit") || btn.Text.Contains("&Add "))
                                    btn.Enabled = false;

                                break;

                            case "Load":

                                if (btn.Text.Contains("&Save") || btn.Text.Contains("&Delete") || btn.Text.Contains("&Cancel") || btnIcon)
                                    btn.Enabled = false;

                                if (btn.Text.Contains("&Edit") || btn.Text.Contains("&Add "))
                                    btn.Enabled = true;

                                break;
                            case "Disable All":

                                if (btn.Text.Contains("&Save") || btn.Text.Contains("&Edit") || btn.Text.Contains("&Delete") || btn.Text.Contains("&Cancel") || btnIcon)
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
                tabPageMenuType.PageEnabled = tabPageMenu.PageEnabled = tabPageRoleMenu.PageEnabled = tabPageAssociateMenuForm.PageEnabled = true;
            else
            {
                if (tabMenuSetting.SelectedTabPage == tabPageMenuType)
                    tabPageMenu.PageEnabled =tabPageRoleMenu.PageEnabled= tabPageAssociateMenuForm.PageEnabled = false;

                else if (tabMenuSetting.SelectedTabPage == tabPageMenu)
                    tabPageMenuType.PageEnabled =tabPageRoleMenu.PageEnabled= tabPageAssociateMenuForm.PageEnabled = false;

                else if (tabMenuSetting.SelectedTabPage == tabPageAssociateMenuForm)
                    tabPageMenuType.PageEnabled = tabPageRoleMenu.PageEnabled =tabPageMenu.PageEnabled = false;
            }
        }

        #endregion

        #region Menu Type Methods

        private void InitializeMenuType()
        {
            PopulateMenuTypeParent();
        }

        private void LoadMenuTypeData()
        {
            ////displays Menu Type info
            txtMenuType.Text = menuTypeData.Type;
            lkeMenuTypeParent.EditValue = menuTypeData.Parent;
            chkMenuTypeIsActive.CheckState = (menuTypeData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;
        }

        private void PopulateMenuTypes()
        {
            grdMenuType.DataSource = menuTypeBL.GetData();
        }

        private void PopulateMenuTypeParent()
        {
            lkeMenuTypeParent.Properties.DataSource = menuTypeBL.GetActiveMenuTypes();
        }

        #endregion

        #region Menu Methods

        private void InitializeMenu()
        {
            PopulateMenus();
            LoadMenuTypes();
            PopulateMenuTypes();
            PopulateParentMenu();
        }

        private void MenuDataAvailable()
        {
            if (menuBL.IsDataAvailable)
            {
                grdViewMenu_RowCellClick(null, null);
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

        private void PopulateMenus(int parentToExpand = 0)
        {
            trMenu.DataSource = menuBL.GetData();
        }

        private void LoadMenuTypes()
        {
            lkeMenuType.Properties.DataSource = menuTypeBL.GetActiveMenuTypes();
        }

        private void LoadMenuData()
        {
            ////displays Menu Type info
            txtMenuCaption.Text = menuData.Caption;
            lkeMenuParent.EditValue = menuData.Parent;
            lkeMenuType.EditValue = menuData.MenuTypeID;
            picMenuIcon.Image = menuData.Icon!=null? Image.FromStream(menuBL.ByteToImage(menuData.Icon)):null;
            chkMenuIsActive.CheckState = (menuData.IsActive == true) ? CheckState.Checked : CheckState.Unchecked;
            chkMenuIsVisible.CheckState = (menuData.Visible == true) ? CheckState.Checked : CheckState.Unchecked;
            chkMenuIsDisabled.CheckState = (menuData.Disabled == true) ? CheckState.Checked : CheckState.Unchecked;
        }

        /// <summary>
        /// Populates lkeMenuParent
        /// </summary>
        private void PopulateParentMenu()
        {
            lkeMenuParent.Properties.DataSource = menuBL.GetActiveParentMenu();
        }

        #endregion

        #region Menu Role

        private void InitializeMenuRole()
        {
            PopulateRole();
        }

        private void PopulateRole()
        {
            grdRole.DataSource = roleBL.GetData(true);
        }

        private void PopulateRoleMenu()
        {
            grdMenuRole.DataSource = menuBL.GetMenuRoles(menuRoleData.RoleID);
        }

        private void GetSelectedMenus()
        {
            selectedMenus.Clear();

            for (int c = 0; c < grdViewMenuRole.RowCount; c++)
            {
                if ((bool)grdViewMenuRole.GetRowCellValue(c, "Applies") == true)
                {
                    selectedMenus.Add((int)grdViewMenuRole.GetRowCellValue(c, "MenuID"));
                }
            }
        }

        #endregion

        #region Menu Form

        private void InitializeMenuForm()
        {
            PopulateMenusForm();

            PopulateForms();

            PopulateMenuForm();
        }

        /// <summary>
        /// populates the Menu<->Form grid
        /// </summary>
        private void PopulateMenuForm()
        {
            grdMenuForm.DataSource = menuBL.GetFormData();
        }

        /// <summary>
        /// Populates the Menu LookupEdit
        /// </summary>
        private void PopulateMenusForm()
        {
            chkCboMenu.Properties.DataSource = menuBL.GetActiveUnassociatedMenu();
        }

        /// <summary>
        /// populate the Forms LookupEdit
        /// </summary>
        private void PopulateForms()
        {
            try
            {
                DataTable _forms = new DataTable();
                _forms.Columns.Add("FormID", typeof(string));
                _forms.Columns.Add ("FormName", typeof(string));

                System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetEntryAssembly();
                Type[] Types = myAssembly.GetTypes();

                _forms.Clear();

                foreach (Type myType in Types)
                {
                    if (myType.BaseType == null) continue;

                    if (myType.BaseType.FullName == "DevExpress.XtraEditors.XtraForm")
                    {
                        //if(!menuBL.IsFormAlreadyAssigned(myType.FullName))
                            _forms.Rows.Add(myType.FullName, myType.Name);
                    }
                }

                _forms.DefaultView.Sort = "FormName asc";
                lkeMenuFormForms.Properties.DataSource = _forms;
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message);
            }
        }

        #endregion

        #region Event Handlers

        private void tabMenuSetting_Click(object sender, EventArgs e)
        {
            LoadMenuTypes();

            if (tabMenuSetting.SelectedTabPage == tabPageMenuType && btnMenuTypeAdd.Enabled)
                trMenu_FocusedNodeChanged(null, null);

            else if (tabMenuSetting.SelectedTabPage == tabPageMenu && btnMenuAdd.Enabled)
                grdViewMenu_RowCellClick(null, null);

            else if (tabMenuSetting.SelectedTabPage == tabPageRoleMenu)
                grdViewRole_RowCellClick(null, null);

            else if (tabMenuSetting.SelectedTabPage == tabPageAssociateMenuForm)
                InitializeMenuForm();
            //    grdViewMenuForm(null, null);
        }

        #region Menu Type

        private void btnAddMenuType_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl2);

                EnableDisableButtons(layoutControl2, "Add");

                SetControlsReadOnly(layoutControl2, false);

                menuTypeBL.IsMenuTypeUpdate = false;

                EnableDisableTabs();

                txtMenuType.Focus();
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message, Application.ProductName);
            }
        }

        private void btnMenuTypeSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtMenuType.Text) && lkeMenuTypeParent.EditValue != null)
                {
                    MenuTypeData _menuTypeData = new MenuTypeData()
                    {
                        Type = txtMenuType.Text.Trim(),
                        Parent = Convert.ToInt32(lkeMenuTypeParent.EditValue),
                        CreatedDate = DateTime.Now,
                        CreatedBy = "LoggedInUser",
                        LastUpdatedDate = DateTime.Now,
                        LastUpdatedBy = "LoggedInUser",
                        IsActive = (chkMenuTypeIsActive.CheckState == CheckState.Checked ? true : false)
                    };

                    if (menuTypeBL.IsMenuTypeUpdate)
                    {
                        _menuTypeData.MenuTypeID = menuTypeData.MenuTypeID;

                        if (menuTypeBL.Update(_menuTypeData))
                            GimjaHelper.ShowInformation("Data successfully updated", Application.ProductName);
                        else
                            GimjaHelper.ShowInformation("Data can not be successfully updated", Application.ProductName);
                    }
                    else
                    {
                        if (menuTypeBL.Validate(txtMenuType.Text.Trim()))
                        {
                            if (menuTypeBL.Add(_menuTypeData))
                                GimjaHelper.ShowInformation("Data successfully added", Application.ProductName);
                            else
                                GimjaHelper.ShowInformation("Data can not successfully added. Please try again", Application.ProductName);
                        }
                        else
                        {
                            GimjaHelper.ShowWarning("Menu type name already exists", Application.ProductName);
                            txtMenuType.SelectAll();
                            return;
                        }
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields", Application.ProductName);
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message, Application.ProductName);
            }
            finally
            {
                SetControlsReadOnly(layoutControl2, true);
                EnableDisableButtons(layoutControl2, "Load");
                EnableDisableTabs(true);
                PopulateMenuTypes();
                grdViewMenuType_RowCellClick(null, null);
            }
        }

        private void grdViewMenuType_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (menuTypeBL.IsDataAvailable)
            {
                menuTypeData = new MenuTypeData()
                {
                    MenuTypeID = Convert.ToInt32(grdViewMenuType.GetRowCellValue(grdViewMenuType.FocusedRowHandle, "MenuTypeID").ToString()),
                    //Parent = Convert.ToInt32(grdViewMenuType.GetRowCellValue(grdViewMenuType.FocusedRowHandle, "Parent").ToString()),
                    Type = grdViewMenuType.GetRowCellValue(grdViewMenuType.FocusedRowHandle, "Type").ToString(),
                    IsActive = Convert.ToBoolean(grdViewMenuType.GetRowCellValue(grdViewMenuType.FocusedRowHandle, "IsActive")),
                };

                LoadMenuTypeData();

                EnableDisableButtons(layoutControl2, "Load");
            }
            else
                EnableDisableButtons(layoutControl2, "Disable All");

            SetControlsReadOnly(layoutControl1, true);
            SetControlsReadOnly(layoutControl2, true);
        }

        private void btnMenuTypeEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl1, false);
            SetControlsReadOnly(layoutControl2, false);

            EnableDisableButtons(layoutControl2, "Edit");

            EnableDisableTabs();

            menuTypeBL.IsMenuTypeUpdate = true;

            txtMenuType.Focus();
        }

        private void btnMenuTypeCancel_Click(object sender, EventArgs e)
        {
            if (menuTypeBL.IsDataAvailable)
            {
                grdViewMenuType_RowCellClick(null, null);
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

        private void btnMenuTypeDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult _ans = XtraMessageBox.Show("Are you sure you want to delete the data?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (_ans == DialogResult.Yes)
                {
                    if (menuTypeBL.Delete(menuTypeData.MenuTypeID))
                    {
                        GimjaHelper.ShowInformation("Data successfully deleted", Application.ProductName);

                        PopulateMenuTypes();

                        if (menuTypeBL.IsDataAvailable)
                        {
                            grdViewMenuType_RowCellClick(null, null);
                            EnableDisableButtons(layoutControl2, "Load");
                        }
                        else
                        {
                            ClearControlValues(layoutControl2);
                            EnableDisableButtons(layoutControl2, "Disable All");
                        }

                        SetControlsReadOnly(layoutControl2, true);
                        EnableDisableTabs(true);
                    }
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be deleted. \n" + ex.Message, Application.ProductName);
            }
        }

        #endregion

        #region Menu

        private void btnMenuAddIcon_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.ico; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.ico; *.png";
            openFileDialog.FileName = "";
            openFileDialog.ShowDialog();

            string strFileName = openFileDialog.FileName;

            if (!strFileName.Equals(""))
            {
                try
                {
                    picMenuIcon.Image = Image.FromFile(strFileName);

                    FileInfo fiImage = new FileInfo(strFileName);
                    long photoFileLength = fiImage.Length;
                    FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    menuData = new MenuData();
                    menuData.Icon = new byte[Convert.ToInt32(photoFileLength)];
                    int ibytesread = fs.Read(menuData.Icon, 0, Convert.ToInt32(photoFileLength));

                    fs.Close();
                }
                catch (Exception ex)
                {
                    GimjaHelper.ShowError(ex.Message, Application.ProductName);
                }
            }
            else
                menuData.Icon = null;
        }

        private void grdViewMenu_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (menuBL.IsDataAvailable)
            {
                menuData = new MenuData()
                {
                    //ID = Convert.ToInt32(grdViewMenu.GetRowCellValue(grdViewMenu.FocusedRowHandle, "ID").ToString()),
                    //Caption = grdViewMenu.GetRowCellValue(grdViewMenu.FocusedRowHandle, "Caption").ToString(),
                    //Parent = Convert.ToInt32(grdViewMenu.GetRowCellValue(grdViewMenu.FocusedRowHandle, "Parent").ToString()),
                    //MenuTypeID = Convert.ToInt32(grdViewMenu.GetRowCellValue(grdViewMenu.FocusedRowHandle, "MenuTypeID").ToString()),
                    //IsActive = Convert.ToBoolean(grdViewMenu.GetRowCellValue(grdViewMenu.FocusedRowHandle, "IsActive")),
                    //Visible = Convert.ToBoolean(grdViewMenu.GetRowCellValue(grdViewMenu.FocusedRowHandle, "Visible")),
                    //Disabled = Convert.ToBoolean(grdViewMenu.GetRowCellValue(grdViewMenu.FocusedRowHandle, "Disabled")),
                    //Icon = (byte[])grdViewMenu.GetRowCellValue(grdViewMenu.FocusedRowHandle, "Icon")
                };

                menuID = menuData.ID;
                //LoadMenuData();

                EnableDisableButtons(layoutControl3, "Load");
            }
            else
                EnableDisableButtons(layoutControl3, "Disable All");

            SetControlsReadOnly(layoutControl3, true);
        }

        private void btnMenuAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControlValues(layoutControl3);

                EnableDisableButtons(layoutControl3, "Add");

                SetControlsReadOnly(layoutControl3, false);

                menuBL.IsMenuUpdate = false;

                EnableDisableTabs();

                txtMenuCaption.Focus();
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message, Application.ProductName);
            }
        }

        private void btnMenuSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtMenuCaption.Text) && lkeMenuType.EditValue != null)
                {
                    MenuData _menuData = new MenuData()
                    {
                        Caption = txtMenuCaption.Text.Trim(),
                        MenuTypeID = Convert.ToInt32(lkeMenuType.EditValue),
                        Parent = Convert.ToInt32(lkeMenuParent.EditValue),
                        Icon = menuData.Icon,
                        CreatedDate = DateTime.Now,
                        CreatedBy = Singleton.Instance.UserID,
                        LastUpdatedDate = DateTime.Now,
                        LastUpdatedBy = Singleton.Instance.UserID,
                        IsActive = (chkMenuIsActive.CheckState == CheckState.Checked ? true : false),
                        Visible = (chkMenuIsVisible.CheckState == CheckState.Checked ? true : false),
                        Disabled = (chkMenuIsDisabled.CheckState == CheckState.Checked ? true : false)
                    };

                    if (menuBL.IsMenuUpdate)
                    {
                        _menuData.ID = menuID;

                        if (menuBL.Update(_menuData))
                            GimjaHelper.ShowInformation("Data successfully updated", Application.ProductName);
                        else
                            GimjaHelper.ShowInformation("Data can not be successfully updated", Application.ProductName);
                    }
                    else
                    {
                        if (menuBL.Validate(txtMenuCaption.Text.Trim(), Convert.ToInt32(lkeMenuType.EditValue), Convert.ToInt32(lkeMenuParent.EditValue)))
                        {
                            if (menuBL.Add(_menuData))
                                GimjaHelper.ShowInformation("Data successfully added", Application.ProductName);
                            else
                                GimjaHelper.ShowError("Data can not successfully added. Please try again", Application.ProductName);
                        }
                        else
                        {
                            GimjaHelper.ShowWarning("Menu type name already exists", Application.ProductName);
                            txtMenuType.SelectAll();
                            return;
                        }
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("Please fill all required fields", Application.ProductName);
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be saved. \n" + ex.Message, Application.ProductName);
            }
            finally
            {
                SetControlsReadOnly(layoutControl3, true);
                EnableDisableButtons(layoutControl3, "Load");
                EnableDisableTabs(true);
                PopulateMenus(menuData.Parent);
                grdViewMenu_RowCellClick(null, null);
            }
        }

        private void btnMenuEdit_Click(object sender, EventArgs e)
        {
            SetControlsReadOnly(layoutControl3, false);

            EnableDisableButtons(layoutControl3, "Edit");

            EnableDisableTabs();

            menuBL.IsMenuUpdate = true;

            txtMenuCaption.Focus();
        }

        private void btnMenuDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult _ans = XtraMessageBox.Show("Are you sure you want to delete the data?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (_ans == DialogResult.Yes)
                {
                    if (menuBL.Delete(menuData.ID))
                    {
                        XtraMessageBox.Show("Data successfully deleted", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        PopulateMenus();

                        if (menuBL.IsDataAvailable)
                        {
                            grdViewMenu_RowCellClick(null, null);
                            EnableDisableButtons(layoutControl3, "Load");
                        }
                        else
                        {
                            ClearControlValues(layoutControl3);
                            EnableDisableButtons(layoutControl3, "Disable All");
                        }

                        SetControlsReadOnly(layoutControl3, true);
                        EnableDisableTabs(true);
                    }
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be deleted.", Application.ProductName);
            }
        }

        private void btnMenuCancel_Click(object sender, EventArgs e)
        {
            if (menuBL.IsDataAvailable)
            {
                grdViewMenu_RowCellClick(null, null);
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

        private void lkeMenuType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (lkeMenuType.EditValue != null)
                {
                    int _menuTypeID = Convert.ToInt32(lkeMenuType.EditValue);
                    lkeMenuParent.Properties.DataSource = menuBL.GetFilteredParentMenus(_menuTypeID);
                    lkeMenuParent.Enabled = menuBL.GetFilteredParentMenus(_menuTypeID).Count() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message, Application.ProductName);
            }
        }

        private void trMenu_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (menuBL.IsDataAvailable)
            {
                menuData = new MenuData()
                {
                    ID = Convert.ToInt32(trMenu.FocusedNode.GetValue("ID").ToString()),
                    Caption = trMenu.FocusedNode.GetValue("Caption").ToString(),
                    Parent = Convert.ToInt32(trMenu.FocusedNode.GetValue("Parent").ToString()),
                    MenuTypeID = Convert.ToInt32(trMenu.FocusedNode.GetValue("MenuTypeID").ToString()),
                    IsActive = Convert.ToBoolean(trMenu.FocusedNode.GetValue("IsActive")),
                    Visible = Convert.ToBoolean(trMenu.FocusedNode.GetValue("Visible")),
                    Disabled = Convert.ToBoolean(trMenu.FocusedNode.GetValue("Disabled")),
                    Icon = (byte[])trMenu.FocusedNode.GetValue("Icon"),
                    Order = Convert.ToInt32(trMenu.FocusedNode.GetValue("Order").ToString())
                };

                menuID = menuData.ID;
                LoadMenuData();

                EnableDisableButtons(layoutControl3, "Load");

                //if(menuData.Order > 0)
                btnMenuUp.Enabled = btnMenuDown.Enabled = true;
            }
            else
                EnableDisableButtons(layoutControl3, "Disable All");

            SetControlsReadOnly(layoutControl3, true);
        }

        private void btnMenuDown_Click(object sender, EventArgs e)
        {
            if (menuBL.Update(menuData, MOVE_MENU_DOWN))
                //GimjaHelper.ShowInformation("");
                PopulateMenus();
        }
        
        private void btnMenuUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (menuBL.Update(menuData, MOVE_MENU_UP))
                    PopulateMenus();
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message);
            }
        }

        #endregion

        #region MenuRole

        private void grdViewRole_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                menuRoleData = new MenuRoleData()
                {
                    RoleID = Convert.ToInt32(grdViewRole.GetRowCellValue(grdViewRole.FocusedRowHandle, "ID").ToString()),
                };

                grdMenuRole.DataSource = menuBL.GetMenuRoles(menuRoleData.RoleID);
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message, Application.ProductName);
            }
        }
        
        private void btnMenuRoleSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (menuRoleData.RoleID != null)
                {
                    GetSelectedMenus();
                    if (menuBL.Add(menuRoleData.RoleID, selectedMenus))
                        GimjaHelper.ShowInformation("Data successfully saved!");
                    else
                        GimjaHelper.ShowError("There was error while saving the records", Application.ProductName);
                }
                else
                    GimjaHelper.ShowInformation("Please specify the user role");
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message, Application.ProductName);
            }
        }
        
        #endregion
                
        #region Menu Form
        private void btnMenuFormSave_Click(object sender, EventArgs e)
        {
            try
            {
                var _menuIDs = (from CheckedListBoxItem item in chkCboMenu.Properties.Items
                           where item.CheckState == CheckState.Checked
                           select (int)item.Value).ToArray();

                if (_menuIDs.Count()>0 && lkeMenuFormForms.EditValue != null)
                {
                    FormData[] _formData = new FormData[_menuIDs.Count()];
                    int _index = 0;
 
                    foreach (var _mID in _menuIDs)
                    {
                        formData = new FormData()
                        {
                            MenuID = Convert.ToInt32(_mID.ToString()),
                            FormID = Convert.ToString(lkeMenuFormForms.EditValue),
                            FormName = lkeMenuFormForms.Text
                        };

                        _formData[_index] = formData;
                        _index++;
                    }

                    if (menuBL.Add(_formData))
                        GimjaHelper.ShowInformation("Menu and form successfully associated");
                    else
                        GimjaHelper.ShowInformation("Could not associate menu and form successfully. Please try again");
                }
                else
                    GimjaHelper.ShowError("Please select Menu and Form to associate");
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError(ex.Message);
            }
            finally
            {
                InitializeMenuForm();
                chkCboMenu.SetEditValue(-1);
                lkeMenuFormForms.EditValue = null;
            }
        }

        private void btnMenuFormDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedMenuForms.Count > 0)
                {
                    DialogResult _ans = XtraMessageBox.Show(String.Format("Are you sure you want to dissociate Menu{0} and Form{0}?", selectedMenuForms.Count>0?"s":String.Empty), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    
                    if (_ans == DialogResult.Yes)
                    {
                        if (menuBL.Delete(selectedMenuForms))
                        {
                            XtraMessageBox.Show("Data successfully deleted", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            InitializeMenuForm();

                            selectedMenuForms.Clear();
                        }
                    }
                }
                else
                    GimjaHelper.ShowWarning("Please select the Menu and Form association you want to delete");
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Data could not be deleted.", Application.ProductName);
            }
        }

        private void grdViewMenuForm_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

            int id = (int)grdViewMenuForm.GetListSourceRowCellValue(e.ListSourceRowIndex, grdViewMenuForm.Columns["ID"]);

            if (e.IsGetData)
                e.Value = selectedMenuForms.ContainsKey(id);
            else
            {
                if (!(bool)e.Value)
                    selectedMenuForms.Remove(id);
                else selectedMenuForms.Add(id, e);
            }
        }

        #endregion
       
        #endregion
    }
}