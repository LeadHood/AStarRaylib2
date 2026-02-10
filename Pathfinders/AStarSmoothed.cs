using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AStarRaylib.Pathfinders
{
    class AStarSmoothed(Func<Vector2, Vector2, int, int> gEvaluator, Func<Vector2, Vector2, int> hEvalutator, string name) : IPathFinder
    {
        public string Name => name;

        private const float RAYCAST_OFFSET = 0.1f;

        public List<Vector2> FindPath(Tile[,] tiles, Tile start, Tile end)
        {
            IPathFinder AStarBase = new AStarBase(gEvaluator, hEvalutator, "AStarBase");
            List<Vector2> path = AStarBase.FindPath(tiles, start, end);

            if (path.Count > 0)
            {
                path = SmoothPath(tiles, new LinkedList<Vector2>(path));
            }

            return path;
        }

        /*
            checkPoint = starting point of path
            currentPoint = next point in path
            while (currentPoint->next != NULL)
            if Walkable(checkPoint, currentPoint->next)
            // Make a straight path between those points:
            temp = currentPoint
            currentPoint = currentPoint->next
            delete temp from the path
            else
            checkPoint = currentPoint
            currentPoint = currentPoint->next
        */

        private List<Vector2> SmoothPath(Tile[,] tiles, LinkedList<Vector2> path)
        {
            if(path.Count <= 2)
            {
                return path.ToList();
            }

            LinkedListNode<Vector2> checkPoint = path.First!;
            LinkedListNode<Vector2> currentPoint = checkPoint.Next!;

            while(currentPoint.Next != null)
            {
                if(Walkable(tiles, checkPoint.Value, currentPoint.Next.Value))
                {
                    path.Remove(currentPoint);
                    currentPoint = checkPoint.Next!; 
                }
                else
                {
                    checkPoint = currentPoint;
                    currentPoint = currentPoint.Next;
                }
            }

            return path.ToList();
        }

        private bool Walkable(Tile[,] tiles, Vector2 a, Vector2 b)
        {
            Vector2 dir = Vector2.Normalize(b - a);
            Vector2 right = new Vector2(dir.Y, - dir.X);


            Vector2[] starts =
            {
                a,
                a + right * RAYCAST_OFFSET,
                a - right * RAYCAST_OFFSET
            };

            Vector2[] ends =
            {
                b,
                b + right * RAYCAST_OFFSET,
                b - right * RAYCAST_OFFSET
            };


            for (int i = 0; i < 3; i++)
            {
                if(!RayCast(tiles, starts[i], ends[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool RayCast(Tile[,] tiles, Vector2 a, Vector2 b)
        {
            List<Tile> hitList = MiscMethods.SupercoverLine(tiles, a, b);

            foreach (Tile t in hitList)
            {
                if (t.Type == TileType.Obstacle)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
