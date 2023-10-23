using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinSystem : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] int startCoins;
    private int coins;
    private float income = 0;
    public float rate;
    public static CoinSystem instance;
    
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        coins = startCoins;
        rate = 1;
        //StartCoroutine(IncreaseCoins());
    }

    //float timeElapse = 0;
    // Update is called once per frame
    void Update()
    {
        //when rate is 1 its about 1 coin per second
        income += rate * Time.deltaTime;
        //timeElapse += Time.deltaTime;
        if(income>1)
        {
            coins++;
            income--;
            //Debug.Log(timeElapse);
            //timeElapse= 0;
        }
        text.text = "Coins: " + coins;
    }

    IEnumerator IncreaseCoins()
    {
        Increment();
        yield return new WaitForSeconds(rate);
        StartCoroutine(IncreaseCoins());
    }
    void Increment()
    {
        coins ++;
    }

    //Enemy Value
    public void DeathCoins(int value)
    {
        coins += value;
    }


    //Tower Methods 
    public bool CheckCost(int amount)
    {
        if (coins >= amount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void PurchaseCoins(int amount)
    {
        coins -= amount;
    }
}
