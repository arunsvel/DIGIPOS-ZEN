using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiposZen.Info
{
    public class UspTaxModeInsertInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public decimal TaxModeID
        {
            get;
            set;
        }
        public string TaxMode
        {
            get;
            set;
        }
        public decimal CalculationID
        {
            get;
            set;
        }
        public decimal SortNo
        {
            get;
            set;
        }
        public decimal ActiveStatus
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
