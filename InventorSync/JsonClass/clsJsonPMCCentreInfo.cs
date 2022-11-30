using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.JsonClass
{
    public class clsJsonPMCCentreInfo
    {
        public decimal CCID { get; set; }
        public string CCName { get; set; }
        public string InCharge { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public int BLNDAMAGED { get; set; }
        //Dipu 21-03-2022 ------- >>
        //public string SystemName { get; set; }
        //public int UserID { get; set; }
        //public DateTime LastUpdateDate { get; set; }
        //public DateTime LastUpdateTime { get; set; }
        public int TenantID { get; set; }
        public int Action { get; set; }

    }
}
