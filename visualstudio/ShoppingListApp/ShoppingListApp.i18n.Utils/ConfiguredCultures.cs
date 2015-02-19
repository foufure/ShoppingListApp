using System.Globalization;
using System.Threading;

namespace ShoppingListApp.I18N.Utils
{
    public static class ConfiguredCultures
    {
        public static CultureInfo GetCurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }

        public static CultureInfo GetCurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }
    }
}
