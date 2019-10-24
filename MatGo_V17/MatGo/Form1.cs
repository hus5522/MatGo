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
    public partial class MatGoMain : Form
    {
        public MatGoMain()
        {
            InitializeComponent();
        }

        //-----------------<StartBtn 이벤트>---------------------------------------------------
        private void StartBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            WhatooPlayform whatoo = new WhatooPlayform();
            whatoo.ShowDialog();
            this.Close();
        }

        //StartBtn 위에 마우스 올렸을 때
        private void StartBtn_MouseMove(object sender, MouseEventArgs e)
        {
            StartBtn.BackgroundImage = MatGo.Properties.Resources.StartBtn2;
        }

        //StartBtn 밖으로 마우스 벗어날 때
        private void StartBtn_MouseLeave(object sender, EventArgs e)
        {
            StartBtn.BackgroundImage = MatGo.Properties.Resources.StartBtn1;
        }

        //-----------------<HowToBtn 이벤트>---------------------------------------------------
        private void HowToBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            HowToform howto = new HowToform();
            howto.ShowDialog();
            this.Close();

        }

        //HowToBtn 위에 마우스 올렸을 때
        private void HowToBtn_MouseMove(object sender, MouseEventArgs e)
        {
            HowToBtn.BackgroundImage = MatGo.Properties.Resources.HowToBtn2;
        }

        //HowToBtn 밖으로 마우스 벗어날 때
        private void HowToBtn_MouseLeave(object sender, EventArgs e)
        {
            HowToBtn.BackgroundImage = MatGo.Properties.Resources.HowToBtn1;
        }

    }
}
