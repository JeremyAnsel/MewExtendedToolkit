using Aprillz.MewUI.Rendering;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace MewExtendedToolkit.Html.Adapters;

/// <summary>
/// Adapter for MewUI brushes.
/// </summary>
internal sealed class BrushAdapter : RBrush
{
    /// <summary>
    /// The actual MewUI brush instance.
    /// </summary>
    private readonly Brush _brush;

    /// <summary>
    /// Init.
    /// </summary>
    public BrushAdapter(Brush brush)
    {
        _brush = brush;
    }

    /// <summary>
    /// The actual MewUI brush instance.
    /// </summary>
    public Brush Brush => _brush;

    public override void Dispose()
    {
    }
}
