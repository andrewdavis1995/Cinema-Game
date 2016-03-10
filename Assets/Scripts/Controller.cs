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
    int selectedStaff = -1;

    public Transform staffMenu;
    public Transform staffInfoObject;
    public Transform staffList;

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

    public GameObject confirmationPanel;

    const string warning1 = "WARNING! \n\nThe Following Screen(s) are inaccessible to Customers:\n\n";
    const string warning2 = "\nYou have built objects which block the path to these Screens. If you do not move them, the customers for these screens will leave and you will not get money from them! Plus, your reputation will be ruined!";

    public string objectSelected = "";
    
    List<GameObject> gameObjectList = new List<GameObject>();
    List<GameObject> screenObjectList = new List<GameObject>();

    public static List<ScreenObject> theScreens = new List<ScreenObject>();
    List<OtherObject> otherObjects = new List<OtherObject>();

    Queue<Customer> ticketQueue = new Queue<Customer>();
    List<Customer> allCustomers = new List<Customer>();

    public int statusCode = 0;     // 0 = free, 1 = dragging staff, 2 = moving object, 3 = in menu, 4 = moving camera, 5 = shop, 6 = staff menu, 7 = staff member info

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

    Button shopButton;
    Button staffMenuButton;

    
    public List<StaffMember> staffMembers = new List<StaffMember>();

    private List<FilmShowing> filmShowings = new List<FilmShowing>();

    List<StaffMember> ticketStaff = new List<StaffMember>();
    TileManager theTileManager;
    public GameObject confirmPanel;

    public delegate void doneWithQueue(Customer c);
    public static event doneWithQueue queueDone;

    public delegate void setTileStates(int startX, int startY, int w, int h, int newState, bool complete);
    public static event setTileStates updateTileState;

    public Sprite ColourBackground;
    public Sprite colourCircle;
    public Sprite marbleBackground;
    public Sprite marbleSquare;

    const int width = 80;
    const int height = 40;
    
    public GameObject[,] floorTiles;

    //List<GameObject> customerObjects = new List<GameObject>();

    public bool simulationRunning = false;

    public GameObject warningPanel;
    public Image warningIcon;
    public Text warningLabel;

    int totalCoins = 0;
    int numPopcorn = 0;

    public int currDay = 0;

    int numScreens = 1;

    float count = 0;

    public int minutes = 0;
    public int hours = 9;

    public void OpenShop()
    {
        if (shopButton.gameObject.active)
        {
            statusCode = 5;
            shopCanvas.SetActive(true);
        }
    }

    public void OpenStaffMenu()
    {
        //if (staffMenu.gameObject.active)
        //{
            statusCode = 6;
            staffMenu.gameObject.SetActive(true);
        //}
    }


    // Use this for initialization
    void Start()
    {

        #region Find Objects
        theTileManager = GameObject.Find("TileManagement").GetComponent<TileManager>();
        confirmPanel = GameObject.Find("pnlConfirm");
        //shopButton = GameObject.Find("cmdShop").GetComponent<Button>();
        shopButton = GameObject.Find("cmdStaffMenu").GetComponent<Button>();
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
        moveButtons.SetActive(false);
        custStatus.SetActive(false);
        warningPanel.SetActive(false);
        warningIcon.enabled = false;
        staffMenu.gameObject.SetActive(false);
        confirmationPanel.SetActive(false);
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

        
        #region Load / New Game
        // get Player data. If not null, load game
        if (ButtonScript.loadGame == null)
        {
            carpetColour = new Color(0, 0, 255, 100);

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

            nextDay(false);

            for (int i = 0; i < 2; i++)
            {
                int index = UnityEngine.Random.Range(0, 5);
                addStaffMember(new StaffMember(i, "Andrew", staffPrefabs[index], currDay, index));
            }

        }
        else
        {
            PlayerData data = ButtonScript.loadGame;

            carpetColour = new Color(data.carpetColour[0], data.carpetColour[1], data.carpetColour[2]);

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

                addStaffMember(newStaff);
            }

            filmShowings = new List<FilmShowing>(data.filmShowings);
            totalCoins = data.totalCoins;
            currDay = data.currentDay;
            numScreens = theScreens.Count;
            numPopcorn = data.numPopcorn;
            otherObjects = new List<OtherObject>(data.otherObjects);


            nextDay(false);
            

            // hopefully un-fucks things
            for (int i = 0; i < theScreens.Count; i++)
            {
                Vector3 pos = new Vector3(theScreens[i].getX(), theScreens[i].getY(), 0);
                theScreens[i].setPosition((int)pos.x, (int)(pos.y));
            }

            coinLabel.text = totalCoins.ToString();
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
            instance.GetComponent<SpriteRenderer>().sortingOrder = height - theScreens[i].getY();

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
                w = 3; h = 5;
                tag = "Vending Machine";
            }

            // align to grid - +/- 1 to move by one tile horizontally, 0.8 for vertical movement
            pos.x += xCorrection;
            pos.y += yCorrection;

            // change pos and element here
            GameObject instance = (GameObject)Instantiate(newItem.gameObject, pos, Quaternion.identity);
            instance.name = "Element#" + (i + otherObjects.Count);
            instance.GetComponent<SpriteRenderer>().sortingOrder = height - otherObjects[i].yPos;
            instance.tag = tag;

            gameObjectList.Add(instance);

            setTiles(2, (int)(otherObjects[i].xPos), (int)(otherObjects[i].yPos), w, h);
        }

        createColourPicker();

        if (updateTileState != null)
        {
            updateTileState(38, 0, 12, 16, 1, true);
            updateTileState(38, 16, 12, 4, 2, true);
        }
    }

    void createStaff(StaffMember staff)
    {
        Vector3 pos = new Vector3(floorTiles[0, 44 + (staff.getIndex() * 2)].transform.position.x, floorTiles[0, 44 + (staff.getIndex() * 2)].transform.position.y, 0);

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
            imgs[6 + (i * 4)].fillAmount = 0.25f * attributes[i];
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

        imgs[6 + (index * 4)].fillAmount = 0.25f * attributeEffected;
        
    }

    public void ChangeColour(Color c, int x, int y, int width, int height)
    {
        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                floorTiles[i, j].GetComponent<SpriteRenderer>().color = c;
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
                    theScreen.upgrade();
                    screenObjectList[i].GetComponent<SpriteRenderer>().sprite = screenImages[0];

                    newShowTimes();
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

            int r, g, b;

            int columnMultiple = (column * 50);

            if (row == 0)
            {
                r = 255 - columnMultiple;
                g = columnMultiple;
                b = 0;
            }
            else if (row == 1)
            {
                r = 0;
                g = 255 - columnMultiple;
                b = columnMultiple;
            }
            else
            {
                r = columnMultiple;
                g = 0;
                b = 255 - columnMultiple;
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
        btn.onClick.AddListener(() => colourClicked(gO.GetComponent<Image>().color, gO.GetComponent<Image>().sprite));
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

    void colourClicked(Color c, Sprite s)
    {
        carpetColour = c;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                floorTiles[i, j].GetComponent<SpriteRenderer>().color = c;

                if (!s.Equals(marbleBackground))
                {
                    floorTiles[i, j].GetComponent<SpriteRenderer>().sprite = ColourBackground;
                }
                else
                {
                    floorTiles[i, j].GetComponent<SpriteRenderer>().sprite = marbleSquare;

                    if ((i % 2 != j % 2))
                    {
                        floorTiles[i, j].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                    }
                }
            }
        }


        // change step colours
        steps.GetComponent<SpriteRenderer>().color = carpetColour;

    }

    void UpdateJobList()
    {
        ticketStaff = staffMembers.FindAll(
            delegate (StaffMember sm)
            {
                return sm.getJobID() == 1;
            }
            );
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





                //// move to movementScript
                //for (int j = 0; j < allCustomers.Count; j++)
                //{
                //    if (!allCustomers[j].arrived)
                //    {
                //        if (allCustomers[j].hasArrived(hours, minutes))
                //        {

                //            allCustomers[j].nextPoint(true);
                //            float left = allCustomers[j].getTravellingToX();

                //            allCustomers[j].transform.position = new Vector3(left, 0, 0); // y = -11
                            

                //        }
                //    }
                //}

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
    }

    public void startDay()
    {
        // hide the button
        startDayButton.SetActive(false);
        shopButton.gameObject.SetActive(false);

        Customer.tiles = floorTiles;

        if (!simulationRunning)
        {
            // start the running of the 'day'
            simulationRunning = true;
        }

        InstantiateCustomers();

    }

    void InstantiateCustomers()
    {
        for (int i = 0; i < allCustomers.Count; i++)
        {
            int sprite = UnityEngine.Random.Range(0, 3);

            if (sprite == 0)
            {
                // change to repositioning
                allCustomers[i].transform = (Instantiate(greenGuy, new Vector3(0, 0, 0), Quaternion.identity) as Transform);
            }
            else if (sprite == 1)
            {
                // change to repositioning
                allCustomers[i].transform = Instantiate(blueGuy, new Vector3(0, 0, 0), Quaternion.identity) as Transform;
            }
            else if (sprite == 2)
            {
                // change to repositioning
                allCustomers[i].transform = (Instantiate(orangeGuy, new Vector3(0, 0, 0), Quaternion.identity) as Transform);
            }

            allCustomers[i].transform.GetComponent<movementScript>().setCustomer(allCustomers[i]);

        }
    }

    public void nextDay(bool shouldCollect)
    {
        simulationRunning = false;
        allCustomers.Clear();

        queueCount = 0;
        ticketQueue.Clear();
        
        for (int i = 0; i < allCustomers.Count; i++)
        {
            Destroy(allCustomers[i].transform);
        }

        startDayButton.SetActive(true);
        shopButton.gameObject.SetActive(true);

        if (shouldCollect)
        {
            totalCoins += getTodaysMoney();
            // output coins
            coinLabel.text = totalCoins.ToString();
            //lblCoins.Text = totalCoins.ToString();
        }

        currDay++;

        for (int i = 0; i < theScreens.Count; i++)
        {
            int days = theScreens[i].GetDaysOfConstruction();
            if (days == 1)
            {
                screenObjectList[i].GetComponent<SpriteRenderer>().sprite = screenImages[theScreens[i].getUpgradeLevel()];
                newShowTimes();
            }

            theScreens[i].progressOneDay();
        }

        if (currDay % 7 == 1)
        {
            nextWeek();
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

            List<Customer> tmp = filmShowings[i].createCustomerList(currentCount);
            allCustomers.AddRange(tmp);
        }

        //// update day output 
        dayLabel.text = "DAY: " + currDay.ToString();

        count = 0;
        minutes = 0;
        hours = 9;

        timeLabel.text = "09:00";

        statusCode = 0;

        Save();

    }

    void nextWeek()
    {
        newShowTimes();
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

    private int getTicketsSoldValue(ScreenObject screen)
    {
        UnityEngine.Random ran = new UnityEngine.Random();
        int min = screen.getNumSeats() / 2;  // this will be affected by the #posters etc
        int max = screen.getNumSeats();
        int ticketsSold = UnityEngine.Random.Range(min, max);

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

    private void newShowTimes()
    {
        filmShowings.Clear();

        for (int i = 0; i < numScreens; i++)
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

    void QueueController()
    {
        if (ticketQueue.Count > 0)
        {
            queueCount += Time.deltaTime;

            if (queueCount > 2.5f)
            {
                queueCount = 0;

                for (int i = 0; i < ticketStaff.Count; i++)
                {
                    int index = ticketQueue.Peek().getCharIndex();

                    if (queueDone != null)
                    {
                        allCustomers[index].doneWithQueue();
                        allCustomers[index].ticketsDone();
                        queueDone(allCustomers[index]);
                        allCustomers[index].nextPoint(true);
                    }
                    ticketQueue.Dequeue();
                }
            }


            for (int j = 0; j < allCustomers.Count; j++)
            {
                if (allCustomers[j].inQueue)
                {
                    allCustomers[j].DecreasePatience(1);

                    if (allCustomers[j].GetPatience() < 1)
                    {
                        WalkAway(j);
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
        float x = floorTiles[0, 45].transform.position.x;
        float y = floorTiles[0, 45].transform.position.y;
        allCustomers[index].SetTravellingTo(x, y);
        allCustomers[index].inQueue = false;
        allCustomers[index].pointsToVisit.Add(new Coordinate(0, 45));
        // have to do the dequeue thing - but from middle... may need to change structure
    }

    public void objectMoveComplete(bool confirmed)
    {
        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");
        for (int i = 0; i < staff.Length; i++)
        {
            staff[i].GetComponent<SpriteRenderer>().enabled = true;
        }

        confirmMovePanel.SetActive(false);
        moveButtons.SetActive(false);
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

                //try
                //{
                //    for (int i = 0; i < screenObjectList.Count; i++)
                //    {
                //        if (screenObjectList[i].name == "ScreenObject#" + (id + 1))
                //        {
                //            temp = screenObjectList[i].GetComponent<Screen_Script>().theScreen;
                //            screenObjectList.RemoveAt(i);
                //        }
                //    }
                //    Destroy(GameObject.Find("ScreenObject#" + theScreens[id].getScreenNumber()));
                //}
                //catch (Exception) { }

                GameObject theScreen = GameObject.Find("Screen#" + theScreens[id].getScreenNumber());
                temp = theScreen.GetComponent<Screen_Script>().theScreen;

                //GameObject theScreen = (GameObject)Instantiate(screen.gameObject, pos, Quaternion.identity);
                //theScreen.GetComponent<Screen_Script>().theScreen = temp;
                //theScreen.name = "ScreenObject#" + temp.getScreenNumber();
                //theScreen.tag = "ScreenObject";
                theScreen.GetComponent<SpriteRenderer>().sortingOrder = height - y;
                theScreen.transform.position = pos;
                //screenObjectList.Add(theScreen);
                if (temp.ConstructionInProgress())
                {
                    theScreen.GetComponent<SpriteRenderer>().sprite = screenImages[0];
                }

                theScreen.GetComponent<Renderer>().enabled = true;
            }
            else {
                //try
                //{
                //    for (int i = 0; i < gameObjectList.Count; i++)
                //    {
                //        if (gameObjectList[i].name == "Element#" + id)
                //        {
                //            gameObjectList.RemoveAt(i);
                //        }
                //    }
                //    Destroy(GameObject.Find("Element#" + id));

                //}
                //catch (Exception) { }

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
            }

            CheckForPath();

        }
        else if (confirmed)
        {

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
                screenThing.GetComponent<SpriteRenderer>().sortingOrder = height - y;
                screenThing.GetComponent<SpriteRenderer>().sprite = screenImages[0];

                screenThing.tag = "Screen";
                screenObjectList.Add(screenThing);
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
                theObject.GetComponent<SpriteRenderer>().sortingOrder = height - y;

                gameObjectList.Add(theObject);

            }
            itemToAddID = -1;

            setTiles(2, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);

            CheckForPath();
        }
        else {
            ChangeColour(carpetColour, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);
        }

        theTileManager.origX = -1;
        theTileManager.origY = -1;
        theTileManager.toMoveX = -1;
        theTileManager.toMoveY = -1;
        theTileManager.fullWidth = -1;
        theTileManager.fullHeight = -1;

        statusCode = 0;

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

    public void moveScreen()
    {
        // hide staff
        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");

        for (int i = 0; i < staff.Length; i++)
        {
            staff[i].GetComponent<SpriteRenderer>().enabled = false;
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

    public void placeObject(int x, int y, int width, int height)
    {
        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");

        for (int i = 0; i < staff.Length; i++)
        {
            staff[i].GetComponent<SpriteRenderer>().enabled = false;
        }
        statusCode = 2;

        shopCanvas.SetActive(false);
        confirmMovePanel.SetActive(true);
        moveButtons.SetActive(true);

        bool valid = theTileManager.checkValidity(x, y, width, height);

        Color newColour;

        

        if (valid) 
        {
            newColour = Color.green;
        }
        else
        {
            newColour = Color.red;
        }

        for (int i = y; i < y + height; i++)
        {
            for (int j = x; j < x + width; j++)
            {
                floorTiles[i, j].GetComponent<SpriteRenderer>().color = newColour;
            }
        }


        theTileManager.toMoveX = 10;
        theTileManager.toMoveY = 10;
        theTileManager.fullWidth = width;
        theTileManager.fullHeight = height;
        theTileManager.NewItemAdded(10, 10);
    }

    void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveState.gd");

        PlayerData data = new PlayerData(theScreens, carpetColour, staffMembers, filmShowings, totalCoins, currDay, numPopcorn, otherObjects);


        formatter.Serialize(file, data);
        file.Close();
    }

    #region staff events
    public void addStaffMember(StaffMember staff)
    {
        staffMembers.Add(staff);
        createStaff(staff);
    }

    public int getStaffSize()
    {
        return staffMembers.Count;
    }

    public void updateStaffJob(int index, int job)
    {
        staffMembers[index].setJob(job);
        UpdateJobList();
    }

    public int getStaffJobById(int index) { return staffMembers[index].getJobID(); }

    void ShowBuildingOptions(string line1, string line2)
    {
        objectInfo.SetActive(true);
        closeInfo.SetActive(true);
        Text[] labels = objectInfo.gameObject.GetComponentsInChildren<Text>();
        labels[0].text = line1;
        labels[1].text = line2;
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
    public int[] carpetColour;
    public SaveableStaff[] staffMembers;
    public FilmShowing[] filmShowings;
    public int totalCoins;
    public int numPopcorn;
    public int currentDay;
    public OtherObject[] otherObjects;

    public PlayerData(List<ScreenObject> screens, Color col, List<StaffMember> staff, List<FilmShowing> films, int coins, int day, int popcorn, List<OtherObject> others)
    {
        //gameObjectList = sO.ToArray();
        theScreens = screens.ToArray();
        carpetColour = new int[4] { (int)col.r, (int)col.g, (int)col.b, (int)col.a };

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