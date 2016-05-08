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

        public void DoLogin(string id)
        {
            string url = "http://silva.computing.dundee.ac.uk/2015-gamesandrewdavis/Login?fbID=" + id;


            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.Method = "POST";
            //request.ContentType = "text/html;charset=UTF-8";


            //request.se

            // Execute the request

            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //Stream rrr = response.GetResponseStream();

            //StreamReader sr = new StreamReader(rrr);

            //string blobData = sr.ReadToEnd();

            Debug.Log("POO");


            //string ownerName = "";

            //using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            //{

            //    string type = "";

            //    // Parse the file and display each of the nodes.
            //    while (reader.Read())
            //    {


            //        switch (reader.NodeType)
            //        {
            //            case XmlNodeType.Element:
            //                type = reader.Name;
            //                break;
            //            case XmlNodeType.Text:
            //                if (type.Equals("owner"))
            //                {
            //                    ownerName = reader.Value;
            //                }
            //                break;
            //        }
            //    }
            //}

            //Debug.Log(ownerName);













            // write to test



            string ggg = "ÿÿÿÿAssembly-CSharpPlayerDatatheScreenscarpetColourstaffMembersfilmShowingstotalCoinsnumPopcorncurrentDayotherObjectshasRedCarpetmarbleFloorreputationboxOfficeLevelfoodAreapostersScreenObject[]Assets.Classes.SaveableStaff[]FilmShowing[]OtherObject[]Assets.Classes.ReputationAssets.Classes.FoodArea 			@œ			ScreenObject	oƒ:×£°>^º)?€?Assets.Classes.SaveableStaffFilmShowing			OtherObjectAssets.Classes.Reputationoverall publicitystaffspeedfacilitiestotalSpeedValuesnumCustomersServedhighestReputationtotalCoinIncometotalQuestionCountcurrDay	ScreenObject screenNumbercapacityupgradeLevelconstructionInProgressconstructionTimeRemainingprojectorClicksRemainingcurrBrokenCountpointXpointYFilmShowingscreeningID screenNumticketsSoldtimeHtimeMtheFloorAssets.Classes.Floor			Assets.Classes.FloorfloorTileswidthheightAssets.Classes.FloorTile[,] P((PAssets.Classes.FloorTile																	!	\"";


            System.IO.File.WriteAllText(Application.persistentDataPath + "/test.test", ggg);





            // create a Binary Formatter
            BinaryFormatter formatter = new BinaryFormatter();

            // read the contents of the save game file
            FileStream file = File.Open(Application.persistentDataPath + "/test.test", FileMode.Open);
            file.Position = 0;

            // deserialise the data and store it
            PlayerData pd = (PlayerData)formatter.Deserialize(file);


            Debug.Log("dfgfdg");






        }            

    }
}
