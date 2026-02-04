using Raylib_cs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using static Raylib_cs.Raylib;

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
        const float DEBUG_LINE_SIZE = 2;

        public const int SQR_PIXEL_SIZE = 30;
        public const int TEXT_OFFSET = 2;
        public const int FONT_SIZE = 9;

        public const int SCREEN_X = 40;
        public const int SCREEN_Y = 30;

        public const int WINDOW_SIZE_X = SCREEN_X * SQR_PIXEL_SIZE + 250;
        public const int WINDOW_SIZE_Y = (SCREEN_Y + 1) * SQR_PIXEL_SIZE;

        const int AGENTS_AMOUNT = 1;

        //Endpos in the beginning, it can be changed during runtime
        public static Vector2 EndPos { get; private set;} = new Vector2(39, 15);

        static double ElapsedMilliseconds = 0;

        //True: Draws which tiles the first agent looked at
        static bool DrawFirstAgentGrid = true;
        static bool GenerateMaze = false;
        static bool DrawAgents = false;
        public static bool DisplayRayCastDebug { get; private set; } = false;

        static List<Agent> Agents = new List<Agent>();
        public static List<Vector2> ObstaclePositions { get; private set; } = new List<Vector2>();

        static List<IPathFinder> Pathfinders = new List<IPathFinder>();
        static int indexOfPathfinder = 1;

        static MouseFunction MouseMode;

        static void Main(string[] args)
        {
            var pathfinderType = typeof(IPathFinder);

            foreach (var type in Assembly.GetAssembly(pathfinderType)!
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    pathfinderType.IsAssignableFrom(t)))
            {
                Pathfinders.Add((IPathFinder)Activator.CreateInstance(type)!);
            }

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

            InitWindow(WINDOW_SIZE_X, WINDOW_SIZE_Y, "A-STAR");
            SetTargetFPS(60);

            //Change and add agents here to get many different agents running at the same time
            for (int y = 0; y < Math.Clamp(AGENTS_AMOUNT, 1, SCREEN_Y); y++)
            {
                Agents.Add(new Agent(Pathfinders[indexOfPathfinder - 1], new Vector2(0, y)));
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
            RunAgents();
        }

        static void InputUpdate()
        {
            Vector2 mousePos = GetMousePosition();
            Vector2 mouseTilePos = HelpMethods.PositionToTile(mousePos);

            //Input
            if (IsMouseButtonDown(MouseButton.Left) && mouseTilePos.X >= 0 && mouseTilePos.X < SCREEN_X && mouseTilePos.Y >= 0 && mouseTilePos.Y < SCREEN_Y)
            {
                switch (MouseMode)
                {
                    case MouseFunction.Obstacles:
                        if(!ObstaclePositions.Contains(mouseTilePos))
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
                        if (ObstaclePositions.Contains(mouseTilePos))
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

            for (int i = 1; i <= 9; i++)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Zero + i))
                {
                    if (i <= Pathfinders.Count)
                    {
                        IPathFinder pathfinder = Pathfinders[i - 1];
                        indexOfPathfinder = i;
                        
                        foreach(Agent a in Agents)
                        {
                            a.Reset();
                            a.Pathfinder = pathfinder;
                        }
                    }
                }
            }

        }

        static void RunAgents()
        {
            Stopwatch stopwatch = new Stopwatch();

            //If many agents, running them parellely increases FPS.
            Parallel.ForEach(Agents, agent =>
            {
                bool foundPath = agent.FindPath(EndPos);

                if (!foundPath)
                {
                    return;
                }

                stopwatch.Start();
            });

            stopwatch.Stop();

            if (stopwatch.Elapsed.TotalMilliseconds > 0.01d)
            {
                ElapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
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
                DrawPath(agent.Path, ColorMapper.ColorsForPaths[index%ColorMapper.ColorsForPaths.Length]);

                if (DrawAgents)
                {
                    agent.Draw();
                }
            }

            DrawText("MouseMode: " + (MouseMode), 10, SQR_PIXEL_SIZE * (SCREEN_Y + 1) - 24, 24, Color.White);

            DrawText("Elapsed time: " + ElapsedMilliseconds + " ms", SQR_PIXEL_SIZE * SCREEN_X - 500, SQR_PIXEL_SIZE * (SCREEN_Y + 1) - 24, 24, Color.White);

            for (int i = 1; i <= Pathfinders.Count; i++)
            {
                string text = i + " : " + Pathfinders[i - 1].Name;

                if (i == indexOfPathfinder)
                {
                    DrawRectangle(WINDOW_SIZE_X - 235, i * 30, MeasureText(text, 24) + 10, 28, Color.White);
                }

                DrawText(i + " : " + Pathfinders[i - 1].Name, WINDOW_SIZE_X - 230, i * 30, 24, i == indexOfPathfinder ? Color.Black : Color.White);
            }

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
