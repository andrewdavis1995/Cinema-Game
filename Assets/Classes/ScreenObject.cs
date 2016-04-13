using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class ScreenObject
{
    int screenNumber;
    int capacity;
    int upgradeLevel = 0;
    bool constructionInProgress = false;
    int constructionTimeRemaining = 0;
    int projectorClicksRemaining = 0;

    int pointX = 0;
    int pointY = 0;

    public void DecreaseScreenNumber()
    {
        screenNumber--;
    }

    public void SetPosition(int x, int y)
    {
        pointX = x;
        pointY = y;
    }

    public int GetX() { return this.pointX; }
    public int GetY() { return this.pointY; }

    public ScreenObject(int screenID, int numSeats)
    {
        screenNumber = screenID;
        capacity = numSeats;
    }


    public int GetScreenNumber() { return screenNumber; }
    public int GetNumSeats() { return capacity; }
            
    public void Upgrade()
    {
        constructionInProgress = true;
        upgradeLevel++;

        CalculateUpgradeCost();

        if (constructionTimeRemaining < 1)
        {
            UpgradeComplete();
            // NYAH
        }

        capacity = GetNewCapacity();
    }

    public void ProjectorBroke()
    {
        projectorClicksRemaining = 20 * upgradeLevel;
    }

    public void ResetClicks()
    {
        projectorClicksRemaining = 0;
    }

    public int ProjectorClicked()
    {
        if (projectorClicksRemaining > 0)
        {
            projectorClicksRemaining--;
        }

        return projectorClicksRemaining;
    }

    public int GetClicksRemaining()
    {
        return this.projectorClicksRemaining;
    }

    public int GetDaysOfConstruction()
    {
        return this.constructionTimeRemaining;
    }

    public void ProgressOneDay()
    {
        if (constructionTimeRemaining > 1)
        {
            constructionTimeRemaining--;
        }
        else if (constructionTimeRemaining == 1)
        {
            UpgradeComplete();
        }
    }

    public int CalculateUpgradeCost()
    {
        if (upgradeLevel == 1)
        {
            constructionTimeRemaining = 3;
            return 450;
        }
        else if (upgradeLevel == 2)
        {
            constructionTimeRemaining = 2;
            return 1800;
        }
        else if (upgradeLevel == 3)
        {
            constructionTimeRemaining = 4;
            return 7000;
        }
        else
        {
            constructionTimeRemaining = 7;
            return 20000;
        }
    }        

    private int GetNewCapacity()
    {
        if (upgradeLevel == 1)
        {
            return 18;
        }
        else if (upgradeLevel == 2)
        {
            return 38;
        }
        else if (upgradeLevel == 3)
        {
            return 54;
        }
        else
        {
            return 70;
        }
    }
    public int GetUpgradeLevel(){ return this.upgradeLevel; }

    public void UpgradeComplete()
    {
        // upgrade complete
        this.constructionInProgress = false;
        this.constructionTimeRemaining = 0;
    }

    public bool ConstructionInProgress()
    {
        return constructionInProgress;
    }
}

