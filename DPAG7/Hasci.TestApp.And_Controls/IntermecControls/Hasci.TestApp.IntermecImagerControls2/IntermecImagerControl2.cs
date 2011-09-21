//#define NO_SNAPSHOT_THREAD
//do not switch streaming! see also "ALL THE TIME"
#define STREAMING_ON
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.ComponentModel.Composition;

using Intermec.DataCollection;

using NativeSync;
using System.Threading;

/* eMail Kirwa 22.07.2011
Scanner & Kamera sollen gleich funktionieren	Kameraablauf anpassen.
Anweisungstexte müssen angepasst werden:
„Fokussieren Sie mit gedrückter Scan-Taste. Beim Loslassen der Scan-Taste wird das Foto gemacht.“
*/

namespace Hasci.TestApp.IntermecImagerControls2
{
    /// <summary>
    /// This is the Intermec Imager Control for MEF
    /// After init you can press the barcode scanner button to start a preview of the imager
    /// when you release the barcode scan button, a snapshot is taken and you can save 
    /// the snapshot image 
    /// Pressing the scan button again will restart the cycle (preview, snapshot)
    /// 
    /// Interface:
    /// + eventhandler InPreview
    ///   is fired when scan button is pressed down
    /// + eventhandler ImageReady
    ///   fired when the scan button is released and a snapshot is ready to be saved
    /// + method bool SaveAsJpg(string savePath)
    ///   can be used to save the current snapshot to a named file
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(Hasci.TestApp.DeviceControlContracts.IImagerControl))]
    public partial class IntermecImagerControl2 : UserControl, Hasci.TestApp.DeviceControlContracts.IImagerControl
    {
        #region VARS

        private delegate void IntermecImager_SnapshotEvent(ImagerSnapshotEventArgs snArgs);
        private event IntermecImager_SnapshotEvent OnImagerSnapshot;

        Intermec.DataCollection.Imager _imager;
        /// <summary>
        /// we are using a temporary file for snapshots
        /// </summary>
        private const string _TempFileName = "\\Temp\\imager.jpg";
        /// <summary>
        /// var to avoid multiple calls to SnapShot()
        /// </summary>
        private bool _bTakingSnapShot = false;

        #region scanbutton
        /// <summary>
        /// this thread will catch the scan button named events
        /// </summary>
        System.Threading.Thread waitThread;
        /// <summary>
        /// var to let a thread stop itself
        /// </summary>
        bool _continueWait = true;
        /// <summary>
        /// avoid multiple calls into stateEvent handling
        /// </summary>
        bool _bLastState = false;
        /// <summary>
        /// used to toggle between streaming and snapshot with each button release
        /// </summary>
        bool bFirstDeltaToggle = true;

        #endregion scanbutton
        /// <summary>
        /// this thread will be used for the blocking snapshot method of the imager
        /// </summary>
        System.Threading.Thread snapshotThread = null;
        /// <summary>
        /// indicate if we showing a snapshot or a preview stream
        /// </summary>
        private bool _bIsSnapshotView = false;

        #endregion VARS

        public IntermecImagerControl2()
        {
            InitializeComponent();
            //setup imager
            try
            {
                //disable HW Trigger of Scanner
                //S9CconfigClass.S9Cconfig.HWTrigger.setHWTrigger(false); //removed as problems with ADCComInterface
                YetAnotherHelperClass.setHWTrigger(false);

                _imager = new Imager(ImagerPreview, Imager.PictureResolutionSize.Quarter);
                _imager.ImagerMode = Imager.ImagerModeType.Imaging;
                _imager.OnScreenLogo = Imager.OnScreenLogoType.Off;

                if (_imager.ImageConditioningAvailable)
                {

                    //change according to Bo Shang, eMail of 22. Juli 2011 02:16
                    _imager.ImageConditioning.ContrastEnhancement = ImageConditioning.ContrastEnhancementValue.Photo;
                    _imager.ImageConditioning.TextEnhancement = ImageConditioning.TextEnhancementValue.None;
                    _imager.ImageConditioning.ImageRotation = ImageConditioning.ImageRotationValue.None;
                    //use the following to enhance streaming performance
                    _imager.ImageConditioning.Subsampling = ImageConditioning.SubsamplingValue.OneOfEight;// ImageConditioning.SubsamplingValue.None;
                    _imager.ImageConditioning.NoiseReduction = 0;

                    //DO NOT USE other than NONE, a bug will then show a blank preview video
                    _imager.ImageConditioning.ImageLightingCorrection = ImageConditioning.ImageLightingCorrectionValue.None;// Enabled;// None;
                    _imager.ImageConditioning.Brightness = 20;
                    //the following setting changes the image very strong (B&W to Gray)
                    _imager.ImageConditioning.ColorMode = ImageConditioning.ColorModeValue.None;// None;

                    //this for the preview AND changes the way of the snapshot
                    _imager.ImageConditioning.ColorModeBrightnessThreshold = ImageConditioning.ColorModeBrightnessThresholdValue.VeryDark;
                    _imager.ImageConditioning.OutputCompression = ImageConditioning.OutputCompressionValue.Bitmap;
                    _imager.ImageConditioning.OutputCompressionQuality = 100;

                    //shutoff laser illumination, gives system exception "Invalid Illumination Status value"
                    //addLog("Trying to set Illumination...\n" + "Current IlluminationAimerStatus=" + _imager.IlluminationAimerStatus.ToString());
                    //_imager.IlluminationAimerStatus = Imager.IlluminationAimerActivationType.IlluminationOnly;

                    addLog("Trying to set AimerFlashing...\n" + "Current AimerFlashing=" + _imager.AimerFlashing.ToString());
                    _imager.AimerFlashing = Imager.AimerFlashingMode.AlwaysOff;

                    //_imager.IllumFlashing = Imager.IllumFlashingMode.Auto;

                    addLog("Trying to set LightGoal...\n" + "Current LightGoal=" + _imager.LightGoal.ToString());
                    _imager.LightGoal = 150;

                    //the following two settings are unsupported on current test device and OS
                    //addLog("Trying to set SetAimerOn...\n" + "Current SetAimerOn=" + _imager.SetAimerOn.ToString());
                    //_imager.SetAimerOn = false;
                    //addLog("Trying to set SetIllumOn...\n" + "Current SetIllumOn=" + _imager.SetIllumOn.ToString());
                    //_imager.SetIllumOn = false;

                    //change defaults for snapshot file using explicit settings
                    _imager.SnapShotConditioning.Brightness = 20;
                    _imager.SnapShotConditioning.ColorConversion = ImageConditioning.ColorConversionValue.Grayscale;
                    _imager.SnapShotConditioning.ColorMode = ImageConditioning.ColorModeValue.None;
                    _imager.SnapShotConditioning.ColorModeBrightnessThreshold = ImageConditioning.ColorModeBrightnessThresholdValue.Normal;
                    _imager.SnapShotConditioning.ContrastEnhancement = ImageConditioning.ContrastEnhancementValue.Photo;
                    _imager.SnapShotConditioning.ImageLightingCorrection = ImageConditioning.ImageLightingCorrectionValue.None;
                    _imager.SnapShotConditioning.ImageRotation = ImageConditioning.ImageRotationValue.None;
                    _imager.SnapShotConditioning.NoiseReduction = 0;
                    _imager.SnapShotConditioning.OutputCompression = ImageConditioning.OutputCompressionValue.Jpeg;
                    _imager.SnapShotConditioning.OutputCompressionQuality = 80;
                    _imager.SnapShotConditioning.Subsampling = ImageConditioning.SubsamplingValue.None;
                    _imager.SnapShotConditioning.TextEnhancement = ImageConditioning.TextEnhancementValue.None;
                    

                    //change defaults for snapshot file by using already defined ImageConditioning values
                    //disagreed by Bo Shang 3. aug. 2011
                    /*
                    _imager.SnapShotConditioning = _imager.ImageConditioning;
                    _imager.SnapShotConditioning.OutputCompression = ImageConditioning.OutputCompressionValue.Jpeg;
                    _imager.SnapShotConditioning.OutputCompressionQuality = 80;
                    */
                    //_imager.SnapShotConditioning.Brightness = 20;
                    //_imager.SnapShotConditioning.ColorMode = ImageConditioning.ColorModeValue.None;
                    addLog("================== Imager ===================\n");
                    addLog(ImageHelper.dumpConditioningValues(_imager.ImageConditioning));
                    addLog("================= Snapshot ==================\n");
                    addLog(ImageHelper.dumpConditioningValues(_imager.SnapShotConditioning));

                }
                else
                {
                    _imager.EnhanceContrast = true;
                }

                //remap scan button key to new events
                ITCTools.KeyBoard.mapKey();
                //start the scan button watch thread
                addLog("IntermecImagerControl-ImagerInit: starting named event watch thread...");
                waitThread = new System.Threading.Thread(waitLoop);
                waitThread.Start();

#if NO_SNAPSHOT_THREAD
#else
                this.OnImagerSnapshot += new IntermecImager_SnapshotEvent(IntermecImagerControl2_OnImagerSnapshot);
#endif
#if STREAMING_ON
                _imager.VideoRunning = true;
                //show hide the right pictureboxes
                ImagerPreview.Visible = false;
                ImagerSnapshot.Image = null;
                ImagerSnapshot.Refresh();
                ImagerSnapshot.Visible = true;
                addLog("IntermecImagerControl2(): calling ShowSnapShot with true");
                //showSnapshot(true); // we start with no streaming View
#else
                //start with streaming=off
                showSnapshot(false);
#endif

            }
            catch (Intermec.DataCollection.ImagerException ex)
            {
                addLog("ImagerException in ImagerInit. Is the runtime 'DC_NET.CAB'/ITCimager.dll installed?\n" + ex.Message);
            }
            catch (Exception ex)
            {
                addLog("Exception in ImagerInit. Is the runtime 'DC_NET.CAB'/ITCimager.dll installed?\n" + ex.Message);
            }
            if (_imager == null)
            {
                addLog("IntermecImagerControl: Imager init FAILED");
                throw new System.IO.FileNotFoundException("IntermecImagerControl: Imager init FAILED - Is the runtime 'DC_NET.CAB'/ITCimager.dll installed?\n");
            }

        }

        string _fileName = "";
        private const string _tempFileName = "\\Temp\\Imager.jpg";
        void IntermecImagerControl2_OnImagerSnapshot(IntermecImagerControl2.ImagerSnapshotEventArgs snArgs)
        {
            addLog("IntermecCamera_SnapshotEvent entered...");
            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            if (snArgs.Status == SnapshotStatus.Ok)
            {
                addLog("SnapshotEvent OK");
                _fileName = snArgs.Filename;
                /* //############### File Rename is not needed with Imager ###################
                //first delete a possibly exisiting file, otherwise File.Move will fail with UnauthorizedAccess
                try
                {
                    if (System.IO.File.Exists(_tempFileName))
                        System.IO.File.Delete(_tempFileName);
                }
                catch (Exception ex)
                {
                    addLog("SnapshotEvent: Deleting file '" + _tempFileName + "' FAILED: " + ex.Message);
                }

                //rename file to fixed named file
                try
                {
                    addLog("SnapshotEvent: Renaming file '" + _fileName + "' to '" + _tempFileName + "'");
                    System.IO.File.Move(_fileName, _tempFileName);
                    addLog("SnapshotEvent: File rename success");
                    _fileName = _tempFileName;
                }
                catch (Exception)
                {
                    addLog("SnapshotEvent: File rename FAILED");
                }
                */
                OnImageReady(new EventArgs());//inform about image is ready does also a //showImage(_fileName);              

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
                //Snapshot not OK
                System.Diagnostics.Debug.WriteLine("SnapshotEvent not OK: " + snArgs.Status.ToString());
//                Cursor.Current = System.Windows.Forms.Cursors.Default;

                addLog("IntermecImagerControl2_OnImagerSnapshot(): calling ShowSnapShot with false");
                showSnapshot(false); //IntermecImagerControl2_OnImagerSnapshot

            }
#if STREAMING_ON
            addLog("SnapshotEvent: we DO NOT SWITCH streaming");
#else
            addLog("SnapshotEvent() IntermecCamera.Streaming=false...");
            IntermecCamera.Streaming = false; //do not show streaming automatically
#endif
            _bTakingSnapShot = false;
            System.Diagnostics.Debug.WriteLine("...IntermecCamera_SnapshotEvent ended.");
            Cursor.Current = System.Windows.Forms.Cursors.Default;
        }

        /// <summary>
        /// this is the main loop
        /// it waits for the named events and then invokes
        /// onStateScan or onDeltaScan
        /// </summary>
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
                    addLog("waitLoop WaitForMultipleObjects...");
                    SystemEvent signaledEvent = SyncBase.WaitForMultipleObjects(
                                                -1,  // wait for ever
                                                _events
                                                 ) as SystemEvent;
                    addLog("waitLoop WaitForMultipleObjects released: ");
                    if (_continueWait)
                    {
                        if (signaledEvent == _events[0])
                        {
                            addLog("######### Caught StateLeftScan ########");
                            onStateScan();
                        }
                        if (signaledEvent == _events[1])
                        {
                            addLog("######### Caught DeltaLeftScan ########");
                            onDeltaScan();
                        }
                        if (signaledEvent == _events[2])
                        {
                            addLog("######### Caught EndWaitLoop52 ########");
                            _continueWait = false;
                        }
                    }
                    addLog("waitLoop sleep(5)");
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
        /// <summary>
        /// called by waitloop when State event is signaled
        /// start preview if not yet in preview mode
        /// </summary>
        void onStateScan()
        {
            addLog("onStateScan started...");
            if (_bTakingSnapShot)
                return;
            if (_bLastState)
            { //already pressed
                addLog("...onStateScan: already pressed (_bLastState)");
                return;
            }
            //else
            //{
            //    addLog("onStateScan: calling abortSnapshot()");
            //    abortSnapshot();
            //}
            _bLastState = true; //avoid multiple calls
            
#if STREAMING_ON
            addLog("onStateScan: we DO NOT SWITCH streaming");
#else
            addLog("Trying to set AimerFlashing...\n" + "Current AimerFlashing=" + _imager.AimerFlashing.ToString());
            _imager.AimerFlashing = Imager.AimerFlashingMode.AlwaysOff;
#endif
            addLog("onStateScan showSnapshot(false)...");
            //ImageIsInPreview();
            showSnapshot(false); //OnStateScan
            addLog("...onStateScan ended");
        }
        /// <summary>
        /// called by waitLoop when Delta event has been caught
        /// if signaled will stop preview and start a snapshot
        /// </summary>
        void onDeltaScan()
        {
            addLog("onDeltaScan started...");
            if (_bTakingSnapShot)
            {
                addLog("onDeltaScan _bTakingSnapShot. Return...");
                return;
            }
            bFirstDeltaToggle = !bFirstDeltaToggle; //ready for next toggle
            //############### Take a snapshot ##################
//            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            addLog("onDeltaScan making snapshot...");
            try
            {
#if NO_SNAPSHOT_THREAD
                makeSnapShot();
#else
                abortSnapshot();
                if (snapshotThread == null)
                {
                    addLog("onDeltaScan: Starting snapShot thread...");
                    snapshotThread = new Thread(DoSnapShot);
                    snapshotThread.Start();
                }
                else
                {
                    addLog("onDeltaScan: starting existing snapShot thread...");
                    snapshotThread = new Thread(DoSnapShot);
                    snapshotThread.Start();
                }
#endif
            }
            catch (ThreadStateException ex)
            {
                System.Diagnostics.Debug.WriteLine("onDeltaScan: ThreadStateException: " + ex.Message);
            }
            catch (ImagerException ex)
            {
                System.Diagnostics.Debug.WriteLine("onDeltaScan: ImagerException: " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("onDeltaScan: Exception: " + ex.Message);
            }
//            Cursor.Current = Cursors.Default;
            Application.DoEvents();
#if NO_SNAPSHOT_THREAD
            _bLastState = false;//enable OnState processing
#endif
        }

        private void abortSnapshot()
        {
            addLog("abortSnapshot: start...");
            //abort current snapshot
            if (_bTakingSnapShot)
            {
                addLog("abortSnapshot: _bTakingSnapShot=true");
                //kill existing snapshot
                if (snapshotThread != null)
                {
                    addLog("abortSnapshot: aborting current snapshot thread...");
                    snapshotThread.Abort();
                    System.Threading.Thread.Sleep(1);
                }
                else
                {
                    addLog("abortSnapshot: NO current snapshot thread");
                }
            }else
                addLog("abortSnapshot: _bTakingSnapShot=false");
            addLog("...abortSnapshot: end");
        }
        /// <summary>
        /// make a snaphot in a thread
        /// </summary>
        private void DoSnapShot()
        {
            Cursor.Current = Cursors.WaitCursor;
            addLog("DoSnapShot: Entering SnapShot thread");
            if (_bTakingSnapShot)
            {
                addLog("DoSnapShot: SnapShot already running");
                return;
            }
            _bTakingSnapShot = true;
            try
            {
                if (File.Exists(_TempFileName))
                {
                    addLog("DoSnapShot: deleting tempfile '" + _TempFileName + "'");
                    File.Delete(_TempFileName);
                }
                else
                {
                    addLog("DoSnapShot: no tempfile '" + _TempFileName + "' to delete");
                }
                addLog("DoSnapShot: invoking '_imager.SnapShot()'...");
                _imager.SnapShot(_TempFileName, Imager.PictureResolutionSize.Full, Imager.FileModeType.JPG); //this line will block for a second or two
                addLog("DoSnapShot: '_imager.SnapShot()' done");
                //wait until the file is ready
                int TryCount = 0;

                addLog("DoSnapShot: waiting for file to be ready...");
                while ((!File.Exists(_TempFileName)) && (TryCount++ < 10)) //high resolution snapshot may take a second or 2
                {
                    System.Threading.Thread.Sleep(500);
                }
                addLog("DoSnapShot: DONE waiting for file to be ready");
                if (File.Exists(_TempFileName))
                {
                    addLog("DoSnapShot: tempfile '" + _TempFileName + "' ready");
                }
                else
                {
                    addLog("DoSnapShot: no tempfile '" + _TempFileName + "' saved by SnapShot!");
                }
                //_imager.VideoRunning = false; //XXX showVideoRunning()
                addLog("DoSnapShot: signaling ImageIsReady()");

                //fire the ready event
                ImagerSnapshotEventArgs snArg = new ImagerSnapshotEventArgs(_TempFileName, SnapshotStatus.Ok);
                addLog("DoSnapShot(): FireImagerSnapshotEvent...");
                FireImagerSnapshotEvent(snArg);
                addLog("DoSnapShot(): calling ShowSnapShot with true");
                showSnapshot(true); //DoSnapShot
                
                //ImageIsReady();
            }
            catch (ThreadAbortException ex)
            {
                addLog("DoSnapShot: ThreadAbortException: " + ex.Message);
            }
            catch (Exception ex)
            {
                addLog("DoSnapShot: Exception: " + ex.Message);
            }
            finally
            {
#if STREAMING_ON
#else
                addLog("Trying to set AimerFlashing...\n" + "Current AimerFlashing=" + _imager.AimerFlashing.ToString());
                _imager.AimerFlashing = Imager.AimerFlashingMode.AlwaysOff;
#endif
            }
            _bTakingSnapShot = false;
            _bLastState = false; //enable next press and hold action
            addLog("DoSnapShot: has been ended.");
            Cursor.Current = Cursors.Default;
        }

        // Fire thread safe event
        private void FireImagerSnapshotEvent(ImagerSnapshotEventArgs snArgs)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new IntermecImager_SnapshotEvent(FireImagerSnapshotEvent), snArgs);
                return;
            }
            else
            {
                if (this.OnImagerSnapshot != null)
                {
                    OnImagerSnapshot(snArgs);
                }
            }
        }  


        void addLog(string s)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(s);
            }
            else
                System.Threading.Thread.Sleep(1);
        }

        public new void Dispose()
        {
            addLog("Dispose() called...");
            _continueWait = false; //signal thread to stop
            Thread.Sleep(100);
            SystemEvent waitEvent = new SystemEvent("EndWaitLoop52", false, false);
            waitEvent.PulseEvent();
            Thread.Sleep(100);

#if STREAMING_ON
            //kill SnapShot Thread
            if (snapshotThread != null)
            {
                snapshotThread.Abort();
            }
#endif
            Thread.Sleep(100);

            if (_imager != null)
            {
                //restore AimerFlashing mode
                _imager.AimerFlashing = Imager.AimerFlashingMode.Auto;
                _imager.LightGoal = 128;
                _imager.VideoRunning = false;
                _imager.Dispose();
                _imager = null;
            }
            //enable HW Trigger of Scanner
            //S9CconfigClass.S9Cconfig.HWTrigger.setHWTrigger(true); //removed as problems with ADCComInterface
            YetAnotherHelperClass.setHWTrigger(true);
            ITCTools.KeyBoard.restoreKey();

            Cursor.Current = Cursors.Default;
            // base.Dispose(); do not use!!
            addLog("...Dispose() finished");
        }
        private void loadImage(string sFileName)
        {
            addLog("showImage(): started with '" + sFileName+"'");
            try
            {
                //var stream = File.Open(sFileName, FileMode.Open);
                //m_stream = new StreamOnFile(stream);
                //m_size = ImageHelper.GetRawImageSize(m_stream);
                //System.Diagnostics.Debug.WriteLine("showImage loading " + sFileName + ", width/height = " + m_size.Width.ToString() + "/" + m_size.Height.ToString());
                //_imager.VideoRunning = false;
                //ImagerPreview.Image = ImageHelper.CreateThumbnail(m_stream, ImagerPreview.Width, ImagerPreview.Height);

                //ImagerPreview.Image = new Bitmap(sFileName);
#if STREAMING_ON
                ImagerSnapshot.Image = new Bitmap(sFileName);
                addLog("showImage(): calling ShowSnapShot with true");
                //showSnapshot(true);
#else
                ImagerPreview.Image = new Bitmap(sFileName);
                showVideoRunning(false);
#endif
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                addLog("showImage(): COMException: " + ex.Message);
            }
            catch (Exception ex)
            {
                addLog("showImage(): Exception: " + ex.Message);
            }
            addLog("showImage(): ended.");
        }
        delegate void setShowSnapshot(bool bShow);
        /// <summary>
        /// show or hide preview with running video or the last taken snapshot
        /// </summary>
        /// <param name="bShowHide">true to show running video
        /// false to show saved image</param>
        private void showSnapshot(bool bShowHide)
        {
            if (this.InvokeRequired)
            {
                setShowSnapshot d = new setShowSnapshot(showSnapshot);
                this.Invoke(d, bShowHide);
            }
            else
            {
                addLog("showSnapshot() called with " + bShowHide.ToString());
                if (bShowHide)
                {
                    // CameraSnapshot.BringToFront();
                    ImagerSnapshot.Visible = true;
                    ImagerPreview.Visible = false;
                    ImageIsReady();
                }
                else
                {
                    //CameraPreview.BringToFront();
                    ImagerSnapshot.Visible = false;// BringToFront();
                    ImagerPreview.Visible = true;
                    ImageIsInPreview();
                }
                _bIsSnapshotView = bShowHide;
                addLog("showSnapshot() call end.");

#if STREAMING_ON                
#else
                _imager.VideoRunning = bShowHide;
#endif
            }
        }
        private void IntermecImagerControl_Resize(object sender, EventArgs e)
        {
            addLog("+++ OnResize called...");
            if (_imager != null)
            {
                //find the best matching resolution
                //Intermec.DataCollection.Imager.PictureResolutionSize.Full; //736x450
                //resize pictbox and center it
                ImagerPreview.Width = 736 / 2;// Parent.Width; 
                ImagerPreview.Height = 450 / 2;// Parent.Height;
                ImagerPreview.Left = (this.Width - ImagerPreview.Width) / 2;
                addLog("+++ OnResize Left=" + ImagerPreview.Left.ToString());
#if STREAMING_ON
                ImagerSnapshot.Location = ImagerPreview.Location;
                ImagerSnapshot.Size = ImagerPreview.Size;
#endif
            }
        }
        private void ImagerPreview_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_imager != null)
            {
                if (!_bTakingSnapShot) //if (_imager.VideoRunning)
                    e.Graphics.DrawString("Preview", new Font("Tahoma", 8, FontStyle.Regular), new SolidBrush(Color.Orange), 10, 10);
                //else
                //    e.Graphics.DrawString("Snapshot", new Font("Tahoma", 8, FontStyle.Regular), new SolidBrush(Color.GreenYellow), 10, 10);
            }
        }

        #region Interface implementation
        #region InPreview
        delegate void SetInPreviewCallback();
        /// <summary>
        /// this event is fired on scan button down to signal the InPreview condition
        /// </summary>
        public event EventHandler InPreview;

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
                ImagerPreview.Update();
                OnInPreview(new EventArgs());
            }
        }
        protected virtual void OnInPreview(EventArgs e)
        {
            if (InPreview != null)
            {
                InPreview(this, e);
            }
        }
        #endregion
        #region ImageReady
        delegate void setImageIsReadyCallback(); //used as it is invoked from separate thread!

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
                //load saved image??
                loadImage(_TempFileName);
                this.ImagerPreview.Refresh(); //refresh control
            }
        }

        /// <summary>
        /// this event is fired when a snapshot is ready to be saved
        /// </summary>
        public event EventHandler ImageReady;
        protected virtual void OnImageReady(EventArgs e)
        {
            if (ImageReady != null)
            {
                ImageReady(this, e);
            }
        }
        #endregion
        /// <summary>
        /// method to save a snapshot to a given file
        /// </summary>
        /// <param name="Filename">full path and file name</param>
        /// <returns>True on success, false on failure</returns>
        public bool SaveAsJpg(string Filename)
        {
            try
            {
                //addLog("SaveAsJpg: Saving image as JPG '" + Filename + "'");
                //saveImage(Filename); //does not work yet
                System.IO.File.Move(_TempFileName, Filename);
                //addLog("SaveAsJpg: deleting TempFile");

                System.IO.File.Delete(_TempFileName);
                addLog("SaveAsJpg: everything OK");
                return true;
            }
            catch (Exception ex)
            {
                addLog("SaveAsJpg: Exception " + ex.Message);
                return false;
            }
        }
        #endregion //Interface Implementation
        #region EventArgs
        private enum SnapshotStatus
        {
            WriteError = -32,
            UnknownDirectory = -31,
            NotStreaming = -25,
            FatalError = -21,
            Ok = 0,
            UnknownError = 1,
        }

        /// <summary>
        /// the GUI eventhandler will get an instance of this EventArgs class
        /// the instance is generated inside the bgThread class
        /// </summary>
        private class ImagerSnapshotEventArgs : EventArgs
        {
            public string Filename;
            public SnapshotStatus Status;

            public ImagerSnapshotEventArgs(string file, SnapshotStatus stat)
            {
                Filename = file;
                Status = stat;
            }
        }
        #endregion

    }
}