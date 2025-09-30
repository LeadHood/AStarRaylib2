using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AStarRaylib.Pathfinders
{
    class AStarBase : IPathFinder
    {
        public Tile? IterationForTile(Tile tile, Tile[,] tiles, Vector2 StartPos, Vector2 endPos)
        {
            if ((int)tile.Position.X == (int)endPos.X && (int)tile.Position.Y == (int)endPos.Y)
            {
                tile.Type = TileType.Closed;
                return tile;
            }

            tile.Type = TileType.Closed;

            int x = (int)tile.Position.X;
            int y = (int)tile.Position.Y;

            //Looping around the 8 positions around the tile
            for (int j = y - 1; j < y + 2; j++)
            {
                for (int i = x - 1; i < x + 2; i++)
                {
                    if (i == x && j == y)
                    {
                        continue;
                    }

                    //Can't be outside of bounds, therefore continue if the index is.
                    if (i < 0 || i >= tiles.GetLength(0) || j < 0 || j >= tiles.GetLength(1))
                    {
                        continue;
                    }

                    Tile curTile = tiles[j, i];

                    if (curTile.Type != TileType.Unopened)
                    {
                        continue;
                    }

                    //Checking if it is a diagonal move and is valid
                    if ((i < x || i > x) && (j < y || j > y))
                    {
                        if (j > y)
                        {
                            if (tiles[y + 1, x].Type == TileType.Obstacle) { continue; }
                        }
                        else
                        {
                            if (tiles[y - 1, x].Type == TileType.Obstacle) { continue; }
                        }
                        if (i > x)
                        {
                            if (tiles[y, x + 1].Type == TileType.Obstacle) { continue; }
                        }
                        else
                        {
                            if (tiles[y, x - 1].Type == TileType.Obstacle) { continue; }
                        }
                    }

                    //Setting it open
                    curTile.Type = TileType.Opened;
                    curTile.CalculateValues(endPos);
                    curTile.Parent = tile;
                }
            }

            return null;
        }
    }
}
