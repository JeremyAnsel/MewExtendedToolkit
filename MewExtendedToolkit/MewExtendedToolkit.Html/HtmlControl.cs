using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using Aprillz.MewUI.Rendering;
using MewExtendedToolkit.Html.Adapters;
using System.ComponentModel;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.Core.Entities;

namespace MewExtendedToolkit.Html;

/// <summary>
/// Provides HTML rendering using the text property.<br/>
/// MewUI control that will render html content in it's client rectangle.<br/>
/// The control will handle mouse and keyboard events on it to support html text selection, copy-paste and mouse clicks.<br/>
/// <para>
/// The major differential to use HtmlPanel or HtmlLabel is size and scrollbars.<br/>
/// If the size of the control depends on the html content the HtmlLabel should be used.<br/>
/// If the size is set by some kind of layout then HtmlPanel is more suitable, also shows scrollbars if the html contents is larger than the control client rectangle.<br/>
/// </para>
/// <para>
/// <h4>LinkClicked event:</h4>
/// Raised when the user clicks on a link in the html.<br/>
/// Allows canceling the execution of the link.
/// </para>
/// <para>
/// <h4>StylesheetLoad event:</h4>
/// Raised when a stylesheet is about to be loaded by file path or URI by link element.<br/>
/// This event allows to provide the stylesheet manually or provide new source (file or uri) to load from.<br/>
/// If no alternative data is provided the original source will be used.<br/>
/// </para>
/// <para>
/// <h4>ImageLoad event:</h4>
/// Raised when an image is about to be loaded by file path or URI.<br/>
/// This event allows to provide the image manually, if not handled the image will be loaded from file or download from URI.
/// </para>
/// <para>
/// <h4>RenderError event:</h4>
/// Raised when an error occurred during html rendering.<br/>
/// </para>
/// </summary>
public abstract class HtmlControl : Control
{
    #region Fields and Consts

    /// <summary>
    /// Underline html container instance.
    /// </summary>
    protected readonly HtmlContainer _htmlContainer;

    /// <summary>
    /// the base stylesheet data used in the control
    /// </summary>
    protected CssData? _baseCssData;

    /// <summary>
    /// The last position of the scrollbars to know if it has changed to update mouse
    /// </summary>
    protected Point _lastScrollOffset;

    #endregion


    #region Dependency properties / routed events

    public static readonly MewProperty<bool> AvoidImagesLateLoadingProperty =
        MewProperty<bool>.Register<HtmlControl>(nameof(AvoidImagesLateLoading), false, MewPropertyOptions.None, (s, o, n) => s.AvoidImagesLateLoadingPropertyChanged(n));

    public static readonly MewProperty<bool> IsSelectionEnabledProperty =
        MewProperty<bool>.Register<HtmlControl>(nameof(IsSelectionEnabled), true, MewPropertyOptions.None, (s, o, n) => s.IsSelectionEnabledPropertyChanged(n));

    public static readonly MewProperty<bool> IsContextMenuEnabledProperty =
        MewProperty<bool>.Register<HtmlControl>(nameof(IsContextMenuEnabled), true, MewPropertyOptions.None, (s, o, n) => s.IsContextMenuEnabledPropertyChanged(n));

    public static readonly MewProperty<string> BaseStylesheetProperty =
        MewProperty<string>.Register<HtmlControl>(nameof(BaseStylesheet), string.Empty, MewPropertyOptions.None, (s, o, n) => s.BaseStylesheetPropertyChanged(n));

    public static readonly MewProperty<string> TextProperty =
        MewProperty<string>.Register<HtmlControl>(nameof(Text), string.Empty, MewPropertyOptions.None, (s, o, n) => s.TextPropertyChanged(n));

    private void AvoidImagesLateLoadingPropertyChanged(bool n)
    {
        _htmlContainer.AvoidImagesLateLoading = n;
    }

    private void IsSelectionEnabledPropertyChanged(bool n)
    {
        _htmlContainer.IsSelectionEnabled = n;
    }

    private void IsContextMenuEnabledPropertyChanged(bool n)
    {
        _htmlContainer.IsContextMenuEnabled = n;
    }

    private void BaseStylesheetPropertyChanged(string n)
    {
        var baseCssData = CssData.Parse(MewAdapter.Instance, n, true);
        _baseCssData = baseCssData;
        _htmlContainer.SetHtml(Text, baseCssData);
    }

    protected virtual void TextPropertyChanged(string n)
    {
        _htmlContainer.ScrollOffset = new Point(0, 0);
        _htmlContainer.SetHtml(n, _baseCssData);
        InvalidateMeasure();
        InvalidateVisual();
        InvokeMouseMove();
    }

    /// <summary>
    /// Raised when the set html document has been fully loaded.<br/>
    /// Allows manipulation of the html dom, scroll position, etc.
    /// </summary>
    public static event EventHandler<EventArgs>? LoadComplete;

    /// <summary>
    /// Raised when the user clicks on a link in the html.<br/>
    /// Allows canceling the execution of the link.
    /// </summary>
    public static event EventHandler<HtmlLinkClickedEventArgs>? LinkClicked;

    /// <summary>
    /// Raised when an error occurred during html rendering.<br/>
    /// </summary>
    public static event EventHandler<HtmlRenderErrorEventArgs>? RenderError;

    //public static event EventHandler<HtmlRefreshEventArgs>? Refresh;

    /// <summary>
    /// Raised when a stylesheet is about to be loaded by file path or URI by link element.<br/>
    /// This event allows to provide the stylesheet manually or provide new source (file or uri) to load from.<br/>
    /// If no alternative data is provided the original source will be used.<br/>
    /// </summary>
    public static event EventHandler<HtmlStylesheetLoadEventArgs>? StylesheetLoad;

    /// <summary>
    /// Raised when an image is about to be loaded by file path or URI.<br/>
    /// This event allows to provide the image manually, if not handled the image will be loaded from file or download from URI.
    /// </summary>
    public static event EventHandler<HtmlImageLoadEventArgs>? ImageLoad;

    #endregion


    /// <summary>
    /// Creates a new HtmlPanel and sets a basic css for it's styling.
    /// </summary>
    protected HtmlControl()
    {
        _htmlContainer = new HtmlContainer();
        _htmlContainer.LoadComplete += OnLoadComplete;
        _htmlContainer.LinkClicked += OnLinkClicked;
        _htmlContainer.RenderError += OnRenderError;
        _htmlContainer.Refresh += OnRefresh;
        _htmlContainer.StylesheetLoad += OnStylesheetLoad;
        _htmlContainer.ImageLoad += OnImageLoad;

        _htmlContainer.IsSelectionEnabled = false;
        _htmlContainer.IsContextMenuEnabled = false;
    }

    /// <summary>
    /// Gets or sets a value indicating if image loading only when visible should be avoided (default - false).<br/>
    /// True - images are loaded as soon as the html is parsed.<br/>
    /// False - images that are not visible because of scroll location are not loaded until they are scrolled to.
    /// </summary>
    /// <remarks>
    /// Images late loading improve performance if the page contains image outside the visible scroll area, especially if there is large 
    /// amount of images, as all image loading is delayed (downloading and loading into memory).<br/>
    /// Late image loading may effect the layout and actual size as image without set size will not have actual size until they are loaded
    /// resulting in layout change during user scroll.<br/>
    /// Early image loading may also effect the layout if image without known size above the current scroll location are loaded as they
    /// will push the html elements down.
    /// </remarks>
    [Category("Behavior")]
    [Description("If image loading only when visible should be avoided")]
    public bool AvoidImagesLateLoading
    {
        get { return (bool)GetValue(AvoidImagesLateLoadingProperty); }
        set { SetValue(AvoidImagesLateLoadingProperty, value); }
    }

    /// <summary>
    /// Is content selection is enabled for the rendered html (default - true).<br/>
    /// If set to 'false' the rendered html will be static only with ability to click on links.
    /// </summary>
    [Category("Behavior")]
    [Description("Is content selection is enabled for the rendered html.")]
    public bool IsSelectionEnabled
    {
        get { return (bool)GetValue(IsSelectionEnabledProperty); }
        set { SetValue(IsSelectionEnabledProperty, value); }
    }

    /// <summary>
    /// Is the build-in context menu enabled and will be shown on mouse right click (default - true)
    /// </summary>
    [Category("Behavior")]
    [Description("Is the build-in context menu enabled and will be shown on mouse right click.")]
    public bool IsContextMenuEnabled
    {
        get { return (bool)GetValue(IsContextMenuEnabledProperty); }
        set { SetValue(IsContextMenuEnabledProperty, value); }
    }

    /// <summary>
    /// Set base stylesheet to be used by html rendered in the panel.
    /// </summary>
    [Category("Appearance")]
    [Description("Set base stylesheet to be used by html rendered in the control.")]
    public string BaseStylesheet
    {
        get { return (string)GetValue(BaseStylesheetProperty); }
        set { SetValue(BaseStylesheetProperty, value); }
    }

    /// <summary>
    /// Gets or sets the text of this panel
    /// </summary>
    [Description("Sets the html of this control.")]
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    /// <summary>
    /// Get the currently selected text segment in the html.
    /// </summary>
    [Browsable(false)]
    public virtual string SelectedText
    {
        get { return _htmlContainer.SelectedText; }
    }

    /// <summary>
    /// Copy the currently selected html segment with style.
    /// </summary>
    [Browsable(false)]
    public virtual string SelectedHtml
    {
        get { return _htmlContainer.SelectedHtml; }
    }

    /// <summary>
    /// Get html from the current DOM tree with inline style.
    /// </summary>
    /// <returns>generated html</returns>
    public virtual string? GetHtml()
    {
        return _htmlContainer?.GetHtml();
    }

    /// <summary>
    /// Get the rectangle of html element as calculated by html layout.<br/>
    /// Element if found by id (id attribute on the html element).<br/>
    /// Note: to get the screen rectangle you need to adjust by the hosting control.<br/>
    /// </summary>
    /// <param name="elementId">the id of the element to get its rectangle</param>
    /// <returns>the rectangle of the element or null if not found</returns>
    public virtual Rect? GetElementRectangle(string elementId)
    {
        return _htmlContainer?.GetElementRectangle(elementId);
    }

    /// <summary>
    /// Clear the current selection.
    /// </summary>
    public void ClearSelection()
    {
        _htmlContainer?.ClearSelection();
    }


    #region Private methods

    /// <summary>
    /// Perform paint of the html in the control.
    /// </summary>
    protected override void OnRender(IGraphicsContext context)
    {
        var htmlWidth = HtmlWidth(RenderSize);
        var htmlHeight = HtmlHeight(RenderSize);

        if (_htmlContainer != null && htmlWidth > 0 && htmlHeight > 0)
        {
            var currentTransform = context.GetTransform();

            if (FindVisualRoot() is Window windows)
            {
                // adjust render location to round point so we won't get anti-alias smugness
                var wPoint = TranslatePoint(new Point(0, 0), windows);
                var wPoint2 = wPoint.Offset(-(int)wPoint.X, -(int)wPoint.Y);
                var xTrans = wPoint2.X < .5 ? -wPoint2.X : 1 - wPoint2.X;
                var yTrans = wPoint2.Y < .5 ? -wPoint2.Y : 1 - wPoint2.Y;
                xTrans += wPoint.X;
                yTrans += wPoint.Y;
                context.SetTransform(System.Numerics.Matrix3x2.CreateTranslation((float)xTrans, (float)yTrans));
            }

            if (Background.A > 0)
            {
                context.FillRectangle(new Rect(RenderSize), Background);
            }

            if (BorderThickness != new Thickness(0))
            {
                var brush = BorderBrush;
                if (BorderThickness > 0)
                    context.DrawRectangle(new Rect(0, 0, RenderSize.Width, BorderThickness), brush);
                if (BorderThickness > 0)
                    context.DrawRectangle(new Rect(0, RenderSize.Height - BorderThickness, RenderSize.Width, BorderThickness), brush);
                if (BorderThickness > 0)
                    context.DrawRectangle(new Rect(0, 0, BorderThickness, RenderSize.Height), brush);
                if (BorderThickness > 0)
                    context.DrawRectangle(new Rect(RenderSize.Width - BorderThickness, 0, BorderThickness, RenderSize.Height), brush);
            }

            context.Save();
            context.ResetClip();
            context.SetClip(new Rect(Padding.Left + BorderThickness, Padding.Top + BorderThickness, (int)htmlWidth, (int)htmlHeight));
            _htmlContainer.Location = new Point(Padding.Left + BorderThickness, Padding.Top + BorderThickness);
            _htmlContainer.PerformPaint(context, new Rect(Padding.Left + BorderThickness, Padding.Top + BorderThickness, htmlWidth, htmlHeight));
            context.ResetClip();
            context.Restore();

            context.SetTransform(currentTransform);

            if (!_lastScrollOffset.Equals(_htmlContainer.ScrollOffset))
            {
                _lastScrollOffset = _htmlContainer.ScrollOffset;
                InvokeMouseMove();
            }
        }
    }

    protected Point _mouseMovePoint = Point.Zero;

    /// <summary>
    /// Handle mouse move to handle hover cursor and text selection. 
    /// </summary>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        _mouseMovePoint = e.GetPosition(this);
        _htmlContainer?.HandleMouseMove(this, _mouseMovePoint);
    }

    /// <summary>
    /// Handle mouse leave to handle cursor change.
    /// </summary>
    protected override void OnMouseLeave()
    {
        base.OnMouseLeave();
        _htmlContainer?.HandleMouseLeave(this);
    }

    /// <summary>
    /// Handle mouse down to handle selection. 
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _htmlContainer?.HandleMouseDown(this, e);
    }

    /// <summary>
    /// Handle mouse up to handle selection and link click. 
    /// </summary>
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _htmlContainer?.HandleMouseUp(this, e);
    }

    /// <summary>
    /// Handle mouse double click to select word under the mouse. 
    /// </summary>
    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e);
        _htmlContainer?.HandleMouseDoubleClick(this, e);
    }

    /// <summary>
    /// Handle key down event for selection, copy and scrollbars handling.
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        _htmlContainer?.HandleKeyDown(this, e);
    }

    /// <summary>
    /// Propagate the LoadComplete event from root container.
    /// </summary>
    protected virtual void OnLoadComplete(EventArgs e)
    {
        LoadComplete?.Invoke(this, e);
    }

    /// <summary>
    /// Propagate the LinkClicked event from root container.
    /// </summary>
    protected virtual void OnLinkClicked(HtmlLinkClickedEventArgs e)
    {
        LinkClicked?.Invoke(this, e);
    }

    /// <summary>
    /// Propagate the Render Error event from root container.
    /// </summary>
    protected virtual void OnRenderError(HtmlRenderErrorEventArgs e)
    {
        RenderError?.Invoke(this, e);
    }

    /// <summary>
    /// Propagate the stylesheet load event from root container.
    /// </summary>
    protected virtual void OnStylesheetLoad(HtmlStylesheetLoadEventArgs e)
    {
        StylesheetLoad?.Invoke(this, e);
    }

    /// <summary>
    /// Propagate the image load event from root container.
    /// </summary>
    protected virtual void OnImageLoad(HtmlImageLoadEventArgs e)
    {
        ImageLoad?.Invoke(this, e);
    }

    /// <summary>
    /// Handle html renderer invalidate and re-layout as requested.
    /// </summary>
    protected virtual void OnRefresh(HtmlRefreshEventArgs e)
    {
        if (e.Layout)
        {
            InvalidateMeasure();
        }
        InvalidateVisual();
    }

    /// <summary>
    /// Get the width the HTML has to render in (not including vertical scroll iff it is visible)
    /// </summary>
    protected virtual double HtmlWidth(Size size)
    {
        return size.Width - Padding.Left - Padding.Right - BorderThickness - BorderThickness;
    }

    /// <summary>
    /// Get the width the HTML has to render in (not including vertical scroll iff it is visible)
    /// </summary>
    protected virtual double HtmlHeight(Size size)
    {
        return size.Height - Padding.Top - Padding.Bottom - BorderThickness - BorderThickness;
    }

    /// <summary>
    /// call mouse move to handle paint after scroll or html change affecting mouse cursor.
    /// </summary>
    protected virtual void InvokeMouseMove()
    {
        _htmlContainer.HandleMouseMove(this, _mouseMovePoint);
    }

    #region Private event handlers

    private void OnLoadComplete(object? sender, EventArgs e)
    {
        if (CheckAccess())
            OnLoadComplete(e);
        else
            Application.Current.Dispatcher!.Invoke(() => OnLoadComplete(e));
    }

    private void OnLinkClicked(object? sender, HtmlLinkClickedEventArgs e)
    {
        if (CheckAccess())
            OnLinkClicked(e);
        else
            Application.Current.Dispatcher!.Invoke(() => OnLinkClicked(e));
    }

    private void OnRenderError(object? sender, HtmlRenderErrorEventArgs e)
    {
        if (CheckAccess())
            OnRenderError(e);
        else
            Application.Current.Dispatcher!.Invoke(() => OnRenderError(e));
    }

    private void OnStylesheetLoad(object? sender, HtmlStylesheetLoadEventArgs e)
    {
        if (CheckAccess())
            OnStylesheetLoad(e);
        else
            Application.Current.Dispatcher!.Invoke(() => OnStylesheetLoad(e));
    }

    private void OnImageLoad(object? sender, HtmlImageLoadEventArgs e)
    {
        if (CheckAccess())
            OnImageLoad(e);
        else
            Application.Current.Dispatcher!.Invoke(() => OnImageLoad(e));
    }

    private void OnRefresh(object? sender, HtmlRefreshEventArgs e)
    {
        if (CheckAccess())
            OnRefresh(e);
        else
            Application.Current.Dispatcher!.Invoke(() => OnRefresh(e));
    }

    private static bool CheckAccess()
    {
        return Application.Current.Dispatcher?.IsOnUIThread ?? true;
    }

    #endregion


    #endregion
}
