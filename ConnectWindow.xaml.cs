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
    /// Логика взаимодействия для ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        public string IP;
        public short Port;
        public bool IsServer = false;
        public ConnectWindow()
        {
            InitializeComponent();
        }

        private void ServerStart(object sender, RoutedEventArgs e)
        {
            IP = null;
            Port = short.Parse(PortBox.Text);
            IsServer = true;
            DialogResult = true;
        }

        private void ClientStart(object sender, RoutedEventArgs e)
        {
            IP = IpBox.Text;
            Port = short.Parse(PortBox.Text);
            DialogResult = true;
        }
    }
}
