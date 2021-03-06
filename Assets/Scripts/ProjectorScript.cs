﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProjectorScript : MonoBehaviour
{

    public GameObject pnlClearProjectors;

    public Controller mainController;   // the instance of Controller to use
    public Transform projector;     // the prefab of the projector icon (clickable)
    public static int numVisible = 0;   // how many projectors are visible
    public bool runGeneration;

    /// <summary>
    /// Run the loop
    /// </summary>
    void FixedUpdate()
    {
        // if the loop should run...
        if (runGeneration && mainController.simulationRunning)
        {
            // for each of the screens...
            for (int i = 0; i < ShopController.theScreens.Count; i++)
            {
                // if the screen is suitable for having its projector broken
                if (!ShopController.theScreens[i].ConstructionInProgress() && ShopController.theScreens[i].GetClicksRemaining() < 1)
                {
                    // get an upper bound for the random number generation
                    int upperRange = 6000;
                    upperRange += (ShopController.theScreens[i].GetUpgradeLevel() - 1) * 4000;

                    if (upperRange == 18000)
                    {
                        upperRange = 20000;
                    }

                    // generate a random number from 0 to upper bound
                    int randomValue = Random.Range(0, upperRange);

                    // if that number is 0, break the screen
                    if (randomValue == 0)
                    {
                        // create a new projector icon to click
                        ShopController.theScreens[i].ProjectorBroke();
                        CreateNew(i);
                    }
                }
            }
        }

    }

    /// <summary>
    /// Create a new Projector icon
    /// </summary>
    /// <param name="index">The screen to create the icon for</param>
    void CreateNew(int index)
    {
        #region Create a projector icon
        Vector3 position = mainController.shopController.screenObjectList[index].transform.position + new Vector3(5.16f, 8.76f, -1);
        numVisible++;

        GameObject proj = Instantiate(projector.gameObject, position, Quaternion.identity) as GameObject;
        proj.name = "Projector#" + (index + 1);
        #endregion

        // display options to clear all
        pnlClearProjectors.SetActive(true);
        Text[] texts = pnlClearProjectors.GetComponentsInChildren<Text>();

        texts[1].text = numVisible.ToString();

    }

    /// <summary>
    /// Clear all of the projector icons
    /// </summary>
    public void ClearAll()
    {
        // confirm that they want to clear all (spend money)
        ConfirmationScript.OptionSelected(7, new string[] { "Fix all broken projectors?", numVisible.ToString(), "1" }, "This will cost: ");
    }

    /// <summary>
    /// When the mouse is released
    /// </summary>
    void OnMouseUp()
    {
        // return the icon to its normal size
        transform.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// When the mouse is clicked down
    /// </summary>
    void OnMouseDown()
    {
        // make the icon bigger
        transform.localScale = new Vector3(1.4f, 1.4f, 1);

        // get the id of the icon
        int id = int.Parse(gameObject.name.Split('#')[1]);

        // get the current clicks remaining for the associated screen
        int prevClicks = ShopController.theScreens[id - 1].GetClicksRemaining();

        
        // update the clicks
        int remaining = ShopController.theScreens[id - 1].ProjectorClicked();

        // if that was the last click... FIXED!!!
        if (prevClicks == 1)
        {
            // destroy the icon
            Destroy(gameObject);

            // reduce the number of icons visible by 1
            numVisible--;

            // update the label for cleat all
            GameObject thePanel = GameObject.Find("cmdFixProjectors");
            Text[] texts = thePanel.GetComponentsInChildren<Text>();
            texts[1].text = numVisible.ToString();

            // hide the clear all label if no projectors are broken
            if (numVisible < 1)
            {
                GameObject.Find("cmdFixProjectors").SetActive(false);
            }

        }

    }


    /// <summary>
    /// Clear all the projector icons from the screen
    /// </summary>
    public void ClearAllProjectors()
    {
        GameObject[] projectors = GameObject.FindGameObjectsWithTag("Projector");

        for (int i = 0; i < projectors.Length; i++)
        {
            Destroy(projectors[i]);
        }

        for (int i = 0; i < ShopController.theScreens.Count; i++)
        {
            ShopController.theScreens[i].ResetClicks();
        }

        // clear the panel
        pnlClearProjectors.SetActive(false);

        ProjectorScript.numVisible = 0;

    }


}