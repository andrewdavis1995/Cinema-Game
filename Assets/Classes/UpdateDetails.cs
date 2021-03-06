﻿using System;
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
            request.Timeout = 90000;
            request.ContentType = "text/xml;charset=UTF-8";

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
            
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream rrr = response.GetResponseStream();

            StreamReader sr = new StreamReader(rrr);

            string blobData = sr.ReadToEnd();
            
            response.Close();

        }

    }
}
