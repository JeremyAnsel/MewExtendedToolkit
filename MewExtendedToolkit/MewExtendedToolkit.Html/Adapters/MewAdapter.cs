using Aprillz.MewUI;
using Aprillz.MewUI.Rendering;
using MewExtendedToolkit.Html.Utilities;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace MewExtendedToolkit.Html.Adapters;

/// <summary>
/// Adapter for MewUI platform.
/// </summary>
internal sealed class MewAdapter : RAdapter
{
    #region Fields and Consts

    /// <summary>
    /// Singleton instance of global adapter.
    /// </summary>
    private static readonly MewAdapter _instance = new();

    #endregion

    /// <summary>
    /// Singleton instance of global adapter.
    /// </summary>
    public static MewAdapter Instance => _instance;

    /// <summary>
    /// Init installed font families and set default font families mapping.
    /// </summary>
    private MewAdapter()
    {
        AddFontFamilyMapping("monospace", "Courier New");
        AddFontFamilyMapping("Helvetica", "Arial");

        //foreach (var family in FontFamily.Families)
        //{
        //    AddFontFamily(new FontFamilyAdapter(family));
        //}
    }

    protected override RColor GetColorInt(string colorName)
    {
        if (!Color.NamedColors.TryGetValue(colorName, out Color color))
        {
            return RColor.Empty;
        }

        return Utils.Convert(color);
    }

    protected override RPen CreatePen(RColor color)
    {
        return new PenAdapter(new SolidColorBrush(Utils.Convert(color)));
    }

    protected override RBrush CreateSolidBrush(RColor color)
    {
        Brush solidBrush;
        if (color == RColor.White)
        {
            solidBrush = new SolidColorBrush(Color.White);
        }
        else if (color == RColor.Black)
        {
            solidBrush = new SolidColorBrush(Color.Black);
        }
        else if (color.A < 1)
        {
            solidBrush = new SolidColorBrush(Color.Transparent);
        }
        else
        {
            solidBrush = new SolidColorBrush(Utils.Convert(color));
        }

        return new BrushAdapter(solidBrush);
    }

    protected override RBrush CreateLinearGradientBrush(RRect rect, RColor color1, RColor color2, double angle)
    {
        Point start = new(rect.Left, rect.Top);
        Point end = new(rect.Right, rect.Bottom);
        IReadOnlyList<GradientStop> gradients = [new GradientStop(0.0, Utils.Convert(color1)), new GradientStop(1.0, Utils.Convert(color2))];
        System.Numerics.Matrix3x2 transform = System.Numerics.Matrix3x2.CreateRotation((float)angle);
        return new BrushAdapter(new LinearGradientBrush(start, end, gradients, SpreadMethod.Pad, GradientUnits.UserSpaceOnUse, transform));
    }

    protected override RImage? ConvertImageInt(object? image)
    {
        return image != null ? new ImageAdapter((ImageSource)image) : null;
    }

    protected override RImage ImageFromStreamInt(Stream memoryStream)
    {
        using var ms = new MemoryStream();
        memoryStream.CopyTo(ms);
        return new ImageAdapter(ImageSource.FromBytes(ms.ToArray()));
    }

    protected override RFont CreateFontInt(string family, double size, RFontStyle style)
    {
        return new FontAdapter(
            family,
            size,
            style.HasFlag(RFontStyle.Bold) ? FontWeight.Bold : FontWeight.Normal,
            style.HasFlag(RFontStyle.Italic),
            style.HasFlag(RFontStyle.Underline),
            style.HasFlag(RFontStyle.Strikeout));
    }

    protected override RFont CreateFontInt(RFontFamily family, double size, RFontStyle style)
    {
        return new FontAdapter(
            ((FontFamilyAdapter)family).FontFamily,
            size,
            style.HasFlag(RFontStyle.Bold) ? FontWeight.Bold : FontWeight.Normal,
            style.HasFlag(RFontStyle.Italic),
            style.HasFlag(RFontStyle.Underline),
            style.HasFlag(RFontStyle.Strikeout));
    }

    protected override object? GetClipboardDataObjectInt(string html, string plainText)
    {
        //return ClipboardHelper.CreateDataObject(html, plainText);
        return null;
    }

    protected override void SetToClipboardInt(string text)
    {
        //ClipboardHelper.CopyToClipboard(text);
    }

    protected override void SetToClipboardInt(string html, string plainText)
    {
        //ClipboardHelper.CopyToClipboard(html, plainText);
    }

    protected override void SetToClipboardInt(RImage image)
    {
        //Clipboard.SetImage(((ImageAdapter)image).Image);
    }

    protected override RContextMenu CreateContextMenuInt()
    {
        return new ContextMenuAdapter();
    }

    protected override void SaveToFileInt(RImage image, string name, string extension, RControl? control = null)
    {
        //using (var saveDialog = new SaveFileDialog())
        //{
        //    saveDialog.Filter = "Images|*.png;*.bmp;*.jpg";
        //    saveDialog.FileName = name;
        //    saveDialog.DefaultExt = extension;

        //    var dialogResult = control == null ? saveDialog.ShowDialog() : saveDialog.ShowDialog(((ControlAdapter)control).Control);
        //    if (dialogResult == DialogResult.OK)
        //    {
        //        ((ImageAdapter)image).Image.Save(saveDialog.FileName);
        //    }
        //}
    }
}
