using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace AStarRaylib
{
    interface IPathFinder
    {
        public string Name { get; }
        public List<Vector2> FindPath(Tile[,] tiles, Tile start, Tile end);
    }
}
