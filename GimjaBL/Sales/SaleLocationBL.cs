using GimjaDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class SaleLocationBL
    {
        public short SalesLocationID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public static IEnumerable<SaleLocationBL> GetActiveSaleLocations()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _locations = from l in db.SalesLocations
                                 where l.isActive
                                 select l;

                List<SaleLocationBL> _retValue = new List<SaleLocationBL>();
                foreach (var l in _locations.ToList())
                {
                    _retValue.Add(new SaleLocationBL()
                    {
                        SalesLocationID = l.lkSalesLocationID,
                        Name = l.name,
                        IsActive = l.isActive
                    });
                }
                return _retValue;
            }
        }
        /// <summary>
        /// gets a sales location known by the name warehouse
        /// </summary>
        /// <returns></returns>
        public SaleLocationBL GetWarehouseLocation()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _locations = (from l in db.SalesLocations
                                  where l.name.ToLower().Equals("warehouse")
                                  select l).FirstOrDefault();

                if (_locations != null)
                    return new SaleLocationBL()
                    {
                        SalesLocationID = _locations.lkSalesLocationID,
                        Name = _locations.name,
                        IsActive = _locations.isActive
                    };
                else
                    return null;
            }
        }
    }
}
