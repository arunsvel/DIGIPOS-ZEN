using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspGetItemMasterFromStockInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public decimal StockID
        {
            get;
            set;
        }
        public int TenantID
        {
            get;
            set;
        }

        public string ItmConvType
        {
            get;
            set;
        }
        #endregion
    }
}
