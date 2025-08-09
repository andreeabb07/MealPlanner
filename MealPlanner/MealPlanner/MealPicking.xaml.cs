using MealPlanner.Interfaces;
using MealPlanner.Models;
using MealPlanner.Services;
using MealPlanner.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MealPlanner
{
    // The partial keyword lets the MealPicking class be split across multiple files, 
    // so they merge into one complete class during compilation
    public partial class MealPicking : Page
    {
        private int _currentDayIndex = 0;

        private readonly string[] _daysOfWeek =
            { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        private readonly List<int> PeopleCountOptions = Enumerable.Range(1, 5).ToList();

        //`readonly` means the variable can only be assigned once (when declared or in the constructor)
        //and can’t point to a different object later, but the contents of that object can still be changed.

        private readonly Database _database = new Database();

        // I need a variable to hold in memory the values read from the ComboBox
        private Dictionary<IInventoryUsageTracker.Days, OneDaysMeals> weeklySelections =
            new Dictionary<IInventoryUsageTracker.Days, OneDaysMeals>();

        // This is the constructor of this class, and it is called when a new instance of the class is created
        // InitializeComponent() is a method that is defined by wpf, and it "matches" the code from here to the .xaml interface
        public MealPicking()
        {
            InitializeComponent();
            UpdateDayDisplay();
            LoadMealOptions();
        }


        // it just updates the days at the top of the page
        private void UpdateDayDisplay()
        {
            if (_currentDayIndex >= 0 && _currentDayIndex < _daysOfWeek.Length)
            {
                DayTextBlock.Text = _daysOfWeek[_currentDayIndex];
            }
        }

        private void LoadMealOptions()
        {
            var allMeals = _database.GetAllMeals();

            var breakfastMeals = allMeals
                .Where(m => m.MealType.Equals("Breakfast", StringComparison.OrdinalIgnoreCase))
                .Select(m => m.Name)
                .ToList();

            var mainMeals = allMeals
                .Where(m => !m.MealType.Equals("Breakfast", StringComparison.OrdinalIgnoreCase))
                .Select(m => m.Name)
                .ToList();

            BreakfastComboBox.ItemsSource = breakfastMeals;
            LunchComboBox.ItemsSource = mainMeals;
            DinnerComboBox.ItemsSource = mainMeals;

            BreakfastPeopleComboBox.ItemsSource = PeopleCountOptions;
            LunchPeopleComboBox.ItemsSource = PeopleCountOptions;
            DinnerPeopleComboBox.ItemsSource = PeopleCountOptions;

            IInventoryUsageTracker.Days currentDay =(IInventoryUsageTracker.Days) _currentDayIndex;

            if (weeklySelections.TryGetValue(currentDay, out var meals))
            {
                BreakfastComboBox.SelectedItem = meals.Breakfast;
                LunchComboBox.SelectedItem = meals.Lunch;
                DinnerComboBox.SelectedItem = meals.Dinner;

                SetSelectedPeople(BreakfastPeopleComboBox, meals.BreakfastPeople);
                SetSelectedPeople(LunchPeopleComboBox, meals.LunchPeople);
                SetSelectedPeople(DinnerPeopleComboBox, meals.DinnerPeople);
            }
            else
            {
                BreakfastComboBox.SelectedItem = null;
                LunchComboBox.SelectedItem = null;
                DinnerComboBox.SelectedItem = null;

                BreakfastPeopleComboBox.SelectedIndex = -1;
                LunchPeopleComboBox.SelectedIndex = -1;
                DinnerPeopleComboBox.SelectedIndex = -1;
            }
        }

        private void SaveCurrentDaySelection()
        {
            int day = _currentDayIndex;

            string breakfast = BreakfastComboBox.SelectedItem as string ?? "No selection";
            string lunch = LunchComboBox.SelectedItem as string ?? "No selection";
            string dinner = DinnerComboBox.SelectedItem as string ?? "No selection";

            int breakfastPeople = GetSelectedPeopleCount(BreakfastPeopleComboBox, 1);
            int lunchPeople = GetSelectedPeopleCount(LunchPeopleComboBox, 1);
            int dinnerPeople = GetSelectedPeopleCount(DinnerPeopleComboBox, 2);

            weeklySelections[(IInventoryUsageTracker.Days) _currentDayIndex] = new (breakfast, breakfastPeople, lunch, lunchPeople, dinner, dinnerPeople);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentDaySelection();

            if (_currentDayIndex < _daysOfWeek.Length - 1)
            {
                _currentDayIndex++;
                UpdateDayDisplay();
                LoadMealOptions();
            }
            else
            {
                // Save weekly meal plan to text file
                SaveAllSelectionsToFile();

                // Generate grocery list dictionary
                var allMeals = _database.GetAllMeals();
                var groceryGenerator = new GroceryListGenerator(_database);
                var groceryList = groceryGenerator.GenerateGroceryList(weeklySelections);

                // Save selections and grocery list into the database state
                _database.SaveGroceryListToToBeBought(weeklySelections, groceryList);

                // Export grocery list to text file
                // Since you created an interface to represent a generic version of file exporter classes,
                // I think it would be better for you to make your variables of type interface and use your specific class
                // (the one that implements said interface) for the object declaration
                IFileExporter groceryExporter = new TextFileExporter();
                var groceryFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "GroceryList.txt");

                groceryExporter.Export(groceryList, groceryFilePath);

                MessageBox.Show("All meals saved! Grocery list generated 🛒");
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDayIndex > 0)
            {
                SaveCurrentDaySelection();
                _currentDayIndex--;
                UpdateDayDisplay();
                LoadMealOptions();
            }
            else
            {
                MessageBox.Show("You're already at Monday!");
            }
        }

        private void SaveAllSelectionsToFile()
        {
            StringBuilder sb = new StringBuilder();

            foreach (IInventoryUsageTracker.Days day in Enum.GetValues(typeof(IInventoryUsageTracker.Days)))
            {
                if (weeklySelections.TryGetValue(day, out var meals))
                {
                    sb.AppendLine(_daysOfWeek[(int)day]);
                    sb.AppendLine($"  Breakfast: {meals.Breakfast} ({meals.BreakfastPeople})");
                    sb.AppendLine($"  Lunch: {meals.Lunch} ({meals.LunchPeople})");
                    sb.AppendLine($"  Dinner: {meals.Dinner} ({meals.DinnerPeople})");
                    sb.AppendLine();
                }
            }

            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MealPlan.txt");

            File.WriteAllText(filePath, sb.ToString());
        }

        private int GetSelectedPeopleCount(ComboBox comboBox, int defaultCount)
        {
            return comboBox.SelectedItem is int value ? value : defaultCount;
        }

        private void SetSelectedPeople(ComboBox comboBox, int count)
        {
            comboBox.SelectedItem = count;
        }

        // Optional: these handlers can be used for future logic if needed
        private void BreakfastComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void LunchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void DinnerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void BreakfastPeopleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void LunchPeopleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void DinnerPeopleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
    }
}