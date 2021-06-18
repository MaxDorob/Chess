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

namespace Chess
{
    /// <summary>
    /// Логика взаимодействия для MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
        }

        private void Leave(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PlayWithSelf(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow game = new MainWindow();
            game.ShowDialog();
            this.Show();
        }

        private void StupidBotGame(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow game = new MainWindow();
            game.SameStation = false;
            game.stupidBot = true;
            game.ShowDialog();
            this.Show();
        }
    }
}
