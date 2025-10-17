using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace AStarRaylib
{
    internal class Agent
    {
        public IPathFinder Pathfinder { get; private set; }
        public Vector2 Position { get; set; }

        public Agent(IPathFinder pathfinder, Vector2 startPos)
        {
            Pathfinder = pathfinder;
            Position = startPos;
        }

        public void Draw()
        {
            Raylib.DrawRectangle((int)(Position.X * Program.SQR_PIXEL_SIZE), (int)(Position.Y * Program.SQR_PIXEL_SIZE), Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, Color.DarkPurple);
        }
    }
}
