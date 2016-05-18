using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Assets.Classes
{
    class Gifting
    {
        public bool SendGift(string id, string recID)
        {
            // database stuff
            string url = "http://silva.computing.dundee.ac.uk/2015-gamesandrewdavis/AddGifts?fbID=" + id + "&recID=" + recID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.SendChunked = true;
            request.Method = "POST";
            request.Timeout = 90000;
            request.ContentType = "text/xml;charset=UTF-8";

            try {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        public List<String> GetGifts(string id)
        {

            List<string> gifts = new List<string>();


            // database stuff
            string url = "http://silva.computing.dundee.ac.uk/2015-gamesandrewdavis/GetGifts?fbID=" + id;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.SendChunked = true;
            request.Method = "POST";
            request.Timeout = 90000;
            request.ContentType = "text/xml;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // xml stuff here
            Stream rrr = response.GetResponseStream();
            StreamReader sr = new StreamReader(rrr);
            string giftData = sr.ReadToEnd();

            XmlDocument document = new XmlDocument();
            try
            {
                document.LoadXml(giftData);
            }
            catch (Exception) {
                response.Close();
                return null;                
            }

            int count = 0;

            try
            {
                do
                {
                    int posStart = giftData.IndexOf("sender" + count);
                    int posEnd = giftData.IndexOf("/sender" + count);

                    if (posStart >= posEnd) { break; }

                    string newGift = giftData.Substring(posStart + 8, posEnd - posStart - 9);
                    String friendName = "";

                    foreach (FacebookFriend ff in FBScript.current.friendList)
                    {
                        if (ff.id.Equals(newGift))
                        {
                            friendName = ff.name;
                            break;
                        }
                    }

                    gifts.Add(friendName);

                    giftData = giftData.Substring(posStart + 7);

                    count++;

                } while (true);
            }catch (Exception) {  }


            return gifts;
        }
    }
}
