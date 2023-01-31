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

    float NEUTRAL_FLOW_THRESHOLD = 0.2f;
    string POSITIVE_FLOW_ICON = "▲";
    string NEUTRAL_FLOW_ICON = " •"; // or ~
    string NEGATIVE_FLOW_ICON = "▼";

    public void Start(){
        symbols = InitializeSymbols(Directory.GetCurrentDirectory() + "/assets/scripts/data/");
        stocks = InitializeStocks(symbols);
        uIManager = FindObjectOfType<UIManager>();
        assetHolder = FindObjectOfType<AssetHolder>();
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
                string flow = POSITIVE_FLOW_ICON;
                float dayChange = close - stock.days[currentDay].open;
                if (Mathf.Abs(dayChange) < NEUTRAL_FLOW_THRESHOLD) flow = NEUTRAL_FLOW_ICON; //or ~
                else if (dayChange < 0) flow = NEGATIVE_FLOW_ICON;
                string mySummary = new string(' ', NUM_OF_SPACES) + stock.symbol + " | $" + string.Format("{0:0.00}", close) + flow;
                totalSummary += mySummary;
            }
        }
        return new string(' ', NUM_OF_SPACES) + "***" + totalSummary;
    }

    public Stock FindStock(string symbol){
        foreach(Stock stock in stocks){
            if(stock.symbol == symbol) return stock;
        }
        return null;
    }

    public bool ValidFileName(string fileName){
        if (!fileName.Contains(".csv")) return false;
        if (fileName.Contains(".meta")) return false;
        if (fileName.Contains("store")) return false;
        return true;
    }

    public void NextDay(){
        UpdateDay(currentDay - 1);
    }

    public void UpdateDay(int newDay){
        currentDay = newDay;
        uIManager.UpdateDayUI(newDay);
        uIManager.UpdateAssetsUI();
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
