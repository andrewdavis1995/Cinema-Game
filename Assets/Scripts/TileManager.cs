using UnityEngine;
using System.Collections;
using Assets.Classes;

public class TileManager : MonoBehaviour {

    private Floor floor;
    public Sprite carpetSprite;

	// Use this for initialization
	void Start () {

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
               
                tilesRenderer.color = new Color(255, 0, 0, 100); 
                

                tilesRenderer.sprite = carpetSprite;

                newTile.transform.SetParent(this.transform, true);
                //currentTile.RegisterCallback((tile) => { OnTileTypeChanged(tile, newTile); });
                //currentTile.tileObject = newTile;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
