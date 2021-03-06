﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts;
using System.Collections.Generic;
using Assets.Classes;
using System.Threading;
using System;

[RequireComponent(typeof(BoxCollider2D))]

public class mouseDrag : MonoBehaviour
{

    public static GameObject staffAttributePanel;
    
    public delegate int GetStaffSize();
    public static event GetStaffSize getStaffListSize;

    public delegate int GetStaffJob(int index);
    public static event GetStaffJob getStaffJobById;

    public delegate void UpdateStaffJob(int index, int job, int posInPost, bool add);
    public static event UpdateStaffJob changeStaffJob;

    public Sprite[] starImages;

    public delegate List<StaffMember> getFullStaffList();

    Text[] attributeTexts;
    Image[] attributeImages;

    public static event getFullStaffList getStaffList;

    float prevCameraZoom = 10f;    

    public float offset;
    private Animator animator;
    
    bool dragging = false;

    bool triggerSet = false;

    public StaffMember staffMember;

    Controller mainController;
    ShopController shopController;

    void Start()
    {
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
        shopController = GameObject.Find("ShopController").GetComponent<ShopController>();

        animator = GetComponent<Animator>();

        attributeImages = staffAttributePanel.GetComponentsInChildren<Image>();
        attributeTexts = staffAttributePanel.GetComponentsInChildren<Text>();
    }

    void Update()
    {
        if (dragging && (mainController.statusCode < 2 || mainController.statusCode == 6))
        {
            DoDragging();
            mainController.statusCode = 1;
        }
    }

    // dodgy function
    void CheckForOutOfBounds()
    {
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
    }
    
    void OnMouseDown()
    {
        // if dragging is valid
        if ((mainController.statusCode < 2) && staffMember.GetClicksRemaining() < 1 && Controller.isOwned)
        {
            dragging = true;

            // set the images for the customer
            SpriteRenderer[] subImages = GetComponentsInChildren<SpriteRenderer>(); 
            foreach (SpriteRenderer sr in subImages)
            {
                sr.sortingOrder = TileManager.floor.height + 7;
            }
            
            subImages[1].sortingOrder += 2;
            subImages[2].sortingOrder += 1;
            subImages[3].sortingOrder += 3;
            subImages[4].sortingOrder += 2;
            subImages[5].sortingOrder += 3;
            subImages[6].sortingOrder += 1;

            subImages[0].color = staffMember.GetColourByIndex(0);
            subImages[1].color = staffMember.GetColourByIndex(2);
            subImages[2].color = staffMember.GetColourByIndex(2);
            subImages[3].color = staffMember.GetColourByIndex(1);
            subImages[4].color = staffMember.GetColourByIndex(1);
            subImages[6].color = new Color(1, 1, 1, 1);

            transform.Translate(new Vector3(0, 0, 1));


            // show the attributes
            attributeTexts[0].text = staffMember.GetStaffname();
            
            int[] values = staffMember.GetAttributes();

            for (int i = 1; i < 5; i++)
            {
                attributeTexts[i].text = values[i - 1].ToString();
            }

            int level = staffMember.GetLevel();

            attributeImages[6].fillAmount = 0.25f * level;

            staffAttributePanel.SetActive(true);

            int posInPost = -1;

            // show the available slots
            for (int i = 0; i < mainController.staffSlot.Count; i++)
            {
                Bounds b1 = mainController.staffSlot[i].GetComponent<Renderer>().bounds;
                b1 = new Bounds(b1.center - new Vector3(0, 1.5f, 0), b1.extents);
                b1.extents = b1.extents * 1.5f;

                Bounds staffBounds = transform.GetComponent<SpriteRenderer>().bounds;


                if (b1.Intersects(staffBounds))
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
                        // a food slot
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
                       
            foreach (SpriteRenderer sr in subImages)
            {
                sr.sortingLayerName = "Staff";
            }

            //make the image shake and grow!
            prevCameraZoom = Camera.main.orthographicSize;
            Camera.main.orthographicSize = 6.5f;
            transform.GetComponent<SpriteRenderer>().color = staffMember.GetColourByIndex(0);
            transform.localScale = new Vector3(2f, 2f, 2f);
            mainController.UpdateStaffJob(staffMember.GetIndex(), staffMember.GetJobID(), posInPost, false);

            for (int i = 0; i < mainController.staffSlot.Count; i++)
            {
                if (!mainController.slotState[i])
                {
                    SpriteRenderer[] srs = mainController.staffSlot[i].GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer sr in srs) { sr.enabled = true; }

                    // get the level required to use this slot
                    int levReq = TileManager.GetAccessLevelForSlot(mainController.staffSlot[i].tag);
                    int currLev = staffMember.GetLevel();

                    // change color
                    if (currLev >= levReq)
                    {
                        srs[0].color = new Color(0, 1, 0, 0.8f);
                    }
                    else
                    {
                        srs[0].color = new Color(1, 0, 0, 0.8f);
                    }

                    // change star outputs
                    srs[2].sprite = starImages[levReq - 1];

                }
            }

        }
    }

    bool ValidDrop(int i)
    {
        return !mainController.slotState[i];
    }

    void OnMouseUp()
    {
        SpriteRenderer[] subImages = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in subImages)
        {
            sr.sortingLayerName = "Front";
        }

        if (dragging && (mainController.statusCode == 1 || mainController.statusCode == 0 || mainController.statusCode != 7) && mainController.statusCode != 50 && mainController.statusCode != 6)
        {
            staffAttributePanel.SetActive(false);

            Bounds staffBounds = new Bounds(transform.position, new Vector3(1, 1, 1));


            Bounds b = GameObject.Find("Box Office").GetComponent<Renderer>().bounds;

            Bounds boxOfficeBounds = new Bounds(b.center - new Vector3(0, 0.5f, 0), b.extents);

            Bounds newStaffBounds = new Bounds(staffBounds.center + new Vector3(0, 0.5f, 0), staffBounds.extents - new Vector3(0, 0.25f, 0));

            if (boxOfficeBounds.Intersects(newStaffBounds))
            {
                float xPos = transform.position.x;
                transform.position = new Vector2(xPos, 8f);
            }
                        

            this.transform.localScale = new Vector3(1, 1, 1);
            animator.SetTrigger("land");

            Animator handimator = GetComponentsInChildren<Animator>()[1];
            handimator.SetTrigger("land");

            // check if in an invalid place - in middle of screen or object - sort layer accordingly
            GameObject hidden = CheckHiddenBehind(staffBounds, shopController.gameObjectList, shopController.screenObjectList);           


            for (int i = 0; i < mainController.staffSlot.Count; i++)
            {
                SpriteRenderer[] srs = mainController.staffSlot[i].GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sr in srs) { sr.enabled = false; }
            }

            triggerSet = false;
            this.transform.position = new Vector3(transform.position.x, transform.position.y + 0.4f, 0);

            float x = this.transform.position.x;
            float y = this.transform.position.y;

            this.staffMember.SetVector(x, y);


            for (int i = 0; i < mainController.staffSlot.Count; i++)
            {
                Bounds b1 = mainController.staffSlot[i].GetComponent<Renderer>().bounds;

                Bounds newStaff = new Bounds(staffBounds.center + new Vector3(0, 1.25f, 0), staffBounds.extents);

                if (b1.Intersects(newStaff))
                {
                    if (ValidDrop(i))
                    {
                        // get the level required to use this slot
                        int levReq = TileManager.GetAccessLevelForSlot(mainController.staffSlot[i].tag);
                        int currLev = staffMember.GetLevel();

                        // change color
                        if (currLev >= levReq)
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

                                if (target == 2)
                                {
                                    transform.Translate(0, 0.27f, 0);
                                }

                                mainController.UpdateStaffJob(staffMember.GetIndex(), target, posInPost, true);
                            }
                        }
                        else
                        {
                            // back away...
                            if (b1.Intersects(newStaff))
                            {
                                transform.position = new Vector3(transform.position.x, transform.position.y + 2, 0);
                            }
                        }
                    }
                    else
                    {
                        // back away...
                        if (b1.Intersects(newStaff))
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y + 2, 0);
                        }
                        mainController.UpdateStaffJob(staffMember.GetIndex(), 0, 0, false);
                    }
                }
            }

            Camera.main.orthographicSize = prevCameraZoom;

            

            // check if overlapping wall at front door
            for (int k = 0; k < mainController.walls.Length; k++)
            {
                Bounds b2 = mainController.walls[k].GetComponent<Renderer>().bounds;

                Bounds newStaffBounds2 = new Bounds(staffBounds.center - new Vector3(0, 0.5f, 0), staffBounds.extents - new Vector3(0, 0.5f, 0));

                if (b2.Intersects(newStaffBounds2))
                {

                    float xPos = transform.position.x;
                    transform.position = new Vector2(xPos, 0f);
                }

            }


            SortStaffLayer(hidden);

            if (mainController.statusCode < 3)
            {
                mainController.statusCode = 0;
            }

        }
        dragging = false;
        

        if (transform.position.x > 79f)
        {
            transform.position = new Vector3(78, transform.position.y, -1);
        }
        if (transform.position.y > (39f * 0.8f))
        {
            transform.position = new Vector3(transform.position.x, 38 * 0.8f, -1);
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, -1);

        staffMember.SetTransform(transform);

    }

    /// <summary>
    /// Move the customer
    /// </summary>
    void DoDragging()
    {

        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x + 0.75f, transform.position.y - 1f, 0);


        if (!triggerSet)
        {
            animator.SetTrigger("fly");
            triggerSet = true;
            Animator handimator = GetComponentsInChildren<Animator>()[1];
            handimator.SetTrigger("fly");
        }

        for (int i = 0; i < mainController.staffSlot.Count; i++)
        {
            Bounds b1 = mainController.staffSlot[i].GetComponent<Renderer>().bounds;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0f);

            if (b1.Contains(mousePos) )
            {
                // get the level required to use this slot
                int levReq = TileManager.GetAccessLevelForSlot(mainController.staffSlot[i].tag);
                int currLev = staffMember.GetLevel();

                // change color
                if (currLev >= levReq)
                {
                    // lock to pos
                    Vector3 position = mainController.staffSlot[i].transform.position;
                    position.y -= 1.8f;
                    transform.position = position;
                }

                // TODO: change image / animation of staff ?
            }

        }



        if (Camera.main.WorldToScreenPoint(transform.position).x > UnityEngine.Screen.width - 50)
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(0.2f, 0, 0);
        }
        if (Camera.main.WorldToScreenPoint(transform.position).x < 100)
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(-0.2f, 0, 0);
        }
        if (Camera.main.WorldToScreenPoint(transform.position).y > UnityEngine.Screen.height - 100)
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0.2f, 0);
        }
        if (Camera.main.WorldToScreenPoint(transform.position).y < 50)
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, -0.2f, 0);
        }

        Camera.main.GetComponent<CameraControls>().endPos = Camera.main.transform.position;
        //Vector3 tmp = Camera.main.WorldToScreenPoint(transform.position);
        //tmp.z = -10;
        //tmp.x -= UnityEngine.ScreenObject.width / 2;
        //tmp.y -= UnityEngine.ScreenObject.height / 2;

        //CheckForOutOfBounds();
    }
    
    /// <summary>
    /// get the bounds for an object - to see if a staff member is hidden behind it
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static Bounds GetObjectHiddenBounds(GameObject go)
    {
        Bounds toReturn = new Bounds();
        Bounds overallBounds = go.GetComponent<Renderer>().bounds; 

        if (go.tag.Equals("Vending Machine"))
        {            
            toReturn = new Bounds(new Vector3(overallBounds.center.x, (overallBounds.center.y - 0.35f), 0), 2 * new Vector3(overallBounds.extents.x, 1.5f, 0.1f));
        }
        else if (go.tag.Equals("Bust"))
        {
            toReturn = new Bounds(new Vector3(overallBounds.center.x, (overallBounds.center.y - 0.65f), 0), 2 * new Vector3(overallBounds.extents.x, 1.24f, 0.1f));
        }
        else if (go.tag.Equals("Plant"))
        {
            toReturn = new Bounds(new Vector3(overallBounds.center.x, (overallBounds.center.y - 0.135f), 0), 2 * new Vector3(overallBounds.extents.x, 0.35f, 0.1f));
        }

        return toReturn;
    }

    /// <summary>
    /// check if a staff member is hidden
    /// </summary>
    /// <param name="staffBounds"></param>
    /// <param name="gameObjectList"></param>
    /// <param name="screenObjectList"></param>
    /// <returns></returns>
    public static GameObject CheckHiddenBehind(Bounds staffBounds, List<GameObject> gameObjectList, List<GameObject> screenObjectList)
    {
        GameObject toHideBehind = null;

        for (int i = 0; i < gameObjectList.Count; i++)
        {
            Bounds bTop = GetObjectHiddenBounds(gameObjectList[i]);

            if (bTop.Intersects(staffBounds)) { toHideBehind = gameObjectList[i]; break; }

        }
        
        if (toHideBehind == null)
        {
            for (int i = 0; i < screenObjectList.Count; i++)
            {

                Bounds b1 = screenObjectList[i].GetComponent<Renderer>().bounds;

                Bounds bTop = new Bounds(new Vector3(b1.center.x, (b1.center.y - b1.extents.y + 8.8f), 0), 2 * new Vector3(b1.extents.x, 1.8f, 0.125f));
                Bounds bBottom = new Bounds(new Vector3(b1.center.x, (b1.center.y - b1.extents.y + 0.9f), 0), 2 * new Vector3(b1.extents.x, 0.65f, 0.1f));
  

                if (bTop.Intersects(staffBounds)) { toHideBehind = screenObjectList[i]; break; }
                if (bBottom.Intersects(staffBounds)) { toHideBehind = screenObjectList[i]; break; }

                if (b1.Intersects(staffBounds) && ShopController.theScreens[i].ConstructionInProgress())
                {
                    toHideBehind = screenObjectList[i];
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
            transform.GetComponent<SpriteRenderer>().sortingOrder = TileManager.floor.height + 6;


            //transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);

            // check bounds with the walls of the screens
            CheckWallCollision();

        }
        else
        {
            SpriteRenderer[] subImages = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in subImages)
            {
                sr.sortingOrder = hidden.GetComponent<SpriteRenderer>().sortingOrder + 6;
                sr.color = new Color(1, 1, 1, 0.175f);
            }
            subImages[0].sortingOrder--;
            subImages[3].sortingOrder++;

            subImages[4].color = new Color(1, 1, 1, 1);
            subImages[4].enabled = true;

            Transform pi3 = transform.FindChild("hiddenPointer");
            pi3.GetComponent<SpriteRenderer>().enabled = true;
            pi3.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

        }
        

    }

    /// <summary>
    /// check if a staff member is hidden behind a wall
    /// </summary>
    void CheckWallCollision()
    {
        Bounds charBounds = transform.GetComponent<Renderer>().bounds;

        for (int i = 0; i < shopController.screenObjectList.Count; i++)
        {
            Bounds screenWallBounds1 = shopController.screenObjectList[i].GetComponent<Renderer>().bounds;
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