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
            components = new System.ComponentModel.Container();
            Export = new Button();
            BusinessName = new Label();
            specialBindingSource = new BindingSource(components);
            BusinessLogo = new PictureBox();
            BusinessID = new Label();
            SpecialName = new Label();
            SpecialLogo = new PictureBox();
            SpecialID = new Label();
            SpecialPrice = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)specialBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BusinessLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SpecialLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SpecialPrice).BeginInit();
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
            // BusinessName
            // 
            BusinessName.AutoSize = true;
            BusinessName.Font = new Font("Franklin Gothic Medium Cond", 20F, FontStyle.Regular, GraphicsUnit.Point, 0);
            BusinessName.Location = new Point(23, 42);
            BusinessName.Name = "BusinessName";
            BusinessName.Size = new Size(168, 34);
            BusinessName.TabIndex = 1;
            BusinessName.Text = "Business Name";
            BusinessName.Click += label1_Click;
            // 
            // specialBindingSource
            // 
            specialBindingSource.DataSource = typeof(Model.Special);
            // 
            // BusinessLogo
            // 
            BusinessLogo.Location = new Point(23, 79);
            BusinessLogo.Name = "BusinessLogo";
            BusinessLogo.Size = new Size(219, 164);
            BusinessLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            BusinessLogo.TabIndex = 2;
            BusinessLogo.TabStop = false;
            // 
            // BusinessID
            // 
            BusinessID.AutoSize = true;
            BusinessID.Font = new Font("Franklin Gothic Medium", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            BusinessID.Location = new Point(23, 246);
            BusinessID.Name = "BusinessID";
            BusinessID.Size = new Size(78, 18);
            BusinessID.TabIndex = 4;
            BusinessID.Text = "Business ID";
            // 
            // SpecialName
            // 
            SpecialName.AutoSize = true;
            SpecialName.Font = new Font("Franklin Gothic Medium Cond", 20F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SpecialName.Location = new Point(294, 42);
            SpecialName.Name = "SpecialName";
            SpecialName.Size = new Size(152, 34);
            SpecialName.TabIndex = 5;
            SpecialName.Text = "Special Name";
            // 
            // SpecialLogo
            // 
            SpecialLogo.Location = new Point(294, 79);
            SpecialLogo.Name = "SpecialLogo";
            SpecialLogo.Size = new Size(219, 164);
            SpecialLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            SpecialLogo.TabIndex = 6;
            SpecialLogo.TabStop = false;
            // 
            // SpecialID
            // 
            SpecialID.AutoSize = true;
            SpecialID.Font = new Font("Franklin Gothic Medium", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SpecialID.Location = new Point(294, 246);
            SpecialID.Name = "SpecialID";
            SpecialID.Size = new Size(68, 18);
            SpecialID.TabIndex = 7;
            SpecialID.Text = "Special ID";
            // 
            // SpecialPrice
            // 
            SpecialPrice.Location = new Point(294, 267);
            SpecialPrice.Name = "SpecialPrice";
            SpecialPrice.Size = new Size(120, 23);
            SpecialPrice.TabIndex = 8;
            // 
            // Backoffice
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(827, 486);
            Controls.Add(SpecialPrice);
            Controls.Add(SpecialID);
            Controls.Add(SpecialLogo);
            Controls.Add(SpecialName);
            Controls.Add(BusinessID);
            Controls.Add(BusinessLogo);
            Controls.Add(BusinessName);
            Controls.Add(Export);
            Name = "Backoffice";
            Text = "BackOffice";
            ((System.ComponentModel.ISupportInitialize)specialBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)BusinessLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)SpecialLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)SpecialPrice).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Export;
        private Label BusinessName;
        private BindingSource specialBindingSource;
        private PictureBox BusinessLogo;
        private Label BusinessID;
        private Label SpecialName;
        private PictureBox SpecialLogo;
        private Label SpecialID;
        private NumericUpDown SpecialPrice;
    }
}
