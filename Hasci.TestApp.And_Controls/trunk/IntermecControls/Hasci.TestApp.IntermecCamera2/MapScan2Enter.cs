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

using ITC_KEYBOARD;

namespace Hasci.TestApp.IntermecPhotoControls2
{
    public partial class IntermecCameraControl2 : UserControl, Hasci.TestApp.DeviceControlContracts.IPhotoControl
    {
#if REMAP_SCAN_TO_ENTERKEY
        #region MapScan2Enter
        CUSBkeys.usbKeyStruct _OldUsbKey = new CUSBkeys.usbKeyStruct();
        int _iScanButtonIndex = -1;
        /// <summary>
        /// change the event names of scanbutton to StateLeftScan1 and DeltaLeftScan1
        /// </summary>
        void mapScan2Enter()
        {
            ITC_KEYBOARD.CUSBkeys _cusb = new ITC_KEYBOARD.CUSBkeys();
            ITC_KEYBOARD.CUSBkeys.usbKeyStruct _usbKey = new CUSBkeys.usbKeyStruct();
            
            //get the index of the ENTER button and load keystruct
            ITC_KEYBOARD.CUSBkeys.usbKeyStruct _usbKeyENTER = new CUSBkeys.usbKeyStruct();
            int iIdxEnter = _cusb.getKeyStruct(0, CUsbKeyTypes.HWkeys.Return, ref _usbKeyENTER);
            if (iIdxEnter != -1)
            { //we found the index
                addLog("ENTER key index is " + iIdxEnter.ToString());
                dumpKey(_usbKeyENTER);
                // 07,28,00,00,00,5A 'Return'  'Return'
            }
            
            //get the index of the scan button
            int iIdx = _cusb.getKeyStruct(0, CUsbKeyTypes.HWkeys.SCAN_Button_KeyLang1, ref _usbKey);
            //change the scan button to fire these events
            if (iIdx != -1)
            {
                _iScanButtonIndex = iIdx; //save for later use
                _OldUsbKey = _usbKey; //save for later restore
                addLog("scanbutton key index is " + iIdx.ToString());
                dumpKey(_usbKey);
                //_usbKey.bFlagHigh = CUsbKeyTypes.usbFlagsHigh.NoFlag;
                //_usbKey.bFlagMid = CUsbKeyTypes.usbFlagsMid.NOOP;
                //_usbKey.bFlagLow = CUsbKeyTypes.usbFlagsLow.NormalKey;
                //_usbKey.bIntScan = 5; //change EventIndex to 5

                //change to a normal key with ENTER as 
                _usbKey.bScanKey = CUsbKeyTypes.HWkeys.SCAN_Button_KeyLang1;
                _usbKey.bFlagHigh = CUsbKeyTypes.usbFlagsHigh.NoFlag;
                _usbKey.bFlagMid = CUsbKeyTypes.usbFlagsMid.Silent | CUsbKeyTypes.usbFlagsMid.NoRepeat;
                _usbKey.bFlagLow = CUsbKeyTypes.usbFlagsLow.NormalKey;
                _usbKey.bIntScan = 0x5A;// _usbKeyENTER.bIntScan;
                addLog("_usbKey after change to ENTER:");
                dumpKey(_usbKey);
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
        /// restore scan button mapping to point to named event 1
        /// </summary>
        void restoreScanKey()
        {
            ITC_KEYBOARD.CUSBkeys _cusb = new ITC_KEYBOARD.CUSBkeys();
            ITC_KEYBOARD.CUSBkeys.usbKeyStruct _usbKey = new CUSBkeys.usbKeyStruct();
            int iIdx = _cusb.getKeyStruct(0, CUsbKeyTypes.HWkeys.SCAN_Button_KeyLang1, ref _usbKey);
            //change the scan button back to the original events
            if (iIdx != -1)
            {
                //idx should be the save value _iScanButtonIndex
                addLog("saved scanbutton key index is " + _iScanButtonIndex.ToString());

                _usbKey = _OldUsbKey; //restore saved key
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
        void dumpKey(CUSBkeys.usbKeyStruct cusb)
        {
            addLog(string.Format(
                "Key struct is \n\tscankey={0}\n\tIntScan={1}\n\tflagHigh={2}\n\tflagMid={3}\n\tflagLow={4}",
                cusb.bScanKey.ToString(),
                cusb.bIntScan.ToString(),
                cusb.bFlagHigh.ToString(),
                cusb.bFlagMid.ToString(),
                cusb.bFlagLow.ToString()
                )
                );
        }
        #endregion
#endif
    }
}