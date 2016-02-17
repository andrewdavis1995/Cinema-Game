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
        public int inUse;       // 0 = empty, 1 = buffer, 2 = full

        public FloorTile(int x, int y)
        {
            xCoord = x;
            yCoord = y;
        }
    }
}