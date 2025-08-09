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
            id, name, mealtype,
            monday, tuesday, wednesday, thursday, friday, saturday, sunday
        FROM meals";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                meals.Add(new Meal
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    MealType = reader.GetString(2),
                    Monday = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    Tuesday = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    Wednesday = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    Thursday = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                    Friday = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                    Saturday = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                    Sunday = reader.IsDBNull(9) ? 0 : reader.GetInt32(9)
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
        SELECT id, name, unit, inventory, tobebought 
        FROM ingredients";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ingredients.Add(new Ingredient
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Unit = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Inventory = reader.IsDBNull(3) ? 0 : reader.GetDouble(3),
                    ToBeBought = reader.IsDBNull(4) ? 0 : reader.GetDouble(4)
                });
            }

            return ingredients;
        }

        public List<MealIngredient> GetIngredientsForBreakfast(int mealId)
        {
            var result = new List<MealIngredient>();

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string query = @"
        SELECT mi.mealid, mi.ingredientid, mi.quantityperperson
        FROM mealingredient mi
        JOIN meals m ON mi.mealid = m.id
        WHERE mi.mealid = @mealId AND m.mealtype = 'Breakfast'";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("mealId", mealId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new MealIngredient
                {
                    MealId = reader.GetInt32(0),
                    IngredientId = reader.GetInt32(1),
                    QuantityPerPerson = reader.GetDouble(2)
                });
            }

            return result;
        }

        public List<MealIngredient> GetIngredientsForMeal(int mealId)
        {
            var result = new List<MealIngredient>();

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string query = @"
        SELECT 
            mi.mealid, 
            mi.ingredientid, 
            mi.quantityperperson,
            i.name, 
            i.unit, 
            i.inventory, 
            i.tobebought
        FROM mealingredient mi
        JOIN ingredients i ON mi.ingredientid = i.id
        WHERE mi.mealid = @mealId";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("mealId", mealId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new MealIngredient
                {
                    MealId = reader.GetInt32(0),
                    IngredientId = reader.GetInt32(1),
                    QuantityPerPerson = reader.GetDouble(2),
                    Ingredient = new Ingredient
                    {
                        Id = reader.GetInt32(1),
                        Name = reader.GetString(3),
                        Unit = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                        Inventory = reader.IsDBNull(5) ? 0 : reader.GetDouble(5),
                        ToBeBought = reader.IsDBNull(6) ? 0 : reader.GetDouble(6)
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
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            const string query = @"
        INSERT INTO ingredients (name, unit, inventory, tobebought)
        VALUES (@name, @unit, @inventory, @tobebought)";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("name", ingredient.Name);
            cmd.Parameters.AddWithValue("unit", ingredient.Unit);
            cmd.Parameters.AddWithValue("inventory", ingredient.Inventory);
            cmd.Parameters.AddWithValue("tobebought", ingredient.ToBeBought);

            cmd.ExecuteNonQuery();
        }

        public void AddMealIngredient(string mealName, string ingredientName, double quantity)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // Step 1: Get meal ID
            int mealId;
            using (var getMealCmd = new NpgsqlCommand("SELECT id FROM meals WHERE name = @name", conn))
            {
                getMealCmd.Parameters.AddWithValue("name", mealName);
                mealId = Convert.ToInt32(getMealCmd.ExecuteScalar() ?? throw new Exception("Meal not found"));
            }

            // Step 2: Get ingredient ID
            int ingredientId;
            using (var getIngredientCmd = new NpgsqlCommand("SELECT id FROM ingredients WHERE name = @name", conn))
            {
                getIngredientCmd.Parameters.AddWithValue("name", ingredientName);
                ingredientId = Convert.ToInt32(getIngredientCmd.ExecuteScalar() ?? throw new Exception("Ingredient not found"));
            }

            // Step 3: Insert into mealingredient
            using var insertCmd = new NpgsqlCommand(@"
        INSERT INTO mealingredient (mealid, ingredientid, quantityperperson)
        VALUES (@mealid, @ingredientid, @quantity)", conn);

            insertCmd.Parameters.AddWithValue("mealid", mealId);
            insertCmd.Parameters.AddWithValue("ingredientid", ingredientId);
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
