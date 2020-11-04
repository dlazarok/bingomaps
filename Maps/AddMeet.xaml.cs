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
    /// Логика взаимодействия для AddMeet.xaml
    /// </summary>
    public partial class AddMeet : Window
    {
        public AddMeet()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public string GetDescription { get => tb.Text; }
        public string GetDate { get => dp.Text; }
        public Category GetCategory()
        {
            if(cb.SelectedItem.ToString() == "Спорт")
            {
                return Category.Sport;
            } else if (cb.SelectedItem.ToString() == "Встреча")
            {
                return Category.Meet;
            } else if(cb.SelectedItem.ToString() == "Мероприятие")
            {
                return Category.Event;
            }
            return Category.Party;
        }

        public bool Success() => (GetDescription != "" && cb.SelectedIndex != -1 && GetDate != "");
    }
}
