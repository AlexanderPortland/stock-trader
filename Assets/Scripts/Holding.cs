using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holding : MonoBehaviour
{
    public string symbol;
    public float buyPrice;
    public float buyQuantity;

    public Holding(string _symbol, float _quantity, float _price){
        symbol = _symbol;
        buyPrice = _price;
        buyQuantity = _quantity;
    }

    public override string ToString() {
        StockDataManager stockDataManager = GetComponent<StockDataManager>();
        float price = stockDataManager.FindStock(symbol).TryGetCloseOnDay(stockDataManager.currentDay);
        return symbol + " x" + buyQuantity + " at " 
                + FancifyMoneyText(buyPrice) + " | " + FancifyMoneyText(price);
    }

    public string FancifyMoneyText(float amount){
        return "$" + amount.ToString("#,##0");
    }
}
