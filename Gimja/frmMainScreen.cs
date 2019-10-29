using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using GimjaBL;
using DevExpress.XtraNavBar;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace Gimja
{
    public partial class frmMainScreen : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //public GimjaBL.UserBL LoginUser { get; set; }
        public UserData LoginUser { get; set; }

        private MenuBL menuBL;

        private XtraForm form;

        public String UserID
        {
            get;
            set;
        }

        public frmMainScreen()
        {
            InitializeComponent();

            menuBL = new MenuBL();
        }

        #region Methods

        private void PopulateNavigationBar()
        {
            var _navigationMenu = menuBL.GetNavigationMenu(UserID);

            foreach (var _menu in _navigationMenu)
            {
                //populates NavBar items
                if (_menu.MenuType.ToLower() == "group")
                {
                    NavBarGroup _navGroup = new NavBarGroup(_menu.Caption);
                    _navGroup.SmallImage = _menu.Icon != null ? Image.FromStream(menuBL.ByteToImage(_menu.Icon)) : null;
                    _navGroup.Visible = _menu.Visible;
                    _navGroup.Tag = _menu.ID;
                    //_navGroup.Enabled = !_menu.Disabled;

                    navBarMain.Groups.Add(_navGroup);

                    if (_menu.ID.Equals(1))
                        _navGroup.Expanded = true;

                    var _menuItem = from mi in menuBL.GetNavigationMenu(UserID) where mi.Parent == _menu.ID select mi;

                    foreach (var _rs in _menuItem)
                    {
                        NavBarItem _navGroupItem = new NavBarItem(_rs.Caption);
                        _navGroupItem.SmallImage = _rs.Icon != null ? Image.FromStream(menuBL.ByteToImage(_rs.Icon)) : null;
                        _navGroupItem.Visible = _rs.Visible;
                        _navGroupItem.Enabled = !_rs.Disabled;
                        _navGroupItem.Tag = _rs.ID;
                        _navGroup.ItemLinks.Add(_navGroupItem);
                    }
                }

                //populates Ribbon items
                else if (_menu.MenuType.ToLower() == "page")
                {
                    RibbonPage _ribbonPage = new RibbonPage(_menu.Caption);
                    _ribbonPage.Visible = _menu.Visible;
                    _ribbonPage.Tag = _menu.ID;

                    var _pageGroup = from pg in menuBL.GetNavigationMenu(UserID) where pg.Parent == _menu.ID select pg;

                    foreach (var _pg in _pageGroup)
                    {
                        RibbonPageGroup _ribbonPageGroup = new RibbonPageGroup(_pg.Caption);
                        _ribbonPageGroup.Visible = _pg.Visible;
                        _ribbonPageGroup.Enabled = !_pg.Disabled;
                        _ribbonPageGroup.Tag = _pg.ID;

                        var _barButtonSubItem = from bbsi in menuBL.GetNavigationMenu(UserID) where bbsi.Parent == _pg.ID select bbsi;

                        //populates button sub item(s)
                        foreach (var _bbsi in _barButtonSubItem)
                        {
                            if (_bbsi.MenuType.ToLower() == "button item")
                            {
                                BarButtonItem _barButtonItem = new BarButtonItem();
                                _barButtonItem.Caption = _bbsi.Caption;
                                _barButtonItem.Tag = _bbsi.ID;
                                _barButtonItem.Visibility = _bbsi.Visible ? BarItemVisibility.Always : BarItemVisibility.Never;
                                _barButtonItem.Enabled = !_bbsi.Disabled;
                                _barButtonItem.Glyph = _bbsi.Icon != null ? Image.FromStream(menuBL.ByteToImage(_bbsi.Icon)) : null;
                                _barButtonItem.RibbonStyle = RibbonItemStyles.Large;

                                _barButtonItem.ItemClick += new ItemClickEventHandler(ribbonMain_ItemClick);

                                _ribbonPageGroup.ItemLinks.Add(_barButtonItem);
                            }
                            //populates sub item(s) and menu items
                            else if (_bbsi.MenuType.ToLower() == "sub item")
                            {
                                BarSubItem _barSubItem = new BarSubItem();

                                _barSubItem.Caption = _bbsi.Caption;
                                _barSubItem.Tag = _bbsi.ID;
                                _barSubItem.Glyph = _bbsi.Icon != null ? Image.FromStream(menuBL.ByteToImage(_bbsi.Icon)) : null;
                                _barSubItem.Enabled = !_bbsi.Disabled;
                                _barSubItem.Visibility = _bbsi.Visible ? BarItemVisibility.Always : BarItemVisibility.Never;
                                _barSubItem.RibbonStyle = RibbonItemStyles.Large;

                                var _subItemButtonItem = from bi in menuBL.GetNavigationMenu(UserID) where bi.Parent == _bbsi.ID select bi;

                                foreach (var _sibi in _subItemButtonItem)
                                {
                                    if (_sibi.MenuType.ToLower() == "menu item")
                                    {
                                        //TODO: These properties are not working ...
                                        BarButtonItem _barSubItemButtonItem = new BarButtonItem();
                                        _barSubItemButtonItem.Caption = _sibi.Caption;
                                        _barSubItemButtonItem.Tag = _sibi.ID;
                                        _barSubItemButtonItem.Glyph = _sibi.Icon != null ? Image.FromStream(menuBL.ByteToImage(_sibi.Icon)) : null;
                                        _barSubItemButtonItem.Enabled = !_sibi.Disabled;
                                        _barSubItemButtonItem.Visibility = _sibi.Visible ? BarItemVisibility.Always : BarItemVisibility.Never;
                                        //_barSubItemButtonItem.ButtonStyle = BarButtonStyle.DropDown;

                                        //TODO: add icon here ...
                                        //if(_sibi.Icon!=null)
                                        //{
                                        //ImageList imgList = new ImageList();

                                        //imgList.Images.Add(_sibi.Icon != null ? Image.FromStream(menuBL.ByteToImage(_sibi.Icon)) : null);

                                        //TODO: Event Handler...
                                        _barSubItemButtonItem.ItemClick += new ItemClickEventHandler(ribbonMain_ItemClick);
                                        //_barSubItem.ItemLinks.Add(new BarButtonItem(ribbonMain.Manager, _barSubItemButtonItem.Caption));
                                        _barSubItemButtonItem.Manager = ribbonMain.Manager;
                                        _barSubItem.ItemLinks.Add(_barSubItemButtonItem);
                                    }
                                }
                                _ribbonPageGroup.ItemLinks.Add(_barSubItem);
                                _ribbonPageGroup.AllowTextClipping = false;
                            }
                        }

                        _ribbonPage.Groups.Add(_ribbonPageGroup);
                    }
                    ribbonMain.Pages.Add(_ribbonPage);
                }
            }
        }

        private bool IsAlreadyOpened(Form frm)
        {
            bool result = false;
            Form[] openedForms = this.MdiChildren;
            foreach (var f in openedForms)
            {
                if (f.GetType() == frm.GetType())
                {
                    result = true;
                    f.Activate();
                    if (f.WindowState == FormWindowState.Minimized)
                        f.WindowState = FormWindowState.Normal;
                    break;
                }
            }
            return result;
        }

        private void OpenForm(int menuId)//string selectedMenu)
        {
            string _formID = menuBL.GetFormToShow(menuId);

            if (!String.IsNullOrEmpty(_formID))
            {
                SplashScreenManager.ShowForm(typeof(WaitForm1), true, true);

                Type _formType = Type.GetType(_formID, true, true);
                form = (XtraForm)Activator.CreateInstance(_formType);
                
                if (!IsAlreadyOpened(form))
                {
                    form.MdiParent = this;
                    form.WindowState = FormWindowState.Maximized;
                    form.Show();
                }

                SplashScreenManager.CloseForm();
            }
        }

        private void CalledAfterLogin()
        {
            Singleton.Instance.UserID = UserID;

            PopulateNavigationBar();

            SplashScreenManager.CloseForm();

            barLoggedInUser.Caption = String.Format("Logged in as: {0}", Singleton.Instance.UserID);
            barCurrentDate.Caption = String.Format("Today is {0} ", DateTime.Now.ToString("D"));
        }

        #endregion

        #region Event Handlers

        private void navBarMain_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            NavBarItem _item = e.Link.Item as NavBarItem;
            if (_item != null)
            {
                int _buttonItemId = Convert.ToInt32(_item.Tag);

                OpenForm(_buttonItemId);
            }
        }

        private void ribbonMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarButtonItem _item = e.Item as BarButtonItem;
            if (_item != null)
            {
                int _buttonItemId = Convert.ToInt32(_item.Tag);
                //OpenForm(e.Link.Caption);
                OpenForm(_buttonItemId);
            }
        }

        private void frmMainScreen_Load(object sender, EventArgs e)
        {
            CalledAfterLogin();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            barCurrentTime.Caption = DateTime.Now.ToString("T");
        }

        #endregion
    }
}