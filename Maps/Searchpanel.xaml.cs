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
using Windows.Devices.Geolocation;
using Microsoft.Maps.MapControl.WPF;
using Windows.Services.Maps;
namespace Maps
{
    /// <summary>
    /// Логика взаимодействия для Searchpanel.xaml
    /// </summary>
    public partial class Searchpanel : UserControl
    {
        public Searchpanel()
        {
            InitializeComponent();
        }
        private Map map;

        public Map CurrentMap { get => map; set => map = value; }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (((TextBox)sender).Text == "Поиск")
                ((TextBox)sender).Text = "";
        }

        public List<Category> GetChosenCategories()
        {
            List<Category> result = new List<Category>();
            if (cbEvent.IsChecked == true) result.Add(Category.Event);
            if (cbSport.IsChecked == true) result.Add(Category.Sport);
            if (cbMeet.IsChecked == true) result.Add(Category.Meet);
            if (cbParty.IsChecked == true) result.Add(Category.Party);
            return result;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string addressToGeocode = search.Text;

            // The nearby location to use as a query hint.
            BasicGeoposition queryHint = new BasicGeoposition();
            queryHint.Latitude = 47.643;
            queryHint.Longitude = -122.131;
            Geopoint hintPoint = new Geopoint(queryHint);

            // Geocode the specified address, using the specified reference point
            // as a query hint. Return no more than 3 results.
            MapLocationFinderResult result =
                  await MapLocationFinder.FindLocationsAsync(
                                    addressToGeocode,
                                    hintPoint,
                                    1);

            // If the query returns results, display the coordinates
            // of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
            {
                try
                {
                    map.Center =
                        new Microsoft.Maps.MapControl.WPF.Location(result.Locations[0].Point.Position.Latitude,
                        result.Locations[0].Point.Position.Longitude);
                }
                catch { }
            }
            
        }
    }
}
