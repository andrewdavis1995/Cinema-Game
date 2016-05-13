using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    List<Customer> allCustomers = new List<Customer>();     // the list of all customers
    public Controller mainController;       // the instance of Controller to use

    public Text timeLabel;      // the label to write the time to

    float count = 0;        // used to control the time
    public int minutes = 0;     // how many minutes have passed (capped at 59)
    public int hours = 9;   // how many hours have past (start a 9 o'clock)

    bool simulationRunning = false;     // whether or not the Business Day simulation is running


    // CONSTRUCTOR for the class
    public TimeController(Controller c, Text lblTime)
    {
        // initialise the variables
        mainController = c;
        timeLabel = lblTime;
        BeginDay();
    }

    /// <summary>
    /// Begin the day - start Time updating
    /// </summary>
    public void BeginDay()
    {
        // validation check to make sure that the mainController is in sync
        if (mainController.simulationRunning)
        {
            // get the list of cusomers
            allCustomers = mainController.customerController.allCustomers;
            // set simulation running to true
            simulationRunning = true;
        }
    }

    /// <summary>
    /// End the day - stop the time from running
    /// </summary>
    void EndDay()
    {
        // reset all variables
        simulationRunning = false;

        count = 0;
        minutes = 0;
        hours = 9;

        timeLabel.text = "09:00";
    }

    // Update is called once per frame
    void Update()
    {
        // if the simulation is running
        if (simulationRunning)
        {
            // add to the count
            count += Time.deltaTime;

            // if the count reaches a certain value, add to the minutes
            if (count > 0.15)
            {
                count = 0;
                minutes++;

                // if minutes reaches 60, set it to 0 and add to the hour count
                if (minutes > 59)
                {
                    hours++;
                    minutes = 0;
                }

                // checks for if hour > 11 - end day
                if (hours > 22)
                {
                    EndDay();
                    mainController.NextDay(true);
                }
            }

            #region Output the time
            string minString = minutes.ToString();
            string hourString = hours.ToString();

            if (minutes < 10)
            {
                minString = "0" + minString;
            }
            if (hours < 10)
            {
                hourString = "0" + hourString;
            }

            timeLabel.text = hourString + ":" + minString;
            #endregion

            #region Check for customers arriving
            // loop through all customers
            for (int i = 0; i < allCustomers.Count; i++)
            {
                // if the customer has not arrived yet...
                if (!allCustomers[i].arrived)
                {
                    // if the customer is now due to arrive...
                    if (allCustomers[i].HasArrived(hours, minutes) && simulationRunning)        // second simulationRunningCheck needed because of delays / synchronisation problems
                    {
                        // initialise
                        allCustomers[i].pointsToVisit = new List<Coordinate>();

                        // get the transform to use and set all the relevant fields
                        allCustomers[i].transform = ObjectPool.current.GetGameObject().transform;
                        allCustomers[i].transform.position = new Vector3(40, -10);
                        allCustomers[i].transform.GetComponent<movementScript>().SetCustomer(allCustomers[i]);
                        allCustomers[i].transform.gameObject.SetActive(true);

                        // set the animator
                        allCustomers[i].animator = allCustomers[i].transform.GetComponent<Animator>();

                        // work out where the customer has to go
                        allCustomers[i].NextPoint(true);

                        // set the the position of the customer
                        float left = allCustomers[i].GetTravellingToX();
                        allCustomers[i].transform.position = new Vector3(left, -15, 0);

                    }
                }
                #endregion
            }
        }
    }
}
