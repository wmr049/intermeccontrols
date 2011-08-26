namespace Hasci.TestApp
{
    partial class Foto
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
            this.panelImage = new System.Windows.Forms.Panel();
            this.labelSaveMessage = new System.Windows.Forms.Label();
            this.labelPhotoHint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // panelImage
            // 
            this.panelImage.BackColor = System.Drawing.Color.Azure;
            this.panelImage.Location = new System.Drawing.Point(0, 120);
            this.panelImage.Name = "panelImage";
            this.panelImage.Size = new System.Drawing.Size(480, 400);
            // 
            // labelSaveMessage
            // 
            this.labelSaveMessage.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Italic);
            this.labelSaveMessage.ForeColor = System.Drawing.Color.Red;
            this.labelSaveMessage.Location = new System.Drawing.Point(0, 520);
            this.labelSaveMessage.Name = "labelSaveMessage";
            this.labelSaveMessage.Size = new System.Drawing.Size(477, 40);
            this.labelSaveMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelPhotoHint
            // 
            this.labelPhotoHint.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.labelPhotoHint.Location = new System.Drawing.Point(16, 13);
            this.labelPhotoHint.Name = "labelPhotoHint";
            this.labelPhotoHint.Size = new System.Drawing.Size(440, 105);
            this.labelPhotoHint.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Foto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(253)))), ((int)(((byte)(153)))));
            this.ClientSize = new System.Drawing.Size(480, 574);
            this.Controls.Add(this.labelPhotoHint);
            this.Controls.Add(this.labelSaveMessage);
            this.Controls.Add(this.panelImage);
            this.Location = new System.Drawing.Point(0, 52);
            this.Name = "Foto";
            this.Text = "Foto";
            this.Activated += new System.EventHandler(this.Foto_Activated);
            this.Load += new System.EventHandler(this.Photo_Load);
            this.Deactivate += new System.EventHandler(this.Foto_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelImage;
        private System.Windows.Forms.Label labelSaveMessage;
        private System.Windows.Forms.Label labelPhotoHint;


    }
}
