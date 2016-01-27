using UnityEngine;
using System.Collections;

public class movementScript : MonoBehaviour {

    public float moveSpeed;

    private Animator animator;

    public Customer[] customers = new Customer[5];

    public Animation anim;
    

    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
        customers[0] = new Customer(0);
    }

    private void moveCustomer(int index)
    {

        float theX = transform.position.x;
        float theY = transform.position.y;

        string direction = "idle";

        //get direction
        int newDir;

        if (theY > customers[index].getTravellingToY() + 0.11f)
        {
            // move up
            transform.Translate(new Vector3(0, -moveSpeed, 0));
            //change direction
            newDir = 1;
            customers[index].updatePosition(theX, theY - moveSpeed);
            direction = "down";
        }
        else if (theY < customers[index].getTravellingToY() - 0.11f)
        {
            // move down
            transform.Translate(new Vector3(0, moveSpeed, 0));
            //change direction
            newDir = 2;
            customers[index].updatePosition(theX, theY + moveSpeed);
            direction = "up";
        }
        else if (theX < customers[index].getTravellingToX() - 0.11f)
        {
            // move left
            transform.Translate(new Vector3(+moveSpeed, 0, 0));
            //change direction
            newDir = 4;
            customers[index].updatePosition(theX + moveSpeed, theY);
            direction = "right";
        }
        else if (theX > customers[index].getTravellingToX() + 0.11f)
        {
            // move right
            transform.Translate(new Vector3(-moveSpeed, 0, 0));
            //change direction
            newDir = 3;
            customers[index].updatePosition(theX - moveSpeed, theY);
            direction = "left";
        }
        else
        {
            newDir = 0;
            customers[index].nextPlace();

            //if (!customers[index].isGoingToSeat())
            //{
            //    customerTimers[index].Stop();
            //    customers[index].getPicture().Top = 300 + (ticketsQueue.Count * 50);
            //    customers[index].ticketsDone();
            //    customers[index].nextPlace();
            //    customers[index].getPicture().Left = 520;

            //    ticketsQueue.Enqueue(new Customer(index));
            //}
            //else
            //{
            //    customerTimers[index].Stop();
            //    customers[index].getPicture().Visible = false;
            //}
        }

        //Debug.Log("DIRECTION: " + direction);


        if (customers[index].currentDirection != newDir)
        {
            // change image

            Debug.Log(direction);
            //GetComponent<animation>.Play("down");
            //GetComponent<Animation>().Play(direction);
            //animation.Play("down");

            animator.SetTrigger(direction);

        }
        
        customers[index].currentDirection = newDir;

    }

    // Update is called once per frame
    void Update () {

        moveCustomer(0);        
        
    }
}
