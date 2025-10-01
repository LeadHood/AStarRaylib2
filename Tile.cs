using Raylib_cs;
using static Raylib_cs.Raylib;
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

        //Returns the path from this tile if it is the ending tile
        public List<Vector2> GetPath()
        {
            Console.WriteLine("GETTED PATH ACTUALLY");

            Tile? tile = this;
            List<Vector2> positions = new List<Vector2>();

            while(tile != null)
            {
                positions.Add(tile.Position);
                tile = tile.Parent;
            }

            positions.Reverse();

            return positions;
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
            DrawRectangle((int)Position.X * Program.SQR_PIXEL_SIZE, (int)Position.Y * Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, ColorMapper.GetColorByTileType(this.Type));

            if (!Program.DebugMode || Type == TileType.Unopened)
            {
                return;
            }

            //G value
            DrawText($"{G}", (int)Position.X * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)Position.Y * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, Program.FONT_SIZE, ColorMapper.TextColor);

            //H value

            DrawText($"{H}", (int)Position.X * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)(Position.Y + 1) * Program.SQR_PIXEL_SIZE - Program.TEXT_OFFSET - (int)MeasureTextEx(GetFontDefault(), "10", Program.FONT_SIZE, 0).Y, Program.FONT_SIZE, ColorMapper.TextColor);
            //F value
        }
    }
}
