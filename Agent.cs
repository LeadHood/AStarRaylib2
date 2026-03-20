using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace AStarRaylib
{
    internal class Agent
    {
        public Vector2Int Position { get; set; }
        public List<Vector2Int> Path { get; set; } = new List<Vector2Int>();

        public Tile StartTile { get; private set;}
        public Tile[,] Tiles { get; private set; } = new Tile[Program.SCREEN_X, Program.SCREEN_Y];

        public IPathFinder Pathfinder;

        public Agent(IPathFinder pathfinder, Vector2Int startPos)
        {
            Pathfinder = pathfinder;
            Position = startPos;

            ResetTiles();
            StartTile = Tiles[(int)startPos.X, (int)startPos.Y];
        }

        public void ResetTiles()
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    Tiles[x, y] = new Tile(new Vector2Int(x, y), Program.ObstaclePositions.Contains(new Vector2Int(x, y)) ? TileType.Obstacle : TileType.Unopened);
                }
            }
        }

        public bool FindPath(Vector2Int endTile)
        {
            if(Path.Count == 0)
            {
                Path = Pathfinder.FindPath(Tiles, StartTile, Tiles[(int)endTile.X, (int)endTile.Y]);
                return true;
            }

            return false;
        }

        public void Reset()
        {
            Path.Clear();            
            ResetTiles();
        }
    }
}
