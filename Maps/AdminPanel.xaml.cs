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
using System.Windows.Shapes;

namespace Maps
{
    class MyTable2
    {
        private string Id;
        public MyTable2(string Id, string Login, string Nickname, string Time, string Device, string Domen)
        {
            this.Login = Login;
            this.Id = Id;
            this.Nickname = Nickname;
            this.Time = Time;
            this.Device = Device;
            this.Domen = Domen;
        }
        public string Login { get; set; }
        public string Nickname { get; set; }
        public string Time { get; set; }
        public string Device { get; set; }
        public string Domen { get; set; }
        public string GetId() => Id;
    }

    public partial class AdminPanel : Window
    {
        MySQLCon MySQLHelper;
        public AdminPanel(MySQLCon MySQLHelper, bool admin)
        {
            this.MySQLHelper = MySQLHelper;
            InitializeComponent();
            logs.Visibility = admin ? Visibility.Visible : Visibility.Hidden;
            ReLoader();
        }

        private void ReLoader()
        {
            string querry = "SELECT idMeet, People.Name, Type.Name, Description, Participants, Date " +
                "FROM Meet inner join Type USING(idType) inner join People" +
                " on People.idPeople = Meet.idCreator";
            List<string[]> res = MySQLHelper.getValues(querry);
            List<MyTable> result = new List<MyTable>();

            foreach (string[] row in res)
            {
                result.Add(new MyTable(row[0], row[1], row[2], row[3], row[4], row[5]));

            }
            dgRedMeets.ItemsSource = result;
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

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReLoaderLogs();
        }

        private void ReLoaderLogs()
        {
            MySQLCon logcon =
                        new MySQLCon("server171.hosting.reg.ru", "u1088253_logs", "u1088253_logs_bd", "Qwer6727");
            string querry = "SELECT idlog, Login, Nickname, Time, Device, Domen FROM log INNER join user using(idUser)";
            List<string[]> res = logcon.getValues(querry);
            List<MyTable2> result = new List<MyTable2>();

            foreach (string[] row in res)
            {
                result.Add(new MyTable2(row[0], row[1], row[2], row[3], row[4], row[5]));

            }
            dgLogs.ItemsSource = result;
        }
    }
}
