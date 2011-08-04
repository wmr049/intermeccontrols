#pragma warning disable 649

using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Composition;

using Intermec.Device;
using System.Runtime.InteropServices;

namespace Hasci.TestApp.IntermecUtilityControls
{
    /// <summary>
    /// A set of utilities to control device features
    /// 
    /// Interfaces:
    /// + Vibration(duration)
    /// + GoodSound(duration)
    /// + BadSound(duration)
    /// + int AccuState()
    ///   returns the current charge level of the battery
    /// + KeyboardBacklightState(bool)
    ///   switches the keyboard backlight on or off
    /// + SipState(bool)
    ///   Shows or hides the on-screen keyboard
    ///   CHANGED: disables/enables the SIP button
    /// </summary>
    [Export(typeof(Hasci.TestApp.DeviceControlContracts.IUtilityControl))]
    public partial class IntermecUtilityControl: UserControl,Hasci.TestApp.DeviceControlContracts.IUtilityControl
    {
        public IntermecUtilityControl()
        {
            if (!System.IO.File.Exists("\\Windows\\ITC50.DLL"))
                throw new System.IO.FileNotFoundException("Missing ITC50.DLL");
        }

        /// <summary>
        /// get the actual battery load as percent value
        /// </summary>
        /// <returns>percent of load or 0</returns>
        [DllImport("ITC50.DLL", SetLastError=true)]
        private extern static UInt32 ITCPowerStatus(out UInt32 lpdwLineStatus, out UInt32 lpdwBatteryStatus, out UInt32 lpdwBackupStatus, out UInt32 puFuelGauge);
        public int AccuState
        {
            get {
                UInt32 dwLineStatus = 0;
                UInt32 dwBattStatus = 0;
                UInt32 dwBackupStatus = 0;
                UInt32 dwFuelGauge = 0;
                UInt32 hRes =ITCPowerStatus(out dwLineStatus, out dwBattStatus, out dwBackupStatus, out dwFuelGauge);
                if (dwFuelGauge != 0xFF)
                    return (int)dwFuelGauge;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Show/hide the Software Input Panel
        /// </summary>
        /// <param name="dwFlag"></param>
        /// <returns></returns>
        [DllImport("coredll.dll", SetLastError=true)]
        private static extern UInt32 SipShowIM(SipStateValues dwFlag);
        private enum SipStateValues{
            SIPF_OFF    =0x00000000,
            SIPF_ON     =0x00000001
        }
        public bool SipState1
        {
            set
            {
                if (value)
                    SipShowIM(SipStateValues.SIPF_ON);
                else
                    SipShowIM(SipStateValues.SIPF_OFF);
            }
        }

        [DllImport("coredll.dll", SetLastError=true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        #region ShowIwndow stuff
        [DllImport("coredll.dll", SetLastError=true)]
        static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);
        [DllImport("coredll.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }
        private enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window is minimized
            /// or maximized, the system restores it to its original size and
            /// position. An application should specify this flag when displaying
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position.
            /// This value is similar to "ShowNormal", except the window is not
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is
            /// similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This
            /// value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is
            /// minimized or maximized, the system restores it to its original size
            /// and position. An application should specify this flag when restoring
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified in the
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread
            /// that owns the window is hung. This flag should only be used when
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }
#endregion
        public bool SipState
        {
            set
            {
                showSIPWindow(value);
                //enableSIPbutton(value);
            }
        }
        [DllImport("coredll.dll", SetLastError=true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        private void enableSIPbutton(bool bEnable)
        {
            IntPtr hWndSIP=IntPtr.Zero;
            try
            {
                hWndSIP = FindWindow("MS_SIPBUTTON", "MS_SIPBUTTON");
                addLog("enableSIPbutton: MS_SIPBUTTON hWnd found =" + string.Format("0x{0:X}", hWndSIP));
            }
            catch (Exception ex)
            {
                addLog("enableSIPbutton: Exception = " + ex.Message);
               
            }
            if (hWndSIP != IntPtr.Zero)
            {
                if (EnableWindow(hWndSIP, bEnable))
                    addLog("enableSIPbutton: EnableWindow OK.");
                else
                    addLog("enableSIPbutton: EnableWindow failed. LastError=" + Marshal.GetLastWin32Error().ToString());
            }
            else
                addLog("enableSIPbutton: failed to find window. LastError=" + Marshal.GetLastWin32Error().ToString());

        }
        private void showSIPWindow(bool bShow)
        {   
            IntPtr hWndSIP=IntPtr.Zero;
            try
            {
                hWndSIP = FindWindow("MS_SIPBUTTON", "MS_SIPBUTTON");
                addLog("showSIPWindow: MS_SIPBUTTON hWnd found =" + string.Format("0x{0:X}", hWndSIP));
            }
            catch (Exception ex)
            {
                addLog("showSIPWindow: Exception = " + ex.Message);
               
            }
            if (hWndSIP != IntPtr.Zero)
            {
                IntPtr hSipButton = GetWindow(hWndSIP, (uint)GetWindow_Cmd.GW_CHILD);
                if (hSipButton != IntPtr.Zero)
                {
                    addLog("showSIPWindow: MS_SIPBUTTON ChildWindow found =" + string.Format("0x{0:X}", hSipButton));
                    if (bShow)
                    {
                        if (ShowWindow(hSipButton, WindowShowStyle.ShowNormal))
                            addLog("showSIPWindow: show SIPbutton OK.");
                        else
                            addLog("showSIPWindow: failed to show SIPbutton. LastError=" + Marshal.GetLastWin32Error().ToString());
                    }
                    else
                    {
                        if (ShowWindow(hSipButton, WindowShowStyle.Hide))
                            addLog("showSIPWindow: hide SIPbutton OK.");
                        else
                            addLog("showSIPWindow: failed to hide SIPbutton. LastError=" + Marshal.GetLastWin32Error().ToString());
                    }
                }
                else
                    addLog("showSIPWindow: failed to find SIPbutton window. LastError=" + Marshal.GetLastWin32Error().ToString());
            }
            else
                addLog("showSIPWindow: failed to find window. LastError=" + Marshal.GetLastWin32Error().ToString());
        }
        private static void addLog(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

        [DllImport("ITC50.DLL", SetLastError=true)]
        private static extern void ITCSetKeyboardLight(bool bKeyLight);
        //alternatively use SSAPI
        /*
        <Subsystem Name="Device Settings">
         <Group Name="Backlight">
          <Field Name="Keypad backlight">4</Field> 
         </Group>
        </Subsystem>
        <!-- 4=AlwaysOn, 0=AlwaysOff, 2=based on environment -->
        */
        /// <summary>
        /// Switch the keyboard backlight on or off
        /// </summary>
        public bool KeyboardBacklightState
        {
            set
            {
                ITCSetKeyboardLight(value);
            }
        }
        /// <summary>
        /// play the good sound
        /// </summary>
        /// <param name="duration">time to play sound</param>
        public void GoodSound(int duration){
            Intermec.Device.Audio.Tone badTone = new Intermec.Device.Audio.Tone(1000, duration, Intermec.Device.Audio.Tone.VOLUME.VERY_LOUD);
            badTone.Play();
        }
        /// <summary>
        /// play the bad sound
        /// </summary>
        /// <param name="duration">time to play sound</param>
        public void BadSound(int duration)
        {
            Intermec.Device.Audio.Tone goodTone = new Intermec.Device.Audio.Tone(500, duration, Intermec.Device.Audio.Tone.VOLUME.VERY_LOUD);
            goodTone.Play();
        }

        [DllImport("aygshell.dll", SetLastError = true)]
        internal static extern int Vibrate(int cvn, IntPtr rgvn, bool fRepeat, uint dwTimeout);
        [DllImport("aygshell.dll", SetLastError = true)]
        internal static extern int VibrateStop();
        /*
        HRESULT Vibrate(
          DWORD cvn,
          const VIBRATENOTE * rgvn,
          BOOL fRepeat,
          DWORD dwTimeout
        );
         * DOES NOT WORK ON CN70
        */
        /// <summary>
        /// let the device vibrate
        /// </summary>
        /// <param name="duration">time for the vibration</param>
        public void Vibration(int duration)
        {
            System.Threading.Thread myThread = new System.Threading.Thread(vibratePlayThread);
            _vibration_duration = duration;
            myThread.Start();
        }
        private int _vibration_duration;
        private const UInt32 INFINITE = 0xffffffff;
        private void vibratePlayThread()
        {
            LED.SetLedStatus(5, LED.LedFlags.STATE_ON);
            
            //Vibrate(0, IntPtr.Zero, true, INFINITE);
            System.Threading.Thread.Sleep(_vibration_duration);
            //VibrateStop();
            LED.SetLedStatus(5, LED.LedFlags.STATE_OFF);
        }
        internal class LED
        {
            [DllImport("CoreDll")]
            public extern static Int32 GetLastError();
            /// <summary>
            /// LED manipulation
            /// 
            /// Normal use:
            ///     First, get the count of LED's by calling GetLedCount()
            ///     Second, if the number of LED's is greater than zero, set the state of each desired LED
            ///         by calling SetLedStatus(led #, flag) where the first LED is zero and the flag is defined
            ///         by one of the LedFlags enumerations
            /// </summary>

            [Flags]
            public enum LedFlags : int
            {
                STATE_OFF = 0x0000,  /* dark LED */
                STATE_ON = 0x0001,  /* light LED */
                STATE_BLINK = 0x0002,  /* flashing LED */
            }

            public const Int32 NLED_COUNT_INFO_ID = 0;
            public const Int32 NLED_SUPPORTS_INFO_ID = 1;
            public const Int32 NLED_SETTINGS_INFO_ID = 2;

            public class NLED_COUNT_INFO
            {
                public UInt32 cLeds;
            }

            public class NLED_SUPPORTS_INFO
            {
                public UInt32 Lednum;
                public Int32 lCycleAdjust;
                public bool fAdjustTotalCycleTime;
                public bool fAdjustOnTime;
                public bool fAdjustOffTime;
                public bool fMetaCycleOn;
                public bool fMetaCycleOff;
            };

            public class NLED_SETTINGS_INFO
            {
                public UInt32 LedNum;
                public LedFlags OnOffBlink;
                public Int32 TotalCycleTime;
                public Int32 OnTime;
                public Int32 OffTime;
                public Int32 MetaCycleOn;
                public Int32 MetaCycleoff;
            };

            [DllImport("CoreDll")]
            private extern static bool NLedGetDeviceInfo(Int32 nID, NLED_COUNT_INFO nci);

            [DllImport("CoreDll")]
            private extern static bool NLedGetDeviceInfo(Int32 nID, NLED_SUPPORTS_INFO nsi);

            [DllImport("CoreDll")]
            private extern static bool NLedGetDeviceInfo(Int32 nID, NLED_SETTINGS_INFO nsi);

            [DllImport("CoreDll")]
            private extern static bool NLedSetDevice(Int32 nID, NLED_SETTINGS_INFO nsi);

            public static uint GetLedCount()
            {
                NLED_COUNT_INFO nci = new NLED_COUNT_INFO();

                uint LedCount = 0;

                if (NLedGetDeviceInfo(NLED_COUNT_INFO_ID, nci))
                    LedCount = nci.cLeds;

                return LedCount;
            }

            public static bool SetLedStatus(uint nLed, LedFlags fState)
            {
                NLED_SETTINGS_INFO nsi = new NLED_SETTINGS_INFO();

                nsi.LedNum = nLed;
                nsi.OnOffBlink = fState;

                return NLedSetDevice(NLED_SETTINGS_INFO_ID, nsi);
            }
        }
    }
}
