using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaDL
{
    public partial class Customer
    {
        public string FullName
        {
            get
            {
                return string.Format("{0} {1} {2}", name, fatherName, grandfatherName);
            }
        }
    }
}
