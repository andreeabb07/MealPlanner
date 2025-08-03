namespace MealPlanner.Interfaces;

public interface IFileExporter
{
    void Export(Dictionary<string, (double Quantity, string Unit)> groceryList, string filePath);
}
