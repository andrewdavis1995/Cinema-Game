using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;

public class Controller : MonoBehaviour {

    Image[] imgs;
    GameObject startDayButton;

    Queue<Customer> ticketQueue = new Queue<Customer>();

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


        imgs = GameObject.Find("Customer Status").GetComponentsInChildren<Image>();

        for (int i = 0; i < imgs.Length; i++)
        {
            imgs[i].enabled = false;
        }

        startDayButton = GameObject.Find("Start Day Button");
        nextDay(false);

    }
    

    // Update is called once per frame
    void Update()
    {
        if (simulationRunning)
        {
            count += Time.deltaTime;

            
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


                for (int i = 0; i < filmShowings.Count; i++)
                {
                    List<Customer> customerArray = filmShowings[i].getCustomers();

                    for (int j = 0; j < customerArray.Count; j++)
                    {
                        if (!customerArray[j].arrived)
                        {
                            if (customerArray[j].hasArrived(hours, minutes))
                            {
                                float left = 12;

                                if (customerArray[j].NeedsTickets())
                                {
                                    left = 16;
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
                                myInstance.GetComponent<movementScript>().customer = customerArray[j];                                
                                                                
                            }
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
            filmShowings[i].createCustomerList();
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
