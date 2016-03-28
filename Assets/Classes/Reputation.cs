using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    class Reputation
    {
        int overall;            // out of 100

        int cleanliness;        // out of 25
        int friendliness;       // out of 25
        int speed;              // out of 2500
        int facilities;         // out of 25            // 1.5 x no.Screens x average upgrade level

        public void SetFacilities(List<ScreenObject> screens)
        {
            int numScreens = screens.Count;
            int totalRating = 0;

            for (int i = 0; i < numScreens; i++)
            {
                totalRating += screens[i].getUpgradeLevel();
            }

            float averageRating = totalRating / numScreens;

            this.facilities = (int)(1.5 * numScreens * averageRating);

            if (facilities > 25) { facilities = 25; }

            SetOverall();

        }

        public void SetOverall()
        {
            overall = cleanliness + friendliness + (speed / 100) + facilities;
        }

        public float GetMultiplier()
        {
            return overall / 75;
        }

    }
}
