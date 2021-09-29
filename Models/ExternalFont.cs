using System;

namespace Etch.OrchardCore.Fonts.Models
{
    public class ExternalFont
    {
        public string Family { get; set; }
        public string Rule { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }

        public string UrlHostname
        {
            get {
                var uri = new Uri(Url);
                return $"{uri.Scheme}://{uri.Host}";
            }
        }
    }
}
