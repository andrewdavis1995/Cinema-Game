using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Classes;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    Controller mainController;      // the instance of Controller to use
    ShopController shopController;      
    GameObject closeBtn;            // the button to close the shop
    public GameObject[] shopTabs;   // the tabs for the shop

    public Popup_Controller popupController;

    // used to initialise the variables
    void Start()
    {
        // find and set the variables
        closeBtn = GameObject.Find("Close Shop");
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
        shopController = GameObject.Find("ShopController").GetComponent<ShopController>();
    }

    /// <summary>
    /// The user has picked which item to buy
    /// </summary>
    /// <param name="id">The id of the item to purchase (0 = Screen, 1 = Staff, 2 = Plant, 3 = Bust, 4 = Popcorn, 5 = Vending Machine, 6 = Red Carpet, 7 = Food Area)</param>
    public void Purchase(int id)
    {
        // set the status variable
        mainController.itemToAddID = id;

        // based on the id, carry out the necessary action
        #region Do Actions
        switch (id)
        {
            #region Screen
            case 0:

                if (ShopController.theScreens.Count < 8)
                {
                    // go to placement of item
                    mainController.ShopItemSelected(11, 15, "SCREEN");
                    // 'ghost' other objects
                    shopController.SemiTransparentObjects();
                }
                else
                {
                    popupController.ShowPopup(5, "Sorry, you have reached the maximum number of Screens permitted (8)");
                }
                break;
            #endregion

            #region Staff
            case 1:
                // Confirm that the user wants to hire a new staff member
                if (mainController.staffMembers.Count < 18)
                {
                    ConfirmationScript.OptionSelected(2, new string[] { "hire a new Staff Member?", "800", "0" }, "This will cost: ");
                }
                else
                {
                    popupController.ShowPopup(5, "Sorry. You are only allowed to have 18 Staff Members hired.");
                }
                break;
            #endregion

            #region Plant
            case 2: // Plant
                // go to placement of item
                mainController.ShopItemSelected(1, 1, "PLANT");
                // ghost other objects
                shopController.SemiTransparentObjects();
                break;
            #endregion

            #region Bust
            case 3: // Bust
                // go to placement of item
                mainController.ShopItemSelected(2, 3, "BUST");
                // ghost other objects
                shopController.SemiTransparentObjects();
                break;
            #endregion

            #region Popcorns
            case 4: // 5 Popcorns
                // micro-transactions - MONEY!
                break;
            #endregion

            #region Vending Machine
            case 5: // Vending Machine
                // go to placement of item
                mainController.ShopItemSelected(3, 3, "VENDING MACHINE");
                // ghost other objects
                shopController.SemiTransparentObjects();
                break;
            #endregion

            #region Red Carpet
            case 6: // Red Carpet
                // check that it has not already been purchased
                if (!shopController.hasUnlockedRedCarpet)
                {
                    ConfirmationScript.OptionSelected(4, new string[] { " unlock the Red Carpet?", "800", "0" }, "This will cost: ");
                }
                else
                {
                    // inform the user that the Red Carpet has already been purchased
                    Debug.Log("Already Got it you silly sausage!");
                    popupController.ShowPopup(5, "You already own this object!");
                }
                break;
            #endregion

            #region Food Area
            case 7: // food area
                // check that the food area has not already been purchased
                if (Controller.foodArea == null)
                {
                    // go to placement of item
                    mainController.ShopItemSelected(10, 18, "FOOD AREA");
                    // 'ghost' the other objects
                    shopController.SemiTransparentObjects();

                }
                else
                {
                    // inform the user that they have already purchased the Food Area
                    Debug.Log("Already Got it you stupid sausage!");
                    popupController.ShowPopup(5, "You already own this object!");
                }
                break;
            #endregion

            #region Posters 1
            case 8: // posters 1
                if (!shopController.postersUnlocked[0])
                {
                    ConfirmationScript.OptionSelected(9, new string[] { "buy this poster pack", "750", "0" }, "This will cost: ");
                }
                else
                {
                    popupController.ShowPopup(5, "You already own this item!");
                }
                break;
            #endregion

            #region Posters 2
            case 9: // posters 2
                if (!shopController.postersUnlocked[1])
                {
                    ConfirmationScript.OptionSelected(10, new string[] { "buy this poster pack", "750", "0" }, "This will cost: ");
                }
                else
                {
                    popupController.ShowPopup(5, "You already own this item!");
                }
                break;
            #endregion

            #region Coin bundle (8000)
            case 10: // posters 2
                ConfirmationScript.OptionSelected(11, new string[] { "buy this bundle?", "6", "1" }, "This will cost: ");
                break;
                #endregion
        }
        #endregion
    }

    /// <summary>
    /// Moves the close button back to the top layer after a new tab is selected
    /// </summary>
    void PutCloseToTop()
    {
        closeBtn.GetComponent<RectTransform>().SetAsLastSibling();
        closeBtn.GetComponent<RectTransform>().GetSiblingIndex();
    }

    /// <summary>
    /// A tab in the shop is clicked
    /// </summary>
    /// <param name="index">Represents which tab to open (0 = Items, 1 = Decorations, 2 = Extras)</param>
    public void TabChanged(int index)
    {
        // move the relevant 'Pane' to the top
        shopTabs[index].GetComponent<Transform>().SetAsLastSibling();

        // change the colour of the tab which is open
        for (int i = 0; i < shopTabs.Length; i++)
        {
            Image[] images = shopTabs[i].GetComponentsInChildren<Image>();

            if (shopTabs[i].name.Equals(shopTabs[index].name))
            {
                images[1].color = new Color(1, 0.310f, 0);
            }
            else
            {
                images[1].color = new Color(0.529f, 0, 0);
            }

        }

        // put the close buttom above all 'panes'
        PutCloseToTop();
    }

    /// <summary>
    /// Shows the benefits of purchasing an item
    /// </summary>
    /// <param name="index">The id of the item being purchased (0 = Screen, 1 = Staff, 2 = Vending Machine, 3 = Food Area)</param>
    public void ViewItemBenefits(int index)
    {
        // get the benefit of the object
        string benefit = GetBenefitString(index);
        
        // display the benefit in a popup
        popupController.ShowPopup(5, benefit);
    }

    /// <summary>
    /// Gets a string which dislays the benefits of a shop item
    /// </summary>
    /// <param name="index">The id of the Item to get the benefits for (0 = Screen, 1 = Staff, 2 = Vending Machine, 3 = Food Area)</param>
    /// <returns></returns>
    public string GetBenefitString(int index)
    {
        // based on the index, return a suitable string
        switch (index)
        {
            case 0: return "Buying more screens will entice more customers to come to your cinema";
            case 1: return "Hiring more staff will allow you to carry out more tasks at a time";
            case 2: return "Vending machines will bring in a small amount of income each day";
            case 3: return "Customers will visit the food area to buy food. Earns coins";
            case 4: return "Posters will raise awareness of your cinemas and entice more customers to come";
            case 5: return "Posters will raise awareness of your cinemas and entice more customers to come";
            default: return "No information available";
        }
    }
}