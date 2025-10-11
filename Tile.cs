using static Raylib_cs.Raylib;
using System.Numerics;

namespace AStarRaylib
{
    public enum TileType
    {
        Unopened, 
        Opened, 
        Closed, 
        Obstacle,
    }

    class Tile
    {
        public Vector2 Position { get; private set; }

        public TileType Type { get; set; }

        private int H = 0;

        public int G { get; set; } = 0;
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

        public void CalculateH(Vector2 endPos)
        {
            //Calculate H
            int xDiff = (int)Math.Abs(endPos.X - Position.X);
            int yDiff = (int)Math.Abs(endPos.Y - Position.Y);
            H = xDiff * 10 + yDiff * 10;
        }

        public int GetG(Tile parent)
        {
            int x = (int)Math.Abs(Position.X - parent.Position.X);
            int y = (int)Math.Abs(Position.Y - parent.Position.Y);

            if (x == 1 && y == 1)
                return parent.G + 14; 
            else
                return parent.G + 10; 
        }


        public void Draw()
        {
            //Debugcolor
            Raylib_cs.Color col = (Position.Equals(Program.EndPos)) ? Raylib_cs.Color.Gray : ColorMapper.GetColorByTileType(this.Type);

            DrawRectangle((int)Position.X * Program.SQR_PIXEL_SIZE, (int)Position.Y * Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, col);

            if (!Program.DebugMode || Type == TileType.Unopened)
            {
                return;
            }

            //G value
            DrawText($"{G}", (int)Position.X * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)Position.Y * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, Program.FONT_SIZE, ColorMapper.TextColor);

            //H value
            DrawText($"{H}", (int)Position.X * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)(Position.Y + 1) * Program.SQR_PIXEL_SIZE - Program.TEXT_OFFSET - (int)MeasureTextEx(GetFontDefault(), "10", Program.FONT_SIZE, 0).Y, Program.FONT_SIZE, ColorMapper.TextColor);

            //F value
            DrawText($"{F}", (int)(Position.X + 1) * Program.SQR_PIXEL_SIZE - 2 * Program.TEXT_OFFSET - (int)MeasureTextEx(GetFontDefault(), $"{F}", Program.FONT_SIZE, 0).X, (int)((Position.Y + 0.5f) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET - 0.5f * (int)MeasureTextEx(GetFontDefault(), "10", Program.FONT_SIZE, 0).Y), Program.FONT_SIZE, ColorMapper.TextColor);
        }
    }
}
