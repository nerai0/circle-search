using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OkGoogle.Infrastructure;

public sealed class WinKeyHookService : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;

    private const int VK_LWIN = 0x5B;
    private const int VK_RWIN = 0x5C;
    private const int VK_SPACE = 0x20;
    private const int VK_LMENU = 0xA4;
    private const int VK_RMENU = 0xA5;
    private const int VK_MENU = 0x12;
    private const int VK_CONTROL = 0x11;

    private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private IntPtr _hookId = IntPtr.Zero;
    private readonly LowLevelKeyboardProc _proc;
    private System.Threading.Timer? _holdTimer;

    private bool _winKeyDown;
    private bool _otherKeyPressedDuringWin;
    private bool _longPressTriggered;

    public event Action? OverlayTriggered;

    public WinKeyHookService()
    {
        _proc = HookCallback;
    }

    public void Start()
    {
        if (_hookId != IntPtr.Zero) return;
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        var moduleHandle = curModule != null ? GetModuleHandle(curModule.ModuleName) : IntPtr.Zero;
        _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, moduleHandle, 0);
        AppLogger.Info("WinKeyHookService started", new { HookInstalled = _hookId != IntPtr.Zero });
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var kb = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            var msg = wParam.ToInt32();
            bool isKeyDown = msg == WM_KEYDOWN || msg == WM_SYSKEYDOWN;
            bool isKeyUp = msg == WM_KEYUP || msg == WM_SYSKEYUP;
            bool isWinKey = kb.vkCode == VK_LWIN || kb.vkCode == VK_RWIN;

            // Резервный хоткей Alt + Space
            if (isKeyDown && kb.vkCode == VK_SPACE && IsAltPressed() && ! _winKeyDown)
            {
                TriggerOverlaySafe();
                return (IntPtr)1;
            }

            if (Domain.ConfigManager.Current.EnableWinHold)
            {
                if (isWinKey)
                {
                    if (isKeyDown)
                    {
                        if (!_winKeyDown)
                        {
                            _winKeyDown = true;
                            _otherKeyPressedDuringWin = false;
                            _longPressTriggered = false;

                            int delay = Math.Clamp(Domain.ConfigManager.Current.LongPressMs, 250, 1500);
                            _holdTimer?.Dispose();
                            _holdTimer = new System.Threading.Timer(OnWinHoldTimerFired, null, delay, Timeout.Infinite);
                        }
                        else if (_longPressTriggered)
                        {
                            // Подавляем автоповтор клавиши Win после срабатывания оверлея
                            return (IntPtr)1;
                        }
                    }
                    else if (isKeyUp)
                    {
                        _winKeyDown = false;
                        _holdTimer?.Dispose();
                        _holdTimer = null;

                        if (_longPressTriggered)
                        {
                            _longPressTriggered = false;
                            // Дополнительно отпускаем Win в системе без открытия меню Пуск
                        }
                    }
                }
                else if (_winKeyDown && isKeyDown && kb.vkCode != VK_CONTROL)
                {
                    // Пользователь нажал комбинацию вроде Win+D, Win+R, Win+Shift+S — отменяем таймер удержания!
                    _otherKeyPressedDuringWin = true;
                    _holdTimer?.Dispose();
                    _holdTimer = null;
                }
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private void OnWinHoldTimerFired(object? state)
    {
        if (!_winKeyDown || _otherKeyPressedDuringWin || _longPressTriggered)
            return;

        _longPressTriggered = true;

        try
        {
            // Отправляем "холостое" нажатие и отпускание VK_CONTROL, чтобы Windows поняла,
            // что это была комбинация, и НЕ открывала меню Пуск при отпускании клавиши Win!
            keybd_event((byte)VK_CONTROL, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
            keybd_event((byte)VK_CONTROL, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
        catch (Exception ex)
        {
            AppLogger.Error("Failed to suppress Start menu via dummy key", ex);
        }

        TriggerOverlaySafe();
    }

    private static bool IsAltPressed()
    {
        return (GetAsyncKeyState(VK_MENU) & 0x8000) != 0 ||
               (GetAsyncKeyState(VK_LMENU) & 0x8000) != 0 ||
               (GetAsyncKeyState(VK_RMENU) & 0x8000) != 0;
    }

    private void TriggerOverlaySafe()
    {
        try
        {
            System.Windows.Application.Current?.Dispatcher.BeginInvoke(() =>
            {
                OverlayTriggered?.Invoke();
            });
        }
        catch (Exception ex)
        {
            AppLogger.Error("Error invoking OverlayTriggered", ex);
        }
    }

    public void Dispose()
    {
        _holdTimer?.Dispose();
        if (_hookId != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }
    }
}
