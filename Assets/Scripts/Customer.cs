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

    int index;
    int characterNum;
    public int currentDirection = 0;     // 1 down, 2 up, 3 left, 4 right, 0 still


    float posX = 0;
    float posY = 0;
    

    bool needsFood;
    bool needsTickets;
    bool needsToilet;
    bool goingToSeats = false;

    int travellingToX = 0;
    int travellingToY = 0;

    public Customer(int id)
    {

        needsFood = Random.Range(0, 10) >= 4;
        needsTickets = Random.Range(0, 10) >= 3;
        needsToilet = Random.Range(0, 10) >= 6;
                
        this.nextPlace();
    }

    public void updatePosition(float x, float y) { posX = x; posY = y; }


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


    public int getIndex() { return index; }
    public float getXCoordinate() { return posX; }
    public float getYCoordinate() { return posY; }
    public int getTravellingToX() { return travellingToX; }
    public int getTravellingToY() { return travellingToY; }
    public void ticketsDone() { this.needsTickets = false; }
    public bool isGoingToSeat() { return this.goingToSeats; }
}
