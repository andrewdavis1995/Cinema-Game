using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConfirmationScript : MonoBehaviour {

    static int actionCode = -1;
    static string[] parameters;

    static Controller mainController;
    static ShopScript theShop;
    static GameObject theConfirmPanel;
    static Text[] textElements;
    static Image[] imageElements;

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
        mainController.confirmationPanel.SetActive(true);
        actionCode = code;
        parameters = p;

        textElements[4].text = "Are you sure you wish to " + p[0];
        // TODO: set the currency image
        textElements[3].text = p[2];

    }

    public void AttributeUpgrade(int index)
    {
        string[] parameters = new string[4];
        parameters[0] = "Upgrade this staff Member's attribute?";
        parameters[1] = "0";
        parameters[2] = "50"; // TODO - set price based on the current level
        parameters[3] = index.ToString();
        OptionSelected(1, parameters);
    }

    public void Cancel() { mainController.confirmationPanel.SetActive(false); }

    public void Confirmed ()
    {
        mainController.confirmationPanel.SetActive(false);

        switch (actionCode)
        {
            case 0:
                int id = int.Parse(parameters[0]);
                theShop.Purchase(id);
                break;
            case 1:
                int index = int.Parse(parameters[3]);
                mainController.UpgradeStaffAttribute(index);
                break;
        }
    }

}
