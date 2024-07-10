# Etch.OrchardCore.Fonts

Orchard Core module for controlling which fonts are loaded based on conditions.

## Build Status

[![NuGet](https://img.shields.io/nuget/v/Etch.OrchardCore.Fonts.svg)](https://www.nuget.org/packages/Etch.OrchardCore.Fonts)

## Orchard Core Reference

This module is referencing a stable build of Orchard Core ([`1.8.3`](https://www.nuget.org/packages/OrchardCore.Module.Targets/1.8.3)).

## Installing

This module is available on [NuGet](https://www.nuget.org/packages/Etch.OrchardCore.Fonts). Add a reference to your Orchard Core web project via the NuGet package manager. Search for "Etch.OrchardCore.Fonts", ensuring include prereleases is checked.

Alternatively you can [download the source](https://github.com/etchuk/Etch.OrchardCore.Fonts/archive/master.zip) or clone the repository to your local machine. Add the project to your solution that contains an Orchard Core project and add a reference to Etch.OrchardCore.Fonts.

## Usage

Enable the "Fonts" feature, which will add definitions for managing which fonts should be loaded. This will add a custom setting ("FontSettings") that is available within the admin menu (under "Configuration" -> "Settings" -> "Fonts"). Font settings contains a collection made up of the content types displayed below. Each font requires a rule to be defined, which is used to determine whether the font should be loaded on the page. The rules logic mimics the behaviour used within the [Layers module](https://docs.orchardcore.net/en/dev/docs/reference/modules/Layers/).

Each font has a "Type" field that is used to determine which CSS variable should be outputted in the HTML rendered when the rule has passed. For example, if a font has a type value of "Default", the output will contain the following CSS:

```
:root { --fontFamilyDefault: 'Comic Sans'; }
```

The stylesheet loaded by the theme needs to be configured to utilise CSS variables to determine the font family.

### Media Library Font

Choose fonts that have been uploaded to the media library. Orchard Core by default won't allow font files to be uploaded to the media library so you'll need to ensure that [Orchard Core is configured](https://docs.orchardcore.net/en/dev/docs/reference/modules/Media/#configuration) to accept any extensions for font files that will be uploaded.

### External CSS Font

Specify a URL to a CSS file that is loaded via `@import`. Ideal for specifying fonts that are loaded from services like [Adobe Fonts](https://fonts.adobe.com/) or [Google Fonts](https://fonts.google.com/).

### External JS Font

Specify a JavaScript snippet that will handle loading fonts on the page. Ideal for specifying fonts that are loaded from services like [Adobe Fonts](https://fonts.adobe.com/) or [Google Fonts](https://fonts.google.com/).

## Packaging

When the theme is compiled (using `dotnet build`) it's configured to generate a `.nupkg` file (this can be found in `\bin\Debug\` or `\bin\Release`).

## Notes

This theme was created using `v0.4.2` of [Etch.OrchardCore.ModuleBoilerplate](https://github.com/EtchUK/Etch.OrchardCore.ModuleBoilerplate) template.
