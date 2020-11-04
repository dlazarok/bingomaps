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

namespace Maps
{
    /// <summary>
    /// Логика взаимодействия для PinInfo.xaml
    /// </summary>
    public partial class PinInfo : UserControl
    {
        public PinInfo()
        {
            InitializeComponent();
        }

        public void Show(double x, double y, Category cat, string disc, string creator, string date, string par)
        {
            lbDesc.Content = "Описание: ";
            lbCategory.Content = "Категория: ";
            Visibility = Visibility.Visible;
            this.Margin = new Thickness(x, y, 0, 0);
            lbCategory.Content += Meet.PinType(cat);
            lbDesc.Content += disc;
            lbCreator.Content = creator;
            lbDate.Content = date;
        }

        public void Hide() => Visibility = Visibility.Hidden;
    }
}
