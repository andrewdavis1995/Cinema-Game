using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Classes;

public class Customer_Controller : MonoBehaviour
{
    #region Paths
    List<List<Coordinate>> ticketToScreen = new List<List<Coordinate>>();
    List<Coordinate> ticketToFood = new List<Coordinate>();
    List<List<Coordinate>> foodToScreen = new List<List<Coordinate>>();
    List<Coordinate> exitPath = new List<Coordinate>();
    #endregion

    public Reputation reputation;

    public int numWalkouts = 0;
    public int customersServed = 0;
    public int customerMoney = 0;

    public List<Customer> allCustomers = new List<Customer>();


    /// <summary>
    /// Get a path to a screen
    /// </summary>
    /// <param name="index">Which screen to get a path to</param>
    /// <returns>The path to the screen</returns>
    public List<Coordinate> GetScreenPath(int index)
    {
        return this.ticketToScreen[index];
    }

    /// <summary>
    /// Get a path from the food area to a screen
    /// </summary>
    /// <param name="index">Which screen to get a path to</param>
    /// <returns></returns>
    public List<Coordinate> GetFoodToScreenPath(int index)
    {
        return foodToScreen[index];
    }

    /// <summary>
    /// Get a path to the food area
    /// </summary>
    /// <returns></returns>
    public List<Coordinate> GetPathToFood()
    {
        return this.ticketToFood;
    }

    /// <summary>
    /// Rest the path lists
    /// </summary>
    public void ResetPaths()
    {
        ticketToScreen.Clear();
        ticketToFood.Clear();
        foodToScreen.Clear();
    }

    public void ResetCounts()
    {
        numWalkouts = 0;
        customersServed = 0;
        customerMoney = 0;
    }

    #region Set Paths
    public void AddScreenPath(List<Coordinate> path)
    {
        ticketToScreen.Add(path);
    }
    public void AddFoodToScreenPath(List<Coordinate> path)
    {
        foodToScreen.Add(path);
    }
    public void SetTicketsToFoodPath(List<Coordinate> path)
    {
        ticketToFood = path;
    }
    #endregion
}
