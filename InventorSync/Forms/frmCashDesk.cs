using DigiposZen.InventorBL.Helper;
using DigiposZen.InventorBL.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigiposZen.Forms
{
    public partial class frmCashDesk : Form
    {

        public clsCashDesk mcashdesk;

        Common Comm = new Common();

        public frmCashDesk(clsCashDesk cashdesk)
        {
            InitializeComponent();
            mcashdesk = cashdesk;
        }

        private void frmCashDesk_Load(object sender, EventArgs e)
        {
            try
            {
                lblMop.Tag = "";
                lblMop.Text = mcashdesk.MOP;
                if (mcashdesk.MOP == "Credit")
                {
                    lblBalance.Visible = false;
                    txtAmount.Visible = false;
                    btnAdd.Visible = false;
                    label4.Visible = false;

                    flpPaymentModes.Visible = false;
                    this.tableLayoutPanel2.ColumnStyles[0].SizeType = SizeType.Absolute;
                    this.tableLayoutPanel2.ColumnStyles[0].Width = 10;

                    txtPreviousBalance.Text = Comm.GetLedgerBalance(Comm.ToInt32(mcashdesk.LedgerID), DateTime.Today).ToString();
                }
                else if (mcashdesk.MOP == "Cash")
                {
                    panel5.Visible = false;
                    lblMop.Text = "CASH";

                    flpPaymentModes.Visible = false;
                    this.tableLayoutPanel2.ColumnStyles[0].SizeType = SizeType.Absolute;
                    this.tableLayoutPanel2.ColumnStyles[0].Width = 10;
                }
                else if (mcashdesk.MOP == "Mixed")
                {
                    panel5.Visible = false;
                    lblMop.Text = "CASH";
                }
                int[] myCurrencies = { 1, 2, 5, 10, 20, 50, 100, 200, 500, 2000 };

                txtBillAmount.Text = Comm.ToDecimal(mcashdesk.BillAmount).ToString();
                //lblMop.Text = mcashdesk.MOP;

                decimal ta = Comm.ToDecimal(txtBillAmount.Text.Replace(" ", ""));
                decimal ta1 = Math.Ceiling(ta);

                int amount = decimal.ToInt32(ta1);

                int[] ProposedAmounts = new int[20];
                int PropIncr = 0;

                int[] PlaceValues = new int[10];
                int PlaceValueLimit = 0;

                string PropFigure = "";

                int LastFigure = amount;

                PropIncr = 0;
                ProposedAmounts[PropIncr] = amount;
                PropIncr++;

                int SeparatedAmount = 0;
                if (amount > 2000) SeparatedAmount = amount - (amount % 2000);

                int ConvDigit = 0;

                int ConvFactor = 1;

                int CurrencyCheck = 0;
                int CurrentCurrency = 0;

                int ConvertingAmount = 0;

                int i = 1;

                try
                {
                    while (PropIncr <= 9)
                    {
                        if (PropIncr >= 9) break;

                        if (ProposedAmounts[PropIncr - 1] == 2000) break;
                        if (ProposedAmounts[PropIncr - 1] > 2000)
                        {

                            if (ProposedAmounts[PropIncr - 1] % 500 == 0)
                                break;
                        }
                     
                        ConvertingAmount = ProposedAmounts[PropIncr - 1] - SeparatedAmount;

                        ConvDigit = Comm.ToInt32(ConvertingAmount.ToString().Substring(ConvertingAmount.ToString().Length - i, 1));
                        if (ConvDigit == 1)
                        {
                            CurrentCurrency = ProposedAmounts[PropIncr - 1] - SeparatedAmount;
                            CurrencyCheck = Comm.ToInt32(ProposedAmounts[PropIncr - 1].ToString().Substring(ProposedAmounts[PropIncr - 1].ToString().Length - i, i));
                            CurrentCurrency = CurrentCurrency - CurrencyCheck;

                            CurrencyCheck = CurrencyCheck + ((2 * ConvFactor) - (ConvDigit * ConvFactor)); //lastdigit will become 2

                            if (CurrencyCheck == 1000 && ProposedAmounts[PropIncr - 1] < 1000) // || CurrencyCheck == 10000 || CurrencyCheck == 100000 || CurrencyCheck == 1000000 || CurrencyCheck == 10000000 || CurrencyCheck == 100000000 || CurrencyCheck == 1000000000)
                                CurrencyCheck = CurrencyCheck * 2;

                            ProposedAmounts[PropIncr] = SeparatedAmount + CurrentCurrency + CurrencyCheck; // ProposedAmounts[PropIncr - 1] + ((2 * ConvFactor) - (ConvDigit * ConvFactor)); //lastdigit will become 2

                            if (ProposedAmounts[PropIncr] == 2000) break;
                            if (ProposedAmounts[PropIncr] > 2000)
                            {
                                if (ProposedAmounts[PropIncr] % 500 == 0)
                                    break;
                            }

                            PropIncr++;
                            if (PropIncr >= 9) break;

                            ConvertingAmount = ProposedAmounts[PropIncr - 1] - SeparatedAmount;
                            ConvDigit = Comm.ToInt32(ConvertingAmount.ToString().Substring(ConvertingAmount.ToString().Length - i, 1));
                        }
                        if (ConvDigit >= 2 && ConvDigit < 5)
                        {
                            CurrentCurrency = ProposedAmounts[PropIncr - 1] - SeparatedAmount;
                            CurrencyCheck = Comm.ToInt32(ProposedAmounts[PropIncr - 1].ToString().Substring(ProposedAmounts[PropIncr - 1].ToString().Length - i, i));
                            CurrentCurrency = CurrentCurrency - CurrencyCheck;

                            CurrencyCheck = CurrencyCheck + ((5 * ConvFactor) - (ConvDigit * ConvFactor));

                            if (CurrencyCheck == 1000 && ProposedAmounts[PropIncr - 1] < 1000)
                                CurrencyCheck = CurrencyCheck * 2;

                            ProposedAmounts[PropIncr] = SeparatedAmount + CurrentCurrency + CurrencyCheck; // ProposedAmounts[PropIncr - 1] + ((5 * ConvFactor) - (ConvDigit * ConvFactor)); //lastdigit will become 5

                            if (ProposedAmounts[PropIncr] == 2000) break;
                            if (ProposedAmounts[PropIncr] > 2000)
                            {
                                if (ProposedAmounts[PropIncr] % 500 == 0)
                                    break;
                            }

                            PropIncr++;
                            if (PropIncr >= 9) break;

                            ConvertingAmount = ProposedAmounts[PropIncr - 1] - SeparatedAmount;
                            ConvDigit = Comm.ToInt32(ConvertingAmount.ToString().Substring(ConvertingAmount.ToString().Length - i, 1));
                        }
                        if (ConvDigit >= 5 && ConvDigit < 10)
                        {
                            CurrentCurrency = ProposedAmounts[PropIncr - 1] - SeparatedAmount;
                            CurrencyCheck = Comm.ToInt32(ProposedAmounts[PropIncr - 1].ToString().Substring(ProposedAmounts[PropIncr - 1].ToString().Length - i, i));
                            CurrentCurrency = CurrentCurrency - CurrencyCheck;

                            CurrencyCheck = CurrencyCheck + ((10 * ConvFactor) - (ConvDigit * ConvFactor));

                            if (CurrencyCheck == 1000 && ProposedAmounts[PropIncr - 1] < 1000)
                                CurrencyCheck = CurrencyCheck * 2;

                            ProposedAmounts[PropIncr] = SeparatedAmount + CurrentCurrency + CurrencyCheck; // ProposedAmounts[PropIncr - 1] + ((10 * ConvFactor) - (ConvDigit * ConvFactor)); //lastdigit will become 0

                            if (ProposedAmounts[PropIncr] == 2000) break;
                            if (ProposedAmounts[PropIncr] > 2000)
                            {
                                if (ProposedAmounts[PropIncr] % 500 == 0)
                                    break;
                            }

                            PropIncr++;
                            if (PropIncr >= 9) break;

                            ConvertingAmount = ProposedAmounts[PropIncr - 1] - SeparatedAmount;
                            ConvDigit = Comm.ToInt32(ConvertingAmount.ToString().Substring(ConvertingAmount.ToString().Length - i, 1));
                        }

                        if (ProposedAmounts[PropIncr - 1] > 2000)
                        {
                            if (ProposedAmounts[PropIncr - 1] % 500 == 0)
                                break;
                        }


                        //if (ProposedAmounts[PropIncr - 1] % 500 == 0)
                        //    break;
                        //When the digit is rounded to 10, then i is incremented so that next time loop runs 2 digits will be separated
                        i++;
                        ConvFactor *= 10;

                        if (PropIncr >= 9) break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Cash Desk", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


                if (ProposedAmounts[0] != 0)
                {
                    btn1.Text = ProposedAmounts[0].ToString();
                }
                else
                {
                    btn1.Visible = false;
                }
                if (ProposedAmounts[1] != 0)
                {
                    btn2.Text = ProposedAmounts[1].ToString();
                }
                else
                {
                    btn2.Visible = false;
                }
                if (ProposedAmounts[2] != 0)
                {
                    btn3.Text = ProposedAmounts[2].ToString();
                }
                else
                {
                    btn3.Visible = false;
                }
                if (ProposedAmounts[3] != 0)
                {
                    btn4.Text = ProposedAmounts[3].ToString();
                }
                else
                {
                    btn4.Visible = false;
                }
                if (ProposedAmounts[4] != 0)
                {
                    btn5.Text = ProposedAmounts[4].ToString();
                }
                else
                {
                    btn5.Visible = false;
                }
                if (ProposedAmounts[5] != 0)
                {
                    btn6.Text = ProposedAmounts[5].ToString();
                }
                else
                {
                    btn6.Visible = false;
                }
                if (ProposedAmounts[6] != 0)
                {
                    btn7.Text = ProposedAmounts[6].ToString();
                }
                else
                {
                    btn7.Visible = false;
                }
                if (ProposedAmounts[7] != 0)
                {
                    btn8.Text = ProposedAmounts[7].ToString();
                }
                else
                {
                    btn8.Visible = false;
                }
                if (ProposedAmounts[8] != 0)
                {
                    btn9.Text = ProposedAmounts[8].ToString();
                }
                else
                {
                    btn9.Visible = false;
                }

                txtTotal.Text = "0";

                string commandText = "Select PaymentID,PaymentType,LedgerID From tblCashDeskMaster ORDER BY PaymentID";

                DataTable dt = Comm.fnGetData(commandText).Tables[0];

                int btnName = 0;
                if (dt.Rows.Count > 0)
                {
                    if (mcashdesk.MOP == "Cash" || mcashdesk.MOP == "Credit")
                    {
                        btnName++;

                        int PosY1 = 5;
                        int rowCount1 = -1;
                        int numOfRows1 = dt.Rows.Count;
                        for (i = 0; i < numOfRows1; i++)
                        {
                            string btnText = "";
                            rowCount1++;
                            //foreach (DataRow row in dt.Rows)
                            {
                                btnText = dt.Rows[rowCount1]["PaymentType"].ToString();
                            }
                            Button button = new Button();
                            button.Enabled = true;
                            button.Text = btnText;
                            button.BackColor = Color.LightBlue;
                            button.ForeColor = Color.LightPink;
                            button.Width = 130;
                            button.Height = 40;
                            button.Name = "btn_" + dt.Rows[rowCount1]["PaymentID"].ToString();
                            button.Tag = dt.Rows[rowCount1]["LedgerID"].ToString();
                            button.Font = new Font("Tahoma", 11);
                            button.Click += Button_Click;
                            button.Enabled = false;
                            flpPaymentModes.Controls.Add(button);
                            PosY1 += 65;
                        }

                    }
                    else
                    {
                        btnName++;

                        int PosY = 5;
                        int rowCount = -1;
                        int numOfRows = dt.Rows.Count;
                        for (i = 0; i < numOfRows; i++)
                        {
                            string btnText = "";
                            rowCount++;
                            //foreach (DataRow row in dt.Rows)
                            {
                                btnText = dt.Rows[rowCount]["PaymentType"].ToString();
                            }
                            Button button = new Button();
                            button.Enabled = true;
                            button.Text = btnText;
                            button.BackColor = Color.LightSteelBlue;
                            button.ForeColor = Color.Black;
                            button.Width = 130;
                            button.Height = 40;
                            button.Name = "btn_" + dt.Rows[rowCount]["PaymentID"].ToString();
                            button.Tag = dt.Rows[rowCount]["LedgerID"].ToString();
                            button.Font = new Font("Tahoma", 11);
                            button.Click += Button_Click;
                            flpPaymentModes.Controls.Add(button);
                            PosY += 65;
                        }
                    }

                    if (flpPaymentModes.Controls.Count > 0)
                    {

                    }
                }

                //MessageBox.Show(dgvPayments.Rows.Count.ToString());

                var deleteButton = new DataGridViewButtonColumn();
                deleteButton.Name = "dataGridViewDeleteButton";
                deleteButton.Text = "*";
                deleteButton.Width = 13;
                deleteButton.UseColumnTextForButtonValue = true;
                this.dgvPayments.Columns.Add(deleteButton);

                var colPaymentID = new DataGridViewTextBoxColumn();
                colPaymentID.Name = "colPaymentID";
                colPaymentID.Visible = false;
                colPaymentID.ReadOnly = true;
                this.dgvPayments.Columns.Add(colPaymentID);

                var colLedgerID = new DataGridViewTextBoxColumn();
                colLedgerID.Name = "colLedgerID";
                colLedgerID.Visible = false;
                colLedgerID.ReadOnly = true;
                this.dgvPayments.Columns.Add(colLedgerID);

                dgvPayments.AllowUserToResizeRows = false;
                dgvPayments.AllowUserToAddRows = false;

                dgvPayments.AllowUserToResizeColumns = false;
                dgvPayments.ColumnHeadersVisible = false;
                dgvPayments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                if (dgvPayments.Rows.Count == 0)
                {
                    dgvPayments.Rows.Clear();

                }
                decimal a = Comm.ToDecimal(txtPreviousBalance.Text);
                decimal b = Comm.ToDecimal(txtBillAmount.Text);
                decimal c = a + b;
                txtOutstanting.Text = c.ToString();
                txtAmount.Text = txtBillAmount.Text;
                


                string d = "select PaymentType,Amount from tblCashDeskItems join tblCashDeskdetails on tblCashDeskItems.id =tblCashDeskdetails.id where InvID=" + mcashdesk.InvID + "";

                SqlDataAdapter da = new SqlDataAdapter("select PaymentType,Amount,BillAmount,PreviousBalance,TotalOutstanting,CurrentReceipt,CurrentBalance from tblCashDeskItems join tblCashDeskdetails on tblCashDeskItems.id =tblCashDeskdetails.id where InvID=" + mcashdesk.InvID + "", DigiposZen.Properties.Settings.Default.ConnectionString);
                DataTable dt1 = new DataTable();
                da.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    if (dt1.Rows[0]["PaymentType"].ToString() == "CREDIT")
                    {
                        txtPreviousBalance.Text = dt1.Rows[0]["PreviousBalance"].ToString();
                        txtOutstanting.Text= dt1.Rows[0]["TotalOutstanting"].ToString();

                        txtCurrentReceipt.Text = dt1.Rows[0]["CurrentReceipt"].ToString();
                        txtCurrentBalance.Text= dt1.Rows[0]["CurrentBalance"].ToString();
                    }
                    else
                    {
                        if (Comm.ToDecimal(dt1.Rows[0]["BillAmount"].ToString()) == Comm.ToDecimal(txtBillAmount.Text.ToString()))
                        {
                            for (int j = 0; j < dt1.Rows.Count; j++)
                            {
                                History_addGrid(dt1.Rows[j]["PaymentType"].ToString(), dt1.Rows[j]["Amount"].ToString(), "", 3.ToString(), 0.ToString());
                            }

                        }
                    }
                }
                txtAmount.SelectAll();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string buttonName = button.Text;
            
            lblMop.Text = buttonName;
            lblMop.Tag = button.Name.Replace("btn_", "");
           if(button.Text==lblMop.Text)
            {
                button.BackColor = Color.White;

            }
           else
            {
                button.BackColor=Color.LightSteelBlue;
            }
            txtAmount.Tag = button.Tag;
            if (dgvPayments.Rows.Count == 0)
            {
                if (txtAmount.Text == "")
                {
                    txtAmount.Text = txtBillAmount.Text;
                }
                else
                {
                    if (txtShortage.Text != "")
                    {
                        txtAmount.Text = txtShortage.Text;
                    }
                    else
                    {
                        txtAmount.Text = txtBillAmount.Text;
                    }
                }
                txtAmount.Focus();
            }
            else
            {
                txtAmount.Text = (Comm.ToDecimal(txtBillAmount.Text) - Comm.ToDecimal(txtTotal.Text)).ToString();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


     

        private void btn1_Click(object sender, EventArgs e)
        {
            try
            {
                txtAmount.Text = btn1.Text;
                btnAdd.PerformClick();
            }
            catch (Exception ex)

            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            try
            {
                txtAmount.Text = btn2.Text;
                btnAdd.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            try
            {
                txtAmount.Text = btn3.Text;
                btnAdd.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            try
            {
                txtAmount.Text = btn4.Text;
                btnAdd.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            try
            {
                txtAmount.Text = btn5.Text;
                btnAdd.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            try
            {
                txtAmount.Text = btn6.Text;
                btnAdd.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btn7_Click(object sender, EventArgs e)
        {
            try
            {
                txtAmount.Text = btn7.Text;
                btnAdd.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            try
            {
                txtAmount.Text = btn8.Text;
                btnAdd.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnno8_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    txtCurrentReceipt.Text = txtCurrentReceipt.Text + 8;
                }
                else
                {
                    if (txtAmount.SelectionLength == txtAmount.Text.Length)
                    {
                        txtAmount.Text = 8.ToString();
                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + 8;
                    }
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnno1_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    txtCurrentReceipt.Text = txtCurrentReceipt.Text + 1;
                }
                else
                {
                    if (txtAmount.SelectionLength == txtAmount.Text.Length)
                    {
                        txtAmount.Text = 1.ToString();
                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnno2_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    txtCurrentReceipt.Text = txtCurrentReceipt.Text + 2;
                }
                else
                {
                    if (txtAmount.SelectionLength == txtAmount.Text.Length)
                    {
                        txtAmount.Text = 2.ToString();
                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + 2;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnno3_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    txtCurrentReceipt.Text = txtCurrentReceipt.Text + 3;
                }
                else
                {
                    if (txtAmount.SelectionLength == txtAmount.Text.Length)
                    {
                        txtAmount.Text = 3.ToString();
                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + 3;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnno4_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    txtCurrentReceipt.Text = txtCurrentReceipt.Text + 4;
                }
                else
                {
                    if (txtAmount.SelectionLength == txtAmount.Text.Length)
                    {
                        txtAmount.Text = 4.ToString();
                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + 4;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnno5_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    txtCurrentReceipt.Text = txtCurrentReceipt.Text + 5;
                }
                else
                {
                    if (txtAmount.SelectionLength == txtAmount.Text.Length)
                    {
                        txtAmount.Text = 5.ToString();
                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + 5;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnno6_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    txtCurrentReceipt.Text = txtCurrentReceipt.Text + 6;
                }
                else
                {
                    if (txtAmount.SelectionLength == txtAmount.Text.Length)
                    {
                        txtAmount.Text = 6.ToString();
                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + 6;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnno7_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    txtCurrentReceipt.Text = txtCurrentReceipt.Text + 7;
                }
                else
                {
                    if (txtAmount.SelectionLength == txtAmount.Text.Length)
                    {
                        txtAmount.Text = 7.ToString();
                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + 7;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnno9_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    txtCurrentReceipt.Text = txtCurrentReceipt.Text + 9;
                }
                else
                {
                    if (txtAmount.SelectionLength == txtAmount.Text.Length)
                    {
                        txtAmount.Text = 9.ToString();
                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + 9;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnC_Click(object sender, EventArgs e)
        {
            try
            {   if (lblMop.Text == "Credit")
                {
                    if (txtCurrentReceipt.Text != "")
                    {
                        string founder = txtCurrentReceipt.Text;
                        txtCurrentReceipt.Text = founder.Remove(founder.Length - 1, 1);

                    }
                    txtCurrentReceipt.Focus();
                }
                else
                {
                    if (txtAmount.Text != "")
                    {
                        string founder = txtAmount.Text;
                        txtAmount.Text = founder.Remove(founder.Length - 1, 1);

                    }
                    txtAmount.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void btndot_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    if (txtCurrentReceipt.Text.Contains("."))
                    {

                    }
                    else
                    {
                        txtCurrentReceipt.Text = txtCurrentReceipt.Text + ".";
                    }
                }
                else 
                {
                    if (txtAmount.Text.Contains("."))
                    {

                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + ".";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnno0_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    txtCurrentReceipt.Text = txtCurrentReceipt.Text + "0"; 
                }
                else
                {
                    if (txtAmount.SelectionLength == txtAmount.Text.Length)
                    {
                        txtAmount.Text = 0.ToString();
                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + "0";
                    }
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(dgvPayments.Rows.Count.ToString());
            try
            {
                if (txtShortage.Text != "0")
                {
                    if (txtAmount.Text != "")
                    {
                        if (lblMop.Text == "")
                        {
                            MessageBox.Show("Select Method.....");
                        }
                        else
                        {
                            if ((Comm.ToDecimal(txtBillAmount.Text) - (Comm.ToDecimal(txtTotal.Text))) > 0)
                            {
                                if (lblMop.Text == "CASH")
                                {
                                    History_addGrid(lblMop.Text, txtAmount.Text, txtShortage.Text, 3.ToString(), 0.ToString());

                                }
                                else if (lblMop.Text == "CASH")
                                {
                                    History_addGrid(lblMop.Text, txtAmount.Text, txtShortage.Text, 1.ToString(), 3.ToString());

                                }
                                else
                                {
                                    History_addGrid(lblMop.Text, txtAmount.Text, txtShortage.Text, lblMop.Tag.ToString(), txtAmount.Tag.ToString());
                                }
                                decimal t = Comm.ToDecimal(txtTotal.Text);
                                decimal s = Comm.ToDecimal(txtShortage.Text);
                                decimal a = Comm.ToDecimal(txtBillAmount.Text);
                                if (s > 0)
                                {
                                    lblBalance.Text = "Balance : 0 ";
                                }
                                else
                                {
                                    decimal b = t - a;
                                    txtShortage.Text = "";
                                    lblBalance.Text = "Balance : " + b;

                                }
                            }

                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void History_addGrid(string mop, string Tender, string Balance, string paymentid, string ledgerid)
        {
            try
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(dgvPayments);

                newRow.Cells[0].Value = mop;
                newRow.Cells[1].Value = Tender;
                //newRow.Cells[2].Value = "delete column";
                newRow.Cells[3].Value = paymentid;
                newRow.Cells[4].Value = ledgerid;

                dgvPayments.Rows.Add(newRow);
                dgvPayments.Rows[dgvPayments.Rows.Count - 1].Height = 30;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            try
            {
                Decimal sum = 0;
                for (int i = 0; i < dgvPayments.Rows.Count; ++i)
                {
                    sum += Comm.ToDecimal(dgvPayments.Rows[i].Cells[1].Value);
                }
                txtTotal.Text = sum.ToString();
                if (!string.IsNullOrEmpty(txtBillAmount.Text) || string.IsNullOrEmpty(txtAmount.Text))

                {

                    decimal ba = Comm.ToDecimal(txtBillAmount.Text);
                    decimal a = 0;
                    if (txtTotal.Text != "")
                    {
                        a = Comm.ToDecimal(txtTotal.Text);
                    }
                    else
                    {
                        a = Comm.ToDecimal("0");
                    }
                    txtShortage.Text = (ba - a).ToString();
                    if ((ba - a) > 0)
                    {
                        txtAmount.Text = (ba - a).ToString();
                    }
                    else
                    {
                        txtAmount.Text = "";
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAmount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd_Click(this, new EventArgs());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void frmCashDesk_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void Dataposs()
        {
            mcashdesk.TenderAmount = Comm.ToDecimal(txtTotal.Text.ToString());
            mcashdesk.Shortage = Comm.ToDecimal(txtShortage.Text.ToString());
            mcashdesk.Balance = Comm.ToDecimal(lblBalance.Text.ToString());
            mcashdesk.PaidAmount = Comm.ToDecimal(txtBillAmount.Text.ToString());

            this.DialogResult = DialogResult.OK;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(dgvPayments.Rows.Count.ToString());
            try
            {
                bool blnsettle = false;
                if (txtAmount.Text == "") txtAmount.Text = "0";

                if ((Comm.ToDecimal(txtBillAmount.Text) - Comm.ToDecimal(txtAmount.Text)) != 0)
                {
                    //this condition works when the gpay, phonepay etc is selected, there they won't click the add button
                    if ((Comm.ToDecimal(txtBillAmount.Text) - (Comm.ToDecimal(txtTotal.Text))) > 0 && (Comm.ToDecimal(txtAmount.Text))< (Comm.ToDecimal(txtBillAmount.Text)))
                    {
                        if (mcashdesk.LedgerID > 1000)
                        {
                            DialogResult dlgResult = MessageBox.Show("Shortage Amount of RS " + txtShortage.Text + " Credited to Customer", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                            if (dlgResult.Equals(DialogResult.Yes))
                            {
                                //clsCashDeskDetail cdd = new clsCashDeskDetail("Credit".ToUpper(), 0, 0, Comm.ToDecimal(txtBillAmount.Text), Comm.ToDecimal(txtPreviousBalance.Text), Comm.ToDecimal(txtOutstanting.Text), Comm.ToDecimal(txtCurrentReceipt.Text), Comm.ToDecimal(txtShortage.Text));
                                //mcashdesk.PaymentDetails.Add(cdd);
                                //Dataposs();

                                clsCashDeskDetail cdd = new clsCashDeskDetail(lblMop.Text.ToUpper(), 1, 3, Comm.ToDecimal(txtTotal.Text), 0, 0, 0, Comm.ToDecimal(txtShortage.Text));
                                mcashdesk.PaymentDetails.Add(cdd);
                                Dataposs();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Settled amount is less than billamount");
                        }
                    }
                    else
                    {
                        blnsettle = true;
                    }
                }
                else
                {
                   
                    blnsettle = true;
                }

                if (blnsettle == true)
                {
                    if (dgvPayments.Rows.Count > 0)
                    {
                        if (mcashdesk.MOP.ToUpper() == "CASH")
                        {
                            clsCashDeskDetail cdd = new clsCashDeskDetail(lblMop.Text.ToUpper(), 1, 3, Comm.ToDecimal(txtBillAmount.Text), 0, 0, 0, 0);
                            mcashdesk.PaymentDetails.Add(cdd);
                            Dataposs();
                        }
                        else if (lblMop.Text == "CASH")
                        {
                            if (mcashdesk.PaymentDetails != null) mcashdesk.PaymentDetails.Clear();
                            for (int i = 0; i < dgvPayments.Rows.Count; i++)
                            {
                                clsCashDeskDetail cdd = new clsCashDeskDetail(dgvPayments[0, i].Value.ToString(), Comm.ToInt32(dgvPayments[3, i].Value.ToString()), Comm.ToInt32(dgvPayments[4, i].Value.ToString()), Comm.ToInt32(dgvPayments[1, i].Value.ToString()),0,0,0,0);
                                mcashdesk.PaymentDetails.Add(cdd);
                            }
                        }
                        else
                        {
                            if (mcashdesk.PaymentDetails != null) mcashdesk.PaymentDetails.Clear();
                            for (int i = 0; i < dgvPayments.Rows.Count; i++)
                            {
                                clsCashDeskDetail cdd = new clsCashDeskDetail(dgvPayments[0, i].Value.ToString(), 1, 3, Comm.ToInt32(dgvPayments[1, i].Value.ToString()),0,0,0,0);
                                mcashdesk.PaymentDetails.Add(cdd);
                            }
                        }
                        Dataposs();
                    }
                    else
                    {
                        if (lblMop.Tag.ToString()=="")
                        {
                            if (mcashdesk.MOP.ToUpper() == "CASH")
                            {
                                clsCashDeskDetail cdd = new clsCashDeskDetail(lblMop.Text.ToUpper(), 1, 3, Comm.ToDecimal(txtBillAmount.Text), 0, 0, 0, 0);
                                mcashdesk.PaymentDetails.Add(cdd);
                                Dataposs();
                            }
                            else if (lblMop.Text == "CASH")
                            {
                                clsCashDeskDetail cdd = new clsCashDeskDetail(lblMop.Text.ToUpper(), 1, 3, Comm.ToDecimal(txtBillAmount.Text),0,0,0,0);
                                mcashdesk.PaymentDetails.Add(cdd);
                                Dataposs();
                            }
                            else if (lblMop.Text == "Mixed")
                            {
                                clsCashDeskDetail cdd = new clsCashDeskDetail("Cash".ToUpper(), 1, 3, Comm.ToDecimal(txtBillAmount.Text),0,0,0,0);
                                mcashdesk.PaymentDetails.Add(cdd);
                                Dataposs();
                            }
                            else if (lblMop.Text == "Credit")
                            {
                                clsCashDeskDetail cdd = new clsCashDeskDetail("Credit".ToUpper(), 0, 0, Comm.ToDecimal(txtBillAmount.Text), Comm.ToDecimal(txtPreviousBalance.Text), Comm.ToDecimal(txtOutstanting.Text), Comm.ToDecimal(txtCurrentReceipt.Text), Comm.ToDecimal(txtCurrentBalance.Text));
                                mcashdesk.PaymentDetails.Add(cdd);
                                Dataposs();
                            }
                        }
                        else
                        {
                            clsCashDeskDetail cdd = new clsCashDeskDetail(lblMop.Text.ToUpper(), Comm.ToInt32(lblMop.Tag.ToString()), Comm.ToInt32(txtAmount.Tag.ToString()), Comm.ToDecimal(txtBillAmount.Text),0,0,0,0);
                            mcashdesk.PaymentDetails.Add(cdd);
                            Dataposs();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvPayments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvPayments.Rows[0].Cells["Mop"].Value != null)
                {
                    if (dgvPayments.CurrentCell.ColumnIndex == 2)
                    {
                        DialogResult dlgResult = MessageBox.Show("Your are in the middle of an Entry. Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (dlgResult.Equals(DialogResult.Yes))
                        {

                            dgvPayments.Rows.RemoveAt(dgvPayments.CurrentRow.Index);
                            decimal sum = 0;
                            for (int i = 0; i < dgvPayments.Rows.Count; ++i)
                            {
                                sum += Comm.ToInt32(dgvPayments.Rows[i].Cells[1].Value);
                            }
                            txtTotal.Text = sum.ToString();
                            if (!string.IsNullOrEmpty(txtBillAmount.Text) || string.IsNullOrEmpty(txtAmount.Text))

                            {

                                decimal ba = Comm.ToDecimal(txtBillAmount.Text);
                                decimal a = 0;
                                if (txtTotal.Text != "")
                                {
                                    a = Comm.ToDecimal(txtTotal.Text);
                                }
                                else
                                {
                                    a = Comm.ToDecimal("0");
                                }
                                decimal s = 0;
                                if (txtShortage.Text != "")
                                {
                                    s = Comm.ToDecimal(txtShortage.Text);
                                }
                                else
                                {
                                    s = Comm.ToDecimal("0");
                                }
                                decimal t = Comm.ToDecimal(txtTotal.Text);
                                decimal g = Comm.ToDecimal(txtBillAmount.Text);
                                if (s > 0)
                                {
                                    lblBalance.Text = "Balance : 0 ";
                                    txtShortage.Text = (g - sum).ToString();
                                }
                                else
                                {
                                    decimal b = t - g;
                                    txtShortage.Text = "";
                                    if (b > 0)
                                    {
                                        lblBalance.Text = "Balance : " + b;
                                    }
                                    else
                                    {
                                        lblBalance.Text = "Balance : 0";
                                    }
                                }
                                if ((ba - a) > 0)
                                {
                                    txtAmount.Text = (ba - a).ToString();
                                    txtAmount.Focus();
                                    txtAmount.SelectAll();
                                }
                                else
                                {
                                    txtAmount.Text = "";
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtCurrentReceipt_TextChanged(object sender, EventArgs e)
        {
            if (txtCurrentReceipt.Text != "")
            {
                decimal t = Comm.ToDecimal(txtOutstanting.Text);
                decimal g = Comm.ToDecimal(txtCurrentReceipt.Text);
                txtCurrentBalance.Text = (t - g).ToString();
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
            if (txtAmount.Text != "")
            {
                decimal t = Comm.ToDecimal(txtTotal.Text);
                decimal a = Comm.ToDecimal(txtBillAmount.Text);

                decimal b = t - a;
                //txtShortage.Text = "";
                lblBalance.Text = "Balance : " + b;
            }

        }

        private void btnEsc_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = MessageBox.Show("Your are in the middle of an Entry. Do you want to exit?", Global.gblMessageCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dlgResult.Equals(DialogResult.Yes))
                this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (lblMop.Text == "Credit")
            {
                txtCurrentReceipt.Text = "";
                txtCurrentReceipt.Focus();
            }
            else
            {
                txtAmount.Text = "";
                txtAmount.Focus();
            }
        }

        private void btnno00_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblMop.Text == "Credit")
                {
                    txtCurrentReceipt.Text = txtCurrentReceipt.Text + "00";
                }
                else
                {
                    if (txtAmount.SelectionLength == txtAmount.Text.Length)
                    {
                        txtAmount.Text = 00.ToString();
                    }
                    else
                    {
                        txtAmount.Text = txtAmount.Text + "00";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + System.Reflection.MethodBase.GetCurrentMethod().Name, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            if (txtAmount.Text != "0.00")
            {
                decimal t = 0;
                if (txtAmount.Text == "")
                {
                    t = Comm.ToDecimal("0");
                }
                else
                {
                    t = Comm.ToDecimal(txtAmount.Text);
                }
                decimal a = Comm.ToDecimal(txtBillAmount.Text);

                decimal b = t - a;
                if (txtShortage.Text == "")
                {
                    lblBalance.Text = "Balance : " + b;
                }
                else
                {
                    decimal x = 0;
                    if (txtAmount.Text == "")
                    {
                        x = Comm.ToDecimal("0");
                    }
                    else
                    {
                        x = Comm.ToDecimal(txtAmount.Text);
                    }
                    decimal y = Comm.ToDecimal(txtShortage.Text);

                    decimal Z = x - y;
                    lblBalance.Text = "Balance : " + Z;
                }
            }
        }

        private void txtCurrentReceipt_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void txtTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmCashDesk_Shown(object sender, EventArgs e)
        {
            txtAmount.Focus();
            txtAmount.SelectAll();
        }
    }
}
