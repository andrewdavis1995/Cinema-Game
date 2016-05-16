using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System.Net;
using Assets.Classes;
using System;

public class ButtonScript : MonoBehaviour {

    public static PlayerData loadGame;      // the data to be loaded (if load is selected)
    public static PlayerData friendData;      // data to load from the friend's cinema
    public static PlayerData dataCopy;      // data to load from the friend's cinema

    public static string owner;
    
    Button loadButton;      // the button to load a game
    public Image loadImage;
    public GameObject popup;
     
    void Start()
    {
        // find the button for loading
        try
        {
            loadButton = (Button)GameObject.Find("LoadButton").GetComponent<Button>();
        }
        catch (Exception) { }
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
            //loadButton.enabled = false;
            //loadButton.image.color = Color.grey;
        }
    }

    /// <summary>
    /// ET...
    /// </summary>
    public void GoHome()
    {
        Controller.isOwned = true;
        loadGame = dataCopy;
        friendData = null;
        dataCopy = null;
        SceneManager.LoadScene(1);
    }
    
    /// <summary>
    /// Start a new game
    /// </summary>
    public void NewGame()
    {
        // check if game/user exists - check_if_exists - get player data. if not null, give warning. otherwise, create a new user (insert_new) and continue
        if (FBScript.current.id.Length > 0)
        {
            Login l = new Login();
            PlayerData pd = l.DoLogin(FBScript.current.id);

            if (pd != null)
            {
                popup.SetActive(true);
            }
            else
            {
                // add new user - if already in table but just null, then call will fail - which is fine
                AddUser au = new AddUser();
                string id = FBScript.current.id;
                string name = FBScript.current.firstname + " " + FBScript.current.surname;
                au.AddTheUser(id, name);

                SceneManager.LoadScene(1);
            }
        }
        else {

            // if the local file does not exist...
            if (File.Exists(Application.persistentDataPath + "/saveState.gd"))
            {
                popup.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Load a previous game
    /// </summary>
    public void LoadSavedGame()
    {
        // get the details to load
        loadGame = Load();

        if (loadGame != null)
        {
            // open the other scene
            SceneManager.LoadScene("main screen");
        }
    }

    /// <summary>
    /// Load the game details from a file
    /// </summary>
    /// <returns>The player details - to load into the game</returns>
    PlayerData Load()
    {

        loadImage.enabled = true;

        if (FBScript.current.id.Length > 0)
        {
            string fbID = FBScript.current.id;

            Login l = new Login();
            loadGame = l.DoLogin(fbID);

            if (loadGame == null)
            {
                popup.SetActive(true);
            }

            return loadGame;
        }
        else { 
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

        }

        return null;
    }
    
    /// <summary>
    ///  go back to the main menu
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }     
    
    /// <summary>
    ///  view the rules page
    /// </summary>
    public void GoToRules()
    {
        SceneManager.LoadScene(3);
    }

    public void WarningConfirmed()
    {
        popup.SetActive(false);

        string fbID = FBScript.current.id;
        
        loadImage.enabled = true;

        // make the load game details = null
        loadGame = null;
        
        // load the other scene
        SceneManager.LoadScene("main screen");
    }

    public void WarningCancelled()
    {
        popup.SetActive(false);
    }

}

