using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Classes;

public class ShopController : MonoBehaviour
{
    public bool[] postersUnlocked = new bool[2];
    public Sprite[] screenImages;

    public Transform plantPrefab;
    public Transform bustPrefab;
    public Transform vendingMachinePrefab;
    public Transform foodAreaPrefab;

    public GameObject redCarpet;
    public bool hasUnlockedRedCarpet = false;
    
    public Transform screenPrefab;
    public Transform builderPrefab;

    public List<GameObject> gameObjectList = new List<GameObject>();
    public List<GameObject> screenObjectList = new List<GameObject>();

    public static List<ScreenObject> theScreens = new List<ScreenObject>();
    public static List<OtherObject> otherObjects = new List<OtherObject>();


    /// <summary>
    /// Unlock a pack of posters
    /// </summary>
    /// <param name="index">The index of the poster pack (0 or 1)</param>
    public void UnlockPosterPack(int index)
    {
        postersUnlocked[index] = true;

        GameObject[] allPosters = GameObject.FindGameObjectsWithTag("Poster");

        for (int i = 0; i < allPosters.Length; i++)
        {
            if (i % 2 == index)
            {
                SpriteRenderer sr = allPosters[i].GetComponent<SpriteRenderer>();
                sr.enabled = true;
            }
        }
    }

    /// <summary>
    /// Loads the values for red carpet and posters
    /// </summary>
    /// <param name="rC"></param>
    /// <param name="pos"></param>
    public void LoadDecorations(bool rC, bool[] pos)
    {
        hasUnlockedRedCarpet = rC;
        postersUnlocked = pos;
        
        if (hasUnlockedRedCarpet)
        {
            redCarpet.SetActive(true);
        }
    }

    public void ShowPosters(int index)
    {
        // show relevant posters
        if (postersUnlocked[index])
        {
            GameObject[] allPosters = GameObject.FindGameObjectsWithTag("Poster");

            for (int i = 0; i < allPosters.Length; i++)
            {
                if (i % 2 == index)
                {
                    SpriteRenderer sr = allPosters[i].GetComponent<SpriteRenderer>();
                    sr.enabled = true;
                }
            }
        }
    }

    public void UpgradeScreen(string objectSelected, Popup_Controller popupController)
    {
        for (int i = 0; i < screenObjectList.Count; i++)
        {
            if (screenObjectList[i].name.Equals(objectSelected))
            {
                ScreenObject theScreen = screenObjectList[i].GetComponent<Screen_Script>().theScreen;

                if (theScreen.GetUpgradeLevel() < 4 && !theScreen.ConstructionInProgress())
                {

                    ConfirmationScript.OptionSelected(3, new string[] { "upgrade Screen " + theScreen.GetScreenNumber(), (theScreen.CalculateUpgradeCost()).ToString(), "0", i.ToString() }, "This will cost: ");

                    break;
                }
                else if (theScreen.ConstructionInProgress())
                {
                    popupController.ShowPopup(3, "Construction on this Screen is already in progress!");
                }
                else
                {
                    popupController.ShowPopup(3, "This Screen is already fully upgraded!");
                }
            }
        }
    }

    public void ReShowObjects()
    {
        for (int i = 0; i < screenObjectList.Count; i++)
        {
            screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        }

        for (int i = 0; i < gameObjectList.Count; i++)
        {
            gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        }
        redCarpet.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void SellScreen(string objectSelected)
    {

        int foundPos = -1;

        for (int i = 0; i < screenObjectList.Count; i++)
        {
            if (objectSelected.Equals(screenObjectList[i].name))
            {
                DestroyBuilderByScreenID(theScreens[i].GetScreenNumber());
                screenObjectList.RemoveAt(i);
                theScreens.RemoveAt(i);
                foundPos = i;
                break;
            }
        }

        if (foundPos > -1)
        {
            for (int i = foundPos; i < theScreens.Count; i++)
            {
                theScreens[i].DecreaseScreenNumber();
                screenObjectList[i].name = "Screen#" + theScreens[i].GetScreenNumber();
                GameObject.Find("BuilderForScreen#" + (theScreens[i].GetScreenNumber() + 1)).name = "BuilderForScreen#" + theScreens[i].GetScreenNumber();
            }
        }
    }

    public void SellObject(string objectSelected)
    {
        int foundPos = -1;

        for (int i = 0; i < gameObjectList.Count; i++)
        {
            if (objectSelected.Equals(gameObjectList[i].name))
            {
                gameObjectList.RemoveAt(i);
                otherObjects.RemoveAt(i);
                foundPos = i;
            }
        }

        if (foundPos > -1)
        {
            for (int i = foundPos; i < otherObjects.Count; i++)
            {
                otherObjects[i].id -= 1;
                gameObjectList[i].name = "Element#" + otherObjects[i].id;
            }
        }
    }

    public GameObject AddScreen(ScreenObject theScreen, Vector3 pos, int height)
    {
        GameObject instance = (GameObject)Instantiate(screenPrefab.gameObject, pos, Quaternion.identity);
        instance.GetComponent<Screen_Script>().theScreen = theScreen;
        instance.name = "Screen#" + theScreen.GetScreenNumber();
        instance.tag = "Screen";
        instance.GetComponent<SpriteRenderer>().sortingOrder = height - theScreen.GetY() - 1;

        if (!theScreen.ConstructionInProgress())
        {
            instance.GetComponent<SpriteRenderer>().sprite = screenImages[theScreen.GetUpgradeLevel()];
        }
        else
        {
            instance.GetComponent<SpriteRenderer>().sprite = screenImages[0];
            CreateBuilder(theScreen.GetX(), theScreen.GetY(), theScreen.GetScreenNumber());
        }

        screenObjectList.Add(instance);

        return instance;

    }

    public DimensionTuple GetBounds(int itemToAddID)
    {
        int w = 0;
        int h = 0;

        #region Get Bounds
        if (itemToAddID == 2)
        {
            //xCorrection = 0.1f;
            //yCorrection = 0.35f;
            w = 1; h = 1;
        }
        else if (itemToAddID == 3)
        {
            w = 2; h = 2;
        }
        else if (itemToAddID == 5)
        {
            w = 3; h = 3;
        }
        else if (itemToAddID == 7)
        {
            w = 10; h = 18;
        }
        #endregion

        return new DimensionTuple(w, h);
    }

    public GameObject AddObject(Vector3 pos, int index, int height, int itemToAddID, bool isNew)
    {
        Transform newItem = null;
        
        string tag = null;
        
        #region Get Bounds
        if (itemToAddID == 2)
        {
            newItem = plantPrefab;
            tag = "Plant";
        }
        else if (itemToAddID == 3)
        {
            newItem = bustPrefab;
            tag = "Bust";
        }
        else if (itemToAddID == 5)
        {
            newItem = vendingMachinePrefab;
            tag = "Vending Machine";
        }
        else if (itemToAddID == 7)
        {
            newItem = foodAreaPrefab;
            tag = "Food Area";
        }
        #endregion

        // change pos and element here
        GameObject instance;
        if (isNew)
        {
            instance = (GameObject)Instantiate(newItem.gameObject, pos, Quaternion.identity);
        }
        else
        {
            instance = GameObject.Find("Element#" + index);
            instance.transform.position = pos;
        }
        instance.name = "Element#" + (index);
        instance.GetComponent<SpriteRenderer>().sortingOrder = height - (int)(pos.y/0.8f) - 1;
        instance.tag = tag;

        if (itemToAddID == 7)
        {

            Popup_Controller puc = GameObject.Find("PopupController").GetComponent<Popup_Controller>();

            FoodAreaScript[] scripts = instance.GetComponentsInChildren<FoodAreaScript>();
            foreach (FoodAreaScript script in scripts)
            {
                script.popupController = null;
            }
        }


        gameObjectList.Add(instance);
        return instance;

    }

    /// <summary>
    /// Remove the builder guy
    /// </summary>
    /// <param name="screenNum">Which screen the builder is associated with</param>
    public void DestroyBuilderByScreenID(int screenNum)
    {
        GameObject[] builders = GameObject.FindGameObjectsWithTag("Builder");

        for (int i = 0; i < builders.Length; i++)
        {
            if (builders[i].name.Contains(screenNum.ToString()))
            {
                Destroy(builders[i]);
            }
        }

    }
    
    /// <summary>
    /// Create a new builder
    /// </summary>
    /// <param name="x">The x position to place the builder in</param>
    /// <param name="y">The y position to place the builder in</param>
    /// <param name="screenNum">Which screen the builder is associated with</param>
    public void CreateBuilder(float x, float y, int screenNum)
    {
        GameObject builder = Instantiate(builderPrefab.gameObject, new Vector2(x + 1.8f, 0.8f * y + 0.82f), Quaternion.identity) as GameObject;
        builder.name = "BuilderForScreen#" + screenNum;
    }

    /// <summary>
    /// Make the objects semi-transparent (for when moving other objects)
    /// </summary>
    public void SemiTransparentObjects()
    {
        for (int i = 0; i < screenObjectList.Count; i++)
        {
            screenObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
        }
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            gameObjectList[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
        }

        redCarpet.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
    }

}
