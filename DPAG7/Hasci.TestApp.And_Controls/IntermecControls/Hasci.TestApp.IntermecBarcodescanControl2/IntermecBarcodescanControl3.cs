//#define TESTMODE
//scan some barcodes in a given sequence as fast as possible
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.ComponentModel.Composition;

using Intermec.DataCollection;
using System.Threading;
using NativeSync;

namespace Hasci.TestApp.IntermecBarcodeScanControls3
{
    /// <summary>
    /// This is the Intermec Barcode Reader Control for MEF
    /// After init you can press the BarcodeScannerButton to start Barcode Scanning
    /// If a barcode is recognized, the scan will stop
    /// If you release the BarcodeScannerButton before a Barcode is recognized, you will 
    /// get a unsuccessfull barcode scan
    /// 
    /// Interface:
    /// + EventHandler ScanReady
    ///   fired at the end of a successfull or stopped scan
    /// + string BarcodeText
    ///   delivers the barcode data as string or, if unsuccessfull scan, the string "Barcode nicht anzeigbar"
    /// + bool IsSuccess
    ///   will be true for successfull barcode scan, false for unsuccessfull barcode scans
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(Hasci.TestApp.DeviceControlContracts.IBarcodeScanControl))]
    public partial class IntermecBarcodescanControl3 : UserControl, Hasci.TestApp.DeviceControlContracts.IBarcodeScanControl
    {
#if TESTMODE
        string[] testcodes = { "0123456789", "CODE-39", "9781860742712", "CODE-39", "05012345678900", "CODE-39", "0123456789", "CODE-39" };
        int testCodeCount = 8;
        int testCodePos = 0;
        int testBarcodeReadCount=0;
        /// <summary>
        /// number of codes read in correct sequence
        /// </summary>
        int testGoodReadCount=0;
#endif
        /// <summary>
        /// the barcode reader object
        /// </summary>
        BarcodeReader bcr;// = new BarcodeReader();
        /// <summary>
        /// a fixed error text for failed barcode scans
        /// </summary>
        private const string _sErrorText = "Barcode nicht anzeigbar";
        /// <summary>
        /// internal var to hold current barcode data
        /// </summary>
        private string _BarcodeText = "Barcode nicht anzeigbar";
        /// <summary>
        /// sync access to _BarcodeText using a lock object
        /// </summary>
        private object lockBarcodeData = new object();
        /// <summary>
        /// internal var to hold good/bad scan
        /// </summary>
        private bool _IsSuccess = false;
        /// <summary>
        /// control var to avoid multiple calls to barcode scans
        /// </summary>
        private bool _bReadingBarcode = false;
        /// <summary>
        /// thread stop var
        /// </summary>
        bool _continueWait = true;
        /// <summary>
        /// thread that starts a barcode read
        /// </summary>
        Thread readThread;
        /// <summary>
        /// thread that watches the scan button
        /// </summary>
        System.Threading.Thread waitThread;

        public IntermecBarcodescanControl3()
        {
            InitializeComponent();
            try
            {
                addLog("IntermecBarcodescanControl2: setHWTrigger(true)...");
                //enable HW trigger, a workaround as HW trigger is sometimes disabled on BarcodeReader.Dispose()
                //if (!S9CconfigClass.S9Cconfig.HWTrigger.setHWTrigger(true))
                //{
                //    addLog("IntermecBarcodescanControl2: setHWTrigger(true)...FAILED. Trying again");
                //    Thread.Sleep(50);
                //    S9CconfigClass.S9Cconfig.HWTrigger.setHWTrigger(true); //try again
                //}
                //replaced above code with the following, there were problems with load/unload the ADCComInterface inside S9CConfig
                YetAnotherHelperClass.setHWTrigger(true);
                //change number of good read beeps to zero
                YetAnotherHelperClass.setNumberOfGoodReadBeeps(0);
                addLog("IntermecBarcodescanControl2: mapKey()...");
                //we need full control of scan start and end
                ITCTools.KeyBoard.mapKey();
            }
            catch (Exception ex)
            {
                addLog("Exception in IntermecBarcodescanControl2: setHWTrigger(true)..." + ex.Message);
            }
            try
            {
                addLog("IntermecBarcodescanControl2: new BarcodeReader()...");
                bcr = new BarcodeReader(this, "default");// ();
                addLog("IntermecBarcodescanControl2: BarcodeReader adding event handlers...");
                bcr.BarcodeRead += new BarcodeReadEventHandler(bcr_BarcodeRead);
                bcr.BarcodeReadCanceled += new BarcodeReadCancelEventHandler(bcr_BarcodeReadCanceled);
                bcr.BarcodeReadError += new BarcodeReadErrorEventHandler(bcr_BarcodeReadError);
                addLog("IntermecBarcodescanControl2: starting named event watch thread...");
                waitThread = new System.Threading.Thread(waitLoop);
                waitThread.Start();
                addLog("IntermecBarcodescanControl2: enabling Scanner...");
                bcr.ScannerEnable = true;
                addLog("IntermecBarcodescanControl2: ScannerOn=false...");
                bcr.ScannerOn = false;

            }
            catch (BarcodeReaderException ex)
            {
                bcr = null;
                System.Diagnostics.Debug.WriteLine("BarcodeReaderException in IntermecScanControl(): " + ex.Message);
            }
            catch (Exception ex)
            {
                bcr = null;
                System.Diagnostics.Debug.WriteLine("Exception in IntermecScanControl(): " + ex.Message);
            }
            if (bcr == null)
            {
                addLog("IntermecBarcodescanControl2: BarcodeReader init FAILED");
                throw new System.IO.FileNotFoundException("Intermec.Datacollection.dll or ITCScan.DLL missing");
            }
#if TESTMODE
            //testcodes = new string[8];
            testCodeCount = testcodes.Length;
            testCodePos = 0;
#endif

        }


        /// <summary>
        /// for debug use we can log messages to DebugOut
        /// </summary>
        /// <param name="s"></param>
        void addLog(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }
        /// <summary>
        /// the main thread watching for state and delta events of scan button
        /// </summary>
        void waitLoop()
        {
            addLog("waitLoop starting...");
            try
            {
                SystemEvent[] _events = new SystemEvent[3];
                addLog("waitLoop setting up event array...");
                _events[0] = new SystemEvent("StateLeftScan1", false, false);
                _events[1] = new SystemEvent("DeltaLeftScan1", false, false);
                _events[2] = new SystemEvent("EndWaitLoop52", false, false);
                do
                {
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
                    addLog("waitLoop sleep(1)");
                    System.Threading.Thread.Sleep(1);
                } while (_continueWait);
                addLog("waitLoop while ended by _continueWait");
            }
            catch (ThreadAbortException ex)
            {
                System.Diagnostics.Debug.WriteLine("waitLoop: ThreadAbortException: " + ex.Message);
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("waitLoop: Exception: " + ex.Message);
            }
            addLog("...waitLoop EXIT");
        }
        /// <summary>
        /// called if a barcode read error (whatever that means) occured 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="breErr"></param>
        void bcr_BarcodeReadError(object sender, BarcodeReadErrorEventArgs breErr)
        {
            addLog("bcr_BarcodeReadError: " + breErr.errMessage);
        }
        /// <summary>
        /// eventhandler that is invoked on barcode CancelRead calls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="bce"></param>
        void bcr_BarcodeReadCanceled(object sender, BarcodeReadCancelEventArgs bce)
        {
            addLog("bcr_BarcodeReadCanceled...");
            lock (lockBarcodeData){
                _BarcodeText = _sErrorText;
                _IsSuccess = false;
            }
            ScanIsReady();
            addLog("bcr_BarcodeReadCanceled: disable scanner");
            scannerOnOff(false);
            addLog("...bcr_BarcodeReadCanceled");
        }
        /// <summary>
        /// eventhandler for a recognized barcode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="bre">data about the barcode that has been read</param>
        void bcr_BarcodeRead(object sender, BarcodeReadEventArgs bre)
        {
            addLog("bcr_BarcodeRead...");
            // ###### changed to provide also 2D barcodes ##########
            //int iSym = bre.SymbologyDetail;
            //addLog("bcr_BarcodeRead: setting vars");
            //if (barcodeID.is2Dcode(iSym))
            //    _BarcodeText = "2D Code gescannt";
            //else
            lock (lockBarcodeData)
            {
                _BarcodeText = Encoding.UTF8.GetString(bre.DataBuffer, 0, bre.BytesInBuffer);
                changeText( _BarcodeText);
            }
#if TESTMODE
            if (testcodes[testCodePos] == _BarcodeText)
            {
                addLog("testcode '" + testcodes[testCodePos] + "' match found");
                testGoodReadCount++;
                _IsSuccess = true;
            }
            else
            {
                addLog("barcode does not match: current='" + testcodes[testCodePos] + "' scanned='" + _BarcodeText + "'");
                _IsSuccess = false;
            }

            testBarcodeReadCount++;
            addLog("testBarcodeReadCount=" + testBarcodeReadCount.ToString());
            addLog("testGoodReadCount=" + testGoodReadCount.ToString());
            testCodePos++;
            if(testCodePos>=testCodeCount)
                testCodePos=0;
            addLog("testCodePos=" + testCodePos.ToString());
#else
            lock (lockBarcodeData)
            {
                _IsSuccess = true;
            }
#endif

            addLog("bcr_BarcodeRead calling ScanIsReady()");
            ScanIsReady();
            addLog("...bcr_BarcodeRead end.");
        }

        /// <summary>
        /// switch scanner off and Cancel a current Read
        /// </summary>
        /// <param name="bOnOff"></param>
        private void scannerOnOff(bool bOnOff)
        {
            if(bOnOff)
                addLog("scannerOnOff called with TRUE...");
            else
                addLog("scannerOnOff called with FALSE...");
            if (bcr != null)
            {
                bcr.ScannerOn = bOnOff;
                bcr.ScannerEnable = bOnOff;
                if (!bOnOff)
                {                    
                    if(_bReadingBarcode)
                        bcr.CancelRead(true);
                }
            }
            addLog("...scannerOnOff ended");
        }
        /// <summary>
        /// this thread simply starts a one shot trial to scan a barcode
        /// </summary>
        private void readBarcodeThread()
        {
            addLog("readBarcodeThread starting...");
            if (bcr == null)
            {
                addLog("...readBarcodeThread returned for bcr==null");
                return;
            }
            if (_bReadingBarcode)
            {
                addLog("...readBarcodeThread returned for _bReadingBarcode (in progress)");
                return;
            }
            addLog("readBarcodeThread setting up vars...");
            _bReadingBarcode = true;
            lock (lockBarcodeData)
            {
                _BarcodeText = "";
                _IsSuccess = false;
            }
            try
            {
                addLog("readBarcodeThread: enabling BarcodeReader and ScannerOn...");
                scannerOnOff(true);
                //start a scan
                addLog("readBarcodeThread: allow one Read...");
                bcr.ThreadedRead(false);
            }
            catch (ThreadAbortException ex)
            {
                addLog("readBarcodeThread: ThreadAbortException:"+ex.Message);
            }
            catch (Exception ex)
            {
                addLog("readBarcodeThread Exception:"+ex.Message);
            }
            addLog("...readBarcodeThread end");
        }

        /// <summary>
        /// var to avoid mutiple calls into OnStateScan
        /// </summary>
        bool _bLastState = false;
        /// <summary>
        /// function that is called periodically during KeyPress
        /// </summary>
        void onStateScan()
        {
            addLog("onStateScan started...");
            if (_bLastState)
            { //already pressed
                addLog("...onStateScan: already pressed (_bLastState)");
                return;
            }
            changeText("");
            _bLastState = true; //avoid multiple calls
            //start a scan
            addLog("onStateScan starting Read thread...");
            readThread = new Thread(readBarcodeThread);
            readThread.Start();
            addLog("...onStateScan ended");
        }
        /// <summary>
        /// function that is called on every KeyRelease
        /// </summary>
        void onDeltaScan()
        {
            addLog("onDeltaScan started...");
            //stop scan
            addLog("onDeltaScan checking thread...");
            if (readThread != null)
            {
                addLog("onDeltaScan ending thread...");
                readThread.Abort();
            }
            addLog("onDeltaScan testing _IsSuccess ...");
            bool __success;
            lock(lockBarcodeData)
            { 
                __success = _IsSuccess; 
            }
            if (!__success)
            {
                addLog("onDeltaScan invoking CancelRead()");
                bcr.CancelRead(true);
            }
            _bLastState = false;
        }
        /// <summary>
        /// this text gives the barcode data
        /// </summary>
        public string BarcodeText
        {
            get {
                lock (lockBarcodeData)
                {
                    return _BarcodeText;
                }
            }
        }
        /// <summary>
        /// return a bool for a good/bad scan
        /// </summary>
        public bool IsSuccess
        {
            get {
                bool __success;
                lock(lockBarcodeData)
                { 
                    __success = _IsSuccess; 
                }
                return __success; 
            }
        }
        //Create an event
        //public event EventHandler ScanReady;
        public event Hasci.TestApp.DeviceControlContracts.BarcodeEventHandler ScanReady;
        /// <summary>
        /// this will be called for successful and faulty scans
        /// </summary>
        private void ScanIsReady()
        {
            addLog("ScanIsReady started...");
            OnScanReady(new EventArgs());
            _bReadingBarcode = false;
        }
        protected virtual void OnScanReady(EventArgs e)
        {
            if (ScanReady != null)
            {
                lock (lockBarcodeData)
                {
                    ScanReady(this, new Hasci.TestApp.DeviceControlContracts.BarcodeEventArgs(_BarcodeText, _IsSuccess));
                }
            }
        }
        public new void Dispose()
        {
            addLog("IntermecScanControl Dispose()...");
            _continueWait = false; //signal thread to stop
            SystemEvent waitEvent = new SystemEvent("EndWaitLoop52", false, false);
            waitEvent.PulseEvent();
            System.Threading.Thread.Sleep(100);

            if (bcr != null)
            {
                addLog("IntermecScanControl Dispose(): Calling ScannerOnOff(false)...");
                scannerOnOff(false);
                addLog("IntermecScanControl Dispose(): Disposing BarcodeReader...");
                //bcr.ThreadedRead(false);
                bcr.BarcodeRead -= bcr_BarcodeRead;
                bcr.Dispose();
                bcr = null;
                addLog("IntermecScanControl Dispose(): BarcodeReader disposed");
            }
            try
            {
                addLog("IntermecScanControl Dispose(): enabling HardwareTrigger...");
                //replaced this call as ADCComInterface made problems
                //S9CconfigClass.S9Cconfig.HWTrigger.setHWTrigger(true);
                YetAnotherHelperClass.setHWTrigger(true);
                YetAnotherHelperClass.setNumberOfGoodReadBeeps(1);
            }
            catch (Exception ex)
            {
                addLog("IntermecScanControl Dispose(), Exception: enabling HardwareTrigger: "+ex.Message);
            }
            addLog("IntermecScanControl Dispose(): restoring Scan Button Key...");
            ITCTools.KeyBoard.restoreKey();
            addLog("IntermecScanControl Dispose(): ending Threads...");
            if (waitThread != null)
            {
                addLog("IntermecScanControl Dispose(): ending event watch thread ...");
                waitThread.Abort();
            }
            if (readThread != null)
            {
                addLog("IntermecScanControl Dispose(): ending Barcode reading thread ...");
                readThread.Abort();
            }
            addLog("...IntermecScanControl Dispose(): end.");
            //base.Dispose(); do not use!!
        }
        delegate void SetTextCallback(string text);
        private void changeText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtBoxScan.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(addLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                txtBoxScan.Text = text;
            }
        }
    }

    [Obsolete("currently unused")]
    /// <summary>
    /// helper class to differ between 1D and 2D barcodes
    /// 
    /// </summary>
    public static class barcodeID{
        public static bool is2Dcode(int iCode){
            bool bRet=false;
            switch(iCode){
                case 33:
                case 36:
                case 40:
                case 41:
                    bRet = true;
                    break;
                default:
                    bRet = false;
                    break;
            }
            return bRet;
        }
        enum tagStcIdentifier
        {
           ID_NULL                 = 0, //Not supported
           ID_EAN13                = 1,
           ID_EAN8                 = 2,
           ID_UPCA                 = 3,
           ID_UPCE                 = 4,
           ID_EAN13_ADD2           = 5,
           ID_EAN8_ADD2            = 6,
           ID_UPCA_ADD2            = 7,
           ID_UPCE_ADD2            = 8,
           ID_EAN13_ADD5           = 9,
           ID_EAN8_ADD5            = 10,
           ID_UPCA_ADD5            = 11,
           ID_UPCE_ADD5            = 12,
           ID_39                   = 13,
           ID_RESERVED1            = 14, //Not supported
           ID_ITF                  = 15,
           ID_25S                  = 16,
           ID_25M                  = 17,
           ID_RESERVED2            = 18, //Not supported
           ID_CODABAR              = 19,
           ID_AMES                 = 20, //Not supported
           ID_MSI                  = 21,
           ID_PLESSEY              = 22,
           ID_128                  = 23,
           ID_16K                  = 24, //Not supported
           ID_93                   = 25,
           ID_11                   = 26,
           ID_TELEPEN              = 27,
           ID_49                   = 28, //Not supported
           ID_39CPI                = 29,  
           ID_CDBCK_A              = 30,
           ID_CDBCK_F              = 31,
           ID_CDBCK_256            = 32, //Not supported
           ID_PDF                  = 33,                    //2D
           ID_GS1_128              = 34,
           ID_ISBT128              = 35,
           ID_MICRO_PDF = 36,                               //2D
           ID_GS1_OD               = 37,
           ID_GS1_LI               = 38,
           ID_GS1_EX               = 39,
           ID_DATAMATRIX = 40,                              //2D
           ID_QR = 41,                                      //2D
           ID_MAXICODE             = 42,
           ID_GS1_OD_CCA           = 43,
           ID_GS1_LI_CCA           = 44,
           ID_GS1_EX_CCA           = 45,
           ID_GS1_128_CCA          = 46,
           ID_EAN13_CCA            = 47,
           ID_EAN8_CCA             = 48,
           ID_UPCA_CCA             = 49,
           ID_UPCE_CCA             = 50,
           ID_GS1_OD_CCB           = 51,
           ID_GS1_LI_CCB           = 52,
           ID_GS1_EX_CCB           = 53,
           ID_GS1_128_CCB          = 54,
           ID_EAN13_CCB            = 55,
           ID_EAN8_CCB             = 56,
           ID_UPCA_CCB             = 57,
           ID_UPCE_CCB             = 58,
           ID_GS1_128_CCC          = 59,
           ID_ISBN                 = 60,
           ID_POSTNET              = 61,
           ID_PLANET               = 62,
           ID_BPO                  = 63,
           ID_CANADAPOST           = 64,
           ID_AUSTRALIANPOST       = 65,
           ID_JAPANPOST            = 66,
           ID_DUTCHPOST            = 67,
           ID_CHINAPOST            = 68,
           ID_KOREANPOST           = 69, //Not supported
           ID_TLC39                = 70,
           ID_TRIOPTIC             = 71, //Not supported as a symbology, but as a derivative of code 39
           ID_ISMN                 = 72,
           ID_ISSN                 = 73,
           ID_AZTEC                = 74,
           ID_SWEDENPOST           = 75,
           ID_INFOMAIL             = 76,
           ID_MULTICODE            = 77,
           ID_INCOMPLETE_MULTICODE = 78,
           ID_INTELLIGENTMAIL      = 79,
           ID_LAST                 = 79
        } ;

    }
}