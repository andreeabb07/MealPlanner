using MealPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlanner.Interfaces
{
    public interface IFileExporter
    {
        void Export(Dictionary<string, (double Quantity, string Unit)> groceryList, string filePath);
    }
}
