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

namespace ImageTwisting
{
    /// <summary>
    /// Entrance.xaml 的交互逻辑
    /// </summary>
    public partial class Entrance : Window
    {
        public Entrance()
        {
            InitializeComponent();
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (cb.SelectionBoxItem.ToString() == "Twist")
            {
                (new MainWindow()).Show();
            }
            else if (cb.SelectionBoxItem.ToString() == "Distort")
            {
                (new Distort()).Show();
            }
            else if (cb.SelectionBoxItem.ToString() == "TPS")
            {
                (new TPS()).Show();
            }
        }
    }
}
