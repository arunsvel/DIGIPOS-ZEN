using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspInsertStateInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public decimal StateId
        {
            get;
            set;
        }
        public string StateCode
        {
            get;
            set;
        }
        public string State
        {
            get;
            set;
        }
        public string StateType
        {
            get;
            set;
        }
        public string Country
        {
            get;
            set;
        }
        public decimal CountryID
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
