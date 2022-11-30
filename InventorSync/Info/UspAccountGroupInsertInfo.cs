using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspAccountGroupInsertInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public int AccountGroupID
        {
            get;
            set;
        }
        public string AccountGroup
        {
            get;
            set;
        }
        public string Nature
        {
            get;
            set;
        }
        public decimal MaintainBudget
        {
            get;
            set;
        }
        public decimal SortOrder
        {
            get;
            set;
        }
        public decimal ParentID
        {
            get;
            set;
        }
        public string HID
        {
            get;
            set;
        }
        public int ACTIVESTATUS
        {
            get;
            set;
        }
        public string SystemName
        {
            get;
            set;
        }
        public decimal UserID
        {
            get;
            set;
        }
        public DateTime LastUpdateDate
        {
            get;
            set;
        }
        public DateTime LastUpdateTime
        {
            get;
            set;
        }
        public decimal TenantID
        {
            get;
            set;
        }
        public int Action
        {
            get;
            set;
        }
        #endregion
    }
}
