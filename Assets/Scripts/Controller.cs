using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;
using Assets.Classes;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class Controller : MonoBehaviour {

    GameObject startDayButton;
    public float mouseSensitivity = 1.0f;
    
    GameObject colourPicker;
    GameObject objectInfo;
    GameObject confirmMovePanel;
    GameObject shopCanvas;

    public string objectSelected = "";

    List<Transform> screenObjects = new List<Transform>();

    List<Screen> theScreens = new List<Screen>();

    Queue<Customer> ticketQueue = new Queue<Customer>();
    List<Customer> allCustomers = new List<Customer>();

    public int statusCode = 0;     // 0 = free, 1 = dragging staff, 2 = moving object, 3 = in menu, 4 = moving camera, 5 = shop

    public Color carpetColour;

    public Transform greenGuy;
    public Transform blueGuy;
    public Transform screen;

    public Text timeLabel;
    public Text dayLabel;
    public Text coinLabel;
    public Text popcornLabel;

    Transform myInstance = null;

    public List<StaffMember> staffMembers = new List<StaffMember>();
    
    private List<FilmShowing> filmShowings = new List<FilmShowing>();

    List<StaffMember> ticketStaff = new List<StaffMember>();
    TileManager theTileManager;
    public GameObject confirmPanel;

    public delegate void doneWithQueue(Customer c);
    public static event doneWithQueue queueDone;

    public delegate void setTileStates(int startX, int startY, bool newState, bool complete);
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

    int currDay = 0;

    int numScreens = 1;

    float count = 0;

    int minutes = 0;
    int hours = 9;

    // Use this for initialization
    void Start() {

        theTileManager = GameObject.Find("TileManagement").GetComponent<TileManager>();
        confirmPanel = GameObject.Find("pnlConfirm");
        shopCanvas = GameObject.Find("Shop Canvas");
        //shopCanvas.SetActive(false);
        

        for (int i = 0; i < 3; i++)
        {
            theScreens.Add(new Screen((i+1), 0));
        }
        theScreens[0].upgrade();
        theScreens[0].upgradeComplete();


        mouseDrag.addStaff += addStaffMember;
        mouseDrag.getStaffListSize += getStaffSize;
        mouseDrag.getStaffJobById += getStaffJobById;
        mouseDrag.changeStaffJob += updateStaffJob;
        mouseDrag.getStaffList += getFullStaffList;
        movementScript.addToQueueTickets += addToQueueTickets;
        movementScript.getQueueTickets += getTicketQueue;
        movementScript.getQueueTicketsSize += getTicketQueueSize;
        Screen_Script.showBuildingMenu += ShowBuildingOptions;

        Image[] imgs = GameObject.Find("Customer Status").GetComponentsInChildren<Image>();

        for (int i = 0; i < imgs.Length; i++)
        {
            imgs[i].enabled = false;
        }

        colourPicker = GameObject.Find("Colour Panel");

        //colourPicker.GetComponent<Renderer>().enabled = false;
        colourPicker.SetActive(false);

        //Text[] txt = colourPicker.GetComponentsInChildren<Text>();

        //for (int i = 0; i < txt.Length; i++)
        //{
        //    txt[i].enabled = false;
        //}

        #region find commands
        startDayButton = GameObject.Find("Start Day Button");
        nextDay(false);

        floorTiles = new GameObject[width, height];
        GameObject[] tmpArray = GameObject.FindGameObjectsWithTag("Floor Tile");
        for (int i = 0; i < tmpArray.Length; i++)
        {
            string name = tmpArray[i].name;

            string[] tmp = name.Split('~');
            int x = int.Parse(tmp[1]);
            int y = int.Parse(tmp[2]);

            carpetColour = new Color(0, 0, 255, 100);

            tmpArray[i].GetComponent<SpriteRenderer>().color = carpetColour;
            
            tmpArray[i].GetComponent<SpriteRenderer>().sprite = ColourBackground;
            floorTiles[x, y] = tmpArray[i];
        }


        #endregion

        createColourPicker();

        // create some test screens
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = floorTiles[i * 11, 0].transform.position;

            // align to grid - +/- 1 to move by one tile horizontally, 0.8 for vertical movement
            pos.x += 4.5f;
            pos.y += 6f;

            Transform instance = Instantiate(screen, pos, Quaternion.identity) as Transform;
            instance.GetComponent<Screen_Script>().theScreen = theScreens[i];
            instance.name = "screen#" + theScreens[i].getScreenNumber();
            theScreens[i].setPosition((int)pos.x - 4, (int)pos.y - 6);
            screenObjects.Add(instance);

            setTiles(true, i * 11, 0);
        }

        objectInfo = GameObject.Find("Object Info");
        objectInfo.SetActive(false);

        confirmMovePanel = GameObject.Find("MovementPanel");
        confirmMovePanel.SetActive(false);

    }

    public void updateTiles(int x, int y, bool newState)
    {
        if (updateTileState != null)
        {
            updateTileState(x, y, newState, false);
        }
    }

    void setTiles(bool newState, int x, int y)
    {
        Color newColour;

        if (newState) { newColour = carpetColour; } else { newColour = Color.green; }
        
        for (int i = x; i < x + 10; i++)
        {
            for (int j = y; j < y + 15; j++)
            {
                floorTiles[i, j].GetComponent<SpriteRenderer>().color = newColour;
            }
        }

        updateTiles(x, y, newState);
    }

    public void hideObjectInfo()
    {
        objectInfo.SetActive(false);
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

    void colourClicked(Color c, Sprite s) {

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

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            colourPicker.SetActive(true);
            objectInfo.SetActive(true);
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
                    simulationRunning = false;
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

        if (!simulationRunning)
        {
            // start the running of the 'day'
            simulationRunning = true;
        }
        
    }

    void nextDay(bool shouldCollect)
    {
        startDayButton.SetActive(true);

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
        }
        else {
            queueCount = 0;
        }
    }

    public void objectMoveComplete(bool confirmed)
    {
        confirmMovePanel.SetActive(false);
        objectInfo.SetActive(true);

        // re-place image
        
       


        int x = -1; int y = -1;
        bool newScreen = !(theTileManager.origX > -1) ;


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

        if (!newScreen)
        {
            string[] tmp = objectSelected.Split('#');
            int id = int.Parse(tmp[1]) - 1;

            Vector3 pos = new Vector3(x, y * 0.8f, 0);

            theScreens[id].setPosition(x, y);

            pos.x = pos.x += 4.5f;
            pos.y += 6f;

            //Destroy(screenObjects[id]);
            try
            {
                Destroy(GameObject.Find("screen#" + theScreens[id].getScreenNumber()));
            }
            catch (Exception) { }

            screenObjects.Add(Instantiate(screen, pos, Quaternion.identity) as Transform);
            screenObjects[id].GetComponent<Screen_Script>().theScreen = theScreens[id];
            screenObjects[id].name = "screen#" + theScreens[id].getScreenNumber();
            screenObjects.Add(screenObjects[id]);

        }
        else if (confirmed)
        {

            int newID = theScreens.Count;

            Screen aScreen = new Screen(newID + 1, 0);
            aScreen.upgrade();
            theScreens.Add(aScreen);

            Vector3 pos = new Vector3(x, y * 0.8f, 0);

            theScreens[newID].setPosition(x, y);

            pos.x = pos.x += 4.5f;
            pos.y += 6f;

            screenObjects.Add(Instantiate(screen, pos, Quaternion.identity) as Transform);
            screenObjects[newID].GetComponent<Screen_Script>().theScreen = theScreens[newID];
            screenObjects[newID].name = "screen#" + theScreens[newID].getScreenNumber();
            screenObjects.Add(screenObjects[newID]);
        }

        setTiles(true, theTileManager.toMoveX, theTileManager.toMoveY);

        theTileManager.origX = -1;
        theTileManager.origY = -1;
        theTileManager.toMoveX = -1;
        theTileManager.toMoveY = -1;

    }

    public void moveObject()
    {
        confirmMovePanel.SetActive(true);
        objectInfo.SetActive(false);

        statusCode = 2;
        Debug.Log("PRESSED");
        for (int i = 0; i < screenObjects.Count; i++)
        {
            if (screenObjects[i].name.Equals(objectSelected))
            {
                screenObjects[i].GetComponent<Renderer>().enabled = false;
                int x = theScreens[i].getX(); //-4
                int y = theScreens[i].getY(); //-6

                theTileManager.toMoveX = x;
                theTileManager.toMoveY = y;

                theTileManager.origX = x;
                theTileManager.origY = y;

                setTiles(false, x, y);
                      
                break;
            }
        }

    }

    public void placeObject(int x, int y)
    {
        shopCanvas.SetActive(false);
        confirmMovePanel.SetActive(true);

        bool valid = theTileManager.checkValidity(x, y, 10, 15);

        Color newColour;

        if (valid)
        {
            newColour = Color.green;
        }
        else
        {
            newColour = Color.red;
        }

        for (int i = x; i < x + 10; i++)
        {
            for (int j = y; j < y + 15; j++)
            {
                floorTiles[i, j].GetComponent<SpriteRenderer>().color = newColour;
            }
        }

        statusCode = 2;

        theTileManager.toMoveX = 10;
        theTileManager.toMoveY = 10;
    }


    void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveState.gd");

        PlayerData data = new PlayerData(theScreens, carpetColour, staffMembers, filmShowings, totalCoins, currDay, numScreens);
        

        formatter.Serialize(file, data);
        file.Close();
    }

    void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/saveState.gd"))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/saveState.gd", FileMode.Open);
            file.Position = 0;

            if (file == null)
            {
                Debug.Log("FUCK YOUR SHIT");
                return;
            }

            PlayerData pd = (PlayerData)formatter.Deserialize(file);
            //SaveLoadScript.savedGames = (List<Controller>)formatter.Deserialize(file);
            file.Close();
        }
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
    
    void ShowBuildingOptions(int screenNum, int upgradeLevel)
    {
        objectInfo.SetActive(true);
        Text[] labels = objectInfo.gameObject.GetComponentsInChildren<Text>();
        labels[0].text = "Screen " + screenNum;
        labels[1].text = "Level " + upgradeLevel;
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
class PlayerData
{
    //Transform[] screenObjects;
    Screen[] theScreens;
    int[] carpetColour;
    StaffMember[] staffMembers;
    FilmShowing[] filmShowings;
    int totalCoins;
    int currentDay;
    int numScreens;

    public PlayerData(List<Screen> screens, Color col, List<StaffMember> staff, List<FilmShowing> films, int coins, int day, int noOfScreens)
    {
        //screenObjects = sO.ToArray();
        theScreens = screens.ToArray();
        carpetColour = new int[4] { (int)col.r, (int)col.g, (int)col.b, (int)col.a};
        staffMembers = staff.ToArray();
        filmShowings = films.ToArray();
        totalCoins = coins;
        currentDay = day;
        numScreens = noOfScreens;
    }

}