using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Finance_Controller : MonoBehaviour
{

    int totalCoins = 450;
    int numPopcorn = 4;

    public Text coinLabel;
    public Text popcornLabel;

    public void Inititalise(int numCoins, int numPop)
    {
        totalCoins = numCoins;
        numPopcorn = numPop;
        coinLabel.text = totalCoins.ToString();
        popcornLabel.text = numPopcorn.ToString();
    }

    public void AddCoins(int num)
    {
        totalCoins += num;
        coinLabel.text = totalCoins.ToString();
    }
    public void RemoveCoins(int num)
    {
        totalCoins -= num;
        coinLabel.text = totalCoins.ToString();
    }
    public int GetNumCoins() { return totalCoins; }
    
    public void AddPopcorn(int num)
    {
        numPopcorn += num;
        popcornLabel.text = numPopcorn.ToString();
    }
    public void RemovePopcorn(int num)
    {
        numPopcorn -= num;
        popcornLabel.text = numPopcorn.ToString();
    }
    public int GetNumPopcorn() { return numPopcorn; }


}
