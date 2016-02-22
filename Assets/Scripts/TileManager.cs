﻿using UnityEngine;
using System.Collections;
using Assets.Classes;
using System;
using System.IO;

public class TileManager : MonoBehaviour {

    public Floor floor;
    public Sprite carpetSprite;

    GameObject movementPanel;

    bool validMove = true;
    bool previousValidMove = true;

    public int toMoveX = -1;
    public int toMoveY = -1;

    public int origX = -1;
    public int origY = -1;

    const int width = 80;
    const int height = 40;

    public int fullWidth = -1;
    public int fullHeight = -1;

    Controller mainController;

    Vector3 currentPosition = new Vector3(0, 0, 0);
    Vector3 previousPosition = new Vector3(0, 0, 0);

    // Use this for initialization
    void Start() {

        Controller.updateTileState += updateTileState;


        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
        

        floor = new Floor(height, width);

        // part of this code was ooperated on with Flatmate
        for (int x = 0; x < floor.height; x++)
        {
            for (int y = 0; y < floor.width; y++)
            {
                GameObject newTile = new GameObject();
                SpriteRenderer tilesRenderer = newTile.AddComponent<SpriteRenderer>();
                FloorTile currentTile = floor.getTileByCoord(x, y);

                RectTransform rectTransform = newTile.AddComponent<RectTransform>();
                rectTransform.localScale = rectTransform.localScale + new Vector3(0, -0.2f, 0);

                newTile.name = "FloorPanel~" + x + "~" + y;
                newTile.tag = "Floor Tile";

                newTile.transform.position = new Vector3(currentTile.yCoord, currentTile.xCoord - (0.2f * x), 0);
                

                tilesRenderer.sprite = carpetSprite;

                newTile.transform.SetParent(this.transform, true);
            }
        }
    }

    // Update is called once per frame
    void Update() {
       
        if (mainController.statusCode == 2 && toMoveX > -1 && toMoveY > -1)
        {
            #region PC movement
            if (Input.GetKeyDown(KeyCode.A))
            {               
                if (toMoveX >= 1)
                {
                    if (validMove)
                    {
                        validMove = checkValidity(toMoveX - 1, toMoveY, 1, fullHeight);
                    }
                    else
                    {
                        validMove = checkValidity(toMoveX - 1, toMoveY, fullWidth, fullHeight);
                    }
                
                    updateColumn(toMoveX + fullWidth - 1, toMoveY, false, -1);
                    updateColumn(toMoveX - 1, toMoveY, true, -1);
                    toMoveX--;
                }

            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (toMoveY > 0)
                {

                    if (validMove)
                    {
                        validMove = checkValidity(toMoveX, toMoveY - 1, fullWidth, 1);
                    }
                    else
                    {
                        validMove = checkValidity(toMoveX, toMoveY - 1, fullWidth, fullHeight);
                    }

                    // check validity
                    updateRow(toMoveX, toMoveY + fullHeight - 1, false, -1);
                    updateRow(toMoveX, toMoveY - 1, true, -1);
                    toMoveY--;
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (toMoveX < width - fullWidth)
                {
                    if (validMove)
                    {
                        validMove = checkValidity(toMoveX + fullWidth, toMoveY, 1, fullHeight);
                    }
                    else
                    {
                        validMove = checkValidity(toMoveX + 1, toMoveY, fullWidth, fullHeight);
                    }

                    // check validity
                    updateColumn(toMoveX, toMoveY, false, 1);
                    updateColumn(toMoveX + fullWidth, toMoveY, true, 1);
                    toMoveX++;
                }
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (toMoveY < height - fullHeight)
                {
                    if (validMove)
                    {
                        validMove = checkValidity(toMoveX, toMoveY + fullHeight, fullWidth, 1);
                    }
                    else
                    {
                        validMove = checkValidity(toMoveX, toMoveY + 1, fullWidth, fullHeight);
                    }

                    // check validity
                    updateRow(toMoveX, toMoveY, false, 1);
                    updateRow(toMoveX, toMoveY + fullHeight, true, 1);
                    toMoveY++;
                }
            }
            #endregion
         
            previousValidMove = validMove;
        }
    }

    public void doMove(int code)
    {
        if (code == 3)
        {
            if (toMoveX >= 1)
            {
                if (validMove)
                {
                    validMove = checkValidity(toMoveX - 1, toMoveY, 1, fullHeight);
                }
                else
                {
                    validMove = checkValidity(toMoveX - 1, toMoveY, fullWidth, fullHeight);
                }

                updateColumn(toMoveX + fullWidth - 1, toMoveY, false, -1);
                updateColumn(toMoveX - 1, toMoveY, true, -1);
                toMoveX--;
            }

        }
        if (code == 1)
        {
            if (toMoveY > 0)
            {

                if (validMove)
                {
                    validMove = checkValidity(toMoveX, toMoveY - 1, fullWidth, 1);
                }
                else
                {
                    validMove = checkValidity(toMoveX, toMoveY - 1, fullWidth, fullHeight);
                }

                // check validity
                updateRow(toMoveX, toMoveY + fullHeight - 1, false, -1);
                updateRow(toMoveX, toMoveY - 1, true, -1);
                toMoveY--;
            }
        }
        if (code == 2)
        {
            if (toMoveX < width - fullWidth)
            {
                if (validMove)
                {
                    validMove = checkValidity(toMoveX + fullWidth, toMoveY, 1, fullHeight);
                }
                else
                {
                    validMove = checkValidity(toMoveX + 1, toMoveY, fullWidth, fullHeight);
                }

                // check validity
                updateColumn(toMoveX, toMoveY, false, 1);
                updateColumn(toMoveX + fullWidth, toMoveY, true, 1);
                toMoveX++;
            }
        }
        if (code == 0)
        {
            if (toMoveY < height - fullHeight)
            {
                if (validMove)
                {
                    validMove = checkValidity(toMoveX, toMoveY + fullHeight, fullWidth, 1);
                }
                else
                {
                    validMove = checkValidity(toMoveX, toMoveY + 1, fullWidth, fullHeight);
                }

                // check validity
                updateRow(toMoveX, toMoveY, false, 1);
                updateRow(toMoveX, toMoveY + fullHeight, true, 1);
                toMoveY++;
            }
        }

        previousValidMove = validMove;
        
    }

    /*public void doMove(int code)
    {
        int xMove = 0, yMove = 0;
        int trueHeight = 0;
        int trueWidth = 0;
        int validWidth = 0;
        int validHeight = 0;
        int width2 = 0;
        int height2 = 0;

        #region calculate Direction
        switch (code)
        {
            case 0:
                xMove = 0;
                yMove = 1;
                trueWidth = 0;
                trueHeight = 1;
                validWidth = fullWidth;
                validHeight = 1;
                break;
            case 1:
                xMove = 0;
                yMove = -1;
                trueWidth = 0;
                trueHeight = fullHeight;
                validWidth = fullWidth;
                validHeight = 1;
                break;
            case 2:
                xMove = 1;
                yMove = 0;
                trueWidth = 1;
                trueHeight = 0;
                trueWidth = toMoveX + trueWidth - 1;
                validHeight = fullHeight;
                break;
            case 3:
                xMove = -1;
                yMove = 0;
                trueWidth = fullWidth;
                trueHeight = 0;
                validWidth = 1;
                validHeight = fullHeight;
                break;
        }
        int direction = xMove + yMove; 
        #endregion

        if ((toMoveX >= 1 && xMove == -1) || (toMoveY > 0 && yMove == -1) || ((toMoveY < height - fullHeight) && yMove == 1) || ((toMoveX < width - fullWidth) && xMove == 1))
        {
            if (validMove)
            {
                showOutput();
                validMove = checkValidity(toMoveX + xMove, toMoveY + yMove, validWidth, validHeight);
            }
            else
            {
                validMove = checkValidity(toMoveX + xMove, toMoveY + yMove, fullWidth, fullHeight);
            }

            updateColumn(, toMoveY + trueHeight, false, direction);
            updateColumn(toMoveX + (fullWidth), toMoveY + yMove + trueHeight, true, direction);
            toMoveX += xMove;
            toMoveY += yMove;
        }
        previousValidMove = validMove;

    }*/

    public bool checkValidity(int startX, int startY, int width, int height)
    {
        for (int i = startY; i < startY + height; i++)
        {
            for (int j = startX; j < startX + width; j++)
            {
                if (floor.floorTiles[i, j].inUse != 0) {
                    return false;
                }

            }
        }

        return true;
    }

    public void NewItemAdded(int x, int y)
    {
        validMove = checkValidity(x, y, fullWidth, fullHeight);
        Color col;
        if (validMove) { col = Color.green; mainController.confirmPanel.SetActive(true); } else { col = Color.red; mainController.confirmPanel.SetActive(false); }
        
        colourAllTiles(x, y, col);

    }

    void colourAllTiles(int startX, int startY, Color col)
    {        
        for (int i = startY; i < startY + fullHeight; i++)
        {
            for (int j = startX; j < startX + fullWidth; j++)
            {
                try
                {
                    mainController.floorTiles[i, j].GetComponent<SpriteRenderer>().color = col;
                }
                catch (Exception)
                {
                    Debug.Log("ERROR!!!! X = " + i + ", Y = " + j);
                }
            }
        }
    }

    void updateColumn(int x, int y, bool newState, int direction)
    {

        mainController.confirmPanel.SetActive(validMove);

        if (!newState) {
            for (int j = y; j < y + fullHeight; j++)
            {
                //if (!floor.floorTiles[x, j].inUse)
                //{
                    //floor.floorTiles[x, j].inUse = newState;
                //}

                mainController.floorTiles[j, x].GetComponent<SpriteRenderer>().color = mainController.carpetColour;
                //floor.floorTiles[x, j].inUse = false;            

            }
        }
        else {

            Color newColour;
            
            if (validMove)
            {
                newColour = Color.green;
            }
            else {
                newColour = Color.red;
            }


            if (validMove != previousValidMove)
            {  
                colourAllTiles(toMoveX + direction, toMoveY, newColour);
                // hide / show confirm button
            }
            else
            {
                for (int j = y; j < y + fullHeight; j++)
                {
                    //if (!floor.floorTiles[x, j].inUse)
                    //{
                        //floor.floorTiles[x, j].inUse = newState;
                    //}

                    mainController.floorTiles[j, x].GetComponent<SpriteRenderer>().color = newColour;

                }
            }
        }
        //}

    }

    void updateRow(int x, int y, bool newState, int direction)
    {
        mainController.confirmPanel.SetActive(validMove);

        if (!newState)
        {
            for (int i = x; i < x + fullWidth; i++)
            {
                //    for (int j = y; j < y + 15; j++)
                //{
                mainController.floorTiles[y, i].GetComponent<SpriteRenderer>().color = mainController.carpetColour;
                //}
            }
        }
        else {

            Color newColour;

            if (validMove)
            {
                newColour = Color.green;
            }
            else {
                newColour = Color.red;
            }


            if (validMove != previousValidMove)
            {
                colourAllTiles(toMoveX, toMoveY + direction, newColour);
                mainController.confirmPanel.SetActive(validMove);
            }
            else
            {
                for (int j = x; j < x + fullWidth; j++)
                {
                    //if (!floor.floorTiles[x, j].inUse)
                    //{
                    //floor.floorTiles[x, j].inUse = newState;
                    //}

                    mainController.floorTiles[y, j].GetComponent<SpriteRenderer>().color = newColour;

                }
            }
        }
        //}
    }

    public void updateTileState(int x, int y, int w, int h, int newState, bool complete)
    {
        for (int i = y; i < y + h; i++)
        {
            for (int j = x; j < x + w; j++)
            {

                Debug.Log(i + ", " + j);

                floor.floorTiles[i, j].inUse = newState;
                
                if (complete)
                {
                    mainController.floorTiles[i, j].GetComponent<SpriteRenderer>().color = mainController.carpetColour;
                }
            }
        }

        

        if (w == 11 && h == 15 && newState == 2)
        {
            for (int i = x; i < x + 11; i++)
            {
                floor.floorTiles[y, i].inUse = 1;
            }
            for (int i = y; i < y + 15; i++)
            {
                floor.floorTiles[i, x + 10].inUse = 1;
            }
        }

        showOutput();

    }

    void showOutput()
    {
        System.IO.StreamWriter file = new System.IO.StreamWriter("C:/Users/asuth/Documents/banana.txt", true);

        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                file.Write(floor.floorTiles[i, j].inUse + " ");
            }
            //Debug.Log(lineOutput);
            file.WriteLine();
        }

        file.Close();

    }

    
}
