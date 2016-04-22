using UnityEngine;
using System.Collections;

public class FoodAreaScript : MonoBehaviour {

    public int index;
    static Controller mainController;


    void Start()
    {
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
    }

    public void OnMouseDown()
    {
        if (mainController.statusCode == 10)
        {
            bool unlocked = false;
            string component = "";
            int cost = 0;

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

            if (unlocked)
            {
                mainController.ShowPopup(10, "You already purchased the " + component + " Component!");
            }
            else
            {
                mainController.newStatusCode = 10;
                ConfirmationScript.OptionSelected(8, new string[] { "Purchase the " + component + " component?", cost.ToString(), "0", index.ToString() }, "This will cost: ");
            }
        }

    }

    public static void ComponentUnlocked(int index)
    {
        // hide all locked components
        SpriteRenderer[] subImages = GameObject.FindGameObjectWithTag("Food Area").GetComponentsInChildren<SpriteRenderer>();


        // update boolean
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
            Controller.foodArea.tableStatus = 1;
            subImages[4].color = new Color(1, 1, 1, 1);

            Transform t = GameObject.FindGameObjectWithTag("Food Area").transform;

            OtherObjectScript.CreateStaffSlot(2, t.position + new Vector3(5.5f, 7.95f, 0));

            mainController.foodQueue.Upgrade();

        }

        mainController.NewShowTimes();

    }
}
