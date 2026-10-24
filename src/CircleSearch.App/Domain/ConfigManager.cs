using System.IO;
using System.Text;
using System.Text.Json;

namespace OkGoogle.Domain;

public sealed class AppSettings
{
    public string UiLanguage { get; set; } = "ru";
    public string Theme { get; set; } = "Dark";
    public int LongPressMs { get; set; } = 400;
    public bool EnableWinHold { get; set; } = true;
    // "Cursor" (по умолчанию — монитор под курсором мыши) или "ActiveWindow" (монитор активного окна)
    public string TargetMonitorMode { get; set; } = "Cursor";
}

public static class ConfigManager
{
    private static readonly string ConfigFilePath;
    private static AppSettings _current = new();

    public static event Action? SettingsChanged;

    static ConfigManager()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dir = Path.Combine(appData, "OkGoogle");
        Directory.CreateDirectory(dir);
        ConfigFilePath = Path.Combine(dir, "settings.json");
        Load();
    }

    public static AppSettings Current => _current;

    public static void Load()
    {
        LoadDotEnv();

        if (File.Exists(ConfigFilePath))
        {
            try
            {
                var json = File.ReadAllText(ConfigFilePath, Encoding.UTF8);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                if (loaded != null)
                {
                    if (string.IsNullOrWhiteSpace(loaded.TargetMonitorMode))
                        loaded.TargetMonitorMode = "Cursor";
                    _current = loaded;
                }
            }
            catch (Exception ex)
            {
                Infrastructure.AppLogger.Error("Failed to load settings.json", ex);
            }
        }
    }

    private static void LoadDotEnv()
    {
        try
        {
            var candidates = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env"),
                Path.Combine(Directory.GetCurrentDirectory(), ".env"),
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", ".env"))
            };

            foreach (var path in candidates)
            {
                if (!File.Exists(path)) continue;

                foreach (var rawLine in File.ReadAllLines(path))
                {
                    var line = rawLine.Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith('#')) continue;
                    var idx = line.IndexOf('=');
                    if (idx <= 0) continue;

                    var key = line[..idx].Trim();
                    var val = line[(idx + 1)..].Trim().Trim('"', '\'');

                    if (key.Equals("DEFAULT_LANGUAGE", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(val))
                        _current.UiLanguage = val;
                    else if (key.Equals("DEFAULT_THEME", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(val))
                        _current.Theme = val;
                    else if (key.Equals("LONG_PRESS_MS", StringComparison.OrdinalIgnoreCase) && int.TryParse(val, out var ms) && ms >= 200)
                        _current.LongPressMs = ms;
                    else if (key.Equals("TARGET_MONITOR_MODE", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(val))
                        _current.TargetMonitorMode = val;
                }
                break;
            }
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Error("Failed to read .env", ex);
        }
    }

    public static void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_current, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath, json, Encoding.UTF8);
            SettingsChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Error("Failed to save settings.json", ex);
        }
    }
}
