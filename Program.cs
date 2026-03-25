using AStarRaylib.Evalutators;
using Raylib_cs;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json;
using static Raylib_cs.Raylib;

namespace AStarRaylib
{
    enum MouseFunction
    {
        Obstacles,
        GoalPos,
    }

    partial class Program
    {
        const float DEBUG_LINE_SIZE = 2;

        public const int SQR_PIXEL_SIZE = 25;
        public const int TEXT_OFFSET = 2;
        public const int FONT_SIZE = 9;

        public const int SCREEN_X = 40;
        public const int SCREEN_Y = 30;

        public const int WINDOW_SIZE_X = SCREEN_X * SQR_PIXEL_SIZE + 250;
        public const int WINDOW_SIZE_Y = (SCREEN_Y + 1) * SQR_PIXEL_SIZE;

        static double ElapsedMilliseconds = 0;
        static int pathfinderIndex = 0;

        private readonly static bool DrawAgentGrid = true;
        private readonly static bool GenerateMaze = false;

        static List<Agent> Agents = [];
        static Vector2 StartPosition = new(1, 15);
        //static Vector2 StartPosition = new (15, 15);
        public static Vector2 EndPos { get; private set; } = new(39, 15);

        public static List<Vector2> ObstaclePositions { get; private set; } = []; 
        public static bool DisplayRayCastDebug { get; private set; } = false;

        static readonly Func<Vector2, Vector2, int> hEvaluator = HEvalutators.Manhattan();
        static readonly Func<Vector2, Vector2, int, int> gEvaluator = GEvaluators.Distance();
        
        static readonly string savePath = "../../../Data/Maps/closedMap.json";
        static readonly string loadPath = "../../../Data/Maps/.json";

        static readonly string dataSavePath = "../../../Data/Results/data.txt";

        static List<IPathFinder> Pathfinders =
        [
            new Pathfinders.AStar(gEvaluator, hEvaluator, "A-Star"),
            new Pathfinders.Dijkstra(gEvaluator, "Dijkstra"),
            new Pathfinders.Greedy(hEvaluator, "Greedy")
        ];

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
           
            InitWindow(WINDOW_SIZE_X, WINDOW_SIZE_Y, "Pathfinding Test");
            SetTargetFPS(300);
           
            for (int i = 0; i < Pathfinders.Count; i++)
            {
                Agents.Add(new Agent(Pathfinders[i], StartPosition));
            }

            if(GenerateMaze)
            {
                ObstaclePositions = MiscMethods.GenerateMaze(SCREEN_X, SCREEN_Y);
            }

            Reset();
        }

        static void Update()
        {
            InputUpdate();
            RunAgent();
        }

        static void InputUpdate()
        {
            Vector2 mousePos = GetMousePosition();
            Vector2 mouseTilePos = MiscMethods.PositionToTile(mousePos);

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
                if(MouseMode == MouseFunction.Obstacles && ObstaclePositions.Contains(mouseTilePos))
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
                int MouseFunctionsAmount = Enum.GetNames(typeof(MouseFunction)).Length;
                MouseMode = (MouseFunction)(((int)MouseMode + 1)%MouseFunctionsAmount);
            }

            if (IsKeyPressed(KeyboardKey.R))
            {
                ObstaclePositions.Clear();
                Reset();
            }

            if (IsKeyPressed(KeyboardKey.Enter))
            {
                RunTests();
            }

            if (IsKeyPressed(KeyboardKey.S))
            {
                SaveObstacles(ObstaclePositions, savePath);
                Reset();
            }

            if (IsKeyPressed(KeyboardKey.L))
            {
                ObstaclePositions = LoadObstacles(loadPath);
                Reset();
            }

            for (int i = 1; i <= 9; i++)
            {
                if (!IsKeyPressed(KeyboardKey.Zero + i))
                {
                    continue;
                }

                if (i <= Pathfinders.Count)
                {
                    pathfinderIndex = i - 1;
                    Reset();
                }
            }
        }

        static void Reset()
        {
            foreach (Agent a in Agents)
            {
                a.Reset();
            }
        }

        static void RunTests()
        {
            (int, double)[] averageTimes = new (int, double)[Agents.Count];

            for (int i = 0; i < 1000; i++) { 
                for (int j = 0; j < Agents.Count; j++)
                {
                    Agents[j].Reset();
                    
                    Stopwatch stopwatch = new();
                    stopwatch.Start();

                    Agents[j].FindPath(EndPos);
                    stopwatch.Stop();

                    averageTimes[j].Item2 = (averageTimes[j].Item1 * averageTimes[j].Item2 + stopwatch.Elapsed.TotalMilliseconds) / (averageTimes[j].Item1 + 1);
                    averageTimes[j].Item1++;
                }
            }

            File.AppendAllText(dataSavePath, $"{savePath}:\n");
            for (int i = 0; i < Agents.Count; i++)
            {
                File.AppendAllText(dataSavePath, $"Agent {i}, ({Agents[i].Pathfinder.Name}) had an average of: {averageTimes[i].Item2}\n");
                Console.WriteLine($"Agent {i}, ({Agents[i].Pathfinder.Name}) had an average of: {averageTimes[i].Item2}");
            }
        }
        
        static void RunAgent()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            bool foundPath = Agents[pathfinderIndex].FindPath(EndPos);
            if (!foundPath)
            {
                return;
            }

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

            if (DrawAgentGrid)
            {
                DrawTiles(Agents[pathfinderIndex]);
            }

            DrawGrid();

            foreach (Agent agent in Agents)
            {
                DrawPath(agent.Path, ColorMapper.ColorsForPaths[0]);
            }

            DrawText("MouseMode: " + (MouseMode), 10, SQR_PIXEL_SIZE * (SCREEN_Y + 1) - 24, 24, Color.White);

            DrawText("Elapsed time: " + ElapsedMilliseconds + " ms", SQR_PIXEL_SIZE * SCREEN_X - 500, SQR_PIXEL_SIZE * (SCREEN_Y + 1) - 24, 24, Color.White);

            for (int i = 1; i <= Pathfinders.Count; i++)
            {
                string text = i + " : " + Pathfinders[i -1].Name;

                if (i - 1 == pathfinderIndex)
                {
                    DrawRectangle(WINDOW_SIZE_X - 235, i * 30, MeasureText(text, 24) + 10, 28, Color.White);
                }

                DrawText(i + " : " + Pathfinders[i - 1].Name, WINDOW_SIZE_X - 230, i * 30, 24, i - 1 == pathfinderIndex ? Color.Black : Color.White);
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

        public static void SaveObstacles(List<Vector2> positions, string filePath)
        {
            List<Vector2Int> intVectors = positions.Select(v => new Vector2Int((int)v.X, (int)v.Y)).ToList();
            string json = JsonSerializer.Serialize(intVectors, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
            Console.WriteLine(json);
            File.WriteAllText(filePath, json);
        }

        public static List<Vector2> LoadObstacles(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Vector2>();

            string json = File.ReadAllText(filePath);
            Console.WriteLine(json);
            JsonSerializerOptions options = new JsonSerializerOptions{ IncludeFields = true};
            List<Vector2Int> intVectors = JsonSerializer.Deserialize<List<Vector2Int>>(json, options);
            List<Vector2> positions = intVectors.Select(v => new Vector2(v.x, v.y)).ToList();

            return positions; 
        }
    }
}
