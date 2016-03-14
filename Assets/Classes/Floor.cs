using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    [System.Serializable]
    public class Floor
    {
        public FloorTile[,] floorTiles;
        public int width;
        public int height;

        public Floor(int h, int w)
        {
            width = w;
            height = h;

            floorTiles = new FloorTile[h, w];

            for (int x = 0; x < h; x++)
            {
                for (int y = 0; y < w; y++)
                {
                    floorTiles[x,y] = new FloorTile(x, y);
                }
            }
        }

        public FloorTile getTileByCoord(int x, int y)
        {
            return floorTiles[x, y];
        }

        public List<Coordinate> FindPath(int startX, int startY, int goalX, int goalY)
        {
            // OutputStates();

            List<FloorTile> tiles = new List<FloorTile>();

            //for (int i = 0; i < 40; i++)
            //{
            //    for (int j = 0; j < 80; j++)
            //    {
            //        if (floorTiles[i, j].inUse != 0)
            //        {
            //            tiles.Add(new FloorTile(i, j));
            //            Console.WriteLine(i + ", " + j);
            //        }
            //    }
            //}

            Node head = new Node(new Coordinate(startX, startY));

            bool found = false;

            List<Node> openNodes = new List<Node>();
            List<Node> exploredNodes = new List<Node>();

            openNodes.Add(head);

            while (!found && openNodes.Count > 0)
            {
                List<Node> tmp = new List<Node>();
                while (openNodes.Count > 0)
                {
                    if (openNodes[0].matchesGoal(new Coordinate(goalX, goalY)))
                    {
                        found = true;
                        head = openNodes[0];
                        break;
                    }
                    else
                    {
                        List<Node> tmpList = (openNodes[0].FindChildren(exploredNodes, tmp, floorTiles));

                        for (int i = 0; i < tmpList.Count; i++)
                        {
                            // add path
                            List<Node> localPath = new List<Node>();

                            for (int j = 0; j < openNodes[0].GetPath().Count; j++)
                            {
                                localPath.Add(openNodes[0].GetPath()[j]);
                            }
                            Node localNode = openNodes[0];

                            if (!localPath.Contains(openNodes[0]))
                            {
                                localPath.Add(localNode);
                            }

                            //openNodes[0].path = new List<Node>();
                            tmpList[i].path = localPath;
                        }

                        exploredNodes.Add(openNodes[0]);
                        openNodes.RemoveAt(0);

                        tmp.AddRange(tmpList);

                    }
                }

                openNodes = tmp;
            }

            if (found)
            {
                head.path.Add(new Node(new Coordinate(goalX, goalY)));

                List<Coordinate> pointsToVisit = new List<Coordinate>();

                for (int i = 0; i < head.path.Count; i++)
                {
                    pointsToVisit.Add(head.path[i].location);
                }
                bool xORy = true;
                for (int i = 0; i < pointsToVisit.Count - 1; i++)
                {
                    if (xORy)
                    {
                        if (pointsToVisit[i].x != pointsToVisit[i + 1].x)
                        {
                            pointsToVisit.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            xORy = false;
                        }
                    }
                    else
                    {
                        if (pointsToVisit[i].y != pointsToVisit[i + 1].y)
                        {
                            pointsToVisit.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            xORy = true;
                        }
                    }
                }

                return pointsToVisit;
                // Customer.path = head.path;

            }
            else
            {
                Console.WriteLine("NO PATH AVAILABLE");
                return null;
            }

        }

    }
}
