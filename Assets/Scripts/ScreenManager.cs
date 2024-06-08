using UnityEngine;
using System.Runtime.InteropServices;

public class ScreenManager : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(System.IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
    private const uint SWP_NOZORDER = 0x0004;
    private const int HWND_TOPMOST = -1;

    void Start()
    {
        SetHalfHeightFullWidthWindow();
        SetWindowAlwaysOnTop();
    }

    void SetHalfHeightFullWidthWindow()
    {
        int screenWidth = Screen.currentResolution.width;
        int screenHeight = Screen.currentResolution.height / 2;
        Screen.SetResolution(screenWidth, screenHeight, false);
        PositionWindowAtTop();
    }

    void PositionWindowAtTop()
    {
        System.IntPtr hWnd = GetUnityWindowHandle();
        SetWindowPos(hWnd, 0, 0, 0, Screen.currentResolution.width, Screen.currentResolution.height / 2, SWP_NOZORDER);
    }

    void SetWindowAlwaysOnTop()
    {
        System.IntPtr hWnd = GetUnityWindowHandle();
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, Screen.currentResolution.width, Screen.currentResolution.height / 2, SWP_NOZORDER);
    }

    System.IntPtr GetUnityWindowHandle()
    {
        #if UNITY_STANDALONE_WIN
            var windowHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            return windowHandle;
        #else
            return System.IntPtr.Zero;
        #endif
    }
}
