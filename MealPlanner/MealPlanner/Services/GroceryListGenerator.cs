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
        // 1. Gather all ingredient needs per meal
        var allIngredientNeeds = new List<(Ingredient Ingredient, double Needed)>();

        foreach (var day in selections.Values)
        {
            allIngredientNeeds.AddRange(GetMealIngredients(day.Breakfast, day.BreakfastPeople));
            allIngredientNeeds.AddRange(GetMealIngredients(day.Lunch, day.LunchPeople));
            allIngredientNeeds.AddRange(GetMealIngredients(day.Dinner, day.DinnerPeople));
        }

        // 2. Aggregate quantities by ingredient name (LINQ grouping and prokjections, more concise than foreach)
        var cumulativeNeeds = allIngredientNeeds
            .GroupBy(x => x.Ingredient.Name) // group by ingredient
            .ToDictionary(
                g => g.Key!,
                g => (Ingredient: g.First().Ingredient, TotalNeeded: g.Sum(x => x.Needed)) // total quantity needed
            );

        // 3. Calculate actual quantities to buy based on inventory and min buy rules
        var groceryList = new Dictionary<string, (double Quantity, string Unit)>();
        foreach (var entry in cumulativeNeeds)
        {
            var ingredient = entry.Value.Ingredient;
            var toBuy = ingredient.GetPurchaseQuantity(entry.Value.TotalNeeded); // encapsulated logic for purchase qty
            if (toBuy <= 0) continue; // skip if enough in inventory

            string unit = ingredient.Unit ?? "unit"; // default unit
            groceryList[ingredient.Name!] = (toBuy, unit); // add to final grocery list
        }

        return groceryList;
    }

    // Returns ingredient quantities needed for a single meal
    private List<(Ingredient Ingredient, double Needed)> GetMealIngredients(string mealName, int people)
    {
        var meal = _database.GetAllMeals().FirstOrDefault(m => m.Name == mealName);
        if (meal == null) return new List<(Ingredient, double)>(); // meal not found

        var mealIngredients = _database.GetIngredientsForMeal(meal.Name);

        // calculate total needed per ingredient for the given number of people
        return mealIngredients
            .Where(mi => mi.Ingredient?.Name != null)
            .Select(mi => (mi.Ingredient!, Math.Round(mi.QuantityPerPerson * people, 1)))
            .ToList();
    }
}
