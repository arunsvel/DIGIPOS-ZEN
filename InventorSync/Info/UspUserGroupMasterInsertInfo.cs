using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspUserGroupMasterInsertInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public string GroupName
        {
            get;
            set;
        }
        public string AccessLevel
        {
            get;
            set;
        }
        public string StrCCID
        {
            get;
            set;
        }
        public string RptAccesslevel
        {
            get;
            set;
        }
        public decimal ID
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
        public float BillDisc
        {
            get;
            set;
        }
        public float ItemDisc
        {
            get;
            set;
        }
        public float CashDisc
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
