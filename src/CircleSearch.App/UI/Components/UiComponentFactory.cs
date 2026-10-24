using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OkGoogle.UI.Components;

public static class UiComponentFactory
{
    public static System.Windows.Controls.Button CreateActionButton(
        string? text,
        string? iconPath = null,
        bool isPrimary = false,
        RoutedEventHandler? onClick = null,
        string? toolTip = null)
    {
        var app = System.Windows.Application.Current;
        var bg = isPrimary
            ? (System.Windows.Media.Brush)app.Resources["PrimaryBrush"]
            : (System.Windows.Media.Brush)app.Resources["BgElevatedBrush"];

        var fg = isPrimary
            ? System.Windows.Media.Brushes.White
            : (System.Windows.Media.Brush)app.Resources["TextPrimaryBrush"];

        var panel = new StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center
        };

        if (!string.IsNullOrEmpty(iconPath))
        {
            var icon = ThemeManager.CreateIcon(iconPath, 15, fg);
            if (!string.IsNullOrEmpty(text))
                icon.Margin = new Thickness(0, 0, 6, 0);
            panel.Children.Add(icon);
        }

        if (!string.IsNullOrEmpty(text))
        {
            panel.Children.Add(new TextBlock
            {
                Text = text,
                Foreground = fg,
                FontSize = 13,
                FontWeight = FontWeights.Medium,
                VerticalAlignment = VerticalAlignment.Center
            });
        }

        var border = new Border
        {
            Background = bg,
            CornerRadius = new CornerRadius(18),
            Padding = string.IsNullOrEmpty(text) ? new Thickness(9) : new Thickness(13, 7, 13, 7),
            Child = panel
        };

        var btn = new System.Windows.Controls.Button
        {
            Content = border,
            Background = System.Windows.Media.Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Cursor = System.Windows.Input.Cursors.Hand,
            Padding = new Thickness(0),
            Margin = new Thickness(3, 0, 3, 0),
            FocusVisualStyle = null
        };

        if (!string.IsNullOrEmpty(toolTip))
            btn.ToolTip = toolTip;

        // Убираем стандартный системный шаблон WPF-кнопки, чтобы не было лишних рамок
        var template = new ControlTemplate(typeof(System.Windows.Controls.Button));
        var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
        template.VisualTree = presenter;
        btn.Template = template;

        btn.MouseEnter += (_, _) =>
        {
            border.Opacity = 0.88;
        };
        btn.MouseLeave += (_, _) =>
        {
            border.Opacity = 1.0;
        };

        if (onClick != null)
            btn.Click += onClick;

        return btn;
    }

    public static Border CreateSurfaceCard(UIElement child, double cornerRadius = 16, Thickness? padding = null)
    {
        var app = System.Windows.Application.Current;
        return new Border
        {
            Background = (System.Windows.Media.Brush)app.Resources["BgSurfaceBrush"],
            CornerRadius = new CornerRadius(cornerRadius),
            Padding = padding ?? new Thickness(14),
            Child = child
        };
    }
}
