using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public static class SaveLoadScript{

    public static List<Controller> savedGames = new List<Controller>();

    public static void Save()
    {
        //SaveLoadScript.savedGames.Add(Controller.theGame);
        //BinaryFormatter formatter = new BinaryFormatter();
        //FileStream file = File.Create(Application.persistentDataPath + "/savedGames.tbs");
        //formatter.Serialize(file, SaveLoadScript.savedGames);

        //file.Position = 0;
        //file.Close();
    }

    public static void Load()
    {
       
        if (File.Exists(Application.persistentDataPath + "/saveState.gd"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            
            FileStream file = File.Open(Application.persistentDataPath + "/saveState.gd", FileMode.Open);
            file.Position = 0;

            if (file == null)
            {
                Debug.Log("FUCK YOUR SHIT");
                return;
            }            


            //SaveLoadScript.savedGames = (List<Controller>)formatter.Deserialize(file);
            file.Close();
        }

        for (int i = 0; i < savedGames.Count; i++)
        {
            Debug.Log(savedGames[i].carpetColour);
        }
    }

}
