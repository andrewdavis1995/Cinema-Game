using Assets.Classes;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    //Transform[] gameObjectList;
    public ScreenObject[] theScreens;
    public float[] carpetColour;
    public SaveableStaff[] staffMembers;
    public FilmShowing[] filmShowings;
    public int totalCoins;
    public int numPopcorn;
    public int currentDay;
    public OtherObject[] otherObjects;
    public bool hasRedCarpet;
    public bool marbleFloor;
    public Reputation reputation;
    public int boxOfficeLevel;
    public FoodArea foodArea;
    public bool[] posters;

    public PlayerData(List<ScreenObject> screens, Color col, List<StaffMember> staff, List<FilmShowing> films, int coins, int day, int popcorn, List<OtherObject> others, bool redCarpet, bool marble, Reputation rep, int boxOffice, FoodArea fa, bool[] poster)
    {
        theScreens = screens.ToArray();
        carpetColour = new float[4] { col.r, col.g, col.b, col.a };

        List<SaveableStaff> staffList = new List<SaveableStaff>();

        for (int i = 0; i < staff.Count; i++)
        {
            SaveableStaff s = new SaveableStaff(staff[i]);
            staffList.Add(s);
        }

        staffMembers = staffList.ToArray();
        filmShowings = films.ToArray();
        totalCoins = coins;
        currentDay = day;
        numPopcorn = popcorn;
        otherObjects = others.ToArray();
        hasRedCarpet = redCarpet;
        marbleFloor = marble;
        reputation = rep;
        boxOfficeLevel = boxOffice;
        posters = poster;
    }

}
