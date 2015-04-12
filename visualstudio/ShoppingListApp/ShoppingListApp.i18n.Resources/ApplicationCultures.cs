using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ShoppingListApp.I18N.Resources
{
    public static class ApplicationCultures
    {
        private static Dictionary<string, string> supportedCultures = new Dictionary<string, string>() { { "US", "en-US" }, { "FR", "fr-FR" }, { "DE", "de-DE" } };

        public static ReadOnlyDictionary<string, string> SupportedCultures 
        {
            get 
            { 
                return new ReadOnlyDictionary<string, string>(supportedCultures); 
            }
        }

        public static bool IsASupportedCulture(string cultureToValidate)
        {
            return supportedCultures.ContainsValue(cultureToValidate);
        }
    }
}
