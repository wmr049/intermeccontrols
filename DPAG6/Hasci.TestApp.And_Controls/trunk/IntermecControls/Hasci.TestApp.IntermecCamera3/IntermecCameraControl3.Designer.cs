namespace Hasci.TestApp.IntermecPhotoControls3
{
    partial class IntermecCameraControl2
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
            this.CameraPreview = new System.Windows.Forms.PictureBox();
            this.CameraSnapshot = new System.Windows.Forms.PictureBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CameraPreview
            // 
            this.CameraPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.CameraPreview.Location = new System.Drawing.Point(135, 41);
            this.CameraPreview.Name = "CameraPreview";
            this.CameraPreview.Size = new System.Drawing.Size(207, 245);
            this.CameraPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.CameraPreview_Paint);
            // 
            // CameraSnapshot
            // 
            this.CameraSnapshot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.CameraSnapshot.Location = new System.Drawing.Point(240, 126);
            this.CameraSnapshot.Name = "CameraSnapshot";
            this.CameraSnapshot.Size = new System.Drawing.Size(207, 245);
            // 
            // lblVersion
            // 
            this.lblVersion.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Regular);
            this.lblVersion.Location = new System.Drawing.Point(9, 10);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(43, 23);
            this.lblVersion.Text = "3.0";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // IntermecCameraControl2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.CameraPreview);
            this.Controls.Add(this.CameraSnapshot);
            this.Name = "IntermecCameraControl2";
            this.Size = new System.Drawing.Size(480, 400);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox CameraPreview;
        private System.Windows.Forms.PictureBox CameraSnapshot;
        private System.Windows.Forms.Label lblVersion;


    }
}
