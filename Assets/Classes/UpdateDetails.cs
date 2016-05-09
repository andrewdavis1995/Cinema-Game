using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

namespace Assets.Classes
{
    class UpdateDetails
    {

        public void DoUpdate(string id, byte[] theBlob)
        {
            
            int blobLength = theBlob.Length;
            string blobString = System.Text.Encoding.Default.GetString(theBlob, 0, blobLength);
            string thrh = Encoding.UTF8.GetString(theBlob);
            string bobTheBlob = Convert.ToString(theBlob);
            var fileData = Convert.ToBase64String(theBlob, Base64FormattingOptions.InsertLineBreaks);
            string response3 = System.Text.Encoding.ASCII.GetString(theBlob);
            string response2 = System.Text.Encoding.UTF8.GetString(theBlob);

            int count = fileData.Length;

            Debug.Log(fileData.Length);

            // database stuff
            string url = "http://silva.computing.dundee.ac.uk/2015-gamesandrewdavis/SaveGame?fbID=" + id + "&theBlob=\"" + fileData + "\"";
            


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml;charset=UTF-8";


            request.GetRequestStream().Write(theBlob, 0, theBlob.Length);




            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Debug.Log("POO");

        }

    }
}


// ÿÿÿÿAssembly-CSharpPlayerDatatheScreenscarpetColourstaffMembersfilmShowingstotalCoinsnumPopcorncurrentDayotherObjectshasRedCarpetmarbleFloorreputationboxOfficeLevelfoodAreapostersScreenObject[]Assets.Classes.SaveableStaff[]FilmShowing[]OtherObject[]Assets.Classes.ReputationAssets.Classes.FoodArea 			@œ			ScreenObject	oƒ:×£°>^º)?€?Assets.Classes.SaveableStaffFilmShowing			OtherObjectAssets.Classes.Reputationoverall publicitystaffspeedfacilitiestotalSpeedValuesnumCustomersServedhighestReputationtotalCoinIncometotalQuestionCountcurrDay	ScreenObject screenNumbercapacityupgradeLevelconstructionInProgressconstructionTimeRemainingprojectorClicksRemainingcurrBrokenCountpointXpointYFilmShowingscreeningID screenNumticketsSoldtimeHtimeMtheFloorAssets.Classes.Floor			Assets.Classes.FloorfloorTileswidthheightAssets.Classes.FloorTile[,] P((PAssets.Classes.FloorTile																	!	"