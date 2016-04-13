using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{

    public TimeController(Controller c, Text lblTime)
    {
        mainController = c;
        timeLabel = lblTime;
        BeginDay();
    }

    List<Customer> allCustomers = new List<Customer>();
    public Controller mainController;

    public Text timeLabel;

    float count = 0;

    public int minutes = 0;
    public int hours = 9;
    bool simulationRunning = false;

    void Start()
    {
        //mainController = GameObject.Find("Controller").GetComponent<Controller>();
    }

    public void BeginDay()
    {
        if (mainController.simulationRunning)
        {
            allCustomers = mainController.allCustomers;
            simulationRunning = true;
        }
    }

    public void EndDay()
    {
        simulationRunning = false;

        count = 0;
        minutes = 0;
        hours = 9;

        timeLabel.text = "09:00";
    }

    // Update is called once per frame
    void Update()
    {
        if (simulationRunning)
        {
            count += Time.deltaTime;

            //QueueController();

            if (count > 0.15)
            {

                count = 0;
                minutes++;

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

            for (int i = 0; i < allCustomers.Count; i++)
            {
                if (!allCustomers[i].arrived)
                {
                    if (allCustomers[i].HasArrived(hours, minutes) && simulationRunning)        // second simulationRunningCheck needed because of delays at the end
                    {
                        allCustomers[i].pointsToVisit = new List<Coordinate>();

                        allCustomers[i].transform = ObjectPool.current.GetGameObject().transform;

                        allCustomers[i].transform.position = new Vector3(40, -10);

                        allCustomers[i].transform.GetComponent<movementScript>().SetCustomer(allCustomers[i]);

                        allCustomers[i].transform.gameObject.SetActive(true);

                        allCustomers[i].animator = allCustomers[i].transform.GetComponent<Animator>();

                        allCustomers[i].NextPoint(true);

                        float left = allCustomers[i].GetTravellingToX();

                        allCustomers[i].transform.position = new Vector3(left, -15, 0); // y = -11

                    }
                }
                //else
                //{
                //    if (allCustomers[i].transform.gameObject.activeInHierarchy)
                //    {
                //        // check if the projector is broken 
                //        int screenNumber = allCustomers[i].GetFilmScreen() - 1;

                //        if (Controller.theScreens[screenNumber].GetClicksRemaining() > 0)
                //        {
                //            Debug.Log("Customer " + i + " is pissed off");
                //        }

                //    }
                //}
            }
        }
    }
}
