using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingListApp.Domain.Concrete
{
    public static class UnitsUtils
    {
        public static Dictionary<string, string> Units 
        {
            get { return new Dictionary<string, string> { { "default", "-" }, { "kg", "kg" }, { "g", "g" }, { "l", "l" }, { "cl", "cl" }, { "ml", "ml" } }; } 
        }
    }
}
