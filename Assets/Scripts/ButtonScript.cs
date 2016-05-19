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
    }
    
    /// <summary>
    /// Screw you guys...
    /// </summary>
    public void GoHome()
    {
        Controller.isOwned = true;
        Controller.friendID = "";
        loadGame = dataCopy;
        friendData = null;
        dataCopy = null;
        Controller.numPopcorn = 0;
        ButtonScript.friendData = null;
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
                Text[] t = popup.GetComponentsInChildren<Text>();
                t[2].text = "Sorry, No Data found. Start a new game?";
            }
            else
            {
                // add new user - if already in table but just null, then call will fail - which is fine
                AddUser au = new AddUser();
                string id = FBScript.current.id;
                string name = FBScript.current.firstname + " " + FBScript.current.surname;
                au.AddTheUser(id, name);

                loadImage.enabled = true;
                SceneManager.LoadScene(1);
            }
        }
        else {

            // if the local file does exist...
            if (File.Exists(Application.persistentDataPath + "/saveState.gd"))
            {
                popup.SetActive(true);
                Text[] t = popup.GetComponentsInChildren<Text>();
                t[2].text = "Warning!\nStarting a new game will automatically overwrite the existing saved game.\n\nDo you wish to continue?";
            }
            else
            {
                loadImage.enabled = true;
                SceneManager.LoadScene(1);
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
            loadImage.enabled = true;
            SceneManager.LoadScene("main screen");
        }
    }

    /// <summary>
    /// Load the game details from a file
    /// </summary>
    /// <returns>The player details - to load into the game</returns>
    PlayerData Load()
    {

        if (FBScript.current.id.Length > 0)
        {
            string fbID = FBScript.current.id;

            Login l = new Login();
            loadGame = l.DoLogin(fbID);

            if (loadGame == null)
            {
                popup.SetActive(true);
                Text[] t = popup.GetComponentsInChildren<Text>();
                t[2].text = "Sorry, No Data found. Start a new game?";
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
        loadImage.enabled = true;
        SceneManager.LoadScene(3);
    }

    public void WarningConfirmed()
    {
        popup.SetActive(false);

        loadImage.enabled = true;
        SceneManager.LoadScene(1);
    }

    public void WarningCancelled()
    {
        popup.SetActive(false);
    }

}

