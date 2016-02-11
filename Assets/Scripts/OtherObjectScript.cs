using UnityEngine;
using System.Collections;

public class OtherObjectScript : MonoBehaviour {

    Controller theController;
    public delegate void showBuildingOptions(string screen, string upgrade);
    public static event showBuildingOptions showBuildingMenu;

    // Use this for initialization
    void Start () {
        theController = GameObject.Find("Central Controller").GetComponent<Controller>();
    }
	
	// Update is called once per frame
	void Update () {
	
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
            showBuildingMenu(tag, "Useless");
            theController.statusCode = 3;
            theController.objectSelected = name;
        }
    }

}
