using Aprillz.MewUI;
using Aprillz.MewUI.Controls;

namespace MewExtendedToolkit;

public static class InputBox
{
    public static string? ShowPrompt(
        string title,
        string message,
        string? placeholder = null)
    {
        return ShowPrompt(null, title, message, placeholder);
    }

    public static string? ShowPrompt(
        Window? owner,
        string title,
        string message,
        string? placeholder = null)
    {
        string? result = null;
        TextBox input = null!;
        Window dialog = null!;

        new Window()
            .Title(title)
            .Ref(out dialog)
            .FitContentHeight(300, 130)
            .Content(
                new StackPanel()
                    .Spacing(8)
                    .Padding(12)
                    .Children(
                        new TextBlock()
                            .Text(message),

                        new TextBox()
                            .Ref(out input)
                            .Width(260)
                            .Placeholder(placeholder ?? string.Empty),

                        new StackPanel()
                            .Horizontal()
                            .Spacing(6)
                            .Children(
                                new Button()
                                    .Content(VisualStates.InputBox_OK)
                                    .OnClick(() =>
                                    {
                                        result = input.Text;
                                        dialog.Close();
                                    }),

                                new Button()
                                    .Content(VisualStates.InputBox_Cancel)
                                    .OnClick(dialog.Close)
                            )
                    )
            ).ShowDialog(owner);

        return result;
    }
}
