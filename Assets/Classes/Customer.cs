using Assets.Classes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
public class Customer
{
    #region Position Constants
    const float doorX = -15;
    const float doorY = -4.65f;
    const float centreX = 0;
    const float centreY = 0;
    //const float ticketsX = 17;
    //const float ticketsY = -3.5f;
    #endregion
    
    int index;
    public int currentDirection = 0;     // 1 down, 2 up, 3 left, 4 right, 0 still
    int patience = 800;
    public Transform transform;

    public int GetPatience()
    {
        return patience;
    }

    public void DecreasePatience(int val)
    {
        patience -= val;

        if (patience < 0) { patience = 0; }

    }

    int hourDue;

    public bool inQueue = false;
    int minuteDue;

    public bool arrived = false;

    bool needsFood;
    bool needsTickets;
    bool needsToilet;

    public bool goingToSeats = false;

    Floor theFloor;

    public List<Coordinate> pointsToVisit = new List<Coordinate>();

    public void SetTravellingTo(float x, float y)
    {
        this.travellingToX = x;
        this.travellingToY = y;
    }

    float travellingToX = 0;
    float travellingToY = 0;

    FilmShowing filmShowing;
    public static GameObject[,] tiles;

    public Customer(FilmShowing fs, int ID, Floor f)
    {

        pointsToVisit = new List<Coordinate>();
        theFloor = f;

        filmShowing = fs;

        index = ID;

        needsFood = Random.Range(0, 10) >= 4;
        needsTickets = Random.Range(0, 10) >= 0;      // 3 - changed because it was being a cuntbucketshitebagprickknobadamjohnson
        needsToilet = Random.Range(0, 10) >= 6;

        int minutesEarly = Random.Range(20, 80);

        int hourStart = fs.timeH;
        int minuteStart = fs.timeM;

        hourDue = hourStart - (minutesEarly / 60);
        minuteDue = minuteStart - minutesEarly - ((hourDue-hourStart) * 60);

        if (minuteDue < 0)
        {
            minuteDue += 60;
            hourDue--;
        }
        
    }

    public bool hasArrived(int hours, int minutes)
    {
        if (hours >= hourDue && minutes >= minuteDue)
        {
            arrived = true;
            return true;
        }       

        return false;
    }

    public void nextPoint(bool first)
    {
        Debug.Log("I HAVE BEEN AFFECTED: " + index);

        //travellingToX = 20;
        //travellingToY = -4;

        try
        {
            pointsToVisit.RemoveAt(0);
        }
        catch (Exception) { }

        if (pointsToVisit.Count > 0)
        {
            travellingToX = tiles[pointsToVisit[0].y, pointsToVisit[0].x].transform.position.x;
            travellingToY = tiles[pointsToVisit[0].y, pointsToVisit[0].x].transform.position.y;
        }
        else
        {
            nextPlace(first);
        }
    }

    public void nextPlace(bool first)
    {
        pointsToVisit.Clear();

        if (needsTickets)
        {
            // find a path and set up the point list
            //travellingToX = ticketsX;
            //travellingToY = ticketsY;

            // 11, 40
            // 0, 40
            pointsToVisit = theFloor.FindPath(40, 0, 40, 11);
            
            //call next point();
            if (!first)
            {
                needsTickets = false;
            }

            nextPoint(false);
        }

        // food, toilets go here

        else if (goingToSeats)
        {
            if (patience > 0)
            {
                // get which screen the customer is going to
                int targetScreen = filmShowing.getScreenNumber();
                List<ScreenObject> screenList = Controller.theScreens;

                int x = 0;
                int y = 0;

                for (int i = 0; i < screenList.Count; i++)
                {
                    // find the location of the screen
                    if (screenList[i].getScreenNumber() == targetScreen)
                    {
                        x = screenList[i].getX() + 5;
                        y = screenList[i].getY();
                    }
                }

                Debug.Log("MY SCREEN IS AT: " + y + ", " + x);

                int currX = (int)Math.Round(travellingToX, 0) - 2;
                int currY = (int)Math.Round((travellingToY / 0.8), 0);

                // get a path to it's location            
                pointsToVisit = theFloor.FindPath(currX, currY, x, y);     // (2, 40) will have to change - TODO
                

                //call next point();
                nextPoint(true);
                
            }
            else
            {
                pointsToVisit.Add(new Coordinate(0, 45));
            }
        }
        else if (!inQueue)
        {
            //UnityEngine.Object.Destroy(transform.gameObject);
        }
    }
    
    public void doneWithQueue() { this.inQueue = false; }
    
    public float getTravellingToX() { return travellingToX; }
    public float getTravellingToY() { return travellingToY; }
    public void ticketsDone() { this.needsTickets = false; }
    public bool isGoingToSeat() { return this.goingToSeats; }
    public int getCharIndex() { return this.index; }

    public bool NeedsTickets()
    {
        return this.needsTickets;
    }
    
}
