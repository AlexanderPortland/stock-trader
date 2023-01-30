using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetHolder : MonoBehaviour
{
    public List<Holding> holdings;
    
    public static float cash = 10000;

    public string FancifyMoneyText(float amount){
        //string[] a = amount.ToString().Split(".");
        return "$" + amount.ToString("#,##0");
    }

    public string GetCashString(){
        return FancifyMoneyText(cash);
    }

    public void AddHolding(Holding newHolding){
        holdings.Add(newHolding);
        TryCombineHoldings();
    }

    public float QuantityOfSymbol(string symbol){
        float count = 0;
        foreach(Holding h in holdings){
            if (h.symbol == symbol) count += h.buyQuantity;
        }
        return count;
    }

    public void RemoveHoldingsOfSymbol(string symbol, float count){
        float numToRemove = count;
        for(int i = holdings.Count - 1; i >= 0; i--){
            if (numToRemove <= 0) return;
            Holding h = holdings[i];
            if (h.symbol == symbol){
                if (numToRemove > h.buyQuantity){
                    Debug.Log("a" + i);
                    numToRemove -= h.buyQuantity;
                    holdings.RemoveAt(i);
                    RemoveHoldingsOfSymbol(symbol, numToRemove);
                    return;
                } else if (numToRemove < h.buyQuantity){
                    Debug.Log("b" + i);
                    h.buyQuantity -= numToRemove;
                    return;
                } else {
                    Debug.Log("c" + i);
                    holdings.RemoveAt(i);
                    return;
                }
            }
        }
    }

    public void TryCombineHoldings(){
        for(int i = 0; i < holdings.Count; i ++){
            for(int j = 0; j < holdings.Count; j ++){
                if (i != j){
                    Holding a = holdings[i];
                    Holding b = holdings[j];
                    if (a.symbol == b.symbol && a.buyPrice == b.buyPrice){
                        a.buyQuantity += b.buyQuantity;
                        holdings.RemoveAt(j);
                        TryCombineHoldings();
                    }
                }
            }
        }
    }

    public float[] HoldingsValueBySymbol(){
        StockDataManager stockDataManager = FindObjectOfType<StockDataManager>();
        float[] values = new float[stockDataManager.symbols.Length];

        for(int i = 0; i < stockDataManager.symbols.Length; i++){
            string symbol = stockDataManager.symbols[i];
            float symbolVal = 0;
            for(int j = 0; j < holdings.Count; j++){
                if (holdings[j].symbol == symbol) symbolVal += holdings[j].GetCurrentValue();
            }
            values[i] = symbolVal;
        }
        return values;
    }
}
