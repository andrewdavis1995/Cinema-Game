using UnityEngine;
using System.Collections;

public class OtherObjectScript : MonoBehaviour {

    Controller theController;
    public delegate void showBuildingOptions(string screen, string upgrade, Sprite s);
    public static event showBuildingOptions showBuildingMenu;
    Transform transform;

    // Use this for initialization
    void Start () {
        theController = GameObject.Find("Central Controller").GetComponent<Controller>();
        transform = GameObject.Find(gameObject.name).GetComponent<Transform>(); 
    }
	
	// Update is called once per frame
	void Update () {
	
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
        string message = "No purpose";
        if (tag.Equals("Bust of Game Creator"))
        {
            message = "Thanks for playing!";
        }

        if (showBuildingMenu != null)
        {

            Sprite s = transform.GetComponent<SpriteRenderer>().sprite;

            showBuildingMenu(tag, message, s);
            theController.statusCode = 3;
            theController.objectSelected = name;
        }
    }

}
