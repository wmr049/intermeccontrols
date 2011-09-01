namespace Hasci.TestApp.IntermecBarcodeScanControls4
{
    partial class IntermecBarcodescanControl4
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtBoxScan = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtBoxScan
            // 
            this.txtBoxScan.Location = new System.Drawing.Point(3, 0);
            this.txtBoxScan.Name = "txtBoxScan";
            this.txtBoxScan.ReadOnly = true;
            this.txtBoxScan.Size = new System.Drawing.Size(224, 21);
            this.txtBoxScan.TabIndex = 1;
            // 
            // IntermecBarcodescanControl4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.txtBoxScan);
            this.Name = "IntermecBarcodescanControl4";
            this.Size = new System.Drawing.Size(243, 20);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.IntermecBarcodescanControl4_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtBoxScan;
    }
}
