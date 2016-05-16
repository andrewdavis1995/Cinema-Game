using UnityEngine;
using System.Collections;

public class DisplaySave : MonoBehaviour {

	static bool shouldRun = false;
    static int count = 0;

    static GameObject icon;

    void Start()
    {
        icon = GameObject.Find("imgSaved");
        icon.SetActive(false);
    }

    public static void Begin()
    {
        count = 0;
        shouldRun = true;
        //icon.SetActive(true);
    }

    int prevState = 0;
    int currState = 0;

	// Update is called once per frame
	void Update ()
    {

        prevState = currState;
        currState = Controller.saveState;

        if (prevState == 0 && currState == 1)
        {
            icon.SetActive(true);
        }

        if (shouldRun)
        {
            if (Controller.saveState == 1)
            {
                count++;
            }

            if (count > 250)
            {
                shouldRun = false;
                count = 0;
                icon.SetActive(false);
                Controller.saveState = 2;
            }
        }
	}
}
