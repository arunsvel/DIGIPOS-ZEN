using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspGetEmployeeInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public int EmpID
        {
            get;
            set;
        }
        public int TenantID
        {
            get;
            set;
        }
        public bool blnSalesStaff
        {
            get;
            set;
        }
        #endregion
    }
}
