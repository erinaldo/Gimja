using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gimja
{
    public class GimjaHelper
    {
        public static DialogResult ShowQuestion(string message, string caption)
        {
            var result = XtraMessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            return result;
        }

        /// <summary>
        /// Shows a question with Yes, No and Cancel buttons
        /// </summary>
        /// <param name="message"></param>
        /// <param name="yesNoCancelButtons"></param>
        /// <returns></returns>
        public static DialogResult ShowQuestionWithYesNoCancelButtons(string message)
        {
            var result = XtraMessageBox.Show(message, Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            return result;
        }

        public static DialogResult ShowQuestion(string message)
        {
            var result = XtraMessageBox.Show(message, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            return result;
        }

        public static DialogResult ShowError(string message, string caption = "")
        {
            if (caption == string.Empty) caption = Application.ProductName;
            var result = XtraMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return result;
        }

        public static DialogResult ShowInformation(string message, string caption)
        {
            var result = XtraMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return result;
        }

        public static DialogResult ShowWarning(string message, string caption)
        {
            var result = XtraMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return result;
        }

        public static DialogResult ShowWarning(string message)
        {
            var result = XtraMessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return result;
        }

        public static DialogResult ShowInformation(string p)
        {
            var result = XtraMessageBox.Show(p, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return result;
        }

        public static GimjaBL.UserData GetCurrentUser(Form currentChildForm)
        {
            if (currentChildForm != null && currentChildForm.IsMdiChild)
            {
                var parent = currentChildForm.MdiParent as frmMainScreen;
                if (parent != null)
                    return parent.LoginUser;
            }
            return null;
        }

        public static string GetCurrentUserID(Form currentChildForm)
        {
            if (currentChildForm.IsMdiChild)
            {
                var parent = currentChildForm.MdiParent as frmMainScreen;
                if (parent != null && parent.LoginUser != null)
                    return parent.LoginUser.UserID;
            }
            return null;
        }

        //public static GimjaBL.UserBL GetCurrentUser()
        //{
        //    return null;// frmMainScreen.LoggedInUser;
        //}

        //public static string GetCurrentUserID()
        //{
        //    //if (frmMainScreen.LoggedInUser != null)
        //    //    return frmMainScreen.LoggedInUser.UserID;
        //    //else
        //        return null;
        //}
    }
}
