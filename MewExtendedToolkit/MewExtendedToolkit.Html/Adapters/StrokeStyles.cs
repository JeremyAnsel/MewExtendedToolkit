using Aprillz.MewUI.Rendering;

namespace MewExtendedToolkit.Html.Adapters;

internal static class StrokeStyles
{
    /// <summary>
    /// Solid - A solid DashArray (no dashes).
    /// </summary>
    public static StrokeStyle Solid => new()
    {
        MiterLimit = 10.0
    };

    /// <summary>
    /// Dash - A DashArray which is 2 on, 2 off
    /// </summary>
    public static StrokeStyle Dash => new()
    {
        MiterLimit = 10.0,
        DashArray = [2.0, 2.0],
        DashOffset = 1.0
    };

    /// <summary>
    /// Dot - A DashArray which is 0 on, 2 off
    /// </summary>
    public static StrokeStyle Dot => new()
    {
        MiterLimit = 10.0,
        DashArray = [0.0, 2.0],
        DashOffset = 0.0
    };

    /// <summary>
    /// DashDot - A DashArray which is 2 on, 2 off, 0 on, 2 off
    /// </summary>
    public static StrokeStyle DashDot => new()
    {
        MiterLimit = 10.0,
        DashArray = [2.0, 2.0, 0.0, 2.0],
        DashOffset = 1.0
    };

    /// <summary>
    /// DashDot - A DashArray which is 2 on, 2 off, 0 on, 2 off, 0 on, 2 off
    /// </summary>
    public static StrokeStyle DashDotDot => new()
    {
        MiterLimit = 10.0,
        DashArray = [2.0, 2.0, 0.0, 2.0, 0.0, 2.0],
        DashOffset = 1.0
    };
}
