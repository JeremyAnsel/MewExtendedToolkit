using Aprillz.MewUI;
using Aprillz.MewUI.Controls;

namespace MewExtendedToolkit;

public sealed class WizardPage : ContentControl
{
    public readonly ObservableValue<bool?> CanCancel = new();
    public readonly ObservableValue<bool?> IsCancelButtonVisible = new();
    public readonly ObservableValue<bool?> CanFinish = new();
    public readonly ObservableValue<bool?> IsFinishButtonVisible = new();
    public readonly ObservableValue<bool?> CanHelp = new();
    public readonly ObservableValue<bool?> IsHelpButtonVisible = new();
    public readonly ObservableValue<bool?> CanSelectNextPage = new();
    public readonly ObservableValue<bool?> IsNextButtonVisible = new();
    public readonly ObservableValue<bool?> CanSelectPreviousPage = new();
    public readonly ObservableValue<bool?> IsPreviousButtonVisible = new();
    public readonly ObservableValue<WizardPage?> NextPage = new();
    public readonly ObservableValue<WizardPage?> PreviousPage = new();
    public readonly ObservableValue<string> Title = new(string.Empty);
    public readonly ObservableValue<string> Description = new(string.Empty);
    public readonly ObservableValue<string> ToolbarText = new(string.Empty);
    public readonly ObservableValue<Element?> ExteriorPanelContent = new();
    public readonly ObservableValue<double> ExteriorPanelMinWidth = new(1);

    public event Action? Enter;
    public event Action? Leave;

    public WizardPage()
    {
        BuildContent();
    }

    public WizardPage(string title, string description)
        : this()
    {
        Title.Value = title;
        Description.Value = description;
    }

    protected override void OnGotFocus()
    {
        base.OnGotFocus();
        Enter?.Invoke();
    }

    protected override void OnLostFocus()
    {
        Leave?.Invoke();
        base.OnLostFocus();
    }

    protected override void OnVisualRootChanged(Element? oldRoot, Element? newRoot)
    {
        base.OnVisualRootChanged(oldRoot, newRoot);

        Wizard? wizard = FindParentWizard();

        if (wizard is not null)
        {
            ExteriorPanelMinWidth.Value = wizard.ExteriorPanelMinWidth.Value;
        }
    }

    public Wizard? FindParentWizard()
    {
        Element current = this;

        while (current.Parent is not null)
        {
            if (current.Parent is Wizard wizard)
            {
                return wizard;
            }

            current = current.Parent;
        }

        return null;
    }

    private void BuildContent()
    {
        var template = new DelegateControlTemplate<ContentControl>((owner, ctx) =>
        {
            var grid = new Grid()
                .Columns(GridLength.Auto, GridLength.Star)
                .Children(
                    new Border()
                        .Column(0)
                        .Bind(MinWidthProperty, ExteriorPanelMinWidth)
                        .Child(
                            new ContentControl()
                                .Bind(ContentProperty, ExteriorPanelContent)
                        ),
                    new Grid()
                        .Column(1)
                        .Margin(14, 0, 0, 0)
                        .Rows(GridLength.Auto, GridLength.Auto, GridLength.Star)
                        .Children(
                            new TextBlock()
                                .Row(0)
                                .Margin(0, 0, 0, 14)
                                .FontSize(18)
                                .FontWeight(FontWeight.Bold)
                                .TextWrapping(TextWrapping.Wrap)
                                .BindText(Title),
                            new TextBlock()
                                .Row(1)
                                .Margin(0, 0, 0, 14)
                                .TextWrapping(TextWrapping.Wrap)
                                .BindText(Description),
                            new ContentPresenter()
                                .Row(2)
                        )
                );

            return grid;
        });

        Template = template;
    }
}
