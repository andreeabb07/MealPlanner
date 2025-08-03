using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Models;

public class OneDaysMeals
{
    public string Breakfast { get; set; }
    public int BreakfastPeople { get; set; }
    public string Lunch { get; set; }
    public int LunchPeople { get; set; }
    public string Dinner { get; set; }
    public int DinnerPeople { get; set; }


    // OneDaysMeals is the constructor, but Breakfast, ..., DinnerPeople, these are properties
    // Properties expose fields. 
    // the way it is written now it is under a shortcut form, because the long way it would be :
    // private string _breakfast ; 
    // public string Breakfast 
    //        {
    //        get {return _breakfast;}
    //        set {_breakfast = value;}
    //        }

    public OneDaysMeals(string breakfast, int breakfastPeople, string lunch, int lunchPeople, string dinner, int dinnerPeople)
    {
        Breakfast = breakfast;
        BreakfastPeople = breakfastPeople;
        Lunch = lunch;
        LunchPeople = lunchPeople;
        Dinner = dinner;
        DinnerPeople = dinnerPeople;
    }
}
