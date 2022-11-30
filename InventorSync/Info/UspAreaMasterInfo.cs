using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using InventorSync.InventorBL.Master;
using InventorSync.InventorBL.Helper;
using InventorSync.Info;
namespace InventorSync.Info
{
  
    class UspAreaMasterInfo
    {
        #region "Parameters--------------------------------------------------- >> "
         public decimal AreaID
        {
            get;
            set;
         }
        public string Area
        {
            get;
            set;
        }
        public string Remarks
        {
            get;
            set;
        }
        public string ParentID
        {
            get;
            set;
        }
        public string HID
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
        #endregion
    }
}
