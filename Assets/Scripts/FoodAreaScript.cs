using UnityEngine;
using System.Collections;

public class FoodAreaScript : MonoBehaviour {

    public int index;
    Controller mainController;


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

            if (index == 0)
            {
                unlocked = Controller.foodArea.hasHotFood;
                component = "Hot Food";
            }
            else if (index == 1)
            {
                unlocked = Controller.foodArea.hasPopcorn;
                component = "Popcorn";
            }
            else if (index == 2)
            {
                unlocked = Controller.foodArea.hasIceCream;
                component = "Ice Cream";
            }

            if (unlocked)
            {
                mainController.ShowPopup(10, "You already purchased the " + component + " Component!");
            }
            else
            {
                // hide all locked components
                SpriteRenderer[] subImages = GameObject.FindGameObjectWithTag("Food Area").GetComponentsInChildren<SpriteRenderer>();

                // TODO: varying costs
                ConfirmationScript.OptionSelected(8, new string[] { "Purchase the " + component + "component?", "600", "0", index.ToString() }, "This will cost: ");

                #region Move this to the method whcih is called once confirmed
                // update boolean
                if (index == 1)
                {
                    Controller.foodArea.hasPopcorn = true;
                    subImages[2].color = new Color(1, 1, 1, 1);
                }
                else if (index == 2)
                {
                    Controller.foodArea.hasIceCream = true;
                    subImages[3].color = new Color(1, 1, 1, 1);
                }
                #endregion

            }
        }

    }
}
