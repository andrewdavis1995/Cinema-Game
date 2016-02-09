using UnityEngine;
using System.Collections;
using Assets.Classes;
using System;

public class TileManager : MonoBehaviour {

    private Floor floor;
    public Sprite carpetSprite;

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

    // Use this for initialization
    void Start() {

        Controller.updateTileState += updateTileState;

        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();

        floor = new Floor(width, height);

        // part of this code was ooperated on with Flatmate
        for (int x = 0; x < floor.width; x++)
        {
            for (int y = 0; y < floor.height; y++)
            {
                GameObject newTile = new GameObject();
                SpriteRenderer tilesRenderer = newTile.AddComponent<SpriteRenderer>();
                FloorTile currentTile = floor.getTileByCoord(x, y);

                RectTransform rectTransform = newTile.AddComponent<RectTransform>();
                rectTransform.localScale = rectTransform.localScale + new Vector3(0, -0.2f, 0);

                newTile.name = "FloorPanel~" + x + "~" + y;
                newTile.tag = "Floor Tile";

                newTile.transform.position = new Vector3(currentTile.xCoord, currentTile.yCoord - (0.2f * y), 0);

                //tilesRenderer.color = new Color(255, 0, 0, 100);


                tilesRenderer.sprite = carpetSprite;

                newTile.transform.SetParent(this.transform, true);
            }
        }
    }

    // Update is called once per frame
    void Update() {
       
        if (mainController.statusCode == 2 && toMoveX > -1 && toMoveY > -1)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {               
                if (toMoveX >= 1)
                {
                    if (validMove)
                    {
                        showOutput();
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
                        showOutput();
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
                        showOutput();
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

            if (!validMove)
            {
                // hide the confirm button
            }
        
            previousValidMove = validMove;

        }
    }

    public bool checkValidity(int startX, int startY, int width, int height)
    {
        for (int i = startX; i < startX + width; i++)
        {
            for (int j = startY; j < startY + height; j++)
            {
                if (floor.floorTiles[i, j].inUse) {
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
        if (validMove) { col = Color.green; } else { col = Color.red; }
        
        colourAllTiles(x, y, col);

    }

    void colourAllTiles(int startX, int startY, Color col)
    {
        
        for (int i = startX; i < startX + fullWidth; i++)
        {
            for (int j = startY; j < startY + fullHeight; j++)
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

        if (!newState) {
            for (int j = y; j < y + fullHeight; j++)
            {
                //if (!floor.floorTiles[x, j].inUse)
                //{
                    //floor.floorTiles[x, j].inUse = newState;
                //}

                mainController.floorTiles[x, j].GetComponent<SpriteRenderer>().color = mainController.carpetColour;
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
                mainController.confirmPanel.SetActive(validMove);
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

                    mainController.floorTiles[x, j].GetComponent<SpriteRenderer>().color = newColour;

                }
            }
        }
        //}

    }

    void updateRow(int x, int y, bool newState, int direction)
    {
        if (!newState)
        {
            for (int i = x; i < x + fullWidth; i++)
            {
                //    for (int j = y; j < y + 15; j++)
                //{
                mainController.floorTiles[i, y].GetComponent<SpriteRenderer>().color = mainController.carpetColour;
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

                    mainController.floorTiles[j, y].GetComponent<SpriteRenderer>().color = newColour;

                }
            }
        }
        //}
    }

    public void updateTileState(int x, int y, bool newState, bool complete)
    {
        for (int i = x; i < x + fullWidth; i++)
        {
            for (int j = y; j < y + fullHeight; j++)
            {
                floor.floorTiles[i, j].inUse = newState;
                
                if (complete)
                {
                    mainController.floorTiles[i, j].GetComponent<SpriteRenderer>().color = mainController.carpetColour;
                }
            }
        }

        showOutput();

    }

    void showOutput()
    {
        for (int i = 0; i < width; i++)
        {
            string lineOutput = i.ToString();
            for (int j = 0; j < height; j++)
            {
                if (!floor.floorTiles[i, j].inUse)
                {
                    lineOutput += '-';
                }
                else
                {
                    lineOutput += '@';
                }
            }
            Debug.Log(lineOutput);
        }
    }

    
}
