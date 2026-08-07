using SharpRtfConvert;

namespace MewExtendedToolkit.Html;

public static class ControlExtensions
{
    /// <summary>
    /// Sets html text.
    /// </summary>
    /// <param name="control">Target html control.</param>
    /// <param name="html">Html content.</param>
    /// <returns>The html block for chaining.</returns>
    public static HtmlControl Text(this HtmlControl control, string html)
    {
        control.Text = html;
        return control;
    }

    /// <summary>
    /// Load rtf content.
    /// </summary>
    /// <param name="control">Target html control.</param>
    /// <param name="text">Rtf content.</param>
    /// <returns>The html block for chaining.</returns>
    public static HtmlControl LoadRtfText(this HtmlControl control, string text)
    {
        string html = Rtf.ToHtml(text);
        control.Text = html;
        return control;
    }

    /// <summary>
    /// Load rtf file.
    /// </summary>
    /// <param name="control">Target html control.</param>
    /// <param name="fileName">Rtf filename.</param>
    /// <returns>The html block for chaining.</returns>
    public static HtmlControl LoadRtfFile(this HtmlControl control, string fileName)
    {
        using var file = File.OpenRead(fileName);
        string html = Rtf.ToHtml(file);
        control.Text = html;
        return control;
    }

    /// <summary>
    /// Load rtf file.
    /// </summary>
    /// <param name="control">Target html control.</param>
    /// <param name="stream">Rtf stream.</param>
    /// <returns>The html block for chaining.</returns>
    public static HtmlControl LoadRtfFile(this HtmlControl control, Stream stream)
    {
        string html = Rtf.ToHtml(stream);
        control.Text = html;
        return control;
    }
}
