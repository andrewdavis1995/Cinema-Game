using UnityEngine;
using System.Collections;

public class OtherObjectScript : MonoBehaviour {

    static Controller mainController;
    public delegate void showBuildingOptions(string screen, string upgrade, Sprite s, int constrDone, int constrTotal);
    public static event showBuildingOptions showBuildingMenu;
    Transform transform;

    // Use this for initialization
    void Start () {
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
        transform = GameObject.Find(gameObject.name).GetComponent<Transform>(); 
    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public static void CreateStaffSlot(int type, Vector3 pos)
    {
        mainController.staffSlot.Add(Instantiate(mainController.slotPrefab, pos, Quaternion.identity) as Transform);
        mainController.staffSlot[mainController.staffSlot.Count - 1].GetComponent<SpriteRenderer>().enabled = false;
        mainController.staffSlot[mainController.staffSlot.Count - 1].name = "StaffSlot" + (mainController.staffSlot.Count - 1);
        mainController.staffSlot[mainController.staffSlot.Count - 1].tag = "Slot Type " + type;
        mainController.slotState.Add(false);
    }

    public static void UpgradeBoxOffice()
    {
        GameObject[] ticketSlots = GameObject.FindGameObjectsWithTag("Slot Type 1");
        int numSlots = ticketSlots.Length;

        // upgrade the queue
        mainController.ticketQueue.Upgrade();

        // generate a new staff slot
        CreateStaffSlot(1, new Vector3(37.8f + (2.65f * numSlots), 12.5f, 0));

        // update the image - bigger desk, computer system etc
        mainController.boxOfficeLevel++;
        SpriteRenderer sr = GameObject.Find("Box Office").GetComponent<SpriteRenderer>();
        sr.sprite = mainController.boxOfficeImages[mainController.boxOfficeLevel - 1];

        // hide the object info menu
        mainController.hideObjectInfo();


    }

    void OnMouseDown()
    {
        if (GetComponent<Renderer>().enabled && mainController.statusCode == 0 && !mainController.simulationRunning)
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
        else if (tag.Equals("Box Office"))
        {
            message = "Level " + mainController.boxOfficeLevel + "\nCustomers will buy tickets from here";
        }

        if (showBuildingMenu != null)
        {
            Sprite s = transform.GetComponent<SpriteRenderer>().sprite;

            showBuildingMenu(tag, message, s, -1, -1);
            mainController.statusCode = 3;
            mainController.objectSelected = name;
            mainController.tagSelected = tag;
        }
    }

}
