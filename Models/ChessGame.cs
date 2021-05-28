using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chess
{
    class ChessGame
    {
        public Dictionary<Point, Figure> Figures;
        
        public ChessGame()
        {
            Figures = new Dictionary<Point, Figure>(32);
            for()
        }
        public List<Point> AvailableForFigure(KeyValuePair<Point,Figure> pair)
        {
            List<Point> toReturn = new List<Point>();
            double xOfFigure = pair.Key.X;
            double yOfFigure = pair.Key.Y;
            Figure figure = pair.Value;
            Point moveAt=new Point(-1,-1);
            switch (pair.Value.Type)
            {
                case Models.FigureType.Peshka:
                    moveAt = new Point(xOfFigure, yOfFigure + (int)figure.Type);
                    if (!Figures.ContainsKey(moveAt)&&PointInField(moveAt))//движение вперед
                    {
                        toReturn.Add(moveAt);
                        if (figure.Steps == 0)//возможность движения при первом ходе пешки
                        {
                            moveAt = new Point(xOfFigure, yOfFigure + ((int)figure.Type * 2));
                            if (!Figures.ContainsKey(moveAt) && PointInField(moveAt))
                                toReturn.Add(moveAt);
                        }
                    }
                    moveAt = new Point(xOfFigure+1, yOfFigure + (int)figure.Type);
                    if (Figures.ContainsKey(moveAt)&&Figures[moveAt].Side!=figure.Side)
                        toReturn.Add(moveAt);//рубить справа
                    moveAt = new Point(xOfFigure - 1, yOfFigure + (int)figure.Type);
                    if (Figures.ContainsKey(moveAt) && Figures[moveAt].Side != figure.Side)
                        toReturn.Add(moveAt);//рубить слева
                    break;
                case Models.FigureType.Horse:
                    List<Point> pointsToAdd = new List<Point>() { };
                    break;
                case Models.FigureType.Slon:
                    break;
                case Models.FigureType.Ladya:
                    break;
                case Models.FigureType.Ferz:
                    break;
                case Models.FigureType.King:
                    break;
                default:
                    break;
            }


            return toReturn;
        }
        static bool PointInField(Point point)
        {
            if (point.X > 7 || point.Y > 7 || point.X < 0 || point.Y < 0)
                return false;
            return true;
        }
    }
}
