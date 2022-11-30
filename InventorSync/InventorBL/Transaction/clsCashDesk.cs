using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.InventorBL.Transaction
{
    public class clsCashDesk
    {
        public decimal InvID { get; set; }
        public decimal BillAmount { get; set; }
        public decimal LedgerID { get; set; }
        public decimal PaidAmount { get; set; }
        public string MOP { get; set; }

        public decimal TenderAmount { get; set; }
        public decimal Shortage { get; set; }
        public decimal Balance { get; set; }


        public List<clsCashDeskDetail> PaymentDetails;
        public clsCashDesk(decimal invID, decimal billAmount, decimal ledgerID, string mop)
        {
            InvID = invID;
            BillAmount = billAmount;
            LedgerID = ledgerID;
            PaidAmount = 0;
            MOP = mop;

            PaymentDetails = new List<clsCashDeskDetail> { };
        }
        public clsCashDesk()
        {
            InvID = 0;
            BillAmount = 0;
            LedgerID = 0;
            PaidAmount = 0;

            PaymentDetails = new List<clsCashDeskDetail> { };
        }
    }
}