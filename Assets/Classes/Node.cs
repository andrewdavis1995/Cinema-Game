using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Node
{
    public Coordinate location;
    public List<Node> path = new List<Node>();

    public Node(Coordinate l)
    {
        location = l;
    }

    public List<Node> GetPath()
    {
        return this.path;
    }

    public List<Node> FindChildren(List<Node> explored, List<Node> tmp, char[,] states)
    {
        List<Node> temp = new List<Node>();

        if (location.x > 0)
        {
            Node toAdd = new Node(new Coordinate(location.x - 1, location.y));

            if (!IsSameLocation(toAdd, explored) && !IsSameLocation(toAdd, tmp) && states[location.x - 1, location.y] != '0')
            {
                temp.Add(toAdd);
            }
        }
        if (location.x < 19)
        {
            Node toAdd = new Node(new Coordinate(location.x + 1, location.y));

            if (!IsSameLocation(toAdd, explored) && !IsSameLocation(toAdd, tmp) && states[location.x + 1, location.y] != '0')
            {
                temp.Add(toAdd);
            }
        }
        if (location.y > 0)
        {
            Node toAdd = new Node(new Coordinate(location.x, location.y - 1));

            if (!IsSameLocation(toAdd, explored) && !IsSameLocation(toAdd, tmp) && states[location.x, location.y - 1] != '0')
            {
                temp.Add(toAdd);
            }
        }
        if (location.y < 24)
        {
            Node toAdd = new Node(new Coordinate(location.x, location.y + 1));

            if (!IsSameLocation(toAdd, explored) && !IsSameLocation(toAdd, tmp) && states[location.x, location.y + 1] != '0')
            {
                temp.Add(toAdd);
            }
        }

        return temp;
    }

    public bool IsSameLocation(Node targetNode, List<Node> explored)
    {
        for (int i = 0; i < explored.Count; i++)
        {
            if (targetNode.location.x == explored[i].location.x && targetNode.location.y == explored[i].location.y)
            {
                return true;
            }
        }
        return false;
    }

    public bool matchesGoal(Coordinate cGoal)
    {
        if (location.x == cGoal.x && location.y == cGoal.y) { return true; } else { return false; }
    }

    public String LocationOutput()
    {
        return "X: " + location.x + ", Y:" + location.y;
    }

    //public void SetChildren(List<Node> childNodes)
    //{
    //    children = childNodes;
    //}

    //public List<Node> GetChildren()
    //{
    //    return children;
    //}

    //public Node GetParent()
    //{
    //    return parent;
    //}

}

