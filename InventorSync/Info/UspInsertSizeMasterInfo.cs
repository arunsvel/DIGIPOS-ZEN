using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspInsertSizeMasterInfo
    {
        #region "Parameters--------------------------------------------------- >> "

        public decimal SizeID
        {
            get;
            set;
        }
        public string SizeName
        {
            get;
            set;
        }
        public string SizeNameShort
        {
            get;
            set;
        }
        public decimal SortOrder
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


