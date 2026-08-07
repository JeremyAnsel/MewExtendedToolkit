using System.Reflection;

namespace MewExtendedToolkit.Helpers;

internal static class AssemblyHelpers
{
    public static string? GetDescription(Assembly? assembly)
    {
        return assembly?.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
    }

    public static string? GetCompany(Assembly? assembly)
    {
        return assembly?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
    }

    public static string? GetCopyright(Assembly? assembly)
    {
        return assembly?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
    }

    public static string? GetInformationalVersion(Assembly? assembly)
    {
        return assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    }

    public static string? GetProduct(Assembly? assembly)
    {
        return assembly?.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
    }

    public static string? GetTitle(Assembly? assembly)
    {
        return assembly?.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
    }

    public static Version? GetVersion(Assembly? assembly)
    {
        return assembly?.GetName()?.Version;
    }

    public static DateTime? GetReleaseDate(Assembly? assembly)
    {
        if (assembly is null)
        {
            return null;
        }

        string location;
#if NET8_0_OR_GREATER
        string? processPath = Environment.ProcessPath;
        if (processPath is null)
        {
            return null;
        }
        location = processPath;
#else
        location = assembly.Location;
#endif

        DateTime date = File.GetLastWriteTime(location);
        return date;
    }
}
