﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorSync.Info
{
    public class UspPriceListInfo
    {
        #region "Parameters--------------------------------------------------- >> "
        public int InvID
        {
            get;
            set;
        }
        public string InvNo
        {
            get;
            set;
        }
        public DateTime InvDate
        {
            get;
            set;
        }
        public int VchtypeID
        {
            get;
            set;
        }
        public string Prefix
        {
            get;
            set;
        }
        public int SalesManID
        {
            get;
            set;
        }
        public string Narration
        {
            get;
            set;
        }
        public int TenantID
        {
            get;
            set;
        }
        public int UserID
        {
            get;
            set;
        }
        public string JsonData
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
