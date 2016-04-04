﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Timers;
using System.Collections.Generic;
using System.Threading;

public class movementScript : MonoBehaviour {

    public delegate void addToTicketQueue(Customer customer);
    public static event addToTicketQueue addToQueueTickets;

    public delegate Queue<Customer> getTicketQueue();
    public static event getTicketQueue getQueueTickets;

    public delegate int getTicketQueueSize();
    public static event getTicketQueueSize getQueueTicketsSize;

    Controller mainController;

    public static GameObject customerStatus;

    public Transform greenGuy;

    public float moveSpeed;

    private Animator animator;

    Customer customer;
    
    public Animation anim;

    Image[] imgs;

    int timeInQueue;


    public void setCustomer(Customer cust)
    {
        this.customer = cust;
    }

    // Use this for initialization
    void Start ()
    {
        //int sprite = UnityEngine.Random.Range(0, 3);

        //Controller.queueDone += sortQueuePosition;

        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
         
        animator = GetComponent<Animator>();

		imgs = customerStatus.GetComponentsInChildren<Image>();
        
    }

    private void moveCustomer()
    {

        if (customer != null && customer.pointsToVisit != null)
        {
            if (!customer.inQueue && customer.pointsToVisit.Count > 0)
            {
                if (customer.leaving)
                {
                    int xPos = (int)transform.position.x;
                    int yPos = (int)transform.position.y;
                    
                    customer.SetTravellingTo(xPos, yPos);
                    transform.position = new Vector3(xPos, yPos, 0);
                    customer.leaving = false;
                }

                float theX = transform.position.x;
                float theY = transform.position.y;


                if (customer.goingToSeats)
                {
                    customer.transform.GetComponent<SpriteRenderer>().sortingOrder =  TileManager.floor.height - (int)(theY) - 1;
                    Debug.Log(customer.getTravellingToX() + ", " + customer.getTravellingToY());
                }

                bool checkY1 = theY <= customer.getTravellingToY() + 0.3f;
                bool checkY2 = theY >= customer.getTravellingToY() - 0.3f;
                bool checkX1 = theX <= customer.getTravellingToX() + 0.3f;
                bool checkX2 = theX >= customer.getTravellingToX() - 0.3f;

                if (checkX1 && checkX2 && checkY1 && checkY2)
                {
                    if (customer.NeedsTickets() && customer.pointsToVisit.Count < 2)
                    {
                        //customer.nextPoint(false);
                        int queueLength = getQueueTicketsSize();

                        // this will have to change - TODO          ????????????????????????????????? can't remember why this has to change!

                        Vector3 temp = gameObject.transform.position;
                        temp.y -= 2 + (queueLength * 0.8f);
                        temp.x -= 1.5f;

                        gameObject.transform.position = temp;

                        // delay here - QUEUE
                        if (addToQueueTickets != null)
                        {

                            addToQueueTickets(customer);
                            customer.inQueue = true;
                            customer.pointsToVisit.Clear();

                            customer.ticketsDone();
                            customer.animator.SetTrigger("queue");

                            customer.nextPlace(false);
                        }
                    }
                    else
                    {
                        customer.nextPoint(false);
                        //customer.SetTravellingTo(customer.pointsToVisit[0].x, customer.pointsToVisit[0].y);
                    }
                }

                //actually move
                else
                {
                    Vector2 movementVector = customer.MovementVector * Time.deltaTime * moveSpeed;
                    transform.Translate(movementVector);
                }
            }
        }

        if (customer.inQueue && customer.shouldMoveUp)
        {
            transform.Translate(0, 0.8f, 0);
            customer.shouldMoveUp = false;
        }
        if (customer.moveToServingSlot > -1)
        {
            customer.MoveToServingSlot();
            customer.servingSlot = customer.moveToServingSlot;
            customer.moveToServingSlot = -1;
        }
        if (customer.walkingAway)
        {
            customer.WalkOut();
            customer.walkingAway = false;
            customer.leaving = true;
        }
        if (customer.hasLeftTheBuilding)
        {
            Vector2 movementVector = customer.MovementVector * Time.deltaTime * moveSpeed;
            transform.Translate(movementVector);
            customer.walkingAway = false;
            customer.leaving = true;
        }
        if (transform.position.y < -20)
        {
            // they have left the building!
            transform.gameObject.SetActive(false);
        }

    }

    IEnumerator showPatienceBar()
    {
        if (imgs != null)
        {
            int patienceVal = customer.GetPatience();

            float val = (float)patienceVal / 1000;

            if (val > 1) { val = 1; }

            // this will be affected by the patience level
            imgs[1].fillAmount = val;
            imgs[1].color = new Color(1 - (float)(val), (float)(val), 0);

            yield return new WaitForSeconds(2.5f);
        
			customerStatus.SetActive (false);
        }
    }


    void OnMouseDown()
    {
		customerStatus.SetActive(true);

        StartCoroutine(showPatienceBar());       
    }


    // Update is called once per frame
    void Update ()
    {
        moveCustomer();
    }

    public int getQueueTicketSize()
    {
        if (getQueueTicketsSize != null)
        {
            return getQueueTicketsSize();
        }

        return 0;
    }

    public void ShowBuildingOptions()
    {

    }


    public void sortQueuePosition()
    {
        if (customer.inQueue)
        {
            transform.position = transform.position + new Vector3(0, 0.8f, 0);
        }
        else
        {
            // finished with Queue
            // set trigger
            //Vector3 tmp = transform.position;
            //tmp.y = customer.getTravellingToY();
            customer.goingToSeats = true;
            customer.SetTravellingTo(38.5f + (3 * customer.servingSlot), (11 * 0.8f));
            customer.servingSlot = -1;
            customer.pointsToVisit.Clear();
            //customer.nextPoint(false);
            //customer.SetTravellingTo(customer.getTravellingToX(), customer.getTravellingToY() / 0.8f);
            //transform.position = tmp;

        }
    }

}
