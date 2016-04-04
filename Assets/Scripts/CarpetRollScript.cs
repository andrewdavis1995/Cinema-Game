using UnityEngine;
using System.Collections;
using System;

public class CarpetRollScript : MonoBehaviour {

    public GameObject carpet;

    public static CarpetRollScript current = new CarpetRollScript();

    Controller mainController;

    Color colour;
    Sprite[] texture;

    static bool shouldRun;

	// Use this for initialization
	void Start () {
        current = new CarpetRollScript();
        current.carpet = carpet;
	}

    public void Begin(Color c, Controller con, Sprite[] t)
    {
        //current = new CarpetRollScript();
        current.colour = c;
        current.mainController = con;
        current.texture = t;
        current.carpet.GetComponent<SpriteRenderer>().color = c;
        shouldRun = true;
        current.carpet.transform.position = new Vector3(-2.62f, 17.97f, 0);
        current.carpet.transform.localScale = new Vector3(5f, 24.42692f, 1);

        if (t.Length > 1)
        {
            current.carpet.SetActive(false);
        }
        else
        {
            current.carpet.SetActive(true);
        }

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (shouldRun)
        {
            carpet.transform.Translate(Time.deltaTime * 16f, 0, 0);
            carpet.transform.localScale = carpet.transform.lossyScale - new Vector3(Time.deltaTime * 0.8f, 0, 0);

            if (carpet.transform.position.x > 80)
            {
                shouldRun = false;
            }

            int x = (int)(carpet.transform.position.x);

            for (int i = 0; i < TileManager.floor.height; i++)
            {

                int index = UnityEngine.Random.Range(0, current.texture.Length);

                try
                {
                    current.mainController.floorTiles[i, x].GetComponent<SpriteRenderer>().color = current.colour;

                    current.mainController.floorTiles[i, x].GetComponent<SpriteRenderer>().sprite = current.texture[index];
                    
                }
                catch (Exception)
                {
                    Debug.Log(i + " " + x);
                }
            }

        }
	}
}
