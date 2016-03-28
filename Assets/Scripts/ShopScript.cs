using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Classes;

public class ShopScript : MonoBehaviour {

    Controller mainController;
    GameObject closeBtn;

    void Start()
    {
        closeBtn = GameObject.Find("Close Shop");
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
    }


    public void Purchase(int id)
    {
        mainController.itemToAddID = id;

        switch (id)
        { 
            case 0: // ScreenObject
                mainController.objectSelected = "NEW SCREEN";
                mainController.placeObject(11, 15);
                break;
            case 1:
                ConfirmationScript.OptionSelected(2, new string[] { "hire a new Staff Member?", "800", "0" });
                break;
            case 2: // Plant
                mainController.objectSelected = "NEW PLANT";
                mainController.placeObject(1, 1);
                break;
            case 3: // Bust
                mainController.objectSelected = "NEW BUST";
                mainController.placeObject(2, 3);
                break;
            case 4: // 5 Popcorns
                // micro-transactions - MONEY!
                break;
            case 5: // Vending Machine
                mainController.objectSelected = "NEW VENDING MACHINE";
                mainController.placeObject(3, 3);
                break;
            case 6: // Red Carpet
                mainController.redCarpet.SetActive(true);
                break;
        }
    }

    public void PutCloseToTop()
    {
        closeBtn.GetComponent<RectTransform>().SetAsLastSibling();
        closeBtn.GetComponent<RectTransform>().GetSiblingIndex();
    }

}


/*

IDs: 

    0   -   ScreenObject
    1   -   Staff Member
    2   -   5 x Popcorn

*/