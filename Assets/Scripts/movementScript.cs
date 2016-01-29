using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class movementScript : MonoBehaviour {

    public float moveSpeed;

    private Animator animator;

    public Customer customer;
    
    public Animation anim;


    public void setCustomer(Customer cust)
    {
        this.customer = cust;
    }


    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
    }

    private void moveCustomer(int index)
    {

        float theX = transform.position.x;
        float theY = transform.position.y;

        string direction = "idle";

        //get direction
        int newDir;

        
        if (theY > customer.getTravellingToY() + 0.11f)
        {
            // move up
            transform.Translate(new Vector3(0, -moveSpeed, 0));
            //change direction
            newDir = 1;
            //customers[index].updatePosition(theX, theY - moveSpeed);
            direction = "down";
        }
        else if (theY < customer.getTravellingToY() - 0.11f)
        {
            // move down
            transform.Translate(new Vector3(0, moveSpeed, 0));
            //change direction
            newDir = 2;
            //customers[index].updatePosition(theX, theY + moveSpeed);
            direction = "up";
        }
        else if (theX < customer.getTravellingToX() - 0.11f)
        {
            // move left
            transform.Translate(new Vector3(+moveSpeed, 0, 0));
            //change direction
            newDir = 4;
            //customers[index].updatePosition(theX + moveSpeed, theY);
            direction = "right";
        }
        else if (theX > customer.getTravellingToX() + 0.11f)
        {
            // move right
            transform.Translate(new Vector3(-moveSpeed, 0, 0));
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
                // delay here

                customer.ticketsDone();
                customer.nextPlace();

            //    ticketsQueue.Enqueue(new Customer(index));
            }
            else
            {
                // finished
            }
        }

        //Debug.Log("DIRECTION: " + direction);
        Debug.Log(customer.currentDirection + " v " + newDir);


        if (customer.currentDirection != newDir)
        {
            animator.SetTrigger(direction);
        }

        
        customer.currentDirection = newDir;
            

    }


    void OnMouseOver()
    {
        Image[] inputs = GameObject.Find("Overlay Canvas").GetComponentsInChildren<Image>();
        inputs[0].enabled = true;
        inputs[0].transform.position = transform.position;
    }

    void OnMouseExit()
    {
        Image [] inputs = GameObject.Find("Overlay Canvas").GetComponentsInChildren<Image>();
        inputs[0].enabled = false;
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

    void OnMouseDown()
    {
        Debug.Log("YIPEE!!!");
    }
}
