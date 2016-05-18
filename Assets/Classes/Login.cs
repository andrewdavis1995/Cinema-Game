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
    class Login
    {

        public PlayerData DoLogin(string id)
        {
            string url = "http://silva.computing.dundee.ac.uk/2015-gamesandrewdavis/Login?fbID=" + id;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml;charset=UTF-8";
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream rrr = response.GetResponseStream();

            StreamReader sr = new StreamReader(rrr);

            string blobData = sr.ReadToEnd();

            XmlDocument document = new XmlDocument();
            try
            {
                document.LoadXml(blobData);
            }
            catch (Exception) { return null; }

            XmlNodeList cinemaData = document.GetElementsByTagName("data");

            string code = cinemaData[0].InnerText;


            response.Close();


            int mod4 = code.Length % 4;

            if (mod4 > 0)
            {
                code += new string('=', 4-mod4);
            }

            
            byte[] bytes = System.Convert.FromBase64String(code);
            //System.IO.File.WriteAllBytes(Application.persistentDataPath + "/test.test", bytes);


            //System.IO.StreamWriter file2 = new System.IO.StreamWriter("C:/Users/asuth/Documents/test.icles", false);

            //StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/test.icles");
            //sw.Write(bytes);

            //file2.Write(bytes);


            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/test.icles", bytes);

            //sw.Close();
            //file2.Close();


            // create a Binary Formatter
            BinaryFormatter formatter = new BinaryFormatter();

            // read the contents of the save game file
            //FileStream file = File.Open(Application.persistentDataPath + "/test.icles", FileMode.Open);
            FileStream file = File.Open(Application.persistentDataPath + "/test.icles", FileMode.Open);
            file.Position = 0;
            
            // deserialise the data and store it
            try {
                PlayerData pd = (PlayerData)formatter.Deserialize(file);
                
                return pd;
            }
            catch (Exception ex)
            {
                return null;
            }
        }            

    }
}
