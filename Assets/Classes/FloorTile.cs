using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    [System.Serializable]
    public class FloorTile
    {
        public int xCoord;      // the x position of the tile
        public int yCoord;      // the y position of the tile
        public int inUse;       // the state of the tile (0 = empty, 1 = buffer, 2 = full)

        // CONSTRUCTOR for the class
        public FloorTile(int x, int y)
        {
            xCoord = x;
            yCoord = y;
        }
    }
}