using Aprillz.MewUI.Controls;

namespace MewExtendedToolkit;

public static class WizardExtensions
{
    public static Wizard BackButtonContent(this Wizard element, Element content)
    {
        element.BackButtonContent.Value = content;
        return element;
    }

    public static Wizard NextButtonContent(this Wizard element, Element content)
    {
        element.NextButtonContent.Value = content;
        return element;
    }

    public static Wizard CancelButtonContent(this Wizard element, Element content)
    {
        element.CancelButtonContent.Value = content;
        return element;
    }

    public static Wizard FinishButtonContent(this Wizard element, Element content)
    {
        element.FinishButtonContent.Value = content;
        return element;
    }

    public static Wizard HelpButtonContent(this Wizard element, Element content)
    {
        element.HelpButtonContent.Value = content;
        return element;
    }

    public static Wizard IsHelpButtonVisible(this Wizard element, bool value)
    {
        element.IsHelpButtonVisible.Value = value;
        return element;
    }

    public static Wizard CancelButtonClosesWindow(this Wizard element, bool value)
    {
        element.CancelButtonClosesWindow.Value = value;
        return element;
    }

    public static Wizard FinishButtonClosesWindow(this Wizard element, bool value)
    {
        element.FinishButtonClosesWindow.Value = value;
        return element;
    }

    public static Wizard ButtonClosesWindow(this Wizard element, bool value)
    {
        element.CancelButtonClosesWindow.Value = value;
        element.FinishButtonClosesWindow.Value = value;
        return element;
    }

    public static Wizard OnPageChanged(this Wizard element, Action handler)
    {
        element.PageChanged += handler;
        return element;
    }

    public static Wizard OnCancel(this Wizard element, Action handler)
    {
        element.Cancel += handler;
        return element;
    }

    public static Wizard OnFinish(this Wizard element, Action handler)
    {
        element.Finish += handler;
        return element;
    }

    public static Wizard OnHelp(this Wizard element, Action handler)
    {
        element.Help += handler;
        return element;
    }

    public static Wizard OnNext(this Wizard element, Action handler)
    {
        element.Next += handler;
        return element;
    }

    public static Wizard OnPrevious(this Wizard element, Action handler)
    {
        element.Previous += handler;
        return element;
    }
}
