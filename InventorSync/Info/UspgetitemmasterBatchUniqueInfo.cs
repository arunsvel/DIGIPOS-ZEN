using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspgetitemmasterBatchUniqueInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public string BatchUnique
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
