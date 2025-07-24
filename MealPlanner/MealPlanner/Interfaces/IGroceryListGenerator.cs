using MealPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Interfaces
{
    public interface IGroceryListGenerator
    {
        // Generate grocery list with total quantities for multiple meals & people counts
        // Dictionary - provides a mapping from a set of keys to a set of values    
        Dictionary<Ingredient, double> GenerateGroceryList(
         List<(Meal meal, int peopleCount)> selectedMeals);
    }
}