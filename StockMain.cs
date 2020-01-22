using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock_Management_Software
{
    public partial class StockMain : Form
    {
        private int childFormNumber = 0;

        public StockMain()
        {
            InitializeComponent();
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Products pro = new Products();
            // show Products screen within StockMain
            pro.MdiParent = this;
            pro.Show();
        }

        private void stockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stock sto = new Stock();
            // show Stock screen within StockMain
            sto.MdiParent = this;
            sto.Show();
        }

        private void StockMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

    }
}
