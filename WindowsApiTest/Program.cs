// See https://aka.ms/new-console-template for more information



using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Xml;
using PInvoke;
using static PInvoke.User32;


var shellWindow = User32.GetShellWindow();
var desktop = User32.GetDesktopWindow();

User32.GetWindowRect(desktop, out var desktopRect);

//User32.GetWindowThreadProcessId(desktop, out var threadProcessId);

var pWnd = User32.GetWindow(desktop, User32.GetWindowCommands.GW_CHILD);
while (pWnd != IntPtr.Zero)
{
    if (User32.IsWindow(pWnd) && User32.IsWindowVisible(pWnd) )
    {
        Console.WriteLine(User32.GetWindowText(pWnd));
        User32.GetWindowThreadProcessId(pWnd, out var  _);

        uint arraySize = 256;
        IntPtr[] iphModules= new IntPtr[arraySize];
        uint bytesCopied = 0;
        uint arrayBytesSize = arraySize * (uint)IntPtr.Size;

        if (Psapi.EnumProcessModulesEx(pWnd, iphModules, (int)arrayBytesSize, out var ipcbNeeded, Psapi.EnumProcessModulesFlags.LIST_MODULES_ALL) &&
             arrayBytesSize == bytesCopied && arraySize <= uint.MaxValue - 128)
        {
            arraySize += 128;
            iphModules = new IntPtr[arraySize];
            arrayBytesSize = arraySize * (uint)IntPtr.Size;
        }
    }

    pWnd = User32.GetWindow(pWnd, User32.GetWindowCommands.GW_HWNDNEXT);
}

//var dom = await  new HttpClient().GetStringAsync("https://www.nvidia.cn/geforce/geforce-experience/games/");



[DllImport("shell32.dll")]
static extern int SHQueryUserNotificationState(
     out QUERY_USER_NOTIFICATION_STATE pquns);

while (true)
{
    var foregroundWindow = User32.GetForegroundWindow();
    if (foregroundWindow != desktop && foregroundWindow != shellWindow)
    {
        User32.GetWindowRect(foregroundWindow, out var foregroundRect);
        if (foregroundRect.left <= desktopRect.left
         && foregroundRect.top <= desktopRect.top
         && foregroundRect.right >= desktopRect.right
         && foregroundRect.bottom >= desktopRect.bottom)
        {
            SHQueryUserNotificationState(out var state);
            Console.WriteLine(User32.GetWindowText(foregroundWindow) + " is fullscreen, and state is" + state.ToString());
        }
    }

    Thread.Sleep(1000);
}


//var ptr = User32.FindWindow(null, "Hollow Knight");


//var dotaPtr = new IntPtr(459710);

//Console.WriteLine(User32.GetWindowText(dotaPtr));


Console.WriteLine(User32.GetWindowText(User32.GetDesktopWindow()));


//IntPtr ptr = IntPtr.Zero;
//while (ptr == IntPtr.Zero)
//{
//    Thread.Sleep(500);
//    ptr = User32.GetForegroundWindow();
//    if (ptr == IntPtr.Zero)
//    {
//        Console.WriteLine("ZERO");
//        continue;
//    }
//    //User32.id
//    var text = User32.GetWindowText(ptr);
//    //Console.WriteLine(text);
//    ptr = IntPtr.Zero;
//}


class QueryUserNotificationState
{

    public enum UserNotificationState
    {
        /// <summary>
        /// A screen saver is displayed, the machine is locked,
        /// or a nonactive Fast User Switching session is in progress.
        /// </summary>
        NotPresent = 1,

        /// <summary>
        /// A full-screen application is running or Presentation Settings are applied.
        /// Presentation Settings allow a user to put their machine into a state fit
        /// for an uninterrupted presentation, such as a set of PowerPoint slides, with a single click.
        /// </summary>
        Busy = 2,

        /// <summary>
        /// A full-screen (exclusive mode) Direct3D application is running.
        /// </summary>
        RunningDirect3dFullScreen = 3,

        /// <summary>
        /// The user has activated Windows presentation settings to block notifications and pop-up messages.
        /// </summary>
        PresentationMode = 4,

        /// <summary>
        /// None of the other states are found, notifications can be freely sent.
        /// </summary>
        AcceptsNotifications = 5,

        /// <summary>
        /// Introduced in Windows 7. The current user is in "quiet time", which is the first hour after
        /// a new user logs into his or her account for the first time. During this time, most notifications
        /// should not be sent or shown. This lets a user become accustomed to a new computer system
        /// without those distractions.
        /// Quiet time also occurs for each user after an operating system upgrade or clean installation.
        /// </summary>
        QuietTime = 6
    }


    [DllImport("shell32.dll")]
    static extern int SHQueryUserNotificationState(out UserNotificationState userNotificationState);

    public static UserNotificationState State()
    {
        UserNotificationState state;
        var returnVal = SHQueryUserNotificationState(out state);

        return state;
    }
}


enum QUERY_USER_NOTIFICATION_STATE
{
    QUNS_NOT_PRESENT = 1,
    QUNS_BUSY = 2,
    QUNS_RUNNING_D3D_FULL_SCREEN = 3,
    QUNS_PRESENTATION_MODE = 4,
    QUNS_ACCEPTS_NOTIFICATIONS = 5,
    QUNS_QUIET_TIME = 6
};

