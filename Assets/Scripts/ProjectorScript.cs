using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProjectorScript : MonoBehaviour
{

    ScreenObject theScreen;
    public Controller mainController;
    public Transform projector;
    public bool runGeneration;
    public static int numVisible = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (runGeneration && mainController.simulationRunning)
        {
            for (int i = 0; i < Controller.theScreens.Count; i++)
            {
                if (!Controller.theScreens[i].ConstructionInProgress() && Controller.theScreens[i].GetClicksRemaining() < 1)
                {
                    int upperRange = 6000;
                    upperRange += (Controller.theScreens[i].GetUpgradeLevel() - 1) * 4000;

                    if (upperRange == 18000)
                    {
                        upperRange = 20000;
                    }

                    int randomValue = Random.Range(0, upperRange);

                    if (randomValue == 0)
                    {
                        CreateNew(i);
                        Controller.theScreens[i].ProjectorBroke();
                    }
                }
            }
        }

    }

    void CreateNew(int index)
    {
        #region Create a projector icon
        Vector3 position = mainController.screenObjectList[index].transform.position + new Vector3(5.16f, 8.76f, -1);
        numVisible++;

        GameObject proj = Instantiate(projector.gameObject, position, Quaternion.identity) as GameObject;
        proj.name = "Projector#" + (index + 1);
        #endregion

        // display options to clear all
        mainController.pnlClearProjectors.SetActive(true);
        Text[] texts = mainController.pnlClearProjectors.GetComponentsInChildren<Text>();

        texts[1].text = numVisible.ToString();

    }

    public void ClearAll()
    {
        // 1 = cost
        // 2 = currency
        ConfirmationScript.OptionSelected(7, new string[] { "Fix all broken projectors?", numVisible.ToString(), "1" }, "This will cost: ");
    }

    void OnMouseUp()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    void OnMouseDown()
    {
        transform.localScale = new Vector3(1.4f, 1.4f, 1);

        // get the id of the thing
        int id = int.Parse(gameObject.name.Split('#')[1]);

        // get the current clicks
        int prevClicks = Controller.theScreens[id - 1].GetClicksRemaining();

        // update the clicks
        int remaining = Controller.theScreens[id - 1].ProjectorClicked();

        if (prevClicks == 1)
        {
            // fixed!
            Destroy(gameObject);

            numVisible--;

            // update the label
            GameObject thePanel = GameObject.Find("cmdFixProjectors");
            Text[] texts = thePanel.GetComponentsInChildren<Text>();
            texts[1].text = numVisible.ToString();

            if (numVisible < 1)
            {
                GameObject.Find("cmdFixProjectors").SetActive(false);
            }

        }

    }

}