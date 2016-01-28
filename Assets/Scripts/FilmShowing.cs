using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class FilmShowing
{
    int screeningID;
    int screenNum;
    int ticketsSold;
    public int timeH;
    public int timeM;
    
    List<Customer> customers = new List<Customer>();

    public FilmShowing(int id, int screenNumber, int tickets, int hours, int minutes)
    {
        screeningID = id;
        screenNum = screenNumber;
        ticketsSold = tickets;
        timeH = hours;
        timeM = minutes;

        createCustomerList();
    }

    public void createCustomerList()
    {
        for (int i = 0; i < ticketsSold; i++)
        {
            customers.Add(new Customer(this));
        }
    }

    int getTimeH() { return this.timeH; }
    int getTimeM() { return this.timeM; }

    public List<Customer> getCustomers() { return this.customers; }

    public int getScreenNumber() { return this.screenNum; }
    public int getTicketsSold() { return this.ticketsSold; }
    public void setTicketsSold(int numTickets) { ticketsSold = numTickets; }
}
