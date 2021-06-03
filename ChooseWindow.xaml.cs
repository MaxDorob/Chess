using Chess.Models;
using System.Windows;

namespace Chess
{
    /// <summary>
    /// Логика взаимодействия для ChooseWindow.xaml
    /// </summary>
    public partial class ChooseWindow : Window
    {
        public FigureType choosed = (FigureType)1;
        public ChooseWindow()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            choosed = FigureType.Horse;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            choosed = FigureType.Elephant;
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            choosed = FigureType.Rook;
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            choosed = FigureType.Queen;
            this.Close();
        }
    }
}
