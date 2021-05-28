using Chess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chess
{
    class Figure
    {
        public Side Side;
        public FigureType Type;
        public int Steps;
        public Figure(Side side,FigureType type)
        {
            Side = side;
            Type=type;
            Steps = 0;
            
        }
        

    }
}
