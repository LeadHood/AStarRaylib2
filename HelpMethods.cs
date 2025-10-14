using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AStarRaylib
{
    internal static class HelpMethods
    {

        public static int CalculateH(Tile tile, Vector2 endPos)
        {
            //Calculate H
            int xDiff = (int)Math.Abs(endPos.X - tile.Position.X);
            int yDiff = (int)Math.Abs(endPos.Y - tile.Position.Y);
            return xDiff * 10 + yDiff * 10;
        }

        public static int CalculateG(Tile tile, Tile? parent)
        {
            int x = (int)Math.Abs(tile.Position.X - parent.Position.X);
            int y = (int)Math.Abs(tile.Position.Y - parent.Position.Y);

            if (x == 1 && y == 1)
                return parent.G + 14;
            else
                return parent.G + 10;
        }
    }
}
