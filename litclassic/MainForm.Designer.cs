namespace litclassic
{
    partial class MainForm
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
            this.buttonBookAd = new System.Windows.Forms.Button();
            this.labelQuote = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.labelLoading = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // buttonBookAd
            // 
            this.buttonBookAd.Location = new System.Drawing.Point(6, 35);
            this.buttonBookAd.Name = "buttonBookAd";
            this.buttonBookAd.Size = new System.Drawing.Size(135, 41);
            this.buttonBookAd.TabIndex = 0;
            this.buttonBookAd.Text = "Добавить книгу";
            this.buttonBookAd.UseVisualStyleBackColor = true;
            this.buttonBookAd.Click += new System.EventHandler(this.buttonBookAd_Click);
            // 
            // labelQuote
            // 
            this.labelQuote.Location = new System.Drawing.Point(153, 9);
            this.labelQuote.Name = "labelQuote";
            this.labelQuote.Size = new System.Drawing.Size(296, 607);
            this.labelQuote.TabIndex = 1;
            this.labelQuote.Text = "label1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 82);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(135, 41);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(6, 129);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(135, 41);
            this.button3.TabIndex = 3;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(6, 176);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(135, 41);
            this.button4.TabIndex = 4;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // labelLoading
            // 
            this.labelLoading.Location = new System.Drawing.Point(3, 9);
            this.labelLoading.Name = "labelLoading";
            this.labelLoading.Size = new System.Drawing.Size(135, 23);
            this.labelLoading.TabIndex = 5;
            this.labelLoading.Text = "label2";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 625);
            this.Controls.Add(this.labelLoading);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.labelQuote);
            this.Controls.Add(this.buttonBookAd);
            this.Name = "MainForm";
            this.Text = "Form2";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonBookAd;
        private System.Windows.Forms.Label labelQuote;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label labelLoading;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}