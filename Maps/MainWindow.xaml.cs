using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using static Maps.MySQLCon;

namespace Maps
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User CurrentUser;
        MySQLCon MySQLHelper;

        BindingList<Meet> meetings = new BindingList<Meet>();
        BindingList<Meet> userMeet = new BindingList<Meet>();

        [Obsolete]
        public MainWindow(User user, MySQLCon mh)
        {
            CurrentUser = user;
            MySQLHelper = mh;
            InitializeComponent();
            MapService.ServiceToken = 
                "0rJY0HyaVZHi1HWPOcos~Gx6GxDM29tFyArXVZM_eRA~" +
                "AidH8MDp9dJj5ntQ7rcsJWSeEZCzvLUtFKzAxt3-4ausZ-ih6xqreMgToaUh1xEf";

            setUserPos();
            UpdateMap();
        }

        [Obsolete]
        private async void setUserPos()
        {
            Geolocator geolocator = new Geolocator();

            // Subscribe to StatusChanged event to get updates of location status changes

            // Carry out the operation
            Geoposition pos = await geolocator.GetGeopositionAsync();

            CurrentUser.SetPosition = pos;
            map.Center = new Location(CurrentUser.GetPosition.Coordinate.Latitude, CurrentUser.GetPosition.Coordinate.Longitude);
        }
        private void UpdateMeets()
        {
            //с бд подргрузка
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new FullMap(ref userMeet, ref meetings, CurrentUser, MySQLHelper).ShowDialog();
        }

        private void UpdateMap()
        {
            userMeet.Clear();
            meetings.Clear();
            map.Children.Clear();
            string NotUserMeets = 
                String.Format("SELECT Type.Name, People.name, x, y, Description, Participants, Date, Meet.idMeet" +
                " FROM Meet INNER JOIN Type using(idType) INNER JOIN People on idPeople = idCreator" +
                " WHERE idCreator <> {0}",
                CurrentUser.GetId);
            string UserMeets = 
                String.Format("SELECT Type.Name, People.name, x, y, Description, Participants, Date, Meet.idMeet" +
                " FROM Meet INNER JOIN Type using(idType) INNER JOIN People on idPeople = idCreator" +
                " WHERE idCreator = {0}",
                CurrentUser.GetId);

            foreach (string[] newmeet in MySQLHelper.getValues(NotUserMeets))
            {
                Meet _meet = 
                   new Meet(newmeet[4], newmeet[1], Meet.RevPinType(newmeet[0]), newmeet[5], newmeet[6], newmeet[7]);
                meetings.Add(_meet);
                AddPinOnMap(_meet, new Location(Convert.ToDouble(newmeet[2]), Convert.ToDouble(newmeet[3])));
            }


            foreach (string[] newmeet in MySQLHelper.getValues(UserMeets))
            {
                Meet _meet = 
                   new Meet(newmeet[4], newmeet[1], Meet.RevPinType(newmeet[0]), newmeet[5], newmeet[6], newmeet[7]);
                userMeet.Add(_meet);
                AddPinOnMap(_meet, new Location(Convert.ToDouble(newmeet[2]), Convert.ToDouble(newmeet[3])));
            }
        }
        private void AddPinOnMap(Meet m, Location point)
        {
            Pushpin pin = new Pushpin();
            pin.Content = Meet.PinType(m.GetCategory)[0];
            pin.Location = point;


            pin.MouseEnter += MouseEnter;
            pin.MouseLeave += MouseLeave;

            m.Pin = pin;
            // Adds the pushpin to the map.
            map.Children.Add(pin);
            //mark.Add(new Marker(pin, meet));
        }
        private new void MouseEnter(object sender, MouseEventArgs e)
        {
            foreach (Meet m in userMeet)
            {
                if (m.Pin == (Pushpin)sender)
                {
                    pininfo.Show(e.GetPosition(this).X - 3, e.GetPosition(this).Y - 103,
                        m.GetCategory, m.GetDescription, m.GetCreator, m.GetDate, m.GetParticipants);
                    break;
                }
            }
            foreach (Meet m in meetings)
            {
                if (m.Pin == (Pushpin)sender)
                {
                    pininfo.Show(e.GetPosition(this).X - 3, e.GetPosition(this).Y - 103,
                        m.GetCategory, m.GetDescription, m.GetCreator, m.GetDate, m.GetParticipants);
                    break;
                }
            }
        }

        private new void MouseLeave(object sender, MouseEventArgs e)
        {
            pininfo.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UpdateMap();
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (((TextBox)sender).Text == "Поиск")
                ((TextBox)sender).Text = "";
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string addressToGeocode = tbSearch.Text;

            // The nearby location to use as a query hint.
            BasicGeoposition queryHint = new BasicGeoposition();
            queryHint.Latitude = map.Center.Latitude;
            queryHint.Longitude = map.Center.Longitude;
            Geopoint hintPoint = new Geopoint(queryHint);

            // Geocode the specified address, using the specified reference point
            // as a query hint. Return no more than 3 results.
            MapLocationFinderResult result =
                  await MapLocationFinder.FindLocationsAsync(
                                    addressToGeocode,
                                    hintPoint, 3);
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            new MeetWindow(CurrentUser, MySQLHelper, userMeet, meetings).ShowDialog();

        }
    }
}
