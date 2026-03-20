using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;

namespace AStarRaylib
{
    static class MiscMethods
    {
        public static float Angle(Vector2 from, Vector2 to)
        {
            float num = (float)Math.Sqrt(from.LengthSquared() * to.LengthSquared());
            if (num < float.Epsilon)
            {
                return 0f;
            }

            float num2 = Math.Clamp(Vector2.Dot(from, to) / num, -1f, 1f);
            return (float)Math.Acos(num2);
        }

        public static List<Tile> SupercoverLine(Tile[,] tiles, Vector2 start, Vector2 end)
        {
            int x0 = (int)MathF.Floor(start.X);
            int y0 = (int)MathF.Floor(start.Y);
            int x1 = (int)MathF.Floor(end.X);
            int y1 = (int)MathF.Floor(end.Y);

            var result = new HashSet<Tile>();

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            int x = x0;
            int y = y0;

            AddIfValid(result, tiles, x, y);

            if (dx >= dy)
            {
                int err = dx / 2;
                for (int i = 0; i < dx; i++)
                {
                    x += sx;
                    err -= dy;

                    if (err < 0)
                    {
                        // Vi korsar en horisontell grid-linje
                        AddIfValid(result, tiles, x - sx, y + sy);
                        y += sy;
                        err += dx;
                    }

                    AddIfValid(result, tiles, x, y);
                }
            }
            else
            {
                int err = dy / 2;
                for (int i = 0; i < dy; i++)
                {
                    y += sy;
                    err -= dx;

                    if (err < 0)
                    {
                        // Vi korsar en vertikal grid-linje
                        AddIfValid(result, tiles, x + sx, y - sy);
                        x += sx;
                        err += dy;
                    }

                    AddIfValid(result, tiles, x, y);
                }
            }

            return result.ToList();
        }

        private static void AddIfValid(HashSet<Tile> set, Tile[,] tiles, int x, int y)
        {
            if (x >= 0 && y >= 0 &&
                x < tiles.GetLength(0) &&
                y < tiles.GetLength(1))
            {
                set.Add(tiles[x, y]);
                //tiles[x, y].OverrideColor = Raylib_cs.Color.Yellow;
            }
        }



        public static readonly (int dx, int dy)[] NeighborOffsets =
        {
            (-1, -1), (0, -1), (1, -1),
            (-1,  0),          (1,  0),
            (-1,  1), (0,  1), (1,  1),
        };

        public static readonly (int dx, int dy)[] CornerOffsets =
{
            (-1, -1),          (1, -1),
          
            (-1,  1),          (1,  1),
        };

        //Test from Simon Malmqvist The Great, This is just to showcase my pathfinding.
        public static List<Vector2> GenerateMaze(int width, int height, int extraPaths = 10)
        {
            Tile[,] maze = new Tile[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    maze[x, y] = new Tile(new Vector2(x, y), TileType.Obstacle);

            var stack = new Stack<Point>();
            int startX = 1, startY = 1;
            maze[startX, startY].Type = TileType.Unopened;
            stack.Push(new Point(startX, startY));

            int[] dx = { 0, 0, 2, -2 };
            int[] dy = { 2, -2, 0, 0 };

            while (stack.Count > 0)
            {
                var point = stack.Pop();
                List<int> dirs = new List<int>() { 0, 1, 2, 3 };
                while (dirs.Count > 0)
                {
                    int i = Random.Shared.Next(dirs.Count);
                    int dir = dirs[i];
                    dirs.RemoveAt(i);

                    int nx = point.X + dx[dir];
                    int ny = point.Y + dy[dir];

                    if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1 && !(maze[nx, ny].Type == TileType.Unopened))
                    {
                        maze[nx, ny].Type = TileType.Unopened;
                        maze[point.X + dx[dir] / 2, point.Y + dy[dir] / 2].Type = TileType.Unopened;
                        stack.Push(new Point(nx, ny));
                    }
                }
            }

            for (int i = 0; i < extraPaths; i++)
            {
                int x = Random.Shared.Next(1, width - 1);
                int y = Random.Shared.Next(1, height - 1);
                maze[x, y].Type = TileType.Unopened;
            }

            List<Vector2> positions = maze.Cast<Tile>().Where(t => t.Type == TileType.Obstacle).Select(t => t.Position).ToList();

            return positions;
        }

        public static Vector2 PositionToTile(Vector2 pos)
        {
            return new Vector2((int)pos.X / Program.SQR_PIXEL_SIZE, (int)pos.Y / Program.SQR_PIXEL_SIZE);
        }

    }
}
