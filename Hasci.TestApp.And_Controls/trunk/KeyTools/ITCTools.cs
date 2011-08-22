using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using ITC_KEYBOARD;

namespace ITCTools
{
    public static class KeyBoard
    {
        /// <summary>
        /// we have to save and restore the keyboard mapping of the scan button
        /// </summary>
        static ITC_KEYBOARD.CUSBkeys.usbKeyStruct _OldUsbKey = new ITC_KEYBOARD.CUSBkeys.usbKeyStruct();

        /// <summary>
        /// change the event names of scanbutton to StateLeftScan1 and DeltaLeftScan1
        /// </summary>
        public static void mapKey()
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
        /// <summary>
        /// restore scan button mapping to point to named event 1
        /// </summary>
        public static void restoreKey()
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
        /// for debug use we can log messages to DebugOut
        /// </summary>
        /// <param name="s"></param>
        static void addLog(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }
    }
}
