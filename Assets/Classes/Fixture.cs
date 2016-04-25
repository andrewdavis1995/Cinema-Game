using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace The_Betting_App
{
    public class Fixture
    {
        #region Variables
        Team homeTeam;
        Team awayTeam;

        int teamsInLeague;

        // Two needed - output and internal
        String competition;
        String league;

        String homeOdds;
        String awayOdds;
        #endregion

        /// <summary>
        /// Constructor for a Fixture
        /// </summary>
        /// <param name="h">The Home Team</param>
        /// <param name="a">The Away Team</param>
        /// <param name="t">The Number of teams in the Competition</param>
        /// <param name="comp">Which competition the fixture is in</param>
        public Fixture(Team h, Team a, int t, String comp) 
        {
            this.homeTeam = h;
            this.awayTeam = a;
            this.teamsInLeague = t;
            this.competition = generateComp(comp);
            this.league = comp;
        }

        #region Accessors
        // ACCESSORS
        public Team getHomeTeam() { return this.homeTeam; }
        public Team getAwayTeam() { return this.awayTeam; }
        public int getTeamsInLeague() { return this.teamsInLeague; }
        public String getHomeOdds() { return this.homeOdds; }
        public String getAwayOdds() { return this.awayOdds; }
        public String getCompetition() { return this.competition; }
        public String getLeague() { return this.league; }
        #endregion

        /// <summary>
        /// Set the odds for the Fixture
        /// </summary>
        /// <param name="h">Odds for the Home team</param>
        /// <param name="a">Odds for the Away team</param>
        public void setOdds(String h, String a) 
        {
            homeOdds = h;
            awayOdds = a;
        }

        /// <summary>
        /// Method to fix the Competition to make it suitable to output
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns></returns>
        private String generateComp(String input)
        {   
            // replace all '-' with a space
            input = input.Replace('-', ' ');

            // convert the first letter of each word to a capital
            TextInfo textInfo = new CultureInfo("en-US",false).TextInfo;
            input = textInfo.ToTitleCase(input);

            return input;
        }

        /// <summary>
        /// Checks if the fixture matches the criteria
        /// </summary>
        /// <returns></returns>
        public bool isSuitableFixture() 
        {
            // Calculates the maximum league position difference that is acceptable
            int allowableRange = (int)(this.teamsInLeague / 2.75);

            // Get the league positions of each team
            int homePos = homeTeam.getPosition();
            int awayPos = awayTeam.getPosition();

            // calculate the difference between the Positions
            int difference = Math.Abs(homePos - awayPos);

            // if the difference is acceptable, return true. Else, false
            if (difference >= allowableRange)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        /// <summary>
        /// Get which team is higher in the table
        /// </summary>
        /// <returns>If negative, Home team is higher. If it's positive, Away team is lower</returns>
        public int getHigherInTable() 
        {
            return this.homeTeam.getPosition() - this.awayTeam.getPosition();
        }

        /// <summary>
        /// Sets the home team
        /// </summary>
        /// <param name="h"></param>
        public void setHomeTeam(String h)
        {
            this.homeTeam.setTeam(h);
        }

        /// <summary>
        /// Sets the away team
        /// </summary>
        /// <param name="a">The team to set</param>
        public void setAwayTeam(String a)
        {
            this.awayTeam.setTeam(a);
        }

        /// <summary>
        /// Checks if the fixture matches the requirements for Form
        /// </summary>
        /// <param name="formGames">how many games to check</param>
        /// <returns></returns>
        public bool checkFormValidity(int formGames)
        {
            // work out how many points each team has got recently
            int homeFormPoints = calcFormPoints(homeTeam.getSomeResults(formGames));
            int awayFormPoints = calcFormPoints(awayTeam.getSomeResults(formGames));

            // calculate the difference
            int difference = awayFormPoints - homeFormPoints;

            // work out which team was higher
            int swing = getHigherInTable();

            // if both form and table 'swing' the same way - i.e. the higher team has better form
            if ((swing < 0 && difference <= 0) || (swing > 0 && difference >= 0)) 
            {
                // if the difference in points is acceptable (an average of 1 point difference per game), return true. Else, false
                int absDiff = Math.Abs(difference);

                if (absDiff >= formGames) 
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Method to calculate how many points a team has picked up recently
        /// </summary>
        /// <param name="games">The list of fixtures</param>
        /// <returns>how many points have been earned</returns>
        public int calcFormPoints(List<Result> games) 
        {

            int totalPoints = 0;

            // loop through all games
            for (int i = 0; i < games.Count; i++)
            {
                // If the game was Won, add 3 to totalPoints
                if (games[i].getOutcome().Equals("Win")) 
                {
                    totalPoints += 3;
                }
                    // if it was a draw, add 1 point
                else if (games[i].getOutcome().Equals("Draw"))
                {
                    totalPoints += 1;
                }
            }

            return totalPoints;
        }

    }
}
