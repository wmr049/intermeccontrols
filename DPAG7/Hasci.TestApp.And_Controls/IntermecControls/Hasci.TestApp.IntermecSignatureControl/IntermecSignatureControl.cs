using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Intermec.Windows.Forms;
using System.ComponentModel.Composition;

using System.Runtime.InteropServices;

namespace Hasci.TestApp.IntermecSignatureControls
{
    /// <summary>
    /// the signature control enables you you subscribe on the touch screen of the device
    /// 
    /// Interface:
    /// + EventHandler SignatureReady
    ///   fired when a signature is ready, will be signaled when the stylus is left of the screen (OnMouseUp)
    /// + bool SaveAsJpg(string savePath)
    ///   save the current captured signature to a given file
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(Hasci.TestApp.DeviceControlContracts.ISignatureControl))]
    public partial class IntermecSignatureControl : UserControl, Hasci.TestApp.DeviceControlContracts.ISignatureControl
    {
        Intermec.Windows.Forms.InkCapture inkCapture1;
        private string _thePosition = "Bitte unterschreiben Sie hier";

        public IntermecSignatureControl()
        {
            InitializeComponent();
            loadInkCap();
        }

        /// <summary>
        /// save the current captured on-screen signature to a given file
        /// </summary>
        /// <param name="savePath"></param>
        /// <returns>true for success, false for failure in saving a file</returns>
        public bool SaveAsJpg(string savePath)
        {
            bool bRes = saveImage(savePath);
            if (bRes){
                inkCapture1.ClearImage();
                Ink_Redraw_Reset();
            }
            return bRes;
        }

        #region SignatureReady
        /// <summary>
        /// fired when a signature is ready to be saved
        /// the event gets signaled on MouseUp
        /// </summary>
        public event EventHandler SignatureReady;

        private void SignatureIsReady()
        {
            OnSignatureReady(new EventArgs());
        }
        protected virtual void OnSignatureReady(EventArgs e)
        {
            if (SignatureReady != null)
            {
                SignatureReady(this, e);
            }
        }
        #endregion

        private void loadInkCap()
        {
            try
            {
                
                inkCapture1 = new Intermec.Windows.Forms.InkCapture();
                inkCapture1.Size = this.Size;
                inkCapture1.Location = this.Location;
                
                inkCapture1.InkCaptureInitEvent += new InkCaptureInitEventHandler(inkCapture1_InkCaptureInitEvent);
                inkCapture1.MouseUp += new MouseEventHandler(inkCapture1_MouseUp);
                this.Controls.Add(inkCapture1);
            }
            catch (Intermec.Windows.Forms.InkCaptureException ex)
            {
                addLog("InkCaptureException in loadInkCap. Is the runtime ITCink.dll installed?\n" + ex.Message);
            }
            catch (Exception ex)
            {
                addLog("Exception in loadInkCap. Is the runtime ITCink.dll installed?\n" + ex.Message);
            }
            if (inkCapture1 == null)
            {
                addLog("Exception in loadInkCap. Is the runtime ITCink.dll installed?");
                throw new MissingMethodException("Exception in loadInkCap. Is the runtime ITCink.dll installed?");
            }
        }

        void inkCapture1_MouseUp(object sender, MouseEventArgs e)
        {
            SignatureIsReady();
        }

        void inkCapture1_InkCaptureInitEvent(object source, InkCaptureInitEventArgs e)
        {
            // Set properties and draw the initial image.
            Ink_Redraw_Reset();
        }
        private void Ink_Redraw_Reset()
        {
            addLog("Entered Ink_Redraw_Reset");
            // Implement exception handling. 
            try
            {
                // Set properties and draw the default image.
                inkCapture1.BackColor = Color.White;

                drawBackground(false);

                inkCapture1.PenWidth = 2;

                inkCapture1.PenColor = Color.Black;
            }
            catch (Intermec.Windows.Forms.InkCaptureException ex)
            {
                // Display the error to the user.
                addLog("InkCaptureException in Ink_Redraw_Reset.\n" + ex.Message);
            }
            catch (Exception ex)
            {
                // Display the error to the user.
                addLog("Exception in Ink_Redraw_Reset.\n" + ex.Message);
            }
            if (inkCapture1 == null)
                throw new MissingMethodException("Missing runtime files. Did you install ITCInk.DLL?");
        }

        /// <summary>
        /// draw some information on the image
        /// </summary>
        /// <param name="bErase">true=draw with background color (invisible)
        /// false=draw 'visible'</param>
        void drawBackground(bool bErase)
        {
            try
            {
                // Set properties and draw the default image.
                if(bErase)
                    inkCapture1.PenColor = inkCapture1.BackColor;
                else
                    inkCapture1.PenColor = Color.Blue;

                inkCapture1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F,
                    System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Regular);

                //Draw a line
                inkCapture1.AddLine(inkCapture1.Width / 10, inkCapture1.Height / 10,
                    inkCapture1.Width / 10 * 9, inkCapture1.Height / 10);

                //write text
                inkCapture1.AddText(_thePosition, inkCapture1.Width / 10 * 9, inkCapture1.Height / 10 * 9,
                    Intermec.Windows.Forms.BmpRotationType.ROTATE_180);

                //add a cross
                inkCapture1.AddLine(inkCapture1.Width/10 * 9, inkCapture1.Height / 10,
                    inkCapture1.Width / 10 * 8, inkCapture1.Height / 10 * 3);
                inkCapture1.AddLine(inkCapture1.Width / 10 * 8, inkCapture1.Height / 10,
                    inkCapture1.Width / 10 * 9, inkCapture1.Height / 10 * 3);
            }
            catch (Intermec.Windows.Forms.InkCaptureException ex)
            {
                // Display the error to the user.
                addLog("InkCaptureException in Ink_Redraw_Reset.\n" + ex.Message);
            }
            catch (Exception ex)
            {
                // Display the error to the user.
                addLog("Exception in Ink_Redraw_Reset.\n" + ex.Message);
            }
        }
        /// <summary>
        /// save the image in rotated to a BMP file
        /// </summary>
        /// <param name="sFileName"></param>
        /// <returns></returns>
        bool saveImage(string sFileName)
        {
            bool bRes = false;
            try
            {
                drawBackground(true);
                inkCapture1.SaveImage(sFileName, BmpColorType.BMPCOLOR_08BPP, BmpRotationType.ROTATE_180, false);
                //make a jpg file
                Bitmap bmp = new Bitmap(sFileName);
                bmp.Save(sFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmp.Dispose();
                drawBackground(false);
                bRes = true;
            }
            catch (Intermec.Windows.Forms.InkCaptureException ex)
            {
                addLog("InkCaptureException in saveImage: " + ex.Message);
            }
            catch (Exception ex)
            {
                addLog("Exception in saveImage: " + ex.Message);
            }
            drawBackground(false);  //restore text and line
            return bRes;
        }
        public new void Dispose()
        {
            if (inkCapture1 != null)
            {
                inkCapture1.Dispose();
                inkCapture1 = null;
            }
            //base.Dispose(); do not use!!
        }

        private void IntermecSignatureControl_Resize(object sender, EventArgs e)
        {
            if (inkCapture1 != null)
            {
                try
                {
                    inkCapture1.Width = this.Width;
                    inkCapture1.Height = this.Height;
                    inkCapture1.Location = new Point(0, 0);
                    addLog("Entered IntermecSignatureControl_Resize: " +
                        inkCapture1.Width.ToString() + "x" + inkCapture1.Height.ToString());
                    Ink_Redraw_Reset();
                }
                catch (Exception)
                {
                    throw new MissingMethodException("Missing ITCInkk.dll");
                }
            }
        }
        
        private void addLog(string s){
            System.Diagnostics.Debug.WriteLine(s);
        }

    }
}