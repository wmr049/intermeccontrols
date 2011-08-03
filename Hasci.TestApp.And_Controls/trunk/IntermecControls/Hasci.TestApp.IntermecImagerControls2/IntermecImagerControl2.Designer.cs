namespace Hasci.TestApp.IntermecImagerControls2
{
    partial class IntermecImagerControl2
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
            this.ImagerPreview = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // ImagerPreview
            // 
            this.ImagerPreview.Location = new System.Drawing.Point(18, 26);
            this.ImagerPreview.Name = "ImagerPreview";
            this.ImagerPreview.Size = new System.Drawing.Size(320, 211);
            this.ImagerPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.ImagerPreview_Paint);
            // 
            // IntermecImagerControl2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.ImagerPreview);
            this.Name = "IntermecImagerControl2";
            this.Size = new System.Drawing.Size(480, 400);
            this.Resize += new System.EventHandler(this.IntermecImagerControl_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox ImagerPreview;
    }
}
