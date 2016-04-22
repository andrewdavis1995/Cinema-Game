using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class ScreenObject
{
    int screenNumber;   // screen number
    int capacity;       // maximum number of tickets that can be sold per screenings in this Screen
    int upgradeLevel = 0;   // the upgrade llevel of the screen

    bool constructionInProgress = false;    // whether or not construction in taking place
    int constructionTimeRemaining = 0;      // how many days are left in the construction process

    int projectorClicksRemaining = 0;       // how many times the projector needs to be clicked in order to be fixed

    // position variables
    int pointX = 0;
    int pointY = 0;

    // CONSTRUCTOR for the class
    public ScreenObject(int screenID, int numSeats)
    {
        screenNumber = screenID;
        capacity = numSeats;
    }

    #region Accessors
    public int GetX() { return this.pointX; }
    public int GetY() { return this.pointY; }
    public int GetScreenNumber() { return screenNumber; }
    public int GetNumSeats() { return capacity; }
    public int GetClicksRemaining()
    {
        return this.projectorClicksRemaining;
    }
    public int GetDaysOfConstruction()
    {
        return this.constructionTimeRemaining;
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
    public int GetUpgradeLevel() { return this.upgradeLevel; }
    #endregion

    /// <summary>
    /// Decreases the Screen number by 1 - after a screen has been removed
    /// </summary>
    public void DecreaseScreenNumber()
    {
        screenNumber--;
    }

    /// <summary>
    /// Set the position of the object
    /// </summary>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    public void SetPosition(int x, int y)
    {
        pointX = x;
        pointY = y;
    }

    /// <summary>
    /// Upgrade the screen: Adds more seats, make bigger screen
    /// </summary>
    public void Upgrade()
    {
        // set construction times and status variables
        constructionInProgress = true;
        upgradeLevel++;

        CalculateUpgradeCost();

        // if the construction is already complete (default screen on new game is), then complete the upgrade
        if (constructionTimeRemaining < 1)
        {
            UpgradeComplete();
        }

        capacity = GetNewCapacity();
    }

    /// <summary>
    /// If the projector has broken
    /// </summary>
    public void ProjectorBroke()
    {
        // add to the number of clicks that are required to fix it
        projectorClicksRemaining = 20 * (int)Math.Round(0.5 * upgradeLevel, 0);
    }

    /// <summary>
    /// Reset the clicks required for the projector to fix
    /// </summary>
    public void ResetClicks()
    {
        projectorClicksRemaining = 0;
    }

    /// <summary>
    /// If the projector has been clicked, updates the number of clicks 
    /// </summary>
    /// <returns>How many clicks remain</returns>
    public int ProjectorClicked()
    {
        // if there are still clicks required, update the count
        if (projectorClicksRemaining > 0)
        {
            projectorClicksRemaining--;
        }

        return projectorClicksRemaining;
    }

    /// <summary>
    /// Move forward one day - updates constructions
    /// </summary>
    public void ProgressOneDay()
    {
        // if there are more than 1 days left, take one of the count
        if (constructionTimeRemaining > 1)
        {
            constructionTimeRemaining--;
        }
        // if this is the last day, finish the upgrade
        else if (constructionTimeRemaining == 1)
        {
            UpgradeComplete();
        }
    }

    /// <summary>
    /// Calculates the cost of an upgrade based on the Screen upgrade level
    /// </summary>
    /// <returns>The number of coins that the upgrade will cost</returns>
    public int CalculateUpgradeCost()
    {
        // based on the upgrade level, return a suitable cost, as well as updating how many days the upgrade will take
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
    
    /// <summary>
    /// The upgrade / construction has finished
    /// </summary>
    public void UpgradeComplete()
    {
        // update the status variables
        this.constructionInProgress = false;
        this.constructionTimeRemaining = 0;
    }

    /// <summary>
    /// Check if construction on the screen is in progress
    /// </summary>
    /// <returns>Whether or not there is construction in progress</returns>
    public bool ConstructionInProgress()
    {
        return constructionInProgress;
    }
}

