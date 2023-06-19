using System.Runtime.InteropServices;
using LiveBackgroundService.Structs;

namespace LiveBackgroundService;


public delegate bool CallBackPtr(int hwnd, int lParam);

/// <summary>
///  PInvoke 查阅 https://github.com/dotnet/pinvoke
/// </summary>
internal static class WindowsNativeApi
{

    /// <summary>
    /// Retrieves a handle to the foreground window (the window with which the user is currently
    /// working). The system assigns a slightly higher priority to the thread that creates the
    /// foreground window than it does to other threads.
    /// <para>
    /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms633505%28v=vs.85%29.aspx
    /// for more information.
    /// </para>
    /// </summary>
    /// <returns>
    /// C++ ( Type: Type: HWND ) <br/> The return value is a handle to the foreground window. The
    /// foreground window can be NULL in certain circumstances, such as when a window is losing activation.
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetForegroundWindow();


    /// <summary>
    /// Retrieves a handle to the Shell's desktop window.
    /// </summary>
    /// <returns>The return value is the handle of the Shell's desktop window. If no Shell process is present, the return value is NULL.</returns>
    [DllImport("user32.dll")]
    public static extern IntPtr GetShellWindow();


    /// <summary>
    /// Retrieves a handle to the desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which other windows are painted.
    /// </summary>
    /// <returns>The return value is a handle to the desktop window.</returns>
    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();



    /// <summary>
    /// Retrieves the length, in characters, of the specified window's title bar text (if the window has a title bar).
    /// If the specified window is a control, the function retrieves the length of the text within the control. However,
    /// GetWindowTextLength cannot retrieve the length of the text of an edit control in another application.
    /// </summary>
    /// <param name="hWnd">A handle to the window or control.</param>
    /// <returns>
    /// If the function succeeds, the return value is the length, in characters, of the text. Under certain
    /// conditions, this value may actually be greater than the length of the text. For more information, see the following
    /// Remarks section.
    /// <para>If the window has no text, the return value is zero. To get extended error information, call GetLastError.</para>
    /// </returns>
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int GetWindowTextLength(IntPtr hWnd);


    /// <summary>
    /// Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is
    /// a control, the text of the control is copied. However, GetWindowText cannot retrieve the text of a control in another
    /// application.
    /// </summary>
    /// <param name="hWnd">A handle to the window or control containing the text.</param>
    /// <param name="lpString">
    /// The buffer that will receive the text. If the string is as long or longer than the buffer, the
    /// string is truncated and terminated with a null character.
    /// </param>
    /// <param name="nMaxCount">
    /// The maximum number of characters to copy to the buffer, including the null character. If the
    /// text exceeds this limit, it is truncated.
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is the length, in characters, of the copied string, not including
    /// the terminating null character. If the window has no title bar or text, if the title bar is empty, or if the window or
    /// control handle is invalid, the return value is zero. To get extended error information, call GetLastError.
    /// <para>This function cannot retrieve the text of an edit control in another application.</para>
    /// </returns>
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern unsafe int GetWindowText(
           IntPtr hWnd,
           [Friendly(FriendlyFlags.Array | FriendlyFlags.Out, ArrayLengthParameter = 2)] char* lpString,
           int nMaxCount);


    public static unsafe int GetWindowText(IntPtr hWnd, char[] lpString, int nMaxCount)
    {
        fixed (char* lpStringLocal = lpString)
        {
            int result = GetWindowText(hWnd, lpStringLocal, nMaxCount);
            return result;
        }
    }

    /// <summary>
    /// Get the text of the specified window's title bar (if it has one). If the specified window is a control, the
    /// text of the control is returned. However, GetWindowText cannot retrieve the text of a control in another application.
    /// </summary>
    /// <param name="hWnd">A handle to the window or control containing the text.</param>
    /// <returns>
    /// The text of the specified window's title bar. If the specified window is a control, the text of the control is
    /// returned.
    /// </returns>
    public static string GetWindowText(IntPtr hWnd)
    {
        int maxLength = GetWindowTextLength(hWnd);
        if (maxLength == 0)
        {
            Win32ErrorCode lastError = GetLastError();
            if (lastError != Win32ErrorCode.ERROR_SUCCESS)
            {
                throw new Win32Exception(lastError);
            }

            return string.Empty;
        }

        char[] text = new char[maxLength + 1];
        int finalLength = GetWindowText(hWnd, text, maxLength + 1);
        if (finalLength == 0)
        {
            Win32ErrorCode lastError = GetLastError();
            if (lastError != Win32ErrorCode.ERROR_SUCCESS)
            {
                throw new Win32Exception(lastError);
            }

            return string.Empty;
        }

        return new string(text, 0, finalLength);
    }


    public static Win32ErrorCode GetLastError()
    {
        return (Win32ErrorCode)Marshal.GetLastWin32Error();
    }


    [DllImport("shell32.dll")]
    public static extern int SHQueryUserNotificationState(out UserNotificationState userNotificationState);


    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, out Structs.RECT lpRect);


    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


    [DllImport("user32.dll")]
    public static extern int EnumWindows(CallBackPtr callPtr, int lPar);


    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindow(IntPtr hWnd);


    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetActiveWindow();


    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

    public static IntPtr FindWindow(string title)
    {
        return FindWindow(null, title);
    }
}