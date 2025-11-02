using static Raylib_cs.Raylib;
using System.Numerics;
using Raylib_cs;

namespace AStarRaylib
{
    public enum TileType
    {
        Unopened, 
        Opened, 
        Closed, 
        Obstacle,
    }

    internal class Tile
    {
        public Vector2 Position { get; private set; }

        public TileType Type { get; set; }

        private int H = 0;

        public int G { get; set; } = 0;
        public int F { get => G + H; }

        public Tile? Parent { get; set; }
        
        //Debug Variables
        public Color? OverrideColor { get; set; } = null;

        public float? DebugAngle { get; set; } = null;
        public float? DebugAngle1 { get; set; } = null;
        public float? DebugAngle2 { get; set; } = null;

        public Vector2? DebugVector { get; set; } = null;
        public Vector2? DebugVector1 { get; set; } = null;
        public Vector2? DebugVector2 { get; set; } = null;


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

        public void SetValues()
        {
            G = HelpMethods.CalculateG(this, Parent);//HelpMethods.CalculateG(this, Parent);
            //H = HelpMethods.CalculateHPythagoras(this, Program.EndPos);
            H = HelpMethods.CalculateHManhattan(this, Program.EndPos);
        }

        public void Draw()
        {
            //Debugcolor
            Color col = (Position.Equals(Program.EndPos)) ? Raylib_cs.Color.Gray : ColorMapper.GetColorByTileType(this.Type);
            col = OverrideColor != null ? (Color)OverrideColor : col;

            DrawRectangle((int)Position.X * Program.SQR_PIXEL_SIZE, (int)Position.Y * Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, col);

            if (!Program.DebugMode || Type == TileType.Unopened)
            {
                return;
            }

        }

        public void DebugDraw()
        {
            if (DebugAngle != null)
            {
                //DrawText($"{DebugAngle}", (int)(Position.X) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)(Position.Y) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, Program.FONT_SIZE, ColorMapper.TextColor);
                DrawText($"{DebugAngle1}", (int)(Position.X) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)(Position.Y) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, Program.FONT_SIZE, ColorMapper.TextColor);
                DrawText($"{DebugAngle2}", (int)(Position.X) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)(Position.Y) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET + 10, Program.FONT_SIZE, ColorMapper.TextColor);
            }

            if (DebugVector != null)
            {
                int thickness = 3;

                //DrawLineEx((Position + new Vector2(0.5f, 0.5f)) * (Program.SQR_PIXEL_SIZE), ((Position + new Vector2(0.5f, 0.5f) + (Vector2)DebugVector) * (Program.SQR_PIXEL_SIZE)), thickness, Color.LightGray);
                //DrawLineEx((Position + new Vector2(0.5f, 0.5f)) * (Program.SQR_PIXEL_SIZE), ((Position + new Vector2(0.5f, 0.5f) + (Vector2)DebugVector1!) * (Program.SQR_PIXEL_SIZE)), thickness, Color.Black);
                //DrawLineEx((Position + new Vector2(0.5f, 0.5f)) * (Program.SQR_PIXEL_SIZE), ((Position + new Vector2(0.5f, 0.5f) + (Vector2)DebugVector2!) * (Program.SQR_PIXEL_SIZE)), thickness, Color.Black);
            }

            ////G value
            //DrawText($"{G}", (int)Position.X * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)Position.Y * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, Program.FONT_SIZE, ColorMapper.TextColor);

            ////H value
            //DrawText($"{H}", (int)Position.X * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)(Position.Y + 1) * Program.SQR_PIXEL_SIZE - Program.TEXT_OFFSET - (int)MeasureTextEx(GetFontDefault(), "10", Program.FONT_SIZE, 0).Y, Program.FONT_SIZE, ColorMapper.TextColor);

            ////F value
            //DrawText($"{F}", (int)(Position.X + 1) * Program.SQR_PIXEL_SIZE - 2 * Program.TEXT_OFFSET - (int)MeasureTextEx(GetFontDefault(), $"{F}", Program.FONT_SIZE, 0).X, (int)((Position.Y + 0.5f) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET - 0.5f * (int)MeasureTextEx(GetFontDefault(), "10", Program.FONT_SIZE, 0).Y), Program.FONT_SIZE, ColorMapper.TextColor);
        }
    }
}
