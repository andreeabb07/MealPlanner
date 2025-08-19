using MealPlanner.Models;

namespace MealPlanner.Interfaces
{
    public interface IMealRepositoryIO
    {
        // A method that returns a generic list of objects of type Meal
        // A method that goes and gets me the name of all meals
        List<Meal> GetAllMeals();

        // A method that returns a generic list of objects of type Ingredient
        // A method that goes and gets me all ingredients
        List<Ingredient> GetAllIngredients();

        // Fetch ingredients for a given meal
        List<MealIngredient> GetIngredientsForMeal(string MealName);

        List<MealIngredient> GetIngredientsForBreakfast(string mealName);


        // Add a new meal to the database
        void AddMeal(Meal meal);

        // Add a new ingredient to the database
        void AddIngredient(Ingredient ingredient);

        // Add ingredient details to a meal (like quantity)
        void AddMealIngredient(string mealName, string ingredientName, double quantity);
    }
}
