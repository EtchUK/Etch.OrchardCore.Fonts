using Etch.OrchardCore.Fields.Code.Fields;
using Etch.OrchardCore.Fields.Code.Settings;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.Flows.Models;
using OrchardCore.Media.Fields;
using OrchardCore.Media.Settings;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Fonts
{
    public class Migrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public Migrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public async Task<int> CreateAsync()
        {
            await _contentDefinitionManager.AlterTypeDefinitionAsync("FontSettings", builder => builder
                .DisplayedAs("Fonts")
                .Stereotype("CustomSettings")
                .WithPart("FontSettings")
                .WithPart("Fonts", nameof(BagPart), part => part
                    .WithDescription("Collection of fonts to be loaded on site.")
                    .WithDisplayName("Fonts")
                    .WithPosition("1")
                    .WithSettings(new BagPartSettings
                    {
                        ContainedContentTypes = new[] { "ExternalCssFont", "ExternalJsFont", "MediaLibraryFont" }  
                    })
                )
            );

            await _contentDefinitionManager.AlterTypeDefinitionAsync("MediaLibraryFont", builder => builder
                .DisplayedAs("Media Library Font")
                .MergeSettings(JObject.FromObject(new
                {
                    Category = "Fonts",
                    Description = "Load font files from media libary.",
                    Icon = "photo-video"
                }))
                .WithPart("MediaLibraryFont")
            );

            await _contentDefinitionManager.AlterTypeDefinitionAsync("ExternalCssFont", builder => builder
                .DisplayedAs("External CSS Font")
                .MergeSettings(JObject.FromObject(new
                {
                    Category = "Fonts",
                    Description = "Load font files from URL of an external CSS file (e.g. Google Fonts, Adobe Fonts).",
                    Icon = "external-link-square-alt"
                }))
                .WithPart("ExternalCssFont")
            );

            await _contentDefinitionManager.AlterTypeDefinitionAsync("ExternalJsFont", builder => builder
                .DisplayedAs("External JS Font")
                .MergeSettings(JObject.FromObject(new
                {
                    Category = "Fonts",
                    Description = "Load font files using a JavaScript snippet (e.g. Adobe Fonts).",
                    Icon = "external-link-square-alt"
                }))
                .WithPart("ExternalJsFont")
            );

            await _contentDefinitionManager.AlterPartDefinitionAsync("MediaLibraryFont", builder => builder
                .WithField("FontName", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Font Name")
                    .WithPosition("1")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "Name of the font, e.g. Comic Sans."
                    })
                )
                .WithField("Family", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Family")
                    .WithPosition("2")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "Name of the font family and any fallback fonts."
                    })
                )
                .WithField("Fonts", field => field
                    .OfType(nameof(MediaField))
                    .WithDisplayName("Fonts")
                    .WithPosition("3")
                    .WithSettings(new MediaFieldSettings
                    {
                        AllowMediaText = false,
                        Multiple = false
                    })
                )
                .WithField("Style", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Style")
                    .WithEditor("PredefinedList")
                    .WithPosition("4")
                    .WithSettings(new TextFieldPredefinedListEditorSettings
                    {
                        Options = new ListValueOption[]
                        {
                            new ListValueOption
                            {
                                Name = "Normal",
                                Value = "normal"
                            },
                            new ListValueOption
                            {
                                Name = "Italic",
                                Value = "italic"
                            },
                            new ListValueOption
                            {
                                Name = "Oblique",
                                Value = "oblique"
                            }
                        },
                        Editor = EditorOption.Dropdown
                    })
                )
                .WithField("Weight", field => field
                    .OfType(nameof(NumericField))
                    .WithDisplayName("Weight")
                    .WithPosition("5")
                )
                .WithField("Type", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Type")
                    .WithEditor("PredefinedList")
                    .WithPosition("6")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "Which variable the font should be applied to."
                    })
                    .WithSettings(new TextFieldPredefinedListEditorSettings
                    {
                        Options = new ListValueOption[]
                        {
                            new ListValueOption
                            {
                                Name = "Default",
                                Value = "Default"
                            },
                            new ListValueOption
                            {
                                Name = "Heading",
                                Value = "Heading"
                            }
                        },
                        Editor = EditorOption.Dropdown,
                        DefaultValue = "Default"
                    })
                )
                .WithField("Rule", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Rule")
                    .WithPosition("7")
                )
            );

            await _contentDefinitionManager.AlterPartDefinitionAsync("ExternalCssFont", builder => builder
                .WithField("Family", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Family")
                    .WithPosition("1")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "Name of the font family and any fallback fonts."
                    })
                )
                .WithField("Url", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Url")
                    .WithPosition("2")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "URL to external CSS file that contains font declaration (e.g. https://use.typekit.net/12345.css).",
                        Required = true
                    })
                )
                .WithField("Type", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Type")
                    .WithEditor("PredefinedList")
                    .WithPosition("3")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "Which variable the font should be applied to."
                    })
                    .WithSettings(new TextFieldPredefinedListEditorSettings
                    {
                        Options = new ListValueOption[]
                        {
                            new ListValueOption
                            {
                                Name = "Default",
                                Value = "Default"
                            },
                            new ListValueOption
                            {
                                Name = "Heading",
                                Value = "Heading"
                            }
                        },
                        Editor = EditorOption.Dropdown,
                        DefaultValue = "Default"
                    })
                )
                .WithField("Rule", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Rule")
                    .WithPosition("4")
                )
            );

            await _contentDefinitionManager.AlterPartDefinitionAsync("ExternalJsFont", builder => builder
                .WithField("Family", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Family")
                    .WithPosition("1")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "Name of the font family and any fallback fonts."
                    })
                )
                .WithField("JavaScript", field => field
                    .OfType(nameof(CodeField))
                    .WithDisplayName("JavaScript")
                    .WithPosition("2")
                    .WithSettings(new CodeFieldSettings
                    {
                        Language = "javascript"
                    })
                )
                .WithField("Type", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Type")
                    .WithEditor("PredefinedList")
                    .WithPosition("3")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "Which variable the font should be applied to."
                    })
                    .WithSettings(new TextFieldPredefinedListEditorSettings
                    {
                        Options = new ListValueOption[]
                        {
                            new ListValueOption
                            {
                                Name = "Default",
                                Value = "Default"
                            },
                            new ListValueOption
                            {
                                Name = "Heading",
                                Value = "Heading"
                            }
                        },
                        Editor = EditorOption.Dropdown,
                        DefaultValue = "Default"
                    })
                )
                .WithField("Rule", field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Rule")
                    .WithPosition("4")
                )
            );

            return 1;
        }
    }
}
