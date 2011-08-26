namespace Hasci.TestApp.IntermecBarcodeScanControls
{
    partial class IntermecBarcodescanControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtBoxScan = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 29);
            this.label1.Text = "IntermecBarcodeScanControl";
            // 
            // txtBoxScan
            // 
            this.txtBoxScan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBoxScan.Location = new System.Drawing.Point(0, 0);
            this.txtBoxScan.Name = "txtBoxScan";
            this.txtBoxScan.ReadOnly = true;
            this.txtBoxScan.Size = new System.Drawing.Size(243, 21);
            this.txtBoxScan.TabIndex = 1;
            // 
            // IntermecBarcodescanControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.txtBoxScan);
            this.Controls.Add(this.label1);
            this.Name = "IntermecBarcodescanControl";
            this.Size = new System.Drawing.Size(243, 20);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBoxScan;
    }
}
