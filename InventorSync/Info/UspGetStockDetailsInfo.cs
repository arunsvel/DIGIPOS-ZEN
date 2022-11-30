using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspGetStockDetailsInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public decimal StockID
        {
            get;
            set;
        }
        public string BatchCode
        {
            get;
            set;
        }
        public double TenantID
        {
            get;
            set;
        }
        public double ItemID
        {
            get;
            set;
        }
        public double CCID
        {
            get;
            set;
        }
        public string BatchUnique
        {
            get;
            set;
        }
        #endregion
    }
}
