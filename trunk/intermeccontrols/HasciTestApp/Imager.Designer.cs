namespace Hasci.TestApp
{
    partial class Imager
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
            this.label1 = new System.Windows.Forms.Label();
            this.panelImage = new System.Windows.Forms.Panel();
            this.labelSaveMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(440, 105);
            this.label1.Text = "Fokussieren Sie mit gedrückter SCAN-Taste. Beim Loslassen der SCAN-Taste wird das" +
                " Foto gemacht";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panelImage
            // 
            this.panelImage.Location = new System.Drawing.Point(0, 120);
            this.panelImage.Name = "panelImage";
            this.panelImage.Size = new System.Drawing.Size(480, 400);
            // 
            // labelSaveMessage
            // 
            this.labelSaveMessage.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Italic);
            this.labelSaveMessage.ForeColor = System.Drawing.Color.Red;
            this.labelSaveMessage.Location = new System.Drawing.Point(0, 523);
            this.labelSaveMessage.Name = "labelSaveMessage";
            this.labelSaveMessage.Size = new System.Drawing.Size(477, 46);
            this.labelSaveMessage.Text = "Richten Sie den Zielstrahl nicht auf Menschen";
            this.labelSaveMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Imager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(480, 574);
            this.Controls.Add(this.labelSaveMessage);
            this.Controls.Add(this.panelImage);
            this.Controls.Add(this.label1);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "Imager";
            this.Text = "Foto mit Imager";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.Imager_Activated);
            this.Load += new System.EventHandler(this.Imager_Load);
            this.Deactivate += new System.EventHandler(this.Imager_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelImage;
        private System.Windows.Forms.Label labelSaveMessage;
    }
}
