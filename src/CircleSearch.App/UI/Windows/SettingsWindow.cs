using System.Windows;
using CircleSearch.Domain;
using CircleSearch.Infrastructure;
using CircleSearch.Services;
using CircleSearch.UI.Components;
using WpfApp = System.Windows.Application;
using WpfBrush = System.Windows.Media.Brush;
using WpfControls = System.Windows.Controls;

namespace CircleSearch.UI.Windows;

public sealed class SettingsWindow : Window
{
    private const double CardSpacing = 12;

    private readonly WpfControls.ComboBox _monitorModeCombo;
    private readonly WpfControls.ComboBox _uiLangCombo;
    private readonly WpfControls.ComboBox _themeCombo;
    private readonly WpfControls.TextBox _longPressBox;
    private readonly WpfControls.CheckBox _enableWinHoldCheck;
    private readonly WpfControls.CheckBox _autoStartCheck;

    public SettingsWindow()
    {
        var app = WpfApp.Current;
        Title = LocalizationService.T("settings.title");
        UiMetrics.FitWindowToWorkArea(this,
            widthRatio: 0.30, heightRatio: 0.78,
            minWidth: 380, minHeight: 360,
            maxWidth: 560, maxHeight: 760);
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ResizeMode = ResizeMode.CanResize;
        WindowStyle = WindowStyle.SingleBorderWindow;
        Topmost = true;
        Background = (WpfBrush)app.Resources["BgPrimaryBrush"];
        SourceInitialized += (_, _) => ThemeManager.ApplyWindowChrome(this);

        var rootGrid = new WpfControls.Grid();
        rootGrid.RowDefinitions.Add(new WpfControls.RowDefinition { Height = GridLength.Auto });
        rootGrid.RowDefinitions.Add(new WpfControls.RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        rootGrid.RowDefinitions.Add(new WpfControls.RowDefinition { Height = GridLength.Auto });

        var header = BuildHeader();
        WpfControls.Grid.SetRow(header, 0);
        rootGrid.Children.Add(header);

        // Карточка 1: внешний вид
        _uiLangCombo = CreateCombo(BuildLanguageOptions(), ConfigManager.Current.UiLanguage);
        _themeCombo = CreateCombo(new[]
        {
            ("Dark", LocalizationService.T("settings.theme_dark")),
            ("Light", LocalizationService.T("settings.theme_light"))
        }, ConfigManager.Current.Theme);

        var appearanceCard = CreateCard(
            CreateLabel(LocalizationService.T("settings.ui_lang_label")), _uiLangCombo,
            CreateLabel(LocalizationService.T("settings.theme_label")), _themeCombo);

        // Карточка 2: монитор для захвата
        _monitorModeCombo = CreateCombo(new[]
        {
            ("Cursor", LocalizationService.T("settings.monitor_cursor")),
            ("ActiveWindow", LocalizationService.T("settings.monitor_active_window"))
        }, ConfigManager.Current.TargetMonitorMode);

        var monitorCard = CreateCard(
            CreateLabel(LocalizationService.T("settings.monitor_mode_label")), _monitorModeCombo);

        // Карточка 3: запуск
        _enableWinHoldCheck = new WpfControls.CheckBox
        {
            Content = LocalizationService.T("settings.enable_win_hold"),
            IsChecked = ConfigManager.Current.EnableWinHold,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 13
        };

        _longPressBox = new WpfControls.TextBox
        {
            Text = ConfigManager.Current.LongPressMs.ToString(),
            VerticalContentAlignment = VerticalAlignment.Center,
            ToolTip = LocalizationService.T("settings.long_press_label")
        };

        var holdRow = new WpfControls.Grid { Margin = new Thickness(0, 0, 0, 14) };
        holdRow.ColumnDefinitions.Add(new WpfControls.ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        holdRow.ColumnDefinitions.Add(new WpfControls.ColumnDefinition { Width = new GridLength(88) });
        WpfControls.Grid.SetColumn(_enableWinHoldCheck, 0);
        WpfControls.Grid.SetColumn(_longPressBox, 1);
        holdRow.Children.Add(_enableWinHoldCheck);
        holdRow.Children.Add(_longPressBox);

        _autoStartCheck = new WpfControls.CheckBox
        {
            Content = LocalizationService.T("settings.autostart"),
            IsChecked = AutoStartService.IsEnabled(),
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 13,
            Margin = new Thickness(0, 0, 0, 8)
        };

        var launchCard = CreateCard(
            CreateLabel(LocalizationService.T("settings.long_press_label")), holdRow, _autoStartCheck);

        var formStack = new WpfControls.StackPanel();
        formStack.Children.Add(appearanceCard);
        formStack.Children.Add(monitorCard);
        formStack.Children.Add(launchCard);

        var scrollViewer = new WpfControls.ScrollViewer
        {
            VerticalScrollBarVisibility = WpfControls.ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = WpfControls.ScrollBarVisibility.Disabled,
            Padding = new Thickness(24, 8, 16, 8),
            Content = formStack
        };
        WpfControls.Grid.SetRow(scrollViewer, 1);
        rootGrid.Children.Add(scrollViewer);

        var footer = BuildFooter();
        WpfControls.Grid.SetRow(footer, 2);
        rootGrid.Children.Add(footer);

        Content = rootGrid;
    }

    private static (string Key, string Label)[] BuildLanguageOptions()
    {
        var options = new List<(string Key, string Label)>
        {
            ("auto", LocalizationService.T("settings.lang_auto"))
        };
        options.AddRange(LocalizationService.SupportedLanguages.Select(l => (l.Code, l.NativeName)));
        return options.ToArray();
    }

    private static UIElement BuildHeader()
    {
        var app = WpfApp.Current;

        var logo = new WpfControls.Border
        {
            Width = 40,
            Height = 40,
            CornerRadius = new CornerRadius(20),
            Background = (WpfBrush)app.Resources["PrimaryBrush"],
            Child = ThemeManager.CreateIcon(ThemeManager.IconSparkle, 20, System.Windows.Media.Brushes.White)
        };

        var titles = new WpfControls.StackPanel
        {
            Margin = new Thickness(14, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        titles.Children.Add(new WpfControls.TextBlock
        {
            Text = LocalizationService.T("settings.title"),
            FontSize = 18,
            FontWeight = FontWeights.SemiBold,
            Foreground = (WpfBrush)app.Resources["TextPrimaryBrush"],
            TextTrimming = TextTrimming.CharacterEllipsis
        });
        titles.Children.Add(new WpfControls.TextBlock
        {
            Text = LocalizationService.T("app.subtitle"),
            FontSize = 12,
            Foreground = (WpfBrush)app.Resources["TextSecondaryBrush"],
            Margin = new Thickness(0, 2, 0, 0),
            TextTrimming = TextTrimming.CharacterEllipsis
        });

        var row = new WpfControls.DockPanel { Margin = new Thickness(24, 20, 24, 12) };
        WpfControls.DockPanel.SetDock(logo, WpfControls.Dock.Left);
        row.Children.Add(logo);
        row.Children.Add(titles);
        return row;
    }

    private UIElement BuildFooter()
    {
        var cancelBtn = UiComponentFactory.CreateActionButton(
            LocalizationService.T("settings.cancel"),
            null,
            isPrimary: false,
            onClick: (_, _) => Close());

        var saveBtn = UiComponentFactory.CreateActionButton(
            LocalizationService.T("settings.save"),
            null,
            isPrimary: true,
            onClick: OnSaveClick);

        var footer = new WpfControls.StackPanel
        {
            Orientation = WpfControls.Orientation.Horizontal,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
            Margin = new Thickness(24, 8, 24, 18)
        };
        footer.Children.Add(cancelBtn);
        footer.Children.Add(saveBtn);
        return footer;
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (_monitorModeCombo.SelectedValue is string monMode)
            ConfigManager.Current.TargetMonitorMode = monMode;

        if (_uiLangCombo.SelectedValue is string uiLang)
        {
            ConfigManager.Current.UiLanguage = uiLang;
            LocalizationService.CurrentLanguage = uiLang.Equals("auto", StringComparison.OrdinalIgnoreCase)
                ? LocalizationService.DetectSystemLanguage()
                : uiLang;
        }

        if (_themeCombo.SelectedValue is string theme)
        {
            ConfigManager.Current.Theme = theme;
            ThemeManager.ApplyTheme(theme);
        }

        ConfigManager.Current.EnableWinHold = _enableWinHoldCheck.IsChecked == true;
        if (int.TryParse(_longPressBox.Text, out var ms) && ms >= 200 && ms <= 2000)
            ConfigManager.Current.LongPressMs = ms;

        AutoStartService.SetEnabled(_autoStartCheck.IsChecked == true);

        ConfigManager.Save();
        Close();
    }

    private static WpfControls.Border CreateCard(params UIElement[] children)
    {
        var stack = new WpfControls.StackPanel();
        foreach (var child in children)
            stack.Children.Add(child);

        var card = UiComponentFactory.CreateSurfaceCard(stack, 14, new Thickness(16, 14, 16, 4));
        card.Margin = new Thickness(0, 0, 0, CardSpacing);
        return card;
    }

    private static WpfControls.TextBlock CreateLabel(string text)
    {
        var app = WpfApp.Current;
        return new WpfControls.TextBlock
        {
            Text = text,
            Foreground = (WpfBrush)app.Resources["TextSecondaryBrush"],
            FontSize = 12,
            FontWeight = FontWeights.Medium,
            Margin = new Thickness(0, 0, 0, 6),
            TextWrapping = TextWrapping.Wrap
        };
    }

    private static WpfControls.ComboBox CreateCombo((string Key, string Label)[] items, string selectedKey)
    {
        var combo = new WpfControls.ComboBox
        {
            DisplayMemberPath = "Value",
            SelectedValuePath = "Key",
            Margin = new Thickness(0, 0, 0, 14)
        };

        foreach (var item in items)
            combo.Items.Add(new KeyValuePair<string, string>(item.Key, item.Label));

        combo.SelectedValue = selectedKey;
        if (combo.SelectedIndex < 0 && combo.Items.Count > 0)
            combo.SelectedIndex = 0;

        return combo;
    }
}
