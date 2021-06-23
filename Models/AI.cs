using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chess.Models
{
    class AI
    {
        public event Action<Point, Point> MoveExecute;
        Point optimalMoveFrom, optimalMoveTo;
        double Max=double.MinValue;
        public async Task Calc(ChessGame game,int depth)
        {
            game.ChooseCall += Game_ChooseCall;

            foreach (var myFigure in game.Figures.Where(x=>x.Value.Side==game.HoldingStep))
            {
                foreach (var availableMove in game.AvailableForFigure(myFigure.Key))
                {

                }
            }


            MoveExecute.Invoke(new Point(0, 0), new Point(0, 0));
        }

        private FigureType Game_ChooseCall()
        {
            var temp =typesLeft[0];
            typesLeft.RemoveAt(0);
            return temp;
        }

        //private List<ChessGame> SituationsAfterChoose = null;
        private List<FigureType> typesLeft = null;
        //void CalcInDepth(ChessGame game,int depth)
        //{
        //    if (depth == 0)
        //    {
        //        if (game.GameCostCalc((Side)((int)game.HoldingStep)) < minMax)
        //        {

        //        }
        //    }
        //    else
        //    {
        //        foreach (var myFigure in game.Figures.Where(x => x.Value.Side == game.HoldingStep))
        //        {
        //            foreach (var availableMove in game.AvailableForFigure(myFigure.Key))
        //            {
        //                if(myFigure.)
        //            }
        //        }
        //    }
        //}
    }
}
