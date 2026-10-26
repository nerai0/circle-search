using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CircleSearch.UI;

/// <summary>
/// Рендерер контекстного меню трея в стиле приложения: цвета берутся из тех же токенов темы,
/// что и WPF-окна (без системных рамок и градиентов), пункты со скруглённой подсветкой.
/// </summary>
public sealed class ThemedMenuRenderer : ToolStripProfessionalRenderer
{
    private readonly Color _background;
    private readonly Color _hover;
    private readonly Color _text;
    private readonly Color _textDisabled;
    private readonly Color _primary;

    public ThemedMenuRenderer() : base(new ThemedColorTable())
    {
        RoundedEdges = false;
        _background = ThemeManager.GetDrawingColor("BgSurfaceBrush");
        _hover = ThemeManager.GetDrawingColor("BgHoverBrush");
        _text = ThemeManager.GetDrawingColor("TextPrimaryBrush");
        _textDisabled = ThemeManager.GetDrawingColor("TextSecondaryBrush");
        _primary = ThemeManager.GetDrawingColor("PrimaryBrush");
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        using var brush = new SolidBrush(_background);
        e.Graphics.FillRectangle(brush, e.AffectedBounds);
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        // Без внешней рамки: меню отделяется от фона только тоном поверхности
    }

    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
        // Единый фон без отдельной колонки для иконок
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        if (!e.Item.Selected || !e.Item.Enabled) return;

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(4, 1, e.Item.Width - 8, e.Item.Height - 2);
        using var path = RoundedRect(rect, 8);
        using var brush = new SolidBrush(_hover);
        g.FillPath(brush, path);
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        e.TextColor = e.Item.Enabled ? _text : _textDisabled;
        base.OnRenderItemText(e);
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = e.ImageRectangle;
        float cx = r.Left + r.Width / 2f;
        float cy = r.Top + r.Height / 2f;

        using var pen = new Pen(_primary, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
        g.DrawLines(pen, new[]
        {
            new PointF(cx - 4.5f, cy),
            new PointF(cx - 1.5f, cy + 3.5f),
            new PointF(cx + 4.5f, cy - 3.5f)
        });
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        int y = e.Item.Height / 2;
        using var pen = new Pen(_hover, 1f);
        e.Graphics.DrawLine(pen, 12, y, e.Item.Width - 12, y);
    }

    private static GraphicsPath RoundedRect(Rectangle r, int radius)
    {
        int d = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(r.Left, r.Top, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private sealed class ThemedColorTable : ProfessionalColorTable
    {
        private static Color Bg => ThemeManager.GetDrawingColor("BgSurfaceBrush");

        public override Color ToolStripDropDownBackground => Bg;
        public override Color ImageMarginGradientBegin => Bg;
        public override Color ImageMarginGradientMiddle => Bg;
        public override Color ImageMarginGradientEnd => Bg;
        public override Color MenuBorder => Bg;
        public override Color MenuItemBorder => Bg;
        public override Color MenuItemSelected => Bg;
    }
}
