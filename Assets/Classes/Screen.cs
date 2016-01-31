using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Screen
{
    int screenNumber;
    int capacity;
    int upgradeLevel = 0;
    bool constructionInProgress = false;
    int constructionTimeRemaining = 0;


    public Screen(int screenID, int numSeats)
    {
        screenNumber = screenID;
        capacity = numSeats;
    }
        

    public int getNumSeats() { return capacity; }

        
    public void upgrade()
    {
        constructionInProgress = true;
        upgradeLevel++;
            
        int cost = calculateUpgradeCost();
            

        if (constructionTimeRemaining < 1)
        {
            upgradeComplete();
        }

        capacity = getNewCapacity();
    }

    public void progressOneDay()
    {
        if (constructionTimeRemaining > 1)
        {
            constructionTimeRemaining--;
        }
        else if (constructionTimeRemaining == 1)
        {
            constructionTimeRemaining = 0;
        }
    }

    private int calculateUpgradeCost()
    {
        if (upgradeLevel == 1)
        {
            constructionTimeRemaining = 3;
            return 350;
        }
        else if (upgradeLevel == 2)
        {
            constructionTimeRemaining = 2;
            return 450;
        }
        else if (upgradeLevel == 3)
        {
            constructionTimeRemaining = 3;
            return 1500;
        }
        else
        {
            constructionTimeRemaining = 6;
            return 20000;
        }
    }
        

    private int getNewCapacity()
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
    public int getUpgradeLevel(){ return this.upgradeLevel; }

    public void upgradeComplete()
    {
        // upgrade complete
        this.constructionInProgress = false;
        this.constructionTimeRemaining = 0;
    }
}

