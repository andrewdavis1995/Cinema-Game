using UnityEngine;
using System.Collections;
using Assets.Classes;

public class TileManager : MonoBehaviour {

    private Floor floor;
    public Sprite carpetSprite;

    public int toMoveX = -1;
    public int toMoveY = -1;

    public int origX = -1;
    public int origY = -1;

    Controller mainController;

    // Use this for initialization
    void Start() {

        Controller.updateTileState += updateTileState;

        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();

        floor = new Floor(80, 40);

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
                //currentTile.RegisterCallback((tile) => { OnTileTypeChanged(tile, newTile); });
                //currentTile.tileObject = newTile;
            }
        }
    }

    // Update is called once per frame
    void Update() {
       
        if (mainController.statusCode == 2 && toMoveX > -1 && toMoveY > -1)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                updateColumn(toMoveX + 9, toMoveY, false);
                updateColumn(toMoveX - 1, toMoveY, true);
                toMoveX--;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                updateRow(toMoveX, toMoveY + 14, false);
                updateRow(toMoveX, toMoveY - 1, true);
                toMoveY--;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                updateColumn(toMoveX, toMoveY, false);
                updateColumn(toMoveX + 10, toMoveY, true);
                toMoveX++;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                updateRow(toMoveX, toMoveY, false);
                updateRow(toMoveX, toMoveY + 15, true);
                toMoveY++;
            }
        }
        else
        {
            Debug.Log("NOT IN MOVE MENU");
        }
    }

    void updateColumn(int x, int y, bool newState)
    {
        Color newColour;
        if (newState) { newColour = Color.green; } else { newColour = mainController.carpetColour; }

        //for (int i = x; i < 10; i++)
        //{
            for (int j = y; j < y + 15; j++)
            {
            if (!floor.floorTiles[x, j].inUse)
            {
                floor.floorTiles[x, j].inUse = newState;
                mainController.floorTiles[x, j].GetComponent<SpriteRenderer>().color = newColour;
            }
            else
            {
                mainController.floorTiles[x, j].GetComponent<SpriteRenderer>().color = newColour;
            }
        }
        //}
    }

    void updateRow(int x, int y, bool newState)
    {
        Color newColour;
        if (newState) { newColour = Color.green; } else { newColour = mainController.carpetColour; }

        for (int i = x; i < x + 10; i++)
        {
        //    for (int j = y; j < y + 15; j++)
        //{
            floor.floorTiles[i, y].inUse = newState;
            mainController.floorTiles[i, y].GetComponent<SpriteRenderer>().color = newColour;
        //}
        }
    }

    public void updateTileState(int x, int y, bool newState, bool complete)
    {
        for (int i = x; i < x + 10; i++)
        {
            for (int j = y; j < y + 15; j++)
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
        for (int i = 0; i < 80; i++)
        {
            string lineOutput = i.ToString();
            for (int j = 0; j < 40; j++)
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
