using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspGetRepackingInfo
    {
        #region "Parameters--------------------------------------------------- >> "

        public decimal InvId
        {
            get;
            set;
        }
        public decimal TenantID
        {
            get;
            set;
        }
        public decimal VchTypeID
        {
            get;
            set;
        }

        #endregion
    }
}
