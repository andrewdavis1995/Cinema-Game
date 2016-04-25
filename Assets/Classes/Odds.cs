using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace The_Betting_App
{
    public class Odds
    {
        public String getUrlSource(String url)
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



        public String getOdds(String source, String team, String day) 
        {
            try
            {
                team = convertTeamName(team);

                int dayPos = source.IndexOf(day);
                String src = source.Substring(dayPos);

                
                int overallPos = source.IndexOf(team);
                int pos = src.IndexOf(team);

                String split = src.Substring(pos);
                pos = split.IndexOf("odds\">");
                                
                int winOutright = source.IndexOf("Outright");

                bool reachedEnd = false;
                
                if (winOutright > 0 && winOutright < (overallPos + pos)) 
                {
                    reachedEnd = true;  
                }

                if (!reachedEnd)
                {

                    split = split.Substring(pos + 6);
                    pos = split.IndexOf("<");
                    split = split.Substring(0, pos);

                    return split;
                }
                else { return null; }
            }
            catch (Exception) 
            {
                Console.WriteLine(team);
                return null;
            }


        }

        public String convertLeague(String orig)
        {
            String toReturn = "";

            if (orig.Contains("league-"))
            {
                toReturn = orig.Substring(0, orig.Count() - 3);


                String number = orig.Substring(orig.Count() - 3);

                if (number.Equals("one"))
                {
                    toReturn += "1";
                }
                else 
                {
                    toReturn += "2";
                }

                return toReturn;
            }
                
            return orig;
            
        }

        // Different names between BBC and skybet
        private String convertTeamName(String orig) 
        {
            switch (orig) 
            {
                case "Man City":
                    return "Manchester City";
                case "Man Utd":
                    return "Manchester Utd";
                case "Sheff Wed":
                    return "Sheffield Wednesday";
                case "Sheff Utd":
                    return "Sheffield United";
                case "Nottm Forest":
                    return "Nottingham Forest";
                case "Airdrieonians":
                    return "Airdrie";
                case "Queen's Park":
                    return "Queens Park";   // stupid
                case "Raith Rovers":
                    return "Raith";   // stupid
                case "Dag & Red":
                    return "Dagenham &amp; Redbridge";  // okay....
                case "Newport":
                    return "Newport County";
                case "Queen of Sth":
                    return "Queen of South";
                default:
                    return orig;
            }
        }


    }
}
