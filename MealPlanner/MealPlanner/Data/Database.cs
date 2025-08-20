using MealPlanner.Interfaces;
using MealPlanner.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Data
{
    public class Database : IDatabase

    {

        private const string _connectionString = "Host = 93.27.145.84; Database=foods;Username=loadev;Password=Zbruhbruh447!;Persist Security Info=True";

        public Database()
        {
        }

        public List<Meal> GetAllMeals()
        {
            var meals = new List<Meal>();

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string query = @"
        SELECT 
            name, mealtype,
            monday, tuesday, wednesday, thursday, friday, saturday, sunday
        FROM meals";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                meals.Add(new Meal
                {
                    Name = reader.GetString(0),
                    MealType = reader.GetString(1),
                    Monday = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    Tuesday = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    Wednesday = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    Thursday = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    Friday = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                    Saturday = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                    Sunday = reader.IsDBNull(8) ? 0 : reader.GetInt32(8)
                });
            }

            return meals;
        }

        public List<Ingredient> GetAllIngredients()
        {
            var ingredients = new List<Ingredient>();

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string query = @"
        SELECT name, unit, inventory, tobebought, calories, minbuyquantity 
        FROM ingredients";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ingredients.Add(new Ingredient
                {
                    Name = reader.GetString(0),
                    Unit = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Inventory = reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                    ToBeBought = reader.IsDBNull(3) ? 0 : reader.GetDouble(3),
                    Calories = reader.IsDBNull(4) ? 0 : reader.GetDouble(4),
                    MinBuyQuantity = reader.IsDBNull(5) ? 0 : reader.GetDouble(5)
                });
            }

            return ingredients;
        }

        public List<MealIngredient> GetIngredientsForBreakfast(string mealName)
        {
            var result = new List<MealIngredient>();

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string query = @"
            SELECT 
                mi.meal, 
                mi.ingredient, 
                mi.quantityperperson,
                i.name, 
                i.unit, 
                i.inventory, 
                i.tobebought
                i.calories,
                i.minbuyquantity
            FROM mealingredient mi
            JOIN meals m ON mi.mealname = m.name
            JOIN ingredients i ON mi.ingredient = i.name
            WHERE mi.meal = @mealName AND m.mealtype = 'Breakfast'";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("mealName", mealName);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new MealIngredient
                {
                    MealName = reader.GetString(0),
                    IngredientName = reader.GetString(1),
                    QuantityPerPerson = reader.GetDouble(2),
                    Ingredient = new Ingredient
                    {
                        Name = reader.GetString(3),
                        Unit = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                        Inventory = reader.IsDBNull(5) ? 0 : reader.GetDouble(5),
                        ToBeBought = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                        Calories = reader.IsDBNull(7) ? 0 : reader.GetDouble(7),
                        MinBuyQuantity = reader.IsDBNull(8) ? 0 : reader.GetDouble(8)
                    }
                });
            }

            return result;
        }

        public List<MealIngredient> GetIngredientsForMeal(string mealName)
        {
            var result = new List<MealIngredient>();

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string query = @"
            SELECT 
                mi.meal, 
                mi.ingredient, 
                mi.quantityperperson,
                i.name, 
                i.unit, 
                i.inventory, 
                i.tobebought,
                i.calories,
                i.minbuyquantity
            FROM mealingredient mi
            JOIN ingredients i ON mi.ingredient = i.name
            WHERE mi.meal = @mealName";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("mealName", mealName);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new MealIngredient
                {
                    MealName = reader.GetString(0),
                    IngredientName = reader.GetString(1),
                    QuantityPerPerson = reader.GetDouble(2),
                    Ingredient = new Ingredient
                    {
                        Name = reader.GetString(3),
                        Unit = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                        Inventory = reader.IsDBNull(5) ? 0 : reader.GetDouble(5),
                        ToBeBought = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                        Calories = reader.IsDBNull(7) ? 0 : reader.GetDouble(7),
                        MinBuyQuantity = reader.IsDBNull(8) ? 0 : reader.GetDouble(8)
                    }
                });
            }

            return result;
        }

        public void AddMeal(Meal meal)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string query = @"
            INSERT INTO meals (name, mealtype, monday, tuesday, wednesday, thursday, friday, saturday, sunday)
            VALUES (@name, @mealtype, @monday, @tuesday, @wednesday, @thursday, @friday, @saturday, @sunday)";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("name", meal.Name);
            cmd.Parameters.AddWithValue("mealtype", meal.MealType);
            cmd.Parameters.AddWithValue("monday", meal.Monday);
            cmd.Parameters.AddWithValue("tuesday", meal.Tuesday);
            cmd.Parameters.AddWithValue("wednesday", meal.Wednesday);
            cmd.Parameters.AddWithValue("thursday", meal.Thursday);
            cmd.Parameters.AddWithValue("friday", meal.Friday);
            cmd.Parameters.AddWithValue("saturday", meal.Saturday);
            cmd.Parameters.AddWithValue("sunday", meal.Sunday);

            cmd.ExecuteNonQuery();
        }

        public void AddIngredient(Ingredient ingredient)
        {
            if (string.IsNullOrWhiteSpace(ingredient.Name))
                throw new ArgumentException("Ingredient name cannot be null or empty.", nameof(ingredient));

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // Check if ingredient already exists
            using (var checkCmd = new NpgsqlCommand("SELECT 1 FROM ingredients WHERE name = @name", conn))
            {
                checkCmd.Parameters.AddWithValue("name", ingredient.Name);

                var exists = checkCmd.ExecuteScalar();
                if (exists != null)
                {
                    Console.WriteLine($"Ingredient '{ingredient.Name}' is already in the list.");
                    return;
                }
            }

            // Insert new ingredient
            const string query = @"
        INSERT INTO ingredients (name, unit, inventory, tobebought, calories, minbuyquantity)
        VALUES (@name, @unit, @inventory, @tobebought, @calories, @minbuyquantity)";

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("name", ingredient.Name);
                cmd.Parameters.AddWithValue("unit", ingredient.Unit ?? string.Empty);
                cmd.Parameters.AddWithValue("inventory", ingredient.Inventory);
                cmd.Parameters.AddWithValue("tobebought", ingredient.ToBeBought);
                cmd.Parameters.AddWithValue("calories", ingredient.Calories);
                cmd.Parameters.AddWithValue("minbuyquantity", ingredient.MinBuyQuantity);

                cmd.ExecuteNonQuery();
            }
        }

        public void AddMealIngredient(string mealName, string ingredientName, double quantity)
        {
            if (string.IsNullOrWhiteSpace(mealName))
                throw new ArgumentException("Meal name cannot be null or empty.", nameof(mealName));

            if (string.IsNullOrWhiteSpace(ingredientName))
                throw new ArgumentException("Ingredient name cannot be null or empty.", nameof(ingredientName));

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // Check if meal exists
            using (var checkMealCmd = new NpgsqlCommand(
                "SELECT 1 FROM meals WHERE name = @mealName", conn))
            {
                checkMealCmd.Parameters.AddWithValue("mealName", mealName);
                if (checkMealCmd.ExecuteScalar() == null)
                    throw new InvalidOperationException($"Meal '{mealName}' not found in meals table.");
            }

            // Check if ingredient exists
            using (var checkIngredientCmd = new NpgsqlCommand(
                "SELECT 1 FROM ingredients WHERE name = @ingredientName", conn))
            {
                checkIngredientCmd.Parameters.AddWithValue("ingredientName", ingredientName);
                if (checkIngredientCmd.ExecuteScalar() == null)
                    throw new InvalidOperationException($"Ingredient '{ingredientName}' not found in ingredients table.");
            }

            // Insert into mealingredient
            using var insertCmd = new NpgsqlCommand(@"
        INSERT INTO mealingredient (mealname, ingredientname, quantityperperson)
        VALUES (@mealName, @ingredientName, @quantity)", conn);

            insertCmd.Parameters.AddWithValue("mealName", mealName);
            insertCmd.Parameters.AddWithValue("ingredientName", ingredientName);
            insertCmd.Parameters.AddWithValue("quantity", quantity);

            insertCmd.ExecuteNonQuery();
        }

        public void Restock()
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var cmd = new NpgsqlCommand(@"
        UPDATE ingredients 
        SET inventory = inventory + tobebought,
            tobebought = 0", conn);

            cmd.ExecuteNonQuery();
        }

        public void SaveGroceryListToToBeBought(
    Dictionary<IInventoryUsageTracker.Days, OneDaysMeals> weeklySelections,
    Dictionary<string, (double Quantity, string Unit)> groceryList)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // 1. Update ToBeBought for each ingredient
            foreach (var kvp in groceryList)
            {
                var cmd = new NpgsqlCommand("UPDATE ingredients SET tobebought = @qty WHERE name = @name", conn);
                cmd.Parameters.AddWithValue("qty", kvp.Value.Quantity);
                cmd.Parameters.AddWithValue("name", kvp.Key);
                cmd.ExecuteNonQuery();
            }

            // 2. Reset day counts in all meals
            var resetCmd = new NpgsqlCommand(@"
        UPDATE meals 
        SET monday = 0, tuesday = 0, wednesday = 0, thursday = 0, 
            friday = 0, saturday = 0, sunday = 0", conn);
            resetCmd.ExecuteNonQuery();

            // 3. Assign meals to days with people counts
            foreach (var entry in weeklySelections)
            {
                var day = entry.Key.ToString().ToLower(); // matches column names
                var mealsForDay = entry.Value;

                UpdateMealDayCount(conn, mealsForDay.Breakfast, day, mealsForDay.BreakfastPeople);
                UpdateMealDayCount(conn, mealsForDay.Lunch, day, mealsForDay.LunchPeople);
                UpdateMealDayCount(conn, mealsForDay.Dinner, day, mealsForDay.DinnerPeople);
            }
        }

        private void UpdateMealDayCount(NpgsqlConnection conn, string mealName, string dayColumn, int people)
        {
            var cmd = new NpgsqlCommand(
                $"UPDATE meals SET {dayColumn} = @people WHERE name = @name", conn);
            cmd.Parameters.AddWithValue("people", people);
            cmd.Parameters.AddWithValue("name", mealName);
            cmd.ExecuteNonQuery();
        }

        // this function is not done yet
        public void CookDayPlan(string day)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // Step 1: Get meals planned for the given day (people > 0)
            var cmd = new NpgsqlCommand($@"
        SELECT id, {day.ToLower()} AS people
        FROM meals
        WHERE {day.ToLower()} > 0", conn);

            using var reader = cmd.ExecuteReader();
            var mealsToCook = new List<(int MealId, int People)>();
            while (reader.Read())
            {
                mealsToCook.Add((reader.GetInt32(0), reader.GetInt32(1)));
            }
            reader.Close();

            // Step 2: For each meal, get ingredients and update inventory
            foreach (var (mealId, people) in mealsToCook)
            {
                var ingredientCmd = new NpgsqlCommand(@"
            SELECT ingredientid, quantityperperson
            FROM mealingredient
            WHERE mealid = @mealid", conn);
                ingredientCmd.Parameters.AddWithValue("mealid", mealId);

                using var ingReader = ingredientCmd.ExecuteReader();
                var ingredientsUsed = new List<(int IngredientId, double UsedQty)>();

                while (ingReader.Read())
                {
                    int ingId = ingReader.GetInt32(0);
                    double qtyPerPerson = ingReader.GetDouble(1);
                    ingredientsUsed.Add((ingId, qtyPerPerson * people));
                }
                ingReader.Close();

                foreach (var (ingId, usedQty) in ingredientsUsed)
                {
                    var updateCmd = new NpgsqlCommand(@"
                UPDATE ingredients 
                SET inventory = GREATEST(inventory - @used, 0)
                WHERE id = @id", conn);

                    updateCmd.Parameters.AddWithValue("used", usedQty);
                    updateCmd.Parameters.AddWithValue("id", ingId);
                    updateCmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateIngredient(Ingredient updatedIngredient)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var cmd = new NpgsqlCommand(@"
        UPDATE ingredients 
        SET inventory = @inv 
        WHERE name = @name", conn);

            cmd.Parameters.AddWithValue("inv", updatedIngredient.Inventory);
            cmd.Parameters.AddWithValue("name", updatedIngredient.Name);

            cmd.ExecuteNonQuery();
        }
    }
}
