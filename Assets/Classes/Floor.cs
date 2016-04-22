using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    [System.Serializable]
    public class Floor
    {
        public FloorTile[,] floorTiles;   // the floor tiles that make up the floor
        public int width;       // the width of the floor (number of tiles)
        public int height;      // the height of the floor (number of tiles)

        // CONSTRUCTOR for the class
        public Floor(int h, int w)
        {
            width = w;
            height = h;

            floorTiles = new FloorTile[h, w];

            // loop through x and y, creating a tile for each position
            for (int x = 0; x < h; x++)
            {
                for (int y = 0; y < w; y++)
                {
                    floorTiles[x,y] = new FloorTile(x, y);
                }
            }
        }

        // Accessors
        public FloorTile GetTileByCoord(int x, int y)
        {
            return floorTiles[x, y];
        }

        /// <summary>
        /// Pathfinding - finding a path from point x to point Y - avoiding obstacles in the way
        /// </summary>
        /// <param name="startX">Starting x coordinate</param>
        /// <param name="startY">Starting Y coordinate</param>
        /// <param name="goalX">Target X coordinate</param>
        /// <param name="goalY">Target Y coordinate</param>
        /// <returns></returns>
        public List<Coordinate> FindPath(int startX, int startY, int goalX, int goalY)
        {
            // initialise the head of the list
            Node head = new Node(new Coordinate(startX, startY));

            // whether or not a path has been found
            bool found = false;

            // setup the lists
            List<Node> openNodes = new List<Node>();
            List<Node> exploredNodes = new List<Node>();

            // add the head to the list
            openNodes.Add(head);

            // while no path has been found and there are still nodes to explore
            while (!found && openNodes.Count > 0)
            {
                // while there are open nodes to check 
                List<Node> tmp = new List<Node>();
                while (openNodes.Count > 0)
                {
                    // if the open node matches the goal
                    if (openNodes[0].MatchesGoal(new Coordinate(goalX, goalY)))
                    {
                        // set found = true and add openNodes[0] to the head
                        found = true;
                        head = openNodes[0];
                        break;
                    }
                    else
                    {
                        // find children
                        List<Node> tmpList = (openNodes[0].FindChildren(exploredNodes, tmp, floorTiles));

                        // loop through the children
                        for (int i = 0; i < tmpList.Count; i++)
                        {
                            // add path
                            List<Node> localPath = new List<Node>();

                            // get the path 
                            for (int j = 0; j < openNodes[0].GetPath().Count; j++)
                            {
                                localPath.Add(openNodes[0].GetPath()[j]);
                            }

                            Node localNode = openNodes[0];

                            // if the node has not already been searched
                            if (!localPath.Contains(openNodes[0]))
                            {
                                // add the node to the path
                                localPath.Add(localNode);
                            }

                            tmpList[i].path = localPath;
                        }

                        // add the node to the list of ones which have been searched
                        exploredNodes.Add(openNodes[0]);

                        // remove the node from the open nodes
                        openNodes.RemoveAt(0);

                        tmp.AddRange(tmpList);

                    }
                }

                openNodes = tmp;
            }

            // if the path has been found...
            if (found)
            {
                // attach the goal node to the path
                head.path.Add(new Node(new Coordinate(goalX, goalY)));

                // add all the points to a new list
                List<Coordinate> pointsToVisit = new List<Coordinate>();

                for (int i = 0; i < head.path.Count; i++)
                {
                    pointsToVisit.Add(head.path[i].location);
                }

                #region Shorten path
                bool xORy = true;
                for (int i = 0; i < pointsToVisit.Count - 2; i++)
                {
                    if (xORy)
                    {
                        if (pointsToVisit[i].x == pointsToVisit[i + 1].x)
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
                        if (pointsToVisit[i].y == pointsToVisit[i + 1].y)
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
                #endregion

                return pointsToVisit;
            }
            else
            {
                // no path available
                Console.WriteLine("NO PATH AVAILABLE");
                return null;
            }

        }

    }
}
