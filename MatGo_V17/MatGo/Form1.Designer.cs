namespace MatGo
{
    partial class MatGoMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartBtn = new System.Windows.Forms.Button();
            this.HowToBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StartBtn
            // 
            this.StartBtn.BackColor = System.Drawing.Color.Transparent;
            this.StartBtn.BackgroundImage = global::MatGo.Properties.Resources.StartBtn1;
            this.StartBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.StartBtn.FlatAppearance.BorderSize = 0;
            this.StartBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.StartBtn.Location = new System.Drawing.Point(85, 608);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(133, 109);
            this.StartBtn.TabIndex = 0;
            this.StartBtn.UseVisualStyleBackColor = false;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            this.StartBtn.MouseLeave += new System.EventHandler(this.StartBtn_MouseLeave);
            this.StartBtn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.StartBtn_MouseMove);
            // 
            // HowToBtn
            // 
            this.HowToBtn.BackColor = System.Drawing.Color.Transparent;
            this.HowToBtn.BackgroundImage = global::MatGo.Properties.Resources.HowToBtn1;
            this.HowToBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.HowToBtn.FlatAppearance.BorderSize = 0;
            this.HowToBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HowToBtn.Location = new System.Drawing.Point(475, 609);
            this.HowToBtn.Name = "HowToBtn";
            this.HowToBtn.Size = new System.Drawing.Size(145, 108);
            this.HowToBtn.TabIndex = 1;
            this.HowToBtn.UseVisualStyleBackColor = false;
            this.HowToBtn.Click += new System.EventHandler(this.HowToBtn_Click);
            this.HowToBtn.MouseLeave += new System.EventHandler(this.HowToBtn_MouseLeave);
            this.HowToBtn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HowToBtn_MouseMove);
            // 
            // MatGoMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::MatGo.Properties.Resources.StartPage;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(698, 853);
            this.Controls.Add(this.HowToBtn);
            this.Controls.Add(this.StartBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MatGoMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Whatoo";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Button HowToBtn;
    }
}

