using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading;

namespace The_Betting_App
{
    public partial class Form1
    {
        #region Variables
        List<Fixture> suitableFixtures = new List<Fixture>();
        #endregion

        /// <summary>
        /// When the user clicks the button to start the calculations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public List<Fixture> Calculate()
        {
            // get values from the form
            //String day = cmbDay.Text;       // what day to get fixtures for
            //int formGames = trckForm.Value; // how many games to take into account for the games

            String day = "Saturday";
            int formGames = 4;

            // Clear labels
           
            // Clear the form picture boxes
            for (int i = 0; i < 10; i++) 
            {
                
            }

            // Remove all fixtures from the list
            suitableFixtures.Clear();

            #region Threads
            // start a thread for each league - pass: name of the league, number of teams in the leage, what day to get fixtures for, how many games to judge form on
            //Thread premThread = new Thread(() => calculateFixtures("premier-league", 20, day, formGames));
            calculateFixtures("premier-league", 20, day, formGames);
            //premThread.Start();
            //Thread champThread = new Thread(() => calculateFixtures("championship", 24, day, formGames));
            //champThread.Start();
            //Thread l1Thread = new Thread(() => calculateFixtures("league-one", 24, day, formGames));
            //l1Thread.Start();
            //Thread l2Thread = new Thread(() => calculateFixtures("league-two", 24, day, formGames));
            //l2Thread.Start();
            //Thread scotPremThread = new Thread(() => calculateFixtures("scottish-premiership", 12, day, formGames));
            //scotPremThread.Start();
            //Thread scotChampThread = new Thread(() => calculateFixtures("scottish-championship", 10, day, formGames));
            //scotChampThread.Start();
            //Thread scotL1Thread = new Thread(() => calculateFixtures("scottish-league-one", 10, day, formGames));
            //scotL1Thread.Start();
            //Thread scotL2Thread = new Thread(() => calculateFixtures("scottish-league-two", 10, day, formGames));
            //scotL2Thread.Start();
            #endregion

            #region Thread Joins
            // Wait for all Threads to Finish
            //premThread.Join();
            //champThread.Join();
            //l1Thread.Join();
            //l2Thread.Join();
            //scotPremThread.Join();
            //scotChampThread.Join();
            //scotL1Thread.Join();
            //scotL2Thread.Join();
            #endregion


            // NOW, all the fixtures have been obtained

            // This part will get the odds for each fixture

            // Loop through all fixtures which matched the criteria so far
            //for (int i = 0; i < suitableFixtures.Count; i++)
            //{
            //    Odds odds = new Odds();

            //    // convert the BBC name for the league into the SkyBet version
            //    String url = odds.convertLeague(suitableFixtures[i].getLeague());

            //    // Get the HTML page source from the page
            //    String data = odds.getUrlSource("http://www.skybet.com/football/" + url);

            //    // Extract the odds for the home team
            //    String homeOdds = odds.getOdds(data, suitableFixtures[i].getHomeTeam().getName(), day);

            //    // Extract the odds for the away team
            //    String awayOdds = odds.getOdds(data, suitableFixtures[i].getAwayTeam().getName(), day);


            //    // check that neither of the odds are null - i.e. team not found
            //    if (!(homeOdds == null || awayOdds == null))
            //    {
            //        // work out which team is higher in the table
            //        int higher = suitableFixtures[i].getHigherInTable();

            //        // get the home and away teams
            //        String home = suitableFixtures[i].getHomeTeam().getName();
            //        String away = suitableFixtures[i].getAwayTeam().getName();

            //        // depending on which team is higher in the league, make their name in all captitals
            //        if (higher < 0)
            //        {
            //            suitableFixtures[i].setHomeTeam(home.ToUpper());
            //        }
            //        else
            //        {
            //            suitableFixtures[i].setAwayTeam(away.ToUpper());
            //        }
            //        suitableFixtures[i].setOdds(homeOdds, awayOdds);
            //    }
            //        // if either team not found, throw away the fixture
            //    else 
            //    {
            //        suitableFixtures.RemoveAt(i);
            //        i--;
            //    }
            //}

            // call the function to create the labels
            //createLabels(suitableFixtures);

            return suitableFixtures;
        }

        /// <summary>
        /// create the labels for each fixture
        /// </summary>
        /// <param name="fixtures"></param>
        private void createLabels(List<Fixture> fixtures) 
        {
            // loop through all fixtures
            for (int i = 0; i < fixtures.Count; i++)
            {
                // add a thing to the list
                Console.WriteLine(fixtures[i].getHomeTeam() + " vs " + fixtures[i].getAwayTeam());
            }

            // display form

            #region Odds Calculation

            // How many teams have been selected
            int numTeams = fixtures.Count;

            // Set the label
            //lblNumTeams.Text = numTeams + " Teams";

            // Get the odds from each fixture
            List<String> oddsList = new List<String>();
            for (int i = 0; i < fixtures.Count; i++)
            {
                // Work out which team is higher in the table - use their odds
                int higher = fixtures[i].getHigherInTable();
                if (higher < 0) { oddsList.Add(fixtures[i].getHomeOdds()); } else { oddsList.Add(fixtures[i].getAwayOdds()); }
            }

            // Calculate the overall odds based on the fixtures
            double odds = calculateOdds(oddsList);    

            // show the form
            #endregion

        }

        /// <summary>
        /// Calculates how confident of being correct about a Fixture we are
        /// </summary>
        /// <param name="fixt">The fixture to check</param>
        /// <returns>Level of Confidence</returns>
        public int calculateConfidence(Fixture fixt)
        {
            // Calculate the gap between the teams
            int higher = Math.Abs(fixt.getHigherInTable());
            
            // Work out this gap as a percentage of the overall table
            int numTeamsInLeague = fixt.getTeamsInLeague();
            float perc = ((float)higher / (float)numTeamsInLeague);
            int percent = (int)(perc * 80);     // 80 is used as it is the width of the output PictureBox

            // If the form option is selected
            //if (chkForm.Checked) 
            //{
            // Get form for home and away teams
            //int numGames = trckForm.Value;
            int numGames = 4;

                List<Result> hForm = fixt.getHomeTeam().getSomeResults(numGames);
                List<Result> aForm = fixt.getAwayTeam().getSomeResults(numGames);

                // Calculate the points they picked up from these games
                int homeForm = fixt.calcFormPoints(hForm);
                int awayForm = fixt.calcFormPoints(aForm);
                
                // Calculate the difference between these points
                int pointDifference = Math.Abs(homeForm - awayForm);

                // Work this ou as a percentage - with some correcting ( * 3 )
                float formPerc = (float)pointDifference / (float)(numGames * 3);
                int formPercent = (int)(formPerc * 80);     // 80 is used as it is the width of the output PictureBox

                // Calculate the new toal Percentage
                percent = (int)((percent + formPercent) / 2);

            //}

            return percent;
        }
        
        /// <summary>
        /// Calculating the total odds for the selected teams
        /// </summary>
        /// <param name="odds">The string versions of all the odds</param>
        /// <returns>The total odds</returns>
        private double calculateOdds(List<String> odds)
        {
            float total = 1;

            // for each of the odds
            for(int i = 0; i < odds.Count; i++)
            {
                // Split the string at the '/' character
                String[] tmp = odds[i].Split('/');

                // Calculate the odds as a Float value
                float odd1 = float.Parse(tmp[0]);
                float odd2 = float.Parse(tmp[1]);
                float oddsFloat = 1 + (odd1 / odd2);

                // Multiply the total by this new value
                total *= oddsFloat;
            }

            // Round the total to 2 decimal places
            double dTotal = Math.Round(total, 2);

            // Need to subtract 1 for some reason - original stake??
            return dTotal - 1;
        }
        
        //private void displayForm(Object sender, EventArgs e) 
        //{
        //    // Work out which fixture was hovered on
        //    String tag = ((Label)sender).Tag.ToString();
        //    int index = int.Parse(tag);

        //    #region Home position string
        //    String hPos;

        //    int pos = suitableFixtures[index].getHomeTeam().getPosition();

        //    hPos = pos.ToString();

        //    if (pos % 10 == 1)
        //    {
        //        hPos += "st";
        //    }
        //    else if (pos % 10 == 2)
        //    {
        //        hPos += "nd";
        //    }
        //    else if (pos % 10 == 3)
        //    {
        //        if (pos != 13)
        //        {
        //            hPos += "rd";
        //        }
        //        else
        //        {
        //            hPos += "th";
        //        }
        //    }
        //    else
        //    {
        //        hPos += "th";
        //    }
        //    #endregion
            
        //    #region Away position string
        //    String aPos;

        //    pos = suitableFixtures[index].getAwayTeam().getPosition();

        //    aPos = pos.ToString();

        //    if (pos % 10 == 1)
        //    {
        //        aPos += "st";
        //    }
        //    else if (pos % 10 == 2) 
        //    {
        //        aPos += "nd";
        //    }
        //    else if (pos % 10 == 3)
        //    {
        //        if (pos != 13)
        //        {
        //            aPos += "rd";
        //        }
        //        else 
        //        {
        //            aPos += "th";
        //        }
        //    }
        //    else 
        //    {
        //        aPos += "th";
        //    }
        //    #endregion

        //    // Set the labels
        //    lblHomeForm.Text = suitableFixtures[index].getHomeTeam().getName() + "'s Form (" + hPos + ")";
        //    lblAwayForm.Text = suitableFixtures[index].getAwayTeam().getName() + "'s Form (" + aPos + ")";

        //    // Get the results for both teams
        //    List<Result> homeResults = suitableFixtures[index].getHomeTeam().getAllResults();
        //    List<Result> awayResults = suitableFixtures[index].getAwayTeam().getAllResults();

        //    // Loop through all results
        //    for (int i = 0; i < homeResults.Count; i++)
        //    {
        //        #region Home Team Form
        //        // If the outcome was a Win, make the square turn green
        //        if (homeResults[i].getOutcome().Equals("Win"))
        //        {
        //            homeForm[i].BackColor = Color.Green;
        //        }
        //        // Otherwise (i.e. it was a Loss), make the square yellow
        //        else if (homeResults[i].getOutcome().Equals("Draw"))
        //        {
        //            homeForm[i].BackColor = Color.Yellow;
        //        }
        //        // Otherwise (i.e. it was a Loss), make the square red
        //        else
        //        {
        //            homeForm[i].BackColor = Color.Red;
        //        }
        //        #endregion

        //        #region Away Team Form
        //        // If the outcome was a Win, make the square turn green
        //        if (awayResults[i].getOutcome().Equals("Win"))
        //        {
        //            awayForm[i].BackColor = Color.Green;
        //        }
        //        // Otherwise (i.e. it was a Loss), make the square yellow
        //        else if (awayResults[i].getOutcome().Equals("Draw"))
        //        {

        //            awayForm[i].BackColor = Color.Yellow;
        //        }
        //        // Otherwise (i.e. it was a Loss), make the square red
        //        else
        //        {
        //            awayForm[i].BackColor = Color.Red;
        //        }
        //        #endregion
        //    }
        //}

        /// <summary>
        /// Calculate the fixtures that are suitable for a specific league
        /// </summary>
        /// <param name="league">Which league to get fixtures for</param>
        /// <param name="teams">How many teams are in that league</param>
        /// <param name="day">What day to get fixtures from</param>
        /// <param name="formGames">How many games to include in the form checks</param>
        private void calculateFixtures(string league, int teams, String day, int formGames) 
        {
            FixturesAndTables calc = new FixturesAndTables();
            
            List<Team> table = calc.getTable(league, teams);
            

            List<Fixture> fixtures = calc.getFixture(league, teams/2, day, table);

            int count = 0;

            for (int i = 0; i < fixtures.Count; i++)
            {
                if (fixtures[i].isSuitableFixture())
                {
                    
                    // check form
                    //if (fixtures[i].checkFormValidity(formGames))
                    //{

                        suitableFixtures.Add(fixtures[i]);
                        count++;
                    //}
                }
                // check form if table not checked
                
            }

            


        }

        //private void trckForm_ValueChanged(object sender, EventArgs e)
        //{
        //    if (trckForm.Value > 1)
        //    {
        //        lblNumFormGames.Text = "Last " + trckForm.Value + " games";
        //    }
        //    else
        //    {
        //        lblNumFormGames.Text = "Last " + trckForm.Value + " game";
        //    }
        //}

        //private void chkForm_CheckStateChanged(object sender, EventArgs e)
        //{
        //    if (chkForm.Checked)
        //    {
        //        lblNumFormGames.Visible = true;
        //        trckForm.Visible = true;
        //    }
        //    else
        //    {
        //        lblNumFormGames.Visible = false;
        //        trckForm.Visible = false;
        //    }
        //}

    }
}
