using System;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace Hasci.TestApp
{
    public static class shFullScreen
    {
        public const int SHFS_SHOWTASKBAR = 1;
        public const int SHFS_HIDETASKBAR = 2;
        public const int SHFS_SHOWSIPBUTTON = 4;
        public const int SHFS_HIDESIPBUTTON = 8;
        public const int SHFS_SHOWSTARTICON = 16;
        public const int SHFS_HIDESTARTICON = 32;

        [DllImport("AygShell.dll")]
        public static extern int SHFullScreen(IntPtr hwndRequester, UInt32 dwState);

        public static bool hideStartButton(Form f)
        {
            bool bRet = false;
            int iRet = SHFullScreen(f.Handle, SHFS_HIDETASKBAR | SHFS_HIDESTARTICON);
            if (iRet == 0)
                bRet = true;
            else
                bRet = false;
            return bRet;
        }
    }
}