using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingListApp.i18n.Resources
{
    public class SupportedCultures
    {
        private string[] supportedCultures = new string[] { "en-US", "fr-FR", "de-DE" };

        public string[] cultures {
            get { return supportedCultures; }
        }
    }
}
