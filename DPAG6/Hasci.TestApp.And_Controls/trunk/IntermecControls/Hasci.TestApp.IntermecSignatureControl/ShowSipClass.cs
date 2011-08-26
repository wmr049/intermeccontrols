using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace shFullScreen
{
    static class ShowSipClass
    {
        [DllImport("AYGShell.dll", SetLastError = true)]
        static extern Int32 SHFullScreen(IntPtr hwndRequester, shStates dwState);

        public enum shStates:uint
        {
            SHFS_SHOWTASKBAR = 0x0001,
            SHFS_HIDETASKBAR = 0x0002,
            SHFS_SHOWSIPBUTTON = 0x0004,
            SHFS_HIDESIPBUTTON = 0x0008,
            SHFS_SHOWSTARTICON = 0x0010,
            SHFS_HIDESTARTICON = 0x0020
        }

        [DllImport("coredll.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);
        [DllImport("coredll.dll", SetLastError = true)]
        static extern IntPtr GetWindowText(IntPtr hWnd, string sText);

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

        public static IntPtr getOwner(IntPtr hWnd)
        {
            IntPtr iPtr = IntPtr.Zero;
            iPtr = GetWindow(hWnd, GetWindow_Cmd.GW_OWNER);
            if (iPtr == IntPtr.Zero) //error 0x06 = ERROR_INVALID_HANDLE
                addLog("getOwner failed for hwnd=" +string.Format("0x{0}",hWnd)  + " with error=" + Marshal.GetLastWin32Error().ToString());

            return iPtr;
        }
        public static int showSip(bool bShow, IntPtr hWndRequester){
            int iRes =0;
            if(bShow)
                iRes = SHFullScreen(hWndRequester, shStates.SHFS_SHOWSIPBUTTON);
            else
                iRes = SHFullScreen(hWndRequester, shStates.SHFS_HIDESIPBUTTON);
            return iRes;
        }
        private static void addLog(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }
        [DllImport("coredll.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("coredll.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("coredll.dll", SetLastError = true)]
        static extern IntPtr GetParent(IntPtr hWnd);
/*
        private void hideSIPbutton(bool bHide)
        {
            try
            {
                IntPtr hParent = shFullScreen.ShowSipClass.getOwner(this.Handle);
                if (hParent != IntPtr.Zero)
                {
                    int iRes = shFullScreen.ShowSipClass.showSip(false, hParent);
                }

                this.HandleCreated += new EventHandler(IntermecSignatureControl_HandleCreated);
                addLog("Found myself with handle " + string.Format("0x{0:X}", this.Handle));
                Control cParent = Parent;
                addLog("Found parent control with handle " + string.Format("0x{0:X}", cParent.Handle));
                Form frm = (Form)this.Parent;
                addLog("Found parent with handle " + frm.Handle.ToString() + ", text = '" + frm.Text + "'");
            }
            catch (Exception ex)
            {
                addLog("Getting parent as form failed!" + ex.Message);
            }
        }

        void IntermecSignatureControl_HandleCreated(object sender, EventArgs e)
        {
            addLog("SignatureControl: handle created = " + this.Handle.ToString());
        }
*/
    }
}
