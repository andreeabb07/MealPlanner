using MealPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Interfaces
{
    public interface IMealRepositoryReading
    {
        // A method that returns a generic list of objects of type Meal
        // A method that goes and gets me the name of all meals
        List<Meal> GetAllMeals();

        // A method that returns a generic list of objects of type Ingredient
        // A method that goes and gets me all ingredients
        List<Ingredient> GetAllIngredients();

        // Fetch ingredients for a given meal
        List<MealIngredient> GetIngredientsForMeal(int mealId);

        List<MealIngredient> GetIngredientsForBreakfast(int mealId);
    }
}
