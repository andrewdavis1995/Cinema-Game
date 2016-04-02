using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;
using Assets.Classes;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class Controller : MonoBehaviour
{
    public int selectedStaff = -1;
    

    int numWalkouts = 0;

    public GameObject[] walls;

    public Transform builderPrefab;

    public Reputation reputation;

    public Sprite[] validityTiles;

    public GameObject picProfile;
    public static Sprite profilePicture;

    public List<Transform> staffSlot = new List<Transform>();
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

    public List<GameObject> gameObjectList = new List<GameObject>();
    public List<GameObject> screenObjectList = new List<GameObject>();

    public static List<ScreenObject> theScreens = new List<ScreenObject>();
    List<OtherObject> otherObjects = new List<OtherObject>();

    Queue<Customer> ticketQueue = new Queue<Customer>();
    public List<Customer> allCustomers = new List<Customer>();

    public int statusCode = 0;     // 0 = free, 1 = dragging staff, 2 = moving object, 3 = in menu, 4 = moving camera, 5 = shop, 6 = staff menu, 7 = staff member info, 8 = Confirmation page, 9 = popup

    public Color carpetColour;

    public Transform greenGuy;
    public Transform blueGuy;
    public Transform orangeGuy;


    public GameObject staffMemberInfo;

    public Transform screen;
    public Transform plant;
    public Transform bust;
    public Transform vendingMachine;
    public Transform[] staffPrefabs;

    public Text timeLabel;
    public Text dayLabel;
    public Text coinLabel;
    public Text popcornLabel;

    public Button shopButton;
    public Button staffMenuButton;


    public List<StaffMember> staffMembers = new List<StaffMember>();

    public List<FilmShowing> filmShowings = new List<FilmShowing>();

    List<StaffMember> ticketStaff = new List<StaffMember>();
    public TileManager theTileManager;
    public GameObject confirmPanel;

    public delegate void doneWithQueue();
    public static event doneWithQueue queueDone;

    public delegate void setTileStates(int startX, int startY, int w, int h, int newState, bool complete);
    public static event setTileStates updateTileState;

    public Sprite ColourBackground;
    public Sprite colourCircle;
    public Sprite marbleBackground;
    public Sprite[] marbleSquares;
    public bool isMarbleFloor = false;

    const int width = 80;
    const int height = 40;

    public GameObject[,] floorTiles;

    //List<GameObject> customerObjects = new List<GameObject>();

    public bool simulationRunning = false;

    public GameObject warningPanel;
    public Image warningIcon;
    public Text warningLabel;


    List<List<Coordinate>> screenPaths = new List<List<Coordinate>>();
    List<Coordinate> exitPath = new List<Coordinate>();

    public int totalCoins = 1000;
    public int numPopcorn = 15;

    public int currDay = 0;

    int numScreens = 1;

    float count = 0;

    public int minutes = 0;
    public int hours = 9;

    public void OpenShop()
    {
        //if (statusCode == 0)
        //{
        statusCode = 5;
        shopCanvas.SetActive(true);
        //}
        //else
        //{ 
        //    shopCanvas.SetActive(false);
        //}
    }

    public void OpenStaffMenu()
    {
        if (statusCode == 0)
        {
            statusCode = 6;
            staffMenu.gameObject.SetActiveRecursively(true);
        }
    }



    // Use this for initialization
    void Start()
    {

        picProfile.GetComponent<SpriteRenderer>().sprite = profilePicture;

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
        #endregion

        #region Add Delegate references
        mouseDrag.addStaff += addStaffMember;
        mouseDrag.getStaffListSize += getStaffSize;
        mouseDrag.getStaffJobById += getStaffJobById;
        mouseDrag.changeStaffJob += updateStaffJob;
        mouseDrag.getStaffList += getFullStaffList;
        movementScript.addToQueueTickets += addToQueueTickets;
        movementScript.getQueueTickets += getTicketQueue;
        movementScript.getQueueTicketsSize += getTicketQueueSize;
        Screen_Script.showBuildingMenu += ShowBuildingOptions;
        OtherObjectScript.showBuildingMenu += ShowBuildingOptions;
        #endregion


        
        // TODO - load slots if load game
        for (int i = 0; i < 2; i++)
        {
            staffSlot.Add(Instantiate(slotPrefab, new Vector3(40, 12.4f - i * 10, 0), Quaternion.identity) as Transform);
            staffSlot[i].GetComponent<SpriteRenderer>().enabled = false;
        }


        #region Load / New Game
        // get Player data. If not null, load game
        if (ButtonScript.loadGame == null)
        {
            carpetColour = GetColourFromID(1);
            reputation = new Reputation();
            reputation.Initialise();

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
                theScreens[i].setPosition((int)pos.x, (int)pos.y);
            }
            theScreens[0].upgrade();
            theScreens[0].upgradeComplete();
            // NYAH

            nextDay(false);

            for (int i = 0; i < 2; i++)
            {
                int index = UnityEngine.Random.Range(0, 5);
                
                int x = 35 + (2 * (i % 6));
                int y = (2 * (i / 6));

                addStaffMember(new StaffMember(i, "Andrew", staffPrefabs[index], currDay, index), x, y);
            }

        }
        else
        {
            PlayerData data = ButtonScript.loadGame;

            carpetColour = new Color(data.carpetColour[0], data.carpetColour[1], data.carpetColour[2]);

            hasUnlockedRedCarpet = data.hasRedCarpet;

            if (hasUnlockedRedCarpet)
            {
                redCarpet.SetActive(true);
            }

            isMarbleFloor = data.marbleFloor;
            reputation = data.reputation;

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
                Transform transform = staffPrefabs[s[i].transformID];
                int dayHired = s[i].dayHired;
                int tID = s[i].transformID;
                int[] attributes = s[i].attributes;

                StaffMember newStaff = new StaffMember(id, name, transform, dayHired, tID);

                newStaff.SetAttributes(attributes);

                int x = 35 + (2 * (newStaff.getIndex() % 6)); ;
                int y = 2 * (newStaff.getIndex() / 6);

                addStaffMember(newStaff, x, y);
            }

            filmShowings = new List<FilmShowing>(data.filmShowings);
            totalCoins = data.totalCoins;
            currDay = data.currentDay;
            numScreens = theScreens.Count;
            numPopcorn = data.numPopcorn;
            otherObjects = new List<OtherObject>(data.otherObjects);


            nextDay(false);
            currDay--; // needed for some reason


            // hopefully un-fucks things
            for (int i = 0; i < theScreens.Count; i++)
            {
                Vector3 pos = new Vector3(theScreens[i].getX(), theScreens[i].getY(), 0);
                theScreens[i].setPosition((int)pos.x, (int)(pos.y));
            }

            dayLabel.text = "DAY: " + currDay.ToString();
            popcornLabel.text = numPopcorn.ToString();


        }
        #endregion

        // create some test screens
        for (int i = 0; i < theScreens.Count; i++)
        {
            Vector3 pos = new Vector3(theScreens[i].getX() + 0.05f, theScreens[i].getY() * 0.8f, 0);

            // align to grid - +/- 1 to move by one tile horizontally, 0.8 for vertical movement
            pos.x += 4.6f;
            pos.y += 6.05f;

            // change pos and element here


            GameObject instance = (GameObject)Instantiate(screen.gameObject, pos, Quaternion.identity);
            instance.GetComponent<Screen_Script>().theScreen = theScreens[i];
            instance.name = "Screen#" + theScreens[i].getScreenNumber();
            instance.tag = "Screen";
            instance.GetComponent<SpriteRenderer>().sortingOrder = height - theScreens[i].getY() - 1;
            if (!theScreens[i].ConstructionInProgress())
            {
                instance.GetComponent<SpriteRenderer>().sprite = screenImages[theScreens[i].getUpgradeLevel()];
            }
            else
            {
                instance.GetComponent<SpriteRenderer>().sprite = screenImages[0];
                CreateBuilder(theScreens[i].getX(), theScreens[i].getY(), theScreens[i].getScreenNumber());
            }

            screenObjectList.Add(instance);

            setTiles(2, (int)(theScreens[i].getX()), (int)(theScreens[i].getY()), 11, 15);
        }

        // do same for other objects
        for (int i = 0; i < otherObjects.Count; i++)
        {
            Vector3 pos = new Vector3(otherObjects[i].xPos + 0.05f, otherObjects[i].yPos * 0.8f, 0);

            float xCorrection = 0;
            float yCorrection = 0;

            Transform newItem = null;

            int w = 0;
            int h = 0;
            String tag = null;

            itemToAddID = otherObjects[i].type;

            if (itemToAddID == 2)
            {
                newItem = plant;
                xCorrection = 0.1f;
                yCorrection = 0.35f;
                w = 1; h = 1;
                tag = "Plant";
            }
            else if (itemToAddID == 3)
            {
                newItem = bust;
                xCorrection = 0.65f;
                yCorrection = 1.5f;
                w = 2; h = 2;
                tag = "Bust of Game Creator";
            }
            else if (itemToAddID == 5)
            {
                newItem = vendingMachine;
                xCorrection = 1.07f;
                yCorrection = 1.62f;
                w = 3; h = 3;
                tag = "Vending Machine";
            }

            // align to grid - +/- 1 to move by one tile horizontally, 0.8 for vertical movement
            pos.x += xCorrection;
            pos.y += yCorrection;

            // change pos and element here
            GameObject instance = (GameObject)Instantiate(newItem.gameObject, pos, Quaternion.identity);
            instance.name = "Element#" + (i);
            instance.GetComponent<SpriteRenderer>().sortingOrder = height - otherObjects[i].yPos - 1;
            instance.tag = tag;

            gameObjectList.Add(instance);

            setTiles(2, (int)(otherObjects[i].xPos), (int)(otherObjects[i].yPos), w, h);
        }

        //createColourPicker();

        if (updateTileState != null)
        {
            updateTileState(34, 0, 13, 16, 1, true);
            updateTileState(34, 16, 13, 4, 2, true);
        }


        coinLabel.text = totalCoins.ToString();
        popcornLabel.text = numPopcorn.ToString();


        GameObject[] pointers = GameObject.FindGameObjectsWithTag("Pointer");

        for (int i = 0; i < pointers.Length; i++)
        {
            pointers[i].GetComponent<Transform>().GetComponent<SpriteRenderer>().enabled = false;
        }
        

    }


    public void HidePopup()
    {
        popup.SetActive(false);
        confirmationPanel.SetActive(false);
        statusCode = 5;
    }

    public List<Coordinate> GetScreenPath(int index)
    {
        return this.screenPaths[index];
    }

    public void CreateBuilder(float x, float y, int screenNum)
    {
        GameObject builder = Instantiate(builderPrefab.gameObject, new Vector2(x + 1.8f, 0.8f * y + 0.5f), Quaternion.identity) as GameObject;
        builder.name = "BuilderForScreen#" + screenNum;
    }

    List<GameObject> staffMenuList = new List<GameObject>();

    void createStaff(StaffMember staff, int xPos, int yPos)
    {
        Vector3 pos = new Vector3(xPos, yPos);

        Transform t = staff.getTransform();
        
        GameObject goStaff = (GameObject)Instantiate(t.gameObject, pos, Quaternion.identity);
        goStaff.name = "Staff#" + staff.getIndex();
        goStaff.tag = "Staff";
        goStaff.GetComponent<mouseDrag>().staffMember = staff;

        float x = pos.x;
        float y = pos.y;

        goStaff.GetComponent<mouseDrag>().staffMember.SetVector(x, y);

        GameObject go = (GameObject)Instantiate(staffInfoObject.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.SetParent(staffList, false);

        go.transform.localPosition = new Vector3(0, 280 - (32 * staffMembers.Count), 0);

        Image[] imgs = go.GetComponentsInChildren<Image>();
        Text[] txts = go.GetComponentsInChildren<Text>();

        imgs[1].sprite = staff.getTransform().GetComponent<SpriteRenderer>().sprite;
        txts[0].text = staff.GetStaffname();

        Button b = imgs[2].GetComponent<Button>();
        b.onClick.AddListener(() => MoveToStaffLocation(staff.getIndex()));

        Button b2 = imgs[3].GetComponent<Button>();
        b2.onClick.AddListener(() => ViewStaffMemberInfo(staff.getIndex()));

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
        Sprite sprite = s.getTransform().GetComponent<SpriteRenderer>().sprite;

        // find the elements of the form
        Image[] imgs = staffMemberInfo.GetComponentsInChildren<Image>();
        Text[] txts = staffMemberInfo.GetComponentsInChildren<Text>();
        InputField ins = staffMemberInfo.GetComponentInChildren<InputField>();

        ins.text = name;
        txts[1].text = name;

        txts[7].text = "Worked here since: DAY " + dayHired;

        imgs[2].sprite = sprite;

        for (int i = 0; i < 4; i++)
        {
            imgs[7 + (i * 4)].fillAmount = 0.25f * attributes[i];
        }

    }

    public void StaffMemberUpdate()
    {
        Text[] txts = staffMemberInfo.GetComponentsInChildren<Text>();

        staffMembers[selectedStaff].UpdateName(txts[2].text);
        staffMemberInfo.SetActive(false);

        // update the object in the staffMenu
        Text[] txts2 = staffList.GetComponentsInChildren<Text>();

        int index = selectedStaff * 2;
        txts2[index].text = txts[2].text;

        selectedStaff = -1;
        statusCode = 0;
    }

    public void MoveToStaffLocation(int staffID)
    {
        Camera.main.orthographicSize = 5f;
        Vector3 pos = staffMembers[staffID].GetVector();
        pos.z = -10;

        //Camera.main.transform.position = (pos);
        Camera.main.GetComponent<CameraControls>().endPos = pos;
    }

    public void UpgradeStaffAttribute(int index)
    {
        //TODO: check if less than 4
        //TODO: check if enough money

        staffMembers[selectedStaff].Upgrade(index);

        Image[] imgs = staffMemberInfo.GetComponentsInChildren<Image>();

        //TODO: remove money
        int attributeEffected = staffMembers[selectedStaff].GetAttributes()[index];

        imgs[7 + (index * 4)].fillAmount = 0.25f * attributeEffected;

        NewTicketQueueSpeed();


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

    public void Upgrade()
    {
        for (int i = 0; i < screenObjectList.Count; i++)
        {
            if (screenObjectList[i].name.Equals(objectSelected))
            {
                ScreenObject theScreen = screenObjectList[i].GetComponent<Screen_Script>().theScreen;

                if (theScreen.getUpgradeLevel() < 4 && !theScreen.ConstructionInProgress())
                {

                    ConfirmationScript.OptionSelected(3, new string[] { "upgrade Screen " + theScreen.getScreenNumber(), (theScreen.calculateUpgradeCost()).ToString(), "0", i.ToString() });

                    break;
                }
            }
        }
    }

    public void updateTiles(int x, int y, int w, int h, int newState)
    {
        if (updateTileState != null)
        {
            updateTileState(x, y, w, h, newState, false);
        }
    }

    void setTiles(int newState, int x, int y, int width, int height)
    {
        Color newColour;

        if (newState != 0) { newColour = carpetColour; } else { newColour = Color.green; }

        ChangeColour(newColour, x, y, width, height);

        updateTiles(x, y, width, height, newState);
    }

    public void hideObjectInfo()
    {
        shopCanvas.SetActive(false);
        objectInfo.SetActive(false);
        closeInfo.SetActive(false);
        staffMenu.gameObject.SetActive(false);
        colourPicker.SetActive(false);
        staffMemberInfo.SetActive(false);

        statusCode = 0;
    }

    void newColourButton(int row, int column, bool texture)
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

    void createColourPicker()
    {
        Image[,] colours = new Image[3, 6];

        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 2; column++)
            {
                newColourButton(row, column, false);
            }
        }

        newColourButton(0, 2, true);

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

    Sprite GetSpriteFromID(int id)
    {
        switch (id)
        {
            case 4: isMarbleFloor = true; return marbleBackground;
            default: isMarbleFloor = false; return ColourBackground;
        }
    }


    public void colourClicked(int id)
    {
        carpetColour = GetColourFromID(id);
        Sprite s = GetSpriteFromID(id);

        //carpetRoll.GetComponent<SpriteRenderer>().color = carpetColour;
        CarpetRollScript.current.Begin(carpetColour, this);

        //for (int i = 0; i < height; i++)
        //{
        //    for (int j = 0; j < width; j++)
        //    {
        //        floorTiles[i, j].GetComponent<SpriteRenderer>().color = carpetColour;

        //        if (!s.Equals(marbleBackground))
        //        {
        //            floorTiles[i, j].GetComponent<SpriteRenderer>().sprite = ColourBackground;
        //        }
        //        else
        //        {
        //            int index = UnityEngine.Random.Range(0, 3);


        //            floorTiles[i, j].GetComponent<SpriteRenderer>().sprite = marbleSquares[index];

        //        }
        //    }
        //}


        // change step colours
        //steps.GetComponent<SpriteRenderer>().color = carpetColour;

    }

    void UpdateJobList()
    {
        ticketStaff = staffMembers.FindAll(
            delegate (StaffMember sm)
            {
                return sm.getJobID() == 1;
            }
            );

        NewTicketQueueSpeed();

    }

    void NewTicketQueueSpeed()
    {
        int overallLevel = 0;

        for (int i = 0; i < ticketStaff.Count; i++)
        {
            overallLevel += ticketStaff[i].GetAttributeByIndex(0);
        }

        ticketStaffLevel = 4.5f - (float)(1.125 * overallLevel / ticketStaff.Count);

        if (ticketStaffLevel < 0.5f) { ticketStaffLevel = 0.5f; }
    }

    public void ShowColourPicker()
    {
        colourPicker.SetActive(!colourPicker.active);
        shopCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }

        if (simulationRunning)
        {
            count += Time.deltaTime;

            QueueController();

            if (count > 0.12)
            {

                count = 0;
                minutes++;

                if (minutes > 59)
                {
                    hours++;
                    minutes = 0;
                }

                // checks for if hour > 11 - end day
                if (hours > 22)
                {
                    nextDay(true);
                }

            }

            string minString = minutes.ToString();
            string hourString = hours.ToString();

            if (minutes < 10)
            {
                minString = "0" + minString;
            }
            if (hours < 10)
            {
                hourString = "0" + hourString;
            }

            timeLabel.text = hourString + ":" + minString;

        }

        for (int i = 0; i < allCustomers.Count; i++)
        {
            if (!allCustomers[i].arrived)
            {
                if (allCustomers[i].hasArrived(hours, minutes) && simulationRunning)        // second simulationRunningCheck needed because of delays at the end
                {
                    allCustomers[i].pointsToVisit = new List<Coordinate>();

                    allCustomers[i].transform = ObjectPool.current.GetGameObject().transform;

                    allCustomers[i].transform.position = new Vector3(40, -10);

                    allCustomers[i].transform.GetComponent<movementScript>().setCustomer(allCustomers[i]);

                    allCustomers[i].transform.gameObject.SetActive(true);

                    allCustomers[i].animator = allCustomers[i].transform.GetComponent<Animator>();

                    allCustomers[i].nextPoint(true);

                    float left = allCustomers[i].getTravellingToX();

                    allCustomers[i].transform.position = new Vector3(left, -15, 0); // y = -11

                }
            }
        }

    }

    public void startDay()
    {

        //for (int i = 0; i < screenObjectList.Count; i++)
        //{
        //    screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.55f);
        //}

        screenPaths.Clear();

        // find paths to allScreens
        for (int i = 0; i < theScreens.Count; i++)
        {
            List<Coordinate> points = theTileManager.floor.FindPath(38, 11, theScreens[i].getX() + 5, theScreens[i].getY());
            screenPaths.Add(points);
        }




        // hide the button
        startDayButton.SetActive(false);
        shopButton.gameObject.SetActive(false);
        staffMenuButton.gameObject.SetActive(false);

        Customer.tiles = floorTiles;

        if (!simulationRunning)
        {
            // start the running of the 'day'
            simulationRunning = true;
        }

    }

    public void nextDay(bool shouldCollect)
    {

        simulationRunning = false;

        queueCount = 0;
        ticketQueue.Clear();

        for (int i = 0; i < allCustomers.Count; i++)
        {
            if (allCustomers[i].transform != null)
            {
                allCustomers[i].transform.gameObject.SetActive(false);
            }
        }
        allCustomers.Clear();

        startDayButton.SetActive(true);
        shopButton.gameObject.SetActive(true);
        staffMenuButton.gameObject.SetActive(true);


        int money = getTodaysMoney();

        if (shouldCollect)
        {
            totalCoins += money;
            reputation.AddCoins(money);
            // output coins
            coinLabel.text = totalCoins.ToString();
            //lblCoins.Text = totalCoins.ToString();
        }

        currDay++;


        if (currDay % 7 == 1)
        {
            nextWeek();
        }

        if (shouldCollect) {
            for (int i = 0; i < theScreens.Count; i++)
            {
                int prevDays = theScreens[i].GetDaysOfConstruction();

                theScreens[i].progressOneDay();
                int days = theScreens[i].GetDaysOfConstruction();

                if (prevDays == 1 && days == 0 && screenObjectList.Count > 0)
                {

                    reputation.SetFacilities(theScreens, redCarpet);

                    screenObjectList[i].GetComponent<SpriteRenderer>().sprite = screenImages[theScreens[i].getUpgradeLevel()];
                    newShowTimes();

                    for (int k = 0; k < filmShowings.Count; k++)       // filmShowings.Count
                    {
                        int index = filmShowings[k].getScreenNumber();
                        int ticketsSold = getTicketsSoldValue(theScreens[index - 1]);
                        filmShowings[k].setTicketsSold(ticketsSold);

                        int currentCount = 0;

                        for (int j = 0; j < k; j++)
                        {
                            currentCount += filmShowings[j].getTicketsSold();
                        }

                        List<Customer> tmp = filmShowings[k].createCustomerList(currentCount, this);
                        allCustomers.AddRange(tmp);

                        DestroyBuilderByScreenID(filmShowings[k].getScreenNumber());

                    }
                }
            }
        }

        for (int i = 0; i < filmShowings.Count; i++)       // filmShowings.Count
        {
            int index = filmShowings[i].getScreenNumber();
            int ticketsSold = getTicketsSoldValue(theScreens[index - 1]);
            filmShowings[i].setTicketsSold(ticketsSold);

            int currentCount = 0;

            for (int j = 0; j < i; j++)
            {
                currentCount += filmShowings[j].getTicketsSold();
            }

            List<Customer> tmp = filmShowings[i].createCustomerList(currentCount, this);
            allCustomers.AddRange(tmp);
        }

        // update day output 
        dayLabel.text = "DAY: " + currDay.ToString();

        count = 0;
        minutes = 0;
        hours = 9;

        timeLabel.text = "09:00";

        movementScript.customerStatus.SetActive(false);

        statusCode = 0;

        if (shouldCollect)
        {
            int oldOverall = reputation.GetOverall();
            reputation.SetOverall();

            int newOverall = reputation.GetOverall();

            int repChange = newOverall - oldOverall;

            ShowEndOfDayPopup(money, numWalkouts, repChange, allCustomers.Count);
        }


        GameObject[] staffObjects = GameObject.FindGameObjectsWithTag("Staff");

        for (int i = 0; i < staffObjects.Length; i++)
        {

            int x = 35 + (2 * (i % 6));
            int y = (2 * (i / 6));

            Transform t = staffObjects[i].transform;
            t.position = new Vector3(x, y, 0);

            staffObjects[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

            try
            {
                Transform pi3 = transform.FindChild("hiddenPointer");
                pi3.GetComponent<SpriteRenderer>().enabled = true;
            }
            catch (Exception) { }
            //t.Translate(new Vector3(10, o0, 0));

        }


        numWalkouts = 0;

        Save();

    }

    void nextWeek()
    {
        newShowTimes();
    }


    public void ViewReputation()
    {
        popupBox.SetActive(false);
        reputationPage.SetActive(true);

        Text[] textElements = reputationPage.gameObject.GetComponentsInChildren<Text>();

        textElements[2].text = reputation.GetTotalCoins().ToString();
        textElements[4].text = reputation.GetOverall().ToString() + "%";
        textElements[5].text = reputation.GetTotalCustomers().ToString();
        textElements[6].text = reputation.GetHighestRep().ToString();

        textElements[9].text = (4 * reputation.GetSpeedRating()).ToString();
        textElements[11].text = (4 * reputation.GetCleanlinessRating()).ToString();
        textElements[13].text = (4 * reputation.GetFacilitiesRating()).ToString();
        textElements[15].text = (4 * reputation.GetFriendlinessRating()).ToString();

        
        Image[] imageElements = reputationPage.gameObject.GetComponentsInChildren<Image>();
        imageElements[4].fillAmount = (float)reputation.GetSpeedRating() / 25f;
        imageElements[7].fillAmount = (float)reputation.GetCleanlinessRating() / 25f;
        imageElements[10].fillAmount = (float)reputation.GetFacilitiesRating() / 25f;
        imageElements[13].fillAmount = (float)reputation.GetFriendlinessRating() / 25f;
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

    private int getTodaysMoney()
    {
        int totalIntake = 0;

        // each ticket is 2 coins + 1 for each upgrade level
        for (int i = 0; i < filmShowings.Count; i++)
        {
            int screenNum = filmShowings[i].getScreenNumber() - 1;

            if (!theScreens[screenNum].ConstructionInProgress())
            {
                int upgradeLevel = theScreens[screenNum].getUpgradeLevel();
                int numCustomers = filmShowings[i].getTicketsSold();

                totalIntake += (2 + (1 * upgradeLevel)) * numCustomers;
            }
        }

        return totalIntake;
    }

    public int getTicketsSoldValue(ScreenObject screen)
    {
        UnityEngine.Random ran = new UnityEngine.Random();
        int min = (int)(screen.getNumSeats() / 1.5);  // this will be affected by the posters etc, and rep
        int max = screen.getNumSeats();
        int ticketsSold = UnityEngine.Random.Range(min, max);
        float repMultiplier = reputation.GetMultiplier();
        ticketsSold = (int)(ticketsSold * repMultiplier);

        return ticketsSold;
    }

    private TimeTuple getShowTime(int i)
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

    public void newShowTimes()
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
                    TimeTuple showTime = getShowTime(j);

                    FilmShowing newFilm = new FilmShowing(filmShowings.Count, i + 1, 0, showTime.hours, showTime.minutes, theTileManager.floor);
                    filmShowings.Add(newFilm);
                }
            }
        }
    }

    float queueCount = 0;
    float ticketStaffLevel = 4.5f;

    void QueueController()
    {
        if (ticketQueue.Count > 0)
        {
            queueCount += Time.deltaTime;

            if (queueCount > ticketStaffLevel)
            {
                queueCount = 0;

                for (int i = 0; i < ticketStaff.Count; i++)
                {
                    int index = ticketQueue.Peek().getCharIndex();

                    //if (queueDone != null)
                    //{
                    //    queueDone();
                    //    allCustomers[index].nextPoint(false);
                    //}
                    allCustomers[index].doneWithQueue();

                    ticketQueue.Dequeue();

                    for (int j = 0; j < allCustomers.Count; j++)
                    {
                        if (allCustomers[j].inQueue && allCustomers[j].GetPatience() > 0)
                        {
                            allCustomers[j].transform.position = new Vector2(allCustomers[j].transform.position.x, allCustomers[j].transform.position.y + 0.8f);
                        }
                    }

                }

                for (int j = 0; j < allCustomers.Count; j++)
                {
                    if (allCustomers[j].inQueue)
                    {
                        allCustomers[j].DecreasePatience(50);

                        if (allCustomers[j].GetPatience() < 1)
                        {
                            WalkAway(j);
                            numWalkouts++;

                            // negatively effect rep
                            reputation.Walkout();

                        }
                        else if (allCustomers[j].GetPatience() == 150)
                        {
                            allCustomers[j].animator.SetTrigger("bored");
                        }
                    }
                }

            }


        }
        else {
            queueCount = 0;
        }
    }



    // move to movementScript
    void WalkAway(int index)
    {
        int x = (int)Math.Round(allCustomers[index].transform.position.x);
        int y = (int)Math.Round(allCustomers[index].transform.position.y);

        allCustomers[index].SetTravellingTo(x + 0.5f, y);
        allCustomers[index].inQueue = false;

        allCustomers[index].pointsToVisit = theTileManager.floor.FindPath(x, y, 45, 0);

        // have to do the dequeue thing - but from middle... may need to change structure
    }

    public void objectMoveComplete(bool confirmed)
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

            GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");
            for (int i = 0; i < staff.Length; i++)
            {
                staff[i].GetComponent<SpriteRenderer>().enabled = true;
            }
            GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");
            for (int i = 0; i < builders.Length; i++)
            {
                builders[i].GetComponent<SpriteRenderer>().enabled = true;
            }

            string[] tmp = objectSelected.Split('#');
            int id = int.Parse(tmp[1]);


            Transform newItem = null;


            if (itemToAddID == 0)
            {
                id -= 1;

                Vector3 pos = new Vector3(x + 0.05f, y * 0.8f, 0);

                theScreens[id].setPosition(x, y);

                pos.x = pos.x += 4.6f;
                pos.y += 6.05f;

                ScreenObject temp = null;

                GameObject theScreen = GameObject.Find("Screen#" + theScreens[id].getScreenNumber());
                temp = theScreen.GetComponent<Screen_Script>().theScreen;

                theScreen.GetComponent<SpriteRenderer>().sortingOrder = height - y - 1;
                theScreen.transform.position = pos;
                if (temp.ConstructionInProgress())
                {
                    theScreen.GetComponent<SpriteRenderer>().sprite = screenImages[0];
                }

                theScreen.GetComponent<Renderer>().enabled = true;


                if (temp.ConstructionInProgress())
                {
                    for (int i = 0; i < builders.Length; i++)
                    {
                        if (builders[i].name.Contains(temp.getScreenNumber().ToString()))
                        {
                            builders[i].transform.position = new Vector2(temp.getX() + 1.8f, (0.8f * temp.getY()));
                        }
                    }
                }






            }
            else {


                Vector3 pos = new Vector3(x + 0.05f, y * 0.8f, 0);

                otherObjects[id].xPos = x;
                otherObjects[id].yPos = y;

                float xCorrection = 0;
                float yCorrection = 0;

                if (itemToAddID == 2)
                {
                    xCorrection = 0.1f;
                    yCorrection = 0.35f;
                }
                else if (itemToAddID == 3)
                {
                    xCorrection = 0.65f;
                    yCorrection = 1.5f;
                }
                else if (itemToAddID == 5)
                {
                    xCorrection = 1.07f;
                    yCorrection = 1.62f;
                }

                pos.x += xCorrection;
                pos.y += yCorrection;

                string theTag = "";

                if (itemToAddID == 2)
                {
                    newItem = plant;
                    theTag = "Plant";
                }
                else if (itemToAddID == 3)
                {
                    newItem = bust;
                    theTag = "Bust of Game Creator";
                }
                else if (itemToAddID == 5)
                {
                    newItem = vendingMachine;
                    theTag = "Vending Machine";
                }

                GameObject theObject = GameObject.Find("Element#" + id);
                theObject.transform.position = pos;

                theObject.GetComponent<Renderer>().enabled = true;
                theObject.GetComponent<SpriteRenderer>().sortingOrder = height - y - 1;

                //GameObject theObject = (GameObject)Instantiate(newItem.gameObject, pos, Quaternion.identity);
                //theObject.name = "Element#" + (otherObjects.Count - 1);
                //theObject.GetComponent<SpriteRenderer>().sortingOrder = height - y;
                //theObject.tag = theTag;
                //gameObjectList.Add(theObject);
            }
            itemToAddID = -1;

            setTiles(2, x, y, theTileManager.fullWidth, theTileManager.fullHeight);

            if (!confirmed)
            {
                setTiles(0, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);
                ChangeColour(carpetColour, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);

                staff = GameObject.FindGameObjectsWithTag("Staff");
                for (int i = 0; i < staff.Length; i++)
                {
                    staff[i].GetComponent<SpriteRenderer>().enabled = true;
                }

                builders = GameObject.FindGameObjectsWithTag("Builder");
                for (int i = 0; i < builders.Length; i++)
                {
                    builders[i].GetComponent<SpriteRenderer>().enabled = true;
                }
            }

            CheckForPath();
            theTileManager.origX = -1;
            theTileManager.origY = -1;
            theTileManager.toMoveX = -1;
            theTileManager.toMoveY = -1;
            theTileManager.fullWidth = -1;
            theTileManager.fullHeight = -1;

            statusCode = 0;
            confirmMovePanel.SetActive(false);
            moveButtons.SetActive(false);



            for (int i = 0; i < screenObjectList.Count; i++)
            {
                screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
            }

            for (int i = 0; i < gameObjectList.Count; i++)
            {
                gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
            }

        }
        else if (confirmed)
        {
            String[] parameters = new String[5];
            parameters[0] = "Add this object?";
            parameters[1] = getCost().ToString();
            parameters[2] = getCurrency();
            parameters[3] = theTileManager.toMoveX.ToString();
            parameters[4] = theTileManager.toMoveY.ToString();
            ConfirmationScript.OptionSelected(0, parameters);

        }
        else {
            ChangeColour(carpetColour, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);
            theTileManager.origX = -1;
            theTileManager.origY = -1;
            theTileManager.toMoveX = -1;
            theTileManager.toMoveY = -1;
            theTileManager.fullWidth = -1;
            theTileManager.fullHeight = -1;
            confirmMovePanel.SetActive(false);
            moveButtons.SetActive(false);

            statusCode = 0;

            GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");
            for (int i = 0; i < staff.Length; i++)
            {
                staff[i].GetComponent<SpriteRenderer>().enabled = true;
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
        }
        

    }


    int getCost()
    {
        switch (objectSelected)
        {
            case "NEW SCREEN": return 500;
            case "NEW PLANT": return 80;
            case "NEW BUST": return 7;
            case "NEW VENDING MACHINE": return 90;
            default: return 0;
        }
    }
    String getCurrency()
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

        Vector3 pos = new Vector3(x + 0.05f, y * 0.8f, 0);


        Transform newItem = null;
        float xCorrection = 0;
        float yCorrection = 0;

        if (itemToAddID == 0)
        {
            newItem = screen;
            xCorrection = 4.6f;
            yCorrection = 6.05f;

            int newID = theScreens.Count;
            ScreenObject aScreen = new ScreenObject(newID + 1, 0);
            aScreen.setPosition(x, y);
            aScreen.upgrade();
            theScreens.Add(aScreen);

            pos.x += xCorrection;
            pos.y += yCorrection;

            GameObject screenThing = (GameObject)Instantiate(screen.gameObject, pos, Quaternion.identity) as GameObject;
            screenThing.GetComponent<Screen_Script>().theScreen = theScreens[newID];
            screenThing.name = "Screen#" + theScreens[newID].getScreenNumber();
            screenThing.GetComponent<SpriteRenderer>().sortingOrder = height - y - 1;
            screenThing.GetComponent<SpriteRenderer>().sprite = screenImages[0];

            screenThing.tag = "Screen";
            screenObjectList.Add(screenThing);

            CreateBuilder(x, y, theScreens[newID].getScreenNumber());


        }
        else {

            string theTag = "";

            if (itemToAddID == 2)
            {
                newItem = plant;
                xCorrection = 0.1f;
                yCorrection = 0.35f;
                otherObjects.Add(new OtherObject(x, y, 2, otherObjects.Count));
                theTag = "Plant";
            }
            else if (itemToAddID == 3)
            {
                newItem = bust;
                xCorrection = 0.65f;
                yCorrection = 1.5f;
                otherObjects.Add(new OtherObject(x, y, 3, otherObjects.Count));
                theTag = "Bust of Game Creator";
            }
            else if (itemToAddID == 5)
            {
                newItem = vendingMachine;
                xCorrection = 1.07f;
                yCorrection = 1.62f;
                otherObjects.Add(new OtherObject(x, y, 5, otherObjects.Count));
                theTag = "Vending Machine";
            }


            pos.x += xCorrection;
            pos.y += yCorrection;


            GameObject theObject = (GameObject)Instantiate(newItem.gameObject, pos, Quaternion.identity) as GameObject;
            theObject.name = "Element#" + (otherObjects.Count - 1);
            theObject.tag = theTag;
            theObject.GetComponent<SpriteRenderer>().sortingOrder = height - y - 1;

            gameObjectList.Add(theObject);

        }
        itemToAddID = -1;

        setTiles(2, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);
        ChangeColour(carpetColour, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);

        CheckForPath();

        theTileManager.origX = -1;
        theTileManager.origY = -1;
        theTileManager.toMoveX = -1;
        theTileManager.toMoveY = -1;
        theTileManager.fullWidth = -1;
        theTileManager.fullHeight = -1;

        statusCode = 0;

        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");
        for (int i = 0; i < staff.Length; i++)
        {
            staff[i].GetComponent<SpriteRenderer>().enabled = true;
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


    public void ShowEndOfDayPopup(int todaysMoney, int walkouts, int repChange, int numCustomers)
    {

        // get values here - pass some as parameters


        popupBox.SetActive(true);

        Text[] txts = popupBox.GetComponentsInChildren<Text>();
        txts[3].text = totalCoins.ToString();
        txts[4].text = todaysMoney.ToString();
        txts[6].text = repChange.ToString() + "%";
        txts[7].text = numCustomers.ToString();
        txts[8].text = walkouts.ToString();

        statusCode = 9;

    }

    public void ClosePopup()
    {
        popupBox.SetActive(false);
        statusCode = 0;
    }


    public void moveScreen()
    {
        
        for (int i = 0; i < screenObjectList.Count; i++)
        {
            screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
        }
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
        }

        // hide staff
        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");

        for (int i = 0; i < staff.Length; i++)
        {
            staff[i].GetComponent<SpriteRenderer>().enabled = false;
        }

        GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");

        for (int i = 0; i < builders.Length; i++)
        {
            builders[i].GetComponent<SpriteRenderer>().enabled = false;
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
                    if (theScreens[j].getScreenNumber() == int.Parse(tmp[1]))
                    {
                        index = j;
                        break;
                    }
                }

                int x = theScreens[index].getX(); //-4
                int y = theScreens[index].getY(); //-6


                itemToAddID = 0;

                theTileManager.toMoveX = x;
                theTileManager.toMoveY = y;

                theTileManager.origX = x;
                theTileManager.origY = y;

                theTileManager.fullWidth = 11;
                theTileManager.fullHeight = 15;

                setTiles(0, x, y, 11, 15);

                break;
            }
        }
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            if (gameObjectList[i].name.Equals(objectSelected))
            {
                gameObjectList[i].GetComponent<Renderer>().enabled = false;
                int x = otherObjects[i].xPos; //-4
                int y = otherObjects[i].yPos; //-6


                int w = 0;
                int h = 0;

                if (gameObjectList[i].tag.Equals("Plant"))
                {
                    itemToAddID = 2;
                    w = 1; h = 1;
                }
                else if (gameObjectList[i].tag.Equals("Bust of Game Creator"))
                {
                    itemToAddID = 3;
                    w = 2; h = 3;
                }
                else if (gameObjectList[i].tag.Equals("Vending Machine"))
                {
                    itemToAddID = 5;
                    w = 3; h = 5;
                }

                theTileManager.toMoveX = x;
                theTileManager.toMoveY = y;

                theTileManager.origX = x;
                theTileManager.origY = y;



                theTileManager.fullWidth = w;
                theTileManager.fullHeight = h;

                setTiles(0, x, y, w, h);

                break;
            }
        }

    }


    public void placeObject(int width, int height)
    {


        int startX = (int)Camera.main.transform.position.x;
        int startY = (int)Camera.main.transform.position.y;

        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");

        for (int i = 0; i < staff.Length; i++)
        {
            staff[i].GetComponent<SpriteRenderer>().enabled = false;
        }

        GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");

        for (int i = 0; i < builders.Length; i++)
        {
            builders[i].GetComponent<SpriteRenderer>().enabled = false;
        }

        statusCode = 2;

        shopCanvas.SetActive(false);
        confirmMovePanel.SetActive(true);
        moveButtons.SetActive(true);

        bool valid = theTileManager.checkValidity(startX, startY, width, height);

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

    void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveState.gd");

        PlayerData data = new PlayerData(theScreens, carpetColour, staffMembers, filmShowings, totalCoins, currDay, numPopcorn, otherObjects, hasUnlockedRedCarpet, isMarbleFloor, reputation);


        formatter.Serialize(file, data);
        file.Close();
    }

    #region staff events
    public void addStaffMember(StaffMember staff, int xPos, int yPos)
    {
        staffMembers.Add(staff);
        createStaff(staff, xPos, yPos);
    }

    public int getStaffSize()
    {
        return staffMembers.Count;
    }

    public void updateStaffJob(int index, int job)
    {
       
        staffMembers[index].setJob(job);

        // TODO: Update the label in the staff list
        if (job != 0)
        {
            //GameObject[] gOs = GameObject.FindGameObjectsWithTag("StaffInfoItem");
            //staffMenuList[index
        }

        staffMenuList[index].GetComponentsInChildren<Text>()[1].text = "Current Job: " + jobTextFromID(job);

        UpdateJobList();
    }

    string jobTextFromID(int index)
    {
        switch (index)
        {
            case 1:
                return "Tickets";
            case 2:
                return "Front Door";
            default:
                return "Unassigned";
        }
    }


    public int getStaffJobById(int index) { return staffMembers[index].getJobID(); }

    void ShowBuildingOptions(string line1, string line2, Sprite theImage)
    {
        objectInfo.SetActive(true);
        closeInfo.SetActive(true);
        Text[] labels = objectInfo.gameObject.GetComponentsInChildren<Text>();
        labels[0].text = line1;
        labels[1].text = line2;
        Image[] images = objectInfo.gameObject.GetComponentsInChildren<Image>();

        if (line1.ToUpper().Contains("SCREEN")) { images[1].gameObject.GetComponent<Image>().color = Color.white; } else { images[1].gameObject.GetComponent<Image>().color = new Color(0.06f, 0.06f, 0.06f); }

        images[3].sprite = theImage;
    }

    public List<StaffMember> getFullStaffList()
    {
        return staffMembers;
    }
    #endregion

    #region Ticket Queue events
    private void addToQueueTickets(Customer customer)
    {
        ticketQueue.Enqueue(customer);
    }
    private Queue<Customer> getTicketQueue()
    {
        return this.ticketQueue;
    }
    private int getTicketQueueSize()
    {
        return this.ticketQueue.Count;
    }
    #endregion
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
    

    public PlayerData(List<ScreenObject> screens, Color col, List<StaffMember> staff, List<FilmShowing> films, int coins, int day, int popcorn, List<OtherObject> others, bool redCarpet, bool marble, Reputation rep)
    {
        //gameObjectList = sO.ToArray();
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
        this.reputation = rep;
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