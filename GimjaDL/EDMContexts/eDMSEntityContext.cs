using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaDL
{
    public partial class eDMSEntity
    {
        public eDMSEntity(string conString)
            : base("name=" + conString)
        {
            var type = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
        }
    }
}
