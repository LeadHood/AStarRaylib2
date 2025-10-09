using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AStarRaylib.Pathfinders
{
    class AStarBase : IPathFinder
    {
        public bool FirstIteration { get; private set; } = true;
        public bool FoundPath { get; set; } = false;

        public Tile? IterationForTile(Tile tile, Tile[,] tiles, Vector2 startPos, Vector2 endPos)
        {
            tile.Type = TileType.Closed;

            if (((int)tile.Position.X == (int)endPos.X) && ((int)tile.Position.Y == (int)endPos.Y))
            {
                return tile;
            }

            int x = (int)tile.Position.X;
            int y = (int)tile.Position.Y;

            //Looping around the 8 positions around the tile
            for (int j = y - 1; j < y + 2; j++)
            {
                for (int i = x - 1; i < x + 2; i++)
                {
                    //Can't be outside of bounds, therefore continue if the index is.
                    if (i < 0 || i >= tiles.GetLength(0) || j < 0 || j >= tiles.GetLength(1))
                    {
                        continue;
                    }

                    Tile curTile = tiles[i, j];

                    if((curTile.Type == TileType.Opened /*|| curTile.Type == TileType.Closed*/)&& (curTile.G > tile.G + curTile.GetG(tile)))
                    {
                        Console.WriteLine("Did the thing");
                        curTile.Parent = tile;
                        curTile.G = curTile.GetG(tile);
                        continue;
                    }

                    if (curTile.Type == TileType.Obstacle || curTile.Type == TileType.Closed || curTile.Type == TileType.Opened)
                    {
                        continue;
                    }

                    //Checking if it is a diagonal move and if it is a obstacle
                    if ((i < x || i > x) && (j < y || j > y))
                    {
                        if (j > y)
                        {
                            if (tiles[x, y + 1].Type == TileType.Obstacle) { continue; }
                        }
                        else
                        {
                            if (tiles[x, y - 1].Type == TileType.Obstacle) { continue; }
                        }
                        if (i > x)
                        {
                            if (tiles[x + 1, y].Type == TileType.Obstacle) { continue; }
                        }
                        else
                        {
                            if (tiles[x - 1, y].Type == TileType.Obstacle) { continue; }
                        }
                    }

                    //Setting it open
                    curTile.Type = TileType.Opened;
                    curTile.Parent = tile;
                    curTile.G = curTile.GetG(tile);
                    curTile.CalculateH(endPos);
                }
            }

            return null;
        }

        public Tile ChooseLowestF(Tile[,] tiles, Vector2 startPos)
        {
            if (FirstIteration == true)
            {
                FirstIteration = false;
                return tiles[(int)startPos.X, (int)startPos.Y];
            }

            //Using linq to find lowest F
            return tiles.Cast<Tile>().Where(tile => tile.Type == TileType.Opened).OrderBy(tile => tile.F).First();
        }

        public void ResetBrain()
        {
            FirstIteration = true;
            FoundPath = false;
        }
    }
}
