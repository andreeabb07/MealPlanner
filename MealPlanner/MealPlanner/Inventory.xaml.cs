using MealPlanner.Data;
using MealPlanner.Interfaces;
using MealPlanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace MealPlanner
{
    public partial class Inventory : Page
    {
        private IDatabase _db;
        public ObservableCollection<Ingredient> InventoryList { get; set; } = new();

        public Inventory()
        {
            InitializeComponent();
            _db = new Database();
            DataContext = this;  // important for binding to InventoryList
            LoadIngredients();
        }


        private void InventoryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void LoadIngredients()
        {
            InventoryList.Clear();
            var ingredients = _db.GetAllIngredients();
            foreach (var ingredient in ingredients)
            {
                InventoryList.Add(ingredient);
            }

            // Also update dropdown
            IngredientDropdown.ItemsSource = _db.GetAllIngredients().Select(i => i.Name).ToList();
        }


        private void UpdateWithLastGroceryList_Click(object sender, RoutedEventArgs e)
        {
            _db.Restock();
            LoadIngredients();

            InventoryGrid.ItemsSource = null;  // <--- force refresh
            InventoryGrid.ItemsSource = InventoryList;
        }

        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button dayButton)
            {
                string? shortDay = dayButton.Content.ToString();
                var dayMap = new Dictionary<string, string>
                                        {
                                            { "Mon", "monday" },
                                            { "Tue", "tuesday" },
                                            { "Wed", "wednesday" },
                                            { "Thu", "thursday" },
                                            { "Fri", "friday" },
                                            { "Sat", "saturday" },
                                            { "Sun", "sunday" }
                                        };

                if (shortDay is not null && dayMap.TryGetValue(shortDay, out string? fullDay))
                {
                    _db.CookDayPlan(fullDay);
                    LoadIngredients();
                }
            }
        }

        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            string name = NewIngredientTextBox.Text.Trim();
            string unit = NewIngredientUnitTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(unit))
            {
                MessageBox.Show("Please enter both the ingredient name and unit.");
                return;
            }

            var newIngredient = new Ingredient
            {
                Name = name,
                Unit = unit,
                Inventory = 0  // Default starting quantity
            };

            _db.AddIngredient(newIngredient);
            LoadIngredients();

            // Clear input fields
            NewIngredientTextBox.Text = "";
            NewIngredientUnitTextBox.Text = "";
        }


        private void UpdateIngredientQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (IngredientDropdown.SelectedItem is not string selectedName || string.IsNullOrWhiteSpace(selectedName))
            {
                MessageBox.Show("Please select an ingredient from the list.");
                return;
            }

            if (!double.TryParse(UpdatedQuantityTextBox.Text, out double newQuantity))
            {
                MessageBox.Show("Please enter a valid number for the quantity.");
                return;
            }

            var ingredient = _db.GetAllIngredients().FirstOrDefault(i => i.Name == selectedName);
            if (ingredient == null)
            {
                MessageBox.Show($"Could not find ingredient: {selectedName}");
                return;
            }

            ingredient.Inventory = newQuantity;
            _db.UpdateIngredient(ingredient);

            // Refresh display by reloading the ObservableCollection
            LoadIngredients();

            // Reset input fields
            UpdatedQuantityTextBox.Text = "";
            IngredientDropdown.SelectedItem = null;

            MessageBox.Show($"Quantity for '{selectedName}' updated.");
        }
    }
}
