using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MealPlanner
{
    /// <summary>
    /// Interaction logic for AddReceipe.xaml
    /// </summary>
    public partial class AddReceipe : Page
    {
        public AddReceipe()
        {
            InitializeComponent();
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Main.xaml", UriKind.Relative));

        }

        private void IngredientName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void QunatityName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void IngredientName_TextChanged_1(object sender, TextChangedEventArgs e)
        {
        }

        private void ListNewIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
