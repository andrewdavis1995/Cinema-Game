using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Timers;
using System.Collections.Generic;

public class movementScript : MonoBehaviour {

    public delegate void addToTicketQueue(Customer customer);
    public static event addToTicketQueue addToQueueTickets;

    public delegate Queue<Customer> getTicketQueue();
    public static event getTicketQueue getQueueTickets;

    public delegate int getTicketQueueSize();
    public static event getTicketQueueSize getQueueTicketsSize;

    public float moveSpeed;

    private Animator animator;

    public Customer customer;
    
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
        animator = GetComponent<Animator>();
        imgs = GameObject.Find("Customer Status").GetComponentsInChildren<Image>();
    }

    private void moveCustomer(int index)
    {
        //get direction
        int newDir = 0;
        string direction = "idle";

        if (customer != null)
        {
            if (!customer.inQueue)
            {

                float theX = transform.position.x;
                float theY = transform.position.y;


                if (theY > customer.getTravellingToY() + 0.11f)
                {
                    // move up
                    transform.Translate(new Vector3(0, -moveSpeed * Time.deltaTime, 0));
                    //change direction
                    newDir = 1;
                    //customers[index].updatePosition(theX, theY - moveSpeed);
                    direction = "down";
                }
                else if (theY < customer.getTravellingToY() - 0.11f)
                {
                    // move down
                    transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
                    //change direction
                    newDir = 2;
                    //customers[index].updatePosition(theX, theY + moveSpeed);
                    direction = "up";
                }
                else if (theX < customer.getTravellingToX() - 0.11f)
                {
                    // move left
                    transform.Translate(new Vector3(+moveSpeed * Time.deltaTime, 0, 0));
                    //change direction
                    newDir = 4;
                    //customers[index].updatePosition(theX + moveSpeed, theY);
                    direction = "right";
                }
                else if (theX > customer.getTravellingToX() + 0.11f)
                {
                    // move right
                    transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0, 0));
                    //change direction
                    newDir = 3;
                    //customers[index].updatePosition(theX - moveSpeed, theY);
                    direction = "left";
                }
                else
                {
                    newDir = 0;

                    if (!customer.isGoingToSeat())
                    {

                        int queueLength = getQueueTicketsSize();

                        float yPos = -4.5f - queueLength * 0.8f;

                        Vector3 temp = gameObject.transform.position;
                        temp.y = yPos;
                        temp.x += 2.2f;

                        gameObject.transform.position = temp;

                        // delay here - QUEUE
                        if (addToQueueTickets != null)
                        {
                            addToQueueTickets(customer);
                            customer.inQueue = true;
                            animator.SetTrigger("queue");
                        }

                        customer.ticketsDone();
                        customer.nextPlace();

                    }
                    else
                    {
                        // finished
                    }
                }
            }
            else
            {
                timeInQueue++;

                if (timeInQueue % 1000 == 0)
                {
                    animator.SetTrigger("bored");
                }
                else if (timeInQueue % 1000 == 500)
                {
                    animator.SetTrigger("queue");
                }
            }

        if (customer.currentDirection != newDir)
        {
            animator.SetTrigger(direction);
        }

        customer.currentDirection = newDir;
        }

    }


    IEnumerator showPatienceBar()
    {
        if (imgs != null)
        {

            // this will be affected by the patience level
            imgs[1].color = new Color(152, 100, 0);

            yield return new WaitForSeconds(1.25f);

        

            for (int i = 0; i < imgs.Length; i++)
            {
                imgs[i].enabled = false;
            }


            imgs = null;
        }
    }


    void OnMouseDown()
    {
        for (int i = 0; i < imgs.Length; i++)
        {
            imgs[i].enabled = true;
        }

        StartCoroutine(showPatienceBar());       
    }

    public Transform greenGuy;


    // Update is called once per frame
    void Update () {

        moveCustomer(0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(greenGuy, new Vector3(15, -10, 0), Quaternion.identity);
            greenGuy = null;
        }
    }

    public int getQueueTicketSize()
    {
        if (getQueueTicketsSize != null)
        {
            return getQueueTicketsSize();
        }

        return 0;

    }
}
