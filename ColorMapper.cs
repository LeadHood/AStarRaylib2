using System;
using Raylib_cs;

namespace AStarRaylib
{
    public static class ColorMapper
    {
        public static Color TextColor = Color.White;
        public static Color DebugLineColor = Color.Maroon;

        public static Color ObstacleColor = Color.White;
        public static Color UnopenedColor = Color.Black;
        public static Color OpenedColor = Color.Lime;
        public static Color ClosedColor = Color.Blue;

        public static Color[] ColorsForPaths = new Color[]
        {
            //Color.Red,
            Color.Orange,
            //Color.Yellow,
            //Color.Lime,
            //Color.Green,
            //Color.SkyBlue,
            //Color.Blue,
            //Color.Purple,
            //Color.Magenta,
            //Color.Brown
        };

        public static Color GetColorByTileType(TileType tileType)
        {
            Color color = new Color();

            switch (tileType)
            {
                case TileType.Obstacle:
                    color = ObstacleColor;
                    break;
                case TileType.Unopened:
                    color = UnopenedColor;
                    break;
                case TileType.Opened:
                    color = OpenedColor;
                    break;
                case TileType.Closed:
                    color = ClosedColor;
                    break;
                default:
                    throw new Exception($"This tiletype does not have a color: {tileType}");
            }

            return color;
        }
    }
}
