using UnityEngine;
using System.Collections;

public class ShopScript : MonoBehaviour {

    Controller mainController;

    void Start()
    {
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
    }


    public void Purchase(int id)
    {
        switch (id)
        {
            case 0: //screen
                mainController.objectSelected = "NEW SCREEN";
                mainController.placeObject(10, 10);
                break;
        }
    }

}


/*

IDs: 

    0   -   Screen
    1   -   Staff Member
    2   -   5 x Popcorn

*/