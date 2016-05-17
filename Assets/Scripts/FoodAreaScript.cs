using UnityEngine;
using System.Collections;

public class FoodAreaScript : MonoBehaviour {

    public int index;       // which component the script represents (0 = hot food, 1 = popcorn, 2 = ice-cream, -1 = unassigned)
    static Controller mainController;       // the Controller instance to read from

    public Popup_Controller popupController;

    // called once at startup
    void Start()
    {
        // find and set the controller
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
        popupController = GameObject.Find("PopupController").GetComponent<Popup_Controller>();
    }

    /// <summary>
    /// When the user clicks on one of the components
    /// </summary>
    public void OnMouseDown()
    {
        // if the status code is 10 - i.e. in the Upgrade menu
        if (mainController.statusCode == 10)
        {
            bool unlocked = false;
            string component = "";
            int cost = 0;

            // get the necessary values based on which component was selected
            #region Get values
            if (index == 0)
            {
                unlocked = Controller.foodArea.hasHotFood;
                component = "Hot Food";
                cost = 800;
            }
            else if (index == 1)
            {
                unlocked = Controller.foodArea.hasPopcorn;
                component = "Popcorn";
                cost = 2500;
            }
            else if (index == 2)
            {
                unlocked = Controller.foodArea.hasIceCream;
                component = "Ice Cream";
                cost = 1400;
            }
            else if (index == 3)
            {
                unlocked = Controller.foodArea.tableStatus == 1;
                component = "Bigger Table";
                cost = 5000;
            }
            #endregion

            // if the item has already been unlocked, tell the user
            if (unlocked)
            {
                popupController.ShowPopup(10, "You already purchased the " + component + " Component!");
            }
            // otherwise, ask them to confirm their purchase
            else
            {
                mainController.newStatusCode = 10;
                ConfirmationScript.OptionSelected(8, new string[] { "Purchase the " + component + " component?", cost.ToString(), "0", index.ToString() }, "This will cost: ");
            }
        }

    }

    /// <summary>
    /// When the user chooses to unlock a new component or upgrade the table
    /// </summary>
    /// <param name="index">The index of the item to be unlocked (0 = hot food, 1 = popcorn, 2 = ice-cream)</param>
    public static void ComponentUnlocked(int index)
    {
        // find all the sub-images of Food Area
        SpriteRenderer[] subImages = GameObject.FindGameObjectWithTag("Food Area").GetComponentsInChildren<SpriteRenderer>();

        // based on the index selected, update the status of each component and change the color of the image
        #region Set Variables and Images       
        if (index == 0)
        {
            Controller.foodArea.hasHotFood = true;
            subImages[0].color = new Color(1, 1, 1, 1);
        }
        else if (index == 1)
        {
            Controller.foodArea.hasPopcorn = true;
            subImages[2].color = new Color(1, 1, 1, 1);
        }
        else if (index == 2)
        {
            Controller.foodArea.hasIceCream = true;
            subImages[3].color = new Color(1, 1, 1, 1);
        }
        else if (index == 3)
        {
            // update the table state
            Controller.foodArea.tableStatus = 1;
            subImages[4].color = new Color(1, 1, 1, 1);
            
            // create a new staff slot, in a position relative to the Food area
            Transform t = GameObject.FindGameObjectWithTag("Food Area").transform;
            OtherObjectScript.CreateStaffSlot(2, t.position + new Vector3(5.5f, 7.95f, 0));

            // add an extra serving slot to the queue
            mainController.foodQueue.Upgrade();

        }
        #endregion

        mainController.NewShowTimes();
    }
}
