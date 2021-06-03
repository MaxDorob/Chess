using Chess.Models;
using System;

namespace Chess
{
    class Figure : ICloneable
    {
        public Side Side;
        public FigureType Type;
        public int Steps;

        public Figure(Side side, FigureType type)
        {
            Side = side;
            Type = type;
            Steps = 0;

        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
