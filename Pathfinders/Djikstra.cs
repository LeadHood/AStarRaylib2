using System;
using System.Collections.Generic;
using System.Linq;
using Raylib_cs;
using System.Numerics;


namespace AStarRaylib.Pathfinders
{
    class Djikstra(Func<Vector2, Vector2, int, int> gEvaluator, string name) : IPathFinder
    {
        string IPathFinder.Name => name;

        public List<Vector2> FindPath(Tile[,] tiles, Tile start, Tile end)
        {
            List<Tile> OpenedTiles = new();
            Tile currentTile = start;

            while (currentTile != end)
            {
                int x = (int)currentTile.Position.X;
                int y = (int)currentTile.Position.Y;

                currentTile.Type = TileType.Closed;

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

                    int newG = gEvaluator(neighbourTile.Position, currentTile.Position, currentTile.G);
                    if (neighbourTile.Type == TileType.Opened && neighbourTile.G > newG)
                    {
                        Tile neighbour1 = tiles[i, y];
                        Tile neighbour2 = tiles[x, j];

                        if (i != x && j != y && !(neighbour1.Type == TileType.Obstacle) && !(neighbour2.Type == TileType.Obstacle))
                        {
                            continue;
                        }

                        neighbourTile.Parent = currentTile;
                        neighbourTile.G = newG;
                        continue;
                    }

                    //Checking if it is a diagonal move and if it is a obstacle
                    if (i != x && j != y && (tiles[x, j].Type == TileType.Obstacle || tiles[i, y].Type == TileType.Obstacle) || neighbourTile.Type != TileType.Unopened)
                    {
                        continue;
                    }

                    //Setting it open
                    neighbourTile.Type = TileType.Opened;
                    neighbourTile.Parent = currentTile;
                    neighbourTile.SetValues(gEvaluator);

                    OpenedTiles.Add(neighbourTile);
                }

                if (OpenedTiles.Count == 0)
                {
                    return new List<Vector2>();
                }

                Tile lowestG = OpenedTiles.MinBy(t => t.G);
                currentTile = lowestG;
                OpenedTiles.Remove(lowestG);
            }

            List<Vector2> path = end.GetPath();

            foreach (Vector2 v in path)
            {
                tiles[(int)v.X, (int)v.Y].OverrideColor = Color.DarkBlue;
            }

            return path;
        }
    }
}
