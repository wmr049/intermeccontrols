using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Intermec.DeviceManagement.SmartSystem;

public static class YetAnotherHelperClass
{
    /// <summary>
    /// mute the speaker
    /// </summary>
    /// <param name="bMuteOn">true = no sound
    /// false = sound enabled</param>
    /// <returns></returns>
    public static bool muteSpeakerVolume(bool bMuteOn)
    {
        addLog("muteSpeakerVolume called with " + bMuteOn.ToString());
        bool bRet = true;
        Int32 max_len = 2048;
        Int32 retSize = max_len;
        StringBuilder sbAnswer = new StringBuilder(max_len);
        string setVolume = "";
        uint iSetting;
        if (bMuteOn)
            iSetting = 0;
        else
            iSetting = 1;
        uint uRes = 0;
        try
        {
            setVolume += "<Subsystem Name=\"Device Settings\">";
            setVolume += "  <Group Name=\"Volume\">";
            if(bMuteOn)
                setVolume += "  <Field Name=\"Beeper and Voice\">0</Field> ";
            else
                setVolume += "  <Field Name=\"Beeper and Voice\">5</Field> ";
            setVolume += "  </Group>";
            setVolume += "  </Subsystem>";

            ITCSSApi ssapi = new ITCSSApi();
            uRes = ssapi.Set(setVolume, sbAnswer, ref retSize, 2000);
            if (uRes == ITCSSErrors.E_SS_SUCCESS)
            {
                addLog("muteSpeakerVolume ssapi.Set OK\nAnswer='" + sbAnswer.ToString() + "'");
                addLog("muteSpeakerVolume OK" + "\n  uRes=" + uRes.ToString());
                bRet = true;
            }
            else
            {
                addLog("muteSpeakerVolume ssapi.Set FAILED. uRes=" + uRes.ToString() + "\nAnswer='" + sbAnswer.ToString() + "'");
                bRet = false;
            }
        }
        catch (Exception ex)
        {
            addLog("muteSpeakerVolume ssapi.Set FAILED\nAnswer='" + sbAnswer.ToString() + "'");
            System.Diagnostics.Debug.WriteLine("Exception in muteSpeakerVolume: " + ex.Message + "\n  uRes=" + uRes.ToString());
            bRet = false;
        }
        addLog("leaving muteSpeakerVolume");
        return bRet;

    }
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
            iSetting = 0;
        else
            iSetting = 1;
        uint uRes = 0;
        try
        {
            ssSetHWTrigger += "<Subsystem Name=\"Data Collection\">";
            ssSetHWTrigger += " <Group Name=\"Scanners\" Instance=\"0\">";
            ssSetHWTrigger += "  <Group Name=\"Scanner Settings\">";
            ssSetHWTrigger += "   <Field Name=\"Hardware trigger\">" + iSetting.ToString() + "</Field>";
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
        addLog("setNumberOfGoodReadBeeps called with " + iNumber.ToString());
        bool bRet = true;
        Int32 max_len = 2048;
        Int32 retSize = max_len;
        StringBuilder sbAnswer = new StringBuilder(max_len);
        string ssSetGoodReadBeeps = "";
        uint uRes = 0;
        try
        {
            ssSetGoodReadBeeps += "<Subsystem Name=\"Device Settings\">";
            ssSetGoodReadBeeps += " <Group Name=\"Good Read Settings\">";
            ssSetGoodReadBeeps += "  <Group Name=\"Internal Scanner\">";
            ssSetGoodReadBeeps += "   <Field Name=\"Beep or Vibrate\">" + iNumber.ToString() + "</Field>";
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
                addLog("setNumberOfGoodReadBeeps ssapi.Set FAILED. uRes=" + uRes.ToString() + "\nAnswer='" + sbAnswer.ToString() + "'");
                bRet = false;
            }
        }
        catch (Exception ex)
        {
            addLog("setNumberOfGoodReadBeeps ssapi.Set FAILED\nAnswer='" + sbAnswer.ToString() + "'");
            System.Diagnostics.Debug.WriteLine("Exception in setNumberOfGoodReadBeeps: " + ex.Message + "\n  uRes=" + uRes.ToString());
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
