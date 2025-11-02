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
        public static int CalculateHManhattan(Tile tile, Vector2 endPos)
        {
            //Calculate H
            int xDiff = (int)Math.Abs(endPos.X - tile.Position.X);
            int yDiff = (int)Math.Abs(endPos.Y - tile.Position.Y);
            return xDiff * 10 + yDiff * 10;
        }

        public static int CalculateHPythagoras(Tile tile, Vector2 endPos)
        {
            //Calculate H
            float xDiff = (int)Math.Abs(endPos.X - tile.Position.X);
            float yDiff = (int)Math.Abs(endPos.Y - tile.Position.Y);
            return (int)(Math.Sqrt(xDiff * xDiff + yDiff * yDiff) * 10);
        }

        public static int CalculateHMinMax(Tile tile, Vector2 endPos)
        {
            //Calculate H
            int xDiff = (int)Math.Abs(endPos.X - tile.Position.X);
            int yDiff = (int)Math.Abs(endPos.Y - tile.Position.Y);
            return 4 *(xDiff + yDiff)+ 6 * Math.Max(xDiff, yDiff);
        }

        public static int CalculateG(Tile tile, Tile? parent)
        {
            int x = (int)Math.Abs(tile.Position.X - parent!.Position.X);
            int y = (int)Math.Abs(tile.Position.Y - parent.Position.Y);

            if (x == 1 && y == 1)
                return parent.G + 14;
            else
                return parent.G + 10;
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            float num = (float)Math.Sqrt(from.LengthSquared() * to.LengthSquared());
            if (num < float.Epsilon)
            {
                return 0f;
            }

            float num2 = Math.Clamp(Vector2.Dot(from, to) / num, -1f, 1f);
            return (float)Math.Acos(num2);
        }

        //Raycast but for raylib. Because unity has it built in, i did not write this myself, just modified it for raylib.
        public static List<Tile> SupercoverLine(Tile[,] tiles, Vector2 start, Vector2 end)
        {
            List<Tile> tilesHitByLine = new List<Tile>();

            int x0 = (int)Math.Floor(start.X);
            int y0 = (int)Math.Floor(start.Y);
            int x1 = (int)Math.Floor(end.X);
            int y1 = (int)Math.Floor(end.Y);

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            int err = dx - dy;
            int e2;

            int x = x0;
            int y = y0;

            tilesHitByLine.Add(tiles[x, y]);

            while (x != x1 || y != y1)
            {
                e2 = err;

                int xOld = x;
                int yOld = y;

                if (2 * e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }

                if (2 * e2 < dx)
                {
                    err += dx;
                    y += sy;
                }

                tilesHitByLine.Add(tiles[x, y]);

                if (x != xOld && y != yOld)
                {
                    tilesHitByLine.Add(tiles[x, yOld]);
                    tilesHitByLine.Add(tiles[xOld, y]);
                }
            }

            return tilesHitByLine;
        }

        public static readonly (int dx, int dy)[] NeighborOffsets =
        {
            (-1, -1), (0, -1), (1, -1),
            (-1,  0),          (1,  0),
            (-1,  1), (0,  1), (1,  1),
        };

        public static readonly (int dx, int dy)[] CornerOffsets =
{
            (-1, -1),          (1, -1),
          
            (-1,  1),          (1,  1),
        };
    }
}
