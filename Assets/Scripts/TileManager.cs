using UnityEngine;
using System.Collections;
using Assets.Classes;
using System;
using System.IO;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{

    public static Floor floor;
    public Sprite carpetSprite;

    GameObject movementPanel;

    bool validMove = true;
    bool previousValidMove = true;

    public int toMoveX = -1;
    public int toMoveY = -1;

    int code;

    public int origX = -1;
    public int origY = -1;

    const int width = 80;
    const int height = 40;

    public int fullWidth = -1;
    public int fullHeight = -1;

    static Controller mainController;
    public Popup_Controller popupController;

    Vector3 currentPosition = new Vector3(0, 0, 0);
    Vector3 previousPosition = new Vector3(0, 0, 0);

    // Use this for initialization
    void Start()
    {

        Controller.updateTileState += UpdateTileState;


        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();

        floor = new Floor(height, width);

        // part of this code was cooperated on with Flatmate
        for (int x = 0; x < floor.height; x++)
        {
            for (int y = 0; y < floor.width; y++)
            {
                GameObject newTile = new GameObject();
                SpriteRenderer tilesRenderer = newTile.AddComponent<SpriteRenderer>();
                FloorTile currentTile = floor.GetTileByCoord(x, y);

                //rectTransform.localScale = rectTransform.localScale - new Vector3(0, 0.2f, 0);

                newTile.name = "FloorPanel~" + x + "~" + y;
                newTile.tag = "Floor Tile";

                newTile.transform.position = new Vector3(currentTile.yCoord, currentTile.xCoord * 0.8f, 0);

                tilesRenderer.sprite = carpetSprite;

                newTile.transform.SetParent(this.transform, true);
                RectTransform rectTransform = newTile.AddComponent<RectTransform>();
                rectTransform.rect.Set(rectTransform.rect.xMin, rectTransform.rect.xMax, 1, 2.8f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (mouseDown && code > -1)
        {
            mouseDownCount++;
            if (mouseDownCount > 10)
            {
                DoMove(code);
                mouseDownCount = 0;
            }
        }

        if (mainController.statusCode == 2 && toMoveX > -1 && toMoveY > -1)
        {
            #region PC movement
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (toMoveX >= 1)
                {
                    if (validMove)
                    {
                        validMove = CheckValidity(toMoveX - 1, toMoveY, 1, fullHeight);
                    }
                    else
                    {
                        validMove = CheckValidity(toMoveX - 1, toMoveY, fullWidth, fullHeight);
                    }

                    UpdateColumn(toMoveX + fullWidth - 1, toMoveY, false, -1);
                    UpdateColumn(toMoveX - 1, toMoveY, true, -1);
                    toMoveX--;
                }

            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (toMoveY > 0)
                {

                    if (validMove)
                    {
                        validMove = CheckValidity(toMoveX, toMoveY - 1, fullWidth, 1);
                    }
                    else
                    {
                        validMove = CheckValidity(toMoveX, toMoveY - 1, fullWidth, fullHeight);
                    }

                    // check validity
                    UpdateRow(toMoveX, toMoveY + fullHeight - 1, false, -1);
                    UpdateRow(toMoveX, toMoveY - 1, true, -1);
                    toMoveY--;
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (toMoveX < width - fullWidth)
                {
                    if (validMove)
                    {
                        validMove = CheckValidity(toMoveX + fullWidth, toMoveY, 1, fullHeight);
                    }
                    else
                    {
                        validMove = CheckValidity(toMoveX + 1, toMoveY, fullWidth, fullHeight);
                    }

                    // check validity
                    UpdateColumn(toMoveX, toMoveY, false, 1);
                    UpdateColumn(toMoveX + fullWidth, toMoveY, true, 1);
                    toMoveX++;
                }
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (toMoveY < height - fullHeight)
                {
                    if (validMove)
                    {
                        validMove = CheckValidity(toMoveX, toMoveY + fullHeight, fullWidth, 1);
                    }
                    else
                    {
                        validMove = CheckValidity(toMoveX, toMoveY + 1, fullWidth, fullHeight);
                    }

                    // check validity
                    UpdateRow(toMoveX, toMoveY, false, 1);
                    UpdateRow(toMoveX, toMoveY + fullHeight, true, 1);
                    toMoveY++;
                }
            }
            #endregion

            previousValidMove = validMove;
        }
    }

    public List<int> IsPathAvailable()
    {
        List<int> unreachableScreens = new List<int>();

        // loop through all screens
        for (int i = 0; i < ShopController.theScreens.Count; i++)
        {
            // check that it is possible to reach them from the start point - i.e. ticket office
            int screenX = ShopController.theScreens[i].GetX() + 5;
            int screenY = ShopController.theScreens[i].GetY();

            List<Coordinate> path = floor.FindPath(40, 5, screenX, screenY);

            // if it finds one that isn't reachable, set bool to false and break
            if (path == null)
            {
                unreachableScreens.Add(ShopController.theScreens[i].GetScreenNumber());
            }
        }

        return unreachableScreens;
    }

    public void DoMove(int code)
    {
        if (code == 3)
        {
            if (toMoveX >= 1)
            {
                if (validMove)
                {
                    validMove = CheckValidity(toMoveX - 1, toMoveY, 1, fullHeight);
                }
                else
                {
                    validMove = CheckValidity(toMoveX - 1, toMoveY, fullWidth, fullHeight);
                }
                
                UpdateColumn(toMoveX + fullWidth - 1, toMoveY, false, -1);
                UpdateColumn(toMoveX - 1, toMoveY, true, -1);
                
                toMoveX--;
            }

        }
        if (code == 1)
        {
            if (toMoveY > 0)
            {

                if (validMove)
                {
                    validMove = CheckValidity(toMoveX, toMoveY - 1, fullWidth, 1);
                }
                else
                {
                    validMove = CheckValidity(toMoveX, toMoveY - 1, fullWidth, fullHeight);
                }
                
                UpdateRow(toMoveX, toMoveY + fullHeight - 1, false, -1);
                UpdateRow(toMoveX, toMoveY - 1, true, -1);
                
                toMoveY--;
            }
        }
        if (code == 2)
        {
            if (toMoveX < width - fullWidth)
            {
                if (validMove)
                {
                    validMove = CheckValidity(toMoveX + fullWidth, toMoveY, 1, fullHeight);
                }
                else
                {
                    validMove = CheckValidity(toMoveX + 1, toMoveY, fullWidth, fullHeight);
                }
                

                UpdateColumn(toMoveX, toMoveY, false, 1);
                UpdateColumn(toMoveX + fullWidth, toMoveY, true, 1);

                toMoveX++;
            }
        }
        if (code == 0)
        {
            if (toMoveY < height - fullHeight)
            {
                if (validMove)
                {
                    validMove = CheckValidity(toMoveX, toMoveY + fullHeight, fullWidth, 1);
                }
                else
                {
                    validMove = CheckValidity(toMoveX, toMoveY + 1, fullWidth, fullHeight);
                }
                

                UpdateRow(toMoveX, toMoveY, false, 1);
                UpdateRow(toMoveX, toMoveY + fullHeight, true, 1);
                toMoveY++;
            }

        }

        previousValidMove = validMove;

    }

    public bool CheckValidity(int startX, int startY, int width, int height)
    {
        for (int i = startY; i < startY + height; i++)
        {
            for (int j = startX; j < startX + width; j++)
            {
                if (floor.floorTiles[i, j].inUse != 0)
                {
                    return false;
                }

            }
        }

        return true;
    }

    public void NewItemAdded(int x, int y)
    {
        validMove = CheckValidity(x, y, fullWidth, fullHeight);
        Color col;
        if (validMove) { col = Color.green; popupController.confirmPanel.SetActive(true); } else { col = Color.red; popupController.confirmPanel.SetActive(false); }

        ColourAllTiles(x, y, col);

    }

    public void ColourAllTiles(int startX, int startY, Color col)
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
                }
            }
        }
    }

    void UpdateColumn(int x, int y, bool newState, int direction)
    {

        popupController.confirmPanel.SetActive(validMove);

        if (!newState)
        {
            for (int j = y; j < y + fullHeight; j++)
            {
                // IMAGE CHANGE HERE
                // MARBLE CHECK HERE
                mainController.floorTiles[j, x].GetComponent<SpriteRenderer>().color = mainController.carpetColour;
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
                ColourAllTiles(toMoveX + direction, toMoveY, newColour);
                // hide / show confirm button
            }
            else
            {
                for (int j = y; j < y + fullHeight; j++)
                {
                    // IMAGE CHANGE HERE
                    mainController.floorTiles[j, x].GetComponent<SpriteRenderer>().color = newColour;
                }
            }
        }
    }

    public void ResetStatusVariables()
    {
        origX = -1;
        origY = -1;
        toMoveX = -1;
        toMoveY = -1;
        fullWidth = -1;
        fullHeight = -1;
    }

    void UpdateRow(int x, int y, bool newState, int direction)
    {
        popupController.confirmPanel.SetActive(validMove);

        if (!newState)
        {
            for (int i = x; i < x + fullWidth; i++)
            {
                // IMAGE CHANGE HERE      
                // MARBLE CHECK HERE     
                mainController.floorTiles[y, i].GetComponent<SpriteRenderer>().color = mainController.carpetColour;     
            }
        }
        else {

            Color newColour;


            // IMAGE CHANGE HERE
            if (validMove)
            {
                newColour = Color.green;
            }
            else {
                newColour = Color.red;
            }


            if (validMove != previousValidMove)
            {
                ColourAllTiles(toMoveX, toMoveY + direction, newColour);
                popupController.confirmPanel.SetActive(validMove);
            }
            else
            {
                for (int j = x; j < x + fullWidth; j++)
                {
                    // IMAGE CHANGE HERE
                    mainController.floorTiles[y, j].GetComponent<SpriteRenderer>().color = newColour;

                }
            }
        }
        //}
    }

    public void UpdateTileState(int x, int y, int w, int h, int newState, bool complete)
    {
        for (int i = y; i < y + h; i++)
        {
            for (int j = x; j < x + w; j++)
            {
                floor.floorTiles[i, j].inUse = newState;

                if (complete)
                {
                    // IMAGE CHANGE HERE
                    mainController.floorTiles[i, j].GetComponent<SpriteRenderer>().color = mainController.carpetColour;
                }
            }
        }



        if (w == 10 && h == 18 && newState == 2)
        {
            for (int i = y; i < y + 6; i++)
            {
                for (int j = x; j < x + 10; j++)
                {
                    floor.floorTiles[i, j].inUse = 1;
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

        ShowOutput();

    }

    /// <summary>
    /// print to a file to check tile states
    /// </summary>
    public void ShowOutput()
    {
        try
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("C:/Users/asuth/Documents/banana.txt", true);

            for (int i = height - 1; i >= 0; i--)
            {
                for (int j = 0; j < width; j++)
                {
                    file.Write(floor.floorTiles[i, j].inUse + " ");
                }
                // (lineOutput);
                file.WriteLine();
            }

            file.Close();
        }
        catch (Exception) { }
    }

    bool mouseDown = false;
    int mouseDownCount = 0;

    public void OnPointerDown(int i)
    {
        code = i;
        mouseDown = true;
    }
    public void OnPointerUp()
    {
        code = -1;
        mouseDownCount = 0;
        mouseDown = false;
    }
          

}
