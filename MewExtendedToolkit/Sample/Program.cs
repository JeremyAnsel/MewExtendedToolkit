using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using MewExtendedToolkit;
using MewExtendedToolkit.Html;
using Sample;

Win32Platform.Register();
Direct2DBackend.Register();

ThemeManager.Default = ThemeVariant.Dark;

Window window;
new Window()
    .Ref(out window)
    .Title("MewExtendedToolkit Sample")
    .Resizable(620, 360)
    .Padding(12)
    .Content(
        new StackPanel()
        .Spacing(8)
        .Children(
            new Button()
                .Content("AboutBox")
                .OnClick(() => new AboutBox()
                    .Details("This tool uses these components:\n- MewUI by Aprillz: https://github.com/aprillz/MewUI")
                    .ShowDialog()),
            new Button()
                .Content("InputBox")
                .OnClick(() => InputBox.ShowPrompt("Title", "Message", "placeholder text")),
            new Button()
                .Content("TransformBox 1")
                .OnClick(() => new Window()
                    .Title("TransformBox 1")
                    .FitContentSize()
                    .Content(
                        new DockPanel()
                            .Children(
                                new StackPanel()
                                    .Children(
                                        new TransformBox()
                                            .Scale(1, 1)
                                            .Child(new TextBlock().FontSize(48).Text("Hello, World!")),
                                        new TransformBox()
                                            .Scale(-1, 1)
                                            .Child(new TextBlock().FontSize(48).Text("Hello, World!")),
                                        new TransformBox()
                                            .Scale(-1, -1)
                                            .Child(new TextBlock().FontSize(48).Text("Hello, World!")),
                                        new TransformBox()
                                            .Scale(1, -1)
                                            .Child(new TextBlock().FontSize(48).Text("Hello, World!")),
                                        new TransformBox()
                                            .RotationDegrees(90)
                                            .Child(new TextBlock().FontSize(48).Text("Hello, World!"))
                                    )
                            )
                    )
                    .ShowDialog()),
            new Button()
                .Content("TransformBox 2")
                .OnClick(() => new Window()
                    .Title("TransformBox 2")
                    .FitContentSize()
                    //.Resizable(500, 500)
                    .Content(
                        new DockPanel()
                            .Children(
                                new Slider()
                                    .Ref(out var transformBoxRotation)
                                    .DockTop()
                                    .Width(150)
                                    .Range(0, 90)
                                    .Value(30),
                                new Slider()
                                    .Ref(out var transformBoxSx)
                                    .DockTop()
                                    .Width(150)
                                    .Range(-3, 3)
                                    .Value(-2),
                                new Slider()
                                    .Ref(out var transformBoxSy)
                                    .DockTop()
                                    .Width(150)
                                    .Range(-3, 3)
                                    .Value(1),
                                new Slider()
                                    .Ref(out var transformBoxTx)
                                    .DockTop()
                                    .Width(150)
                                    .Range(-300, 300)
                                    .Value(0),
                                new Slider()
                                    .Ref(out var transformBoxTy)
                                    .DockTop()
                                    .Width(150)
                                    .Range(-300, 300)
                                    .Value(0),
                                new StackPanel()
                                    .DockTop()
                                    .Horizontal()
                                    .Spacing(4)
                                    .Children(
                                        new TextBlock().Text("R"),
                                        new TextBlock().Bind(TextBlock.TextProperty, transformBoxRotation, Slider.ValueProperty, t => t.ToString("F2")),
                                        new TextBlock().Text("Sx"),
                                        new TextBlock().Bind(TextBlock.TextProperty, transformBoxSx, Slider.ValueProperty, t => t.ToString("F2")),
                                        new TextBlock().Text("Sy"),
                                        new TextBlock().Bind(TextBlock.TextProperty, transformBoxSy, Slider.ValueProperty, t => t.ToString("F2")),
                                        new TextBlock().Text("Tx"),
                                        new TextBlock().Bind(TextBlock.TextProperty, transformBoxTx, Slider.ValueProperty, t => t.ToString("F2")),
                                        new TextBlock().Text("Ty"),
                                        new TextBlock().Bind(TextBlock.TextProperty, transformBoxTy, Slider.ValueProperty, t => t.ToString("F2"))
                                    ),
                                new TransformBox()
                                    .Bind(TransformBox.RotationDegreesProperty, transformBoxRotation, Slider.ValueProperty)
                                    .Bind(TransformBox.ScaleXProperty, transformBoxSx, Slider.ValueProperty)
                                    .Bind(TransformBox.ScaleYProperty, transformBoxSy, Slider.ValueProperty)
                                    .Bind(TransformBox.TranslateXProperty, transformBoxTx, Slider.ValueProperty)
                                    .Bind(TransformBox.TranslateYProperty, transformBoxTy, Slider.ValueProperty)
                                    .Child(new Button().Margin(0).Padding(0).StretchHorizontal().StretchVertical().FontSize(48).Content("Hello, World!"))
                            )
                    )
                    .ShowDialog()),
            new Button()
                .Content("Wizard")
                .OnClick(() => new Window()
                    .Title("Wizard")
                    .IsToolWindow()
                    .Content(
                        new Wizard()
                            .Add(new WizardPage("Welcome Page Title", "Welcome Page Description"))
                            .Add(new WizardPage("Page 1 Title", "Page 1 Description"))
                            .Add(new WizardPage("Page 2 Title", "Page 2 Description"))
                            .Add(new WizardPage("Finish Page Title", "Finish Page Description").CanFinish())
                    )
                    .ShowDialog()),
            new Button()
                .Content("HTML 1")
                .OnClick(() => new Window()
                    .Title("HTML 1")
                    .IsToolWindow()
                    .Content(
                        new DockPanel()
                            .Children(
                                new Button().DockLeft().Content("Button"),
                                new Button().DockRight().Content("Button"),
                                new Button().DockTop().Content("Button"),
                                new Button().DockBottom().Content("Button"),
                                new HtmlLabel()
                                    .DockBottom()
                                    .Text("<html><body><div style=\"color: green; font-size: 48px;\">Hello, <b>World</b><i>!</i></div></body></html>"),
                                new HtmlPanel()
                                    .Text("<html><body><table><tr><td>1</td><td>2</td></tr><tr><td>3</td><td>4</td></tr></table></body></html>")
                            )
                    )
                    .ShowDialog()),
            new Button()
                .Content("HTML 2")
                .OnClick(() => new Window()
                    .Title("HTML 2")
                    .IsToolWindow()
                    .Content(
                        new StackPanel()
                            .Children(
                                new TextBlock().Text("Text 1"),
                                new HtmlLabel().Text("<html><body><div style=\"font-size: 48px;\">Label 1</div></body></html>")
                                    .Background(Color.Yellow)
                                    .BorderBrush(Color.Red),
                                new HtmlPanel().Text("<html><body><div style=\"font-size: 48px;\">Label 2</div><br /><div style=\"font-size: 48px;\">Label 2</div></body></html>")
                                    .Background(Color.Yellow)
                                    .BorderBrush(Color.Red),
                                new TextBlock().Text("Text 2"),
                                new HtmlLabel().Text("<html><body><div style=\"font-size: 48px;\">Label 3</div></body></html>")
                                    .Background(Color.Yellow)
                                    .BorderBrush(Color.Red),
                                new TextBlock().Text("End")
                            )
                    )
                    .ShowDialog()),
            new Button()
                .Content("RTF document")
                .OnClick(() => new Window()
                    .Title("RTF document")
                    .IsToolWindow()
                    .Content(
                        new HtmlPanel().LoadRtfText(RtfTestDocument.Content)
                    )
                    .ShowDialog()),
            new Button()
                .Content("Quit")
                .OnClick(() => Application.Quit())
        )
    );

if (File.Exists("Sample.ico"))
{
    window.Icon(IconSource.FromFile("Sample.ico"));
}

Application.Run(window);
