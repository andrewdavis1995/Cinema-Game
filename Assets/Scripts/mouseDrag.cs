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

    public delegate void addStaffMember(StaffMember staff, int x, int y);
    public static event addStaffMember addStaff;

    public delegate int getStaffSize();
    public static event getStaffSize getStaffListSize;

    public delegate int getStaffJob(int index);
    public static event getStaffJob getStaffJobById;

    public delegate void updateStaffJob(int index, int job, int posInPost, bool add);
    public static event updateStaffJob changeStaffJob;

    public delegate List<StaffMember> getFullStaffList();
    public static event getFullStaffList getStaffList;

    float prevCameraZoom = 10f;

    

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


        animator = GetComponent<Animator>();

    }

    void addToStaffList(StaffMember staff)
    {

        int x = 35 + (2 * staff.getIndex() % 6);
        int y = 3 * (staff.getIndex() / 6);


        if (addStaff != null)
        {
            addStaff(staff, x, y);
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
        if ((mainController.statusCode < 3))
        {
            int posInPost = -1;

            for (int i = 0; i < mainController.staffSlot.Count; i++)
            {
                Bounds b1 = mainController.staffSlot[i].GetComponent<Renderer>().bounds;
                b1.extents = b1.extents * 1.5f;

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos = new Vector3(mousePos.x, mousePos.y, 0f);

                if (b1.Contains(mousePos))
                {
                    mainController.slotState[i] = false;

                    int target = 0;

                    if (mainController.staffSlot[i].tag.Contains("1"))
                    {
                        // a ticket slot
                        target = 1;
                    }
                    else
                    {
                        target = 2;
                    }

                    GameObject[] slots = GameObject.FindGameObjectsWithTag("Slot Type " + target);

                    for (int j = 0; j < slots.Length; j++)
                    {
                        if (slots[j].name.Equals(mainController.staffSlot[i].name))
                        {
                            posInPost = j;
                            break;
                        }
                    }

                    if (posInPost > -1)
                    {
                        break;
                    }
                }

            }

            Transform pi3 = transform.FindChild("hiddenPointer");
            pi3.GetComponent<SpriteRenderer>().enabled = false;

            //make the image shake and grow!
            GetComponent<SpriteRenderer>().sortingLayerName = "Staff";
            dragging = true;
            prevCameraZoom = Camera.main.orthographicSize;
            Camera.main.orthographicSize = 9;
            transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            transform.localScale = new Vector3(2f, 2f, 2f);
            changeStaffJob(staffMember.getIndex(), staffMember.getJobID(), posInPost, false);

            for (int i = 0; i < mainController.staffSlot.Count; i++)
            {
                if (!mainController.slotState[i])
                {
                    mainController.staffSlot[i].GetComponent<Renderer>().enabled = true;
                }
            }
        }
    }

    bool validDrop(int i)
    {
        return !mainController.slotState[i];
    }

    void OnMouseUp()
    {
        GetComponent<SpriteRenderer>().sortingLayerName = "Front";
        

        if (dragging && (mainController.statusCode == 1 || mainController.statusCode == 0 || mainController.statusCode == 6 || mainController.statusCode != 7))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0f);


            // check if in an invalid place - in middle of screen or object - sort layer accordingly
            GameObject hidden = CheckHiddenBehind(mousePos);           


            for (int i = 0; i < mainController.staffSlot.Count; i++)
            {
                mainController.staffSlot[i].GetComponent<Renderer>().enabled = false;
            }

            this.transform.localScale = new Vector3(1, 1, 1);
            animator.SetTrigger("land");
            triggerSet = false;
            this.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, 0);

            float x = this.transform.position.x;
            float y = this.transform.position.y;

            this.staffMember.SetVector(x, y);


            for (int i = 0; i < mainController.staffSlot.Count; i++)
            {
                Bounds b1 = mainController.staffSlot[i].GetComponent<Renderer>().bounds;
                
                if (b1.Contains(mousePos))
                {
                    if (validDrop(i))
                    {
                        // add to list of staff at ticket booth
                        if (changeStaffJob != null)
                        {
                            int target = 0;

                            if (mainController.staffSlot[i].tag.Contains("1"))
                            {
                                // a ticket slot
                                target = 1;
                            }
                            else
                            {
                                target = 2;
                            }

                            mainController.slotState[i] = true;

                            GameObject[] slots = GameObject.FindGameObjectsWithTag("Slot Type " + target);

                            int posInPost = -1;
                            for (int j = 0; j < slots.Length; j++)
                            {
                                if (slots[j].name.Equals(mainController.staffSlot[i].name))
                                {
                                    posInPost = j;
                                    break;
                                }
                            }
                            
                            changeStaffJob(staffMember.getIndex(), target, posInPost, true);
                        }
                    }
                    else
                    {
                        Bounds b2 = new Bounds(mousePos, new Vector3(2, 2, 0));

                        if (b1.Intersects(b2))
                        {
                            transform.position = new Vector3(transform.position.x + 3, transform.position.y, 0);
                        }
                        changeStaffJob(staffMember.getIndex(), 0, 0, false);
                    }
                }
            }

            Camera.main.orthographicSize = prevCameraZoom;
            

            // check if overlapping wall at front door
            for (int k = 0; k < mainController.walls.Length; k++)
            {
                Bounds b = mainController.walls[k].GetComponent<Renderer>().bounds;

                if (b.Contains(mousePos))
                {

                    float xPos = transform.position.x;

                    if (xPos > 73)
                    {
                        xPos = 73;
                    }

                    transform.position = new Vector2(xPos, 0.8f);
                }

            }


            SortStaffLayer(hidden);

        }
        dragging = false;

        if (mainController.statusCode < 3)
        {
            mainController.statusCode = 0;
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

        for (int i = 0; i < mainController.staffSlot.Count; i++)
        {
            Bounds b1 = mainController.staffSlot[i].GetComponent<Renderer>().bounds;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0f);

            if (b1.Contains(mousePos))
            {
                mainController.staffSlot[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("empty slot 2");
                // lock to pos
                Vector3 position = mainController.staffSlot[i].transform.position;
                position.y -= 1.8f;
                transform.position = position;
                // change image / animation ?
            }
            else
            {
                mainController.staffSlot[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("empty slot");
            }

        }



        if (Camera.main.WorldToScreenPoint(transform.position).x > UnityEngine.Screen.width - 50)
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(0.20f, 0, 0);
        }
        if (Camera.main.WorldToScreenPoint(transform.position).x < 100)
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(-0.20f, 0, 0);
        }
        if (Camera.main.WorldToScreenPoint(transform.position).y > UnityEngine.Screen.height)
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

    GameObject CheckHiddenBehind(Vector3 mousePos)
    {
        GameObject toHideBehind = null;

        for (int i = 0; i < mainController.gameObjectList.Count; i++)
        {

            Bounds b1 = mainController.gameObjectList[i].GetComponent<Renderer>().bounds;


            if (b1.Contains(mousePos)) { toHideBehind = mainController.gameObjectList[i]; break; }

        }
        
        if (toHideBehind == null)
        {
            for (int i = 0; i < mainController.screenObjectList.Count; i++)
            {

                Bounds b1 = mainController.screenObjectList[i].GetComponent<Renderer>().bounds;

                Bounds bTop = new Bounds(new Vector3(b1.center.x, (b1.center.y + 3.741147f), 0), 2 * new Vector3(b1.extents.x, 2.15883f, 0.1f));
                Bounds bBottom = new Bounds(new Vector3(b1.center.x, (b1.center.y - b1.extents.y + 1.352084f), 0), 2 * new Vector3(b1.extents.x, 1.352084f, 0.1f));
  

                if (bTop.Contains(mousePos)) { toHideBehind = mainController.screenObjectList[i]; break; }
                if (bBottom.Contains(mousePos)) { toHideBehind = mainController.screenObjectList[i]; break; }

                if (b1.Contains(mousePos) && Controller.theScreens[i].ConstructionInProgress())
                {
                    toHideBehind = mainController.screenObjectList[i];
                    break;
                }

            }
        }

        return toHideBehind;

    }

    public void SortStaffLayer(GameObject hidden)
    {
        if (hidden == null)
        {
            transform.GetComponent<SpriteRenderer>().sortingOrder = TileManager.floor.height;
            transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);

            // check bounds with the walls of the screens
            CheckWallCollision();

        }
        else
        {
            transform.GetComponent<SpriteRenderer>().sortingOrder = hidden.GetComponent<SpriteRenderer>().sortingOrder + 5;
            transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.175f);

            Transform pi3 = transform.FindChild("hiddenPointer");
            pi3.GetComponent<SpriteRenderer>().enabled = true;

        }
        

    }

    void CheckWallCollision()
    {
        Bounds charBounds = transform.GetComponent<Renderer>().bounds;

        for (int i = 0; i < mainController.screenObjectList.Count; i++)
        {
            Bounds screenWallBounds1 = mainController.screenObjectList[i].GetComponent<Renderer>().bounds;
            Bounds b1 = new Bounds(new Vector3(screenWallBounds1.center.x - screenWallBounds1.extents.x + 0.1f, screenWallBounds1.center.y, screenWallBounds1.center.z), 2 * new Vector3(0.1f, screenWallBounds1.extents.y, screenWallBounds1.extents.z));
            Bounds b2 = new Bounds(new Vector3(screenWallBounds1.center.x + screenWallBounds1.extents.x - 0.1f, screenWallBounds1.center.y, screenWallBounds1.center.z), 2 * new Vector3(0.1f, screenWallBounds1.extents.y, screenWallBounds1.extents.z));

            if (b1.Intersects(charBounds))
            {
                transform.Translate(new Vector3(+2f, 0, 0));
            }
            if (b2.Intersects(charBounds))
            {
                transform.Translate(new Vector3(-2f, 0, 0));
            }

        }
    }

}