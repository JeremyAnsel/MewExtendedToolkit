using TheArtOfDev.HtmlRenderer.Adapters;

namespace MewExtendedToolkit.Html.Adapters;

/// <summary>
/// Adapter for MewUI Font family object for core.
/// </summary>
internal sealed class FontFamilyAdapter : RFontFamily
{
    /// <summary>
    /// the underline MewUI font.
    /// </summary>
    private readonly string _fontFamily;

    /// <summary>
    /// Init.
    /// </summary>
    public FontFamilyAdapter(string fontFamily)
    {
        _fontFamily = fontFamily;
    }

    /// <summary>
    /// the underline MewUI font family.
    /// </summary>
    public string FontFamily => _fontFamily;

    public override string Name => _fontFamily;
}
