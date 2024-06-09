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
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")]
    static extern IntPtr SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte alpha, uint dwFlags);
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
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    static readonly IntPtr HWND_TOP = new IntPtr(0);
    const uint LWA_COLORKEY = 0x00000001;

    IntPtr hWnd;

    private void Start()
    {
#if !UNITY_EDITOR
        hWnd = GetActiveWindow();
        // Make window transparent
        MARGINS margins = new MARGINS { left = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);
        // Interact with what's behind this window
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        // Display this window above others
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif
    }

    [SerializeField] LayerMask _interactLayer;

    private void Update()
    {
        // SetClickThrough(Physics2D.OverlapPoint());
        if (Input.GetMouseButtonDown(0))
        {
            var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                            Vector2.zero, 100, _interactLayer);

            SetClickThrough(hits.Length > 0);
        }
    }

    public void SetClickThrough(bool value)
    {
        if (value)
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);

            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
        }
        else
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);

            SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, 0);
        }
    }
}
