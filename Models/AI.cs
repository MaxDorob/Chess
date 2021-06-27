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
        const double checkMateCost = 15000;

        public event Action<Point, Point,FigureType> MoveExecute;
        Point optimalMoveFrom, optimalMoveTo;
        FigureType optimalType;
        double Max = double.MinValue;
        double avg = double.MinValue;
        public void Calc(ChessGame game, int depth)
        {
            game.ChooseCall += Game_ChooseCall;
            game.CheckAndMateAction += Game_CheckAndMateAction;
            List<List<StateInfo>> Summaries = new List<List<StateInfo>>();
            foreach (var myFigure in game.Figures.Where(x => x.Value.Side == game.HoldingStep))
            {
                foreach (var availableMove in game.AvailableForFigure(myFigure.Key))
                {
                    double currentMin, currentAvg;

                    if (myFigure.Value.Type == FigureType.Pawn && (availableMove.Y == 7 || availableMove.Y == 0))
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            type = (FigureType)i;
                            var newVarWithFigureChoose = game.Clone() as ChessGame;
                            newVarWithFigureChoose.MoveFigureAt(myFigure.Key, availableMove);//ход иишки
                            if (checkMate)
                            {
                                Summaries.Add(new List<StateInfo>() { new StateInfo() { game = newVarWithFigureChoose, choosenType = type, totalCost = checkMateCost } });
                                //toReturnSum.Add(new StateInfo() { game = newVarWithFigureChoose, choosenType = type, totalCost = double.MaxValue });
                                checkMate = false;
                            }
                            else
                                //toReturn.Add(new StateInfo() { game = newVarWithFigureChoose, choosenType = type, totalCost = newVarWithFigureChoose.GameCostCalc(game.HoldingStep) });
                                Summaries.Add(AllPossibleVariantsInDepth(newVarWithFigureChoose, depth));
                            if (Summaries.Last().Count == 0)
                                continue;
                            if ((currentMin = Summaries.Last().Min(x => x.totalCost)) > Max)
                            {
                                optimalMoveFrom = myFigure.Key;
                                optimalMoveTo = availableMove;
                                optimalType = type;
                                Max = currentMin;
                                avg = Summaries.Last().Average(x => x.totalCost);

                            }
                            else if (currentMin == Max && ((currentAvg = Summaries.Last().Average(x => x.totalCost)) > avg))
                            {
                                optimalMoveFrom = myFigure.Key;
                                optimalMoveTo = availableMove;
                                optimalType = type;
                                Max = currentMin;
                                avg = Summaries.Last().Average(x => x.totalCost);
                            }
                        }
                        continue;
                    }
                    var newVar = game.Clone() as ChessGame;
                    newVar.MoveFigureAt(myFigure.Key, availableMove);
                    if (checkMate)
                    {
                        Summaries.Add(new List<StateInfo>() { new StateInfo() { game = newVar, choosenType = type, totalCost = checkMateCost } });

                        //toReturn.Add(new StateInfo() { game = newVar, choosenType = type, totalCost = double.MaxValue });
                        //toReturnSum.Add(new StateInfo() { game = newVar, choosenType = type, totalCost = double.MaxValue });
                        checkMate = false;
                    }
                    else
                        //toReturn.Add(new StateInfo() { game = newVar, choosenType = type, totalCost = newVar.GameCostCalc(game.HoldingStep) });
                        Summaries.Add(AllPossibleVariantsInDepth(newVar, depth));
                    if (Summaries.Last().Count == 0)
                        continue;
                    if ((currentMin = Summaries.Last().Min(x => x.totalCost)) > Max)
                    {
                        optimalMoveFrom = myFigure.Key;
                        optimalMoveTo = availableMove;
                        optimalType = type;
                        Max = currentMin;
                        //avg = Summaries.Last().Average(x => x.totalCost);

                    }
                    //    //else if (currentMin == Max && ((currentAvg = Summaries.Last().Average(x => x.totalCost)) > avg))
                    //    //{
                    //    //    optimalMoveFrom = myFigure.Key;
                    //    //    optimalMoveTo = availableMove;
                    //    //    optimalType = type;
                    //    //    Max = currentMin;
                    //    //    avg = Summaries.Last().Average(x => x.totalCost);
                    //    //}
                    //}
                }


                
            }
            MoveExecute.Invoke(optimalMoveFrom, optimalMoveTo, type);
        }

        private void Game_CheckAndMateAction(object sender, EventArgs e)
        {
            checkMate = true;
        }

        private FigureType Game_ChooseCall()
        {
            return type;
        }

        //private List<ChessGame> SituationsAfterChoose = null;
        bool checkMate = false;
        FigureType type;
        List<StateInfo> AllPossibleVariantsInDepth(ChessGame game, int depth)
        {
            List<StateInfo> toReturnSum = new List<StateInfo>();
            List<StateInfo> toReturn = new List<StateInfo>();
            foreach (var myFigure in game.Figures.Where(x => x.Value.Side == game.HoldingStep))
            {
                foreach (var availableMove in game.AvailableForFigure(myFigure.Key))
                {
                    if (myFigure.Value.Type == FigureType.Pawn && (availableMove.Y == 7 || availableMove.Y == 0))
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            type = (FigureType)i;
                            var newVarWithFigureChoose = game.Clone() as ChessGame;
                            newVarWithFigureChoose.MoveFigureAt(myFigure.Key, availableMove);//ход противника иишки
                            if (checkMate)
                            {
                                toReturn.Add(new StateInfo() { game = newVarWithFigureChoose, choosenType = type, totalCost = -checkMateCost });
                                toReturnSum.Add(new StateInfo() { game = newVarWithFigureChoose, choosenType = type, totalCost = -checkMateCost });
                                checkMate = false;
                            }
                            else
                                toReturn.Add(new StateInfo() { game = newVarWithFigureChoose, choosenType = type, totalCost = newVarWithFigureChoose.GameCostCalc(newVarWithFigureChoose.HoldingStep) });
                            if (depth == 0 && toReturn.Last().totalCost < Max)
                                return new List<StateInfo>();
                        }
                        
                        continue;
                    }
                    var newVar = game.Clone() as ChessGame;
                    newVar.MoveFigureAt(myFigure.Key, availableMove);//ход противника иишки
                    if (checkMate)
                    {
                        toReturn.Add(new StateInfo() { game = newVar, choosenType = type, totalCost = -checkMateCost });
                        toReturnSum.Add(new StateInfo() { game = newVar, choosenType = type, totalCost = -checkMateCost });
                        checkMate = false;
                    }
                    else
                        toReturn.Add(new StateInfo() { game = newVar, choosenType = type, totalCost = newVar.GameCostCalc(newVar.HoldingStep) });
                    if (depth == 0 && toReturn.Last().totalCost < Max)
                        return new List<StateInfo>();

                }
            }
            if (depth == 0)
            {


                return toReturn;
            }
            else
            {
                List<List<StateInfo>> variantsToCheck = new List<List<StateInfo>>();
                foreach (var variant in toReturn)
                {
                    variantsToCheck.Add(new List<StateInfo>());
                    foreach (var myFigure in variant.game.Figures.Where(x => x.Value.Side == variant.game.HoldingStep))
                    {
                        foreach (var availableMove in variant.game.AvailableForFigure(myFigure.Key))
                        {
                            if (myFigure.Value.Type == FigureType.Pawn && (availableMove.Y == 7 || availableMove.Y == 0))
                            {
                                for (int i = 1; i < 5; i++)
                                {
                                    type = (FigureType)i;
                                    var newVarWithFigureChoose = game.Clone() as ChessGame;
                                    newVarWithFigureChoose.MoveFigureAt(myFigure.Key, availableMove);//ход иишки
                                    if (checkMate)
                                    {
                                        variantsToCheck.Last().Add(new StateInfo() { game = newVarWithFigureChoose, choosenType = type, totalCost = checkMateCost });
                                        checkMate = false;
                                    }
                                    else
                                        variantsToCheck.Last().AddRange(AllPossibleVariantsInDepth(variant.game, depth - 1));

                                }
                                continue;
                            }
                            var newVar = variant.game.Clone() as ChessGame;
                            newVar.MoveFigureAt(myFigure.Key, availableMove);
                            if (checkMate)
                            {
                                variantsToCheck.Last().Add(new StateInfo() { game = newVar, totalCost = checkMateCost, });
                                checkMate = false;
                            }
                            else
                                variantsToCheck.Last().AddRange(AllPossibleVariantsInDepth(variant.game, depth - 1));

                        }
                    }


                    
                }
                double lastMax = double.MinValue;
                for (int i = 0; i < variantsToCheck.Count; i++)
                {
                    double min;
                    //if (variantsToCheck[i].Count>0&&(min = variantsToCheck[i].Min(x => x.totalCost)) > lastMax)
                    //{
                    //    lastMax = min;
                    //    toReturnSum = variantsToCheck[i];
                    //}
                    if (variantsToCheck[i].Count > 0 && !(variantsToCheck[i].Any(x => x.totalCost < lastMax)))
                    {
                        lastMax = variantsToCheck[i].Min(x => x.totalCost);
                        toReturnSum = variantsToCheck[i];
                    }
                }
                return toReturnSum;
            }
        }
        struct StateInfo
        {
            public ChessGame game;
            public double totalCost;
            public FigureType choosenType;
        }
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
