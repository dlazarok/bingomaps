using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using Windows.ApplicationModel.Resources;

namespace Maps
{
    class MyTable
    {
        private string Id;
        public MyTable(string Id, string CreatorName, 
            string Category, string Description, string Participants, string Date)
        {
            this.CreatorName = CreatorName;
            this.Id = Id;
            this.Category = Category;
            this.Description = Description;
            this.Participants = Participants;
            this.Date = Date;
        }
        public string CreatorName { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Participants { get; set; }
        public string Date { get; set; }
        public string GetIdMeet() => Id;
    }

    public partial class MeetWindow : Window
    {
        User CurrentUser;

        BindingList<Meet> meetings = new BindingList<Meet>();
        BindingList<Meet> userMeet = new BindingList<Meet>();

        MySQLCon MySQLHelper;
        public MeetWindow(User u, MySQLCon mh, BindingList<Meet> um, BindingList<Meet> m)
        {
            meetings = m;
            userMeet = um;
            CurrentUser = u;
            MySQLHelper = mh;
            InitializeComponent();
            ReLoader();
        }
        private void ReLoader()
        {
            {
                string querry = String.Format("SELECT idMeet, People.Name, Type.Name, Description, Participants, Date " +
                    "FROM People_has_Meet inner join Meet USING(idMeet) inner join Type USING(idType) inner join People" +
                    " on People.idPeople = Meet.idCreator where People_has_Meet.idPeople = {0}",
                    CurrentUser.GetId);
                List<string[]> res = MySQLHelper.getValues(querry);
                List<MyTable> result = new List<MyTable>();

                foreach (string[] row in res)
                {
                    result.Add(new MyTable(row[0], row[1], row[2], row[3], row[4], row[5]));

                }
                dgCheckMeets.ItemsSource = result;
            }

            {
                string querry = String.Format("SELECT idMeet, People.Name, Type.Name, Description, Participants, Date " +
                    "FROM Meet inner join Type USING(idType) inner join People" +
                    " on People.idPeople = Meet.idCreator where Meet.idCreator = {0}",
                    CurrentUser.GetId);
                List<string[]> res = MySQLHelper.getValues(querry);
                List<MyTable> result = new List<MyTable>();

                foreach (string[] row in res)
                {
                    result.Add(new MyTable(row[0], row[1], row[2], row[3], row[4], row[5]));

                }
                dgRedMeets.ItemsSource = result;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dgCheckMeets.SelectedIndex != -1)
            {
                MyTable path = dgCheckMeets.SelectedItem as MyTable;
                MessageBox.Show("Вы успешно вышли!", "Выход", MessageBoxButton.OK, MessageBoxImage.Information);
                string querry = String.Format("DELETE FROM People_has_Meet WHERE idMeet = {0} and idPeople = {1}",
                    path.GetIdMeet(), CurrentUser.GetId);
                MySQLHelper.execute(querry);
            }
            ReLoader();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (dgRedMeets.SelectedIndex != -1)
            {
                MyTable path = dgRedMeets.SelectedItem as MyTable;
                MessageBox.Show("Вы успешно удалили событие!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                {
                    string querry = String.Format("DELETE FROM People_has_Meet WHERE idMeet = {0}",
                      path.GetIdMeet());
                    MySQLHelper.execute(querry);
                }
                {
                    string querry = String.Format("DELETE FROM Meet WHERE idMeet = {0}",
                        path.GetIdMeet());
                    MySQLHelper.execute(querry);
                }
            }
            ReLoader();
        }
    }
}
