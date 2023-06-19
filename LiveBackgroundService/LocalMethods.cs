namespace LiveBackgroundService;

using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using static WindowsNativeApi;

public class LocalMethods
{
    private static List<string> _games = new();
    private static readonly List<string> _excludeTitle = new() { "steam" };

    public LocalMethods()
    {
        //https://www.nvidia.cn/geforce/geforce-experience/games/ see browser console :D hhh
        var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "support_games.json"), Encoding.UTF8);
        _games = JsonConvert.DeserializeObject<List<string>>(text)!;
    }


    /// <summary>
    /// boolean:intptr
    /// </summary>
    /// <returns></returns>
    public string DetectGameRunning()
    {
        var desktop = GetDesktopWindow();
        var shell = GetShellWindow();
        GetWindowRect(desktop, out var desktopRect);

        while (true)
        {
            //10秒间隔
            Thread.Sleep(1000);

            try
            {
                var foreground = GetForegroundWindow();
                if (foreground == desktop || foreground == shell)
                {
                    continue;
                }

                GetWindowRect(foreground, out var foregroundRect);

                if (foregroundRect.left <= desktopRect.left
                 && foregroundRect.top <= desktopRect.top
                 && foregroundRect.right >= desktopRect.right
                 && foregroundRect.bottom >= desktopRect.bottom)
                {
                    var title = GetWindowText(foreground);

                    if (string.IsNullOrEmpty(title) || _excludeTitle.Contains(title, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (!_games.Contains(title, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var (fileName, executablePath, imageBase64) = GetProgramExecutablePathAndIcon(foreground);
                    return $"true|{foreground.ToInt32()}|{title}|{fileName}|{executablePath}|{imageBase64}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }


    public static (string, string, string) GetProgramExecutablePathAndIcon(IntPtr ptr)
    {
        GetWindowThreadProcessId(ptr, out var pid);
        Process proc = Process.GetProcessById(pid);

        if (proc == null)
        {
            return (string.Empty, string.Empty, string.Empty);
        }

        var filePath = proc.MainModule!.FileName;
        if (!File.Exists(filePath))
        {
            return (string.Empty, string.Empty, string.Empty);
        }

        var fileName = Path.GetFileNameWithoutExtension(filePath).Replace("_", " ").Replace("-", " ");

        var icon = Icon.ExtractAssociatedIcon(filePath);
        var bitmap = icon.ToBitmap();

        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Bmp);
        var bytes = ms.ToArray();

        var base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
        string imageBase64 = "data:image/bmp;base64," + base64;

        return (fileName, filePath, imageBase64);
    }
}