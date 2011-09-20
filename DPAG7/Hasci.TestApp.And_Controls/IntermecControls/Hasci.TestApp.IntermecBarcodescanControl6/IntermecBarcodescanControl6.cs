//scan some barcodes in a given sequence as fast as possible
//#define TESTMODE
//#define RANDOMBADSCANS
//the following define enables usage of new NoBarcodeRead event
#define NoBarcodeRead

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

namespace Hasci.TestApp.IntermecBarcodeScanControls6
{
    /*  problem 31.08.
     *  scan long barcode on p. 12 of test doc fast and often
     *  produce a wrong scan
     *  scan the short code of p. 12
     *  ERROR: scanner shows old barcode!
     *  ERROR: press scan into the the air and may get the short barcode
     *  
     * Now started this BarcodeScanControl to use the BarcodeReader as it was designed
     */
    /* started 02. sept 2011 a version that maps scanbutton to multikey and these multikeys are linked to
     * StateLeftScan, DeltaLeftScan and StateLeftScan1, DeltaLeftScan1 
     * so the code is able to get information about scanbutton pressed/released in parallel to the DCE
     * switch code on/off by #def USEWAITLOOP
     */
    /*
     * This BarcodeScanControl6 will work with a changed DCE:
       •  The DCE will produce a zero length bar code when the user stops the decode session (i.e. user releases trigger, etc.).  
       •  When the IDL receives the zero length bar code, it generates a BarcodeReadError event.
       •  The “Bad Read” feature will be enabled from a registry setting which is read on boot.
    
     * * The new DCE will fire the BarcodeReadEvent with BarcodeData="" (empty string) for 'canceled' (bad) barcode reads
     * a 'bad' read is where a Datacollection Session has been started and ended without a barcode read in between
     * 
     * 20.sept 2011 re-added mapAllSide2SCAN()
    */
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
    public partial class IntermecBarcodescanControl5 : UserControl, Hasci.TestApp.DeviceControlContracts.IBarcodeScanControl
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
        /// internal var to hold good/bad scan
        /// </summary>
        private bool _IsSuccess = false;
#if RANDOMBADSCANS
        RandomClass rc;
#endif
        public IntermecBarcodescanControl5()
        {
            InitializeComponent();
            try
            {
#if RANDOMBADSCANS
                rc=new RandomClass(0.3);
#endif
                addLog("IntermecBarcodescanControl5: setHWTrigger(true)...");
                //enable HW trigger, a workaround as HW trigger is sometimes disabled on BarcodeReader.Dispose()
                //if (!S9CconfigClass.S9Cconfig.HWTrigger.setHWTrigger(true))
                //{
                //    addLog("IntermecBarcodescanControl5: setHWTrigger(true)...FAILED. Trying again");
                //    Thread.Sleep(50);
                //    S9CconfigClass.S9Cconfig.HWTrigger.setHWTrigger(true); //try again
                //}
                //replaced above code with the following, there were problems with load/unload the ADCComInterface inside S9CConfig
                YetAnotherHelperClass.setHWTrigger(true);
                //change number of good read beeps to zero
                YetAnotherHelperClass.setNumberOfGoodReadBeeps(0);
                addLog("IntermecBarcodescanControl5: mapKey()...");
                ITCTools.KeyBoard.mapKey();
                //we use the standard scan key assignement:
                //addLog("IntermecBarcodescanControl5: restoreScanKeyDefault()...");
                //ITCTools.KeyBoard.restoreScanKeyDefault();
            }
            catch (Exception ex)
            {
                addLog("Exception in IntermecBarcodescanControl5: setHWTrigger(true)..." + ex.Message);
            }
            try
            {
                //Cannot use Keydown etc within a usercontrol, we will not get the events!!!!!
                //ITCTools.KeyBoard.createMultiKey2Events();
                addLog("IntermecBarcodescanControl5: new BarcodeReader()...");
                //create a new BarcodeReader instance
                bcr = new BarcodeReader(this, "default");// ();
                try
                {
#if NoBarcodeRead
                    //modded code for new NoBarcodeRead feature
                    bcr.NoBarcodeRead += new NoBarcodeReadEventHandler(bcr_NoBarcodeRead);
                    bcr.EnableNoBarcodeReadEvent = true;
                    addLog("Setting bcr.EnableNoBarcodeReadEvent. OK?");
#endif
                }
                catch (Exception)
                {
                    addLog("Unable to set bcr.EnableNoBarcodeReadEvent. Old DLL?");
                }
                addLog("IntermecBarcodescanControl5: BarcodeReader adding event handlers...");
                bcr.BarcodeRead += new BarcodeReadEventHandler(bcr_BarcodeRead);
                bcr.BarcodeReadCanceled += new BarcodeReadCancelEventHandler(bcr_BarcodeReadCanceled);
                bcr.BarcodeReadError += new BarcodeReadErrorEventHandler(bcr_BarcodeReadError);
                addLog("IntermecBarcodescanControl5: enabling Scanner...");
                bcr.ScannerEnable = true;
                addLog("IntermecBarcodescanControl5: ScannerOn=false...");
                bcr.ScannerOn = false;
                addLog("Enabling event driver scanning");
                bcr.ThreadedRead(true);
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
                addLog("IntermecBarcodescanControl5: BarcodeReader init FAILED");
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
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debug.WriteLine(s);
            //else
            //    System.Threading.Thread.Sleep(1);
        }
#if NoBarcodeRead
        /// <summary>
        /// called if a barcode read error (whatever that means) occured 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="breErr"></param>
        void bcr_BarcodeReadError(object sender, BarcodeReadErrorEventArgs breErr)
        {            
            addLog("bcr_BarcodeReadError: " + breErr.errMessage);
            addLog("bcr_BarcodeReadError: firing ScanIsReady with error");
            ScanIsReady(_sErrorText, false);
        }
#endif
        /// <summary>
        /// eventhandler that is invoked on barcode CancelRead calls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="bce"></param>
        void bcr_BarcodeReadCanceled(object sender, BarcodeReadCancelEventArgs bce)
        {            
            addLog("bcr_BarcodeReadCanceled...");
            //lock (lockBarcodeData){
            //    _BarcodeText = _sErrorText;
            //    _IsSuccess = false;
            //}
            //direct call!
            addLog("bcr_BarcodeReadCanceled calling ScanIsReady()");
            ScanIsReady(_sErrorText, false);
            //ScanIsReady(); //indirect call

            addLog("...bcr_BarcodeReadCanceled ended");
        }

        /// <summary>
        /// new barcode object event handler
        /// supports a NoBarcodeRead feature
        /// user pressed/released scan button without any barcode read in between
        /// signals datacollection session started and ended without a barcode read
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="noBre"></param>
        void bcr_NoBarcodeRead(object sender, NoBarcodeReadEventArgs noBre)
        {
            ScanIsReady(_sErrorText, false);
        }


        /// <summary>
        /// eventhandler for a recognized barcode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="bre">data about the barcode that has been read</param>
        void bcr_BarcodeRead(object sender, BarcodeReadEventArgs bre)
        {
            addLog("bcr_BarcodeRead...");
#if RANDOMBADSCANS
            if(rc.getRandom())
                bre.BytesInBuffer = 0;
#endif
            //direct call!
            addLog("bcr_BarcodeRead calling ScanIsReady()");
            if (bre.BytesInBuffer == 0) //is this a bad scan?
                ScanIsReady(_sErrorText, false);
            else
                ScanIsReady(bre.strDataBuffer, true);
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
#endif
            //addLog("bcr_BarcodeRead calling ScanIsReady()");
            //ScanIsReady();
            addLog("...bcr_BarcodeRead end.");
        }

        
        /// <summary>
        /// this text gives the barcode data
        /// </summary>
        public string BarcodeText
        {
            get {
                    return _BarcodeText;
            }
        }
        /// <summary>
        /// return a bool for a good/bad scan
        /// </summary>
        public bool IsSuccess
        {
            get {
                return _IsSuccess; 
            }
        }
        //Create an event, do not use directly!
        //public event EventHandler ScanReady;
        public event Hasci.TestApp.DeviceControlContracts.BarcodeEventHandler ScanReady;
        delegate void deleScanIsReady(string sData, bool bIsSuccess);
        /// <summary>
        /// this will be called for successful and faulty scans
        /// </summary>
        private void ScanIsReady(string sData, bool bIsSuccess)
        {
            addLog("ScanIsReady started...");
            if (this.InvokeRequired)
            {
                deleScanIsReady d = new deleScanIsReady(ScanIsReady);
                this.Invoke(d, new object[]{sData,bIsSuccess});
            }
            else
            {
                //OnScanReady(new EventArgs());
                _BarcodeText = sData;
                _IsSuccess = bIsSuccess;
                OnScanReady(new Hasci.TestApp.DeviceControlContracts.BarcodeEventArgs(sData, bIsSuccess)); //call event fire function
                //_bReadingBarcode = false;
            }
        }
        protected virtual void OnScanReady(EventArgs e)
        {
            if (ScanReady != null) //check if there is any listener
            {
                //fire event
                ScanReady(this, new Hasci.TestApp.DeviceControlContracts.BarcodeEventArgs(_BarcodeText, _IsSuccess));
            }
        }
        protected virtual void OnScanReady(Hasci.TestApp.DeviceControlContracts.BarcodeEventArgs e)
        {
            if (ScanReady != null)
            {
                ScanReady(this, e);
            }
        }
        public new void Dispose()
        {
            addLog("IntermecScanControl Dispose()...");
            //dispose BarcodeReader
            if (bcr != null)
            {
//                addLog("IntermecScanControl Dispose(): Calling CancelRead(true)...");
//                bcr.CancelRead(true);
                addLog("IntermecScanControl Dispose(): Disposing BarcodeReader...");
                bcr.BarcodeRead -= bcr_BarcodeRead;
                bcr.BarcodeReadCanceled -= bcr_BarcodeReadCanceled;
                bcr.BarcodeReadError -= bcr_BarcodeReadError;
                try
                {
                    bcr.NoBarcodeRead -= bcr_NoBarcodeRead;
                    bcr.EnableNoBarcodeReadEvent = false;
                }
                catch (Exception)
                {
                    addLog("Unable to unset bcr.EnableNoBarcodeReadEvent. Old DLL?");
                }
                bcr.ThreadedRead(false);
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

        private void IntermecBarcodescanControl5_KeyDown(object sender, KeyEventArgs e)
        {
            addLog("BarcodeScanControl5 got Keydown: " + e.KeyCode.ToString());
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