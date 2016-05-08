using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Assets.Classes
{
    class UpdateDetails
    {

        public void DoUpdate(string id, byte[] theBlob)
        {
            
            int blobLength = theBlob.Length;
            string blobString = System.Text.Encoding.Default.GetString(theBlob);

            // database stuff
            string url = "http://silva.computing.dundee.ac.uk/2015-gamesandrewdavis/SaveGame?fbID=" + id + "&theBlob=" + blobString;
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml;charset=UTF-8";


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        }

    }
}


// ÿÿÿÿAssembly-CSharpPlayerDatatheScreenscarpetColourstaffMembersfilmShowingstotalCoinsnumPopcorncurrentDayotherObjectshasRedCarpetmarbleFloorreputationboxOfficeLevelfoodAreapostersScreenObject[]Assets.Classes.SaveableStaff[]FilmShowing[]OtherObject[]Assets.Classes.ReputationAssets.Classes.FoodArea 			@œ			ScreenObject	oƒ:×£°>^º)?€?Assets.Classes.SaveableStaffFilmShowing			OtherObjectAssets.Classes.Reputationoverall publicitystaffspeedfacilitiestotalSpeedValuesnumCustomersServedhighestReputationtotalCoinIncometotalQuestionCountcurrDay	ScreenObject screenNumbercapacityupgradeLevelconstructionInProgressconstructionTimeRemainingprojectorClicksRemainingcurrBrokenCountpointXpointYFilmShowingscreeningID screenNumticketsSoldtimeHtimeMtheFloorAssets.Classes.Floor			Assets.Classes.FloorfloorTileswidthheightAssets.Classes.FloorTile[,] P((PAssets.Classes.FloorTile																	!	"