using Etch.OrchardCore.Fields.Code.Fields;
using Etch.OrchardCore.Fonts.Models;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.Media.Fields;
using System;
using System.Collections.Generic;
using System.Text;

namespace Etch.OrchardCore.Fonts.Extensions
{
    public static class ContentItemExtensions
    {
        public static string GetRule(this ContentItem contentItem)
        {
            var part = contentItem.Get<ContentPart>(contentItem.ContentType);

            if (part == null)
            {
                return null;
            }

            return part.Get<TextField>("Rule")?.Text;
        }

        public static ExternalCssFont ToExternalCssFont(this ContentItem contentItem)
        {
            var part = contentItem.Get<ContentPart>("ExternalCssFont");

            if (part == null)
            {
                return null;
            }

            return new ExternalCssFont
            {
                Family = part.Get<TextField>("Family")?.Text,
                Rule = part.Get<TextField>("Rule")?.Text,
                Type = part.Get<TextField>("Type")?.Text,
                Url = part.Get<TextField>("Url")?.Text
            };
        }

        public static ExternalJsFont ToExternalJsFont(this ContentItem contentItem)
        {
            var part = contentItem.Get<ContentPart>("ExternalJsFont");

            if (part == null)
            {
                return null;
            }

            return new ExternalJsFont
            {
                Family = part.Get<TextField>("Family")?.Text,
                JavaScript = part.Get<CodeField>("JavaScript")?.Value,
                Rule = part.Get<TextField>("Rule")?.Text,
                Type = part.Get<TextField>("Type")?.Text
            };
        }

        public static MediaLibraryFont ToMediaLibraryFont(this ContentItem contentItem)
        {
            var part = contentItem.Get<ContentPart>("MediaLibraryFont");

            if (part == null)
            {
                return null;
            }

            return new MediaLibraryFont
            {
                Family = part.Get<TextField>("Family")?.Text,
                FontName = part.Get<TextField>("FontName")?.Text,
                FontPaths = part.Get<MediaField>("Fonts")?.Paths,
                Rule = part.Get<TextField>("Rule")?.Text,
                Style = part.Get<TextField>("Style")?.Text,
                Type = part.Get<TextField>("Type")?.Text,
                Weight = part.Get<NumericField>("Weight").Value.HasValue ? part.Get<NumericField>("Weight").Value.Value : 400
            };
        }
    }
}
