using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using CircleSearch.Domain;
using CircleSearch.Infrastructure;
using CircleSearch.Services;
using CircleSearch.UI.Components;

namespace CircleSearch.UI.Windows;

public sealed class OverlayWindow : Window
{
    private enum DragOperation
    {
        None,
        NewSelection,
        Move,
        ResizeTopLeft,
        ResizeTop,
        ResizeTopRight,
        ResizeRight,
        ResizeBottomRight,
        ResizeBottom,
        ResizeBottomLeft,
        ResizeLeft
    }

    private readonly CapturedScreenContext _context;

    private readonly Canvas _interactiveCanvas;
    private readonly Grid _selectionContainer;

    private Border _lensDrawerCard = null!;
    private WebView2 _lensWebView = null!;
    private bool _webViewInitialized;
    private Task? _webViewInitTask;
    private bool _isDrawerExpanded;

    private DragOperation _currentDragOp = DragOperation.None;
    private System.Windows.Point _dragStartMouseDip;
    private Rect _dragStartSelectionDip = Rect.Empty;
    private Rect _currentSelectionDip = Rect.Empty;
    private Rectangle _currentSelectionPx = Rectangle.Empty;

    private byte[]? _selectedRegionPngBytes;
    private byte[]? _lastUploadedPngBytes;

    private double _dpiScaleX = 1.0;
    private double _dpiScaleY = 1.0;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private static readonly IntPtr HWND_TOPMOST = new(-1);
    private const uint SWP_SHOWWINDOW = 0x0040;
    private const uint SWP_NOACTIVATE = 0x0010;

    public event Action? RequestOpenSettings;

    public OverlayWindow(CapturedScreenContext context)
    {
        _context = context;
        var app = System.Windows.Application.Current;
        var primaryBrush = (System.Windows.Media.Brush)app.Resources["PrimaryBrush"];

        Title = LocalizationService.T("app.title");
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Topmost = true;
        ShowInTaskbar = false;
        ResizeMode = ResizeMode.NoResize;
        WindowStartupLocation = WindowStartupLocation.Manual;
        Cursor = System.Windows.Input.Cursors.Cross;
        Background = System.Windows.Media.Brushes.Black;

        SourceInitialized += OnOverlaySourceInitialized;
        Loaded += OnOverlayLoaded;
        Closed += OnOverlayClosed;
        PreviewKeyDown += OnOverlayPreviewKeyDown;

        var rootGrid = new Grid();

        // 1. Замороженный скриншот экрана
        var screenImg = new System.Windows.Controls.Image
        {
            Source = _context.FullScreenWpfSource,
            Stretch = Stretch.Fill,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        rootGrid.Children.Add(screenImg);

        // 2. Мягкое затемнение фона
        var dimLayer = new Border
        {
            Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(65, 10, 12, 18)),
            IsHitTestVisible = false
        };
        rootGrid.Children.Add(dimLayer);

        // 3. Аккуратная статичная рамка по краям экрана
        var frameBorder = new Border
        {
            BorderBrush = primaryBrush,
            BorderThickness = new Thickness(3),
            IsHitTestVisible = false,
            Opacity = 0.85
        };
        rootGrid.Children.Add(frameBorder);

        // 4. Интерактивный холст для выделения и изменения размера области
        _interactiveCanvas = new Canvas
        {
            Background = System.Windows.Media.Brushes.Transparent
        };
        _interactiveCanvas.MouseLeftButtonDown += OnCanvasMouseDown;
        _interactiveCanvas.MouseMove += OnCanvasMouseMove;
        _interactiveCanvas.MouseLeftButtonUp += OnCanvasMouseUp;

        _selectionContainer = BuildResizableSelectionBox(primaryBrush);
        _selectionContainer.Visibility = Visibility.Collapsed;
        _interactiveCanvas.Children.Add(_selectionContainer);

        rootGrid.Children.Add(_interactiveCanvas);

        // 5. Верхняя подсказка и кнопки настроек/выхода
        var topBar = BuildTopBar();
        rootGrid.Children.Add(topBar);

        // 6. Встроенный широкий браузер Google Объектив
        var lensDrawer = BuildEmbeddedLensDrawer();
        rootGrid.Children.Add(lensDrawer);

        // 7. Компактная нижняя панель выбора источника (Активное окно / Весь экран)
        var bottomDock = BuildBottomDock();
        rootGrid.Children.Add(bottomDock);

        Content = rootGrid;
    }

    private Grid BuildResizableSelectionBox(System.Windows.Media.Brush primaryBrush)
    {
        var container = new Grid();

        // Внутренняя область выделения (позволяет перетаскивать всё выделение целиком)
        var body = new Border
        {
            BorderBrush = primaryBrush,
            BorderThickness = new Thickness(2),
            Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(30, 65, 105, 225)),
            CornerRadius = new CornerRadius(8),
            Cursor = System.Windows.Input.Cursors.SizeAll,
            Tag = DragOperation.Move
        };
        container.Children.Add(body);

        // 4 невидимые полосы вдоль граней для растягивания сторон
        container.Children.Add(CreateEdgeResizeGrip(System.Windows.HorizontalAlignment.Stretch, VerticalAlignment.Top, double.NaN, 10, System.Windows.Input.Cursors.SizeNS, DragOperation.ResizeTop));
        container.Children.Add(CreateEdgeResizeGrip(System.Windows.HorizontalAlignment.Stretch, VerticalAlignment.Bottom, double.NaN, 10, System.Windows.Input.Cursors.SizeNS, DragOperation.ResizeBottom));
        container.Children.Add(CreateEdgeResizeGrip(System.Windows.HorizontalAlignment.Left, VerticalAlignment.Stretch, 10, double.NaN, System.Windows.Input.Cursors.SizeWE, DragOperation.ResizeLeft));
        container.Children.Add(CreateEdgeResizeGrip(System.Windows.HorizontalAlignment.Right, VerticalAlignment.Stretch, 10, double.NaN, System.Windows.Input.Cursors.SizeWE, DragOperation.ResizeRight));

        // 4 угловых маркера (белые кружки с синей обводкой) +4 маркера по центрам сторон
        container.Children.Add(CreateCornerHandle(System.Windows.HorizontalAlignment.Left, VerticalAlignment.Top, System.Windows.Input.Cursors.SizeNWSE, DragOperation.ResizeTopLeft, primaryBrush));
        container.Children.Add(CreateCornerHandle(System.Windows.HorizontalAlignment.Right, VerticalAlignment.Top, System.Windows.Input.Cursors.SizeNESW, DragOperation.ResizeTopRight, primaryBrush));
        container.Children.Add(CreateCornerHandle(System.Windows.HorizontalAlignment.Right, VerticalAlignment.Bottom, System.Windows.Input.Cursors.SizeNWSE, DragOperation.ResizeBottomRight, primaryBrush));
        container.Children.Add(CreateCornerHandle(System.Windows.HorizontalAlignment.Left, VerticalAlignment.Bottom, System.Windows.Input.Cursors.SizeNESW, DragOperation.ResizeBottomLeft, primaryBrush));

        container.Children.Add(CreateCornerHandle(System.Windows.HorizontalAlignment.Center, VerticalAlignment.Top, System.Windows.Input.Cursors.SizeNS, DragOperation.ResizeTop, primaryBrush));
        container.Children.Add(CreateCornerHandle(System.Windows.HorizontalAlignment.Center, VerticalAlignment.Bottom, System.Windows.Input.Cursors.SizeNS, DragOperation.ResizeBottom, primaryBrush));
        container.Children.Add(CreateCornerHandle(System.Windows.HorizontalAlignment.Left, VerticalAlignment.Center, System.Windows.Input.Cursors.SizeWE, DragOperation.ResizeLeft, primaryBrush));
        container.Children.Add(CreateCornerHandle(System.Windows.HorizontalAlignment.Right, VerticalAlignment.Center, System.Windows.Input.Cursors.SizeWE, DragOperation.ResizeRight, primaryBrush));

        return container;
    }

    private static FrameworkElement CreateEdgeResizeGrip(
        System.Windows.HorizontalAlignment hAlign,
        VerticalAlignment vAlign,
        double width,
        double height,
        System.Windows.Input.Cursor cursor,
        DragOperation op)
    {
        return new Border
        {
            HorizontalAlignment = hAlign,
            VerticalAlignment = vAlign,
            Width = width,
            Height = height,
            Background = System.Windows.Media.Brushes.Transparent,
            Cursor = cursor,
            Tag = op
        };
    }

    private static FrameworkElement CreateCornerHandle(
        System.Windows.HorizontalAlignment hAlign,
        VerticalAlignment vAlign,
        System.Windows.Input.Cursor cursor,
        DragOperation op,
        System.Windows.Media.Brush primaryBrush)
    {
        double marginL = hAlign == System.Windows.HorizontalAlignment.Left ? -6 : 0;
        double marginR = hAlign == System.Windows.HorizontalAlignment.Right ? -6 : 0;
        double marginT = vAlign == VerticalAlignment.Top ? -6 : 0;
        double marginB = vAlign == VerticalAlignment.Bottom ? -6 : 0;

        return new Border
        {
            Width = 12,
            Height = 12,
            CornerRadius = new CornerRadius(6),
            Background = System.Windows.Media.Brushes.White,
            BorderBrush = primaryBrush,
            BorderThickness = new Thickness(2.5),
            HorizontalAlignment = hAlign,
            VerticalAlignment = vAlign,
            Margin = new Thickness(marginL, marginT, marginR, marginB),
            Cursor = cursor,
            Tag = op
        };
    }

    private void EnforceTargetMonitorBounds()
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        if (hwnd != IntPtr.Zero)
        {
            var b = _context.ScreenBounds;
            // Вызываем дважды: первый вызов переносит окно на второй монитор (после чего Windows шлёт WM_DPICHANGED),
            // а второй вызов жёстко фиксирует 100% физического размера монитора, чтобы WPF не сжимал окно!
            SetWindowPos(hwnd, HWND_TOPMOST, b.Left, b.Top, b.Width, b.Height, SWP_SHOWWINDOW);
            SetWindowPos(hwnd, HWND_TOPMOST, b.Left, b.Top, b.Width, b.Height, SWP_SHOWWINDOW);
        }

        if (ActualWidth > 0 && ActualHeight > 0)
        {
            _dpiScaleX = _context.FullScreenBitmap.Width / ActualWidth;
            _dpiScaleY = _context.FullScreenBitmap.Height / ActualHeight;
        }
    }

    private void OnOverlaySourceInitialized(object? sender, EventArgs e)
    {
        EnforceTargetMonitorBounds();
    }

    protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
    {
        base.OnDpiChanged(oldDpi, newDpi);
        EnforceTargetMonitorBounds();
        UpdateDrawerSize();
    }

    private async void OnOverlayLoaded(object sender, RoutedEventArgs e)
    {
        EnforceTargetMonitorBounds();
        UpdateDrawerSize();
        Activate();

        // Прогреваем WebView2 сразу, чтобы выделение отправлялось мгновенно
        await EnsureWebViewReadyAsync();
    }

    private void UpdateDrawerSize()
    {
        double screenW = ActualWidth > 0 ? ActualWidth : SystemParameters.PrimaryScreenWidth;
        double targetRatio = _isDrawerExpanded ? 0.68 : 0.48;
        double targetW = Math.Clamp(screenW * targetRatio, 760, Math.Max(780, screenW - 60));
        _lensDrawerCard.Width = targetW;
    }

    private void OnOverlayClosed(object? sender, EventArgs e)
    {
        try
        {
            _lensWebView?.Dispose();
        }
        catch { }
    }

    private void OnOverlayPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            Close();
        }
    }

    private UIElement BuildTopBar()
    {
        var app = System.Windows.Application.Current;
        var container = new Grid
        {
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(24, 16, 24, 0)
        };
        container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var hintBadge = new Border
        {
            Background = (System.Windows.Media.Brush)app.Resources["BgSurfaceBrush"],
            CornerRadius = new CornerRadius(18),
            Padding = new Thickness(16, 8, 16, 8),
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left
        };

        var hintRow = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal };
        hintRow.Children.Add(ThemeManager.CreateIcon(ThemeManager.IconSparkle, 15, (System.Windows.Media.Brush)app.Resources["PrimaryBrush"]));
        hintRow.Children.Add(new TextBlock
        {
            Text = LocalizationService.T("overlay.hint_drag"),
            Foreground = (System.Windows.Media.Brush)app.Resources["TextPrimaryBrush"],
            FontSize = 13,
            Margin = new Thickness(8, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        });
        hintBadge.Child = hintRow;
        Grid.SetColumn(hintBadge, 0);
        container.Children.Add(hintBadge);

        var rightControls = new StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Right
        };

        var settingsBtn = UiComponentFactory.CreateActionButton(
            null,
            ThemeManager.IconSettings,
            isPrimary: false,
            onClick: (_, _) =>
            {
                Close();
                RequestOpenSettings?.Invoke();
            },
            toolTip: LocalizationService.T("overlay.btn_settings"));

        var closeBtn = UiComponentFactory.CreateActionButton(
            null,
            ThemeManager.IconClose,
            isPrimary: false,
            onClick: (_, _) => Close(),
            toolTip: LocalizationService.T("overlay.btn_close"));

        rightControls.Children.Add(settingsBtn);
        rightControls.Children.Add(closeBtn);
        Grid.SetColumn(rightControls, 1);
        container.Children.Add(rightControls);

        return container;
    }

    private UIElement BuildEmbeddedLensDrawer()
    {
        var app = System.Windows.Application.Current;

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var header = new Grid
        {
            Margin = new Thickness(6, 2, 4, 8)
        };
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var titleStack = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal };
        titleStack.Children.Add(ThemeManager.CreateIcon(ThemeManager.IconSearch, 15, (System.Windows.Media.Brush)app.Resources["PrimaryBrush"]));
        titleStack.Children.Add(new TextBlock
        {
            Text = LocalizationService.T("overlay.lens_panel_title"),
            Foreground = (System.Windows.Media.Brush)app.Resources["TextPrimaryBrush"],
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(8, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        });
        Grid.SetColumn(titleStack, 0);
        header.Children.Add(titleStack);

        var headerBtns = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal };

        var expandBtn = UiComponentFactory.CreateActionButton(
            LocalizationService.T("overlay.lens_expand"),
            ThemeManager.IconWindow,
            isPrimary: false,
            onClick: (_, _) =>
            {
                _isDrawerExpanded = !_isDrawerExpanded;
                UpdateDrawerSize();
            });

        var openExternalBtn = UiComponentFactory.CreateActionButton(
            LocalizationService.T("overlay.lens_open_external"),
            null,
            isPrimary: false,
            onClick: async (_, _) =>
            {
                var bytes = _lastUploadedPngBytes ?? _selectedRegionPngBytes ?? _context.FullScreenPngBytes;
                Close();
                await GoogleLensService.OpenInExternalBrowserAsync(bytes);
            });

        var closeDrawerBtn = UiComponentFactory.CreateActionButton(
            null,
            ThemeManager.IconClose,
            isPrimary: false,
            onClick: (_, _) =>
            {
                _lensDrawerCard.Visibility = Visibility.Collapsed;
                ClearRegionSelectionVisuals();
            },
            toolTip: LocalizationService.T("overlay.lens_close_panel"));

        headerBtns.Children.Add(expandBtn);
        headerBtns.Children.Add(openExternalBtn);
        headerBtns.Children.Add(closeDrawerBtn);
        Grid.SetColumn(headerBtns, 1);
        header.Children.Add(headerBtns);

        Grid.SetRow(header, 0);
        grid.Children.Add(header);

        _lensWebView = new WebView2
        {
            DefaultBackgroundColor = System.Drawing.Color.FromArgb(255, 22, 24, 29)
        };
        Grid.SetRow(_lensWebView, 1);
        grid.Children.Add(_lensWebView);

        _lensDrawerCard = UiComponentFactory.CreateSurfaceCard(grid, 18, new Thickness(12, 10, 12, 12));
        _lensDrawerCard.Width = 820;
        _lensDrawerCard.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
        _lensDrawerCard.VerticalAlignment = VerticalAlignment.Stretch;
        _lensDrawerCard.Margin = new Thickness(0, 62, 20, 96);
        _lensDrawerCard.Visibility = Visibility.Collapsed;
        _lensDrawerCard.Cursor = System.Windows.Input.Cursors.Arrow;

        return _lensDrawerCard;
    }

    private UIElement BuildBottomDock()
    {
        var dockStack = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 22),
            Cursor = System.Windows.Input.Cursors.Arrow
        };

        // Компактный док только из 2 кнопок выбора экрана: Активное окно и Весь экран
        var buttonsRow = new StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center
        };

        var winTitleShort = _context.ActiveWindowTitle.Length > 30
            ? _context.ActiveWindowTitle[..30] + "..."
            : _context.ActiveWindowTitle;

        var attachWinBtn = UiComponentFactory.CreateActionButton(
            LocalizationService.Tf("overlay.btn_attach_window", winTitleShort),
            ThemeManager.IconWindow,
            isPrimary: true,
            onClick: async (_, _) =>
            {
                ClearRegionSelectionVisuals();
                await UploadBytesToEmbeddedLensAsync(_context.ActiveWindowPngBytes ?? _context.FullScreenPngBytes);
            });

        var attachScreenBtn = UiComponentFactory.CreateActionButton(
            LocalizationService.T("overlay.btn_attach_screen"),
            ThemeManager.IconScreen,
            isPrimary: false,
            onClick: async (_, _) =>
            {
                ClearRegionSelectionVisuals();
                await UploadBytesToEmbeddedLensAsync(_context.FullScreenPngBytes);
            });

        buttonsRow.Children.Add(attachWinBtn);
        buttonsRow.Children.Add(attachScreenBtn);

        var mainBarCard = UiComponentFactory.CreateSurfaceCard(buttonsRow, 24, new Thickness(14, 10, 14, 10));
        dockStack.Children.Add(mainBarCard);

        return dockStack;
    }

    private void ClearRegionSelectionVisuals()
    {
        _selectionContainer.Visibility = Visibility.Collapsed;
        _selectedRegionPngBytes = null;
        _currentSelectionDip = Rect.Empty;
        _currentSelectionPx = Rectangle.Empty;
    }

    private void ApplySelectionRectDip(Rect rect)
    {
        _currentSelectionDip = rect;
        Canvas.SetLeft(_selectionContainer, rect.X);
        Canvas.SetTop(_selectionContainer, rect.Y);
        _selectionContainer.Width = rect.Width;
        _selectionContainer.Height = rect.Height;
    }

    // === Выделение области, перемещение и изменение размера за края/углы ===

    private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
    {
        _dragStartMouseDip = e.GetPosition(_interactiveCanvas);

        // Проверяем, кликнул ли пользователь по углу/краю/телу уже существующего выделения
        if (_selectionContainer.Visibility == Visibility.Visible &&
            e.OriginalSource is FrameworkElement fe &&
            fe.Tag is DragOperation op &&
            op != DragOperation.None)
        {
            _currentDragOp = op;
            _dragStartSelectionDip = _currentSelectionDip;
        }
        else
        {
            // Начинаем новое выделение
            _currentDragOp = DragOperation.NewSelection;
            ApplySelectionRectDip(new Rect(_dragStartMouseDip.X, _dragStartMouseDip.Y, 0, 0));
            _selectionContainer.Visibility = Visibility.Visible;
        }

        _interactiveCanvas.CaptureMouse();
        e.Handled = true;
    }

    private void OnCanvasMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (_currentDragOp == DragOperation.None) return;

        var pos = e.GetPosition(_interactiveCanvas);
        double dx = pos.X - _dragStartMouseDip.X;
        double dy = pos.Y - _dragStartMouseDip.Y;

        const double minSize = 24;
        double maxW = ActualWidth > 0 ? ActualWidth : SystemParameters.PrimaryScreenWidth;
        double maxH = ActualHeight > 0 ? ActualHeight : SystemParameters.PrimaryScreenHeight;

        if (_currentDragOp == DragOperation.NewSelection)
        {
            var x = Math.Min(_dragStartMouseDip.X, pos.X);
            var y = Math.Min(_dragStartMouseDip.Y, pos.Y);
            var w = Math.Abs(pos.X - _dragStartMouseDip.X);
            var h = Math.Abs(pos.Y - _dragStartMouseDip.Y);
            ApplySelectionRectDip(new Rect(x, y, w, h));
            return;
        }

        if (_currentDragOp == DragOperation.Move)
        {
            double newX = Math.Clamp(_dragStartSelectionDip.X + dx, 0, Math.Max(0, maxW - _dragStartSelectionDip.Width));
            double newY = Math.Clamp(_dragStartSelectionDip.Y + dy, 0, Math.Max(0, maxH - _dragStartSelectionDip.Height));
            ApplySelectionRectDip(new Rect(newX, newY, _dragStartSelectionDip.Width, _dragStartSelectionDip.Height));
            return;
        }

        double left = _dragStartSelectionDip.Left;
        double top = _dragStartSelectionDip.Top;
        double right = _dragStartSelectionDip.Right;
        double bottom = _dragStartSelectionDip.Bottom;

        if (_currentDragOp is DragOperation.ResizeTopLeft or DragOperation.ResizeLeft or DragOperation.ResizeBottomLeft)
        {
            left = Math.Clamp(left + dx, 0, right - minSize);
        }
        if (_currentDragOp is DragOperation.ResizeTopRight or DragOperation.ResizeRight or DragOperation.ResizeBottomRight)
        {
            right = Math.Clamp(right + dx, left + minSize, maxW);
        }
        if (_currentDragOp is DragOperation.ResizeTopLeft or DragOperation.ResizeTop or DragOperation.ResizeTopRight)
        {
            top = Math.Clamp(top + dy, 0, bottom - minSize);
        }
        if (_currentDragOp is DragOperation.ResizeBottomLeft or DragOperation.ResizeBottom or DragOperation.ResizeBottomRight)
        {
            bottom = Math.Clamp(bottom + dy, top + minSize, maxH);
        }

        ApplySelectionRectDip(new Rect(new System.Windows.Point(left, top), new System.Windows.Point(right, bottom)));
    }

    private async void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_currentDragOp == DragOperation.None) return;

        var finishedOp = _currentDragOp;
        _currentDragOp = DragOperation.None;
        _interactiveCanvas.ReleaseMouseCapture();

        if (finishedOp == DragOperation.NewSelection &&
            (_currentSelectionDip.Width < 18 || _currentSelectionDip.Height < 18))
        {
            ClearRegionSelectionVisuals();
            return;
        }

        if (ActualWidth > 0 && ActualHeight > 0)
        {
            _dpiScaleX = _context.FullScreenBitmap.Width / ActualWidth;
            _dpiScaleY = _context.FullScreenBitmap.Height / ActualHeight;
        }

        _currentSelectionPx = new Rectangle(
            (int)Math.Round(_currentSelectionDip.X * _dpiScaleX),
            (int)Math.Round(_currentSelectionDip.Y * _dpiScaleY),
            (int)Math.Round(_currentSelectionDip.Width * _dpiScaleX),
            (int)Math.Round(_currentSelectionDip.Height * _dpiScaleY));

        _selectedRegionPngBytes = ScreenCaptureService.CropBitmapToPng(_context.FullScreenBitmap, _currentSelectionPx);

        // Сразу после выделения или изменения размера автоматически отправляем обновлённую область в Google Объектив!
        await UploadBytesToEmbeddedLensAsync(_selectedRegionPngBytes);
    }

    // === Встроенный браузер WebView2 ===

    private Task EnsureWebViewReadyAsync()
    {
        if (_webViewInitialized) return Task.CompletedTask;
        if (_webViewInitTask != null) return _webViewInitTask;

        _webViewInitTask = InitializeWebViewInternalAsync();
        return _webViewInitTask;
    }

    private async Task InitializeWebViewInternalAsync()
    {
        var userDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CircleSearch",
            "WebView2");
        var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
        await _lensWebView.EnsureCoreWebView2Async(env);

        _lensWebView.ZoomFactor = 0.88;

        _lensWebView.CoreWebView2.NewWindowRequested += (_, args) =>
        {
            args.Handled = true;
            _lensWebView.CoreWebView2.Navigate(args.Uri);
        };

        _lensWebView.CoreWebView2.NavigationCompleted += async (_, _) =>
        {
            try
            {
                await _lensWebView.CoreWebView2.ExecuteScriptAsync(
                    "document.documentElement.style.overflowX = 'hidden'; document.body.style.overflowX = 'hidden';");
            }
            catch { }
        };

        _webViewInitialized = true;
    }

    private async Task UploadBytesToEmbeddedLensAsync(byte[] pngBytes)
    {
        _lastUploadedPngBytes = pngBytes;

        try
        {
            UpdateDrawerSize();
            _lensDrawerCard.Visibility = Visibility.Visible;

            await EnsureWebViewReadyAsync();

            // Записываем локальный HTML во временный файл и открываем через Source / CoreWebView2.Navigate,
            // чтобы не превышать лимит 2 МБ метода NavigateToString на экранах высокого разрешения (2880x1800)!
            var htmlFilePath = GoogleLensService.WriteTempAutoSubmitHtmlFile(pngBytes);
            var fileUri = new Uri(htmlFilePath).AbsoluteUri;
            _lensWebView.CoreWebView2.Navigate(fileUri);
        }
        catch (Exception ex)
        {
            AppLogger.Error("Embedded WebView2 Lens failed, falling back to external browser", ex);
            Close();
            await GoogleLensService.OpenInExternalBrowserAsync(pngBytes);
        }
    }
}
