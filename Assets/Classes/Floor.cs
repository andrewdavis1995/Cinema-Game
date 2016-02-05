using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    [System.Serializable]
    class Floor
    {
        public FloorTile[,] floorTiles;
        public int width;
        public int height;

        public Floor(int w, int h)
        {
            width = w;
            height = h;

            floorTiles = new FloorTile[w, h];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    floorTiles[x,y] = new FloorTile(x, y);
                }
            }
        }

        public FloorTile getTileByCoord(int x, int y)
        {
            return floorTiles[x, y];
        }

    }
}
