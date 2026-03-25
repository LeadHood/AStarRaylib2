//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Numerics;
//using Raylib_cs;

//namespace AStarRaylib.Pathfinders
//{
//    class AStarOptimized(Func<Vector2, Vector2, int, int> gEvaluator, Func<Vector2, Vector2, int> hEvalutator, string name) : IPathFinder
//    {
//        string IPathFinder.Name => name;

//        public List<Vector2> FindPath(Tile[,] tiles, Tile start, Tile end)
//        {
//            IPathFinder AStarBase = new AStar(gEvaluator, hEvalutator, "AStarBase");
//            List<Vector2> path = AStarBase.FindPath(tiles, start, end);

//            path = EnhancePath(path, tiles);

//            return path;
//        }

//        public List<Vector2> EnhancePath(List<Vector2> path, Tile[,] tiles)
//        {
//            if(path.Count <= 2)
//            {
//                return path;
//            }

//            List<Vector2> onlyCorners = [path[0]];

//            for (int i = 1; i < path.Count - 1; i++)
//            {
//                Vector2 currPos = path[i];

//                if (IsCorner(currPos, tiles))
//                {
//                    onlyCorners.Add(currPos);
//                    //tiles[(int)currPos.X, (int)currPos.Y].OverrideColor = Raylib_cs.Color.DarkPurple;
//                }

//                continue;
//            }

//            //Adding last elemebnt of path to onlycorners to include both start and endpos
//            onlyCorners.Add(path[^1]);

//            List<Vector2> newPath = [..onlyCorners];

//            bool foundFalseCorner = true;

//            //Removing false corners until none exist
//            while (foundFalseCorner)
//            {
//                foundFalseCorner = false;
//                //Finding the false corners
//                for (int i = 1; i < onlyCorners.Count - 1; i++)
//                {
//                    int nPIndex = newPath.IndexOf(onlyCorners[i]);

//                    if (nPIndex == -1)
//                    {
//                        continue;
//                    }

//                    Vector2 currPos = onlyCorners[i];
//                    Vector2 nextPos = newPath[nPIndex + 1];
//                    Vector2 prevPos = newPath[nPIndex - 1];

//                    if (IsFalseCorner(currPos, prevPos, nextPos, tiles))
//                    {
//                        foundFalseCorner = true;
//                        newPath.Remove(onlyCorners[i]);
//                    }
//                }
//            }

//            List<Vector2> pathWithoutCollision = [];

//            //Raycasting for exceptional exceptions
//            for (int i = 0; i < newPath.Count - 1; i++)
//            {
//                Vector2 pos = newPath[i];
//                Vector2 nextPos = newPath[i + 1];

//                pathWithoutCollision.Add(pos);

//                List<Tile> tilesHitByRayCast = MiscMethods.SupercoverLine(tiles, pos, nextPos);

//                //Debug color for raycast

//                if (Program.DisplayRayCastDebug)
//                {
//                    foreach (Tile tile in tilesHitByRayCast)
//                    {
//                        tile.OverrideColor = Color.Yellow;
//                    }
//                }

//                Tile? hitTile = tilesHitByRayCast.FirstOrDefault(tile => tile.Type == TileType.Obstacle);

//                if (hitTile != null)
//                {
//                    Agent newAgent = new Agent(new AStar(gEvaluator, hEvalutator, "Wow"), tilesHitByRayCast[tilesHitByRayCast.IndexOf(hitTile)-1].Position);
//                    newAgent.FindPath(nextPos);
//                    List<Vector2> pathBetweenRaycast = newAgent.Path;
//                    pathBetweenRaycast.RemoveAt(pathBetweenRaycast.Count - 1);
//                    pathBetweenRaycast.RemoveAt(0);
//                    pathWithoutCollision.AddRange(pathBetweenRaycast);
//                }
//            }

//            pathWithoutCollision.Add(newPath[^1]);

//            return pathWithoutCollision;
//        }
        
//        private bool IsCorner(Vector2 pos, Tile[,] tiles)
//        {
//            foreach(var (dx, dy) in MiscMethods.CornerOffsets)
//            {
//                int i = (int)pos.X + dx;
//                int j = (int)pos.Y + dy;

//                //if it Is outside then continue
//                if (i < 0 || i >= tiles.GetLength(0) || j < 0 || j >= tiles.GetLength(1))
//                {
//                    continue;
//                }

//                Tile neighbour1 = tiles[i, (int)pos.Y];
//                Tile neighbour2 = tiles[(int)pos.X, j];

//                if (i != pos.X && j != pos.Y && tiles[i, j].Type == TileType.Obstacle)
//                {
//                    if (!(neighbour1.Type == TileType.Obstacle) && !(neighbour2.Type == TileType.Obstacle))
//                    {
//                        return true;
//                    }
//                }
//            }

//            return false;
//        }

//        private bool IsFalseCorner(Vector2 pos, Vector2 prevTile, Vector2 nextTile, Tile[,] tiles)
//        {
//            foreach (var (dx, dy) in MiscMethods.CornerOffsets)
//            {
//                int i = (int)pos.X + dx;
//                int j = (int)pos.Y + dy;

//                //if it Is outside then continue
//                if (i < 0 || i >= tiles.GetLength(0) || j < 0 || j >= tiles.GetLength(1))
//                {
//                    continue;
//                }

//                if (tiles[i, j].Type == TileType.Obstacle)
//                {
//                    Vector2 vecToPrev = prevTile - pos;
//                    Vector2 vecToObstacle = new Vector2(i, j) - pos;
//                    Vector2 vecToNext = nextTile - pos;

//                    tiles[(int)pos.X, (int)pos.Y].DebugVector = vecToObstacle;
//                    tiles[(int)pos.X, (int)pos.Y].DebugVector1 = vecToPrev;
//                    tiles[(int)pos.X, (int)pos.Y].DebugVector2 = vecToNext;

//                    //Angle math mathing
//                    double angle1 = MiscMethods.Angle(vecToPrev, vecToObstacle);
//                    double angle2 = MiscMethods.Angle(vecToNext, vecToObstacle);

//                    double angle = angle1 + angle2;

//                    tiles[(int)pos.X, (int)pos.Y].DebugAngle = (float)angle;
//                    tiles[(int)pos.X, (int)pos.Y].DebugAngle1 = (float)angle1;
//                    tiles[(int)pos.X, (int)pos.Y].DebugAngle2 = (float)angle2;

//                    double epsilon = 0.01d;

//                    if (angle < Math.PI)
//                    {
//                        return false;
//                    }
//                }
//            }

//            //tiles[(int)pos.X, (int)pos.Y].OverrideColor = Raylib_cs.Color.Beige;

//            return true;
//        }
//    }
//}
