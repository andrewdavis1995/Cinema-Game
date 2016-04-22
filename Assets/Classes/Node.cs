using Assets.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Node
{
    public Coordinate location;     // the location of the node
    public List<Node> path = new List<Node>();  //  the  path

    // CONSTRUCTOR for the class
    public Node(Coordinate l)
    {
        location = l;
    }
    
    // Accessors
    public List<Node> GetPath()
    {
        return this.path;
    }

    /// <summary>
    /// Find all children nodes for the current node. (i.e. the adjacent nodes)
    /// </summary>
    /// <param name="explored"></param>
    /// <param name="tmp"></param>
    /// <param name="states"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Check that the node getting checked has not already been checked
    /// </summary>
    /// <param name="targetNode"></param>
    /// <param name="explored"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Check if the goal state matches the current node
    /// </summary>
    /// <param name="cGoal"></param>
    /// <returns></returns>
    public bool MatchesGoal(Coordinate cGoal)
    {
        if (location.x == cGoal.x && location.y == cGoal.y) { return true; } else { return false; }
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

