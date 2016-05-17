using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Assets.Classes
{
    class Gifting
    {
        public void SendGift(string id, string recID)
        {
            // database stuff
            string url = "http://silva.computing.dundee.ac.uk/2015-gamesandrewdavis/AddGifts?fbID=" + id + "&recID=" + recID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.SendChunked = true;
            request.Method = "POST";
            request.Timeout = 90000;
            request.ContentType = "text/xml;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }


        public void GetGifts(string id)
        {
            // database stuff
            string url = "http://silva.computing.dundee.ac.uk/2015-gamesandrewdavis/GetGift/fbID=" + id;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.SendChunked = true;
            request.Method = "POST";
            request.Timeout = 90000;
            request.ContentType = "text/xml;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();


            // xml stuff here


        }

    }
}
