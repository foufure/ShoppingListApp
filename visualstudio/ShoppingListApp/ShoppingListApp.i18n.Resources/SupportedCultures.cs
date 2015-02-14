using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingListApp.i18n.Resources
{
    public static class SupportedCultures
    {
        // en-US is the culture
        // US is the specific culture
        private static string[] supportedCultures = new string[] { "en-US", "fr-FR", "de-DE" };

        public static string[] cultures 
        {
            get { return supportedCultures; }
        }

        public static string specificCulture(string culture)
        {
            return culture.Split('-')[1];
        }
    }
}
