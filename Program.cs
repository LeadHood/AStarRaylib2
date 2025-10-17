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

        const float DEBUG_LINE_SIZE = 4;

        public const int SQR_PIXEL_SIZE = 60/2;
        public const int TEXT_OFFSET = 2;
        public const int FONT_SIZE = 18;

        public const int SCREEN_X = 20 * 2;
        public const int SCREEN_Y = 15 * 2;

        static Vector2 StartPos = new Vector2(1, 9);
        public static Vector2 EndPos = new Vector2(39, 29);

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

            //Agents.Add(new Agent(new Pathfinders.AStarBase(), new Vector2(1, 7)));
            //Agents.Add(new Agent(new Pathfinders.AStarBase(), StartPos));
            //Agents.Add(new Agent(new Pathfinders.AStarBase(), new Vector2(4, 6)));

            for (int x = 0; x < SCREEN_X; x++)
            {
                    Agents.Add(new Agent(new Pathfinders.AStarBase(), new Vector2(x, 0)));
            }


            Reset();
        }

        static void Update()
        {
            //Input
            if (IsMouseButtonDown(MouseButton.Left))
            {
                Vector2 mousePos = GetMousePosition();
                Vector2 mouseTilePos = new Vector2((int)mousePos.X/SQR_PIXEL_SIZE, (int)mousePos.Y / SQR_PIXEL_SIZE);

                if (!ObstaclePositions.Exists(vec => vec.X == (int)mouseTilePos.X && vec.Y == (int)mouseTilePos.Y) && !Erasing)
                {
                    ObstaclePositions.Add(mouseTilePos);
                }
                else if(Erasing)
                {
                    ObstaclePositions.Remove(mouseTilePos);
                }

                Reset();
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

            //Instant pathfinding
            if(DebugFrames == 0)
            {
                RunAgents();
            }

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
        }

        static void RunAgents()
        {
            foreach (Agent agent in Agents)
            {
                if (agent.Pathfinder.FoundPath)
                {
                    continue;
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                while (!agent.Pathfinder.FoundPath && (agent.Tiles.Cast<Tile>().Where(tile => tile.Type == TileType.Opened).Any() || agent.Pathfinder.FirstIteration))
                {
                    IterationForAlgoritm(agent);
                }

                stopwatch.Stop();
                elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

                continue;
            }
        }

        static void IterationForAlgoritm(Agent agent)
        {
            Tile? chosenTile = agent.Pathfinder.ChooseLowestF(agent.Tiles, agent.StartPos);

            if (chosenTile == null)
            {
                return;
            }

            Tile? endingTile = agent.Pathfinder.IterationForTile(chosenTile, agent.Tiles, agent.StartPos, EndPos);

            if (endingTile != null)
            {
                agent.Pathfinder.FoundPath = true;
                agent.Path = endingTile.GetPath();
                agent.Path = agent.Pathfinder.EnhancePath(agent.Path, agent.Tiles);
            }
        }

        static void Draw()
        {
            BeginDrawing();
            ClearBackground(Color.White);

            DrawTiles(Agents[1]);

            DrawText("Tool: "  + (Erasing ? "Eraser" : "Brush"), 10, SQR_PIXEL_SIZE * SCREEN_Y - 24, 24, Color.White);
            DrawText("Elapsed time: " + elapsedMilliseconds + " ms", SQR_PIXEL_SIZE * SCREEN_X - 500, SQR_PIXEL_SIZE * SCREEN_Y - 24, 24, Color.White);

            DrawGrid();

            foreach (Agent agent in Agents)
            {
                DrawPath(agent.Path, Color.Red);
                //agent.Draw();
            }

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
