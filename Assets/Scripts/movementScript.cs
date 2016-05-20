using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Timers;
using System.Collections.Generic;
using System.Threading;

public class movementScript : MonoBehaviour {

    public delegate void AddToTicketQueue(Customer customer);
    public static event AddToTicketQueue addToQueueTickets;
    
    public delegate int GetTicketQueueSize();
    public static event GetTicketQueueSize getQueueTicketsSize;

    public delegate void AddToFoodQueue(Customer customer);
    public static event AddToFoodQueue addToQueueFood;
    
    public delegate int GetFoodQueueSize();
    public static event GetFoodQueueSize getQueueFoodSize;

    public AudioClip boredSound;
    public AudioClip neutralSound;

    Controller mainController;

    public static GameObject customerStatus;

    public Transform greenGuy;

    public float moveSpeed;

    private Animator animator;

    Customer customer;
    
    public Animation anim;

    Image[] imgs;

    int timeInQueue;

    int patienceCount = 0;
    bool showPatience = false;


    public void SetCustomer(Customer cust)
    {
        this.customer = cust;
    }

    // Use this for initialization
    void Start ()
    {
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
        
        animator = GetComponent<Animator>();

		imgs = customerStatus.GetComponentsInChildren<Image>();
        
    }

    private void MoveCustomer()
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

                    try
                    {
                        GetComponent<AudioSource>().Play();
                    }
                    catch (Exception) { }

                }


                float theX = transform.position.x;
                float theY = transform.position.y;


                if (customer.goingToSeats)
                {
                    customer.transform.GetComponent<SpriteRenderer>().sortingOrder = TileManager.floor.height - (int)(theY) - 1;
                }

                bool checkY1 = theY <= customer.GetTravellingToY() + 0.3f;
                bool checkY2 = theY >= customer.GetTravellingToY() - 0.3f;
                bool checkX1 = theX <= customer.GetTravellingToX() + 0.3f;
                bool checkX2 = theX >= customer.GetTravellingToX() - 0.3f;

                if (checkX1 && checkX2 && checkY1 && checkY2)
                {
                    
                    if (customer.goingToFood && customer.pointsToVisit.Count < 2)
                    {
                        int queueLength = getQueueFoodSize();

                        Vector3 temp = gameObject.transform.position;
                        temp.y -= 2 + (queueLength * 0.8f);
                        temp.x -= 0.7f;

                        gameObject.transform.position = temp;

                        // delay here - QUEUE
                        if (addToQueueFood != null)
                        {
                            addToQueueFood(customer);
                            customer.inQueue = true;
                            customer.pointsToVisit.Clear();

                            customer.goingToFood = false;
                            customer.goingToSeats = true;
                            customer.animator.SetTrigger("queue");

                            customer.NextPlace(false);
                        }
                    }

                    else if (customer.NeedsTickets() && customer.pointsToVisit.Count < 2)
                    {
                        int queueLength = getQueueTicketsSize();

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

                            customer.TicketsDone();
                            customer.animator.SetTrigger("queue");

                            customer.NextPlace(false);
                        }
                    }
                    else
                    {
                        customer.NextPoint(false);
                    }
                }

                //actually move
                else
                {
                    Vector2 movementVector = customer.MovementVector * Time.deltaTime * moveSpeed;
                    transform.Translate(movementVector);
                }
            }
            else if (customer.leaving)
            {
                Vector2 movementVector = customer.MovementVector * Time.deltaTime * moveSpeed;
                transform.Translate(movementVector);
            }

            // check status variables and perform necessary updates 
            if (customer.queueDoneWith != -1)
            {
                transform.GetComponent<SpriteRenderer>().sortingOrder = 40 - (int)(transform.position.y / 0.8f) - 1;
                customer.queueDoneWith = -1;
            }
            if (customer.inQueue && customer.shouldMoveUp > 0)
            {
                int move = customer.shouldMoveUp;
                customer.shouldMoveUp = 0;
                transform.Translate(0, move * 0.8f, 0);
                GetComponent<SpriteRenderer>().sortingOrder--;
            }
            if (customer.moveToServingSlot > -1)
            {
                customer.MoveToServingSlot();
                customer.servingSlot = customer.moveToServingSlot;
                customer.moveToServingSlot = -1;
                GetComponent<SpriteRenderer>().sortingLayerName = "Front";
                GetComponent<SpriteRenderer>().sortingOrder = 70;
            }
            if (customer.walkingAway)
            {
                transform.GetComponent<Animator>().SetTrigger("right");
                customer.WalkOut();
                customer.customerController.reputation.Walkout();
                customer.walkingAway = false;
                customer.leaving = true;
            }
            if (customer.hasLeftTheBuilding)
            {
                Vector2 movementVector = customer.MovementVector * Time.deltaTime * moveSpeed;
                transform.Translate(movementVector);
                customer.walkingAway = false;
            }
            if (customer.isBored)
            {
                transform.GetComponent<Animator>().SetTrigger("bored");
                customer.isBored = false;
            }
            if (transform.position.y < -20 && customer.GetPatience() < 1)
            {
                // they have left the building!
                transform.gameObject.SetActive(false);
            }

            if (patienceCount < 1 && transform.position.x > 42)
            {
                transform.GetComponent<Animator>().SetTrigger("down");
                customer.MovementVector = new Vector3(0, -1, 0);
            }

            
            if (customer.leaving && customer.MovementVector.Equals(new Vector2(0, 0)))
            {
                customer.MovementVector = new Vector2(0, -1);
                transform.GetComponent<Animator>().SetTrigger("down");
            }
        }
    }
    

    void OnMouseDown()
    {
		customerStatus.SetActive(true);

        int patienceVal = customer.GetPatience();

        float val = (float)patienceVal / 1000;

        if (val > 1) { val = 1; }

        // this will be affected by the patience level
        imgs[1].fillAmount = val;
        imgs[1].color = new Color(1 - (float)(val), (float)(val), 0);

        patienceCount = 0;
        showPatience = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (!mainController.paused)
        {
            MoveCustomer();
        }
        
        if (showPatience)
        {
            patienceCount++;
            if (patienceCount > 180)
            {
                patienceCount = 0;
                showPatience = false;
                customerStatus.SetActive(false);
            }
        }

    }

    public int GetQueueTicketSize()
    {
        if (getQueueTicketsSize != null)
        {
            return getQueueTicketsSize();
        }

        return 0;
    }
        

}
