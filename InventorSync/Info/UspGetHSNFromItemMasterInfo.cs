using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspGetHSNFromItemMasterInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public double ItemID
        {
            get;
            set;
        }
        public double TenantID
        {
            get;
            set;
        }
        public double HSNID
        {
            get;
            set;
        }
        public double IGSTTaxPer
        {
            get;
            set;
        }

        #endregion

    }
}
