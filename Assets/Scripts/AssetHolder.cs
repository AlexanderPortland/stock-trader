using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetHolder : MonoBehaviour
{
    public List<Holding> holdings;
    
    public static float cash = 10000;

    public string FancifyMoneyText(float amount){
        string[] a = amount.ToString().Split(".");
        return "$" + amount.ToString("#,##0");
    }

    public string GetCashString(){
        return FancifyMoneyText(cash);
    }

    public void AddHolding(Holding newHolding){
        holdings.Add(newHolding);
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
        for(int i = 0; i < holdings.Count; i++){
            if (numToRemove <= 0) return;
            Holding h = holdings[i];
            if (h.symbol == symbol){
                if (numToRemove > h.buyQuantity){
                    numToRemove -= h.buyQuantity;
                    holdings.Remove(h);
                } else if (numToRemove < h.buyQuantity){
                    h.buyQuantity -= numToRemove;
                    numToRemove = 0;
                } else {
                    holdings.Remove(h);
                    numToRemove = 0;
                }
                RemoveHoldingsOfSymbol(symbol, numToRemove);
                return;
            }
        }
    }
}
