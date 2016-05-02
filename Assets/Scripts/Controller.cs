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

[System.Serializable]
public class Controller : MonoBehaviour
{
    public int selectedStaff = -1;
    public int newStatusCode = 0;

    public int numWalkouts = 0;
    public int customersServed = 0;
    public int customerMoney = 0;

    public bool[] postersUnlocked = new bool[2];

    public GameObject confirmBtn;

    public Sprite[] boxOfficeImages;
    public int boxOfficeLevel = 0;

    public GameObject[] walls;

    public GameObject pnlClearProjectors;
    public Transform builderPrefab;

    public Reputation reputation;

    public Sprite[] validityTiles;
    public Sprite[] foodTableSprites;

    public Sprite completeFoodAreaSprite;
    
    //public static Sprite profilePicture;

    public List<Transform> staffSlot = new List<Transform>();
    public List<bool> slotState = new List<bool>();
    public Transform slotPrefab;

    public Transform staffMenu;
    public Transform staffInfoObject;
    public Transform staffList;

    public GameObject redCarpet;
    public GameObject reputationPage;
    public bool hasUnlockedRedCarpet = false;

    public Sprite[] screenImages;
    public int itemToAddID = -1;

    GameObject startDayButton;
    public float mouseSensitivity = 1.0f;

    public GameObject colourPicker;
    public GameObject objectInfo;
    GameObject confirmMovePanel;
    GameObject shopCanvas;
    GameObject steps;
    public GameObject closeInfo;
    public GameObject closeColourPicker;
    GameObject moveButtons;

    public GameObject popup;

    public GameObject confirmationPanel;

    const string warning1 = "WARNING! \n\nThe Following Screen(s) are inaccessible to Customers:\n\n";
    const string warning2 = "\nYou have built objects which block the path to these Screens. If you do not move them, the customers for these screens will leave and you will not get money from them! Plus, your reputation will be ruined!";

    public string objectSelected = "";
    public string tagSelected = "";
    public int upgradeLevelSelected = 0;

    public List<GameObject> gameObjectList = new List<GameObject>();
    public List<GameObject> screenObjectList = new List<GameObject>();

    public static List<ScreenObject> theScreens = new List<ScreenObject>();
    List<OtherObject> otherObjects = new List<OtherObject>();

    public List<Customer> allCustomers = new List<Customer>();

    public int statusCode = 99;     // 0 = free, 1 = dragging staff, 2 = moving object, 3 = in menu, 4 = moving camera, 5 = shop, 6 = staff menu, 7 = staff member info, 8 = Confirmation page, 9 = popup, 10 = upgrade food area

    public Color carpetColour;

    public Transform greenGuy;
    public Transform blueGuy;
    public Transform orangeGuy;


    public GameObject staffMemberInfo;

    public Transform screenPrefab;
    public Transform plantPrefab;
    public Transform bustPrefab;
    public Transform vendingMachinePrefab;
    public Transform staffPrefab;
    public Transform foodAreaPrefab;

    public Text timeLabel;
    public Text dayLabel;
    public Text coinLabel;
    public Text popcornLabel;

    public Button shopButton;
    public Button staffMenuButton;

    public static FoodArea foodArea = null;

    public List<StaffMember> staffMembers = new List<StaffMember>();

    public List<FilmShowing> filmShowings = new List<FilmShowing>();

    public TileManager theTileManager;
    public GameObject confirmPanel;

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
    
    public CustomerQueue ticketQueue = new CustomerQueue(11, 38.5f, 6.8f, 0);
    public CustomerQueue foodQueue;

    public bool simulationRunning = false;

    public GameObject warningPanel;
    public Image warningIcon;
    public Text warningLabel;
    
    List<List<Coordinate>> ticketToScreen = new List<List<Coordinate>>();
    List<Coordinate> ticketToFood = new List<Coordinate>();
    List<List<Coordinate>> foodToScreen = new List<List<Coordinate>>();
    List<Coordinate> exitPath = new List<Coordinate>();
    
    public GameObject staffModel;
    public GameObject staffAppearanceMenu;
    
    public int totalCoins = 40000;
    public int numPopcorn = 15;

    public int currDay = 0;

    int numScreens = 1;

    public void OpenShop()
    {
        if (statusCode != 2 && statusCode != 8 && statusCode != 9)
        {
            HideObjectInfo();
            statusCode = 5;
            shopCanvas.SetActive(true);
        }
    }

    public void OpenStaffMenu()
    {
        if (statusCode == 0)
        {
            statusCode = 6;
            staffMenu.gameObject.SetActiveRecursively(true);
        }
    }

    public bool paused = false;

    void OnApplicationQuit()
    {
        // close off all open threads - memory issues
        ticketQueue.End();
    }

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

    // Use this for initialization
    void Start()
    {
        postersUnlocked = new bool[2];
        
        #region Find Objects
        theTileManager = GameObject.Find("TileManagement").GetComponent<TileManager>();
        confirmPanel = GameObject.Find("pnlConfirm");
        //shopButton = GameObject.Find("cmdShop").GetComponent<Button>();
        //shopButton = GameObject.Find("cmdStaffMenu").GetComponent<Button>();
        shopCanvas = GameObject.Find("Shop Canvas");
        //colourPicker = GameObject.Find("Colour Panel");
        GameObject custStatus = GameObject.Find("Customer Status");
        movementScript.customerStatus = custStatus;
        startDayButton = GameObject.Find("Start Day Button");
        //objectInfo = GameObject.Find("Object Info");
        confirmMovePanel = GameObject.Find("MovementPanel");
        moveButtons = GameObject.Find("buttonPanel");
        floorTiles = new GameObject[height, width];
        staffMemberInfo.SetActive(false);
        GameObject[] tmpArray = GameObject.FindGameObjectsWithTag("Floor Tile");
        //closeInfo = GameObject.Find("Close Info");
        steps = GameObject.Find("Steps");
        mouseDrag.staffAttributePanel = GameObject.Find("Staff Attributes");
        #endregion

        Customer.tiles = floorTiles;

        #region Hide Objects on Start
        confirmMovePanel.SetActive(false);
        objectInfo.SetActive(false);
        shopCanvas.SetActive(false);
        closeInfo.SetActive(false);
        //closeColourPicker.SetActive(false);
        colourPicker.SetActive(false);
        popupBox.SetActive(false);
        moveButtons.SetActive(false);
        custStatus.SetActive(false);
        warningPanel.SetActive(false);
        warningIcon.enabled = false;
        staffMenu.gameObject.SetActive(false);
        confirmationPanel.SetActive(false);
        popup.SetActive(false);
        redCarpet.SetActive(false);
        reputationPage.SetActive(false);
        mouseDrag.staffAttributePanel.SetActive(false);
        #endregion

        #region Add Delegate references
        mouseDrag.getStaffListSize += GetStaffSize;
        mouseDrag.getStaffJobById += GetStaffJobById;
        mouseDrag.changeStaffJob += UpdateStaffJob;
        mouseDrag.getStaffList += GetFullStaffList;
        movementScript.addToQueueTickets += AddToQueueTickets;
        movementScript.getQueueTicketsSize += GetTicketQueueSize;
        movementScript.addToQueueFood += AddToQueueFood;
        movementScript.getQueueFoodSize += GetFoodQueueSize;
        Screen_Script.showBuildingMenu += ShowBuildingOptions;
        OtherObjectScript.showBuildingMenu += ShowBuildingOptions;
        #endregion


        // this will change depending on starting upgrade levels and other queues etc

        #region Load / New Game
        // get Player data. If not null, load game
        if (ButtonScript.loadGame == null)
        {
            numPopcorn = 15;
            totalCoins = 40000;

            carpetColour = GetColourFromID(1);
            reputation = new Reputation();
            reputation.Initialise();

            OtherObjectScript.CreateStaffSlot(1, new Vector3(37.8f, 12.3f, 0));

            #region Floor Tiles
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
                theScreens.Add(new ScreenObject((i + 1), 0));
                theScreens[i].SetPosition((int)pos.x, (int)pos.y);
            }
            theScreens[0].Upgrade();
            theScreens[0].UpgradeComplete();
            // NYAH

            NextDay(false);
            
            // do staff intro thing here
            ShowPopup(99, "Welcome!!! This is your cinema!\nLets get started by hiring some staff shall we?");

            foodArea = null;

        }
        else
        {
            statusCode = 0;

            PlayerData data = ButtonScript.loadGame;

            carpetColour = new Color(data.carpetColour[0], data.carpetColour[1], data.carpetColour[2]);

            hasUnlockedRedCarpet = data.hasRedCarpet;

            if (hasUnlockedRedCarpet)
            {
                redCarpet.SetActive(true);
            }

            isMarbleFloor = data.marbleFloor;
            reputation = data.reputation;
            foodArea = data.foodArea;
            postersUnlocked = data.posters;

            int boxLevel = data.boxOfficeLevel;
            OtherObjectScript.CreateStaffSlot(1, new Vector3(37.8f, 12.3f, 0));

            for (int i = 0; i < boxLevel - 1; i++)
            {
                OtherObjectScript.UpgradeBoxOffice();
            }

            #region Floor Tiles
            // initialise the floor tiles
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

            theScreens = new List<ScreenObject>(data.theScreens);


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
            totalCoins = data.totalCoins;
            currDay = data.currentDay;
            numScreens = theScreens.Count;
            numPopcorn = data.numPopcorn;
            otherObjects = new List<OtherObject>(data.otherObjects);


            NextDay(false);
            currDay--; // needed for some reason


            // hopefully un-breaks things
            for (int i = 0; i < theScreens.Count; i++)
            {
                Vector3 pos = new Vector3(theScreens[i].GetX(), theScreens[i].GetY(), 0);
                theScreens[i].SetPosition((int)pos.x, (int)(pos.y));
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

            // show relevant posters
            if (postersUnlocked[0])
            {
                GameObject[] allPosters = GameObject.FindGameObjectsWithTag("Poster");

                for (int i = 0; i < allPosters.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        SpriteRenderer sr = allPosters[i].GetComponent<SpriteRenderer>();
                        sr.enabled = true;
                    }
                }
            }

            if (postersUnlocked[1])
            {
                GameObject[] allPosters = GameObject.FindGameObjectsWithTag("Poster");

                for (int i = 0; i < allPosters.Length; i++)
                {
                    if (i % 2 == 1)
                    {
                        SpriteRenderer sr = allPosters[i].GetComponent<SpriteRenderer>();
                        sr.enabled = true;
                    }
                }
            }

        }
        #endregion

        // create some test screens
        for (int i = 0; i < theScreens.Count; i++)
        {
            Vector3 pos = new Vector3(theScreens[i].GetX(), theScreens[i].GetY() * 0.8f, 0);

            //// align to grid - +/- 1 to move by one tile horizontally, 0.8 for vertical movement
            //pos.x += 4.6f;
            //pos.y += 6.05f;

            //// change pos and element here
            pos.y += 0.8f;

            GameObject instance = (GameObject)Instantiate(screenPrefab.gameObject, pos, Quaternion.identity);
            instance.GetComponent<Screen_Script>().theScreen = theScreens[i];
            instance.name = "Screen#" + theScreens[i].GetScreenNumber();
            instance.tag = "Screen";
            instance.GetComponent<SpriteRenderer>().sortingOrder = height - theScreens[i].GetY() - 1;
            if (!theScreens[i].ConstructionInProgress())
            {
                instance.GetComponent<SpriteRenderer>().sprite = screenImages[theScreens[i].GetUpgradeLevel()];
            }
            else
            {
                instance.GetComponent<SpriteRenderer>().sprite = screenImages[0];
                CreateBuilder(theScreens[i].GetX(), theScreens[i].GetY(), theScreens[i].GetScreenNumber());
            }

            screenObjectList.Add(instance);

            SetTiles(2, (int)(theScreens[i].GetX()), (int)(theScreens[i].GetY()), 11, 15);
        }

        // do same for other objects
        for (int i = 0; i < otherObjects.Count; i++)
        {
            Vector3 pos = new Vector3(otherObjects[i].xPos, otherObjects[i].yPos * 0.8f, 0);

            //float xCorrection = 0;
            //float yCorrection = 0;

            Transform newItem = null;

            int w = 0;
            int h = 0;
            String tag = null;

            itemToAddID = otherObjects[i].type;

            if (itemToAddID == 2)
            {
                newItem = plantPrefab;
                //xCorrection = 0.1f;
                //yCorrection = 0.35f;
                w = 1; h = 1;
                tag = "Plant";
            }
            else if (itemToAddID == 3)
            {
                newItem = bustPrefab;
                //xCorrection = 0.65f;
                //yCorrection = 1.5f;
                w = 2; h = 2;
                tag = "Bust";
            }
            else if (itemToAddID == 5)
            {
                newItem = vendingMachinePrefab;
                //xCorrection = 1.07f;
                //yCorrection = 1.62f;
                w = 3; h = 3;
                tag = "Vending Machine";
            }
            else if (itemToAddID == 7)
            {
                newItem = foodAreaPrefab;
                //xCorrection = 1.07f;
                //yCorrection = 1.62f;
                w = 10; h = 18;
                tag = "Food Area";
            }

            // align to grid - +/- 1 to move by one tile horizontally, 0.8 for vertical movement
            //pos.x += xCorrection;
            //pos.y += yCorrection;

            // change pos and element here
            GameObject instance = (GameObject)Instantiate(newItem.gameObject, pos, Quaternion.identity);
            instance.name = "Element#" + (i);
            instance.GetComponent<SpriteRenderer>().sortingOrder = height - otherObjects[i].yPos - 1;
            instance.tag = tag;

            gameObjectList.Add(instance);

            SetTiles(2, (int)(otherObjects[i].xPos), (int)(otherObjects[i].yPos), w, h);
        }

        //createColourPicker();

        if (updateTileState != null)
        {
            updateTileState(33, 0, 14, 16, 1, true);
            updateTileState(33, 16, 14, 4, 2, true);
        }


        coinLabel.text = totalCoins.ToString();
        popcornLabel.text = numPopcorn.ToString();


        GameObject[] pointers = GameObject.FindGameObjectsWithTag("Pointer");

        for (int i = 0; i < pointers.Length; i++)
        {
            pointers[i].GetComponent<Transform>().GetComponent<SpriteRenderer>().enabled = false;
        }


        dayLabel.text = "DAY: " + currDay.ToString();
        popcornLabel.text = numPopcorn.ToString();

        Save();
        
    }

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

    public int GetTicketQueueSize()
    {
        return ticketQueue.GetQueueSize();
    }
    public int GetFoodQueueSize()
    {
        return foodQueue.GetQueueSize();
    }

    public void HidePopup()
    {
        popup.SetActive(false);
        confirmationPanel.SetActive(false);

        popupBox.SetActive(false);

        statusCode = newStatusCode;

        if (statusCode == 99)
        {
            AppearanceScript.Initialise(true, null, 2, new Color(1,1,1,1), -1, null, "", null);
            // hide all staff
            GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");
            
            // change camera position
            Camera.main.transform.position = new Vector3(32.68f, 0, 1);
            Camera.main.orthographicSize = 14;
            staffAppearanceMenu.SetActive(true);
            staffModel.SetActive(true);
        }

    }

    public List<Coordinate> GetScreenPath(int index)
    {
        return this.ticketToScreen[index];
    }
    public List<Coordinate> GetFoodToScreenPath(int index)
    {
        return foodToScreen[index];
    }

    public List<Coordinate> GetPathToFood()
    {
        return this.ticketToFood;
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

        staffMemberInfo.SetActive(false);

        // move the camera into place
        Camera.main.transform.position = new Vector3(32.68f, 0, 1);
        Camera.main.orthographicSize = 14;
        
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
        Text[] txts = staffList.GetComponentsInChildren<Text>();
        txts[index * 2].text = name;
        
        Image[] imgs = staffList.GetComponentsInChildren<Image>();

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

    public void CreateBuilder(float x, float y, int screenNum)
    {
        GameObject builder = Instantiate(builderPrefab.gameObject, new Vector2(x + 1.8f, 0.8f * y + 0.7f), Quaternion.identity) as GameObject;
        builder.name = "BuilderForScreen#" + screenNum;
    }

    List<GameObject> staffMenuList = new List<GameObject>();

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
        go.transform.SetParent(staffList, false);

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

    public void ViewStaffMemberInfo(int staffID)
    {
        statusCode = 7;
        staffMenu.gameObject.SetActive(false);
        staffMemberInfo.SetActive(true);

        selectedStaff = staffID;

        // get the details from the staff member
        StaffMember s = staffMembers[staffID];
        int[] attributes = s.GetAttributes();
        string name = s.GetStaffname();
        int dayHired = s.GetStartDay();
        Sprite sprite = s.GetTransform().GetComponent<SpriteRenderer>().sprite;

        // find the elements of the form
        Image[] imgs = staffMemberInfo.GetComponentsInChildren<Image>();
        Text[] txts = staffMemberInfo.GetComponentsInChildren<Text>();

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
    
    public void MoveToStaffLocation(int staffID)
    {
        statusCode = 50;
        Camera.main.orthographicSize = 5f;
        Vector3 pos = staffMembers[staffID].GetVector();
        pos.x = pos.x - 1.2f;
        pos.z = -10;
        
        Camera.main.GetComponent<CameraControls>().endPos = pos;
    }

    public void UpgradeStaffAttribute(int index)
    {
        staffMembers[selectedStaff].Upgrade(index);

        Image[] imgs = staffMemberInfo.GetComponentsInChildren<Image>();

        int attributeEffected = staffMembers[selectedStaff].GetAttributes()[index];

        imgs[9 + (index * 4)].fillAmount = 0.25f * attributeEffected;
    }

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

    public void SellItem()
    {
        if (theScreens.Count > 1 || !objectSelected.Contains("Screen"))
        {
            int xPos = (int)(GetPositionOfObject().x);

            float tempY = GetPositionOfObject().y / 0.8f;

            int yPos = (int)(tempY);

            int width = GetWidthOfObject();
            int height = GetHeightOfObject();

            if (objectSelected.Contains("Screen"))
            {
                yPos -= 1;
            }

            int moneyToReturn = GetReturnedCoins();
            ConfirmationScript.OptionSelected(6, new string[] { "sell this item?", moneyToReturn.ToString(), "0", xPos.ToString(), yPos.ToString(), width.ToString(), height.ToString() }, "You will receive: ");
        }
        else
        {
            ShowPopup(3, "Uh-oh!\nYou can't sell this Screen because your cinema must have at least 1 screen!");
        }
    }

    public int GetWidthOfObject()
    {
        switch (tagSelected)
        {
            case "Screen":
                return 11;
            case "Vending Machine":
                return 3;
            case "Plant":
                return 1;
            case "Food Area":
                return 10;
            default: return 0;
        }
    }
    public int GetHeightOfObject()
    {
        switch (tagSelected)
        {
            case "Screen":
                return 15;
            case "Vending Machine":
                return 3;
            case "Plant":
                return 1;
            case "Food Area":
                return 18;
            default: return 0;
        }
    }
    public Vector2 GetPositionOfObject()
    {
        return GameObject.Find(objectSelected).transform.position;
    }

    public int GetReturnedCoins()
    {
        int paidMoney = 0;

        #region Get the money that was paid for this object
        switch (tagSelected)
        {
            case "Screen":
                paidMoney = 500;
                break;
            case "Vending Machine":
                paidMoney = 90;
                break;
            case "Plant":
                paidMoney = 80;
                break;
            case "Food Area":
                paidMoney = 5000;
                break;
        }
        #endregion

        int coinsReturned = (int)(Math.Round(0.6f * (float)paidMoney, 0));

        #region Calculate upgrade costs
        int upgradeCosts = 0;

        if (tagSelected.Equals("Screen"))
        {
            switch (upgradeLevelSelected)
            {
                case 2: upgradeCosts = 180; break;
                case 3: upgradeCosts = 720; break;
                case 4: upgradeCosts = 2800; break;
            }
        }

        coinsReturned += upgradeCosts;
        #endregion

        return coinsReturned;
    }

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
                ShowPopup(0, "The Box Office is already fully upgraded!");
            }
        }
        else if (tagSelected.Equals("Food Area"))
        {
            String ob = objectSelected;

            HideObjectInfo();

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

            for (int i = 0; i < screenObjectList.Count; i++)
            {
                if (screenObjectList[i].name.Equals(objectSelected))
                {
                    ScreenObject theScreen = screenObjectList[i].GetComponent<Screen_Script>().theScreen;

                    if (theScreen.GetUpgradeLevel() < 4 && !theScreen.ConstructionInProgress())
                    {

                        ConfirmationScript.OptionSelected(3, new string[] { "upgrade Screen " + theScreen.GetScreenNumber(), (theScreen.CalculateUpgradeCost()).ToString(), "0", i.ToString() }, "This will cost: ");

                        break;
                    }
                    else if (theScreen.ConstructionInProgress())
                    {
                        ShowPopup(3, "Construction on this Screen is already in progress!");
                    }
                }
            }
        }
    }

    public void UpdateTiles(int x, int y, int w, int h, int newState)
    {
        if (updateTileState != null)
        {
            updateTileState(x, y, w, h, newState, false);
        }
    }

    void SetTiles(int newState, int x, int y, int width, int height)
    {
        Color newColour;

        if (newState != 0) { newColour = carpetColour; } else { newColour = Color.green; }

        ChangeColour(newColour, x, y, width, height);

        UpdateTiles(x, y, width, height, newState);
    }

    public void ShowPopup(int status, string theString)
    {
        newStatusCode = status;
        popup.SetActive(true);
        Text[] texts = popup.gameObject.GetComponentsInChildren<Text>();
        texts[1].text = theString;
    }

    public void HideObjectInfo()
    {
        shopCanvas.SetActive(false);
        objectInfo.SetActive(false);
        closeInfo.SetActive(false);
        staffMenu.gameObject.SetActive(false);
        colourPicker.SetActive(false);
        staffMemberInfo.SetActive(false);

        statusCode = 0;
        objectSelected = "";
        tagSelected = "";
        upgradeLevelSelected = 0;
        selectedStaff = -1;
    }

    void NewColourButton(int row, int column, bool texture)
    {
        GameObject gO = new GameObject();
        gO.name = "ColorCircle~" + row + "~" + column;
        gO.tag = "Colour Button";
        gO.AddComponent<Image>();

        if (!texture)
        {
            gO.GetComponent<Image>().sprite = colourCircle;

            float r, g, b;

            float columnMultiple = (column * 0.2f);

            if (row == 0)
            {
                r = 1 - columnMultiple;
                g = columnMultiple;
                b = 0;
            }
            else if (row == 1)
            {
                r = 0;
                g = 1 - columnMultiple;
                b = columnMultiple;
            }
            else
            {
                r = columnMultiple;
                g = 0;
                b = 1 - columnMultiple;
            }
            gO.GetComponent<Image>().color = new Color(r, g, b, 100);
        }
        else
        {
            gO.GetComponent<Image>().sprite = marbleBackground;
            //gO.tag = "untagged";
        }


        //// create an Image
        //SpriteRenderer theRenderer = gO.AddComponent<SpriteRenderer>();
        //theRenderer.sprite = ColourBackground;

        gO.GetComponent<Image>().rectTransform.sizeDelta = new Vector3(15, 15);
        gO.transform.SetParent(colourPicker.transform);

        gO.transform.localPosition = new Vector3(column * 20 - 60, row * 30, 0);

        gO.AddComponent<Button>();
        Button btn = gO.GetComponent<Button>();
        // btn.onClick.AddListener(() => colourClicked(gO.GetComponent<Image>().color, gO.GetComponent<Image>().sprite));
    }

    void CreateColourPicker()
    {
        Image[,] colours = new Image[3, 6];

        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 2; column++)
            {
                NewColourButton(row, column, false);
            }
        }

        NewColourButton(0, 2, true);

    }

    Color GetColourFromID(int id)
    {
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

    Sprite[] GetSpriteFromID(int id)
    {
        switch (id)
        {
            case 4: isMarbleFloor = true; return marbleSquares;
            default: isMarbleFloor = false; return new Sprite[] { ColourBackground };
        }
    }

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

    public void ShowColourPicker()
    {
        colourPicker.SetActive(!colourPicker.active);
        shopCanvas.SetActive(false);
    }

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
    
    public void StartDay()
    {
        // check if any screens available
        bool screensAvailable = false;

        for (int i = 0; i < theScreens.Count; i++)
        {
            if (!theScreens[i].ConstructionInProgress())
            {
                screensAvailable = true;
                break;
            }
        }

        // if none, 
        if (!screensAvailable)
        {
            ShowPopup(0, "None of your screens are available today - they are all under construction. No customers will arrive and you will receive 0 coins for this day");

            NextDay(false);
            return;
        }
        else
        {

            if (!simulationRunning)
            {
                // start the running of the 'day'
                simulationRunning = true;
            }

            ticketQueue.Begin();

            if (foodQueue != null)
            {
                foodQueue.Begin();
            }
            
            ticketToScreen.Clear();
            ticketToFood.Clear();
            foodToScreen.Clear();

            // find paths to allScreens
            for (int i = 0; i < theScreens.Count; i++)
            {
                List<Coordinate> points = TileManager.floor.FindPath(38, 11, theScreens[i].GetX() + 5, theScreens[i].GetY());
                ticketToScreen.Add(points);

                if (foodArea != null)
                {
                    GameObject foodCourt = GameObject.FindGameObjectWithTag("Food Area");

                    points = TileManager.floor.FindPath((int)foodCourt.transform.position.x + 3, (int)(foodCourt.transform.position.y / 0.8f) + 5, theScreens[i].GetX() + 5, theScreens[i].GetY());
                    foodToScreen.Add(points);
                }

            }
            if (foodArea != null)
            {
                //foodQueue = null;

                GameObject foodCourt = GameObject.FindGameObjectWithTag("Food Area");

                ticketToFood = TileManager.floor.FindPath(38, 11, (int)foodCourt.transform.position.x + 3, (int)(foodCourt.transform.position.y / 0.8f) + 5);

            }

            // hide the buttons and menus
            HideObjectInfo();
            startDayButton.SetActive(false);
            shopButton.gameObject.SetActive(false);
            staffMenuButton.gameObject.SetActive(false);
            colourPicker.SetActive(false);
            //staffMemberInfo.SetActive(false);
            //staffMenu.gameObject.SetActive(false);

            Customer.tiles = floorTiles;

            // Optimise Object Pool

            // check if the current size is enough
            int currObjectCount = ObjectPool.current.pooledObjects.Count;

            // if there are not enough objects, add some so that there are enough
            if (currObjectCount < allCustomers.Count / 2.2) {
                for (int i = ObjectPool.current.pooledObjects.Count; i < allCustomers.Count / 2.2; i++)
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

    public void ClearAllProjectors()
    {
        GameObject[] projectors = GameObject.FindGameObjectsWithTag("Projector");

        for (int i = 0; i < projectors.Length; i++)
        {
            Destroy(projectors[i]);
        }

        for (int i = 0; i < theScreens.Count; i++)
        {
            theScreens[i].ResetClicks();
        }

        // clear the panel
        pnlClearProjectors.SetActive(false);

        ProjectorScript.numVisible = 0;

    }

    public void NextDay(bool shouldCollect)
    {
        // update the reputation fields
        reputation.SetFacilities(theScreens, hasUnlockedRedCarpet, foodArea);
        reputation.SetPublicityRating(postersUnlocked);
        reputation.SetStaffRating(staffMembers);


        for (int i = 0; i < theScreens.Count; i++)
        {
            theScreens[i].ProgressOneDay();
            theScreens[i].ResetClicks();
        }

        ClearAllProjectors();

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


        if (shouldCollect)
        {
            int money = GetTodaysMoney();

            totalCoins += money;
            reputation.AddCoins(money);
            // output coins
            coinLabel.text = totalCoins.ToString();
            //lblCoins.Text = totalCoins.ToString();

            int oldOverall = reputation.GetOverall();
            reputation.SetOverall();

            int newOverall = reputation.GetOverall();

            int repChange = newOverall - oldOverall;

            ShowEndOfDayPopup(money, numWalkouts, repChange, customersServed);
        }

        simulationRunning = false;

        queueCount = 0;
        

        //ticketQueue.Clear();

        for (int i = 0; i < allCustomers.Count; i++)
        {
            if (allCustomers[i].transform != null)
            {
                allCustomers[i].transform.gameObject.SetActive(false);

                allCustomers[i].transform.GetComponent<SpriteRenderer>().sortingLayerName = "Front";
                allCustomers[i].transform.GetComponent<SpriteRenderer>().sortingOrder = 70;
            }
        }


        // reset the layer of each customer transform 
        for (int i = 0; i < ObjectPool.current.pooledObjects.Count; i++)
        {
            SpriteRenderer sr = ObjectPool.current.pooledObjects[i].GetComponent<SpriteRenderer>();

            sr.sortingLayerName = "Front";
            sr.sortingOrder = 70;
        }


        allCustomers.Clear();

        startDayButton.SetActive(true);
        shopButton.gameObject.SetActive(true);
        staffMenuButton.gameObject.SetActive(true);




        currDay++;


        if (currDay % 7 == 1)
        {
            NextWeek();
        }

        //if (shouldCollect) {
        for (int i = 0; i < theScreens.Count; i++)
        {

            if (theScreens[i].GetDaysOfConstruction() + 1 == 1 && theScreens[i].GetDaysOfConstruction() == 0 && screenObjectList.Count > 0)
            {

                screenObjectList[i].GetComponent<SpriteRenderer>().sprite = screenImages[theScreens[i].GetUpgradeLevel()];
                NewShowTimes();

                for (int k = 0; k < filmShowings.Count; k++)       // filmShowings.Count
                {
                    int index = filmShowings[k].GetScreenNumber();
                    int ticketsSold = GetTicketsSoldValue(theScreens[index - 1]);
                    filmShowings[k].SetTicketsSold(ticketsSold);

                    int currentCount = 0;

                    for (int j = 0; j < k; j++)
                    {
                        currentCount += filmShowings[j].GetTicketsSold();
                    }

                    //List<Customer> tmp = filmShowings[k].CreateCustomerList(currentCount, this);
                    //allCustomers.AddRange(tmp);

                    DestroyBuilderByScreenID(filmShowings[k].GetScreenNumber());

                }
            }
        }
        //}

        for (int i = 0; i < filmShowings.Count; i++)       // filmShowings.Count
        {
            int index = filmShowings[i].GetScreenNumber();
            int ticketsSold = GetTicketsSoldValue(theScreens[index - 1]);
            filmShowings[i].SetTicketsSold(ticketsSold);

            int currentCount = 0;

            for (int j = 0; j < i; j++)
            {
                currentCount += filmShowings[j].GetTicketsSold();
            }

            //List<Customer> tmp = filmShowings[i].CreateCustomerList(currentCount, this);
            //allCustomers.AddRange(tmp);
        }

        // update day output 
        dayLabel.text = "DAY: " + currDay.ToString();

        movementScript.customerStatus.SetActive(false);

        //statusCode = 0;



        ResetStaff();


        // reset the state of each slot
        for (int i = 0; i < slotState.Count; i++)
        {
            slotState[i] = false;
        }

        numWalkouts = 0;
        customersServed = 0;
        customerMoney = 0;

        Save();

    }

    void NextWeek()
    {
        NewShowTimes();
    }

    public void ViewReputation()
    {
        statusCode = 9;
        popupBox.SetActive(false);
        reputationPage.SetActive(true);

        Text[] textElements = reputationPage.gameObject.GetComponentsInChildren<Text>();

        textElements[2].text = reputation.GetTotalCoins().ToString();
        textElements[4].text = reputation.GetOverall().ToString() + "%";
        textElements[5].text = reputation.GetTotalCustomers().ToString();
        textElements[6].text = reputation.GetHighestRep().ToString();

        textElements[9].text = (4 * reputation.GetSpeedRating()).ToString();
        textElements[11].text = (4 * reputation.GetPublicityRating()).ToString();
        textElements[13].text = (4 * reputation.GetFacilitiesRating()).ToString();
        textElements[15].text = (4 * reputation.GetStaffRating()).ToString();


        Image[] imageElements = reputationPage.gameObject.GetComponentsInChildren<Image>();
        imageElements[7].fillAmount = (float)reputation.GetPublicityRating() / 25f;
        imageElements[10].fillAmount = (float)reputation.GetFacilitiesRating() / 25f;
        imageElements[13].fillAmount = (float)reputation.GetStaffRating() / 25f;
    }

    public void CloseReputation()
    {
        reputationPage.SetActive(false);
        popupBox.SetActive(true);
        statusCode = 0;
    }

    public void DestroyBuilderByScreenID(int screenNum)
    {
        GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");

        for (int i = 0; i < builders.Length; i++)
        {
            if (builders[i].name.Contains(screenNum.ToString()))
            {
                Destroy(builders[i]);
            }
        }

    }

    private int GetTodaysMoney()
    {
        int totalIntake = customerMoney;

        // each ticket is 2 coins + 1 for each upgrade level
        for (int i = 0; i < filmShowings.Count; i++)
        {
            int screenNum = filmShowings[i].GetScreenNumber() - 1;

            if (!theScreens[screenNum].ConstructionInProgress())
            {
                int upgradeLevel = theScreens[screenNum].GetUpgradeLevel();
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

    public void UnlockPosterPack(int index)
    {
        postersUnlocked[index] = true;

        GameObject[] allPosters = GameObject.FindGameObjectsWithTag("Poster");

        for (int i = 0; i < allPosters.Length; i++)
        {
            if (i % 2 == index)
            {
                SpriteRenderer sr = allPosters[i].GetComponent<SpriteRenderer>();
                sr.enabled = true;
            }
        }
    }

    public int GetTicketsSoldValue(ScreenObject screen)
    {
        UnityEngine.Random ran = new UnityEngine.Random();
        int min = (int)(screen.GetNumSeats() / 1.5);  // this will be affected by the posters etc, and rep
        int max = screen.GetNumSeats();
        int ticketsSold = UnityEngine.Random.Range(min, max);
        float repMultiplier = reputation.GetMultiplier();
        ticketsSold = (int)(ticketsSold * repMultiplier);

        return ticketsSold;
    }

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

    public void NewShowTimes()
    {
        allCustomers.Clear();
        filmShowings.Clear();

        for (int i = 0; i < theScreens.Count; i++)
        {
            if (!theScreens[i].ConstructionInProgress())
            {
                int screeningsThisDay = UnityEngine.Random.Range(2, 4); // number of films per screen per day

                for (int j = 0; j < screeningsThisDay; j++)     // screeningsThisDay
                {
                    TimeTuple showTime = GetShowTime(j);

                    FilmShowing newFilm = new FilmShowing(filmShowings.Count, i + 1, 0, showTime.hours, showTime.minutes, TileManager.floor);
                    int ticketsSold = GetTicketsSoldValue(theScreens[i]);
                    newFilm.SetTicketsSold(ticketsSold);
                    filmShowings.Add(newFilm);

                    List<Customer> custs = newFilm.CreateCustomerList(allCustomers.Count, this);
                    allCustomers.AddRange(custs);

                }
            }
        }
    }

    float queueCount = 0;
    float ticketStaffLevel = 4.5f;

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

                theScreens[id].SetPosition(x, y);

                pos.y += 0.8f;

                ScreenObject temp = null;

                GameObject theScreen = GameObject.Find("Screen#" + theScreens[id].GetScreenNumber());
                temp = theScreen.GetComponent<Screen_Script>().theScreen;

                theScreen.GetComponent<SpriteRenderer>().sortingOrder = height - y - 1;
                theScreen.transform.position = pos;
                if (temp.ConstructionInProgress())
                {
                    theScreen.GetComponent<SpriteRenderer>().sprite = screenImages[0];
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
            else {


                Vector3 pos = new Vector3(x, y * 0.8f, 0);

                otherObjects[id].xPos = x;
                otherObjects[id].yPos = y;

                string theTag = "";

                if (itemToAddID == 2)
                {
                    newItem = plantPrefab;
                    theTag = "Plant";
                }
                else if (itemToAddID == 3)
                {
                    newItem = bustPrefab;
                    theTag = "Bust";
                }
                else if (itemToAddID == 5)
                {
                    newItem = vendingMachinePrefab;
                    theTag = "Vending Machine";
                }
                else if (itemToAddID == 7)
                {
                    newItem = foodAreaPrefab;
                    theTag = "Food Area";
                    
                    //foodQueue = new CustomerQueue(4, 4, 4);
                    // change position
                }

                GameObject theObject = GameObject.Find("Element#" + id);
                theObject.transform.position = pos;

                theObject.GetComponent<Renderer>().enabled = true;
                theObject.GetComponent<SpriteRenderer>().sortingOrder = height - y - 1;

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
                            staffMembers[i].GetTransform().position = slots[posInLayer].transform.position - new Vector3(0, 1.1f , 1);
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
            confirmMovePanel.SetActive(false);
            moveButtons.SetActive(false);


            ReShowStaffAndBuildings();

            redCarpet.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);

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
            parameters[1] = GetCost().ToString();
            parameters[2] = GetCurrency();
            parameters[3] = theTileManager.toMoveX.ToString();
            parameters[4] = theTileManager.toMoveY.ToString();
            ConfirmationScript.OptionSelected(0, parameters, "This will cost: ");

        }
        else {
            ChangeColour(carpetColour, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);

            theTileManager.ResetStatusVariables();

            confirmMovePanel.SetActive(false);
            moveButtons.SetActive(false);

            statusCode = 0;

            ReShowStaffAndBuildings();

            objectSelected = "";
            tagSelected = "";
            upgradeLevelSelected = 0;
        }
    }

    public void ReShowStaffAndBuildings()
    {
        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");
        for (int i = 0; i < staff.Length; i++)
        {
            SpriteRenderer[] srs = staff[i].GetComponentsInChildren<SpriteRenderer>();
            for(int j = 0; j < 5; j++)
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


        for (int i = 0; i < screenObjectList.Count; i++)
        {
            screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        }

        for (int i = 0; i < gameObjectList.Count; i++)
        {
            gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        }

        redCarpet.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

        GameObject foodPlace = GameObject.FindGameObjectWithTag("Food Area");
        if (foodPlace != null)
        {
            SpriteRenderer[] foodAreaRenderers = foodPlace.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in foodAreaRenderers)
            {
                sr.color = new Color(1, 1, 1, 1);
            }
        }

    }

    int GetCost()
    {
        switch (objectSelected.ToUpper())
        {
            case "NEW SCREEN": return 500;
            case "NEW PLANT": return 80;
            case "NEW BUST": return 7;
            case "NEW VENDING MACHINE": return 90;
            case "NEW FOOD AREA": return 5000;
            default: return 0;
        }
    }

    String GetCurrency()
    {
        // 0 = coins, 1 = popcorn
        switch (objectSelected)
        {
            case "NEW SCREEN": return "0";
            case "NEW PLANT": return "0";
            case "NEW BUST": return "1";
            case "NEW VENDING MACHINE": return "0";
            default: return "0";
        }
    }

    public void AddNewObject(int x, int y)
    {
        confirmMovePanel.SetActive(false);
        moveButtons.SetActive(false);

        Vector3 pos = new Vector3(x, y * 0.8f, 0);


        Transform newItem = null;
        //float xCorrection = 0;
        //float yCorrection = 0;

        if (itemToAddID == 0)
        {
            newItem = screenPrefab;
            //xCorrection = 4.6f;
            //yCorrection = 6.05f;

            int newID = theScreens.Count;
            ScreenObject aScreen = new ScreenObject(newID + 1, 0);
            aScreen.SetPosition(x, y);
            aScreen.Upgrade();
            theScreens.Add(aScreen);

            //pos.x += xCorrection;
            //pos.y += yCorrection;
            pos.y += 0.8f; // gap at bottom

            GameObject screenThing = (GameObject)Instantiate(screenPrefab.gameObject, pos, Quaternion.identity) as GameObject;
            screenThing.GetComponent<Screen_Script>().theScreen = theScreens[newID];
            screenThing.name = "Screen#" + theScreens[newID].GetScreenNumber();
            screenThing.GetComponent<SpriteRenderer>().sortingOrder = 40 - y - 1;
            screenThing.GetComponent<SpriteRenderer>().sprite = screenImages[0];

            screenThing.tag = "Screen";
            screenObjectList.Add(screenThing);

            CreateBuilder(x, y, theScreens[newID].GetScreenNumber());

            // check staff position
            CheckStaffPosition(screenThing);

            for (int i = 0; i < staffMembers.Count; i++)
            {
                staffMembers[i].GetTransform().Translate(new Vector3(0, 0, -1));
            }
        }
        else {

            string theTag = "";

            if (itemToAddID == 2)
            {
                newItem = plantPrefab;
                //xCorrection = 0.1f;
                //yCorrection = 0.35f;
                otherObjects.Add(new OtherObject(x, y, 2, otherObjects.Count));
                theTag = "Plant";
            }
            else if (itemToAddID == 3)
            {
                newItem = bustPrefab;
                //xCorrection = 0.65f;
                //yCorrection = 1.5f;
                otherObjects.Add(new OtherObject(x, y, 3, otherObjects.Count));
                theTag = "Bust";
            }
            else if (itemToAddID == 5)
            {
                newItem = vendingMachinePrefab;
                //xCorrection = 1.07f;
                //yCorrection = 1.62f;
                otherObjects.Add(new OtherObject(x, y, 5, otherObjects.Count));
                theTag = "Vending Machine";
            }
            else if (itemToAddID == 7)
            {
                newItem = foodAreaPrefab;
                //xCorrection = 1.07f;
                //yCorrection = 1.62f;
                otherObjects.Add(new OtherObject(x, y, 7, otherObjects.Count));
                theTag = "Food Area";

                foodArea = new FoodArea();
                foodArea.hasHotFood = true;     // give them 1 thing to start with

                foodQueue = new CustomerQueue(70, x + 3, ((y + 4) * 0.8f) - 1, 1);

            }
            

            GameObject theObject = (GameObject)Instantiate(newItem.gameObject, pos, Quaternion.identity) as GameObject;
            theObject.name = "Element#" + (otherObjects.Count - 1);
            theObject.tag = theTag;
            theObject.GetComponent<SpriteRenderer>().sortingOrder = height - y - 1;

            gameObjectList.Add(theObject);
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
        GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");
        for (int i = 0; i < builders.Length; i++)
        {
            builders[i].GetComponent<SpriteRenderer>().enabled = true;
        }

        for (int i = 0; i < screenObjectList.Count; i++)
        {
            screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        }

        for (int i = 0; i < gameObjectList.Count; i++)
        {
            gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        }

        redCarpet.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);


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

            warningIcon.enabled = true;

            warningLabel.text = warning1 + screenList + warning2;
            Debug.Log("CAUTION: " + screenList + " ARE UNREACHABLE. YOUR CUSTOMERS FOR THIS SCREEN WILL NOT REACH THE SCREEN AND WILL LEAVE - RUINING THE REPUTATION OF YOUR FINE CINEMA!");
        }
        else
        {
            warningIcon.enabled = false;
        }
        warningPanel.SetActive(false);
    }

    public void DisplayWarning()
    {
        warningPanel.SetActive(!warningPanel.active);
    }

    public GameObject popupBox;

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
                    GameObject hiddenBehind = mouseDrag.CheckHiddenBehind(staffBounds, gameObjectList, screenObjectList);

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

            if (!theScreens[id].ConstructionInProgress())
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
                    GameObject hiddenBehind = mouseDrag.CheckHiddenBehind(staffBounds, gameObjectList, screenObjectList);

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

    public void ShowEndOfDayPopup(int todaysMoney, int walkouts, int repChange, int numCustomers)
    {

        // get values here - pass some as parameters
        newStatusCode = 0;

        popupBox.SetActive(true);

        Text[] txts = popupBox.GetComponentsInChildren<Text>();
        txts[3].text = totalCoins.ToString();
        txts[4].text = todaysMoney.ToString();
        txts[6].text = repChange.ToString() + "%";
        txts[7].text = numCustomers.ToString();
        txts[8].text = walkouts.ToString();

        statusCode = 9;

    }
    
    public void MoveScreen()
    {

        for (int i = 0; i < staffMembers.Count; i++)
        {
            staffMembers[i].GetTransform().position = new Vector3(staffMembers[i].GetTransform().position.x, staffMembers[i].GetTransform().position.y, 0);
        }
        
        for (int i = 0; i < screenObjectList.Count; i++)
        {
            screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
        }
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
        }
        redCarpet.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);

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
        confirmMovePanel.SetActive(true);
        moveButtons.SetActive(true);
        objectInfo.SetActive(false);
        closeInfo.SetActive(false);

        statusCode = 2;

        for (int i = 0; i < screenObjectList.Count; i++)
        {
            if (screenObjectList[i].name.Equals(objectSelected))
            {
                screenObjectList[i].GetComponent<Renderer>().enabled = false;

                String[] tmp = screenObjectList[i].name.Split('#');
                int index = -1;

                for (int j = 0; j < theScreens.Count; j++)
                {
                    if (theScreens[j].GetScreenNumber() == int.Parse(tmp[1]))
                    {
                        index = j;
                        break;
                    }
                }

                int x = theScreens[index].GetX(); //-4
                int y = theScreens[index].GetY(); //-6


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
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            if (gameObjectList[i].name.Equals(objectSelected))
            {
                gameObjectList[i].GetComponent<Renderer>().enabled = false;
                SpriteRenderer[] subImages = gameObjectList[i].GetComponentsInChildren<SpriteRenderer>();

                for (int j = 0; j < subImages.Length; j++)
                {
                    subImages[j].GetComponent<SpriteRenderer>().enabled = false;
                }

                int x = otherObjects[i].xPos; //-4
                int y = otherObjects[i].yPos; //-6


                int w = 0;
                int h = 0;

                if (gameObjectList[i].tag.Equals("Plant"))
                {
                    itemToAddID = 2;
                    w = 1; h = 1;
                }
                else if (gameObjectList[i].tag.Equals("Bust"))
                {
                    itemToAddID = 3;
                    w = 2; h = 3;
                }
                else if (gameObjectList[i].tag.Equals("Vending Machine"))
                {
                    itemToAddID = 5;
                    w = 3; h = 3;
                }
                else if (gameObjectList[i].tag.Equals("Food Area"))
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

        shopCanvas.SetActive(false);
        confirmMovePanel.SetActive(true);
        moveButtons.SetActive(true);

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

            int foundPos = -1;

            for (int i = 0; i < screenObjectList.Count; i++)
            {
                if (objectSelected.Equals(screenObjectList[i].name))
                {
                    DestroyBuilderByScreenID(theScreens[i].GetScreenNumber());
                    screenObjectList.RemoveAt(i);
                    theScreens.RemoveAt(i);
                    foundPos = i;
                    break;
                }
            }

            if (foundPos > -1)
            {
                for (int i = foundPos; i < theScreens.Count; i++)
                {
                    theScreens[i].DecreaseScreenNumber();
                    screenObjectList[i].name = "Screen#" + theScreens[i].GetScreenNumber();
                    GameObject.Find("BuilderForScreen#" + (theScreens[i].GetScreenNumber() + 1)).name = "BuilderForScreen#" + theScreens[i].GetScreenNumber();
                }
            }

        }
        else
        {
            int foundPos = -1;

            for (int i = 0; i < gameObjectList.Count; i++)
            {
                if (objectSelected.Equals(gameObjectList[i].name))
                {
                    gameObjectList.RemoveAt(i);
                    otherObjects.RemoveAt(i);
                    foundPos = i;
                }
            }

            if (foundPos > -1)
            {
                for (int i = foundPos; i < otherObjects.Count; i++)
                {
                    otherObjects[i].id -= 1;
                    gameObjectList[i].name = "Element#" + otherObjects[i].id;
                }
            }

        }

        GameObject.Destroy(go);
        theTileManager.fullHeight = h;
        theTileManager.fullWidth = w;
        HideObjectInfo();
        SetTiles(0, x, y, w, h);
        theTileManager.ColourAllTiles(x, y, carpetColour);
        theTileManager.fullHeight = -1;
        theTileManager.fullHeight = -1;

        theTileManager.ShowOutput();
    }

    void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveState.gd");

        PlayerData data = new PlayerData(theScreens, carpetColour, staffMembers, filmShowings, totalCoins, currDay, numPopcorn, otherObjects, hasUnlockedRedCarpet, isMarbleFloor, reputation, boxOfficeLevel, foodArea, postersUnlocked);


        formatter.Serialize(file, data);
        file.Close();
    }

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

    public int GetStaffSize()
    {
        return staffMembers.Count;
    }

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

    public void SemiTransparentObjects()
    {
        for (int i = 0; i < screenObjectList.Count; i++)
        {
            screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
        }
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
        }

        redCarpet.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
    }

    public void ShopItemSelected(int w, int h, string type)
    {
        objectSelected = "NEW " + type;
        PlaceObject(w, h);
    }

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

    public int GetStaffJobById(int index) { return staffMembers[index].GetJobID(); }

    void ShowBuildingOptions(string line1, string line2, Sprite theImage, int constrDone, int constrTotal)
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
                images[3].sprite = completeFoodAreaSprite;
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

        }
        else
        {
            //hide the bar and label
            images[4].enabled = false;
            images[5].enabled = false;
            labels[3].enabled = false;
            labels[2].enabled = false;
        }
        

    }

    public List<StaffMember> GetFullStaffList()
    {
        return staffMembers;
    }

    private void AddToQueueTickets(Customer customer)
    {
        ticketQueue.AddCustomer(customer);
    }
    private void AddToQueueFood(Customer customer)
    {
        foodQueue.AddCustomer(customer);
        Debug.Log("FOOD: " + foodQueue.GetQueueSize());
    }
}

[Serializable]
public class PlayerData
{
    //Transform[] gameObjectList;
    public ScreenObject[] theScreens;
    public float[] carpetColour;
    public SaveableStaff[] staffMembers;
    public FilmShowing[] filmShowings;
    public int totalCoins;
    public int numPopcorn;
    public int currentDay;
    public OtherObject[] otherObjects;
    public bool hasRedCarpet;
    public bool marbleFloor;
    public Reputation reputation;
    public int boxOfficeLevel;
    public FoodArea foodArea;
    public bool[] posters;

    public PlayerData(List<ScreenObject> screens, Color col, List<StaffMember> staff, List<FilmShowing> films, int coins, int day, int popcorn, List<OtherObject> others, bool redCarpet, bool marble, Reputation rep, int boxOffice, FoodArea fa, bool[] poster)
    {
        theScreens = screens.ToArray();
        carpetColour = new float[4] { col.r, col.g, col.b, col.a };

        List<SaveableStaff> staffList = new List<SaveableStaff>();

        for (int i = 0; i < staff.Count; i++)
        {
            SaveableStaff s = new SaveableStaff(staff[i]);
            staffList.Add(s);
        }

        staffMembers = staffList.ToArray();
        filmShowings = films.ToArray();
        totalCoins = coins;
        currentDay = day;
        numPopcorn = popcorn;
        otherObjects = others.ToArray();
        hasRedCarpet = redCarpet;
        marbleFloor = marble;
        reputation = rep;
        boxOfficeLevel = boxOffice;
        posters = poster;
    }

}


[Serializable]
public class OtherObject
{
    public int xPos;
    public int yPos;
    public int type;
    public int id;

    public OtherObject(int x, int y, int t, int i)
    {
        xPos = x;
        yPos = y;
        type = t;
        id = i;
    }
}