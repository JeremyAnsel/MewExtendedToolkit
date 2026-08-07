using Aprillz.MewUI.Rendering;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace MewExtendedToolkit.Html.Adapters;

/// <summary>
/// Adapter for MewUI graphics path object for core.
/// </summary>
internal sealed class GraphicsPathAdapter : RGraphicsPath
{
    /// <summary>
    /// The actual MewUI graphics geometry instance.
    /// </summary>
    private readonly PathGeometry _geometry = new();

    public GraphicsPathAdapter()
    {
    }

    public override void Start(double x, double y)
    {
        _geometry.Reset();
        _geometry.MoveTo(x, y);
    }

    public override void LineTo(double x, double y)
    {
        _geometry.LineTo(x, y);
    }

    public override void ArcTo(double x, double y, double size, Corner corner)
    {
        _geometry.SvgArcTo(size, size, 0, false, true, x, y);
    }

    /// <summary>
    /// Close the geometry to so no more path adding is allowed and return the instance so it can be rendered.
    /// </summary>
    public PathGeometry GetClosedGeometry()
    {
        _geometry.Close();
        _geometry.Freeze();
        return _geometry;
    }

    public override void Dispose()
    {
    }
}
