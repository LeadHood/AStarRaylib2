using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace AStarRaylib
{
    interface IPathFinder
    {
        string Name { get; }
        int SearchedTiles { get; }
        List<Vector2> FindPath(Tile[,] tiles, Tile start, Tile end);
    }
}
