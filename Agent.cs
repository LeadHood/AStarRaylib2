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
        
        private int negativeSize = 5;

        public List<Vector2> Path { get; set; } = new List<Vector2>();

        public Vector2 StartPos { get; private set;}

        private int WalkIndex = 1;
        private float MoveSpeed = 10f;
        private float Timer = 0f;
        private float Rotation = 0f;
        private float RotationSpeed = 10f;

        private bool CanMove = true;

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

        public void FindPath(Vector2 endPos)
        {
            while (!Pathfinder.FoundPath && (Tiles.Cast<Tile>().Where(tile => tile.Type == TileType.Opened).Any() || Pathfinder.FirstIteration))
            {
                Tile? chosenTile = Pathfinder.ChooseLowestF(Tiles, StartPos);

                if (chosenTile == null)
                {
                    return;
                }

                Tile? endingTile = Pathfinder.IterationForTile(chosenTile, Tiles, StartPos, endPos);

                if (endingTile != null)
                {
                    Pathfinder.FoundPath = true;
                    Path = Pathfinder.EnhancePath(endingTile.GetPath(), Tiles);
                }
            }
        }

        public void Walk()
        {
            if (Path == null || Path.Count < 2 || !CanMove)
                return;

            Vector2 target = Path[WalkIndex];
            Vector2 dir = target - Position;
            float distanceToPostion = dir.Length();

            if (distanceToPostion > 0.001f)
            {
                //Normalizing
                dir /= dir.Length();

                float targetAngle = MathF.Atan2(dir.Y, dir.X) * 180f / MathF.PI;
                Rotation = LerpAngle(Rotation, targetAngle, Raylib.GetFrameTime() * RotationSpeed);

                // Flytta framåt
                float moveDist = MoveSpeed * Raylib.GetFrameTime();

                if (moveDist >= distanceToPostion)
                {
                    Position = target;
                    WalkIndex++;

                    if (WalkIndex >= Path.Count)
                    {
                        CanMove = false;
                        return;
                    }
                }
                else
                {
                    Position += dir * moveDist;
                }
            }
        }

        //Random lerping method i found
        private float LerpAngle(float a, float b, float t)
        {
            float diff = (b - a + 540f) % 360f - 180f;
            return a + diff * Math.Clamp(t, 0f, 1f);
        }

        public void Reset()
        {
            Timer = 0;
            WalkIndex = 1;
            CanMove = true;

            StartPos = Position;

            Pathfinder.ResetBrain();
            Path.Clear();            
            ResetTiles();
        }

        public void Draw()
        {
            float size = Program.SQR_PIXEL_SIZE - 2 * negativeSize;
            Vector2 pos = new Vector2((Position.X * Program.SQR_PIXEL_SIZE) + negativeSize, (Position.Y * Program.SQR_PIXEL_SIZE) + negativeSize);

            Rectangle rect = new Rectangle(pos.X + size / 2, pos.Y + size / 2, size, size);
            Vector2 origin = new Vector2(size / 2, size / 2);

            Raylib.DrawRectanglePro(rect, origin, Rotation, Color.DarkPurple);
        }
    }
}
