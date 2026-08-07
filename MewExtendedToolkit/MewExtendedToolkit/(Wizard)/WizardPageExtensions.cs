namespace MewExtendedToolkit;

public static class WizardPageExtensions
{
    public static WizardPage CanCancel(this WizardPage element, bool? canCancel = true)
    {
        element.CanCancel.Value = canCancel;
        return element;
    }

    public static WizardPage CanFinish(this WizardPage element, bool? canFinish = true)
    {
        element.CanFinish.Value = canFinish;
        return element;
    }

    public static WizardPage CanHelp(this WizardPage element, bool? canHelp = true)
    {
        element.CanHelp.Value = canHelp;
        return element;
    }

    public static WizardPage ToolbarText(this WizardPage element, string text)
    {
        element.ToolbarText.Value = text;
        return element;
    }

    public static WizardPage OnEnter(this WizardPage element, Action handler)
    {
        element.Enter += handler;
        return element;
    }

    public static WizardPage OnLeave(this WizardPage element, Action handler)
    {
        element.Leave += handler;
        return element;
    }
}
