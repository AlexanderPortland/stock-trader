using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTrader : Trader
{

    public override void PlanTrades(){
        int r1 = Random.RandomRange(0, symbols.Length);
        symbolsToSell.Add(symbols[r1]);
        int r2 = Random.RandomRange(0, symbols.Length);
        symbolsToBuy.Add(symbols[r2]);
    }
}
