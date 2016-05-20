using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.EventSystems;

public class Screen_Script : MonoBehaviour {
    
    public ScreenObject theScreen;
    
    public delegate void ShowBuildingOptions(string screen, string upgrade, Sprite s, int constrDone, int constrTotal);     // delegate call back to Controller
    public static event ShowBuildingOptions showBuildingMenu;

    Popup_Controller popupController;

    Controller theController;       // the instance of Controller to use

    // Use this for initialization
    void Start ()
    {
        // set the Controller up
        theController = GameObject.Find("Central Controller").GetComponent<Controller>();
        popupController = GameObject.Find("PopupController").GetComponent<Popup_Controller>();
    }    

    /// <summary>
    /// When the user clicks on the screen game object
    /// </summary>
    void OnMouseDown()
    {
        // if the object is enabled, the status code = 0, the Business Day is not running and there is not a UI element overlapping the object, the show the menu
        if (GetComponent<Renderer>().enabled && theController.statusCode == 0 && !theController.simulationRunning && !EventSystem.current.IsPointerOverGameObject())
        {
            ShowMenu();
        }
    }
        
    /// <summary>
    /// Show the object info menu
    /// </summary>
    void ShowMenu()
    {
        string screenNum = "Screen " + theScreen.GetScreenNumber();
        string level;

        // get the right message - depending on whether or not construction is taking place
        if (!theScreen.ConstructionInProgress())
        {
            level = "Level " + theScreen.GetUpgradeLevel();
        }
        else
        {
            level = "(Level " + (theScreen.GetUpgradeLevel() - 1) + " >>> Level " + theScreen.GetUpgradeLevel() + ")";
        }

        // get the sprite of the object
        Sprite s = transform.GetComponent<SpriteRenderer>().sprite;

        // calculate days done / needed
        int done = -1;
        int total = -1;

        // if construction is in progress, calculate how many days it will take and how many have been done
        if (theScreen.ConstructionInProgress())
        {
            total = GetUpgradeTime(theScreen.GetUpgradeLevel());
            done = total - theScreen.GetDaysOfConstruction();
        }
        
        // reset status variables
        theController.statusCode = 3;

        popupController.ShowBuildingOptions(screenNum, level, s, done, total);

        theController.objectSelected = name;
        theController.tagSelected = tag;
        theController.upgradeLevelSelected = theScreen.GetUpgradeLevel();
        
    }

    /// <summary>
    /// Get the number of days that construction of the Screen will take
    /// </summary>
    /// <param name="level">The upgrade level of the screen</param>
    /// <returns>The number of days that construction will take</returns>
    public int GetUpgradeTime(int level)
    {
        // get the number of days, based on the level
        if (level == 1)
        {
            return 3;
        }
        else if (level == 2)
        {
            return 2;
        }
        else if (level == 3)
        {
            return 4;
        }
        else
        {
            return 7;
        }
    }

}
