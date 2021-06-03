using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chess
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int xSelected = -1, ySelected = -1;
        Image Selected = new Image() { Source = new BitmapImage(new Uri("Img/Selected.png", UriKind.Relative)) };
        Image Selection = new Image() { Source = new BitmapImage(new Uri("Img/Selection.png", UriKind.Relative)) };
        Models.Side PlayerSide = Models.Side.White;
        bool SameStation = true;//Если на одном экземпляре 2ое людей
        ChessGame game;
        public MainWindow()
        {
            game = new ChessGame();
            game.ChooseCall += Game_ChooseCall;
            InitializeComponent();

            Selection.Visibility = Visibility.Collapsed;
            Selection.Focusable = false;
            Selected.Visibility = Visibility.Collapsed;
            Selected.Focusable = false;
            BitmapImage imageDark = new BitmapImage(new Uri("Img/landing-first.jpg", UriKind.Relative));
            BitmapImage imageLight = new BitmapImage(new Uri("Img/landing-second.jpg", UriKind.Relative));

            for (int y = 0; y < 9; y++)
            {
                //ChessGrid.RowDefinitions.Add(new RowDefinition());
                for (int x = 0; x < 9; x++)
                {
                    if (x == 0 && y == 0) continue;
                    if (x > 0 && y == 0)
                    {

                        Label label = new Label() { Content = (char)('A' + x - 1) };
                        label.MouseEnter += HideSelection;
                        label.HorizontalAlignment = HorizontalAlignment.Center;
                        label.VerticalAlignment = VerticalAlignment.Bottom;
                        label.Margin = new Thickness(0, 0, 0, 0);
                        ChessGrid.Children.Add(label);
                        Grid.SetRow(label, y);
                        Grid.SetColumn(label, x);
                        continue;
                    }
                    if (x == 0 && y > 0)
                    {
                        Label label = new Label() { Content = (8 - y + 1).ToString() };
                        label.MouseEnter += HideSelection;
                        label.HorizontalAlignment = HorizontalAlignment.Right;
                        label.VerticalAlignment = VerticalAlignment.Center;
                        label.Margin = new Thickness(0, 0, 0, 0);
                        ChessGrid.Children.Add(label);
                        Grid.SetRow(label, y);
                        Grid.SetColumn(label, x);
                        continue;
                    }
                    UIElement added;
                    if (y % 2 + x % 2 == 0 || y % 2 + x % 2 == 2)
                    {
                        added = ChessGrid.Children[ChessGrid.Children.Add(new Image() { Source = imageLight, Stretch = Stretch.Uniform })];
                    }
                    else
                    {
                        added = ChessGrid.Children[ChessGrid.Children.Add(new Image() { Source = imageDark, Stretch = Stretch.Uniform })];

                    }
                    added.MouseEnter += MoveAndShowSelection;
                    added.MouseLeave += Added_MouseLeave;

                    Grid.SetRow(added, y);
                    Grid.SetColumn(added, x);

                }
            }
            ChessGrid.Children.Add(Selected);
            ChessGrid.Children.Add(Selection);
            Selection.MouseUp += ClickOnField;
            Selected.MouseEnter += MoveAndShowSelection;
            DrawCurrentSituation();
            Panel.SetZIndex(Selection, 900);
            //var addedTest = ChessGrid.Children[ChessGrid.Children.Add(new Image() { Source = Selected, Stretch = Stretch.Uniform })];
            //Grid.SetRow(addedTest, 1);
            //Grid.SetColumn(addedTest, 1);
        }

        private Models.FigureType Game_ChooseCall()
        {
            ChooseWindow window = new ChooseWindow();
            window.ShowDialog();
            return window.choosed;
        }

        List<Image> FigureImages;
        private void DrawCurrentSituation()
        {
            if (FigureImages != null)
                foreach (var item in FigureImages)
                {
                    ChessGrid.Children.Remove(item);
                }
            FigureImages = new List<Image>(game.Figures.Count);
            foreach (var figure in game.Figures)
            {
                string path = $"Img/{figure.Value.Type}{figure.Value.Side}.png";
                FigureImages.Add(new Image() { Source = new BitmapImage(new Uri(path, UriKind.Relative)), Stretch = Stretch.Uniform });
                ChessGrid.Children.Add(FigureImages.Last());
                FigureImages.Last().MouseEnter += MoveAndShowSelection;
                FigureImages.Last().MouseLeave += Added_MouseLeave;
                Grid.SetColumn(FigureImages.Last(), (int)figure.Key.X + 1);
                Grid.SetRow(FigureImages.Last(), (int)figure.Key.Y + 1);


            }

        }
        List<Image> AvailablePointsImages;

        private void ClickOnField(object sender, MouseButtonEventArgs e)
        {
            int xClicked = Grid.GetColumn(sender as UIElement);
            int yClicked = Grid.GetRow(sender as UIElement);
            if (xSelected == -1 || ySelected == -1)
            {
                Figure figure;
                if (!game.Figures.TryGetValue(new Point(xClicked - 1, yClicked - 1), out figure))
                    return;
                if (figure.Side != PlayerSide)
                    return;
                xSelected = xClicked;
                ySelected = yClicked;
                AvailablePointsImages = new List<Image>();
                foreach (var point in game.AvailableForFigure(new Point(xClicked - 1, yClicked - 1)))
                {
                    var currentImg = new Image() { Source = new BitmapImage(new Uri("Img/Available.png", UriKind.Relative)), Stretch = Stretch.Uniform };
                    Panel.SetZIndex(currentImg, 899);
                    currentImg.MouseEnter += MoveAndShowSelection;
                    currentImg.MouseLeave += Added_MouseLeave;

                    ChessGrid.Children.Add(currentImg);
                    Grid.SetColumn(currentImg, (int)point.X + 1);
                    Grid.SetRow(currentImg, (int)point.Y + 1);
                    AvailablePointsImages.Add(currentImg);

                }
                Selected.Visibility = Visibility.Visible;
                Grid.SetColumn(Selected, xClicked);
                Grid.SetRow(Selected, yClicked);
                return;
            }
            if (xClicked == xSelected && yClicked == ySelected)
            {
                foreach (var item in AvailablePointsImages)
                    ChessGrid.Children.Remove(item);
                AvailablePointsImages = null;
                Selected.Visibility = Visibility.Collapsed;
                xSelected = -1;
                yClicked = -1;
            }
            if (xSelected != -1 && ySelected != -1)
            {
                if (game.MoveFigureAt(new Point(xSelected - 1, ySelected - 1), new Point(xClicked - 1, yClicked - 1)))
                {
                    DrawCurrentSituation();
                    xSelected = -1;
                    ySelected = -1;
                    Selected.Visibility = Visibility.Collapsed;
                    foreach (var item in AvailablePointsImages)
                        ChessGrid.Children.Remove(item);
                    if (SameStation)
                        PlayerSide = (Models.Side)(-(int)PlayerSide);

                }

            }
            // Добавить действие

        }

        private void Added_MouseLeave(object sender, MouseEventArgs e)
        {
            //Selection.Visibility = Visibility.Collapsed;
        }

        private void MoveAndShowSelection(object sender, MouseEventArgs e)
        {

            Selection.Visibility = Visibility.Visible;
            Grid.SetColumn(Selection, Grid.GetColumn((UIElement)sender));
            Grid.SetRow(Selection, Grid.GetRow((UIElement)sender));
        }

        private void HideSelection(object sender, MouseEventArgs e)
        {
            Selection.Visibility = Visibility.Collapsed;
        }
    }
}
