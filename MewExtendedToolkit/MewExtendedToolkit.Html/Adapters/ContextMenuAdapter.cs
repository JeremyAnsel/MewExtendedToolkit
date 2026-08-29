using Aprillz.MewUI.Controls;
using MewExtendedToolkit.Html.Utilities;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace MewExtendedToolkit.Html.Adapters;

/// <summary>
/// Adapter for MewUI context menu for core.
/// </summary>
internal sealed class ContextMenuAdapter : RContextMenu
{
    #region Fields and Consts

    /// <summary>
    /// the underline MewUI context menu
    /// </summary>
    private readonly ContextMenu _contextMenu;

    #endregion


    /// <summary>
    /// Init.
    /// </summary>
    public ContextMenuAdapter()
    {
        _contextMenu = new ContextMenu();
    }

    public override int ItemsCount
    {
        get { return _contextMenu.Items.Count; }
    }

    public override void AddDivider()
    {
        _contextMenu.Items.Add(MenuSeparator.Instance);
    }

    public override void AddItem(string text, bool enabled, EventHandler onClick)
    {
        ArgChecker.AssertArgNotNullOrEmpty(text, nameof(text));
        ArgChecker.AssertArgNotNull(onClick, nameof(onClick));

        var item = new MenuItem
        {
            Text = text,
            IsEnabled = enabled
        };

        //item.Click += () => onClick(item, EventArgs.Empty);

        _contextMenu.Items.Add(item);
        throw new NotSupportedException("MenuItem.Click is not supported");
    }

    public override void RemoveLastDivider()
    {
        if (_contextMenu.Items[_contextMenu.Items.Count - 1] == MenuSeparator.Instance)
        {
            _contextMenu.Items.RemoveAt(_contextMenu.Items.Count - 1);
        }
    }

    public override void Show(RControl parent, RPoint location)
    {
        _contextMenu.ShowAt(((ControlAdapter)parent).Control, Utils.ConvertRound(location));
    }

    public override void Dispose()
    {
        _contextMenu.IsEnabled = false;
        _contextMenu.Items.Clear();
    }
}
