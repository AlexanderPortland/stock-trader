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
        float price = GetCurrentPrice();
        return symbol + " x" + buyQuantity + " at " 
                + FancifyMoneyText(buyPrice) + " | " + FancifyMoneyText(price);
    }

    public string FancifyMoneyText(float amount){
        return "$" + amount.ToString("#,##0:n");
    }

    public float GetCurrentPrice(){
        StockDataManager stockDataManager = FindObjectOfType<StockDataManager>();
        return stockDataManager.FindStock(symbol).TryGetCloseOnDay(stockDataManager.currentDay);
    }
}
