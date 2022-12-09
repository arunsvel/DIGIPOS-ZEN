using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiposZen.Info
{
    public class UspAccVoucherItemInsertInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public int InvID
        {
            get;
            set;
        }
        public decimal LID
        {
            get;
            set;
        }
        public decimal Qty
        {
            get;
            set;
        }
        public decimal Amount
        {
            get;
            set;
        }
        public decimal TaxPer
        {
            get;
            set;
        }
        public decimal TaxAmount
        {
            get;
            set;
        }
        public int SlNo
        {
            get;
            set;
        }
        public decimal ITaxableAmount
        {
            get;
            set;
        }
        public decimal INetAmount
        {
            get;
            set;
        }
        public decimal CGSTTaxPer
        {
            get;
            set;
        }
        public decimal CGSTTaxAmt
        {
            get;
            set;
        }
        public decimal SGSTTaxPer
        {
            get;
            set;
        }
        public decimal SGSTTaxAmt
        {
            get;
            set;
        }
        public decimal IGSTTaxPer
        {
            get;
            set;
        }
        public decimal IGSTTaxAmt
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
