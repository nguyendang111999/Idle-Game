using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class WindowManager : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint SWP_SHOWWINDOW = 0x0040;

    private const int GWL_STYLE = -16;
    private const int WS_CAPTION = 0x00C00000;

    private const string TASKBAR_CLASS_NAME = "Shell_TrayWnd";
    private const string TASKBAR_WINDOW_NAME = "System Notification Area";

    void Start()
    {
        IntPtr windowHandle = FindWindow(null, Application.productName);
        if (windowHandle != IntPtr.Zero)
        {
            SetWindowStyles(windowHandle);
            SetWindowPosition(windowHandle);
        }
        else
        {
            Debug.LogError("Window not found.");
        }
    }

    void SetWindowStyles(IntPtr hWnd)
    {
        int style = GetWindowLong(hWnd, GWL_STYLE);
        style &= ~WS_CAPTION; // Remove caption bar
        SetWindowLong(hWnd, GWL_STYLE, style);
    }

    void SetWindowPosition(IntPtr hWnd)
    {
        int screenWidth = Screen.currentResolution.width;
        int screenHeight = Screen.currentResolution.height;
        int taskbarHeight = GetTaskbarHeight();

        // Calculate window position at the bottom of the screen above the taskbar
        int windowWidth = screenWidth;
        int windowHeight = (screenHeight - taskbarHeight) / 2;
        int windowX = 0;
        int windowY = screenHeight - windowHeight - taskbarHeight;

        SetWindowPos(hWnd, IntPtr.Zero, windowX, windowY, windowWidth, windowHeight, SWP_NOZORDER | SWP_FRAMECHANGED | SWP_SHOWWINDOW);
    }

    int GetTaskbarHeight()
    {
        IntPtr taskbarHandle = FindWindow(TASKBAR_CLASS_NAME, TASKBAR_WINDOW_NAME);
        RECT rect;
        GetWindowRect(taskbarHandle, out rect);
        return rect.bottom - rect.top;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
