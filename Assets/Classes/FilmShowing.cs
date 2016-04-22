using Assets.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class FilmShowing
{
    int screeningID;    // the id for the screening
    int screenNum;      // the screen number
    int ticketsSold;    // the number of tickets sold for the screening
    public int timeH;   // the hour of the showing
    public int timeM;   // the minute of the showing

    Floor theFloor;     // the floor object - used by customers for pathfinding

    // CONSTRUCTOR for the class
    public FilmShowing(int id, int screenNumber, int tickets, int hours, int minutes, Floor f)
    {
        screeningID = id;
        screenNum = screenNumber;
        ticketsSold = tickets;
        timeH = hours;
        timeM = minutes;
        theFloor = f;
    }

    public List<Customer> CreateCustomerList(int currentCount, Controller c)
    {
        // for loop - from 0 to the number of ticketsSold, create a new customer and add a 
        List<Customer> customers = new List<Customer>();

        for (int i = 0; i < ticketsSold; i++)     // ticketsSold
        {
            // create a new Customer object and add it to the list
            customers.Add(new Customer(this, (currentCount + i), theFloor, c));
        }
        
        // return the list of Customer objects
        return customers;
    }

    // get the time for the showing
    int GetTimeH() { return this.timeH; }
    int GetTimeM() { return this.timeM; }
    
    // Accessors
    public int GetScreenNumber() { return this.screenNum; }
    public int GetTicketsSold() { return this.ticketsSold; }

    // Mutators
    public void SetTicketsSold(int numTickets) { ticketsSold = numTickets; }
}
