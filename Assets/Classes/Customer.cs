using System;
using Random = UnityEngine.Random;


[System.Serializable]
public class Customer
{
    #region Position Constants
    const float doorX = -15;
    const float doorY = -4.65f;
    const float centreX = 0;
    const float centreY = 0;
    const float ticketsX = 17;
    const float ticketsY = -3.5f;
    #endregion
    
    int index;
    public int currentDirection = 0;     // 1 down, 2 up, 3 left, 4 right, 0 still

    int hourDue;

    public bool inQueue = false;
    int minuteDue;

    public bool arrived = false;

    bool needsFood;
    bool needsTickets;
    bool needsToilet;
    bool goingToSeats = false;

    float travellingToX = 0;
    float travellingToY = 0;

    FilmShowing filmShowing;

    public Customer(FilmShowing fs, int ID)
    {
        filmShowing = fs;

        index = ID;

        needsFood = Random.Range(0, 10) >= 4;
        needsTickets = Random.Range(0, 10) >= 3;
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

        this.nextPlace();
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

    public void nextPlace()
    {
        if (needsTickets)
        {
            travellingToX = ticketsX;
            travellingToY = ticketsY;
        }

        // food, toilets go here

        else
        {
            travellingToX = doorX;
            travellingToY = doorY;
            goingToSeats = true;
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
