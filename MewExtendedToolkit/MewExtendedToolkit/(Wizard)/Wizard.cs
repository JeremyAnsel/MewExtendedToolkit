using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using System.Collections.ObjectModel;

namespace MewExtendedToolkit;

public sealed class Wizard : Control
{
    public const double ExteriorPanelMinWidthValue = 165.0;

    private readonly ObservableCollection<WizardPage> _pages = new();

    public IReadOnlyList<WizardPage> Pages => _pages;

    public Wizard()
    {
        _pages.CollectionChanged += (s, e) => OnPagesCollectionChanged();
        CurrentPage.Changed += NotifyPageChanged;
        BuildContent();
    }

    private void OnPagesCollectionChanged()
    {
        if (Pages.Count > 0 && CurrentPage.Value is null)
        {
            CurrentPage.Value = Pages[0];
        }
    }

    public Wizard Add(WizardPage page)
    {
        _pages.Add(page);
        return this;
    }

    public Wizard AddRange(params WizardPage[] pages)
    {
        foreach (var page in pages)
        {
            Add(page);
        }
        return this;
    }

    public bool Remove(WizardPage page)
    {
        if (_pages.Remove(page))
        {
            return true;
        }

        return false;
    }

    public void Clear()
    {
        _pages.Clear();
    }

    public WizardPage this[int index] => _pages[index];

    public int Count => _pages.Count;

    public void Insert(int index, WizardPage page)
    {
        _pages.Insert(index, page);
    }

    public void RemoveAt(int index)
    {
        _pages.RemoveAt(index);
    }

    public readonly ObservableValue<Element?> BackButtonContent = new(new TextBlock().Text("< " + VisualStates.Wizard_Back));
    public readonly ObservableValue<bool> IsBackButtonVisible = new(true);
    public readonly ObservableValue<Element?> NextButtonContent = new(new TextBlock().Text(VisualStates.Wizard_Next + " >"));
    public readonly ObservableValue<bool> IsNextButtonVisible = new(true);
    public readonly ObservableValue<bool> CanCancel = new(true);
    public readonly ObservableValue<bool> CancelButtonClosesWindow = new(true);
    public readonly ObservableValue<Element?> CancelButtonContent = new(new TextBlock().Text(VisualStates.Wizard_Cancel));
    public readonly ObservableValue<bool> IsCancelButtonVisible = new(true);
    public readonly ObservableValue<bool> CanFinish = new(false);
    public readonly ObservableValue<bool> CanHelp = new(true);
    public readonly ObservableValue<bool> CanSelectNextPage = new(true);
    public readonly ObservableValue<bool> CanSelectPreviousPage = new(true);
    public readonly ObservableValue<WizardPage?> CurrentPage = new();
    public readonly ObservableValue<bool> FinishButtonClosesWindow = new(true);
    public readonly ObservableValue<Element?> FinishButtonContent = new(new TextBlock().Text(VisualStates.Wizard_Finish));
    public readonly ObservableValue<bool> IsFinishButtonVisible = new(false);
    public readonly ObservableValue<Element?> HelpButtonContent = new(new TextBlock().Text(VisualStates.Wizard_Help));
    public readonly ObservableValue<bool> IsHelpButtonVisible = new(true);
    public readonly ObservableValue<double> ExteriorPanelMinWidth = new(ExteriorPanelMinWidthValue);

    public event Action? PageChanged;
    public event Action? Cancel;
    public event Action? Finish;
    public event Action? Help;
    public event Action? Next;
    public event Action? Previous;

    private void NotifyPageChanged()
    {
        PageChanged?.Invoke();
    }

    private bool CanNotifyCancel()
    {
        return CanCancel.Value && (CurrentPage.Value is null || CurrentPage.Value.CanCancel.Value != false);
    }

    private void NotifyCancel()
    {
        Cancel?.Invoke();

        if (CancelButtonClosesWindow.Value)
        {
            CloseParentWindow(false);
        }
    }

    private bool CanNotifyFinish()
    {
        bool canFinish = CanFinish.Value || CurrentPage.Value?.CanFinish.Value == true;
        IsFinishButtonVisible.Value = canFinish;
        return canFinish;
    }

    private void NotifyFinish()
    {
        Finish?.Invoke();

        if (FinishButtonClosesWindow.Value)
        {
            CloseParentWindow(true);
        }
    }

    private bool CanNotifyHelp()
    {
        return CanHelp.Value && (CurrentPage.Value is null || CurrentPage.Value.CanHelp.Value != false);
    }

    private void NotifyHelp()
    {
        Help?.Invoke();
    }

    private bool CanNotifyNext()
    {
        bool can = CanSelectNextPage.Value && (CurrentPage.Value is null || CurrentPage.Value.CanSelectNextPage.Value != false);
        return can && NextPageExists();
    }

    private void NotifyNext()
    {
        WizardPage? nextPage = null;

        if (CurrentPage.Value is not null)
        {
            Next?.Invoke();

            //check next page
            if (CurrentPage.Value.NextPage.Value is not null)
            {
                nextPage = CurrentPage.Value.NextPage.Value;
            }
            else
            {
                //no next page defined use index
                int currentIndex = _pages.IndexOf(CurrentPage.Value);
                int nextPageIndex = currentIndex + 1;
                if (nextPageIndex < _pages.Count)
                {
                    nextPage = _pages[nextPageIndex];
                }
            }
        }

        CurrentPage.Value = nextPage;
    }

    private bool CanNotifyPrevious()
    {
        bool can = CanSelectPreviousPage.Value && (CurrentPage.Value is null || CurrentPage.Value.CanSelectPreviousPage.Value != false);
        return can && PreviousPageExists();
    }

    private void NotifyPrevious()
    {
        WizardPage? previousPage = null;

        if (CurrentPage.Value is not null)
        {
            Previous?.Invoke();

            //check previous page
            if (CurrentPage.Value.PreviousPage.Value is not null)
            {
                previousPage = CurrentPage.Value.PreviousPage.Value;
            }
            else
            {
                //no previous page defined so use index
                int currentIndex = _pages.IndexOf(CurrentPage.Value);
                int previousPageIndex = currentIndex - 1;
                if (previousPageIndex >= 0 && previousPageIndex < _pages.Count)
                {
                    previousPage = _pages[previousPageIndex];
                }
            }
        }

        CurrentPage.Value = previousPage;
    }

    private void CloseParentWindow(bool dialogResult)
    {
        if (FindVisualRoot() is Window window)
        {
            window.Tag(dialogResult);
            window.Close();
        }
    }

    private bool NextPageExists()
    {
        if (CurrentPage.Value is null)
        {
            return false;
        }

        //check to see if a next page has been specified
        if (CurrentPage.Value.NextPage.Value is not null)
        {
            return true;
        }

        //lets use an index to find the next page
        int currentIndex = _pages.IndexOf(CurrentPage.Value);
        int nextPageIndex = currentIndex + 1;
        if (nextPageIndex < _pages.Count)
        {
            return true;
        }

        return false;
    }

    private bool PreviousPageExists()
    {
        if (CurrentPage.Value is null)
        {
            return false;
        }

        //check to see if a previous page has been specified
        if (CurrentPage.Value.PreviousPage.Value is not null)
        {
            return true;
        }

        //lets use an index to find the next page
        int currentIndex = _pages.IndexOf(CurrentPage.Value);
        int previousPageIndex = currentIndex - 1;
        if (previousPageIndex >= 0 && previousPageIndex < _pages.Count)
        {
            return true;
        }

        return false;
    }

    private void BuildContent()
    {
        var template = new DelegateControlTemplate<Control>((owner, ctx) =>
        {
            var grid = new DockPanel();
            grid.Add(BuildToolbar().DockBottom());
            grid.Add(new Border().Bind(Border.ChildProperty, CurrentPage, t => t));
            return grid;
        });

        Template = template;
    }

    private DockPanel BuildToolbar()
    {
        double buttonMargin = 4.0;

        var toolbar = new StackPanel()
            .Horizontal()
            .Children(
                new Button()
                    .Margin(buttonMargin)
                    .BindContent(HelpButtonContent)
                    .BindIsVisible(IsHelpButtonVisible)
                    .OnCanClick(CanNotifyHelp)
                    .OnClick(NotifyHelp),
                new Button()
                    .Margin(buttonMargin)
                    .BindContent(BackButtonContent)
                    .BindIsVisible(IsBackButtonVisible)
                    .OnCanClick(CanNotifyPrevious)
                    .OnClick(NotifyPrevious),
                new Button()
                    .Margin(buttonMargin)
                    .BindContent(NextButtonContent)
                    //.BindIsVisible(IsNextButtonVisible)
                    .BindIsVisible(IsFinishButtonVisible, t => !t)
                    .OnCanClick(CanNotifyNext)
                    .OnClick(NotifyNext),
                new Button()
                    .Margin(buttonMargin)
                    .BindContent(FinishButtonContent)
                    .BindIsVisible(IsFinishButtonVisible)
                    .OnCanClick(CanNotifyFinish)
                    .OnClick(NotifyFinish),
                new Button()
                    .Margin(buttonMargin)
                    .BindContent(CancelButtonContent)
                    .BindIsVisible(IsCancelButtonVisible)
                    .OnCanClick(CanNotifyCancel)
                    .OnClick(NotifyCancel)
            );

        var toolbarText = new TextBlock()
            .Bind(TextBlock.TextProperty, CurrentPage, page => page?.ToolbarText.Value ?? string.Empty);

        var control = new DockPanel()
            .Children(
                toolbar
                    .DockRight(),
                toolbarText
                    .CenterHorizontal()
            );

        return control;
    }

    public WizardPage? GetPreviousPage(WizardPage page)
    {
        WizardPage? previousPage = page.PreviousPage.Value;

        if (previousPage is null)
        {
            int index = _pages.IndexOf(page) - 1;

            if (index >= 0)
            {
                previousPage = _pages[index];
            }
        }

        return previousPage;
    }

    public WizardPage? GetNextPage(WizardPage page)
    {
        WizardPage? nextPage = page.NextPage.Value;

        if (nextPage is null)
        {
            int index = _pages.IndexOf(page) + 1;

            if (index < _pages.Count)
            {
                nextPage = _pages[index];
            }
        }

        return nextPage;
    }

    public void RemovePage(WizardPage page)
    {
        WizardPage? previousPage = GetPreviousPage(page);
        WizardPage? nextPage = GetNextPage(page);

        if (previousPage is not null)
        {
            previousPage.NextPage.Value = nextPage;
        }

        if (nextPage is not null)
        {
            nextPage.PreviousPage.Value = previousPage;
        }

        if (CurrentPage.Value == page)
        {
            CurrentPage.Value = nextPage;
        }

        _pages.Remove(page);
    }
}
