using Aprillz.MewUI;
using Aprillz.MewUI.Controls;

namespace MewExtendedToolkit;

public sealed class AboutBox : Window
{
    public AboutBox()
    {
        BuildContent();
    }

    private readonly ObservableValue<string> _details = new(string.Empty);

    public string Details
    {
        get => _details.Value;
        set => _details.Value = value;
    }

    private void BuildContent()
    {
        Image image;

        this.Title(VisualStates.AboutBox_Title)
        .Fixed(320, 300)
        .StartCenterScreen()
        .CenterHorizontal()
        .CenterVertical()
        .Content(
            new Grid()
                .Rows(GridLength.Star, GridLength.Auto, GridLength.Star, GridLength.Auto)
                .Children(
                    new DockPanel()
                        .Row(0)
                        .Children(
                            new Image()
                                .Ref(out image)
                                .DockLeft()
                                .Margin(0, 10)
                                .MaxWidth(100)
                                .MaxHeight(100),
                            new StackPanel()
                                .CenterVertical()
                                .Children(
                                    new TextBlock()
                                        .CenterHorizontal()
                                        .FontWeight(FontWeight.Bold)
                                        .TextWrapping(TextWrapping.Wrap)
                                        .Text(SharpAppProperties.Product),
                                    new TextBlock()
                                        .CenterHorizontal()
                                        .TextWrapping(TextWrapping.Wrap)
                                        .Text("v" + SharpAppProperties.Version),
                                    new TextBlock()
                                        .CenterHorizontal()
                                        .TextWrapping(TextWrapping.Wrap)
                                        .Text(SharpAppProperties.ReleaseDate!.Value.ToString("HH:mm dd-MM-yyyy"))
                                )
                        ),
                    new TextBlock()
                        .Row(1)
                        .TextWrapping(TextWrapping.Wrap)
                        .Text(SharpAppProperties.Description),
                    new MultiLineTextBox()
                        .Row(2)
                        .Margin(0, 10, 0, 10)
                        .IsReadOnly()
                        .Wrap()
                        .BindText(_details),
                    new DockPanel()
                        .Row(3)
                        .Children(
                            new Button()
                                .DockRight()
                                .Margin(5)
                                .Width(75)
                                .Height(25)
                                .Content(VisualStates.AboutBox_Close)
                                .OnClick(() => this.Close()),
                            new TextBlock()
                                .CenterHorizontal()
                                .FontWeight(FontWeight.SemiBold)
                                .TextWrapping(TextWrapping.Wrap)
                                .Text(SharpAppProperties.Copyright)
                        )
                )
        );

        if (Environment.ProcessPath is not null)
        {
            string iconPath = Path.ChangeExtension(Environment.ProcessPath, "ico");

            if (File.Exists(iconPath))
            {
                image.SourceFile(iconPath);
            }
        }
    }
}
