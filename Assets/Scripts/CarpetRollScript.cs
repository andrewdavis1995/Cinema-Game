using UnityEngine;
using System.Collections;
using System;

public class CarpetRollScript : MonoBehaviour {

    public GameObject carpet;

    public static CarpetRollScript current = new CarpetRollScript();

    Controller mainController;

    Color colour;

    static bool shouldRun;

	// Use this for initialization
	void Start () {
        current.carpet = carpet;
	}

    public void Begin(Color c, Controller con)
    {
        current.colour = c;
        current.mainController = con;
        carpet.GetComponent<SpriteRenderer>().color = c;
        shouldRun = true;
        carpet.transform.position = new Vector3(-2.62f, 17.97f, 0);
        carpet.transform.localScale = new Vector3(5f, 24.42692f, 1);
    }
	
	// Update is called once per frame
	void Update () {
        if (shouldRun)
        {
            carpet.transform.Translate(Time.deltaTime * 16f, 0, 0);
            carpet.transform.localScale = carpet.transform.lossyScale - new Vector3(Time.deltaTime * 0.8f, 0, 0);

            if (carpet.transform.position.x > 80)
            {
                shouldRun = false;
            }

            int x = (int)(carpet.transform.position.x);

            for (int i = 0; i < current.mainController.theTileManager.floor.height; i++)
            {
                try
                {
                    current.mainController.floorTiles[i, x].GetComponent<SpriteRenderer>().color = current.colour;
                }
                catch (Exception)
                {
                    Debug.Log(i + " " + x);
                }
            }

        }
	}
}
