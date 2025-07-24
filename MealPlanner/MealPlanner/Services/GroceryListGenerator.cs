using MealPlanner.Data;
using MealPlanner.Models;
using System.Collections.Generic;
using System.Linq;

namespace MealPlanner.Services
{
    public class GroceryListGenerator
    {
        private readonly List<Meal> _allMeals;
        private readonly Database _database;

        public GroceryListGenerator(List<Meal> allMeals, Database database)
        {
            _allMeals = allMeals;
            _database = database;
        }

        public Dictionary<string, (double Quantity, string Unit)> GenerateGroceryList(
            Dictionary<string, (string Breakfast, int BreakfastPeople, string Lunch, int LunchPeople, string Dinner, int DinnerPeople)> selections)
        {
            var ingredients = new Dictionary<string, (double Quantity, string Unit)>();

            foreach (var day in selections.Values)
            {
                AddMeal(day.Breakfast, day.BreakfastPeople);
                AddMeal(day.Lunch, day.LunchPeople);
                AddMeal(day.Dinner, day.DinnerPeople);
            }

            return ingredients;

            void AddMeal(string mealName, int people)
            {
                var meal = _allMeals.FirstOrDefault(m => m.Name == mealName);
                if (meal == null) return;

                var mealIngredients = _database.GetIngredientsForMeal(meal.Id);

                foreach (var mi in mealIngredients)
                {
                    string name = mi.Ingredient.Name;
                    double quantity = Math.Round(mi.QuantityPerPerson * people, 1);
                    string unit = mi.Ingredient.Unit ?? "unit";

                    if (ingredients.ContainsKey(name))
                    {
                        var current = ingredients[name];
                        ingredients[name] = (current.Quantity + quantity, unit);
                    }
                    else
                    {
                        ingredients[name] = (quantity, unit);
                    }
                }
            }
        }
    }
}
