using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace AStarRaylib
{
    interface IPathFinder
    {
        bool FirstIteration { get; }
        bool FoundPath { get; set; }
        List<Tile> OpenedTiles { get; set; }

        Tile? IterationForTile(Tile tile, Tile[,] tiles, Vector2 startPos, Vector2 endPos);

        Tile? ChooseLowestF(Tile[,] tiles, Vector2 startpos);

        List<Vector2> EnhancePath(List<Vector2> path, Tile[,] tiles);

        void ResetBrain();
    }
}
