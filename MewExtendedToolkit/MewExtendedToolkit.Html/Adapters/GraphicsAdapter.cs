using Aprillz.MewUI;
using Aprillz.MewUI.Rendering;
using Aprillz.MewUI.Text;
using MewExtendedToolkit.Html.Utilities;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace MewExtendedToolkit.Html.Adapters;

/// <summary>
/// Adapter for MewUI Graphics.
/// </summary>
internal sealed class GraphicsAdapter : RGraphics
{
    #region Fields and Consts

    /// <summary>
    /// The wrapped MewUI graphics object
    /// </summary>
    private readonly IGraphicsContext? _g;

    /// <summary>
    /// if to release the graphics object on dispose
    /// </summary>
    private readonly bool _releaseGraphics;

    #endregion

    /// <summary>
    /// Init.
    /// </summary>
    /// <param name="g">the MewUI graphics object to use</param>
    /// <param name="initialClip">the initial clip of the graphics</param>
    /// <param name="releaseGraphics">optional: if to release the graphics object on dispose (default - false)</param>
    public GraphicsAdapter(IGraphicsContext? g, RRect initialClip, bool releaseGraphics = false)
        : base(MewAdapter.Instance, initialClip)
    {
        ArgChecker.AssertArgNotNull(g, nameof(g));

        _g = g;
        _releaseGraphics = releaseGraphics;
    }

    /// <summary>
    /// Init.
    /// </summary>
    public GraphicsAdapter()
        : base(MewAdapter.Instance, RRect.Empty)
    {
        _g = null;
        _releaseGraphics = false;
    }

    public override void PopClip()
    {
        _clipStack.Pop();
        _g?.ResetClip();
        _g?.SetClip(Utils.Convert(_clipStack.Peek()));
    }

    public override void PushClip(RRect rect)
    {
        if (_clipStack.Count == 1)
        {
            // rect is (99999, 99999)
            rect = _clipStack.Peek();
        }

        _clipStack.Push(rect);
        _g?.ResetClip();
        _g?.SetClip(Utils.Convert(rect));
    }

    public override void PushClipExclude(RRect rect)
    {
        _clipStack.Push(_clipStack.Peek());
        _g?.IntersectClip(Utils.Convert(rect));
    }

    public override object? SetAntiAliasSmoothingMode()
    {
        return null;
    }

    public override void ReturnPreviousSmoothingMode(object prevMode)
    {
    }

    public override RSize MeasureString(string str, RFont font)
    {
        var fontAdapter = (FontAdapter)font;
        var realFont = fontAdapter.Font;
        uint dpi = Application.Current.AllWindows[0].GetDpi();

        ITextLayout layout = Application.DefaultGraphicsFactory.TextEngine.CreateLayout(new TextLayoutRequest
        {
            Text = str.AsMemory(),
            Dpi = dpi,
            DefaultStyle = new TextRunStyle(realFont.Family, realFont.Size, realFont.Weight, realFont.IsItalic)
        });

        Size size = layout.MeasuredSize;
        return Utils.Convert(size);
    }

    public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
    {
        charFit = 0;
        charFitWidth = 0;

        if (str.Length == 0 || maxWidth == 0.0)
        {
            return;
        }

        int count = 1;
        double width = 0;

        for (; count <= str.Length; count++)
        {
            string s = str[..count];
            RSize size = MeasureString(s, font);

            if (size.Width <= maxWidth)
            {
                width = size.Width;
            }
            else
            {
                break;
            }
        }

        charFit = count - 1;
        charFitWidth = width;
    }

    public override void DrawString(string str, RFont font, RColor color, RPoint point, RSize size, bool rtl)
    {
        var fontAdapter = (FontAdapter)font;
        var realFont = fontAdapter.Font;
        uint dpi = Application.Current.AllWindows[0].GetDpi();

        ITextLayout layout = Application.DefaultGraphicsFactory.TextEngine.CreateLayout(new TextLayoutRequest
        {
            Text = str.AsMemory(),
            Dpi = dpi,
            DefaultStyle = new TextRunStyle(realFont.Family, realFont.Size, realFont.Weight, realFont.IsItalic),
            Paragraph = new TextParagraphStyle
            {
                MaxWidth = size.Width,
                MaxHeight = size.Height,
                FlowDirection = rtl ? TextFlowDirection.RightToLeft : TextFlowDirection.LeftToRight
            }
        });

        _g?.Text.Draw(layout, Utils.ConvertRound(point), new TextDrawOptions
        {
            Foreground = Utils.Convert(color)
        });
    }

    public override RBrush GetTextureBrush(RImage image, RRect dstRect, RPoint translateTransformLocation)
    {
        IImage srcImage = ((ImageAdapter)image).Image;
        Rect srcRect = new(0, 0, srcImage.PixelWidth, srcImage.PixelHeight);
        var transform = System.Numerics.Matrix3x2.CreateTranslation((float)translateTransformLocation.X, (float)translateTransformLocation.Y);

        var brush = new ImageBrush(
            srcImage,
            srcRect,
            Utils.Convert(dstRect),
            TileMode.Tile,
            1,
            transform);

        return new BrushAdapter(brush);
    }

    public override RGraphicsPath GetGraphicsPath()
    {
        return new GraphicsPathAdapter();
    }

    public override void Dispose()
    {
        if (_releaseGraphics)
        {
            _g?.Dispose();
        }
    }


    #region Delegate graphics methods

    public override void DrawLine(RPen pen, double x1, double y1, double x2, double y2)
    {
        x1 = (int)x1;
        x2 = (int)x2;
        y1 = (int)y1;
        y2 = (int)y2;

        var adj = pen.Width;
        if (Math.Abs(x1 - x2) < .1 && Math.Abs(adj % 2 - 1) < .1)
        {
            x1 += .5;
            x2 += .5;
        }
        if (Math.Abs(y1 - y2) < .1 && Math.Abs(adj % 2 - 1) < .1)
        {
            y1 += .5;
            y2 += .5;
        }

        _g?.DrawLine(new Point(x1, y1), new Point(x2, y2), ((PenAdapter)pen).CreatePen());
    }

    public override void DrawRectangle(RPen pen, double x, double y, double width, double height)
    {
        var adj = pen.Width;
        if (Math.Abs(adj % 2 - 1) < .1)
        {
            x += .5;
            y += .5;
        }

        _g?.DrawRectangle(new Rect(x, y, width, height), ((PenAdapter)pen).CreatePen());
    }

    public override void DrawRectangle(RBrush brush, double x, double y, double width, double height)
    {
        _g?.FillRectangle(new Rect(x, y, width, height), ((BrushAdapter)brush).Brush);
    }

    public override void DrawImage(RImage image, RRect destRect, RRect srcRect)
    {
        _g?.DrawImage(((ImageAdapter)image).Image, Utils.ConvertRound(destRect), Utils.ConvertRound(srcRect));
    }

    public override void DrawImage(RImage image, RRect destRect)
    {
        _g?.DrawImage(((ImageAdapter)image).Image, Utils.ConvertRound(destRect));
    }

    public override void DrawPath(RPen pen, RGraphicsPath path)
    {
        _g?.DrawPath(((GraphicsPathAdapter)path).GetClosedGeometry(), ((PenAdapter)pen).CreatePen());
    }

    public override void DrawPath(RBrush brush, RGraphicsPath path)
    {
        _g?.FillPath(((GraphicsPathAdapter)path).GetClosedGeometry(), ((BrushAdapter)brush).Brush);
    }

    public override void DrawPolygon(RBrush brush, RPoint[]? points)
    {
        if (points != null && points.Length > 0)
        {
            var g = new PathGeometry();
            g.MoveTo(Utils.Convert(points[0]));
            for (int i = 1; i < points.Length; i++)
            {
                g.LineTo(Utils.Convert(points[i]));
            }
            g.Freeze();

            _g?.FillPath(g, ((BrushAdapter)brush).Brush);
        }
    }

    #endregion
}
