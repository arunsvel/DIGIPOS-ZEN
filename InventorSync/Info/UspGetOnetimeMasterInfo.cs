using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspGetOnetimeMasterInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public int OtmID
        {
            get;
            set;
        }
        public int TenantID
        {
            get;
            set;
        }
        public string OtmType
        {
            get;
            set;
        }
        public string OtmIds
        {
            get;
            set;
        }
        #endregion
    }
}
