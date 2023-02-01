using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trader : MonoBehaviour
{
    public StockDataManager stockDataManager;
    public AssetHolder myHolder;

    public string[] symbols;
    [SerializeField]
    public List<string> symbolsToBuy;
    [SerializeField]
    public List<string> symbolsToSell;

    public virtual void Initialize(){
        Debug.Log("init");
        symbolsToBuy = new List<string>();
        symbolsToSell = new List<string>();
        //myHolder = new AssetHolder(10000);
        myHolder = FindObjectOfType<AssetHolder>();

        stockDataManager = FindObjectOfType<StockDataManager>();
        symbols = stockDataManager.symbols;
    }

    public abstract void PlanTrades();

    public void MakeTrades(){
        symbolsToBuy.Clear();
        symbolsToSell.Clear();
        Debug.Log("making trades");

        PlanTrades();

        for(int i = 0; i < symbolsToBuy.Count; i++){
            myHolder.Buy(symbolsToBuy[i], 10);
        }

        for(int i = 0; i < symbolsToSell.Count; i++){
            myHolder.Sell(symbolsToSell[i], 10);
        }
    }
}
