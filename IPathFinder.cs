using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace AStarRaylib
{
    interface IPathFinder
    {
        Tile? IterationForTile(Tile tile, Tile[,] tiles, Vector2 startPos, Vector2 endPos);
    }
}
