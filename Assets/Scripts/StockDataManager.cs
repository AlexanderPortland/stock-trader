using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class StockDataManager : MonoBehaviour
{
    public string[] symbols;
    public List<Stock> stocks;

    public void Start(){
        symbols = InitializeSymbols(Directory.GetCurrentDirectory() + "/assets/scripts/data/");
        stocks = InitializeStocks(symbols);
    }

    //parses data from data files and for accessability
    public List<Stock> InitializeStocks(string[] symbolsToInitialize){
        List<Stock> los = new List<Stock>();
        foreach(string symbol in symbolsToInitialize){
            Stock newStock = new Stock(symbol);
            los.Add(newStock);
        }
        return los;
    }

    public string[] InitializeSymbols(string directoryPath){
        string[] files = Directory.GetFiles(directoryPath);
        List<string> names = new List<string>();
        for(int i = 0; i < files.Length; i++){
            string s = Path.GetFileName(files[i]);
            if (ValidFileName(s)){
                Debug.Log(files[i]);
                names.Add(s.Substring(0, s.IndexOf('.')));
            }
        }
        return names.ToArray();
    }

    public bool ValidFileName(string fileName){
        if (!fileName.Contains(".csv")) return false;
        if (fileName.Contains(".meta")) return false;
        if (fileName.Contains("store")) return false;
        return true;
    }
}
