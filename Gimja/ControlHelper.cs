using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gimja
{
    public class ControlHelper
    {
        public static void ToggleEnable(System.Windows.Forms.Control ctrl)
        {
            ctrl.Enabled = !ctrl.Enabled;
        }


    }
}
