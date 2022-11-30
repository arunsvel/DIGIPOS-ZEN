using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspGetStockHistoryInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public decimal ItemID
        {
            get;
            set;
        }
        public decimal VchTypID
        {
            get;
            set;
        }
        public decimal BatchUnique
        {
            get;
            set;
        }
        public decimal CostCentreID
        {
            get;
            set;
        }
        public string FromDate
        {
            get;
            set;
        }
        public string ToDate
        {
            get;
            set;
        }
        public decimal TenantID
        {
            get;
            set;
        }
        #endregion
    }
}
