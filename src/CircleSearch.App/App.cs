using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using CircleSearch.Domain;
using CircleSearch.Infrastructure;
using CircleSearch.Services;
using CircleSearch.UI;
using CircleSearch.UI.Windows;

namespace CircleSearch;

public sealed class App : System.Windows.Application
{
    private WinKeyHookService? _hookService;
    private System.Windows.Forms.NotifyIcon? _trayIcon;
    private OverlayWindow? _activeOverlay;
    private SettingsWindow? _activeSettings;
    private Mutex? _singleInstanceMutex;

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetProcessDpiAwarenessContext(IntPtr value);

    private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new(-4);

    [STAThread]
    public static void Main()
    {
        try
        {
            SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
            AppLogger.Info("Main() invoked");
            var app = new App
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };
            app.DispatcherUnhandledException += (_, args) =>
            {
                AppLogger.Error("DispatcherUnhandledException", args.Exception);
                args.Handled = true;
            };
            app.Run();
            AppLogger.Info("app.Run() exited normally");
        }
        catch (Exception ex)
        {
            AppLogger.Error("Fatal exception in Main()", ex);
        }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        AppLogger.Info("OnStartup entered", new { Desktop = ScreenCaptureService.GetCurrentDesktopInfo() });

        _singleInstanceMutex = new Mutex(true, "Local\\CircleSearch_SingleInstance", out bool createdNew);
        if (!createdNew)
        {
            AppLogger.Warning("Another instance is already running, shutting down.");
            Shutdown();
            return;
        }

        try
        {
            ConfigManager.Load();
            var configuredLang = ConfigManager.Current.UiLanguage;
            if (string.IsNullOrWhiteSpace(configuredLang) || configuredLang.Equals("auto", StringComparison.OrdinalIgnoreCase))
            {
                LocalizationService.CurrentLanguage = LocalizationService.DetectSystemLanguage();
            }
            else
            {
                LocalizationService.CurrentLanguage = configuredLang;
            }

            ThemeManager.ApplyTheme(ConfigManager.Current.Theme);

            InitializeTrayIcon();

            _hookService = new WinKeyHookService();
            _hookService.OverlayTriggered += ToggleOverlay;
            _hookService.Start();

            LocalizationService.LanguageChanged += RebuildTrayMenu;

            AppLogger.Info("CircleSearch application started ready in tray");
        }
        catch (Exception ex)
        {
            AppLogger.Error("Exception during OnStartup initialization", ex);
        }
    }

    private void InitializeTrayIcon()
    {
        _trayIcon = new System.Windows.Forms.NotifyIcon
        {
            Icon = CreateRoyalBlueAppIcon(),
            Text = LocalizationService.T("app.title"),
            Visible = true
        };

        _trayIcon.DoubleClick += (_, _) => ToggleOverlay();
        RebuildTrayMenu();

        try
        {
            var title = LocalizationService.T("tray.ready_title");
            var tip = LocalizationService.T("tray.ready_tip");
            _trayIcon.ShowBalloonTip(3000, title, tip, System.Windows.Forms.ToolTipIcon.Info);
        }
        catch
        {
            // Ignore if notification center is unavailable
        }
    }

    private void RebuildTrayMenu()
    {
        if (_trayIcon == null) return;

        var menu = new System.Windows.Forms.ContextMenuStrip();
        menu.Items.Add(LocalizationService.T("tray.open_overlay"), null, (_, _) => ToggleOverlay());
        menu.Items.Add(LocalizationService.T("tray.settings"), null, (_, _) => OpenSettingsWindow());
        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
        menu.Items.Add(LocalizationService.T("tray.exit"), null, (_, _) => Shutdown());

        _trayIcon.ContextMenuStrip = menu;
        _trayIcon.Text = LocalizationService.T("app.title");
    }

    private void ToggleOverlay()
    {
        if (_activeOverlay != null && _activeOverlay.IsLoaded)
        {
            _activeOverlay.Close();
            _activeOverlay = null;
            return;
        }

        try
        {
            // Захватываем экран и активное окно ДО показа нашего оверлея
            var context = ScreenCaptureService.CaptureCurrentContext();

            _activeOverlay = new OverlayWindow(context);
            _activeOverlay.RequestOpenSettings += OpenSettingsWindow;
            _activeOverlay.Closed += (_, _) =>
            {
                context.FullScreenBitmap.Dispose();
                context.ActiveWindowBitmap?.Dispose();
                _activeOverlay = null;
            };
            _activeOverlay.Show();
        }
        catch (Exception ex)
        {
            AppLogger.Error("Failed to open OverlayWindow", ex);
        }
    }

    private void OpenSettingsWindow()
    {
        if (_activeSettings != null && _activeSettings.IsLoaded)
        {
            _activeSettings.Activate();
            return;
        }

        _activeSettings = new SettingsWindow();
        _activeSettings.Closed += (_, _) => _activeSettings = null;
        _activeSettings.Show();
        _activeSettings.Activate();
    }

    private static Icon CreateRoyalBlueAppIcon()
    {
        try
        {
            var icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "app.ico");
            if (File.Exists(icoPath))
            {
                return new Icon(icoPath);
            }
        }
        catch
        {
            // fallback to dynamic draw
        }

        using var bmp = new Bitmap(32, 32);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            using var bgBrush = new SolidBrush(Color.FromArgb(255, 65, 105, 225)); // RoyalBlue
            g.FillEllipse(bgBrush, 2, 2, 28, 28);

            using var starBrush = new SolidBrush(Color.White);
            var pts = new PointF[]
            {
                new(16, 6),
                new(18.5f, 13.5f),
                new(26, 16),
                new(18.5f, 18.5f),
                new(16, 26),
                new(13.5f, 18.5f),
                new(6, 16),
                new(13.5f, 13.5f)
            };
            g.FillPolygon(starBrush, pts);
        }

        return Icon.FromHandle(bmp.GetHicon());
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _hookService?.Dispose();
        if (_trayIcon != null)
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
        }
        _singleInstanceMutex?.Dispose();
        base.OnExit(e);
    }
}
