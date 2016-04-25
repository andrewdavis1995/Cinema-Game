using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {

    public static PlayerData loadGame;      // the data to be loaded (if load is selected)
    Button loadButton;      // the button to load a game
     
    void Start()
    {
        // find the button for loading
        loadButton = (Button)GameObject.Find("LoadButton").GetComponent<Button>();
        // check if a game has been saved (and is loadable)
        CheckIfLoadExists();
    }

    /// <summary>
    /// Checks if there is a Saved Game available for loading
    /// </summary>
    void CheckIfLoadExists()
    {
        // if a file does not exist...
        if (!File.Exists(Application.persistentDataPath + "/saveState.gd"))
        {
            // disable the load button
            loadButton.enabled = false;
            loadButton.image.color = Color.grey;
        }
    }
    
    /// <summary>
    /// Start a new game
    /// </summary>
    public void NewGame()
    {
        // make the load game details = null
        loadGame = null;
        // load the other scene
        SceneManager.LoadScene("main screen");
    }

    /// <summary>
    /// Load a previous game
    /// </summary>
    public void LoadSavedGame()
    {
        // get the details to load
        loadGame = Load();
        // open the other scene
        SceneManager.LoadScene("main screen");
    }

    /// <summary>
    /// Load the game details from a file
    /// </summary>
    /// <returns>The player details - to load into the game</returns>
    PlayerData Load()
    {
        // if the file exists
        if (File.Exists(Application.persistentDataPath + "/saveState.gd"))
        {
            // create a Binary Formatter
            BinaryFormatter formatter = new BinaryFormatter();

            // read the contents of the save game file
            FileStream file = File.Open(Application.persistentDataPath + "/saveState.gd", FileMode.Open);
            file.Position = 0;

            // deserialise the data and store it
            PlayerData pd = (PlayerData)formatter.Deserialize(file);

            file.Close();

            return pd;
        }

        return null;
    }

    // TEMP
    public void TestStaff()
    {
        SceneManager.LoadScene(2);
    }

}

