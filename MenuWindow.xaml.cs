using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            game.InitSameStation();
            game.ShowDialog();
            this.Show();
        }

        private void StupidBotGame(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow game = new MainWindow();
            game.InitStupidBot();
            game.ShowDialog();
            this.Show();
        }

        private void SmartBotGame(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow game = new MainWindow();
            game.InitSmartBot();
            game.ShowDialog();
            this.Show();
        }

        private void SmartBotFight(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow game = new MainWindow();
            game.InitBotVsBot();
            game.ShowDialog();
            this.Show();
        }

        private void NetSessionStart(object sender, RoutedEventArgs e)
        {
            ConnectWindow connectWindow = new ConnectWindow();
            if (connectWindow.ShowDialog().Value)
            {
                this.Hide();
                MainWindow game = new MainWindow();
                
                if (connectWindow.IsServer)
                {
                    game.InitNetSession(new Models.NetSession(IPAddress.Any, connectWindow.Port));
                    //game.session = new Models.NetSession(IPAddress.Any,connectWindow.Port);
                }
                else
                {
                    game.PlayerSide = Models.Side.Black;
                    game.InitNetSession(new Models.NetSession(connectWindow.IP, connectWindow.Port));

                }
                game.ShowDialog();
                this.Show();
            }
            
        }
    }
}
