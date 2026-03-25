using System;
using System.Numerics;
using Raylib_cs;

namespace AStarRaylib.Pathfinders
{
    class Greedy(Func<Vector2, Vector2, int> hEvalutator, string name) : IPathFinder
    {
        public string Name => name;
        public int SearchedTiles { get; private set; }

        public List<Vector2> FindPath(Tile[,] tiles, Tile start, Tile end)
        {
            List<Tile> openedTiles = new();

            Tile currentTile = start;

            while (currentTile != end)
            {
                currentTile!.Type = TileType.Closed;

                int x = (int)currentTile.Position.X;
                int y = (int)currentTile.Position.Y;

                foreach (var (dx, dy) in MiscMethods.NeighborOffsets)
                {
                    int i = x + dx;
                    int j = y + dy;

                    //Can't be outside of bounds, therefore continue if the index is.
                    if (i < 0 || i >= tiles.GetLength(0) || j < 0 || j >= tiles.GetLength(1))
                    {
                        continue;
                    }

                    Tile neighbourTile = tiles[i, j];

                    //Checking if it is a diagonal move and if it is a obstacle
                    if (i != x && j != y && (tiles[x, j].Type == TileType.Obstacle || tiles[i, y].Type == TileType.Obstacle) || neighbourTile.Type != TileType.Unopened)
                    {
                        continue;
                    }

                    //Setting it open
                    neighbourTile.Type = TileType.Opened;
                    neighbourTile.Parent = currentTile;
                    neighbourTile.SetValues(hEvalutator);

                    openedTiles.Add(neighbourTile);
                }

                if (openedTiles.Count == 0)
                {
                    return new List<Vector2>();
                }

                Tile lowestF = openedTiles.MinBy(t => t.F);
                currentTile = lowestF;
                openedTiles.Remove(lowestF);
            }

            List<Vector2> path = end.GetPath();

            foreach (Vector2 v in path)
            {
                tiles[(int)v.X, (int)v.Y].OverrideColor = Color.DarkBlue;
            }

            SearchedTiles = openedTiles.Count;

            return path;
        }
    }
}

