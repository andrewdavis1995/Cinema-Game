using Boo.Lang;
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
    int patience = 1000;

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

    public void nextPoint()
    {
        // if (pointsToVisit.Count > 0){ travellingToX = pointsToVisit[0].x; travellingToY = pointsToVisit[0].y; }else { nextPlace(); }
    }

    public void nextPlace()
    {
        if (needsTickets)
        {
            // find a path and set up the point list
            travellingToX = ticketsX;
            travellingToY = ticketsY;

            //call next point();
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



    void FindPath(int startX, int startY, int goalX, int goalY)
    {

        // OutputStates();

        Node head = new Node(new Coordinate(startX, startY));

        bool found = false;

        List<Node> openNodes = new List<Node>();
        List<Node> exploredNodes = new List<Node>();

        openNodes.Add(head);

        while (!found && openNodes.Count > 0)
        {
            List<Node> tmp = new List<Node>();
            while (openNodes.Count > 0)
            {

                if (openNodes[0].matchesGoal(new Coordinate(goalX, goalY)))
                {
                    found = true;
                    head = openNodes[0];
                    break;
                }
                else
                {
                    List<Node> tmpList = (openNodes[0].FindChildren(exploredNodes, tmp, states));

                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        // add path
                        List<Node> localPath = new List<Node>();

                        for (int j = 0; j < openNodes[0].GetPath().Count; j++)
                        {
                            localPath.Add(openNodes[0].GetPath()[j]);
                        }
                        Node localNode = openNodes[0];

                        if (!localPath.Contains(openNodes[0]))
                        {
                            localPath.Add(localNode);
                        }

                        //openNodes[0].path = new List<Node>();
                        tmpList[i].path = localPath;
                    }

                    exploredNodes.Add(openNodes[0]);
                    openNodes.RemoveAt(0);

                    tmp.AddRange(tmpList);

                }
            }

            openNodes = tmp;
        }

        if (found)
        {
            head.path.Add(new Node(new Coordinate(goalX, goalY)));

            // Customer.path = head.path;

        }
        else
        {
            Console.WriteLine("NO PATH AVAILABLE");
        }

    }

}
