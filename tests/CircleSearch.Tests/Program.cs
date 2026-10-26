using CircleSearch.Domain;
using CircleSearch.Infrastructure;
using CircleSearch.Services;

namespace CircleSearch.Tests;

public static class Program
{
    private static readonly string[] RequiredKeys =
    {
        "app.title",
        "app.subtitle",
        "tray.open_overlay",
        "tray.settings",
        "tray.exit",
        "overlay.hint_drag",
        "overlay.btn_attach_window",
        "overlay.btn_attach_screen",
        "overlay.btn_settings",
        "overlay.btn_close",
        "overlay.lens_panel_title",
        "overlay.lens_expand",
        "overlay.lens_open_external",
        "overlay.lens_close_panel",
        "overlay.lens_loading",
        "overlay.desktop_title",
        "settings.title",
        "settings.monitor_mode_label",
        "settings.monitor_cursor",
        "settings.monitor_active_window",
        "settings.ui_lang_label",
        "settings.theme_label",
        "settings.theme_dark",
        "settings.theme_light",
        "settings.long_press_label",
        "settings.enable_win_hold",
        "settings.autostart",
        "settings.save",
        "settings.cancel"
    };

    public static int Main()
    {
        // Изолируем тесты от реальных настроек пользователя (%LocalAppData%\CircleSearch\settings.json)
        var isolatedConfigDir = Path.Combine(Path.GetTempPath(), "CircleSearch.Tests", Guid.NewGuid().ToString("N"));
        Environment.SetEnvironmentVariable(ConfigManager.ConfigDirEnvVar, isolatedConfigDir);

        try
        {
            TestAllTwelveLocales();
            TestConfigPersistence();
            TestLensHtmlGeneration();
            TestDefaultBrowserCommandParsing();
            TestAdaptiveUiMetrics();
            TestLocaleKeyParity();
            TestThemeStylesAndSettingsWindow();
            Console.WriteLine("ALL TESTS PASSED SUCCESSFULLY (12 LOCALES VERIFIED).");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"TEST FAILED: {ex.Message}");
            return 1;
        }
        finally
        {
            try { Directory.Delete(isolatedConfigDir, recursive: true); } catch { }
        }
    }

    private static void TestAdaptiveUiMetrics()
    {
        foreach (var width in new double[] { 640, 1024, 1366, 1920, 2560, 3840, 7680 })
        {
            foreach (var expanded in new[] { false, true })
            {
                var drawer = CircleSearch.UI.UiMetrics.DrawerWidth(width, expanded);
                if (drawer <= 0 || drawer > width)
                    throw new Exception($"Drawer width {drawer} does not fit screen {width}");
            }

            var scale = CircleSearch.UI.UiMetrics.ChromeScale(width, width * 9 / 16);
            if (scale < 0.85 || scale > 1.25)
                throw new Exception($"Chrome scale {scale} is out of bounds for width {width}");
        }

        if (CircleSearch.UI.UiMetrics.DrawerWidth(1920, true) <= CircleSearch.UI.UiMetrics.DrawerWidth(1920, false))
            throw new Exception("Expanded drawer must be wider than collapsed");
    }
    private static void TestThemeStylesAndSettingsWindow()
    {
        // Проверяем, что XAML-стили валидны и окно настроек строится в обеих темах (требуется STA-поток)
        Exception? error = null;
        var thread = new Thread(() =>
        {
            try
            {
                var app = new System.Windows.Application();
                foreach (var theme in new[] { "Dark", "Light" })
                {
                    CircleSearch.UI.ThemeManager.ApplyTheme(theme);
                    var window = new CircleSearch.UI.Windows.SettingsWindow();
                    if (window.Content is not System.Windows.FrameworkElement root)
                        throw new Exception($"SettingsWindow has no content in theme {theme}");
                    root.Measure(new System.Windows.Size(520, 760));
                    root.Arrange(new System.Windows.Rect(0, 0, 520, 760));
                    root.UpdateLayout();
                    if (root.DesiredSize.Width <= 0 || root.DesiredSize.Height <= 0)
                        throw new Exception($"SettingsWindow has empty layout in theme {theme}");
                }

                var menuRenderer = new CircleSearch.UI.ThemedMenuRenderer();
                if (menuRenderer == null) throw new Exception("Menu renderer was not created");
            }
            catch (Exception ex)
            {
                error = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (error != null)
            throw new Exception($"Theme/Settings UI smoke test failed: {error.GetType().Name}: {error.Message} {error.InnerException?.Message}");
    }
    private static void TestLocaleKeyParity()
    {
        // Все языки должны содержать ровно те же ключи, что и английский, с теми же плейсхолдерами {0}
        var asm = typeof(LocalizationService).Assembly;
        var locales = new Dictionary<string, Dictionary<string, string>>();
        foreach (var resource in asm.GetManifestResourceNames().Where(n => n.EndsWith(".common.json", StringComparison.OrdinalIgnoreCase)))
        {
            using var stream = asm.GetManifestResourceStream(resource)!;
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(stream)!;
            var code = resource.Split('.')[^3];
            locales[code] = dict;
        }

        if (!locales.TryGetValue("en", out var en))
            throw new Exception("English locale not found");

        static string Placeholders(string s) =>
            string.Join(",", System.Text.RegularExpressions.Regex.Matches(s, @"\{\d+\}").Select(m => m.Value).OrderBy(x => x));

        foreach (var (code, dict) in locales)
        {
            var missing = en.Keys.Except(dict.Keys).ToList();
            var extra = dict.Keys.Except(en.Keys).ToList();
            if (missing.Count > 0) throw new Exception($"Locale '{code}' is missing keys: {string.Join(", ", missing)}");
            if (extra.Count > 0) throw new Exception($"Locale '{code}' has unknown keys: {string.Join(", ", extra)}");

            foreach (var (key, value) in dict)
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception($"Locale '{code}' has empty value for '{key}'");
                if (Placeholders(value) != Placeholders(en[key]))
                    throw new Exception($"Locale '{code}' has mismatched placeholders for '{key}'");
            }
        }

        if (GoogleLensService.ToGoogleLanguageCode("zh") != "zh-CN" || GoogleLensService.ToGoogleLanguageCode("ru") != "ru")
            throw new Exception("Unexpected Google language code mapping");
    }
    private static void TestDefaultBrowserCommandParsing()
    {
        var cmdExe = Path.Combine(Environment.SystemDirectory, "cmd.exe");

        if (DefaultBrowserResolver.ParseExecutable($"\"{cmdExe}\" --single-argument %1") != cmdExe)
            throw new Exception("ParseExecutable failed for quoted command line");

        if (DefaultBrowserResolver.ParseExecutable($"{cmdExe} /c \"%1\"") != cmdExe)
            throw new Exception("ParseExecutable failed for unquoted command line");

        if (DefaultBrowserResolver.ParseExecutable("\"C:\\does\\not\\exist\\browser.exe\" %1") != null)
            throw new Exception("ParseExecutable must return null for a missing executable");

        if (DefaultBrowserResolver.ParseExecutable("  ") != null || DefaultBrowserResolver.ParseExecutable(null) != null)
            throw new Exception("ParseExecutable must return null for empty input");
    }

    private static void TestAllTwelveLocales()
    {
        if (LocalizationService.SupportedLanguages.Length < 12)
            throw new Exception("Expected at least 12 supported languages");

        foreach (var (code, nativeName) in LocalizationService.SupportedLanguages)
        {
            if (string.IsNullOrWhiteSpace(nativeName))
                throw new Exception($"NativeName is empty for language {code}");

            LocalizationService.CurrentLanguage = code;
            foreach (var key in RequiredKeys)
            {
                var val = LocalizationService.T(key);
                if (string.IsNullOrWhiteSpace(val) || val == key)
                    throw new Exception($"Missing translation for key '{key}' in locale '{code}'");
            }

            var formatted = LocalizationService.Tf("overlay.btn_attach_window", "Chrome");
            if (!formatted.Contains("Chrome", StringComparison.Ordinal))
                throw new Exception($"Formatting failed for 'overlay.btn_attach_window' in locale '{code}'");
        }

        LocalizationService.CurrentLanguage = "ru";
    }

    private static void TestConfigPersistence()
    {
        var origMs = ConfigManager.Current.LongPressMs;
        var origMode = ConfigManager.Current.TargetMonitorMode;
        try
        {
            ConfigManager.Current.LongPressMs = 450;
            ConfigManager.Current.TargetMonitorMode = "ActiveWindow";
            ConfigManager.Save();
            ConfigManager.Load();
            if (ConfigManager.Current.LongPressMs != 450 || ConfigManager.Current.TargetMonitorMode != "ActiveWindow")
                throw new Exception("ConfigManager failed to persist settings");
        }
        finally
        {
            ConfigManager.Current.LongPressMs = origMs;
            ConfigManager.Current.TargetMonitorMode = origMode;
            ConfigManager.Save();
        }
    }

    private static void TestLensHtmlGeneration()
    {
        var dummyBytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
        var html = GoogleLensService.BuildAutoSubmitHtml(dummyBytes);
        if (!html.Contains(GoogleLensService.UploadEndpoint, StringComparison.Ordinal))
            throw new Exception("Generated HTML does not contain Google Lens upload endpoint");
    }
}
