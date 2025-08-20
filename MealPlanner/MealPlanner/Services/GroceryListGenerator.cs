using MealPlanner.Data;
using MealPlanner.Interfaces;
using MealPlanner.Models;
using System.Collections.Generic;
using System.Linq;

namespace MealPlanner.Services;

public class GroceryListGenerator
{
    private readonly IDatabase _database;
    public GroceryListGenerator(IDatabase database)
    {
        _database = database;
    }

    public Dictionary<string, (double Quantity, string Unit)> GenerateGroceryList(
       Dictionary<IInventoryUsageTracker.Days, OneDaysMeals> selections)
    {
        // for my grocery list, I need a dictionary that will save the ingredients I need, their quantity and their unit
        var ingredients = new Dictionary<string, (double Quantity, string Unit)>();

        // then, for each element from my dictionary called 'selections', I call the function addMeal for breakfast lunch and so on, and I return these ingredients
        // selections is a dictionary that has (key - MONDAY : value - an OneDaysMeal object that has all of these monday.Breakfast (=pancakes for ex), monday.Lunch, monday.Dinner, monday.BreakfastPeople, etc etc etc),
        // where 'monday' is a object of type OneDaysMeals, and 'MONDAY" is an element of an enumeration

        // selection.Values gives me a collection of all the values from that dictionary

        foreach (var day in selections.Values)
        {
            AddMealToGroceryList(day.Breakfast, day.BreakfastPeople, ingredients);
            AddMealToGroceryList(day.Lunch, day.LunchPeople, ingredients);
            AddMealToGroceryList(day.Dinner, day.DinnerPeople, ingredients);
        }
        return ingredients;
    }


    // the AddMealToGroceryList, takes as argument the name of the meal (pancake) and nb. of people (2)
    void AddMealToGroceryList(string mealName, int people, Dictionary<string, (double Quantity, string Unit)> ingredients)
    {
        var meal = _database.GetAllMeals().FirstOrDefault(m => m.Name == mealName);
        if (meal == null) return;

        var mealIngredients = _database.GetIngredientsForMeal(meal.Name);

        foreach (var mi in mealIngredients)
        {
            var ingredient = mi.Ingredient;
            if (ingredient?.Name == null) continue;

            double needed = Math.Round(mi.QuantityPerPerson * people, 1);

            // Check if enough inventory exists
            if (ingredient.Inventory >= needed)
            {
                // If there is already in stock, it skips this ingredient
                continue;
            }

            // If not enough stock :
            double toBuy = ingredient.MinBuyQuantity > 0 ? ingredient.MinBuyQuantity : needed;
            string unit = ingredient.Unit ?? "unit";

            if (ingredients.ContainsKey(ingredient.Name))
            {
                var current = ingredients[ingredient.Name];
                ingredients[ingredient.Name] = (current.Quantity + toBuy, unit);
            }
            else
            {
                ingredients[ingredient.Name] = (toBuy, unit);
            }
        }
    }

}