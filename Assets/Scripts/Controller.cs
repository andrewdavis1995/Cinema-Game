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


    public int itemToAddID = -1;

    GameObject startDayButton;
    public float mouseSensitivity = 1.0f;

    GameObject colourPicker;
    GameObject objectInfo;
    GameObject confirmMovePanel;
    GameObject shopCanvas;
    GameObject steps;
    GameObject closeInfo;
    GameObject moveButtons;

    Sprite[] screenSprites = new Sprite[4];

    public string objectSelected = "";

    List<GameObject> gameObjectList = new List<GameObject>();
    List<GameObject> screenObjectList = new List<GameObject>();

    List<Screen> theScreens = new List<Screen>();
    List<OtherObject> otherObjects = new List<OtherObject>();

    Queue<Customer> ticketQueue = new Queue<Customer>();
    List<Customer> allCustomers = new List<Customer>();

    public int statusCode = 0;     // 0 = free, 1 = dragging staff, 2 = moving object, 3 = in menu, 4 = moving camera, 5 = shop

    public Color carpetColour;

    public Transform greenGuy;
    public Transform blueGuy;
    public Transform screen;
    public Transform plant;
    public Transform bust;
    public Transform vendingMachine;

    public Text timeLabel;
    public Text dayLabel;
    public Text coinLabel;
    public Text popcornLabel;

    Button shopButton;

    Transform myInstance = null;

    public List<StaffMember> staffMembers = new List<StaffMember>();

    private List<FilmShowing> filmShowings = new List<FilmShowing>();

    List<StaffMember> ticketStaff = new List<StaffMember>();
    TileManager theTileManager;
    public GameObject confirmPanel;

    public delegate void doneWithQueue(Customer c);
    public static event doneWithQueue queueDone;

    public delegate void setTileStates(int startX, int startY, int w, int h, bool newState, bool complete);
    public static event setTileStates updateTileState;

    public Sprite ColourBackground;
    public Sprite colourCircle;
    public Sprite marbleBackground;
    public Sprite marbleSquare;

    const int width = 80;
    const int height = 40;

    public GameObject[,] floorTiles;

    bool simulationRunning = false;

    int totalCoins = 0;
    int numPopcorn = 0;

    int currDay = 0;

    int numScreens = 1;

    float count = 0;

    int minutes = 0;
    int hours = 9;

    public void OpenShop()
    {
        if (shopButton.gameObject.active)
        {
            statusCode = 5;
            shopCanvas.SetActive(true);
        }
    }


    // Use this for initialization
    void Start()
    {

        #region Find Objects
        theTileManager = GameObject.Find("TileManagement").GetComponent<TileManager>();
        confirmPanel = GameObject.Find("pnlConfirm");
        shopButton = GameObject.Find("cmdShop").GetComponent<Button>();
        shopCanvas = GameObject.Find("Shop Canvas");
        colourPicker = GameObject.Find("Colour Panel");
        GameObject custStatus = GameObject.Find("Customer Status");
        movementScript.customerStatus = custStatus;
        startDayButton = GameObject.Find("Start Day Button");
        objectInfo = GameObject.Find("Object Info");
        confirmMovePanel = GameObject.Find("MovementPanel");
        moveButtons = GameObject.Find("buttonPanel");
        floorTiles = new GameObject[width, height];
        GameObject[] tmpArray = GameObject.FindGameObjectsWithTag("Floor Tile");
        closeInfo = GameObject.Find("Close Info");
        steps = GameObject.Find("Steps");
        #endregion

        #region Hide Objects on Start
        confirmMovePanel.SetActive(false);
        objectInfo.SetActive(false);
        shopCanvas.SetActive(false);
        colourPicker.SetActive(false);
        moveButtons.SetActive(false);
        closeInfo.SetActive(false);
        custStatus.SetActive(false);
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
                theScreens.Add(new Screen((i + 1), 0));
                theScreens[i].setPosition((int)pos.x, (int)pos.y);
            }
            theScreens[0].upgrade();
            theScreens[0].upgradeComplete();

            nextDay(false);
        }
        else
        {
            PlayerData data = ButtonScript.loadGame;

            theScreens = new List<Screen>(data.theScreens);
            carpetColour = new Color(data.carpetColour[0], data.carpetColour[1], data.carpetColour[2]);
            staffMembers = new List<StaffMember>(data.staffMembers);
            filmShowings = new List<FilmShowing>(data.filmShowings);
            totalCoins = data.totalCoins;
            currDay = data.currentDay;
            numScreens = theScreens.Count;
            numPopcorn = data.numPopcorn;
            otherObjects = new List<OtherObject>(data.otherObjects);


            nextDay(false);

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

            setTiles(true, (int)(theScreens[i].getX()), (int)(theScreens[i].getY()), 11, 15);
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

            setTiles(true, (int)(otherObjects[i].xPos), (int)(otherObjects[i].yPos), w, h);
        }

        createColourPicker();

    }

    public void changeColour(Color c, int x, int y, int width, int height)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                floorTiles[i, j].GetComponent<SpriteRenderer>().color = c;
            }
        }
    }

    public void updateTiles(int x, int y, int w, int h, bool newState)
    {
        if (updateTileState != null)
        {
            updateTileState(x, y, w, h, newState, false);
        }
    }

    void setTiles(bool newState, int x, int y, int width, int height)
    {
        Color newColour;

        if (newState) { newColour = carpetColour; } else { newColour = Color.green; }

        changeColour(newColour, x, y, width, height);

        updateTiles(x, y, width, height, newState);
    }

    public void hideObjectInfo()
    {
        shopCanvas.SetActive(false);
        objectInfo.SetActive(false);
        closeInfo.SetActive(false);
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            colourPicker.SetActive(!colourPicker.active);
            //objectInfo.SetActive(true);
        }

        if (simulationRunning)
        {
            count += Time.deltaTime;

            QueueController();

            if (count > 0.2)
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



                for (int j = 0; j < allCustomers.Count; j++)
                {
                    if (!allCustomers[j].arrived)
                    {
                        if (allCustomers[j].hasArrived(hours, minutes))
                        {
                            float left = 12;

                            if (allCustomers[j].NeedsTickets())
                            {
                                left = 17;
                            }


                            int sprite = UnityEngine.Random.Range(0, 2);

                            if (sprite == 0)
                            {
                                myInstance = Instantiate(greenGuy, new Vector3(left, -15), Quaternion.identity) as Transform;
                            }
                            else
                            {
                                myInstance = Instantiate(blueGuy, new Vector3(left, -15), Quaternion.identity) as Transform;
                            }
                            myInstance.GetComponent<movementScript>().customer = allCustomers[j];

                        }
                    }
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
    }

    public void startDay()
    {
        // hide the button
        startDayButton.SetActive(false);
        shopButton.gameObject.SetActive(false);

        if (!simulationRunning)
        {
            // start the running of the 'day'
            simulationRunning = true;
        }

    }

    public void nextDay(bool shouldCollect)
    {
        simulationRunning = false;
        allCustomers.Clear();

        queueCount = 0;
        ticketQueue.Clear();

        GameObject[] customers = GameObject.FindGameObjectsWithTag("Customer");

        for (int i = 0; i < customers.Length; i++)
        {
            Destroy(customers[i]);
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
            theScreens[i].progressOneDay();
        }

        if (currDay % 7 == 1)
        {
            nextWeek();
        }

        for (int i = 0; i < filmShowings.Count; i++)
        {
            int index = filmShowings[i].getScreenNumber();
            int ticketsSold = getTicketsSoldValue(theScreens[index]);
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
            int screenNum = filmShowings[i].getScreenNumber();
            int upgradeLevel = theScreens[screenNum].getUpgradeLevel();
            int numCustomers = filmShowings[i].getTicketsSold();

            totalIntake += (2 + 1 * upgradeLevel) * numCustomers;
        }

        return totalIntake;
    }

    private int getTicketsSoldValue(Screen screen)
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
            int screeningsThisDay = UnityEngine.Random.Range(2, 4); // number of films per screen per day

            for (int j = 0; j < screeningsThisDay; j++)
            {
                TimeTuple showTime = getShowTime(j);

                FilmShowing newFilm = new FilmShowing(filmShowings.Count, i, 0, showTime.hours, showTime.minutes);
                filmShowings.Add(newFilm);
            }

        }
    }

    float queueCount = 0;

    void QueueController()
    {
        if (ticketQueue.Count > 0)
        {
            queueCount += Time.deltaTime;

            Debug.Log("Queue Count: " + queueCount);
            Debug.Log("Staff: " + ticketStaff.Count);

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
                        allCustomers[index].nextPlace();
                        queueDone(allCustomers[index]);
                    }
                    ticketQueue.Dequeue();
                }
            }


            for (int j = 0; j < allCustomers.Count; j++)
            {
                if (allCustomers[j].inQueue) { allCustomers[j].DecreasePatience(1); }
            }

        }
        else {
            queueCount = 0;
        }
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

                Screen temp = null;

                try
                {
                    for (int i = 0; i < screenObjectList.Count; i++)
                    {
                        if (screenObjectList[i].name == "Screen#" + (id + 1))
                        {
                            temp = screenObjectList[i].GetComponent<Screen_Script>().theScreen;
                            screenObjectList.RemoveAt(i);
                        }
                    }
                    Destroy(GameObject.Find("Screen#" + theScreens[id].getScreenNumber()));
                }
                catch (Exception) { }


                GameObject theScreen = (GameObject)Instantiate(screen.gameObject, pos, Quaternion.identity);
                theScreen.GetComponent<Screen_Script>().theScreen = temp;
                theScreen.name = "Screen#" + temp.getScreenNumber();
                theScreen.tag = "Screen";
                theScreen.GetComponent<SpriteRenderer>().sortingOrder = height - y;
                screenObjectList.Add(theScreen);

            }
            else {
                try
                {
                    for (int i = 0; i < gameObjectList.Count; i++)
                    {
                        if (gameObjectList[i].name == "Element#" + id)
                        {
                            gameObjectList.RemoveAt(i);
                        }
                    }
                    Destroy(GameObject.Find("Element#" + id));

                }
                catch (Exception) { }

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


                GameObject theObject = (GameObject)Instantiate(newItem.gameObject, pos, Quaternion.identity);
                theObject.name = "Element#" + (otherObjects.Count - 1);
                theObject.GetComponent<SpriteRenderer>().sortingOrder = height - y;
                theObject.tag = theTag;
                gameObjectList.Add(theObject);
            }
            itemToAddID = -1;

            setTiles(true, x, y, theTileManager.fullWidth, theTileManager.fullHeight);

            if (!confirmed)
            {
                //setTiles(false, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);
                changeColour(carpetColour, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);
            }
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
                Screen aScreen = new Screen(newID + 1, 0);
                aScreen.setPosition(x, y);
                aScreen.upgrade();
                theScreens.Add(aScreen);

                pos.x += xCorrection;
                pos.y += yCorrection;

                GameObject screenThing = (GameObject)Instantiate(screen.gameObject, pos, Quaternion.identity) as GameObject;
                screenThing.GetComponent<Screen_Script>().theScreen = theScreens[newID];
                screenThing.name = "Screen#" + theScreens[newID].getScreenNumber();
                screenThing.GetComponent<SpriteRenderer>().sortingOrder = height - y;
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

            setTiles(true, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);

        }
        else {
            changeColour(carpetColour, theTileManager.toMoveX, theTileManager.toMoveY, theTileManager.fullWidth, theTileManager.fullHeight);
        }

        theTileManager.origX = -1;
        theTileManager.origY = -1;
        theTileManager.toMoveX = -1;
        theTileManager.toMoveY = -1;
        theTileManager.fullWidth = -1;
        theTileManager.fullHeight = -1;

        statusCode = 0;

    }

    public void moveScreen()
    {
        GameObject[] staff = GameObject.FindGameObjectsWithTag("Staff");

        for (int i = 0; i < staff.Length; i++)
        {
            staff[i].GetComponent<SpriteRenderer>().enabled = false;
        }

        confirmMovePanel.SetActive(true);
        moveButtons.SetActive(true);
        objectInfo.SetActive(false);
        closeInfo.SetActive(false);

        statusCode = 2;
        Debug.Log("PRESSED");

        for (int i = 0; i < screenObjectList.Count; i++)
        {
            if (screenObjectList[i].name.Equals(objectSelected))
            {
                screenObjectList[i].GetComponent<Renderer>().enabled = false;
                int x = theScreens[i].getX(); //-4
                int y = theScreens[i].getY(); //-6


                itemToAddID = 0;

                theTileManager.toMoveX = x;
                theTileManager.toMoveY = y;

                theTileManager.origX = x;
                theTileManager.origY = y;

                theTileManager.fullWidth = 11;
                theTileManager.fullHeight = 15;

                setTiles(false, x, y, 11, 15);

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

                setTiles(false, x, y, w, h);

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

        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
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
    public Screen[] theScreens;
    public int[] carpetColour;
    public StaffMember[] staffMembers;
    public FilmShowing[] filmShowings;
    public int totalCoins;
    public int numPopcorn;
    public int currentDay;
    public OtherObject[] otherObjects;

    public PlayerData(List<Screen> screens, Color col, List<StaffMember> staff, List<FilmShowing> films, int coins, int day, int popcorn, List<OtherObject> others)
    {
        //gameObjectList = sO.ToArray();
        theScreens = screens.ToArray();
        carpetColour = new int[4] { (int)col.r, (int)col.g, (int)col.b, (int)col.a };
        staffMembers = staff.ToArray();
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