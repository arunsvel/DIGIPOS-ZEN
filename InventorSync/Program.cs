using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.WinForms;

namespace DigiposZen
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDk1NzU1QDMxMzkyZTMyMmUzMERTN0JSS0kzYlI3NUpPeGVhR3pkWGNnTkhZYUI1MksvM1I4WXpub2l2L1U9");
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt/QHNqVVhkW1pFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF9iSH5XdkZhXnpdd3JQQw==;Mgo+DSMBPh8sVXJ0S0V+XE9AcVRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3xSd0VnWXxcd3FWQWhfVA==;ORg4AjUWIQA/Gnt2VVhjQlFaclhJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRd0RjXX9dcnNQRWdVVkE=;NzU0MDU5QDMyMzAyZTMzMmUzMGEwMFhVWVA4bFVpNy93ZHZqd0xxVUxZbVhiRkpYRzZ2c0xBb2h0SGdhU2s9;NzU0MDYwQDMyMzAyZTMzMmUzMElRMlJMWEdqUWNueDhyaFBlUWd4d0E1eGord2swWHEwRElYTEpJa3U1b0k9;NRAiBiAaIQQuGjN/V0Z+X09EaFtFVmJLYVB3WmpQdldgdVRMZVVbQX9PIiBoS35RdERiWXtec3ZSQ2NUWUNz;NzU0MDYyQDMyMzAyZTMzMmUzMENuRGZVS0h6ZXdlNDBjNDd5TDhLUEh5Q1ZrZ1FjZmxyQ1FQbUFnUFJEQms9;NzU0MDYzQDMyMzAyZTMzMmUzMFp2eWVGUm10WmhJWkZLQldZV1gyUXNJTVdOMStRZU11RHZ4OGhrK1MvOHM9;Mgo+DSMBMAY9C3t2VVhjQlFaclhJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRd0RjXX9dcnNQRWlZWEE=;NzU0MDY1QDMyMzAyZTMzMmUzMEh5K0gvK1JOdmdIcUxMcmI4d01SaVkvSUdTdjhJTEVuMWtzZjloYTlyVzA9;NzU0MDY2QDMyMzAyZTMzMmUzME9zQUFBazQ1SHp4K3VTdThKeXNIbENIeG9UeW5veU9TdCtRenRWaDRhM289;NzU0MDY3QDMyMzAyZTMzMmUzMENuRGZVS0h6ZXdlNDBjNDd5TDhLUEh5Q1ZrZ1FjZmxyQ1FQbUFnUFJEQms9");
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDk1NzU1QDMxMzkyZTMyMmUzMERTN0JSS0kzYlI3NUpPeGVhR3pkWGNnTkhZYUI1MksvM1I4WXpub2l2L1U9");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMDI());
            //Application.Run(new Login());
        }
    }
}
