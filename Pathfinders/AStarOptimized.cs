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

            foreach (var (dx, dy) in HelpMethods.NeighborOffsets)
            {
                int i = x + dx;
                int j = y + dy;

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
            List<Vector2> onlyCorners = new List<Vector2> { path[0] };

            for (int i = 1; i < path.Count - 1; i++)
            {
                Vector2 currPos = path[i];

                //bool hitRaycast = HelpMethods.RaycastBetween(tiles, path[index - 1], path[index + 1]);

                if (IsCorner(currPos, tiles))
                {
                    onlyCorners.Add(currPos);
                    tiles[(int)currPos.X, (int)currPos.Y].OverrideColor = Raylib_cs.Color.DarkPurple;
                }

                continue;
            }

            //Adding last elemebnt of path to onlycorners to include both start and endpos
            onlyCorners.Add(path[^1]);

            List<Vector2> newPath = [..onlyCorners];

            //Finding the false corners
            for (int i = 1; i < onlyCorners.Count - 1; i++)
            {
                Vector2 currPos = onlyCorners[i];
                Vector2 nextPos = onlyCorners[i + 1];
                Vector2 prevPos = onlyCorners[newPath.FindIndex(v => v.Equals(currPos)) - 1];

                if (IsFalseCorner(currPos, prevPos, nextPos, tiles))
                {
                    newPath.Remove(onlyCorners[i]);
                }
            }

            return newPath;
        }

        
        private bool IsCorner(Vector2 pos, Tile[,] tiles)
        {
            foreach(var (dx, dy) in HelpMethods.CornerOffsets)
            {
                int i = (int)pos.X + dx;
                int j = (int)pos.Y + dy;

                //if it Is outside then continue
                if (i < 0 || i >= tiles.GetLength(0) || j < 0 || j >= tiles.GetLength(1))
                {
                    continue;
                }

                Tile neighbour1 = tiles[i, (int)pos.Y];
                Tile neighbour2 = tiles[(int)pos.X, j];

                if (i != pos.X && j != pos.Y && tiles[i, j].Type == TileType.Obstacle)
                {
                    if (!(neighbour1.Type == TileType.Obstacle) && !(neighbour2.Type == TileType.Obstacle))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsFalseCorner(Vector2 pos, Vector2 prevTile, Vector2 nextTile, Tile[,] tiles)
        {
            foreach (var (dx, dy) in HelpMethods.CornerOffsets)
            {
                int i = (int)pos.X + dx;
                int j = (int)pos.Y + dy;

                //if it Is outside then continue
                if (i < 0 || i >= tiles.GetLength(0) || j < 0 || j >= tiles.GetLength(1))
                {
                    continue;
                }

                if (tiles[i, j].Type == TileType.Obstacle)
                {
                    Vector2 vecToPrev = Vector2.Normalize(prevTile - pos);
                    Vector2 vecToObstacle = Vector2.Normalize(new Vector2(i, j) - pos);
                    Vector2 vecToNext = Vector2.Normalize(nextTile - pos);

                    //Angle math mathing
                    double angle1 = HelpMethods.Angle(vecToPrev, vecToObstacle);
                    double angle2 = HelpMethods.Angle(vecToNext, vecToObstacle);

                    double angle = angle1 + angle2;

                    //Console.WriteLine(angle1 + ", " + angle2 + ", " + angle + ", " + pos);
                    //Console.WriteLine($"{angle1, -20} {angle2, -20} {angle, -20} {pos, -20}");

                    //tiles[(int)pos.X, (int)pos.Y].OverrideColor = Raylib_cs.Color.Yellow;

                    if (angle <= Math.PI)
                    {
                        return false;
                    }
                }
            }

            tiles[(int)pos.X, (int)pos.Y].OverrideColor = Raylib_cs.Color.Beige;

            return true;
        }


        public void ResetBrain()
        {
            FirstIteration = true;
            FoundPath = false;
            OpenedTiles.Clear();
        }


    }
}
