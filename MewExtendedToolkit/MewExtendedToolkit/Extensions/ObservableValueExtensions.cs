using Aprillz.MewUI;

namespace MewExtendedToolkit;

public static class ObservableValueExtensions
{
    public static void SetInitAndChanged(this ObservableValue<bool> src, ObservableValue<bool> dst)
    {
        dst.Value = src.Value;
        src.Changed += () => dst.Value = src.Value;
    }

    public static void SetInitAndChanged(this ObservableValue<bool> src, ObservableValue<bool?> dst)
    {
        dst.Value = src.Value;
        src.Changed += () => dst.Value = src.Value;
    }
}
