using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {

    public static PlayerData loadGame;
    Button loadButton;
     
    void Start()
    {
        loadButton = (Button)GameObject.Find("LoadButton").GetComponent<Button>();
        CheckIfLoadExists();
    }

    void CheckIfLoadExists()
    {
        if (!File.Exists(Application.persistentDataPath + "/saveState.gd"))
        {
            loadButton.enabled = false;
            loadButton.image.color = Color.grey;
        }
    }


    public void NewGame()
    {
        loadGame = null;
        SceneManager.LoadScene("main screen");
    }

    public void LoadSavedGame()
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

