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

        int publicity;        // out of 25
        int staff;              // out of 25
        int speed;              // out of 25
        int facilities;         // out of 25            // 1.5 x no.Screens x average upgrade level
        int totalSpeedValues;   // value of all remaining patiences
        int numCustomersServed; // how many customers were served
        int highestReputation;  // the max that the reputation has been
        int totalCoinIncome;    // how many coins have been earnt in total - does not include expenditure
        int totalQuestionCount = 0;     // the amount of time a question has been open for
        int currDay = 0;        // the current day


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
            publicity = 0;
            staff = 2;
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
        public int GetPublicityRating() { return publicity; }
        public int GetStaffRating() { return staff; }
        public int GetFacilitiesRating() { return facilities; }
        public float GetMultiplier()
        {
            return (float)overall / (float)50f;
        }
        #endregion

        #region Mutuators
        public void SetOverall()
        {
            // calculate the overall rating
            overall = (int)((0.5f * (float)publicity) + staff + (0.5 * (float)speed) + facilities);

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
            if (speed < 0)
            {
                speed = 0;
            }

        }
        public void SetPublicityRating(bool[] posters)
        {
            int rating = 0;
            if (posters[0]) { rating += 12; }
            if (posters[1]) { rating += 12; }

            publicity = rating;

        }
        public void SetStaffRating(List<StaffMember> sl)
        {
            currDay++;

            int rating = 0;

            int temp = 0;

            foreach (StaffMember sm in sl)
            {
                temp += sm.GetAttributeByIndex(2);
                temp += sm.GetAttributeByIndex(3);

                temp = temp / 2;
                rating += temp;
            }
            
            int maxExpected = currDay * 25;

            float waitTimePerDay = totalQuestionCount / maxExpected;

            int toMinus = (int)Math.Round(waitTimePerDay, 0) * 2;

            rating -= toMinus;    

            if (rating > 25) { rating = 25; }
            if (rating < 0) { rating = 0; }

            staff = rating;

        }



        public void SetStaffQuestionSpeed(int speed)
        {
            totalQuestionCount += speed;
        }

        public void Walkout()
        {
            UpdateSpeedRating(-150);
        }
        public void SetFacilities(List<ScreenObject> screens, bool redCarpet, FoodArea food)
        {
            int numScreens = screens.Count;
            int totalRating = 0;

            // get the combined total of the screen upgrade level
            for (int i = 0; i < numScreens; i++)
            {
                int level = screens[i].GetUpgradeLevel();

                if (screens[i].GetDaysOfConstruction() > 1)
                {
                    level--;
                }
                totalRating += level;
                if (screens[i].GetUpgradeLevel() == 4) { totalRating += 2; }
                if (screens[i].GetUpgradeLevel() == 3) { totalRating += 1; }
            }
            
            // set the facilities value
            this.facilities = totalRating;

            if (redCarpet) { facilities += 2; }
            if (food != null) { facilities += 4; }

            if (facilities > 25) { facilities = 25; }

        }
        #endregion
        
    }
}
