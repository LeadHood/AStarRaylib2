using System.Numerics;

namespace AStarRaylib.Evalutators
{
    static class GEvaluators
    {
        public static Func<Vector2, Vector2, int, int> Distance()
        {
            return (pos, parentPos, g) =>
            {
                int x = (int)Math.Abs(pos.X - parentPos.X);
                int y = (int)Math.Abs(pos.Y - parentPos.Y);

                if (x == 1 && y == 1)
                    return g + 14;
                else
                    return g + 10;
            };
        }

        public static Func<Vector2, Vector2, int, int> Straight()
        {
            return (pos, parentPos, g) =>
            {
                int x = (int)Math.Abs(pos.X - parentPos.X);
                int y = (int)Math.Abs(pos.Y - parentPos.Y);

                if (x == 1 && y == 1)
                    return g + 20;
                else
                    return g + 10;
            };
        }
    }
}
