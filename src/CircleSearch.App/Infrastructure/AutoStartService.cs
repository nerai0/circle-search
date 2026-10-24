using System.Diagnostics;
using Microsoft.Win32;

namespace OkGoogle.Infrastructure;

public static class AutoStartService
{
    private const string RunRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppValueName = "OkGoogleScreenSearch";

    public static bool IsEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, writable: false);
            var val = key?.GetValue(AppValueName) as string;
            return !string.IsNullOrWhiteSpace(val);
        }
        catch (Exception ex)
        {
            AppLogger.Error("Failed to read autostart registry value", ex);
            return false;
        }
    }

    public static void SetEnabled(bool enabled)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunRegistryKey, writable: true);
            if (key == null) return;

            if (enabled)
            {
                var exePath = Process.GetCurrentProcess().MainModule?.FileName;
                if (!string.IsNullOrWhiteSpace(exePath))
                {
                    key.SetValue(AppValueName, $"\"{exePath}\"");
                }
            }
            else
            {
                key.DeleteValue(AppValueName, throwOnMissingValue: false);
            }
        }
        catch (Exception ex)
        {
            AppLogger.Error("Failed to update autostart registry value", ex);
        }
    }
}
