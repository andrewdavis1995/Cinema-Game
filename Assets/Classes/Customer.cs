﻿using Assets.Classes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
public class Customer
{

    #region Status Check Variables
    public int shouldMoveUp = 0;
    public bool sortFoodQueuePos = false;
    public int moveToServingSlot = -1;
    public float servingPositionX = 0;
    public float servingPositionY = 0;
    public int servingSlot = -1;
    public int queueDoneWith = -1;
    public bool walkingAway = false;
    public bool leaving;
    public bool hasLeftTheBuilding = false;
    public bool isBored = false;
    #endregion

    int index;
    public int currentDirection = 0;     // 1 down, 2 up, 3 left, 4 right, 0 still
    int patience = 1200;

    public Transform transform;
    public Controller mainController;
    public Animator animator;
    
    int hourDue;

    public bool inQueue = false;
    int minuteDue;

    public bool arrived = false;

    bool needsFood;
    int foodDesired = -1;

    bool needsTickets;
    bool needsToilet;

    public bool goingToFood = false;
    public bool goingToSeats = false;

    Floor theFloor;

    public List<Coordinate> pointsToVisit = new List<Coordinate>();

    float travellingToX = 0;
    float travellingToY = 0;

    FilmShowing filmShowing;

    public static GameObject[,] tiles;
    
    public void SetTravellingTo(float x, float y)
    {
        this.travellingToX = x;
        this.travellingToY = y;
    }

    public Customer(FilmShowing fs, int ID, Floor f, Controller c)
    {

        pointsToVisit = new List<Coordinate>();
        theFloor = f;
        mainController = c;

        filmShowing = fs;

        index = ID;

        needsFood = Random.Range(0, 10) >= 5;         // 5
        needsTickets = Random.Range(0, 10) >= 0;      // 3
        needsToilet = Random.Range(0, 10) >= 6;

        if (needsFood)
        {

            if (Controller.foodArea != null)
            {

                int rand = Random.Range(0, 15);
                if (rand < 8) { foodDesired = 0; }
                else if (rand < 13) { foodDesired = 1; }
                else { foodDesired = 2; }
            }
            else
            {
                needsFood = false;
                foodDesired = -1;
                patience -= 150;
            }
        }

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

    public bool HasArrived(int hours, int minutes)
    {
        if (hours >= hourDue && minutes >= minuteDue)
        {
            arrived = true;
            return true;
        }       

        return false;
    }

    public Vector2 MovementVector = new Vector2(2, 0);

    public int GetPatience()
    {
        return patience;
    }

    public void DecreasePatience(int val)
    {
        patience -= val;

        if (patience < 0) { patience = 0; }
        else if (patience < 152 && patience > 145)
        {
            isBored = true;
        }

    }

    public void MoveUpInQueue()
    {
        transform.Translate(0, 0.8f, 0);
        transform.GetComponent<SpriteRenderer>().sortingOrder++;
    }

    public void MoveToServingSlot()
    {
        transform.position = new Vector3(servingPositionX + (2.6f * moveToServingSlot), (servingPositionY), -1);
        SetTravellingTo(servingPositionX + (2.6f * moveToServingSlot), servingPositionY);
    }

    public void NextPoint(bool first)
    {
        //if (!first)
        //{
        //    transform.Translate(-2 * MovementVector);
        //}

        try
        {
            this.pointsToVisit.RemoveAt(0);
        }
        catch (Exception) { }

        if (this.pointsToVisit.Count > 0)
        {
            this.travellingToX = pointsToVisit[0].x;
            this.travellingToY = pointsToVisit[0].y;

            if (goingToSeats || needsTickets || goingToFood)
            {
                travellingToY *= 0.8f;
            }
            
        }
        else
        {
            if (goingToSeats) { goingToSeats = false; }

            NextPlace(first);
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

    public void NextPlace(bool first)
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
                NextPoint(false);
            }
        }

        // food, toilets go here

        else if (goingToFood)
        {
            if (patience > 0)
            {
                // the path gets set here
                List<Coordinate> copy = mainController.GetPathToFood();

                for (int i = 0; i < copy.Count; i++)
                {
                    pointsToVisit.Add(copy[i]);
                }

                int xPos = (int)(Math.Round(transform.position.x, 0));

                SetTravellingTo(xPos, 11);

                transform.GetComponent<SpriteRenderer>().sortingOrder = 40;

            }
            else
            {
                pointsToVisit.Add(new Coordinate(0, 42));
            }
            
        }
        else if (goingToSeats)
        {
            if (patience > 0)
            {
                int targetScreen = filmShowing.GetScreenNumber();

                List<Coordinate> copy;

                // the path gets set here
                if (foodDesired == -1)
                {
                    copy = mainController.GetScreenPath(targetScreen - 1);
                }
                else
                {
                    copy = mainController.GetFoodToScreenPath(targetScreen - 1);
                }

                for (int i = 0; i < copy.Count; i++)
                {
                    pointsToVisit.Add(copy[i]);
                }

                //float xPos = transform.position.x;
                //float yPos = (transform.position.y) + 1;

                //SetTravellingTo(xPos, 8000);

            }
            else
            {
                pointsToVisit.Add(new Coordinate(0, 42));
            }
        }
        else if (!inQueue && patience > 0)
        {
            // arrived at screen - FINISHED

            int price = (int)(1.5 + (1.5 * Controller.theScreens[0].GetUpgradeLevel()));
            mainController.customerMoney += price;
            mainController.customersServed++;

            transform.gameObject.SetActive(false);

            // update the speed portion of the Reputation
            mainController.reputation.UpdateSpeedRating(GetPatience());
        }
        else if (patience < 1)
        {
            hasLeftTheBuilding = true;
        }
        
    }

    public void AddPatience(int val)
    {
        patience += val;
    }

    public int GetFilmScreen() { return filmShowing.GetScreenNumber(); }
    public float GetTravellingToX() { return travellingToX; }
    public float GetTravellingToY() { return travellingToY; }
    public void TicketsDone() {
        this.needsTickets = false;

        if (needsFood && mainController.foodQueue.GetQueueSize() > 10)
        {
            needsFood = false;
            foodDesired = -1;
        }

        if (!needsFood)
        {
            goingToSeats = true;
        }
        else
        {
            goingToFood = true;
            needsFood = false;
        }
    }
    public bool IsGoingToSeat() { return this.goingToSeats; }
    public int GetCharIndex() { return this.index; }

    public bool NeedsTickets()
    {
        return this.needsTickets;
    }

    // move to movementScript
    public void WalkOut()
    {
        int x = (int)transform.position.x;
        int y = (int)(transform.position.y);
        
        inQueue = false;
        goingToSeats = false;

        transform.GetComponent<SpriteRenderer>().sortingLayerName = "Front";
        transform.GetComponent<SpriteRenderer>().sortingOrder = 70;

        if (y > 1)
        {
            pointsToVisit = TileManager.floor.FindPath(x, y, 42, y - 1);
        }
        else
        {
            transform.Translate(2.5f, 0, 0);
            pointsToVisit = new List<Coordinate>();
        }

        mainController.numWalkouts++;
        //mainController.allCustomers.RemoveAt(ind);
       
    }

}
