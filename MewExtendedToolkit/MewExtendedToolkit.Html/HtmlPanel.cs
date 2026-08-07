using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using Aprillz.MewUI.Rendering;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace MewExtendedToolkit.Html;

/// <summary>
/// Provides HTML rendering using the text property.<br/>
/// MewUI control that will render html content in it's client rectangle.<br/>
/// If the layout of the html resulted in its content beyond the client bounds of the panel it will show scrollbars (horizontal/vertical) allowing to scroll the content.<br/>
/// The control will handle mouse and keyboard events on it to support html text selection, copy-paste and mouse clicks.<br/>
/// </summary>
/// <remarks>
/// See <see cref="HtmlControl"/> for more info.
/// </remarks>
public class HtmlPanel : HtmlControl, IVisualTreeHost
{
    #region Fields and Consts

    protected const double SmallChange = 25.0;

    protected const double LargeChangeFactor = 0.9;

    protected readonly ObservableValue<double> _horizontalScrollBarValue = new();

    protected readonly ObservableValue<double> _verticalScrollBarValue = new();

    private readonly ScrollBar _hBar;
    private readonly ScrollBar _vBar;

    #endregion

    static HtmlPanel()
    {
        BackgroundProperty.OverrideDefaultValue<HtmlPanel>(ThemeManager.Default == ThemeVariant.Dark ? Color.LightGray : Color.White);
        FocusableProperty.OverrideDefaultValue<HtmlPanel>(true);
    }

    /// <summary>
    /// Creates a new HtmlPanel and sets a basic css for it's styling.
    /// </summary>
    public HtmlPanel()
    {
        _htmlContainer.IsSelectionEnabled = false;
        _htmlContainer.IsContextMenuEnabled = false;

        _htmlContainer.ScrollChange += OnScrollChange;

        _horizontalScrollBarValue.Changed += CoerseHorizontalScrollValue;
        _verticalScrollBarValue.Changed += CoerseVerticalScrollValue;

        _hBar = new ScrollBar
        {
            Orientation = Orientation.Horizontal,
            IsVisible = true,
            SkipViewportCull = true,
            Background = Color.DarkGray
        };
        AttachChild(_hBar);

        _vBar = new ScrollBar
        {
            Orientation = Orientation.Vertical,
            IsVisible = true,
            SkipViewportCull = true,
            Background = Color.DarkGray
        };
        AttachChild(_vBar);

        _hBar.ValueChanged += h => ScrollToPoint(h, _vBar.Value);
        _vBar.ValueChanged += v => ScrollToPoint(_hBar.Value, v);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        _hBar.Dispose();
        _vBar.Dispose();
    }

    private void CoerseHorizontalScrollValue()
    {
        double value = _horizontalScrollBarValue.Value;
        value = Math.Min(value, _htmlContainer.ActualSize.Width - this.ActualWidth);
        value = Math.Max(value, 0);
        _horizontalScrollBarValue.Value = value;
        UpdateScrollOffsets();
    }

    private void CoerseVerticalScrollValue()
    {
        double value = _verticalScrollBarValue.Value;
        value = Math.Min(value, _htmlContainer.ActualSize.Height - this.ActualHeight);
        value = Math.Max(value, 0);
        _verticalScrollBarValue.Value = value;
        UpdateScrollOffsets();
    }

    bool IVisualTreeHost.VisitChildren(Func<Element, bool> visitor) => visitor(_hBar) && visitor(_vBar);

    /// <summary>
    /// Adjust the scrollbar of the panel on html element by the given id.<br/>
    /// The top of the html element rectangle will be at the top of the panel, if there
    /// is not enough height to scroll to the top the scroll will be at maximum.<br/>
    /// </summary>
    /// <param name="elementId">the id of the element to scroll to</param>
    public virtual void ScrollToElement(string elementId)
    {
        ArgChecker.AssertArgNotNullOrEmpty(elementId, nameof(elementId));

        if (_htmlContainer != null)
        {
            var rect = _htmlContainer.GetElementRectangle(elementId);
            if (rect.HasValue)
            {
                ScrollToPoint(rect.Value.Left, rect.Value.Top);
                _htmlContainer.HandleMouseMove(this, _mouseMovePoint);
            }
        }
    }

    #region Private methods

    /// <summary>
    /// Perform the layout of the html in the control.
    /// </summary>
    protected override Size MeasureOverride(Size constraint)
    {
        Size size = PerformHtmlLayout(constraint);

        // to handle if scrollbar is appearing or disappearing
        //bool relayout = false;
        //var htmlWidth = HtmlWidth(constraint);
        //var htmlHeight = HtmlHeight(constraint);

        PerformHtmlLayout(constraint);

        if (double.IsPositiveInfinity(constraint.Width) || double.IsPositiveInfinity(constraint.Height))
        {
            constraint = size;
        }

        return constraint;
    }

    /*
    /// <summary>
    /// After measurement arrange the scrollbars of the panel.
    /// </summary>
    protected override Size ArrangeOverride(Size bounds)
    {
        var scrollHeight = HtmlHeight(bounds) + Padding.Top + Padding.Bottom;
        scrollHeight = scrollHeight > 1 ? scrollHeight : 1;
        var scrollWidth = HtmlWidth(bounds) + Padding.Left + Padding.Right;
        scrollWidth = scrollWidth > 1 ? scrollWidth : 1;
        //_verticalScrollBar.Arrange(new Rect(System.Math.Max(bounds.Width - _verticalScrollBar.Width - BorderThickness, 0), BorderThickness, _verticalScrollBar.Width, scrollHeight));
        //_horizontalScrollBar.Arrange(new Rect(BorderThickness, System.Math.Max(bounds.Height - _horizontalScrollBar.Height - BorderThickness, 0), scrollWidth, _horizontalScrollBar.Height));

        if (_htmlContainer != null)
        {
            // update the scroll offset because the scroll values may have changed
            UpdateScrollOffsets();
        }

        return bounds;
    }
    */

    /// <summary>
    /// Perform html container layout by the current panel client size.
    /// </summary>
    protected Size PerformHtmlLayout(Size constraint)
    {
        if (_htmlContainer != null)
        {
            _htmlContainer.MaxSize = new Size(HtmlWidth(constraint), 0);
            _htmlContainer.PerformLayout();
            return _htmlContainer.ActualSize;
        }
        return Size.Empty;
    }

    /// <summary>
    /// Handle minor case where both scroll are visible and create a rectangle at the bottom right corner between them.
    /// </summary>
    protected override void OnRender(IGraphicsContext context)
    {
        base.OnRender(context);

        // Bars render on top (overlay).
        _hBar.Value = _horizontalScrollBarValue.Value;
        _hBar.Render(context);
        _vBar.Value = _verticalScrollBarValue.Value;
        _vBar.Render(context);
    }

    protected override void OnDpiChanged(uint oldDpi, uint newDpi)
    {
        base.OnDpiChanged(oldDpi, newDpi);

        InvalidateArrange();
        SyncBars();
    }

    protected override UIElement? OnHitTest(Point point)
    {
        if (!IsVisible || !IsHitTestVisible || !IsEffectivelyEnabled)
        {
            return null;
        }

        if (_hBar.IsVisible && _hBar.Bounds.Contains(point))
        {
            return _hBar;
        }

        if (_vBar.IsVisible && _vBar.Bounds.Contains(point))
        {
            return _vBar;
        }

        return base.OnHitTest(point);
    }

    private void SyncBars()
    {
        _hBar.Minimum = 0;
        _hBar.Maximum = _htmlContainer.ActualSize.Width - this.ActualWidth;
        _hBar.ViewportSize = this.ActualWidth;
        _hBar.SmallChange = Theme.Metrics.ScrollBarSmallChange;
        _hBar.LargeChange = Theme.Metrics.ScrollBarLargeChange;
        _hBar.Value = _horizontalScrollBarValue.Value;

        _vBar.Minimum = 0;
        _vBar.Maximum = _htmlContainer.ActualSize.Height - this.ActualHeight;
        _vBar.ViewportSize = this.ActualHeight;
        _vBar.SmallChange = Theme.Metrics.ScrollBarSmallChange;
        _vBar.LargeChange = Theme.Metrics.ScrollBarLargeChange;
        _vBar.Value = _verticalScrollBarValue.Value;
    }

    private void ArrangeBars(Rect viewport)
    {
        double t = Theme.Metrics.ScrollBarHitThickness;
        _hBar.Arrange(new Rect(viewport.X, viewport.Y + viewport.Height - t, viewport.Width, t));
        _vBar.Arrange(new Rect(viewport.Right - t, viewport.Y, t, viewport.Height));
    }

    protected override void ArrangeCore(Rect finalRect)
    {
        base.ArrangeCore(finalRect);
        SyncBars();
        ArrangeBars(finalRect);
    }

    /// <summary>
    /// Handle mouse up to set focus on the control. 
    /// </summary>
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        Focus();
    }

    /// <summary>
    /// Handle mouse wheel for scrolling.
    /// </summary>
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _horizontalScrollBarValue.Value -= e.Delta.X * SmallChange;
        _verticalScrollBarValue.Value -= e.Delta.Y * SmallChange;
        UpdateScrollOffsets();
    }

    /// <summary>
    /// Handle key down event for selection, copy and scrollbars handling.
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        double largeChange = this.ActualHeight * LargeChangeFactor;

        if (e.Key == Key.Up)
        {
            _verticalScrollBarValue.Value -= SmallChange;
            UpdateScrollOffsets();
            e.Handled = true;
        }
        else if (e.Key == Key.Down)
        {
            _verticalScrollBarValue.Value += SmallChange;
            UpdateScrollOffsets();
            e.Handled = true;
        }
        else if (e.Key == Key.PageUp)
        {
            _verticalScrollBarValue.Value -= largeChange;
            UpdateScrollOffsets();
            e.Handled = true;
        }
        else if (e.Key == Key.PageDown)
        {
            _verticalScrollBarValue.Value += largeChange;
            UpdateScrollOffsets();
            e.Handled = true;
        }
        else if (e.Key == Key.Home)
        {
            _verticalScrollBarValue.Value = 0.0;
            UpdateScrollOffsets();
            e.Handled = true;
        }
        else if (e.Key == Key.End)
        {
            _verticalScrollBarValue.Value = _htmlContainer.ActualSize.Height - this.ActualHeight;
            UpdateScrollOffsets();
            e.Handled = true;
        }

        if (e.Key == Key.Left)
        {
            _horizontalScrollBarValue.Value -= SmallChange;
            UpdateScrollOffsets();
            e.Handled = true;
        }
        else if (e.Key == Key.Right)
        {
            _horizontalScrollBarValue.Value += SmallChange;
            UpdateScrollOffsets();
            e.Handled = true;
        }
    }

    /// <summary>
    /// Get the width the HTML has to render in (not including vertical scroll iff it is visible)
    /// </summary>
    protected override double HtmlWidth(Size size)
    {
        //var width = base.HtmlWidth(size) - (_verticalScrollBar.IsVisible ? _verticalScrollBar.Width : 0);
        var width = base.HtmlWidth(size);

        return width > 1 ? width : 1;
    }

    /// <summary>
    /// Get the width the HTML has to render in (not including vertical scroll iff it is visible)
    /// </summary>
    protected override double HtmlHeight(Size size)
    {
        //var height = base.HtmlHeight(size) - (_horizontalScrollBar.IsVisible ? _horizontalScrollBar.Height : 0);
        var height = base.HtmlHeight(size);
        return height > 1 ? height : 1;
    }

    /// <summary>
    /// On HTML container scroll change request scroll to the requested location.
    /// </summary>
    private void OnScrollChange(object? sender, HtmlScrollEventArgs e)
    {
        ScrollToPoint(e.X, e.Y);
    }

    /// <summary>
    /// Set the control scroll offset to the given values.
    /// </summary>
    public void ScrollToPoint(double x, double y)
    {
        _horizontalScrollBarValue.Value = x;
        _verticalScrollBarValue.Value = y;
        UpdateScrollOffsets();
    }

    /// <summary>
    /// Update the scroll offset of the HTML container and invalidate visual to re-render.
    /// </summary>
    private void UpdateScrollOffsets()
    {
        var newScrollOffset = new Point(-_horizontalScrollBarValue.Value, -_verticalScrollBarValue.Value);
        if (!newScrollOffset.Equals(_htmlContainer.ScrollOffset))
        {
            _htmlContainer.ScrollOffset = newScrollOffset;
            InvalidateVisual();
        }
    }

    /// <summary>
    /// On text property change reset the scrollbars to zero.
    /// </summary>
    protected override void TextPropertyChanged(string n)
    {
        _horizontalScrollBarValue.Value = 0.0;
        _verticalScrollBarValue.Value = 0.0;

        base.TextPropertyChanged(n);
    }

    #endregion
}
