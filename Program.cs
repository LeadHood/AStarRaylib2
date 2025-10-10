using System;
using static Raylib_cs.Raylib;
using Raylib_cs;
using System.Numerics;
using System.Linq;

namespace AStarRaylib
{
    class Program
    {
        public static bool DebugMode = true;

        // 0 for instant pathfinding
        const int DEBUG_FRAMES = 0;
        static int FrameTimer = 0;

        const float DEBUG_LINE_SIZE = 4;

        public const int SQR_PIXEL_SIZE = 60;
        public const int TEXT_OFFSET = 2;
        public const int FONT_SIZE = 18;

        const int SCREEN_X = 20;
        const int SCREEN_Y = 15;

        static Vector2 StartPos = new Vector2(1, 1);
        static Vector2 EndPos = new Vector2(19, 10);

        static IPathFinder CurrentPathFinder = new Pathfinders.AStarBase();
        static List<Vector2> ThePath = new List<Vector2>();
        static List<Vector2> ObstaclePositions = new List<Vector2>();

        static Tile[,] Tiles = new Tile[SCREEN_X, SCREEN_Y];

        static void Main(string[] args)
        {
            Start();

            while (!WindowShouldClose())
            {
                Update();

                Draw();
            }
        }

        static void Start()
        {
            SetTraceLogLevel(TraceLogLevel.Error);

            InitWindow(SCREEN_X * SQR_PIXEL_SIZE, SCREEN_Y * SQR_PIXEL_SIZE, "ASTAR");
            SetTargetFPS(60);

            Reset();
        }

        static void Update()
        {
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                Vector2 mousePos = GetMousePosition();
                Vector2 mouseTilePos = new Vector2((int)mousePos.X/SQR_PIXEL_SIZE, (int)mousePos.Y / SQR_PIXEL_SIZE);

                if (!ObstaclePositions.Exists(vec => vec.X == (int)mouseTilePos.X && vec.Y == (int)mouseTilePos.Y))
                {
                    ObstaclePositions.Add(mouseTilePos);
                }
                else 
                {
                    ObstaclePositions.Remove(mouseTilePos);
                }

                Reset();
            }

            if(FrameTimer >= DEBUG_FRAMES)
            {
                FrameTimer = 0;

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
            BeginDrawing();
            ClearBackground(Color.White);

            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x  = 0; x < Tiles.GetLength(0); x++)
                {
                    Tiles[x, y].Draw();
                }
            }

            for (int y = 1; y <= SCREEN_Y; y++)
            {
                DrawLine(0, y * SQR_PIXEL_SIZE, SCREEN_X * SQR_PIXEL_SIZE, y * SQR_PIXEL_SIZE, Color.White);
            }

            for (int x = 1; x <= SCREEN_X; x++)
            {
                DrawLine(x * SQR_PIXEL_SIZE, 0 , x * SQR_PIXEL_SIZE, SCREEN_Y * SQR_PIXEL_SIZE, Color.White);
            }

            if (ThePath.Count > 0)
            {
                DrawPath(ThePath);
            }

            EndDrawing();
        }

        static void DrawPath(List<Vector2> positions)
        {
            for (int i = 0; i < positions.Count - 1; i++)
            {
                DrawLineEx(SQR_PIXEL_SIZE * new Vector2(positions[i].X + 0.5f, positions[i].Y + 0.5f), SQR_PIXEL_SIZE * new Vector2(positions[i + 1].X + 0.5f, positions[i + 1].Y + 0.5f), DEBUG_LINE_SIZE, ColorMapper.DebugLineColor);
            }
        }

        static void Reset()
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    if(ObstaclePositions.Exists(vec => (int)vec.X == x && (int)vec.Y == y))
                    {
                        Tiles[x, y] = new Tile(new Vector2(x, y), TileType.Obstacle);
                        continue;
                    }

                    Tiles[x, y] = new Tile(new Vector2(x, y), TileType.Unopened);
                }
            }

            FrameTimer = 0;
            ThePath = new List<Vector2>();
            CurrentPathFinder.ResetBrain();
        }
    }
}
