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
        Width = 490;
        Height = 540;
        MinWidth = 420;
        MinHeight = 440;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ResizeMode = ResizeMode.CanResizeWithGrip;
        WindowStyle = WindowStyle.SingleBorderWindow;
        Topmost = true;
        Background = (WpfBrush)app.Resources["BgPrimaryBrush"];

        var rootGrid = new WpfControls.Grid
        {
            Margin = new Thickness(0)
        };
        rootGrid.RowDefinitions.Add(new WpfControls.RowDefinition { Height = GridLength.Auto });
        rootGrid.RowDefinitions.Add(new WpfControls.RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        rootGrid.RowDefinitions.Add(new WpfControls.RowDefinition { Height = GridLength.Auto });

        // Header
        var headerBorder = new WpfControls.Border
        {
            Padding = new Thickness(24, 20, 24, 16),
            BorderThickness = new Thickness(0, 0, 0, 1),
            BorderBrush = (WpfBrush)app.Resources["BorderSubtleBrush"]
        };

        var headerStack = new WpfControls.StackPanel();
        var headerTitle = new WpfControls.TextBlock
        {
            Text = LocalizationService.T("settings.title"),
            FontSize = 18,
            FontWeight = FontWeights.SemiBold,
            Foreground = (WpfBrush)app.Resources["TextPrimaryBrush"]
        };
        var headerSubtitle = new WpfControls.TextBlock
        {
            Text = LocalizationService.T("app.subtitle"),
            FontSize = 12,
            Foreground = (WpfBrush)app.Resources["TextSecondaryBrush"],
            Margin = new Thickness(0, 2, 0, 0)
        };
        headerStack.Children.Add(headerTitle);
        headerStack.Children.Add(headerSubtitle);
        headerBorder.Child = headerStack;

        WpfControls.Grid.SetRow(headerBorder, 0);
        rootGrid.Children.Add(headerBorder);

        // Content wrapped in ScrollViewer
        var scrollViewer = new WpfControls.ScrollViewer
        {
            VerticalScrollBarVisibility = WpfControls.ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = WpfControls.ScrollBarVisibility.Disabled,
            Padding = new Thickness(24, 16, 24, 16)
        };

        var formStack = new WpfControls.StackPanel();

        // 1. Language selector
        formStack.Children.Add(CreateLabel(LocalizationService.T("settings.ui_lang_label")));
        _uiLangCombo = CreateCombo(LocalizationService.SupportedLanguages, ConfigManager.Current.UiLanguage);
        formStack.Children.Add(_uiLangCombo);

        // 2. Target Monitor Mode
        formStack.Children.Add(CreateLabel(LocalizationService.T("settings.monitor_mode_label")));
        _monitorModeCombo = CreateCombo(new[]
        {
            ("Cursor", LocalizationService.T("settings.monitor_cursor")),
            ("ActiveWindow", LocalizationService.T("settings.monitor_active_window"))
        }, ConfigManager.Current.TargetMonitorMode);
        formStack.Children.Add(_monitorModeCombo);

        // 3. Theme selector
        formStack.Children.Add(CreateLabel(LocalizationService.T("settings.theme_label")));
        _themeCombo = CreateCombo(new[]
        {
            ("Dark", LocalizationService.T("settings.theme_dark")),
            ("Light", LocalizationService.T("settings.theme_light"))
        }, ConfigManager.Current.Theme);
        formStack.Children.Add(_themeCombo);

        // 4. Hotkey / Long press
        formStack.Children.Add(CreateLabel(LocalizationService.T("settings.long_press_label")));
        var holdRow = new WpfControls.Grid { Margin = new Thickness(0, 0, 0, 16) };
        holdRow.ColumnDefinitions.Add(new WpfControls.ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        holdRow.ColumnDefinitions.Add(new WpfControls.ColumnDefinition { Width = new GridLength(100) });

        _enableWinHoldCheck = new WpfControls.CheckBox
        {
            Content = LocalizationService.T("settings.enable_win_hold"),
            IsChecked = ConfigManager.Current.EnableWinHold,
            Foreground = (WpfBrush)app.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 13
        };
        WpfControls.Grid.SetColumn(_enableWinHoldCheck, 0);
        holdRow.Children.Add(_enableWinHoldCheck);

        _longPressBox = new WpfControls.TextBox
        {
            Text = ConfigManager.Current.LongPressMs.ToString(),
            Background = (WpfBrush)app.Resources["BgElevatedBrush"],
            Foreground = (WpfBrush)app.Resources["TextPrimaryBrush"],
            BorderThickness = new Thickness(1),
            BorderBrush = (WpfBrush)app.Resources["BorderSubtleBrush"],
            Padding = new Thickness(8, 6, 8, 6),
            FontSize = 13,
            VerticalContentAlignment = VerticalAlignment.Center,
            ToolTip = LocalizationService.T("settings.long_press_label")
        };
        WpfControls.Grid.SetColumn(_longPressBox, 1);
        holdRow.Children.Add(_longPressBox);
        formStack.Children.Add(holdRow);

        // 5. Windows Autostart
        _autoStartCheck = new WpfControls.CheckBox
        {
            Content = LocalizationService.T("settings.autostart"),
            IsChecked = AutoStartService.IsEnabled(),
            Foreground = (WpfBrush)app.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 13,
            Margin = new Thickness(0, 0, 0, 8)
        };
        formStack.Children.Add(_autoStartCheck);

        scrollViewer.Content = formStack;
        WpfControls.Grid.SetRow(scrollViewer, 1);
        rootGrid.Children.Add(scrollViewer);

        // Footer
        var footerBorder = new WpfControls.Border
        {
            Padding = new Thickness(24, 14, 24, 16),
            BorderThickness = new Thickness(0, 1, 0, 0),
            BorderBrush = (WpfBrush)app.Resources["BorderSubtleBrush"],
            Background = (WpfBrush)app.Resources["BgSecondaryBrush"]
        };

        var footer = new WpfControls.StackPanel
        {
            Orientation = WpfControls.Orientation.Horizontal,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Right
        };

        var cancelBtn = UiComponentFactory.CreateActionButton(
            LocalizationService.T("settings.cancel"),
            null,
            isPrimary: false,
            onClick: (_, _) => Close());
        cancelBtn.Margin = new Thickness(0, 0, 10, 0);

        var saveBtn = UiComponentFactory.CreateActionButton(
            LocalizationService.T("settings.save"),
            null,
            isPrimary: true,
            onClick: OnSaveClick);

        footer.Children.Add(cancelBtn);
        footer.Children.Add(saveBtn);
        footerBorder.Child = footer;

        WpfControls.Grid.SetRow(footerBorder, 2);
        rootGrid.Children.Add(footerBorder);

        Content = rootGrid;
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (_monitorModeCombo.SelectedValue is string monMode)
            ConfigManager.Current.TargetMonitorMode = monMode;

        if (_uiLangCombo.SelectedValue is string uiLang)
        {
            ConfigManager.Current.UiLanguage = uiLang;
            LocalizationService.CurrentLanguage = uiLang;
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

    private static WpfControls.TextBlock CreateLabel(string text)
    {
        var app = WpfApp.Current;
        return new WpfControls.TextBlock
        {
            Text = text,
            Foreground = (WpfBrush)app.Resources["TextSecondaryBrush"],
            FontSize = 12,
            FontWeight = FontWeights.Medium,
            Margin = new Thickness(0, 0, 0, 6)
        };
    }

    private static WpfControls.ComboBox CreateCombo((string Key, string Label)[] items, string selectedKey)
    {
        var combo = new WpfControls.ComboBox
        {
            DisplayMemberPath = "Value",
            SelectedValuePath = "Key",
            Padding = new Thickness(10, 8, 10, 8),
            FontSize = 13,
            Margin = new Thickness(0, 0, 0, 16)
        };

        foreach (var item in items)
        {
            combo.Items.Add(new KeyValuePair<string, string>(item.Key, item.Label));
        }

        combo.SelectedValue = selectedKey;
        if (combo.SelectedIndex < 0 && combo.Items.Count > 0)
            combo.SelectedIndex = 0;

        return combo;
    }
}
