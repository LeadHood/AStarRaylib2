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

                    if(curTile.Type == TileType.Opened && (curTile.G > curTile.GetG(tile)))
                    {
                        curTile.Parent = tile;
                        curTile.G = curTile.GetG(tile);
                        continue;
                    }

                    if (curTile.Type == TileType.Obstacle || curTile.Type == TileType.Closed || curTile.Type == TileType.Opened)
                    {
                        continue;
                    }

                    //Checking if it is a diagonal move and if it is a obstacle
                    if (i != x && j != y)
                    { 
                        if (tiles[x, j].Type == TileType.Obstacle || tiles[i, y].Type == TileType.Obstacle)
                        {
                            continue;
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

        public Tile? ChooseLowestF(Tile[,] tiles, Vector2 startPos)
        {
            if (FirstIteration == true)
            {
                FirstIteration = false;
                return tiles[(int)startPos.X, (int)startPos.Y];
            }

            //Using linq to find lowest F
            var fTiles = tiles.Cast<Tile>().Where(tile => tile.Type == TileType.Opened).OrderBy(tile => tile.F);

            if(!fTiles.Any())
            {
                return null;
            }

            return fTiles.First();
        }

        public List<Vector2> EnhancePath(List<Vector2> path, Tile[,] tiles)
        {
            List<Vector2> newPath = [.. path];

            int index = 0;

            foreach (Vector2 pos in path)
            {
                if (index == 0 || index == path.Count - 1)
                {
                    goto End;
                }

                for (int j = (int)pos.Y - 1; j < (int)pos.Y + 2; j++)
                {
                    for (int i = (int)pos.X - 1; i < (int)pos.X + 2; i++)
                    {
                        if (i < 0 || i >= tiles.GetLength(0) || j < 0 || j >= tiles.GetLength(1))
                        {
                            continue;
                        }

                        if (i != pos.X && j != pos.X && tiles[i, j].Type == TileType.Obstacle)
                        {
                            goto End;
                        }
                    }
                }

                newPath.Remove(pos);

            End:
                index++;
                continue;
            }

            return newPath;
        }


        public void ResetBrain()
        {
            FirstIteration = true;
            FoundPath = false;
        }
    }
}
