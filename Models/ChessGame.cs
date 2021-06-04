using Chess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Chess
{
    class ChessGame : ICloneable
    {
        public ChessGame PreviusState, NextState;
        public Dictionary<Point, Figure> Figures;
        public Models.Side HoldingStep;
        public bool CheatOnlyMineSteps = false;
        private Point LastPawnDoubleStep = new Point(-1, -1);
        bool Dang = false, BDang = false;//Шах
        bool inDepth = true;
        public ChessGame()
        {
            HoldingStep = Models.Side.White;
            Figures = new Dictionary<Point, Figure>(32);
            Figures.Add(new Point(0, 0), new Figure(Models.Side.Black, Models.FigureType.Rook));
            Figures.Add(new Point(7, 0), new Figure(Models.Side.Black, Models.FigureType.Rook));
            Figures.Add(new Point(0, 7), new Figure(Models.Side.White, Models.FigureType.Rook));
            Figures.Add(new Point(7, 7), new Figure(Models.Side.White, Models.FigureType.Rook));

            Figures.Add(new Point(1, 0), new Figure(Models.Side.Black, Models.FigureType.Horse));
            Figures.Add(new Point(6, 0), new Figure(Models.Side.Black, Models.FigureType.Horse));
            Figures.Add(new Point(6, 7), new Figure(Models.Side.White, Models.FigureType.Horse));
            Figures.Add(new Point(1, 7), new Figure(Models.Side.White, Models.FigureType.Horse));

            Figures.Add(new Point(2, 0), new Figure(Models.Side.Black, Models.FigureType.Elephant));
            Figures.Add(new Point(5, 0), new Figure(Models.Side.Black, Models.FigureType.Elephant));
            Figures.Add(new Point(2, 7), new Figure(Models.Side.White, Models.FigureType.Elephant));
            Figures.Add(new Point(5, 7), new Figure(Models.Side.White, Models.FigureType.Elephant));

            Figures.Add(new Point(3, 0), new Figure(Models.Side.Black, Models.FigureType.King));
            Figures.Add(new Point(3, 7), new Figure(Models.Side.White, Models.FigureType.King));

            Figures.Add(new Point(4, 0), new Figure(Models.Side.Black, Models.FigureType.Queen));
            Figures.Add(new Point(4, 7), new Figure(Models.Side.White, Models.FigureType.Queen));
            for (int i = 0; i < 8; i++)
            {
                Figures.Add(new Point(i, 1), new Figure(Models.Side.Black, Models.FigureType.Pawn));
                Figures.Add(new Point(i, 6), new Figure(Models.Side.White, Models.FigureType.Pawn));
            }
        }
        private ChessGame(Dictionary<Point, Figure> ToCopy, Side side)
        {
            Figures = new Dictionary<Point, Figure>(ToCopy.Count);
            foreach (var item in ToCopy)
                Figures.Add(item.Key, item.Value.Clone() as Figure);
            HoldingStep = side;

        }

        public List<Point> AvailableForFigure(Point figurePos)
        {
            return AvailableForFigure(new KeyValuePair<Point, Figure>(figurePos, Figures[figurePos]));
        }

        public List<Point> AvailableForFigure(KeyValuePair<Point, Figure> pair)
        {
            List<Point> toReturn = new List<Point>();
            double xOfFigure = pair.Key.X;
            double yOfFigure = pair.Key.Y;
            Figure figure = pair.Value;
            int i = 1;
            bool
                up = true,
                down = true,
                rightUp = true,
                right = true,
                rightDown = true,

                leftDown = true,
                left = true,
                leftUp = true;
            Point moveAt = new Point(-1, -1);
            switch (pair.Value.Type)
            {
                case Models.FigureType.Pawn:
                    //if (Dang && figure.Side == HoldingStep) return toReturn;
                    moveAt = new Point(xOfFigure, yOfFigure + (int)figure.Side);
                    if (!Figures.ContainsKey(moveAt) && PointInField(moveAt))//движение вперед
                    {
                        toReturn.Add(moveAt);
                        if (figure.Steps == 0)//возможность движения при первом ходе пешки
                        {
                            moveAt = new Point(xOfFigure, yOfFigure + ((int)figure.Side * 2));
                            if (!Figures.ContainsKey(moveAt) && PointInField(moveAt))
                                toReturn.Add(moveAt);
                        }
                    }
                    moveAt = new Point(xOfFigure + 1, yOfFigure + (int)figure.Side);
                    if (Figures.ContainsKey(moveAt) && Figures[moveAt].Side != figure.Side)
                        toReturn.Add(moveAt);//рубить справа
                    moveAt = new Point(xOfFigure - 1, yOfFigure + (int)figure.Side);//Добавить взятие на проходе
                    if (Figures.ContainsKey(moveAt) && Figures[moveAt].Side != figure.Side)
                        toReturn.Add(moveAt);//рубить слева
                    if (LastPawnDoubleStep.Y == yOfFigure && ((int)Math.Abs(xOfFigure - LastPawnDoubleStep.X) == 1))
                        toReturn.Add(new Point(LastPawnDoubleStep.X, yOfFigure + (int)figure.Side));
                    break;
                case Models.FigureType.Horse:
                    //if (Dang && figure.Side == HoldingStep) return toReturn;
                    List<Point> pointsToAdd = new List<Point>()
                    {
                        new Point(xOfFigure-1,yOfFigure+2),
                        new Point(xOfFigure+1,yOfFigure+2),
                        new Point(xOfFigure+2,yOfFigure+1),
                        new Point(xOfFigure+2,yOfFigure-1),
                        new Point(xOfFigure+1,yOfFigure-2),
                        new Point(xOfFigure-1,yOfFigure-2),
                        new Point(xOfFigure-2,yOfFigure-1),
                        new Point(xOfFigure-2,yOfFigure+1)
                    };
                    toReturn.AddRange(pointsToAdd.Where(x => PointInField(x) && !Figures.Where(y => y.Value.Side == figure.Side).Any(y => y.Key == x)).ToList());//ispravit

                    break;
                case Models.FigureType.Elephant:
                    //if (Dang && figure.Side == HoldingStep) return toReturn;
                    while (rightUp || rightDown || leftDown || leftUp)
                    {
                        if (rightDown)
                        {
                            moveAt = new Point(xOfFigure + i, yOfFigure + i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        rightDown = false;
                                }
                                else
                                    rightDown = false;
                            }
                            else
                                rightDown = false;
                        }
                        if (rightUp)
                        {
                            moveAt = new Point(xOfFigure + i, yOfFigure - i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        rightUp = false;
                                }
                                else
                                    rightUp = false;
                            }
                            else
                                rightUp = false;
                        }
                        if (leftDown)
                        {
                            moveAt = new Point(xOfFigure - i, yOfFigure + i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        leftDown = false;
                                }
                                else
                                    leftDown = false;
                            }
                            else
                                leftDown = false;
                        }
                        if (leftUp)
                        {
                            moveAt = new Point(xOfFigure - i, yOfFigure - i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        leftUp = false;
                                }
                                else
                                    leftUp = false;
                            }
                            else
                                leftUp = false;
                        }
                        i++;

                    }

                    break;
                case Models.FigureType.Rook:
                    //if (Dang && figure.Side == HoldingStep) return toReturn;
                    while (up || right || down || left)
                    {
                        if (up)
                        {
                            moveAt = new Point(xOfFigure, yOfFigure - i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        up = false;
                                }
                                else
                                    up = false;
                            }
                            else
                                up = false;
                        }
                        if (left)
                        {
                            moveAt = new Point(xOfFigure - i, yOfFigure);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        left = false;
                                }
                                else
                                    left = false;
                            }
                            else
                                left = false;
                        }
                        if (down)
                        {
                            moveAt = new Point(xOfFigure, yOfFigure + i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        down = false;
                                }
                                else
                                    down = false;
                            }
                            else
                                down = false;
                        }
                        if (right)
                        {
                            moveAt = new Point(xOfFigure + i, yOfFigure);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        right = false;
                                }
                                else
                                    right = false;
                            }
                            else
                                right = false;
                        }
                        i++;
                    }
                    break;
                case Models.FigureType.Queen:
                    //if (Dang && figure.Side == HoldingStep) return toReturn;
                    while (rightUp || rightDown || leftDown || leftUp || up || right || down || left)
                    {
                        if (rightDown)
                        {
                            moveAt = new Point(xOfFigure + i, yOfFigure + i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        rightDown = false;
                                }
                                else
                                    rightDown = false;
                            }
                            else
                                rightDown = false;
                        }
                        if (rightUp)
                        {
                            moveAt = new Point(xOfFigure + i, yOfFigure - i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        rightUp = false;
                                }
                                else
                                    rightUp = false;
                            }
                            else
                                rightUp = false;
                        }
                        if (leftDown)
                        {
                            moveAt = new Point(xOfFigure - i, yOfFigure + i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        leftDown = false;
                                }
                                else
                                    leftDown = false;
                            }
                            else
                                leftDown = false;
                        }
                        if (leftUp)
                        {
                            moveAt = new Point(xOfFigure - i, yOfFigure - i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        leftUp = false;
                                }
                                else
                                    leftUp = false;
                            }
                            else
                                leftUp = false;
                        }
                        if (up)
                        {
                            moveAt = new Point(xOfFigure, yOfFigure - i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        up = false;
                                }
                                else
                                    up = false;
                            }
                            else
                                up = false;
                        }
                        if (left)
                        {
                            moveAt = new Point(xOfFigure - i, yOfFigure);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        left = false;
                                }
                                else
                                    left = false;
                            }
                            else
                                left = false;
                        }
                        if (down)
                        {
                            moveAt = new Point(xOfFigure, yOfFigure + i);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        down = false;
                                }
                                else
                                    down = false;
                            }
                            else
                                down = false;
                        }
                        if (right)
                        {
                            moveAt = new Point(xOfFigure + i, yOfFigure);
                            if (PointInField(moveAt))
                            {
                                if (!Figures.Where(x => x.Value.Side == figure.Side).Any(x => x.Key == moveAt))//Если никакая из союзных фигур не на пути
                                {
                                    toReturn.Add(moveAt);
                                    if (Figures.ContainsKey(moveAt))
                                        right = false;
                                }
                                else
                                    right = false;
                            }
                            else
                                right = false;
                        }
                        i++;
                    }
                    break;
                case Models.FigureType.King:
                    toReturn.AddRange(new List<Point>()
                    {
                        new Point(xOfFigure-1,yOfFigure-1),
                        new Point(xOfFigure,yOfFigure-1),
                        new Point(xOfFigure+1,yOfFigure-1),
                        new Point(xOfFigure-1,yOfFigure),
                        new Point(xOfFigure+1,yOfFigure),
                        new Point(xOfFigure-1,yOfFigure+1),
                        new Point(xOfFigure,yOfFigure+1),
                        new Point(xOfFigure+1,yOfFigure+1),

                    }.Where(x => PointInField(x)
                    && !Figures.Where(y => y.Value.Side == figure.Side).Any(y => y.Key == x))
                    .ToList());
                    if (figure.Steps == 0 && !Dang)
                    {
                        Figure rook;
                        if (Figures.TryGetValue(new Point(0, yOfFigure), out rook) && rook.Type == FigureType.Rook && rook.Steps == 0)
                            if (!Figures.Any(x => x.Key.X < xOfFigure && x.Key.Y == yOfFigure && x.Key.X != 0))
                            {
                                List<Point> betwen = new List<Point>() { new Point(1, yOfFigure), new Point(2, yOfFigure), new Point(3, yOfFigure) };
                                if (!betwen.Any(x => check(x, (Side)(-(int)HoldingStep))))
                                    toReturn.Add(new Point(xOfFigure - 2, yOfFigure));//Длинная рокировка, влево.
                            }
                        if (Figures.TryGetValue(new Point(7, yOfFigure), out rook) && rook.Type == FigureType.Rook && rook.Steps == 0)
                            if (!Figures.Any(x => x.Key.X > xOfFigure && x.Key.Y == yOfFigure && x.Key.X != 7))
                            {
                                List<Point> betwen = new List<Point>() { new Point(6, yOfFigure), new Point(5, yOfFigure)};
                                if (!betwen.Any(x => check(x, (Side)(-(int)HoldingStep))))
                                    toReturn.Add(new Point(xOfFigure + 2, yOfFigure));//Короткая рокировка, вправо.
                            }
                    }
                    break;
                default:
                    throw new Exception("UnknownMove");

            }

            if (inDepth)
            {
                List<Point> availableInCheck = new List<Point>();
                foreach (var item in toReturn)
                {
                    var futureVar = (ChessGame)this.Clone();
                    futureVar.DangerSource = new List<Point>();
                    futureVar.inDepth = false;
                    //futureVar.HoldingStep = (Models.Side)(-(int)HoldingStep);
                    futureVar.MoveFigureAt(new Point(xOfFigure, yOfFigure), item);
                    if (!futureVar.check(futureVar.Figures.First(x => x.Value.Side == HoldingStep && x.Value.Type == FigureType.King).Key, futureVar.HoldingStep))
                        availableInCheck.Add(item);
                }
                return availableInCheck;
            }

            return toReturn;
        }

        static bool PointInField(Point point)
        {
            if (point.X > 7 || point.Y > 7 || point.X < 0 || point.Y < 0)
                return false;
            return true;
        }
        
        

        private bool check(Point checkPoint, Side side)
        {
            //var king = Figures.First(x => x.Value.Type == Models.FigureType.King && x.Value.Side != HoldingStep);
            if (Figures.Where(x => x.Value.Side == side).Any(x => AvailableForFigure(x.Key).Contains(checkPoint))||Figures.Where(x=>x.Value.Side==side&&x.Value.Type==FigureType.Pawn).Any(x=>(x.Key.Y+(int)side)==checkPoint.Y&&(int)Math.Abs(checkPoint.X-x.Key.X)==1))
                return true;
            return false;
        }

        public delegate Models.FigureType ChooseAction();
        public event ChooseAction ChooseCall;
        public List<Point> DangerSource;
        public bool MoveFigureAt(Point PosOfFigureToMove, Point PosToMove)
        {

            Figure figure = Figures[PosOfFigureToMove];
            if (figure.Side != HoldingStep && !CheatOnlyMineSteps)
                throw new Exception("Invalid turn");
            if (AvailableForFigure(new KeyValuePair<Point, Figure>(PosOfFigureToMove, figure)).Contains(PosToMove))
            {
                //if (inDepth)
                //{
                    //ChessGame prevTemp=null;
                    //if (PreviusState != null)
                    //{
                        //prevTemp = PreviusState;
                    //}
                    //PreviusState = this.Clone() as ChessGame;

                //}
                if (Figures.ContainsKey(PosToMove))
                {
                    //рубить
                    Figures.Remove(PosToMove);
                }
                if (figure.Type == Models.FigureType.Pawn && LastPawnDoubleStep.Y == PosOfFigureToMove.Y && LastPawnDoubleStep.X == PosToMove.X)
                    Figures.Remove(LastPawnDoubleStep);
                if (figure.Type == Models.FigureType.Pawn && (int)Math.Abs(PosOfFigureToMove.Y - PosToMove.Y) == 2)
                    LastPawnDoubleStep = PosToMove;

                else
                    LastPawnDoubleStep = new Point(-1, -1);
                Figures.Remove(PosOfFigureToMove);
                Figures.Add(PosToMove, figure);
                if (figure.Type == Models.FigureType.Pawn && (PosToMove.Y == 0 || PosToMove.Y == 7)&& inDepth)
                    figure.Type = ChooseCall.Invoke();
                figure.Steps++;
                var king = Figures.FirstOrDefault(x => x.Value.Type == FigureType.King && x.Value.Side != HoldingStep);
                if (king.Value == null)
                    return false;
                //Dang = check(Figures.First(x => x.Value.Type == FigureType.King && x.Value.Side != HoldingStep).Key, HoldingStep);
                var  tempDangerSource = new List<Point>();
                foreach (var item in Figures.Where(x=>x.Value.Side==HoldingStep&&AvailableForFigure(x).Contains(king.Key)))
                {
                    tempDangerSource.Add(item.Key);
                }
                DangerSource = tempDangerSource;
                HoldingStep = (Models.Side)(-(int)HoldingStep);
                
                return true;

            }
            return false;
        }

        public object Clone()
        {
            return new ChessGame(Figures, HoldingStep);
        }
    }
}
