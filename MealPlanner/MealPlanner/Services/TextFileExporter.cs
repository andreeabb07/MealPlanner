using System;
using System.Linq;
using System.Threading.Tasks;
using MealPlanner.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MealPlanner.Services
{
    public class TextFileExporter : IFileExporter
    {
        public void Export(Dictionary<string, (double Quantity, string Unit)> groceryList, string filePath)
        {
            var lines = groceryList
                // LINQ query expressions. .Select transforms each element into a collection IEnumerable<string>. 
                .OrderBy(kv => kv.Key)
                .Select(kv => $"{kv.Key}: {kv.Value.Quantity} {kv.Value.Unit}");

            File.WriteAllLines(filePath, lines);
        }
    }
}