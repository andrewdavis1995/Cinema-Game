using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class Screen_Script : MonoBehaviour {

    Transform transform;

    public ScreenObject theScreen;
    
    public delegate void showBuildingOptions(string screen, string upgrade, Sprite s, int constrDone, int constrTotal);
    public static event showBuildingOptions showBuildingMenu;

    Controller theController;

    // Use this for initialization
    void Start ()
    {
        theController = GameObject.Find("Central Controller").GetComponent<Controller>();
        transform = GameObject.Find(gameObject.name).GetComponent<Transform>();
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

            Sprite s = transform.GetComponent<SpriteRenderer>().sprite;

            // calculate days done / needed
            int done = -1;
            int total = -1;

            if (theScreen.ConstructionInProgress())
            {
                total = GetUpgradeTime(theScreen.getUpgradeLevel());
                done = total - theScreen.GetDaysOfConstruction();
            }

            showBuildingMenu(screenNum, level, s, done, total);
            theController.statusCode = 3;
            theController.objectSelected = name;
        }
    }

    public int GetUpgradeTime(int level)
    {
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
