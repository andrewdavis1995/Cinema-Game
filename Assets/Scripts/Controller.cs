using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;

public class Controller : MonoBehaviour {

    GameObject startDayButton;
    public float mouseSensitivity = 1.0f;

    GameObject colourPicker;

    Queue<Customer> ticketQueue = new Queue<Customer>();
    List<Customer> allCustomers = new List<Customer>();

    public Transform greenGuy;
    public Transform blueGuy;

    public Text timeLabel;
    public Text dayLabel;
    public Text coinLabel;
    public Text popcornLabel;

    Transform myInstance = null;

    public List<StaffMember> staffMembers = new List<StaffMember>();

    private Screen[] theScreens = new Screen[5];
    private List<FilmShowing> filmShowings = new List<FilmShowing>();

    List<StaffMember> ticketStaff = new List<StaffMember>();

    public delegate void doneWithQueue(Customer c);
    public static event doneWithQueue queueDone;

    public Sprite ColourBackground;
    public Sprite colourCircle;
    public Sprite marbleBackground;
    public Sprite marbleSquare;

    GameObject[] floorTiles;

    bool simulationRunning = false;

    int totalCoins = 0;

    int currDay = 0;

    int numScreens = 1;

    float count = 0;

    int minutes = 0;
    int hours = 9;

    // Use this for initialization
    void Start() {

        //inputs[0].gameObject.SetActive(false);

        for (int i = 0; i < 5; i++)
        {
            theScreens[i] = new Screen(i, 0);
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
        

        Image[] imgs = GameObject.Find("Customer Status").GetComponentsInChildren<Image>();

        for (int i = 0; i < imgs.Length; i++)
        {
            imgs[i].enabled = false;
        }

        colourPicker = GameObject.Find("Colour Panel");

        colourPicker.SetActive(false);

        //Text[] txt = colourPicker.GetComponentsInChildren<Text>();

        //for (int i = 0; i < txt.Length; i++)
        //{
        //    txt[i].enabled = false;
        //}

        #region find commands
        startDayButton = GameObject.Find("Start Day Button");
        nextDay(false);

        floorTiles = GameObject.FindGameObjectsWithTag("Floor Tile");

        #endregion

        createColourPicker();

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
        for (int i = 0; i < floorTiles.Length; i++)
        {
            string name = floorTiles[i].name;

            string[] tmp = name.Split('~');
            int x = int.Parse(tmp[1]);
            int y = int.Parse(tmp[2]);
            
            floorTiles[i].GetComponent<SpriteRenderer>().color = c;
            if (!s.Equals(marbleBackground))
            {
                floorTiles[i].GetComponent<SpriteRenderer>().sprite = ColourBackground;
            }
            else
            {
                floorTiles[i].GetComponent<SpriteRenderer>().sprite = marbleSquare;

                if ((x % 2 != y % 2))
                {
                    floorTiles[i].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            colourPicker.SetActive(true);
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

        for (int i = 0; i < theScreens.Length; i++)
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
