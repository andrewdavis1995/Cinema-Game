using Assets.Classes;
using System;

[Serializable]
public class OtherObject
{
    public int xPos;
    public int yPos;
    public int type;
    public int id;

    public OtherObject(int x, int y, int t, int i)
    {
        xPos = x;
        yPos = y;
        type = t;
        id = i;
    }

    /// <summary>
    /// Get the cost of adding a new object
    /// </summary>
    /// <returns>The number of coins / popcorn to spend</returns>
    public static int GetCost(string objectSelected)
    {
        switch (objectSelected)
        {
            case "NEW SCREEN": return 1000;
            case "NEW PLANT": return 80;
            case "NEW BUST": return 7;
            case "NEW VENDING MACHINE": return 150;
            case "NEW FOOD AREA": return 7000;
            case "Screen": return 1000;
            case "Plant": return 80;
            case "Bust": return 7;
            case "Vending Machine": return 150;
            case "Food Area": return 7000;
            default: return 0;
        }
    }

    /// <summary>
    /// Calculate how many coins should be returned once an object is sold
    /// </summary>
    /// <returns>How many coins should be returned</returns>
    public static int GetReturnedCoins(string tagSelected, int upgradeLevelSelected, FoodArea fa)
    {
        int paidMoney = 0;

        paidMoney = GetCost(tagSelected);

        int coinsReturned = (int)(Math.Round(0.6f * (float)paidMoney, 0));

        #region Calculate upgrade costs
        int upgradeCosts = 0;

        if (tagSelected.Equals("Screen"))
        {
            switch (upgradeLevelSelected)
            {
                case 2: upgradeCosts = 180; break;
                case 3: upgradeCosts = 720; break;
                case 4: upgradeCosts = 2800; break;
            }
        }
        else if (tagSelected.Equals("Food Area"))
        {
            if (fa.hasHotFood)
            {
                upgradeCosts += 200;
            }
            if (fa.hasIceCream)
            {
                upgradeCosts += 900;
            }
            if (fa.hasPopcorn)
            {
                upgradeCosts += 1500;
            }
            if (fa.tableStatus == 2)
            {
                upgradeCosts += 450;
            }
        }

        coinsReturned += upgradeCosts;
        #endregion

        return coinsReturned;
    }

    /// <summary>
    /// Get which currency is used for the relevant object type
    /// </summary>
    /// <returns>0 for coins, 1 for popcorn</returns>
    public static String GetCurrency(string objectSelected)
    {
        // 0 = coins, 1 = popcorn
        switch (objectSelected)
        {
            case "NEW SCREEN": return "0";
            case "NEW PLANT": return "0";
            case "NEW BUST": return "1";
            case "NEW VENDING MACHINE": return "0";
            default: return "0";
        }
    }

    /// <summary>
    /// Get the width of an object
    /// </summary>
    /// <returns></returns>
    public static int GetWidthOfObject(string tagSelected)
    {
        // switch on the tag of the item that was selected
        switch (tagSelected)
        {
            // return the appropriate width based on the tag
            case "Screen":
                return 11;
            case "Vending Machine":
                return 3;
            case "Plant":
                return 1;
            case "Food Area":
                return 10;
            default: return 0;
        }
    }

    /// <summary>
    /// Get the height of the object
    /// </summary>
    /// <returns>The tag that was selected</returns>
    public static int GetHeightOfObject(string tagSelected)
    {
        // swich on the tag that was selected
        switch (tagSelected)
        {
            // return the approopriate height based on the tag
            case "Screen":
                return 15;
            case "Vending Machine":
                return 3;
            case "Plant":
                return 1;
            case "Food Area":
                return 18;
            default: return 0;
        }
    }
    
}