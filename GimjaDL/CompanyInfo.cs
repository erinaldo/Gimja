using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaDL
{
    public partial class CompanyInfo
    {
        public static CompanyInfo GetCompanyInfo(string engName)
        {
            using (eDMSEntity db = new eDMSEntity("eDMSEntities"))
            {
                CompanyInfo _retValue = new CompanyInfo();
                var _companyInfo = db.CompanyInfoes.Where(c => c.englishName.ToLower() == engName.ToLower()).ToList();
                if (_companyInfo != null && _companyInfo.Count > 0)
                {
                    _retValue = _companyInfo.First();
                }
                else
                {
                    _retValue = null;
                }

                return _retValue;
            }
        }


    }
}
