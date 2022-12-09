using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiposZen.InventorBL.Transaction
{
    public class clsCashDeskDetail
    {
        public string PaymentType { get; set; }
        public int PaymentID { get; set; }
        public int LedgerID { get; set; }
        public decimal Amount { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal TotalOutstanting { get; set; }
        public decimal CurrentReceipt { get; set; }
        public decimal CurrentBalance { get; set; }

        public clsCashDeskDetail(string paymenttype, int paymentid, int ledgerid, decimal amount, decimal previousBalance, decimal totalOutstanting, decimal currentReceipt, decimal currentBalance)
        {
            PaymentType = paymenttype;
            PaymentID = paymentid;
            LedgerID = ledgerid;
            Amount = amount;
            PreviousBalance = previousBalance;
            TotalOutstanting = totalOutstanting;
            CurrentReceipt = currentReceipt;
            CurrentBalance = currentBalance;
        }
        public clsCashDeskDetail()
        {
            PaymentType = "";
            PaymentID = 0;
            LedgerID = 0;
            Amount = 0;
            PreviousBalance = 0;
            TotalOutstanting = 0;
            CurrentReceipt = 0;
            CurrentBalance = 0;
        }

    }
}