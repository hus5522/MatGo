namespace MatGo
{
    partial class HowToform
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BackToTheStartPageBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BackToTheStartPageBtn
            // 
            this.BackToTheStartPageBtn.BackColor = System.Drawing.Color.Transparent;
            this.BackToTheStartPageBtn.BackgroundImage = global::MatGo.Properties.Resources.BackToTheStartPage2;
            this.BackToTheStartPageBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BackToTheStartPageBtn.FlatAppearance.BorderSize = 0;
            this.BackToTheStartPageBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackToTheStartPageBtn.Location = new System.Drawing.Point(18, 18);
            this.BackToTheStartPageBtn.Name = "BackToTheStartPageBtn";
            this.BackToTheStartPageBtn.Size = new System.Drawing.Size(88, 54);
            this.BackToTheStartPageBtn.TabIndex = 0;
            this.BackToTheStartPageBtn.UseVisualStyleBackColor = false;
            this.BackToTheStartPageBtn.Click += new System.EventHandler(this.BackToTheStartPageBtn_Click);
            // 
            // HowToform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = global::MatGo.Properties.Resources.RulePage;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(698, 853);
            this.Controls.Add(this.BackToTheStartPageBtn);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "HowToform";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Whatoo";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BackToTheStartPageBtn;
    }
}