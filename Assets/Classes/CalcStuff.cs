using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Policy;
using System.Net;
using System.IO;

namespace The_Betting_App
{
    public class FixturesAndTables
    {

        /*
         * Takes the important parts out of the Page data
         * @PARAMETERS: - pageSource: the HTML content of the page
         * @RETURNS: a string - the weather type
         */
        public List<Fixture> extractFixtures(String pageSource, int numFixtures, String day, List<Team> table, String comp){

            List<Fixture> fixtures = new List<Fixture>();
        
            // Find the position of the first instance of the string "first active" 
            int startPos = pageSource.IndexOf(day);
            
            // Cut all of the data before the start position off
            String cut1 = pageSource.Substring(startPos);

            for (int i = 0; i < numFixtures; i++)
            {
                // Find the position of the first instance of the string "alt=" (in the substring)
                startPos = cut1.IndexOf("team-home");

                int[] days = new int[7];
                days[0] = cut1.IndexOf("Monday");
                days[1] = cut1.IndexOf("Tuesday");
                days[2] = cut1.IndexOf("Wednesday");
                days[3] = cut1.IndexOf("Thursday");
                days[4] = cut1.IndexOf("Friday");
                days[5] = cut1.IndexOf("Saturday");
                days[6] = cut1.IndexOf("Sunday");

                bool sameDay = true;
                for (int j = 0; j < 7; j++)
                {
                    if (days[j] < startPos && days[j] > 0)
                    {
                        sameDay = false;
                        break;
                    }
                }
                

                if (sameDay)
                {
                    
                    // Cut the relevant part out of the string
                    cut1 = cut1.Substring(startPos);

                    startPos = cut1.IndexOf("teams/");

                    cut1 = cut1.Substring(startPos + 6);

                    startPos = cut1.IndexOf(">");

                    cut1 = cut1.Substring(startPos + 1);

                    // Find the position of the first instance of the string "\"" (in the substring)
                    int endPos = cut1.IndexOf("<");

                    // Cut the relevant part out of the string
                    String homeTeam = cut1.Substring(0, endPos);

                    startPos = cut1.IndexOf("team-away");
                    String awayTeam = cut1.Substring(startPos);

                    String secondPart = awayTeam;

                    startPos = awayTeam.IndexOf("teams/");
                    awayTeam = awayTeam.Substring(startPos + 6);
                    startPos = awayTeam.IndexOf(">");
                    awayTeam = awayTeam.Substring(startPos + 1);
                    endPos = awayTeam.IndexOf("<");
                    awayTeam = awayTeam.Substring(0, endPos);

                    startPos = secondPart.IndexOf("kickoff");
                    String kickOff = secondPart.Substring(startPos + 18, 5);


                    if (kickOff.Equals("15:00") || kickOff.Equals("19:45"))
                    {
                        Team theHomeTeam = findTeam(homeTeam, table);
                        Team theAwayTeam = findTeam(awayTeam, table);


                        //return the fully trimmed String
                        Fixture newFixture = new Fixture(theHomeTeam, theAwayTeam, numFixtures * 2, comp);
                        fixtures.Add(newFixture);
                    }
                    
                }
                else
                {
                    break;
                }
            }
            return fixtures;
        }

        private Team findTeam(String team, List<Team> table) 
        {
            for (int i = 0; i < table.Count; i++) 
            {
                Team theTeam = table[i];
                if(team.Equals(theTeam.getName()))
                {
                    return theTeam;
                }
            }
            return null;
        }

        private static String getUrlSource(String url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            String data = "";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }
            return data;
        }

        public List<Fixture> getFixture(String league, int numFixtures, String day, List<Team> table)
        {            
            try
            {
                String pageSource = getUrlSource("http://www.bbc.co.uk/sport/football/" + league + "/fixtures");
                List<Fixture> fixtures = extractFixtures(pageSource, numFixtures, day, table, league);
                return fixtures;
            }
            catch (Exception e)
            {
                Console.WriteLine("SOMETHING WENT WRONG - " + e.Message);
                return new List<Fixture>();
            }

        }

        public List<Team> extractTable(String pageSource, int numTeams)
        {

            List<Team> teams = new List<Team>();

            // Find the position of the first instance of the string "first active" 
            int startPos = pageSource.IndexOf("data-team-slug");

            // Cut all of the data before the start position off
            String cut1 = pageSource.Substring(startPos);

            for (int i = 0; i < numTeams; i++)
            {
                startPos = cut1.IndexOf("position-number");
                String position = cut1.Substring(startPos + 17);
                int endPos = position.IndexOf("<");
                position = position.Substring(0, endPos);

                startPos = cut1.IndexOf("team-name");
                cut1 = cut1.Substring(startPos + 11);

                startPos = cut1.IndexOf(">");
                cut1 = cut1.Substring(startPos+1);

                endPos = cut1.IndexOf("<");
                String teamName = cut1.Substring(0, endPos);
                
                startPos = cut1.IndexOf("last-10-games");
                cut1 = cut1.Substring(startPos);


                List<Result> results = new List<Result>();

                for (int j = 0; j < 10; j++)
                {
                    startPos = cut1.IndexOf("title");
                    cut1 = cut1.Substring(startPos + 7);

                    endPos = cut1.IndexOf("class");
                    String result = cut1.Substring(0, endPos-3);

                    String outcome = result.Substring(0, 4).Trim();

                    int start = result.IndexOf(" v ");
                    int end = result.IndexOf("-");
                    String opposition = result.Substring(start + 3, end - 9).Trim();

                    String score = result.Substring(end -2 , 5);

                    Result theResult = new Result(opposition, outcome, score);
                    results.Add(theResult);

                }

                Team newTeam = new Team(teamName, results, i+1);
                teams.Add(newTeam);
            }

            return teams;
        }

        public List<Team> getTable(String league, int numTeams)
        {
            String pageSource = getUrlSource("http://www.bbc.co.uk/sport/football/" + league + "/table");
            List<Team> fixtures = extractTable(pageSource, numTeams);
            return fixtures;
        }

    }
}
