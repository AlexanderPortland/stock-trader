using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;

public class StockDataManager : MonoBehaviour
{
    public string[] symbols;
    public List<Stock> stocks;
    public int currentDay = 500;
    public UIManager uIManager;
    public AssetHolder assetHolder;

    public void Start(){
        symbols = InitializeSymbols(Directory.GetCurrentDirectory() + "/assets/scripts/data/");
        stocks = InitializeStocks(symbols);
        uIManager = GetComponent<UIManager>();
        assetHolder = GetComponent<AssetHolder>();
        InitializeUI();
    }

    //parses data from data files and for accessability
    public List<Stock> InitializeStocks(string[] symbolsToInitialize){
        Debug.Log("intializing stocks");
        List<Stock> los = new List<Stock>();
        foreach(string symbol in symbolsToInitialize){
            Stock newStock = new Stock(symbol);
            los.Add(newStock);
        }
        return los;
    }

    public string[] InitializeSymbols(string directoryPath){
        Debug.Log("intializing symbols");
        string[] files = Directory.GetFiles(directoryPath);
        List<string> names = new List<string>();
        for(int i = 0; i < files.Length; i++){
            string s = Path.GetFileName(files[i]);
            if (ValidFileName(s)){
                names.Add(s.Substring(0, s.IndexOf('.')));
            }
        }
        return names.ToArray();
    }

    public void InitializeUI(){
        //uIManager.Start();
        uIManager.InitializeUI();
        UpdateDay(2000);
    }

    public string GetStockSummary(){
        int NUM_OF_SPACES = 10;
        string totalSummary = "";
        for (int i = 0; i < stocks.Count(); i++){
            Stock stock = stocks[i];
            float close = stock.TryGetCloseOnDay(currentDay);
            if (close > 0){
                string mySummary = new string(' ', NUM_OF_SPACES) + stock.symbol + " | $" + string.Format("{0:0.00}", close);
                totalSummary += mySummary;
            }
        }
        return "." + totalSummary;
    }

    public Stock FindStock(string symbol){
        foreach(Stock stock in stocks){
            if(stock.symbol == symbol) return stock;
        }
        return null;
    }

    public void Buy(string symbol, float quantity){
        float price = FindStock(symbol).TryGetCloseOnDay(currentDay);
        float totalPrice = price * quantity;
        Debug.Log(symbol + ", " + quantity + ", " + price);
        if (totalPrice < 0) {
            Debug.Log("buy cancelled: no price found");
            return;
        } else if (AssetHolder.cash < totalPrice){
            Debug.Log("buy cancelled: not enough money to cover the transaction");
            return;
        }
        AssetHolder.cash -= totalPrice;
        Holding h = new Holding(symbol, quantity, price);
        assetHolder.AddHolding(h);

        uIManager.UpdateAssetsUI();
    }

    public void Sell(string symbol, float quantity){
        if (assetHolder.QuantityOfSymbol(symbol) < quantity){
            Debug.Log("sell cancelled: not enough stock to cover the transaction");
            return;
        }
        float price = FindStock(symbol).TryGetCloseOnDay(currentDay);
        float totalPrice = price * quantity;
        Debug.Log(symbol + ", " + quantity + ", " + price);
        if (totalPrice < 0) {
            Debug.Log("sell cancelled: no price found");
            return;
        }
        AssetHolder.cash += totalPrice;
        assetHolder.RemoveHoldingsOfSymbol(symbol, quantity);

        uIManager.UpdateAssetsUI();
    }

    public bool ValidFileName(string fileName){
        if (!fileName.Contains(".csv")) return false;
        if (fileName.Contains(".meta")) return false;
        if (fileName.Contains("store")) return false;
        return true;
    }

    public void NextDay(){
        UpdateDay(currentDay + 10);
    }

    public void UpdateDay(int newDay){
        currentDay = newDay;
        uIManager.UpdateDayUI(newDay);
    }

    public string HoldingString(Holding h){
        float price = FindStock(h.symbol).TryGetCloseOnDay(currentDay);
        return h.symbol + " x" + h.buyQuantity + " at " 
                + FancifyMoneyText(h.buyPrice) + " | " + FancifyMoneyText(price);
    }

    public string FancifyMoneyText(float amount){
        return "$" + amount.ToString("#,##0");
    }
}
