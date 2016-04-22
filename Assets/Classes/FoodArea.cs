using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    [Serializable]
    public class FoodArea
    {
        // simply consists of 4 public variables
        public bool hasHotFood = false; // whether or not the Hot Food component is unlocked
        public bool hasPopcorn = false; // whether or not the Popcorn component is unlocked
        public bool hasIceCream = false; // whether or not the Ice Cream component is unlocked
        public int tableStatus = 0; // the upgrade level of the table - effects the number of staff members who can be assigned
    }
}
