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
        public int Id { get; set; }

        public string Name { get; set; }

        public string Unit { get; set; }

        public double Inventory { get; set; }

        // a column property in my database that holds the value for how much of that ingredient needs to be bought based on my meal plan for that week
        public double ToBeBought { get; set; }

    }

}
