using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiposZen.JsonClass
{
    public class clsJsonPMStateInfo
    {
        public decimal StateId { get; set; }
        public string StateCode { get; set; }
        public string State { get; set; }
        public string StateType { get; set; }
        public string Country { get; set; }
        public decimal CountryID { get; set; }
        //Dipu 21-03-2022 ------- >>
        //public string SystemName { get; set; }
        //public decimal UserID { get; set; }
        //public DateTime LastUpdateDate { get; set; }
        //public DateTime LastUpdateTime { get; set; }
        public decimal TenantID { get;  set; }
    }
}
