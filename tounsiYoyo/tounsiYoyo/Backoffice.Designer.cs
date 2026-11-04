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
            SuspendLayout();
            // 
            // Export
            // 
            Export.Location = new Point(666, 401);
            Export.Name = "Export";
            Export.Size = new Size(75, 23);
            Export.TabIndex = 0;
            Export.Text = "Export";
            Export.UseVisualStyleBackColor = true;
            Export.Click += Export_button_click;
            // 
            // Backoffice
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Export);
            Name = "Backoffice";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button Export;
    }
}
