using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.JsonClass
{
    public class clsJSonAccounts
    {
        public clsJsonACCInfo clsJsonPMInfo_ { get; set; }
        public clsJsonPMLedgerInfo clsJsonPMLedgerInfo_ { get; set; }
        public clsJsonPMCCentreInfo clsJsonPMCCentreInfo_ { get; set; }
        public clsJsonPMEmployeeInfo clsJsonPMEmployeeInfo_ { get; set; }

        public List<clsJsonACCDetailsInfo> clsJsonACCDetailsInfoList_ { get; set; }
        public List<clsJsonPMLedgerInfo> clsJsonACCIteminfoList_ { get; set; }
    }
}
