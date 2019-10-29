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
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {
        private HashPasswordBL hashPasswordBL;

        private const int MAX_TRIAL = 3;
        private int login_trial = 0;

        public string UserID
        {
            get;
            set;
        }

        public UserData LoggedInUser { get; set; }

        public frmLogin()
        {
            InitializeComponent();

            hashPasswordBL = new HashPasswordBL();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            this.Text = String.Format("{0} - Login", Application.ProductName);
            int startYear = 2014;
            string copyright = DateTime.Now.Year == startYear ? DateTime.Now.Year.ToString() : string.Format("2014 - {0}", DateTime.Now.Year);
            lblCopyright.Text = String.Format("Copyright © {0}. Rani Bearing P.L.C", copyright);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UserID = null;
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtUserID.Text.Trim()) && !String.IsNullOrWhiteSpace(txtPassword.Text.Trim()))
                {
                    if (login_trial < MAX_TRIAL)
                    {
                        string _userID = txtUserID.Text.Trim();
                        string _password = txtPassword.Text.Trim();

                        if (hashPasswordBL.ValidateUser(_userID, _password))
                        {
                            UserID = _userID;
                            
                            UserBL _userBL = new UserBL();
                            
                            LoggedInUser = _userBL.GetUser(_userID);
                            
                            this.Close();
                        }
                        else
                        {
                            GimjaHelper.ShowError("In correct User Name and/or Password", Application.ProductName);

                            login_trial++;
                            txtUserID.SelectAll();
                            txtPassword.Text = String.Empty;

                            txtUserID.Focus();
                        }
                    }
                    else
                    {
                        GimjaHelper.ShowError("Sorry, you reached maximum trial. Good Bye!");
                        Application.Exit();
                    }
                }
                else
                {
                    GimjaHelper.ShowWarning("User Name and/or Password cannot be blank");
                    txtUserID.Focus();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}