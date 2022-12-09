using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiposZen.Info
{
    public class UspGetItemMasterInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public decimal ItemID
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
