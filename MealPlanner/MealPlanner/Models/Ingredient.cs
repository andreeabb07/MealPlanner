using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MealPlanner.Models
{ 
    public class Ingredient
    {
        public string? Name { get; set; }

        public string? Unit { get; set; }

        public double Inventory { get; set; }

        // a column property in my database that holds the value for how much of that ingredient needs to be bought based on my meal plan for that week
        public double ToBeBought { get; set; }
        public double Calories { get; set; }

        public double MinBuyQuantity { get; set; }

        // Encapsulates purchase logic here so data and behavior stay together, avoiding duplication.
        public double GetPurchaseQuantity(double requiredQuantity)
        {
            if (Inventory >= requiredQuantity)
                return 0; // we already have enough

            double missing = requiredQuantity - Inventory;

            if (MinBuyQuantity > 0)
            {
                int multiples = (int)Math.Ceiling(missing / MinBuyQuantity);
                return multiples * MinBuyQuantity;
            }

            return missing; // if MinBuyQuantity == 0
        }
    }

}
