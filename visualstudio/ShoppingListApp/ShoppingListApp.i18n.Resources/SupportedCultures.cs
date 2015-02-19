using System.Collections.ObjectModel;
using System.Linq; 

namespace ShoppingListApp.I18N.Resources
{
    public static class SupportedCultures
    {
        // en-US is the culture
        // US is the specific culture
        private static string[] supportedCultures = { "en-US", "fr-FR", "de-DE" };
        private static string defaultCulture = "en-US";

        public static ReadOnlyCollection<string> Cultures 
        {
            get { return new ReadOnlyCollection<string>(supportedCultures); }
        }

        public static string SpecificCulture(string culture)
        {
            if (culture != null)
            {
                if (supportedCultures.Contains(culture))
                {
                    return culture.Split('-')[1];
                }
            }
            
            return defaultCulture;
        }
    }
}
