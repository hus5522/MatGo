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
    public partial class ScoreForm : Form
    {
        int whowin;
        int winscore;
        int whochongtong;

        public ScoreForm(int WHOWIN, int WINSCORE, int WHOCHONGTONG)
        {
            InitializeComponent();

            this.whowin = WHOWIN;
            this.winscore = WINSCORE;
            this.whochongtong = WHOCHONGTONG;
        }

        private void ScoreForm_Load(object sender, EventArgs e)
        {
            if (whowin == 1)
            {
                Companel.BackColor = Color.Red;
                Userpanel.BackColor = Color.Cyan;
                
                ComFace.BackgroundImage = Properties.Resources.computer1;
                ComFace.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                UserFace.BackgroundImage = Properties.Resources.user2;
                UserFace.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                
                ComResultMark.BackgroundImage = Properties.Resources.mark_lose;
                ComResultMark.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                UserResultMark.BackgroundImage = Properties.Resources.mark_win;
                UserResultMark.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

                ComPrice.Text = "";
                UserPrice.Text = winscore + "점" + " X 1000 원 = " + winscore * 1000 + "원";
                Drawlabel.Text = "";
            }
            else if (whowin == 2)
            {
                Companel.BackColor = Color.Cyan;
                Userpanel.BackColor = Color.Red;

                ComFace.BackgroundImage = Properties.Resources.computer2;
                ComFace.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                UserFace.BackgroundImage = Properties.Resources.user1;
                UserFace.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

                ComResultMark.BackgroundImage = Properties.Resources.mark_win;
                ComResultMark.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                UserResultMark.BackgroundImage = Properties.Resources.mark_lose;
                UserResultMark.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

                ComPrice.Text = winscore + "점" + " X 1000 원 = " + winscore * 1000 + "원";
                UserPrice.Text = "";
                Drawlabel.Text = "";
            }
            else if (whowin == 3)
            {
                Companel.BackColor = Color.LightGreen;
                Userpanel.BackColor = Color.LightGreen;
                ComPrice.Text = "";
                UserPrice.Text = "";

                ComResultMark.BackgroundImage = Properties.Resources.computer1;
                ComResultMark.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                UserResultMark.Location = new Point(12, 190);
                UserResultMark.BackgroundImage = Properties.Resources.user1;
                UserResultMark.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            }

            if (whochongtong == 1)
                UserChongtonglabel.Text = "User의 총통";

            else if (whochongtong == 2)
                ComChongtonglabel.Text = "Computer의 총통";
        }

        private void ScoreForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
