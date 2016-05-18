 using UnityEngine;
using System.Collections;
using System;

public class CarpetRollScript : MonoBehaviour {

    public GameObject carpet;   // the carpet image

    Controller mainController;      // the instance of Controller to use

    Color colour;       // the colour to make the carpet
    Sprite[] texture;   // the texture to make the floor (carpet or marble)

    public bool shouldRun;      // whether or not the animation should run


    /// <summary>
    /// Start the animation
    /// </summary>
    /// <param name="c">The colour to make the carpet</param>
    /// <param name="con">The controller which uses the animation</param>
    /// <param name="t">The texture to make the floor</param>
    public void Begin(Color c, Controller con, Sprite[] t)
    {
        // set the values for colour, texture and Controller
        colour = c;
        mainController = con;
        texture = t;
        shouldRun = true;

        // change the colour of the carpet image
        carpet.GetComponent<SpriteRenderer>().color = c;
        carpet.transform.position = new Vector3(-2.62f, 17.97f, 0);
        carpet.transform.localScale = new Vector3(5f, 24f, 1);

        // if the user has selected marble floor, hide the carpet animation image
        if (t.Length > 1)
        {
            carpet.SetActive(false);
        }
        else
        {
            carpet.SetActive(true);
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
                carpet.transform.Translate(Time.deltaTime * 16f, 0, 0);
                carpet.transform.localScale = carpet.transform.lossyScale - new Vector3(Time.deltaTime * 0.8f, 0, 0);
            }
            catch (Exception) { }

            // stop the animation when the carpet reaches the edge of the screen
            if (carpet.transform.position.x > 80)
            {
                shouldRun = false;
                carpet.SetActive(false);
            }

            // update the image of all tiles
            int x = (int)(carpet.transform.position.x);

            for (int i = 0; i < TileManager.floor.height; i++)
            {
                // generate a random index for the texture to use (for marble purposes)
                int index = UnityEngine.Random.Range(0, texture.Length);

                try
                {
                    mainController.floorTiles[i, x].GetComponent<SpriteRenderer>().color = colour;
                    mainController.floorTiles[i, x].GetComponent<SpriteRenderer>().sprite = texture[index];
                    
                }
                catch (Exception) { }
            }

        }
	}

    public void FinishPlacement()
    {
        int x = (int)(carpet.transform.position.x) - 1;

        for (int i = x; i < 80; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                // generate a random index for the texture to use (for marble purposes)
                int index = UnityEngine.Random.Range(0, texture.Length);

                try
                {
                    if (i > -1)
                    {
                        mainController.floorTiles[j, i].GetComponent<SpriteRenderer>().color = colour;
                        mainController.floorTiles[j, i].GetComponent<SpriteRenderer>().sprite = texture[index];
                    }
                }
                catch (Exception) { }
            }
        }

    }

}
