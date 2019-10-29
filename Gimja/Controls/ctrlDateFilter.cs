using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Gimja
{
    public partial class ctrlDateFilter : DevExpress.XtraEditors.XtraUserControl
    {
        private string[] months ={"January", "February", "March",
                                    "April", "May", "June", "July",
                                    "August", "September", "October",
                                    "November", "December"};
        private string[] quarters = { "QI", "QII", "QIII", "QIV" };
        private string selectedPart = string.Empty;

        public delegate void ButtonClickEventHandler(object sender, DateFilterEventArgs x);
        [Description("Fires when the button is clicked.")]
        [Category("Mouse")]
        public event ButtonClickEventHandler ButtonClick;
        public virtual void OnButtonClick(object sender, EventArgs e)
        {
            DateFilterEventArgs args = new DateFilterEventArgs(DateFrom, DateTo);
            if (ButtonClick != null)
                ButtonClick(this, args);
        }

        public ctrlDateFilter()
        {
            InitializeComponent();
            //select the date range as default category
            if (cboDateCategory.Properties.Items.Count > 0)
                cboDateCategory.SelectedIndex = 0;
        }

        private void cboDateCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            var _selectedCategory = Convert.ToString(cboDateCategory.SelectedItem);
            if (!string.IsNullOrEmpty(_selectedCategory))
            {
                selectedPart = _selectedCategory.ToLower();

                switch (_selectedCategory.ToLower())
                {
                    case "date range":
                        lblDatePart.Text = "Range";
                        cboQuarter.Enabled = false;
                        cboQuarter.Properties.Items.Clear();//clear the date part items
                        //dtFrom.Enabled = true;
                        //dtTo.Enabled = true;
                        dtFrom.DateTime = DateTime.Today;
                        dtTo.DateTime = DateTime.Today;
                        SetMinMaxDate(DateTime.MinValue, DateTime.MaxValue);
                        break;
                    case "month":
                        lblDatePart.Text = "Month";
                        cboQuarter.Enabled = true;
                        PopulateMonths();
                        //dtFrom.Enabled = false;
                        //dtTo.Enabled = false;
                        break;
                    case "quarter":
                        lblDatePart.Text = "Quarter";
                        cboQuarter.Enabled = true;
                        PopulateQuarters();
                        //dtFrom.Enabled = false;
                        //dtTo.Enabled = false;
                        break;
                    case "year":
                        lblDatePart.Text = "Year";
                        cboQuarter.Enabled = true;
                        PopulateYears();
                        //dtFrom.Enabled = false;
                        //dtFrom.Enabled = false;
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Populates fiver year entries to the list
        /// </summary>
        private void PopulateYears()
        {
            cboQuarter.Properties.Items.Clear();
            var _currentYear = DateTime.Today.Year;
            int[] years = new int[5];
            for (int i = 0; i < 5; i++)
            {
                years[i] = _currentYear - i;
            }
            cboQuarter.Properties.Items.AddRange(years);
            cboQuarter.SelectedIndex = 0;//select the first year value
            dtFrom.DateTime = new DateTime(years[0], 1, 1);
            dtTo.DateTime = dtFrom.DateTime.AddYears(1).AddDays(-1);
            SetMinMaxDate(new DateTime(years[0], 1, 1), dtFrom.DateTime.AddYears(1).AddDays(-1));
        }

        private void PopulateQuarters()
        {
            cboQuarter.Properties.Items.Clear();
            cboQuarter.Properties.Items.AddRange(quarters);
            cboQuarter.SelectedIndex = 0;//select Q I
            var _currentDate = DateTime.Today;
            var _startDate = new DateTime(_currentDate.Year, 1, 1);
            var _maxDate = _startDate.AddMonths(3).AddDays(-1);

            SetMinMaxDate(new DateTime(_startDate.Year, 1, 1), new DateTime(_startDate.Year, _maxDate.Month, DateTime.DaysInMonth(_startDate.Year, _maxDate.Month)));
            dtFrom.DateTime = new DateTime(_startDate.Year, 1, 1);
            dtTo.DateTime = new DateTime(_startDate.Year, _maxDate.Month, DateTime.DaysInMonth(_startDate.Year, _maxDate.Month));
        }

        private void PopulateMonths()
        {
            cboQuarter.Properties.Items.Clear();//clear any previous items
            cboQuarter.Properties.Items.AddRange(months);
            cboQuarter.SelectedIndex = 0;//select the january month
            var _currentDate = DateTime.Today;
            SetMinMaxDate(new DateTime(_currentDate.Year, 1, 1), new DateTime(_currentDate.Year, 1, DateTime.DaysInMonth(_currentDate.Year, 1)));
            dtFrom.DateTime = new DateTime(_currentDate.Year, 1, 1);
            dtTo.DateTime = new DateTime(_currentDate.Year, 1, DateTime.DaysInMonth(_currentDate.Year, 1));
        }

        private void SetMinMaxDate(DateTime minDate, DateTime maxDate)
        {
            dtTo.Enabled = dtFrom.Enabled = true;
            dtFrom.Properties.MinValue = dtTo.Properties.MinValue = minDate;
            dtFrom.Properties.MaxValue = dtTo.Properties.MaxValue = maxDate;
        }

        private void cboQuarter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var _currentDate = DateTime.Today;
            switch (selectedPart)
            {
                case "date range":
                    //do nothing
                    break;
                case "month":
                    int _monthSelected = cboQuarter.SelectedIndex;
                    if (_monthSelected >= 0)
                    {
                        SetMinMaxDate(new DateTime(_currentDate.Year, _monthSelected + 1, 1), new DateTime(_currentDate.Year, _monthSelected + 1, DateTime.DaysInMonth(_currentDate.Year, _monthSelected + 1)));
                        dtFrom.DateTime = new DateTime(_currentDate.Year, _monthSelected + 1, 1);
                        dtTo.DateTime = new DateTime(_currentDate.Year, _monthSelected + 1, DateTime.DaysInMonth(_currentDate.Year, _monthSelected + 1));
                    }
                    break;
                case "quarter":
                    int _quarterSelected = cboQuarter.SelectedIndex;
                    if (_quarterSelected >= 0)
                    {
                        _currentDate = DateTime.Today;
                        var _startDate = new DateTime(_currentDate.Year, (_quarterSelected * 3) + 1, 1);
                        var _maxDate = _startDate.AddMonths(3).AddDays(-1);
                        SetMinMaxDate(_startDate, new DateTime(_startDate.Year, _maxDate.Month, DateTime.DaysInMonth(_startDate.Year, _maxDate.Month)));
                        dtFrom.DateTime = _startDate;
                        dtTo.DateTime = new DateTime(_startDate.Year, _maxDate.Month, DateTime.DaysInMonth(_startDate.Year, _maxDate.Month));
                    }
                    break;
                case "year":
                    int _selectedYear;
                    var isValidYear = int.TryParse(Convert.ToString(cboQuarter.SelectedItem), out _selectedYear);
                    if (isValidYear)
                    {
                        DateTime fromDate = new DateTime(_selectedYear, 1, 1);
                        DateTime maxDate = fromDate.AddYears(1).AddDays(-1);
                        SetMinMaxDate(fromDate, maxDate);// dtFrom.DateTime, dtTo.DateTime);
                        if (dtTo.DateTime < fromDate)
                        {
                            dtTo.DateTime = maxDate;
                            dtFrom.DateTime = fromDate;
                        }
                        else
                        {
                            dtFrom.DateTime = fromDate;
                            dtTo.DateTime = maxDate;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        [DescriptionAttribute("Text to be used as caption for the button.")]
        [Category("Appearance")]
        public string ButtonText
        {
            get
            {
                return btnGo.Text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    value = "&Go";
                btnGo.Text = value;
            }
        }

        [Category("Appearance")]
        public DateTime DateFrom
        {
            get
            {
                return dtFrom.DateTime;
            }
            set
            {
                dtFrom.DateTime = value;
            }
        }

        [Category("Appearance")]
        public DateTime DateTo
        {
            get
            {
                return dtTo.DateTime;
            }
            set
            {
                dtTo.DateTime = value;
            }
        }

        [Category("Behavior")]
        [Description("Determines whether the selected date criteria is shown.")]
        public bool ShowCriteria
        {
            get
            {
                return layoutControlItem2.Visible;
            }
            set
            {
                //lblSelectedCriteria.Visible = value;
                if (!value)
                {//it is hidden, reduce the height
                    layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.Height -= 12;
                }
                else//it is shown, increase the height
                    layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.Height += 12;
            }
        }

        private void dtFrom_EditValueChanged(object sender, EventArgs e)
        {
            if (dtFrom.DateTime > dtTo.DateTime)
            {//the to date must be at least the from date
                dtTo.DateTime = dtFrom.DateTime;
            }
            lblSelectedCriteria.Text = string.Format("From {0} to {1}", dtFrom.DateTime.ToShortDateString(), dtTo.DateTime.ToShortDateString());
        }

        private void dtTo_EditValueChanged(object sender, EventArgs e)
        {
            if (dtTo.DateTime < dtFrom.DateTime)
            {//the from date must be utmost the to date
                dtFrom.DateTime = dtTo.DateTime;
            }
            lblSelectedCriteria.Text = string.Format("From {0} to {1}", dtFrom.DateTime.ToShortDateString(), dtTo.DateTime.ToShortDateString());
        }
    }

    public class DateFilterEventArgs
    {
        private DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        private DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        public DateFilterEventArgs(DateTime from, DateTime to)
        {
            StartDate = from;
            EndDate = to;
        }
    }
}
