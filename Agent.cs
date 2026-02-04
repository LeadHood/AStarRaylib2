using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace AStarRaylib
{
    internal class Agent
    {
        public Tile[,] Tiles { get; private set; } = new Tile[Program.SCREEN_X, Program.SCREEN_Y];
        public Vector2 Position { get; set; }
        
        private int NegativeSize = 5;
        public IPathFinder Pathfinder;

        public List<Vector2> Path { get; set; } = new List<Vector2>();

        public Tile StartTile { get; private set;}

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

            ResetTiles();
            StartTile = Tiles[(int)startPos.X, (int)startPos.Y];

        }

        public void ResetTiles()
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    Tiles[x, y] = new Tile(new Vector2(x, y), Program.ObstaclePositions.Contains(new Vector2(x, y)) ? TileType.Obstacle : TileType.Unopened);
                }
            }
        }

        public bool FindPath(Vector2 endTile)
        {
            if(Path.Count == 0)
            {
                Path = Pathfinder.FindPath(Tiles, StartTile, Tiles[(int)endTile.X, (int)endTile.Y]);
                return true;
            }

            return false;
        }

        //public void Walk()
        //{
        //    if (Path == null || Path.Count < 2 || !CanMove)
        //        return;

        //    Vector2 target = Path[WalkIndex];
        //    Vector2 dir = target - Position;
        //    float distanceToPostion = dir.Length();

        //    if (distanceToPostion > 0.001f)
        //    {
        //        //Normalizing
        //        dir /= dir.Length();

        //        float targetAngle = MathF.Atan2(dir.Y, dir.X) * 180f / MathF.PI;
        //        Rotation = LerpAngle(Rotation, targetAngle, Raylib.GetFrameTime() * RotationSpeed);

        //        // Flytta framåt
        //        float moveDist = MoveSpeed * Raylib.GetFrameTime();

        //        if (moveDist >= distanceToPostion)
        //        {
        //            Position = target;
        //            WalkIndex++;

        //            if (WalkIndex >= Path.Count)
        //            {
        //                CanMove = false;
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            Position += dir * moveDist;
        //        }
        //    }
        //}

        //Random lerping method i found
        private float LerpAngle(float a, float b, float t)
        {
            float diff = (b - a + 540f) % 360f - 180f;
            return a + diff * Math.Clamp(t, 0f, 1f);
        }

        public void Reset()
        {
            Path.Clear();            
            ResetTiles();
        }

        public void Draw()
        {
            float size = Program.SQR_PIXEL_SIZE - 2 * NegativeSize;
            Vector2 pos = new Vector2((Position.X * Program.SQR_PIXEL_SIZE) + NegativeSize, (Position.Y * Program.SQR_PIXEL_SIZE) + NegativeSize);

            Rectangle rect = new Rectangle(pos.X + size / 2, pos.Y + size / 2, size, size);
            Vector2 origin = new Vector2(size / 2, size / 2);

            Raylib.DrawRectanglePro(rect, origin, Rotation, Color.DarkPurple);
        }
    }
}
