using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    [System.Serializable]
    public class TimeTuple
    {
        // store both the time elements (hours and minutes)
        public int hours;
        public int minutes;

        // CONSTRUCTOR for the class
        public TimeTuple(int h, int m)
        {
            hours = h;
            minutes = m;
        }
    }
}
