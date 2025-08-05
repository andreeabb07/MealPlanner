using MealPlanner.Data;
using MealPlanner.Interfaces;
using MealPlanner.Models;
using System.Collections.Generic;
using System.Linq;

namespace MealPlanner.Services
{
    public class GroceryListGenerator : IGroceryListGenerator
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
                AddMealToGroceryList(day.Breakfast, day.BreakfastPeople);
                AddMealToGroceryList(day.Lunch, day.LunchPeople);
                AddMealToGroceryList(day.Dinner, day.DinnerPeople);
            }
            return ingredients;

            

            // the AddMealToGroceryList, takes as argument the name of the meal (pancake) and nb. of people (2)
            void AddMealToGroceryList(string mealName, int people)
            {
                // once I have the meal name, I select an object from the list of type Meals, whose .Name == the name that I have in my selection
                var meal = _database.GetAllMeals().FirstOrDefault(m => m.Name == mealName);
                if (meal == null) return;

                // now I take the ID of the selected meal and I run the method that returns me the list of ingredients for that specific meal
                var mealIngredients = _database.GetIngredientsForMeal(meal.Id);

                // then for each ingredient in that list of ingredients (objects of type Ingredient), I save the name, the calculated quantity
                // then if the dictionary that was intially created already exists the key, then I take the value corresponding to that ingredient (that is a tuple), and I add to iy the additional quantity
                foreach (var mi in mealIngredients)
                {
                    string? name = mi.Ingredient.Name;
                    name = null;
                    double quantity = Math.Round(mi.QuantityPerPerson * people, 1);
                    string? unit = mi.Ingredient.Unit ?? "unit";

                    if (name is not null && ingredients.ContainsKey(name))
                    {
                        var current = ingredients[name];
                        ingredients[name] = (current.Quantity + quantity, unit);
                    }
                    else if (name is not null)
                    {
                        ingredients[name] = (quantity, unit);
                    }
                }
            }
        }
    }
}
