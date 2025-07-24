using MealPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This interface is created for any class that need to talk to the database
// to get data. The methods return either a GENERIC LIST or nothing (those who write)

namespace MealPlanner.Interfaces
{
    public interface IMealRepositoryWritting
    {
        // Add a new meal to the database
        void AddMeal(Meal meal);

        // Add a new ingredient to the database
        void AddIngredient(Ingredient ingredient);

        // Add ingredient details to a meal (like quantity)
        void AddMealIngredient(string mealName, string ingredientName, double quantity);

    }
}
