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

    internal class Tile(Vector2 position, TileType type)
    {
        public Vector2 Position { get; private set; } = position;

        public TileType Type { get; set; } = type;

        public int H { get; set; } = 0;
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

        public void SetValues(Func<Vector2, Vector2, int, int> gEval, Func<Vector2, Vector2, int> hEval)
        {
            G = gEval(Position, Parent!.Position, Parent.G);
            H = hEval(Position, Program.EndPos);
        }
        
        public void SetValues(Func<Vector2, Vector2, int> hEval)
        {
            H = hEval(Position, Program.EndPos);
        }


        public void SetValues(Func<Vector2Int, Vector2Int, int, int> gEval)
        {
            G = gEval(Position, Parent!.Position, Parent.G);
        }

        public void Draw()
        {
            Color col = (Position.Equals(Program.EndPos)) ? Raylib_cs.Color.Gray : ColorMapper.GetColorByTileType(this.Type);
            col = OverrideColor != null ? (Color)OverrideColor : col;

            DrawRectangle((int)Position.X * Program.SQR_PIXEL_SIZE, (int)Position.Y * Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, Program.SQR_PIXEL_SIZE, col);
        }

        public void DebugDraw()
        {
            if (DebugAngle != null)
            {
                //DrawText($"{DebugAngle}", (int)(Position.X) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)(Position.Y) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, Program.FONT_SIZE, ColorMapper.TextColor);
                //DrawText($"{DebugAngle1}", (int)(Position.X) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)(Position.Y) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, Program.FONT_SIZE, ColorMapper.TextColor);
                //DrawText($"{DebugAngle2}", (int)(Position.X) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET, (int)(Position.Y) * Program.SQR_PIXEL_SIZE + Program.TEXT_OFFSET + 10, Program.FONT_SIZE, ColorMapper.TextColor);
            }

            if (DebugVector != null)
            {
                int thickness = 3;

                //DrawLineEx((Position + new Vector2Int(0.5f, 0.5f)) * (Program.SQR_PIXEL_SIZE), ((Position + new Vector2Int(0.5f, 0.5f) + (Vector2Int)DebugVector) * (Program.SQR_PIXEL_SIZE)), thickness, Color.LightGray);
                //DrawLineEx((Position + new Vector2Int(0.5f, 0.5f)) * (Program.SQR_PIXEL_SIZE), ((Position + new Vector2Int(0.5f, 0.5f) + (Vector2Int)DebugVector1!) * (Program.SQR_PIXEL_SIZE)), thickness, Color.Black);
                //DrawLineEx((Position + new Vector2Int(0.5f, 0.5f)) * (Program.SQR_PIXEL_SIZE), ((Position + new Vector2(0.5f, 0.5f) + (Vector2)DebugVector2!) * (Program.SQR_PIXEL_SIZE)), thickness, Color.Black);
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
