using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspInsertColorMasterInfo
    {
        #region "Parameters--------------------------------------------------- >> "

        public decimal ColorID

        {
            get;
            set;
        }
        public string ColorName
        {
            get;
            set;
        }
        public string ColorHexCode
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
