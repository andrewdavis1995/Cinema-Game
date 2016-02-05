using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadGames()
    {
        SaveLoadScript.Load();
        List<Controller> gameList = SaveLoadScript.savedGames;
    }
}
