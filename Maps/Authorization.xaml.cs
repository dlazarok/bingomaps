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
using static Maps.MySQLCon;

namespace Maps
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        MySQLCon MySQLHelper;
        public Authorization()
        {
            MySQLHelper = 
                new MySQLCon("server171.hosting.reg.ru", "u1088253_maps", "u1088253_maps_bd", "Qwer6727");
            InitializeComponent();
            
        }

        [Obsolete]
        private void but1_Click_1(object sender, RoutedEventArgs e)
        {
            if(tbLogin.Text != "" && pbPass.Password != "")
                try
                {
                    string querry = 
                        String.Format("SELECT Name, (SELECT idPeople FROM People WHERE idAuthorization = Authorizate('{0}', '{1}')), idRole " +
                        "FROM People WHERE idAuthorization = Authorizate('{0}', '{1}');",
                        //SELECT Name, (SELECT idPeople FROM People WHERE idAuthorization = Authorizate('Denis', '123456')), idRole FROM People WHERE idAuthorization = Authorizate('Denis', '123456');
                        tbLogin.Text, pbPass.Password);

                    List<string[]> userinfo = MySQLHelper.getValues(querry);

                    
                    MySQLCon logcon = 
                        new MySQLCon("server171.hosting.reg.ru", "u1088253_logs", "u1088253_logs_bd", "Qwer6727");
                    try
                    {
                    
                        logcon.execute(String.Format("INSERT INTO user " +
                            "VALUES(null, '{0}', '{1}')", tbLogin.Text, userinfo[0][0]));
                    }
                    catch
                    {
                        logcon.getDb().Close();
                    }
                    try
                    {
                        logcon.execute(String.Format("INSERT INTO log " +
                        "VALUES(null, (SELECT idUser FROM user where Login = '{0}'), now(), '{1}', '{2}')",
                        tbLogin.Text, Environment.OSVersion, Environment.UserDomainName));
                    }
                    catch { logcon.getDb().Close();  }

                    if (userinfo[0][2] == "2")
                    {
                        new MainWindow(new User(userinfo[0][0], userinfo[0][1]), MySQLHelper).Show();
                    } else if (userinfo[0][2] == "3")
                    {
                        new AdminPanel(MySQLHelper, false).Show();
                    }
                    else
                    {
                        new AdminPanel(MySQLHelper, true).Show();
                    }
                    Close();
                }
                catch {
                    MessageBox.Show("Неверный логин или пароль!", "Вход", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            else
            {
                MessageBox.Show("Заполните все поля!", "Вход", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new Registration(MySQLHelper).ShowDialog();
        }
    }
}
