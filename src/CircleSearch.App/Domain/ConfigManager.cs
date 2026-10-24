using System.IO;
using System.Text;
using System.Text.Json;
using CircleSearch.Services;

namespace CircleSearch.Domain;

public sealed class AppSettings
{
    // "auto" (по умолчанию — автоопределение по языку системы) либо конкретный код языка ("en", "kk", "ru"...)
    public string UiLanguage { get; set; } = "auto";
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
        var dir = Path.Combine(appData, "CircleSearch");
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
                    if (string.IsNullOrWhiteSpace(loaded.UiLanguage))
                        loaded.UiLanguage = "auto";
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

                    var key = line.Substring(0, idx).Trim();
                    var val = line.Substring(idx + 1).Trim().Trim('"', '\'');

                    switch (key.ToUpperInvariant())
                    {
                        case "LONG_PRESS_MS":
                            if (int.TryParse(val, out var ms)) _current.LongPressMs = ms;
                            break;
                        case "THEME":
                            _current.Theme = val;
                            break;
                        case "TARGET_MONITOR_MODE":
                            _current.TargetMonitorMode = val;
                            break;
                        case "UI_LANGUAGE":
                            _current.UiLanguage = val;
                            break;
                    }
                }
                break;
            }
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Error("Failed to parse .env", ex);
        }
    }

    public static void Save()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_current, options);
            File.WriteAllText(ConfigFilePath, json, Encoding.UTF8);
            SettingsChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Error("Failed to save settings.json", ex);
        }
    }
}
