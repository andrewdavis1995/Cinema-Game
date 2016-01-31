using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]

/*

    All comments (other than this one!) in this file represent steps that need to be added
    TOTAL COMMENTS REMAINING: 3

*/


public class mouseDrag : MonoBehaviour {

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

    int numSlots = 2;
    int numTicketSlots = 1;
    int numFrontDoorSlots = 1;

    List<GameObject> staffSlot = new List<GameObject>();

    public float offset;
    private Animator animator;

    int staffIndex = -1;

    int maxTicketStaff = 1;

    bool dragging = false;

    bool triggerSet = false;
    

    void Start()
    {
        for (int i = 0; i < numSlots; i++)
        {
            staffSlot.Add((GameObject)GameObject.Find("Slot " + i));
            staffSlot[i].GetComponent<Renderer>().enabled = false;
        }

        animator = GetComponent<Animator>();

        // calculate staffIndex
                
        if (getStaffListSize != null)
        {
            staffIndex = getStaffListSize();
        }

        StaffMember staff = new StaffMember(staffIndex);

        addToStaffList(staff);


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
        if (dragging)
        {
            doDragging();
        }
    }

    void OnMouseDown()
    {
        dragging = true;
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        changeStaffJob(staffIndex, 0);

        for (int i = 0; i < numSlots; i++)
        {
            if (validDrop(i))
            {
                staffSlot[i].GetComponent<Renderer>().enabled = true;
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
        dragging = false;

        for (int i = 0; i < numSlots; i++)
        {
            staffSlot[i].GetComponent<Renderer>().enabled = false;
        }

        transform.localScale = new Vector3(1, 1, 1);
        animator.SetTrigger("land");
        triggerSet = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1f, 0);

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

                        changeStaffJob(staffIndex, target);
                    }
                }
                else
                {
                    Bounds b2 = new Bounds(mousePos, new Vector3(2, 2, 0));

                    if (b1.Intersects(b2))
                    {
                        transform.position = new Vector3(transform.position.x + 3, transform.position.y, 0);
                    }
                    changeStaffJob(staffIndex, 0);
                }
            }
        }

    }


    void doDragging()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x + 0.75f, transform.position.y -1f, 0);

       
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
            }
            else
            {
                staffSlot[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("empty slot");
            }
        }
    }
    
}