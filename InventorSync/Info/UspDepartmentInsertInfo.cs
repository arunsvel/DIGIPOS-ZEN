using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiposZen.Info
{
    public class UspDepartmentInsertInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public Int32 DepartmentID
        {
            get;
            set;
        }
        public string Department
        {
            get;
            set;
        }
        public string Description
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
        public int DepartmentType
        {
            get;
            set;
        }
        #endregion
    }
}
