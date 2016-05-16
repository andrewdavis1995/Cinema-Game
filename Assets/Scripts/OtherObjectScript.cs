using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class OtherObjectScript : MonoBehaviour {

    static Controller mainController;       // the instance of Controller to use
    public delegate void ShowBuildingOptions(string screen, string upgrade, Sprite s, int constrDone, int constrTotal); //      delegate call back to Controller
    public static event ShowBuildingOptions showBuildingMenu;
    
    static Popup_Controller popupController;

    // Use this for initialization
    void Start () {
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
        popupController = GameObject.Find("PopupController").GetComponent<Popup_Controller>();
    }
	
    /// <summary>
    /// Create a new Slot to drag staff members into
    /// </summary>
    /// <param name="type">The id for which type of slot to make (1 = Ticket, 2 = Food)</param>
    /// <param name="pos">A vector of the position in which to place the slot</param>
    public static void CreateStaffSlot(int type, Vector3 pos)
    {
        // instantiate a new slot and name/tag/disable it
        mainController.staffSlot.Add(Instantiate(mainController.slotPrefab, pos, Quaternion.identity) as Transform);
        mainController.staffSlot[mainController.staffSlot.Count - 1].GetComponent<SpriteRenderer>().enabled = false;
        mainController.staffSlot[mainController.staffSlot.Count - 1].name = "StaffSlot" + (mainController.staffSlot.Count - 1);
        mainController.staffSlot[mainController.staffSlot.Count - 1].tag = "Slot Type " + type;
        // initialise the slot to not be in use
        mainController.slotState.Add(false);
    }

    /// <summary>
    /// Upgrades the box office - add 1 staff slot and update image
    /// </summary>
    public static void UpgradeBoxOffice()
    {
        // get all the slots of type ticket
        GameObject[] ticketSlots = GameObject.FindGameObjectsWithTag("Slot Type 1");
        int numSlots = ticketSlots.Length;
        
        // upgrade the queue
        mainController.ticketQueue.Upgrade();

        // generate a new staff slot
        CreateStaffSlot(1, new Vector3(37.8f + (2.65f * numSlots), 12.3f, 0));

        // update the image - bigger desk, computer system etc
        mainController.boxOfficeLevel++;
        SpriteRenderer sr = GameObject.Find("Box Office").GetComponent<SpriteRenderer>();
        sr.sprite = mainController.boxOfficeImages[mainController.boxOfficeLevel - 1];

        // hide the object info menu
        popupController.HideObjectInfo();


    }

    /// <summary>
    /// When the user clicks on the Object
    /// </summary>
    void OnMouseDown()
    {
        // if the object is enabled, the status code = 0, the Business Day is not running and there is not a UI element overlapping the object, the show the menu
        if (GetComponent<Renderer>().enabled && mainController.statusCode == 0 && !mainController.simulationRunning && !EventSystem.current.IsPointerOverGameObject())
        {
            ShowMenu();
        }
    }

    /// <summary>
    /// Show the object info menu
    /// </summary>
    void ShowMenu()
    {
        #region Get Message
        // based on the tag of the object, get the relevant message to display
        string message = "No purpose";
        if (tag.Equals("Bust"))
        {
            message = "Thanks for playing!";
        }
        else if (tag.Equals("Box Office"))
        {
            message = "Level " + mainController.boxOfficeLevel + "\nCustomers will buy tickets from here";
        }
        else if (tag.Equals("Food Area"))
        {
            message = "";
        }
        #endregion
        
        // get the object sprite
        Sprite s = transform.GetComponent<SpriteRenderer>().sprite;

        // TODO: if box office or food court, change the sprite

        // reset the status variables
        mainController.statusCode = 3;

        //call the delegate
        popupController.ShowBuildingOptions(tag, message, s, -1, -1);

        mainController.objectSelected = name;
        mainController.tagSelected = tag;
        
    }

}
