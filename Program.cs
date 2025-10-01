using System;
using Raylib_cs;
using System.Numerics;
using System.Linq;

namespace AStarRaylib
{
    class Program
    {
        public static bool DebugMode = true;

        // 0 for instant pathfinding
        const int DEBUG_FRAMES = 120;
        static int FrameTimer = 0;

        const float DEBUG_LINE_SIZE = 4;

        public const int SQR_PIXEL_SIZE = 60;
        public const int TEXT_OFFSET = 2;
        public const int FONT_SIZE = 18;

        const int SCREEN_X = 20;
        const int SCREEN_Y = 15;

        static Vector2 StartPos = new Vector2(1, 1);
        static Vector2 EndPos = new Vector2(7, 3);

        static IPathFinder CurrentPathFinder = new Pathfinders.AStarBase();
        static List<Vector2> ThePath = new List<Vector2>();

        static Tile[,] Tiles = new Tile[SCREEN_X, SCREEN_Y];

        static void Main(string[] args)
        {
            Start();

            while (!Raylib.WindowShouldClose())
            {
                Update();


                Draw();
            }
        }

        static void Start()
        {
            Raylib.SetTraceLogLevel(TraceLogLevel.Error);

            Raylib.InitWindow(SCREEN_X * SQR_PIXEL_SIZE, SCREEN_Y * SQR_PIXEL_SIZE, "ASTAR");
            Raylib.SetTargetFPS(60);

            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    Tiles[x, y] = new Tile(new Vector2(x, y), TileType.Unopened);
                }
            }

            Tiles[(int)StartPos.X, (int)StartPos.Y].Type = TileType.Start;
            Tiles[(int)EndPos.X, (int)EndPos.Y].Type = TileType.Start;
        }

        static void Update()
        {
            if(FrameTimer >= DEBUG_FRAMES)
            {
                FrameTimer = 0;

                Console.Write($"{CurrentPathFinder.FoundPath}");

                if (CurrentPathFinder.FoundPath)
                {
                    return;
                }

                Tile chosenTile = CurrentPathFinder.ChooseLowestF(Tiles, StartPos);
                Tile? endingTile = CurrentPathFinder.IterationForTile(chosenTile, Tiles, StartPos, EndPos);

                if(endingTile != null)
                {
                    CurrentPathFinder.FoundPath = true;
                    ThePath = endingTile.GetPath();
                }
            }

            FrameTimer++;
        }

        static void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x  = 0; x < Tiles.GetLength(0); x++)
                {
                    Tiles[x, y].Draw();
                }
            }

            if (ThePath.Count > 0)
            {
                DrawPath(ThePath);
            }

            Raylib.EndDrawing();
        }

        static void DrawPath(List<Vector2> positions)
        {
            for (int i = 0; i < positions.Count - 1; i++)
            {
                Raylib.DrawLineEx(positions[i], positions[i + 1], DEBUG_LINE_SIZE, ColorMapper.DebugLineColor);
            }
        }
    }
}
