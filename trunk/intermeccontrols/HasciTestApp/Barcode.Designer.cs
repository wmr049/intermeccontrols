namespace Hasci.TestApp
{
    partial class Barcode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Barcode));
            this.pictureBoxOK = new System.Windows.Forms.PictureBox();
            this.pictureBoxNotOk = new System.Windows.Forms.PictureBox();
            this.textBoxScanResult = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxVibro = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // pictureBoxOK
            // 
            this.pictureBoxOK.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxOK.Image")));
            this.pictureBoxOK.Location = new System.Drawing.Point(20, 380);
            this.pictureBoxOK.Name = "pictureBoxOK";
            this.pictureBoxOK.Size = new System.Drawing.Size(128, 128);
            this.pictureBoxOK.Visible = false;
            // 
            // pictureBoxNotOk
            // 
            this.pictureBoxNotOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(253)))), ((int)(((byte)(153)))));
            this.pictureBoxNotOk.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxNotOk.Image")));
            this.pictureBoxNotOk.Location = new System.Drawing.Point(332, 380);
            this.pictureBoxNotOk.Name = "pictureBoxNotOk";
            this.pictureBoxNotOk.Size = new System.Drawing.Size(128, 128);
            this.pictureBoxNotOk.Visible = false;
            // 
            // textBoxScanResult
            // 
            this.textBoxScanResult.Location = new System.Drawing.Point(20, 209);
            this.textBoxScanResult.Name = "textBoxScanResult";
            this.textBoxScanResult.ReadOnly = true;
            this.textBoxScanResult.Size = new System.Drawing.Size(440, 41);
            this.textBoxScanResult.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.label1.Location = new System.Drawing.Point(20, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(440, 156);
            this.label1.Text = "Bitte fokussieren Sie den Barcode mit dem Barcodescanner und drücken die SCAN-Tas" +
                "te";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // checkBoxVibro
            // 
            this.checkBoxVibro.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.checkBoxVibro.Location = new System.Drawing.Point(20, 307);
            this.checkBoxVibro.Name = "checkBoxVibro";
            this.checkBoxVibro.Size = new System.Drawing.Size(440, 40);
            this.checkBoxVibro.TabIndex = 6;
            this.checkBoxVibro.Text = "Vibrationfeedback";
            // 
            // Barcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(253)))), ((int)(((byte)(153)))));
            this.ClientSize = new System.Drawing.Size(480, 588);
            this.Controls.Add(this.checkBoxVibro);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxScanResult);
            this.Controls.Add(this.pictureBoxNotOk);
            this.Controls.Add(this.pictureBoxOK);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "Barcode";
            this.Text = "Barcode";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.Barcode_Activated);
            this.Deactivate += new System.EventHandler(this.Barcode_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxOK;
        private System.Windows.Forms.PictureBox pictureBoxNotOk;
        private System.Windows.Forms.TextBox textBoxScanResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxVibro;


    }
}
