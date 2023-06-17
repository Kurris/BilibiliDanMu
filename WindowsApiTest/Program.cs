using System.Buffers.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PInvoke;
using static PInvoke.User32;


//var shellWindow = User32.GetShellWindow();
//var desktop = User32.GetDesktopWindow();

//User32.GetWindowRect(desktop, out var desktopRect);

////User32.GetWindowThreadProcessId(desktop, out var threadProcessId);

//var pWnd = User32.GetWindow(desktop, User32.GetWindowCommands.GW_CHILD);
//while (pWnd != IntPtr.Zero)
//{
//    if (User32.IsWindow(pWnd) && User32.IsWindowVisible(pWnd) )
//    {
//        Console.WriteLine(User32.GetWindowText(pWnd));
//        User32.GetWindowThreadProcessId(pWnd, out var  _);

//        uint arraySize = 256;
//        IntPtr[] iphModules= new IntPtr[arraySize];
//        uint bytesCopied = 0;
//        uint arrayBytesSize = arraySize * (uint)IntPtr.Size;

//        if (Psapi.EnumProcessModulesEx(pWnd, iphModules, (int)arrayBytesSize, out var ipcbNeeded, Psapi.EnumProcessModulesFlags.LIST_MODULES_ALL) &&
//             arrayBytesSize == bytesCopied && arraySize <= uint.MaxValue - 128)
//        {
//            arraySize += 128;
//            iphModules = new IntPtr[arraySize];
//            arrayBytesSize = arraySize * (uint)IntPtr.Size;
//        }
//    }

//    pWnd = User32.GetWindow(pWnd, User32.GetWindowCommands.GW_HWNDNEXT);
//}

//var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "support_games.json"), Encoding.UTF8);
//var tokens = JToken.Parse(text);
//var titles = tokens.ToList().Select(x => x["title"].Value<string>());
//var json = JsonConvert.SerializeObject(titles);
//var domString = await new HttpClient().GetStringAsync("https://www.nvidia.cn/geforce/geforce-experience/games/");



//while (true)
//{
//    var foregroundWindow = User32.GetForegroundWindow();
//    if (foregroundWindow != desktop && foregroundWindow != shellWindow)
//    {
//        User32.GetWindowRect(foregroundWindow, out var foregroundRect);
//        if (foregroundRect.left <= desktopRect.left
//         && foregroundRect.top <= desktopRect.top
//         && foregroundRect.right >= desktopRect.right
//         && foregroundRect.bottom >= desktopRect.bottom)
//        {
//            SHQueryUserNotificationState(out var state);
//            Console.WriteLine(User32.GetWindowText(foregroundWindow) + " is fullscreen, and state is" + state.ToString());
//        }
//    }

//    Thread.Sleep(1000);
//}


var foreground = User32.GetForegroundWindow();
var text = User32.GetWindowText(foreground);

while (!text.Equals("Dota 2"))
{
    Console.WriteLine(text);
    foreground = User32.GetForegroundWindow();
    text = User32.GetWindowText(foreground);
    Thread.Sleep(1000);
}


[DllImport(nameof(User32), SetLastError = true)]
static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

//[DllImport("psapi.dll")]
//static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);


GetWindowThreadProcessId(foreground, out var pid);
Process proc = Process.GetProcessById(pid);

var fileName = proc.MainModule.FileName;

var icon = Icon.ExtractAssociatedIcon(fileName);
var bitmap = icon.ToBitmap();


using (var ms = new MemoryStream())
{
    bitmap.Save(ms, ImageFormat.Bmp);
    var bytes = ms.ToArray();

    var base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
    string imageString = "data:image/bmp;base64," + base64;

    Console.WriteLine(imageString);
}


Console.ReadKey();