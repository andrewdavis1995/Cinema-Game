using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Classes;
using System.Collections.Generic;

public class ConfirmationScript : MonoBehaviour {

    static int actionCode = -1;
    static string[] parameters;

    static Controller mainController;
    static ShopScript theShop;
    static GameObject theConfirmPanel;
    static Text[] textElements;
    static Image[] imageElements;
    public Sprite[] currencyImages;

    void Start()
    {
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
        theShop = GameObject.Find("Shop Canvas").GetComponent<ShopScript>();
        theConfirmPanel = mainController.confirmationPanel;
        textElements = theConfirmPanel.GetComponentsInChildren<Text>();
        imageElements = theConfirmPanel.GetComponentsInChildren<Image>();
    }

    public static void OptionSelected(int code, string[] p)
    {
        mainController.statusCode = 8;
        mainController.confirmationPanel.SetActive(true);
        
        actionCode = code;
        parameters = p;

        textElements[4].text = "Are you sure you wish to " + p[0];
        textElements[3].text = p[1];
        imageElements[4].sprite = mainController.confirmationPanel.GetComponentInChildren<ConfirmationScript>().currencyImages[int.Parse(p[2])];
    }

    public void AttributeUpgrade(int index)
    {
        if (mainController.staffMembers[mainController.selectedStaff].GetAttributeByIndex(index) < 4)
        {
            string[] parameters = new string[4];
            parameters[0] = "Upgrade this staff Member's attribute?";
            parameters[1] = "50"; // TODO - set price based on the current level
            parameters[2] = "0";
            parameters[3] = index.ToString();
            OptionSelected(1, parameters);
        }
        else
        {
            mainController.popup.SetActive(true);
            Text[] texts = mainController.popup.gameObject.GetComponentsInChildren<Text>();
            texts[1].text = "This attribute is already fully upgraded!";
        }
    }

    public void Cancel() { mainController.confirmationPanel.SetActive(false); }

    public void Confirmed ()
    {
        int cost = int.Parse(parameters[1]);

        if ((mainController.totalCoins >= cost && parameters[2].Equals("0")) || (mainController.numPopcorn >= cost && parameters[2].Equals("1")))
        {

            mainController.confirmationPanel.SetActive(false);
            if (parameters[2].Equals("0"))
            {
                mainController.totalCoins -= cost;
                mainController.coinLabel.text = mainController.totalCoins.ToString();
            }
            else
            {
                mainController.numPopcorn -= int.Parse(parameters[1]);
                mainController.popcornLabel.text = mainController.numPopcorn.ToString();
            }

            switch (actionCode)
            {
                case 0:
                    mainController.AddNewObject(int.Parse(parameters[3]), int.Parse(parameters[4]));
                    break;
                case 1:
                    int index = int.Parse(parameters[3]);
                    mainController.UpgradeStaffAttribute(index);
                    break;
                case 2:
                    // hire new staff
                    // ( index2 & name would be passed from the appearance selection page inside parameters )
                    int index2 = UnityEngine.Random.Range(0, 5);
                    StaffMember sm = new StaffMember(mainController.staffMembers.Count, "New", mainController.staffPrefabs[index2], mainController.currDay, index2);

                    int x = 35 + 2 * (sm.getIndex() % 6);
                    int y = 2 * (sm.getIndex() / 6);

                    mainController.addStaffMember(sm, x, y);
                    break;
                case 3:
                    // upgrade screen
                    int screenIndex = int.Parse(parameters[3]);
                    ScreenObject theScreen = mainController.screenObjectList[screenIndex].GetComponent<Screen_Script>().theScreen;
                    theScreen.upgrade();
                    mainController.screenObjectList[screenIndex].GetComponent<SpriteRenderer>().sprite = mainController.screenImages[0];

                    mainController.objectInfo.SetActive(false);
                    mainController.closeInfo.SetActive(false);

                    mainController.newShowTimes();
                    mainController.statusCode = 0;

                    mainController.CreateBuilder(theScreen.getX(), theScreen.getY(), theScreen.getScreenNumber());

                    for (int k = 0; k < mainController.filmShowings.Count; k++)       // filmShowings.Count
                    {
                        int index3 = mainController.filmShowings[k].getScreenNumber();
                        int ticketsSold = mainController.getTicketsSoldValue(Controller.theScreens[index3 - 1]);
                        mainController.filmShowings[k].setTicketsSold(ticketsSold);

                        int currentCount = 0;

                        for (int j = 0; j < k; j++)
                        {
                            currentCount += mainController.filmShowings[j].getTicketsSold();
                        }

                        List<Customer> tmp = mainController.filmShowings[k].createCustomerList(currentCount, mainController);
                        mainController.allCustomers.AddRange(tmp);
                    }

                    break;
                case 4:
                    mainController.redCarpet.SetActive(true);
                    mainController.hasUnlockedRedCarpet = true;
                    break;
            }
        }
        else
        {
            mainController.popup.SetActive(true);
            Text[] texts = mainController.popup.gameObject.GetComponentsInChildren<Text>();
            texts[1].text = "You do not have enough money for this purchase";
        }
    }


}
