using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using MewExtendedToolkit.Html.Utilities;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace MewExtendedToolkit.Html.Adapters;

/// <summary>
/// Adapter for MewUI Control for core.
/// </summary>
internal sealed class ControlAdapter : RControl
{
    /// <summary>
    /// the underline MewUI control.
    /// </summary>
    private readonly Control _control;

    private Point _mousePosition = Point.Zero;
    private bool _leftMouseButton = false;
    private bool _righttMouseButton = false;

    /// <summary>
    /// Init.
    /// </summary>
    public ControlAdapter(Control control)
        : base(MewAdapter.Instance)
    {
        ArgChecker.AssertArgNotNull(control, "control");

        _control = control;

        _control.MouseMove += e =>
        {
            _mousePosition = e.GetPosition(_control);
        };

        _control.MouseDown += e =>
        {
            _leftMouseButton = e.LeftButton;
            _righttMouseButton = e.RightButton;
        };

        _control.MouseUp += e =>
        {
            _leftMouseButton = e.LeftButton;
            _righttMouseButton = e.RightButton;
        };
    }

    /// <summary>
    /// Get the underline MewUI control
    /// </summary>
    public Control Control => _control;

    public override RPoint MouseLocation => Utils.Convert(_control.PointFromScreen(_mousePosition));

    public override bool LeftMouseButton => _leftMouseButton;

    public override bool RightMouseButton => _righttMouseButton;

    public override void SetCursorDefault()
    {
        _control.Cursor = CursorType.Arrow;
    }

    public override void SetCursorHand()
    {
        _control.Cursor = CursorType.Hand;
    }

    public override void SetCursorIBeam()
    {
        _control.Cursor = CursorType.IBeam;
    }

    public override void DoDragDropCopy(object dragDropData)
    {
        // Not Implemented
        //DragDrop.DoDragDrop(_control, dragDropData, DragDropEffects.Copy);
    }

    public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
    {
        using var g = new GraphicsAdapter();
        g.MeasureString(str, font, maxWidth, out charFit, out charFitWidth);
    }

    public override void Invalidate()
    {
        _control.InvalidateVisual();
    }
}
