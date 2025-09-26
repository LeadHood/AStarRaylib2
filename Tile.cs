using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AStarRaylib
{
    public enum TileType
    {
        Unopened, 
        Opened, 
        Closed, 
        Obstacle,
        Start,
        Goal
    }

    class Tile
    {
        public Vector2 Position { get; private set; }

        public TileType Type { get; set; }

        private int H = 0;

        public int G { get; private set; } = 0;
        public int F { get => G + H; }

        public Tile? Parent { get; set; }

        public Tile(Vector2 position, TileType type)
        {
            Position = position;
            Type = type;
        }

        public void CalculateValues(Vector2 endPos)
        {
            //G adds senders G + 14 if diagonal, 10 if straight
            if (Parent == null)
            {
                G = 0;
            }
            else
            {
                
                G = Parent.G + (int)Math.Abs(Vector2.Distance(Parent.Position, Position) * 10);
            }

            //Calculate H
            int xDiff = (int)Math.Abs(endPos.X - Position.X);
            int yDiff = (int)Math.Abs(endPos.Y - Position.Y);
            H = xDiff * 10 + yDiff * 10;
        }

        public void Draw()
        {
            Raylib.DrawRectangle((int)Position.X * Program.SQR_PIXEL_SIZE, (int)Position.Y * Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, ColorMapper.GetColorByTileType(this.Type));

            if (!Program.DebugMode || Type == TileType.Unopened)
            {
                return;
            }

            //G value
            Raylib.DrawText($"{G}", (int)Position.X * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)Position.Y * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, 15, ColorMapper.TextColor);

            //H value

            Raylib.DrawText($"{G}", (int)Position.X * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)Position.Y * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, 15, ColorMapper.TextColor);


            //F value
        }
    }
}
