using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiposZen.Info
{
    public class UspGetLedgerInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public decimal LID
        {
            get;
            set;
        }
        public decimal TenantID
        {
            get;
            set;
        }
        public string GroupName
        {
            get;
            set;
        }
        public decimal AccGroupID
        {
            get;
            set;
        }
        #endregion
    }
}
