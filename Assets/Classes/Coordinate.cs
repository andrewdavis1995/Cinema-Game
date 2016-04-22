using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class Coordinate
{
    // the x and y coordinates of the point
    public int x;
    public int y;

    // CONSTRUCTOR for the class
    public Coordinate(int theX, int theY)
    {
        x = theX;
        y = theY;
    }
}

