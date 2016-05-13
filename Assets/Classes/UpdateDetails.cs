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

            var fileData = Convert.ToBase64String(theBlob, Base64FormattingOptions.InsertLineBreaks);

            // database stuff
            string url = "http://silva.computing.dundee.ac.uk/2015-gamesandrewdavis/SaveGame";
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.SendChunked = true;
            request.Method = "POST";

            request.ContentType = "text/xml;charset=UTF-8";
            //request.ContentLength = theBlob.Length;
           
            
            //UTF8Encoding encodedData = new UTF8Encoding();
            //byte[] byteArray = encodedData.GetBytes(postData);


            Stream newStream = request.GetRequestStream();
            
            string byteput = "theBlob=";

            byte[] chars = Encoding.GetEncoding("UTF-8").GetBytes(byteput.ToCharArray());

            id = id + "%26";

            byte[] idThings = Encoding.GetEncoding("UTF-8").GetBytes(id.ToCharArray());

            byte[] blobBytes = Encoding.GetEncoding("UTF-8").GetBytes(fileData.ToCharArray());

            

            IEnumerable<byte> rwerwer = theBlob.Take(150);

            byte[] result = rwerwer.ToArray();



            byte[] toSend = new byte[id.Length + byteput.Length + blobBytes.Length];
            chars.CopyTo(toSend, 0);
            idThings.CopyTo(toSend, chars.Length);
            blobBytes.CopyTo(toSend, chars.Length + idThings.Length);


            IEnumerable<byte> ewfewfewfewf = toSend.Take(150);

            byte[] result2222222 = ewfewfewfewf.ToArray();


            newStream.Write(toSend, 0, toSend.Length);

            //newStream.Close();
            
            //System.IO.File.WriteAllBytes("C:/Users/asuth/Documents/baba.txt", theBlob);
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream rrr = response.GetResponseStream();

            StreamReader sr = new StreamReader(rrr);

            string blobData = sr.ReadToEnd();
            
            Debug.Log("POOADFADFADFADF");
            Debug.Log(blobData);

        }

    }
}


// ÿÿÿÿAssembly-CSharpPlayerDatatheScreenscarpetColourstaffMembersfilmShowingstotalCoinsnumPopcorncurrentDayotherObjectshasRedCarpetmarbleFloorreputationboxOfficeLevelfoodAreapostersScreenObject[]Assets.Classes.SaveableStaff[]FilmShowing[]OtherObject[]Assets.Classes.ReputationAssets.Classes.FoodArea 			@œ			ScreenObject	oƒ:×£°>^º)?€?Assets.Classes.SaveableStaffFilmShowing			OtherObjectAssets.Classes.Reputationoverall publicitystaffspeedfacilitiestotalSpeedValuesnumCustomersServedhighestReputationtotalCoinIncometotalQuestionCountcurrDay	ScreenObject screenNumbercapacityupgradeLevelconstructionInProgressconstructionTimeRemainingprojectorClicksRemainingcurrBrokenCountpointXpointYFilmShowingscreeningID screenNumticketsSoldtimeHtimeMtheFloorAssets.Classes.Floor			Assets.Classes.FloorfloorTileswidthheightAssets.Classes.FloorTile[,] P((PAssets.Classes.FloorTile																	!	"