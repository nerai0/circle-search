using System.IO;
using System.Reflection;
using System.Text.Json;

namespace CircleSearch.Services;

public static class LocalizationService
{
    public static readonly (string Code, string NativeName)[] SupportedLanguages =
    {
        ("kk", "Қазақша"),
        ("ru", "Русский"),
        ("en", "English"),
        ("es", "Español"),
        ("de", "Deutsch"),
        ("fr", "Français"),
        ("pt", "Português"),
        ("tr", "Türkçe"),
        ("uk", "Українська"),
        ("zh", "中文 (简体)"),
        ("ja", "日本語"),
        ("ko", "한국어")
    };

    private static readonly Dictionary<string, Dictionary<string, string>> Translations = new(StringComparer.OrdinalIgnoreCase);
    private static string _currentLang = "kk";

    public static event Action? LanguageChanged;

    static LocalizationService()
    {
        foreach (var (code, _) in SupportedLanguages)
        {
            LoadEmbeddedLocale(code);
        }
        _currentLang = Domain.ConfigManager.Current.UiLanguage;
    }

    private static void LoadEmbeddedLocale(string lang)
    {
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            var resourceName = asm.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith($"Locales.{lang}.common.json", StringComparison.OrdinalIgnoreCase));

            if (resourceName == null) return;

            using var stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null) return;
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (dict != null)
            {
                Translations[lang] = dict;
            }
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Error($"Failed to load locale {lang}", ex);
        }
    }

    public static string CurrentLanguage
    {
        get => _currentLang;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            _currentLang = value.ToLowerInvariant();
            LanguageChanged?.Invoke();
        }
    }

    public static string T(string key)
    {
        if (Translations.TryGetValue(_currentLang, out var dict) && dict.TryGetValue(key, out var val))
            return val;

        if (Translations.TryGetValue("kk", out var kkDict) && kkDict.TryGetValue(key, out var kkVal))
            return kkVal;

        if (Translations.TryGetValue("en", out var enDict) && enDict.TryGetValue(key, out var enVal))
            return enVal;

        if (Translations.TryGetValue("ru", out var ruDict) && ruDict.TryGetValue(key, out var ruVal))
            return ruVal;

        return key;
    }

    public static string Tf(string key, params object[] args)
    {
        var template = T(key);
        try
        {
            return string.Format(template, args);
        }
        catch
        {
            return template;
        }
    }
}
