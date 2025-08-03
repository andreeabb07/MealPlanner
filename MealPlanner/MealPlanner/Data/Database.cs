using MealPlanner.Interfaces;
using MealPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Data
{
    public class Database : IDatabase
    {
        private readonly List<Meal> meals = new List<Meal>();
        private readonly List<Ingredient> ingredients = new List<Ingredient>();
        private readonly List<MealIngredient> mealIngredients = new List<MealIngredient>();

        private int mealIdCounter = 1;
        private int ingredientIdCounter = 1;

        public Database()
        {
            SeedFakeData();
        }

        // All the implemented methods from the contracts

        public void AddMeal(Meal meal)
        {
            meal.Id = mealIdCounter++;
            meals.Add(meal);
        }

        public void AddIngredient(Ingredient ingredient)
        {
            ingredient.Id = ingredientIdCounter++;
            ingredients.Add(ingredient);
        }

        public void AddMealIngredient(string mealName, string ingredientName, double quantityPerPerson)
        {
            var meal = meals.FirstOrDefault(m => m.Name == mealName);
            var ingredient = ingredients.FirstOrDefault(i => i.Name == ingredientName);

            if (meal == null || ingredient == null)
                throw new InvalidOperationException("Meal or Ingredient not found.");

            var mealIngredient = new MealIngredient
            {
                MealId = meal.Id,
                IngredientId = ingredient.Id,
                QuantityPerPerson = quantityPerPerson,
                Meal = meal,
                Ingredient = ingredient
            };

            mealIngredients.Add(mealIngredient);
        }

        public List<Meal> GetAllMeals() => meals;

        public List<Ingredient> GetAllIngredients() => ingredients;

        public List<MealIngredient> GetIngredientsForMeal(int mealId) =>
            mealIngredients.Where(mi => mi.MealId == mealId).ToList();

        public List<MealIngredient> GetIngredientsForBreakfast(int mealId)
        {
            var meal = meals.FirstOrDefault(m => m.Id == mealId);
            if (meal == null || !string.Equals(meal.MealType, "Breakfast", StringComparison.OrdinalIgnoreCase))
                return new List<MealIngredient>();

            return GetIngredientsForMeal(mealId);
        }

        public void Restock()
        {
            foreach (var ingredient in ingredients)
            {
                ingredient.Inventory += ingredient.ToBeBought;
                ingredient.ToBeBought = 0;
            }
        }
        public void CookDayPlan(string day)
        {
            Func<Meal, int> peopleEating = day.ToLower() switch
            {
                "monday" => m => m.Monday,
                "tuesday" => m => m.Tuesday,
                "wednesday" => m => m.Wednesday,
                "thursday" => m => m.Thursday,
                "friday" => m => m.Friday,
                "saturday" => m => m.Saturday,
                "sunday" => m => m.Sunday,
                _ => _ => 0
            };

            foreach (var meal in meals)
            {
                int persons = peopleEating(meal);
                if (persons <= 0) continue;

                var mealIngredients = GetIngredientsForMeal(meal.Id);
                foreach (var mi in mealIngredients)
                {
                    var ingredient = ingredients.FirstOrDefault(i => i.Id == mi.IngredientId);
                    if (ingredient == null) continue;

                    double used = persons * mi.QuantityPerPerson;
                    ingredient.Inventory -= used;
                    if (ingredient.Inventory < 0) ingredient.Inventory = 0;
                }
            }
        }
        public void UpdateIngredient(Ingredient updatedIngredient)
        {
            var existing = ingredients.FirstOrDefault(i => i.Name == updatedIngredient.Name);
            if (existing != null)
            {
                existing.Inventory = updatedIngredient.Inventory;
            }
        }
        public void SaveGroceryListToToBeBought(Dictionary<IInventoryUsageTracker.Days, OneDaysMeals> weeklySelections,
                                               Dictionary<string, (double Quantity, string Unit)> groceryList)
        {
            // 1. Save the grocery quantities to Ingredients
            foreach (var kvp in groceryList)
            {
                var ingredient = ingredients.FirstOrDefault(i => i.Name == kvp.Key);
                if (ingredient != null)

                    ingredient.ToBeBought = kvp.Value.Quantity;
            }

            // 2. Reset all day values
            foreach (var meal in meals)
            {
                meal.Monday = meal.Tuesday = meal.Wednesday = meal.Thursday =
                meal.Friday = meal.Saturday = meal.Sunday = 0;
            }

            // 3. For each day in your weeklySelections, assign people counts to each meal
            foreach (var entry in weeklySelections)
            {
                IInventoryUsageTracker.Days day = entry.Key;
                OneDaysMeals meal = entry.Value;



                AssignMealToDay(day, meal.Breakfast, meal.BreakfastPeople);
                AssignMealToDay(day, meal.Lunch, meal.LunchPeople);
                AssignMealToDay(day, meal.Dinner, meal.DinnerPeople);
            }
        }

        // Test data
        private void SeedFakeData()
        {
            string[] ingredientNames = {
        "Eggs", "Milk", "Flour", "Chicken", "Lettuce",
        "Tomatoes", "Cheese", "Beef", "Onion", "Oats",
        "Banana", "Yogurt"
    };

            foreach (var name in ingredientNames)
            {
                AddIngredient(new Ingredient
                {
                    Name = name,
                    Unit = GetUnitForIngredient(name),
                    Inventory = GetInitialInventory(name) // ✅ Set initial inventory
                });
            }

            // Meals
            AddMeal(new Meal { Name = "Pancakes", MealType = "Breakfast" });
            AddMeal(new Meal { Name = "Oatmeal", MealType = "Breakfast" });
            AddMeal(new Meal { Name = "Smoothie", MealType = "Breakfast" });

            AddMeal(new Meal { Name = "Grilled Chicken", MealType = "Main" });
            AddMeal(new Meal { Name = "Tacos", MealType = "Main" });
            AddMeal(new Meal { Name = "Burger", MealType = "Main" });
            AddMeal(new Meal { Name = "Salad", MealType = "Main" });

            // Link meals to ingredients
            AddMealIngredient("Pancakes", "Eggs", 1.5);
            AddMealIngredient("Pancakes", "Milk", 0.25);
            AddMealIngredient("Pancakes", "Flour", 0.3);

            AddMealIngredient("Oatmeal", "Oats", 0.5);
            AddMealIngredient("Oatmeal", "Milk", 0.2);
            AddMealIngredient("Oatmeal", "Banana", 0.5);

            AddMealIngredient("Smoothie", "Banana", 0.5);
            AddMealIngredient("Smoothie", "Yogurt", 0.3);

            AddMealIngredient("Grilled Chicken", "Chicken", 1.0);
            AddMealIngredient("Grilled Chicken", "Onion", 0.2);

            AddMealIngredient("Tacos", "Beef", 0.7);
            AddMealIngredient("Tacos", "Cheese", 0.1);
            AddMealIngredient("Tacos", "Tomatoes", 0.3);

            AddMealIngredient("Burger", "Beef", 1.0);
            AddMealIngredient("Burger", "Lettuce", 0.2);
            AddMealIngredient("Burger", "Cheese", 0.15);

            AddMealIngredient("Salad", "Lettuce", 0.5);
            AddMealIngredient("Salad", "Tomatoes", 0.3);
            AddMealIngredient("Salad", "Onion", 0.2);
        }

        // Helper to assign units
        private string GetUnitForIngredient(string name)
        {
            return name switch
            {
                "Eggs" => "piece",
                "Milk" => "liter",
                "Flour" => "kg",
                "Chicken" => "kg",
                "Lettuce" => "head",
                "Tomatoes" => "piece",
                "Cheese" => "kg",
                "Beef" => "kg",
                "Onion" => "piece",
                "Oats" => "kg",
                "Banana" => "piece",
                "Yogurt" => "liter",
                _ => "unit"
            };
        }

        // Helper to assign initial inventory
        private double GetInitialInventory(string name)
        {
            return name switch
            {
                "Eggs" => 6,
                "Milk" => 1.5,
                "Flour" => 2,
                "Chicken" => 1,
                "Lettuce" => 2,
                "Tomatoes" => 4,
                "Cheese" => 0.5,
                "Beef" => 1.2,
                "Onion" => 3,
                "Oats" => 1.5,
                "Banana" => 3,
                "Yogurt" => 0.75,
                _ => 0
            };
        }

        private void AssignMealToDay( IInventoryUsageTracker.Days day, string mealName, int people)
        {
            var meal = meals.FirstOrDefault(m => m.Name == mealName);
            if (meal == null || people <= 0) return;

            switch(day)
            {
                case IInventoryUsageTracker.Days.MONDAY: meal.Monday += people; break;
                case IInventoryUsageTracker.Days.TUESDAY: meal.Tuesday += people; break;
                case IInventoryUsageTracker.Days.WEDNESDAY: meal.Wednesday += people; break;
                case IInventoryUsageTracker.Days.THURSDAY: meal.Thursday += people; break;
                case IInventoryUsageTracker.Days.FRIDAY: meal.Friday += people; break;
                case IInventoryUsageTracker.Days.SATURDAY: meal.Saturday += people; break;
                case IInventoryUsageTracker.Days.SUNDAY: meal.Sunday += people; break;
            }
        }

    }
} 