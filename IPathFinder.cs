using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace AStarRaylib
{
    interface IPathFinder
    {


        List<Vector2> FindPath(Tile[,] tiles, Vector2 startPos, Vector2 endPos);
    }
}
