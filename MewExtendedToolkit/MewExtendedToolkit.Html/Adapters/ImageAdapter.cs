using Aprillz.MewUI;
using Aprillz.MewUI.Rendering;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace MewExtendedToolkit.Html.Adapters;

/// <summary>
/// Adapter for MewUI Image object for core.
/// </summary>
internal sealed class ImageAdapter : RImage
{
    /// <summary>
    /// the underline MewUI image.
    /// </summary>
    private readonly IImage _image;

    /// <summary>
    /// Init.
    /// </summary>
    public ImageAdapter(ImageSource image)
    {
        _image = image.CreateImage(Application.DefaultGraphicsFactory);
    }

    /// <summary>
    /// the underline MewUI image.
    /// </summary>
    public IImage Image => _image;

    public override double Width => _image.PixelWidth;

    public override double Height => _image.PixelHeight;

    public override void Dispose()
    {
    }
}
