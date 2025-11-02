using System;
using static Raylib_cs.Raylib;
using Raylib_cs;
using System.Numerics;
using System.Linq;
using System.Diagnostics;

namespace AStarRaylib
{
    enum MouseFunction
    {
        Obstacles,
        GoalPos,
    }

    class Program
    {
        //Variables to mess around with:
        const float DEBUG_LINE_SIZE = /*4*/2;

        public const int SQR_PIXEL_SIZE = 30;
        public const int TEXT_OFFSET = 2;
        public const int FONT_SIZE = 9;

        public const int SCREEN_X = 40;
        public const int SCREEN_Y = 30;

        const int AGENTS_AMOUNT = 10;

        //Endpos in the beginning, it can be changed during runtime
        public static Vector2 EndPos { get; private set;} = new Vector2(39, 15);

        //True: Draws which tiles the first agent looked at
        static bool DrawFirstAgentGrid = true;
        static bool GenerateMaze = false;
        static bool AgentsWalking = false;

        //This is for debugging how the algoritm searches, should not be changed.
        static int DebugFrames = 0;
        static int FrameTimer = 0;
        public static bool DisplayRayCastDebug { get; private set;} = false;

        static List<Agent> Agents = new List<Agent>();

        public static List<Vector2> ObstaclePositions { get; private set; } = new List<Vector2>();

        static double ElapsedMilliseconds = 0;

        static MouseFunction MouseMode;

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

            InitWindow(SCREEN_X * SQR_PIXEL_SIZE, (SCREEN_Y + 1) * SQR_PIXEL_SIZE, "ASTAR");
            SetTargetFPS(60);

            //Agents.Add(new Agent(new Pathfinders.AStarOptimized(), new Vector2(1, 7)));
            //Agents.Add(new Agent(new Pathfinders.AStarOptimized(), StartPos));
            //Agents.Add(new Agent(new Pathfinders.AStarOptimized(), new Vector2(4, 6)));

            //Change and add agents here to get many different agents running at the same time
            for (int y = 0; y < Math.Clamp(AGENTS_AMOUNT, 1, SCREEN_Y); y++)
            {
                Agents.Add(new Agent(new Pathfinders.AStarOptimized(), new Vector2(0, y)));
                //Agents.Add(new Agent(new Pathfinders.AStarBase(), new Vector2(0, y)));
            }

            if(GenerateMaze)
            {
                ObstaclePositions = HelpMethods.GenerateMaze(SCREEN_X, SCREEN_Y);
            }

            Reset();
        }

        static void Update()
        {
            InputUpdate();

            //Instant pathfinding, which should be used in final release.
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
            Vector2 mousePos = GetMousePosition();
            Vector2 mouseTilePos = HelpMethods.PositionToTile(mousePos);

            //Input
            if (IsMouseButtonDown(MouseButton.Left))
            {
                switch (MouseMode)
                {
                    case MouseFunction.Obstacles:
                        if(!ObstaclePositions.Exists(vec => vec.X == (int)mouseTilePos.X && vec.Y == (int)mouseTilePos.Y))
                        {
                            ObstaclePositions.Add(mouseTilePos);
                            Reset();
                        }
                        break;
                    case MouseFunction.GoalPos:
                        if(!ObstaclePositions.Contains(mouseTilePos))
                        {
                            EndPos = mouseTilePos;
                            Reset();
                        }
                        break;
                }
            }

            if (IsMouseButtonDown(MouseButton.Right))
            {
                switch (MouseMode)
                {
                    case MouseFunction.Obstacles:
                        if (ObstaclePositions.Exists(vec => vec.X == (int)mouseTilePos.X && vec.Y == (int)mouseTilePos.Y))
                        {
                            ObstaclePositions.Remove(mouseTilePos);
                            Reset();
                        }
                        break;
                }
            }

            if (IsKeyPressed(KeyboardKey.C))
            {
                Console.Clear();
            }

            if (IsKeyPressed(KeyboardKey.E))
            {
                int MouseFunctionsAmount = Enum.GetNames(typeof(MouseFunction)).Length;
                MouseMode = (MouseFunction)(((int)MouseMode + 1)%MouseFunctionsAmount);
            }

            if (IsKeyPressed(KeyboardKey.R))
            {
                ObstaclePositions.Clear();
                Reset();
            }

            if (IsKeyPressed(KeyboardKey.P))
            {
                AgentsWalking ^= true; 
            }
        }

        static void RunAgents()
        {
            Stopwatch stopwatch = new Stopwatch();

            //If many agents, running them parellely increases FPS.
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

            if (stopwatch.Elapsed.TotalMilliseconds > 0.01d)
            {
                ElapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
            }

            if (!AgentsWalking)
            {
                return;
            }

            foreach (var agent in Agents)
            {
                agent.Walk();
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

            if (DrawFirstAgentGrid)
            {
                DrawTiles(Agents[0]);
            }

            DrawGrid();

            int index = 0;

            foreach (Agent agent in Agents)
            {
                index++;
                //if(agent.Path.Count != agent.Path.Distinct().Count())
                //{
                //    //Console.WriteLine("BRUH MOMENt");
                //}
                DrawPath(agent.Path, ColorMapper.ColorsForPaths[index%ColorMapper.ColorsForPaths.Length]);
                agent.Draw();
            }

            DrawText("MouseMode: " + (MouseMode), 10, SQR_PIXEL_SIZE * (SCREEN_Y + 1) - 24, 24, Color.White);
            DrawText(AgentsWalking ? "Playing" : "Paused", 350, SQR_PIXEL_SIZE * (SCREEN_Y + 1) - 24, 24, Color.White);

            DrawText("Elapsed time: " + ElapsedMilliseconds + " ms", SQR_PIXEL_SIZE * SCREEN_X - 500, SQR_PIXEL_SIZE * (SCREEN_Y + 1) - 24, 24, Color.White);

            DrawDebugTiles(Agents[0]);

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

        static void DrawDebugTiles(Agent agent)
        {
            for (int y = 0; y < agent.Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < agent.Tiles.GetLength(0); x++)
                {
                    agent.Tiles[x, y].DebugDraw();
                }
            }
        }

        static void DrawPath(List<Vector2> positions, Color color)
        {
            for (int i = 0; i < positions.Count - 1; i++)
            {
                DrawLineEx(SQR_PIXEL_SIZE * new Vector2(positions[i].X + 0.5f, positions[i].Y + 0.5f), SQR_PIXEL_SIZE * new Vector2(positions[i + 1].X + 0.5f, positions[i + 1].Y + 0.5f), DEBUG_LINE_SIZE, color);
            }
        }

        static void Reset()
        {   
            foreach(Agent agent in Agents)
            {
                agent.Reset();
            }
        }
    }
}
