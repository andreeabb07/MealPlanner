using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MealPlanner.Models
{ 

    public class Ingredient : INotifyPropertyChanged
    {
        private string _name;
        private string _unit;
        private double _inventory;
        private double _toBeBought;

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Unit
        {
            get => _unit;
            set { _unit = value; OnPropertyChanged(); }
        }

        public double Inventory
        {
            get => _inventory;
            set { _inventory = value; OnPropertyChanged(); }
        }

        public double ToBeBought
        {
            get => _toBeBought;
            set { _toBeBought = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
