using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace The_Betting_App
{
    public class Team
    {
        #region Variables
        String name;
        List<Result> results;
        int position;
        #endregion

        #region Constructors
        public Team(String name, List<Result> results)
        {
            this.name = name;
            this.results = results;
        }
        public Team(String name, List<Result> results, int position)
        {
            this.name = name;
            this.results = results;
            this.position = position;
        }
        #endregion

        #region Accessors
        public String getName() { return this.name; }
        public List<Result> getAllResults() { return this.results; }
        public int getPosition()
        {
            return this.position;
        }
        public String getFormString() 
        {
            String output = "";
            for (int i = 0; i < results.Count; i++) 
            {
                String outcome = this.results[i].getOutcome();
                output += outcome[0];
            }

            return output;

        }
        #endregion

        /// <summary>
        /// method to set the name of the team
        /// </summary>
        /// <param name="name">The name of the team</param>
        public void setTeam(String name) { this.name = name; }

        /// <summary>
        /// get some fixtures - i.e. latest 4 games or latest 2 games
        /// </summary>
        /// <param name="numToGet">How many results to get</param>
        /// <returns></returns>
        public List<Result> getSomeResults(int numToGet) 
        {
            List<Result> toReturn = new List<Result>();

            int i = results.Count - 1;
            int count = 0;

            // loop through all elements - from back to front. Add each one to toReturn
            while (count < numToGet)
            {
                toReturn.Add(results[i]);
                i--;
                count++;
            }            
               
            return toReturn;

        }
    }
}
