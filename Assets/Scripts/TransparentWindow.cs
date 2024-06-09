using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    private struct MARGINS
    {
        public int left;
        public int right;
        public int top;
        public int bottom;
    }

    [DllImport("Dwmapi.dll")]
    private static extern IntPtr DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    const int GWL_EXSTYLE = -20;
    const int WS_EX_LAYERED = 0x00080000;
    const int WS_EX_TRANSPARENT = 0x00000020;

    private void Start()
    {
#if !UNITY_EDITOR
        IntPtr hWnd = GetActiveWindow();
        MARGINS margins = new MARGINS { left = -1 };

        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
#endif
    }
}
