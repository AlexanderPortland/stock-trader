using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetHolder : MonoBehaviour
{
    public List<Holding> holdings;
    
    public float cash = 10000;

    public void RemoveEmptyHoldings(){
        foreach(Holding h in holdings){
            if (h.buyQuantity == 0){
                holdings.Remove(h);
            }
        }
    }

    public string FancifyMoneyText(float amount){
        string[] a = amount.ToString().Split(".");
        return "$" + amount.ToString("#,##0");
    }

    public string GetCashString(){
        return FancifyMoneyText(cash);
    }
}
