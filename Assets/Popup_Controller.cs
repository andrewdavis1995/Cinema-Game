using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Popup_Controller : MonoBehaviour
{
    public GameObject popupBox;

    public Slider musicVal;
    public Slider fxVal;
    public Toggle autosave;

    public Transform friendList;    // the menu to display the Facebook friends
    public GameObject bottomPanel;

    public Transform staffMenu;
    public Transform staffList;

    public GameObject settingsPage;
    public GameObject settingsButton;

    public GameObject reputationPage;

    public Canvas giftMenu;

    public GameObject confirmationPanel;

    public GameObject popup;

    public GameObject moveButtons;

    public GameObject closeInfo;

    public GameObject colourPicker;
    public GameObject objectInfo;
    public GameObject confirmMovePanel;
    public GameObject shopCanvas;

    public GameObject staffMemberInfo;

    public GameObject confirmPanel;

    public GameObject warningPanel;
    public Image warningIcon;
    public Text warningLabel;

    public Sprite boxOfficeIcon;


    // controllers
    public Controller mainController;
    public Customer_Controller customerController;


    void Start()
    {
        #region Hide Objects
        staffMemberInfo.SetActive(false);
        warningPanel.SetActive(false);
        warningIcon.enabled = false;
        staffMenu.gameObject.SetActive(false);
        confirmationPanel.SetActive(false);
        popup.SetActive(false);
        reputationPage.SetActive(false);
        confirmMovePanel.SetActive(false);
        objectInfo.SetActive(false);
        shopCanvas.SetActive(false);
        closeInfo.SetActive(false);
        colourPicker.SetActive(false);
        popupBox.SetActive(false);
        moveButtons.SetActive(false);
        #endregion
    }
    /// <summary>
    /// Open the shop menu
    /// </summary>
    public void OpenShop()
    {
        if (mainController.statusCode != 2 && mainController.statusCode != 8 && mainController.statusCode != 9 && Controller.isOwned)
        {
            HideObjectInfo();
            mainController.statusCode = 5;
            shopCanvas.SetActive(true);
        }
    }

    /// <summary>
    /// Hide all menus
    /// </summary>
    public void HideObjectInfo()
    {
        shopCanvas.SetActive(false);
        objectInfo.SetActive(false);
        closeInfo.SetActive(false);
        staffMenu.gameObject.SetActive(false);
        reputationPage.SetActive(false);
        popupBox.SetActive(false);
        
        colourPicker.SetActive(false);
        staffMemberInfo.SetActive(false);
        friendList.GetComponentsInParent<Canvas>()[0].enabled = false;

        mainController.statusCode = 0;
        mainController.objectSelected = "";
        mainController.tagSelected = "";
        mainController.upgradeLevelSelected = 0;
        mainController.selectedStaff = -1;
        giftMenu.enabled = false;
    }

    /// <summary>
    /// Open the staff menu - list of all staff members
    /// </summary>
    public void OpenStaffMenu()
    {
        if (mainController.statusCode == 0 || mainController.statusCode == 5 || mainController.statusCode == 55)
        {
            HideObjectInfo();
            mainController.statusCode = 6;
            staffMenu.gameObject.SetActiveRecursively(true);
        }
    }
    
    /// <summary>
    /// Hide the popup option
    /// </summary>
    public void HidePopup()
    {
        popup.SetActive(false);
        confirmationPanel.SetActive(false);

        popupBox.SetActive(false);

        mainController.statusCode = mainController.newStatusCode;

        if (mainController.statusCode == 99)
        {
            AppearanceScript.Initialise(true, null, 2, new Color(1, 1, 1, 1), -1, null, "", null);
            // hide all staff
            GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");

            // change camera position
            Camera.main.transform.position = new Vector3(32.68f, 0, 1);
            Camera.main.orthographicSize = 14;
            mainController.staffAppearanceMenu.SetActive(true);
            bottomPanel.SetActive(false);
            settingsButton.SetActive(false);
            mainController.staffModel.SetActive(true);
        }

    }

    /// <summary>
    /// Open the menu for displaying the list of facebook friends
    /// </summary>
    public void OpenFacebookFriends()
    {
        HideObjectInfo();
        mainController.statusCode = 55;
        friendList.GetComponentsInParent<Canvas>()[0].enabled = true;
    }

    /// <summary>
    /// Show the popup box
    /// </summary>
    /// <param name="status">The status to return to once the popup has been closed</param>
    /// <param name="theString">The message to display</param>
    public void ShowPopup(int status, string theString)
    {
        mainController.newStatusCode = status;
        popup.SetActive(true);
        Text[] texts = popup.gameObject.GetComponentsInChildren<Text>();
        texts[1].text = theString;
    }

    /// <summary>
    /// Show the colour picker menu (carpet)
    /// </summary>
    public void ShowColourPicker()
    {
        colourPicker.SetActive(!colourPicker.active);
        shopCanvas.SetActive(false);
    }
    
    /// <summary>
    /// Display the reputation menu
    /// </summary>
    public void ViewReputation()
    {
        mainController.statusCode = 9;
        popupBox.SetActive(false);
        reputationPage.SetActive(true);

        Text[] textElements = reputationPage.gameObject.GetComponentsInChildren<Text>();

        textElements[2].text = customerController.reputation.GetTotalCoins().ToString();
        textElements[4].text = customerController.reputation.GetOverall().ToString() + "%";
        textElements[5].text = customerController.reputation.GetTotalCustomers().ToString();
        textElements[6].text = customerController.reputation.GetHighestRep().ToString() + "%";

        textElements[9].text = (4 * customerController.reputation.GetSpeedRating()).ToString();
        textElements[11].text = (4 * customerController.reputation.GetPublicityRating()).ToString();
        textElements[13].text = (4 * customerController.reputation.GetFacilitiesRating()).ToString();
        textElements[15].text = (4 * customerController.reputation.GetStaffRating()).ToString();


        Image[] imageElements = reputationPage.gameObject.GetComponentsInChildren<Image>();
        imageElements[4].fillAmount = (float)customerController.reputation.GetSpeedRating() / 25f;
        imageElements[7].fillAmount = (float)customerController.reputation.GetPublicityRating() / 25f;
        imageElements[10].fillAmount = (float)customerController.reputation.GetFacilitiesRating() / 25f;
        imageElements[13].fillAmount = (float)customerController.reputation.GetStaffRating() / 25f;
    }

    /// <summary>
    /// Close the reputation menu
    /// </summary>
    public void CloseReputation()
    {
        reputationPage.SetActive(false);
        popupBox.SetActive(true);
        mainController.statusCode = 0;
    }

    /// <summary>
    /// Show the building menu (object info)
    /// </summary>
    /// <param name="line1">The first line to display</param>
    /// <param name="line2">The seconf line to display</param>
    /// <param name="theImage">Which image to use</param>
    /// <param name="constrDone">How many days of construction have been done</param>
    /// <param name="constrTotal">How many days of construction were needed to begin with</param>
    public void ShowBuildingOptions(string line1, string line2, Sprite theImage, int constrDone, int constrTotal)
    {

        if (Controller.isOwned)
        {

            objectInfo.SetActive(true);
            closeInfo.SetActive(true);
            Text[] labels = objectInfo.gameObject.GetComponentsInChildren<Text>();
            labels[0].text = line1;
            labels[1].text = line2;
            Image[] images = objectInfo.gameObject.GetComponentsInChildren<Image>();


            images[3].sprite = theImage;


            images[6].gameObject.GetComponent<Image>().color = Color.white;
            images[6].gameObject.GetComponent<Button>().enabled = true;

            if (line1.ToUpper().Contains("SCREEN") || line1.ToUpper().Contains("FOOD"))
            {
                images[2].gameObject.GetComponent<Image>().color = Color.white;
                images[2].gameObject.GetComponent<Button>().enabled = true;
                images[1].gameObject.GetComponent<Image>().color = Color.white;

                if (line1.ToUpper().Contains("FOOD"))
                {
                    images[3].sprite = mainController.completeFoodAreaSprite;
                }

            }
            else if (line1.ToUpper().Contains("BOX"))
            {
                images[2].gameObject.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
                images[2].gameObject.GetComponent<Button>().enabled = false;
                images[1].gameObject.GetComponent<Image>().color = Color.white;
                images[6].gameObject.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
                images[6].gameObject.GetComponent<Button>().enabled = false;
            }
            else
            {
                images[2].gameObject.GetComponent<Image>().color = Color.white;
                images[2].gameObject.GetComponent<Button>().enabled = true;
                images[1].gameObject.GetComponent<Image>().color = new Color(0.06f, 0.06f, 0.06f);
            }

            if (line1.ToUpper().Contains("BUST"))
            {
                images[6].gameObject.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
                images[6].gameObject.GetComponent<Button>().enabled = false;
            }


            // construction in progress section
            if (constrDone > -1)
            {
                // do the green bar
                images[4].enabled = true;
                images[5].enabled = true;
                labels[3].enabled = true;
                labels[2].enabled = true;

                float percent = (float)constrDone / (float)constrTotal;

                images[5].fillAmount = percent;

                labels[2].text = constrDone + "/" + constrTotal;

                // finish now
                images[7].enabled = true;
                images[8].enabled = true;
                labels[4].enabled = true;
                labels[4].text = "Finish work now for " + (int)(1.5 * (constrTotal - constrDone)) + " popcorn";


            }
            else
            {
                //hide the bar and label
                images[4].enabled = false;
                images[5].enabled = false;
                labels[3].enabled = false;
                labels[2].enabled = false;

                // finish now
                images[7].enabled = false;
                images[8].enabled = false;
                labels[4].enabled = false;
            }
        }
        else
        {
            mainController.statusCode = 0;
        }

    }

    /// <summary>
    /// Shows the popup at the end of the day - displays the days earnings etc
    /// </summary>
    /// <param name="todaysMoney">Coins earnt today</param>
    /// <param name="walkouts">Number of customers who walked out</param>
    /// <param name="repChange">How much the reputation of the cinema changed</param>
    /// <param name="numCustomers">The number of customers who were served</param>
    public void ShowEndOfDayPopup(int todaysMoney, int walkouts, int repChange, int numCustomers, int popcornEarned)
    {

        // get values here - pass some as parameters
        mainController.newStatusCode = 0;

        popupBox.SetActive(true);

        Text[] txts = popupBox.GetComponentsInChildren<Text>();
        txts[3].text = mainController.financeController.GetNumCoins().ToString();
        txts[4].text = todaysMoney.ToString();
        txts[6].text = repChange.ToString() + "%";
        txts[7].text = numCustomers.ToString();
        txts[8].text = walkouts.ToString();

        mainController.statusCode = 9;

        popupBox.SetActiveRecursively(true);

        if (popcornEarned > 0)
        {
            GameObject go = GameObject.Find("pnlPopcornEarned");
            go.SetActive(true);
            Text[] lbls = go.GetComponentsInChildren<Text>();
            lbls[0].text = "You have earned " + popcornEarned + " today";
        }
        else
        {
            GameObject.Find("pnlPopcornEarned").SetActive(false);
        }

    }

    /// <summary>
    /// Opens the settings popup
    /// </summary>
    public void ShowSettings()
    {
        HideObjectInfo();

        mainController.statusCode = 73;
        settingsPage.SetActive(true);

        // set values based on current options selected
        musicVal.value = mainController.options.GetMusicLevel();
        fxVal.value = mainController.options.GetFXLevel();
        autosave.isOn = mainController.options.GetAutosave();

    }

    /// <summary>
    /// Display a warning that one (or more) of the screens are inaccessible
    /// </summary>
    public void DisplayWarning()
    {
        warningPanel.SetActive(!warningPanel.active);
    }

    /// <summary>
    /// Saves/Updates the chosen settings and closes the popup
    /// </summary>
    public void SaveSettings()
    {
        mainController.statusCode = 0;
        settingsPage.SetActive(false);

        int music = (int)musicVal.value;
        int fx = (int)fxVal.value;
        bool auto = autosave.isOn;

        mainController.options.UpdateDetails(music, fx, auto);
    }

    /// <summary>
    /// When the option for sending a gift to a Facebook friend is clicked
    /// </summary>
    public void SendGift()
    {
        string userID = FBScript.current.id;
        string friendID = Controller.friendID;

        ConfirmationScript.OptionSelected(13, new string[] { "send 1 popcorn to this friend?", "1", "1", userID, friendID }, "This will cost you: ");     // 3 = user ID, 4 = friend ID
    }

    /// <summary>
    /// Return to the main menu
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
