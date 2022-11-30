using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.JsonClass
{
    public class clsJsonPMAgentInfo
    {
        public decimal AgentID { get; set; }
        public string AgentCode { get; set; }
        public string AgentName { get; set; }
        public string Area { get; set; }
        public decimal Commission { get; set; }
        public int blnPOstAccounts { get; set; }
        public string ADDRESS { get; set; }
        public string LOCATION { get; set; }
        public string PHONE { get; set; }
        public string WEBSITE { get; set; }
        public string EMAIL { get; set; }
        public int BLNROOMRENT { get; set; }
        public int BLNSERVICES { get; set; }
        public int blnItemwiseCommission { get; set; }
        public decimal AgentDiscount { get; set; }
        public int LID { get; set; }
        //Dipu 21-03-2022 ------- >>
        //public string SystemName { get; set; }
        //public int UserID { get; set; }
        //public DateTime LastUpdateDate { get; set; }
        //public DateTime LastUpdateTime { get; set; }
        public int TenantID { get; set; }
    }
}
