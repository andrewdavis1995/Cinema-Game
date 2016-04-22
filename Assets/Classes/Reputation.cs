using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    [Serializable]
    public class Reputation
    {
        int overall;            // out of 100

        int cleanliness;        // out of 25
        int friendliness;       // out of 25
        int speed;              // out of 25
        int facilities;         // out of 25            // 1.5 x no.Screens x average upgrade level
        int totalSpeedValues;   // value of all remaining patiences
        int numCustomersServed; // how many customers were served
        int highestReputation;  // the max that the reputation has been
        int totalCoinIncome;    // how many coins have been earnt in total - does not include expenditure


        /// <summary>
        /// add coins to the total amount earnt 
        /// </summary>
        /// <param name="numCoins">How many coins to add</param>     
        public void AddCoins(int numCoins)
        {
            totalCoinIncome += numCoins;
        }

        /// <summary>
        /// Set up a new Reputation instance
        /// </summary>
        public void Initialise()
        {
            // initial values
            cleanliness = 15;
            friendliness = 8;
            speed = 8;
            facilities = 1;
            highestReputation = 0;
            totalCoinIncome = 0;

            // calculate the overall rating
            SetOverall();
        }

        #region Accessors
        // Accessors
        public int GetOverall() { return overall; }
        public int GetTotalCoins() { return totalCoinIncome; }
        public int GetTotalCustomers() { return numCustomersServed; }
        public int GetHighestRep() { return highestReputation; }
        
        public int GetSpeedRating() { return speed; }
        public int GetCleanlinessRating() { return cleanliness; }
        public int GetFriendlinessRating() { return friendliness; }
        public int GetFacilitiesRating() { return facilities; }
        public float GetMultiplier()
        {
            return (float)overall / (float)75f;
        }
        #endregion

        #region Mutuators
        public void SetOverall()
        {
            // calculate the overall rating
            overall = (int)((1.25 * cleanliness) + (1.25 * friendliness) + (0.5* speed) + facilities);

            // cap at 100 - just in case
            if (overall > 100)
            {
                overall = 100;
            }

            // if the current reputation level beats the record highest, replace the record with current
            if (overall > highestReputation)
            {
                highestReputation = overall;
            }

        }
        public void UpdateSpeedRating(int patience)
        {
            numCustomersServed++;
            totalSpeedValues += patience;

            speed = (int)((float)(((float)totalSpeedValues / (float)(numCustomersServed * 1250)) * 25));

            if (speed > 100)
            {
                speed = 100;
            }

        }
        public void Walkout()
        {
            UpdateSpeedRating(-150);
        }
        public void SetFacilities(List<ScreenObject> screens, bool redCarpet)
        {
            int numScreens = screens.Count;
            int totalRating = 0;

            // get the combined total of the screen upgrade level
            for (int i = 0; i < numScreens; i++)
            {
                totalRating += screens[i].GetUpgradeLevel();
            }

            // calculate the average screen upgrade level
            float averageRating = totalRating / numScreens;

            // set the facilities value
            this.facilities = (int)(1.5 * numScreens * averageRating);

            if (redCarpet) { facilities += 2; }

            if (facilities > 25) { facilities = 25; }

        }
        #endregion
        
    }
}
