using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Classes;
using System.Collections.Generic;

public class ConfirmationScript : MonoBehaviour {

    static int actionCode = -1;     // the code of which action to do
    static string[] parameters;     // the parameters which are passed through

    // references to the instances of other scripts to use
    static Controller mainController;
    static ShopScript theShop;
    static GameObject theConfirmPanel;

    // the elements of the confirmation popups
    static Text[] textElements;
    static Image[] imageElements;
    public Sprite[] currencyImages;

    // runs once at startup of script
    void Start()
    {
        // set up all variables
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
        theShop = GameObject.Find("Shop Canvas").GetComponent<ShopScript>();
        theConfirmPanel = mainController.confirmationPanel;
        textElements = theConfirmPanel.GetComponentsInChildren<Text>();
        imageElements = theConfirmPanel.GetComponentsInChildren<Image>();
    }

    /// <summary>
    /// When the user has selected an option
    /// </summary>
    /// <param name="code">The action code representing the action they wish to happen (0 = Add new item, 1 = Upgrade staff, 2 = Hire staff, 3 = Upgrade screen, 4 = Buy red carpet, 5 = Upgrade Box Office, 6 = Sell Object, 7 = Clear all projectors, 8 = Food Area unlocked)</param>
    /// <param name="p">The parameters associated with the action ([0] = Action string, [1] = cost, [2] = currency, [3->] = extras)</param>
    /// <param name="inOrOut"></param>
    public static void OptionSelected(int code, string[] p, string inOrOut)
    {
        // set the status code of main controller
        mainController.statusCode = 8;
        // show the conirmation panel
        mainController.confirmationPanel.SetActive(true);
        
        actionCode = code;
        parameters = p;

        // display the action that needs confirmation
        textElements[4].text = "Are you sure you wish to " + p[0];
        textElements[3].text = p[1];
        textElements[2].text = inOrOut;
        imageElements[4].sprite = mainController.confirmationPanel.GetComponentInChildren<ConfirmationScript>().currencyImages[int.Parse(p[2])];
    }

    /// <summary>
    /// Upgrading a staff's attribute
    /// </summary>
    /// <param name="index">Which attribute to upgrade (0 = Ticket, 1 = Food, 2 = Friendliness, 3 = Clarity)</param>
    public void AttributeUpgrade(int index)
    {
        // get the current upgrade level of the relevant attribute
        int currValue = mainController.staffMembers[mainController.selectedStaff].GetAttributeByIndex(index);

        // if the current level is less than 4 (i.e. not full)...
        if (currValue < 4)
        {
            // calculate the cost
            int cost = 100;
            int addition = 0;

            for (int i = 1; i < currValue; i++)
            {
                addition += (500 * i);
            }

            cost += addition;

            // setup the parameters to pass
            string[] parameters = new string[4];
            parameters[0] = "Upgrade " + mainController.staffMembers[mainController.selectedStaff].GetStaffname() + "'s " + GetAttributeName(index) + " attribute?";
            parameters[1] = cost.ToString();
            parameters[2] = "0";
            parameters[3] = index.ToString();
            OptionSelected(1, parameters, "This will cost: ");
        }
        else
        {
            mainController.ShowPopup(7, "This attribute is already fully upgraded!");
        }
    }

    /// <summary>
    /// Get the name of an attribute (for display purposes)
    /// </summary>
    /// <param name="index">The index of the attribute required</param>
    /// <returns>The name of the attribute</returns>
    public string GetAttributeName(int index)
    {
        // return the relevant string based on the index
        switch (index)
        {
            case 0:
                return "Ticket Speed";
            case 1:
                return "Food Speed";
            case 2:
                return "Friendliness";
            case 3:
                return "Clarity";
        }

        return "";

    }

    /// <summary>
    /// When the operation has been cancelled
    /// </summary>
    public void Cancel()
    {
        // hide the panel and return to the original status code
        mainController.confirmationPanel.SetActive(false);
        mainController.statusCode = mainController.newStatusCode;
    }

    /// <summary>
    /// When the user clicks 'Confirm'
    /// </summary>
    public void Confirmed ()
    {
        // get the cost from the parameter list
        int cost = int.Parse(parameters[1]);

        // if the user has enough coins / popcorn to afford the action
        if ((mainController.totalCoins >= cost && parameters[2].Equals("0")) || (mainController.numPopcorn >= cost && parameters[2].Equals("1")) || actionCode == 6)
        {
            // hide the panel
            mainController.confirmationPanel.SetActive(false);

            #region Remove money / Add Money (if selling item)
            // depending on which currency is to be used, remove the necessary amount from the players balance
            if (parameters[2].Equals("0"))
            {
                if (actionCode != 6)
                {
                    mainController.totalCoins -= cost;;
                }
                else
                {
                    mainController.totalCoins += cost;
                }
                mainController.coinLabel.text = mainController.totalCoins.ToString();
            }
            else
            {
                mainController.numPopcorn -= int.Parse(parameters[1]);
                mainController.popcornLabel.text = mainController.numPopcorn.ToString();
            }
            #endregion

            #region Carry out actions
            switch (actionCode)
            {
                #region Add a new object
                case 0:
                    mainController.AddNewObject(int.Parse(parameters[3]), int.Parse(parameters[4]));
                    mainController.statusCode = 0;
                    break;
                #endregion

                #region Upgrade staff attribute
                case 1:
                    // get the index of the attribute to upgrade
                    int index = int.Parse(parameters[3]);
                    mainController.UpgradeStaffAttribute(index);
                    mainController.statusCode = 7;
                    break;
                #endregion

                #region Hire new staff
                case 2:
                    
                    // set the status code
                    mainController.statusCode = 99;

                    // initialise the values for the staff customisation
                    AppearanceScript.Initialise(true, null, 1, mainController.staffMembers[0].GetColourByIndex(0), -1, AppearanceScript.hairStyle, "", AppearanceScript.extraOption);

                    // show the necessary menus / objects
                    mainController.staffModel.SetActive(true);
                    mainController.staffAppearanceMenu.SetActive(true);
                    theShop.gameObject.SetActive(false);

                    // move the camera into place
                    Camera.main.transform.position = new Vector3(32.68f, 0, 1);
                    Camera.main.orthographicSize = 14;

                    break;
                #endregion

                #region Upgrade a screen
                case 3:
                    // get the index of the screen to upgrade
                    int screenIndex = int.Parse(parameters[3]);
                    // get the actual screen object
                    ScreenObject theScreen = mainController.screenObjectList[screenIndex].GetComponent<Screen_Script>().theScreen;
                    // upgrade the screen
                    theScreen.Upgrade();
                    // change the image to the contruction image
                    mainController.screenObjectList[screenIndex].GetComponent<SpriteRenderer>().sprite = mainController.screenImages[0];

                    // hide the object info info menu
                    mainController.objectInfo.SetActive(false);
                    mainController.closeInfo.SetActive(false);

                    // generate new show times
                    mainController.NewShowTimes();
                    mainController.statusCode = 0;

                    // create a builder for the screen
                    mainController.CreateBuilder(theScreen.GetX(), theScreen.GetY(), theScreen.GetScreenNumber());

                    break;
                #endregion

                #region Red carpet
                case 4:
                    // show the carpet
                    mainController.redCarpet.SetActive(true);
                    mainController.hasUnlockedRedCarpet = true;
                    mainController.statusCode = 5;
                    break;
                #endregion

                #region Upgrade Box Office
                case 5:
                    OtherObjectScript.UpgradeBoxOffice();
                    mainController.statusCode = 0;
                    break;
                #endregion

                #region Remove an object
                case 6:

                    // get the height, width, xPos and yPos
                    int xPos = int.Parse(parameters[3]);
                    int yPos = int.Parse(parameters[4]);
                    int width = int.Parse(parameters[5]);
                    int height = int.Parse(parameters[6]);

                    // remove the object
                    mainController.RemoveObject(xPos, yPos, width, height);
                    mainController.statusCode = 0;
                    break;
                #endregion

                #region Clear all projectors
                case 7:
                    mainController.ClearAllProjectors();
                    mainController.statusCode = 0;
                    break;
                #endregion

                #region Purchase Food Area Component
                case 8:
                    // get which component is to be purchased
                    int theIndex = int.Parse(parameters[3]);
                    // unlock the component
                    FoodAreaScript.ComponentUnlocked(theIndex);
                    mainController.statusCode = 10;
                    break;
                #endregion

                #region Purchase Poster pack 1
                case 9:
                        mainController.UnlockPosterPack(0);
                    break;
                #endregion

                #region Purchase Poster pack 2
                case 10:
                        mainController.UnlockPosterPack(1);                    
                    break;
                    #endregion
            }
            #endregion
        }
        else
        {
            mainController.ShowPopup(5, "You do not have enough money for this purchase. You can buy more in the shop");
        }
    }


}
