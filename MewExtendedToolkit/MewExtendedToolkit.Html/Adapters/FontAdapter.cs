using Aprillz.MewUI;
using Aprillz.MewUI.Rendering;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace MewExtendedToolkit.Html.Adapters;

/// <summary>
/// Adapter for MewUI Font.
/// </summary>
internal sealed class FontAdapter : RFont
{
    #region Fields and Consts

    /// <summary>
    /// the underline MewUI font.
    /// </summary>
    private readonly IFont _font;

    /// <summary>
    /// the vertical offset of the font underline location from the top of the font.
    /// </summary>
    private double _underlineOffset = -1;

    /// <summary>
    /// Cached font height.
    /// </summary>
    private double _height = -1;

    /// <summary>
    /// Cached font whitespace width.
    /// </summary>
    private double _whitespaceWidth = -1;

    #endregion

    /// <summary>
    /// Init.
    /// </summary>
    public FontAdapter(string family, double size,
        FontWeight weight = FontWeight.Normal,
        bool italic = false, bool underline = false, bool strikethrough = false)
    {
        size *= 1.5;
        _font = Application.DefaultGraphicsFactory.CreateFont(family, size, weight, italic, underline, strikethrough);
        _height = _font.Ascent + _font.Descent;
        //_underlineOffset = _font.CapHeight + _font.Descent * 0.5;
        _underlineOffset = _font.Ascent + _font.Descent;
    }

    /// <summary>
    /// the underline MewUI font.
    /// </summary>
    public IFont Font => _font;

    public override double Size => _font.Size;

    public override double UnderlineOffset => _underlineOffset;

    public override double Height => _height;

    public override double LeftPadding => _height / 6f;

    public override double GetWhitespaceWidth(RGraphics graphics)
    {
        if (_whitespaceWidth < 0)
        {
            _whitespaceWidth = graphics.MeasureString(" ", this).Width;
        }
        return _whitespaceWidth;
    }
}
