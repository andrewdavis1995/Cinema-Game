using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Assets.Classes
{
    class AddUser
    {

        public void AddTheUser(string id, string name)
        {
            string url = "http://silva.computing.dundee.ac.uk/2015-gamesandrewdavis/AddUser?fbID=" + id + "&username=" + name;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            request.GetResponse();
            
        }

    }
}
