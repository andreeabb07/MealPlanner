using MealPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media;

namespace MealPlanner.Interfaces
{
    public interface IInventoryUsageTracker
    {
        // TODO: Instead of having many parameters in the class template placeholder, create a class that 
        // represents the same information
        // For the first parameter it may be called OneDaysMeals or something of the sort
        // You can also create an ENUM instead of having the days as strings because there are only 7 and they never change names

        enum Days
        {
            MONDAY,
            TUESDAY,
            WEDNESDAY,
            THURSDAY,
            FRIDAY,
            SATURDAY,
            SUNDAY
        }

        void SaveGroceryListToToBeBought(
            Dictionary<Days, OneDaysMeals> weeklySelections,
            Dictionary<string, (double Quantity, string Unit)> groceryList);
        void Restock();
        void CookDayPlan(string day);
        void UpdateIngredient(Ingredient updatedIngredient);
    }
}