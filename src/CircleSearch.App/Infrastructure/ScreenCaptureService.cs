using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;

namespace CircleSearch.Infrastructure;

public sealed class CapturedScreenContext
{
    public required Bitmap FullScreenBitmap { get; init; }
    public required byte[] FullScreenPngBytes { get; init; }
    public required BitmapSource FullScreenWpfSource { get; init; }
    public required Rectangle ScreenBounds { get; init; }

    public Bitmap? ActiveWindowBitmap { get; init; }
    public byte[]? ActiveWindowPngBytes { get; init; }
    public string ActiveWindowTitle { get; init; } = string.Empty;
    public Rectangle ActiveWindowBounds { get; init; }
}

public static class ScreenCaptureService
{
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;
    private const uint MONITOR_DEFAULTTONEAREST = 2;
    private const uint SRCCOPY = 0x00CC0020;
    private const uint CAPTUREBLT = 0x40000000;

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(
        IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
        IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

    [DllImport("user32.dll")]
    private static extern IntPtr GetProcessWindowStation();

    [DllImport("kernel32.dll")]
    private static extern int GetCurrentThreadId();

    [DllImport("user32.dll")]
    private static extern IntPtr GetThreadDesktop(int dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool GetUserObjectInformationW(IntPtr hObj, int nIndex, StringBuilder pvInfo, int nLength, out int lpnLengthNeeded);

    public static string GetCurrentDesktopInfo()
    {
        const int UOI_NAME = 2;
        var sbWinsta = new StringBuilder(128);
        GetUserObjectInformationW(GetProcessWindowStation(), UOI_NAME, sbWinsta, sbWinsta.Capacity, out _);

        var sbDesk = new StringBuilder(128);
        GetUserObjectInformationW(GetThreadDesktop(GetCurrentThreadId()), UOI_NAME, sbDesk, sbDesk.Capacity, out _);

        return $"{sbWinsta}\\{sbDesk}";
    }

    private static Rectangle GetPhysicalMonitorBounds(IntPtr hMonitor)
    {
        if (hMonitor != IntPtr.Zero)
        {
            var mi = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };
            if (GetMonitorInfo(hMonitor, ref mi))
            {
                return Rectangle.FromLTRB(
                    mi.rcMonitor.Left,
                    mi.rcMonitor.Top,
                    mi.rcMonitor.Right,
                    mi.rcMonitor.Bottom);
            }
        }

        return System.Windows.Forms.Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
    }

    private static Bitmap CapturePhysicalRectangle(Rectangle rect)
    {
        var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        var hdcDest = g.GetHdc();
        var hdcSrc = GetDC(IntPtr.Zero);
        try
        {
            BitBlt(
                hdcDest, 0, 0, rect.Width, rect.Height,
                hdcSrc, rect.Left, rect.Top,
                SRCCOPY | CAPTUREBLT);
        }
        finally
        {
            ReleaseDC(IntPtr.Zero, hdcSrc);
            g.ReleaseHdc(hdcDest);
        }
        return bmp;
    }

    public static CapturedScreenContext CaptureCurrentContext()
    {
        var fgHwnd = GetForegroundWindow();
        var activeTitle = GetWindowTitle(fgHwnd);

        // Определяем целевой монитор через нативный Win32 API (MonitorFromPoint / MonitorFromWindow + GetMonitorInfo),
        // чтобы при разных масштабах DPI (100% / 125% / 150%) на втором мониторе всегда брать 100% физического разрешения!
        IntPtr targetMonitor;
        var mode = Domain.ConfigManager.Current.TargetMonitorMode;

        if (string.Equals(mode, "ActiveWindow", StringComparison.OrdinalIgnoreCase) && fgHwnd != IntPtr.Zero)
        {
            targetMonitor = MonitorFromWindow(fgHwnd, MONITOR_DEFAULTTONEAREST);
        }
        else
        {
            GetCursorPos(out var pt);
            targetMonitor = MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);
        }

        var screenBounds = GetPhysicalMonitorBounds(targetMonitor);
        var fullBmp = CapturePhysicalRectangle(screenBounds);

        var fullPng = BitmapToPngBytes(fullBmp);
        var wpfSource = BytesToBitmapSource(fullPng);

        Bitmap? winBmp = null;
        byte[]? winPng = null;
        Rectangle winRect = Rectangle.Empty;

        if (fgHwnd != IntPtr.Zero)
        {
            if (DwmGetWindowAttribute(fgHwnd, DWMWA_EXTENDED_FRAME_BOUNDS, out var dwmRect, Marshal.SizeOf<RECT>()) == 0)
            {
                winRect = Rectangle.FromLTRB(dwmRect.Left, dwmRect.Top, dwmRect.Right, dwmRect.Bottom);
            }
            else if (GetWindowRect(fgHwnd, out var rawRect))
            {
                winRect = Rectangle.FromLTRB(rawRect.Left, rawRect.Top, rawRect.Right, rawRect.Bottom);
            }

            var clampedOnTarget = Rectangle.Intersect(screenBounds, winRect);
            if (clampedOnTarget.Width > 40 && clampedOnTarget.Height > 40)
            {
                try
                {
                    var localRect = new Rectangle(
                        clampedOnTarget.X - screenBounds.X,
                        clampedOnTarget.Y - screenBounds.Y,
                        clampedOnTarget.Width,
                        clampedOnTarget.Height);

                    winBmp = fullBmp.Clone(localRect, PixelFormat.Format32bppArgb);
                    winPng = BitmapToPngBytes(winBmp);
                }
                catch (Exception ex)
                {
                    AppLogger.Warning("Could not clone active window region", new { ex.Message });
                }
            }
            else if (winRect.Width > 40 && winRect.Height > 40)
            {
                try
                {
                    winBmp = CapturePhysicalRectangle(winRect);
                    winPng = BitmapToPngBytes(winBmp);
                }
                catch (Exception ex)
                {
                    AppLogger.Warning("Could not capture active window from secondary monitor", new { ex.Message });
                }
            }
        }

        if (string.IsNullOrWhiteSpace(activeTitle))
            activeTitle = "Рабочий стол";

        return new CapturedScreenContext
        {
            FullScreenBitmap = fullBmp,
            FullScreenPngBytes = fullPng,
            FullScreenWpfSource = wpfSource,
            ScreenBounds = screenBounds,
            ActiveWindowBitmap = winBmp,
            ActiveWindowPngBytes = winPng,
            ActiveWindowTitle = activeTitle,
            ActiveWindowBounds = winRect
        };
    }

    public static byte[] CropBitmapToPng(Bitmap source, Rectangle cropArea)
    {
        var validRect = Rectangle.Intersect(new Rectangle(0, 0, source.Width, source.Height), cropArea);
        if (validRect.Width <= 2 || validRect.Height <= 2)
            return BitmapToPngBytes(source);

        using var cropped = source.Clone(validRect, PixelFormat.Format32bppArgb);
        return BitmapToPngBytes(cropped);
    }

    public static byte[] BitmapToPngBytes(Bitmap bmp)
    {
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }

    public static BitmapSource BytesToBitmapSource(byte[] pngBytes)
    {
        using var ms = new MemoryStream(pngBytes);
        var bi = new BitmapImage();
        bi.BeginInit();
        bi.CacheOption = BitmapCacheOption.OnLoad;
        bi.StreamSource = ms;
        bi.EndInit();
        bi.Freeze();
        return bi;
    }

    private static string GetWindowTitle(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero) return string.Empty;
        var sb = new StringBuilder(256);
        return GetWindowText(hWnd, sb, sb.Capacity) > 0 ? sb.ToString().Trim() : string.Empty;
    }
}
