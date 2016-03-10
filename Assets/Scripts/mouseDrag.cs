using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts;
using System.Collections.Generic;
using Assets.Classes;

[RequireComponent(typeof(BoxCollider2D))]

/*

    All comments (other than this one!) in this file represent steps that need to be added
    TOTAL COMMENTS REMAINING: 3

*/


public class mouseDrag : MonoBehaviour
{

    public delegate void addStaffMember(StaffMember staff);
    public static event addStaffMember addStaff;

    public delegate int getStaffSize();
    public static event getStaffSize getStaffListSize;

    public delegate int getStaffJob(int index);
    public static event getStaffJob getStaffJobById;

    public delegate void updateStaffJob(int index, int job);
    public static event updateStaffJob changeStaffJob;

    public delegate List<StaffMember> getFullStaffList();
    public static event getFullStaffList getStaffList;

    float prevCameraZoom = 10f;

    int numSlots = 2;
    int numTicketSlots = 1;
    int numFrontDoorSlots = 1;

    List<GameObject> staffSlot = new List<GameObject>();

    public float offset;
    private Animator animator;

    int maxTicketStaff = 1;

    bool dragging = false;

    bool triggerSet = false;

    public StaffMember staffMember;

    Controller mainController;

    void Start()
    {
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();

        for (int i = 0; i < numSlots; i++)
        {
            staffSlot.Add((GameObject)GameObject.Find("Slot " + i));
            staffSlot[i].GetComponent<Renderer>().enabled = false;
        }

        animator = GetComponent<Animator>();

        // calculate staffIndex


        //StaffMember staff = new StaffMember(staffIndex);

        //addToStaffList(staff);


    }

    void addToStaffList(StaffMember staff)
    {
        if (addStaff != null)
        {
            addStaff(staff);
        }
    }

    void Update()
    {
        //Debug.Log(Input.mousePosition);
        //Debug.Log("WtS: " + Camera.main.WorldToScreenPoint(Camera.main.transform.transform.position).x);
        //Debug.Log("WtV: " + Camera.main.WorldToViewportPoint(Camera.main.transform.transform.position).x);
        ////Debug.Log("VtS: " + Camera.main.ViewportToScreenPoint(Camera.main.transform.transform.position).x);
        //Debug.Log("VtW: " + Camera.main.ViewportToWorldPoint(Camera.main.transform.transform.position).x);
        //Debug.Log("StW: " + Camera.main.ScreenToWorldPoint(Camera.main.transform.transform.position).x); // this one
        //Debug.Log("StV: " + Camera.main.ScreenToViewportPoint(Camera.main.transform.transform.position).x);

        //Debug.Log("WtS: " + Camera.main.WorldToScreenPoint(Input.mousePosition).x);
        //Debug.Log("WtV: " + Camera.main.WorldToViewportPoint(Input.mousePosition).x);
        ////Debug.Log("VtS: " + Camera.main.ViewportToScreenPoint(Input.mousePosition).x);
        //Debug.Log("VtW: " + Camera.main.ViewportToWorldPoint(Input.mousePosition).x);
        //Debug.Log("StW: " + Camera.main.ScreenToWorldPoint(Input.mousePosition).x); // this one
        ////Debug.Log("StV: " + Camera.main.ScreenToViewportPoint(Input.mousePosition).x);

        if (dragging && (mainController.statusCode == 1 || mainController.statusCode == 0 || mainController.statusCode == 6 || mainController.statusCode != 7))
        {
            doDragging();
            mainController.statusCode = 1;
        }

        //checkForOutOfBounds();

    }

    // dodgy function
    void checkForOutOfBounds()
    {
        // between - 40 and - 10
        //Debug.Log("StW: " + Camera.main.ScreenToWorldPoint(transform.position).x); // this one
        float theX = Camera.main.ScreenToWorldPoint(transform.position).x;
        if (theX < -40)
        {
            Vector3 tmp = Camera.main.transform.position;
            tmp.x = -15f;
            tmp.z = -10f;
            Camera.main.transform.position = tmp;
        }
        if (theX > -10)
        {
            Vector3 tmp = Camera.main.transform.position;
            tmp.x = 17f;
            tmp.z = -10f;
            Camera.main.transform.position = tmp;
        }

        //float theY = Camera.main.ScreenToWorldPoint(transform.position).y;
        //Debug.Log("StW: " + Camera.main.ScreenToWorldPoint(transform.position).y); // this one
        //if (theY < -12)
        //{
        //    Vector3 tmp = Camera.main.transform.position;
        //    tmp.z = -10f;
        //    Camera.main.transform.position = tmp;
        //}
        //if (theY > -25f)
        //{
        //    Vector3 tmp = Camera.main.transform.position;
        //    tmp.y = -15f;
        //    tmp.z = -10f;
        //    Camera.main.transform.position = tmp;
        //}
    }

    void OnMouseDown()
    {

        if ((mainController.statusCode == 1 || mainController.statusCode == 0 || mainController.statusCode == 6 || mainController.statusCode != 7))
        {
            dragging = true;
            prevCameraZoom = Camera.main.orthographicSize;
            Camera.main.orthographicSize = 9;
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            changeStaffJob(staffMember.getIndex(), 0);

            for (int i = 0; i < numSlots; i++)
            {
                if (validDrop(i))
                {
                    staffSlot[i].GetComponent<Renderer>().enabled = true;
                }
            }
        }
    }

    bool validDrop(int i)
    {
        int target = 0;
        int maxStaff = 0;

        if (i < numTicketSlots)
        {
            // a ticket slot
            target = 1;
            maxStaff = numTicketSlots;
        }
        else
        {
            target = 2;
            maxStaff = numFrontDoorSlots;
        }

        int staffCount = 0;

        List<StaffMember> staff = new List<StaffMember>();

        if (getStaffList != null)
        {
            staff = getStaffList();
        }
        for (int j = 0; j < staff.Count; j++)
        {
            if (staff[j].getJobID() == target) { staffCount++; }
        }

        return staffCount < maxStaff;
    }

    void OnMouseUp()
    {

        if (dragging && (mainController.statusCode == 1 || mainController.statusCode == 0 || mainController.statusCode == 6 || mainController.statusCode != 7))
        {
            dragging = false;
            mainController.statusCode = 0;

            for (int i = 0; i < numSlots; i++)
            {
                staffSlot[i].GetComponent<Renderer>().enabled = false;
            }

            this.transform.localScale = new Vector3(1, 1, 1);
            animator.SetTrigger("land");
            triggerSet = false;
            this.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, 0);

            float x = this.transform.position.x;
            float y = this.transform.position.y;

            this.staffMember.SetVector(x, y);

            for (int i = 0; i < numSlots; i++)
            {
                Bounds b1 = staffSlot[i].GetComponent<Renderer>().bounds;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos = new Vector3(mousePos.x, mousePos.y, 0f);

                if (b1.Contains(mousePos))
                {
                    if (validDrop(i))
                    {
                        // add to list of staff at ticket booth
                        if (changeStaffJob != null)
                        {
                            int target = 0;

                            if (i < numTicketSlots)
                            {
                                // a ticket slot
                                target = 1;
                            }
                            else
                            {
                                target = 2;
                            }

                            changeStaffJob(staffMember.getIndex(), target);
                        }
                    }
                    else
                    {
                        Bounds b2 = new Bounds(mousePos, new Vector3(2, 2, 0));

                        if (b1.Intersects(b2))
                        {
                            transform.position = new Vector3(transform.position.x + 3, transform.position.y, 0);
                        }
                        changeStaffJob(staffMember.getIndex(), 0);
                    }
                }
            }

            Camera.main.orthographicSize = prevCameraZoom;
        }
        
    }

    void doDragging()
    {


        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x + 0.75f, transform.position.y - 1f, 0);


        if (!triggerSet)
        {
            animator.SetTrigger("fly");
            triggerSet = true;
        }

        for (int i = 0; i < numSlots; i++)
        {
            Bounds b1 = staffSlot[i].GetComponent<Renderer>().bounds;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0f);

            if (b1.Contains(mousePos))
            {
                staffSlot[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("empty slot 2");
                // lock to pos
                Vector3 position = staffSlot[i].transform.position;
                position.y -= 1.8f;
                transform.position = position;
                // change image / animation ?
            }
            else
            {
                staffSlot[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("empty slot");
            }

        }



        if (Camera.main.WorldToScreenPoint(transform.position).x > UnityEngine.Screen.width - 50)
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(0.20f, 0, 0); 
        }
        if (Camera.main.WorldToScreenPoint(transform.position).x < 50)
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(-0.20f, 0, 0); 
        }
        if (Camera.main.WorldToScreenPoint(transform.position).y > UnityEngine.Screen.height - 50)
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0.20f, 0); 
        }
        if (Camera.main.WorldToScreenPoint(transform.position).y < 50)
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, -0.20f, 0); 
        }

        Camera.main.GetComponent<CameraControls>().endPos = Camera.main.transform.position;
        //Vector3 tmp = Camera.main.WorldToScreenPoint(transform.position);
        //tmp.z = -10;
        //tmp.x -= UnityEngine.ScreenObject.width / 2;
        //tmp.y -= UnityEngine.ScreenObject.height / 2;
    }

}