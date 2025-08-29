using System;
using Raylib_cs;

namespace AStarRaylib
{
    class Program
    {
        static void Main(string[] args)
        {
            Raylib.InitWindow(800, 800, "ASTAR");

            while (!Raylib.WindowShouldClose())
            {
                Update();

                Draw();
            }
        }

        static void Update()
        {

        }

        static void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            Raylib.DrawText("Hello guy", 500, 500, 20, Color.Red);

            Raylib.DrawRectangle(10, 10, 100, 100, Color.Brown);

            Raylib.EndDrawing();
        }
    }
}
