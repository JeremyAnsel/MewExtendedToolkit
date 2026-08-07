using Aprillz.MewUI.Controls;

namespace MewExtendedToolkit;

public static class TransformBoxExtensions
{
    public static TransformBox Child(this TransformBox box, UIElement? child)
    {
        box.Child = child;
        return box;
    }

    public static TransformBox ScaleX(this TransformBox box, double value)
    {
        box.ScaleX = value;
        return box;
    }

    public static TransformBox ScaleY(this TransformBox box, double value)
    {
        box.ScaleY = value;
        return box;
    }

    public static TransformBox Scale(this TransformBox box, double x, double y)
    {
        box.ScaleX = x;
        box.ScaleY = y;
        return box;
    }

    public static TransformBox TranslateX(this TransformBox box, double value)
    {
        box.TranslateX = value;
        return box;
    }

    public static TransformBox TranslateY(this TransformBox box, double value)
    {
        box.TranslateY = value;
        return box;
    }

    public static TransformBox Translate(this TransformBox box, double x, double y)
    {
        box.TranslateX = x;
        box.TranslateY = y;
        return box;
    }

    public static TransformBox RotationDegrees(this TransformBox box, double value)
    {
        box.RotationDegrees = value;
        return box;
    }
}
