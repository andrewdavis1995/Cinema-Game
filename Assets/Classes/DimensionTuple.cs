using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    [System.Serializable]
    public class DimensionTuple
    {
        // store both the time elements (hours and minutes)
        public int width;
        public int height;

        // CONSTRUCTOR for the class
        public DimensionTuple(int w, int h)
        {
            width = w;
            height = h;
        }
    }
}
