using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspGetSizeInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public decimal SizeID
        {
            get;
            set;
        }
        public decimal TenantID
        {
            get;
            set;
        }
        public string SizeIds
        {
            get;
            set;
        }
        
        #endregion
    }
}
