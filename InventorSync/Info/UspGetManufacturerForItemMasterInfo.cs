using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspGetManufacturerForItemMasterInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public Decimal ItemID
        {
            get;
            set;
        }
        public Decimal TenantID
        {
            get;
            set;
        }
        #endregion
    }
}
