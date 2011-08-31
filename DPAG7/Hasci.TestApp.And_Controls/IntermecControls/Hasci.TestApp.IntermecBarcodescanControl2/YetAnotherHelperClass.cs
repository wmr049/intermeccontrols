using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Intermec.DeviceManagement.SmartSystem;

namespace Hasci.TestApp.IntermecBarcodeScanControls3
{
    public static class YetAnotherHelperClass
    {
        public static bool setHWTrigger(bool bOnOff)
        {
            addLog("setHWTrigger called with " + bOnOff.ToString());
            bool bRet = true;
            Int32 max_len = 2048;
            Int32 retSize = max_len;
            StringBuilder sbAnswer = new StringBuilder(max_len);
            string ssSetHWTrigger = "";
            uint iSetting;
            if (bOnOff)
                iSetting = 1;
            else
                iSetting = 0;
            uint uRes = 0;
            try
            {
                ssSetHWTrigger += "<Subsystem Name=\"Data Collection\">";
                ssSetHWTrigger += " <Group Name=\"Scanners\" Instance=\"0\">";
                ssSetHWTrigger += "  <Group Name=\"Scanner Settings\">";
                ssSetHWTrigger += "   <Field Name=\"Hardware trigger\">"+iSetting.ToString()+"</Field>";
                ssSetHWTrigger += "  </Group>";
                ssSetHWTrigger += " </Group>";
                ssSetHWTrigger += "</Subsystem>";

                ITCSSApi ssapi = new ITCSSApi();
                uRes = ssapi.Set(ssSetHWTrigger, sbAnswer, ref retSize, 2000);
                if (uRes == ITCSSErrors.E_SS_SUCCESS)
                {
                    addLog("setHWTrigger ssapi.Set OK\nAnswer='" + sbAnswer.ToString() + "'");
                    addLog("setHWTrigger OK" + "\n  uRes=" + uRes.ToString());
                    bRet = true;
                }
                else
                {
                    addLog("setHWTrigger ssapi.Set FAILED. uRes=" + uRes.ToString() + "\nAnswer='" + sbAnswer.ToString() + "'");
                    bRet = false;
                }
            }
            catch (Exception ex)
            {
                addLog("setHWTrigger ssapi.Set FAILED\nAnswer='" + sbAnswer.ToString() + "'");
                System.Diagnostics.Debug.WriteLine("Exception in setHWTrigger: " + ex.Message + "\n  uRes=" + uRes.ToString());
                bRet = false;
            }
            addLog("leaving setHWTrigger");
            return bRet;
        }
        public static bool setNumberOfGoodReadBeeps(int iNumber)
        {
            addLog("setNumberOfGoodReadBeeps called with "+iNumber.ToString());
            bool bRet = true;
            Int32 max_len = 2048;
            Int32 retSize = max_len;
            StringBuilder sbAnswer = new StringBuilder(max_len);
            string ssSetGoodReadBeeps = "";
            uint uRes=0;
            try
            {
                ssSetGoodReadBeeps += "<Subsystem Name=\"Device Settings\">";
                ssSetGoodReadBeeps += " <Group Name=\"Good Read Settings\">";
                ssSetGoodReadBeeps += "  <Group Name=\"Internal Scanner\">";
                ssSetGoodReadBeeps += "   <Field Name=\"Beep or Vibrate\">" + iNumber.ToString() +"</Field>";
                ssSetGoodReadBeeps += "  </Group>";
                ssSetGoodReadBeeps += " </Group>";
                ssSetGoodReadBeeps += "</Subsystem>";

                ITCSSApi ssapi = new ITCSSApi();
                uRes = ssapi.Set(ssSetGoodReadBeeps, sbAnswer, ref retSize, 2000);
                if (uRes == ITCSSErrors.E_SS_SUCCESS)
                {
                    addLog("setNumberOfGoodReadBeeps ssapi.Set OK\nAnswer='" + sbAnswer.ToString() + "'");
                    addLog("setNumberOfGoodReadBeeps OK" + "\n  uRes=" + uRes.ToString());
                    bRet = true;
                }
                else
                {
                    addLog("setNumberOfGoodReadBeeps ssapi.Set FAILED. uRes="+uRes.ToString()+"\nAnswer='" + sbAnswer.ToString() + "'");
                    bRet = false;
                }
            }
            catch (Exception ex)
            {
                addLog("setNumberOfGoodReadBeeps ssapi.Set FAILED\nAnswer='" + sbAnswer.ToString() + "'");
                System.Diagnostics.Debug.WriteLine("Exception in setNumberOfGoodReadBeeps: " + ex.Message+"\n  uRes="+uRes.ToString());
                bRet = false;
            }
            addLog("leaving setNumberOfGoodReadBeeps");
            return bRet;
        }
        static void addLog(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

    }
}
