using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingListApp.Domain.Entities
{
    public class ShoppingListLine
    {
        public int LinePresentationOrder { get; set; }

        public Item ItemToBuy { get; set; }

        public decimal QuantityToBuy { get; set; }

        public string Unit { get; set; }

        public bool Done { get; set; }
    }
}
