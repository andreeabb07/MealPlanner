using MealPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MealPlanner.Interfaces
{
    public interface IInventoryUsageTracker
    {
        void SaveGroceryListToToBeBought(
            Dictionary<string, (string Breakfast, int BreakfastPeople, string Lunch, int LunchPeople, string Dinner, int DinnerPeople)> weeklySelections,
            Dictionary<string, (double Quantity, string Unit)> groceryList);
        void Restock();
        void CookDayPlan(string day);
        void UpdateIngredient(Ingredient updatedIngredient);
    }
}