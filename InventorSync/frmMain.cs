using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorSync
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnMaster_Click(object sender, EventArgs e)
        {
            if (btnCategory.Visible == true)
            {
                btnCategory.Visible = false;
                btnBrand.Visible = false;
                btnManufacturer.Visible = false;
            }
            else if (btnCategory.Visible == false)
            {
                btnCategory.Visible = true;
                btnBrand.Visible = true;
                btnManufacturer.Visible = true;
            }
        }
    }
}
