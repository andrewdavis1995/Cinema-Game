using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    [System.Serializable]
    public class FloorTile
    {
        public int xCoord;
        public int yCoord;
        public bool inUse;

        public FloorTile(int x, int y)
        {
            xCoord = x;
            yCoord = y;
        }
    }
}