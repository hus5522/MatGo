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
    public partial class Go_or_Stop : Form
    {

        public enum gostop
        {
            GO,
            STOP
        };
        

        public Go_or_Stop()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void btnGo_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            
            this.Close();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.No;

            this.Close();
        }


       
        
    }
}
