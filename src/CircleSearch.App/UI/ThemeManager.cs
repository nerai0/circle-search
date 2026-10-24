using System.Windows;
using System.Windows.Media;

namespace CircleSearch.UI;

public static class ThemeManager
{
    public const string IconSparkle = "M12,2 L14.5,9.5 L22,12 L14.5,14.5 L12,22 L9.5,14.5 L2,12 L9.5,9.5 Z";
    public const string IconTranslate = "M12.87,15.07 L10.33,12.56 L10.36,12.53 C12.1,10.59 13.34,8.36 14.07,6 L17,6 L17,4 L10,4 L10,2 L8,2 L8,4 L1,4 L1,6 L12.17,6 C11.5,7.92 10.44,9.75 9,11.35 C8.07,10.32 7.3,9.19 6.69,8 L4.69,8 C5.42,9.63 6.42,11.17 7.67,12.56 L2.58,17.58 L4,19 L9,14 L12.11,17.11 L12.87,15.07 Z M18.5,10 L16.5,10 L12,22 L14,22 L15.12,19 L19.87,19 L21,22 L23,22 L18.5,10 Z M15.88,17 L17.5,12.67 L19.12,17 L15.88,17 Z";
    public const string IconWindow = "M4,4 L20,4 C21.1,4 22,4.9 22,6 L22,18 C22,19.1 21.1,20 20,20 L4,20 C2.9,20 2,19.1 2,18 L2,6 C2,4.9 2.9,4 4,4 Z M4,8 L4,18 L20,18 L20,8 L4,8 Z";
    public const string IconScreen = "M21,3 L3,3 C1.9,3 1,3.9 1,5 L1,17 C1,18.1 1.9,19 3,19 L8,19 L8,21 L16,21 L16,19 L21,19 C22.1,19 23,18.1 23,17 L23,5 C23,3.9 22.1,3 21,3 Z M21,17 L3,17 L3,5 L21,5 L21,17 Z";
    public const string IconMic = "M12,14 C13.66,14 15,12.66 15,11 L15,5 C15,3.34 13.66,2 12,2 C10.34,2 9,3.34 9,5 L9,11 C9,12.66 10.34,14 12,14 Z M17,11 C17,13.76 14.76,16 12,16 C9.24,16 7,13.76 7,11 L5,11 C5,14.53 7.61,17.43 11,17.92 L11,21 L13,21 L13,17.92 C16.39,17.43 19,14.53 19,11 L17,11 Z";
    public const string IconSend = "M2.01,21 L23,12 L2.01,3 L2,10 L17,12 L2,14 L2.01,21 Z";
    public const string IconCopy = "M16,1 L4,1 C2.9,1 2,1.9 2,3 L2,17 L4,17 L4,3 L16,3 L16,1 Z M19,5 L8,5 C6.9,5 6,5.9 6,7 L6,21 C6,22.1 6.9,23 8,23 L19,23 C20.1,23 21,22.1 21,21 L21,7 C21,5.9 20.1,5 19,5 Z M19,21 L8,21 L8,7 L19,7 L19,21 Z";
    public const string IconSearch = "M15.5,14 L14.71,14 L14.43,13.73 C15.41,12.59 16,11.11 16,9.5 C16,5.91 13.09,3 9.5,3 C5.91,3 3,5.91 3,9.5 C3,13.09 5.91,16 9.5,16 C11.11,16 12.59,15.41 13.73,14.43 L14,14.71 L14,15.5 L19,20.49 L20.49,19 L15.5,14 Z M9.5,14 C7.01,14 5,11.99 5,9.5 C5,7.01 7.01,5 9.5,5 C11.99,5 14,7.01 14,9.5 C14,11.99 11.99,14 9.5,14 Z";
    public const string IconSettings = "M19.14,12.94 C19.18,12.64 19.2,12.33 19.2,12 C19.2,11.68 19.18,11.36 19.13,11.06 L21.16,9.48 C21.34,9.34 21.39,9.07 21.28,8.87 L19.36,5.55 C19.24,5.33 18.99,5.26 18.77,5.33 L16.38,6.29 C15.88,5.91 15.35,5.59 14.76,5.35 L14.4,2.81 C14.36,2.57 14.16,2.4 13.92,2.4 L10.08,2.4 C9.84,2.4 9.65,2.57 9.61,2.81 L9.25,5.35 C8.66,5.59 8.12,5.92 7.63,6.29 L5.24,5.33 C5.02,5.25 4.77,5.33 4.65,5.55 L2.74,8.87 C2.62,9.08 2.66,9.34 2.86,9.48 L4.89,11.06 C4.84,11.36 4.8,11.69 4.8,12 C4.8,12.31 4.82,12.64 4.87,12.94 L2.84,14.52 C2.66,14.66 2.61,14.93 2.72,15.13 L4.64,18.45 C4.76,18.67 5.01,18.74 5.23,18.67 L7.62,17.71 C8.12,18.09 8.65,18.41 9.24,18.65 L9.6,21.19 C9.65,21.43 9.84,21.6 10.08,21.6 L13.92,21.6 C14.16,21.6 14.36,21.43 14.39,21.19 L14.75,18.65 C15.34,18.41 15.88,18.09 16.37,17.71 L18.76,18.67 C18.98,18.75 19.23,18.67 19.35,18.45 L21.27,15.13 C21.39,14.91 21.34,14.66 21.15,14.52 L19.14,12.94 Z M12,15.6 C10.02,15.6 8.4,13.98 8.4,12 C8.4,10.02 10.02,8.4 12,8.4 C13.98,8.4 15.6,10.02 15.6,12 C15.6,13.98 13.98,15.6 12,15.6 Z";
    public const string IconClose = "M19,6.41 L17.59,5 L12,10.59 L6.41,5 L5,6.41 L10.59,12 L5,17.59 L6.41,19 L12,13.41 L17.59,19 L19,17.59 L13.41,12 L19,6.41 Z";

    public static void ApplyTheme(string themeName)
    {
        var app = System.Windows.Application.Current;
        if (app == null) return;

        bool isDark = !string.Equals(themeName, "Light", StringComparison.OrdinalIgnoreCase);

        // RoyalBlue (#4169E1) как основной акцентный цвет
        SetBrush(app, "PrimaryBrush", "#4169E1");
        SetBrush(app, "PrimaryHoverBrush", "#3558C9");
        SetBrush(app, "DangerBrush", "#D93025");

        if (isDark)
        {
            SetBrush(app, "BgPrimaryBrush", "#16181D");
            SetBrush(app, "BgSurfaceBrush", "#20232A");
            SetBrush(app, "BgElevatedBrush", "#2B2F3A");
            SetBrush(app, "TextPrimaryBrush", "#F1F3F4");
            SetBrush(app, "TextSecondaryBrush", "#9AA0A6");
            SetBrush(app, "TranslationBadgeBgBrush", "#E816181D");
            SetBrush(app, "TranslationBadgeTextBrush", "#FFFFFF");
        }
        else
        {
            SetBrush(app, "BgPrimaryBrush", "#F8F9FA");
            SetBrush(app, "BgSurfaceBrush", "#FFFFFF");
            SetBrush(app, "BgElevatedBrush", "#EEF1F6");
            SetBrush(app, "TextPrimaryBrush", "#202124");
            SetBrush(app, "TextSecondaryBrush", "#5F6368");
            SetBrush(app, "TranslationBadgeBgBrush", "#F2FFFFFF");
            SetBrush(app, "TranslationBadgeTextBrush", "#202124");
        }
    }

    private static void SetBrush(System.Windows.Application app, string key, string hex)
    {
        var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        app.Resources[key] = brush;
    }

    public static System.Windows.Shapes.Path CreateIcon(string pathData, double size = 16, System.Windows.Media.Brush? fill = null)
    {
        return new System.Windows.Shapes.Path
        {
            Data = Geometry.Parse(pathData),
            Stretch = Stretch.Uniform,
            Width = size,
            Height = size,
            Fill = fill ?? (System.Windows.Media.Brush)System.Windows.Application.Current.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center
        };
    }
}
