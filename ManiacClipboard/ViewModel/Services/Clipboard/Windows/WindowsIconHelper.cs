using System;

namespace ManiacClipboard.ViewModel
{
    /// <summary>
    /// Provides functions to get icons from diffrent processes.
    /// </summary>
    public static class WindowsIconHelper
    {
        public static int GetIconIndex(string pszFile)
        {
            SHFILEINFO sfi = new SHFILEINFO();
            Shell32.SHGetFileInfo(pszFile
                , 0
                , ref sfi
                , (uint)System.Runtime.InteropServices.Marshal.SizeOf(sfi)
                , (uint)(SHGFI.SysIconIndex | SHGFI.LargeIcon | SHGFI.UseFileAttributes));
            return sfi.iIcon;
        }

        // 256*256
        public static IntPtr GetJumboIcon(int iImage)
        {
            IImageList spiml = null;
            Guid guil = new Guid(Shell32.IID_IImageList2);//or IID_IImageList

            Shell32.SHGetImageList(Shell32.SHIL_JUMBO, ref guil, ref spiml);
            IntPtr hIcon = IntPtr.Zero;
            spiml.GetIcon(iImage, Shell32.ILD_TRANSPARENT | Shell32.ILD_IMAGE, ref hIcon); //

            return hIcon;
        }
    }
}