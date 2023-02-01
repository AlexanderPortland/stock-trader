using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearRegressionTrader : Trader
{
    int REGRESSION_DAYS_BACK = 260;
    public int[] valuations;
    public float STANDARD_DEVIATION_MULTIPLIER = 1;

    public override void Initialize(){
        
        base.Initialize();
        Debug.Log("start");
        valuations = new int[stockDataManager.symbols.Length];
    }

    public override void PlanTrades(){
        for(int i = 0; i < symbols.Length; i++){
            string symbol = symbols[i];
            Stock s = stockDataManager.FindStock(symbol);
            int today = stockDataManager.currentDay;
            int lastDayCount = Mathf.Min(s.days.Length - 1, today + REGRESSION_DAYS_BACK);
            
            List<Vector2> pointsToFit = new List<Vector2>();
            for(int j = today; j < lastDayCount; j++){
                float close = s.days[j].close;
                pointsToFit.Add(new Vector2(j - today, close));
            }

            LinearFunction fit = Calculator.FindLineOfBestFit(pointsToFit);
            float deviation = Calculator.StandardDeviation(fit, pointsToFit);

            float currrentPrice = s.TryGetCloseOnDay(today);

            if(currrentPrice > 0){
                //Debug.Log(symbol + "deviation " + deviation);
                float expectedPrice = fit.f(0);
                float topBound = expectedPrice + deviation;
                float bottomBound = expectedPrice - deviation;
                if (currrentPrice < bottomBound){
                    Debug.Log(symbol + " undervalued: " + currrentPrice + " which is below " + bottomBound);
                    valuations[i] = -1;
                } else if (currrentPrice > topBound){
                    Debug.Log(symbol + " overvalued");
                    valuations[i] = 1;
                } else {
                    //Debug.Log(symbol + " rightly valued");
                    if (valuations[i] == -1) symbolsToBuy.Add(symbol);
                    else if(valuations[i] == 1) symbolsToSell.Add(symbol);
                    valuations[i] = 0;
                }
            }
        }
    }
}
