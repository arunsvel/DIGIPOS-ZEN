using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspGetAccountGroupInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public int AccountGroupID
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
