using UnityEngine;
using System.Collections;
using The_Betting_App;
using System.Collections.Generic;

public class Runner : MonoBehaviour {

    Form1 form = new Form1();
    List<Fixture> fixtures = new List<Fixture>();

	// Use this for initialization
	void Start () {
        fixtures = form.Calculate();

        for (int i = 0; i < fixtures.Count; i++)
        {
            Debug.Log(fixtures[i].getHomeTeam().getName() + " vs " + fixtures[i].getAwayTeam().getName());
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
