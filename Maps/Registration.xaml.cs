using MySql.Data.MySqlClient;
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
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        MySQLCon MySQLHelper;
        public Registration(MySQLCon mh)
        {
            MySQLHelper = mh;
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string p = MySQLHelper.getValue(String.Format("SELECT login from Authorization where login = '{0}'", tbLogin.Text));
                MessageBox.Show("Логин уже занят!", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception exee)
            {
                MySQLHelper.getDb().Close();
                try
                {
                    string p = MySQLHelper.getValue(String.Format("SELECT e-mail from People where e-mail = '{0}'", tbMail.Text));
                    MessageBox.Show("Е-Мейл уже зарегестрирован!", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch(Exception ex){

                    MySQLHelper.getDb().Close();
                    MySQLHelper.execute(String.Format("insert into Authorization values(null, '{0}', '{1}')", tbLogin.Text, tbPass.Text));
                    string s = MySQLHelper.getValue(String.Format("select idAuthorization " +
                        "from Authorization Where login = '{0}' and Password = '{1}'", tbLogin.Text, tbPass.Text));

                    MySQLHelper.execute(String.Format("insert into People " +
                        "values(null, '2', '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
                        s, tbNickName.Text, tbYears.Text, tbTele.Text, tbFIO.Text, tbCountry.Text, tbMail.Text));
                    MessageBox.Show("Регистрация успешна!", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();

                }

            }
        }
    }
}
