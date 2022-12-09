using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiposZen.Info
{
    public class UspInsertCategoryInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public decimal CategoryID
        {
            get;
            set;
        }
        public string Category
        {
            get;
            set;
        }
        public string Remarks
        {
            get;
            set;
        }
        public string ParentID
        {
            get;
            set;
        }
        public string HID
        {
            get;
            set;
        }
        public decimal CatDiscPer
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
        public decimal TenantId
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
