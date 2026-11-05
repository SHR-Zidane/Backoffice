namespace tounsiYoyo
{
    partial class Backoffice
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Export = new Button();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // Export
            // 
            Export.Location = new Point(642, 381);
            Export.Name = "Export";
            Export.Size = new Size(99, 43);
            Export.TabIndex = 0;
            Export.Text = "Export";
            Export.UseVisualStyleBackColor = true;
            Export.Click += Export_button_click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Franklin Gothic Medium Cond", 20F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(312, 55);
            label1.Name = "label1";
            label1.Size = new Size(147, 34);
            label1.TabIndex = 1;
            label1.Text = "TOUNSI YOYO";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Yoyo2;
            pictureBox1.Location = new Point(255, 107);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(262, 209);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.Tounes;
            pictureBox2.Location = new Point(523, 128);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(264, 167);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
            // 
            // Backoffice
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(Export);
            Controls.Add(pictureBox2);
            Name = "Backoffice";
            Text = "BackOffice";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Export;
        private Label label1;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
    }
}
