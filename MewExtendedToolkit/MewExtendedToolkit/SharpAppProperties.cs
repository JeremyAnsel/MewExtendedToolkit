using MewExtendedToolkit.Helpers;
using System.Reflection;

namespace MewExtendedToolkit;

public static class SharpAppProperties
{
    private static Assembly? AppAssembly => Assembly.GetEntryAssembly();

    /// <summary>
    /// The description.
    /// </summary>
    public static string Description => AssemblyHelpers.GetDescription(AppAssembly) ?? string.Empty;

    /// <summary>
    /// The company.
    /// </summary>
    public static string Company => AssemblyHelpers.GetCompany(AppAssembly) ?? string.Empty;

    /// <summary>
    /// The copyright.
    /// </summary>
    public static string Copyright => AssemblyHelpers.GetCopyright(AppAssembly) ?? string.Empty;

    /// <summary>
    /// The informational version.
    /// </summary>
    public static string InformationalVersion => AssemblyHelpers.GetInformationalVersion(AppAssembly) ?? string.Empty;

    /// <summary>
    /// The product.
    /// </summary>
    public static string Product => AssemblyHelpers.GetProduct(AppAssembly) ?? string.Empty;

    /// <summary>
    /// The title.
    /// </summary>
    public static string Title => AssemblyHelpers.GetTitle(AppAssembly) ?? string.Empty;

    /// <summary>
    /// The version.
    /// </summary>
    public static Version? Version => AssemblyHelpers.GetVersion(AppAssembly);

    public static DateTime? ReleaseDate => AssemblyHelpers.GetReleaseDate(AppAssembly);
}
