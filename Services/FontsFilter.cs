using Etch.OrchardCore.Fonts.Extensions;
using Etch.OrchardCore.Fonts.Utilities;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement.Theming;
using OrchardCore.Entities;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Flows.Models;
using OrchardCore.Media;
using OrchardCore.ResourceManagement;
using OrchardCore.Scripting;
using OrchardCore.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Fonts.Services
{
    public class FontsFilter : IAsyncResultFilter
    {
        private readonly IAdminThemeService _adminThemeService;
        private readonly ILogger _logger;
        private readonly IMediaFileStore _mediaFileStore;
        private readonly IResourceManager _resourceManager;
        private readonly IScriptingManager _scriptingManager;
        private readonly ISiteService _siteService;
        private readonly IThemeManager _themeManager;

        public FontsFilter(
            IAdminThemeService adminThemeService,
            ILogger<FontsFilter> logger,
            IMediaFileStore mediaFileStore,
            IResourceManager resourceManager,
            IScriptingManager scriptingManager,
            ISiteService siteService, 
            IThemeManager themeManager)
        {
            _adminThemeService = adminThemeService;
            _logger = logger;
            _mediaFileStore = mediaFileStore;
            _resourceManager = resourceManager;
            _scriptingManager = scriptingManager;
            _siteService = siteService;
            _themeManager = themeManager;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!(context.Result is ViewResult || context.Result is PageResult) &&
                AdminAttribute.IsApplied(context.HttpContext))
            {
                await next.Invoke();
                return;
            }

            // Even if the Admin attribute is not applied we might be using the admin theme, for instance in Login views.
            // In this case don't render Layers.
            var selectedTheme = (await _themeManager.GetThemeAsync())?.Id;
            var adminTheme = await _adminThemeService.GetAdminThemeNameAsync();

            if (selectedTheme == adminTheme)
            {
                await next.Invoke();
                return;
            }

            var site = await _siteService.GetSiteSettingsAsync();
            var fontSettings = site.As<ContentItem>("FontSettings");
            var fonts = fontSettings.Get<BagPart>("Fonts");

            if (fonts == null || !fonts.ContentItems.Any())
            {
                await next.Invoke();
                return;
            }

            var engine = _scriptingManager.GetScriptingEngine("js");
            var scope = engine.CreateScope(_scriptingManager.GlobalMethodProviders.SelectMany(x => x.GetMethods()), ShellScope.Services, null, null);
            var rulesCache = new Dictionary<string, bool>();

            foreach (var font in fonts.ContentItems)
            {
                var rule = font.GetRule();

                if (string.IsNullOrWhiteSpace(rule))
                {
                    continue;
                } 

                if (!rulesCache.TryGetValue(rule, out bool display))
                {
                    display = !string.IsNullOrEmpty(rule) && Convert.ToBoolean(engine.Evaluate(scope, rule));
                    rulesCache[rule] = display;
                }

                if (!display)
                {
                    continue;
                }

                switch (font.ContentType)
                {
                    case "ExternalCssFont":
                        RenderExternalCssFont(font);
                        continue;
                    case "ExternalJsFont":
                        RenderExternalJsFont(font);
                        continue;
                    case "MediaLibraryFont":
                        RenderMediaLibraryFont(font);
                        continue;
                }
            }

            await next.Invoke();
        }

        private void RenderExternalCssFont(ContentItem contentItem)
        {
            var externalFont = contentItem.ToExternalCssFont();

            if (externalFont == null)
            {
                return;
            }

            try
            {
                var html = new StringBuilder($"<link rel=\"preconnect\" href=\"{externalFont.UrlHostname}\">");
                html.AppendLine($"<style>@import url('{externalFont.Url}');:root {{ --fontFamily{externalFont.Type}:{externalFont.Family}; }}</style>");
                _resourceManager.RegisterStyle(new HtmlString(html.ToString()));
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load external CSS font");
            }
        }

        private void RenderExternalJsFont(ContentItem contentItem)
        {
            var externalFont = contentItem.ToExternalJsFont();

            if (externalFont == null)
            {
                return;
            }

            try
            {
                var html = new StringBuilder($"<script>{externalFont.JavaScript}</script><style>:root {{ --fontFamily{externalFont.Type}:{externalFont.Family}; }}</style>");
                _resourceManager.RegisterStyle(new HtmlString(html.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load external JS font");
            }
        }

        private void RenderMediaLibraryFont(ContentItem contentItem)
        {
            var mediaLibraryFont = contentItem.ToMediaLibraryFont();

            if (mediaLibraryFont == null)
            {
                return;
            }

            try
            {
                var html = new StringBuilder(string.Empty);
                var src = new List<string>();

                foreach (var path in mediaLibraryFont.FontPaths)
                {
                    var assetUrl = _mediaFileStore.MapPathToPublicUrl(path);

                    html.AppendLine($"<link rel=\"preload\" href=\"{assetUrl}\" as=\"font\" type=\"{FontMimeTypes.GetMimetype(path)}\" crossorigin=\"anonymous\">");

                    src.Add($"url('{assetUrl}') format('{Path.GetExtension(path).Substring(1)}')");
                }

                html.AppendLine(@$"<style>@font-face {{
                    font-display: swap;
                    font-family: '{mediaLibraryFont.FontName}';
                    font-style: {mediaLibraryFont.Style};
                    font-weight: {mediaLibraryFont.Weight};
                    src: local(''), {string.Join(", ", src)};
                }}:root {{ --fontFamily{mediaLibraryFont.Type}:{mediaLibraryFont.Family}; }}</style>");

                _resourceManager.RegisterStyle(new HtmlString(html.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load media library font");
            }
        }
    }
}
