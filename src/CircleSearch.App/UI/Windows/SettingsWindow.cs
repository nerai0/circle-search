using System.Windows;
using System.Windows.Controls;
using OkGoogle.Domain;
using OkGoogle.Infrastructure;
using OkGoogle.Services;
using OkGoogle.UI.Components;

namespace OkGoogle.UI.Windows;

public sealed class SettingsWindow : Window
{
    private readonly System.Windows.Controls.ComboBox _monitorModeCombo;
    private readonly System.Windows.Controls.ComboBox _uiLangCombo;
    private readonly System.Windows.Controls.ComboBox _themeCombo;
    private readonly System.Windows.Controls.TextBox _longPressBox;
    private readonly System.Windows.Controls.CheckBox _enableWinHoldCheck;
    private readonly System.Windows.Controls.CheckBox _autoStartCheck;

    public SettingsWindow()
    {
        var app = System.Windows.Application.Current;
        Title = LocalizationService.T("settings.title");
        Width = 460;
        Height = 430;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ResizeMode = ResizeMode.NoResize;
        WindowStyle = WindowStyle.SingleBorderWindow;
        Topmost = true;
        Background = (System.Windows.Media.Brush)app.Resources["BgPrimaryBrush"];

        var root = new Grid
        {
            Margin = new Thickness(20)
        };
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var header = new TextBlock
        {
            Text = LocalizationService.T("settings.title"),
            FontSize = 18,
            FontWeight = FontWeights.SemiBold,
            Foreground = (System.Windows.Media.Brush)app.Resources["TextPrimaryBrush"],
            Margin = new Thickness(0, 0, 0, 14)
        };
        Grid.SetRow(header, 0);
        root.Children.Add(header);

        var formStack = new StackPanel();

        // Target Monitor Mode (Cursor by default vs ActiveWindow)
        formStack.Children.Add(CreateLabel(LocalizationService.T("settings.monitor_mode_label")));
        _monitorModeCombo = CreateCombo(new[]
        {
            ("Cursor", LocalizationService.T("settings.monitor_cursor")),
            ("ActiveWindow", LocalizationService.T("settings.monitor_active_window"))
        }, ConfigManager.Current.TargetMonitorMode);
        formStack.Children.Add(_monitorModeCombo);

        // UI language (all 12 supported languages)
        formStack.Children.Add(CreateLabel(LocalizationService.T("settings.ui_lang_label")));
        _uiLangCombo = CreateCombo(LocalizationService.SupportedLanguages, ConfigManager.Current.UiLanguage);
        formStack.Children.Add(_uiLangCombo);

        // Theme
        formStack.Children.Add(CreateLabel(LocalizationService.T("settings.theme_label")));
        _themeCombo = CreateCombo(new[]
        {
            ("Dark", LocalizationService.T("settings.theme_dark")),
            ("Light", LocalizationService.T("settings.theme_light"))
        }, ConfigManager.Current.Theme);
        formStack.Children.Add(_themeCombo);

        // Win hold duration
        var holdRow = new Grid { Margin = new Thickness(0, 4, 0, 10) };
        holdRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        holdRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });

        _enableWinHoldCheck = new System.Windows.Controls.CheckBox
        {
            Content = LocalizationService.T("settings.enable_win_hold"),
            IsChecked = ConfigManager.Current.EnableWinHold,
            Foreground = (System.Windows.Media.Brush)app.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 13
        };
        Grid.SetColumn(_enableWinHoldCheck, 0);
        holdRow.Children.Add(_enableWinHoldCheck);

        _longPressBox = new System.Windows.Controls.TextBox
        {
            Text = ConfigManager.Current.LongPressMs.ToString(),
            Background = (System.Windows.Media.Brush)app.Resources["BgElevatedBrush"],
            Foreground = (System.Windows.Media.Brush)app.Resources["TextPrimaryBrush"],
            BorderThickness = new Thickness(0),
            Padding = new Thickness(8, 6, 8, 6),
            FontSize = 13,
            ToolTip = LocalizationService.T("settings.long_press_label")
        };
        Grid.SetColumn(_longPressBox, 1);
        holdRow.Children.Add(_longPressBox);
        formStack.Children.Add(holdRow);

        // Windows Startup
        _autoStartCheck = new System.Windows.Controls.CheckBox
        {
            Content = LocalizationService.T("settings.autostart"),
            IsChecked = AutoStartService.IsEnabled(),
            Foreground = (System.Windows.Media.Brush)app.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 13,
            Margin = new Thickness(0, 2, 0, 4)
        };
        formStack.Children.Add(_autoStartCheck);

        var card = UiComponentFactory.CreateSurfaceCard(formStack, 14, new Thickness(16));
        Grid.SetRow(card, 1);
        root.Children.Add(card);

        // Footer buttons
        var footer = new StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
            Margin = new Thickness(0, 14, 0, 0)
        };

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

        footer.Children.Add(cancelBtn);
        footer.Children.Add(saveBtn);
        Grid.SetRow(footer, 2);
        root.Children.Add(footer);

        Content = root;
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
        if (int.TryParse(_longPressBox.Text, out var ms) && ms >= 250 && ms <= 1500)
            ConfigManager.Current.LongPressMs = ms;

        AutoStartService.SetEnabled(_autoStartCheck.IsChecked == true);

        ConfigManager.Save();
        Close();
    }

    private static TextBlock CreateLabel(string text)
    {
        var app = System.Windows.Application.Current;
        return new TextBlock
        {
            Text = text,
            Foreground = (System.Windows.Media.Brush)app.Resources["TextSecondaryBrush"],
            FontSize = 12,
            Margin = new Thickness(0, 0, 0, 4)
        };
    }

    private static System.Windows.Controls.ComboBox CreateCombo((string Key, string Label)[] items, string selectedKey)
    {
        var combo = new System.Windows.Controls.ComboBox
        {
            DisplayMemberPath = "Value",
            SelectedValuePath = "Key",
            Padding = new Thickness(8, 6, 8, 6),
            FontSize = 13,
            Margin = new Thickness(0, 0, 0, 12)
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
