# MewExtendedToolkit

[![Build status](https://ci.appveyor.com/api/projects/status/ce9aeh0rq0f8vhd8/branch/main?svg=true)](https://ci.appveyor.com/project/JeremyAnsel/mewextendedtoolkit/branch/main)
[![NuGet Version](https://img.shields.io/nuget/v/MewExtendedToolkit)](https://www.nuget.org/packages/MewExtendedToolkit)
![License](https://img.shields.io/github/license/JeremyAnsel/MewExtendedToolkit)

MewExtendedToolkit is a .NET library with custom controls for the [aprillz/MewUI](https://github.com/aprillz/MewUI) framework. It includes controls to show html and rtf documents.

Description     | Value
----------------|----------------
License         | [The MIT License (MIT)](https://github.com/JeremyAnsel/MewExtendedToolkit/blob/main/LICENSE.txt)
Documentation   | http://jeremyansel.github.io/MewExtendedToolkit
Source code     | https://github.com/JeremyAnsel/MewExtendedToolkit
Nuget           | https://www.nuget.org/packages/MewExtendedToolkit
Nuget           | https://www.nuget.org/packages/MewExtendedToolkit.Html
Build           | https://ci.appveyor.com/project/JeremyAnsel/mewextendedtoolkit/branch/main

## Controls

MewExtendedToolkit includes these controls:
- AboutBox
- InputBox
- TransformBox
- Wizard

MewExtendedToolkit.Html includes these controls:
- HtmlLabel
- HtmlPanel

## MewExtendedToolkit Controls

Here are the MewExtendedToolkit controls.

### AboutBox

To show an AboutBox:

```csharp
new AboutBox()
.Details("This tool uses these components:\n- MewUI by Aprillz: https://github.com/aprillz/MewUI")
.ShowDialog()
```

### InputBox

To show an InputBox:

```csharp
InputBox.ShowPrompt("Title", "Message", "placeholder text")
```

### TransformBox

To create a TransformBox:

```csharp
new TransformBox()
.Scale(1, -1)
.Translate(100, 50)
.RotationDegrees(15)
.Child(new TextBlock().FontSize(48).Text("Hello, World!"))
```

### Wizard

To create a Wizard:

```csharp
new Wizard()
.Add(new WizardPage("Welcome Page Title", "Welcome Page Description"))
.Add(new WizardPage("Page 1 Title", "Page 1 Description"))
.Add(new WizardPage("Page 2 Title", "Page 2 Description"))
.Add(new WizardPage("Finish Page Title", "Finish Page Description").CanFinish())
```

## MewExtendedToolkit.Html Controls

MewExtendedToolkit.Html controls are based on the [ArthurHub/HTML-Renderer](https://github.com/ArthurHub/HTML-Renderer) framework.
Support for RTF documents is added via the [JeremyAnsel/SharpRtfConvert](https://github.com/JeremyAnsel/SharpRtfConvert) library.

Here are the MewExtendedToolkit.Html controls.

### HtmlLabel

To show a HtmlLabel:

```csharp
new HtmlLabel()
.Text("<html><body><div style=\"color: green; font-size: 48px;\">Hello, <b>World</b><i>!</i></div></body></html>")
```

### HtmlPanel

To show a HtmlPanel:

```csharp
new HtmlPanel()
.Text("<html><body><div style=\"font-size: 48px;\">Line 1</div><br /><div style=\"font-size: 48px;\">Line 2</div></body></html>")
```

The HtmlPanel control supports RTF documents.

```csharp
new HtmlPanel().LoadRtfText(text);
new HtmlPanel().LoadRtfFile(fileName);
new HtmlPanel().LoadRtfFile(stream);
```

## Sample

This repository contains a Sample application to show and test the different controls.
