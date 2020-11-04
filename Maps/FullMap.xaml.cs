using Microsoft.Maps.MapControl.WPF;
using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Windows.Networking.NetworkOperators;
using Windows.UI.Xaml.Controls;

namespace Maps
{


    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class FullMap : Window
    {
        User CurrentUser;
        MySQLCon MySQLHelper;


        BindingList<Meet> userMeet;
        BindingList<Meet> meetings;

        public FullMap(ref BindingList<Meet> userMeet, ref BindingList<Meet> meetings, User us, MySQLCon mh)
        {
            this.userMeet = userMeet;
            this.meetings = meetings;
            MySQLHelper = mh;

            CurrentUser = us;

            InitializeComponent();
            LeftPanel.CurrentMap = map;
            UpdateMap();
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

            List<Category> list = LeftPanel.GetChosenCategories();
            if (list.Count > 0)
            {
                string categorystring = " AND (Type.Name = '" + Meet.PinType(list[0]) + "'";
                for (int i = 1; i < list.Count; ++i)
                {
                    categorystring += " OR Type.Name = '" + Meet.PinType(list[i]) + "'";
                }
                categorystring += ")";
                NotUserMeets += categorystring;
                UserMeets += categorystring;
            }

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

        private void DoubleClickMap(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point pos = e.GetPosition(this);

            System.Windows.Point mousePosition = pos;
            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = map.ViewportPointToLocation(mousePosition);

            e.Handled = true;
            AddMeet am = new AddMeet();
            am.ShowDialog();
            if (am.Success()) {
                Meet meet = new Meet(am.GetDescription, CurrentUser.GetName, am.GetCategory(), "1", am.GetDate, "0");
                userMeet.Add(meet);
                AddPinOnMap(meet, pinLocation);
                AddPushPinToDB(meet);
            }
        }

        private void AddPushPinToDB(Meet meet)
        {
            string querry = String.Format("INSERT INTO `Meet` " +
                "(`idMeet`, `idType`, `idCreator`, `X`, `Y`, `Description`, `Participants`, `Date`) VALUES" +
                "(NULL, (SELECT idType FROM Type WHERE Name = '{0}'), '{1}', '{2}', '{3}', '{4}', '{5}', '{6}'); ", 
                Meet.PinType(meet.GetCategory), CurrentUser.GetId, meet.Pin.Location.Latitude, 
                meet.Pin.Location.Longitude, meet.GetDescription, 1, meet.GetDate) ;
            MySQLHelper.execute(querry);
            meet.ID = MySQLHelper.getValue(String.Format("SELECT idMeet FROM Meet" +
                " WHERE `Description` = '{0}' and `X` = '{1}' and `Y` = '{2}'",
                meet.GetDescription, meet.Pin.Location.Latitude, meet.Pin.Location.Longitude));
        }

        private void AddPinOnMap(Meet m, Location point)
        {
            Pushpin pin = new Pushpin();
            pin.Content = Meet.PinType(m.GetCategory)[0];
            pin.Location = point;


            pin.MouseEnter += MouseEnter;
            pin.MouseLeave += MouseLeave;
            pin.MouseDown += MouseDown;

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
        private new void MouseDown(object sender, MouseEventArgs e)
        {
            MessageBoxResult result = 
                MessageBox.Show("Уверены, что хотите посетить это событие?", "Участие", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                bool check = false;
                foreach (Meet m in meetings)
                {
                    if (m.Pin == (Pushpin)sender)
                    {
                        check = true;
                        string querry = 
                            String.Format("INSERT INTO `People_has_Meet` VALUES('{0}', '{1}')",
                            CurrentUser.GetId, m.ID);
                        try
                        {
                            MySQLHelper.execute(querry);
                            MessageBox.Show("Вы успешно записаны!", "Участие", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch { MessageBox.Show("Вы уже записаны!", "Участие", MessageBoxButton.OK, MessageBoxImage.Warning); MySQLHelper.getDb().Close(); }
                        break;
                    }
                }

                if (!check)
                    MessageBox.Show("Нельзя записаться на своё событие!", "Участие", MessageBoxButton.OK, MessageBoxImage.Error);
            }
       }

        private new void MouseLeave(object sender, MouseEventArgs e)
        {
            pininfo.Hide();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.GetPosition(this).X < PanelShowZone.Width.Value)
            {
                LeftPanel.Visibility = Visibility.Visible;
            } else if (e.GetPosition(Panel).X > PanelShowZone.ActualWidth + Panel.ActualWidth)
            {
                LeftPanel.Visibility = Visibility.Hidden;
            }
        }

        private void LeftPanel_MouseLeave(object sender, MouseEventArgs e) => UpdateMap();
    }
}
