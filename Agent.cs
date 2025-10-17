using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace AStarRaylib
{
    internal class Agent
    {
        public Tile[,] Tiles { get; private set; } = new Tile[Program.SCREEN_X, Program.SCREEN_Y];
        public IPathFinder Pathfinder { get; private set; }
        public Vector2 Position { get; set; }

        public List<Vector2> Path { get; set; } = new List<Vector2>();

        public Vector2 StartPos { get; private set;}

        public Agent(IPathFinder pathfinder, Vector2 startPos)
        {
            Pathfinder = pathfinder;
            Position = startPos;
            StartPos = startPos;

            ResetTiles();
        }

        public void ResetTiles()
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    if (Program.ObstaclePositions.Exists(vec => (int)vec.X == x && (int)vec.Y == y))
                    {
                        Tiles[x, y] = new Tile(new Vector2(x, y), TileType.Obstacle);
                        continue;
                    }

                    Tiles[x, y] = new Tile(new Vector2(x, y), TileType.Unopened);
                }
            }
        }

        public void Reset()
        {
            Position = StartPos;
            Pathfinder.ResetBrain();
            Path.Clear();            
            ResetTiles();
        }

        public void Draw()
        {
            Raylib.DrawRectangle((int)(Position.X * Program.SQR_PIXEL_SIZE), (int)(Position.Y * Program.SQR_PIXEL_SIZE), Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, Color.DarkPurple);
        }
    }
}
