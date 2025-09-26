using System;
using Raylib_cs;
using System.Numerics;

namespace AStarRaylib
{
    class Program
    {
        public const int SQR_PIXEL_SIZE = 100;

        const int SCREEN_X = 10;
        const int SCREEN_Y = 5;

        static Vector2 StartPos = new Vector2(1, 1);
        static Vector2 EndPos = new Vector2(7, 3);

        static Tile[,] Tiles = new Tile[SCREEN_X, SCREEN_Y];

        static void Main(string[] args)
        {
            Raylib.InitWindow(SCREEN_X * SQR_PIXEL_SIZE, SCREEN_Y * SQR_PIXEL_SIZE, "ASTAR");

            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    Tiles[x, y] = new Tile(new Vector2(x, y), TileType.Unopened);
                }
            }

           // Tiles[(int)StartPos.X, (int)StartPos.

            while (!Raylib.WindowShouldClose())
            {
                Update();


                Draw();
            }
        }

        static void Update()
        {
            //if (Raylib.GetKeyPressed() == (int)KeyboardKey.T)
            //{
            //    Raylib.SetWindowTitle("Epi");
            //}
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

            Raylib.EndDrawing();
        }
    }
}
