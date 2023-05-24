using OrchardCore.Modules.Manifest;

[assembly: Module(
    Author = "Etch UL Ltd.",
    Category = "Content",
    Description = "Control which fonts are loaded based on conditions.",
    Name = "Fonts",
    Version = "$(VersionNumber)",
    Website = "https://etchuk.com",
    Dependencies = new [] { "OrchardCore.CustomSettings" }
)]