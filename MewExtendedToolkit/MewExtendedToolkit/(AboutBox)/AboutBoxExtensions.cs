namespace MewExtendedToolkit;

public static class AboutBoxExtensions
{
    public static AboutBox Details(this AboutBox element, string details)
    {
        element.Details = details;
        return element;
    }
}
