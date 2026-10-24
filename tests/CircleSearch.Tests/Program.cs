using CircleSearch.Domain;
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
        try
        {
            TestAllTwelveLocales();
            TestConfigPersistence();
            TestLensHtmlGeneration();
            Console.WriteLine("ALL TESTS PASSED SUCCESSFULLY (12 LOCALES VERIFIED).");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"TEST FAILED: {ex.Message}");
            return 1;
        }
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
        if (!html.Contains("https://lens.google.com/upload", StringComparison.Ordinal))
            throw new Exception("Generated HTML does not contain Google Lens upload endpoint");
    }
}
