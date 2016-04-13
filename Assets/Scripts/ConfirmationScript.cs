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

    public static void OptionSelected(int code, string[] p, string inOrOut)
    {
        mainController.statusCode = 8;
        mainController.confirmationPanel.SetActive(true);
        
        actionCode = code;
        parameters = p;

        textElements[4].text = "Are you sure you wish to " + p[0];
        textElements[3].text = p[1];
        textElements[2].text = inOrOut;
        imageElements[4].sprite = mainController.confirmationPanel.GetComponentInChildren<ConfirmationScript>().currencyImages[int.Parse(p[2])];
    }

    public void AttributeUpgrade(int index)
    {
        int currValue = mainController.staffMembers[mainController.selectedStaff].GetAttributeByIndex(index);

        if (currValue < 4)
        {
            int cost = 100;
            int addition = 0;

            for (int i = 1; i < currValue; i++)
            {
                addition += (500 * i);
            }

            cost += addition;

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

    public string GetAttributeName(int index)
    {
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

    public void Cancel() { mainController.confirmationPanel.SetActive(false); }

    public void Confirmed ()
    {
        int cost = int.Parse(parameters[1]);

        if ((mainController.totalCoins >= cost && parameters[2].Equals("0")) || (mainController.numPopcorn >= cost && parameters[2].Equals("1")))
        {

            mainController.confirmationPanel.SetActive(false);
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

            switch (actionCode)
            {
                case 0:
                    mainController.AddNewObject(int.Parse(parameters[3]), int.Parse(parameters[4]));
                    mainController.statusCode = 0;
                    break;
                case 1:
                    int index = int.Parse(parameters[3]);
                    mainController.UpgradeStaffAttribute(index);
                    mainController.statusCode = 7;
                    break;
                case 2:
                    // hire new staff
                    // ( index2 & name would be passed from the appearance selection page inside parameters )
                    int index2 = UnityEngine.Random.Range(0, 5);
                    StaffMember sm = new StaffMember(mainController.staffMembers.Count, "New", mainController.staffPrefabs[index2], mainController.currDay, index2);

                    int x = 35 + 2 * (sm.GetIndex() % 6);
                    int y = 2 * (sm.GetIndex() / 6);

                    mainController.AddStaffMember(sm, x, y);
                    mainController.HideObjectInfo();
                    mainController.statusCode = 0;
                    break;
                case 3:
                    // upgrade screen
                    int screenIndex = int.Parse(parameters[3]);
                    ScreenObject theScreen = mainController.screenObjectList[screenIndex].GetComponent<Screen_Script>().theScreen;
                    theScreen.Upgrade();
                    mainController.screenObjectList[screenIndex].GetComponent<SpriteRenderer>().sprite = mainController.screenImages[0];

                    mainController.objectInfo.SetActive(false);
                    mainController.closeInfo.SetActive(false);

                    mainController.NewShowTimes();
                    mainController.statusCode = 0;

                    mainController.CreateBuilder(theScreen.GetX(), theScreen.GetY(), theScreen.GetScreenNumber());

                    for (int k = 0; k < mainController.filmShowings.Count; k++)       // filmShowings.Count
                    {
                        int index3 = mainController.filmShowings[k].GetScreenNumber();
                        int ticketsSold = mainController.GetTicketsSoldValue(Controller.theScreens[index3 - 1]);
                        mainController.filmShowings[k].SetTicketsSold(ticketsSold);

                        int currentCount = 0;

                        for (int j = 0; j < k; j++)
                        {
                            currentCount += mainController.filmShowings[j].GetTicketsSold();
                        }

                        List<Customer> tmp = mainController.filmShowings[k].CreateCustomerList(currentCount, mainController);
                        mainController.allCustomers.AddRange(tmp);
                    }

                    break;
                case 4:
                    mainController.redCarpet.SetActive(true);
                    mainController.hasUnlockedRedCarpet = true;
                    mainController.statusCode = 5;
                    break;
                case 5:
                    OtherObjectScript.UpgradeBoxOffice();
                    mainController.statusCode = 0;
                    break;
                case 6:

                    int xPos = int.Parse(parameters[3]);
                    int yPos = int.Parse(parameters[4]);
                    int width = int.Parse(parameters[5]);
                    int height = int.Parse(parameters[6]);

                    mainController.RemoveObject(xPos, yPos, width, height);
                    mainController.statusCode = 0;
                    break;
                case 7:
                    mainController.ClearAllProjectors();
                    mainController.statusCode = 0;
                    break;
            }
        }
        else
        {
            mainController.ShowPopup(5, "You do not have enough money for this purchase");
        }
    }


}
