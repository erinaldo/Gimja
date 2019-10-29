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

namespace Gimja
{
    public partial class frmItemCount : DevExpress.XtraEditors.XtraForm
    {
        public IList<StockTakingBL> stocktakingList;
        public IList<LossAdjustmentBL> lossAdjustmentList;
        private StockTakingBL currentStocktaking;
        private LossAdjustmentBL currentLossAdjustment;
        private bool isSTAdding, isLAAdding, isSTUpdating, isLAUpdating;
        public frmItemCount()
        {
            InitializeComponent();
            currentStocktaking = new StockTakingBL();
            currentLossAdjustment = new LossAdjustmentBL();
            //populate the list of items
            PopulateItemList();
            //populate stocktaking
            PopulateStocktaking();
            //populate loss adjustment list
            PopulateLossAdjustment();
        }

        private void PopulateLossAdjustment()
        {
            var _adjustments = currentLossAdjustment.GetLossAdjustments();
            lossAdjustmentList = new List<LossAdjustmentBL>(_adjustments);
            int _currentRowIndex = grdViewLossAdjustment.FocusedRowHandle;
            grdLossAdjustment.DataSource = lossAdjustmentList;
            if (_currentRowIndex >= 0 && _currentRowIndex < grdViewLossAdjustment.RowCount)
                grdViewLossAdjustment.FocusedRowHandle = _currentRowIndex;
            else if (_currentRowIndex >= grdViewLossAdjustment.RowCount)
                grdViewLossAdjustment.FocusedRowHandle = grdViewLossAdjustment.RowCount - 1;
            else if (grdViewLossAdjustment.RowCount > 0)
                grdViewLossAdjustment.FocusedRowHandle = 0;
            else
                grdViewLossAdjustment.FocusedRowHandle = -1;
        }

        private void PopulateStocktaking()
        {
            var _stocktakings = currentStocktaking.GetStocktakings();
            stocktakingList = new List<StockTakingBL>(_stocktakings);
            int _currentRowIndex = grdViewStockTaking.FocusedRowHandle;
            grdStockTaking.DataSource = stocktakingList;
            if (_currentRowIndex >= 0 && _currentRowIndex < grdViewStockTaking.RowCount)
                grdViewStockTaking.FocusedRowHandle = _currentRowIndex;
            else if (_currentRowIndex >= grdViewStockTaking.RowCount)
                grdViewStockTaking.FocusedRowHandle = grdViewStockTaking.RowCount - 1;
            else if (grdViewStockTaking.RowCount > 0)
                grdViewStockTaking.FocusedRowHandle = 0;
            else
                grdViewStockTaking.FocusedRowHandle = -1;
        }

        private void PopulateItemList()
        {
            ItemBL itemBL = new ItemBL();
            var _items = itemBL.GetData(true);
            lkeStockTakingItemId.Properties.DataSource = _items;
            lkeRepositorySTItemID.DataSource = _items;
            lkeLossAdjustmentItemID.Properties.DataSource = _items;
            lkeRepositoryLAItemId.DataSource = _items;
        }

        private void btnStockTakingAdd_Click(object sender, EventArgs e)
        {
            if (isSTUpdating)
            {
                GimjaHelper.ShowError("The update operation is still pending!");
                return;
            }
            isSTAdding = true;//trying to add new stocktaking record
            //disable the stocktaking list
            grdStockTaking.Enabled = false;
            //disable the loss adjustment page
            tabPgLossAdjustment.PageEnabled = false;
            //enable and clear the input controls
            ClearStocktakingControls();
            //enable/disable the command buttons
            EnableStocktakingControls(false);
        }

        private void ClearStocktakingControls()
        {
            lkeStockTakingItemId.EditValue = null;
            txtStockTakingQuantity.Text = string.Empty;
            dtStockTakingDate.DateTime = DateTime.Today;
        }

        private void EnableStocktakingControls(bool readOnlyStatus)
        {
            lkeStockTakingItemId.Properties.ReadOnly = readOnlyStatus;
            txtStockTakingQuantity.Properties.ReadOnly = readOnlyStatus;
            dtStockTakingDate.Properties.ReadOnly = readOnlyStatus;

            btnStockTakingSave.Enabled = !readOnlyStatus;
            btnStockTakingDelete.Enabled = readOnlyStatus;
            btnStockTakingCancel.Enabled = !readOnlyStatus;
            btnStockTakingEdit.Enabled = readOnlyStatus;
            btnStockTakingAdd.Enabled = readOnlyStatus;

            grdStockTaking.Enabled = readOnlyStatus;
            tabPgLossAdjustment.PageEnabled = readOnlyStatus;
        }

        private void btnStockTakingEdit_Click(object sender, EventArgs e)
        {
            if (isSTAdding)
            {
                GimjaHelper.ShowError("The add operation is still pending!");
                return;
            }
            isSTUpdating = true;
            //disable the stocktaking list
            grdStockTaking.Enabled = false;
            //disable the loss adjustment page
            tabPgLossAdjustment.PageEnabled = false;
            //enable/disable the input controls and command buttons
            EnableStocktakingControls(false);
        }

        private void btnStockTakingSave_Click(object sender, EventArgs e)
        {
            var _currentUser = Singleton.Instance.UserID;
            var _currentDateTime = DateTime.Now;
            if (isSTAdding)
            {//adding new stocktaking record
                bool isValid = ValidateStocktaking();
                if (isValid)
                {//the given values are valid
                    currentStocktaking = new StockTakingBL()
                    {
                        ItemID = Convert.ToString(lkeStockTakingItemId.EditValue),
                        Date = dtStockTakingDate.DateTime,
                        Quantity = Convert.ToInt32(txtStockTakingQuantity.Text),
                        CreatedBy = _currentUser,
                        CreatedDate = _currentDateTime
                    };
                    try
                    {
                        bool isSaved = currentStocktaking.Insert();
                        if (isSaved)
                        {
                            GimjaHelper.ShowInformation("The stocktaking record is inserted successfully.");
                            isSTAdding = false;
                            EnableStocktakingControls(true);
                            //refresh the list of stocktaking record
                            PopulateStocktaking();
                            //var maxIdObj = stocktakingList.OrderByDescending(s => s.ID).Take(1);
                            //if(maxIdObj!=null)
                        }
                        else
                        {
                            GimjaHelper.ShowError("The stocktaking insert failed.");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GimjaHelper.ShowError("Error encountered while inserting: " + ex.Message);
                        return;
                    }
                }
            }
            else if (isSTUpdating)
            {//updating an existing stocktaking record
                bool isValid = ValidateStocktaking();
                if (isValid)
                {//the values for the stocktaking is valid
                    currentStocktaking = (StockTakingBL)grdViewStockTaking.GetFocusedRow();
                    if (currentStocktaking != null)
                    {
                        currentStocktaking.ItemID = Convert.ToString(lkeStockTakingItemId.EditValue);
                        currentStocktaking.Date = dtStockTakingDate.DateTime;
                        currentStocktaking.Quantity = Convert.ToInt32(txtStockTakingQuantity.Text);
                        currentStocktaking.LastUpdatedBy = _currentUser;
                        currentStocktaking.LastUpdatedDate = _currentDateTime;
                        try
                        {//save to database
                            bool isSaved = currentStocktaking.Update();
                            if (isSaved)
                            {
                                isSTUpdating = false;
                                GimjaHelper.ShowInformation("Update is successfull.");
                                PopulateStocktaking();
                                EnableStocktakingControls(true);
                            }
                            else
                            {
                                GimjaHelper.ShowError("Update failed.");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            GimjaHelper.ShowError("Error encounterd: " + ex.Message);
                            return;
                        }
                    }
                }
            }
        }

        private bool ValidateStocktaking()
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(lkeStockTakingItemId.EditValue)))
            {
                lkeStockTakingItemId.Focus();
                GimjaHelper.ShowError("Invalid Item choice.");
                return false;
            }
            DateTime d = dtStockTakingDate.DateTime;
            if (d > DateTime.Today || d == DateTime.MinValue)
            {
                dtStockTakingDate.Focus();
                GimjaHelper.ShowError("Invalid stocktaking date.");
                return false;
            }
            int q;
            bool isValidQty = int.TryParse(txtStockTakingQuantity.Text, out q);
            if (!isValidQty || q <= 0)
            {
                txtStockTakingQuantity.Focus();
                GimjaHelper.ShowError("Invalid stocktaking quantity.");
                return false;
            }
            //otherwise
            return true;
        }

        private void grdViewStockTaking_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            currentStocktaking = grdViewStockTaking.GetRow(e.FocusedRowHandle) as StockTakingBL;
            if (currentStocktaking != null)
            {
                lkeStockTakingItemId.EditValue = currentStocktaking.ItemID;
                txtStockTakingQuantity.Text = Convert.ToString(currentStocktaking.Quantity);
                dtStockTakingDate.DateTime = currentStocktaking.Date;
                //enable the edit and delete buttons
                btnStockTakingDelete.Enabled = true;
                btnStockTakingEdit.Enabled = true;
            }
        }

        private void btnStockTakingDelete_Click(object sender, EventArgs e)
        {
            currentStocktaking = (StockTakingBL)grdViewStockTaking.GetFocusedRow();
            if (currentStocktaking != null)
            {
                var _response = GimjaHelper.ShowQuestion("Are you sure to delete the selected stocktaking record.");
                if (_response == System.Windows.Forms.DialogResult.Yes)
                {
                    currentStocktaking.LastUpdatedBy = Singleton.Instance.UserID;
                    currentStocktaking.LastUpdatedDate = DateTime.Now;
                    try
                    {
                        bool isSaved = currentStocktaking.Delete();
                        if (isSaved)
                        {
                            GimjaHelper.ShowInformation("Deleted successfully.");
                            PopulateStocktaking();
                        }
                        else
                        {
                            GimjaHelper.ShowError("Delete Failed.");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GimjaHelper.ShowError("Error encounterd: " + ex.Message);
                        return;
                    }
                }
            }
        }

        private void btnStockTakingCancel_Click(object sender, EventArgs e)
        {
            if (isSTAdding)
            {//cancel the add
                isSTAdding = false;
                PopulateStocktaking();
                EnableStocktakingControls(true);
            }
            else if (isSTUpdating)
            {
                isSTUpdating = false;
                PopulateStocktaking();
                EnableStocktakingControls(true);
            }
        }

        private void tabItemCount_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page == tabPgLossAdjustment)
            {
                PopulateLossAdjustment();
            }
            else if (e.Page == tabPgStockTaking)
            {
                PopulateStocktaking();
            }
        }

        private void btnLossAdjustmentAdd_Click(object sender, EventArgs e)
        {
            if (isLAUpdating)
            {
                GimjaHelper.ShowError("An update is pending.");
                return;
            }
            isLAAdding = true;
            //clear the controls in loss adjustment
            ClearLossAdjustmentControls();
            //enable/disable loss adjustment controls
            EnableLossAdjustmentControls(false);
        }

        private void EnableLossAdjustmentControls(bool readOnlyStatus)
        {
            btnLossAdjustmentAdd.Enabled = readOnlyStatus;
            btnLossAdjustmentSave.Enabled = !readOnlyStatus;
            btnLossAdjustmentEdit.Enabled = readOnlyStatus;
            btnLossAdjustmentDelete.Enabled = readOnlyStatus;
            btnLossAdjustmentCancel.Enabled = !readOnlyStatus;
            //enable/disable input controls
            lkeLossAdjustmentItemID.Properties.ReadOnly = readOnlyStatus;
            txtLossAdjustmentQuantity.Properties.ReadOnly = readOnlyStatus;
            txtLossAdjustmentCost.Properties.ReadOnly = readOnlyStatus;
            txtLossAdjustmentReason.Properties.ReadOnly = readOnlyStatus;
            chkLossAdjustmentIsLoss.Properties.ReadOnly = readOnlyStatus;
            //enable/disable the grid view
            grdLossAdjustment.Enabled = readOnlyStatus;
            tabPgStockTaking.PageEnabled = readOnlyStatus;
        }

        private void ClearLossAdjustmentControls()
        {
            lkeLossAdjustmentItemID.EditValue = null;
            txtLossAdjustmentQuantity.Text = string.Empty;
            txtLossAdjustmentCost.Text = string.Empty;
            txtLossAdjustmentReason.Text = string.Empty;
            chkLossAdjustmentIsLoss.Checked = false;
        }

        private void btnLossAdjustmentSave_Click(object sender, EventArgs e)
        {
            var _currentUser = Singleton.Instance.UserID;
            var _currentDate = DateTime.Now;
            if (isLAAdding)
            {
                if (ValidateLossAdjustment())
                {
                    //try to save to database
                    try
                    {
                        //create the loss adjustment object
                        currentLossAdjustment = new LossAdjustmentBL()
                        {
                            ItemID = Convert.ToString(lkeLossAdjustmentItemID.EditValue),
                            Quantity = Convert.ToInt32(txtLossAdjustmentQuantity.Text),
                            Cost = Convert.ToDecimal(txtLossAdjustmentCost.Text),
                            Reason = txtLossAdjustmentReason.Text.Trim(),
                            IsLoss = chkLossAdjustmentIsLoss.Checked,
                            CreatedBy = _currentUser,
                            CreatedDate = _currentDate
                        };
                        var _isSaved = currentLossAdjustment.Insert();
                        if (_isSaved)
                        {
                            GimjaHelper.ShowInformation("Inserted successfully.");
                            PopulateLossAdjustment();
                            EnableLossAdjustmentControls(true);
                            isLAAdding = false;
                        }
                        else
                        {
                            GimjaHelper.ShowError("Insert failed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        GimjaHelper.ShowError("Error encountered: " + ex.Message);
                    }
                }
                else
                {//invalid loss adjustment found
                    return;
                }
            }
            else if (isLAUpdating)
            {
                if (ValidateLossAdjustment())
                {
                    try
                    {//get the currently selected object
                        currentLossAdjustment = (LossAdjustmentBL)grdViewLossAdjustment.GetFocusedRow();
                        if (currentLossAdjustment != null)
                        {
                            //change the properties
                            currentLossAdjustment.ItemID = Convert.ToString(lkeLossAdjustmentItemID.EditValue);
                            currentLossAdjustment.Quantity = Convert.ToInt32(txtLossAdjustmentQuantity.Text);
                            currentLossAdjustment.Cost = Convert.ToDecimal(txtLossAdjustmentCost.Text);
                            currentLossAdjustment.IsLoss = chkLossAdjustmentIsLoss.Checked;
                            currentLossAdjustment.Reason = txtLossAdjustmentReason.Text.Trim();
                            currentLossAdjustment.LastUpdatedBy = _currentUser;
                            currentLossAdjustment.LastUpdatedDate = _currentDate;

                            bool _isSaved = currentLossAdjustment.Update();
                            if (_isSaved)
                            {
                                GimjaHelper.ShowInformation("Update successfully.");
                                PopulateLossAdjustment();
                                EnableLossAdjustmentControls(true);
                                isLAUpdating = false;
                            }
                            else
                            {
                                GimjaHelper.ShowError("Update failed.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GimjaHelper.ShowError("Error encountered: " + ex.Message);
                    }
                }
            }
        }

        private bool ValidateLossAdjustment()
        {
            string _itemId = Convert.ToString(lkeLossAdjustmentItemID.EditValue);
            if (string.IsNullOrWhiteSpace(_itemId))
            {
                lkeLossAdjustmentItemID.Focus();
                GimjaHelper.ShowError("Item is required.");
                return false;
            }
            int _qty;
            bool _validQty = int.TryParse(txtLossAdjustmentQuantity.Text, out _qty);
            if (!_validQty || _qty <= 0)
            {
                txtLossAdjustmentQuantity.Focus();
                GimjaHelper.ShowError("Invalid quantity.");
                return false;
            }
            decimal _cost;
            bool _validCost = decimal.TryParse(txtLossAdjustmentCost.Text, out _cost);
            if (!_validCost || _cost <= 0)
            {
                txtLossAdjustmentCost.Focus();
                GimjaHelper.ShowError("Invalid cost.");
                return false;
            }
            string _reason = txtLossAdjustmentReason.Text;
            if (string.IsNullOrWhiteSpace(_reason))
            {
                txtLossAdjustmentReason.Focus();
                GimjaHelper.ShowError("Reason is required.");
                return false;
            }
            //otherwise
            return true;
        }

        private void btnLossAdjustmentEdit_Click(object sender, EventArgs e)
        {
            if (isLAAdding)
            {
                GimjaHelper.ShowError("Add operation is pending.");
                return;
            }
            isLAUpdating = true;
            //enable/disable controls
            EnableLossAdjustmentControls(false);
        }

        private void btnLossAdjustmentDelete_Click(object sender, EventArgs e)
        {
            if (isLAAdding || isLAUpdating)
            {
                GimjaHelper.ShowError("Data operation is pending.");
                return;
            }
            currentLossAdjustment = (LossAdjustmentBL)grdViewLossAdjustment.GetFocusedRow();
            if (currentLossAdjustment != null)
            {
                var _response = GimjaHelper.ShowQuestion("Are you sure to remove the currently selected loss adjustment record.");
                if (_response == System.Windows.Forms.DialogResult.Yes)
                {
                    currentLossAdjustment.LastUpdatedBy = Singleton.Instance.UserID;
                    currentLossAdjustment.LastUpdatedDate = DateTime.Now;
                    //try to save to database
                    try
                    {
                        bool _isDeleted = currentLossAdjustment.Delete();
                        if (_isDeleted)
                        {
                            GimjaHelper.ShowInformation("Deleted successfully.");
                            PopulateLossAdjustment();
                        }
                        else
                        {
                            GimjaHelper.ShowInformation("Delete failed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        GimjaHelper.ShowError("Error encountered: " + ex.Message);
                    }
                }
            }
            else
            {
                GimjaHelper.ShowError("There is no loss adjustment to delete.");
            }
        }

        private void btnLossAdjustmentCancel_Click(object sender, EventArgs e)
        {
            if (isLAAdding)
            {
                isLAAdding = false;
                PopulateLossAdjustment();
                EnableLossAdjustmentControls(true);
            }
            else if (isLAUpdating)
            {
                isLAUpdating = false;
                PopulateLossAdjustment();
                EnableLossAdjustmentControls(true);
            }
        }
    }
}