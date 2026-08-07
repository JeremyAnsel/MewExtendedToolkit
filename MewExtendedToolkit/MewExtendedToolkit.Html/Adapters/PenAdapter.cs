using Aprillz.MewUI.Rendering;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace MewExtendedToolkit.Html.Adapters;

/// <summary>
/// Adapter for MewUI pens objects for core.
/// </summary>
internal sealed class PenAdapter : RPen
{
    /// <summary>
    /// The actual MewUI brush instance.
    /// </summary>
    private readonly Brush _brush;

    /// <summary>
    /// the width of the pen
    /// </summary>
    private double _width;

    /// <summary>
    /// the dash style of the pen
    /// </summary>
    private StrokeStyle _dashStyle = StrokeStyles.Solid;

    /// <summary>
    /// Init.
    /// </summary>
    public PenAdapter(Brush brush)
    {
        _brush = brush;
    }

    public override double Width
    {
        get { return _width; }
        set { _width = value; }
    }

    public override RDashStyle DashStyle
    {
        set
        {
            _dashStyle = value switch
            {
                RDashStyle.Solid => StrokeStyles.Solid,
                RDashStyle.Dash => StrokeStyles.Dash,
                RDashStyle.Dot => StrokeStyles.Dot,
                RDashStyle.DashDot => StrokeStyles.DashDot,
                RDashStyle.DashDotDot => StrokeStyles.DashDotDot,
                _ => StrokeStyles.Solid,
            };
        }
    }

    /// <summary>
    /// Create the actual MewUI pen instance.
    /// </summary>
    public Pen CreatePen()
    {
        var pen = new Pen(_brush, _width, _dashStyle);
        return pen;
    }
}
