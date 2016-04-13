using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.EventSystems;

public class Screen_Script : MonoBehaviour {

    Transform transform;

    public ScreenObject theScreen;
    
    public delegate void ShowBuildingOptions(string screen, string upgrade, Sprite s, int constrDone, int constrTotal);
    public static event ShowBuildingOptions showBuildingMenu;

    Controller theController;

    // Use this for initialization
    void Start ()
    {
        theController = GameObject.Find("Central Controller").GetComponent<Controller>();
        transform = GameObject.Find(gameObject.name).GetComponent<Transform>();
    }
    

    void OnMouseDown()
    {
        if (GetComponent<Renderer>().enabled && theController.statusCode == 0 && !theController.simulationRunning && !EventSystem.current.IsPointerOverGameObject())
        {
            ShowMenu();
        }
    }

    
    void ShowMenu()
    {
        if (showBuildingMenu != null)
        {
            string screenNum = "Screen " + theScreen.GetScreenNumber();
            string level;

            if (!theScreen.ConstructionInProgress())
            {
                level = "Level " + theScreen.GetUpgradeLevel();
            }
            else
            {
                level = "(Level " + (theScreen.GetUpgradeLevel() - 1) + " >>> Level " + theScreen.GetUpgradeLevel() + ")";
            }

            Sprite s = transform.GetComponent<SpriteRenderer>().sprite;

            // calculate days done / needed
            int done = -1;
            int total = -1;

            if (theScreen.ConstructionInProgress())
            {
                total = GetUpgradeTime(theScreen.GetUpgradeLevel());
                done = total - theScreen.GetDaysOfConstruction();
            }

            showBuildingMenu(screenNum, level, s, done, total);
            theController.statusCode = 3;
            theController.objectSelected = name;
            theController.tagSelected = tag;
            theController.upgradeLevelSelected = theScreen.GetUpgradeLevel();
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
