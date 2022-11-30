using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspGetCheckedListInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public string IDs
        {
            get;
            set;
        }

        public int TenantId
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }
        #endregion
    }
}
