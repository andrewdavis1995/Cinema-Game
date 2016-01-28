using System;
using Random = UnityEngine.Random;

public class Customer
{
    #region Position Constants
    const int doorX = -15;
    const int doorY = -10;
    const int ticketsX = 15;
    const int ticketsY = 2;
    #endregion
    
    int characterNum;
    public int currentDirection = 0;     // 1 down, 2 up, 3 left, 4 right, 0 still

    int hourDue;
    int minuteDue;

    public bool arrived = false;

    bool needsFood;
    bool needsTickets;
    bool needsToilet;
    bool goingToSeats = false;

    int travellingToX = 0;
    int travellingToY = 0;

    FilmShowing filmShowing;

    public Customer(FilmShowing fs)
    {
        filmShowing = fs;

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

    
    public int getTravellingToX() { return travellingToX; }
    public int getTravellingToY() { return travellingToY; }
    public void ticketsDone() { this.needsTickets = false; }
    public bool isGoingToSeat() { return this.goingToSeats; }
}
