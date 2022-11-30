using InventorSync.InventorBL.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.JsonClass
{
    public class clsJSonSales
    {
        public clsJsonPMInfo clsJsonPMInfo_ { get; set; }
        public clsJsonPMLedgerInfo clsJsonPMLedgerInfo_ { get; set; }
        public clsJsonPMTaxmodeInfo clsJsonPMTaxmodeInfo_ { get; set; }
        public clsJsonPMAgentInfo clsJsonPMAgentInfo_ { get; set; }
        public clsJsonPMCCentreInfo clsJsonPMCCentreInfo_ { get; set; }
        public clsJsonPMStateInfo clsJsonPMStateInfo_ { get; set; }
        public clsJsonPMEmployeeInfo clsJsonPMEmployeeInfo_ { get; set; }

        public List<clsJsonPDetailsInfo> clsJsonPDetailsInfoList_ { get; set; }
        public List<clsJsonPDUnitinfo> clsJsonPDUnitinfoList_ { get; set; }
        public List<clsJsonPDIteminfo> clsJsonPDIteminfoList_ { get; set; }
    }
}
