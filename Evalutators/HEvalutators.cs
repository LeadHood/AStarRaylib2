using System.Numerics;

namespace AStarRaylib.Evalutators
{
    static class HEvalutators
    {
        public static Func<Vector2Int, Vector2Int, int> Manhattan()
        {
            return (pos, endPos) =>
            {
                int xDiff = (int)Math.Abs(endPos.X - pos.X);
                int yDiff = (int)Math.Abs(endPos.Y - pos.Y);
                return xDiff * 10 + yDiff * 10;
            };
        }

        public static Func<Vector2Int, Vector2Int, int> Pythagoras()
        {
            return (pos, endPos) =>
            {
                float xDiff = (int)Math.Abs(endPos.X - pos.X);
                float yDiff = (int)Math.Abs(endPos.Y - pos.Y);
                return (int)(Math.Sqrt(xDiff * xDiff + yDiff * yDiff) * 10);
            };
        }

        public static Func<Vector2Int, Vector2Int, int> MinMax()
        {
            return (pos, endPos) =>
            {
                int xDiff = (int)Math.Abs(endPos.X - pos.X);
                int yDiff = (int)Math.Abs(endPos.Y - pos.Y);
                return 4 * (xDiff + yDiff) + 6 * Math.Max(xDiff, yDiff);
            };
        }
    }
}
