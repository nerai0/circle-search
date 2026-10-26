using System.Windows;

namespace CircleSearch.UI;

/// <summary>
/// Единое место адаптивных размеров интерфейса: все значения вычисляются от фактических
/// размеров экрана/окна (в DIP), а не прописываются константами в окнах.
/// </summary>
public static class UiMetrics
{
    // Опорный размер, при котором элементы имеют масштаб 1.0
    private const double ReferenceWidth = 1366;
    private const double ReferenceHeight = 768;
    private const double MinChromeScale = 0.85;
    private const double MaxChromeScale = 1.25;

    private const double DrawerRatioCollapsed = 0.48;
    private const double DrawerRatioExpanded = 0.68;
    private const double DrawerMinWidth = 420;
    private const double DrawerScreenGap = 40;

    /// <summary>Масштаб панелей управления (верхняя панель, нижний док) относительно размера экрана.</summary>
    public static double ChromeScale(double width, double height)
    {
        if (width <= 0 || height <= 0) return 1.0;
        return Math.Clamp(Math.Min(width / ReferenceWidth, height / ReferenceHeight), MinChromeScale, MaxChromeScale);
    }

    /// <summary>Ширина боковой панели Google Lens: доля экрана, но всегда помещается в экран.</summary>
    public static double DrawerWidth(double screenWidth, bool expanded)
    {
        if (screenWidth <= 0) return DrawerMinWidth;
        var maxWidth = Math.Max(1, screenWidth - DrawerScreenGap);
        var minWidth = Math.Min(DrawerMinWidth, maxWidth);
        var ratio = expanded ? DrawerRatioExpanded : DrawerRatioCollapsed;
        return Math.Clamp(screenWidth * ratio, minWidth, maxWidth);
    }

    /// <summary>Отступы боковой панели: сверху — под верхней панелью, снизу — над нижним доком.</summary>
    public static Thickness DrawerMargin(double screenWidth, double screenHeight, double chromeScale)
    {
        var right = Math.Clamp(screenWidth * 0.012, 10, 28);
        var top = Math.Clamp(screenHeight * 0.075, 48, 84) * chromeScale;
        var bottom = Math.Clamp(screenHeight * 0.115, 72, 132) * chromeScale;
        return new Thickness(0, top, right, bottom);
    }

    /// <summary>Максимальная ширина подписи кнопки в нижнем доке (длинные названия окон обрезаются с многоточием).</summary>
    public static double DockLabelMaxWidth(double screenWidth, double chromeScale)
    {
        if (screenWidth <= 0) return 260;
        return Math.Clamp(screenWidth / chromeScale * 0.28, 140, 480);
    }

    /// <summary>Подгоняет размер обычного окна под рабочую область экрана.</summary>
    public static void FitWindowToWorkArea(
        Window window,
        double widthRatio, double heightRatio,
        double minWidth, double minHeight,
        double maxWidth, double maxHeight)
    {
        var work = SystemParameters.WorkArea;
        var limitW = work.Width * 0.95;
        var limitH = work.Height * 0.95;

        window.MaxWidth = limitW;
        window.MaxHeight = limitH;
        window.Width = Math.Min(Math.Clamp(work.Width * widthRatio, minWidth, maxWidth), limitW);
        window.Height = Math.Min(Math.Clamp(work.Height * heightRatio, minHeight, maxHeight), limitH);
        window.MinWidth = Math.Min(minWidth, window.Width);
        window.MinHeight = Math.Min(minHeight, window.Height);
    }
}
