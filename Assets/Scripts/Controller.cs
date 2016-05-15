using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;
using Assets.Classes;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using System.Threading;

[System.Serializable]
public class Controller : MonoBehaviour
{
    #region Variables

    public GameObject confirmBtn;

    public int selectedStaff = -1;
    public int newStatusCode = 0;

    public Popup_Controller popupController;
    public ProjectorScript projectorController;
    public Customer_Controller customerController;
    public ShopController shopController;
    
    public GameObject[] walls;

    public GameObject cmdFriends;

    public Sprite[] boxOfficeImages;
    public int boxOfficeLevel = 0;

    public Transform friendObject;

    public Sprite[] validityTiles;
    public Sprite[] foodTableSprites;

    public Sprite completeFoodAreaSprite;

    List<GameObject> staffMenuList = new List<GameObject>();

    //public static Sprite profilePicture;

    public List<Transform> staffSlot = new List<Transform>();
    public List<bool> slotState = new List<bool>();
    public Transform slotPrefab;

    public Transform staffInfoObject;

    public int itemToAddID = -1;

    public GameObject startDayButton;
    public float mouseSensitivity = 1.0f;

    GameObject steps;

    const string warning1 = "WARNING! \n\nThe Following Screen(s) are inaccessible to Customers:\n\n";
    const string warning2 = "\nYou have built objects which block the path to these Screens. If you do not move them, the customers for these screens will leave and you will not get money from them! Plus, your reputation will be ruined!";

    public string objectSelected = "";
    public string tagSelected = "";
    public int upgradeLevelSelected = 0;


    public int statusCode = 99;     // 0 = free, 1 = dragging staff, 2 = moving object, 3 = in menu, 4 = moving camera, 5 = shop, 6 = staff menu, 7 = staff member info, 8 = Confirmation page, 9 = popup, 10 = upgrade food area

    public Color carpetColour;

    public Transform greenGuy;
    public Transform blueGuy;
    public Transform orangeGuy;

    public Transform staffPrefab;

    public Text timeLabel;
    public Text dayLabel;

    public Button shopButton;
    public Button staffMenuButton;

    public static FoodArea foodArea = null;

    public List<StaffMember> staffMembers = new List<StaffMember>();

    public List<FilmShowing> filmShowings = new List<FilmShowing>();

    public TileManager theTileManager;

    public delegate void SetTileStates(int startX, int startY, int w, int h, int newState, bool complete);
    public static event SetTileStates updateTileState;

    public Sprite ColourBackground;
    public Sprite colourCircle;
    public Sprite marbleBackground;
    public Sprite[] marbleSquares;
    public bool isMarbleFloor = false;

    const int width = 80;
    const int height = 40;

    public GameObject[,] floorTiles;

    public CustomerQueue ticketQueue;
    public CustomerQueue foodQueue;

    public bool simulationRunning = false;

    public Finance_Controller financeController;

    public GameObject staffModel;
    public GameObject staffAppearanceMenu;

    public FacebookFriend facebookProfile;

    public int currDay = 0;

    int numScreens = 1;

    public bool paused = false;

    float queueCount = 0;
    float ticketStaffLevel = 4.5f;


    #endregion

    // Use this for initialization
    void Start()
    {
        ticketQueue = new CustomerQueue(11, 38.5f, 6.8f, 0);

        #region Find Objects
        theTileManager = GameObject.Find("TileManagement").GetComponent<TileManager>();

        GameObject custStatus = GameObject.Find("Customer Status");
        movementScript.customerStatus = custStatus;
        GameObject[] tmpArray = GameObject.FindGameObjectsWithTag("Floor Tile");
        steps = GameObject.Find("Steps");
        mouseDrag.staffAttributePanel = GameObject.Find("Staff Attributes");
        #endregion

        Customer.tiles = floorTiles;

        #region Hide Objects on Start
        custStatus.SetActive(false);

        shopController.redCarpet.SetActive(false);
        mouseDrag.staffAttributePanel.SetActive(false);
        #endregion

        #region Add Delegate references
        mouseDrag.getStaffJobById += GetStaffJobById;
        mouseDrag.changeStaffJob += UpdateStaffJob;
        movementScript.addToQueueTickets += AddToQueueTickets;
        movementScript.getQueueTicketsSize += GetTicketQueueSize;
        movementScript.addToQueueFood += AddToQueueFood;
        movementScript.getQueueFoodSize += GetFoodQueueSize;
        #endregion

        #region Facebook stuff
        GameObject pnlNoFriends = GameObject.Find("pnlNoFriends");

        try
        {
            string fbUserID = FBScript.current.id;
            if (fbUserID.Length > 0)
            {
                cmdFriends.SetActive(true);
                facebookProfile = new FacebookFriend();
                facebookProfile.name = FBScript.current.firstname + " " + FBScript.current.surname;
                facebookProfile.id = FBScript.current.id;
                facebookProfile.friends = FBScript.current.friendList;

                if (facebookProfile.friends.Count > 0)
                {
                    pnlNoFriends.SetActive(false);
                }
                else
                {
                    pnlNoFriends.SetActive(true);
                }

                for (int i = 0; i < facebookProfile.friends.Count; i++)
                {
                    GameObject go = (GameObject)Instantiate(friendObject.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
                    go.transform.SetParent(popupController.friendList, false);

                    Text[] textComponents = go.GetComponentsInChildren<Text>();
                    textComponents[0].text = facebookProfile.friends[i].name;

                    Button[] buttonComponents = go.GetComponentsInChildren<Button>();
                    string idToSend = facebookProfile.friends[i].id;
                    string nameToSend = facebookProfile.friends[i].name;
                    buttonComponents[0].onClick.AddListener(() => ViewFriendsCinema(idToSend, nameToSend));

                }
            }
            else
            {
                pnlNoFriends.SetActive(true);
                // if the user has not logged into Facebook, hide the facebook friends button
                cmdFriends.SetActive(false);
            }
        }
        catch (Exception) { }
        #endregion

        // this will change depending on starting upgrade levels and other queues etc

        #region Load / New Game


        if (ButtonScript.friendData != null)
        {
            GameObject.Find("Bottom Panel").SetActive(false);
            GameObject.Find("lblOwnerName").GetComponent<Text>().text = ButtonScript.owner + "'s Cinema";           
            ButtonScript.dataCopy = ButtonScript.loadGame;
            ButtonScript.loadGame = ButtonScript.friendData;
            ButtonScript.friendData = null;
        }
        else
        {
            GameObject.Find("FriendPanel").SetActive(false);
        }
        
        // get Player data. If not null, load game
        if (ButtonScript.loadGame == null)
        {
            financeController.Inititalise(40000, 15);

            carpetColour = GetColourFromID(1);

            customerController.reputation = new Reputation();
            customerController.reputation.Initialise();

            OtherObjectScript.CreateStaffSlot(1, new Vector3(37.8f, 12.3f, 0));

            #region Floor Tiles
            floorTiles = new GameObject[40, 80];

            // initialise the floor tiles
            for (int i = 0; i < tmpArray.Length; i++)
            {
                string name = tmpArray[i].name;

                string[] tmp = name.Split('~');
                int x = int.Parse(tmp[1]);
                int y = int.Parse(tmp[2]);

                tmpArray[i].GetComponent<SpriteRenderer>().color = carpetColour;
                tmpArray[i].GetComponent<SpriteRenderer>().sprite = ColourBackground;
                //tmpArray[i].GetComponent<SpriteRenderer>().sprite = profilePicture;   // for funny times, uncomment this line

                floorTiles[x, y] = tmpArray[i];
            }
            #endregion

            for (int i = 0; i < 1; i++)
            {
                Vector3 pos = floorTiles[i * 11, 0].transform.position;
                ShopController.theScreens.Add(new ScreenObject((i + 1), 0));
                ShopController.theScreens[i].SetPosition((int)pos.x, (int)pos.y);
            }
            ShopController.theScreens[0].Upgrade();
            ShopController.theScreens[0].UpgradeComplete();
            // NYAH

            NextDay(false, false);

            // do staff intro thing here
            popupController.ShowPopup(99, "Welcome!!! This is your cinema!\nLets get started by hiring some staff shall we?");

            foodArea = null;

        }
        else
        {
            statusCode = 0;

            PlayerData data = ButtonScript.loadGame;

            carpetColour = new Color(data.carpetColour[0], data.carpetColour[1], data.carpetColour[2]);

            shopController.LoadDecorations(data.hasRedCarpet, data.posters);

            isMarbleFloor = data.marbleFloor;
            customerController.reputation = data.reputation;
            foodArea = data.foodArea;

            int boxLevel = data.boxOfficeLevel;
            OtherObjectScript.CreateStaffSlot(1, new Vector3(37.8f, 12.3f, 0));

            for (int i = 0; i < boxLevel - 1; i++)
            {
                OtherObjectScript.UpgradeBoxOffice();
            }

            #region Floor Tiles
            // initialise the floor tiles
            floorTiles = new GameObject[40, 80];

            for (int i = 0; i < tmpArray.Length; i++)
            {
                string name = tmpArray[i].name;

                string[] tmp = name.Split('~');
                int x = int.Parse(tmp[1]);
                int y = int.Parse(tmp[2]);

                tmpArray[i].GetComponent<SpriteRenderer>().color = carpetColour;
                if (!isMarbleFloor)
                {
                    tmpArray[i].GetComponent<SpriteRenderer>().sprite = ColourBackground;
                }
                else
                {
                    tmpArray[i].GetComponent<SpriteRenderer>().sprite = marbleSquares[UnityEngine.Random.Range(0, 3)];
                }

                floorTiles[x, y] = tmpArray[i];
            }
            #endregion

            ShopController.theScreens = new List<ScreenObject>(data.theScreens);


            SaveableStaff[] s = data.staffMembers;

            for (int i = 0; i < s.Length; i++)
            {
                int id = s[i].index;
                string name = s[i].name;
                Transform transform = staffPrefab;
                int dayHired = s[i].dayHired;
                int tID = s[i].transformID;
                int[] attributes = s[i].attributes;
                float[,] cols = s[i].colourArrays;
                int hair = s[i].hairStyleID;
                int extras = s[i].extrasID;


                GameObject go = GameObject.Find("AppearanceController");
                AppearanceScript aS = go.GetComponent<AppearanceScript>();

                Sprite hairSprite = null;
                Sprite extraSprite = null;

                if (hair != 6)
                {
                    hairSprite = aS.hairStyles[hair];
                }
                if (extras != 3)
                {
                    extraSprite = aS.extraImages[extras];
                }
                Color[] c = new Color[3];

                for (int colID = 0; colID < 3; colID++)
                {
                    c[colID] = new Color(cols[colID, 0], cols[colID, 1], cols[colID, 2]);
                }

                // TODO: set colours and hair style
                StaffMember newStaff = new StaffMember(id, name, transform, dayHired, tID, hairSprite);
                newStaff.SetColours(c, hair, extras);
                newStaff.SetSprites(hairSprite, extraSprite);
                newStaff.SetAttributes(attributes);

                int x = 35 + (2 * (newStaff.GetIndex() % 6)); ;
                int y = 2 * (newStaff.GetIndex() / 6);

                staffMembers.Add(newStaff);
                CreateStaff(newStaff, x, y);
            }

            filmShowings = new List<FilmShowing>(data.filmShowings);
            currDay = data.currentDay;
            numScreens = ShopController.theScreens.Count;
            financeController.Inititalise(data.totalCoins, data.numPopcorn);
            ShopController.otherObjects = new List<OtherObject>(data.otherObjects);


            NextDay(false, false);
            currDay--; // needed for some reason


            // hopefully un-breaks things
            for (int i = 0; i < ShopController.theScreens.Count; i++)
            {
                Vector3 pos = new Vector3(ShopController.theScreens[i].GetX(), ShopController.theScreens[i].GetY(), 0);
                ShopController.theScreens[i].SetPosition((int)pos.x, (int)(pos.y));
            }


            // sort staff appearance
            GameObject[] staffs = GameObject.FindGameObjectsWithTag("Staff");
            for (int i = 0; i < staffs.Length; i++)
            {
                SpriteRenderer[] srs = staffs[i].GetComponentsInChildren<SpriteRenderer>();
                srs[0].color = staffMembers[i].GetColourByIndex(0);
                srs[1].color = staffMembers[i].GetColourByIndex(2);
                srs[2].color = staffMembers[i].GetColourByIndex(2);
                srs[3].color = staffMembers[i].GetColourByIndex(1);
                srs[4].color = staffMembers[i].GetColourByIndex(1);

                srs[3].sprite = staffMembers[i].GetHairStyle();
                srs[4].sprite = staffMembers[i].GetExtras();
            }

            shopController.ShowPosters(0);
            shopController.ShowPosters(1);

        }
        
        #endregion

        // create some test screens
        for (int i = 0; i < ShopController.theScreens.Count; i++)
        {
            Vector3 pos = new Vector3(ShopController.theScreens[i].GetX(), ShopController.theScreens[i].GetY() * 0.8f, 0);

            //// align to grid - +/- 1 to move by one tile horizontally, 0.8 for vertical movement
            //pos.x += 4.6f;
            //pos.y += 6.05f;

            //// change pos and element here
            pos.y += 0.8f;

            shopController.AddScreen(ShopController.theScreens[i], pos, height);

            SetTiles(2, (int)(ShopController.theScreens[i].GetX()), (int)(ShopController.theScreens[i].GetY()), 11, 15);
        }

        // do same for other objects
        for (int i = 0; i < ShopController.otherObjects.Count; i++)
        {
            Vector3 pos = new Vector3(ShopController.otherObjects[i].xPos, ShopController.otherObjects[i].yPos * 0.8f, 0);

            //float xCorrection = 0;
            //float yCorrection = 0;

            DimensionTuple t = shopController.GetBounds(itemToAddID);

            shopController.AddObject(pos, i, height, itemToAddID, true);

            SetTiles(2, (int)(ShopController.otherObjects[i].xPos), (int)(ShopController.otherObjects[i].yPos), t.width, t.height);
        }

        //createColourPicker();

        if (updateTileState != null)
        {
            updateTileState(33, 0, 14, 16, 1, true);
            updateTileState(33, 16, 14, 4, 2, true);
        }




        GameObject[] pointers = GameObject.FindGameObjectsWithTag("Pointer");

        for (int i = 0; i < pointers.Length; i++)
        {
            pointers[i].GetComponent<Transform>().GetComponent<SpriteRenderer>().enabled = false;
        }


        dayLabel.text = "DAY: " + currDay.ToString();

    }

    /// <summary>
    /// When the application is closed
    /// </summary>
    void OnApplicationQuit()
    {
        // close off all open threads - memory issues
        ticketQueue.End();
    }

    /// <summary>
    /// When the application is paused
    /// </summary>
    /// <param name="pauseStatus">Current paused state of the game</param>
    void OnApplicationPause(bool pauseStatus)
    {
        paused = pauseStatus;

        if (simulationRunning)
        {
            if (pauseStatus)
            {
                ticketQueue.Pause();
            }
            else
            {
                ticketQueue.Resume();
            }
        }
    }

    /// <summary>
    /// When the food area upgrades have been completed
    /// </summary>
    public void FoodUpgradesDone()
    {
        statusCode = 0;
        confirmBtn.SetActive(false);
        Camera.main.orthographicSize = 10;

        // hide all locked components
        SpriteRenderer[] subImages = GameObject.FindGameObjectWithTag("Food Area").GetComponentsInChildren<SpriteRenderer>();

        subImages[1].enabled = foodArea.hasHotFood;
        subImages[2].enabled = foodArea.hasPopcorn;
        subImages[3].enabled = foodArea.hasIceCream;

        if (foodArea.hasHotFood) { subImages[1].color = new Color(1, 1, 1, 1); }
        if (foodArea.hasPopcorn) { subImages[2].color = new Color(1, 1, 1, 1); }
        if (foodArea.hasIceCream) { subImages[3].color = new Color(1, 1, 1, 1); }

        subImages[4].color = new Color(1, 1, 1, 1);

        subImages[4].sprite = foodTableSprites[foodArea.tableStatus];

        //int count = 0;
        //for (int i = 0; i < staffSlot.Count; i++)
        //{
        //    if (staffSlot)
        //}

    }

    /// <summary>
    /// Get the size of the ticket queue
    /// </summary>
    /// <returns>The size of the queue</returns>
    public int GetTicketQueueSize()
    {
        return ticketQueue.GetQueueSize();
    }

    /// <summary>
    /// Get the size of the food queue
    /// </summary>
    /// <returns>The size of the food queue</returns>
    public int GetFoodQueueSize()
    {
        return foodQueue.GetQueueSize();
    }

    /// <summary>
    /// Opens the editing menu for the staff member
    /// </summary>
    public void EditStaff()
    {
        // set the status code
        statusCode = 99;

        // initialise the values for the staff customisation
        AppearanceScript.Initialise(false, staffMembers[selectedStaff].GetAllColours(), 1, staffMembers[0].GetColourByIndex(0), staffMembers[selectedStaff].GetIndex(), staffMembers[selectedStaff].GetHairStyle(), staffMembers[selectedStaff].GetStaffname(), staffMembers[selectedStaff].GetExtras());

        // show the necessary menus / objects
        staffModel.SetActive(true);
        staffAppearanceMenu.SetActive(true);

        popupController.staffMemberInfo.SetActive(false);

        // move the camera into place
        Camera.main.transform.position = new Vector3(32.68f, 0, 1);
        Camera.main.orthographicSize = 14;

    }

    /// <summary>
    /// Open the cinema for the selected friend
    /// </summary>
    public void ViewFriendsCinema(string fbid, string name)
    {
        Debug.Log(fbid);

        Login l = new Login();
        PlayerData friendData = l.DoLogin(fbid);

        if (friendData != null)
        {

            ButtonScript.owner = name;

            ButtonScript.friendData = friendData;

            SceneManager.LoadScene(1);
        }
        else
        {
            popupController.ShowPopup(55, "No cinema data available for " + name);
        }
    }

    /// <summary>
    /// 
    /// Once the editing is complete
    /// </summary>
    /// <param name="index">The index of the staff member who was edited</param>
    public void StaffEditComplete(int index, string name, int hID, int eID)
    {
        Color[] c = new Color[3];
        c[0] = AppearanceScript.colours[0];
        c[1] = AppearanceScript.colours[1];
        c[2] = AppearanceScript.colours[2];

        Sprite hs = AppearanceScript.hairStyle;
        Sprite ex = AppearanceScript.extraOption;

        staffMembers[index].UpdateName(name);
        staffMembers[index].SetHair(hs);
        staffMembers[index].SetColours(c, hID, eID);
        staffMembers[index].SetSprites(hs, ex);

        GameObject staffOb = GameObject.Find("Staff#" + index);
        SpriteRenderer[] srs = staffOb.GetComponentsInChildren<SpriteRenderer>();

        srs[0].color = staffMembers[index].GetColourByIndex(0);
        srs[1].color = staffMembers[index].GetColourByIndex(2);
        srs[2].color = staffMembers[index].GetColourByIndex(2);
        srs[3].color = staffMembers[index].GetColourByIndex(1);
        srs[4].color = staffMembers[index].GetColourByIndex(1);

        srs[3].sprite = hs;
        srs[4].sprite = ex;


        for (int i = 0; i < staffMembers.Count; i++)
        {
            GameObject staffObj = GameObject.Find("Staff#" + i);
            SpriteRenderer sr = staffObj.GetComponent<SpriteRenderer>();
            sr.color = c[0];
            staffMembers[i].UniformChanged(c[0]);
        }


        // update label
        Text[] txts = popupController.staffList.GetComponentsInChildren<Text>();
        txts[index * 2].text = name;

        Image[] imgs = popupController.staffList.GetComponentsInChildren<Image>();

        imgs[2 + (6 * index)].color = staffMembers[index].GetColourByIndex(2);
        imgs[3 + (6 * index)].color = staffMembers[index].GetColourByIndex(1);
        imgs[4 + (6 * index)].color = staffMembers[index].GetColourByIndex(1);

        if (staffMembers[index].GetHairStyle() != null)
        {
            imgs[4 + (6 * index)].sprite = staffMembers[index].GetHairStyle();
        }
        else
        {
            imgs[4 + (6 * index)].sprite = null;
            imgs[4 + (6 * index)].color = new Color(0, 0, 0, 0);
        }


        if (staffMembers[index].GetExtras() != null)
        {
            imgs[3 + (6 * index)].sprite = staffMembers[index].GetExtras();
        }
        else
        {
            imgs[3 + (6 * index)].sprite = null;
            imgs[3 + (6 * index)].color = new Color(0, 0, 0, 0);
        }


    }

    /// <summary>
    /// Create a staff member
    /// </summary>
    /// <param name="staff">The staff member object associated with the Game Object</param>
    /// <param name="xPos">The x position of the staff member</param>
    /// <param name="yPos">The y position of the staff member</param>
    void CreateStaff(StaffMember staff, int xPos, int yPos)
    {
        // get the colours to set the components to
        Color[] colours = AppearanceScript.colours;

        Vector3 pos = new Vector3(xPos, yPos);

        Transform t = staff.GetTransform();

        GameObject goStaff = (GameObject)Instantiate(t.gameObject, pos, Quaternion.identity);
        goStaff.name = "Staff#" + staff.GetIndex();
        goStaff.tag = "Staff";
        goStaff.GetComponent<mouseDrag>().staffMember = staff;

        float x = pos.x;
        float y = pos.y;

        goStaff.GetComponent<mouseDrag>().staffMember.SetVector(x, y);

        // colours[2] = skin
        // colours[1] = hair
        // colours[0] = shirt

        SpriteRenderer[] components = goStaff.GetComponentsInChildren<SpriteRenderer>();
        components[0].color = colours[0];
        components[1].color = colours[2];
        components[2].color = colours[2];
        components[3].color = colours[1];
        components[4].color = colours[1];

        // TODO: sort the colour if not 0 or 2
        components[3].sprite = AppearanceScript.hairStyle;
        components[4].sprite = AppearanceScript.extraOption;

        GameObject[] sms = GameObject.FindGameObjectsWithTag("Staff");
        for (int i = 0; i < sms.Length; i++)
        {
            sms[i].GetComponent<SpriteRenderer>().color = colours[0];
        }



        // staff menu stuff 
        GameObject go = (GameObject)Instantiate(staffInfoObject.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.SetParent(popupController.staffList, false);

        Image[] imgs = go.GetComponentsInChildren<Image>();
        Text[] txts = go.GetComponentsInChildren<Text>();


        imgs[1].color = staff.GetColourByIndex(2);
        imgs[2].color = staff.GetColourByIndex(1);
        imgs[3].color = staff.GetColourByIndex(1);

        if (staff.GetHairStyle() != null)
        {
            imgs[3].sprite = staff.GetHairStyle();
        }
        else
        {
            imgs[3].sprite = null;
            imgs[3].color = new Color(0, 0, 0, 0);
        }


        if (staff.GetExtras() != null)
        {
            imgs[2].sprite = staff.GetExtras();
        }
        else
        {
            imgs[2].sprite = null;
            imgs[2].color = new Color(0, 0, 0, 0);
        }

        txts[0].text = staff.GetStaffname();

        Button b = imgs[4].GetComponent<Button>();
        b.onClick.AddListener(() => MoveToStaffLocation(staff.GetIndex()));

        Button b2 = imgs[5].GetComponent<Button>();
        b2.onClick.AddListener(() => ViewStaffMemberInfo(staff.GetIndex()));

        staffMenuList.Add(go);

    }

    /// <summary>
    /// View staff member information for a specific member
    /// </summary>
    /// <param name="staffID">The index of the staff member to view</param>
    public void ViewStaffMemberInfo(int staffID)
    {
        statusCode = 7;
        popupController.staffMenu.gameObject.SetActive(false);
        popupController.staffMemberInfo.SetActive(true);

        selectedStaff = staffID;

        // get the details from the staff member
        StaffMember s = staffMembers[staffID];
        int[] attributes = s.GetAttributes();
        string name = s.GetStaffname();
        int dayHired = s.GetStartDay();
        Sprite sprite = s.GetTransform().GetComponent<SpriteRenderer>().sprite;

        // find the elements of the form
        Image[] imgs = popupController.staffMemberInfo.GetComponentsInChildren<Image>();
        Text[] txts = popupController.staffMemberInfo.GetComponentsInChildren<Text>();

        txts[5].text = "Edit " + name;

        txts[4].text = "Worked here since: DAY " + dayHired;

        imgs[1].sprite = sprite;

        imgs[1].color = s.GetColourByIndex(0);
        imgs[2].color = s.GetColourByIndex(2);
        imgs[5].color = s.GetColourByIndex(2);
        imgs[3].color = s.GetColourByIndex(1);
        imgs[4].color = s.GetColourByIndex(1);



        if (s.GetHairStyle() != null)
        {
            imgs[4].sprite = s.GetHairStyle();
        }
        else
        {
            imgs[4].sprite = null;
            imgs[4].color = new Color(0, 0, 0, 0);
        }


        if (s.GetExtras() != null)
        {
            imgs[3].sprite = s.GetExtras();
        }
        else
        {
            imgs[3].sprite = null;
            imgs[3].color = new Color(0, 0, 0, 0);
        }

        for (int i = 0; i < 4; i++)
        {
            imgs[9 + (i * 4)].fillAmount = 0.25f * attributes[i];
        }

    }

    /// <summary>
    /// Move the camera to the location of the staff member
    /// </summary>
    /// <param name="staffID">The ID of the staff member to move to</param>
    public void MoveToStaffLocation(int staffID)
    {
        statusCode = 50;
        Camera.main.orthographicSize = 5f;
        Vector3 pos = staffMembers[staffID].GetVector();
        pos.x = pos.x - 1.2f;
        pos.z = -10;

        Camera.main.GetComponent<CameraControls>().endPos = pos;
    }

    /// <summary>
    /// Upgrade an attribute of the staff member
    /// </summary>
    /// <param name="index">The index of the staff member to upgrade the attribute of</param>
    public void UpgradeStaffAttribute(int index)
    {
        staffMembers[selectedStaff].Upgrade(index);

        Image[] imgs = popupController.staffMemberInfo.GetComponentsInChildren<Image>();

        int attributeEffected = staffMembers[selectedStaff].GetAttributes()[index];

        imgs[9 + (index * 4)].fillAmount = 0.25f * attributeEffected;
    }

    /// <summary>
    /// Change the color of components
    /// </summary>
    /// <param name="c">Which colour to change to</param>
    /// <param name="x">The starting x coordinate</param>
    /// <param name="y">The starting y coordinate</param>
    /// <param name="width">The number of tiles to alter in the x axis - moving to the right</param>
    /// <param name="height">The number of tiles to alter in they y axis - moving up</param>
    public void ChangeColour(Color c, int x, int y, int width, int height)
    {
        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                floorTiles[i, j].GetComponent<SpriteRenderer>().color = c;
                //floorTiles[i, j].GetComponent<SpriteRenderer>().sprite = validityTiles[1];
            }
        }
    }

    /// <summary>
    /// Sell an object 
    /// </summary>
    public void SellItem()
    {
        if (ShopController.theScreens.Count > 1 || !objectSelected.Contains("Screen"))
        {
            int xPos = (int)(GetPositionOfObject().x);

            float tempY = GetPositionOfObject().y / 0.8f;

            int yPos = (int)(tempY);

            int width = OtherObject.GetWidthOfObject(tagSelected);
            int height = OtherObject.GetHeightOfObject(tagSelected);

            if (objectSelected.Contains("Screen"))
            {
                yPos -= 1;
            }

            int moneyToReturn = OtherObject.GetReturnedCoins(tagSelected, upgradeLevelSelected, foodArea);
            ConfirmationScript.OptionSelected(6, new string[] { "sell this item?", moneyToReturn.ToString(), "0", xPos.ToString(), yPos.ToString(), width.ToString(), height.ToString() }, "You will receive: ");
        }
        else
        {
            popupController.ShowPopup(3, "Uh-oh!\nYou can't sell this Screen because your cinema must have at least 1 screen!");
        }
    }

    /// <summary>
    /// Get the position of the of an object
    /// </summary>
    /// <returns>The position of the object</returns>
    public Vector2 GetPositionOfObject()
    {
        return GameObject.Find(objectSelected).transform.position;
    }

    /// <summary>
    /// Upgrade an object
    /// </summary>
    public void Upgrade()
    {
        if (objectSelected.Equals("Box Office"))
        {
            if (boxOfficeLevel < 3)
            {
                int cost = 3500;
                cost += (boxOfficeLevel - 1) * 5500;

                ConfirmationScript.OptionSelected(5, new string[] { "upgrade the box office?", cost.ToString(), "0", "0" }, "This will cost: ");
            }
            else
            {
                popupController.ShowPopup(0, "The Box Office is already fully upgraded!");
            }
        }
        else if (tagSelected.Equals("Food Area"))
        {
            String ob = objectSelected;

            popupController.HideObjectInfo();

            statusCode = 10;

            GameObject foodGO = GameObject.Find(ob);

            confirmBtn.SetActive(true);

            Camera.main.orthographicSize = 8f;
            Camera.main.GetComponent<RectTransform>().position = foodGO.transform.position + new Vector3(4.3f, 6.1f, 0);

            SpriteRenderer[] subImages = foodGO.GetComponentsInChildren<SpriteRenderer>();

            subImages[1].enabled = true;
            subImages[2].enabled = true;
            subImages[3].enabled = true;

            if (foodArea.hasHotFood) { subImages[1].color = new Color(1, 1, 1, 1); } else { subImages[1].color = new Color(0.15f, 0.15f, 0.15f, 0.5f); }
            if (foodArea.hasPopcorn) { subImages[2].color = new Color(1, 1, 1, 1); } else { subImages[2].color = new Color(0.15f, 0.15f, 0.15f, 0.5f); }
            if (foodArea.hasIceCream) { subImages[3].color = new Color(1, 1, 1, 1); } else { subImages[3].color = new Color(0.15f, 0.15f, 0.15f, 0.5f); }

            if (foodArea.tableStatus == 0)
            {
                subImages[4].color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                subImages[4].sprite = foodTableSprites[1];
            }

        }
        else {

            shopController.UpgradeScreen(objectSelected, popupController);
        }
    }

    /// <summary>
    /// Finish the construction of a screen - spend popcorn
    /// </summary>
    public void CompleteConstructionNow()
    {
        GameObject go = GameObject.Find(objectSelected);

        ScreenObject so = go.GetComponent<Screen_Script>().theScreen;
        so.UpgradeComplete();
        //change image
        go.GetComponent<SpriteRenderer>().sprite = shopController.screenImages[so.GetUpgradeLevel()];
        popupController.HideObjectInfo();

        shopController.DestroyBuilderByScreenID(so.GetScreenNumber());

        NewShowTimes();
    }

    /// <summary>
    /// When the user chooses the 'Finish construction now'
    /// </summary>
    public void FinishConstruction()
    {
        GameObject go = GameObject.Find(objectSelected);

        Screen_Script ss = go.GetComponent<Screen_Script>();

        ScreenObject so = ss.theScreen;


        int total = ss.GetUpgradeTime(so.GetUpgradeLevel());
        int done = total - so.GetDaysOfConstruction();

        int cost = (int)(1.5 * (total - done));

        ConfirmationScript.OptionSelected(12, new string[] { "Finish the work on this Screen now?", cost.ToString(), "1" }, "This will cost: ");
    }

    /// <summary>
    /// update the state of tiles
    /// </summary>
    /// <param name="x">The starting x position</param>
    /// <param name="y">The starting y position</param>
    /// <param name="w">The width of the tiles to effect (how many tiles to move x)</param>
    /// <param name="h">The height of the tiles to effect (how many tiles to move y)</param>
    /// <param name="newState">Which state to change the tiles to</param>
    public void UpdateTiles(int x, int y, int w, int h, int newState)
    {
        if (updateTileState != null)
        {
            updateTileState(x, y, w, h, newState, false);
        }
    }

    /// <summary>
    /// Changes the current tile state - WHILE MOVING AN OBJECT
    /// </summary>
    /// <param name="newState">The state to change to</param>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    /// <param name="width">How many tiles to change on the x-axis (moving right)</param>
    /// <param name="height">How many tiles to change on the y-axis (moving up)</param>
    void SetTiles(int newState, int x, int y, int width, int height)
    {
        Color newColour;

        if (newState != 0) { newColour = carpetColour; } else { newColour = Color.green; }

        ChangeColour(newColour, x, y, width, height);

        UpdateTiles(x, y, width, height, newState);
    }

    /// <summary>
    /// Get a colour based on the id of it
    /// </summary>
    /// <param name="id">The id to get a colour for</param>
    /// <returns>The colour associated with the id</returns>
    Color GetColourFromID(int id)
    {
        // return the relevant colour based on the id
        switch (id)
        {
            case 0: return new Color(0.663f, 0.149f, 0.149f);
            case 1: return new Color(0.001f, 0.345f, 0.663f);
            case 2: return new Color(0.098f, 0.361f, 0.098f);
            case 3: return new Color(0.325f, 0.082f, 0.267f);
            case 5: return new Color(0.333f, 0.332f, 0.494f);
            case 6: return new Color(0.839f, 0.749f, 0.376f);
            case 7: return new Color(0.573f, 0.376f, 0.165f);
            default: return new Color(100, 100, 100);
        }
    }

    /// <summary>
    /// Get a marble floor sprite based on an id
    /// </summary>
    /// <param name="id">The id of the required sprite</param>
    /// <returns>The sprites to use</returns>
    Sprite[] GetSpriteFromID(int id)
    {
        switch (id)
        {
            case 4: isMarbleFloor = true; return marbleSquares;
            default: isMarbleFloor = false; return new Sprite[] { ColourBackground };
        }
    }

    /// <summary>
    /// When a colour (carpet menu) has been selected)
    /// </summary>
    /// <param name="id"></param>
    public void colourClicked(int id)
    {
        carpetColour = GetColourFromID(id);
        Sprite[] s = GetSpriteFromID(id);

        // check if the script is already running
        if (CarpetRollScript.shouldRun)
        {
            // if so, finish the current placement
            CarpetRollScript.current.FinishPlacement();
        }

        CarpetRollScript.current.Begin(carpetColour, this, s);
    }

    /// <summary>
    /// Reset the position of the staff
    /// </summary>
    void ResetStaff()
    {
        GameObject[] staffObjects = GameObject.FindGameObjectsWithTag("Staff");

        for (int i = 0; i < staffObjects.Length; i++)
        {

            int x = 35 + (2 * (i % 6));
            int y = (2 * (i / 6));

            Transform t = staffObjects[i].transform;
            t.position = new Vector3(x, y, 0);

            UpdateStaffJob(i, 0, 0, false);

            staffObjects[i].GetComponent<SpriteRenderer>().color = staffMembers[i].GetColourByIndex(0);

            //try
            //{
            Transform pi3 = staffObjects[i].transform.FindChild("hiddenPointer");
            pi3.GetComponent<SpriteRenderer>().enabled = false;
            staffMembers[i].SetVector(x, y);
            //}
            //catch (Exception) { }
            //t.Translate(new Vector3(10, o0, 0));

        }
    }

    /// <summary>
    /// Start the business day
    /// </summary>
    public void StartDay()
    {
        // check if any screens available
        bool screensAvailable = false;

        for (int i = 0; i < ShopController.theScreens.Count; i++)
        {
            if (!ShopController.theScreens[i].ConstructionInProgress())
            {
                screensAvailable = true;
                break;
            }
        }

        // if none, 
        if (!screensAvailable)
        {
            popupController.ShowPopup(0, "None of your screens are available today - they are all under construction. No customers will arrive and you will receive 0 coins for this day");

            NextDay(false, true);
            return;
        }
        else
        {

            if (!simulationRunning)
            {
                // start the running of the 'day'
                simulationRunning = true;
            }

            cmdFriends.SetActive(false);

            ticketQueue.Begin();

            if (foodQueue != null)
            {
                foodQueue.Begin();
            }

            customerController.ResetPaths();

            // find paths to allScreens
            for (int i = 0; i < ShopController.theScreens.Count; i++)
            {
                List<Coordinate> points = TileManager.floor.FindPath(38, 11, ShopController.theScreens[i].GetX() + 5, ShopController.theScreens[i].GetY());
                customerController.AddScreenPath(points);

                if (foodArea != null)
                {
                    GameObject foodCourt = GameObject.FindGameObjectWithTag("Food Area");

                    points = TileManager.floor.FindPath((int)foodCourt.transform.position.x + 3, (int)(foodCourt.transform.position.y / 0.8f) + 5, ShopController.theScreens[i].GetX() + 5, ShopController.theScreens[i].GetY());
                    customerController.AddFoodToScreenPath(points);
                }

            }
            if (foodArea != null)
            {
                //foodQueue = null;

                GameObject foodCourt = GameObject.FindGameObjectWithTag("Food Area");

                List<Coordinate> newPoints = TileManager.floor.FindPath(38, 11, (int)foodCourt.transform.position.x + 3, (int)(foodCourt.transform.position.y / 0.8f) + 5);

                customerController.SetTicketsToFoodPath(newPoints);

            }

            // hide the buttons and menus
            popupController.HideObjectInfo();
            startDayButton.SetActive(false);
            shopButton.gameObject.SetActive(false);
            staffMenuButton.gameObject.SetActive(false);
            popupController.colourPicker.SetActive(false);
            //staffMemberInfo.SetActive(false);
            //staffMenu.gameObject.SetActive(false);

            Customer.tiles = floorTiles;

            // Optimise Object Pool

            // check if the current size is enough
            int currObjectCount = ObjectPool.current.pooledObjects.Count;

            // if there are not enough objects, add some so that there are enough
            if (currObjectCount < customerController.allCustomers.Count / 2.2)
            {
                for (int i = ObjectPool.current.pooledObjects.Count; i < customerController.allCustomers.Count / 2.2; i++)
                {
                    ObjectPool.current.AddNewItem();
                }
            }

            GameObject[] staffs = GameObject.FindGameObjectsWithTag("Staff");
            for (int i = 0; i < staffs.Length; i++)
            {
                QuestionScript qs = staffs[i].GetComponent<QuestionScript>();
                qs.Begin(staffMembers[i]);
            }


        }

    }

    /// <summary>
    /// Move on to the next day
    /// </summary>
    /// <param name="shouldCollect">Whether or not earnings should be collected for that day</param>
    public void NextDay(bool shouldCollect, bool shouldSave)
    {
        // update the reputation fields

        int questionCount = 0;

        for (int i = 0; i < staffMembers.Count; i++)
        {
            questionCount += staffMembers[i].GetQuestionCount() / 100;
        }


        // use questions to effect staff

        // do the same for projectors and facilities

        customerController.reputation.SetStaffQuestionSpeed(questionCount);
        customerController.reputation.SetFacilities(ShopController.theScreens, shopController.hasUnlockedRedCarpet, foodArea);
        customerController.reputation.SetPublicityRating(shopController.postersUnlocked);
        customerController.reputation.SetStaffRating(staffMembers);


        for (int i = 0; i < ShopController.theScreens.Count; i++)
        {
            ShopController.theScreens[i].ProgressOneDay();
            ShopController.theScreens[i].ResetClicks();
        }

        projectorController.ClearAllProjectors();

        ticketQueue.End();
        ticketQueue.Clear();

        if (foodArea != null)
        {
            foodQueue.End();
            foodQueue.Clear();
        }

        // stop the questions from running
        GameObject[] staffs = GameObject.FindGameObjectsWithTag("Staff");
        for (int i = 0; i < staffs.Length; i++)
        {
            staffs[i].GetComponent<QuestionScript>().End();
        }


        for (int i = 0; i < staffMembers.Count; i++)
        {
            staffMembers[i].ResetQuestionCount();
        }


        if (shouldCollect)
        {
            int money = GetTodaysMoney();

            financeController.AddCoins(money);
            customerController.reputation.AddCoins(money);

            int popcorn = 0;

            // work out if popcorn should be added
            if (foodArea != null && foodArea.hasPopcorn)
            {
                int rand = UnityEngine.Random.Range(0, 100);

                if (rand < 7)
                {

                    int randVal = UnityEngine.Random.Range(0, 100);

                    if (randVal < 8)
                    {
                        popcorn = 3;
                    }
                    else if (randVal < 22)
                    {
                        popcorn = 2;
                    }
                    else
                    {
                        popcorn = 1;
                    }

                    financeController.AddPopcorn(popcorn);
                }
            }


            int oldOverall = customerController.reputation.GetOverall();
            customerController.reputation.SetOverall();

            int newOverall = customerController.reputation.GetOverall();

            int repChange = newOverall - oldOverall;

            popupController.ShowEndOfDayPopup(money, customerController.numWalkouts, repChange, customerController.customersServed, popcorn);
        }

        simulationRunning = false;

        queueCount = 0;

        //ticketQueue.Clear();

        // reset the layer of each customer transform 
        for (int i = 0; i < ObjectPool.current.pooledObjects.Count; i++)
        {
            SpriteRenderer sr = ObjectPool.current.pooledObjects[i].GetComponent<SpriteRenderer>();

            sr.sortingLayerName = "Front";
            sr.sortingOrder = 70;
        }

        customerController.allCustomers.Clear();

        startDayButton.SetActive(true);
        shopButton.gameObject.SetActive(true);
        staffMenuButton.gameObject.SetActive(true);


        currDay++;


        if (currDay % 7 == 1)
        {
            NextWeek();
        }

        //if (shouldCollect) {
        for (int i = 0; i < ShopController.theScreens.Count; i++)
        {

            if (ShopController.theScreens[i].GetDaysOfConstruction() + 1 == 1 && ShopController.theScreens[i].GetDaysOfConstruction() == 0 && shopController.screenObjectList.Count > 0)
            {

                shopController.screenObjectList[i].GetComponent<SpriteRenderer>().sprite = shopController.screenImages[ShopController.theScreens[i].GetUpgradeLevel()];
                NewShowTimes();

                for (int k = 0; k < filmShowings.Count; k++)       // filmShowings.Count
                {
                    int index = filmShowings[k].GetScreenNumber();
                    int ticketsSold = GetTicketsSoldValue(ShopController.theScreens[index - 1]);
                    filmShowings[k].SetTicketsSold(ticketsSold);

                    int currentCount = 0;

                    for (int j = 0; j < k; j++)
                    {
                        currentCount += filmShowings[j].GetTicketsSold();
                    }

                    shopController.DestroyBuilderByScreenID(filmShowings[k].GetScreenNumber());

                }
            }
        }
        //}

        for (int i = 0; i < filmShowings.Count; i++)       // filmShowings.Count
        {
            int index = filmShowings[i].GetScreenNumber();
            int ticketsSold = GetTicketsSoldValue(ShopController.theScreens[index - 1]);
            filmShowings[i].SetTicketsSold(ticketsSold);

            int currentCount = 0;

            for (int j = 0; j < i; j++)
            {
                currentCount += filmShowings[j].GetTicketsSold();
            }

        }

        // update day output 
        dayLabel.text = "DAY: " + currDay.ToString();

        movementScript.customerStatus.SetActive(false);

        //statusCode = 0;

        if (facebookProfile != null && facebookProfile.id.Length > 0)
        {
            cmdFriends.SetActive(true);
        }

        ResetStaff();


        // reset the state of each slot
        for (int i = 0; i < slotState.Count; i++)
        {
            slotState[i] = false;
        }

        customerController.ResetCounts();

        if (shouldSave)
        {
            string persPath = Application.persistentDataPath;
            Thread thr = new Thread(() => Save(persPath));
            thr.Start();
        }

    }

    /// <summary>
    /// TEMPORARY - for the admin test button
    /// </summary>
    public void SkipDay()
    {
        NextDay(false, true);
    }

    /// <summary>
    /// After 7 days, move to next week - new show times etc
    /// </summary>
    void NextWeek()
    {
        NewShowTimes();
    }

    /// <summary>
    /// Calculate the number of coins that were generated today
    /// </summary>
    /// <returns>The number of coins earnt</returns>
    private int GetTodaysMoney()
    {
        int totalIntake = customerController.customerMoney;

        // each ticket is 2 coins + 1 for each upgrade level
        for (int i = 0; i < filmShowings.Count; i++)
        {
            int screenNum = filmShowings[i].GetScreenNumber() - 1;

            if (!ShopController.theScreens[screenNum].ConstructionInProgress())
            {
                int upgradeLevel = ShopController.theScreens[screenNum].GetUpgradeLevel();
                int numCustomers = filmShowings[i].GetTicketsSold();

                #region 3D glasses income!
                int moneyFor3dGlasses = 0;

                if (upgradeLevel == 4)
                {
                    int random = UnityEngine.Random.Range(0, numCustomers);
                    moneyFor3dGlasses += 2 * random;
                }
                #endregion

                totalIntake += moneyFor3dGlasses;
            }
        }

        GameObject[] vendingMachines = GameObject.FindGameObjectsWithTag("Vending Machine");

        // each vending machine will generate between 10 and 50 coins each day
        int vendingAmount = UnityEngine.Random.Range(10, 50);

        int vendingMachineIncome = vendingAmount * vendingMachines.Length;

        totalIntake += vendingMachineIncome;

        return totalIntake;
    }

    /// <summary>
    /// Get the value of tickets sold for a specific screen
    /// </summary>
    /// <param name="screen">The Screen object to get the tickets for</param>
    /// <returns>The value of tickets sold</returns>
    public int GetTicketsSoldValue(ScreenObject screen)
    {
        UnityEngine.Random ran = new UnityEngine.Random();
        int min = (int)(screen.GetNumSeats() / 1.5);  // this will be affected by the posters etc, and rep
        int max = screen.GetNumSeats();
        int ticketsSold = UnityEngine.Random.Range(min, max);
        float repMultiplier = customerController.reputation.GetMultiplier();
        ticketsSold = 3 + (int)(ticketsSold * repMultiplier);

        return ticketsSold;
    }

    /// <summary>
    /// Get a show time for a showing
    /// </summary>
    /// <param name="i">First, second, or third showing</param>
    /// <returns>The time of the showing</returns>
    private TimeTuple GetShowTime(int i)
    {
        int startTime = UnityEngine.Random.Range(0, 5);

        int hours;
        int minutes;

        int totalMinutes = 30 * startTime;

        hours = 10 + (totalMinutes / 60);
        minutes = totalMinutes - ((hours - 10) * 60);

        int minutesToAddOn = i * 180;
        int hoursToAdd = minutesToAddOn / 60;

        hours += hoursToAdd;
        minutes += minutesToAddOn - (hoursToAdd * 60);



        TimeTuple toReturn = new TimeTuple(hours, minutes);

        return toReturn;

    }

    /// <summary>
    /// Generate new show times
    /// </summary>
    public void NewShowTimes()
    {
        customerController.allCustomers.Clear();
        filmShowings.Clear();

        for (int i = 0; i < ShopController.theScreens.Count; i++)
        {
            if (!ShopController.theScreens[i].ConstructionInProgress())
            {
                int screeningsThisDay = UnityEngine.Random.Range(2, 4); // number of films per screen per day

                for (int j = 0; j < screeningsThisDay; j++)     // screeningsThisDay
                {
                    TimeTuple showTime = GetShowTime(j);

                    FilmShowing newFilm = new FilmShowing(filmShowings.Count, i + 1, 0, showTime.hours, showTime.minutes, TileManager.floor);
                    int ticketsSold = GetTicketsSoldValue(ShopController.theScreens[i]);
                    newFilm.SetTicketsSold(ticketsSold);
                    filmShowings.Add(newFilm);

                    List<Customer> custs = newFilm.CreateCustomerList(customerController.allCustomers.Count, this);
                    customerController.allCustomers.AddRange(custs);

                }
            }
        }
    }

    /// <summary>
    /// When the object move has been complete
    /// </summary>
    /// <param name="confirmed">Whether the move was confirmed (true) or cancelled (false)</param>
    public void ObjectMoveComplete(bool confirmed)
    {
        statusCode = 0;

        //objectInfo.SetActive(true);

        // re-place image

        int x = -1; int y = -1;
        bool newObject = !(theTileManager.origX > -1);

        if (confirmed)
        {

            x = theTileManager.toMoveX;
            y = theTileManager.toMoveY;

        }
        else
        {
            x = theTileManager.origX;
            y = theTileManager.origY;
        }

        if (!newObject)
        {
            ReShowStaffAndBuildings();

            string[] tmp = objectSelected.Split('#');
            int id = int.Parse(tmp[1]);

            Transform newItem = null;

            if (itemToAddID == 0)
            {
                id -= 1;

                Vector3 pos = new Vector3(x, y * 0.8f, 0);

                ShopController.theScreens[id].SetPosition(x, y);

                pos.y += 0.8f;

                ScreenObject temp = null;

                GameObject theScreen = GameObject.Find("Screen#" + ShopController.theScreens[id].GetScreenNumber());
                temp = theScreen.GetComponent<Screen_Script>().theScreen;

                theScreen.GetComponent<SpriteRenderer>().sortingOrder = height - y - 1;
                theScreen.transform.position = pos;
                if (temp.ConstructionInProgress())
                {
                    theScreen.GetComponent<SpriteRenderer>().sprite = shopController.screenImages[0];
                }

                theScreen.GetComponent<Renderer>().enabled = true;

                GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");
                if (temp.ConstructionInProgress())
                {
                    for (int i = 0; i < builders.Length; i++)
                    {
                        if (builders[i].name.Contains(temp.GetScreenNumber().ToString()))
                        {
                            builders[i].transform.position = new Vector2(temp.GetX() + 1.8f, 0.7f + (0.8f * temp.GetY()));
                        }
                    }
                }


                // check staff position
                CheckStaffPosition(theScreen);

                objectSelected = "";
                tagSelected = "";
                upgradeLevelSelected = 0;

                for (int i = 0; i < staffMembers.Count; i++)
                {
                    staffMembers[i].GetTransform().position = new Vector3(staffMembers[i].GetTransform().position.x, staffMembers[i].GetTransform().position.y, -0.4f);
                }


            }
            else
            {

                Vector3 pos = new Vector3(x, y * 0.8f, 0);

                ShopController.otherObjects[id - 1].xPos = x;
                ShopController.otherObjects[id - 1].yPos = y;

                GameObject theObject = shopController.AddObject(pos, id, height, itemToAddID, false);

                SpriteRenderer[] subImages = theObject.GetComponentsInChildren<SpriteRenderer>();

                for (int j = 0; j < subImages.Length; j++)
                {
                    subImages[j].GetComponent<SpriteRenderer>().enabled = true;
                }

                try
                {

                    subImages[1].enabled = foodArea.hasHotFood;
                    subImages[2].enabled = foodArea.hasPopcorn;
                    subImages[3].enabled = foodArea.hasIceCream;

                    int baseOrder = subImages[0].sortingOrder - 6;

                    subImages[0].sortingOrder = baseOrder;
                    subImages[1].sortingOrder = baseOrder + 1;
                    subImages[2].sortingOrder = baseOrder + 1;
                    subImages[3].sortingOrder = baseOrder + 1;
                    subImages[4].sortingOrder = baseOrder + 1;
                    subImages[5].sortingOrder = baseOrder - 1;


                    GameObject[] go = GameObject.FindGameObjectsWithTag("Staff Slot 2");

                    // todo this

                }
                catch (Exception) { }


                if (tagSelected.Equals("Food Area"))
                {
                    // move the slots
                    GameObject[] slots = GameObject.FindGameObjectsWithTag("Slot Type 2");

                    slots[0].transform.position = theObject.transform.position + new Vector3(3, 7.95f, 0);
                    try
                    {
                        slots[1].transform.position = theObject.transform.position + new Vector3(5.2f, 7.95f, 0);
                    }
                    catch (Exception) { }

                    int posInLayer = 0;

                    // move the staff members who are on that post
                    for (int i = 0; i < staffMembers.Count; i++)
                    {
                        if (staffMembers[i].GetJobID() == 2)
                        {
                            UpdateStaffJob(i, 2, posInLayer, true);
                            staffMembers[i].GetTransform().position = slots[posInLayer].transform.position - new Vector3(0, 1.1f, 1);
                            posInLayer++;
                        }
                    }
                }




                // check staff position
                CheckStaffPosition(theObject);




            }
            itemToAddID = -1;

            SetTiles(2, x, y, theTileManager.fullWidth, theTileManager.fullHeight);

            if (!confirmed)
            {
                SetTiles(0, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);
                ChangeColour(carpetColour, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);

                SetTiles(2, theTileManager.origX, theTileManager.origY, theTileManager.fullWidth, theTileManager.fullHeight);

            }

            CheckForPath();
            theTileManager.ResetStatusVariables();

            statusCode = 0;
            popupController.confirmMovePanel.SetActive(false);
            popupController.moveButtons.SetActive(false);


            ReShowStaffAndBuildings();

            shopController.redCarpet.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);

            // show food area
            GameObject foodPlace = GameObject.FindGameObjectWithTag("Food Area");
            if (foodPlace != null)
            {
                SpriteRenderer[] foodAreaRenderers = foodPlace.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sr in foodAreaRenderers)
                {
                    sr.color = new Color(1, 1, 1, 1);
                }
            }

            objectSelected = "";
            tagSelected = "";
            upgradeLevelSelected = 0;

        }
        else if (confirmed)
        {
            String[] parameters = new String[5];
            parameters[0] = "Add this object?";
            parameters[1] = OtherObject.GetCost(objectSelected).ToString();
            parameters[2] = OtherObject.GetCurrency(objectSelected);
            parameters[3] = theTileManager.toMoveX.ToString();
            parameters[4] = theTileManager.toMoveY.ToString();
            ConfirmationScript.OptionSelected(0, parameters, "This will cost: ");

        }
        else {
            ChangeColour(carpetColour, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);

            theTileManager.ResetStatusVariables();

            popupController.confirmMovePanel.SetActive(false);
            popupController.moveButtons.SetActive(false);

            statusCode = 0;

            ReShowStaffAndBuildings();

            objectSelected = "";
            tagSelected = "";
            upgradeLevelSelected = 0;
        }
    }

    /// <summary>
    /// Show all the building and staff members (after they were hidden while moving objects)
    /// </summary>
    public void ReShowStaffAndBuildings()
    {
        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");
        for (int i = 0; i < staff.Length; i++)
        {
            SpriteRenderer[] srs = staff[i].GetComponentsInChildren<SpriteRenderer>();
            for (int j = 0; j < 5; j++)
            {
                srs[j].enabled = true;
            }

            // sort z position
            staff[i].transform.position = new Vector3(staff[i].transform.position.x, staff[i].transform.position.y, -1);

        }
        GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");
        for (int i = 0; i < builders.Length; i++)
        {
            builders[i].GetComponent<SpriteRenderer>().enabled = true;
        }

        shopController.ReShowObjects();

        GameObject foodPlace = GameObject.FindGameObjectWithTag("Food Area");
        if (foodPlace != null)
        {
            SpriteRenderer[] foodAreaRenderers = foodPlace.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in foodAreaRenderers)
            {
                sr.color = new Color(1, 1, 1, 1);
            }
        }

        statusCode = 0;

    }

    /// <summary>
    /// Add a new object to the cinema
    /// </summary>
    /// <param name="x">The x position to add it in</param>
    /// <param name="y">The y position to add it in</param>
    public void AddNewObject(int x, int y)
    {
        popupController.confirmMovePanel.SetActive(false);
        popupController.moveButtons.SetActive(false);

        Vector3 pos = new Vector3(x, y * 0.8f, 0);


        //float xCorrection = 0;
        //float yCorrection = 0;

        if (itemToAddID == 0)
        {
            //xCorrection = 4.6f;
            //yCorrection = 6.05f;

            int newID = ShopController.theScreens.Count;
            ScreenObject aScreen = new ScreenObject(newID + 1, 0);
            aScreen.SetPosition(x, y);
            aScreen.Upgrade();
            ShopController.theScreens.Add(aScreen);

            //pos.x += xCorrection;
            //pos.y += yCorrection;
            pos.y += 0.8f; // gap at bottom

            shopController.AddScreen(ShopController.theScreens[newID], pos, height);

            GameObject screenThing = shopController.AddScreen(ShopController.theScreens[newID], pos, height);

            // check staff position
            CheckStaffPosition(screenThing);

            for (int i = 0; i < staffMembers.Count; i++)
            {
                staffMembers[i].GetTransform().Translate(new Vector3(0, 0, -1));
            }
        }
        else {

            if (itemToAddID == 7)
            {
                foodArea = new FoodArea();
                foodArea.hasHotFood = true;     // give them 1 thing to start with

                foodQueue = new CustomerQueue(70, x + 3, ((y + 4) * 0.8f) - 1, 1);

            }

            OtherObject oo = new OtherObject(x, y, itemToAddID, ShopController.otherObjects.Count + 1);

            ShopController.otherObjects.Add(oo);

            GameObject theObject = shopController.AddObject(pos, ShopController.otherObjects.Count, height, itemToAddID, true);

            // check staff position
            CheckStaffPosition(theObject);

            for (int i = 0; i < staffMembers.Count; i++)
            {
                staffMembers[i].GetTransform().Translate(new Vector3(0, 0, -1));
            }


            SpriteRenderer[] subImages = theObject.GetComponentsInChildren<SpriteRenderer>();

            try
            {
                subImages[1].enabled = foodArea.hasHotFood;
                subImages[2].enabled = foodArea.hasPopcorn;
                subImages[3].enabled = foodArea.hasIceCream;
                subImages[4].enabled = true;
                subImages[5].enabled = true;

                int baseOrder = subImages[0].sortingOrder - 6;
                subImages[0].sortingOrder = baseOrder;
                subImages[1].sortingOrder = baseOrder + 1;
                subImages[2].sortingOrder = baseOrder + 1;
                subImages[3].sortingOrder = baseOrder + 1;
                subImages[4].sortingOrder = baseOrder + 1;
                subImages[5].sortingOrder = baseOrder - 1;

                OtherObjectScript.CreateStaffSlot(2, theObject.transform.position + new Vector3(3, 7.95f, 0));
                NewShowTimes();
            }
            catch (Exception) { }
        }
        itemToAddID = -1;

        SetTiles(2, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);
        ChangeColour(carpetColour, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);

        CheckForPath();

        theTileManager.ResetStatusVariables();

        statusCode = 0;

        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");
        for (int i = 0; i < staff.Length; i++)
        {
            SpriteRenderer[] srs = staff[i].GetComponentsInChildren<SpriteRenderer>();
            for (int j = 0; j < 4; j++)
            {
                srs[j].enabled = true;
            }

            // sort z position
            staff[i].transform.position = new Vector3(staff[i].transform.position.x, staff[i].transform.position.y, -1);

        }

        ReShowStaffAndBuildings();

        GameObject foodPlace = GameObject.FindGameObjectWithTag("Food Area");
        if (foodPlace != null)
        {
            SpriteRenderer[] foodAreaRenderers = foodPlace.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in foodAreaRenderers)
            {
                sr.color = new Color(1, 1, 1, 1);
            }
        }

        objectSelected = "";
        tagSelected = "";
        upgradeLevelSelected = 0;
    }

    /// <summary>
    /// Check for paths to all the screens
    /// </summary>
    public void CheckForPath()
    {
        List<int> unreachableScreens = theTileManager.IsPathAvailable();

        if (unreachableScreens.Count > 0)
        {
            String screenList = "";
            for (int i = 0; i < unreachableScreens.Count; i++)
            {
                screenList += "Screen " + unreachableScreens[i] + "\n";
            }

            popupController.warningIcon.enabled = true;

            popupController.warningLabel.text = warning1 + screenList + warning2;
            Debug.Log("CAUTION: " + screenList + " ARE UNREACHABLE. YOUR CUSTOMERS FOR THIS SCREEN WILL NOT REACH THE SCREEN AND WILL LEAVE - RUINING THE REPUTATION OF YOUR FINE CINEMA!");
        }
        else
        {
            popupController.warningIcon.enabled = false;
        }
        popupController.warningPanel.SetActive(false);
    }

    /// <summary>
    /// Check the position of the staff members (after an object move)
    /// </summary>
    /// <param name="go">The game object which was moved</param>
    void CheckStaffPosition(GameObject go)
    {
        Bounds objectBounds;

        if (go.tag != "Screen")
        {
            objectBounds = mouseDrag.GetObjectHiddenBounds(go);

            for (int i = 0; i < staffMembers.Count; i++)
            {
                // check it
                staffMembers[i].GetTransform().Translate(new Vector3(0, 0, 1));
                //Bounds staffBounds = staffMembers[i].GetTransform().GetComponent<Renderer>().bounds;
                Bounds staffBounds = new Bounds(staffMembers[i].GetTransform().position, new Vector3(1, 1, 1));

                if (objectBounds.Intersects(staffBounds))
                {
                    staffMembers[i].GetTransform().GetComponent<mouseDrag>().SortStaffLayer(go);
                    // show the arrow
                    staffMembers[i].GetTransform().GetComponentsInChildren<SpriteRenderer>()[4].enabled = true;
                }
                else
                {
                    GameObject hiddenBehind = mouseDrag.CheckHiddenBehind(staffBounds, shopController.gameObjectList, shopController.screenObjectList);

                    if (hiddenBehind == null)
                    {
                        staffMembers[i].GetTransform().GetComponent<Renderer>().sortingOrder = 40;

                        // sort the colours
                        SpriteRenderer[] subImages = staffMembers[i].GetTransform().GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer sr in subImages)
                        {
                            sr.sortingOrder = TileManager.floor.height + 7;
                        }
                        subImages[3].sortingOrder++;
                        subImages[0].sortingOrder--;
                        subImages[0].color = staffMembers[i].GetColourByIndex(0);
                        subImages[1].color = staffMembers[i].GetColourByIndex(2);
                        subImages[2].color = staffMembers[i].GetColourByIndex(2);
                        subImages[3].color = staffMembers[i].GetColourByIndex(1);



                        staffMembers[i].GetTransform().FindChild("hiddenPointer").GetComponent<SpriteRenderer>().enabled = false;
                    }
                    else
                    {
                        // show the arrow
                        staffMembers[i].GetTransform().GetComponentsInChildren<SpriteRenderer>()[4].enabled = true;
                    }

                }
            }
        }
        else
        {
            Bounds b1 = go.GetComponent<Renderer>().bounds;
            Bounds bTop, bBottom;

            String name = go.name;



            int id = int.Parse(name.Split('#')[1]) - 1;

            if (!ShopController.theScreens[id].ConstructionInProgress())
            {

                bTop = new Bounds(new Vector3(b1.center.x, (b1.center.y - b1.extents.y + 8.8f), 0), 2 * new Vector3(b1.extents.x, 1.8f, 0.125f));
                bBottom = new Bounds(new Vector3(b1.center.x, (b1.center.y - b1.extents.y + 0.9f), 0), 2 * new Vector3(b1.extents.x, 0.65f, 0.1f));
            }
            else
            {
                bTop = b1;
                bBottom = b1;
            }

            for (int i = 0; i < staffMembers.Count; i++)
            {
                // check it
                staffMembers[i].GetTransform().Translate(new Vector3(0, 0, 1));
                //Bounds staffBounds = staffMembers[i].GetTransform().GetComponent<Renderer>().bounds;
                Bounds staffBounds = new Bounds(staffMembers[i].GetTransform().position, new Vector3(1, 1, 1));


                if (bTop.Intersects(staffBounds) || bBottom.Intersects(staffBounds))
                {
                    staffMembers[i].GetTransform().GetComponent<mouseDrag>().SortStaffLayer(go);
                    // show the arrow
                    staffMembers[i].GetTransform().GetComponentsInChildren<SpriteRenderer>()[4].enabled = true;
                }
                else
                {
                    GameObject hiddenBehind = mouseDrag.CheckHiddenBehind(staffBounds, shopController.gameObjectList, shopController.screenObjectList);

                    if (hiddenBehind == null)
                    {
                        staffMembers[i].GetTransform().GetComponent<Renderer>().sortingOrder = 40;


                        // sort the colours
                        SpriteRenderer[] subImages = staffMembers[i].GetTransform().GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer sr in subImages)
                        {
                            sr.sortingOrder = TileManager.floor.height + 7;
                        }
                        subImages[3].sortingOrder++;
                        subImages[0].sortingOrder--;
                        subImages[0].color = staffMembers[i].GetColourByIndex(0);
                        subImages[1].color = staffMembers[i].GetColourByIndex(2);
                        subImages[2].color = staffMembers[i].GetColourByIndex(2);
                        subImages[3].color = staffMembers[i].GetColourByIndex(1);


                        staffMembers[i].GetTransform().FindChild("hiddenPointer").GetComponent<SpriteRenderer>().enabled = false;
                    }
                    else
                    {
                        // show the arrow
                        staffMembers[i].GetTransform().GetComponentsInChildren<SpriteRenderer>()[4].enabled = true;
                    }
                }
            }

        }




    }

    /// <summary>
    /// Move an object
    /// </summary>
    public void MoveScreen()
    {

        for (int i = 0; i < staffMembers.Count; i++)
        {
            staffMembers[i].GetTransform().position = new Vector3(staffMembers[i].GetTransform().position.x, staffMembers[i].GetTransform().position.y, 0);
        }

        shopController.SemiTransparentObjects();

        // ghost the food area
        GameObject foodPlace = GameObject.FindGameObjectWithTag("Food Area");
        if (foodPlace != null)
        {
            SpriteRenderer[] foodAreaRenderers = foodPlace.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in foodAreaRenderers)
            {
                sr.color = new Color(1, 1, 1, 0.25f);
            }
        }

        // hide staff
        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");

        for (int i = 0; i < staff.Length; i++)
        {

            SpriteRenderer[] srs = staff[i].GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in srs)
            {
                sr.enabled = false;
            }
            //srs[4].enabled = false;
        }

        GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");

        for (int i = 0; i < builders.Length; i++)
        {
            builders[i].GetComponent<SpriteRenderer>().enabled = false;
        }

        GameObject[] pointers = GameObject.FindGameObjectsWithTag("Pointer");

        for (int i = 0; i < pointers.Length; i++)
        {
            pointers[i].GetComponent<SpriteRenderer>().enabled = false;
        }


        // hide / show necessary buttons
        popupController.confirmMovePanel.SetActive(true);
        popupController.moveButtons.SetActive(true);
        popupController.objectInfo.SetActive(false);
        popupController.closeInfo.SetActive(false);

        statusCode = 2;

        for (int i = 0; i < shopController.screenObjectList.Count; i++)
        {
            if (shopController.screenObjectList[i].name.Equals(objectSelected))
            {
                shopController.screenObjectList[i].GetComponent<Renderer>().enabled = false;

                String[] tmp = shopController.screenObjectList[i].name.Split('#');
                int index = -1;

                for (int j = 0; j < ShopController.theScreens.Count; j++)
                {
                    if (ShopController.theScreens[j].GetScreenNumber() == int.Parse(tmp[1]))
                    {
                        index = j;
                        break;
                    }
                }

                int x = ShopController.theScreens[index].GetX(); //-4
                int y = ShopController.theScreens[index].GetY(); //-6


                itemToAddID = 0;

                theTileManager.toMoveX = x;
                theTileManager.toMoveY = y;

                theTileManager.origX = x;
                theTileManager.origY = y;

                theTileManager.fullWidth = 11;
                theTileManager.fullHeight = 15;

                SetTiles(0, x, y, 11, 15);

                break;
            }
        }
        for (int i = 0; i < shopController.gameObjectList.Count; i++)
        {
            if (shopController.gameObjectList[i].name.Equals(objectSelected))
            {
                shopController.gameObjectList[i].GetComponent<Renderer>().enabled = false;
                SpriteRenderer[] subImages = shopController.gameObjectList[i].GetComponentsInChildren<SpriteRenderer>();

                for (int j = 0; j < subImages.Length; j++)
                {
                    subImages[j].GetComponent<SpriteRenderer>().enabled = false;
                }

                int x = ShopController.otherObjects[i].xPos; //-4
                int y = ShopController.otherObjects[i].yPos; //-6


                int w = 0;
                int h = 0;

                if (shopController.gameObjectList[i].tag.Equals("Plant"))
                {
                    itemToAddID = 2;
                    w = 1; h = 1;
                }
                else if (shopController.gameObjectList[i].tag.Equals("Bust"))
                {
                    itemToAddID = 3;
                    w = 2; h = 3;
                }
                else if (shopController.gameObjectList[i].tag.Equals("Vending Machine"))
                {
                    itemToAddID = 5;
                    w = 3; h = 3;
                }
                else if (shopController.gameObjectList[i].tag.Equals("Food Area"))
                {
                    itemToAddID = 7;
                    w = 10; h = 18;
                }

                theTileManager.toMoveX = x;
                theTileManager.toMoveY = y;

                theTileManager.origX = x;
                theTileManager.origY = y;

                theTileManager.fullWidth = w;
                theTileManager.fullHeight = h;

                SetTiles(0, x, y, w, h);

                break;
            }
        }

    }

    /// <summary>
    /// Place a new object
    /// </summary>
    /// <param name="width">The width of the object (number of tiles)</param>
    /// <param name="height">The height of the object (number of tiles)</param>
    public void PlaceObject(int width, int height)
    {

        int startX = (int)Camera.main.transform.position.x;
        int startY = (int)Camera.main.transform.position.y;

        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");

        for (int i = 0; i < staff.Length; i++)
        {

            SpriteRenderer[] srs = staff[i].GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in srs)
            {
                sr.enabled = false;
            }
            //srs[4].enabled = false;
        }

        GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");

        for (int i = 0; i < builders.Length; i++)
        {
            builders[i].GetComponent<SpriteRenderer>().enabled = false;
        }



        // ghost the food area
        GameObject foodPlace = GameObject.FindGameObjectWithTag("Food Area");
        if (foodPlace != null)
        {
            SpriteRenderer[] foodAreaRenderers = foodPlace.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in foodAreaRenderers)
            {
                sr.color = new Color(1, 1, 1, 0.25f);
            }
        }


        statusCode = 2;

        popupController.shopCanvas.SetActive(false);
        popupController.confirmMovePanel.SetActive(true);
        popupController.moveButtons.SetActive(true);

        bool valid = theTileManager.CheckValidity(startX, startY, width, height);

        Color newColour;



        if (valid)
        {
            newColour = Color.green;
        }
        else
        {
            newColour = Color.red;
        }

        for (int i = startY; i < startY + height; i++)
        {
            for (int j = startX; j < startX + width; j++)
            {
                floorTiles[i, j].GetComponent<SpriteRenderer>().color = newColour;
            }
        }


        theTileManager.toMoveX = startX;
        theTileManager.toMoveY = startY;
        theTileManager.fullWidth = width;
        theTileManager.fullHeight = height;

        theTileManager.NewItemAdded(startX, startY);

    }

    /// <summary>
    /// Remove an object from the cinema
    /// </summary>
    /// <param name="x">The x coordinate of the item to remove</param>
    /// <param name="y">The y coordinate of the item to remove</param>
    /// <param name="w">How many tiles wide the item to remove is</param>
    /// <param name="h">How many tiles high the item to remove is</param>
    public void RemoveObject(int x, int y, int w, int h)
    {
        if (tagSelected.Equals("Food Area"))
        {
            foodArea = null;

            // destroy all staff slots associated
            GameObject[] foodSlots = GameObject.FindGameObjectsWithTag("Slot Type 2");

            for (int i = 0; i < staffSlot.Count; i++)
            {
                for (int j = 0; j < foodSlots.Length; j++)
                {
                    if (staffSlot[i].name.Equals(foodSlots[j].name))
                    {
                        staffSlot.RemoveAt(i);
                        Destroy(foodSlots[j]);
                    }
                }
            }

            // unassign staff members associated with the food area
            for (int i = 0; i < staffMembers.Count; i++)
            {
                if (staffMembers[i].GetJobID() == 2)
                {
                    UpdateStaffJob(i, 0, -1, false);
                }
            }
        }

        GameObject go = GameObject.Find(objectSelected);
        if (objectSelected.Contains("Screen"))
        {
            shopController.SellScreen(objectSelected);
        }
        else
        {
            shopController.SellObject(objectSelected);

        }

        GameObject.Destroy(go);
        theTileManager.fullHeight = h;
        theTileManager.fullWidth = w;
        popupController.HideObjectInfo();
        SetTiles(0, x, y, w, h);
        theTileManager.ColourAllTiles(x, y, carpetColour);
        theTileManager.fullHeight = -1;
        theTileManager.fullHeight = -1;

        theTileManager.ShowOutput();
    }

    /// <summary>
    /// Save the current state of the game (both locally, and on Facebook)
    /// </summary>
    void Save(string persPath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(persPath + "/saveState.gd");

        PlayerData data = new PlayerData(ShopController.theScreens, carpetColour, staffMembers, filmShowings, financeController.GetNumCoins(), currDay, financeController.GetNumPopcorn(), ShopController.otherObjects, shopController.hasUnlockedRedCarpet, isMarbleFloor, customerController.reputation, boxOfficeLevel, foodArea, shopController.postersUnlocked);

        formatter.Serialize(file, data);

        file.Close();

        // save to database
        if (facebookProfile != null && facebookProfile.id.Length > 0)
        {
            byte[] ba = ConvertToByteArray(persPath);

            System.IO.File.WriteAllBytes(persPath + "/tes2.cles", ba);

            string outputting = System.Text.Encoding.UTF8.GetString(ba);
            
            UpdateDetails ud = new UpdateDetails();
            ud.DoUpdate(facebookProfile.id, ba);

            Debug.Log("Saved to Database");

        }

    }

    /// <summary>
    /// Convert the contents of a file to a BLOB
    /// </summary>
    /// <returns></returns>
    byte[] ConvertToByteArray(string persPath)
    {
        byte[] byteArray = null;

        string fileName = persPath + "/saveState.gd";

        byteArray = File.ReadAllBytes(fileName);

        return byteArray;

    }

    /// <summary>
    /// Hiring a new staff member (after the appearance has been selected)
    /// </summary>
    /// <param name="name">The staff members name</param>
    /// <param name="hID">The ID of the hair sprite to use</param>
    /// <param name="eID">The ID of the extras sprite to use</param>
    public void AddStaffMember(String name, int hID, int eID)
    {
        int id = staffMembers.Count;

        StaffMember sm = new StaffMember(id, name, staffPrefab, currDay, 0, AppearanceScript.hairStyle);

        Color[] cols = new Color[3];
        cols[0] = AppearanceScript.colours[0];
        cols[1] = AppearanceScript.colours[1];
        cols[2] = AppearanceScript.colours[2];
        sm.SetColours(cols, hID, eID);
        Sprite hairSprite, extrasSprite;

        hairSprite = AppearanceScript.hairStyle;
        extrasSprite = AppearanceScript.extraOption;

        sm.SetSprites(hairSprite, extrasSprite);

        staffMembers.Add(sm);

        int x = 35 + (2 * (id % 6));
        int y = (2 * (id / 6));

        CreateStaff(sm, x, y);
        Transform t = staffMembers[staffMembers.Count - 1].GetTransform();
        t.FindChild("hiddenPointer").GetComponent<SpriteRenderer>().enabled = false;

        Color c = AppearanceScript.colours[0];

        Color toPass = new Color(c.r, c.g, c.b, 1);

        for (int i = 0; i < staffMembers.Count; i++)
        {
            staffMembers[i].UniformChanged(toPass);
        }

    }

    /// <summary>
    /// Update the Job allocated to a staff member
    /// </summary>
    /// <param name="index">The index of the staff member</param>
    /// <param name="job">The job ID to change to</param>
    /// <param name="posInPost">Which slot in that post they are allocated to</param>
    /// <param name="add">Whether they are being added (true) or removed (false)</param>
    public void UpdateStaffJob(int index, int job, int posInPost, bool add)
    {

        staffMembers[index].SetJob(job);

        if (add)
        {
            // set new job
            if (job == 1)
            {
                ticketQueue.StaffMemberAssigned(staffMembers[index], posInPost);
            }
            else if (job == 2)
            {
                foodQueue.StaffMemberAssigned(staffMembers[index], posInPost);
            }
        }
        else
        {

            // remove from previous job
            if (job == 1)
            {
                ticketQueue.StaffMemberRemoved(staffMembers[index], posInPost);
            }
            else if (job == 2)
            {
                foodQueue.StaffMemberRemoved(staffMembers[index], posInPost);
            }

            staffMembers[index].SetJob(0);

        }

        staffMenuList[index].GetComponentsInChildren<Text>()[1].text = "Current Job: " + JobTextFromID(staffMembers[index].GetJobID());

        //UpdateJobList();
    }

    /// <summary>
    /// An item has been selected from the shop
    /// </summary>
    /// <param name="w">The width of the item</param>
    /// <param name="h">The height of the item</param>
    /// <param name="type">The type of object selected</param>
    public void ShopItemSelected(int w, int h, string type)
    {
        objectSelected = "NEW " + type;
        PlaceObject(w, h);
    }

    /// <summary>
    /// Get the description of the job post based on its ID
    /// </summary>
    /// <param name="index">The index of the job</param>
    /// <returns>The string describing the job</returns>
    string JobTextFromID(int index)
    {
        switch (index)
        {
            case 1:
                return "Tickets";
            case 2:
                return "Food Area";
            default:
                return "Unassigned";
        }
    }

    /// <summary>
    /// Get the job ID of a staff member - based on the staff member iD
    /// </summary>
    /// <param name="index">The ID of the staff member</param>
    /// <returns>The Job ID of the staff member</returns>
    public int GetStaffJobById(int index) { return staffMembers[index].GetJobID(); }

    /// <summary>
    /// Add a customer to the ticket queue
    /// </summary>
    /// <param name="customer">The customer to add</param>
    private void AddToQueueTickets(Customer customer)
    {
        ticketQueue.AddCustomer(customer);
    }

    /// <summary>
    /// Add a customer to the food queue
    /// </summary>
    /// <param name="customer">The customer to add</param>
    private void AddToQueueFood(Customer customer)
    {
        foodQueue.AddCustomer(customer);
        Debug.Log("FOOD: " + foodQueue.GetQueueSize());
    }
}