using Chess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public Models.Side PlayerSide = Models.Side.White;
        //public bool SameStation = true, stupidBot = false, SmartBot = false, BotVsBot = false, Net = false;//Если на одном экземпляре 2ое людей
        GameType gameType = GameType.NotInited;
        ChessGame game;
        public NetSession session;
        bool ended = false;
        Timer BotMoveTimer;

        void UIInit()
        {
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
                    Panel.SetZIndex(Selection, 900);

                }
            }
            ChessGrid.Children.Add(Selected);
            ChessGrid.Children.Add(Selection);
            Selection.MouseUp += ClickOnField;
            Selected.MouseEnter += MoveAndShowSelection;
        }
        public MainWindow()
        {

            game = new ChessGame();
            game.ChooseCall += Game_ChooseCall;
            game.CheckAndMateAction += Game_CheckAndMateAction;
            InitializeComponent();
            UIInit();
            DrawCurrentSituation();
            

            
        }
        #region Inititalization
        public void InitSameStation()
        {
            if(gameType!=GameType.NotInited)
                throw new Exception("AlreadyInited");
            gameType = GameType.SameStaion;
        }
        public void InitStupidBot()
        {
            if (gameType != GameType.NotInited)
                throw new Exception("AlreadyInited");
            gameType = GameType.StupidBot;
        }
        public void InitSmartBot()
        {
            if (gameType != GameType.NotInited)
                throw new Exception("AlreadyInited");
            gameType = GameType.SmartBot;
        }
        public void InitBotVsBot()
        {
            if (gameType != GameType.NotInited)
                throw new Exception("AlreadyInited");
            gameType = GameType.BotVsBot;
            BotMoveTimer = new Timer(new TimerCallback(BotVsBotStart), null, 3000, 1000);
        }
        public void InitNetSession(NetSession session)
        {
            if (gameType != GameType.NotInited)
                throw new Exception("AlreadyInited");
            gameType = GameType.NetSession;
            this.session = session;
            session.OnActionReady += Session_OnActionReady;
            if (PlayerSide == Side.Black)
                Task.Run(session.AsyncGetAction);

        }
        #endregion

        private void BotVsBotStart(object obj)
        {

            AI aiMove = new AI();
            aiMove.MoveExecute += AiMove_MoveExecute;
            game.ChooseCall -= Game_ChooseCall;
            var gameToCheck = game.Clone() as ChessGame;
            gameToCheck.ChooseCall -= Game_ChooseCall;
            gameToCheck.CheckAndMateAction -= Game_CheckAndMateAction;
            int depth = game.Figures.Count < 8 ? (game.Figures.Count < 5 ? (game.Figures.Count < 2 ? 2 : 2) : 1) : 0;
            aiMove.Calc(gameToCheck, depth);


            BotMoveTimer.Change(0, Timeout.Infinite);

        }
        private void Game_CheckAndMateAction(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(DrawCurrentSituation));
            ended = true;
            MessageBox.Show($"Игра окончена. Шах и мат для {game.HoldingStep}");

            this.Dispatcher.BeginInvoke(new Action(() => this.Close()));
        }

        private Models.FigureType Game_ChooseCall()
        {
            ChooseWindow window = new ChooseWindow();
            window.ShowDialog();
            botChoose = window.choosed;
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
            if (gameType==GameType.BotVsBot)
                return;
            int xClicked = Grid.GetColumn(sender as UIElement);
            int yClicked = Grid.GetRow(sender as UIElement);
            if (xSelected == -1 || ySelected == -1)
            {
                Figure figure;
                if (!game.Figures.TryGetValue(new Point(xClicked - 1, yClicked - 1), out figure))
                    return;
                if (figure.Side != PlayerSide)
                    return;
                if (gameType==GameType.NetSession && PlayerSide != game.HoldingStep)
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
                Point moveFrom = new Point(xSelected - 1, ySelected - 1), moveTo = new Point(xClicked - 1, yClicked - 1);
                if (game.MoveFigureAt(moveFrom, moveTo))
                {
                    DrawCurrentSituation();
                    xSelected = -1;
                    ySelected = -1;
                    Selected.Visibility = Visibility.Collapsed;
                    foreach (var item in AvailablePointsImages)
                        ChessGrid.Children.Remove(item);

                    if(!ended)
                    switch (gameType)
                    {
                        case GameType.NotInited:
                            throw new Exception("NotInited");
                            
                        case GameType.SameStaion:
                            PlayerSide = (Models.Side)(-(int)PlayerSide);
                            break;
                        case GameType.StupidBot:
                            var botFigures = game.Figures.Where(x => x.Value.Side != PlayerSide).ToList();
                            int indexToMove = 0;
                            List<Point> availableForThatFigure = null;
                            while (availableForThatFigure == null || availableForThatFigure.Count == 0)
                            {
                                indexToMove = new Random().Next(botFigures.Count);
                                availableForThatFigure = game.AvailableForFigure(botFigures[indexToMove]);
                            }
                            game.MoveFigureAt(botFigures[indexToMove].Key, availableForThatFigure[new Random().Next(availableForThatFigure.Count)]);
                            DrawCurrentSituation();
                            break;
                        case GameType.SmartBot:
                            AI aiMove = new AI();
                            aiMove.MoveExecute += AiMove_MoveExecute;
                            game.ChooseCall -= Game_ChooseCall;
                            var gameToCheck = game.Clone() as ChessGame;
                            gameToCheck.ChooseCall -= Game_ChooseCall;
                            gameToCheck.CheckAndMateAction -= Game_CheckAndMateAction;
                            int depth = game.Figures.Count < 16 ? (game.Figures.Count < 8 ? (game.Figures.Count < 4 ? 4 : 2) : 1) : 0;
                            aiMove.Calc(gameToCheck, depth);
                            break;
                        case GameType.BotVsBot:
                            break;
                        case GameType.NetSession:
                            session.TrySendAction(moveFrom, moveTo, (int)botChoose);
                            Task.Run(session.AsyncGetAction);
                            break;
                        default:
                            throw new Exception("NotInited");
                            break;
                    }
                }

            }
            // Добавить действие

        }
        FigureType botChoose;
        private void AiMove_MoveExecute(Point arg1, Point arg2, FigureType arg3)
        {
            if (ended)
                return;
            botChoose = arg3;
            game.ChooseCall += BotChooseCall;
            this.Dispatcher.BeginInvoke(new Action(() => game.MoveFigureAt(arg1, arg2))).Wait();
            game.ChooseCall -= BotChooseCall;
            game.ChooseCall += Game_ChooseCall;
            this.Dispatcher.BeginInvoke(new Action(DrawCurrentSituation));
            if (!ended && BotMoveTimer!=null)
                BotMoveTimer.Change(100, 1000);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Session_OnActionReady(Point arg1, Point arg2, FigureType arg3)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                game.ChooseCall -= Game_ChooseCall;
                game.ChooseCall += NetChooseCall;
                botChoose = arg3;
                game.MoveFigureAt(arg1, arg2);
                game.ChooseCall -= NetChooseCall;
                game.ChooseCall += Game_ChooseCall;
                DrawCurrentSituation();
            }));

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            session?.TryClose();
        }

        private FigureType NetChooseCall()
        {
            return botChoose;
        }

        private FigureType BotChooseCall()
        {
            return botChoose;
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
