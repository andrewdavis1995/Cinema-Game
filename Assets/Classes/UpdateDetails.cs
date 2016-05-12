using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO.Compression;
using UnityEngine;
using System.IO;

namespace Assets.Classes
{
    class UpdateDetails
    {

        public void DoUpdate(string id, byte[] theBlob)
        {

            //var fileData = Convert.ToBase64String(theBlob, Base64FormattingOptions.InsertLineBreaks);

            // database stuff
            string url = "http://silva.computing.dundee.ac.uk/2015-gamesandrewdavis/SaveGame/fbID=" + id;
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.SendChunked = true;
            request.Method = "POST";

            request.ContentType = "multipart/form-data";
            //request.ContentLength = theBlob.Length;
           
            
            //UTF8Encoding encodedData = new UTF8Encoding();
            //byte[] byteArray = encodedData.GetBytes(postData);


            Stream newStream = request.GetRequestStream();
            newStream.Write(theBlob, 0, theBlob.Length);
            newStream.Close();


            System.IO.File.WriteAllBytes("C:/Users/asuth/Documents/baba.txt", theBlob);

            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            

            Debug.Log("POO");

        }

    }
}


// ÿÿÿÿAssembly-CSharpPlayerDatatheScreenscarpetColourstaffMembersfilmShowingstotalCoinsnumPopcorncurrentDayotherObjectshasRedCarpetmarbleFloorreputationboxOfficeLevelfoodAreapostersScreenObject[]Assets.Classes.SaveableStaff[]FilmShowing[]OtherObject[]Assets.Classes.ReputationAssets.Classes.FoodArea 			@œ			ScreenObject	oƒ:×£°>^º)?€?Assets.Classes.SaveableStaffFilmShowing			OtherObjectAssets.Classes.Reputationoverall publicitystaffspeedfacilitiestotalSpeedValuesnumCustomersServedhighestReputationtotalCoinIncometotalQuestionCountcurrDay	ScreenObject screenNumbercapacityupgradeLevelconstructionInProgressconstructionTimeRemainingprojectorClicksRemainingcurrBrokenCountpointXpointYFilmShowingscreeningID screenNumticketsSoldtimeHtimeMtheFloorAssets.Classes.Floor			Assets.Classes.FloorfloorTileswidthheightAssets.Classes.FloorTile[,] P((PAssets.Classes.FloorTile																	!	"