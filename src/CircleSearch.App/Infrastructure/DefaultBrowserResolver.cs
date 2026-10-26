using System.IO;
using Microsoft.Win32;

namespace CircleSearch.Infrastructure;

/// <summary>
/// Определяет исполняемый файл браузера, выбранного пользователем в Windows «по умолчанию»
/// (Chrome, Edge, Firefox, Brave, Opera, Yandex и любой другой), без привязки к конкретному браузеру.
/// </summary>
public static class DefaultBrowserResolver
{
    private static readonly string[] UrlSchemes = { "https", "http" };

    public static string? ResolveExecutablePath()
    {
        foreach (var scheme in UrlSchemes)
        {
            try
            {
                using var choiceKey = Registry.CurrentUser.OpenSubKey(
                    $@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\{scheme}\UserChoice");
                var progId = choiceKey?.GetValue("ProgId") as string;
                if (string.IsNullOrWhiteSpace(progId)) continue;

                using var commandKey = Registry.ClassesRoot.OpenSubKey($@"{progId}\shell\open\command");
                var exePath = ParseExecutable(commandKey?.GetValue(string.Empty) as string);
                if (exePath != null) return exePath;
            }
            catch (Exception ex)
            {
                AppLogger.Warning("Failed to resolve default browser", new { scheme, ex.Message });
            }
        }

        return null;
    }

    /// <summary>
    /// Извлекает путь к .exe из командной строки shell\open\command
    /// (например, <c>"C:\...\chrome.exe" --single-argument %1</c>).
    /// </summary>
    public static string? ParseExecutable(string? command)
    {
        if (string.IsNullOrWhiteSpace(command)) return null;

        command = command.Trim();
        string path;

        if (command[0] == '"')
        {
            var end = command.IndexOf('"', 1);
            if (end < 0) return null;
            path = command[1..end];
        }
        else
        {
            var idx = command.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return null;
            path = command[..(idx + 4)];
        }

        path = Environment.ExpandEnvironmentVariables(path);
        return File.Exists(path) ? path : null;
    }
}
