using System;
using static Raylib_cs.Raylib;
using Raylib_cs;
using System.Numerics;
using System.Linq;
using System.Diagnostics;

namespace AStarRaylib
{
    class Program
    {
        public static bool DebugMode = true;

        // 0 for instant pathfinding
        static int DebugFrames = 0;
        static int FrameTimer = 0;

        const float DEBUG_LINE_SIZE = /*4*/1;

        public const int SQR_PIXEL_SIZE = 60/2;
        public const int TEXT_OFFSET = 2;
        public const int FONT_SIZE = 18/2;

        public const int SCREEN_X = 20 * 2;
        public const int SCREEN_Y = 15 * 2;

        static Vector2 StartPos = new Vector2(1, 9);
        public static Vector2 EndPos = new Vector2(39, 15);

        static List<Agent> Agents = new List<Agent>();

        public static List<Vector2> ObstaclePositions { get; private set; } = new List<Vector2>();

        static bool Erasing = false;
        static double elapsedMilliseconds = 0;

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

            //Agents.Add(new Agent(new Pathfinders.AStarOptimized(), new Vector2(1, 7)));
            //Agents.Add(new Agent(new Pathfinders.AStarOptimized(), StartPos));
            //Agents.Add(new Agent(new Pathfinders.AStarOptimized(), new Vector2(4, 6)));

            for (int x = 0; x < 1/*SCREEN_Y*/; x++)
            {
                Agents.Add(new Agent(new Pathfinders.AStarOptimized(), new Vector2(0, x)));
                //Agents.Add(new Agent(new Pathfinders.AStarBase(), new Vector2(0, x)));
            }

            Reset();
        }

        static void Update()
        {
            InputUpdate();

            //Instant pathfinding
            if(DebugFrames == 0)
            {
                RunAgents();
            }

            #region Debugging pathfinding
            //Debugging pathfinding
            //if (FrameTimer >= DebugFrames)
            //{
            //    FrameTimer = 0;

            //    if (CurrentPathFinder.FoundPath)
            //    {
            //        return;
            //    }

            //    IterationForAlgoritm();
            //}

            //FrameTimer++;
            #endregion
        }

        static void InputUpdate()
        {
            //Input
            if (IsMouseButtonDown(MouseButton.Left))
            {
                Vector2 mousePos = GetMousePosition();
                Vector2 mouseTilePos = new Vector2((int)mousePos.X / SQR_PIXEL_SIZE, (int)mousePos.Y / SQR_PIXEL_SIZE);

                if (!Erasing && !ObstaclePositions.Exists(vec => vec.X == (int)mouseTilePos.X && vec.Y == (int)mouseTilePos.Y))
                {
                    ObstaclePositions.Add(mouseTilePos);
                    Reset();
                }
                else if (Erasing && ObstaclePositions.Exists(vec => vec.X == (int)mouseTilePos.X && vec.Y == (int)mouseTilePos.Y))
                {
                    ObstaclePositions.Remove(mouseTilePos);
                    Reset();
                }
            }

            if (IsKeyPressed(KeyboardKey.C))
            {
                Console.Clear();
            }

            if (IsKeyPressed(KeyboardKey.E))
            {
                Erasing = !Erasing;
            }

            if (IsKeyPressed(KeyboardKey.R))
            {
                ObstaclePositions.Clear();
                Reset();
            }
        }

        static void RunAgents()
        {
            Stopwatch stopwatch = new Stopwatch();


            Parallel.ForEach(Agents, agent =>
            {
                if (agent.Pathfinder.FoundPath)
                {
                    return;
                }

                stopwatch.Start();

                agent.FindPath(EndPos);
            });

            stopwatch.Stop();


            if(stopwatch.Elapsed.TotalMilliseconds > 0.01d)
            {
                elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
            }
        }

        static void Draw()
        {
            BeginDrawing();
            ClearBackground(Color.Black);

            foreach (var item in ObstaclePositions)
            {
                Tile tile = new Tile(item, TileType.Obstacle);
                tile.Draw();
            }

            DrawTiles(Agents[0]);

            DrawGrid();

            int index = 0;

            foreach (Agent agent in Agents)
            {
                index++;
                DrawPath(agent.Path, ColorMapper.ColorsForPaths[index%ColorMapper.ColorsForPaths.Length]);
            }

            DrawText("Tool: " + (Erasing ? "Eraser" : "Brush"), 10, SQR_PIXEL_SIZE * SCREEN_Y - 24, 24, Color.Gray);
            DrawText("Elapsed time: " + elapsedMilliseconds + " ms", SQR_PIXEL_SIZE * SCREEN_X - 500, SQR_PIXEL_SIZE * SCREEN_Y - 24, 24, Color.Gray);

            EndDrawing();
        }

        static void DrawGrid()
        {
            for (int y = 1; y <= SCREEN_Y; y++)
            {
                DrawLine(0, y * SQR_PIXEL_SIZE, SCREEN_X * SQR_PIXEL_SIZE, y * SQR_PIXEL_SIZE, Color.White);
            }

            for (int x = 1; x <= SCREEN_X; x++)
            {
                DrawLine(x * SQR_PIXEL_SIZE, 0, x * SQR_PIXEL_SIZE, SCREEN_Y * SQR_PIXEL_SIZE, Color.White);
            }
        }

        static void DrawTiles(Agent agent)
        {
            for (int y = 0; y < agent.Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < agent.Tiles.GetLength(0); x++)
                {
                    agent.Tiles[x, y].Draw();
                }
            }
        }

        static void DrawPath(List<Vector2> positions)
        {
            for (int i = 0; i < positions.Count - 1; i++)
            {
                DrawLineEx(SQR_PIXEL_SIZE * new Vector2(positions[i].X + 0.5f, positions[i].Y + 0.5f), SQR_PIXEL_SIZE * new Vector2(positions[i + 1].X + 0.5f, positions[i + 1].Y + 0.5f), DEBUG_LINE_SIZE, ColorMapper.DebugLineColor);
            }
        }

        static void DrawPath(List<Vector2> positions, Color color)
        {
            //Console.WriteLine(positions.Count);

            for (int i = 0; i < positions.Count - 1; i++)
            {
                DrawLineEx(SQR_PIXEL_SIZE * new Vector2(positions[i].X + 0.5f, positions[i].Y + 0.5f), SQR_PIXEL_SIZE * new Vector2(positions[i + 1].X + 0.5f, positions[i + 1].Y + 0.5f), DEBUG_LINE_SIZE, color);
            }
        }

        static void Reset()
        {
            FrameTimer = 0;
            
            foreach(Agent agent in Agents)
            {
                agent.Reset();
            }
        }
    }
}
