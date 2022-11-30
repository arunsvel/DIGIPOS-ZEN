using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspInsertDiscountGroupInfo
    {

        #region "Parameters--------------------------------------------------- >> "

        public decimal DiscountGroupID
        {
            get;
            set;
        }
        public string DiscountGroupName
        {
            get;
            set;
        }
        public decimal DiscPer
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
        public decimal TenantID
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


        #endregion
    }
}
