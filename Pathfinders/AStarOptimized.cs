using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Diagnostics;

namespace AStarRaylib.Pathfinders
{
    class AStarOptimized : IPathFinder
    {
        public bool FirstIteration { get; private set; } = true;
        public bool FoundPath { get; set; } = false;
        public List<Tile> OpenedTiles { get ; set;} = new List<Tile>();

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

                    int newG = HelpMethods.CalculateG(curTile, tile);
                    if (curTile.Type == TileType.Opened && (curTile.G > newG))
                    {
                        Tile neighbour1 = tiles[i, (int)curTile.Position.Y];
                        Tile neighbour2 = tiles[(int)curTile.Position.X, j];

                        if (i != x && j != y && !(neighbour1.Type == TileType.Obstacle) && !(neighbour2.Type == TileType.Obstacle))
                        {
                            continue;
                        }

                        curTile.Parent = tile;
                        curTile.G = newG;
                        continue;
                    }

                    if (curTile.Type == TileType.Obstacle || curTile.Type == TileType.Closed || curTile.Type == TileType.Opened)
                    {
                        continue;
                    }

                    //Checking if it is a diagonal move and if it is a obstacle
                    if (i != x && j != y && (tiles[x, j].Type == TileType.Obstacle || tiles[i, y].Type == TileType.Obstacle))
                    { 
                        continue;
                    }

                    //Setting it open
                    curTile.Type = TileType.Opened;
                    curTile.Parent = tile;
                    curTile.SetValues();

                    OpenedTiles.Add(curTile);
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

            Tile? returnTile = OpenedTiles.MinBy(tile => tile.F);

            if (returnTile == null)
            {
                return null;
            }

            OpenedTiles.Remove(returnTile);

            return returnTile;
        }

        public List<Vector2> EnhancePath(List<Vector2> path, Tile[,] tiles)
        {
            List<Vector2> newPath = [.. path];

            int index = 0;

            foreach (Vector2 pos in path)
            {
                //First and last pos is static
                if (index == 0 || index == path.Count - 1)
                {
                    index++;
                    continue;
                }

                bool isCorner = false;

                for (int j = (int)pos.Y - 1; j < (int)pos.Y + 2; j++)
                {
                    for (int i = (int)pos.X - 1; i < (int)pos.X + 2; i++)
                    {
                        //if it Is outside then continue
                        if (i < 0 || i >= tiles.GetLength(0) || j < 0 || j >= tiles.GetLength(1))
                        {
                            continue;
                        }

                        //Diagnoally is obstacle, then End, it is a corner tile
                        if (i != pos.X && j != pos.Y && tiles[i, j].Type == TileType.Obstacle)
                        {
                            Tile neighbour1 = tiles[i, (int)pos.Y];
                            Tile neighbour2 = tiles[(int)pos.X, j];

                            if(!(neighbour1.Type == TileType.Obstacle) && !(neighbour2.Type == TileType.Obstacle))
                            { 
                                isCorner = true;
                                break;
                            }
                        }
                    }

                    if (isCorner)
                    {
                        break;
                    }
                }

                if (!isCorner)
                {
                    newPath.Remove(pos);
                }

                index++;

                continue;
            }

            return newPath;
        }


        public void ResetBrain()
        {
            FirstIteration = true;
            FoundPath = false;
            OpenedTiles.Clear();
        }
    }
}
