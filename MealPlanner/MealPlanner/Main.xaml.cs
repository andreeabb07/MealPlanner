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

namespace MealPlanner
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        public Main()
        {
            InitializeComponent();
        }

        private void GoToReceipes_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("AddReceipe.xaml", UriKind.Relative));
        }

        private void GoToGroceries_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("MealPicking.xaml", UriKind.Relative));

        }

        private void GoToInventory_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Inventory.xaml", UriKind.Relative));
        }
    }
}
