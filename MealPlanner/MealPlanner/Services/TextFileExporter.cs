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
    public class TextFileExporter
    {
        public void Export(Dictionary<string, (double Quantity, string Unit)> groceryList, string filePath)
        {
            var lines = groceryList
                .OrderBy(kv => kv.Key)
                .Select(kv => $"{kv.Key}: {kv.Value.Quantity} {kv.Value.Unit}");

            File.WriteAllLines(filePath, lines);
        }
    }
}