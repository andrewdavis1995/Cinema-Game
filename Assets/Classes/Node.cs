using Assets.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Node
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

    public List<Node> FindChildren(List<Node> explored, List<Node> tmp, FloorTile[,] states)
    {
        List<Node> temp = new List<Node>();

        if (location.x > 0)
        {
            Node toAdd = new Node(new Coordinate(location.x - 1, location.y));

            if (!IsSameLocation(toAdd, explored) && !IsSameLocation(toAdd, tmp) && states[location.y, location.x-1].inUse != 2)
            {
                temp.Add(toAdd);
            }
        }
        if (location.x < 79)
        {
            Node toAdd = new Node(new Coordinate(location.x + 1, location.y));

            if (!IsSameLocation(toAdd, explored) && !IsSameLocation(toAdd, tmp) && states[location.y, location.x + 1].inUse != 2)
            {
                temp.Add(toAdd);
            }
        }
        if (location.y > 0)
        {
            Node toAdd = new Node(new Coordinate(location.x, location.y-1));

            if (!IsSameLocation(toAdd, explored) && !IsSameLocation(toAdd, tmp) && states[location.y-1, location.x].inUse != 2)
            {
                temp.Add(toAdd);
            }
        }
        if (location.y < 39)
        {
            Node toAdd = new Node(new Coordinate(location.x, location.y+1));

            if (!IsSameLocation(toAdd, explored) && !IsSameLocation(toAdd, tmp) && states[location.y+1, location.x].inUse != 2)
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

