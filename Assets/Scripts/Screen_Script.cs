using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Screen_Script : MonoBehaviour {

    
    public Sprite sprite;

    public Screen theScreen;
    
    public delegate void showBuildingOptions(int screen, int upgrade);
    public static event showBuildingOptions showBuildingMenu;

    Controller theController;

    // Use this for initialization
    void Start ()
    {
        theController = GameObject.Find("Central Controller").GetComponent<Controller>();
    }


    void OnMouseDown()
    {
        if (GetComponent<Renderer>().enabled && theController.statusCode == 0)
        {
            ShowMenu();
        }
    }

    void ShowMenu()
    {
        if (showBuildingMenu != null)
        {
            showBuildingMenu(theScreen.getScreenNumber(), theScreen.getUpgradeLevel());
            theController.statusCode = 3;
            theController.objectSelected = name;
        }
    }
}
