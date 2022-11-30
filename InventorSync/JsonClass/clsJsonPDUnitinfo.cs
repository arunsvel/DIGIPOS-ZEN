using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.JsonClass
{
    public class clsJsonPDUnitinfo
    {
        public decimal UnitID { get; set; }
        public string UnitName { get; set; }
        public string UnitShortName { get; set; }
        public decimal TenantID { get; set; }
    }
}
