using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class Screen_Script : MonoBehaviour {

    
    public Sprite sprite;

    public ScreenObject theScreen;
    
    public delegate void showBuildingOptions(string screen, string upgrade);
    public static event showBuildingOptions showBuildingMenu;

    Controller theController;

    // Use this for initialization
    void Start ()
    {
        theController = GameObject.Find("Central Controller").GetComponent<Controller>();
    }
    

    void OnMouseDown()
    {
        if (GetComponent<Renderer>().enabled && theController.statusCode == 0 && !theController.simulationRunning)
        {
            ShowMenu();
        }
    }

    
    void ShowMenu()
    {
        if (showBuildingMenu != null)
        {
            string screenNum = "Screen " + theScreen.getScreenNumber();
            string level;

            if (!theScreen.ConstructionInProgress())
            {
                level = "Level " + theScreen.getUpgradeLevel();
            }
            else
            {
                if (theScreen.getUpgradeLevel() > 1)
                {
                    level = "Upgrading: " + theScreen.GetDaysOfConstruction() + " day(s) remaining";
                }
                else
                {
                    level = "Constructing: " + theScreen.GetDaysOfConstruction() + " day(s) remaining";
                }
            }

            showBuildingMenu(screenNum, level);
            theController.statusCode = 3;
            theController.objectSelected = name;
        }
    }
}
