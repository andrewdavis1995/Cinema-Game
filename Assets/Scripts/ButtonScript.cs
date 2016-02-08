using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ButtonScript : MonoBehaviour {

    public static PlayerData loadGame;

    public void newGame()
    {
        loadGame = null;
        SceneManager.LoadScene("main screen");
    }

    public void loadSavedGame()
    {
        loadGame = Load();
        SceneManager.LoadScene("main screen");
    }


    PlayerData Load()
    {
        if (File.Exists(Application.persistentDataPath + "/saveState.gd"))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/saveState.gd", FileMode.Open);
            file.Position = 0;

            PlayerData pd = (PlayerData)formatter.Deserialize(file);
            //SaveLoadScript.savedGames = (List<Controller>)formatter.Deserialize(file);
            file.Close();

            return pd;
        }

        return null;
    }

}

