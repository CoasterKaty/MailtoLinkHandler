using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailtoHandler
{
    public partial class frmPick : Form
    {
        public frmPick()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rad365.Checked)
            {
                this.DialogResult = (chkDontAsk.Checked ? DialogResult.Yes : DialogResult.OK);
            }
            else
            {
                this.DialogResult = (chkDontAsk.Checked ? DialogResult.No : DialogResult.Cancel);
            }
            this.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            rad365.Checked = true;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            rad2016.Checked = true;
        }

        private void frmPick_Load(object sender, EventArgs e)
        {

        }
    }
}
