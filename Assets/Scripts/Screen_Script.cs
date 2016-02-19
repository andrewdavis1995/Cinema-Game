using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class Screen_Script : MonoBehaviour {

    
    public Sprite sprite;

    public Screen theScreen;
    
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

    void OnMouseDrag()
    {
        theController.statusCode = 0;
        theController.objectInfo.SetActive(false);
        theController.closeInfo.SetActive(false);
    }

    void ShowMenu()
    {
        if (showBuildingMenu != null)
        {
            showBuildingMenu("Screen " + theScreen.getScreenNumber(), "Level " + theScreen.getUpgradeLevel());
            theController.statusCode = 3;
            theController.objectSelected = name;
        }
    }
}
