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
            return (int)(Math.Sqrt(xDiff * xDiff + yDiff * yDiff) * 15);
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

        public static bool RaycastBetween(Tile[,] tiles, Vector2 start, Vector2 end)
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            int tileSize = Program.SQR_PIXEL_SIZE;

            // Riktning och total längd
            Vector2 direction = end - start;
            float distanceToEnd = direction.Length();
            direction /= distanceToEnd; // normalisera

            // Tile-koordinater
            int tileX = (int)(start.X / tileSize);
            int tileY = (int)(start.Y / tileSize);

            int stepX = (direction.X > 0) ? 1 : -1;
            int stepY = (direction.Y > 0) ? 1 : -1;

            float deltaDistX = MathF.Abs(tileSize / direction.X);
            float deltaDistY = MathF.Abs(tileSize / direction.Y);

            float startOffsetX = (tileX + (direction.X > 0 ? 1 : 0)) * tileSize - start.X;
            float startOffsetY = (tileY + (direction.Y > 0 ? 1 : 0)) * tileSize - start.Y;

            float sideDistX = (direction.X == 0) ? float.MaxValue : MathF.Abs(startOffsetX / direction.X);
            float sideDistY = (direction.Y == 0) ? float.MaxValue : MathF.Abs(startOffsetY / direction.Y);

            float distance = 0;

            while (distance < distanceToEnd)
            {
                // Kontrollera bounds
                if (tileX < 0 || tileY < 0 || tileX >= width || tileY >= height)
                    return true;

                // Hinder?
                if (tiles[tileX, tileY].Type == TileType.Obstacle)
                    return true; // Strålen blockeras

                // Nästa tile
                if (sideDistX < sideDistY)
                {
                    tileX += stepX;
                    distance = sideDistX;
                    sideDistX += deltaDistX;
                }
                else
                {
                    tileY += stepY;
                    distance = sideDistY;
                    sideDistY += deltaDistY;
                }
            }

            return false; // Ingen vägg hittades mellan punkterna
        }
    }
}
