using Assets.Classes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
public class Customer
{

    #region Status Check Variables
    public int shouldMoveUp = 0;
    public int moveToServingSlot = -1;
    public int servingSlot = -1;
    public bool walkingAway = false;
    public bool leaving;
    public bool hasLeftTheBuilding = false;
    #endregion

    int index;
    public int currentDirection = 0;     // 1 down, 2 up, 3 left, 4 right, 0 still
    int patience = 1200;
    public Transform transform;
    public Controller mainController;
    public Animator animator;

    public Vector2 MovementVector = new Vector2(2, 0);

    public int GetPatience()
    {
        return patience;
    }

    public void DecreasePatience(int val)
    {
        patience -= val;

        if (patience < 0) { patience = 0; }

    }

    public void MoveUpInQueue()
    {
        transform.Translate(0, 0.8f, 0);
        transform.GetComponent<SpriteRenderer>().sortingOrder++;
    }

    public void MoveToServingSlot()
    {
        transform.position = new Vector3(38.5f + (2.6f * moveToServingSlot), 11 * 0.8f, 0);
        SetTravellingTo(38.5f + (2.6f * moveToServingSlot), 11 * 0.8f);
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

    public Customer(FilmShowing fs, int ID, Floor f, Controller c)
    {

        pointsToVisit = new List<Coordinate>();
        theFloor = f;
        mainController = c;

        filmShowing = fs;

        index = ID;

        needsFood = Random.Range(0, 10) >= 4;
        needsTickets = Random.Range(0, 10) >= 0;      // 3
        needsToilet = Random.Range(0, 10) >= 6;

        int minutesEarly = Random.Range(20, 80);

        try
        {
            int hourStart = fs.timeH;
            int minuteStart = fs.timeM;


            hourDue = hourStart - (minutesEarly / 60);
            minuteDue = minuteStart - minutesEarly - ((hourDue - hourStart) * 60);

            if (minuteDue < 0)
            {
                minuteDue += 60;
                hourDue--;
            }
        }
        catch (Exception) { }
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

        try
        {
            this.pointsToVisit.RemoveAt(0);
        }
        catch (Exception) { }

        if (this.pointsToVisit.Count > 0)
        {
            this.travellingToX = pointsToVisit[0].x;
            this.travellingToY = pointsToVisit[0].y;

            if (goingToSeats || needsTickets)
            {
                travellingToY *= 0.8f;
            }

        }
        else
        {
            if (goingToSeats) { goingToSeats = false; }

            nextPlace(first);
        }
       

        try
        {
            // set up the new movement vector
            int x = 0; int y = 0;

            
            string trigger = "idle";

            if (transform.position.x < (tiles[pointsToVisit[0].y, pointsToVisit[0].x].transform.position.x) - 0.3f)
            {
                x = 1;
                trigger = "right";
            }
            else if (transform.position.x > (tiles[pointsToVisit[0].y, pointsToVisit[0].x].transform.position.x) + 0.3f)
            {
                x = -1;
                trigger = "left";
            }
            else if (transform.position.y < (tiles[pointsToVisit[0].y, pointsToVisit[0].x].transform.position.y) - 0.3f)
            {
                y = 1;
                trigger = "up";
            }
            else if (transform.position.y > (tiles[pointsToVisit[0].y, pointsToVisit[0].x].transform.position.y) + 0.3f)
            {
                y = -1;
                trigger = "down";
            }
            // ---------------------------------------------------------------------------------------------------------


            // generate a new movement vecctor
            if (x != MovementVector.x || y != MovementVector.y)
            {
                // do trigger
                animator.SetTrigger(trigger);
            }

            MovementVector = new Vector2(x, y);
        }
        catch (Exception) { }

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
            else {
                nextPoint(false);
            }
        }

        // food, toilets go here

        else if (goingToSeats)
        {
            if (patience > 0)
            {
                int targetScreen = filmShowing.getScreenNumber();




                // the path gets set here
                List<Coordinate> copy = mainController.GetScreenPath(targetScreen - 1);

                for (int i = 0; i < copy.Count; i++)
                {
                    pointsToVisit.Add(copy[i]);
                }


            }
            else
            {
                pointsToVisit.Add(new Coordinate(0, 42));
            }
        }
        else if (!inQueue && patience > 0)
        {
            // arrived at screen - FINISHED
            transform.gameObject.SetActive(false);

            // update the speed portion of the Reputation
            mainController.reputation.UpdateSpeedRating(GetPatience());
        }
        else if (patience < 1)
        {
            hasLeftTheBuilding = true;
        }
    }
    
    public void doneWithQueue() {
        this.inQueue = false;
        SetTravellingTo(38.5f, 11 * 0.8f);
        transform.GetComponent<SpriteRenderer>().sortingLayerName = "Front";
        transform.GetComponent<SpriteRenderer>().sortingOrder = 11;
    }
    
    public float getTravellingToX() { return travellingToX; }
    public float getTravellingToY() { return travellingToY; }
    public void ticketsDone() { this.needsTickets = false; goingToSeats = true; }
    public bool isGoingToSeat() { return this.goingToSeats; }
    public int getCharIndex() { return this.index; }

    public bool NeedsTickets()
    {
        return this.needsTickets;
    }

    // move to movementScript
    public void WalkOut()
    {
        int x = (int) transform.position.x;
        int y = (int) (transform.position.y);
        
        inQueue = false;
        goingToSeats = false;

        transform.GetComponent<SpriteRenderer>().sortingLayerName = "Front";

        if (y > 0)
        {
            pointsToVisit = TileManager.floor.FindPath(x, y, 42, y-1);
        }
        else
        {
            pointsToVisit = new List<Coordinate>();
        }

        mainController.numWalkouts++;
        mainController.allCustomers.RemoveAt(index);
        
    }

}
