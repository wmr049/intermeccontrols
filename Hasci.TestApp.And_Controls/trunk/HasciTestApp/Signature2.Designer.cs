namespace Hasci.TestApp
{
    partial class Signature2
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
            this.labelSaveMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelSaveMessage
            // 
            this.labelSaveMessage.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Italic);
            this.labelSaveMessage.Location = new System.Drawing.Point(0, 520);
            this.labelSaveMessage.Name = "labelSaveMessage";
            this.labelSaveMessage.ForeColor = System.Drawing.Color.Red;
            this.labelSaveMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelSaveMessage.Size = new System.Drawing.Size(477, 40);
            // 
            // Signature2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(253)))), ((int)(((byte)(153)))));
            this.ClientSize = new System.Drawing.Size(480, 640);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "Signature2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Signature2_Load);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Label labelSaveMessage;

        #endregion
    }
}
