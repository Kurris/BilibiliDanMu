namespace LiveBackgroundService;

using LiveBackgroundService.Enums;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using static WindowsNativeApi;

public class LocalMethods
{
    //"steam"
    private static List<string> _games = new();
    private static readonly List<string> _excludeTitle = new() { };

    private bool _gamePrgramExists = false;
    private IntPtr _gameHandle = IntPtr.Zero;

    private bool _musicePrgramExists = false;
    private string _musiceTitle = string.Empty;

    static LocalMethods()
    {
        //https://www.nvidia.cn/geforce/geforce-experience/games/ see browser console output :D 
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
        var foreground = GetForegroundWindow();

        try
        {
            var window = GetWindow(desktop, GetWindowCommands.GW_CHILD);
            var title = string.Empty;
            var windows = new List<IntPtr>();

            while (window != IntPtr.Zero)
            {
                if (window == _gameHandle && window == foreground)
                {
                    //正在玩的游戏
                    return string.Empty;
                }

                if (IsWindow(window) && IsWindowVisible(window))
                {
                    //不考虑子窗口问题, 游戏一般不会有
                    title = GetWindowText(window);

                    if (!string.IsNullOrEmpty(title)
                        && !_excludeTitle.Contains(title, StringComparer.OrdinalIgnoreCase)
                        && _games.Contains(title, StringComparer.OrdinalIgnoreCase))
                    {
                        windows.Add(window);
                    }
                }

                window = GetNextWindow(window, GetNextWindowCommands.GW_HWNDNEXT);
            }

            //存在游戏窗口
            if (windows.Any())
            {
                _gamePrgramExists = true;
                //默认取第一个
                window = windows.First();

                //多个游戏运行时,优先取前景
                if (windows.Count > 1)
                {

                    //GetWindowRect(foreground, out var foregroundRect);
                    //GetWindowRect(desktop, out var desktopRect
                    var mybeForeground = windows.FirstOrDefault(x =>
                    {
                        //检测全屏
                        //if (foregroundRect.left <= desktopRect.left
                        // && foregroundRect.top <= desktopRect.top
                        // && foregroundRect.right >= desktopRect.right
                        // && foregroundRect.bottom >= desktopRect.bottom)
                        //{

                        //}

                        return x == foreground;
                    });

                    //当没有前景时候,mybeForeground == IntPtr.Zero
                    if (mybeForeground != IntPtr.Zero)
                        window = mybeForeground;
                }
            }

            //还是之前的游戏窗口,则输出空
            if (_gameHandle == window)
                return string.Empty;

            //记录当前句柄
            _gameHandle = window;

            //如果找不到游戏窗口
            if (window == IntPtr.Zero)
            {
                //需要清空之前的数据
                if (_gamePrgramExists)
                {
                    _gamePrgramExists = false;
                    return JsonConvert.SerializeObject(new
                    {
                        method = nameof(DetectGameRunning),
                        handle = window.ToInt32(),
                        title = string.Empty,
                        name = string.Empty,
                        path = string.Empty,
                        image = string.Empty
                    });
                }

                //直接返回空,不处理
                return string.Empty;
            }


            var (fileName, executablePath, imageBase64) = GetProgramExecutablePathAndIcon(window);
            title = GetWindowText(window);
            return JsonConvert.SerializeObject(new
            {
                method = nameof(DetectGameRunning),
                handle = window.ToInt32(),
                title,
                name = fileName,
                path = executablePath,
                image = imageBase64
            });
        }
        catch (Exception ex)
        {
            //可被拒绝访问
            Console.WriteLine(ex.Message);
        }

        return string.Empty;
    }


    public static (string, string, string) GetProgramExecutablePathAndIcon(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
        {
            return (string.Empty, string.Empty, string.Empty);
        }
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


    public string GetMusicInfo()
    {
        //简单暴力
        var musicProcess = Process.GetProcesses().Where(x => x.ProcessName.Contains("music", StringComparison.OrdinalIgnoreCase));
        var program = musicProcess.FirstOrDefault(x => IsWindow(x.MainWindowHandle));

        if (program == null)
        {
            if (_musicePrgramExists)
            {
                _musicePrgramExists = false;
                _musiceTitle = string.Empty;

                return JsonConvert.SerializeObject(new
                {
                    method = nameof(GetMusicInfo),
                    title = string.Empty
                });
            }

            return string.Empty;
        }

        var title = program.MainWindowTitle;
        if (!string.IsNullOrEmpty(title) && !_musiceTitle.Equals(title))
        {
            _musiceTitle = title;
            _musicePrgramExists = true;

            return JsonConvert.SerializeObject(new
            {
                method = nameof(GetMusicInfo),
                handle = program.MainWindowHandle.ToInt32(),
                title,
            });
        }
        return string.Empty;
    }

    public string GameIsForeground()
    {
        try
        {
            var foreground = GetForegroundWindow();
            if (_gameHandle == foreground)
            {
                return JsonConvert.SerializeObject(new
                {
                    method = nameof(GameIsForeground),
                    isForeground = true
                });
            }

            return JsonConvert.SerializeObject(new
            {
                method = nameof(GameIsForeground),
                isForeground = false
            });
        }
        catch
        {
            return string.Empty;
        }
    }
}