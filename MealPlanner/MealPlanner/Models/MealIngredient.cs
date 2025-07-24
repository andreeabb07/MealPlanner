using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Models
{
    public class MealIngredient
    {
        public int MealId { get; set; }
        public int IngredientId { get; set; }

        public double QuantityPerPerson { get; set; }

        // for EntityFramework?
        // I am not sure if I should allow it to have a null value or initialize it

        public Meal Meal { get; set; } = null!;
        public Ingredient Ingredient { get; set; } = null!;

    }
}
