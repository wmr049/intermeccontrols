#define USEGDI
//do not switch streaming! see also "ALL THE TIME"
#define STREAMING_ON
//#define MYDEBUG
#define USE_ENTER_KEY
#define USE_PRESS_N_HOLD
#define REMAP_SCAN_TO_ENTERKEY
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Intermec.Multimedia;
using System.ComponentModel.Composition;

using System.IO;

/* 03. august 2011
change request to use ScanButton instead of ENTER key
done by remapping the scanbutton to enter as the rest of the code works fine
*/

/*
 * base is subversion r95
 * 1. changed to use Streaming all the time, see "ALL THE TIME"
 * 2. changed to use KeyUp of ENTER key
*/

/* eMail Kirwa 22.07.2011
Scanner & Kamera sollen gleich funktionieren	Kameraablauf anpassen.
Anweisungstexte müssen angepasst werden:
„Fokussieren Sie mit gedrückter Scan-Taste. Beim Loslassen der Scan-Taste wird das Foto gemacht.“
*/

#if USEGDI
//imagefactory
using OpenNETCF.Drawing;
using OpenNETCF.Drawing.Imaging;
#endif

/*
 * changed to use Scan button 
 * on first press/release the preview starts
 * on second press/release the snapshot takes place
*/
#if USE_ENTER_KEY
#else
using NativeSync;
using ITC_KEYBOARD;
#endif
using System.Threading;

namespace Hasci.TestApp.IntermecPhotoControls3
{
    /// <summary>
    /// This is the Intermec Photo Control for MEF
    /// it enables you to preview the camera image and save of a photo
    /// On the first release of the activation key, the preview starts and streams
    /// On every second release of the activation key, a snapshot of the current 
    /// camera image is taken and ready to be saved.
    /// 
    /// Interface:
    /// + EventHandler ImageReady
    ///   signals that a captured photo is ready to be saved
    /// + EventHandler InPreview
    ///   signals that the control is in preview mode
    /// + string PhotoKey
    ///   this is the name of the active Photo key, the key that starts/stop video streaming
    /// + bool SaveAsJpg(string savePath)
    ///   use this to save the last captured snapshot
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export (typeof (Hasci.TestApp.DeviceControlContracts.IPhotoControl))]
    public partial class IntermecCameraControl2 : UserControl,Hasci.TestApp.DeviceControlContracts.IPhotoControl
    {
        /// <summary>
        /// the intermec camera object
        /// </summary>
        private Camera IntermecCamera = null;
        /// <summary>
        /// the key that will handle preview on KeyDown and SnapShot on KeyUP
        /// </summary>
#if USE_ENTER_KEY
#if REMAP_SCAN_TO_ENTERKEY
        private string _PhotoKeyText = "ScanTaste";
#else
        private string _PhotoKeyText = "ENTER";
#endif
#else
        private string _PhotoKeyText = "ScanButton";
#endif
        /// <summary>
        /// internal var to hold temporary filename
        /// </summary>
        private string _fileName = "";
        private const string _tempFileName = "\\Temp\\Foto.jpg";
        /// <summary>
        /// var to avoid multiple calls to SnapShot()
        /// </summary>
        private bool _bTakingSnapShot = false;
#if USE_GDI
#else
        /// <summary>
        /// indicate if we showing a snapshot or a preview stream
        /// </summary>
        private bool _bIsSnapshotView = false;
#endif
        bool _bFirstEnterUP = true;
        bool _bEnterIsDown = false;
        /// <summary>
        /// the internal filename template, do not add(include a dot or an extension!
        /// </summary>
        private string _sFileTemplate = "FotoKamera";

#if USE_ENTER_KEY
#else
        #region scanbutton
        ITC_KEYBOARD.CUSBkeys.usbKeyStruct _OldUsbKey = new ITC_KEYBOARD.CUSBkeys.usbKeyStruct();
        System.Threading.Thread waitThread;
        bool _continueWait = true;

        #endregion
#endif
        /// <summary>
        /// Init the CameraControl
        /// </summary>
        public IntermecCameraControl2()
        {
            InitializeComponent();
#if USE_ENTER_KEY
#else
            //disable HW Trigger of Scanner
            //S9CconfigClass.S9Cconfig.HWTrigger.setHWTrigger(false);
            YetAnotherHelperClass.setHWTrigger(false);
#endif
            try
            {
                if (IntermecCamera != null)
                {
                    addLog("Init() old IntermecCamera found. Disposing...");
                    // IntermecCamera.Streaming = false; //we dont switch streaming except once, ALL THE TIME
                    IntermecCamera.Dispose();
                    IntermecCamera = null;
                }
                else
                    addLog("Init() Creating NEW IntermecCamera...");

                //added to check for exception in creating a new Camera
                int iTry = 1; bool bSuccess = false; string sEx = "";
                do
                {
                    try
                    {
                        //using the same sequence as in sample CN3Camera
                        IntermecCamera = new Camera(CameraPreview, Camera.ImageResolutionType.Medium);
                        //IntermecCamera.PictureBoxUpdate = Camera.PictureBoxUpdateType.None;
                        IntermecCamera.PictureBoxUpdate = Camera.PictureBoxUpdateType.Fit; // None;// Fit;// AdjustToFrameSize;
                        bSuccess = true;
                        addLog("Init() IntermecCamera creation OK. Try " + iTry.ToString() + " of 3");
                    }
                    catch (Intermec.Multimedia.CameraException ex)
                    {
                        sEx = ex.Message;
                        addLog("Init() IntermecCamera creation failed. Try " + iTry.ToString() + " of 3 \nCameraException: \n" + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        sEx = ex.Message;
                        addLog("Init() IntermecCamera creation failed. Try " + iTry.ToString() + " of 3 \nException: \n" + ex.Message);
                    }
                    finally
                    {
                        GC.Collect();
                        iTry++;
                    }
                } while (iTry <= 3 && !bSuccess);
                if (IntermecCamera == null)
                {
                    throw new FileNotFoundException("IntermecCamera did not load. CnxDShow.cab installed?\nException: " + sEx );
                }

                // moving to end of INIT() does not fix problem with NO STREAM AT FIRST INIT()!
#if STREAMING_ON
                addLog("Init() 1 IntermecCamera.Streaming=true...");
                IntermecCamera.Streaming = true;   //we start with streaming = true ALL THE TIME
#else
                addLog("Init() 1 IntermecCamera.Streaming=true...");
                IntermecCamera.Streaming = true;   //we start with streaming = true ALL THE TIME
#endif
                #region AutoFlash
                try
                {
                    addLog("IntermecCamera testing Flash.Available...");
                    if (IntermecCamera.Features.Flash.Available)
                    {
                        addLog("IntermecCamera testing Flash.Available OK. Changing to Auto...");
                        if (IntermecCamera.Features.Flash.SupportsAutoMode)
                        {
                            addLog("IntermecCamera testing Flash.Available OK. Changed to Auto OK");
                            IntermecCamera.Features.Flash.Auto = true;
                        }
                        else
                            addLog("IntermecCamera testing Flash.Available OK. No AutoMode support");
                    }
                    else
                        addLog("IntermecCamera testing Flash.Available OK. No Flash support");
                }
                catch (Exception)
                {
                    addLog("IntermecCamera testing Flash throwed exception.");

                }
                #endregion
#if MYDEBUG
                //for DEBUG only
                System.Diagnostics.Debug.WriteLine("CurrentViewfinderResolution 1=" +
                    IntermecCamera.CurrentViewfinderResolution.Width.ToString() + "x" +
                    IntermecCamera.CurrentViewfinderResolution.Height.ToString());
#endif
                //System.Diagnostics.Debug.WriteLine("Current viewfinderRes 2=" +
                //    IntermecCamera.CurrentViewfinderResolution.Width.ToString() + "x" +
                //    IntermecCamera.CurrentViewfinderResolution.Height.ToString());
                
                //moved to end
                //######## IntermecCamera.SnapshotEvent += new SnapshotEventHandler(IntermecCamera_SnapshotEvent);

                //IntermecCamera.SnapshotFile.Filename = "FotoKamera_"+ DateTime.Now.ToShortDateString()+ "_" + DateTime.Now.ToShortTimeString() + ".jpg";
                IntermecCamera.SnapshotFile.ImageFormatType = Camera.ImageType.JPG;
                //WARNING, if you dont set this property, snapshot may fail with garbage image
                IntermecCamera.SnapshotFile.ImageResolution = Camera.ImageResolutionType.Medium;
                IntermecCamera.SnapshotFile.JPGQuality = 90;
                IntermecCamera.SnapshotFile.Directory = "\\Temp";
                IntermecCamera.SnapshotFile.Filename = _sFileTemplate;
                IntermecCamera.SnapshotFile.FilenamePadding = Camera.FilenamePaddingType.IncrementalCounter;// None;// Camera.FilenamePaddingType.IncrementalCounter;

#if USE_ENTER_KEY
#if USE_PRESS_N_HOLD
                showSnapshot(true); //show a still image
#endif
#else
                //remap scan button key to new events
                mapKey();
                //start the scan button watch thread
                addLog("IntermecBarcodescanControl: starting named event watch thread...");
                waitThread = new System.Threading.Thread(waitLoop);
                waitThread.Start();
#endif
#if STREAMING_ON
                addLog("Init(): we DO NOT SWITCH streaming");
#else
                addLog("Init() IntermecCamera.Streaming=false...");
                IntermecCamera.Streaming = false;   //we use streaming=true ALL THE TIME
#endif
#if REMAP_SCAN_TO_ENTERKEY
                mapScan2Enter();
#endif
                //######### TEST ####### does not fix problem with NO STREAM AT FIRST INIT()!
                //addLog("Init() IntermecCamera.Streaming=true at END of INIT()...");
                //IntermecCamera.Streaming = true;   //we use streaming=true ALL THE TIME

                //CameraPreview.Refresh();
                //ImageIsInPreview();
                
                // Hook the snapshot event.
                IntermecCamera.SnapshotEvent += new SnapshotEventHandler(IntermecCamera_SnapshotEvent);
            }
            catch (Intermec.Multimedia.CameraException ex)
            {
                addLog("CameraException in CameraInit. Is the runtime 'CNxDShow.CAB' installed? " + ex.Message);
            }
            catch (Exception ex)
            {
                addLog("Exception in CameraInit. Is the runtime 'CNxDShow.CAB' installed?\n" + ex.Message);
            }
            if (IntermecCamera == null)
            {
                System.Diagnostics.Debug.WriteLine("Exception in CameraInit. Is the runtime 'CNxDShow.CAB' installed?");
                throw new FileNotFoundException("Missing Runtimes. Is CNxDShow.CAB installed?");
            }

            //if (IntermecCamera == null)
            //    return;
        }

        /// <summary>
        /// this event is called async by the Camera object after the SnapShot method was called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="snArgs"></param>
        void IntermecCamera_SnapshotEvent(object sender, Camera.SnapshotArgs snArgs)
        {
            addLog("IntermecCamera_SnapshotEvent entered...");
            Cursor.Current = System.Windows.Forms.Cursors.Default;
            if (snArgs.Status == Camera.SnapshotStatus.Ok)
            {
                addLog("SnapshotEvent OK");
                _fileName = snArgs.Filename;
                //rename file to fixed named file
                try
                {
                    //first delete a possibly exisiting file, otherwise File.Move will fail with UnauthorizedAccess
                    try
                    {
                        if(System.IO.File.Exists(_tempFileName))
                            System.IO.File.Delete(_tempFileName);
                    }
                    catch (Exception ex)
                    {
                        addLog("SnapshotEvent: Deleting file '" + _tempFileName + "' FAILED: " +ex.Message);
                    }
                    addLog("SnapshotEvent: Renaming file '" + _fileName + "' to '" + _tempFileName + "'");
                    System.IO.File.Move(_fileName, _tempFileName);
                    addLog("SnapshotEvent: File rename success");
                    _fileName = _tempFileName;
                }
                catch (Exception ex)
                {
                    addLog("SnapshotEvent: File rename FAILED: " +ex.Message);
                }

                OnImageReady(new EventArgs());//inform about image is ready
#if STREAMING_ON
#if USEGDI
                showImage(_fileName);
#endif
#endif
                //the following will give Out-of-memory exceptions!
                //CameraPreview.Image = new Bitmap(_fileName);
                //###########################################
                //System.Drawing.Bitmap _bitmap = new Bitmap(_fileName); 
                //Graphics g = Graphics.FromImage(new System.Drawing.Bitmap(_fileName)); //OOM exception
                //g.DrawImage(this.CameraPreview.Image, 
                //    new Rectangle(CameraPreview.Left, CameraPreview.Top, CameraPreview.Right, CameraPreview.Bottom),
                //    new Rectangle(0,0,480,400),
                //    GraphicsUnit.Pixel);// = new System.Drawing.Bitmap(_fileName);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SnapshotEvent not OK: " + snArgs.Status.ToString());
                System.Diagnostics.Debug.WriteLine("File: '" + IntermecCamera.SnapshotFile.Directory +"' '"+ IntermecCamera.SnapshotFile.Filename+"'");
                Cursor.Current = System.Windows.Forms.Cursors.Default;
#if USEGDI
                showSnapshot(false);
#endif
            }
#if STREAMING_ON
            addLog("SnapshotEvent: we DO NOT SWITCH streaming");
#else
            addLog("SnapshotEvent() IntermecCamera.Streaming=false...");
            IntermecCamera.Streaming = false; //do not show streaming automatically
#endif
            _bTakingSnapShot = false;
            System.Diagnostics.Debug.WriteLine("...IntermecCamera_SnapshotEvent ended.");
        }

#if USEGDI
        OpenNETCF.Drawing.Imaging.StreamOnFile m_stream;
        Size m_size;
        /// <summary>
        /// this will handle also large bitmaps and show a thumbnailed version on a picturebox
        /// see http://blog.opennetcf.com/ctacke/2010/10/13/LoadingPartsOfLargeImagesInTheCompactFramework.aspx
        /// </summary>
        /// <param name="sFileName">the name of the file to load</param>
        private void showImage(string sFileName)
        {
            var stream = File.Open(sFileName, FileMode.Open);
            m_stream = new StreamOnFile(stream);
            m_size = ImageHelper.GetRawImageSize(m_stream);
            System.Diagnostics.Debug.WriteLine("showImage loading " + sFileName + ", width/height = " + m_size.Width.ToString() + "/"+ m_size.Height.ToString());
            //CameraPreview.Image = ImageHelper.CreateThumbnail(m_stream, CameraPreview.Width, CameraPreview.Height);
            CameraSnapshot.Image = ImageHelper.CreateThumbnail(m_stream, CameraPreview.Width, CameraPreview.Height);
            showSnapshot(true);
            m_stream.Dispose();
            stream.Close();
        }
#endif
        /// <summary>
        /// OnResize we would like to have the largest possible preview
        /// Unfortunately the given dimensions dont match the defined picturebox (480x400) target that much
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
#if MYDEBUG
            System.Diagnostics.Debug.WriteLine("OnResize: " + this.Width.ToString() + "x" + this.Height.ToString());
#endif
            //#############################################################################
            if (IntermecCamera != null)
            {
#if STREAMING_ON
                addLog("OnResize(): start- we DO NOT SWITCH streaming");
#else
                addLog("OnResize() IntermecCamera.Streaming=false...");
                IntermecCamera.Streaming = false; //dont stream during resize! Do not switch streaming! ALL THE TIME
#endif
                //find the best matching resolution
                Intermec.Multimedia.Camera.Resolution[] _res = IntermecCamera.AvailableImageResolutions;
                Intermec.Multimedia.Camera.Resolution _maxRes = _res[0];
                /*
                Checking for matching resolution: 486/648
                Checking for matching resolution: 972/1296
                Checking for matching resolution: 1936/2592
                Checking for matching resolution: 120/162
                Max resolution is now: 120/162
                Checking for matching resolution: 242/324
                Max resolution is now: 242/324
                Checking for matching resolution: 486/648
                Checking for matching resolution: 972/1296
                Checking for matching resolution: 1936/2592
                 * or just
                Checking for matching resolution: 242/324
                Checking for matching resolution: 1936/2592
                */
                foreach (Intermec.Multimedia.Camera.Resolution r in _res)
                {
                    System.Diagnostics.Debug.WriteLine("Checking for matching resolution: " + r.Width.ToString() + "/" + r.Height.ToString());
                    //if(r.Width == 486)
                    if (r.Width < this.Width && r.Height < this.Height)
                    {
                        _maxRes = r;
                        System.Diagnostics.Debug.WriteLine("Max resolution is now: " + _maxRes.Width.ToString() + "/" + _maxRes.Height.ToString());
                    }
                    //resize Preview pictbox and center it                    
                    CameraPreview.Width = _maxRes.Width; 
                    CameraPreview.Height = _maxRes.Height;
                    CameraPreview.Left = (this.Width - CameraPreview.Width) / 2;
                    //resize Snapshot pictbox and center it                    
                    CameraSnapshot.Width = _maxRes.Width;
                    CameraSnapshot.Height = _maxRes.Height;
                    //CameraSnapshot.Left = (this.Width - CameraSnapshot.Width) / 2;
                    CameraSnapshot.Location = CameraPreview.Location;
                }
                //initCamera(ref this.CameraPreview);
                //IntermecCamera.PictureBoxUpdate = Camera.PictureBoxUpdateType.None;// Fit;// None;// Fit;// AdjustToFrameSize;
#if STREAMING_ON
                addLog("OnResize(): end- we DO NOT SWITCH streaming");
#else
                addLog("OnResize() IntermecCamera.Streaming=true/false...");
                IntermecCamera.Streaming = bOldStream;    //enable streaming after resize?
#endif
            }
        }
#if USE_PRESS_N_HOLD

        /// <summary>
        /// handle onKeyDown and react on the Preview/SnapShot key
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                addLog("KeyDown - Keys.Enter");
                if (_bEnterIsDown)
                { //we are already down
                    addLog("KeyDown - key was already down");
                    return;
                }
                //is there a pending snapshot?
                if (_bTakingSnapShot)
                {
                    addLog("KeyDown - already taking snapshot");
                    return;
                }
                if (!_bIsSnapshotView)
                {
                    addLog("KeyDown - already showing a preview");
                    return;
                }
                //remember key
                _bEnterIsDown = true;
                addLog("KeyDown - showSnapshot(false)...");
                showSnapshot(false); //show the preview and set
                addLog("KeyDown - ImageIsInPreview()");
                //ImageIsInPreview(); //moved to showSnapshot()
            }
            //base.OnKeyDown(e);
            addLog("KeyDown - END");
        }
#endif

#if USE_ENTER_KEY
        /// <summary>
        /// handle onKeyUp and react on the Preview/Snapshot key
        /// On first KeyUp start preview
        /// On second KeyUp take snapshot and start Preview
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {                
                _bEnterIsDown = false;
#if USEGDI
                if (_bIsSnapshotView)
                {
                    showSnapshot(false); //switch back to preview
                    return;
                }
#endif
                addLog("KeyUp - Keys.Enter");
                //if (_bKeyWasDown)
                //{
                //    System.Diagnostics.Debug.WriteLine("KeyUp - Keys.Enter : _bKeyWasDown = false");
                //    _bKeyWasDown = false;
                //}
                if (_bTakingSnapShot)
                {
                    System.Diagnostics.Debug.WriteLine("KeyUp - Keys.Enter : is taking snapshot, return");
                    return;
                }
                _bTakingSnapShot = true;
                //GC.Collect();
                System.Diagnostics.Debug.WriteLine("KeyUp - Keys.Enter Taking SnapShot");
                //within the snapshot event _bIsTakingSnapshot is reset
                IntermecCamera.Snapshot(972, 1296); //according to eMail RVDP, Wednesday, August 03, 2011 1:23 AM
                //IntermecCamera.Snapshot(Camera.ImageResolutionType.Highest);// Highest);
                Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            }
            //base.OnKeyUp(e);
        }
#endif
        /// <summary>
        /// this is called by the host to let this control save the picture
        /// we just rename the exisiting picture to the desired name
        /// </summary>
        /// <param name="Filename">the name for the image file</param>
        /// <returns>true for success and false on error</returns>
        public bool SaveAsJpg(string Filename)
        {
            bool bRes = false;
            try
            {
                System.IO.File.Move(_fileName, Filename);
                bRes = true;
            }
            catch (Exception)
            {
                bRes = false;
            }
            finally
            {
                //switch automatically back to preview???
                //showSnapshot(false); //switch back to preview
            }
            return bRes;
        }
        /// <summary>
        /// the name of the key that starts Preview/Snapshot
        /// </summary>
        public string PhotoKey
        {
            get { return _PhotoKeyText; }
        }

        #region ImageReady
        /// <summary>
        /// this event will be called when the image is ready to be saved
        /// </summary>
        public event EventHandler ImageReady;

        delegate void setImageIsReadyCallback(); //used as it is invoked from separate thread!
        /// <summary>
        /// helper to fire the ImageReady event
        /// </summary>
        private void ImageIsReady()
        {
            if (this.InvokeRequired)
            {
                setImageIsReadyCallback d = new setImageIsReadyCallback(ImageIsReady);
                this.Invoke(d, null);
            }
            else
            {
                OnImageReady(new EventArgs());
            }
        }
        /// <summary>
        /// another helper to fire attached ImageReady eventhandlers
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnImageReady(EventArgs e)
        {
            if (ImageReady != null)
            {
                ImageReady(this, e);
            }
        }
        #endregion

        #region InPreview
        delegate void SetInPreviewCallback();
        /// <summary>
        /// eventhandler InPreview
        /// will be fired once when the control enters preview mode
        /// </summary>
        public event EventHandler InPreview;
        /// <summary>
        /// helper to fire the InPreview event
        /// </summary>
        private void ImageIsInPreview()
        {
            if (this.InvokeRequired)
            {
                SetInPreviewCallback d = new SetInPreviewCallback(ImageIsInPreview);
                this.Invoke(d, null);
            }
            else
            {
                addLog("ImageIsInPreview");
                CameraPreview.Update();
                OnInPreview(new EventArgs());
            }
        }
        /// <summary>
        /// another helper to fire the attached InPreview event handlers
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnInPreview(EventArgs e)
        {
            if (InPreview != null)
            {
                InPreview(this, e);
            }
        }
        #endregion
        /// <summary>
        /// On shutdown we need to dispose our object manually before disposing the control
        /// </summary>
        public new void Dispose()
        {
            addLog("Dispose() called...");
#if USE_ENTER_KEY
#else
            _continueWait = false; //signal thread to stop
            Thread.Sleep(100);
            SystemEvent waitEvent = new SystemEvent("EndWaitLoop52", false, false);
            waitEvent.PulseEvent();
            Thread.Sleep(100);
            restoreKey();
#endif
            if (IntermecCamera != null)
            {
#if STREAMING_ON
            addLog("...Dispose() we DO NOT SWITCH streaming");
#else
                IntermecCamera.Streaming = false;
#endif
                IntermecCamera.Dispose();
                IntermecCamera = null;
            }
#if USE_ENTER_KEY
#else
            //enable HW Trigger of Scanner
            //S9CconfigClass.S9Cconfig.HWTrigger.setHWTrigger(true);
            YetAnotherHelperClass.setHWTrigger(true);
#endif
#if REMAP_SCAN_TO_ENTERKEY
            restoreScanKey();
#endif
            Cursor.Current = Cursors.Default;
            //base.Dispose(); do not use!!
            addLog("...Dispose() finished");
        }
        void addLog(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }
        void addLog2(string s)
        {
#if MYDEBUG
            System.Diagnostics.Debug.WriteLine(s);
#endif
        }
#if USE_ENTER_KEY
#else
        #region keyboard remapping
        /// <summary>
        /// restore scan button mapping to point to named event 1
        /// </summary>
        void restoreKey()
        {
            ITC_KEYBOARD.CUSBkeys _cusb = new ITC_KEYBOARD.CUSBkeys();
            ITC_KEYBOARD.CUSBkeys.usbKeyStruct _usbKey = new CUSBkeys.usbKeyStruct();
            int iIdx = _cusb.getKeyStruct(0, CUsbKeyTypes.HWkeys.SCAN_Button_KeyLang1, ref _usbKey);
            //change the scan button back to the original events
            if (iIdx != -1)
            {
                _usbKey = _OldUsbKey; //save for later restore
                addLog("scanbutton key index is " + iIdx.ToString());
                //_usbKey.bFlagHigh = CUsbKeyTypes.usbFlagsHigh.NoFlag;
                //_usbKey.bFlagMid = CUsbKeyTypes.usbFlagsMid.NOOP;
                //_usbKey.bFlagLow = CUsbKeyTypes.usbFlagsLow.NormalKey;
                _usbKey.bIntScan = 1;
                for (int i = 0; i < _cusb.getNumPlanes(); i++)
                {
                    addLog("using plane: " + i.ToString());
                    if (_cusb.setKey(0, _usbKey.bScanKey, _usbKey) == 0)
                        addLog("setKey for scanbutton key OK");
                    else
                        addLog("setKey for scanbutton key failed");
                }
                _cusb.writeKeyTables();
            }
            else
            {
                addLog("Could not get index for scanbutton key");
            }
        }
        /// <summary>
        /// change the event names of scanbutton to StateLeftScan1 and DeltaLeftScan1
        /// </summary>
        void mapKey()
        {
            ITC_KEYBOARD.CUSBkeys _cusb = new ITC_KEYBOARD.CUSBkeys();
            ITC_KEYBOARD.CUSBkeys.usbKeyStruct _usbKey = new CUSBkeys.usbKeyStruct();
            int iIdx = _cusb.getKeyStruct(0, CUsbKeyTypes.HWkeys.SCAN_Button_KeyLang1, ref _usbKey);

            //add two new events
            string sReg = ITC_KEYBOARD.CUSBkeys.getRegLocation();
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sReg + "\\Events\\State", true);
            reg.SetValue("Event5", "StateLeftScan1");
            reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sReg + "\\Events\\Delta", true);
            reg.SetValue("Event5", "DeltaLeftScan1");

            //change the scan button to fire these events
            if (iIdx != -1)
            {
                _OldUsbKey = _usbKey; //save for later restore
                addLog("scanbutton key index is " + iIdx.ToString());
                //_usbKey.bFlagHigh = CUsbKeyTypes.usbFlagsHigh.NoFlag;
                //_usbKey.bFlagMid = CUsbKeyTypes.usbFlagsMid.NOOP;
                //_usbKey.bFlagLow = CUsbKeyTypes.usbFlagsLow.NormalKey;
                _usbKey.bIntScan = 5;
                for (int i = 0; i < _cusb.getNumPlanes(); i++)
                {
                    addLog("using plane: " + i.ToString());
                    if (_cusb.setKey(0, _usbKey.bScanKey, _usbKey) == 0)
                        addLog("setKey for scanbutton key OK");
                    else
                        addLog("setKey for scanbutton key failed");
                }
                _cusb.writeKeyTables();
            }
            else
            {
                addLog("Could not get index for scanbutton key");
            }
        }
        #endregion
        //######################################################################
        void waitLoop()
        {
            addLog("waitLoop starting...");
            SystemEvent[] _events = new SystemEvent[3];
            addLog("waitLoop setting up event array...");
            _events[0] = new SystemEvent("StateLeftScan1", false, false);
            _events[1] = new SystemEvent("DeltaLeftScan1", false, false);
            _events[2] = new SystemEvent("EndWaitLoop52", false, false);
            try
            {
                do
                {
                    //Sleep as long as a snapshot is pending
                    while (_bTakingSnapShot && _continueWait)
                    {
                        Thread.Sleep(50);
                    }
                    if (!_continueWait)
                        Thread.CurrentThread.Abort();
                    addLog2("waitLoop WaitForMultipleObjects...");
                    SystemEvent signaledEvent = SyncBase.WaitForMultipleObjects(
                                                -1,  // wait for ever
                                                _events
                                                 ) as SystemEvent;
                    addLog2("waitLoop WaitForMultipleObjects released: ");
                    if (_continueWait)
                    {
                        if (signaledEvent == _events[0])
                        {
                            addLog2("######### Caught StateLeftScan ########");
                            onStateScan();
                        }
                        if (signaledEvent == _events[1])
                        {
                            addLog2("######### Caught DeltaLeftScan ########");
                            onDeltaScan();
                        }
                        if (signaledEvent == _events[2])
                        {
                            addLog2("######### Caught EndWaitLoop52 ########");
                            _continueWait = false;
                        }
                    }
                    addLog2("waitLoop sleep(5)");
                    System.Threading.Thread.Sleep(5);
                } while (_continueWait);
                addLog("waitLoop while ended by _continueWait");
            }
            catch (ThreadAbortException ex)
            {
                System.Diagnostics.Debug.WriteLine("waitLoop: ThreadAbortException: " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("waitLoop: Exception: " + ex.Message);
            }
            finally
            {
                _events[0].Dispose(); _events[1].Dispose(); _events[2].Dispose();
            }
            addLog("...waitLoop EXIT");
        }
#endif

        bool _bLastState = false;
        void onStateScan()
        {
            addLog2("onStateScan started...");
            if (_bTakingSnapShot) //do not disturb snapshot
            {
                return;
            }
            if (_bLastState)
            { //already pressed
                addLog2("...onStateScan: already pressed (_bLastState)");
                return;
            }
            else
            {
                addLog("...onStateScan: _bLastState toggled");
            }
            _bLastState = true; //avoid multiple calls
#if STREAMING_ON
            addLog("onStateScan: we DO NOT SWITCH streaming");
            
#else
            addLog("onStateScan IntermecCamera.Streaming=True...");
            IntermecCamera.Streaming = true;
            ImageIsInPreview();
#endif
            addLog("...onStateScan ended");
        }

        void onDeltaScan()
        {
            addLog("onDeltaScan started...");
            //return immediately, if we are taking a snapshot
            if (_bTakingSnapShot)
            {
                addLog("onDeltaScan _bTakingSnapShot. Return...");
                return;
            }
            ////is this the first call, only use below code to toggle preview/snapshot with DeltaScan event
            //if (bFirstDeltaToggle)
            //{
            //    addLog("onDeltaScan Streaming=ON...");
            //    IntermecCamera.Streaming = true;
            //    addLog("onDeltaScan Streaming=ON OK");
            //    bFirstDeltaToggle = !bFirstDeltaToggle; //ready for next toggle
            //    ImageIsInPreview();
            //    return;
            //}else
            //    addLog("onDeltaScan going for snapshot...");
            //bFirstDeltaToggle = !bFirstDeltaToggle; //ready for next toggle
            //############### Take a snapshot ##################
            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            addLog("onDeltaScan making snapshot...");
            try
            {
                if (_bTakingSnapShot)
                    return;
                _bTakingSnapShot = true;
                //issue a snapshot, async call!
                if(IntermecCamera.Streaming)
                    IntermecCamera.Snapshot(Camera.ImageResolutionType.Medium);// Highest);
            }
            catch (CameraException ex)
            {
                addLog("CameraException in onDeltaScan. Is the runtime 'CNxDShow.cab' installed?\n" + ex.Message);
            }
            catch (Exception ex)
            {
                addLog("Exception in onDeltaScan. Is the runtime 'CNxDShow.cab' installed?\n" + ex.Message);
            }
            Cursor.Current = System.Windows.Forms.Cursors.Default;

            _bLastState = false; //ready for next preview
        }

        private void CameraPreview_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            if (IntermecCamera != null)
            {
                if (!_bTakingSnapShot)// IntermecCamera.Streaming)
                    e.Graphics.DrawString("Preview", new Font("Tahoma", 8, FontStyle.Regular), new SolidBrush(Color.Orange), 10, 10);
                //else
                //    e.Graphics.DrawString("Snapshot", new Font("Tahoma", 8, FontStyle.Regular), new SolidBrush(Color.GreenYellow), 10, 10);
            }
        }
        /// <summary>
        /// show the snapshot or preview picturebox in front 
        /// </summary>
        /// <param name="bShow">true for snapshot
        /// false for preview</param>
        private void showSnapshot(bool bShow)
        {
#if USEGDI
            addLog("showSnapshot() called with " + bShow.ToString());
            if (bShow)
            {
                // CameraSnapshot.BringToFront();
                CameraSnapshot.Visible = true;
                CameraPreview.Visible = false;
                ImageIsReady();
            }
            else
            {
                //CameraPreview.BringToFront();
                CameraSnapshot.Visible = false;// BringToFront();
                CameraPreview.Visible = true;
                ImageIsInPreview();
            }
            _bIsSnapshotView = bShow;
            addLog("showSnapshot() call end.");
#else
            return;
#endif
        }
    }
}
