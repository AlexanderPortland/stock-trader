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
    public int currentDay = 5;

    private TickerManager tickerManager;
    private TextMeshProUGUI dateText;

    public void Start(){
        symbols = InitializeSymbols(Directory.GetCurrentDirectory() + "/assets/scripts/data/");
        stocks = InitializeStocks(symbols);
        tickerManager = FindObjectOfType<TickerManager>();
        tickerManager.UpdateTextContent(StockSummary(stocks));
        dateText = GameObject.Find("DateText").GetComponent<TextMeshProUGUI>();
        UpdateDay(5);
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

    public string StockSummary(List<Stock> stocksToSummarize){
        int NUM_OF_SPACES = 10;
        string totalSummary = "";
        for (int i = 0; i < stocksToSummarize.Count(); i++){
            Stock stock = stocksToSummarize[i];
            float close = stock.TryGetCloseOnDay(currentDay);
            if (close > 0){
                string mySummary = new string(' ', NUM_OF_SPACES) + stock.symbol + " | $" + string.Format("{0:0.00}", close);
                totalSummary += mySummary;
            }
        }
        return "->" + totalSummary;
    }

    public bool ValidFileName(string fileName){
        if (!fileName.Contains(".csv")) return false;
        if (fileName.Contains(".meta")) return false;
        if (fileName.Contains("store")) return false;
        return true;
    }

    public void NextDay(){
        UpdateDay(currentDay + 1);
    }

    public void UpdateDay(int newDay){
        currentDay = newDay;
        dateText.text = stocks[0].days[newDay].DateToString();
        tickerManager.UpdateTextContent(StockSummary(stocks));
    }
}
