using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace ShoppingListApp.i18n.Utils
{
    public static class ConfiguredCultures
    {
        public static CultureInfo getCurrentUICulture()
        {
            return Thread.CurrentThread.CurrentUICulture;
        }

        public static CultureInfo getCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture;
        }
    }
}
