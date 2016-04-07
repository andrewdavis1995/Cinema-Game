using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Classes;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour {

    Controller mainController;
    GameObject closeBtn;
    public GameObject[] shopTabs;

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
                
                for (int i = 0; i < mainController.screenObjectList.Count; i++)
                {
                    mainController.screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
                }
                for (int i = 0; i < mainController.gameObjectList.Count; i++)
                {
                    mainController.gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
                }

                break;
            case 1:
                ConfirmationScript.OptionSelected(2, new string[] { "hire a new Staff Member?", "800", "0" });
                break;
            case 2: // Plant
                mainController.objectSelected = "NEW PLANT";
                mainController.placeObject(1, 1);
                
                for (int i = 0; i < mainController.screenObjectList.Count; i++)
                {
                    mainController.screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
                }
                for (int i = 0; i < mainController.gameObjectList.Count; i++)
                {
                    mainController.gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
                }

                break;
            case 3: // Bust
                mainController.objectSelected = "NEW BUST";
                mainController.placeObject(2, 3);
                
                for (int i = 0; i < mainController.screenObjectList.Count; i++)
                {
                    mainController.screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
                }
                for (int i = 0; i < mainController.gameObjectList.Count; i++)
                {
                    mainController.gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
                }

                break;
            case 4: // 5 Popcorns
                // micro-transactions - MONEY!
                break;
            case 5: // Vending Machine
                mainController.objectSelected = "NEW VENDING MACHINE";
                mainController.placeObject(3, 3);

                for (int i = 0; i < mainController.screenObjectList.Count; i++)
                {
                    mainController.screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
                }
                for (int i = 0; i < mainController.gameObjectList.Count; i++)
                {
                    mainController.gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
                }

                break;
            case 6: // Red Carpet
                if (!mainController.hasUnlockedRedCarpet)
                {
                    ConfirmationScript.OptionSelected(4, new string[] { " unlock the Red Carpet?", "800", "0" });
                }
                else
                {
                    Debug.Log("Already Got it you silly sausage!");
                }
                break;
        }
    }

    void PutCloseToTop()
    {
        closeBtn.GetComponent<RectTransform>().SetAsLastSibling();
        closeBtn.GetComponent<RectTransform>().GetSiblingIndex();
    }

    public void TabChanged(int index)
    {
        Debug.Log(shopTabs[index].name);
        shopTabs[index].GetComponent<Transform>().SetAsLastSibling();

        for (int i = 0; i < shopTabs.Length; i++)
        {
            Image[] images = shopTabs[i].GetComponentsInChildren<Image>();

            if (!shopTabs[i].name.Equals(shopTabs[index].name))
            {
                images[1].color = new Color(1, 0.8353f, 0);
            }
            else
            {
                images[1].color = new Color(1, 0.5176f, 0);
            }
        }

        PutCloseToTop();
    }

    public void ViewItemBenefits(int index)
    {

        string benefit = GetBenefitString(index);
        
        mainController.newStatusCode = 5;
        mainController.popup.SetActive(true);
        Text[] texts = mainController.popup.gameObject.GetComponentsInChildren<Text>();
        texts[1].text = benefit;

    }

    public string GetBenefitString(int index)
    {
        switch (index)
        {
            case 0: return "Buying more screens will entice more customers to come to your cinema";
            case 1: return "Hiring more staff will allow you to carry out more tasks at a time";
            case 2: return "Vending machines will bring in a small amount of income each day";
            default: return "No information available";
        }
    }
}


/*

IDs: 

    0   -   ScreenObject
    1   -   Staff Member
    2   -   5 x Popcorn

*/