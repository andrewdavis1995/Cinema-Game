 using UnityEngine;
using System.Collections;
using System;

public class CarpetRollScript : MonoBehaviour {

    public GameObject carpet;   // the carpet image

    public static CarpetRollScript current;     // static instance of the class for accessibility
    Controller mainController;      // the instance of Controller to use

    Color colour;       // the colour to make the carpet
    Sprite[] texture;   // the texture to make the floor (carpet or marble)

    public static bool shouldRun;      // whether or not the animation should run

	// Use this for initialization
	void Start () {
        // setup the static variable
        current = this;
        current.carpet = carpet;
    }

    /// <summary>
    /// Start the animation
    /// </summary>
    /// <param name="c">The colour to make the carpet</param>
    /// <param name="con">The controller which uses the animation</param>
    /// <param name="t">The texture to make the floor</param>
    public void Begin(Color c, Controller con, Sprite[] t)
    {
        // set the values for colour, texture and Controller
        current.colour = c;
        current.mainController = con;
        current.texture = t;
        shouldRun = true;

        // change the colour of the carpet image
        current.carpet.GetComponent<SpriteRenderer>().color = c;
        current.carpet.transform.position = new Vector3(-2.62f, 17.97f, 0);
        current.carpet.transform.localScale = new Vector3(5f, 24f, 1);

        // if the user has selected marble floor, hide the carpet animation image
        if (t.Length > 1)
        {
            current.carpet.SetActive(false);
        }
        else
        {
            current.carpet.SetActive(true);
        }

    }
	
    /// <summary>
    /// Do the animation
    /// </summary>
	void FixedUpdate () {

        // if the animation should run...
        if (shouldRun)
        {
            // move the carpet image
            try
            {
                current.carpet.transform.Translate(Time.deltaTime * 16f, 0, 0);
                current.carpet.transform.localScale = carpet.transform.lossyScale - new Vector3(Time.deltaTime * 0.8f, 0, 0);
            }
            catch (Exception) { }

            // stop the animation when the carpet reaches the edge of the screen
            if (current.carpet.transform.position.x > 80)
            {
                shouldRun = false;
            }

            // update the image of all tiles
            int x = (int)(current.carpet.transform.position.x);

            for (int i = 0; i < TileManager.floor.height; i++)
            {
                // generate a random index for the texture to use (for marble purposes)
                int index = UnityEngine.Random.Range(0, current.texture.Length);

                try
                {
                    current.mainController.floorTiles[i, x].GetComponent<SpriteRenderer>().color = current.colour;
                    current.mainController.floorTiles[i, x].GetComponent<SpriteRenderer>().sprite = current.texture[index];
                    
                }
                catch (Exception) { }
            }

        }
	}

    public void FinishPlacement()
    {
        int x = (int)(current.carpet.transform.position.x) - 1;

        for (int i = x; i < 80; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                // generate a random index for the texture to use (for marble purposes)
                int index = UnityEngine.Random.Range(0, current.texture.Length);

                try
                {
                    current.mainController.floorTiles[j, i].GetComponent<SpriteRenderer>().color = current.colour;
                    current.mainController.floorTiles[j, i].GetComponent<SpriteRenderer>().sprite = current.texture[index];
                }
                catch (Exception) { }
            }
        }

    }

}
