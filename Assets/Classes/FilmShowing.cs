using Assets.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class FilmShowing
{
    int screeningID;
    int screenNum;
    int ticketsSold;
    public int timeH;
    public int timeM;

    Floor theFloor;
    //Controller mainController;

    public FilmShowing(int id, int screenNumber, int tickets, int hours, int minutes, Floor f)
    {
        screeningID = id;
        screenNum = screenNumber;
        ticketsSold = tickets;
        timeH = hours;
        timeM = minutes;
        theFloor = f;
        //mainController = c;
    }

    public List<Customer> createCustomerList(int currentCount, Controller c)
    {
        List<Customer> customers = new List<Customer>();

        for (int i = 0; i < ticketsSold; i++)     // ticketsSold
        {
            customers.Add(new Customer(this, (currentCount + i), theFloor, c));
        }
        

        return customers;
    }

    int getTimeH() { return this.timeH; }
    int getTimeM() { return this.timeM; }
    

    public int getScreenNumber() { return this.screenNum; }
    public int getTicketsSold() { return this.ticketsSold; }
    public void setTicketsSold(int numTickets) { ticketsSold = numTickets; }
}
