using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MatGo
{
    public partial class HowToform : Form
    {
        public HowToform()
        {
            InitializeComponent();
        }

        private void BackToTheStartPageBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            MatGoMain main = new MatGoMain();
            main.ShowDialog();
            this.Close();
        }
    }
}


