using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using Aprillz.MewUI.Rendering;
using System.Numerics;

namespace MewExtendedToolkit;

/// <summary>
/// Decorator that applies a composed Matrix3x2 transform before rendering the child.
/// Transform order: Translate(-origin) → Scale → Rotate → Translate(+origin + offset).
/// Origin is relative (0.0–1.0) to the content bounds; defaults to center (0.5, 0.5).
/// </summary>
public sealed class TransformBox : FrameworkElement, IVisualTreeHost
{
    public static readonly MewProperty<UIElement?> ChildProperty =
        MewProperty<UIElement?>.Register<TransformBox>(nameof(Child), null,
            MewPropertyOptions.AffectsLayout,
            static (self, oldValue, newValue) => self.OnChildChanged(oldValue, newValue));

    private void OnChildChanged(UIElement? oldValue, UIElement? newValue)
    {
        if (oldValue != null) DetachChild(oldValue);
        if (newValue != null) AttachChild(newValue);
    }

    public static readonly MewProperty<double> TranslateXProperty =
        MewProperty<double>.Register<TransformBox>(nameof(TranslateX), 0.0, MewPropertyOptions.AffectsRender | MewPropertyOptions.AffectsLayout);

    public static readonly MewProperty<double> TranslateYProperty =
        MewProperty<double>.Register<TransformBox>(nameof(TranslateY), 0.0, MewPropertyOptions.AffectsRender | MewPropertyOptions.AffectsLayout);

    public static readonly MewProperty<double> RotationDegreesProperty =
        MewProperty<double>.Register<TransformBox>(nameof(RotationDegrees), 0.0, MewPropertyOptions.AffectsRender | MewPropertyOptions.AffectsLayout);

    public static readonly MewProperty<double> ScaleXProperty =
        MewProperty<double>.Register<TransformBox>(nameof(ScaleX), 1.0, MewPropertyOptions.AffectsRender | MewPropertyOptions.AffectsLayout);

    public static readonly MewProperty<double> ScaleYProperty =
        MewProperty<double>.Register<TransformBox>(nameof(ScaleY), 1.0, MewPropertyOptions.AffectsRender | MewPropertyOptions.AffectsLayout);

    public UIElement? Child
    {
        get => GetValue(ChildProperty);
        set => SetValue(ChildProperty, value);
    }

    public double TranslateX
    {
        get => GetValue(TranslateXProperty);
        set => SetValue(TranslateXProperty, value);
    }

    public double TranslateY
    {
        get => GetValue(TranslateYProperty);
        set => SetValue(TranslateYProperty, value);
    }

    public double RotationDegrees
    {
        get => GetValue(RotationDegreesProperty);
        set => SetValue(RotationDegreesProperty, value);
    }

    public double ScaleX
    {
        get => GetValue(ScaleXProperty);
        set => SetValue(ScaleXProperty, value);
    }

    public double ScaleY
    {
        get => GetValue(ScaleYProperty);
        set => SetValue(ScaleYProperty, value);
    }

    public bool HasTransform =>
        TranslateX != 0 || TranslateY != 0
        || RotationDegrees != 0
        || ScaleX != 1.0 || ScaleY != 1.0;

    protected override Size MeasureContent(Size availableSize)
    {
        var child = Child;
        if (child is null) return Size.Empty;

        child.Measure(availableSize);

        var transform = Matrix3x2.Identity;

        var sx = ScaleX;
        var sy = ScaleY;
        if (sx != 1.0 || sy != 1.0)
        {
            transform *= Matrix3x2.CreateScale((float)Math.Abs(sx), (float)Math.Abs(sy));
        }

        var rotation = RotationDegrees;
        if (rotation != 0)
        {
            transform *= Matrix3x2.CreateRotation((float)(rotation * (Math.PI / 180.0)));
        }

        Vector2 point = TransformPoint(new Vector2((float)child.DesiredSize.Width, (float)child.DesiredSize.Height), transform);
        point += new Vector2((float)TranslateX, (float)TranslateY);
        return new Size(point.X + 1, point.Y + 1);
    }

    protected override void ArrangeContent(Rect bounds)
    {
        var child = Child;
        if (child is null) return;

        var size = child.DesiredSize;
        bounds = bounds.WithSize(size);

        child.Arrange(bounds);
    }

    protected override UIElement? OnHitTest(Point point)
    {
        if (!IsVisible || !IsHitTestVisible) return null;

        var child = Child;
        if (child is not null)
        {
            var bounds = Bounds;
            var desiredSize = child.DesiredSize;

            float cx = (float)(bounds.X + desiredSize.Height * Math.Abs(ScaleY) * 0.5);
            float cy = (float)(bounds.Y + desiredSize.Height * Math.Abs(ScaleY) * 0.5);

            var transform = Matrix3x2.Identity;

            var sx = ScaleX;
            var sy = ScaleY;
            if (sx != 1.0 || sy != 1.0)
            {
                float x = (float)bounds.X;
                float y = (float)bounds.Y;
                transform *= Matrix3x2.CreateTranslation(-x, -y);
                transform *= Matrix3x2.CreateScale((float)sx, (float)sy);
                if (sx < 0.0) x -= (float)(desiredSize.Width * sx);
                if (sy < 0.0) y -= (float)(desiredSize.Height * sy);
                transform *= Matrix3x2.CreateTranslation(x, y);
            }

            transform *= Matrix3x2.CreateTranslation(-cx, -cy);

            var rotation = RotationDegrees;
            if (rotation != 0)
            {
                transform *= Matrix3x2.CreateRotation((float)(rotation * (Math.PI / 180.0)));
            }

            transform *= Matrix3x2.CreateTranslation(cx + (float)TranslateX, cy + (float)TranslateY);

            Matrix3x2.Invert(transform, out transform);
            Vector2 v = Vector2.Transform(new Vector2((float)point.X, (float)point.Y), transform);
            Point p = new Point(v.X, v.Y);

            if (child.HitTest(p) is UIElement hit)
            {
                return hit;
            }
        }

        return Bounds.Contains(point) ? this : null;
    }

    private Vector2 TransformPoint(Vector2 point, Matrix3x2 transform)
    {
        Vector2 point00 = Vector2.Transform(new Vector2(0.0f, 0.0f), transform);
        Vector2 point01 = Vector2.Transform(new Vector2(0.0f, point.Y), transform);
        Vector2 point10 = Vector2.Transform(new Vector2(point.X, 0.0f), transform);
        Vector2 point11 = Vector2.Transform(new Vector2(point.X, point.Y), transform);

        float maxX = Math.Max(Math.Max(point00.X, point01.X), Math.Max(point10.X, point11.X));
        float maxY = Math.Max(Math.Max(point00.Y, point01.Y), Math.Max(point10.Y, point11.Y));
        float minX = Math.Min(Math.Min(point00.X, point01.X), Math.Min(point10.X, point11.X));
        float minY = Math.Min(Math.Min(point00.Y, point01.Y), Math.Min(point10.Y, point11.Y));
        point = new Vector2(maxX - minX, maxY - minY);

        return point;
    }

    protected override void RenderSubtree(IGraphicsContext context)
    {
        var child = Child;
        if (child is null) return;

        var bounds = Bounds;

        context.Save();
        context.SetClip(bounds);

        var current = context.GetTransform();
        var desiredSize = child.DesiredSize;

        float cx = (float)(bounds.X + desiredSize.Height * Math.Abs(ScaleY) * 0.5);
        float cy = (float)(bounds.Y + desiredSize.Height * Math.Abs(ScaleY) * 0.5);

        var transform = current;

        var sx = ScaleX;
        var sy = ScaleY;
        if (sx != 1.0 || sy != 1.0)
        {
            float x = (float)bounds.X;
            float y = (float)bounds.Y;
            transform *= Matrix3x2.CreateTranslation(-x, -y);
            transform *= Matrix3x2.CreateScale((float)sx, (float)sy);
            if (sx < 0.0) x -= (float)(desiredSize.Width * sx);
            if (sy < 0.0) y -= (float)(desiredSize.Height * sy);
            transform *= Matrix3x2.CreateTranslation(x, y);
        }

        transform *= Matrix3x2.CreateTranslation(-cx, -cy);

        var rotation = RotationDegrees;
        if (rotation != 0)
        {
            transform *= Matrix3x2.CreateRotation((float)(rotation * (Math.PI / 180.0)));
        }

        transform *= Matrix3x2.CreateTranslation(cx + (float)TranslateX, cy + (float)TranslateY);

        context.SetTransform(transform);

        child.Render(context);

        context.Restore();
    }

    bool IVisualTreeHost.VisitChildren(Func<Element, bool> visitor)
        => Child == null || visitor(Child);
}
