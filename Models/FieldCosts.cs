using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models
{
    static class FieldCosts
    {
        public static double KingMod = 900, queenMod = 90, rookMod = 50, ElephantMod = 30, horseMod = 30, pawnMod = 20;
        public static double[,] KingCost = new double[8, 8]
        {
            {-3,-4,-4,-5,-5,-4,-4,-3 },
            {-3,-4,-4,-5,-5,-4,-4,-3 },
            {-3,-4,-4,-5,-5,-4,-4,-3 },
            {-3,-4,-4,-5,-5,-4,-4,-3 },
            {-2,-3,-3,-4,-4,-3,-3,-2 },
            {-1,-2,-2,-2,-2,-2,-2,-1 },
            {2,2,0,0,0,0,2,2 },
            {2,3,1,0,0,1,3,2 },

        };//Invert for black
        public static double[,] QueenCost = new double[8, 8]
        {
            {-2,-1,-1,-0.5,-0.5,-1,-1,-2 },
            {-1,0,0,0,0,0,0,-1 },
            {-1,0,0.5,0.5,0.5,0.5,0,-1 },
           {-0.5,0,0.5,0.5,0.5,0.5,0,-0.5 },
           {0,0,0.5,0.5,0.5,0.5,0,-0.5 },
           {-1,0.5,0.5,0.5,0.5,0.5,0,-1 },
           {-1,0,0.5,0,0,0,0,-1 },
           {-2,-1,-1,-0.5,-0.5,-1,-1,-2 }


        };//Invert for black
        public static double[,] RookCost = new double[8, 8]
        {
            {0,0,0,0,0,0,0,0 },
            {0.5,1,1,1,1,1,1,0.5 },
            {-0.5,0,0,0,0,0,0,-0.5 },
            {-0.5,0,0,0,0,0,0,-0.5 },
            {-0.5,0,0,0,0,0,0,-0.5 },
            {-0.5,0,0,0,0,0,0,-0.5 },
            {-0.5,0,0,0,0,0,0,-0.5 },
            {0,0,0,0.5,0.5,0,0,0 }

        };

        public static double[,] ElephantCost = new double[8, 8]
        {
            {-2,-1,-1,-1,-1,-1,-1,-2 },
            {-1,0,0,0,0,0,0,-1 },
            {-1,0,0.5,1,1,0.5,0,-1 },
            {-1,0.5,0.5,1,1,0.5,0.5,-1 },
            {-1,0,1,1,1,1,0,-1 },
            {-1,1,1,1,1,1,1,-1 },
            {-1,0.5,0,0,0,0,0.5,-1 },
            {-2,-1,-1,-1,-1,-1,-1,-2 }
        };
        public static double[,] HorseCost = new double[8, 8]
        {
            {-5,-4,-3,-3,-3,-3,-4,-5 },
            {-4,-2,0,0,0,0,-2,-4 },
            {-3,0,1,1.5,1.5,1,0,-3 },
            {-3,0.5,1.5,2,2,1.5,0.5,-3 },
            {-3,0.5,1.5,2,2,1.5,0.5,-3 },
            {-3,0.5,1,1.5,1.5,1,0.5,-3 },
            {-4,-2,0,0.5,0.5,0,-2,-4 },
            {-5,-4,-3,-3,-3,-3,-4,-5 }
        };
        public static double[,] PawnCost = new double[8, 8]
        {
            {0,0,0,0,0,0,0,0 },
            {5,5,5,5,5,5,5,5 },
            {1,1,2,3,3,2,1,1 },
            {0.5,0.5,1,2.5,2.5,1,0.5,0.5 },
            {0,0,0,2,2,0,0,0 },
            {0.5,-0.5,-1,0,0,-1,-0.5,0.5 },
            {0.5,1,1,-2,-2,1,1,0.5 },
            {0,0,0,0,0,0,0,0 }

        };
        public static double GameCostCalc(this ChessGame game, Side positiveSide)
        {
            double toReturn = 0;
            foreach (var figureWhite in game.Figures.Where(x => x.Value.Side == Side.White))
            {
                double weight = double.NaN;
                switch (figureWhite.Value.Type)
                {
                    case FigureType.Pawn:
                        weight = (game.Figures.Count > 20 ? 1  : 1) * pawnMod * PawnCost[(int)figureWhite.Key.Y, (int)figureWhite.Key.X];
                        break;
                    case FigureType.Horse:
                        weight = (game.Figures.Count > 20 ? 3.8 : 1) * horseMod * HorseCost[(int)figureWhite.Key.Y, (int)figureWhite.Key.X];
                        break;
                    case FigureType.Elephant:
                        weight = (game.Figures.Count > 20 ? 2 : 1) * ElephantMod * ElephantCost[(int)figureWhite.Key.Y, (int)figureWhite.Key.X];
                        break;
                    case FigureType.Rook:
                        weight = (game.Figures.Count > 20 ? 1.8 : 1) * rookMod * RookCost[(int)figureWhite.Key.Y, (int)figureWhite.Key.X];
                        break;
                    case FigureType.Queen:
                        weight = (game.Figures.Count > 16 ? 3 : 1) * queenMod * QueenCost[(int)figureWhite.Key.Y, (int)figureWhite.Key.X];
                        break;
                    case FigureType.King:
                        weight = (game.Figures.Count>20?0.5 :1) *KingMod * KingCost[(int)figureWhite.Key.Y, (int)figureWhite.Key.X];
                        break;
                    default:
                        break;
                }
                if (positiveSide == Side.White)
                    toReturn += weight;
                else
                    toReturn -= weight;
            }
            foreach (var figureBlack in game.Figures.Where(x => x.Value.Side == Side.Black))
            {
                double weight = double.NaN;
                switch (figureBlack.Value.Type)
                {
                    case FigureType.Pawn:
                        weight = pawnMod * PawnCost[7 - (int)figureBlack.Key.Y, (int)figureBlack.Key.X];
                        break;
                    case FigureType.Horse:
                        weight = horseMod * HorseCost[7 - (int)figureBlack.Key.Y, (int)figureBlack.Key.X];
                        break;
                    case FigureType.Elephant:
                        weight = ElephantMod * ElephantCost[7 - (int)figureBlack.Key.Y, (int)figureBlack.Key.X];
                        break;
                    case FigureType.Rook:
                        weight = rookMod * RookCost[7 - (int)figureBlack.Key.Y, (int)figureBlack.Key.X];
                        break;
                    case FigureType.Queen:
                        weight = queenMod * QueenCost[7 - (int)figureBlack.Key.Y, (int)figureBlack.Key.X];
                        break;
                    case FigureType.King:
                        weight = KingMod * KingCost[7 - (int)figureBlack.Key.Y, (int)figureBlack.Key.X];
                        break;
                    default:
                        break;
                }
                if (positiveSide == Side.Black)
                    toReturn += weight;
                else
                    toReturn -= weight;

            }

            return toReturn;
        }
    }
}
